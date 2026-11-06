-- =====================================================================
-- BISAYA: FSIC renewal (069) 3. General Occupancy Specification
--   "Previous After Inspection Report: Type of occupancy" = PICKER (dropdown) nga naay 12 ka klase.
--   Kung "Special Structure": mogawas ang "Please specify" (kinahanglan sulatan).
--   Sa print: "Special Structure (gisulat)".
--   Ang daan nga gi-type: kung pareho sa usa sa listahan -> kana; kung lain -> Special Structure + gi-type.
--   (Ang Picker = PickerItems sa FormPageViewModel.cs)
-- Modagan KAUSA RA (seed.069g).
-- =====================================================================

UPDATE form_item SET data_type = 'CHOICE' WHERE id = 'fsic-renewal.a03.prev_occupancy';

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('fsic-renewal.a03.prev_occupancy.ASSEMBLY','fsic-renewal.a03.prev_occupancy','ASSEMBLY','Assembly',1),
 ('fsic-renewal.a03.prev_occupancy.EDUCATIONAL','fsic-renewal.a03.prev_occupancy','EDUCATIONAL','Educational',2),
 ('fsic-renewal.a03.prev_occupancy.DAY_CARE','fsic-renewal.a03.prev_occupancy','DAY_CARE','Day Care',3),
 ('fsic-renewal.a03.prev_occupancy.HEALTH_CARE','fsic-renewal.a03.prev_occupancy','HEALTH_CARE','Health Care',4),
 ('fsic-renewal.a03.prev_occupancy.DETENTION','fsic-renewal.a03.prev_occupancy','DETENTION','Detention and Correctional',5),
 ('fsic-renewal.a03.prev_occupancy.RESIDENTIAL','fsic-renewal.a03.prev_occupancy','RESIDENTIAL','Residential',6),
 ('fsic-renewal.a03.prev_occupancy.RESIDENTIAL_BOARD','fsic-renewal.a03.prev_occupancy','RESIDENTIAL_BOARD','Residential Board and Care',7),
 ('fsic-renewal.a03.prev_occupancy.MERCANTILE','fsic-renewal.a03.prev_occupancy','MERCANTILE','Mercantile',8),
 ('fsic-renewal.a03.prev_occupancy.BUSINESS','fsic-renewal.a03.prev_occupancy','BUSINESS','Business',9),
 ('fsic-renewal.a03.prev_occupancy.INDUSTRIAL','fsic-renewal.a03.prev_occupancy','INDUSTRIAL','Industrial',10),
 ('fsic-renewal.a03.prev_occupancy.STORAGE','fsic-renewal.a03.prev_occupancy','STORAGE','Storage',11),
 ('fsic-renewal.a03.prev_occupancy.SPECIAL','fsic-renewal.a03.prev_occupancy','SPECIAL','Special Structure',12);

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-renewal.a03.prev_occupancy.specify','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a03.prev_occupancy','a03.prev_occupancy.specify','Please specify',465,'TEXT',NULL,NULL,1,NULL,'fsic-renewal.a03.prev_occupancy','SPECIAL');

-- Ibalhin ang daan nga gi-type (apil ang COMPLETED)
DROP TRIGGER IF EXISTS trg_answer_lock_ins;
DROP TRIGGER IF EXISTS trg_answer_lock_upd;

-- lain nga sulat (dili sa listahan): ibutang sa "Please specify"
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'fsic-renewal.a03.prev_occupancy.specify', 'value', 0, TRIM(a.value_text), NULL
  FROM inspection_answer a
 WHERE a.item_id = 'fsic-renewal.a03.prev_occupancy' AND a.slot_key = 'value'
   AND TRIM(COALESCE(a.value_text, '')) <> ''
   AND NOT EXISTS (SELECT 1 FROM form_choice c WHERE c.item_id = a.item_id
                    AND (UPPER(c.label) = UPPER(TRIM(a.value_text)) OR c.value = UPPER(TRIM(a.value_text))));

UPDATE inspection_answer
   SET value_text = COALESCE((SELECT c.value FROM form_choice c WHERE c.item_id = inspection_answer.item_id
                               AND (UPPER(c.label) = UPPER(TRIM(inspection_answer.value_text)) OR c.value = UPPER(TRIM(inspection_answer.value_text)))),
                             'SPECIAL')
 WHERE item_id = 'fsic-renewal.a03.prev_occupancy' AND slot_key = 'value' AND TRIM(COALESCE(value_text, '')) <> '';

CREATE TRIGGER trg_answer_lock_ins BEFORE INSERT ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = NEW.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

CREATE TRIGGER trg_answer_lock_upd BEFORE UPDATE ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = OLD.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;
