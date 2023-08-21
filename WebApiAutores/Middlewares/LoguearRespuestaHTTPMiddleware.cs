using Microsoft.Extensions.Logging;

namespace WebApiAutores.Middlewares
{
    public static class LoguearRespuestaHTTPMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
        }
    }
    public class LoguearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;


        public LoguearRespuestaHTTPMiddleware(RequestDelegate next, ILogger<LoguearRespuestaHTTPMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        // Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext contexto)
        {
            // Entrada del middleware

            // 1. Creo un MemoryStream para poder manipular
            // y copiarme el cuerpo de la respuesta.
            // Esto se hace porque el stream del cuerpo de la
            // respuesta no tiene permisos de lectura.
            using var ms = new MemoryStream();

            // 2. Guardo la referencia del Stream donde se
            // escribe el cuerpo de la respuesta
            var cuerpoOriginalRespues = contexto.Response.Body;

            // 3. Cambio el stream por defecto del cuerpo
            // de la respuesta por el MemoryStream creado
            // para poder manipularlo
            contexto.Response.Body = ms;

            // 4. Esperamos a que el siguiente middleware
            // devuelva la respuesta.
            await next(contexto);

            // Salida del middleware

            // 5. Nos movemos al principio del MemoryStream
            // Para copiar el cuerpo de la respuesta
            ms.Seek(0, SeekOrigin.Begin);

            // 6. Leemos stream hasta el final y almacenamos
            // el cuerpo de la respuesta obtenida
            var respuesta = new StreamReader(ms).ReadToEnd();

            // 5. Nos volvemos a posicionar al principio
            // del MemoryStream para poder copiarlo al 
            // cuerpo original de la respuesta
            ms.Seek(0, SeekOrigin.Begin);

            // 7. Copiamos el contenido del MemoryStream al
            // stream original del cuerpo de la respuesta
            await ms.CopyToAsync(cuerpoOriginalRespues);

            // 8.Volvemos asignar el stream original al el cuerpo
            // de la respuesta para que siga el flujo normal.
            contexto.Response.Body = cuerpoOriginalRespues;

            // 9. Escribimos en el log la respuesta obtenida
            logger.LogInformation(respuesta);
        }
    }
}
