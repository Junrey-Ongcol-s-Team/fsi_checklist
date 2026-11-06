-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (BFP-QSF-FSED-061).
--   1. MEANS OF EGRESS: ang A. Exit access, B. Exits ug C. Exits discharge = tagsa-tagsa ka ExpanderView
--      (bag-ong header; ang tanang pangutana sa seksyon nahimong anak niini). Ang circle progress
--      moihap sa Passed / Failed / N/A ug sa ubang kinahanglan nga tubag.
--   2. SECTIONAL OCCUPANCY: kinahanglan ma-fill up ang matag floor (isulat "N/A" kung wala),
--      aron naay sakto nga circle progress (0/6 -> ✓).
-- Pareho ra ang mga key, so walay kausaban sa print ug sa mga tubag. Modagan KAUSA RA (seed.061e).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-new-occupancy.egress.a','fsic-new-occupancy','fsic-new-occupancy.s.egress_access',NULL,'egress.a','A. Exit access',44,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.egress.b','fsic-new-occupancy','fsic-new-occupancy.s.egress_exits',NULL,'egress.b','B. Exits',75,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.egress.c','fsic-new-occupancy','fsic-new-occupancy.s.egress_discharge',NULL,'egress.c','C. Exits discharge',117,'NONE',NULL,NULL,0,NULL,NULL,NULL);

UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.egress.a'
 WHERE section_id = 'fsic-new-occupancy.s.egress_access' AND parent_item_id IS NULL AND id <> 'fsic-new-occupancy.egress.a';
UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.egress.b'
 WHERE section_id = 'fsic-new-occupancy.s.egress_exits' AND parent_item_id IS NULL AND id <> 'fsic-new-occupancy.egress.b';
UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.egress.c'
 WHERE section_id = 'fsic-new-occupancy.s.egress_discharge' AND parent_item_id IS NULL AND id <> 'fsic-new-occupancy.egress.c';

-- Sectional occupancy: kinahanglan ang matag floor (N/A kung wala)
UPDATE form_item SET is_required = 1, help_text = 'Write N/A if none.'
 WHERE id IN ('fsic-new-occupancy.sectional.basement','fsic-new-occupancy.sectional.ground','fsic-new-occupancy.sectional.second',
              'fsic-new-occupancy.sectional.third','fsic-new-occupancy.sectional.fourth','fsic-new-occupancy.sectional.nth');
