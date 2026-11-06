-- =====================================================================
-- BISAYA: FSIC renewal (069), Section V (Fire Safety Measures Assessment).
-- TUMONG: kung "No changes" ang gipili, WALA nay laing kahon nga mogawas sa
-- Present/Actual Inspection. Kung "With changes" ra mogawas ang Renovation,
-- Repair, Additional, State particular ... (Item 1 hangtod Item 12).
--
-- Unsa ang giusab:
--   1. Duha ka bag-ong column set (ang daan nga cs-ccp wala gi-delete):
--        cs-ccs = Previous Compliant + Compliant Yes/No + No changes/With changes   (Item 1-12)
--        cs-cc  = Previous Compliant + Compliant Yes/No                              (Item 13: walay "No changes" sa form)
--      Ang "Particulars" nga column gitangtang kay sa daghang item walay linya sa form.
--   2. Ang "State particular violation" sa Item 6, 7, 9, 10, 12 nahimong kaugalingong
--      pangutana ubos sa item (para matago kung No changes). Pareho ra ang key sa
--      print (aXX.particulars), so dili kinahanglan usbon ang 069.json.
--   3. show_if = 'change_status:WITH_CHANGES' -> mogawas ra kung With changes.
--      (Ang "change_status:" nagpasabot nga ang column ang tan-awon, dili ang value.)
--   4. Ang daan nga tubag sa Particulars column gibalhin ngadto sa bag-ong pangutana.
--   5. Daan nga tubag nga naay detalye pero walay gipili -> "With changes" (para dili matago).
-- Modagan KAUSA RA (seed.069e).
-- =====================================================================

-- 1. Bag-ong column sets (same ang key sa column, so ang daan nga tubag mogawas gihapon)
INSERT OR IGNORE INTO form_column_set (id, code) VALUES
 ('cs-ccs','COMPLIANT_CHANGE_STATUS'),
 ('cs-cc','COMPLIANT_PREV_PRESENT');

INSERT OR IGNORE INTO form_column (id, column_set_id, key, label, data_type, sort_order) VALUES
 ('c-ccs-0','cs-ccs','prev_compliant','Previous After Inspection Report: Compliant (Yes)','BOOL',0),
 ('c-ccs-1','cs-ccs','compliant','Present/Actual Inspection: Compliant (Yes / No)','YES_NO',1),
 ('c-ccs-2','cs-ccs','change_status','Present/Actual Inspection: No changes / With changes','CHOICE',2),
 ('c-cc-0','cs-cc','prev_compliant','Previous After Inspection Report: Compliant (Yes)','BOOL',0),
 ('c-cc-1','cs-cc','compliant','Present/Actual Inspection: Compliant (Yes / No)','YES_NO',1);

INSERT OR IGNORE INTO form_choice (id, column_id, value, label, sort_order) VALUES
 ('ch-ccs-1','c-ccs-2','NO_CHANGES','No changes from previous inspection',1),
 ('ch-ccs-2','c-ccs-2','WITH_CHANGES','With changes',2);

UPDATE form_item SET column_set_id = 'cs-ccs'
 WHERE template_id = 'fsic-renewal' AND column_set_id = 'cs-ccp' AND key <> 'a13';
UPDATE form_item SET column_set_id = 'cs-cc'
 WHERE id = 'fsic-renewal.a13';

-- 2. "State particular violation" isip pangutana ubos sa item
INSERT OR IGNORE INTO form_item (id, template_id, section_id, parent_item_id, key, label, sort_order, data_type, unit, column_set_id, is_required, help_text, show_if_item_id, show_if_values) VALUES
 ('fsic-renewal.a06a.particulars','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a06a','a06a.particulars','Present/Actual Inspection: State particular violation',502,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a06b.particulars','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a06b','a06b.particulars','Present/Actual Inspection: State particular violation',512,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a07.particulars','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a07','a07.particulars','Present/Actual Inspection: State particular violation',522,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a09.particulars','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a09','a09.particulars','Present/Actual Inspection: State particular violation',665,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a10.particulars','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a10','a10.particulars','Present/Actual Inspection: State particular violation',755,'TEXT',NULL,NULL,0,NULL,NULL,NULL),
 ('fsic-renewal.a12.particulars','fsic-renewal','fsic-renewal.s.assessment','fsic-renewal.a12','a12.particulars','Present/Actual Inspection: State particular violation',845,'TEXT',NULL,NULL,0,NULL,NULL,NULL);

-- 3. Mogawas ra kung "With changes" (tanang Present/Actual nga kahon sa Item 1-12)
UPDATE form_item
   SET show_if_item_id = parent_item_id,
       show_if_values  = 'change_status:WITH_CHANGES'
 WHERE template_id = 'fsic-renewal'
   AND key IN (
     'a01.renovation','a01.repair','a01.additional',
     'a02.renovation','a02.repair','a02.additional','a02.particular_floor',
     'a03.change_of_occupancy',
     'a04.renovation','a04.repair','a04.additional','a04.particular_changes',
     'a05.renovation','a05.repair','a05.additional','a05.particular_violation',
     'a06a.particulars','a06b.particulars','a07.particulars',
     'a08.maq','a08.no_additional_protection','a08.no_msds','a08.no_proper_storage',
     'a09.particulars',
     'a10.good_condition','a10.particulars',
     'a11.good_condition','a11.need_additional','a11.location','a11.number',
     'a12.particulars');

-- 4. Ibalhin ang daan nga tubag sa Particulars column ngadto sa bag-ong pangutana.
--    Temporaryo nga gitangtang ang lock trigger aron ma-apil ang COMPLETED nga inspection,
--    dayon gibalik dayon (pareho ra sa schema.sql).
DROP TRIGGER IF EXISTS trg_answer_lock_upd;

UPDATE OR IGNORE inspection_answer
   SET item_id = item_id || '.particulars', slot_key = 'value'
 WHERE slot_key = 'particulars'
   AND item_id IN ('fsic-renewal.a06a','fsic-renewal.a06b','fsic-renewal.a07',
                   'fsic-renewal.a09','fsic-renewal.a10','fsic-renewal.a12');

-- 5. Daan nga inspection: kung naay tubag sa Renovation/Repair/violation pero WALA gipili
--    ang "No changes / With changes", i-set sa "With changes" aron dili matago ang tubag
--    ug dili mausab ang printout.
DROP TRIGGER IF EXISTS trg_answer_lock_ins;

INSERT OR IGNORE INTO inspection_answer (inspection_id, item_id, slot_key, repeat_no, value_text)
SELECT DISTINCT a.inspection_id, i.show_if_item_id, 'change_status', 0, 'WITH_CHANGES'
  FROM inspection_answer a
  JOIN form_item i ON i.id = a.item_id
 WHERE i.template_id = 'fsic-renewal'
   AND i.show_if_values = 'change_status:WITH_CHANGES'
   AND (NULLIF(TRIM(a.value_text), '') IS NOT NULL OR IFNULL(a.value_num, 0) <> 0)
   AND NOT EXISTS (SELECT 1 FROM inspection_answer x
                    WHERE x.inspection_id = a.inspection_id
                      AND x.item_id = i.show_if_item_id
                      AND x.slot_key = 'change_status');

CREATE TRIGGER trg_answer_lock_ins BEFORE INSERT ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = NEW.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

CREATE TRIGGER trg_answer_lock_upd BEFORE UPDATE ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = OLD.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;
