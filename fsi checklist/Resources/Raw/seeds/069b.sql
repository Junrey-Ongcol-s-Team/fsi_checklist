-- BISAYA: DUGANG sa FSIC renewal (BFP-QSF-FSED-069) aron kompleto sama sa official form.
-- Mga kahon ug linya nga wala pa sa app kaniadto:
--   - "Compliant [ ] Yes" sa Previous After Inspection Report (bag-ong column sa cs-ccp: prev_compliant)
--   - "Dated: ____" sa Previous After Inspection Report
--   - [ ] FSIC for Renewal of Business Permit / Certificate of Annual Inspection (PEZA)
--   - ang mga kahon sa wala nga bahin sa 1, 8, 9, 10, 12 ug 13
--   - 13: Submitted dated ug Signed and certified by sa Previous nga column
-- Modagan KAUSA RA (seed.069b). Wala kini mo-usab sa mga tubag nga naa na: dugang ug han-ay ra.
-- =====================================================================

INSERT OR IGNORE INTO form_column (id, column_set_id, key, label, data_type, sort_order) VALUES
 ('c-ccp-0','cs-ccp','prev_compliant','Previous inspection: compliant','BOOL',0);

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-renewal.renewal_bp_peza','fsic-renewal','fsic-renewal.s.general',NULL,'renewal_bp_peza','FSIC for Renewal of Business Permit / Certificate of Annual Inspection (PEZA)',150,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.prev_report_date','fsic-renewal','fsic-renewal.s.assessment',NULL,'prev_report_date','Previous after-inspection report: dated',220,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a01.walls_ceiling','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a01','a01.walls_ceiling','Walls/ceiling interior finish',240,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a01.floor_finish','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a01','a01.floor_finish','Floor interior finish',250,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a08.hazard_classification','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a08','a08.hazard_classification','Hazard classification',540,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a08.other_flammable','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a08','a08.other_flammable','Other flammable liquids',550,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a08.misc_hazard','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a08','a08.misc_hazard','Miscellaneous hazard (mechanical equipment room, storage, supply room)',560,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a08.no_smoking_signages','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a08','a08.no_smoking_signages','No smoking signages',570,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a08.fuel_storage','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a08','a08.fuel_storage','Storage of gasoline/diesel in proper place',580,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a09.maintenance','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a09','a09.maintenance','Maintenance',640,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a09.storage','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a09','a09.storage','Storage',650,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a09.waste_disposal','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a09','a09.waste_disposal','Waste disposal',660,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.automatic','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10','a10.automatic','Automatic fire protection',680,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.standpipe','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10','a10.standpipe','Wet standpipe/fire hose cabinet',690,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.fire_pump','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10','a10.fire_pump','Fire pump',700,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.fdas','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10','a10.fdas','Fire detection and alarm communication system',710,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.fdas.alarm_facilities','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10.fdas','a10.fdas.alarm_facilities','Fire alarm facilities',720,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.fdas.location','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10.fdas','a10.fdas.location','Location',730,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.fdas.alarm_panel','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10.fdas','a10.fdas.alarm_panel','Alarm panel',740,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a12.utilities','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a12','a12.utilities','Utilities',820,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a12.smoke_control','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a12','a12.smoke_control','Smoke control system/smoke management',830,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a12.rubbish_chutes','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a12','a12.rubbish_chutes','Rubbish chutes/laundry chutes/flue-fed incinerators',840,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a13.passive','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a13','a13.passive','Passive fire protection (fire door, fire walls, compartmentation, horizontal exits, stairs, ramps, exit passageways)',860,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a13.active','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a13','a13.active','Active fire protection (automatic fire suppression, fire detection alarm & communication system, wet standpipe, smoke control system/pressurization)',870,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a13.prev_submitted_date','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a13','a13.prev_submitted_date','Previous inspection: FSMR submitted dated',880,'DATE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a13.prev_signed_by','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a13','a13.prev_signed_by','Previous inspection: FSMR signed and certified by',890,'TEXT',NULL,NULL,0,NULL,NULL,NULL);

