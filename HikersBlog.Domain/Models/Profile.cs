using System.ComponentModel.DataAnnotations;

namespace HikersBlog.Domain.Models;

public class Profile
{
    [Key]
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
}
