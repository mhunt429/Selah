namespace Domain.Models.DbUtils;

public record SortParameters(string SortColumn, string SortDirection);


public static class DbUtils
{
    public static readonly Dictionary<string, string> SortColumnMap = new()
    {
        ["id"] = "id",
        ["transactionDate"] = "transaction_date",
        ["amount"] = "amount"
    };
    
    public static string NormalizeSortDirection(string direction)
    {
        return direction?.ToUpperInvariant() == "DESC" ? "DESC" : "ASC";
    }
}