-- =====================================================================
-- BISAYA: 021 Dwelling ug 026 Gasoline Station: "Nature of inspection conducted"
--   = EXPANDER (FormPageViewModel ExpanderItems) ug USA RA ang mapili.
--   Kaniadto 7 ka checkbox (pwede daghan). Karon usa ka CHOICE.
--   Ang value = ang daan nga key (pananglitan periodic_inspection), busa ang print mo-check gihapon
--   sa daan nga marker "nature.periodic_inspection" (PrintService: CHOICE gamay nga letra -> key.value).
--   Others: ang "Others (specify)" mogawas ra kung Others ang gipili.
--   Daan nga tubag: ang UNA nga naka-check ang ibalhin; ang daan nga checkbox gitago ug ang tubag
--   niini tangtangon (usa ra ang check sa print). LOCKED: dili hilabtan.
-- Modagan KAUSA RA (seed.nature2).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values)
SELECT t.tpl || '.nature', t.tpl, t.tpl || '.s.nature', NULL, 'nature', 'Nature of inspection conducted', 1,
       'CHOICE', NULL, NULL, 0, 'Choose one', NULL, NULL
  FROM (SELECT 'dwelling' AS tpl UNION ALL SELECT 'gasoline-station') t;

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order)
SELECT i.template_id || '.nature.c.' || SUBSTR(i.key, 8), i.template_id || '.nature', SUBSTR(i.key, 8),
       CASE WHEN i.key = 'nature.others' THEN 'Others (specify)' ELSE i.label END, i.sort_order
  FROM form_item i
 WHERE i.template_id IN ('dwelling', 'gasoline-station') AND i.key LIKE 'nature.%' AND i.data_type = 'BOOL';

-- Others (specify): mogawas ra kung Others
UPDATE form_item
   SET parent_item_id = template_id || '.nature', show_if_item_id = template_id || '.nature',
       show_if_values = 'others', is_required = 1
 WHERE template_id IN ('dwelling', 'gasoline-station') AND key = 'nature.others_specify';

-- Ibalhin ang daan nga tubag: ang UNA nga naka-check (dili locked)
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, i.template_id || '.nature', 'value', 0, SUBSTR(i.key, 8), NULL
  FROM inspection_answer a
  JOIN form_item i ON i.id = a.item_id
 WHERE i.template_id IN ('dwelling', 'gasoline-station') AND i.key LIKE 'nature.%' AND i.data_type = 'BOOL'
   AND a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1)
   AND a.inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL)
   AND i.sort_order = (SELECT MIN(i2.sort_order)
                         FROM inspection_answer a2 JOIN form_item i2 ON i2.id = a2.item_id
                        WHERE a2.inspection_id = a.inspection_id AND i2.template_id = i.template_id
                          AND i2.key LIKE 'nature.%' AND i2.data_type = 'BOOL'
                          AND a2.slot_key = 'value' AND (a2.value_text = '1' OR a2.value_num = 1));

-- Tangtangon ang daan nga checkbox nga tubag (dili locked)
DELETE FROM inspection_answer
 WHERE item_id IN (SELECT id FROM form_item WHERE template_id IN ('dwelling', 'gasoline-station')
                      AND key LIKE 'nature.%' AND data_type = 'BOOL')
   AND inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL);

-- Itago ang daan nga checkbox (ubos sa bag-ong pilian, aron usa ra ang expander sa seksyon)
UPDATE form_item
   SET parent_item_id = template_id || '.nature', show_if_item_id = template_id || '.nature', show_if_values = '-'
 WHERE template_id IN ('dwelling', 'gasoline-station') AND key LIKE 'nature.%' AND data_type = 'BOOL';
