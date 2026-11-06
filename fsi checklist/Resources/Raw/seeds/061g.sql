-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (BFP-QSF-FSED-061). 3. REQUIREMENTS sama sa form:
--   [ ] FSIC for Occupancy:                         -> • FSCCR  Yes / No / N/A
--   [ ] FSIC for New Business Permit / Renewal / ...-> • FSMR   Yes / No / N/A
-- Usa ra ang mapili (ExpanderView), ug ang FSCCR o FSMR sa napili ra ang mogawas.
-- Kaniadto ang duha ka kahon wala sa app, mao nga wala ma-check sa printout.
-- Ang daan nga tubag: kung naay FSCCR = Occupancy; kung FSMR ra = New Business Permit;
-- kung duha: sunod sa "FSIC for" sa General information. Modagan KAUSA RA (seed.061g).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-new-occupancy.req.fsic_for','fsic-new-occupancy','fsic-new-occupancy.s.requirements',NULL,'req.fsic_for','3. Requirements (check one)',12,'CHOICE',NULL,NULL,0,NULL,NULL,NULL);

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('fsic-new-occupancy.req.fsic_for.OCCUPANCY','fsic-new-occupancy.req.fsic_for','OCCUPANCY','FSIC for Occupancy',1),
 ('fsic-new-occupancy.req.fsic_for.NEW_BP','fsic-new-occupancy.req.fsic_for','NEW_BP','FSIC for New Business Permit / Renewal / Certificate of Annual Inspection / Others',2);

UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.req.fsic_for', show_if_item_id = 'fsic-new-occupancy.req.fsic_for',
       show_if_values = 'OCCUPANCY', label = 'Fire Safety Compliance and Commissioning Report (FSCCR)'
 WHERE id = 'fsic-new-occupancy.req.fsccr';
UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.req.fsic_for', show_if_item_id = 'fsic-new-occupancy.req.fsic_for',
       show_if_values = 'NEW_BP', label = 'Fire Safety Maintenance Report (FSMR)'
 WHERE id = 'fsic-new-occupancy.req.fsmr';

-- Ibalhin ang daan nga tubag (apil COMPLETED)
DROP TRIGGER IF EXISTS trg_answer_lock_ins;

-- Duha ang natubag: sunod sa "FSIC for" sa General information
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT f.inspection_id, 'fsic-new-occupancy.req.fsic_for', 'value', 0, f.value_text, NULL
  FROM inspection_answer f
 WHERE f.item_id = 'fsic-new-occupancy.fsic_for' AND f.value_text IN ('OCCUPANCY','NEW_BP')
   AND EXISTS (SELECT 1 FROM inspection_answer a WHERE a.inspection_id = f.inspection_id AND a.item_id = 'fsic-new-occupancy.req.fsccr')
   AND EXISTS (SELECT 1 FROM inspection_answer a WHERE a.inspection_id = f.inspection_id AND a.item_id = 'fsic-new-occupancy.req.fsmr');

INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT DISTINCT a.inspection_id, 'fsic-new-occupancy.req.fsic_for', 'value', 0, 'OCCUPANCY', NULL
  FROM inspection_answer a WHERE a.item_id = 'fsic-new-occupancy.req.fsccr';

INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT DISTINCT a.inspection_id, 'fsic-new-occupancy.req.fsic_for', 'value', 0, 'NEW_BP', NULL
  FROM inspection_answer a WHERE a.item_id = 'fsic-new-occupancy.req.fsmr';

CREATE TRIGGER trg_answer_lock_ins BEFORE INSERT ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = NEW.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;
