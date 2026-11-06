-- =====================================================================
-- BISAYA: FSIC renewal (069), Section V. HAN-AY SAMA SA FORM (matag item, 1 hangtod 13):
--   1. ang mga kahon sa wala (pananglitan Walls/Ceiling Interior Finish, Floor Interior Finish)
--   2. Previous After Inspection Report: Compliant [ ] Yes, dayon FSIC No., Dated ...
--   3. Present/Actual Inspection: Compliant [ ] Yes [ ] No
--   4. [ ] No changes from previous inspection / [ ] With changes
--        -> ubos sa "With changes" (naka-indent): Renovation, Repair, Additional ...
--           mogawas RA kung "With changes" ang gipili.
-- Kaniadto ang Compliant ug No changes kay COLUMN sa ulohan sa item (mao nga nauna sila).
-- Karon tagsa-tagsa na sila ka pangutana aron mahan-ay. Pareho ra ang key sa print
-- (pananglitan a01.compliant=YES), so ang 069.json wala giusab.
-- Ang daan nga tubag gibalhin. Modagan KAUSA RA (seed.069f).
-- =====================================================================

-- 1. Bag-ong mga pangutana (Compliant sa Previous, Compliant sa Present, No changes / With changes)
INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-renewal.a01.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a01','a01.prev_compliant','Previous After Inspection Report: Compliant (Yes)',270,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a01.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a01','a01.compliant','Present/Actual Inspection: Compliant (Yes / No)',300,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a01.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a01','a01.change_status','Present/Actual Inspection: No changes / With changes',310,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a02.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a02','a02.prev_compliant','Previous After Inspection Report: Compliant (Yes)',360,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a02.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a02','a02.compliant','Present/Actual Inspection: Compliant (Yes / No)',380,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a02.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a02','a02.change_status','Present/Actual Inspection: No changes / With changes',390,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a03.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a03','a03.prev_compliant','Previous After Inspection Report: Compliant (Yes)',450,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a03.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a03','a03.compliant','Present/Actual Inspection: Compliant (Yes / No)',470,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a03.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a03','a03.change_status','Present/Actual Inspection: No changes / With changes',480,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a04.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a04','a04.prev_compliant','Previous After Inspection Report: Compliant (Yes)',510,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a04.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a04','a04.compliant','Present/Actual Inspection: Compliant (Yes / No)',520,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a04.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a04','a04.change_status','Present/Actual Inspection: No changes / With changes',530,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a05.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a05','a05.prev_compliant','Previous After Inspection Report: Compliant (Yes)',590,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a05.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a05','a05.compliant','Present/Actual Inspection: Compliant (Yes / No)',600,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a05.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a05','a05.change_status','Present/Actual Inspection: No changes / With changes',610,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a06a.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a06a','a06a.prev_compliant','Previous After Inspection Report: Compliant (Yes)',670,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a06a.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a06a','a06a.compliant','Present/Actual Inspection: Compliant (Yes / No)',680,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a06a.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a06a','a06a.change_status','Present/Actual Inspection: No changes / With changes',690,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a06b.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a06b','a06b.prev_compliant','Previous After Inspection Report: Compliant (Yes)',720,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a06b.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a06b','a06b.compliant','Present/Actual Inspection: Compliant (Yes / No)',730,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a06b.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a06b','a06b.change_status','Present/Actual Inspection: No changes / With changes',740,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a07.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a07','a07.prev_compliant','Previous After Inspection Report: Compliant (Yes)',770,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a07.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a07','a07.compliant','Present/Actual Inspection: Compliant (Yes / No)',780,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a07.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a07','a07.change_status','Present/Actual Inspection: No changes / With changes',790,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a08.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a08','a08.prev_compliant','Previous After Inspection Report: Compliant (Yes)',870,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a08.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a08','a08.compliant','Present/Actual Inspection: Compliant (Yes / No)',880,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a08.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a08','a08.change_status','Present/Actual Inspection: No changes / With changes',890,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a09.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a09','a09.prev_compliant','Previous After Inspection Report: Compliant (Yes)',980,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a09.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a09','a09.compliant','Present/Actual Inspection: Compliant (Yes / No)',990,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a09.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a09','a09.change_status','Present/Actual Inspection: No changes / With changes',1000,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10','a10.prev_compliant','Previous After Inspection Report: Compliant (Yes)',1100,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10','a10.compliant','Present/Actual Inspection: Compliant (Yes / No)',1110,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10','a10.change_status','Present/Actual Inspection: No changes / With changes',1120,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a11.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a11','a11.prev_compliant','Previous After Inspection Report: Compliant (Yes)',1160,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a11.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a11','a11.compliant','Present/Actual Inspection: Compliant (Yes / No)',1170,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a11.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a11','a11.change_status','Present/Actual Inspection: No changes / With changes',1180,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a12.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a12','a12.prev_compliant','Previous After Inspection Report: Compliant (Yes)',1270,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a12.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a12','a12.compliant','Present/Actual Inspection: Compliant (Yes / No)',1280,'YES_NO',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a12.change_status','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a12','a12.change_status','Present/Actual Inspection: No changes / With changes',1290,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a13.prev_compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a13','a13.prev_compliant','Previous After Inspection Report: Compliant (Yes)',1340,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a13.compliant','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a13','a13.compliant','Present/Actual Inspection: Compliant (Yes / No)',1370,'YES_NO',NULL,NULL,0,NULL,NULL,NULL);

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('fsic-renewal.a01.change_status.NO','fsic-renewal.a01.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a01.change_status.WITH','fsic-renewal.a01.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a02.change_status.NO','fsic-renewal.a02.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a02.change_status.WITH','fsic-renewal.a02.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a03.change_status.NO','fsic-renewal.a03.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a03.change_status.WITH','fsic-renewal.a03.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a04.change_status.NO','fsic-renewal.a04.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a04.change_status.WITH','fsic-renewal.a04.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a05.change_status.NO','fsic-renewal.a05.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a05.change_status.WITH','fsic-renewal.a05.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a06a.change_status.NO','fsic-renewal.a06a.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a06a.change_status.WITH','fsic-renewal.a06a.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a06b.change_status.NO','fsic-renewal.a06b.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a06b.change_status.WITH','fsic-renewal.a06b.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a07.change_status.NO','fsic-renewal.a07.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a07.change_status.WITH','fsic-renewal.a07.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a08.change_status.NO','fsic-renewal.a08.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a08.change_status.WITH','fsic-renewal.a08.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a09.change_status.NO','fsic-renewal.a09.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a09.change_status.WITH','fsic-renewal.a09.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a10.change_status.NO','fsic-renewal.a10.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a10.change_status.WITH','fsic-renewal.a10.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a11.change_status.NO','fsic-renewal.a11.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a11.change_status.WITH','fsic-renewal.a11.change_status','WITH_CHANGES','With changes',2),
 ('fsic-renewal.a12.change_status.NO','fsic-renewal.a12.change_status','NO_CHANGES','No changes from previous inspection',1),
 ('fsic-renewal.a12.change_status.WITH','fsic-renewal.a12.change_status','WITH_CHANGES','With changes',2);

