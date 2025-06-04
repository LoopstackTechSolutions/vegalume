using Microsoft.AspNetCore.Mvc;
using vegalume.Models;
using vegalume.Repositorio;

namespace vegalume.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ClienteRepositorio _clienteRepositorio;
        private readonly FuncionarioRepositorio _funcionarioRepositorio;

        public ClienteController(ClienteRepositorio clienteRepositorio, FuncionarioRepositorio funcionarioRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
            _funcionarioRepositorio = funcionarioRepositorio;
        }

        public IActionResult CadastroCliente()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarCliente(Cliente cliente)
        {
            _clienteRepositorio.CadastrarCliente(cliente);
            if (!HttpContext.Session.GetInt32("WorkerId").HasValue)
            {
                int id = _clienteRepositorio.ObterClientePeloEmail(cliente.email).idCliente;
                HttpContext.Session.SetInt32("UserId", id);
                return RedirectToAction("Index", "Home");
            }
            string email = _funcionarioRepositorio.ObterFuncionarioPeloRm(HttpContext.Session.GetInt32("WorkerId")).email;
            return RedirectToAction("HomeFuncionario", "Home", new { email });
        }

        public IActionResult ObterCliente()
        {
            var cliente = _clienteRepositorio.ObterClientePeloId(HttpContext.Session.GetInt32("UserId"));
            return Json(cliente);
        }

        public IActionResult ObterClientePeloId(int IdCliente)
        {
            return Json(_clienteRepositorio.ObterClientePeloId(IdCliente));
        }

        [HttpPost]
        public IActionResult EditarCliente(Cliente cliente)
        {
            _clienteRepositorio.EditarCliente(cliente, HttpContext.Session.GetInt32("UserId"));
            return Redirect("/Home/MinhaConta#dados-cadastrais");
        }

        public IActionResult TodosClientes()
        {
            return Json(_clienteRepositorio.TodosClientes());
        }

        public IActionResult ListarCliente()
        {
            return View();
        }

        public IActionResult FiltrarClientes(string filtro)
        {
            return Json(_clienteRepositorio.FiltrarClientes(filtro));
        }

        [HttpGet]
        public IActionResult TodosEnderecos()
        {
            return Json(_clienteRepositorio.TodosEnderecos(HttpContext.Session.GetInt32("UserId")));
        }

        [HttpGet]
        public IActionResult TodosCartoes()
        {
            return Json(_clienteRepositorio.TodosCartoes(HttpContext.Session.GetInt32("UserId")));
        }

        [HttpPost]
        public IActionResult CadastrarEndereco(Endereco endereco)
        {
            _clienteRepositorio.CadastrarEndereco(endereco, HttpContext.Session.GetInt32("UserId"));
            return Redirect("/Home/MinhaConta#meus-enderecos");
        }

        [HttpPost]
        public IActionResult CadastrarCartao(Cartao cartao)
        {
            _clienteRepositorio.CadastrarCartao(cartao, HttpContext.Session.GetInt32("UserId"));
            return Redirect("/Home/MinhaConta#formas-de-pagamento");
        }

        [HttpPost]
        public IActionResult ExcluirEndereco(int idEndereco)
        {
            _clienteRepositorio.ExcluirEndereco(idEndereco);
            return Ok();
        }

        [HttpPost]
        public IActionResult ExcluirCartao(int idCartao)
        {
            _clienteRepositorio.ExcluirCartao(idCartao);
            return Ok();
        }
    }
}
