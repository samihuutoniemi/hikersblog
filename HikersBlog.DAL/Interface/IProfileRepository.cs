using HikersBlog.Domain.Models;

namespace HikersBlog.DAL.Interface
{
    public interface IProfileRepository
    {
        Profile GetProfile(Guid userId);
        Profile GetProfile(string urlName);
    }
}