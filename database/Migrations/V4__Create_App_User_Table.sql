CREATE TABLE app_user
(
    id              SERIAL PRIMARY KEY,
    account_id      SERIAL REFERENCES user_account(id) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED,
    created_date    TIMESTAMPTZ,
    encrypted_email VARCHAR(64),
    password        VARCHAR(64),
    encrypted_name  VARCHAR(64),
    encrypted_phone VARCHAR(64),
    last_login_date TIMESTAMPTZ,
    last_login_ip   VARCHAR(39), --supports ipv6 and v4
    phone_verified  BOOLEAN,
    email_verified  BOOLEAN,
    email_hash      CHAR(64)
)INHERITS(base_audit_table);

CREATE INDEX au_email_hash_indx ON app_user (email_hash);

CREATE INDEX au_user_id ON app_user (account_id)

/*
 ROLLBACK 
 DROP INDEX au_email_hash_indx;
 DROP index au_username_hash_indx;
 DROP index au_user_id;
 DELETE FROM flyway_schema_history WHERE script = 'V4__Create_App_User_Table.sql';
 DROP TABLE app_user CASCADE
 */