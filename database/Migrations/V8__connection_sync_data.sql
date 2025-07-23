CREATE TABLE connection_sync_data(
    id SERIAL PRIMARY KEY,
    user_id SERIAL REFERENCES app_user(id),
    sync_type VARCHAR(20), -- AccountBalance, Investments, Transactions, RecurringTransactions
    last_sync_date TIMESTAMPTZ,
    next_sync_date TIMESTAMPTZ
) INHERITS(base_audit_table);

CREATE INDEX connection_sync_userId on connection_sync_data(user_id);

/*
 ROLLBACK
 DROP INDEX connection_sync_userId;
 DROP TABLE connection_sync_data;
 DELETE FROM fly_schema_history where script = 'V8__connection_sync_data.sql';
 */