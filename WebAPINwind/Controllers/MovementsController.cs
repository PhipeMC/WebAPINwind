using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPINwind.Data;
using WebAPINwind.Models;

namespace WebAPINwind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovementsController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public MovementsController(NorthwindContext context)
        {
            _context = context;
        }

        // GET: api/Movements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movement>>> GetMovements()
        {
            if (_context.Movements == null)
            {
                return NotFound();
            }
            return await _context.Movements.ToListAsync();
        }

        // GET: api/Movements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movement>> GetMovement(int id)
        {
            if (_context.Movements == null)
            {
                return NotFound();
            }
            var movement = await _context.Movements.FindAsync(id);

            if (movement == null)
            {
                return NotFound();
            }

            return movement;
        }

        // PUT: api/Movements/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovement(int id, Movement movement)
        {
            if (id != movement.MovementId)
            {
                return BadRequest();
            }

            _context.Entry(movement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovementExists(id))
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

        // POST: api/Movements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Movement>> PostMovement(Movement movement)
        {
            if (_context.Movements == null)
            {
                return Problem("Entity set 'NorthwindContext.Movements'  is null.");
            }
            _context.Movements.Add(movement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovement", new { id = movement.MovementId }, movement);
        }

        // DELETE: api/Movements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovement(int id)
        {
            if (_context.Movements == null)
            {
                return NotFound();
            }
            var movement = await _context.Movements.FindAsync(id);
            if (movement == null)
            {
                return NotFound();
            }

            _context.Movements.Remove(movement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovementExists(int id)
        {
            return (_context.Movements?.Any(e => e.MovementId == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Route("comportamientoproducto")]
        public IEnumerable<Object> comportamientoProducto(int productid, DateTime fechaInicio, DateTime fechaLimite)
        {
            return _context.Movements
                .Where(m => (m.CompanyId == 1) && (m.Type == "VENTA") && (m.Date >= fechaInicio && m.Date <= fechaLimite))
                .Join(_context.Movementdetails,
                    m => m.MovementId,
                    md => md.MovementId,
                    (m, md) => new
                    {
                        MovementId = m.MovementId,
                        ProductId = md.ProductId,
                        Fecha = m.Date,
                        Tipo = m.Type,
                        Cantidad = md.Quantity
                    })
                .Join(_context.Products,
                    mmd => mmd.ProductId,
                    p => p.ProductId,
                    (mmd, p) => new
                    {
                        mmd.ProductId,
                        p.ProductName,
                        mmd.MovementId,
                        mmd.Fecha,
                        mmd.Cantidad
                    })
                .Where(x => x.ProductId == productid)
                .GroupBy(x => new { x.Fecha.Month, x.Fecha.Year })
                .Select(x => new
                {
                    Nombre = x.Select(a => a.ProductName).First(),
                    Mes = x.Select(a => a.Fecha.Month).First(),
                    Año = x.Select(a => a.Fecha.Year).First(),
                    Cantidad = x.Sum(a => a.Cantidad)
                });
        }
    }
}
