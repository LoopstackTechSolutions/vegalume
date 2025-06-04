using Microsoft.AspNetCore.Mvc;
using vegalume.Models;
using vegalume.Repositorio;

namespace vegalume.Controllers
{
    public class FuncionarioController : Controller
    {
        private readonly FuncionarioRepositorio _funcionarioRepositorio;

        public FuncionarioController(FuncionarioRepositorio funcionarioRepositorio)
        {
            _funcionarioRepositorio = funcionarioRepositorio;
        }

        public IActionResult Index()
        {
            return View(_funcionarioRepositorio.TodosFuncionarios());
        }

        public IActionResult CadastroFuncionario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarFuncionario(Funcionario funcionario)
        {
            _funcionarioRepositorio.Cadastrar(funcionario);
            string email = _funcionarioRepositorio.ObterFuncionarioPeloRm(HttpContext.Session.GetInt32("WorkerId")).email;
            return RedirectToAction("HomeFuncionario", "Home", new { email });
        }

        public IActionResult ObterFuncionarioPeloRm(int rm)
        {
            return Json(_funcionarioRepositorio.ObterFuncionarioPeloRm(rm));
        }

        public IActionResult EditarFuncionario(string email)
        {
            var funcionario = _funcionarioRepositorio.ObterFuncionarioPeloEmail(email);

            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarFuncionario(int id, [Bind("rm, nome, senha, telefone, email")] Funcionario funcionario)
        {
            if (id != funcionario.rm)
            {
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (_funcionarioRepositorio.Atualizar(funcionario))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocorreu um erro ao Editar.");
                    return View(funcionario);
                }
            }
            return View(funcionario);
        }

        public IActionResult ExcluirFuncionario(int id)
        {
            _funcionarioRepositorio.Excluir(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
