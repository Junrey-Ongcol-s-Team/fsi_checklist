-- BISAYA: FSIC renewal (069): klaro nga ngalan sa duha ka column sa
-- "V. FIRE SAFETY MEASURES ASSESSMENT OF THE BUILDING/STRUCTURES/FACILITY":
--   "Previous After Inspection Report: ..." = wala nga column sa form
--   "Present/Actual Inspection: ..."         = tuo nga column sa form
-- (Version 2: gisulat og tibuok sama sa form aron dili makalibog.)
-- Ngalan (label) ra ang giusab; walay tubag nga mausab. Modagan KAUSA RA (seed.069d).
-- =====================================================================

UPDATE form_column SET label = 'Previous After Inspection Report: Compliant (Yes)' WHERE id = 'c-ccp-0';
UPDATE form_column SET label = 'Present/Actual Inspection: Compliant (Yes / No)' WHERE id = 'c-ccp-1';
UPDATE form_column SET label = 'Present/Actual Inspection: No changes / With changes' WHERE id = 'c-ccp-2';
UPDATE form_column SET label = 'Present/Actual Inspection: State particular violation / changes' WHERE id = 'c-ccp-3';

UPDATE form_item SET label = 'Previous After Inspection Report: Dated' WHERE id = 'fsic-renewal.prev_report_date';
UPDATE form_item SET label = 'Previous After Inspection Report: FSIC No. (latest)' WHERE id = 'fsic-renewal.a01.prev_fsic_no';
UPDATE form_item SET label = 'Previous After Inspection Report: FSIC dated' WHERE id = 'fsic-renewal.a01.prev_fsic_date';
UPDATE form_item SET label = 'Present/Actual Inspection: Renovation' WHERE id = 'fsic-renewal.a01.renovation';
UPDATE form_item SET label = 'Present/Actual Inspection: Repair' WHERE id = 'fsic-renewal.a01.repair';
UPDATE form_item SET label = 'Present/Actual Inspection: Additional' WHERE id = 'fsic-renewal.a01.additional';
UPDATE form_item SET label = 'Previous After Inspection Report: No. of floors' WHERE id = 'fsic-renewal.a02.prev_floors';
UPDATE form_item SET label = 'Present/Actual Inspection: Renovation' WHERE id = 'fsic-renewal.a02.renovation';
UPDATE form_item SET label = 'Present/Actual Inspection: Repair' WHERE id = 'fsic-renewal.a02.repair';
UPDATE form_item SET label = 'Present/Actual Inspection: Additional' WHERE id = 'fsic-renewal.a02.additional';
UPDATE form_item SET label = 'Present/Actual Inspection: State particular floor' WHERE id = 'fsic-renewal.a02.particular_floor';
UPDATE form_item SET label = 'Previous After Inspection Report: Type of occupancy' WHERE id = 'fsic-renewal.a03.prev_occupancy';
UPDATE form_item SET label = 'Present/Actual Inspection: Change of occupancy' WHERE id = 'fsic-renewal.a03.change_of_occupancy';
UPDATE form_item SET label = 'Present/Actual Inspection: Renovation' WHERE id = 'fsic-renewal.a04.renovation';
UPDATE form_item SET label = 'Present/Actual Inspection: Repair' WHERE id = 'fsic-renewal.a04.repair';
UPDATE form_item SET label = 'Present/Actual Inspection: Additional' WHERE id = 'fsic-renewal.a04.additional';
UPDATE form_item SET label = 'Present/Actual Inspection: State particular changes' WHERE id = 'fsic-renewal.a04.particular_changes';
UPDATE form_item SET label = 'Present/Actual Inspection: Renovation' WHERE id = 'fsic-renewal.a05.renovation';
UPDATE form_item SET label = 'Present/Actual Inspection: Repair' WHERE id = 'fsic-renewal.a05.repair';
UPDATE form_item SET label = 'Present/Actual Inspection: Additional' WHERE id = 'fsic-renewal.a05.additional';
UPDATE form_item SET label = 'Present/Actual Inspection: State particular violation' WHERE id = 'fsic-renewal.a05.particular_violation';
UPDATE form_item SET label = 'Present/Actual Inspection: Maximum allowable quantity' WHERE id = 'fsic-renewal.a08.maq';
UPDATE form_item SET label = 'Present/Actual Inspection: No additional fire protection' WHERE id = 'fsic-renewal.a08.no_additional_protection';
UPDATE form_item SET label = 'Present/Actual Inspection: No MSDS' WHERE id = 'fsic-renewal.a08.no_msds';
UPDATE form_item SET label = 'Present/Actual Inspection: No proper storage' WHERE id = 'fsic-renewal.a08.no_proper_storage';
UPDATE form_item SET label = 'Present/Actual Inspection: Good and operational condition' WHERE id = 'fsic-renewal.a10.good_condition';
UPDATE form_item SET label = 'Present/Actual Inspection: Good and operational condition' WHERE id = 'fsic-renewal.a11.good_condition';
UPDATE form_item SET label = 'Present/Actual Inspection: Need additional fire extinguisher' WHERE id = 'fsic-renewal.a11.need_additional';
UPDATE form_item SET label = 'Present/Actual Inspection: Location' WHERE id = 'fsic-renewal.a11.location';
UPDATE form_item SET label = 'Present/Actual Inspection: No. of fire extinguishers' WHERE id = 'fsic-renewal.a11.number';
UPDATE form_item SET label = 'Previous After Inspection Report: FSMR submitted dated' WHERE id = 'fsic-renewal.a13.prev_submitted_date';
UPDATE form_item SET label = 'Previous After Inspection Report: FSMR signed and certified by' WHERE id = 'fsic-renewal.a13.prev_signed_by';
UPDATE form_item SET label = 'Present/Actual Inspection: FSMR submitted dated' WHERE id = 'fsic-renewal.a13.submitted_date';
UPDATE form_item SET label = 'Present/Actual Inspection: FSMR signed and certified by' WHERE id = 'fsic-renewal.a13.signed_by';
UPDATE form_item SET label = 'Present/Actual Inspection: FSMR submitted' WHERE id = 'fsic-renewal.a13.submitted';
