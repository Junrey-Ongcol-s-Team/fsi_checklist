-- =====================================================================
-- BISAYA: 075 Conveyance thru Pipelines: RECOMMENDATIONS = USA RA ang mapili
--   For issuance of Conveyance Clearance  /  Notice of Disapproval
--   Kaniadto 2 ka checkbox (pwede duha). Karon usa ka CHOICE.
--   Print (print/075.json): "rec.recommend=ISSUE_CLEARANCE" / "rec.recommend=NOTICE_DISAPPROVAL"
--   = parehas nga kahon sa daan nga checkbox.
--   Daan nga tubag: ang UNA nga naka-check ang ibalhin; ang daan nga checkbox gitago ug ang tubag
--   niini tangtangon (usa ra ang √). LOCKED: dili hilabtan.
-- Modagan KAUSA RA (seed.075d).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values)
SELECT 'conveyance-pipeline.rec.recommend','conveyance-pipeline','conveyance-pipeline.s.findings',NULL,'rec.recommend',
       'Recommendation (choose one)', MIN(sort_order),'CHOICE',NULL,NULL,0,NULL,NULL,NULL
  FROM form_item WHERE id IN ('conveyance-pipeline.rec.issue_clearance','conveyance-pipeline.rec.notice_disapproval');

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('conveyance-pipeline.rec.recommend.ISSUE_CLEARANCE',   'conveyance-pipeline.rec.recommend','ISSUE_CLEARANCE',   'For issuance of Conveyance Clearance',1),
 ('conveyance-pipeline.rec.recommend.NOTICE_DISAPPROVAL','conveyance-pipeline.rec.recommend','NOTICE_DISAPPROVAL','Notice of Disapproval',2);

-- Ibalhin ang daan nga tubag: ang UNA nga naka-check (dili locked)
INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text, value_num)
SELECT a.inspection_id, 'conveyance-pipeline.rec.recommend', 'value', 0, UPPER(SUBSTR(i.key, 5)), NULL
  FROM inspection_answer a
  JOIN form_item i ON i.id = a.item_id
 WHERE i.id IN ('conveyance-pipeline.rec.issue_clearance','conveyance-pipeline.rec.notice_disapproval')
   AND a.slot_key = 'value' AND (a.value_text = '1' OR a.value_num = 1)
   AND a.inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL)
   AND i.sort_order = (SELECT MIN(i2.sort_order)
                         FROM inspection_answer a2 JOIN form_item i2 ON i2.id = a2.item_id
                        WHERE a2.inspection_id = a.inspection_id
                          AND i2.id IN ('conveyance-pipeline.rec.issue_clearance','conveyance-pipeline.rec.notice_disapproval')
                          AND a2.slot_key = 'value' AND (a2.value_text = '1' OR a2.value_num = 1));

-- Tangtangon ang daan nga checkbox nga tubag (dili locked)
DELETE FROM inspection_answer
 WHERE item_id IN ('conveyance-pipeline.rec.issue_clearance','conveyance-pipeline.rec.notice_disapproval')
   AND inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL);

-- Itago ang daan nga 2 ka checkbox
UPDATE form_item SET show_if_item_id = 'conveyance-pipeline.rec.recommend', show_if_values = '-'
 WHERE id IN ('conveyance-pipeline.rec.issue_clearance','conveyance-pipeline.rec.notice_disapproval');
