-- =====================================================================
-- BISAYA: DUGANG sa Conveyance clearance thru pipelines (BFP-QSF-FSED-075).
-- "Prohibited Acts" sama sa official form: duha ra ka linya, walay kahon [ ] (pahibalo lang).
-- Gibutang tali sa Insulation ug sa Defects, pareho sa han-ay sa form.
-- Walay tubag nga kinahanglan (NONE), so dili apil sa progress ug dili makababag sa Sign-off.
-- Sa printout naa na kini (naka-imprinta sa 075.pdf), so walay kausaban sa print.
-- Modagan KAUSA RA (seed.075b).
-- =====================================================================

-- Ibalhin ang Defects ngadto sa 10 aron ang Prohibited Acts mahimong 9.
UPDATE form_section SET sort_order = 10 WHERE id = 'conveyance-pipeline.s.findings';

INSERT OR IGNORE INTO form_section (id, template_id, key, title, sort_order) VALUES
 ('conveyance-pipeline.s.prohibited','conveyance-pipeline','prohibited','Prohibited Acts',9);

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('conveyance-pipeline.prohibited.01','conveyance-pipeline','conveyance-pipeline.s.prohibited',NULL,'prohibited.01','• Smoking is prohibited within 50ft in the vicinity of pipeline.',1,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.prohibited.02','conveyance-pipeline','conveyance-pipeline.s.prohibited',NULL,'prohibited.02','• Kaingin/Intentional grass fire at the vicinity of pipeline.',2,'NONE',NULL,NULL,0,NULL,NULL,NULL);
