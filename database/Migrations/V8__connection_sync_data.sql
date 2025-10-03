CREATE TABLE connection_sync_data(
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES app_user(id),
    sync_type VARCHAR(40), -- AccountBalance, Investments, Transactions, RecurringTransactions
    last_sync_date TIMESTAMPTZ,
    next_sync_date TIMESTAMPTZ,
    connector_id INT REFERENCES account_connector(id) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED
) INHERITS(base_audit_table);

CREATE TRIGGER set_audit_timestamps
BEFORE INSERT OR UPDATE ON connection_sync_data
FOR EACH ROW
EXECUTE FUNCTION set_audit_timestamps();

CREATE INDEX connection_sync_userId on connection_sync_data(user_id);

/*
 ROLLBACK
 DROP INDEX connection_sync_userId;
 DROP TABLE connection_sync_data;
 DELETE FROM fly_schema_history where script = 'V8__connection_sync_data.sql';
 */