using AutoMapper;
using FinalProject.DTO;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    [Route("api/product")]
    [ApiController]
    //[Authorize]
    public class ProductController : Controller
    {
        public PRN231DBContext _context;
        public IMapper mapper;

        public ProductController(PRN231DBContext _context)
        {
            this._context = _context;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this.mapper = config.CreateMapper();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<ProductDTO>> GetAll()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            if (products.Count() != 0)
            {
                return Ok(mapper.Map<List<ProductDTO>>(products));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products.Include(p => p.Category).Where(p => p.ProductId == id).FirstOrDefaultAsync();
            if (product != null)
            {
                return Ok(mapper.Map<ProductDTO>(product));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = "EmpOnly")]
        public async Task<IActionResult> Create(Product E)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var C = await _context.Products.FindAsync(E.ProductId);
                if (C != null)
                    return NoContent();

                await _context.AddAsync<Product>(E);
                await _context.SaveChangesAsync();
                return Ok(E);
            }
            catch (Exception ex)
            {
                return NoContent();
            }

        }

        [HttpDelete("[action]")]
        [Authorize(Policy = "EmpOnly")]
        public ActionResult<string> Delete(int id)
        {
            try
            {
                var product = _context.Products
               .Where(e => e.ProductId == id).FirstOrDefault();
                var orderDetail = _context.OrderDetails.Where(o => o.ProductId == id).ToList();
                if (product != null)
                {
                    foreach (var item in orderDetail)
                    {
                        _context.Remove<OrderDetail>(item);

                    }
                    _context.Remove<Product>(product);
                    _context.SaveChanges();
                    return Ok("Delete success product with id = " + id + "!");
                }
                else
                {
                    return BadRequest("Product id doesn't existed!");
                }
            }
            catch (Exception)
            {
                return BadRequest("Can not delete product with id = " + id + "!");
            }

        }

        [HttpPut("[action]")]
        [Authorize(Policy = "EmpOnly")]
        public IActionResult Update(Product product)
        {
            var emp = _context.Products.Where(e => e.ProductId == product.ProductId).AsNoTracking().FirstOrDefault();
            if (emp != null)
            {
                if (ModelState.IsValid)
                {
                    _context.Update<Product>(product);
                    _context.SaveChanges();
                    return Ok(product);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet("GetProductByCategory/{id}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(int id)
        {
            var product = await _context.Products.Where(e => e.CategoryId == id).Include(x => x.Category).ToListAsync();

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }


        [HttpGet("[action]")]
        [Authorize(Policy = "EmpOnly")]
        public async Task<ActionResult<ProductDTO>> GetWithFilter(int categoryId, string? search)
        {
            if (search == null)
            {
                search = "";
            }

            var products = await _context.Products.Include(p => p.Category)
                .OrderByDescending(p => p.ProductId)
                .Where(p => (categoryId == 0 || p.CategoryId == categoryId) && p.ProductName.Contains(search))
                .ToListAsync();

            if (products.Count() != 0)
            {
                return Ok(mapper.Map<List<ProductDTO>>(products));
            }
            else
            {
                return Ok(new List<ProductDTO>());
            }
        }
        
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductBestSale()
        {
            var saleproduct = await _context.OrderDetails.OrderByDescending(od => od.Discount).Take(6).Include(od => od.Product).ThenInclude(x => x.Category).Select(x => x.Product).ToListAsync();

            return saleproduct;
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductHot()
        {
            var hot1 = await _context.OrderDetails.Where(x => x.Discount > 0)
                                                  .Include(x => x.Product)
                                                  .GroupBy(x => x.ProductId)
                                                  .OrderByDescending(x => x.Count(x => x.ProductId == x.ProductId))
                                                  .Select(x => x.SingleOrDefault().Product).Take(6)
                                                  .ToListAsync();
            return hot1;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductNew()
        {
            var newest = await _context.Products.OrderByDescending(id => id.ProductId).Take(6).ToListAsync();
            //return await _context.Products.ToListAsync();
            return newest;

        }
    }
}
