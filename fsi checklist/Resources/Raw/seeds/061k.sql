-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (BFP-QSF-FSED-061).
-- FIRE SAFETY INSPECTOR 2: ang form naay DUHA ka linya sa "Fire Safety Inspector/s".
--   - Bag-ong role INSPECTOR_2 = "Fire Safety Inspector 2"
--   - ON_SITE, OPTIONAL (is_required = 0): kung walay ikaduha nga inspector, blangko ra ug dili makababag sa Complete
--   - Mano-mano nga pirma sa Sign-off (dili pre-signed, kay lahi-lahi ang mouban)
--   - Print: ^INSPECTOR_2_NAME = ang ikaduha (tuo) nga linya sa 061.json
-- Modagan KAUSA RA (seed.061k).
-- =====================================================================

INSERT OR IGNORE INTO signatory_role (code, label) VALUES ('INSPECTOR_2', 'Fire Safety Inspector 2');

-- Han-ay sa Sign-off: Owner, Inspector, Inspector 2, Team Leader, Chief, Fire Marshal
UPDATE form_template_signatory SET sort_order = sort_order + 1
 WHERE template_id = 'fsic-new-occupancy' AND sort_order >= 3;

INSERT OR IGNORE INTO form_template_signatory (template_id, role_code, stage, is_required, min_count, sort_order)
VALUES ('fsic-new-occupancy', 'INSPECTOR_2', 'ON_SITE', 0, 1, 3);
