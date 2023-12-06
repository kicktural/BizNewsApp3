namespace BizNewsAppDB1.Helper
{
    public class TimeCommentHelper
    {

        public static string TimeAgo(DateTime dateTime)
        {
            TimeSpan timeSince = DateTime.Now.Subtract(dateTime);

            if (timeSince.TotalSeconds < 60)
            {
                int seconds = (int)timeSince.TotalSeconds;
                return $"{seconds} second(s) ago";
            }
            else if (timeSince.TotalMinutes < 60)
            {
                int minutes = (int)timeSince.TotalMinutes;
                return $"{minutes} minute(s) ago";
            }
            else if (timeSince.TotalHours < 24)
            {
                int hours = (int)timeSince.TotalHours;
                return $"{hours} hour(s) ago";
            }
            else if (timeSince.TotalDays < 30)
            {
                int days = (int)timeSince.TotalDays;
                return $"{days} day(s) ago";
            }
            else if (timeSince.TotalDays < 365)
            {
                int months = (int)(timeSince.TotalDays / 30);
                return $"{months} month(s) ago";
            }
            else
            {
                int years = (int)(timeSince.TotalDays / 365);
                return $"{years} year(s) ago";
            }
        }
    }
}
