namespace Client;
public class CustomerCreateDto
{
    public CustomerCreateDto(String name, String email)
    {
        this.name = name;
        this.email = email;
    }

    public String name { get; set; }

    public String email { get; set; }
}