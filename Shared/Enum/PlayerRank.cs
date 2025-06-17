using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Shared.Enum
{
    public static class PlayerRankExtension
    {
        public static string GetDisplayName(this PlayerRank value)
        {
            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

            return member?.GetCustomAttribute<DisplayAttribute>()?.Name
                   ?? value.ToString();
        }
    }

    public enum PlayerRank
    {
        [Display(Name = "30k")]
        Kyu30 = 0,

        MinValue = 0,

        [Display(Name = "29k")]
        Kyu29 = 1,

        [Display(Name = "28k")]
        Kyu28 = 2,

        [Display(Name = "27k")]
        Kyu27 = 3,

        [Display(Name = "26k")]
        Kyu26 = 4,

        [Display(Name = "25k")]
        Kyu25 = 5,

        [Display(Name = "24k")]
        Kyu24 = 6,

        [Display(Name = "23k")]
        Kyu23 = 7,

        [Display(Name = "22k")]
        Kyu22 = 8,

        [Display(Name = "21k")]
        Kyu21 = 9,

        [Display(Name = "20k")]
        Kyu20 = 10,

        [Display(Name = "19k")]
        Kyu19 = 11,

        [Display(Name = "18k")]
        Kyu18 = 12,

        [Display(Name = "17k")]
        Kyu17 = 13,

        [Display(Name = "16k")]
        Kyu16 = 14,

        [Display(Name = "15k")]
        Kyu15 = 15,

        [Display(Name = "14k")]
        Kyu14 = 16,

        [Display(Name = "13k")]
        Kyu13 = 17,

        [Display(Name = "12k")]
        Kyu12 = 18,

        [Display(Name = "11k")]
        Kyu11 = 19,

        [Display(Name = "10k")]
        Kyu10 = 20,

        [Display(Name = "9k")]
        Kyu9 = 21,

        [Display(Name = "8k")]
        Kyu8 = 22,

        [Display(Name = "7k")]
        Kyu7 = 23,

        [Display(Name = "6k")]
        Kyu6 = 24,

        [Display(Name = "5k")]
        Kyu5 = 25,

        [Display(Name = "4k")]
        Kyu4 = 26,

        [Display(Name = "3k")]
        Kyu3 = 27,

        [Display(Name = "2k")]
        Kyu2 = 28,

        [Display(Name = "1k")]
        Kyu1 = 29,

        [Display(Name = "1d")]
        Dan1 = 30,

        [Display(Name = "2d")]
        Dan2 = 31,

        [Display(Name = "3d")]
        Dan3 = 32,

        [Display(Name = "4d")]
        Dan4 = 33,

        [Display(Name = "5d")]
        Dan5 = 34,

        [Display(Name = "6d")]
        Dan6 = 35,

        [Display(Name = "7d")]
        Dan7 = 36,

        [Display(Name = "8d")]
        Dan8 = 37,

        [Display(Name = "9d")]
        Dan9 = 38,

        [Display(Name = "1p")]
        Pro1 = 39,

        [Display(Name = "2p")]
        Pro2 = 40,

        [Display(Name = "3p")]
        Pro3 = 41,

        [Display(Name = "4p")]
        Pro4 = 42,

        [Display(Name = "5p")]
        Pro5 = 43,

        [Display(Name = "6p")]
        Pro6 = 44,

        [Display(Name = "7p")]
        Pro7 = 45,

        [Display(Name = "8p")]
        Pro8 = 46,

        [Display(Name = "9p")]
        Pro9 = 47,

        MaxValue = 47,
    }
}