using HikersBlog.Domain.Models;

namespace HikersBlog.DAL.Interface;

public interface IExternalUserRepository
{
    ExternalUser GetByUserId(string userId);
    ExternalUser Get(string externalId);
    ExternalUser Save(ExternalUser externalUser);
    ExternalUser Forget(ExternalUser externalUser);
}
