using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ApiLogitech.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace ApiLogitech.Controllers
{
    [Authorize] //Autorizacion para todos los metodos
    [EnableCors("reglaCORS")] //habiliar CORS
    [Route("api/[controller]")]   
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly string cadenaSQL;
        public ProductoController(IConfiguration configuracion)
        {
            cadenaSQL = configuracion.GetConnectionString("CadenaSQL");
        }
        
        [HttpGet] //Obtener
        [Route("Lista")] //Lista de productos
        public IActionResult Lista()
        {
            List<Producto> lst = new List<Producto>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var comando = new SqlCommand("sp_lista_productos", conexion);
                    comando.CommandType = CommandType.StoredProcedure;
                    using (var rd = comando.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lst.Add(new Producto()
                            {
                                IdProducto = Convert.ToInt32(rd["IdProducto"]),
                                CodigoBarra = rd["CodigoBarra"].ToString(),
                                Nombre = rd["Nombre"].ToString(),
                                Marca = rd["Marca"].ToString(),
                                Categoria = rd["Categoria"].ToString(),
                                Precio = Convert.ToDecimal(rd["Precio"])
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lst });
            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = lst });
            }
        }
        [HttpGet] //Obtener
        [Route("Obtener/{idProducto:int}")] //Obtener producto por Id
        public IActionResult Obtener(int idProducto)
        {
            List<Producto> lst = new List<Producto>();
            Producto producto = new Producto();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var comando = new SqlCommand("sp_lista_productos", conexion);
                    comando.CommandType = CommandType.StoredProcedure;
                    using (var rd = comando.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lst.Add(new Producto()
                            {
                                IdProducto = Convert.ToInt32(rd["IdProducto"]),
                                CodigoBarra = rd["CodigoBarra"].ToString(),
                                Nombre = rd["Nombre"].ToString(),
                                Marca = rd["Marca"].ToString(),
                                Categoria = rd["Categoria"].ToString(),
                                Precio = Convert.ToDecimal(rd["Precio"])
                            });
                        }
                    }
                }
                //El producto es = a la lista donde el item del producto sea igual al idProducto y retorna el primero o un valor nulo
                producto = lst.Where(item => item.IdProducto == idProducto).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = producto });
            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = producto });
            }
        }
        [HttpPost] //Insertar
        [Route("Guardar")] //Guardar producto
        public IActionResult Guardar([FromBody]Producto oProducto) //Recibe una estructura de la clase producto
        {           
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var comando = new SqlCommand("sp_guardar_producto", conexion);
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@codigoBarra", oProducto.CodigoBarra);
                    comando.Parameters.AddWithValue("@nombre", oProducto.Nombre);
                    comando.Parameters.AddWithValue("@marca", oProducto.Marca);
                    comando.Parameters.AddWithValue("@categoria", oProducto.Categoria);
                    comando.Parameters.AddWithValue("@precio", oProducto.Precio);                   
                    comando.ExecuteNonQuery();
                }               
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok"});
            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message});
            }
        }
        [HttpPut] //Modificar
        [Route("Editar")] //Modificar producto
        public IActionResult Editar([FromBody] Producto oProducto) //Recibe una estructura de la clase producto
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var comando = new SqlCommand("sp_editar_producto", conexion);
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@idProducto", oProducto.IdProducto == 0 ? DBNull.Value : oProducto.IdProducto); //si es 0 o null retorna null, si no toma idProducto
                    comando.Parameters.AddWithValue("@codigoBarra", oProducto.CodigoBarra is null ? DBNull.Value : oProducto.CodigoBarra); // si es null retorna null, si no toma el codigo de barra
                    comando.Parameters.AddWithValue("@nombre", oProducto.Nombre is null ? DBNull.Value : oProducto.Nombre); //lo mismo...
                    comando.Parameters.AddWithValue("@marca", oProducto.Marca is null ? DBNull.Value : oProducto.Marca);
                    comando.Parameters.AddWithValue("@categoria", oProducto.Categoria is null ? DBNull.Value : oProducto.Categoria);
                    comando.Parameters.AddWithValue("@precio", oProducto.Precio == 0 ? DBNull.Value : oProducto.Precio);                    
                    comando.ExecuteNonQuery();
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "editado" });
            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        [HttpDelete] //Eliminar
        [Route("Eliminar/{idProducto:int}")] //Eliminar producto por id
        public IActionResult Eliminar(int idProducto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var comando = new SqlCommand("sp_eliminar_producto", conexion);
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@idProducto", idProducto);
                    comando.ExecuteNonQuery();
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "eliminado" });
            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}
