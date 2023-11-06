using AutoMapper;
using FinalProject.DTO;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    [Route("api/category")]
    [ApiController]
    //[Authorize]
    public class CategoryController : Controller
    {
        public PRN231DBContext _context;
        public IMapper mapper;

        public CategoryController(PRN231DBContext _context)
        {
            this._context = _context;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this.mapper = config.CreateMapper();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Category>> GetAllCate()
        {
            var categories = await _context.Categories.ToListAsync();
            if (categories.Count() != 0)
            {
                return Ok(mapper.Map<List<CategoryDTO>>(categories));
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var categories = await _context.Categories.Where(x=>x.CategoryId == id).FirstOrDefaultAsync();
            return Ok(categories);
        }
    }
}
