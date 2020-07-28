using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NewOPAL.Models;
using NewOPAL.Services;
using Newtonsoft.Json;

namespace NewOPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly IMemoryCache cache;
        private ITranslationService translationService;
        private IHelpService helpService;
        private UserManager<OpalUser> _userManager;
        private readonly SignInManager<OpalUser> _signInManager;

        public LanguageController(ITranslationService service, UserManager<OpalUser> userManager, SignInManager<OpalUser> signInManager, IHelpService help, IMemoryCache memoryCache)
        {
            translationService = service;
            _userManager = userManager;
            _signInManager = signInManager;
            helpService = help;
            cache = memoryCache;
        }
        // GET: api/Language
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("GetWord/{id}")]
        [HttpGet("{id}", Name = "GetWord")]
        [ActionName("GetWord")]
        public string Get(int id, string word)
        {
            return translationService.TranslateWord(id, word);
        }

        [Route("GetTxtById/{IdLanguage}")]
        [HttpGet("{IdLanguage}", Name = "GetTxtById")]
        [ActionName("GetTxtById")]
        public string GetTxtById(int IdTxt, int IdLanguage)
        {
            return translationService.GetTxtById(IdTxt, IdLanguage);
        }


        [Route("GetModalTxt/{Action,IdLanguage}")]
        [HttpGet("{Action,IdLanguage}", Name = "GetModalTxt")]
        [ActionName("GetModalTxt")]
        public JsonResult GetModalTxt(string Action, int IdLanguage)
        {
            return new JsonResult(helpService.GetModalTxt(Action, IdLanguage));
        }

        // POST: api/Language/5
        [HttpPost("{id}")]
        public async Task<JsonResult> PostAsync(int id, [FromBody] List<Words> words = null, int? LastLanguageId = 0)
        {
            if (LastLanguageId == 0)
            {
                LastLanguageId = HttpContext.Session.GetInt32("language").GetValueOrDefault();
            }

            if (id == LastLanguageId)
            {
                return new JsonResult(words);
            }
            else
            {
                var translations = translationService.Translate(id, words, LastLanguageId);

                if (_signInManager.IsSignedIn(User))
                {
                    var user = await _userManager.GetUserAsync(User);
                    user.IdLanguage = id;
                    await _userManager.UpdateAsync(user);
                    await _signInManager.RefreshSignInAsync(user);
                }

                HttpContext.Session.SetInt32("language", id);
                return new JsonResult(translations);
            }
        }

        // PUT: api/Language/5
        [HttpPut("{id}")]
        public async Task PutAsync(int id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                user.IdLanguage = id;
                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);
            }

            HttpContext.Session.SetInt32("language", id);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
