-- =====================================================================
-- BISAYA: 026 Gasoline Service Station: N/A button sa
--   Building Permit No, Occupancy Permit No., Latest FSIC Issued Control No.,
--   Latest Notice to Correct Violations Control No., Latest Mayor's/Bus. Permit,
--   Municipal License No., Latest Certificate of Electrical Inspection No.
--   (Certificate of Fire Drill ug Fire insurance policy No.: naa na sa na_text.sql)
--   Sa app: "N/A" button tapad sa sulatanan (FormPageViewModel.cs NaTextItems).
--   Kung N/A: ang iyang "date issued" mawala sa app ug "N/A" ang ma-print (PrintService).
--   show_if '!N/A' = makita kung DILI "N/A" (apil kung wala pay tubag).
-- Modagan KAUSA RA (seed.026c). Kinahanglan human sa 026b (ang date ubos na sa iyang numero).
-- =====================================================================

UPDATE form_item
   SET parent_item_id  = 'gasoline-station.' || REPLACE(key, '_date', '_no'),
       show_if_item_id = 'gasoline-station.' || REPLACE(key, '_date', '_no'),
       show_if_values  = '!N/A'
 WHERE template_id = 'gasoline-station'
   AND key IN ('building_permit_date', 'occupancy_permit_date', 'fsic_date', 'ntcv_date',
               'business_permit_date', 'municipal_license_date', 'electrical_date');

-- Ayohon ang sayop nga ngalan ("Latesttice")
UPDATE form_item SET label = 'Latest Notice to Correct Violations: date issued'
 WHERE id = 'gasoline-station.ntcv_date';
