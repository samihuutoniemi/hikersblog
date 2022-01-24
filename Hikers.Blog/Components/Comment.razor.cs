using HikersBlog.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace HikersBlog.Components;

public class CommentBase : ComponentBase
{
    [Parameter]
    public Domain.Models.Comment CommentObject { get; set; }

    [Parameter]
    public bool IsSubComment { get; set; }

    [Parameter]
    public ExternalUser MeUser { get; set; }

    [Parameter]
    public EventCallback<int> Like { get; set; }

    [Parameter]
    public EventCallback<int> SetCommentContextCommentId { get; set; }
}
