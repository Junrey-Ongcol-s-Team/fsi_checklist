-- =====================================================================
-- BISAYA: 061 New FSIC / Occupancy: "Residential: sub classification" = PICKER (dropdown)
--   Kaniadto: kahon nga sulatan (TEXT). Karon: pili-on sa 5:
--     Single and Two Family Dwellings, Hotels, Dormitories, Apartments, Lodging or rooming
--   Ang value = parehas sa label, busa ang print mogawas gihapon sama sa gipili.
--   Ang daan nga tubag nga parehas sa usa sa 5 (bisan lahi ang dako/gamay nga letra) i-ayo.
--   Ang lain nga daan nga tubag: magpabilin (ma-print gihapon), pili-a lang pag-usab sa picker.
--   Ang LOCKED nga inspection dili hilabtan.
-- Modagan KAUSA RA (seed.061m). Kinahanglan human sa lock.sql (locked_at).
-- =====================================================================

UPDATE form_item SET data_type = 'CHOICE'
 WHERE id = 'fsic-new-occupancy.residential_subclass';

INSERT OR IGNORE INTO form_choice (id, item_id, value, label, sort_order) VALUES
 ('fsic-new-occupancy.residential_subclass.DWELLING',  'fsic-new-occupancy.residential_subclass', 'Single and Two Family Dwellings', 'Single and Two Family Dwellings', 1),
 ('fsic-new-occupancy.residential_subclass.HOTEL',     'fsic-new-occupancy.residential_subclass', 'Hotels',                          'Hotels',                          2),
 ('fsic-new-occupancy.residential_subclass.DORMITORY', 'fsic-new-occupancy.residential_subclass', 'Dormitories',                     'Dormitories',                     3),
 ('fsic-new-occupancy.residential_subclass.APARTMENT', 'fsic-new-occupancy.residential_subclass', 'Apartments',                      'Apartments',                      4),
 ('fsic-new-occupancy.residential_subclass.LODGING',   'fsic-new-occupancy.residential_subclass', 'Lodging or rooming',              'Lodging or rooming',              5);

-- Ayohon ang daan nga tubag nga parehas (pananglitan "hotels" -> "Hotels")
UPDATE inspection_answer
   SET value_text = (SELECT c.value FROM form_choice c
                      WHERE c.item_id = inspection_answer.item_id
                        AND LOWER(TRIM(c.value)) = LOWER(TRIM(inspection_answer.value_text)))
 WHERE item_id = 'fsic-new-occupancy.residential_subclass'
   AND inspection_id IN (SELECT id FROM inspection WHERE locked_at IS NULL)
   AND EXISTS (SELECT 1 FROM form_choice c
                WHERE c.item_id = inspection_answer.item_id
                  AND LOWER(TRIM(c.value)) = LOWER(TRIM(inspection_answer.value_text)));
