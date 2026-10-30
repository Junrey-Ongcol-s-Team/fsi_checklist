-- BISAYA: kini nga file mao ang sulod sa usa ka form (mga seksyon, pangutana ug pilian).
-- Gi-load kini sa DatabaseService KAUSA RA. Ang matag id nagsugod sa ngalan sa form.
-- =====================================================================
-- Seed: FSED-075 Conveyance clearance thru pipelines
-- Load AFTER schema.sql. Keys follow the printed numbers: test.01-03 for the test records and cond.01-30 for the field checks
-- (the printed form counts the INSULATION heading as item 24, so cond.24 does not exist).
-- Generated from the official form. Ids start with the template id so every row is easy to identify.
-- =====================================================================

INSERT INTO form_template (id, code, revision, revision_date, name, display_name, subject_type, purpose, sort_order) VALUES
 ('conveyance-pipeline','BFP-QSF-FSED-075','00','2025-08-29','Fire Safety Inspection Checklist for Conveyance Clearance thru Pipelines','Conveyance clearance (pipelines)','ESTABLISHMENT','CONVEYANCE_PIPELINE',6);

WITH r(role_code, stage, req, sort) AS (VALUES ('OWNER_REP','ON_SITE',1,1),('INSPECTOR','ON_SITE',1,2),('TEAM_LEADER','ON_SITE',1,3),('CHIEF_FSES','APPROVAL',0,4),('FIRE_MARSHAL','APPROVAL',0,5))
INSERT INTO form_template_signatory (template_id, role_code, stage, is_required, sort_order)
SELECT 'conveyance-pipeline', r.role_code, r.stage, r.req, r.sort FROM r;

INSERT INTO form_section (id, template_id, key, title, sort_order) VALUES
 ('conveyance-pipeline.s.pipeline','conveyance-pipeline','pipeline','Pipeline',1),
 ('conveyance-pipeline.s.tests','conveyance-pipeline','tests','Test records',2),
 ('conveyance-pipeline.s.identification','conveyance-pipeline','identification','Field identification',3),
 ('conveyance-pipeline.s.corrosion','conveyance-pipeline','corrosion','Corrosion',4),
 ('conveyance-pipeline.s.support','conveyance-pipeline','support','Pipe support',5),
 ('conveyance-pipeline.s.leaks','conveyance-pipeline','leaks','Leaks',6),
 ('conveyance-pipeline.s.vibration','conveyance-pipeline','vibration','Vibration',7),
 ('conveyance-pipeline.s.insulation','conveyance-pipeline','insulation','Insulation',8),
 ('conveyance-pipeline.s.findings','conveyance-pipeline','findings','Defects and recommendations',9);

