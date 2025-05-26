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

        public IActionResult Index()
        {
            return View(_clienteRepositorio.TodosClientes());
        }


        public IActionResult CadastrarCliente()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarCliente(Cliente cliente)
        {

            _clienteRepositorio.Cadastrar(cliente);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditarCliente(int id)
        {
            var cliente = _clienteRepositorio.ObterCliente(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
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


        public IActionResult ExcluirCliente(int id)
        {
            _clienteRepositorio.Excluir(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
