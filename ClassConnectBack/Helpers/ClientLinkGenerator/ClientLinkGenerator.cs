using Microsoft.Extensions.Options;

namespace ClassConnect.Helpers;

public class ClientLinkGenerator
{
    private IOptions<ClientSettings> _client;

    public ClientLinkGenerator(IOptions<ClientSettings> client)
    {
        _client = client;
    }

    public string GenerateLink(string path) => $"{_client.Value.Url}{path}";
}
