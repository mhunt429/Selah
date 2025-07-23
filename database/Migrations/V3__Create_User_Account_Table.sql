CREATE TABLE user_account
(
    id           SERIAL PRIMARY KEY,
    created_on   TIMESTAMPTZ,
    account_name VARCHAR(20)
) INHERITS(base_audit_table);

/*
 Rollback
 DELETE FROM flyway_schema_history where script = 'V3__Create_User_Account_Table.sql';
 DROP TABLE user_account CASCADE;
 */