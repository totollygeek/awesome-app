namespace TOTOllyGeek.Awesome.Dump;

public class Person
{
    private readonly string _firstName;
    private readonly string _lastName;
    
    // ReSharper disable once ConvertToAutoProperty
    public string FirstName
    {
        get => _firstName;
        init => _firstName = value;
    }
    
    // ReSharper disable once ConvertToAutoProperty
    public string LastName 
    {
        get => _lastName;
        init => _lastName = value;
    }
    
    public ContactInfo Contact { get; init; }
    public Job Occupation { get; init; }
}

public class ContactInfo
{
    public Address HomeAddress { get; init; }
    public string Email { get; init; }
}

public class Address
{
    public string Street { get; init; }
    public string City { get; init; }
    public string ZipCode { get; init; }

    private string CityAndZip => $"{ZipCode}, {City}";
    
    public Person Resident { get; set; }
}

public class Job
{
    public string Title { get; init; }
    public int Salary { get; init; }
}
