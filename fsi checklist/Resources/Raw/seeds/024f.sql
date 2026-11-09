-- =====================================================================
-- BISAYA: 024 Small Business: II. Nature of inspection conducted = USA RA ang mapili
--   Kaniadto 8 ka checkbox (pwede daghan). Karon usa ka CHOICE (sulod sa expander II).
--   Ang value = ang daan nga key (pananglitan fsic_occupancy), busa ang print mo-√ gihapon
--   sa daan nga marker "nature.fsic_occupancy" (PrintService: CHOICE gamay nga letra -> key.value = √).
--   Verification: ang NTC / NTCV / Abatement / Closure mogawas ra kung Verification ang gipili.
--   Others: ang "Others (specify)" mogawas ra kung Others ang gipili.
--   Daan nga tubag: ang UNA nga naka-check (sunod sa han-ay) ang ibalhin; ang daan nga checkbox gitago
--   ug ang daan nga tubag niini tangtangon (aron dili doble ang √ sa print). LOCKED: dili hilabtan.
-- Modagan KAUSA RA (seed.024f). Kinahanglan human sa 024e (nature.grp) ug lock.sql.
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('small-business.nature','small-business','small-business.s.nature','small-business.nature.grp','nature','Nature of inspection conducted (choose one)',1,'CHOICE',NULL,NULL,0,NULL,NULL,NULL);

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('small-business.nature.c.during_construction',  'small-business.nature','during_construction',  'Inspection during construction',1),
 ('small-business.nature.c.fsic_occupancy',       'small-business.nature','fsic_occupancy',       'FSIC for Certificate for Occupancy',2),
 ('small-business.nature.c.fsic_business_new',    'small-business.nature','fsic_business_new',    'FSIC for Business Permit (New)',3),
 ('small-business.nature.c.fsic_business_renewal','small-business.nature','fsic_business_renewal','FSIC for Business Permit (Renewal)',4),
 ('small-business.nature.c.fsic_peza',            'small-business.nature','fsic_peza',            'FSIC for Certificate of Annual Inspection (PEZA)',5),
 ('small-business.nature.c.verification',         'small-business.nature','verification',         'Verification inspection for compliance',6),
 ('small-business.nature.c.notice_of_disapproval','small-business.nature','notice_of_disapproval','Notice of Disapproval, if Certificate of Occupancy',7),
 ('small-business.nature.c.others',               'small-business.nature','others',               'Others (specify)',8);

-- NTC / NTCV / Abatement / Closure: ubos sa pilian, mogawas ra kung Verification
UPDATE form_item SET parent_item_id = 'small-business.nature',
       show_if_item_id = 'small-business.nature', show_if_values = 'verification'
 WHERE id IN ('small-business.nature.verification.ntc','small-business.nature.verification.ntcv',
              'small-business.nature.verification.abatement','small-business.nature.verification.closure');

-- Others (specify): mogawas ra kung Others
UPDATE form_item SET parent_item_id = 'small-business.nature',
       show_if_item_id = 'small-business.nature', show_if_values = 'others', is_required = 1
 WHERE id = 'small-business.nature.others_specify';

-- Ibalhin ang daan nga tubag: ang UNA nga naka-check (dili locked)
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'small-business.nature', 'value', 0, SUBSTR(i.key, 8), NULL
  FROM inspection_answer a
  JOIN form_item i ON i.id = a.item_id
 WHERE i.template_id = 'small-business' AND i.parent_item_id = 'small-business.nature.grp' AND i.data_type = 'BOOL'
   AND a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1)
   AND a.inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL)
   AND i.sort_order = (SELECT MIN(i2.sort_order)
                         FROM inspection_answer a2 JOIN form_item i2 ON i2.id = a2.item_id
                        WHERE a2.inspection_id = a.inspection_id AND i2.parent_item_id = 'small-business.nature.grp'
                          AND i2.data_type = 'BOOL' AND a2.slot_key = 'value' AND (a2.value_text = '1' OR a2.value_num = 1));

-- Tangtangon ang daan nga checkbox nga tubag (dili locked), aron usa ra ang √ sa print
DELETE FROM inspection_answer
 WHERE item_id IN (SELECT id FROM form_item WHERE parent_item_id = 'small-business.nature.grp' AND data_type = 'BOOL')
   AND inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL);

-- Itago ang daan nga 8 ka checkbox
UPDATE form_item SET show_if_item_id = 'small-business.nature', show_if_values = '-'
 WHERE parent_item_id = 'small-business.nature.grp' AND data_type = 'BOOL';
