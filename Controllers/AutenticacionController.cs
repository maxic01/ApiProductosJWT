using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiLogitech.Models;
using System.Security.Claims; //1
using Microsoft.IdentityModel.Tokens; //2
using System.IdentityModel.Tokens.Jwt; //3
using System.Text;

namespace ApiLogitech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly string secretKey;
        public AutenticacionController(IConfiguration configuration)
        {
            secretKey = configuration.GetSection("settings").GetSection("secretkey").ToString(); //obtener secret key
        }

        [HttpPost]
        [Route("Validar")]
        public IActionResult Validar([FromBody] Credencial request) //Recibe una estructura de la clase credencial
        {
            if(request.Correo == "maxi1998calvo@gmail.com" && request.Clave == "123")
            {
                var keyBytes = Encoding.ASCII.GetBytes(secretKey); //convertir secret key a bytes
                var claims = new ClaimsIdentity(); //crear solicitudes de permisos
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.Correo)); //agregar solicitud de permiso
                var tokenDescripcion = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(15),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };
                //Lectura token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescripcion);
                string tokenCreado = tokenHandler.WriteToken(tokenConfig); //Obtener token creado
                return StatusCode(StatusCodes.Status200OK, new {token = tokenCreado});
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
            }
        }
    }
}
