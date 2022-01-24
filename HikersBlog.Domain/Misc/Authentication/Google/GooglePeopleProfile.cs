namespace HikersBlog.Domain.Misc.Authentication.Google;

public class GooglePeopleProfile
{
    public string resourceName { get; set; }
    public GoogleName[] names { get; set; }
    public GoogleCoverPhoto[] coverPhotos { get; set; }
    public GoogleEmail[] emailAddresses { get; set; }
}

public class GoogleName
{
    public string displayName { get; set; }
}

public class GoogleCoverPhoto
{
    public string url { get; set; }
    public bool @default { get; set; }
}

public class GoogleEmail
{
    public string value { get; set; }
}