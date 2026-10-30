-- BISAYA: kini nga file mao ang sulod sa usa ka form (mga seksyon, pangutana ug pilian).
-- Gi-load kini sa DatabaseService KAUSA RA. Ang matag id nagsugod sa ngalan sa form.
-- =====================================================================
-- Seed: FSED-024 Small/general business establishment (printed Rev. 02, 02.09.23)
-- Load AFTER schema.sql.
-- Generated from the official form. Ids start with the template id so every row is easy to identify.
-- =====================================================================

INSERT INTO form_template (id, code, revision, revision_date, name, display_name, subject_type, purpose, sort_order) VALUES
 ('small-business','BFP-QSF-FSED-024','02',NULL,'Fire Safety Inspection Checklist - Small/General Business Establishment','Small / general business','ESTABLISHMENT','SMALL_BUSINESS',7);

WITH r(role_code, stage, req, sort) AS (VALUES ('OWNER_REP','ON_SITE',1,1),('INSPECTOR','ON_SITE',1,2),('TEAM_LEADER','ON_SITE',1,3),('CHIEF_FSES','APPROVAL',0,4),('FIRE_MARSHAL','APPROVAL',0,5))
INSERT INTO form_template_signatory (template_id, role_code, stage, is_required, sort_order)
SELECT 'small-business', r.role_code, r.stage, r.req, r.sort FROM r;

INSERT INTO form_section (id, template_id, key, title, sort_order) VALUES
 ('small-business.s.reference','small-business','reference','Reference',1),
 ('small-business.s.nature','small-business','nature','Nature of inspection conducted',2),
 ('small-business.s.general','small-business','general','General information',3),
 ('small-business.s.other_info','small-business','other_info','Other information',4),
 ('small-business.s.egress','small-business','egress','Means of egress',5),
 ('small-business.s.signage','small-business','signage','Signs, lighting, and exits signage',6),
 ('small-business.s.hazard','small-business','hazard','Hazard',7),
 ('small-business.s.protection','small-business','protection','Fire protection',8),
 ('small-business.s.findings','small-business','findings','Defects and recommendations',9);

