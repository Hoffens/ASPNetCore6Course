using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor : IValidatableObject
    {
        public int Id { get; set; }

        //[PrimeraLetraMayuscula]  // validacion personalizada por atributo
        [Required(ErrorMessage = "Custom message error: {0}")]  /* Siempre se debe otorgar el valor de este campo */
        [StringLength(maximumLength: 100, ErrorMessage = "Se supero el limite de 100 caracteres")] /* La longitud maxima es de 100 caracteres */
        public string Nombre { get; set; }

        [Range(18, 120)] // el numero debe estar entre 18 y 120
        [NotMapped] // no se mapea en la db
        public int Edad { get; set; }

        [CreditCard] // valida que la numeracion de la tarjeta sea correcta
        public string TarjetaDeCredito { get; set; }

        [Url] // que sea URL
        public string URL { get; set; }
        public int Menor { get; set; }
        public int Mayor { get; set; }
        public List<Libro> Libros { get; set; }

        // Validacion de todo el modelo 
        // Para que se ejecute en una propiedad especifica, esa propiedad debe pasar todas las validaciones por
        // atributo que tenga
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    // yield inserta un elemento en la coleccion que vamos a retornar (IEnumerable<ValidationResult>)
                    yield return new ValidationResult("La primera letra debe ser mayuscula", 
                        new string[] { nameof(Nombre) });
                }
            }

            if (Menor > Mayor)
            {
                yield return new ValidationResult("Este valor no puede ser mas grande que el campo mayor",
                    new string[] { nameof(Menor) });
            }
        }
    }
}
