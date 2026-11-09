-- =====================================================================
-- BISAYA: 024 Small Business: N/A button sa
--   FSEC No. / Building/Renovation Permit / FSIC No. (Latest) / Business Permit No.
--   (ang Fire Insurance Policy No. naa na sa na_text.sql)
--   Sa app: "N/A" button tapad sa sulatanan (FormPageViewModel.cs NaTextItems).
--   Kung N/A: ang iyang "date issued" mawala sa app ug "N/A" ang ma-print (PrintService).
--   show_if '!N/A' = makita kung DILI "N/A" (apil kung wala pay tubag).
-- Modagan KAUSA RA (seed.024d).
-- =====================================================================

UPDATE form_item
   SET parent_item_id  = 'small-business.' || REPLACE(key, '_date', '_no'),
       show_if_item_id = 'small-business.' || REPLACE(key, '_date', '_no'),
       show_if_values  = '!N/A'
 WHERE template_id = 'small-business'
   AND key IN ('fsec_date', 'renovation_permit_date', 'fsic_date', 'business_permit_date');
