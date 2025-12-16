CREATE TABLE transaction (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES app_user(id) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED,
    amount DECIMAL(10, 2) NOT NULL,
    transaction_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    imported_date TIMESTAMPTZ,
    pending BOOLEAN,
    account_id INT NOT NULL REFERENCES financial_account(id) ON DELETE CASCADE DEFERRABLE INITIALLY DEFERRED,
    merchant_name VARCHAR(100),
    transaction_name VARCHAR(100),
    external_transaction_id VARCHAR(100),
    merchant_logo_url VARCHAR(200)
) INHERITS(base_audit_table);


CREATE TRIGGER set_audit_timestamps
BEFORE INSERT OR UPDATE ON transaction
FOR EACH ROW
EXECUTE FUNCTION set_audit_timestamps();    

CREATE INDEX transaction_user_id ON transaction(user_id);
CREATE INDEX transaction_account_id ON transaction(account_id);
CREATE INDEX transaction_transaction_date ON transaction(transaction_date);

/*
ROLLBACK
DROP INDEX transaction_transaction_date;
DROP INDEX transaction_account_id;
DROP INDEX transaction_user_id;
DROP TABLE transaction;
DELETE FROM flyway_schema_history where script = 'V12__Create_Transaction_Table.sql';

*/

CREATE TABLE transaction_line_item (
    id SERIAL PRIMARY KEY,
    transaction_id INT NOT NULL REFERENCES transaction(id) ON DELETE CASCADE DEFERRABLE INITIALLY DEFERRED,
    description VARCHAR,
    amount DECIMAL(10, 2) NOT NULL,
    category_id INT REFERENCES user_transaction_category(id) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED
) INHERITS(base_audit_table);

CREATE TRIGGER set_audit_timestamps
BEFORE INSERT OR UPDATE ON transaction_line_item
FOR EACH ROW
EXECUTE FUNCTION set_audit_timestamps();

CREATE INDEX transaction_line_item_transaction_id ON transaction_line_item(transaction_id);

CREATE INDEX transaction_line_item_category_id ON transaction_line_item(category_id);

/*
ROLLBACK

DROP INDEX transaction_line_item_transaction_id;
DROP TABLE transaction_line_item;
DELETE FROM flyway_schema_history where script = 'V12__Create_Transaction_Table.sql';
*/
