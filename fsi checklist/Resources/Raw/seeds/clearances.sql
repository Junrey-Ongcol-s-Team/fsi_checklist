-- =====================================================================
-- BISAYA: "Secure the following Fire Safety Clearance/s" = CHIPS (061 ug 069)
--   Mogawas ra kung naka-check ang "Secure the following Fire Safety Clearance/s".
--   Search box: mo-filter sa mga chip; i-tap ang chip aron idugang/tangtangon; pwede pud mag-type og lain (Add).
--   Ang data_type LONG_TEXT gihapon (ChipItems sa FormPageViewModel.cs ang naghimo niini nga chips).
--   label = tibuok ngalan sa chip; value = mubo nga sulat nga mohaum sa linya sa print (matag chip = usa ka linya).
--   Kung molapas og 6: ang uban isulat sa dugang nga page sa printout (061.json/069.json "lists").
-- Version 2: ang listahan gikan sa BFP (gipuli ang daan nga 6).
-- =====================================================================

UPDATE form_item
   SET label = 'Fire Safety Clearance/s to secure (search or tap to add)',
       parent_item_id = REPLACE(id, '.rec.clearances', '.rec.secure_clearances'),
       show_if_item_id = REPLACE(id, '.rec.clearances', '.rec.secure_clearances'),
       show_if_values = '1'
 WHERE id IN ('fsic-new-occupancy.rec.clearances', 'fsic-renewal.rec.clearances');

DELETE FROM form_choice WHERE item_id IN ('fsic-new-occupancy.rec.clearances', 'fsic-renewal.rec.clearances');

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order)
SELECT i.id || '.' || c.code, i.id, c.value, c.label, c.sort
  FROM form_item i,
       (SELECT 'FIREWORKS' AS code, 'Fireworks Exhibition' AS value, 'Fireworks Exhibition' AS label, 1 AS sort
        UNION ALL SELECT 'FIRE_DRILL', 'Cert. of Fire Drill', 'Certificate of Fire Drill', 2
        UNION ALL SELECT 'AFSS', 'AFSS Installation', 'Installation of Automatic Fire Suppression System (AFSS)', 3
        UNION ALL SELECT 'LPG_STATION', 'LPGas Station', 'Installation of LPGas Station', 4
        UNION ALL SELECT 'FCL_STORAGE', 'Storage of Flam. Liquids', 'Storage for Flammable and Combustible Liquids', 5
        UNION ALL SELECT 'HAZMAT', 'Conveyance of HazMat', 'Conveyance of Hazardous Materials and Chemicals in Cargo Vehicles', 6
        UNION ALL SELECT 'ELECTRICAL', 'Electrical Installation', 'Electrical Installation', 7
        UNION ALL SELECT 'FUMIGATION', 'Fumigation/Fogging', 'Fumigation and Thermal Insecticidal Fogging', 8
        UNION ALL SELECT 'KITCHEN_HOOD', 'Kitchen Hood System', 'Installation of Kitchen Hood Suppression Systems', 9
        UNION ALL SELECT 'FCL_TANKS', 'Flam. Liquid Tanks', 'Installation of Flammable and Combustible Liquids Storage Tanks', 10
        UNION ALL SELECT 'HOT_WORK', 'Hot Work', 'Welding, Cutting and Other Hot Work', 11
        UNION ALL SELECT 'STANDPIPE', 'Standpipe Installation', 'Installation of Standpipe', 12) c
 WHERE i.id IN ('fsic-new-occupancy.rec.clearances', 'fsic-renewal.rec.clearances');
