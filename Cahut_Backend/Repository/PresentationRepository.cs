﻿using Cahut_Backend.Models;
using Newtonsoft.Json.Linq;

namespace Cahut_Backend.Repository
{
    public class PresentationRepository : BaseRepository
    {
        public PresentationRepository(AppDbContext context) : base(context)
        {
        }

        public bool CheckExisted(Guid userId, string presentationName)
        {
            return context.Presentation.Any(p => p.TeacherId == userId && p.PresentationName == presentationName);
        }

        public int Create(Guid userId, string presentationName)
        {
            Presentation present = new Presentation
            {
                PresentationName = presentationName,
                CreatedDate = DateTime.UtcNow.AddHours(7),
                TeacherId = userId
            };
            context.Presentation.Add(present);
            return context.SaveChanges();
        }
        public Presentation GetPresentationByNameAndTeacherId(string name, Guid teacherId)
        {
            return context.Presentation.Where(p => p.PresentationName == name && p.TeacherId == teacherId).SingleOrDefault();
        }

        public int Delete(Guid presentationId)
        {
            Presentation present = context.Presentation.Find(presentationId);
            context.Remove(present);
            return context.SaveChanges();
        }

        public int Update(Guid presentationId, Guid userId, string newName)
        {
            Presentation present = context.Presentation.Where(p => p.PresentationId == presentationId && p.TeacherId == userId).SingleOrDefault();
            present.PresentationName = newName;
            return context.SaveChanges();
        }

        public int CountNumOfSlide(Guid presentationId)
        {
            return context.Slide.Count(p => p.PresentationId == presentationId);
        }

        public List<object> GetPresentationList(Guid userId)
        {
            Dictionary<Guid, int> presentDict = new Dictionary<Guid, int>();
            var countNum = from slide in context.Slide
                           group slide by slide.PresentationId into g
                           select new
                           {
                               count = g.Count(),
                               presentationId = g.Key,
                           };
            foreach(var item in countNum)
            {
                presentDict.Add(item.presentationId, item.count);
            }

            var res = from present in context.Presentation
                      where present.TeacherId == userId
                      select new
                      {
                          numOfSlides = presentDict.ContainsKey(present.PresentationId)?presentDict[present.PresentationId]:0,
                          presentationId = present.PresentationId,
                          createdDate = present.CreatedDate,
                          presentationName = present.PresentationName,
                      };
            return res.ToList<object>();
        }
        public bool presentationExisted(Guid presentationId, Guid userId)
        {
            return context.Presentation.Any(p => p.PresentationId == presentationId && p.TeacherId == userId);
        }

        public string GetPresentationName(Guid presentationId)
        {
            return context.Presentation.Find(presentationId).PresentationName;
        }

        public List<object> GetPresentationSlides(Guid presentationId)
        {
            List<object> lst = (from Slide in context.Slide
                               orderby Slide.DateCreated
                               where Slide.PresentationId == presentationId
                               select new
                               {
                                   slideId = Slide.SlideId,
                                   dateCreated = Slide.DateCreated
                               }).ToList<object>();
            return lst;
        }

        public int CountPresentationOwned(Guid userId)
        {
            return context.Presentation.Count(p => p.TeacherId == userId);
        }
    }
}
