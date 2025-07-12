namespace Domain.Constants;

public static class SqlQueries
{
    public const string InsertIntoAccount =
        @"INSERT INTO Account (original_insert, last_update, id, app_last_changed_by, date_created, account_name) VALUES(@original_insert, @last_update, @id, @app_last_changed_by,  @date_created, @account_name)";

    public const string InsertIntoAppUser =
        @"INSERT INTO app_user(original_insert, last_update, app_last_changed_by,  id, account_id, created_date, encrypted_email, username, password,
                     encrypted_name, encrypted_phone, last_login, last_login_ip, phone_verified, email_verified, email_hash) 
        VALUES(@original_insert, @last_update, @app_last_changed_by, @id,  @account_id, @created_date, 
               @encrypted_email, @username, @password, @encrypted_name, 
               @encrypted_phone, @last_login, 
               @last_login_ip, @phone_verified, @email_verified, @email_hash)";

    public const string GetUserById = "SELECT * FROM app_user WHERE id=@id";

    public const string GetUserByEmail =
        "SELECT * FROM app_user WHERE email_hash=@email_hash LIMIT 1";

    public const string InsertIntoAccountConnector =
        @"INSERT INTO account_connector (original_insert, last_update, app_last_changed_by, user_id, institution_id, institution_name, date_connected, encrypted_access_token)
            VALUES(@original_insert, @last_update, @app_last_changed_by,  @user_id, @institution_id, @institution_name, @date_connected, @encrypted_access_token) RETURNING id";

    public const string InsertIntoFinancialAccount = @"INSERT INTO financial_accounts(original_insert, last_update, 
                              app_last_changed_by, user_id, 
                              connector_id, 
                              external_id, current_balance, 
                              display_name, official_name, 
                              subtype, is_external_api_import,
                                  last_api_import_time)
                            VALUES(@original_insert, @last_update, 
                                   @app_last_changed_by, @user_id, 
                                   @connector_id, @external_id, 
                                   @current_balance, @display_name, 
                                   @official_name, @subtype, 
                                   @is_external_api_import, 
                                   @last_api_import_time)RETURNING id";

    public const string GetFinancialAccountsByUserId =
        "SELECT * FROM financial_accounts WHERE user_id=@user_id";
    
    public const string GetFinancialAccountsById =
        "SELECT * FROM financial_accounts WHERE user_id=@user_id AND id = @id";
    
    public const string UpdateAccountBalance = @"UPDATE financial_accounts SET current_balance= @current_balance WHERE user_id = @user_id AND id=@id";
    
    public const string DeleteFinancialAccount = @"DELETE FROM financial_accounts WHERE user_id = @user_id  AND id=@id";
}