-- Mas mubo nga ngalan (ang detalye naa na sa mga bag-ong kahon sa ilawom).
UPDATE form_item SET label = '1. Building Construction' WHERE id = 'fsic-renewal.a01';
UPDATE form_item SET label = '8. Hazard Contents' WHERE id = 'fsic-renewal.a08';
UPDATE form_item SET label = '9. Housekeeping' WHERE id = 'fsic-renewal.a09';
UPDATE form_item SET label = '10. Fire Protection' WHERE id = 'fsic-renewal.a10';
UPDATE form_item SET label = '12. Building Service Equipment' WHERE id = 'fsic-renewal.a12';
UPDATE form_item SET label = '13. Fire Safety Maintenance Report (FSMR)' WHERE id = 'fsic-renewal.a13';
UPDATE form_item SET label = 'Present inspection: FSMR submitted dated' WHERE id = 'fsic-renewal.a13.submitted_date';
UPDATE form_item SET label = 'Present inspection: FSMR signed and certified by' WHERE id = 'fsic-renewal.a13.signed_by';

-- Han-ay (sort_order) sama sa official form.
UPDATE form_item SET sort_order = 10 WHERE id = 'fsic-renewal.io_date_issued';
UPDATE form_item SET sort_order = 20 WHERE id = 'fsic-renewal.nature.fsic_business_permit';
UPDATE form_item SET sort_order = 30 WHERE id = 'fsic-renewal.nature.fsic_peza';
UPDATE form_item SET sort_order = 40 WHERE id = 'fsic-renewal.nature.others';
UPDATE form_item SET sort_order = 50 WHERE id = 'fsic-renewal.nature.others_specify';
UPDATE form_item SET sort_order = 60 WHERE id = 'fsic-renewal.nature.reinspection';
UPDATE form_item SET sort_order = 70 WHERE id = 'fsic-renewal.nature.reinspection.ntc';
UPDATE form_item SET sort_order = 80 WHERE id = 'fsic-renewal.nature.reinspection.ntcv';
UPDATE form_item SET sort_order = 90 WHERE id = 'fsic-renewal.nature.reinspection.abatement';
UPDATE form_item SET sort_order = 100 WHERE id = 'fsic-renewal.nature.reinspection.closure';
UPDATE form_item SET sort_order = 110 WHERE id = 'fsic-renewal.fsmr_required';
UPDATE form_item SET sort_order = 120 WHERE id = 'fsic-renewal.building_name';
UPDATE form_item SET sort_order = 130 WHERE id = 'fsic-renewal.business_name';
UPDATE form_item SET sort_order = 140 WHERE id = 'fsic-renewal.nature_of_business';
UPDATE form_item SET sort_order = 150 WHERE id = 'fsic-renewal.renewal_bp_peza';
UPDATE form_item SET sort_order = 160 WHERE id = 'fsic-renewal.fire_drill_no';
UPDATE form_item SET sort_order = 170 WHERE id = 'fsic-renewal.fire_drill_date';
UPDATE form_item SET sort_order = 180 WHERE id = 'fsic-renewal.business_permit_no';
UPDATE form_item SET sort_order = 190 WHERE id = 'fsic-renewal.business_permit_date';
UPDATE form_item SET sort_order = 200 WHERE id = 'fsic-renewal.insurance_policy_no';
UPDATE form_item SET sort_order = 210 WHERE id = 'fsic-renewal.insurance_policy_date';
UPDATE form_item SET sort_order = 220 WHERE id = 'fsic-renewal.prev_report_date';
UPDATE form_item SET sort_order = 230 WHERE id = 'fsic-renewal.a01';
UPDATE form_item SET sort_order = 240 WHERE id = 'fsic-renewal.a01.walls_ceiling';
UPDATE form_item SET sort_order = 250 WHERE id = 'fsic-renewal.a01.floor_finish';
UPDATE form_item SET sort_order = 260 WHERE id = 'fsic-renewal.a01.prev_fsic_no';
UPDATE form_item SET sort_order = 270 WHERE id = 'fsic-renewal.a01.prev_fsic_date';
UPDATE form_item SET sort_order = 280 WHERE id = 'fsic-renewal.a01.renovation';
UPDATE form_item SET sort_order = 290 WHERE id = 'fsic-renewal.a01.repair';
UPDATE form_item SET sort_order = 300 WHERE id = 'fsic-renewal.a01.additional';
UPDATE form_item SET sort_order = 310 WHERE id = 'fsic-renewal.a02';
UPDATE form_item SET sort_order = 320 WHERE id = 'fsic-renewal.a02.prev_floors';
UPDATE form_item SET sort_order = 330 WHERE id = 'fsic-renewal.a02.renovation';
UPDATE form_item SET sort_order = 340 WHERE id = 'fsic-renewal.a02.repair';
UPDATE form_item SET sort_order = 350 WHERE id = 'fsic-renewal.a02.additional';
UPDATE form_item SET sort_order = 360 WHERE id = 'fsic-renewal.a02.particular_floor';
UPDATE form_item SET sort_order = 370 WHERE id = 'fsic-renewal.a03';
UPDATE form_item SET sort_order = 380 WHERE id = 'fsic-renewal.a03.prev_occupancy';
UPDATE form_item SET sort_order = 390 WHERE id = 'fsic-renewal.a03.change_of_occupancy';
UPDATE form_item SET sort_order = 400 WHERE id = 'fsic-renewal.a04';
UPDATE form_item SET sort_order = 410 WHERE id = 'fsic-renewal.a04.renovation';
UPDATE form_item SET sort_order = 420 WHERE id = 'fsic-renewal.a04.repair';
UPDATE form_item SET sort_order = 430 WHERE id = 'fsic-renewal.a04.additional';
UPDATE form_item SET sort_order = 440 WHERE id = 'fsic-renewal.a04.particular_changes';
UPDATE form_item SET sort_order = 450 WHERE id = 'fsic-renewal.a05';
UPDATE form_item SET sort_order = 460 WHERE id = 'fsic-renewal.a05.renovation';
UPDATE form_item SET sort_order = 470 WHERE id = 'fsic-renewal.a05.repair';
UPDATE form_item SET sort_order = 480 WHERE id = 'fsic-renewal.a05.additional';
UPDATE form_item SET sort_order = 490 WHERE id = 'fsic-renewal.a05.particular_violation';
UPDATE form_item SET sort_order = 500 WHERE id = 'fsic-renewal.a06a';
UPDATE form_item SET sort_order = 510 WHERE id = 'fsic-renewal.a06b';
UPDATE form_item SET sort_order = 520 WHERE id = 'fsic-renewal.a07';
UPDATE form_item SET sort_order = 530 WHERE id = 'fsic-renewal.a08';
UPDATE form_item SET sort_order = 540 WHERE id = 'fsic-renewal.a08.hazard_classification';
UPDATE form_item SET sort_order = 550 WHERE id = 'fsic-renewal.a08.other_flammable';
UPDATE form_item SET sort_order = 560 WHERE id = 'fsic-renewal.a08.misc_hazard';
UPDATE form_item SET sort_order = 570 WHERE id = 'fsic-renewal.a08.no_smoking_signages';
UPDATE form_item SET sort_order = 580 WHERE id = 'fsic-renewal.a08.fuel_storage';
UPDATE form_item SET sort_order = 590 WHERE id = 'fsic-renewal.a08.maq';
UPDATE form_item SET sort_order = 600 WHERE id = 'fsic-renewal.a08.no_additional_protection';
UPDATE form_item SET sort_order = 610 WHERE id = 'fsic-renewal.a08.no_msds';
UPDATE form_item SET sort_order = 620 WHERE id = 'fsic-renewal.a08.no_proper_storage';
UPDATE form_item SET sort_order = 630 WHERE id = 'fsic-renewal.a09';
UPDATE form_item SET sort_order = 640 WHERE id = 'fsic-renewal.a09.maintenance';
UPDATE form_item SET sort_order = 650 WHERE id = 'fsic-renewal.a09.storage';
UPDATE form_item SET sort_order = 660 WHERE id = 'fsic-renewal.a09.waste_disposal';
UPDATE form_item SET sort_order = 670 WHERE id = 'fsic-renewal.a10';
UPDATE form_item SET sort_order = 680 WHERE id = 'fsic-renewal.a10.automatic';
UPDATE form_item SET sort_order = 690 WHERE id = 'fsic-renewal.a10.standpipe';
UPDATE form_item SET sort_order = 700 WHERE id = 'fsic-renewal.a10.fire_pump';
UPDATE form_item SET sort_order = 710 WHERE id = 'fsic-renewal.a10.fdas';
UPDATE form_item SET sort_order = 720 WHERE id = 'fsic-renewal.a10.fdas.alarm_facilities';
UPDATE form_item SET sort_order = 730 WHERE id = 'fsic-renewal.a10.fdas.location';
UPDATE form_item SET sort_order = 740 WHERE id = 'fsic-renewal.a10.fdas.alarm_panel';
UPDATE form_item SET sort_order = 750 WHERE id = 'fsic-renewal.a10.good_condition';
UPDATE form_item SET sort_order = 760 WHERE id = 'fsic-renewal.a11';
UPDATE form_item SET sort_order = 770 WHERE id = 'fsic-renewal.a11.good_condition';
UPDATE form_item SET sort_order = 780 WHERE id = 'fsic-renewal.a11.need_additional';
UPDATE form_item SET sort_order = 790 WHERE id = 'fsic-renewal.a11.location';
UPDATE form_item SET sort_order = 800 WHERE id = 'fsic-renewal.a11.number';
UPDATE form_item SET sort_order = 810 WHERE id = 'fsic-renewal.a12';
UPDATE form_item SET sort_order = 820 WHERE id = 'fsic-renewal.a12.utilities';
UPDATE form_item SET sort_order = 830 WHERE id = 'fsic-renewal.a12.smoke_control';
UPDATE form_item SET sort_order = 840 WHERE id = 'fsic-renewal.a12.rubbish_chutes';
UPDATE form_item SET sort_order = 850 WHERE id = 'fsic-renewal.a13';
UPDATE form_item SET sort_order = 860 WHERE id = 'fsic-renewal.a13.passive';
UPDATE form_item SET sort_order = 870 WHERE id = 'fsic-renewal.a13.active';
UPDATE form_item SET sort_order = 880 WHERE id = 'fsic-renewal.a13.prev_submitted_date';
UPDATE form_item SET sort_order = 890 WHERE id = 'fsic-renewal.a13.prev_signed_by';
UPDATE form_item SET sort_order = 900 WHERE id = 'fsic-renewal.a13.submitted_date';
UPDATE form_item SET sort_order = 910 WHERE id = 'fsic-renewal.a13.signed_by';
UPDATE form_item SET sort_order = 920 WHERE id = 'fsic-renewal.a13.submitted';
UPDATE form_item SET sort_order = 930 WHERE id = 'fsic-renewal.defects';
UPDATE form_item SET sort_order = 940 WHERE id = 'fsic-renewal.rec.comply';
UPDATE form_item SET sort_order = 950 WHERE id = 'fsic-renewal.rec.secure_clearances';
UPDATE form_item SET sort_order = 960 WHERE id = 'fsic-renewal.rec.clearances';
UPDATE form_item SET sort_order = 970 WHERE id = 'fsic-renewal.rec.pay_fees';
UPDATE form_item SET sort_order = 980 WHERE id = 'fsic-renewal.rec.issue.fsic_business_renewal';
UPDATE form_item SET sort_order = 990 WHERE id = 'fsic-renewal.rec.issue.fsic_peza';
UPDATE form_item SET sort_order = 1000 WHERE id = 'fsic-renewal.rec.issue.notice_to_comply';
UPDATE form_item SET sort_order = 1010 WHERE id = 'fsic-renewal.rec.issue.notice_to_correct';
UPDATE form_item SET sort_order = 1020 WHERE id = 'fsic-renewal.rec.issue.closure_order';
UPDATE form_item SET sort_order = 1030 WHERE id = 'fsic-renewal.rec.issue.abatement_order';
UPDATE form_item SET sort_order = 1040 WHERE id = 'fsic-renewal.rec.issue.closure_nonpayment';
