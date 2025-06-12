using Microsoft.AspNetCore.Mvc;
using vegalume.Models;
using vegalume.Repositorio;

namespace vegalume.Controllers
{
    public class PratoController : Controller
    {
        private readonly PratoRepositorio _pratoRepositorio;

        public PratoController(PratoRepositorio pratoRepositorio)
        {
            _pratoRepositorio = pratoRepositorio;
        }

        public IActionResult AdicionarAoCarrinho(int id, int qtd, string? anotacoes)
        {
            var carrinho = HttpContext.Session.GetObject<List<PratoCarrinho>>("Carrinho");

            var existingItem = carrinho!.FirstOrDefault(c => c.Id == id);

            if (existingItem != null)
            {
                existingItem.Qtd += qtd;
                existingItem.Anotacoes = anotacoes;
            }
            else
            {
                carrinho!.Add(new PratoCarrinho
                {
                    Id = id,
                    Qtd = qtd,
                    Anotacoes = anotacoes
                });
            }

            HttpContext.Session.SetObject("Carrinho", carrinho);
            return Redirect(Url.Action("Index", "Home") + "#nosso-cardapio");
        }

        [HttpPost]
        public IActionResult CadastrarPrato(Prato prato)
        {
            _pratoRepositorio.Cadastrar(prato);
            return RedirectToAction("EditarCardapio", "Prato");
        }

        public IActionResult EditarPrato(int idPrato)
        {
            var prato = _pratoRepositorio.ObterPratoPeloId(idPrato);
            return View(prato);
        }

        [HttpGet]
        public IActionResult TodosPratos()
        {
            return Json(_pratoRepositorio.TodosPratos());
        }

        [HttpGet]
        public IActionResult TodosPratosPorStatus(short status)
        {
            return Json(_pratoRepositorio.TodosPratosPorStatus(status));
        }

        [HttpGet]
        public IActionResult TodosPratosPorPedido(int idPedido)
        {
            return Json(_pratoRepositorio.TodosPratosPorPedido(idPedido));
        }

        [HttpGet]
        public IActionResult DetalhesPrato(int idPrato)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            Prato prato = _pratoRepositorio.ObterPratoPeloId(idPrato);
            return View(prato);
        }

        public IActionResult EditarCardapio()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TrocarStatus(int idPrato)
        {
            _pratoRepositorio.TrocarStatus(idPrato);
            return Ok();
        }

        [HttpPost]
        public IActionResult AlterarPrato(Prato prato)
        {
            System.Diagnostics.Debug.WriteLine(prato.idPrato);
            _pratoRepositorio.Editar(prato);
            return RedirectToAction("EditarCardapio", "Prato");
        }

        public IActionResult ObterCarrinho()
        {
            return Json(HttpContext.Session.GetObject<List<PratoCarrinho>>("Carrinho"));
        }

        public IActionResult ObterPratoPeloId(int idPrato)
        {
            return Json(_pratoRepositorio.ObterPratoPeloId(idPrato));
        }
    }
}
