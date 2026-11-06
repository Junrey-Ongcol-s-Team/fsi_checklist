-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (BFP-QSF-FSED-061). V. FIRE PROTECTION:
-- ang matag bahin (A. Sprinkler, B. Wet standpipe, C. Fire pump ... K. Fire wall) = usa ka ExpanderView.
-- Bag-ong header sa matag seksyon (key = <seksyon>.grp, ngalan = titulo sa seksyon nga walay "Fire protection: "),
-- ug ang tanang pangutana sa seksyon nahimong anak niini.
-- Pareho ra ang mga key, so walay kausaban sa print ug sa mga tubag. Modagan KAUSA RA (seed.061j).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values)
SELECT 'fsic-new-occupancy.' || s.key || '.grp', s.template_id, s.id, NULL, s.key || '.grp',
       REPLACE(s.title, 'Fire protection: ', ''), 0, 'NONE', NULL, NULL, 0, NULL, NULL, NULL
  FROM form_section s
 WHERE s.template_id = 'fsic-new-occupancy' AND s.key LIKE 'fp\_%' ESCAPE '\';

UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.' ||
       (SELECT s.key FROM form_section s WHERE s.id = form_item.section_id) || '.grp'
 WHERE template_id = 'fsic-new-occupancy' AND parent_item_id IS NULL
   AND section_id IN (SELECT id FROM form_section WHERE template_id = 'fsic-new-occupancy' AND key LIKE 'fp\_%' ESCAPE '\')
   AND key NOT LIKE '%.grp';
