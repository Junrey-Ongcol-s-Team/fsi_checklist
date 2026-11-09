-- =====================================================================
-- BISAYA: 024 Small Business: [ ] For issuance of -> USA RA ang mapili
--   Bag-o nga checkbox "For issuance of". Kung i-check: mogawas ang pilian (usa ra):
--     Notice to Comply / Notice to Correct Violation / Closure Order /
--     Abatement Order with Administrative Fine / Closure Order for the non-payment of Administrative Fine /
--     Notice of Disapproval, if Certificate of Occupancy is applied
--   Print (print/024.json): "rec.for_issuance" = ang kahon sa "For issuance of";
--     "rec.issue_of=NOTICE_TO_COMPLY" ug uban pa = parehas nga kahon sa daan nga checkbox.
--   Daan nga tubag: kung naay naka-check -> For issuance of = check, ug ang UNA nga naka-check ang pilian.
--   Ang daan nga 6 ka checkbox gitago ug ang tubag niini tangtangon (usa ra ang √). LOCKED: dili hilabtan.
-- Modagan KAUSA RA (seed.024i).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('small-business.rec.for_issuance','small-business','small-business.s.findings',NULL,'rec.for_issuance','For issuance of',93,'BOOL',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.rec.issue_of','small-business','small-business.s.findings','small-business.rec.for_issuance','rec.issue_of','For issuance of (choose one)',94,'CHOICE',NULL,NULL,1,NULL,'small-business.rec.for_issuance','1');

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('small-business.rec.issue_of.NOTICE_TO_COMPLY',  'small-business.rec.issue_of','NOTICE_TO_COMPLY',  'Notice to Comply',1),
 ('small-business.rec.issue_of.NOTICE_TO_CORRECT', 'small-business.rec.issue_of','NOTICE_TO_CORRECT', 'Notice to Correct Violation',2),
 ('small-business.rec.issue_of.CLOSURE_ORDER',     'small-business.rec.issue_of','CLOSURE_ORDER',     'Closure Order',3),
 ('small-business.rec.issue_of.ABATEMENT_FINE',    'small-business.rec.issue_of','ABATEMENT_FINE',    'Abatement Order with Administrative Fine',4),
 ('small-business.rec.issue_of.CLOSURE_NONPAYMENT','small-business.rec.issue_of','CLOSURE_NONPAYMENT','Closure Order for the non-payment of Administrative Fine',5),
 ('small-business.rec.issue_of.NOD',               'small-business.rec.issue_of','NOD',               'Notice of Disapproval, if Certificate of Occupancy is applied',6);

-- Daan nga tubag (dili locked): naay naka-check -> For issuance of = check
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT DISTINCT a.inspection_id, 'small-business.rec.for_issuance', 'value', 0, '1', 1
  FROM inspection_answer a
 WHERE a.item_id IN ('small-business.rec.notice_to_comply','small-business.rec.notice_to_correct','small-business.rec.closure_order',
                     'small-business.rec.abatement_fine','small-business.rec.closure_nonpayment','small-business.rec.nod')
   AND a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1)
   AND a.inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL);

-- ... ug ang UNA nga naka-check ang pilian
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'small-business.rec.issue_of', 'value', 0, UPPER(SUBSTR(i.key, 5)), NULL
  FROM inspection_answer a
  JOIN form_item i ON i.id = a.item_id
 WHERE i.id IN ('small-business.rec.notice_to_comply','small-business.rec.notice_to_correct','small-business.rec.closure_order',
                'small-business.rec.abatement_fine','small-business.rec.closure_nonpayment','small-business.rec.nod')
   AND a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1)
   AND a.inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL)
   AND i.sort_order = (SELECT MIN(i2.sort_order)
                         FROM inspection_answer a2 JOIN form_item i2 ON i2.id = a2.item_id
                        WHERE a2.inspection_id = a.inspection_id
                          AND i2.id IN ('small-business.rec.notice_to_comply','small-business.rec.notice_to_correct','small-business.rec.closure_order',
                                        'small-business.rec.abatement_fine','small-business.rec.closure_nonpayment','small-business.rec.nod')
                          AND a2.slot_key = 'value' AND (a2.value_text = '1' OR a2.value_num = 1));

-- Tangtangon ang daan nga checkbox nga tubag (dili locked)
DELETE FROM inspection_answer
 WHERE item_id IN ('small-business.rec.notice_to_comply','small-business.rec.notice_to_correct','small-business.rec.closure_order',
                   'small-business.rec.abatement_fine','small-business.rec.closure_nonpayment','small-business.rec.nod')
   AND inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL);

-- Itago ang daan nga 6 ka checkbox
UPDATE form_item SET show_if_item_id = 'small-business.rec.for_issuance', show_if_values = '-'
 WHERE id IN ('small-business.rec.notice_to_comply','small-business.rec.notice_to_correct','small-business.rec.closure_order',
              'small-business.rec.abatement_fine','small-business.rec.closure_nonpayment','small-business.rec.nod');
