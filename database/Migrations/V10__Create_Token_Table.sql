CREATE TABLE token(
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES app_user(id) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED, 
    token VARCHAR,
    token_type VARCHAR(20),
    created_at TIMESTAMPTZ,
    expires_at TIMESTAMPTZ
) INHERITS(base_audit_table);

CREATE TRIGGER set_audit_timestamps
BEFORE INSERT OR UPDATE ON token
FOR EACH ROW
EXECUTE FUNCTION set_audit_timestamps();

CREATE INDEX token_user_id ON token(user_id);

/*
ROLLBACK
DROP INDEX token_user_id;
DROP TABLE token;
DELETE FROM flyway_schema_history where script = 'V10__Create_Token_Table.sql';
*/