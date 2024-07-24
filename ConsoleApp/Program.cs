// See https://aka.ms/new-console-template for more information

using Core;

IWorkdayCalendar calendar = new WorkdayCalendar();
calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
calendar.SetRecurringHoliday(5, 17);
calendar.SetHoliday(new DateTime(2004, 5, 27));
string format = "dd-MM-yyyy HH:mm";
var start = new DateTime(2004, 5, 24, 19, 3, 0);
decimal increment = 44.723656m;
var incrementedDate = calendar.GetWorkdayIncrement(start, increment);
Console.WriteLine(
    start.ToString(format) +
    " with an addition of " +
    increment +
    " work days is " +
    incrementedDate.ToString(format));
    
    Console.ReadKey();