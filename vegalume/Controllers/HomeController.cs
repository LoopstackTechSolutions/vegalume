using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
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
            if (isFuncionario)
            {
                var funcionario = _funcionarioRepositorio.ObterFuncionarioPeloEmail(email);

                if (funcionario != null)
                {
                    HttpContext.Session.SetInt32("WorkerId", funcionario.rm);
                    return RedirectToAction("HomeFuncionario", "Home", funcionario);
                }

                ViewBag.Email = email;
                ViewBag.Erro = "Email ou senha incorretos.(f)";
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
                    HttpContext.Session.SetObject("Carrinho", new List<PratoCarrinho>());
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Email = email;
                ViewBag.Erro = "Email ou senha incorretos.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult HomeFuncionario(string email)
        {
            Funcionario funcionario = _funcionarioRepositorio.ObterFuncionarioPeloEmail(email);
            return View(funcionario);
        }

        [HttpGet]
        public IActionResult MinhaConta()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}