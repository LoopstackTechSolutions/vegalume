using Microsoft.AspNetCore.Mvc;
using vegalume.Models;
using vegalume.Repositorio;

namespace vegalume.Controllers
{
    public class PedidoController : Controller
    {
        private readonly PedidoRepositorio _pedidoRepositorio;

        public PedidoController(PedidoRepositorio pedidoRepositorio)
        {
            _pedidoRepositorio = pedidoRepositorio;
        }

        public IActionResult Index()
        {
            return View(_pedidoRepositorio.TodosPedidos());
        }


        public IActionResult CadastrarPedido()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarPedido(Pedido pedido)
        {

            _pedidoRepositorio.Cadastrar(pedido);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditarPedido(int id)
        {
            var pedido = _pedidoRepositorio.ObterPedido(id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPedido(int id, [Bind("idPedido, statusPagamento, dataHoraPedido, rm, cep, idCliente")] Pedido pedido)
        {
            if (id != pedido.idPedido)
            {
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (_pedidoRepositorio.Atualizar(pedido))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocorreu um erro ao Editar.");
                    return View(pedido);
                }
            }
            return View(pedido);
        }


        public IActionResult ExcluirPedido(int id)
        {
            _pedidoRepositorio.Excluir(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
