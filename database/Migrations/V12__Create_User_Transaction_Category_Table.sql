CREATE TABLE user_transaction_category (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES app_user(id) ON DELETE CASCADE DEFERRABLE INITIALLY DEFERRED,
    category_name VARCHAR (50) NOT NULL
) INHERITS(base_audit_table);
CREATE TRIGGER set_audit_timestamps
BEFORE INSERT OR UPDATE ON user_transaction_category
FOR EACH ROW
EXECUTE FUNCTION set_audit_timestamps();
CREATE INDEX user_transaction_category_user_id ON user_transaction_category(user_id);

/*
ROLLBACK
DROP INDEX user_transaction_category_user_id;
DROP TABLE user_transaction_category;
DELETE FROM flyway_schema_history where script = 'V13__Create_User_Transaction_Category_Table.sql';
*/