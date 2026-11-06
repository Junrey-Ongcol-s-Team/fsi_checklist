-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (061) VI. DEFECTS/DEFICIENCIES
--   Kaniadto usa ra ka taas nga sulatanan, busa ang sulat sa ITEM I moagas ngadto sa ITEM II.
--   Karon: lima ka sulatanan (Item I hangtod Item V) ubos sa "Defects / deficiencies".
--   Sa print: ang matag Item mopuno sa iyang kaugalingon nga 3 ka linya ra (061.json defects.i1#1 ...).
--   Ang "Defects / deficiencies" nahimong ulohan na lang (diri gihapon ang Photos).
--   Ang daan nga tubag sa defects gibalhin sa Item I (apil ang COMPLETED, para dili mawala sa printout).
-- Modagan KAUSA RA (seed.061l).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-new-occupancy.defects.i1','fsic-new-occupancy','fsic-new-occupancy.s.findings','fsic-new-occupancy.defects','defects.i1','Item I',1,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.defects.i2','fsic-new-occupancy','fsic-new-occupancy.s.findings','fsic-new-occupancy.defects','defects.i2','Item II',2,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.defects.i3','fsic-new-occupancy','fsic-new-occupancy.s.findings','fsic-new-occupancy.defects','defects.i3','Item III',3,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.defects.i4','fsic-new-occupancy','fsic-new-occupancy.s.findings','fsic-new-occupancy.defects','defects.i4','Item IV',4,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.defects.i5','fsic-new-occupancy','fsic-new-occupancy.s.findings','fsic-new-occupancy.defects','defects.i5','Item V',5,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL);

-- Ang daan nga tubag: ibalhin sa Item I
DROP TRIGGER IF EXISTS trg_answer_lock_ins;

INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'fsic-new-occupancy.defects.i1', 'value', 0, a.value_text, NULL
  FROM inspection_answer a
 WHERE a.item_id = 'fsic-new-occupancy.defects' AND a.slot_key = 'value'
   AND a.value_text IS NOT NULL AND TRIM(a.value_text) <> '';

CREATE TRIGGER trg_answer_lock_ins BEFORE INSERT ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = NEW.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

-- "Defects / deficiencies": ulohan na lang (walay sulatanan), ang Photos naa gihapon
UPDATE form_item SET data_type = 'NONE' WHERE id = 'fsic-new-occupancy.defects';
