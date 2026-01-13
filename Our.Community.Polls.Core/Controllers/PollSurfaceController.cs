using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Our.Community.Polls.Interfaces;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Cms.Infrastructure.Persistence;


namespace Our.Community.Polls.Controllers
{
    public class PollSurfaceController : SurfaceController
    {

        private readonly IPollService _pollService;
        public PollSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider,
            IPollService pollService) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _pollService = pollService;
        }        
        
        public IActionResult Get(int id)
        {
            var poll = _pollService.GetQuestion(id);
            return this.View(poll);
        }
        [HttpPost]
        public IActionResult Poll(IFormCollection form)
        {
            var questionId = form["questionId"];
            var answerId = form["answerId"];
            if (!form.ContainsKey("answerId"))
            {
                ModelState.AddModelError("polls","You must select an answer");
                return CurrentUmbracoPage();
            }
            var poll = _pollService.Vote(Convert.ToInt32(questionId), Convert.ToInt32(answerId));
            if(poll != null)
            {
                if(!Request.Cookies.ContainsKey("Poll_" + questionId))
                { 
                    Response.Cookies.Append("Poll_" + questionId,"true",new CookieOptions(){Expires = DateTime.UtcNow.AddMonths(6)});
                }
            }
            TempData["Question"] = poll;
            return CurrentUmbracoPage();
        }

        public IActionResult Vote(int questionId,int answerId)
        {

            var poll = _pollService.Vote(questionId, answerId);
            TempData["Question"] = poll;
            if(poll != null)
            {
                if(!Request.Cookies.ContainsKey("Poll_" + questionId))
                { 
                    Response.Cookies.Append("Poll_" + questionId,"true",new CookieOptions(){Expires = DateTime.UtcNow.AddMonths(6)});
                }
            }
            return ViewComponent("Polls",new {Model = poll});
        }
    }
}
