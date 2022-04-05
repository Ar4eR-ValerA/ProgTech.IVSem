using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Client;

public class Requester
{
    private static HttpClient _client;

    public Requester()
    {
        _client = new HttpClient();
    }

    public async Task Post()
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent("valera"), "name");
        content.Add(new StringContent("ca@vsd"), "email");

        var response = await _client.PostAsync("http://localhost:8080/create", content);
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseString);
    }

    public async Task Get()
    {
        var response = await _client.GetStringAsync("http://localhost:8080/getAll");
        Console.WriteLine(response);
    }
}

class CustomerCreateDto
{
    public CustomerCreateDto(string name, string email)
    {
        this.name = name;
        this.email = email;
    }

    public string name { get; set; }
    public string email { get; set; }
}