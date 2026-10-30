-- BISAYA: kini nga file mao ang sulod sa usa ka form (mga seksyon, pangutana ug pilian).
-- Gi-load kini sa DatabaseService KAUSA RA. Ang matag id nagsugod sa ngalan sa form.
-- =====================================================================
-- Seed: BFP-QSF-FSED-051 Conveyance clearance (hazardous materials and chemicals in vehicles)
-- Load AFTER schema.sql. The template and its signatories already exist in schema.sql.
-- Generated from the official form. Ids start with the template id so every row is easy to identify.
-- =====================================================================

INSERT INTO form_section (id, template_id, key, title, sort_order) VALUES
 ('conveyance-clearance.s.vehicle','conveyance-clearance','vehicle','Driver and vehicle',1),
 ('conveyance-clearance.s.conditions','conveyance-clearance','conditions','Conditions',2),
 ('conveyance-clearance.s.findings','conveyance-clearance','findings','Defects and recommendations',3);

INSERT INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('conveyance-clearance.driver_name','conveyance-clearance','conveyance-clearance.s.vehicle',NULL,'driver_name','Name of Driver',1,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.driver_license_no','conveyance-clearance','conveyance-clearance.s.vehicle',NULL,'driver_license_no','Driver License No.',2,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.vehicle_type','conveyance-clearance','conveyance-clearance.s.vehicle',NULL,'vehicle_type','Type of Vehicle',3,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.plate_no','conveyance-clearance','conveyance-clearance.s.vehicle',NULL,'plate_no','Plate No.',4,'TEXT',NULL,NULL,1,NULL,NULL,NULL),
 ('conveyance-clearance.engine_no','conveyance-clearance','conveyance-clearance.s.vehicle',NULL,'engine_no','Motor (Engine) Number',5,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.chassis_no','conveyance-clearance','conveyance-clearance.s.vehicle',NULL,'chassis_no','Chassis No.',6,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.quantity_description','conveyance-clearance','conveyance-clearance.s.vehicle',NULL,'quantity_description','Quantity & Description',7,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.registration_validity','conveyance-clearance','conveyance-clearance.s.vehicle',NULL,'registration_validity','Validity of Registration',8,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.trailer_no','conveyance-clearance','conveyance-clearance.s.vehicle',NULL,'trailer_no','If with trailer, Trailer No.',9,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.01','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.01','LTO Certificate of Registration & Updated LTO Official Receipt (OR) of vehicle available.',10,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.02','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.02','Business Permit and Fire Safety Inspection Certificate for Business (Operator).',11,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.02.fsic_no','conveyance-clearance','conveyance-clearance.s.conditions','conveyance-clearance.cond.02','cond.02.fsic_no','Fire Safety Inspection Certificate No.',12,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.02.fsic_date','conveyance-clearance','conveyance-clearance.s.conditions','conveyance-clearance.cond.02','cond.02.fsic_date','Date issued',13,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.03','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.03','Certification of the design, construction & testing of tank from manufacturer shall readily available (for new tank vehicle).',14,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.04','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.04','With hydrotest certificate per compartment from fabricator at 3psi for 5 min. (NFPA-385).',15,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.05','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.05','Each compartment provided with internal/emergency valve (pneumatic/ mechanical).',16,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.06','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.06','Hazardous Material Identification System (HMIS) label. Referred to (NFPA 400, Globally Harmonized System (GHS) and other Internationally recognized standards. Material Safety Data Sheet (MSDS) of products being loaded shall be available at all times.',17,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.07','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.07','"NO SMOKING" sign within 50 ft. (Rear & both sides). Signage size at least (273mm) in height and the text indicating the "NO SMOKING" sign should be at least (41mm).',18,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.08','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.08','"FLAMMABLE LIQUID" or "FLAMMABLE GAS" sign. (Rear & both sides). Signage size at least (273mm) in height and the text indicating the "FLAMMABLE LIQUID" or "FLAMMABLE GAS" sign should be at least (41mm).',19,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.09','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.09','At least one (1)-unit portable fire extinguishers with appropriate rating based on the products and shall be properly maintained at all times located conspicuously and good operational condition.',20,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.10','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.10','Wheel chocks are readily available with two (2) pieces (with truck body number painted on it) to prevent motion of the vehicle while it is being loaded, unloaded or parked.',21,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.11','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.11','Bonding/grounding system for tank vehicle shall be provided/available.',22,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.12','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.12','Piping & hoses are maintained in good condition while in service.',23,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.13','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.13','Basic oil spill containment kit available (absorbent pads/ pail of sand with towel).',24,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.14','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.14','Warning cones four (4) pieces and at least two (2) feet high.',25,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.15','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.15','Early warning device (triangular in shape) two (2) pieces available and not damaged.',26,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.16','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.16','All tires in good condition.',27,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.17','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.17','Whistle with audible sound for emergency attraction shall be provided for driver.',28,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.cond.18','conveyance-clearance','conveyance-clearance.s.conditions',NULL,'cond.18','Medical first aid kit available.',29,'NONE',NULL,'cs-yn',0,NULL,NULL,NULL),
 ('conveyance-clearance.defects','conveyance-clearance','conveyance-clearance.s.findings',NULL,'defects','Defects / deficiencies noted during inspection',30,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.recommendations','conveyance-clearance','conveyance-clearance.s.findings',NULL,'recommendations','Recommendations',31,'LONG_TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('conveyance-clearance.rec.recommend_issuance','conveyance-clearance','conveyance-clearance.s.findings',NULL,'rec.recommend_issuance','Recommend issuance of FSC for Conveyance',32,'BOOL',NULL,NULL,0,NULL,NULL,NULL);

