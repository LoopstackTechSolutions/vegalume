using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using vegalume.Models;
using vegalume.Repositorio;

namespace vegalume.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClienteRepositorio _clienteRepositorio;
        private readonly FuncionarioRepositorio _funcionarioRepositorio;

        public HomeController(FuncionarioRepositorio funcionarioRepositorio, ClienteRepositorio clienteRepositorio, ILogger<HomeController> logger)
        {
            _funcionarioRepositorio = funcionarioRepositorio;
            _clienteRepositorio = clienteRepositorio;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha, bool isFuncionario)
        {
            if (isFuncionario) {
                ViewBag.Erro = "funcionário!";
                return View();
            }
            else
            {
                var clientes = _clienteRepositorio.TodosClientes();

                var cliente = clientes.FirstOrDefault(u =>
                u.email == email && u.senha == senha);

                if (cliente != null)
                {
                    HttpContext.Session.SetInt32("UserId", cliente.idCliente);
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Email = email;
                ViewBag.Erro = "Email ou senha incorretos.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
