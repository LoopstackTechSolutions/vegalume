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

        [HttpPost]
        public IActionResult FazerPedido(decimal valorTotal, int idEndereco, string formaPagamento, int? idCartao = null)
        {
            System.Diagnostics.Debug.WriteLine(valorTotal + " " + idEndereco + " " + formaPagamento + " " + idCartao);
            int ? idCliente = HttpContext.Session.GetInt32("UserId");
            int idPedido = _pedidoRepositorio.FazerPedido(idCliente, valorTotal, idEndereco, formaPagamento, idCartao);

            var carrinho = HttpContext.Session.GetObject<List<PratoCarrinho>>("Carrinho")!;

            foreach (var prato in carrinho)
            {
                _pedidoRepositorio.AdicionarPratoAoPedido(idPedido, prato.Id, prato.Qtd, prato.Anotacoes ?? "");
            }

            HttpContext.Session.Remove("Carrinho");

            return Json(new { idPedido });
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

        public IActionResult TodosPedidosPorCliente(int idCliente)
        {
            return Json(_pedidoRepositorio.TodosPedidosPorCliente(idCliente));
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

        public IActionResult AcompanharPedido(int idPedido)
        {
            return View(_pedidoRepositorio.ObterPedidoPeloId(idPedido));
        }
    }
}
