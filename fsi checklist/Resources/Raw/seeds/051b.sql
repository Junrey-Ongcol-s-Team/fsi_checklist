-- =====================================================================
-- BISAYA: DUGANG sa Conveyance clearance (BFP-QSF-FSED-051).
-- "Prohibited Acts": kini ra nga duha ka linya ang makita (gipili ni Prince):
--   * Smoking is prohibited within 50ft in the vicinity of pipeline.
--   * Kaingin/Intentional grass fire at the vicinity of pipeline.
-- Pahibalo lang (NONE): walay kahon, dili apil sa progress, dili makababag sa Sign-off.
-- Gibutang tali sa Conditions ug sa Defects, pareho sa han-ay sa printout (051.pdf / 051.docx).
-- Modagan KAUSA RA (seed.051b).
-- =====================================================================

UPDATE form_section SET sort_order = 4 WHERE id = 'conveyance-clearance.s.findings';

INSERT OR IGNORE INTO form_section (id, template_id, key, title, sort_order) VALUES
 ('conveyance-clearance.s.prohibited','conveyance-clearance','prohibited','Prohibited Acts',3);

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('conveyance-clearance.prohibited.01','conveyance-clearance','conveyance-clearance.s.prohibited',NULL,'prohibited.01','• Smoking is prohibited within 50ft in the vicinity of pipeline.',1,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.prohibited.02','conveyance-clearance','conveyance-clearance.s.prohibited',NULL,'prohibited.02','• Kaingin/Intentional grass fire at the vicinity of pipeline.',2,'NONE',NULL,NULL,0,NULL,NULL,NULL);
