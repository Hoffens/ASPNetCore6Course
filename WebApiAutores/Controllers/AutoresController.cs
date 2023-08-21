using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.Services;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/[controller]")]     // ruta => api/autores (nombre del controlador, Autores)
    //[Authorize]   // todos los endpoints del controlador estan protegidos
    public class AutoresController : ControllerBase
    {
        public readonly ApplicationDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;

        public AutoresController(ApplicationDbContext context, IServicio servicio,
            ServicioTransient servicioTransient, ServicioScoped servicioScoped,
            ServicioSingleton servicioSingleton)
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
        }

        [HttpGet("GUID")]
        [ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult ObtenerGuids()
        {
            return Ok(new {
                AutoresController_Transient = servicioTransient.Guid,
                AutoresController_Scoped = servicioScoped.Guid,
                Autores_Singleton = servicioSingleton.Guid,
                ServicioA_Transients = servicio.ObtenerTransients(),
                ServicioA_Scoped = servicio.ObtenerScoped(),
                ServicioA_Singleton = servicio.ObtenerSingleton(),
            });
        }

        // Endpoint con triple ruta => api/autores, api/autores/listado y listado
        [HttpGet]   // hereda ruta api/autores
        [HttpGet("listado")]    // concatena ruta del controlador => api/autores/listado
        [HttpGet("/listado")] // nueva ruta => listado
        public async Task<ActionResult<List<Autor>>> Get()
        {
            servicio.RealizarTarea();
            return await context.Autores.ToListAsync();
        }

        [HttpGet("primero")] // api/autores/primero
        public async Task<ActionResult<Autor>> PrimerAutor()
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        // parametros de ruta
        [HttpGet("{id:int}/{param2?}")]   // api/autores/{id}/{param2} (param2 es opcional)
        public async Task<ActionResult<Autor>> AutorPorId(int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor/*[FromBody] Autor autor => model binding*/)
        {
            context.Add(autor);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:int}")] // parametro de ruta => api/autores/1
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            var exists = await context.Autores.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            context.Update(autor);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        //[Authorize] este endpoint esta protegido
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Autores.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
