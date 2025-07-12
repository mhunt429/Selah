namespace Domain.ApiContracts;

public class ApplicationUser
{
    public required Guid Id { get; set; }
    
    public required Guid AccountId { get; set; }
    
    public required string Email { get; set; } 
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public string PhoneNumber { get; set; } = String.Empty;
    
    public DateTimeOffset CreatedDate { get; set; }
    
}