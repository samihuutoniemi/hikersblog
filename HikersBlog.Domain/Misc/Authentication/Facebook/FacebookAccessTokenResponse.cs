﻿namespace HikersBlog.Domain.Misc.Authentication.Facebook;

public class FacebookAccessTokenResponse
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public long expires_in { get; set; }
}
