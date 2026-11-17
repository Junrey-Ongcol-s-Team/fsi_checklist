-- =====================================================================
-- BISAYA: 006 Building Plans: EXPANDERVIEW sa TANANG seksyon
--   I. hangtod VI. (numero 1 - 90): matag numero usa ka expander (FormPageViewModel ExpanderEveryNumber).
--   General information, VII. Recommendations, Fire code fees: usa ka expander matag usa (ExpanderItems).
-- Paagi sa tulo (sama sa 024e): ULOHAN (NONE) nga parehas ang ngalan sa seksyon (dili doble sa screen),
-- dayon ang mga pangutana sa seksyon gibutang ubos niini. Ang tubag ug print dili mausab.
-- Modagan KAUSA RA (seed.006b).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values)
SELECT 'building-plans.general.grp','building-plans',id,NULL,'general.grp',title,-1,'NONE',NULL,NULL,0,NULL,NULL,NULL
  FROM form_section WHERE id = 'building-plans.s.general';

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values)
SELECT 'building-plans.recommendation.grp','building-plans',id,NULL,'recommendation.grp',title,-1,'NONE',NULL,NULL,0,NULL,NULL,NULL
  FROM form_section WHERE id = 'building-plans.s.recommendation';

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values)
SELECT 'building-plans.fees.grp','building-plans',id,NULL,'fees.grp',title,-1,'NONE',NULL,NULL,0,NULL,NULL,NULL
  FROM form_section WHERE id = 'building-plans.s.fees';

UPDATE form_item SET parent_item_id = 'building-plans.general.grp'
 WHERE section_id = 'building-plans.s.general' AND parent_item_id IS NULL AND id <> 'building-plans.general.grp';

UPDATE form_item SET parent_item_id = 'building-plans.recommendation.grp'
 WHERE section_id = 'building-plans.s.recommendation' AND parent_item_id IS NULL AND id <> 'building-plans.recommendation.grp';

UPDATE form_item SET parent_item_id = 'building-plans.fees.grp'
 WHERE section_id = 'building-plans.s.fees' AND parent_item_id IS NULL AND id <> 'building-plans.fees.grp';
