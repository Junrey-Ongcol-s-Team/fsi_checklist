-- =====================================================================
-- BISAYA: DEFAULT nga LETTERHEAD (Settings -> Fire station) para sa TANANG form nga naay
-- "(FIRE STATION LETTER HEAD)" / "(Region) (Station) ..." aron kanunay naay sulat ang printout:
--   Region 10
--   Misamis Occidental Provincial Office
--   Tangub City Fire Station
--   1st South Street, Brgy III, Tangub City
--   545-0500/tangubcfs_bfp10@yahoo.com
-- Ibutang RA kung wala pa o blangko ang setting (dili ilisan ang gi-type na sa Settings).
-- Usbon gihapon sa Settings kung lahi nga station.
-- Modagan KAUSA RA (seed.station_defaults).
-- =====================================================================

INSERT OR IGNORE INTO app_setting (key, value) VALUES
 ('station_region',  'Region 10'),
 ('station_office',  'Misamis Occidental Provincial Office'),
 ('station_name',    'Tangub City Fire Station'),
 ('station_address', '1st South Street, Brgy III, Tangub City'),
 ('station_hotline', '545-0500'),
 ('station_email',   'tangubcfs_bfp10@yahoo.com');

UPDATE app_setting SET value = 'Region 10'                                WHERE key = 'station_region'  AND TRIM(COALESCE(value, '')) = '';
UPDATE app_setting SET value = 'Misamis Occidental Provincial Office'     WHERE key = 'station_office'  AND TRIM(COALESCE(value, '')) = '';
UPDATE app_setting SET value = 'Tangub City Fire Station'                 WHERE key = 'station_name'    AND TRIM(COALESCE(value, '')) = '';
UPDATE app_setting SET value = '1st South Street, Brgy III, Tangub City'  WHERE key = 'station_address' AND TRIM(COALESCE(value, '')) = '';
UPDATE app_setting SET value = '545-0500'                                 WHERE key = 'station_hotline' AND TRIM(COALESCE(value, '')) = '';
UPDATE app_setting SET value = 'tangubcfs_bfp10@yahoo.com'                WHERE key = 'station_email'   AND TRIM(COALESCE(value, '')) = '';
