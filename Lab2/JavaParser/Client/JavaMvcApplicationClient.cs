using System.Text.Json;

namespace Client;
public class JavaMvcApplication
{
    private static HttpClient _client = new HttpClient();
    public async Task<int> create(CustomerCreateDto customerCreateDto)
    {
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(customerCreateDto.customerCreateDto), "customerCreateDto");
            var response = await _client.PostAsync("http://localhost:8080/create", content);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<int>(responseString);
        }
    }

    public async Task<List<CustomerGetDto>> getAll()
    {
        {
            var content = new StringContent($"");
            var response = await _client.GetAsync("http://localhost:8080/getAll", content);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CustomerGetDto>>(responseString);
        }
    }
}