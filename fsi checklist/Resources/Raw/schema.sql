-- =====================================================================
-- SCHEMA (disenyo sa tibuok database). Bisaya nga pasabot:
--   A. FORM DEFINITION  = unsa ang sulod sa matag form (template, section, item, column, choice)
--   B. RECORDING        = ang mga inspection ug tubag (subject, inspection, inspection_answer,
--                         inspection_signatory = pirma, attachment = litrato)
--   C. INTEGRITY        = mga trigger: dili na mausab ang tubag kung COMPLETED na
--   D. SEED             = mga default nga data (column sets, roles, ug ang 4 ka form header)
-- Kini modagan KAUSA RA (inig himo sa database). Kung usbon nimo kini, i-delete ang bfp_inspection.db3.
-- =====================================================================
-- =====================================================================
-- BFP Fire Safety Inspection App  --  FINAL SQLite schema
-- Stack: .NET MAUI + SQLite (sqlite-net)
--
-- Layers
--   A. Form definition   : template, section, item, choice, column sets, required signatories
--   B. Recording         : subject, personnel, inspection, answers, findings, signatures, attachments
--   C. Integrity         : template guard, lock after completion, helper views
--   D. Seed              : roles, column sets, form template headers (items load from JSON)
--
-- Conventions: TEXT GUID primary keys (generated in app), ISO-8601 TEXT dates,
--              booleans 0/1, files (photos, signatures) on disk with PATH stored here.
-- Run PRAGMA foreign_keys = ON on every connection.
-- Set PRAGMA user_version = 1 after creation; bump it per migration.
-- =====================================================================
PRAGMA foreign_keys = ON;

-- ---------------------------------------------------------------------
-- A. FORM DEFINITION (shipped with the app / updated by seed; read-only at runtime)
-- ---------------------------------------------------------------------
CREATE TABLE subject_type (                      -- new kinds of subject = new rows, not a migration
    code  TEXT PRIMARY KEY,
    label TEXT NOT NULL
);

CREATE TABLE form_template (
    id            TEXT PRIMARY KEY,
    code          TEXT NOT NULL,                 -- 'BFP-QSF-FSED-069'
    revision      TEXT NOT NULL,                 -- '01'
    revision_date TEXT,
    name          TEXT NOT NULL,                 -- official title
    -- checklist picker metadata
    display_name  TEXT NOT NULL,                 -- 'FSIC renewal'
    subject_type  TEXT NOT NULL REFERENCES subject_type(code),
    purpose       TEXT,                          -- 'NEW','RENEWAL','REINSPECTION','CONVEYANCE','DURING_CONSTRUCTION'
    sort_order    INTEGER NOT NULL DEFAULT 0,
    is_active     INTEGER NOT NULL DEFAULT 1,    -- 0 = hidden from picker (old revisions)
    UNIQUE (code, revision)
);

CREATE TABLE form_section (
    id          TEXT PRIMARY KEY,
    template_id TEXT NOT NULL REFERENCES form_template(id),
    parent_id   TEXT REFERENCES form_section(id),
    key         TEXT NOT NULL,
    title       TEXT NOT NULL,
    sort_order  INTEGER NOT NULL DEFAULT 0,
    -- conditional visibility: show only when another item's answer is in this comma-separated list
    -- (compare against value_text; a ticked BOOL counts as '1'). NULL = always shown.
    show_if_item_id TEXT REFERENCES form_item(id),
    show_if_values  TEXT,
    UNIQUE (template_id, key)
);

-- Reusable extra answer slots for grid rows (Installed / Compliant / Remarks ...)
CREATE TABLE form_column_set (
    id   TEXT PRIMARY KEY,
    code TEXT NOT NULL UNIQUE
);

CREATE TABLE form_column (
    id            TEXT PRIMARY KEY,
    column_set_id TEXT NOT NULL REFERENCES form_column_set(id),
    key           TEXT NOT NULL,
    label         TEXT NOT NULL,
    data_type     TEXT NOT NULL,
    sort_order    INTEGER NOT NULL DEFAULT 0,
    UNIQUE (column_set_id, key)
);

