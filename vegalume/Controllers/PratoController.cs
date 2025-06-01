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
            var carrinho = HttpContext.Session.GetObject<List<PratoCarrinho>>("Carrinho") ?? new List<PratoCarrinho>();

            var existingItem = carrinho.FirstOrDefault(c => c.Id == id);

            if (existingItem != null)
            {
                existingItem.Qtd += qtd;
                existingItem.Anotacoes = anotacoes;
            }
            else
            {
                carrinho.Add(new PratoCarrinho
                {
                    Id = id,
                    Qtd = qtd,
                    Anotacoes = anotacoes
                });
            }

            HttpContext.Session.SetObject("Carrinho", carrinho);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult CadastrarPrato()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarPrato(Prato prato)
        {

            _pratoRepositorio.Cadastrar(prato);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditarPrato(int id)
        {
            var prato = _pratoRepositorio.ObterPratoPeloId(id);

            if (prato == null)
            {
                return NotFound();
            }

            return View(prato);
        }

        [HttpPost]
        public IActionResult EditarPrato(int id, [Bind("idPrato, nomePrato, precoPrato, descricaoPrato, valorCalorico, peso, pessoasServidas")] Prato prato)
        {
            if (id != prato.idPrato)
            {
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (_pratoRepositorio.Atualizar(prato))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocorreu um erro ao Editar.");
                    return View(prato);
                }
            }
            return View(prato);
        }

        public IActionResult ExcluirPrato(int id)
        {
            _pratoRepositorio.Excluir(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult TodosPratos()
        {
            return Json(_pratoRepositorio.TodosPratos());
        }

        [HttpGet]
        public IActionResult DetalhesPrato(int idPrato)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            Prato prato = _pratoRepositorio.ObterPratoPeloId(idPrato);
            return View(prato);
        }
    }
}
