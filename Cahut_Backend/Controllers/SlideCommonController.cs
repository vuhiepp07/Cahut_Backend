using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cahut_Backend.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class SlideCommonController : BaseController
    {
        //Xử lí hàm chung cho các slide trong controller này (Create, update, delete slide..)
        //Các controller con như MultipleChoiceSlideController hay ParagraphController để xử lí 1 số field riêng mà chỉ loại slide đó mới có
    }
}
