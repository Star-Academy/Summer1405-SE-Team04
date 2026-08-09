public static class TestData
{
    public static IReadOnlyList<(int ID, string FirstName, bool IsMale, int Age)> Students { get; } =
    [
        (1, "Ali", true, 19),
        (2, "Amirali", true, 12),
        (3, "Sara", false, 34),
        (4, "Zahra", false, 22)
    ];
}