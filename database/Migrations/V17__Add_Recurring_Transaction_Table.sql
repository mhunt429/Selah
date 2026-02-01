CREATE TABLE recurring_transaction (
     id SERIAL PRIMARY KEY,
     user_id INT NOT NULL REFERENCES app_user(id) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED,
     account_id INT NOT NULL REFERENCES financial_account(id) ON DELETE CASCADE DEFERRABLE INITIALLY DEFERRED,
     external_id TEXT,
     primary_description TEXT,
     detailed_description TEXT,
     category_id INT REFERENCES user_transaction_category(id) ON DELETE CASCADE DEFERRABLE INITIALLY DEFERRED,
     average_amount DECIMAL,
     last_amount DECIMAL,
     first_date TIMESTAMPTZ,
     last_date TIMESTAMPTZ,
     predicted_next_date TIMESTAMPTZ,
     frequency VARCHAR(30)
) INHERITS(base_audit_table);

CREATE TRIGGER set_audit_timestamps
BEFORE INSERT OR UPDATE ON recurring_transaction
FOR EACH ROW
EXECUTE FUNCTION set_audit_timestamps();    

CREATE INDEX recurring_transaction_user_id on recurring_transaction(user_id);
CREATE INDEX recurring_transaction_external_id ON recurring_transaction(external_id);
CREATE INDEX recurring_transaction_category_id ON recurring_transaction(category_id);
CREATE INDEX recurring_transaction_account_id ON recurring_transaction(account_id);

ALTER TABLE transaction ADD COLUMN recurring_transaction_id INT REFERENCES recurring_transaction(id) ON DELETE CASCADE DEFERRABLE INITIALLY DEFERRED;

/*
ROLLBACK
DROP INDEX recurring_transaction_user_id;
DROP INDEX recurring_transaction_external_id;
DROP INDEX recurring_transaction_category_id;
DROP INDEX recurring_transaction_account_id;
ALTER TABLE transaction DROP COLUMN recurring_transaction_id;
DROP TABLE recurring_transaction;
DELETE FROM flyway_schema_history where script = 'V17__Add_Recurring_Transaction_Table.sql';
*/

