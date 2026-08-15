using IntegrationTests;

public static class TestData
{
    public static IReadOnlyList<Student> Students { get; } =
    [
        new Student(1, "Ali", true, 19),
        new Student(2, "Amirali", true, 12),
        new Student(3, "Sara", false, 34),
        new Student(4, "Zahra", false, 22),
        new Student(5,"Farhad",true, null)
    ];
}