-- =====================================================================
-- BISAYA: "For Issuance of" = USA RA ang mapili (061 New FSIC ug 069 FSIC Renewal)
--   Kaniadto 7 ka checkbox (pwede daghan). Karon usa ka pangutana nga CHOICE (usa ra).
--   Ang value = ang daan nga key (pananglitan closure_order), busa ang print mo-check gihapon
--   sa daan nga marker "rec.issue.closure_order" (PrintService: CHOICE -> key.value = √) ug "rec.issue.any".
--   Ang daan nga tubag: ang UNA nga naka-check (sunod sa han-ay sa form) ang ibalhin. Ang daan nga checkbox gitago.
-- Modagan KAUSA RA (seed.issuance).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values)
SELECT t.tpl || '.rec.issue', t.tpl, t.tpl || '.s.findings', NULL, 'rec.issue', 'For issuance of (check one)',
       (SELECT MIN(sort_order) FROM form_item WHERE template_id = t.tpl AND key LIKE 'rec.issue.%'),
       'CHOICE', NULL, NULL, 0, NULL, NULL, NULL
  FROM (SELECT 'fsic-new-occupancy' AS tpl UNION ALL SELECT 'fsic-renewal') t;

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order)
SELECT i.template_id || '.rec.issue.' || SUBSTR(i.key, 11), i.template_id || '.rec.issue',
       SUBSTR(i.key, 11), REPLACE(i.label, 'For issuance of: ', ''), i.sort_order
  FROM form_item i
 WHERE i.template_id IN ('fsic-new-occupancy', 'fsic-renewal') AND i.key LIKE 'rec.issue.%' AND i.data_type = 'BOOL';

-- Ibalhin ang daan nga tubag (apil ang COMPLETED, para dili mausab ang printout)
DROP TRIGGER IF EXISTS trg_answer_lock_ins;

INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, i.template_id || '.rec.issue', 'value', 0, SUBSTR(i.key, 11), NULL
  FROM inspection_answer a
  JOIN form_item i ON i.id = a.item_id
 WHERE i.template_id IN ('fsic-new-occupancy', 'fsic-renewal') AND i.key LIKE 'rec.issue.%' AND i.data_type = 'BOOL'
   AND a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1)
   AND i.sort_order = (SELECT MIN(i2.sort_order) FROM inspection_answer a2 JOIN form_item i2 ON i2.id = a2.item_id
                        WHERE a2.inspection_id = a.inspection_id AND i2.template_id = i.template_id
                          AND i2.key LIKE 'rec.issue.%' AND i2.data_type = 'BOOL'
                          AND a2.slot_key = 'value' AND (a2.value_text = '1' OR a2.value_num = 1));

CREATE TRIGGER trg_answer_lock_ins BEFORE INSERT ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = NEW.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

-- Itago ang daan nga 7 ka checkbox ('-' = kanunay nakatago)
UPDATE form_item SET show_if_item_id = template_id || '.rec.issue', show_if_values = '-'
 WHERE template_id IN ('fsic-new-occupancy', 'fsic-renewal') AND key LIKE 'rec.issue.%' AND data_type = 'BOOL';
