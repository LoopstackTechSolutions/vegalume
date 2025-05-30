using Microsoft.AspNetCore.Mvc;
using vegalume.Models;
using vegalume.Repositorio;

namespace vegalume.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ClienteRepositorio _clienteRepositorio;

        public ClienteController(ClienteRepositorio clienteRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
        }

        public IActionResult CadastroCliente()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarCliente(Cliente cliente)
        {

            _clienteRepositorio.Cadastrar(cliente);
            HttpContext.Session.SetInt32("UserId", cliente.idCliente);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult EditarCliente(int id)
        {
            var cliente = _clienteRepositorio.ObterClientePeloId(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        public IActionResult ObterCliente()
        {
            var cliente = _clienteRepositorio.ObterClientePeloId(HttpContext.Session.GetInt32("UserId"));
            return Json(cliente);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCliente(int id, [Bind("idCliente, nome, senha, telefone, email")] Cliente cliente)
        {
            if (id != cliente.idCliente)
            {
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (_clienteRepositorio.Atualizar(cliente))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocorreu um erro ao Editar.");
                    return View(cliente);
                }
            }
            return View(cliente);
        }

        [HttpGet]
        public IActionResult TodosEnderecos()
        {
            return Json(_clienteRepositorio.TodosEnderecos(HttpContext.Session.GetInt32("UserId")));
        }

        [HttpPost]
        public IActionResult CadastrarEndereco(Endereco endereco)
        {
            _clienteRepositorio.CadastrarEndereco(endereco, HttpContext.Session.GetInt32("UserId"));
            return Redirect("/Home/MinhaConta#meus-enderecos");
        }

        public IActionResult ExcluirCliente(int id)
        {
            _clienteRepositorio.Excluir(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
