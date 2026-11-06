-- =====================================================================
-- BISAYA: Small business (024) DEFECTS/DEFICIENCIES: tagsa-tagsa nga Item IV hangtod Item VIII (sama sa 061l).
--   Sa print: ang matag Item mopuno sa iyang kaugalingon nga 2 ka linya ra (defects.iN#1, #2).
--   Kung puno na ang una nga linya, mobalhin sa ikaduha; dili moagas sa sunod nga Item.
--   Ang "Defects / deficiencies" nahimong ulohan na lang (diri gihapon ang Photos).
--   Ang daan nga tubag sa defects gibalhin sa Item IV (apil ang COMPLETED, para dili mawala).
-- Modagan KAUSA RA (seed.024b).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('small-business.defects.i4','small-business','small-business.s.findings','small-business.defects','defects.i4','Item IV',1,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.defects.i5','small-business','small-business.s.findings','small-business.defects','defects.i5','Item V',2,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.defects.i6','small-business','small-business.s.findings','small-business.defects','defects.i6','Item VI',3,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.defects.i7','small-business','small-business.s.findings','small-business.defects','defects.i7','Item VII',4,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.defects.i8','small-business','small-business.s.findings','small-business.defects','defects.i8','Item VIII',5,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL);

DROP TRIGGER IF EXISTS trg_answer_lock_ins;

INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'small-business.defects.i4', 'value', 0, a.value_text, NULL
  FROM inspection_answer a
 WHERE a.item_id = 'small-business.defects' AND a.slot_key = 'value'
   AND a.value_text IS NOT NULL AND TRIM(a.value_text) <> '';

CREATE TRIGGER trg_answer_lock_ins BEFORE INSERT ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = NEW.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

UPDATE form_item SET data_type = 'NONE', label = 'Defects / deficiencies (attach pictures, sketch and others)'
 WHERE id = 'small-business.defects';
