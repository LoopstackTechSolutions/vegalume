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

        public IActionResult ObterPedidoPeloId(int idPedido)
        {
            return Json(_pedidoRepositorio.ObterPedidoPeloId(idPedido));
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
        public IActionResult CancelarPedido(int idPedido, int? rm)
        {
            _pedidoRepositorio.CancelarPedido(idPedido, rm);
            return Ok();
        }

        [HttpPost]
        public IActionResult AvancarPedido(int idPedido, string statusAtual, int rm)
        {
            string newStatus = _pedidoRepositorio.AvancarPedido(idPedido, statusAtual, rm);
            return Json(new { status = newStatus });
        }

        public IActionResult AcompanharPedido(int idPedido)
        {
            Pedido pedido = _pedidoRepositorio.ObterPedidoPeloId(idPedido)!;
            System.Diagnostics.Debug.WriteLine("status:" + pedido.idEndereco);
            return View(pedido);
        }

        public IActionResult ObterEnderecoPeloId(int idEndereco)
        {
            return Json(_pedidoRepositorio.ObterEnderecoPeloId(idEndereco));
        }

        public IActionResult ObterCartaoPeloId(int idCartao)
        {
            return Json(_pedidoRepositorio.ObterCartaoPeloId(idCartao));
        }
    }
}
