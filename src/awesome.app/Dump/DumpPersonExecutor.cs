#nullable enable
using System;
using Dumpify;
using Spectre.Console;

namespace TOTOllyGeek.Awesome.Dump;

internal class DumpPersonExecutor(
    Person? person = null, 
    bool? showPrivateFields = null,
    bool? showPrivateMembers = null,
    bool? showSalary = null) : OperationExecutor
{
    public override string OperationName => "Dump Objects";
    public override int Execute()
    {
        var dumpPerson = person ?? GeneratePerson();
        var includeFields = showPrivateFields ?? false;
        var includeNonePublicMembers = showPrivateMembers ?? false;
        var includeSalary = showSalary ?? false;

        var config = new MembersConfig
        {
            IncludeFields = includeFields,
            IncludeNonPublicMembers = includeNonePublicMembers,
            MemberFilter = member => !member.Name.Equals("Salary", StringComparison.OrdinalIgnoreCase) || includeSalary
        };

        dumpPerson.Dump(
            "Dump of a `Person` object",
            members: config);
    
        return 0;
    }

    private static Person GeneratePerson()
    {
        var homeAddress = new Address
        {
            Street = "1 Nedelya Sq.",
            City = "Sofia",
            ZipCode = "1000"
        };
        
        var contact = new ContactInfo
        {
            HomeAddress = homeAddress,
            Email = "spam@acme.corp"
        };

        var occupation = new Job
        {
            Title = "Software Wizard",
            Salary = 1_000_000
        };
        
        var person = new Person
        {
            FirstName = "Todor",
            LastName = "Todorov",
            Contact = contact,
            Occupation = occupation,
        };

        // Add a circular reference here.
        homeAddress.Resident = person;

        return person;
    }
}