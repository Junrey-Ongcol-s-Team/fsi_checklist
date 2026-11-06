-- =====================================================================
-- BISAYA: UNIT sa tanang numero nga field sa TANANG FORMS (makita sa tuo sa numero).
-- Ang 061 naa sa 061h.sql. Kini ang nahabilin nga walay unit (mga ihap: persons, floors, units ...).
-- Walay unit gihapon: "Year of Manufacturing" (tuig) ug "K-factor" (walay unit ang K-factor).
-- Unit ra ang giusab; walay tubag nga mausab. Modagan KAUSA RA (seed.units).
-- =====================================================================

-- Building plans (006) ug During construction (068)
UPDATE form_item SET unit = 'floors'  WHERE id = 'building-plans.no_of_floors';
UPDATE form_item SET unit = 'persons' WHERE template_id = 'building-plans' AND key LIKE 'cond.01.%' AND data_type = 'INTEGER';
UPDATE form_item SET unit = 'exits'   WHERE template_id IN ('building-plans','during-construction') AND key IN ('cond.08','cond.08.b');
UPDATE form_item SET unit = 'risers'  WHERE template_id IN ('building-plans','during-construction') AND key = 'cond.68';

-- FSIC renewal (069): unit ra (walay laing kausaban)
UPDATE form_item SET unit = 'floors' WHERE id = 'fsic-renewal.a02.prev_floors';
UPDATE form_item SET unit = 'units'  WHERE id = 'fsic-renewal.a11.number';

-- Gasoline service station (026)
UPDATE form_item SET unit = 'storey' WHERE id = 'gasoline-station.storeys';
UPDATE form_item SET unit = 'units'  WHERE id IN ('gasoline-station.ael.units_per_floor','gasoline-station.fe.units');
UPDATE form_item SET unit = 'bays'   WHERE id = 'gasoline-station.of.lubricating_bays';
UPDATE form_item SET unit = 'racks'  WHERE id = 'gasoline-station.of.wash_racks';

-- Small / general business (024)
UPDATE form_item SET unit = 'storey' WHERE id = 'small-business.stories';
UPDATE form_item SET unit = 'units'  WHERE id IN ('small-business.fe.quantity','small-business.fd.units_per_floor');
