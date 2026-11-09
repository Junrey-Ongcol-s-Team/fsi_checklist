-- =====================================================================
-- BISAYA: 024 Small Business: EXPANDERVIEW sa
--   II. Nature of inspection conducted      (usa ka expander)
--   IV. Other information                   (usa ka expander)
--   V.  Means of egress -> A. Exit access ug B. Exits (duha ka expander)
--   VI. Signs, lighting, and exits signage  (usa ka expander)
--   VII. Hazard                             (usa ka expander)
--   VIII. Fire protection                   (usa ka expander)
-- Paagi (sama sa 068 General information): naghimo og ULOHAN (NONE) matag usa,
-- dayon ang mga pangutana sa seksyon gibutang ubos niini (parent). Ang tubag ug print dili mausab.
-- Ang ngalan sa seksyon gidugangan og numero (I. ... VIII.) sama sa papel nga form.
-- FormPageViewModel.cs ExpanderItems: ang mga ulohan.
-- Modagan KAUSA RA (seed.024e).
-- =====================================================================

-- Numero sa ngalan sa seksyon (sama sa form)
UPDATE form_section SET title = 'I. Reference'                          WHERE id = 'small-business.s.reference';
UPDATE form_section SET title = 'II. Nature of inspection conducted'    WHERE id = 'small-business.s.nature';
UPDATE form_section SET title = 'III. General information'              WHERE id = 'small-business.s.general';
UPDATE form_section SET title = 'IV. Other information'                 WHERE id = 'small-business.s.other_info';
UPDATE form_section SET title = 'V. Means of egress'                    WHERE id = 'small-business.s.egress';
UPDATE form_section SET title = 'VI. Signs, lighting, and exits signage' WHERE id = 'small-business.s.signage';
UPDATE form_section SET title = 'VII. Hazard'                           WHERE id = 'small-business.s.hazard';
UPDATE form_section SET title = 'VIII. Fire protection'                 WHERE id = 'small-business.s.protection';

-- Ang mga ULOHAN (expander). Parehas ang ngalan sa seksyon aron dili doble ang ulohan sa screen.
INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('small-business.nature.grp',     'small-business','small-business.s.nature',    NULL,'nature.grp',     'II. Nature of inspection conducted',     0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.other_info.grp', 'small-business','small-business.s.other_info',NULL,'other_info.grp', 'IV. Other information',                  0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.egress.a',       'small-business','small-business.s.egress',    NULL,'egress.a',       'A. Exit access',                         0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.egress.b',       'small-business','small-business.s.egress',    NULL,'egress.b',       'B. Exits',                               1,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.signage.grp',    'small-business','small-business.s.signage',   NULL,'signage.grp',    'VI. Signs, lighting, and exits signage', 0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.hazard.grp',     'small-business','small-business.s.hazard',    NULL,'hazard.grp',     'VII. Hazard',                            0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.protection.grp', 'small-business','small-business.s.protection',NULL,'protection.grp', 'VIII. Fire protection',                  0,'NONE',NULL,NULL,0,NULL,NULL,NULL);

-- Ang mga pangutana sa seksyon (ang naa sa ibabaw ra) -> ubos sa iyang ulohan
UPDATE form_item SET parent_item_id = 'small-business.nature.grp'
 WHERE section_id = 'small-business.s.nature' AND parent_item_id IS NULL AND id <> 'small-business.nature.grp';

UPDATE form_item SET parent_item_id = 'small-business.other_info.grp'
 WHERE section_id = 'small-business.s.other_info' AND parent_item_id IS NULL AND id <> 'small-business.other_info.grp';

UPDATE form_item SET parent_item_id = 'small-business.signage.grp'
 WHERE section_id = 'small-business.s.signage' AND parent_item_id IS NULL AND id <> 'small-business.signage.grp';

UPDATE form_item SET parent_item_id = 'small-business.hazard.grp'
 WHERE section_id = 'small-business.s.hazard' AND parent_item_id IS NULL AND id <> 'small-business.hazard.grp';

UPDATE form_item SET parent_item_id = 'small-business.protection.grp'
 WHERE section_id = 'small-business.s.protection' AND parent_item_id IS NULL AND id <> 'small-business.protection.grp';

-- V. Means of egress: A. Exit access (Doors, Corridors) ug B. Exits (ang uban)
UPDATE form_item SET parent_item_id = 'small-business.egress.a'
 WHERE section_id = 'small-business.s.egress' AND parent_item_id IS NULL AND key LIKE 'egress.access.%';

UPDATE form_item SET parent_item_id = 'small-business.egress.b'
 WHERE section_id = 'small-business.s.egress' AND parent_item_id IS NULL
   AND id NOT IN ('small-business.egress.a', 'small-business.egress.b');
