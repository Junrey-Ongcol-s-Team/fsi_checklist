-- =====================================================================
-- BISAYA: LOCK THIS INSPECTION (sugyot ni Sir Junrey)
--   Kaniadto: human ma-Complete, dili na ma-usab ang tubag (trigger sa status = 'COMPLETED').
--   Karon:    pwede gihapon i-EDIT bisan na-Complete na (Edit entries sa dashboard).
--             Dili na ma-usab kung gi-LOCK na ("Lock this inspection"): inspection.locked_at naay petsa.
--   Ang trigger sa ubos ang mobantay: bisan unsa nga code ang mo-sulat, dili molusot kung locked.
-- Modagan KAUSA RA (seed.lock). Kini ang ULAHI nga seed nga mo-usab sa lock triggers.
-- =====================================================================

ALTER TABLE inspection ADD COLUMN locked_at TEXT;

-- Tangtangon ang daan nga lock (COMPLETED) ...
DROP TRIGGER IF EXISTS trg_answer_lock_ins;
DROP TRIGGER IF EXISTS trg_answer_lock_upd;
DROP TRIGGER IF EXISTS trg_answer_lock_del;
DROP TRIGGER IF EXISTS trg_finding_lock_ins;
DROP TRIGGER IF EXISTS trg_finding_lock_upd;
DROP TRIGGER IF EXISTS trg_finding_lock_del;

-- ... ilisan sa bag-o: LOCKED ra ang dili na ma-usab
CREATE TRIGGER trg_answer_lock_ins BEFORE INSERT ON inspection_answer
WHEN (SELECT locked_at FROM inspection WHERE id = NEW.inspection_id) IS NOT NULL
BEGIN SELECT RAISE(ABORT, 'inspection is locked'); END;

CREATE TRIGGER trg_answer_lock_upd BEFORE UPDATE ON inspection_answer
WHEN (SELECT locked_at FROM inspection WHERE id = OLD.inspection_id) IS NOT NULL
BEGIN SELECT RAISE(ABORT, 'inspection is locked'); END;

CREATE TRIGGER trg_answer_lock_del BEFORE DELETE ON inspection_answer
WHEN (SELECT locked_at FROM inspection WHERE id = OLD.inspection_id) IS NOT NULL
BEGIN SELECT RAISE(ABORT, 'inspection is locked'); END;

CREATE TRIGGER trg_finding_lock_ins BEFORE INSERT ON inspection_finding
WHEN (SELECT locked_at FROM inspection WHERE id = NEW.inspection_id) IS NOT NULL
BEGIN SELECT RAISE(ABORT, 'inspection is locked'); END;

CREATE TRIGGER trg_finding_lock_upd BEFORE UPDATE ON inspection_finding
WHEN (SELECT locked_at FROM inspection WHERE id = OLD.inspection_id) IS NOT NULL
BEGIN SELECT RAISE(ABORT, 'inspection is locked'); END;

CREATE TRIGGER trg_finding_lock_del BEFORE DELETE ON inspection_finding
WHEN (SELECT locked_at FROM inspection WHERE id = OLD.inspection_id) IS NOT NULL
BEGIN SELECT RAISE(ABORT, 'inspection is locked'); END;

-- Mga litrato: dili na madugang o matangtang kung locked
DROP TRIGGER IF EXISTS trg_attachment_lock_ins;
DROP TRIGGER IF EXISTS trg_attachment_lock_del;

CREATE TRIGGER trg_attachment_lock_ins BEFORE INSERT ON attachment
WHEN (SELECT locked_at FROM inspection WHERE id = NEW.inspection_id) IS NOT NULL
BEGIN SELECT RAISE(ABORT, 'inspection is locked'); END;

CREATE TRIGGER trg_attachment_lock_del BEFORE DELETE ON attachment
WHEN (SELECT locked_at FROM inspection WHERE id = OLD.inspection_id) IS NOT NULL
BEGIN SELECT RAISE(ABORT, 'inspection is locked'); END;

-- Ang pirma: dili na ma-usab kung locked
DROP TRIGGER IF EXISTS trg_signatory_lock_upd;

CREATE TRIGGER trg_signatory_lock_upd BEFORE UPDATE ON inspection_signatory
WHEN (SELECT locked_at FROM inspection WHERE id = OLD.inspection_id) IS NOT NULL
BEGIN SELECT RAISE(ABORT, 'inspection is locked'); END;