CREATE TABLE form_item (
    id             TEXT PRIMARY KEY,
    template_id    TEXT NOT NULL REFERENCES form_template(id),
    section_id     TEXT NOT NULL REFERENCES form_section(id),
    parent_item_id TEXT REFERENCES form_item(id),
    key            TEXT NOT NULL,                -- STABLE across revisions
    label          TEXT NOT NULL,
    sort_order     INTEGER NOT NULL DEFAULT 0,
    data_type      TEXT NOT NULL DEFAULT 'NONE' CHECK (data_type IN
                     ('NONE','TEXT','LONG_TEXT','NUMBER','INTEGER','DATE','DATETIME',
                      'BOOL','YES_NO','YES_NO_NA','PASS_FAIL','CHOICE',
                      'PHOTO')),            -- photos linked via attachment; choices = required photo slots
    unit           TEXT,
    column_set_id  TEXT REFERENCES form_column_set(id),
    is_required    INTEGER NOT NULL DEFAULT 0,
    is_repeating   INTEGER NOT NULL DEFAULT 0,
    help_text      TEXT,
    -- conditional visibility: show only when another item's answer is in this comma-separated list
    -- (compare against value_text; a ticked BOOL counts as '1'). NULL = always shown.
    show_if_item_id TEXT REFERENCES form_item(id),
    show_if_values  TEXT,
    UNIQUE (template_id, key)
);
CREATE INDEX ix_item_section ON form_item(section_id, sort_order);

CREATE TABLE form_choice (
    id         TEXT PRIMARY KEY,
    item_id    TEXT REFERENCES form_item(id),
    column_id  TEXT REFERENCES form_column(id),
    value      TEXT NOT NULL,
    label      TEXT NOT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0,
    CHECK ((item_id IS NULL) <> (column_id IS NULL))
);

-- Signature roles and which ones each form needs (drives the sign-off screen)
CREATE TABLE signatory_role (
    code  TEXT PRIMARY KEY,
    label TEXT NOT NULL
);

CREATE TABLE form_template_signatory (
    template_id TEXT NOT NULL REFERENCES form_template(id),
    role_code   TEXT NOT NULL REFERENCES signatory_role(code),
    stage       TEXT NOT NULL CHECK (stage IN ('ON_SITE','APPROVAL')),  -- signed at inspection vs. later at the office
    is_required INTEGER NOT NULL DEFAULT 1,
    min_count   INTEGER NOT NULL DEFAULT 1,      -- e.g. at least 1 inspector
    sort_order  INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (template_id, role_code)
);

-- ---------------------------------------------------------------------
-- B. RECORDING
-- ---------------------------------------------------------------------
CREATE TABLE app_setting (                       -- station_name, station_address, device_id, ...
    key   TEXT PRIMARY KEY,
    value TEXT
);

CREATE TABLE subject (                           -- what is inspected
    id           TEXT PRIMARY KEY,
    subject_type TEXT NOT NULL REFERENCES subject_type(code),
    name         TEXT NOT NULL,                  -- business / plate no. / project title
    address      TEXT,
    owner_name   TEXT,
    contact_no   TEXT,
    created_at   TEXT NOT NULL DEFAULT (datetime('now')),
    updated_at   TEXT NOT NULL DEFAULT (datetime('now'))
);
CREATE INDEX ix_subject_search ON subject(subject_type, name);

