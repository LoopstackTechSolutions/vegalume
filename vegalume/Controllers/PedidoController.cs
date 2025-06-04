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

        public IActionResult ListarPedido()
        {
            return View();
        }

        public IActionResult FiltrarPedidos(string filtro)
        {
            return Json(_pedidoRepositorio.FiltrarPedidos(filtro));
        }

        public IActionResult TodosPedidosPorStatus(string status)
        {
            return Json(_pedidoRepositorio.TodosPedidosPorStatus(status));
        }

        [HttpPost]
        public IActionResult CancelarPedido(int idPedido, int rm)
        {
            _pedidoRepositorio.CancelarPedido(idPedido, rm);
            return Ok();
        }

        [HttpPost]
        public IActionResult AvancarPedido(int idPedido, string statusAtual, int rm)
        {
            _pedidoRepositorio.AvancarPedido(idPedido, statusAtual, rm);
            return Ok();
        }
    }
}
