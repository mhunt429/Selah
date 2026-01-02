ALTER TABLE account_connector ADD COLUMN requires_reauthentication BOOLEAN DEFAULT FALSE;

ALTER TABLE account_connector ADD COLUMN last_sync_date TIMESTAMPTZ;

ALTER TABLE account_connector ADD COLUMN next_sync_date TIMESTAMPTZ;

/*
ROLLBACK 
ALTER TABLE account_connector DROP COLUMN requires_reauthentication;
ALTER TABLE account_connector DROP COLUMN last_sync_date;
ALTER TABLE account_connector DROP COLUMN next_sync_date;

DELETE FROM flyway_schema_history WHERE script = 'V16_Add_New_Account_Connector_Columns.sql';
*/