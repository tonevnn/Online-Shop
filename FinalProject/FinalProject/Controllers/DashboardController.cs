using AutoMapper;
using FinalProject.DTO;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class DashboardController : Controller
    {
        public PRN231DBContext _context;
        public IMapper mapper;

        public DashboardController(PRN231DBContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            this.mapper = config.CreateMapper();
        }

        [HttpGet("[action]")]
        [Authorize(Policy = "EmpOnly")]
        public IActionResult GetDashboard()
        {
            decimal totalOrders = _context.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * ((decimal)(1 - od.Discount)));
            var totalCustomer = _context.Customers.Select(c => c.CustomerId).ToList().Count;
            var totalGuest = _context.Customers.Select(c => c.CustomerId).ToList()
                .Except(_context.Accounts.Where(a => a.CustomerId != null).Select(a => a.CustomerId).ToList())
                .ToList().Count;
            DashboardDTO dashboardDTO = new DashboardDTO
            {
                TotalOrders = totalOrders,
                TotalCustomer = totalCustomer,
                TotalGuest = totalGuest
            };
            return Ok(dashboardDTO);
        }
        [HttpGet("[action]")]
        public IActionResult GetYear()
        {
            var years = _context.Orders.GroupBy(y => y.OrderDate.Value.Year);
            return Ok(years);
        }
        [HttpGet("[action]")]
        [Authorize(Policy = "EmpOnly")]
        public IActionResult GetStaticOrder(int year)
        {
            var list = _context.Orders.Where(o => o.OrderDate.Value.Year == year);
            var result = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                var orderByMonth = list.Where(o => o.OrderDate != null && o.OrderDate.Value.Month == i).ToList();
                result.Add(orderByMonth.Count);
            }
            return Ok(result);
        }
    }
}
