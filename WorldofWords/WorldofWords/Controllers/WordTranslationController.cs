using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Services;

namespace WorldofWords.Controllers
{
    [WowAuthorization(AllRoles = new[] { "Teacher", "Admin" })]
    [RoutePrefix("api/WordTranslation")]
    public class WordTranslationController : BaseController
    {
        private const int TranslationLanguageId = 4;

        private readonly IWordTranslationMapper wordTranslationMapper;
        private readonly IWordTranslationService wordTranslationService;
        private readonly IWordService wordService;
        private readonly IWordMapper wordMapper;
        private readonly ITagService tagService;
        private readonly ITagMapper tagMapper;

        public WordTranslationController(IWordTranslationService wordTranslationService,
                                         IWordTranslationMapper wordTranslationMapper,
                                         IWordService wordService,
                                         IWordMapper wordMapper,
                                         ITagService tagService,
                                         ITagMapper tagMapper)
        {
            this.wordTranslationService = wordTranslationService;
            this.wordTranslationMapper = wordTranslationMapper;
            this.wordService = wordService;
            this.wordMapper = wordMapper;
            this.tagService = tagService;
            this.tagMapper = tagMapper;
        }

        [Route("ImportWordTranslations")]
        public List<WordTranslationModel> Post(List<WordTranslationImportModel> wordTranslations)
        {
            List<WordTranslationModel> wordTranslationsToReturn = new List<WordTranslationModel>();

            foreach (WordTranslationImportModel wordTranslation in wordTranslations)
            {
                wordTranslation.OriginalWordId = wordService.Exists(wordTranslation.OriginalWord, wordTranslation.LanguageId);

                if (wordTranslation.OriginalWordId == 0)
                {
                    wordTranslation.OriginalWordId = wordService.Add(wordMapper.ToDomainModel(new WordModel()
                        {
                            Value = wordTranslation.OriginalWord,
                            LanguageId = wordTranslation.LanguageId,
                            Transcription = wordTranslation.Transcription,
                            Description = wordTranslation.Description
                        }));
                }

                wordTranslation.TranslationWordId = wordService.Exists(wordTranslation.TranslationWord, TranslationLanguageId);

                if (wordTranslation.TranslationWordId == 0)
                {
                    wordTranslation.TranslationWordId = wordService.Add(wordMapper.ToDomainModel(new WordModel()
                        {
                            Value = wordTranslation.TranslationWord,
                            LanguageId = TranslationLanguageId
                        }));
                }


                foreach (TagModel tag in wordTranslation.Tags)
                {
                    tag.Id = tagService.Exists(tag.Name);

                    if (tag.Id == 0)
                    {
                        tag.Id = tagService.Add(tagMapper.Map(tag));
                    }
                }

                wordTranslation.OwnerId = UserId;
                int wordTranslationId = wordTranslationService.Add(wordTranslationMapper.Map(wordTranslation, tagMapper.MapRange(wordTranslation.Tags)));

                wordTranslationsToReturn.Add(new WordTranslationModel()
                {
                    Id = wordTranslationId,
                    OriginalWord = wordTranslation.OriginalWord,
                    TranslationWord = wordTranslation.TranslationWord
                });
            }

            return wordTranslationsToReturn;
        }

        [Route("CreateWordTranslation")]
        public IHttpActionResult Post(WordTranslationImportModel wordtranslation)
        {
            if (wordtranslation == null)
                throw new ArgumentNullException("word translation model can't be empty");
            if (wordTranslationService.Exists(wordtranslation.OriginalWordId, wordtranslation.TranslationWordId) == 0)
            {
                return Ok(wordTranslationService.Add(wordTranslationMapper.Map(wordtranslation)).ToString());
            }
            else
            {
                return BadRequest("Such wordtranslation exists");
            }
        }

        [Route("SearchWordTranslations")]
        public List<WordTranslationModel> Post(SearchWordTranslationModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "Search model can't be null");
            }

          if (model.SearchByTag)
            {
                return wordTranslationMapper
                   .MapRange(wordTranslationService.GetAllByTags(model.Tags, model.LanguageId));
            }
            else
            {
                return wordTranslationMapper
                   .MapRange(wordTranslationService.GetTopBySearchWord(model.SearchWord, model.LanguageId));
            }            
        }

        public List<WordTranslationModel> Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID can't be negative or 0", "id");
            }

            return wordTranslationMapper.MapRange(wordTranslationService.GetByWordSuiteID(id));
        }
    }
}