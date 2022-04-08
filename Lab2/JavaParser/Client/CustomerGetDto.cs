namespace Client;
class CustomerGetDto
{
    public CustomerGetDto(int id, string name, string email)
    {
        this.id = id;
        this.name = name;
        this.email = email;
    }

    public int id { get; set; }

    public string name { get; set; }

    public string email { get; set; }
}