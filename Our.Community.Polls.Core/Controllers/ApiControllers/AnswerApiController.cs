using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Our.Community.Polls.Models;
using Our.Community.Polls.Repositories;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Web.Website.Controllers;

namespace Our.Community.Polls.Controllers.ApiControllers
{
    public class AnswerApiController : ManagementApiControllerBase
    {
        private readonly IAnswers _answers;
        private readonly IResponses _responses;

        public AnswerApiController(IAnswers answers, IResponses responses)
        {
            _answers = answers;
            _responses = responses;
        }

        [HttpPost]
        [Route("post-answer")]
        public IActionResult Post(Answer answer)
        {
            try
            {
                var result = _answers.Save(answer);

                if (result != null)
                {
                    return Ok();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            return StatusCode((int)HttpStatusCode.InternalServerError, "Can't save answer");
        }

        [HttpDelete]
        [Route("post-delete")]
        public bool Delete(int id, int questionId)
        {
            if (_responses.DeleteByAnswerId(id) && _answers.Delete(id))
            {
                return true;
            }
            return false;
        }
    }
}