INSERT INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('conveyance-pipeline.location','conveyance-pipeline','conveyance-pipeline.s.pipeline',NULL,'location','Location of the Pipeline System',1,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.pipeline_name','conveyance-pipeline','conveyance-pipeline.s.pipeline',NULL,'pipeline_name','Name of the Pipeline',2,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.length_m','conveyance-pipeline','conveyance-pipeline.s.pipeline',NULL,'length_m','Length of the Pipeline',3,'NUMBER','m',NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.diameter_m','conveyance-pipeline','conveyance-pipeline.s.pipeline',NULL,'diameter_m','Diameter of the Pipeline',4,'NUMBER','m',NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.thickness_in','conveyance-pipeline','conveyance-pipeline.s.pipeline',NULL,'thickness_in','Thickness of the Pipeline',5,'NUMBER','in',NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.year_manufactured','conveyance-pipeline','conveyance-pipeline.s.pipeline',NULL,'year_manufactured','Year of Manufacturing of the Pipeline',6,'INTEGER',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.product','conveyance-pipeline','conveyance-pipeline.s.pipeline',NULL,'product','Product Convey in the Pipeline',7,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.total_volume_m3','conveyance-pipeline','conveyance-pipeline.s.pipeline',NULL,'total_volume_m3','Total Volume Convey in the Pipeline',8,'NUMBER','m3',NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.inlet_pressure_bar','conveyance-pipeline','conveyance-pipeline.s.pipeline',NULL,'inlet_pressure_bar','Inlet Gas Pressure in the Pipeline',9,'NUMBER','bar',NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.outlet_pressure_bar','conveyance-pipeline','conveyance-pipeline.s.pipeline',NULL,'outlet_pressure_bar','Outlet Gas Pressure in the Pipeline',10,'NUMBER','bar',NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.test.01','conveyance-pipeline','conveyance-pipeline.s.tests',NULL,'test.01','Radiography test',11,'NONE',NULL,'cs-ynr',0,'Attach hardcopy of all test records, if conducted.',NULL,NULL),
 ('conveyance-pipeline.test.02','conveyance-pipeline','conveyance-pipeline.s.tests',NULL,'test.02','Ultrasonic test',12,'NONE',NULL,'cs-ynr',0,'Attach hardcopy of all test records, if conducted.',NULL,NULL),
 ('conveyance-pipeline.test.03','conveyance-pipeline','conveyance-pipeline.s.tests',NULL,'test.03','Hydrostatic test',13,'NONE',NULL,'cs-ynr',0,'Attach hardcopy of all test records, if conducted.',NULL,NULL),
 ('conveyance-pipeline.cond.01','conveyance-pipeline','conveyance-pipeline.s.identification',NULL,'cond.01','Is piping labeled/tagged with equipment ID number?',14,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.02','conveyance-pipeline','conveyance-pipeline.s.identification',NULL,'cond.02','Is the pipeline adequately identified?',15,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.03','conveyance-pipeline','conveyance-pipeline.s.identification',NULL,'cond.03','Are labels/tags readable?',16,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.04','conveyance-pipeline','conveyance-pipeline.s.identification',NULL,'cond.04','Do labels/tags need replacing?',17,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.05','conveyance-pipeline','conveyance-pipeline.s.corrosion',NULL,'cond.05','Any corrosion at support points or fixtures?',18,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.06','conveyance-pipeline','conveyance-pipeline.s.corrosion',NULL,'cond.06','Any coating or paint deterioration?',19,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.07','conveyance-pipeline','conveyance-pipeline.s.corrosion',NULL,'cond.07','Any areas with scale, pits, or rust?',20,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.08','conveyance-pipeline','conveyance-pipeline.s.corrosion',NULL,'cond.08','Any corrosion between flanges?',21,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.09','conveyance-pipeline','conveyance-pipeline.s.corrosion',NULL,'cond.09','Any significant corrosion of flange bolts?',22,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.10','conveyance-pipeline','conveyance-pipeline.s.support',NULL,'cond.10','Any shoes off their supports?',23,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.11','conveyance-pipeline','conveyance-pipeline.s.support',NULL,'cond.11','Any hangers missing or damaged?',24,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.12','conveyance-pipeline','conveyance-pipeline.s.support',NULL,'cond.12','Any springs hangers bottomed-out?',25,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.13','conveyance-pipeline','conveyance-pipeline.s.support',NULL,'cond.13','Any problems with support braces?',26,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.14','conveyance-pipeline','conveyance-pipeline.s.support',NULL,'cond.14','Any corrosion of supports?',27,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.15','conveyance-pipeline','conveyance-pipeline.s.support',NULL,'cond.15','Any loose or broken brackets?',28,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.16','conveyance-pipeline','conveyance-pipeline.s.leaks',NULL,'cond.16','Any leaks from the process?',29,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.17','conveyance-pipeline','conveyance-pipeline.s.leaks',NULL,'cond.17','Any leaks from steam tracing?',30,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.18','conveyance-pipeline','conveyance-pipeline.s.leaks',NULL,'cond.18','Are any valves leaking?',31,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.19','conveyance-pipeline','conveyance-pipeline.s.leaks',NULL,'cond.19','Any leaks at leak repair clamps?',32,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.20','conveyance-pipeline','conveyance-pipeline.s.leaks',NULL,'cond.20','Any evidence of past leaks?',33,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.21','conveyance-pipeline','conveyance-pipeline.s.vibration',NULL,'cond.21','Any significant vibration observed?',34,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.22','conveyance-pipeline','conveyance-pipeline.s.vibration',NULL,'cond.22','Any evidence of excessive movement?',35,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.23','conveyance-pipeline','conveyance-pipeline.s.vibration',NULL,'cond.23','Any pipe distortion observed?',36,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.25','conveyance-pipeline','conveyance-pipeline.s.insulation',NULL,'cond.25','Any physical damage or penetration?',37,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.26','conveyance-pipeline','conveyance-pipeline.s.insulation',NULL,'cond.26','Any deterioration/damage of weather seals?',38,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.27','conveyance-pipeline','conveyance-pipeline.s.insulation',NULL,'cond.27','Any bulging or wet insulation?',39,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.28','conveyance-pipeline','conveyance-pipeline.s.insulation',NULL,'cond.28','Any discoloration indicating leakage?',40,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL);

INSERT INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('conveyance-pipeline.cond.29','conveyance-pipeline','conveyance-pipeline.s.insulation',NULL,'cond.29','Any retaining bands missing/broken?',41,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.cond.30','conveyance-pipeline','conveyance-pipeline.s.insulation',NULL,'cond.30','Any missing jackets, plugs, or insulation?',42,'NONE',NULL,'cs-ynr',0,NULL,NULL,NULL),
 ('conveyance-pipeline.defects','conveyance-pipeline','conveyance-pipeline.s.findings',NULL,'defects','Defects / deficiencies noted during inspection',43,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.rec.issue_clearance','conveyance-pipeline','conveyance-pipeline.s.findings',NULL,'rec.issue_clearance','For issuance of Conveyance Clearance',44,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-pipeline.rec.notice_disapproval','conveyance-pipeline','conveyance-pipeline.s.findings',NULL,'rec.notice_disapproval','Notice of Disapproval',45,'BOOL',NULL,NULL,0,NULL,NULL,NULL);

