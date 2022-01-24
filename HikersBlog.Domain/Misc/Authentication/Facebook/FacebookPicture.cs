namespace HikersBlog.Domain.Misc.Authentication.Facebook;

public class FacebookPicture
{
    public FacebookPictureData data { get; set; }
}

public class FacebookPictureData
{
    public int height { get; set; }
    public bool is_silhouette { get; set; }
    public string url { get; set; }
    public int width { get; set; }
}