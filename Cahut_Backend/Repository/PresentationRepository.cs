using Cahut_Backend.Models;

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

        public int Update(Guid presentationId, string newName)
        {
            Presentation present = context.Presentation.Find(presentationId);
            present.PresentationName = newName;
            return context.SaveChanges();
        }

        public List<object> GetPresentationList(Guid userId)
        {
            var res = from present in context.Presentation
                      where present.TeacherId == userId
                      select new
                      {
                          createdDate = present.CreatedDate,
                          presentationName = present.PresentationName,
                      };
            return res.ToList<object>();
        }
    }
}
