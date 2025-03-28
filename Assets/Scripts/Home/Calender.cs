using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calender : MonoBehaviour
{
    public class day
    {
        public int dayNum;
        public Color dayColor;
        public GameObject obj;

        public day(int dayNum, Color dayColor, GameObject obj)
        {
            this.dayNum = dayNum;
            this.obj = obj;
            UpdateColor(dayColor);
            UpdateDay(dayNum);
        }

        public void UpdateColor(Color newColor)
        {
            obj.GetComponent<Image>().color = newColor;
            this.dayColor = newColor;
        }

        public void UpdateDay(int newdayNum)
        {
            this.dayNum = newdayNum;
            if (dayColor == Color.white || dayColor == Color.green || dayColor == new Color(0.7f, 0.7f, 0.7f))
            {
                obj.GetComponentInChildren<Text>().text = (dayNum + 1).ToString();
            }
            else
            {
                obj.GetComponentInChildren<Text>().text = " ";
            }
        }
    }

    List<day>  days = new List<day>();

    public Transform[] week;

    public Text MonthAndYear;

    public DateTime currDate = DateTime.Now;
    void Start()
    {
        UpdateCalender(DateTime.Now.Year, DateTime.Now.Month);
    }

    void UpdateCalender(int year, int month)
    {
        DateTime temp = new DateTime(year, month, 1);
        currDate = temp;
        MonthAndYear.text = temp.ToString("MMMM") + " " + temp.Year.ToString();

        int startDay = GetMonthStartDay(year, month);
        int endDay = GetTotalNumberofDays(year, month);

        // Previous month calculations
        int prevMonth = month == 1 ? 12 : month - 1;
        int prevYear = month == 1 ? year - 1 : year;
        int prevMonthDays = GetTotalNumberofDays(prevYear, prevMonth);

        // Next month calculations
        int nextMonth = month == 12 ? 1 : month + 1;
        int nextYear = month == 12 ? year + 1 : year;

        if (days.Count == 0)
        {
            for (int w = 0; w < 6; w++)
            {
                for (int i = 0; i < 7; i++)
                {
                    day newDay;
                    int currDay = (w * 7) + i;

                    if (currDay < startDay) // Previous month days
                    {
                        newDay = new day(prevMonthDays - (startDay - currDay), new Color(0.7f, 0.7f, 0.7f), week[w].GetChild(i).gameObject);
                    }
                    else if (currDay - startDay >= endDay) // Next month days
                    {
                        newDay = new day((currDay - startDay) - endDay, new Color(0.7f, 0.7f, 0.7f), week[w].GetChild(i).gameObject);
                    }
                    else // Current month days
                    {
                        newDay = new day(currDay - startDay, Color.white, week[w].GetChild(i).gameObject);
                    }

                    days.Add(newDay);
                }
            }
        }
        else
        {
            for (int i = 0; i < 42; i++)
            {
                if (i < startDay) // Previous month days
                {
                    days[i].UpdateColor(new Color(0.7f, 0.7f, 0.7f));
                    days[i].UpdateDay(prevMonthDays - (startDay - i));
                }
                else if (i - startDay >= endDay) // Next month days
                {
                    days[i].UpdateColor(new Color(0.7f, 0.7f, 0.7f));
                    days[i].UpdateDay((i - startDay) - endDay);
                }
                else // Current month days
                {
                    days[i].UpdateColor(Color.white);
                    days[i].UpdateDay(i - startDay);
                }
            }
        }

        // Highlight today's date
        if (DateTime.Now.Year == year && DateTime.Now.Month == month)
        {
            days[(DateTime.Now.Day - 1) + startDay].UpdateColor(Color.green);
        }
    }


    int GetMonthStartDay(int year, int month)
    {
        DateTime temp = new DateTime(year, month, 1);

        return (int)temp.DayOfWeek;
    }
    int GetTotalNumberofDays(int year, int month)
    {
        return DateTime.DaysInMonth(year, month);
    }

    public void SwitchMonth(int Direction)
    {
        if (Direction < 0)
        {
            currDate = currDate.AddMonths(-1);
        }
        else
        {
            currDate = currDate.AddMonths(1);
        }
        UpdateCalender(currDate.Year, currDate.Month);
    }
}
