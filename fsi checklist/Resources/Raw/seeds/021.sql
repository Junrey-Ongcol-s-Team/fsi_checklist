-- BISAYA: kini nga file mao ang sulod sa usa ka form (mga seksyon, pangutana ug pilian).
-- Gi-load kini sa DatabaseService KAUSA RA. Ang matag id nagsugod sa ngalan sa form.
-- =====================================================================
-- Seed: FSED-021 Single and two-family dwellings checklist (printed Rev. 01, 07.05.19)
-- Load AFTER schema.sql.
-- Generated from the official form. Ids start with the template id so every row is easy to identify.
-- =====================================================================

INSERT INTO form_template (id, code, revision, revision_date, name, display_name, subject_type, purpose, sort_order) VALUES
 ('dwelling','BFP-QSF-FSED-021','01',NULL,'Single and Two-Family Dwellings Checklist','Single / two-family dwelling','ESTABLISHMENT','DWELLING',8);

WITH r(role_code, stage, req, sort) AS (VALUES ('OWNER_REP','ON_SITE',1,1),('INSPECTOR','ON_SITE',1,2),('TEAM_LEADER','ON_SITE',1,3),('CHIEF_FSES','APPROVAL',0,4),('FIRE_MARSHAL','APPROVAL',0,5))
INSERT INTO form_template_signatory (template_id, role_code, stage, is_required, sort_order)
SELECT 'dwelling', r.role_code, r.stage, r.req, r.sort FROM r;

INSERT INTO form_section (id, template_id, key, title, sort_order) VALUES
 ('dwelling.s.reference','dwelling','reference','Reference',1),
 ('dwelling.s.nature','dwelling','nature','Nature of inspection conducted',2),
 ('dwelling.s.general','dwelling','general','General information',3),
 ('dwelling.s.construction','dwelling','construction','Building construction',4),
 ('dwelling.s.occupancy','dwelling','occupancy','Sectional occupancy',5),
 ('dwelling.s.egress','dwelling','egress','Means of egress',6),
 ('dwelling.s.findings','dwelling','findings','Defects and recommendations',7);

INSERT INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('dwelling.io_date_issued','dwelling','dwelling.s.reference',NULL,'io_date_issued','Inspection order: date issued',1,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.nature.building_under_construction','dwelling','dwelling.s.nature',NULL,'nature.building_under_construction','Building under construction',2,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.nature.application_occupancy_permit','dwelling','dwelling.s.nature',NULL,'nature.application_occupancy_permit','Application for Occupancy Permit',3,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.nature.application_business_permit','dwelling','dwelling.s.nature',NULL,'nature.application_business_permit','Application for Business Permit',4,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.nature.periodic_inspection','dwelling','dwelling.s.nature',NULL,'nature.periodic_inspection','Periodic Inspection of Occupancy',5,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.nature.verification_ntcv','dwelling','dwelling.s.nature',NULL,'nature.verification_ntcv','Verification Inspection of Compliance to NTCV',6,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.nature.verification_complaint','dwelling','dwelling.s.nature',NULL,'nature.verification_complaint','Verification Inspection of Complaint Received',7,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.nature.others','dwelling','dwelling.s.nature',NULL,'nature.others','Others',8,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.nature.others_specify','dwelling','dwelling.s.nature','dwelling.nature.others','nature.others_specify','Others (specify)',9,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.total_land_area','dwelling','dwelling.s.general',NULL,'total_land_area','Total Land Area',10,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.total_floor_area','dwelling','dwelling.s.general',NULL,'total_floor_area','Total Floor Area',11,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.building_permit','dwelling','dwelling.s.general',NULL,'building_permit','Building Permit',12,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.building_permit_date','dwelling','dwelling.s.general',NULL,'building_permit_date','Building permit: date issued',13,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.fire_code_fee','dwelling','dwelling.s.general',NULL,'fire_code_fee','Fire Code Fee',14,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.or_no','dwelling','dwelling.s.general',NULL,'or_no','OR No.',15,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.fee_date','dwelling','dwelling.s.general',NULL,'fee_date','Fire code fee: date issued',16,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.beams','dwelling','dwelling.s.construction',NULL,'construction.beams','Beams',17,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.columns','dwelling','dwelling.s.construction',NULL,'construction.columns','Columns',18,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.flooring','dwelling','dwelling.s.construction',NULL,'construction.flooring','Flooring',19,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.exterior_walls','dwelling','dwelling.s.construction',NULL,'construction.exterior_walls','Exterior Walls',20,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.corridor_walls','dwelling','dwelling.s.construction',NULL,'construction.corridor_walls','Corridor Walls',21,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.room_partitions','dwelling','dwelling.s.construction',NULL,'construction.room_partitions','Room Partitions',22,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.main_stair','dwelling','dwelling.s.construction',NULL,'construction.main_stair','Main Stair',23,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.windows','dwelling','dwelling.s.construction',NULL,'construction.windows','Windows',24,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.ceiling','dwelling','dwelling.s.construction',NULL,'construction.ceiling','Ceiling',25,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.main_door','dwelling','dwelling.s.construction',NULL,'construction.main_door','Main Door',26,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.trusses','dwelling','dwelling.s.construction',NULL,'construction.trusses','Trusses',27,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.construction.roof','dwelling','dwelling.s.construction',NULL,'construction.roof','Roof',28,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.sectional_occupancy','dwelling','dwelling.s.occupancy',NULL,'sectional_occupancy','Specific usage of each floor, section or rooms',29,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.01','dwelling','dwelling.s.egress',NULL,'egress.01','Readily accessible?',30,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.02','dwelling','dwelling.s.egress',NULL,'egress.02','Obstructed?',31,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.03','dwelling','dwelling.s.egress',NULL,'egress.03','Travel distance within limits?',32,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.04','dwelling','dwelling.s.egress',NULL,'egress.04','Dead-ends within limits?',33,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.05','dwelling','dwelling.s.egress',NULL,'egress.05','Adequate illumination',34,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.06','dwelling','dwelling.s.egress',NULL,'egress.06','Proper rating of illumination?',35,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.07','dwelling','dwelling.s.egress',NULL,'egress.07','Panic hardware operational?',36,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.08','dwelling','dwelling.s.egress',NULL,'egress.08','Door swing in the direction of exit?',37,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.09','dwelling','dwelling.s.egress',NULL,'egress.09','Doors open easily?',38,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.10','dwelling','dwelling.s.egress',NULL,'egress.10','Self-closure operational?',39,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.egress.11','dwelling','dwelling.s.egress',NULL,'egress.11','Bldg w/Mezzanine?',40,'YES_NO',NULL,NULL,0,NULL,NULL,NULL);

INSERT INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('dwelling.egress.12','dwelling','dwelling.s.egress',NULL,'egress.12','Mezzanine with proper exits?',41,'YES_NO',NULL,NULL,0,NULL,'dwelling.egress.11','YES'),
 ('dwelling.defects','dwelling','dwelling.s.findings',NULL,'defects','Defects / deficiencies noted during inspection (attach pictures, sketch and others)',42,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('dwelling.recommendations','dwelling','dwelling.s.findings',NULL,'recommendations','Recommendations',43,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL);

