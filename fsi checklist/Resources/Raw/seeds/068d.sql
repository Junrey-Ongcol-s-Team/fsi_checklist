-- =====================================================================
-- BISAYA: Inspection during construction (068) DEFECTS/DEFICIENCIES: tagsa-tagsa nga Item I hangtod Item VII (sama sa 061l).
--   Sa print: ang matag Item mopuno sa iyang kaugalingon nga 2 ka linya ra (defects.iN#1, #2).
--   Kung puno na ang una nga linya, mobalhin sa ikaduha; dili moagas sa sunod nga Item.
--   Ang "Defects / deficiencies" nahimong ulohan na lang (diri gihapon ang Photos).
--   Ang daan nga tubag sa defects gibalhin sa Item I (apil ang COMPLETED, para dili mawala).
-- Modagan KAUSA RA (seed.068d).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('during-construction.defects.i1','during-construction','during-construction.s.findings','during-construction.defects','defects.i1','Item I',1,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('during-construction.defects.i2','during-construction','during-construction.s.findings','during-construction.defects','defects.i2','Item II',2,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('during-construction.defects.i3','during-construction','during-construction.s.findings','during-construction.defects','defects.i3','Item III',3,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('during-construction.defects.i4','during-construction','during-construction.s.findings','during-construction.defects','defects.i4','Item IV',4,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('during-construction.defects.i5','during-construction','during-construction.s.findings','during-construction.defects','defects.i5','Item V',5,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('during-construction.defects.i6','during-construction','during-construction.s.findings','during-construction.defects','defects.i6','Item VI',6,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('during-construction.defects.i7','during-construction','during-construction.s.findings','during-construction.defects','defects.i7','Item VII',7,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL);

DROP TRIGGER IF EXISTS trg_answer_lock_ins;

INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'during-construction.defects.i1', 'value', 0, a.value_text, NULL
  FROM inspection_answer a
 WHERE a.item_id = 'during-construction.defects' AND a.slot_key = 'value'
   AND a.value_text IS NOT NULL AND TRIM(a.value_text) <> '';

CREATE TRIGGER trg_answer_lock_ins BEFORE INSERT ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = NEW.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

UPDATE form_item SET data_type = 'NONE', label = 'Defects / deficiencies (attach pictures, sketch and others)'
 WHERE id = 'during-construction.defects';
