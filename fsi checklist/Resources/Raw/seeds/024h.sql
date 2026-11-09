-- =====================================================================
-- BISAYA: 024 Small Business: VIII. Fire protection -> A., B., C. (tulo ka expander, sama sa form)
--   A. First aid fire protection (Fire extinguishers)   = fe.*
--   B. Emergency lighting systems                       = el.*
--   C. Fire detection and alarm                         = fd.*
--   Ang usa ka dako nga expander (protection.grp gikan sa 024e) gitangtang; ang tubag ug print dili mausab.
-- Modagan KAUSA RA (seed.024h). Kinahanglan human sa 024e.
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('small-business.fp.a','small-business','small-business.s.protection',NULL,'fp.a','A. First aid fire protection (Fire extinguishers)',0,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fp.b','small-business','small-business.s.protection',NULL,'fp.b','B. Emergency lighting systems',1,'NONE',NULL,NULL,0,NULL,NULL,NULL),
 ('small-business.fp.c','small-business','small-business.s.protection',NULL,'fp.c','C. Fire detection and alarm',2,'NONE',NULL,NULL,0,NULL,NULL,NULL);

UPDATE form_item SET parent_item_id = 'small-business.fp.a'
 WHERE section_id = 'small-business.s.protection' AND parent_item_id = 'small-business.protection.grp' AND key LIKE 'fe.%';

UPDATE form_item SET parent_item_id = 'small-business.fp.b'
 WHERE section_id = 'small-business.s.protection' AND parent_item_id = 'small-business.protection.grp' AND key LIKE 'el.%';

UPDATE form_item SET parent_item_id = 'small-business.fp.c'
 WHERE section_id = 'small-business.s.protection' AND parent_item_id = 'small-business.protection.grp' AND key LIKE 'fd.%';

-- Ang daan nga usa ka expander (wala nay sulod)
DELETE FROM form_item
 WHERE id = 'small-business.protection.grp'
   AND NOT EXISTS (SELECT 1 FROM form_item c WHERE c.parent_item_id = 'small-business.protection.grp');
