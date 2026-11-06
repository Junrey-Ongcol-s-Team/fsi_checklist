-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (BFP-QSF-FSED-061).
-- Kung unsa ang gipili, kinahanglan ma-fill up tanan niini, aron sakto ang circle progress
-- ug ang progress bar (ang nakatago dili iapil, kay naa sa show_if).
--   FSIC for Occupancy  -> FSEC No., Date Issued, Building Permit, Date Issued
--   New Business Permit -> FSIC No. (Latest), Certificate of Fire Drill, Business Permit No. + ilang Date Issued
--                          (ang Fire Insurance Policy No. "(If any)" optional gihapon, sama sa form)
--   Nature = Others     -> Others (specify)
--   Residential         -> sub classification;  Special Structure -> specify
-- Modagan KAUSA RA (seed.061c).
-- =====================================================================

UPDATE form_item SET is_required = 1
 WHERE id IN ('fsic-new-occupancy.fsec_no','fsic-new-occupancy.fsec_date',
              'fsic-new-occupancy.building_permit_no','fsic-new-occupancy.building_permit_date',
              'fsic-new-occupancy.fsic_no','fsic-new-occupancy.fsic_date',
              'fsic-new-occupancy.fire_drill_no','fsic-new-occupancy.fire_drill_date',
              'fsic-new-occupancy.business_permit_no','fsic-new-occupancy.business_permit_date',
              'fsic-new-occupancy.nature.others_specify',
              'fsic-new-occupancy.residential_subclass','fsic-new-occupancy.special_structure');
