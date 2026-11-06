-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (BFP-QSF-FSED-061). Usa ra ang mapili, ug mogawas ra ang angay.
--   1. 2. NATURE OF INSPECTION CONDUCTED: USA RA ang mapili (kaniadto 5 ka checkbox).
--      Kung Verification: usa ra sa NTC / NTCV / Abatement / Closure. Kung Others: "Others (specify)".
--   2. IV. GENERAL INFORMATION: [ ] FSIC for Occupancy / [ ] FSIC for New Business Permit ... (usa ra).
--      FSIC for Occupancy  -> FSEC No., Building Permit ug ang ilang Date Issued ra ang mogawas.
--      New Business Permit -> FSIC No. (Latest), Certificate of Fire Drill, Business Permit No.,
--                             Fire Insurance Policy No. ug ang ilang Date Issued ra ang mogawas.
--   3. GENERAL OCCUPANCY CLASSIFICATION: "Residential (sub classification)" ug "Special Structure"
--      mogawas ra kung mao ang gipili.
--   (Ang Construction type, Walls/Ceiling ug Floor finish usa na ra ang mapili kaniadto pa.)
-- Ang daan nga mga checkbox gitago ra (dili gi-delete). Ang daan nga tubag gibalhin sa bag-ong pilian.
-- Ang print (061.json) gi-update aron mohaum sa bag-ong mga key. Modagan KAUSA RA (seed.061b).
-- =====================================================================

-- ---------- 1. Nature of inspection: usa ra ----------
INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-new-occupancy.nature','fsic-new-occupancy','fsic-new-occupancy.s.nature',NULL,'nature','2. Nature of inspection conducted (check one)',2,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-new-occupancy.nature.verification_for','fsic-new-occupancy','fsic-new-occupancy.s.nature','fsic-new-occupancy.nature','nature.verification_for','Verification inspection for compliance with (check one)',6,'CHOICE',NULL,NULL,0,NULL,'fsic-new-occupancy.nature','VERIFICATION');

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('fsic-new-occupancy.nature.FSIC_OCCUPANCY','fsic-new-occupancy.nature','FSIC_OCCUPANCY','FSIC for Certificate for Occupancy',1),
 ('fsic-new-occupancy.nature.FSIC_BP_NEW','fsic-new-occupancy.nature','FSIC_BP_NEW','FSIC for Business Permit (New)',2),
 ('fsic-new-occupancy.nature.FSIC_PEZA_NEW','fsic-new-occupancy.nature','FSIC_PEZA_NEW','FSIC for Certificate of Annual Inspection (PEZA) (New)',3),
 ('fsic-new-occupancy.nature.VERIFICATION','fsic-new-occupancy.nature','VERIFICATION','Verification Inspection for Compliance',4),
 ('fsic-new-occupancy.nature.OTHERS','fsic-new-occupancy.nature','OTHERS','Others (Specify)',5),
 ('fsic-new-occupancy.nature.verification_for.NTC','fsic-new-occupancy.nature.verification_for','NTC','NTC',1),
 ('fsic-new-occupancy.nature.verification_for.NTCV','fsic-new-occupancy.nature.verification_for','NTCV','NTCV',2),
 ('fsic-new-occupancy.nature.verification_for.ABATEMENT','fsic-new-occupancy.nature.verification_for','ABATEMENT','Abatement',3),
 ('fsic-new-occupancy.nature.verification_for.CLOSURE','fsic-new-occupancy.nature.verification_for','CLOSURE','Closure',4);

-- "Others (specify)": ubos sa Nature, mogawas ra kung Others
UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.nature', show_if_item_id = 'fsic-new-occupancy.nature', show_if_values = 'OTHERS', sort_order = 11
 WHERE id = 'fsic-new-occupancy.nature.others_specify';

-- Itago ang daan nga mga checkbox ('-' dili gyud mahimong tubag, busa kanunay nakatago)
UPDATE form_item SET show_if_item_id = 'fsic-new-occupancy.nature', show_if_values = '-'
 WHERE id IN ('fsic-new-occupancy.nature.fsic_occupancy','fsic-new-occupancy.nature.fsic_business_new',
              'fsic-new-occupancy.nature.fsic_peza_new','fsic-new-occupancy.nature.verification','fsic-new-occupancy.nature.others');

-- ---------- 2. General information: FSIC for Occupancy / New Business Permit ----------
INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-new-occupancy.fsic_for','fsic-new-occupancy','fsic-new-occupancy.s.general',NULL,'fsic_for','FSIC for (check one)',17,'CHOICE',NULL,NULL,0,NULL,NULL,NULL);

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('fsic-new-occupancy.fsic_for.OCCUPANCY','fsic-new-occupancy.fsic_for','OCCUPANCY','FSIC for Occupancy',1),
 ('fsic-new-occupancy.fsic_for.NEW_BP','fsic-new-occupancy.fsic_for','NEW_BP','FSIC for New Business Permit / Certificate of Annual Inspection / Others',2);

UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.fsic_for', show_if_item_id = 'fsic-new-occupancy.fsic_for', show_if_values = 'OCCUPANCY'
 WHERE id IN ('fsic-new-occupancy.fsec_no','fsic-new-occupancy.fsec_date','fsic-new-occupancy.building_permit_no','fsic-new-occupancy.building_permit_date');

UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.fsic_for', show_if_item_id = 'fsic-new-occupancy.fsic_for', show_if_values = 'NEW_BP'
 WHERE id IN ('fsic-new-occupancy.fsic_no','fsic-new-occupancy.fsic_date','fsic-new-occupancy.fire_drill_no','fsic-new-occupancy.fire_drill_date',
              'fsic-new-occupancy.business_permit_no','fsic-new-occupancy.business_permit_date',
              'fsic-new-occupancy.insurance_policy_no','fsic-new-occupancy.insurance_policy_date');

-- ---------- 3. General occupancy: Residential / Special structure mogawas ra kung gipili ----------
UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.occupancy_class', show_if_item_id = 'fsic-new-occupancy.occupancy_class', show_if_values = 'RESIDENTIAL'
 WHERE id = 'fsic-new-occupancy.residential_subclass';
UPDATE form_item SET parent_item_id = 'fsic-new-occupancy.occupancy_class', show_if_item_id = 'fsic-new-occupancy.occupancy_class', show_if_values = 'SPECIAL'
 WHERE id = 'fsic-new-occupancy.special_structure';

-- ---------- 4. Ibalhin ang daan nga tubag (apil ang COMPLETED, para dili mausab ang printout) ----------
DROP TRIGGER IF EXISTS trg_answer_lock_ins;

-- Nature: ang una nga naka-check (sunod sa han-ay sa form)
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'fsic-new-occupancy.nature', 'value', 0, m.code, NULL
  FROM inspection_answer a
  JOIN (SELECT 'fsic-new-occupancy.nature.fsic_occupancy' AS old_id, 'FSIC_OCCUPANCY' AS code, 1 AS prio
        UNION ALL SELECT 'fsic-new-occupancy.nature.fsic_business_new', 'FSIC_BP_NEW', 2
        UNION ALL SELECT 'fsic-new-occupancy.nature.fsic_peza_new', 'FSIC_PEZA_NEW', 3
        UNION ALL SELECT 'fsic-new-occupancy.nature.verification', 'VERIFICATION', 4
        UNION ALL SELECT 'fsic-new-occupancy.nature.others', 'OTHERS', 5) m ON m.old_id = a.item_id
 WHERE a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1)
 ORDER BY m.prio;

INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'fsic-new-occupancy.nature.verification_for', 'value', 0, m.code, NULL
  FROM inspection_answer a
  JOIN (SELECT 'fsic-new-occupancy.nature.verification.ntc' AS old_id, 'NTC' AS code, 1 AS prio
        UNION ALL SELECT 'fsic-new-occupancy.nature.verification.ntcv', 'NTCV', 2
        UNION ALL SELECT 'fsic-new-occupancy.nature.verification.abatement', 'ABATEMENT', 3
        UNION ALL SELECT 'fsic-new-occupancy.nature.verification.closure', 'CLOSURE', 4) m ON m.old_id = a.item_id
 WHERE a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1)
 ORDER BY m.prio;

-- FSIC for: kung naay gisulat sa FSEC/Building Permit = Occupancy; kung sa FSIC No./Fire drill/... = New Business Permit
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT DISTINCT a.inspection_id, 'fsic-new-occupancy.fsic_for', 'value', 0, 'OCCUPANCY', NULL
  FROM inspection_answer a
 WHERE a.item_id IN ('fsic-new-occupancy.fsec_no','fsic-new-occupancy.fsec_date','fsic-new-occupancy.building_permit_no','fsic-new-occupancy.building_permit_date')
   AND NULLIF(TRIM(a.value_text), '') IS NOT NULL;

INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT DISTINCT a.inspection_id, 'fsic-new-occupancy.fsic_for', 'value', 0, 'NEW_BP', NULL
  FROM inspection_answer a
 WHERE a.item_id IN ('fsic-new-occupancy.fsic_no','fsic-new-occupancy.fsic_date','fsic-new-occupancy.fire_drill_no','fsic-new-occupancy.fire_drill_date',
                     'fsic-new-occupancy.business_permit_no','fsic-new-occupancy.business_permit_date',
                     'fsic-new-occupancy.insurance_policy_no','fsic-new-occupancy.insurance_policy_date')
   AND NULLIF(TRIM(a.value_text), '') IS NOT NULL;

-- Kung walay gisulat: sunod sa Nature (Occupancy -> Occupancy; Business Permit / PEZA / Others -> New Business Permit)
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'fsic-new-occupancy.fsic_for', 'value', 0,
       CASE a.value_text WHEN 'FSIC_OCCUPANCY' THEN 'OCCUPANCY' ELSE 'NEW_BP' END, NULL
  FROM inspection_answer a
 WHERE a.item_id = 'fsic-new-occupancy.nature' AND a.value_text IN ('FSIC_OCCUPANCY','FSIC_BP_NEW','FSIC_PEZA_NEW','OTHERS');

CREATE TRIGGER trg_answer_lock_ins BEFORE INSERT ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = NEW.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

-- Papason ang daan nga checkbox nga tubag sa mga draft ra (ang COMPLETED naka-lock; nakatago na sila)
DELETE FROM inspection_answer
 WHERE item_id IN ('fsic-new-occupancy.nature.fsic_occupancy','fsic-new-occupancy.nature.fsic_business_new','fsic-new-occupancy.nature.fsic_peza_new',
                   'fsic-new-occupancy.nature.verification','fsic-new-occupancy.nature.verification.ntc','fsic-new-occupancy.nature.verification.ntcv',
                   'fsic-new-occupancy.nature.verification.abatement','fsic-new-occupancy.nature.verification.closure','fsic-new-occupancy.nature.others')
   AND inspection_id IN (SELECT id FROM inspection WHERE status <> 'COMPLETED');
