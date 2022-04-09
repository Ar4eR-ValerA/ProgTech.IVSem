namespace Client;
public class TestDto
{
    public TestDto(List<String> stringArray)
    {
        this.stringArray = stringArray;
    }

    public List<String> stringArray { get; set; }
}