INSERT INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('small-business.io_date_issued','small-business','small-business.s.reference',NULL,'io_date_issued','Inspection order: date issued',1,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.during_construction','small-business','small-business.s.nature',NULL,'nature.during_construction','Inspection during construction',2,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.fsic_occupancy','small-business','small-business.s.nature',NULL,'nature.fsic_occupancy','FSIC for Certificate for Occupancy',3,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.fsic_business_new','small-business','small-business.s.nature',NULL,'nature.fsic_business_new','FSIC for Business Permit (New)',4,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.fsic_business_renewal','small-business','small-business.s.nature',NULL,'nature.fsic_business_renewal','FSIC for Business Permit (Renewal)',5,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.fsic_peza','small-business','small-business.s.nature',NULL,'nature.fsic_peza','FSIC for Certificate of Annual Inspection (PEZA)',6,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.verification','small-business','small-business.s.nature',NULL,'nature.verification','Verification inspection for compliance',7,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.verification.ntc','small-business','small-business.s.nature','small-business.nature.verification','nature.verification.ntc','NTC',8,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.verification.ntcv','small-business','small-business.s.nature','small-business.nature.verification','nature.verification.ntcv','NTCV',9,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.verification.abatement','small-business','small-business.s.nature','small-business.nature.verification','nature.verification.abatement','Abatement',10,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.verification.closure','small-business','small-business.s.nature','small-business.nature.verification','nature.verification.closure','Closure',11,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.notice_of_disapproval','small-business','small-business.s.nature',NULL,'nature.notice_of_disapproval','Notice of Disapproval, if Certificate of Occupancy',12,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.others','small-business','small-business.s.nature',NULL,'nature.others','Others',13,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature.others_specify','small-business','small-business.s.nature','small-business.nature.others','nature.others_specify','Others (specify)',14,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.building_name','small-business','small-business.s.general',NULL,'building_name','Name of Building',15,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.business_name','small-business','small-business.s.general',NULL,'business_name','Business Name',16,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.nature_of_business','small-business','small-business.s.general',NULL,'nature_of_business','Nature of Business',17,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fsec_no','small-business','small-business.s.general',NULL,'fsec_no','FSEC No.',18,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fsec_date','small-business','small-business.s.general',NULL,'fsec_date','FSEC No.: date issued',19,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.renovation_permit_no','small-business','small-business.s.general',NULL,'renovation_permit_no','Building/Renovation Permit',20,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.renovation_permit_date','small-business','small-business.s.general',NULL,'renovation_permit_date','Building/Renovation Permit: date issued',21,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fsic_no','small-business','small-business.s.general',NULL,'fsic_no','FSIC No. (Latest)',22,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fsic_date','small-business','small-business.s.general',NULL,'fsic_date','FSIC No. (Latest): date issued',23,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.business_permit_no','small-business','small-business.s.general',NULL,'business_permit_no','Business Permit No.',24,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.business_permit_date','small-business','small-business.s.general',NULL,'business_permit_date','Business Permit No.: date issued',25,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.insurance_policy_no','small-business','small-business.s.general',NULL,'insurance_policy_no','Fire Insurance Policy No.',26,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.insurance_policy_date','small-business','small-business.s.general',NULL,'insurance_policy_date','Fire Insurance Policy No.: date issued',27,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.occupancy_type','small-business','small-business.s.other_info',NULL,'occupancy_type','Type of Occupancy',28,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.occupancy_type_other','small-business','small-business.s.other_info',NULL,'occupancy_type_other','Type of occupancy: others (specify)',29,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.construction_type','small-business','small-business.s.other_info',NULL,'construction_type','Construction Type',30,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.total_floor_area','small-business','small-business.s.other_info',NULL,'total_floor_area','Total Floor Area',31,'NUMBER','m2',NULL,0,NULL,NULL,NULL),
 ('small-business.occupant_load','small-business','small-business.s.other_info',NULL,'occupant_load','Occupant Load',32,'INTEGER','persons',NULL,0,NULL,NULL,NULL),
 ('small-business.stories','small-business','small-business.s.other_info',NULL,'stories','No. of Stories',33,'INTEGER',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.portion_occupied','small-business','small-business.s.other_info',NULL,'portion_occupied','Portion Occupied',34,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.building_height','small-business','small-business.s.other_info',NULL,'building_height','Building Height',35,'NUMBER','m',NULL,0,NULL,NULL,NULL),
 ('small-business.mezzanine','small-business','small-business.s.other_info',NULL,'mezzanine','With Mezzanine',36,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.handrails','small-business','small-business.s.other_info',NULL,'handrails','Handrails/Railings provided',37,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.egress.access.doors','small-business','small-business.s.egress',NULL,'egress.access.doors','Exit access: Doors',38,'NUMBER','m','cs-pfnr',0,NULL,NULL,NULL),
 ('small-business.egress.access.corridors','small-business','small-business.s.egress',NULL,'egress.access.corridors','Exit access: Corridors / Hallways',39,'NUMBER','m','cs-pfnr',0,NULL,NULL,NULL),
 ('small-business.egress.exits.doors','small-business','small-business.s.egress',NULL,'egress.exits.doors','Exits: Exit Doors',40,'NUMBER','m','cs-pfnr',0,'Clear width.',NULL,NULL);

