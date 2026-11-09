-- =====================================================================
-- BISAYA: 024 Small Business: Verification -> NTC / NTCV / Abatement / Closure = USA RA ang mapili
--   Kaniadto 4 ka checkbox (pwede daghan). Karon usa ka CHOICE nga mogawas ra kung Verification.
--   Print: ang marker "nature.verification_for=NTC" (ug uban pa) sa print/024.json = parehas nga kahon
--   sa daan nga "nature.verification.ntc".
--   Daan nga tubag: ang UNA nga naka-check ang ibalhin; ang daan nga checkbox gitago ug ang tubag
--   niini tangtangon (aron usa ra ang √). LOCKED: dili hilabtan.
-- Modagan KAUSA RA (seed.024g). Kinahanglan human sa 024f.
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('small-business.nature.verification_for','small-business','small-business.s.nature','small-business.nature',
  'nature.verification_for','Verification inspection for compliance with (choose one)',8,'CHOICE',NULL,NULL,1,NULL,
  'small-business.nature','verification');

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('small-business.nature.verification_for.NTC',      'small-business.nature.verification_for','NTC',      'NTC',1),
 ('small-business.nature.verification_for.NTCV',     'small-business.nature.verification_for','NTCV',     'NTCV',2),
 ('small-business.nature.verification_for.ABATEMENT','small-business.nature.verification_for','ABATEMENT','Abatement',3),
 ('small-business.nature.verification_for.CLOSURE',  'small-business.nature.verification_for','CLOSURE',  'Closure',4);

-- Ibalhin ang daan nga tubag: ang UNA nga naka-check (dili locked)
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'small-business.nature.verification_for', 'value', 0, UPPER(SUBSTR(i.key, 21)), NULL
  FROM inspection_answer a
  JOIN form_item i ON i.id = a.item_id
 WHERE i.id IN ('small-business.nature.verification.ntc','small-business.nature.verification.ntcv',
                'small-business.nature.verification.abatement','small-business.nature.verification.closure')
   AND a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1)
   AND a.inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL)
   AND i.sort_order = (SELECT MIN(i2.sort_order)
                         FROM inspection_answer a2 JOIN form_item i2 ON i2.id = a2.item_id
                        WHERE a2.inspection_id = a.inspection_id
                          AND i2.id IN ('small-business.nature.verification.ntc','small-business.nature.verification.ntcv',
                                        'small-business.nature.verification.abatement','small-business.nature.verification.closure')
                          AND a2.slot_key = 'value' AND (a2.value_text = '1' OR a2.value_num = 1));

-- Tangtangon ang daan nga checkbox nga tubag (dili locked)
DELETE FROM inspection_answer
 WHERE item_id IN ('small-business.nature.verification.ntc','small-business.nature.verification.ntcv',
                   'small-business.nature.verification.abatement','small-business.nature.verification.closure')
   AND inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL);

-- Itago ang daan nga 4 ka checkbox
UPDATE form_item SET show_if_item_id = 'small-business.nature', show_if_values = '-'
 WHERE id IN ('small-business.nature.verification.ntc','small-business.nature.verification.ntcv',
              'small-business.nature.verification.abatement','small-business.nature.verification.closure');
