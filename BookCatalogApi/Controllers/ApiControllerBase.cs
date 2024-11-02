using AutoMapper;
using BookCatalogApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogApi.Controllers
{
    [ApiController]
   // [ValidationActionFilter]
  //  [CustomExceptionFilter]
    public class ApiControllerBase : ControllerBase
    {
        private readonly IMapper mapper;
        protected IMapper _mapper => mapper ?? HttpContext.RequestServices.GetRequiredService<IMapper>();
      
    }
}
