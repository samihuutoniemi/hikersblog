namespace HikersBlog.Domain.Misc.Authentication.Facebook;

public class FacebookUser
{
    public string id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public FacebookPicture picture { get; set; }
}