-- =====================================================================
-- BISAYA: 075 Conveyance thru Pipelines: EXPANDERVIEW matag grupo:
--   Test records, Field identification, Corrosion, Pipe support, Leaks, Vibration, Insulation
-- Paagi (sama sa 024e): ULOHAN (NONE) matag seksyon, parehas ang ngalan sa seksyon (dili doble sa screen),
-- dayon ang mga pangutana sa seksyon gibutang ubos niini. Ang tubag (Yes / No / Remarks) ug print dili mausab.
-- FormPageViewModel.cs ExpanderItems: ang mga ulohan.
-- Modagan KAUSA RA (seed.075c).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('conveyance-pipeline.tests.grp','conveyance-pipeline','conveyance-pipeline.s.tests',NULL,'tests.grp','Test records',0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.identification.grp','conveyance-pipeline','conveyance-pipeline.s.identification',NULL,'identification.grp','Field identification',0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.corrosion.grp','conveyance-pipeline','conveyance-pipeline.s.corrosion',NULL,'corrosion.grp','Corrosion',0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.support.grp','conveyance-pipeline','conveyance-pipeline.s.support',NULL,'support.grp','Pipe support',0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.leaks.grp','conveyance-pipeline','conveyance-pipeline.s.leaks',NULL,'leaks.grp','Leaks',0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.vibration.grp','conveyance-pipeline','conveyance-pipeline.s.vibration',NULL,'vibration.grp','Vibration',0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.insulation.grp','conveyance-pipeline','conveyance-pipeline.s.insulation',NULL,'insulation.grp','Insulation',0,'NONE',NULL,NULL,0,NULL,NULL,NULL);

UPDATE form_item SET parent_item_id = 'conveyance-pipeline.tests.grp'
 WHERE section_id = 'conveyance-pipeline.s.tests' AND parent_item_id IS NULL AND id <> 'conveyance-pipeline.tests.grp';

UPDATE form_item SET parent_item_id = 'conveyance-pipeline.identification.grp'
 WHERE section_id = 'conveyance-pipeline.s.identification' AND parent_item_id IS NULL AND id <> 'conveyance-pipeline.identification.grp';

UPDATE form_item SET parent_item_id = 'conveyance-pipeline.corrosion.grp'
 WHERE section_id = 'conveyance-pipeline.s.corrosion' AND parent_item_id IS NULL AND id <> 'conveyance-pipeline.corrosion.grp';

UPDATE form_item SET parent_item_id = 'conveyance-pipeline.support.grp'
 WHERE section_id = 'conveyance-pipeline.s.support' AND parent_item_id IS NULL AND id <> 'conveyance-pipeline.support.grp';

UPDATE form_item SET parent_item_id = 'conveyance-pipeline.leaks.grp'
 WHERE section_id = 'conveyance-pipeline.s.leaks' AND parent_item_id IS NULL AND id <> 'conveyance-pipeline.leaks.grp';

UPDATE form_item SET parent_item_id = 'conveyance-pipeline.vibration.grp'
 WHERE section_id = 'conveyance-pipeline.s.vibration' AND parent_item_id IS NULL AND id <> 'conveyance-pipeline.vibration.grp';

UPDATE form_item SET parent_item_id = 'conveyance-pipeline.insulation.grp'
 WHERE section_id = 'conveyance-pipeline.s.insulation' AND parent_item_id IS NULL AND id <> 'conveyance-pipeline.insulation.grp';
