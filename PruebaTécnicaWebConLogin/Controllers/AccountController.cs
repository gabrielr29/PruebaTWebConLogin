using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaTécnicaWebConLogin.Models;
using PruebaTécnicaWebConLogin.Repositories;

namespace PruebaTécnicaWebConLogin.Controllers
{
    public class AccountController : Controller
    {

        private readonly UsuarioRepository _usuarioRepository;
        private readonly Services.PasswordHasher _passwordHasher;

        public AccountController(UsuarioRepository usuarioRepository, Services.PasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Login()

        {

                return View("~/Views/Home/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ViewBag.ErrorUsuario = "El usuario es requerido.";
                return View("~/Views/Home/Login.cshtml");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorPassword = "La contraseña es requerida.";
                return View("~/Views/Home/Login.cshtml");
            }

            Usuario usuario = _usuarioRepository.ObtenerPorUsername(username);

            if (usuario == null)
            {
                ViewBag.ErrorUsuario = "Usuario incorrecto";
                return View("~/Views/Home/Login.cshtml");
            }

            // ===================================================================
            // VALIDACIÓN CRÍTICA: Control del tiempo de bloqueo (15 minutos)
            // ===================================================================
            if (usuario.Bloqueado == true || usuario.IntentosFallidos >= 5)
            {
                DateTime ahora = DateTime.Now;
                DateTime inicioBloqueo = usuario.FechaBloqueo ?? ahora;
                double minutosTranscurridos = (ahora - inicioBloqueo).TotalMinutes;

                if (minutosTranscurridos < 15)
                {
                    // Sigue dentro de la ventana de 15 minutos
                    return RedirectToAction("CuentaBloqueada", "Home");
                }
                else
                {
                    // Ventana superada: Se reestablece de inmediato en el almacén de datos
                    _usuarioRepository.RestablecerIntentos(username);

                    // Modificamos el objeto local para permitir que la ejecución continúe sin re-consultar
                    usuario.Bloqueado = false;
                    usuario.IntentosFallidos = 0;
                }
            }

            bool esValida = _passwordHasher.VerificarPassword(password, usuario.PasswordHash);

            if (!esValida)
            {
                _usuarioRepository.IncrementarIntentosFallidos(username);

                int intentosActuales = (usuario.IntentosFallidos ?? 0) + 1;

                if (intentosActuales >= 5)
                {
                    _usuarioRepository.BloquearCuenta(username);
                    return RedirectToAction("CuentaBloqueada", "Home");
                }

                ViewBag.ErrorPassword = "Contraseña incorrecta";
                return View("~/Views/Home/Login.cshtml");
            }

            // Autenticación Exitosa: Se limpian los contadores
            _usuarioRepository.RestablecerIntentos(username);
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpGet]
        public IActionResult ForzarCierreInactividad()
        {
            // Limpieza de cualquier rastro residual en la sesión del servidor
            HttpContext.Session.Clear();

            // Almacenamiento seguro y efímero de un solo uso para mitigar la manipulación de URLs
            TempData["SesionExpirada"] = true;

            return RedirectToAction("Login", "Account");
        }


        // GET: AccountControllerç
        public ActionResult Index()
        {
            return View();
        }

        // GET: AccountControllerç/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AccountControllerç/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountControllerç/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountControllerç/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccountControllerç/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountControllerç/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountControllerç/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
