using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using WebAPINwind.Data;
using WebAPINwind.Models;

namespace WebAPINwind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovementdetailsController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public MovementdetailsController(NorthwindContext context)
        {
            _context = context;
        }

        // GET: api/Movementdetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movementdetail>>> GetMovementdetails()
        {
            if (_context.Movementdetails == null)
            {
                return NotFound();
            }
            return await _context.Movementdetails.ToListAsync();
        }

        // GET: api/Movementdetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movementdetail>> GetMovementdetail(int id)
        {
            if (_context.Movementdetails == null)
            {
                return NotFound();
            }
            var movementdetail = await _context.Movementdetails.FindAsync(id);

            if (movementdetail == null)
            {
                return NotFound();
            }

            return movementdetail;
        }

        // PUT: api/Movementdetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovementdetail(int id, Movementdetail movementdetail)
        {
            if (id != movementdetail.MovementId)
            {
                return BadRequest();
            }

            _context.Entry(movementdetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovementdetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Movementdetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Movementdetail>> PostMovementdetail(Movementdetail movementdetail)
        {
            if (_context.Movementdetails == null)
            {
                return Problem("Entity set 'NorthwindContext.Movementdetails'  is null.");
            }
            _context.Movementdetails.Add(movementdetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MovementdetailExists(movementdetail.MovementId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMovementdetail", new { id = movementdetail.MovementId }, movementdetail);
        }

        // DELETE: api/Movementdetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovementdetail(int id)
        {
            if (_context.Movementdetails == null)
            {
                return NotFound();
            }
            var movementdetail = await _context.Movementdetails.FindAsync(id);
            if (movementdetail == null)
            {
                return NotFound();
            }

            _context.Movementdetails.Remove(movementdetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovementdetailExists(int id)
        {
            return (_context.Movementdetails?.Any(e => e.MovementId == id)).GetValueOrDefault();
        }


        [HttpGet]
        [Route("top5")]
        public IEnumerable<Object> CategoryProductTop() {
            var Query = (from md in _context.Movementdetails
                         join p in _context.Products on md.ProductId equals p.ProductId
                         join c in _context.Categories on p.CategoryId equals c.CategoryId
                         where p.CompanyId == 1
                         select new
                         {
                             Product = p.ProductName,
                             Category = c.CategoryName,
                             Quantity1 = (from md in _context.Movementdetails
                                          where md.ProductId == p.ProductId
                                          orderby md.Quantity descending
                                          select md.Quantity).Sum()
                         }).Distinct().OrderBy(x => x.Category).ThenByDescending(x => x.Quantity1);

            var products = Query.ToList();
            List<Object> result = new List<Object>();

            String category = "";
            int cont = 0;
            foreach (var product in products) {
                if (product.Category != category) {
                    category = product.Category;
                    result.Add(product);
                    cont = 1;
                    continue;
                }
                if (cont < 5) {
                    result.Add(product);
                    cont++;
                }
            }

            return result;
        }
    }
}
