-- =====================================================================
-- BISAYA: N/A sa "Certificate of Fire Drill" ug "Fire Insurance Policy No." (tanang forms nga naa niini)
--   Sa app: naay "N/A" button tapad sa sulatanan (FormPageViewModel.cs NaTextKeys).
--   Kung N/A: ang iyang "date issued" mawala sa app (dili na kinahanglan) ug "N/A" ang ma-print.
--   Kung naay gi-type (dili N/A): mogawas ang date issued sama sa una.
--   show_if '!N/A' = makita kung DILI "N/A" (apil kung wala pay tubag).
--   Ang date nahimong anak sa iyang numero, busa sundon gihapon niini ang lagda sa numero (pananglitan 061 New BP ra).
-- Modagan KAUSA RA (seed.na_text).
-- =====================================================================

UPDATE form_item
   SET parent_item_id = REPLACE(id, '.fire_drill_date', '.fire_drill_no'),
       show_if_item_id = REPLACE(id, '.fire_drill_date', '.fire_drill_no'),
       show_if_values = '!N/A'
 WHERE key = 'fire_drill_date'
   AND EXISTS (SELECT 1 FROM form_item n WHERE n.id = REPLACE(form_item.id, '.fire_drill_date', '.fire_drill_no'));

UPDATE form_item
   SET parent_item_id = REPLACE(id, '.insurance_policy_date', '.insurance_policy_no'),
       show_if_item_id = REPLACE(id, '.insurance_policy_date', '.insurance_policy_no'),
       show_if_values = '!N/A'
 WHERE key = 'insurance_policy_date'
   AND EXISTS (SELECT 1 FROM form_item n WHERE n.id = REPLACE(form_item.id, '.insurance_policy_date', '.insurance_policy_no'));
