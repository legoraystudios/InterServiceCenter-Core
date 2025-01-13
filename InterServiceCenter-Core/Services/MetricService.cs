using System.Globalization;
using System.Runtime.Serialization;
using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace InterServiceCenter_Core.Services;

public class MetricService
{
    private readonly InterServiceCenterContext _dbContext;

    public MetricService(InterServiceCenterContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<JsonResponse> GetTotalPostsTwelveMonths()
    {
        DateTime twelveMonthsAgo = DateTime.Now.AddMonths(-12);
        DateTime twentyFourMonthsAgo = DateTime.Now.AddYears(-36);

        // Get post count of the Last 12-rolling months (i.e. October 2024 - October 2023)
        var monthlyCountsTY = await _dbContext.IscPosts.Where(p => p.PublishedAt >= twelveMonthsAgo)
            .GroupBy(p => new { p.PublishedAt.Year, p.PublishedAt.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(g => g.Year)
            .ThenBy(g => g.Month)
            .ToListAsync();
        
        // Get post count of the Last 12-24 rolling months (i.e. October 2023 - October 2022)
        var monthlyCountsLY = await _dbContext.IscPosts.Where(p => p.PublishedAt >= twentyFourMonthsAgo && p.PublishedAt <= twelveMonthsAgo)
            .GroupBy(p => new { p.PublishedAt.Year, p.PublishedAt.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(g => g.Year)
            .ThenByDescending(g => g.Month)
            .ToListAsync();

        // Results of Today's Year will stored in this Dictionary
        var resultsTY = new Dictionary<string, int>();
        // Results of Last Year will be stored in this Dictionary
        var resultsLY = new Dictionary<string, int>();
        // First month to count down (today's month).
        var firstMonth = DateTime.Now.Month;
        // Months left out of the calendar year that we need to count.
        var monthsLeft = 12 - firstMonth;

        // Counting the months of the Today's year inside of the current calendar year.
        for (var i = firstMonth; i > 0; i--)
        {
            var currentMonth = monthlyCountsTY.FirstOrDefault(p => p.Month == i);
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i);

            if (currentMonth == null)
            {
                resultsTY.Add(monthName + " " + DateTime.Now.Year, 0);
            }
            else
            {
                resultsTY.Add(monthName + " " + currentMonth.Year, currentMonth.Count);
            }
        }

        // Counting the months of the Today's year OUTSIDE of the current calendar year.
        // (i.e. If the 12-rolling months has a month with a different year)
        if (monthsLeft > 0)
        {
            for (var i = monthsLeft; i > 0; i--)
            {
                var monthToSearch = i + 12 - monthsLeft;
                var currentMonth = monthlyCountsTY.FirstOrDefault(p => p.Month == monthToSearch);
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthToSearch);
                var previousYear = DateTime.Now.Year - 1;

                if (currentMonth != null && currentMonth.Year == previousYear)
                {
                    resultsTY.Add(monthName + " " + previousYear, currentMonth.Count);
                }
                else
                {
                    resultsTY.Add(monthName + " " + previousYear, 0);
                }
            }
        }
        
        
        // ------------------------
        
        // Counting the months of the Last year inside of the current calendar year.
        for (var i = firstMonth; i > 0; i--)
        {
            var currentMonth = monthlyCountsLY.FirstOrDefault(p => p.Month == i);
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i);
            var previousYear = DateTime.Now.Year - 1;

            if (currentMonth == null)
            {
                resultsLY.Add(monthName + " " + previousYear, 0);
            }
            else
            {
                resultsLY.Add(monthName + " " + currentMonth.Year, currentMonth.Count);
            }
        }

        // Counting the months of the Last year OUTSIDE of the current calendar year.
        // (i.e. If the 12-rolling months has a month with a different year)
        if (monthsLeft > 0)
        {
            for (var i = monthsLeft; i > 0; i--)
            {
                var monthToSearch = i + 12 - monthsLeft;
                var currentMonth = monthlyCountsLY.FirstOrDefault(p => p.Month == monthToSearch);
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthToSearch);
                var previousYear = DateTime.Now.Year - 2;

                if (currentMonth != null && currentMonth.Year == previousYear)
                {
                    resultsLY.Add(monthName + " " + previousYear, currentMonth.Count);
                }
                else
                {
                    resultsLY.Add(monthName + " " + previousYear, 0);
                }
            }
        }

        
        
        return new JsonResponse { StatusCode = 200, TodaysYear = resultsTY, LastYear = resultsLY  };
    }
}