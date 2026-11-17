-- =====================================================================
-- BISAYA: 069 FSIC Renewal: N/A BUTTON sa matag numero 1 - 13
--   (Fire safety measures assessment of the building/structures/facility)
--   Kung wala ana ang establisyemento (pananglitan walay Fire Pump): i-tap ang "N/A".
--   -> ang tanang pangutana sulod sa numero mawala sa app (dili na kinahanglan tubagon)
--   -> sa print: "N/A" tapad sa "Compliant" sa Previous ug Present nga column (print/069.json: aNN.na, aNN.na2)
--   Ang N/A = BOOL nga item "aNN.na" (una sa sulod sa numero); ang ubang anak show_if '!1'
--   (makita kung DILI naka-N/A). Control sa app: Controls/FsicRenewal/FsicRenewalNaButton.
-- Modagan KAUSA RA (seed.069h).
-- =====================================================================

INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values)
SELECT h.id || '.na', h.template_id, h.section_id, h.id, h.key || '.na',
       'Not applicable to this establishment', h.sort_order, 'BOOL', NULL, NULL, 0, NULL, NULL, NULL
  FROM form_item h
 WHERE h.section_id = 'fsic-renewal.s.assessment' AND h.parent_item_id IS NULL AND h.data_type = 'NONE'
   AND h.key LIKE 'a__%' AND h.key NOT LIKE '%.%';

-- Ang ubang pangutana sa numero: makita ra kung DILI N/A
UPDATE form_item
   SET show_if_item_id = parent_item_id || '.na', show_if_values = '!1'
 WHERE section_id = 'fsic-renewal.s.assessment'
   AND show_if_item_id IS NULL
   AND key NOT LIKE '%.na'
   AND parent_item_id IN (SELECT id FROM form_item
                           WHERE section_id = 'fsic-renewal.s.assessment' AND parent_item_id IS NULL
                             AND data_type = 'NONE' AND key LIKE 'a__%' AND key NOT LIKE '%.%');
