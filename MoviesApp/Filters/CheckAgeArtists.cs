using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MoviesApp.Filters
{
    public class CheckAgeArtists: Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var formDate = DateTime.Parse(context.HttpContext.Request.Form["BirthdayDate"]);
            var age = DateTime.Now.Year - formDate.Year;
            if (DateTime.Now.DayOfYear < formDate.DayOfYear)
            {
                age++;
            }
            if (age is < 7 or > 99)
            {
                context.Result = new BadRequestResult();
            }
        }
        
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}