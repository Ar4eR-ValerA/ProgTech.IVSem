using System.Net.Http.Json;
using System.Text.Json;

namespace Client;
public class JavaMvcApplication
{
    private static HttpClient _client = new HttpClient();
    public async Task<int> create(CustomerCreateDto customerCreateDto)
    {
        {
            var content = JsonContent.Create(customerCreateDto);
            var response = await _client.PostAsync("http://localhost:8080/create", content);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<int>(responseString);
        }
    }

    public async Task<int> onlyName(String name)
    {
        {
            var content = $"?name={name}";
            var response = await _client.PostAsync("http://localhost:8080/onlyName" + content, new StringContent(""));
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<int>(responseString);
        }
    }

    public async Task<List<CustomerGetDto>> getAll()
    {
        {
            var content = $"";
            var response = await _client.GetAsync("http://localhost:8080/getAll" + content);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CustomerGetDto>>(responseString);
        }
    }

    public async Task<bool> isNumber(int number)
    {
        {
            var content = $"?number={number}";
            var response = await _client.GetAsync("http://localhost:8080/isNumber" + content);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseString);
        }
    }

    public async Task<List<CustomerGetDto>> getByName(String name)
    {
        {
            var content = $"?name={name}";
            var response = await _client.GetAsync("http://localhost:8080/getByName" + content);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CustomerGetDto>>(responseString);
        }
    }

    public async Task<bool> changeEmailToItmo(int id)
    {
        {
            var content = $"?id={id}";
            var response = await _client.PatchAsync("http://localhost:8080/{id}" + content, new StringContent(""));
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseString);
        }
    }
}