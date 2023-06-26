using Microsoft.Extensions.Options;

namespace ClassConnect.Helpers;

public class ClientLinkGenerator
{
    private IOptions<Client> _client;

    public ClientLinkGenerator(IOptions<Client> client)
    {
        _client = client;
    }

    public string GenerateLink(string path) => $"{_client.Value.Url}{path}";
}
