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

        public IActionResult Index()
        {
            return View(_pratoRepositorio.TodosPratos());
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
            var prato = _pratoRepositorio.ObterPrato(id);

            if (prato == null)
            {
                return NotFound();
            }

            return View(prato);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPrato(int id, [Bind("idPrato, nomePrato, precoPrato, descricaoPrato")] Prato prato)
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
    }
}
