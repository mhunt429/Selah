CREATE TABLE account_connector
(
    id                      SERIAL PRIMARY KEY,
    user_id                 INT REFERENCES app_user (id) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED,
    institution_id          TEXT,
    institution_name        TEXT,
    date_connected          TIMESTAMPTZ,
    encrypted_access_token  VARCHAR(64),
    transaction_sync_cursor TEXT,
    external_event_id       TEXT
) INHERITS(base_audit_table);

CREATE INDEX ac_userId ON account_connector (user_id);

/*
 ROLLBACK 
 DROP INDEX ac_userId;
DELETE FROM flyway_schema_history WHERE script = 'V5__Create_Account_Connector_Table.sql';
 DROP TABLE account_connector CASCADE;
 */