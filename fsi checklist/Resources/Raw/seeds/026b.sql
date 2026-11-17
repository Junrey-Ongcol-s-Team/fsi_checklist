-- =====================================================================
-- BISAYA: 026 Gasoline Service Station: EXPANDERVIEW sa matag seksyon:
--   General information, Underground tanks, Underground tank 1-3, Vent pipe 1-3,
--   Pump (dispensing unit) 1-4, Emergency lights, Warning / safety signs,
--   First aid fire protection equipment, Other fire safety features
--   (Nature of inspection: naa na sa nature2.sql. Reference ug Defects: wala giusab.)
-- Paagi (sama sa 024e): ULOHAN (NONE) nga parehas ang ngalan sa seksyon (dili doble sa screen),
-- dayon ang mga pangutana sa seksyon gibutang ubos niini. Ang tubag ug print dili mausab.
-- FormPageViewModel.cs ExpanderItems: ang mga ulohan (gasoline-station.<seksyon>.grp).
-- Modagan KAUSA RA (seed.026b).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values)
SELECT REPLACE(s.id, '.s.', '.') || '.grp', s.template_id, s.id, NULL, s.key || '.grp', s.title,
       (SELECT MIN(sort_order) FROM form_item WHERE section_id = s.id) - 1,
       'NONE', NULL, NULL, 0, NULL, NULL, NULL
  FROM form_section s
 WHERE s.template_id = 'gasoline-station'
   AND s.id NOT IN ('gasoline-station.s.reference', 'gasoline-station.s.nature', 'gasoline-station.s.findings');

UPDATE form_item
   SET parent_item_id = REPLACE(section_id, '.s.', '.') || '.grp'
 WHERE template_id = 'gasoline-station'
   AND parent_item_id IS NULL
   AND id NOT LIKE '%.grp'
   AND section_id NOT IN ('gasoline-station.s.reference', 'gasoline-station.s.nature', 'gasoline-station.s.findings');
