CREATE TABLE user_session(
    id UUID PRIMARY KEY,
    user_id INT REFERENCES app_user(id) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED,
    issued_at TIMESTAMPTZ,
    expires_at TIMESTAMPTZ
)INHERITS(base_audit_table);

CREATE TRIGGER set_audit_timestamps
BEFORE INSERT OR UPDATE ON user_session
FOR EACH ROW
EXECUTE FUNCTION set_audit_timestamps();


CREATE INDEX user_session_user_id ON user_session(user_id);

/*
 ROLLBACK;
 DROP INDEX user_session_user_id;
 DELETE FROM flyway_schema_history where script = 'V7__Create_UserSession_Table.sql';
 DROP TABLE user_session CASCADE;
 */