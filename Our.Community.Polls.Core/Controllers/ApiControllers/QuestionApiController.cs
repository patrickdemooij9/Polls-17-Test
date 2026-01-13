using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using Our.Community.Polls.Core.Converters;
using Our.Community.Polls.Models;
using Our.Community.Polls.Repositories;
using Umbraco.Cms.Web.Common.Controllers;

namespace Our.Community.Polls.Controllers.ApiControllers
{

    [ApiController]
    public class QuestionApiController : ControllerBase 
    {
        private readonly IQuestions _questions;
        private readonly ILogger<QuestionApiController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionApiController(IQuestions questions,ILogger<QuestionApiController> logger,IHttpContextAccessor httpContextAccessor)
        {
            _questions = questions;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("get-question")]
        public IEnumerable<Question> Get()
        {
            return _questions.Get();
        }

        [HttpGet]
        [Route("get-question/{id}")]
        public Question GetById(int id)
        {
            var result = _questions.GetById(id);

            return result;
        }

        [HttpPost]
        [Route("post-question")]
        public async Task<Question?> PostAsync()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeConverterUsingDateTimeParse());

            string requestBody = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            //need to make the Id an int by removing the surrounding "
            var parsedstring = Regex.Replace(requestBody,"(?:\"Id\":\")(\\d)\"","\"Id\":$1");
            //change any undefined properties as null
            parsedstring = Regex.Replace(parsedstring,"\"undefined\"",DateTime.MinValue.ToString());

            var question = JsonSerializer.Deserialize<Question>(parsedstring,options);

            return _questions.Save(question);
        }

        [HttpDelete]
        [Route("delete-question/{id}")]
        public bool Delete(int id)
        {
            try
            {
                if (_questions.Delete(id))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Unable to delete question");
                throw;
            }

            return false;
        }

        [HttpGet]
        [Route("get-answers/{id}")]
        public IEnumerable<Answer> GetAnswers(int id)
        {
            return _questions.GetAnswers(id);
        }

        [HttpPost]
        [Route("post-question-answer")]
        public Answer PostAnswer(int id, Answer answer)
        {
            try
            {
                var result = _questions.PostAnswer(id, answer);

                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Unable to add answer to question");
                throw;
            }

            return null; 
        }

        [HttpGet]
        [Route("get-response/{id}")]
        public IEnumerable<Response> GetResponses(int id)
        {
            return _questions.GetResponses(id);
        }
    }
}
