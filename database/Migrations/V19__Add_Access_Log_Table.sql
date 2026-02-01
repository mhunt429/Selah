CREATE TABLE access_log(
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES app_user(id) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED,
    access_type VARCHAR(40),
    access_date TIMESTAMPTZ,
    ip_address VARCHAR(40),
    user_agent VARCHAR(255),
    success BOOLEAN
) INHERITS(base_audit_table);

/*
ROLLBACK
DROP INDEX access_log_user_id;
DROP TABLE access_log;
DELETE FROM flyway_schema_history where script = 'V19__Add_Access_Log_Table.sql';
*/
