CREATE TABLE user_mailbox(
    id SERIAL PRIMARY KEY,
    message_key VARCHAR(100) NOT NULL,
    user_id INT NOT NULL REFERENCES app_user(id) ON DELETE SET NULL,
    has_seen BOOLEAN NOT NULL,
    message_body TEXT NOT NULL,
    action_type VARCHAR(20) NOT NULL
) INHERITS(base_audit_table);

CREATE TRIGGER set_audit_timestamps 
    BEFORE INSERT OR UPDATE ON user_mailbox
    FOR EACH ROW
    EXECUTE FUNCTION set_audit_timestamps();

CREATE INDEX idx_user_mailbox_user_id ON user_mailbox(user_id);
CREATE INDEX idx_user_mailbox_message_key ON user_mailbox(message_key);

/*
ROLLBACK 
DROP INDEX idx_user_mailbox_user_id;
DROP INDEX idx_user_mailbox_message_key;
DROP TABLE user_mailbox;

DELETE FROM flyway_schema_history WHERE script = 'V15__Create_User_Mailbox_Table.sql';
*/