INSERT INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('small-business.egress.exits.horizontal','small-business','small-business.s.egress',NULL,'egress.exits.horizontal','Exits: Horizontal Exits',41,'NUMBER','m','cs-pfnr',0,'Clear width.',NULL,NULL),
 ('small-business.egress.exits.stairs','small-business','small-business.s.egress',NULL,'egress.exits.stairs','Exits: Stairs',42,'NUMBER','m','cs-pfnr',0,'Min. width 0.60 m between rails of fire escape stairs. Min. new stair width 0.915 m for fewer than 50 persons occupant load.',NULL,NULL),
 ('small-business.egress.two_means','small-business','small-business.s.egress',NULL,'egress.two_means','At least two (2) means of egress for each floor',43,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.egress.doors_open_close','small-business','small-business.s.egress',NULL,'egress.doors_open_close','Exits doors open and close properly?',44,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.egress.door_swing','small-business','small-business.s.egress',NULL,'egress.door_swing','Do doors swing in direction of egress?',45,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.signage.letter_height','small-business','small-business.s.signage',NULL,'signage.letter_height','Marking of means of egress: minimum letter height, 150 mm; width of stroke, 19 mm',46,'NONE',NULL,'cs-pfnr',0,NULL,NULL,NULL),
 ('small-business.signage.posted','small-business','small-business.s.signage',NULL,'signage.posted','EXIT signs are posted along Exit access, Exits and Exit discharge',47,'NONE',NULL,'cs-pfnr',0,NULL,NULL,NULL),
 ('small-business.signage.illuminated','small-business','small-business.s.signage',NULL,'signage.illuminated','EXIT signs are properly illuminated',48,'NONE',NULL,'cs-pfnr',0,NULL,NULL,NULL),
 ('small-business.hazard.classification','small-business','small-business.s.hazard',NULL,'hazard.classification','Hazard Classification',49,'CHOICE',NULL,NULL,0,'Low: no self-propagating fire can occur. Ordinary: burns with moderate rapidity or gives off considerable smoke. High: burns with extreme rapidity or explosions are likely.',NULL,NULL),
 ('small-business.hazard.storage_clearance_required','small-business','small-business.s.hazard',NULL,'hazard.storage_clearance_required','Storage Clearance Required?',50,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.hazard.storage_clearance','small-business','small-business.s.hazard',NULL,'hazard.storage_clearance','Fire Safety Clearance for Storage',51,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.hazard.storage_clearance_date','small-business','small-business.s.hazard',NULL,'hazard.storage_clearance_date','Storage clearance: date issued',52,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.hazard.storage_clearance_control_no','small-business','small-business.s.hazard',NULL,'hazard.storage_clearance_control_no','Storage clearance: control no.',53,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.hazard.content','small-business','small-business.s.hazard',NULL,'hazard.content','Hazard Content',54,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.hazard.total_volume','small-business','small-business.s.hazard',NULL,'hazard.total_volume','Total Volume',55,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.hazard.location','small-business','small-business.s.hazard',NULL,'hazard.location','Location',56,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.flammable.stock_clearance','small-business','small-business.s.hazard',NULL,'flammable.stock_clearance','Other flammable liquids: clearance of stocks from the ceiling',57,'NONE',NULL,'cs-pfnr',0,NULL,NULL,NULL),
 ('small-business.flammable.gas_detector','small-business','small-business.s.hazard',NULL,'flammable.gas_detector','Other flammable liquids: gas detector and shut-off device for LPG',58,'NONE',NULL,'cs-pfnr',0,NULL,NULL,NULL),
 ('small-business.lpg.approved_plans','small-business','small-business.s.hazard',NULL,'lpg.approved_plans','LPG system provided with approved plans (if 300 kgs / 300 GWC)',59,'YES_NO_NA',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.lpg.installation_clearance','small-business','small-business.s.hazard',NULL,'lpg.installation_clearance','Installation clearance',60,'YES_NO_NA',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.lpg.installation_clearance_date','small-business','small-business.s.hazard',NULL,'lpg.installation_clearance_date','Installation clearance: date issued',61,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.lpg.installation_clearance_control_no','small-business','small-business.s.hazard',NULL,'lpg.installation_clearance_control_no','Installation clearance: control no.',62,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.flammable.sealed_containers','small-business','small-business.s.hazard',NULL,'flammable.sealed_containers','Stored in sealed metal containers?',63,'YES_NO_NA',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.flammable.no_smoking_sign','small-business','small-business.s.hazard',NULL,'flammable.no_smoking_sign','Provided with "NO SMOKING" sign?',64,'YES_NO_NA',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fe.size','small-business','small-business.s.protection',NULL,'fe.size','Fire extinguisher: Fire Extinguisher Size',65,'NONE',NULL,'cs-pfnr',0,'Minimal sizes of fire extinguishers for the listed grades of hazards shall be provided on the basis of table 7 & 8 of RIRR of RA 9514.',NULL,NULL),
 ('small-business.fe.number','small-business','small-business.s.protection',NULL,'fe.number','Fire extinguisher: Minimum number of Extinguisher',66,'NONE',NULL,'cs-pfnr',0,'The minimum number of extinguishers shall be sufficient to meet the requirements of table 7 & 8 of RIRR of RA 9514.',NULL,NULL),
 ('small-business.fe.location','small-business','small-business.s.protection',NULL,'fe.location','Fire extinguisher: Location',67,'NONE',NULL,'cs-pfnr',0,'All extinguishers are in their proper location.',NULL,NULL),
 ('small-business.fe.seals','small-business','small-business.s.protection',NULL,'fe.seals','Fire extinguisher: Seals & Tags',68,'NONE',NULL,'cs-pfnr',0,'Extinguisher seals and tags are intact and extinguisher was serviced in the last 12 months.',NULL,NULL),
 ('small-business.fe.markings','small-business','small-business.s.protection',NULL,'fe.markings','Fire extinguisher: Markings',69,'NONE',NULL,'cs-pfnr',0,'Proper marking on extinguisher to indicate the type of fire the extinguisher can be used on.',NULL,NULL),
 ('small-business.fe.condition','small-business','small-business.s.protection',NULL,'fe.condition','Fire extinguisher: Condition',70,'NONE',NULL,'cs-pfnr',0,'No leaks, corrosion or other defects noticed.',NULL,NULL),
 ('small-business.fe.pressure','small-business','small-business.s.protection',NULL,'fe.pressure','Fire extinguisher: Pressure',71,'NONE',NULL,'cs-pfnr',0,'Pressure gauge arrow pointing in the "green" area.',NULL,NULL),
 ('small-business.fe.type','small-business','small-business.s.protection',NULL,'fe.type','Fire extinguisher type',72,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fe.quantity','small-business','small-business.s.protection',NULL,'fe.quantity','Fire extinguisher quantity',73,'INTEGER',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fe.capacity','small-business','small-business.s.protection',NULL,'fe.capacity','Fire extinguisher capacity',74,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.el.provided','small-business','small-business.s.protection',NULL,'el.provided','Emergency lighting systems: provided',75,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.el.functional','small-business','small-business.s.protection',NULL,'el.functional','Emergency lighting systems: functional',76,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.el.loc.hallways','small-business','small-business.s.protection',NULL,'el.loc.hallways','Emergency lighting location: Hallways',77,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.el.loc.exit_doors','small-business','small-business.s.protection',NULL,'el.loc.exit_doors','Emergency lighting location: Exit Doors',78,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.el.loc.stair_landings','small-business','small-business.s.protection',NULL,'el.loc.stair_landings','Emergency lighting location: Stairway Landings',79,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.el.other','small-business','small-business.s.protection',NULL,'el.other','Emergency lighting: others (specify)',80,'TEXT',NULL,NULL,0,NULL,NULL,NULL);

INSERT INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('small-business.fd.provided','small-business','small-business.s.protection',NULL,'fd.provided','Fire detection and alarm: provided',81,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fd.smoke','small-business','small-business.s.protection',NULL,'fd.smoke','Fire detection: smoke',82,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fd.heat','small-business','small-business.s.protection',NULL,'fd.heat','Fire detection: heat',83,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fd.mode','small-business','small-business.s.protection',NULL,'fd.mode','Fire detection mode',84,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fd.functional','small-business','small-business.s.protection',NULL,'fd.functional','Fire detection and alarm: functional',85,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fd.integrated','small-business','small-business.s.protection',NULL,'fd.integrated','Fire detection and alarm: integrated',86,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fd.units_per_floor','small-business','small-business.s.protection',NULL,'fd.units_per_floor','Fire detection: units per floor',87,'INTEGER',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fd.adequate','small-business','small-business.s.protection',NULL,'fd.adequate','Fire detection and alarm: adequate',88,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fd.panel_location','small-business','small-business.s.protection',NULL,'fd.panel_location','Location of control panel (if applicable)',89,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.defects','small-business','small-business.s.findings',NULL,'defects','Defects / deficiencies (list by item IV to VIII)',90,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.rec.comply_and_pay','small-business','small-business.s.findings',NULL,'rec.comply_and_pay','Comply with the defects/deficiencies stated above and pay the corresponding Fire Code Fees (including Storage Clearance Fee, Conveyance Clearance Fee) before issuance of FSIC',91,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.rec.fees_to_pay','small-business','small-business.s.findings',NULL,'rec.fees_to_pay','Fire Code Fees to pay',92,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.rec.notice_to_comply','small-business','small-business.s.findings',NULL,'rec.notice_to_comply','For issuance of: Notice to Comply',93,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.rec.notice_to_correct','small-business','small-business.s.findings',NULL,'rec.notice_to_correct','For issuance of: Notice to Correct Violation',94,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.rec.closure_order','small-business','small-business.s.findings',NULL,'rec.closure_order','For issuance of: Closure Order',95,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.rec.abatement_fine','small-business','small-business.s.findings',NULL,'rec.abatement_fine','For issuance of: Abatement Order with Administrative Fine',96,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.rec.closure_nonpayment','small-business','small-business.s.findings',NULL,'rec.closure_nonpayment','For issuance of: Closure Order for the non-payment of Administrative Fine',97,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.rec.nod','small-business','small-business.s.findings',NULL,'rec.nod','For issuance of: Notice of Disapproval, if Certificate of Occupancy is applied',98,'BOOL',NULL,NULL,0,NULL,NULL,NULL);

INSERT INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('small-business.ch.occupancy_type.MERCANTILE','small-business.occupancy_type','MERCANTILE','Mercantile',1),
 ('small-business.ch.occupancy_type.BUSINESS','small-business.occupancy_type','BUSINESS','Business',2),
 ('small-business.ch.occupancy_type.OTHERS','small-business.occupancy_type','OTHERS','Others',3),
 ('small-business.ch.construction_type.TIMBER','small-business.construction_type','TIMBER','Timber Framed and Walls',1),
 ('small-business.ch.construction_type.RC_MASONRY','small-business.construction_type','RC_MASONRY','Reinforced Concrete Framed with Masonry Walls',2),
 ('small-business.ch.construction_type.STEEL','small-business.construction_type','STEEL','Steel Framed and Walls',3),
 ('small-business.ch.construction_type.MIXED','small-business.construction_type','MIXED','Mixed Construction',4),
 ('small-business.ch.hazard.classification.LOW','small-business.hazard.classification','LOW','Low',1),
 ('small-business.ch.hazard.classification.ORDINARY','small-business.hazard.classification','ORDINARY','Ordinary',2),
 ('small-business.ch.hazard.classification.HIGH','small-business.hazard.classification','HIGH','High',3),
 ('small-business.ch.fd.mode.MANUAL','small-business.fd.mode','MANUAL','Manual',1),
 ('small-business.ch.fd.mode.AUTOMATIC','small-business.fd.mode','AUTOMATIC','Automatic',2);

