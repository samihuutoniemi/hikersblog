using HikersBlog.DAL.Context;
using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Models;

namespace HikersBlog.DAL;

public class ProfileRepository : IProfileRepository
{
    private readonly ApplicationDbContext _context;

    public ProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Profile GetProfile(Guid userId)
    {
        var result = (from profile in _context.Profiles
                      where profile.UserId == userId
                      select profile).FirstOrDefault();

        return result;
    }

    public Profile GetProfile(string urlName)
    {
        var result = (from profile in _context.Profiles
                      where profile.UserName == urlName
                      select profile).FirstOrDefault();

        return result;
    }
}
