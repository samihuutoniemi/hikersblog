using Microsoft.AspNetCore.Components.Forms;

namespace HikersBlog.Domain.Models;

public class File : IBrowserFile
{
    public string Name { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public long Size { get; set; }

    public string ContentType { get; set; }

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
