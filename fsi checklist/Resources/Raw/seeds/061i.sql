-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (BFP-QSF-FSED-061). VII. HAZARD:
-- ang A, B ug C = tagsa-tagsa ka ExpanderView (sama sa official form):
--   A. Other flammable liquids (i.e. alcohol, ether, etc.)                   -> hazard.a1 - a3
--   B. Miscellaneous hazards (mechanical equipment room, storage, supply room) -> hazard.b1 - b2
--   C. Housekeeping, maintenance, storage & waste disposal                  -> hazard.c1 - c3
-- Ang "Hazard Contents" (contents, quantity, placard ...) magpabilin sa ibabaw.
-- Pareho ra ang mga key, so walay kausaban sa print ug sa mga tubag. Modagan KAUSA RA (seed.061i).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-new-occupancy.hazard.grp_a','fsic-new-occupancy','fsic-new-occupancy.s.hazard',NULL,'hazard.grp_a','A. Other flammable liquids (i.e. alcohol, ether, etc.)',900,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.hazard.grp_b','fsic-new-occupancy','fsic-new-occupancy.s.hazard',NULL,'hazard.grp_b','B. Miscellaneous hazards (mechanical equipment room, storage, supply room)',901,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.hazard.grp_c','fsic-new-occupancy','fsic-new-occupancy.s.hazard',NULL,'hazard.grp_c','C. Housekeeping, maintenance, storage & waste disposal',902,'NONE',NULL,NULL,0,NULL,NULL,NULL);

UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.hazard.grp_a'
 WHERE section_id = 'fsic-new-occupancy.s.hazard' AND parent_item_id IS NULL AND key IN ('hazard.a1','hazard.a2','hazard.a3');
UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.hazard.grp_b'
 WHERE section_id = 'fsic-new-occupancy.s.hazard' AND parent_item_id IS NULL AND key IN ('hazard.b1','hazard.b2');
UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.hazard.grp_c'
 WHERE section_id = 'fsic-new-occupancy.s.hazard' AND parent_item_id IS NULL AND key IN ('hazard.c1','hazard.c2','hazard.c3');
