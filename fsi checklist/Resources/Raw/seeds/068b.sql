-- =====================================================================
-- BISAYA: Inspection During Construction (068) GENERAL INFORMATION = ExpanderView
--   Ang tanang pangutana sa General information gibutang ubos sa usa ka ulohan nga "General information"
--   (expander nga naay circle progress ug Done).
--   "Other % completion" mogawas ra kung ang Stage of inspection = "At other % completion".
--   Kinahanglan sulatan ang tanan (para tarong ang circle), ug ang Other % kung mao ang gipili.
-- Modagan KAUSA RA (seed.068b).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('during-construction.general','during-construction','during-construction.s.general',NULL,'general','General information',0,'NONE',NULL,NULL,0,NULL,NULL,NULL);

-- Tanan sa General information: ubos sa ulohan
UPDATE form_item SET parent_item_id = 'during-construction.general', is_required = 1
 WHERE id IN ('during-construction.completion_stage','during-construction.io_date_issued',
              'during-construction.building_plan_checklist_no','during-construction.fsec_no',
              'during-construction.fsec_date','during-construction.occupancy_classification');

-- Other % completion: ubos sa Stage of inspection, mogawas ra kung OTHER
UPDATE form_item SET parent_item_id = 'during-construction.completion_stage',
       show_if_item_id = 'during-construction.completion_stage', show_if_values = 'OTHER', is_required = 1
 WHERE id = 'during-construction.completion_pct';
