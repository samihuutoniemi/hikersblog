using HikersBlog.DAL.Context;
using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Models;

namespace HikersBlog.DAL;

public class ExternalUserRepository : IExternalUserRepository
{
    private readonly ApplicationDbContext _context;

    public ExternalUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public ExternalUser Get(string externalId)
    {
        var result = (from eu in _context.ExternalUsers
                      where eu.ExternalId == externalId
                      select eu).FirstOrDefault();

        return result;
    }


    public ExternalUser GetByUserId(string userId)
    {
        var id = int.Parse(userId.Split('|').FirstOrDefault());
        var externalId = userId.Split('|').LastOrDefault();

        var result = (from eu in _context.ExternalUsers
                      where eu.Id == id
                      && eu.ExternalId == externalId
                      select eu).FirstOrDefault();

        return result;
    }

    public ExternalUser Save(ExternalUser externalUser)
    {
        var eu = Get(externalUser.ExternalId);

        if (eu == null)
        {
            eu = externalUser;
            _context.Add(eu);
        }
        else
        {
            eu.Name = externalUser.Name;
            eu.ImageUrl = externalUser.ImageUrl;
            eu.AccessToken = externalUser.AccessToken;
            eu.AccessTokenExpiresAt = externalUser.AccessTokenExpiresAt;
            eu.RefreshToken = externalUser.RefreshToken;
        }

        _context.SaveChanges();

        return eu;
    }
}
