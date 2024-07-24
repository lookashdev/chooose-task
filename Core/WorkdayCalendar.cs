namespace Core;

public class WorkdayCalendar: IWorkdayCalendar
{
    private readonly HashSet<DateTime> _holidays = new();
    private readonly HashSet<(int Month, int Day)> _recurringHolidays = new();
    private TimeSpan _workdayStartTimeSpan;
    private TimeSpan _workdayEndTimeSpan;
    
    public void SetHoliday(DateTime date)
    {
        _holidays.Add(date);
    }

    public void SetRecurringHoliday(int month, int day)
    {
        _recurringHolidays.Add((month, day));
    }

    public void SetWorkdayStartAndStop(int startHours, int startMinutes, int stopHours, int stopMinutes)
    {
        _workdayStartTimeSpan = new TimeSpan(startHours, startMinutes, 0);
        _workdayEndTimeSpan = new TimeSpan(stopHours, stopMinutes, 0);
    }

    public DateTime GetWorkdayIncrement(DateTime startDate, decimal incrementInWorkdays)
    {
        int incrementSign = Math.Sign(incrementInWorkdays); 
        var bussinessStartDate = AlignDateTimeToCorrectWorkDay(startDate, incrementSign);
        
        if (incrementInWorkdays == 0)
            return bussinessStartDate;

        DateTime resultDateTime = bussinessStartDate;
        int incrementInSeconds = (int)Math.Round(Convert.ToDecimal(GetWorkdayDuration().TotalSeconds) * incrementInWorkdays);
        var remainingIncrementInSeconds = Math.Abs(incrementInSeconds);

        int oneWorkDayInSeconds = (int)GetWorkdayDuration().TotalSeconds;

        while (remainingIncrementInSeconds > 0)
        {
            if (remainingIncrementInSeconds >= oneWorkDayInSeconds)
            {
                resultDateTime = GetNextWorkday(resultDateTime, incrementSign);
                remainingIncrementInSeconds -= oneWorkDayInSeconds;
            }
            else
            {
                var currentTimeSpan = resultDateTime.TimeOfDay;
                var diffTimeLeft = DiffTimeLeftOnWorkday(incrementSign, currentTimeSpan);
                if (diffTimeLeft.TotalSeconds < remainingIncrementInSeconds)
                {
                    remainingIncrementInSeconds -= (int)diffTimeLeft.TotalSeconds;
                    resultDateTime = GetNextWorkday(resultDateTime, incrementSign);
                }
                
                resultDateTime = resultDateTime.AddSeconds(remainingIncrementInSeconds * incrementSign);
                remainingIncrementInSeconds = 0;
            }
        }

        return resultDateTime;
    }

    private TimeSpan DiffTimeLeftOnWorkday(int incrementSign, TimeSpan currentTimeSpan)
    {
        TimeSpan diffTimeLeft;
        if (incrementSign > 0)
        {
            diffTimeLeft = _workdayEndTimeSpan - currentTimeSpan;
        }
        else
        {
            diffTimeLeft = currentTimeSpan - _workdayStartTimeSpan;
        }

        return diffTimeLeft;
    }

    private DateTime GetNextWorkday(DateTime date, int direction)
    {
        
        date = date.AddDays(direction);
        while (!IsWorkday(date))
        {
            date = date.AddDays(direction);
        }
        return date;
    }
    
    private bool IsWorkday(DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday
               && date.DayOfWeek != DayOfWeek.Sunday
               && !_holidays.Contains(date.Date)
               && !_recurringHolidays.Contains((date.Month, date.Day));
    }
    
    private DateTime AlignDateTimeToCorrectWorkDay(DateTime date, int incrementSign)
    {
        if (incrementSign > 0)
        {
            if (date.TimeOfDay < _workdayStartTimeSpan)
                return date.Date.Add(_workdayStartTimeSpan);
            if (date.TimeOfDay > _workdayEndTimeSpan)
                return date.Date.Add(_workdayStartTimeSpan).AddDays(1);
        }
        else
        {
            if (date.TimeOfDay < _workdayStartTimeSpan)
                return date.Date.Add(_workdayEndTimeSpan).AddDays(-1);
            if (date.TimeOfDay > _workdayEndTimeSpan)
                return date.Date.Add(_workdayEndTimeSpan);
        }
        
        return date;
    }
    
    private TimeSpan GetWorkdayDuration()
    {
        if (_workdayEndTimeSpan < _workdayStartTimeSpan)
        {
            return (TimeSpan.FromHours(24) - _workdayStartTimeSpan) + _workdayEndTimeSpan;
        }

        return _workdayEndTimeSpan - _workdayStartTimeSpan;
    }
}