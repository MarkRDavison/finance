namespace mark.davison.finance.accounting.rules.test;

[TestClass]
public class DateRulesTests
{

    [TestMethod]
    public void GetMonthRange_WorksAsExpected_ForDateInMiddleOfMonth()
    {
        var date = new DateOnly(2022, 3, 15);
        var (start, end) = date.GetMonthRange();

        Assert.AreEqual(new DateOnly(2022, 3, 1), start);
        Assert.AreEqual(new DateOnly(2022, 3, 31), end);
    }

    [TestMethod]
    public void GetMonthRange_WorksAsExpected_ForDateAtStartOfMonth()
    {
        var date = new DateOnly(2022, 3, 1);
        var (start, end) = date.GetMonthRange();

        Assert.AreEqual(new DateOnly(2022, 3, 1), start);
        Assert.AreEqual(new DateOnly(2022, 3, 31), end);
    }

    [TestMethod]
    public void GetMonthRange_WorksAsExpected_ForDateAtEndOfMonth()
    {
        var date = new DateOnly(2022, 3, 31);
        var (start, end) = date.GetMonthRange();

        Assert.AreEqual(new DateOnly(2022, 3, 1), start);
        Assert.AreEqual(new DateOnly(2022, 3, 31), end);
    }

    [TestMethod]
    public void GetMonthRange_WorksAsExpected_ForDateInJanuary()
    {
        var date = new DateOnly(2022, 1, 31);
        var (start, end) = date.GetMonthRange();

        Assert.AreEqual(new DateOnly(2022, 1, 1), start);
        Assert.AreEqual(new DateOnly(2022, 1, 31), end);
    }

    [TestMethod]
    public void GetMonthRange_WorksAsExpected_ForDateInDecember()
    {
        var date = new DateOnly(2022, 12, 31);
        var (start, end) = date.GetMonthRange();

        Assert.AreEqual(new DateOnly(2022, 12, 1), start);
        Assert.AreEqual(new DateOnly(2022, 12, 31), end);
    }

}