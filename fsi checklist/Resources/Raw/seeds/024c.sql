-- =====================================================================
-- BISAYA: 024 Small Business: "Type of Occupancy" ug "Construction Type"
--   USA RA ang mapili (CHOICE) ug EXPANDER (FormPageViewModel ExpanderItems).
--   Type of Occupancy: Mercantile / Business / Others (specify)
--     -> ang "Others (specify)" nga kahon mogawas RA kung Others ang gipili (sulod sa expander).
--   Construction Type: Timber / Reinforced Concrete / Steel / Mixed (usa ra).
--   Ang print dili mausab (parehas nga value).
-- Modagan KAUSA RA (seed.024c).
-- =====================================================================

UPDATE form_choice SET label = 'Others (specify)'
 WHERE item_id = 'small-business.occupancy_type' AND value = 'OTHERS';

UPDATE form_item
   SET parent_item_id = 'small-business.occupancy_type',
       show_if_item_id = 'small-business.occupancy_type',
       show_if_values = 'OTHERS',
       is_required = 1,
       label = 'Others (specify)'
 WHERE id = 'small-business.occupancy_type_other';

UPDATE form_item SET label = 'Type of Occupancy (choose one)'
 WHERE id = 'small-business.occupancy_type';

UPDATE form_item SET label = 'Construction Type (choose one)'
 WHERE id = 'small-business.construction_type';
