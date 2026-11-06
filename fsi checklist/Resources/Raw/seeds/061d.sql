-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (BFP-QSF-FSED-061).
-- SECTIONAL OCCUPANCY = usa ka ExpanderView. Header = "Sectional occupancy", sulod = Basement hangtod Nth Floor.
-- Bag-ong header (NONE) ug ang 6 ka floor nahimong anak niini. Pareho ra ang key (sectional.basement ...),
-- so walay kausaban sa print ug sa mga tubag. Modagan KAUSA RA (seed.061d).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-new-occupancy.sectional','fsic-new-occupancy','fsic-new-occupancy.s.sectional',NULL,'sectional','Sectional occupancy (indicate specific usage of each floor)',39,'NONE',NULL,NULL,0,'Use separate sheet if necessary',NULL,NULL);

UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.sectional'
 WHERE id IN ('fsic-new-occupancy.sectional.basement','fsic-new-occupancy.sectional.ground','fsic-new-occupancy.sectional.second',
              'fsic-new-occupancy.sectional.third','fsic-new-occupancy.sectional.fourth','fsic-new-occupancy.sectional.nth');