-- 2. Han-ay (sort_order), ug ang Renovation/Repair/... ubos na sa "With changes"
UPDATE form_item SET column_set_id = NULL, sort_order = 240 WHERE id = 'fsic-renewal.a01';
UPDATE form_item SET sort_order = 250 WHERE id = 'fsic-renewal.a01.walls_ceiling';
UPDATE form_item SET sort_order = 260 WHERE id = 'fsic-renewal.a01.floor_finish';
UPDATE form_item SET sort_order = 280 WHERE id = 'fsic-renewal.a01.prev_fsic_no';
UPDATE form_item SET sort_order = 290 WHERE id = 'fsic-renewal.a01.prev_fsic_date';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a01.change_status', show_if_item_id = 'fsic-renewal.a01.change_status', show_if_values = 'WITH_CHANGES', sort_order = 320 WHERE id = 'fsic-renewal.a01.renovation';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a01.change_status', show_if_item_id = 'fsic-renewal.a01.change_status', show_if_values = 'WITH_CHANGES', sort_order = 330 WHERE id = 'fsic-renewal.a01.repair';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a01.change_status', show_if_item_id = 'fsic-renewal.a01.change_status', show_if_values = 'WITH_CHANGES', sort_order = 340 WHERE id = 'fsic-renewal.a01.additional';
UPDATE form_item SET column_set_id = NULL, sort_order = 350 WHERE id = 'fsic-renewal.a02';
UPDATE form_item SET sort_order = 370 WHERE id = 'fsic-renewal.a02.prev_floors';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a02.change_status', show_if_item_id = 'fsic-renewal.a02.change_status', show_if_values = 'WITH_CHANGES', sort_order = 400 WHERE id = 'fsic-renewal.a02.renovation';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a02.change_status', show_if_item_id = 'fsic-renewal.a02.change_status', show_if_values = 'WITH_CHANGES', sort_order = 410 WHERE id = 'fsic-renewal.a02.repair';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a02.change_status', show_if_item_id = 'fsic-renewal.a02.change_status', show_if_values = 'WITH_CHANGES', sort_order = 420 WHERE id = 'fsic-renewal.a02.additional';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a02.change_status', show_if_item_id = 'fsic-renewal.a02.change_status', show_if_values = 'WITH_CHANGES', sort_order = 430 WHERE id = 'fsic-renewal.a02.particular_floor';
UPDATE form_item SET column_set_id = NULL, sort_order = 440 WHERE id = 'fsic-renewal.a03';
UPDATE form_item SET sort_order = 460 WHERE id = 'fsic-renewal.a03.prev_occupancy';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a03.change_status', show_if_item_id = 'fsic-renewal.a03.change_status', show_if_values = 'WITH_CHANGES', sort_order = 490 WHERE id = 'fsic-renewal.a03.change_of_occupancy';
UPDATE form_item SET column_set_id = NULL, sort_order = 500 WHERE id = 'fsic-renewal.a04';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a04.change_status', show_if_item_id = 'fsic-renewal.a04.change_status', show_if_values = 'WITH_CHANGES', sort_order = 540 WHERE id = 'fsic-renewal.a04.renovation';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a04.change_status', show_if_item_id = 'fsic-renewal.a04.change_status', show_if_values = 'WITH_CHANGES', sort_order = 550 WHERE id = 'fsic-renewal.a04.repair';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a04.change_status', show_if_item_id = 'fsic-renewal.a04.change_status', show_if_values = 'WITH_CHANGES', sort_order = 560 WHERE id = 'fsic-renewal.a04.additional';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a04.change_status', show_if_item_id = 'fsic-renewal.a04.change_status', show_if_values = 'WITH_CHANGES', sort_order = 570 WHERE id = 'fsic-renewal.a04.particular_changes';
UPDATE form_item SET column_set_id = NULL, sort_order = 580 WHERE id = 'fsic-renewal.a05';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a05.change_status', show_if_item_id = 'fsic-renewal.a05.change_status', show_if_values = 'WITH_CHANGES', sort_order = 620 WHERE id = 'fsic-renewal.a05.renovation';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a05.change_status', show_if_item_id = 'fsic-renewal.a05.change_status', show_if_values = 'WITH_CHANGES', sort_order = 630 WHERE id = 'fsic-renewal.a05.repair';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a05.change_status', show_if_item_id = 'fsic-renewal.a05.change_status', show_if_values = 'WITH_CHANGES', sort_order = 640 WHERE id = 'fsic-renewal.a05.additional';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a05.change_status', show_if_item_id = 'fsic-renewal.a05.change_status', show_if_values = 'WITH_CHANGES', sort_order = 650 WHERE id = 'fsic-renewal.a05.particular_violation';
UPDATE form_item SET column_set_id = NULL, sort_order = 660 WHERE id = 'fsic-renewal.a06a';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a06a.change_status', show_if_item_id = 'fsic-renewal.a06a.change_status', show_if_values = 'WITH_CHANGES', sort_order = 700 WHERE id = 'fsic-renewal.a06a.particulars';
UPDATE form_item SET column_set_id = NULL, sort_order = 710 WHERE id = 'fsic-renewal.a06b';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a06b.change_status', show_if_item_id = 'fsic-renewal.a06b.change_status', show_if_values = 'WITH_CHANGES', sort_order = 750 WHERE id = 'fsic-renewal.a06b.particulars';
UPDATE form_item SET column_set_id = NULL, sort_order = 760 WHERE id = 'fsic-renewal.a07';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a07.change_status', show_if_item_id = 'fsic-renewal.a07.change_status', show_if_values = 'WITH_CHANGES', sort_order = 800 WHERE id = 'fsic-renewal.a07.particulars';
UPDATE form_item SET column_set_id = NULL, sort_order = 810 WHERE id = 'fsic-renewal.a08';
UPDATE form_item SET sort_order = 820 WHERE id = 'fsic-renewal.a08.hazard_classification';
UPDATE form_item SET sort_order = 830 WHERE id = 'fsic-renewal.a08.other_flammable';
UPDATE form_item SET sort_order = 840 WHERE id = 'fsic-renewal.a08.misc_hazard';
UPDATE form_item SET sort_order = 850 WHERE id = 'fsic-renewal.a08.no_smoking_signages';
UPDATE form_item SET sort_order = 860 WHERE id = 'fsic-renewal.a08.fuel_storage';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a08.change_status', show_if_item_id = 'fsic-renewal.a08.change_status', show_if_values = 'WITH_CHANGES', sort_order = 900 WHERE id = 'fsic-renewal.a08.maq';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a08.change_status', show_if_item_id = 'fsic-renewal.a08.change_status', show_if_values = 'WITH_CHANGES', sort_order = 910 WHERE id = 'fsic-renewal.a08.no_additional_protection';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a08.change_status', show_if_item_id = 'fsic-renewal.a08.change_status', show_if_values = 'WITH_CHANGES', sort_order = 920 WHERE id = 'fsic-renewal.a08.no_msds';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a08.change_status', show_if_item_id = 'fsic-renewal.a08.change_status', show_if_values = 'WITH_CHANGES', sort_order = 930 WHERE id = 'fsic-renewal.a08.no_proper_storage';
UPDATE form_item SET column_set_id = NULL, sort_order = 940 WHERE id = 'fsic-renewal.a09';
UPDATE form_item SET sort_order = 950 WHERE id = 'fsic-renewal.a09.maintenance';
UPDATE form_item SET sort_order = 960 WHERE id = 'fsic-renewal.a09.storage';
UPDATE form_item SET sort_order = 970 WHERE id = 'fsic-renewal.a09.waste_disposal';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a09.change_status', show_if_item_id = 'fsic-renewal.a09.change_status', show_if_values = 'WITH_CHANGES', sort_order = 1010 WHERE id = 'fsic-renewal.a09.particulars';
UPDATE form_item SET column_set_id = NULL, sort_order = 1020 WHERE id = 'fsic-renewal.a10';
UPDATE form_item SET sort_order = 1030 WHERE id = 'fsic-renewal.a10.automatic';
UPDATE form_item SET sort_order = 1040 WHERE id = 'fsic-renewal.a10.standpipe';
UPDATE form_item SET sort_order = 1050 WHERE id = 'fsic-renewal.a10.fire_pump';
UPDATE form_item SET sort_order = 1060 WHERE id = 'fsic-renewal.a10.fdas';
UPDATE form_item SET sort_order = 1070 WHERE id = 'fsic-renewal.a10.fdas.alarm_facilities';
UPDATE form_item SET sort_order = 1080 WHERE id = 'fsic-renewal.a10.fdas.location';
UPDATE form_item SET sort_order = 1090 WHERE id = 'fsic-renewal.a10.fdas.alarm_panel';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a10.change_status', show_if_item_id = 'fsic-renewal.a10.change_status', show_if_values = 'WITH_CHANGES', sort_order = 1130 WHERE id = 'fsic-renewal.a10.good_condition';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a10.change_status', show_if_item_id = 'fsic-renewal.a10.change_status', show_if_values = 'WITH_CHANGES', sort_order = 1140 WHERE id = 'fsic-renewal.a10.particulars';
UPDATE form_item SET column_set_id = NULL, sort_order = 1150 WHERE id = 'fsic-renewal.a11';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a11.change_status', show_if_item_id = 'fsic-renewal.a11.change_status', show_if_values = 'WITH_CHANGES', sort_order = 1190 WHERE id = 'fsic-renewal.a11.good_condition';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a11.change_status', show_if_item_id = 'fsic-renewal.a11.change_status', show_if_values = 'WITH_CHANGES', sort_order = 1200 WHERE id = 'fsic-renewal.a11.need_additional';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a11.change_status', show_if_item_id = 'fsic-renewal.a11.change_status', show_if_values = 'WITH_CHANGES', sort_order = 1210 WHERE id = 'fsic-renewal.a11.location';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a11.change_status', show_if_item_id = 'fsic-renewal.a11.change_status', show_if_values = 'WITH_CHANGES', sort_order = 1220 WHERE id = 'fsic-renewal.a11.number';
UPDATE form_item SET column_set_id = NULL, sort_order = 1230 WHERE id = 'fsic-renewal.a12';
UPDATE form_item SET sort_order = 1240 WHERE id = 'fsic-renewal.a12.utilities';
UPDATE form_item SET sort_order = 1250 WHERE id = 'fsic-renewal.a12.smoke_control';
UPDATE form_item SET sort_order = 1260 WHERE id = 'fsic-renewal.a12.rubbish_chutes';
UPDATE form_item SET parent_item_id = 'fsic-renewal.a12.change_status', show_if_item_id = 'fsic-renewal.a12.change_status', show_if_values = 'WITH_CHANGES', sort_order = 1300 WHERE id = 'fsic-renewal.a12.particulars';
UPDATE form_item SET column_set_id = NULL, sort_order = 1310 WHERE id = 'fsic-renewal.a13';
UPDATE form_item SET sort_order = 1320 WHERE id = 'fsic-renewal.a13.passive';
UPDATE form_item SET sort_order = 1330 WHERE id = 'fsic-renewal.a13.active';
UPDATE form_item SET sort_order = 1350 WHERE id = 'fsic-renewal.a13.prev_submitted_date';
UPDATE form_item SET sort_order = 1360 WHERE id = 'fsic-renewal.a13.prev_signed_by';
UPDATE form_item SET sort_order = 1380 WHERE id = 'fsic-renewal.a13.submitted_date';
UPDATE form_item SET sort_order = 1390 WHERE id = 'fsic-renewal.a13.signed_by';
UPDATE form_item SET sort_order = 1400 WHERE id = 'fsic-renewal.a13.submitted';

-- 3. Ibalhin ang daan nga tubag: (a01, column compliant) -> (a01.compliant, value) ug uban pa.
--    Temporaryo nga gitangtang ang lock trigger aron maapil ang COMPLETED, dayon gibalik.
DROP TRIGGER IF EXISTS trg_answer_lock_upd;

UPDATE OR IGNORE inspection_answer
   SET item_id = item_id || '.' || slot_key, slot_key = 'value'
 WHERE item_id IN ('fsic-renewal.a01','fsic-renewal.a02','fsic-renewal.a03','fsic-renewal.a04','fsic-renewal.a05','fsic-renewal.a06a','fsic-renewal.a06b','fsic-renewal.a07','fsic-renewal.a08','fsic-renewal.a09','fsic-renewal.a10','fsic-renewal.a11','fsic-renewal.a12','fsic-renewal.a13')
   AND slot_key IN ('prev_compliant','compliant','change_status')
   AND EXISTS (SELECT 1 FROM form_item f WHERE f.id = inspection_answer.item_id || '.' || inspection_answer.slot_key);

CREATE TRIGGER trg_answer_lock_upd BEFORE UPDATE ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = OLD.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;
