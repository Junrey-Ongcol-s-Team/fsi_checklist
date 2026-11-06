-- BISAYA: FSIC renewal (BFP-QSF-FSED-069): ang "Nature of inspection conducted" USA RA ang mapili.
-- Kaniadto: upat ka checkbox (BOOL) nga pwede tanan ma-check.
-- Karon:    usa ka pilian (CHOICE) "nature". Kung Re-Inspection, usa sab ra sa NTC / NTCV / Abatement / Closure.
-- Ang daan nga mga checkbox gitago ra (dili gi-delete) aron dili maguba ang mga daan nga record.
-- Ang tubag sa mga draft nga inspection gibalhin ngadto sa bag-ong pilian.
-- Modagan KAUSA RA (seed.069c).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-renewal.nature','fsic-renewal','fsic-renewal.s.nature',NULL,'nature','Nature of inspection conducted (choose one)',15,'CHOICE',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.nature.reinspection_for','fsic-renewal','fsic-renewal.s.nature','fsic-renewal.nature','nature.reinspection_for','Re-inspection for compliance with (choose one)',65,'CHOICE',NULL,NULL,0,NULL,'fsic-renewal.nature','REINSPECTION');

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('fsic-renewal.nature.FSIC_BP','fsic-renewal.nature','FSIC_BP','FSIC for Business Permit',1),
 ('fsic-renewal.nature.FSIC_PEZA','fsic-renewal.nature','FSIC_PEZA','FSIC for PEZA (Annual Inspection)',2),
 ('fsic-renewal.nature.OTHERS','fsic-renewal.nature','OTHERS','Others',3),
 ('fsic-renewal.nature.REINSPECTION','fsic-renewal.nature','REINSPECTION','Re-inspection for compliance',4),
 ('fsic-renewal.nature.reinspection_for.NTC','fsic-renewal.nature.reinspection_for','NTC','NTC',1),
 ('fsic-renewal.nature.reinspection_for.NTCV','fsic-renewal.nature.reinspection_for','NTCV','NTCV',2),
 ('fsic-renewal.nature.reinspection_for.ABATEMENT','fsic-renewal.nature.reinspection_for','ABATEMENT','Abatement',3),
 ('fsic-renewal.nature.reinspection_for.CLOSURE','fsic-renewal.nature.reinspection_for','CLOSURE','Closure',4);

-- "Others (specify)": makita ra kung Others ang gipili
UPDATE form_item SET parent_item_id = 'fsic-renewal.nature', show_if_item_id = 'fsic-renewal.nature', show_if_values = 'OTHERS', sort_order = 55
 WHERE id = 'fsic-renewal.nature.others_specify';

-- Itago ang daan nga mga checkbox (ang '-' dili gyud mahimong tubag, busa kanunay nakatago)
UPDATE form_item SET show_if_item_id = 'fsic-renewal.nature', show_if_values = '-'
 WHERE id IN ('fsic-renewal.nature.fsic_business_permit','fsic-renewal.nature.fsic_peza','fsic-renewal.nature.others','fsic-renewal.nature.reinspection');

-- Ibalhin ang tubag sa mga draft (dili ang COMPLETED, kay naka-lock na kana)
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'fsic-renewal.nature', 'value', 0, m.code, NULL
  FROM inspection_answer a
  JOIN inspection i ON i.id = a.inspection_id
  JOIN (SELECT 'fsic-renewal.nature.fsic_business_permit' AS old_id, 'FSIC_BP' AS code, 1 AS prio
        UNION ALL SELECT 'fsic-renewal.nature.fsic_peza', 'FSIC_PEZA', 2
        UNION ALL SELECT 'fsic-renewal.nature.others', 'OTHERS', 3
        UNION ALL SELECT 'fsic-renewal.nature.reinspection', 'REINSPECTION', 4) m ON m.old_id = a.item_id
 WHERE a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1) AND i.status <> 'COMPLETED'
 ORDER BY m.prio;

INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'fsic-renewal.nature.reinspection_for', 'value', 0, m.code, NULL
  FROM inspection_answer a
  JOIN inspection i ON i.id = a.inspection_id
  JOIN (SELECT 'fsic-renewal.nature.reinspection.ntc' AS old_id, 'NTC' AS code, 1 AS prio
        UNION ALL SELECT 'fsic-renewal.nature.reinspection.ntcv', 'NTCV', 2
        UNION ALL SELECT 'fsic-renewal.nature.reinspection.abatement', 'ABATEMENT', 3
        UNION ALL SELECT 'fsic-renewal.nature.reinspection.closure', 'CLOSURE', 4) m ON m.old_id = a.item_id
 WHERE a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1) AND i.status <> 'COMPLETED'
 ORDER BY m.prio;

DELETE FROM inspection_answer
 WHERE item_id IN ('fsic-renewal.nature.fsic_business_permit','fsic-renewal.nature.fsic_peza','fsic-renewal.nature.others',
                   'fsic-renewal.nature.reinspection','fsic-renewal.nature.reinspection.ntc','fsic-renewal.nature.reinspection.ntcv',
                   'fsic-renewal.nature.reinspection.abatement','fsic-renewal.nature.reinspection.closure')
   AND inspection_id IN (SELECT id FROM inspection WHERE status <> 'COMPLETED');
