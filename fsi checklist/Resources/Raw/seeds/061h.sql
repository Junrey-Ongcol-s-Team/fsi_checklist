-- =====================================================================
-- BISAYA: New FSIC / Certificate of Occupancy (BFP-QSF-FSED-061). UNIT sa matag numero nga field,
-- makita sa tuo sa numero (pananglitan Doors ___ m, Capacity ___ persons, Number of Stories ___ storey).
-- Ang "Actual Dim." sa mga lagda: unit sunod sa gisulat sa form (mm, m, lux, min, hours ...).
-- Label ra / unit ra ang giusab; walay tubag nga mausab. Modagan KAUSA RA (seed.061h).
-- =====================================================================

UPDATE form_item SET unit = 'persons'
 WHERE template_id = 'fsic-new-occupancy' AND data_type = 'INTEGER' AND key LIKE '%.capacity';

UPDATE form_item SET unit = 'storey'  WHERE id = 'fsic-new-occupancy.stories';
UPDATE form_item SET unit = 'persons/floor' WHERE id = 'fsic-new-occupancy.max_occupant_load';

-- A. Exit access (Actual Dim.)
UPDATE form_item SET unit = 'mm'    WHERE id IN ('fsic-new-occupancy.access.s01','fsic-new-occupancy.access.s02');
UPDATE form_item SET unit = 'exits' WHERE id = 'fsic-new-occupancy.access.s03';
UPDATE form_item SET unit = 'min'   WHERE id = 'fsic-new-occupancy.access.s04';

-- B. Exits (Actual Dim.)
UPDATE form_item SET unit = 'exits'  WHERE id = 'fsic-new-occupancy.exits.s01';
UPDATE form_item SET unit = 'min'    WHERE id IN ('fsic-new-occupancy.exits.s02','fsic-new-occupancy.exits.s03');
UPDATE form_item SET unit = 'storey' WHERE id = 'fsic-new-occupancy.exits.s04';
UPDATE form_item SET unit = 'mm'     WHERE id IN ('fsic-new-occupancy.exits.s05','fsic-new-occupancy.exits.s06','fsic-new-occupancy.exits.s07');

-- C. Exits discharge
UPDATE form_item SET unit = 'm' WHERE id IN ('fsic-new-occupancy.discharge.s01','fsic-new-occupancy.discharge.s02');

-- Signs, lighting, and exits signage
UPDATE form_item SET unit = 'mm'    WHERE id IN ('fsic-new-occupancy.signage.a1','fsic-new-occupancy.signage.d1',
                                                 'fsic-new-occupancy.signage.d2','fsic-new-occupancy.signage.d3');
UPDATE form_item SET unit = 'lux'   WHERE id IN ('fsic-new-occupancy.illum.c1','fsic-new-occupancy.illum.c2',
                                                 'fsic-new-occupancy.illum.c3','fsic-new-occupancy.illum.e1');
UPDATE form_item SET unit = 'hours' WHERE id = 'fsic-new-occupancy.illum.e2';