CREATE TABLE personnel (                         -- fire safety inspectors, team leaders, chief, fire marshal
    id                     TEXT PRIMARY KEY,
    full_name              TEXT NOT NULL,
    rank_title             TEXT,
    position               TEXT,
    station                TEXT,
    default_signature_path TEXT,                 -- optional saved signature for one-tap reuse
    is_active              INTEGER NOT NULL DEFAULT 1,
    created_at             TEXT NOT NULL DEFAULT (datetime('now')),
    updated_at             TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE inspection (
    id             TEXT PRIMARY KEY,
    subject_id     TEXT NOT NULL REFERENCES subject(id),
    template_id    TEXT NOT NULL REFERENCES form_template(id),   -- pins the exact form revision
    reference_no   TEXT,                         -- Inspection Order No. / Control No.
    reference_date TEXT,                         -- date issued
    date_inspected TEXT,
    latitude       REAL,
    longitude      REAL,
    status         TEXT NOT NULL DEFAULT 'DRAFT' CHECK (status IN ('DRAFT','COMPLETED')),
    completed_at   TEXT,
    remarks        TEXT,
    created_at     TEXT NOT NULL DEFAULT (datetime('now')),
    updated_at     TEXT NOT NULL DEFAULT (datetime('now'))
);
CREATE INDEX ix_inspection_subject  ON inspection(subject_id, date_inspected DESC);
CREATE INDEX ix_inspection_template ON inspection(template_id);
CREATE INDEX ix_inspection_status   ON inspection(status, date_inspected DESC);

-- Typed key-value answers
CREATE TABLE inspection_answer (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    inspection_id TEXT NOT NULL REFERENCES inspection(id) ON DELETE CASCADE,
    item_id       TEXT NOT NULL REFERENCES form_item(id),
    slot_key      TEXT NOT NULL DEFAULT 'value',   -- 'value' or a form_column.key
    repeat_no     INTEGER NOT NULL DEFAULT 0,
    repeat_label  TEXT,
    value_text    TEXT,                            -- text, date, choice, YES/NO/NA, PASS/FAIL
    value_num     REAL,                            -- numbers and BOOL (0/1)
    UNIQUE (inspection_id, item_id, slot_key, repeat_no)
);
CREATE INDEX ix_answer_item ON inspection_answer(item_id);

CREATE TABLE inspection_finding (                -- Defects / Deficiencies
    id            TEXT PRIMARY KEY,
    inspection_id TEXT NOT NULL REFERENCES inspection(id) ON DELETE CASCADE,
    item_id       TEXT REFERENCES form_item(id),
    description   TEXT NOT NULL,
    sort_order    INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX ix_finding_inspection ON inspection_finding(inspection_id);

-- Signatures: owner/representative, inspectors, team leader, evaluator, chief, fire marshal
CREATE TABLE inspection_signatory (
    id               TEXT PRIMARY KEY,
    inspection_id    TEXT NOT NULL REFERENCES inspection(id) ON DELETE CASCADE,
    role_code        TEXT NOT NULL REFERENCES signatory_role(code),
    personnel_id     TEXT REFERENCES personnel(id),   -- NULL for the owner/representative
    printed_name     TEXT NOT NULL,                   -- "Signature over printed name"
    position_title   TEXT,
    status           TEXT NOT NULL DEFAULT 'PENDING' CHECK (status IN ('PENDING','SIGNED','REFUSED')),
    signature_path   TEXT,                            -- PNG drawn on the canvas
    signature_sha256 TEXT,                            -- hash of the PNG, detects file tampering
    signed_at        TEXT,                            -- "Date & Time"
    refused_reason   TEXT,
    decision         TEXT,                            -- 'APPROVED' / 'DISAPPROVED' (form 051)
    sort_order       INTEGER NOT NULL DEFAULT 0,
    CHECK (status <> 'SIGNED'  OR (signature_path IS NOT NULL AND signed_at IS NOT NULL)),
    CHECK (status <> 'REFUSED' OR refused_reason IS NOT NULL)
);
CREATE INDEX ix_signatory_inspection ON inspection_signatory(inspection_id, role_code);

CREATE TABLE attachment (                        -- photos, sketches
    id            TEXT PRIMARY KEY,
    inspection_id TEXT NOT NULL REFERENCES inspection(id) ON DELETE CASCADE,
    item_id       TEXT REFERENCES form_item(id),
    finding_id    TEXT REFERENCES inspection_finding(id) ON DELETE SET NULL,
    kind          TEXT NOT NULL DEFAULT 'PHOTO' CHECK (kind IN ('PHOTO','SKETCH','OTHER')),
    file_path     TEXT NOT NULL,
    caption       TEXT,
    tag           TEXT,                          -- photo slot, e.g. 'FRONT' (matches form_choice.value)
    taken_at      TEXT NOT NULL DEFAULT (datetime('now'))
);
CREATE INDEX ix_attachment_inspection ON attachment(inspection_id);

-- ---------------------------------------------------------------------
-- C. INTEGRITY + VIEWS
-- ---------------------------------------------------------------------
-- An answer's item must belong to the inspection's template
CREATE TRIGGER trg_answer_template BEFORE INSERT ON inspection_answer
WHEN (SELECT template_id FROM form_item WHERE id = NEW.item_id)
  <> (SELECT template_id FROM inspection WHERE id = NEW.inspection_id)
BEGIN
    SELECT RAISE(ABORT, 'item does not belong to the inspection''s template');
END;

-- Lock answers and findings once an inspection is COMPLETED
-- (reopen = set status back to 'DRAFT' on purpose)
CREATE TRIGGER trg_answer_lock_ins BEFORE INSERT ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = NEW.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

CREATE TRIGGER trg_answer_lock_upd BEFORE UPDATE ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = OLD.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

CREATE TRIGGER trg_answer_lock_del BEFORE DELETE ON inspection_answer
WHEN (SELECT status FROM inspection WHERE id = OLD.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

CREATE TRIGGER trg_finding_lock_ins BEFORE INSERT ON inspection_finding
WHEN (SELECT status FROM inspection WHERE id = NEW.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

CREATE TRIGGER trg_finding_lock_upd BEFORE UPDATE ON inspection_finding
WHEN (SELECT status FROM inspection WHERE id = OLD.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

CREATE TRIGGER trg_finding_lock_del BEFORE DELETE ON inspection_finding
WHEN (SELECT status FROM inspection WHERE id = OLD.inspection_id) = 'COMPLETED'
BEGIN SELECT RAISE(ABORT, 'inspection is completed'); END;

-- Filled form, flat (UI / PDF export)
CREATE VIEW v_inspection_answers AS
SELECT a.inspection_id, sec.title AS section, sec.sort_order AS section_order,
       i.sort_order AS item_order, i.key AS item_key, i.label AS item_label,
       a.slot_key, a.repeat_no, a.repeat_label, a.value_text, a.value_num, i.unit
FROM inspection_answer a
JOIN form_item i      ON i.id = a.item_id
JOIN form_section sec ON sec.id = i.section_id;

-- On-site signatures still missing (drives the "Complete inspection" check)
CREATE VIEW v_missing_signatures AS
SELECT i.id AS inspection_id, r.role_code, r.min_count,
       (SELECT COUNT(*) FROM inspection_signatory s
         WHERE s.inspection_id = i.id AND s.role_code = r.role_code
           AND s.status IN ('SIGNED','REFUSED')) AS have
FROM inspection i
JOIN form_template_signatory r ON r.template_id = i.template_id
WHERE r.stage = 'ON_SITE' AND r.is_required = 1
  AND (SELECT COUNT(*) FROM inspection_signatory s
        WHERE s.inspection_id = i.id AND s.role_code = r.role_code
          AND s.status IN ('SIGNED','REFUSED')) < r.min_count;

-- Quick look at every form: which one it is, and whether its questions are loaded
CREATE VIEW v_form_overview AS
SELECT t.id AS template_id, t.code, t.display_name, t.subject_type, t.purpose, t.revision,
       (SELECT COUNT(*) FROM form_section s WHERE s.template_id = t.id) AS sections,
       (SELECT COUNT(*) FROM form_item i WHERE i.template_id = t.id) AS items,
       CASE WHEN EXISTS (SELECT 1 FROM form_item i WHERE i.template_id = t.id)
            THEN 'READY' ELSE 'EMPTY' END AS status
FROM form_template t
ORDER BY t.sort_order;

-- =====================================================================
-- D. SEED (items, sections, choices are loaded from per-form JSON files)
-- =====================================================================
INSERT INTO subject_type (code, label) VALUES
 ('ESTABLISHMENT','Building or business'),
 ('VEHICLE','Vehicle'),
 ('CONSTRUCTION_PROJECT','Construction project'),
 ('OTHER','Other');

INSERT INTO signatory_role (code, label) VALUES
 ('OWNER_REP','Owner / Representative'),
 ('INSPECTOR','Fire Safety Inspector'),
 ('TEAM_LEADER','Team Leader'),
 ('BUILDING_PLAN_EVALUATOR','Building Plan Evaluator'),
 ('CHIEF_FSES','Chief, Fire Safety Enforcement Section/Unit'),
 ('FIRE_MARSHAL','City / Municipal Fire Marshal');

INSERT INTO form_column_set (id, code) VALUES
 ('cs-yn','YES_NO'),
 ('cs-pfr','PASS_FAIL_REMARKS'),
 ('cs-icr','INSTALLED_COMPLIANT_REMARKS'),
 ('cs-ccp','COMPLIANT_CHANGE_PARTICULARS');

INSERT INTO form_column (id, column_set_id, key, label, data_type, sort_order) VALUES
 ('c-yn-1','cs-yn','result','Yes / No','YES_NO',1),
 ('c-pfr-1','cs-pfr','result','Passed / Failed','PASS_FAIL',1),
 ('c-pfr-2','cs-pfr','remarks','Remarks / Corrective Action','TEXT',2),
 ('c-icr-1','cs-icr','installed','Installed/Constructed?','YES_NO_NA',1),
 ('c-icr-2','cs-icr','compliant','Compliant','YES_NO_NA',2),
 ('c-icr-3','cs-icr','remarks','Remarks','TEXT',3),
 ('c-ccp-1','cs-ccp','compliant','Compliant','YES_NO',1),
 ('c-ccp-2','cs-ccp','change_status','Changes','CHOICE',2),
 ('c-ccp-3','cs-ccp','particulars','Particulars','TEXT',3);

INSERT INTO form_choice (id, column_id, value, label, sort_order) VALUES
 ('ch-ccp-1','c-ccp-2','NO_CHANGES','No changes',1),
 ('ch-ccp-2','c-ccp-2','WITH_CHANGES','With changes',2);

-- Extra answer layouts used by the other BFP forms
INSERT INTO form_column_set (id, code) VALUES
 ('cs-ynr','YES_NO_REMARKS'),
 ('cs-pfnr','PASS_FAIL_NA_REMARKS'),
 ('cs-compliant','COMPLIANT_YES_NO_NA');

INSERT INTO form_column (id, column_set_id, key, label, data_type, sort_order) VALUES
 ('c-ynr-1','cs-ynr','result','Yes / No','YES_NO',1),
 ('c-ynr-2','cs-ynr','remarks','Remarks','TEXT',2),
 ('c-pfnr-1','cs-pfnr','result','Passed / Failed / N/A','CHOICE',1),
 ('c-pfnr-2','cs-pfnr','remarks','Remarks / Corrective Action','TEXT',2),
 ('c-comp-1','cs-compliant','compliant','Compliant','YES_NO_NA',1);

INSERT INTO form_choice (id, column_id, value, label, sort_order) VALUES
 ('ch-pfnr-1','c-pfnr-1','PASS','Passed',1),
 ('ch-pfnr-2','c-pfnr-1','FAIL','Failed',2),
 ('ch-pfnr-3','c-pfnr-1','NA','N/A',3);

INSERT INTO form_template (id, code, revision, revision_date, name, display_name, subject_type, purpose, sort_order) VALUES
 ('fsic-renewal','BFP-QSF-FSED-069','01','2024-08-11','Fire Safety Inspection Checklist for the Renewal of FSIC','FSIC renewal','ESTABLISHMENT','RENEWAL',1),
 ('fsic-new-occupancy','BFP-QSF-FSED-061','02',NULL,'Fire Safety Inspection Checklist for New FSIC and FSIC for Certificate of Occupancy','New FSIC / occupancy','ESTABLISHMENT','NEW',2),
 ('conveyance-clearance','BFP-QSF-FSED-051','00','2020-02-03','Fire Safety Inspection Checklist for Conveyance Clearance','Conveyance clearance','VEHICLE','CONVEYANCE',3),
 ('during-construction','BFP-QSF-FSED-068','01',NULL,'Inspection Checklist During Construction','During construction','CONSTRUCTION_PROJECT','DURING_CONSTRUCTION',4);

-- Required signatures: the same set for the four inspection forms (other forms set their own)...
WITH r(role_code, stage, req, sort) AS (VALUES
 ('OWNER_REP','ON_SITE',1,1),('INSPECTOR','ON_SITE',1,2),('TEAM_LEADER','ON_SITE',1,3),
 ('CHIEF_FSES','APPROVAL',0,4),('FIRE_MARSHAL','APPROVAL',0,5))
INSERT INTO form_template_signatory (template_id, role_code, stage, is_required, sort_order)
SELECT t.id, r.role_code, r.stage, r.req, r.sort FROM form_template t CROSS JOIN r
WHERE t.id IN ('fsic-renewal','fsic-new-occupancy','conveyance-clearance','during-construction');

-- ...plus the Building Plan Evaluator on the construction form
INSERT INTO form_template_signatory (template_id, role_code, stage, is_required, sort_order)
VALUES ('during-construction','BUILDING_PLAN_EVALUATOR','ON_SITE',1,0);
