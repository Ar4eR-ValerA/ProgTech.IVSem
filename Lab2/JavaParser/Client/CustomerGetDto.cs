namespace Client;
class CustomerGetDto
{
    public CustomerGetDto(int id, String name, String email)
    {
        this.id = id;
        this.name = name;
        this.email = email;
    }

    public int id { get; set; }

    public String name { get; set; }

    public String email { get; set; }
}