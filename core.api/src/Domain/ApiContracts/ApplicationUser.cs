namespace Domain.ApiContracts;

public class ApplicationUser
{
    public required int Id { get; set; }
    
    public required int AccountId { get; set; }
    
    public required string Email { get; set; } 
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public string PhoneNumber { get; set; } = String.Empty;
    
    public DateTimeOffset CreatedDate { get; set; }
    
}