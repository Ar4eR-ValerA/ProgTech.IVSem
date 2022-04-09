namespace Client;

public class RequesterExample
{
    private static HttpClient _client;

    public RequesterExample()
    {
        _client = new HttpClient();
    }

    public async Task Post1(CustomerCreateDto customerCreateDto)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(customerCreateDto.name), "name");
        content.Add(new StringContent(customerCreateDto.email), "email");

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