using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualSuspectNaturalLanguage.Component {
    public static class TimeNaturalLanguageGenerator {

        public static string Generate(Dictionary<DateTime, List<KeyValuePair<DateTime, DateTime>>> dateTimeGroupedByDay) {

            string answer = "";

            //Iterate each day
            for (int i = 0; i < dateTimeGroupedByDay.Keys.Count; i++) {

                DateTime currentDate = dateTimeGroupedByDay.Keys.ElementAt(i);
                List<KeyValuePair<DateTime, DateTime>> currentTimeSpans = dateTimeGroupedByDay.Values.ElementAt(i);

                //Print the day
                answer += "on " + ConvertDateToText(currentDate) + " from ";

                //Iterate each time span
                for (int j = 0; j < currentTimeSpans.Count; j++) {

                    answer += ConvertTimeToText(currentTimeSpans.ElementAt(j).Key) + " to " + ConvertTimeToText(currentTimeSpans.ElementAt(j).Value);

                    if (j != currentTimeSpans.Count - 1) {
                        answer += ", ";
                    }

                    if (j == currentTimeSpans.Count - 2) {
                        answer += "and ";
                    }

                }

                if (i != dateTimeGroupedByDay.Keys.Count - 1) {

                    answer += ", ";

                    if (i == dateTimeGroupedByDay.Keys.Count - 2) {
                        answer += "and ";
                    }

                }
                else {
                    answer += ".";
                }
            }
            return answer;
        }

        #region Utility Methods
        private static string CombineValues(string term, List<string> values) {

            string combinedValues = "";

            for (int i = 0; i < values.Count(); i++) {

                combinedValues += values[i];

                if (i == values.Count() - 2) {
                    combinedValues += " " + term + " ";
                }
                else if (i < values.Count() - 1) {
                    combinedValues += ", ";
                }
            }

            return combinedValues;

        }

        private static string ConvertDateToText(DateTime date) {

            return "the " + ConvertDayToCardinal(date.Day) + " of " + date.ToString("MMMM", CultureInfo.InvariantCulture) + " " + date.Year;

        }

        private static string ConvertTimeToText(DateTime time) {

            return time.Hour + ":" + time.ToString("mm", CultureInfo.InvariantCulture);

        }

        private static string ConvertDateTimeToText(KeyValuePair<DateTime, DateTime> timeSpan) {

            DateTime firstDate = timeSpan.Key;
            DateTime secondDate = timeSpan.Value;

            //Same day Answer once(Ex. 3rd of March 2016 between 14:00 and 15:00)
            if (firstDate.Day == secondDate.Day && firstDate.Month == secondDate.Month && firstDate.Year == secondDate.Year) {

                //Convert to Text Date
                String date = "the " + ConvertDayToCardinal(firstDate.Day) + " of " + firstDate.ToString("MMMM", CultureInfo.InvariantCulture) + " " + firstDate.Year;

                //Convert to Text First Hour
                String firstHour = firstDate.Hour + ":" + firstDate.ToString("mm", CultureInfo.InvariantCulture);

                //Convert to Text Second Hour
                String lastHour = secondDate.Hour + ":" + secondDate.ToString("mm", CultureInfo.InvariantCulture);

                return date + ", between " + firstHour + " and " + lastHour;

            }
            else {//Different days

                return "";
            }

        }

        private static string ConvertDayToCardinal(int day) {

            switch (day) {
                case 1:
                    return "1st";
                case 2:
                    return "2nd";
                case 3:
                    return "3rd";
                case 21:
                    return "21st";
                case 22:
                    return "22nd";
                case 23:
                    return "23rd";
                case 31:
                    return "31st";
                default:
                    return day + "th";
            }
        }
        
        #endregion   
    }
}
