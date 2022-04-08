namespace Client;
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