using Cahut_Backend.Models;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Cahut_Backend.Repository
{
    public class PresentationRepository : BaseRepository
    {
        public PresentationRepository(AppDbContext context) : base(context)
        {
        }

        public bool CheckExisted(Guid userId, string presentationName)
        {
            return context.Presentation.Any(p => p.OwnerId == userId && p.PresentationName == presentationName);
        }

        public int Create(Guid userId, string presentationName)
        {
            Presentation present = new Presentation
            {
                PresentationName = presentationName,
                CreatedDate = DateTime.UtcNow.AddHours(7),
                OwnerId = userId
            };
            context.Presentation.Add(present);
            return context.SaveChanges();
        }
        public Presentation GetPresentationByNameAndTeacherId(string name, Guid teacherId)
        {
            return context.Presentation.Where(p => p.PresentationName == name && p.OwnerId == teacherId).SingleOrDefault();
        }

        public int Delete(Guid presentationId)
        {
            Presentation present = context.Presentation.Find(presentationId);
            context.Remove(present);
            return context.SaveChanges();
        }

        public int Update(Guid presentationId, Guid userId, string newName)
        {
            Presentation present = context.Presentation.Where(p => p.PresentationId == presentationId && p.OwnerId == userId).SingleOrDefault();
            present.PresentationName = newName;
            return context.SaveChanges();
        }

        public int CountNumOfSlide(Guid presentationId)
        {
            return context.Slide.Count(p => p.PresentationId == presentationId);
        }

        public List<object> GetPresentationList(Guid userId)
        {
            var res = from present in context.Presentation
                      where present.OwnerId == userId
                      select new
                      {
                          presentationId = present.PresentationId,
                          createdDate = present.CreatedDate,
                          presentationName = present.PresentationName,
                      };
            List<object> result = new List<object>();
            foreach(var item in res)
            {
                int NumOfSlides = 0;
                NumOfSlides += context.MultipleChoiceSlide.Count(p => p.PresentationId == item.presentationId);
                NumOfSlides += context.ParagraphSlide.Count(p => p.PresentationId == item.presentationId);
                NumOfSlides += context.HeadingSlide.Count(p => p.PresentationId == item.presentationId);

                result.Add(new
                {
                    presentationId = item.presentationId,
                    createdDate = item.createdDate,
                    presentationName = item.presentationName,
                    numOfSlides = NumOfSlides
                });
            }
            return result;
        }
        public bool presentationExisted(Guid presentationId, Guid userId)
        {
            return context.Presentation.Any(p => p.PresentationId == presentationId && p.OwnerId == userId);
        }

        public string GetPresentationName(Guid presentationId)
        {
            return context.Presentation.Find(presentationId).PresentationName;
        }

        public List<object> GetPresentationSlides(Guid presentationId)
        {
            List<MultipleChoiceSlide> multipleChoiceSlides = context.MultipleChoiceSlide.Where(p => p.PresentationId == presentationId).ToList<MultipleChoiceSlide>();
            List<ParagraphSlide> paragraphSlides = context.ParagraphSlide.Where(p => p.PresentationId == presentationId).ToList<ParagraphSlide>();
            List<HeadingSlide> headingSLides = context.HeadingSlide.Where(p => p.PresentationId == presentationId).ToList<HeadingSlide>();
            List<Slide> totalSlides = new List<Slide>();
            totalSlides.AddRange(multipleChoiceSlides);
            totalSlides.AddRange(paragraphSlides);
            totalSlides.AddRange(headingSLides);
            var totalSlidesSorted = totalSlides.OrderBy(p => p.DateCreated);
            List<object> result = new List<object> { totalSlidesSorted };
            return result;
        }

        public int CountPresentationOwned(Guid userId)
        {
            return context.Presentation.Count(p => p.OwnerId == userId);
        }

        public bool AddCollaborators(Guid presentationId, string email)
        {
            var res = context.User.Where(p => p.Email == email)
                                .Select(p => p.UserId)
                                .FirstOrDefault();
            if (res == Guid.Empty)
            {
                return false;
            }
            bool isCollabExist = context.PresentationDetail.Any(c => c.PresentationId == presentationId && c.ColaboratorId == res);
            if (isCollabExist)
            {
                return false;
            }


            PresentationDetail presentationDetail = new PresentationDetail
            {
                PresentationId = presentationId,
                ColaboratorId = res,
            };
            context.PresentationDetail.Add(presentationDetail);
            context.SaveChanges();
            return true;
        }

        public int DeletCollaborators(Guid presentationId, string email)
        {
            var res = context.User.Where(p => p.Email == email)
                                .Select(p => p.UserId)
                                .FirstOrDefault();
            if (res == null)
            {
                return 0;
            }

            PresentationDetail? presentationDetail = context.PresentationDetail.Find(presentationId, res);
            context.PresentationDetail.Remove(presentationDetail);
            return context.SaveChanges();
        }

        public object GetCollaborators(Guid presentationId)
        {
            var collaborators = from presentationDetail in context.PresentationDetail
                                join user in context.User
                                on presentationDetail.ColaboratorId equals user.UserId
                                where presentationDetail.PresentationId == presentationId
                                select new
                                {
                                    username = user.UserName,
                                    email = user.Email,
                                };
            return collaborators;
        }

        public List<object> GetCollaboratorPresentation(Guid userId)
        {
            var res = from presentationDetail in context.PresentationDetail
                      join presentation in context.Presentation
                      on presentationDetail.PresentationId equals presentation.PresentationId 
                      where presentationDetail.ColaboratorId == userId
                      select new
                      {
                          presentationId = presentation.PresentationId,
                          createdDate = presentation.CreatedDate,
                          presentationName = presentation.PresentationName,
                      };
            List<object> result = new List<object>();
            foreach (var item in res)
            {
                int NumOfSlides = 0;
                NumOfSlides += context.MultipleChoiceSlide.Count(p => p.PresentationId == item.presentationId);
                NumOfSlides += context.ParagraphSlide.Count(p => p.PresentationId == item.presentationId);
                NumOfSlides += context.HeadingSlide.Count(p => p.PresentationId == item.presentationId);

                result.Add(new
                {
                    presentationId = item.presentationId,
                    createdDate = item.createdDate,
                    presentationName = item.presentationName,
                    numOfSlides = NumOfSlides
                });
            }
            return result;
        }

        public bool isCollaborator(Guid presentationId, Guid userId)
        {
            bool res = context.PresentationDetail.Any(p => p.PresentationId == presentationId && p.ColaboratorId == userId);
            return res;
        }

        public int StartPublicPresentation(Guid presentationId)
        {
            Presentation present = context.Presentation.Find(presentationId);
            present.IsBeingPresented = true;
            present.PresentationType = "public";
            //get slides list and set first slide as current slide
            List<MultipleChoiceSlide> multipleChoiceSlides = context.MultipleChoiceSlide.Where(p => p.PresentationId == presentationId).ToList<MultipleChoiceSlide>();
            List<ParagraphSlide> paragraphSlides = context.ParagraphSlide.Where(p => p.PresentationId == presentationId).ToList<ParagraphSlide>();
            List<HeadingSlide> headingSLides = context.HeadingSlide.Where(p => p.PresentationId == presentationId).ToList<HeadingSlide>();
            List<Slide> slideList = new List<Slide>();
            slideList.AddRange(multipleChoiceSlides);
            slideList.AddRange(paragraphSlides);
            slideList.AddRange(headingSLides);
            slideList.OrderBy(p => p.DateCreated);

            var totalSlidesSorted = slideList.OrderBy(p => p.DateCreated);
            if (slideList.Count > 0)
            {
                var firstSlide = totalSlidesSorted.First();
                firstSlide.IsCurrent = 1;
                return context.SaveChanges();
            }
            return context.SaveChanges();
        }

        public int StartGroupPresentation(Guid presentationId, Guid groupId)
        {
            Presentation present = context.Presentation.Find(presentationId);
            present.IsBeingPresented = true;
            present.PresentationType = "group";
            Group presentGroup = context.Group.Find(groupId);
            presentGroup.PresentationId = presentationId.ToString();
            presentGroup.HasPresentationPresenting = true;
            //get slides list and set first slide as current slide
            List<MultipleChoiceSlide> multipleChoiceSlides = context.MultipleChoiceSlide.Where(p => p.PresentationId == presentationId).ToList<MultipleChoiceSlide>();
            List<ParagraphSlide> paragraphSlides = context.ParagraphSlide.Where(p => p.PresentationId == presentationId).ToList<ParagraphSlide>();
            List<HeadingSlide> headingSLides = context.HeadingSlide.Where(p => p.PresentationId == presentationId).ToList<HeadingSlide>();
            List<Slide> slideList = new List<Slide>();
            slideList.AddRange(multipleChoiceSlides);
            slideList.AddRange(paragraphSlides);
            slideList.AddRange(headingSLides);

            var totalSlidesSorted = slideList.OrderBy(p => p.DateCreated);
            if (slideList.Count > 0)
            {
                var firstSlide = totalSlidesSorted.First();
                firstSlide.IsCurrent = 1;
                return context.SaveChanges();
            }
            return context.SaveChanges();
        }

        public int EndPresentation(Guid presentationId)
        {
            Presentation present = context.Presentation.Find(presentationId);
            present.IsBeingPresented = false;
            present.PresentationType = null;

            Group presetatingGroup = context.Group.Where(group => group.PresentationId == presentationId.ToString()).Select(g => g).FirstOrDefault();
            if (presetatingGroup != null)
            {
                presetatingGroup.PresentationId = null;
                presetatingGroup.HasPresentationPresenting = false;
            }

            Slide currentSlide = GetCurrentSlide(presentationId);
            if(currentSlide is not null)
            {
                string currentSlideId = currentSlide.SlideId;
                Slide slide = context.Slide.Find(currentSlideId);
                slide.IsCurrent = 0;
                

                return context.SaveChanges();
            }

            

            return context.SaveChanges();

        }

        public bool isPresentating(Guid presentationId)
        {
            Presentation presentation = context.Presentation.Find(presentationId);
            if (presentation != null && presentation.IsBeingPresented)
            {
                return true;
            }
            return false;
        }

        public int EndGroupPresentation(Guid presentationId, Guid groupId)
        {
            Presentation present = context.Presentation.Find(presentationId);
            present.IsBeingPresented = false;
            present.PresentationType = null;

            Group presentatingGroup = context.Group.Find(groupId);
            presentatingGroup.PresentationId = null;

            var currentSlide = GetCurrentSlide(presentationId);
            currentSlide.IsCurrent = 0;

            return context.SaveChanges();
        }

        public Slide GetCurrentSlide(Guid presentationId)
        {
            string currentMultipleChoiceSlide = context.MultipleChoiceSlide.Where(p => p.PresentationId == presentationId && p.IsCurrent == 1)
                                                                  .Select(p => p.SlideId)
                                                                  .FirstOrDefault();
            if(currentMultipleChoiceSlide != null)
            {
                return new MultipleChoiceSlide
                {
                    SlideId = currentMultipleChoiceSlide,
                    SlideType = "multipleChoice"
                };
            }

            string currentParagraphChoiceSlide = context.ParagraphSlide.Where(p => p.PresentationId == presentationId && p.IsCurrent == 1)
                                                                 .Select(p => p.SlideId)
                                                                 .FirstOrDefault();
            if (currentParagraphChoiceSlide != null)
            {
                return new ParagraphSlide
                {
                    SlideId = currentParagraphChoiceSlide,
                    SlideType = "paragraph"
                };
            }

            string currentHeadingChoiceSlide = context.HeadingSlide.Where(p => p.PresentationId == presentationId && p.IsCurrent == 1)
                                                                 .Select(p => p.SlideId)
                                                                 .FirstOrDefault();
            if (currentHeadingChoiceSlide != null)
            {
                return new HeadingSlide
                {
                    SlideId = currentHeadingChoiceSlide,
                    SlideType = "heading"
                };
            }
            return null;
        }

        public int HandleChangeSlide(string slideId, string slideType)
        {
            if (slideType == "MultipleChoice")
            {
                MultipleChoiceSlide multipleChoiceSlide = context.MultipleChoiceSlide.Find(slideId);
                multipleChoiceSlide.IsCurrent = 1;
                return context.SaveChanges();
            }
            if (slideType == "Heading")
            {
                HeadingSlide headingSlide = context.HeadingSlide.Find(slideId);
                headingSlide.IsCurrent = 1;
                return context.SaveChanges();
            }
            if (slideType == "Paragraph")
            {
                ParagraphSlide paragraphSlide = context.ParagraphSlide.Find(slideId);
                paragraphSlide.IsCurrent = 1;
                return context.SaveChanges();
            }
            return 0;
        }


        public object HandleReturnSLide(string slideId, string slideType)
        {
            if (slideType == "MultipleChoice")
            {
                return new MultipleChoiceSlide
                {
                    SlideId = slideId,
                    SlideType = slideType,
                };
            }
            if (slideType == "Heading")
            {
                return new HeadingSlide
                {
                    SlideId = slideId,
                    SlideType = slideType,
                };
            }
            if (slideType == "Paragraph")
            {
                return new ParagraphSlide
                {
                    SlideId = slideId,
                    SlideType = slideType,
                };
            }
            return null;
        }

        public object GetNextSlide(Guid presentationId)
        {

            var currentSlideId = GetCurrentSlide(presentationId).SlideId;
            Slide currentSlide = context.Slide.Find(currentSlideId);

            //get slides list and set first slide as current slide
            List<MultipleChoiceSlide> multipleChoiceSlides = context.MultipleChoiceSlide.Where(p => p.PresentationId == presentationId).ToList<MultipleChoiceSlide>();
            List<ParagraphSlide> paragraphSlides = context.ParagraphSlide.Where(p => p.PresentationId == presentationId).ToList<ParagraphSlide>();
            List<HeadingSlide> headingSLides = context.HeadingSlide.Where(p => p.PresentationId == presentationId).ToList<HeadingSlide>();
            List<Slide> slideList = new List<Slide>();
            slideList.AddRange(multipleChoiceSlides);
            slideList.AddRange(paragraphSlides);
            slideList.AddRange(headingSLides);
            slideList.OrderBy(p => p.DateCreated);

            var totalSlidesSorted = slideList.OrderBy(p => p.DateCreated).ToList();

            int slidesCount = totalSlidesSorted.Count;
            for(int i = 0; i < slidesCount; i++)
            {
                Slide slide = totalSlidesSorted[i];
                if(slide.SlideId == currentSlideId)
                {
                    currentSlide.IsCurrent = 0;
                    if(i + 1 < slidesCount)
                    {
                        HandleChangeSlide(totalSlidesSorted[i + 1].SlideId, totalSlidesSorted[i + 1].SlideType);
                        return HandleReturnSLide(totalSlidesSorted[i + 1].SlideId, totalSlidesSorted[i + 1].SlideType);
                    }
                }
            }
            return null;
        }


        public object GetPrevSlide(Guid presentationId)
        {
            var currentSlideId = GetCurrentSlide(presentationId).SlideId;
            Slide currentSlide = context.Slide.Find(currentSlideId);

            //get slides list and set first slide as current slide
            List<MultipleChoiceSlide> multipleChoiceSlides = context.MultipleChoiceSlide.Where(p => p.PresentationId == presentationId).ToList<MultipleChoiceSlide>();
            List<ParagraphSlide> paragraphSlides = context.ParagraphSlide.Where(p => p.PresentationId == presentationId).ToList<ParagraphSlide>();
            List<HeadingSlide> headingSLides = context.HeadingSlide.Where(p => p.PresentationId == presentationId).ToList<HeadingSlide>();
            List<Slide> slideList = new List<Slide>();
            slideList.AddRange(multipleChoiceSlides);
            slideList.AddRange(paragraphSlides);
            slideList.AddRange(headingSLides);
            slideList.OrderBy(p => p.DateCreated);

            var totalSlidesSorted = slideList.OrderBy(p => p.DateCreated).ToList();

            int slidesCount = totalSlidesSorted.Count;
            for (int i = 0; i < slidesCount; i++)
            {
                Slide slide = totalSlidesSorted[i];
                if (slide.SlideId == currentSlideId)
                {
                    currentSlide.IsCurrent = 0;
                    if (i - 1 > -1)
                    {
                        HandleChangeSlide(totalSlidesSorted[i - 1].SlideId, totalSlidesSorted[i - 1].SlideType);
                        return HandleReturnSLide(totalSlidesSorted[i - 1].SlideId, totalSlidesSorted[i - 1].SlideType);
                    }
                }
            }
            return null;
        }

        public string GetPresentationType(Guid presentationId)
        {
            Presentation presentation = context.Presentation.Find(presentationId);
            return presentation.PresentationType;
        }

        public Guid GetPresentGroup(Guid presentationId)
        {
            string presentId = presentationId.ToString();
            Guid groupId = context.Group.Where(g => g.HasPresentationPresenting == true && g.PresentationId == presentId)
                                        .Select(g => g.GroupId).FirstOrDefault();
            return groupId;
        }
    }
}
