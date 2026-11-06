-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (BFP-QSF-FSED-061).
-- VI. SIGNS, LIGHTING, AND EXITS SIGNAGE = tulo ka ExpanderView, sama sa official form:
--   A. Marking of means of egress (EXIT)                        -> signage.a1 - a3
--   B. Marking of means of egress (Emergency evacuation plan)   -> signage.b1 - b3 (+ 12 ka basic info) ug design format d1 - d3
--   C. Illumination of means of egress (+ Emergency lighting)   -> illum.c1 - c3 ug illum.e1 - e3
-- Pareho ra ang mga key, so walay kausaban sa print ug sa mga tubag. Modagan KAUSA RA (seed.061f).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-new-occupancy.signs.a','fsic-new-occupancy','fsic-new-occupancy.s.signage',NULL,'signs.a','A. Marking of means of egress (EXIT)',1,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.signs.b','fsic-new-occupancy','fsic-new-occupancy.s.signage',NULL,'signs.b','B. Marking of means of egress (Emergency evacuation plan)',2,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.signs.c','fsic-new-occupancy','fsic-new-occupancy.s.signage',NULL,'signs.c','C. Illumination of means of egress',3,'NONE',NULL,NULL,0,NULL,NULL,NULL);

UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.signs.a'
 WHERE section_id = 'fsic-new-occupancy.s.signage' AND parent_item_id IS NULL AND key LIKE 'signage.a%';
UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.signs.b'
 WHERE section_id = 'fsic-new-occupancy.s.signage' AND parent_item_id IS NULL AND (key LIKE 'signage.b%' OR key LIKE 'signage.d%');
UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.signs.c'
 WHERE section_id = 'fsic-new-occupancy.s.signage' AND parent_item_id IS NULL AND key LIKE 'illum.%';
