ALTER TABLE account_connector ADD COLUMN disconnected_ts TIMESTAMPTZ;

/*
ROLLBACK;
ALTER TABLE account_connector DROP COLUMN disconnected;
DELETE FROM flyway_schema_history where script = 'V14__Add_Account_Connector_Disconnected_Column.sql'
*/