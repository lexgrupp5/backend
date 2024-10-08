namespace Domain.Entities;

#nullable disable

public class Role
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    //Navigation
    public ICollection<User> Users { get; set; }    
}