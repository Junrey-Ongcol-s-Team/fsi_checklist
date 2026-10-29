-- =====================================================================
-- Seed: FSED-064 (Conveyance of LPG in cylinders, 2/3-wheeled vehicles)
-- Load AFTER fsic_final_schema.sql.
-- =====================================================================

INSERT INTO form_template (id, code, revision, revision_date, name, display_name, subject_type, purpose, sort_order) VALUES
 ('t064','BFP-QSF-FSED-064','01',NULL,
  'Fire Safety Inspection Checklist for Fire Safety Clearance - Conveyance of LPG in Cylinders (2- or 3-Wheeled Vehicles)',
  'LPG motorcycle conveyance','VEHICLE','CONVEYANCE_LPG',5);

-- Signatures: the usual set
WITH r(role_code, stage, req, sort) AS (VALUES
 ('OWNER_REP','ON_SITE',1,1),('INSPECTOR','ON_SITE',1,2),('TEAM_LEADER','ON_SITE',1,3),
 ('CHIEF_FSES','APPROVAL',0,4),('FIRE_MARSHAL','APPROVAL',0,5))
INSERT INTO form_template_signatory (template_id, role_code, stage, is_required, sort_order)
SELECT 't064', r.role_code, r.stage, r.req, r.sort FROM r;

-- =====================================================================
-- FSED-064  LPG conveyance (same shape as 051, plus a variant switch and photo slots)
-- =====================================================================
INSERT INTO form_section (id, template_id, key, title, sort_order) VALUES
 ('s064-hdr','t064','header','Operator and vehicle',1),
 ('s064-cond','t064','conditions','Conditions',2),
 ('s064-rec','t064','recommendation','Recommendation',3);

INSERT INTO form_item (id, template_id, section_id, key, label, sort_order, data_type, is_required) VALUES
 ('i064-vtype','t064','s064-hdr','vehicle_type','Vehicle type',1,'CHOICE',1),
 ('i064-op','t064','s064-hdr','operator_name','Name of Operator',2,'TEXT',1),
 ('i064-addr','t064','s064-hdr','operator_address','Address',3,'TEXT',0),
 ('i064-brand','t064','s064-hdr','vehicle_brand','Brand of Vehicle',4,'TEXT',0),
 ('i064-plate','t064','s064-hdr','plate_no','Plate No.',5,'TEXT',1),
 ('i064-eng','t064','s064-hdr','engine_no','Motor (Engine) Number',6,'TEXT',0),
 ('i064-chs','t064','s064-hdr','chassis_no','Chassis No.',7,'TEXT',0),
 ('i064-reg','t064','s064-hdr','registration_validity','Validity of Registration',8,'DATE',0);

INSERT INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('ch064-1','i064-vtype','TWO_WHEELED','Two (2)-wheeled vehicle',1),
 ('ch064-2','i064-vtype','THREE_WHEELED','Three (3)-wheeled vehicle',2);

-- 13 yes/no conditions (column set YES_NO). Items 10 and 11 apply to one vehicle type each.
INSERT INTO form_item (id, template_id, section_id, key, label, sort_order, data_type, column_set_id, show_if_item_id, show_if_values) VALUES
 ('i064-c01','t064','s064-cond','cond.01','LTO Certificate of Registration and updated LTO OR of vehicle available',1,'NONE','cs-yn',NULL,NULL),
 ('i064-c02','t064','s064-cond','cond.02','Business Permit and FSIC (for renewal) for Business (Operator)',2,'NONE','cs-yn',NULL,NULL),
 ('i064-c03','t064','s064-cond','cond.03','Copy of the latest License to Operate issued by the DOE',3,'NONE','cs-yn',NULL,NULL),
 ('i064-c04','t064','s064-cond','cond.04','Cargo space is a metal cage, fully ventilated',4,'NONE','cs-yn',NULL,NULL),
 ('i064-c05','t064','s064-cond','cond.05','Metal cage has rubber saddle and belt or equivalent on both sides',5,'NONE','cs-yn',NULL,NULL),
 ('i064-c06','t064','s064-cond','cond.06','All signal, brake and emergency/hazard lights working',6,'NONE','cs-yn',NULL,NULL),
 ('i064-c07','t064','s064-cond','cond.07','Tire ply intact: no visible separation, wide tear, holes or bulges',7,'NONE','cs-yn',NULL,NULL),
 ('i064-c08','t064','s064-cond','cond.08','Required signages provided at the required locations',8,'NONE','cs-yn',NULL,NULL),
 ('i064-c09','t064','s064-cond','cond.09','All filled and used empty LPG cylinders loaded upright',9,'NONE','cs-yn',NULL,NULL),
 ('i064-c10','t064','s064-cond','cond.10','Two-wheeled: maximum load of 2 cylinders (11 kg) or 4 cylinders (2.7 kg)',10,'NONE','cs-yn','i064-vtype','TWO_WHEELED'),
 ('i064-c11','t064','s064-cond','cond.11','Three-wheeled: maximum 2 stacks, 12 cylinders (11 kg) or equivalent (1 stack = 6 cylinders)',11,'NONE','cs-yn','i064-vtype','THREE_WHEELED'),
 ('i064-c12','t064','s064-cond','cond.12','Driver has an unobstructed view',12,'NONE','cs-yn',NULL,NULL);

INSERT INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type) VALUES
 ('i064-c02a','t064','s064-cond','i064-c02','cond.02.fsic_no','FSIC No.',1,'TEXT'),
 ('i064-c02b','t064','s064-cond','i064-c02','cond.02.fsic_date','Date issued',2,'DATE');

-- Item 13: photos of the vehicle. Required photo slots are the item's choices.
INSERT INTO form_item (id, template_id, section_id, key, label, sort_order, data_type, column_set_id) VALUES
 ('i064-c13','t064','s064-cond','cond.13','5R size photos of the motor vehicle',13,'PHOTO','cs-yn');
INSERT INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('ch064-p1','i064-c13','FRONT','Front',1),
 ('ch064-p2','i064-c13','REAR','Rear',2),
 ('ch064-p3','i064-c13','SIDE','Side',3);

INSERT INTO form_item (id, template_id, section_id, key, label, sort_order, data_type) VALUES
 ('i064-r1','t064','s064-rec','rec.issue_clearance','For issuance of Conveyance Clearance',1,'BOOL'),
 ('i064-r2','t064','s064-rec','rec.notice_disapproval','Notice of Disapproval',2,'BOOL');

-- ---------------------------------------------------------------------
-- Example: items visible for inspection :id (the app does this in code;
-- shown as SQL because the rule is data, not logic)
--
--   SELECT i.key FROM form_item i
--   WHERE i.template_id = (SELECT template_id FROM inspection WHERE id = :id)
--     AND (i.show_if_item_id IS NULL OR EXISTS (
--           SELECT 1 FROM inspection_answer a
--           WHERE a.inspection_id = :id AND a.item_id = i.show_if_item_id
--             AND a.slot_key = 'value'
--             AND ',' || i.show_if_values || ',' LIKE '%,' || a.value_text || ',%'))
--   ORDER BY i.sort_order;
-- ---------------------------------------------------------------------
