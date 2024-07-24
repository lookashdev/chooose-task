namespace Tests;

public class WorkCalendarTests
{
    [Theory]
    [InlineData("24-05-2004 18:05", -5.5, "14-05-2004 12:00")]
    [InlineData("24-05-2004 19:03", 44.723656, "27-07-2004 13:47")]
    [InlineData("24-05-2004 18:03", -6.7470217, "13-05-2004 10:02")]
    [InlineData("24-05-2004 08:03", 12.782709, "10-06-2004 14:18")]
    [InlineData("24-05-2004 07:03", 8.276628, "04-06-2004 10:12")]
    public void TestWorkdayIncrement(string startStr, decimal increment, string expectedEndStr)
    {
        var calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));
        string format = "dd-MM-yyyy HH:mm";
        var start = DateTime.ParseExact(startStr, format, null);
        
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);
        var actualResult = $"{start.ToString(format)} with an addition of {increment} work days is {incrementedDate.ToString(format)}";
        var expectedResult = $"{startStr} with an addition of {increment} work days is {expectedEndStr}";

        Assert.Equal(expectedResult, actualResult);
    }
}