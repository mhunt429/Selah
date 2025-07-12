CREATE TABLE base_audit_table
(
    original_insert     TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_update         TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    app_last_changed_by UUID        NOT NULL
);