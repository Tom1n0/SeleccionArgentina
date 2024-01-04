using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApplicationSeleccionArgentina.Data;
using WebApplicationSeleccionArgentina.Models;

namespace WebApplicationSeleccionArgentina.Controllers
{
    public class ImagenController : Controller
    {
        private readonly Context _context;
        public ImagenController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            using (SqlConnection con = new(_context.Conexion))
            {
                List<Imagen> listaImagenes = new();
                using (SqlCommand cmd = new("sp_listar_imagenes", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();

                    var rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        listaImagenes.Add(new Imagen
                        {
                            IdImagen = (int)rd["IdImagen"],
                            Nombre = rd["Nombre"].ToString(),
                            Image = rd["Imagen"].ToString(),
                            Edad = (int)rd["Edad"],
                            Equipo = rd["Equipo"].ToString(),
                            Altura = (decimal)rd["Altura"]

                        });
                    }
                }
                ViewBag.listado = listaImagenes;
                return View();
            }
        }
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Imagen imagen)
        {
            try
            {
                byte[] bytes;
                if (imagen.File != null && imagen.Nombre != null)
                {
                    using (Stream fs = imagen.File.OpenReadStream())
                    {
                        using (BinaryReader br = new(fs))
                        {
                            bytes = br.ReadBytes((int)fs.Length);
                            imagen.Image = Convert.ToBase64String(bytes, 0, bytes.Length);

                            using (SqlConnection con = new(_context.Conexion))
                            {
                                using (SqlCommand cmd = new("sp_insertar_imagen", con))
                                {
                                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                    cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.VarChar).Value = imagen.Nombre;
                                    cmd.Parameters.Add("@Imagen", System.Data.SqlDbType.VarChar).Value = imagen.Image;
                                    cmd.Parameters.Add("@Edad", System.Data.SqlDbType.Int).Value = imagen.Edad;
                                    cmd.Parameters.Add("@Equipo", System.Data.SqlDbType.VarChar).Value = imagen.Equipo;
                                    cmd.Parameters.Add("@Altura", System.Data.SqlDbType.Decimal).Value = Convert.ToDecimal(imagen.Altura);
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException e)
            {
                ViewBag.error = e.Message;
                return View();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            using (SqlConnection con = new(_context.Conexion))
            {
                Imagen registro = new();
                using (SqlCommand cmd = new("sp_buscar_imagenes", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = id;
                    con.Open();

                    SqlDataAdapter da = new(cmd);
                    DataTable dt = new();
                    da.Fill(dt);
                    registro.IdImagen = (int)dt.Rows[0][0];
                    registro.Nombre = dt.Rows[0][1].ToString();
                    registro.Image = dt.Rows[0][2].ToString();
                    registro.Edad = (int)dt.Rows[0][3];
                    registro.Equipo = dt.Rows[0][4].ToString();
                    registro.Altura = (decimal)dt.Rows[0][5];

                }
                return View(registro);
            }
        }

        [HttpPost]
        public IActionResult Editar(Imagen imagen)
        {
            try
            {
                using (SqlConnection con = new(_context.Conexion))
                {
                    string i;
                    if (imagen.File == null)
                    {
                        i = "null";
                    }
                    else
                    {
                        byte[] bytes;
                        using (Stream fs = imagen.File.OpenReadStream())
                        {
                            using (BinaryReader br = new(fs))
                            {
                                bytes = br.ReadBytes((int)fs.Length);
                                i = Convert.ToBase64String(bytes, 0, bytes.Length);
                            }
                        }
                    }
                    using (SqlCommand cmd = new("sp_actualizar_imagen", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = imagen.IdImagen;
                        cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.VarChar).Value = imagen.Nombre;
                        cmd.Parameters.Add("@Imagen", System.Data.SqlDbType.VarChar).Value = i;
                        cmd.Parameters.Add("@Edad", System.Data.SqlDbType.Int).Value = imagen.Edad;
                        cmd.Parameters.Add("@Equipo", System.Data.SqlDbType.VarChar).Value = imagen.Equipo;
                        cmd.Parameters.Add("@Altura", System.Data.SqlDbType.Decimal).Value = Convert.ToDecimal(imagen.Altura);


                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (SystemException e)
            {
                ViewBag.error = e.Message;
                return View();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Eliminar(int id)
        {
            using (SqlConnection con = new(_context.Conexion))
            {
                Imagen registro = new();
                using (SqlCommand cmd = new("sp_buscar_imagenes", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = id;
                    con.Open();

                    SqlDataAdapter da = new(cmd);
                    DataTable dt = new();
                    da.Fill(dt);
                    registro.IdImagen = (int)dt.Rows[0][0];
                    registro.Nombre = dt.Rows[0][1].ToString();
                    registro.Image = dt.Rows[0][2].ToString();
                    registro.Edad = (int)dt.Rows[0][3];
                    registro.Equipo = dt.Rows[0][4].ToString();
                    registro.Altura = (decimal)dt.Rows[0][5];
                }
                return View(registro);
            }
        }

        [HttpPost]
        public IActionResult Eliminar(Imagen img)
        {
            using (SqlConnection con = new(_context.Conexion))
            {
                using (SqlCommand cmd = new("sp_eliminar_imagen", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = img.IdImagen;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("Index");
            }
        }
    }
}
