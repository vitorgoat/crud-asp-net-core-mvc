using ListaDeTarefas.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace ListaDeTarefas.Controllers
{
    public class TarefasController : Controller
    {
        private readonly string _caminhoArquivo;

        public TarefasController()
        {
            _caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "Models", "Dados", "tarefas.json");
        }
        public IActionResult Index(string filtro, string status)
        {
            var tarefas = ObterListaJson();
            if (!string.IsNullOrEmpty(filtro) || !string.IsNullOrEmpty(status))
            {
                tarefas = FiltrarTarefas(tarefas, filtro, status);
            }
            ViewBag.Tarefas = tarefas;
            ViewBag.Filtro = filtro;
            ViewBag.Status = status; 
            return View();
        }

        private List<TarefasModel> FiltrarTarefas(List<TarefasModel> tarefas, string filtro, string status)
        {
      
            if (string.IsNullOrEmpty(filtro) && string.IsNullOrEmpty(status))
            {
                return tarefas;
            }

            var tarefasFiltradas = tarefas.Where(t =>
                (string.IsNullOrEmpty(filtro) || t.Titulo.Contains(filtro) || t.Descricao.Contains(filtro)) &&
                (string.IsNullOrEmpty(status) || t.Status.Equals(status))
            ).ToList();

            return tarefasFiltradas;
        }

        [HttpGet]
        public IActionResult Criar(int id)
        {
            return View();
        }

        private int GerarNovoId(List<TarefasModel> tarefas)
        {
            if (tarefas.Count > 0)
            {
                return tarefas.Max(t => t.Id) + 1;
            }
            else
            {
                return 1;
            }
        }

        [HttpPost]
        public IActionResult Criar(TarefasModel novaTarefa)
        {
            if (ModelState.IsValid)
            {
                var listaTarefas = ObterListaJson();
                novaTarefa.Id = GerarNovoId(listaTarefas);
                listaTarefas.Add(novaTarefa);
                SalvarListaJson(listaTarefas);
                return RedirectToAction("Index");
            }
            return View(novaTarefa);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var listaTarefas = ObterListaJson();
            var tarefa = listaTarefas.Where(w => w.Id == id).FirstOrDefault();
            ViewBag.TarefaEdit = tarefa;
            return View();
        }

        [HttpPost]
        public IActionResult Salvar(TarefasModel tarefaEditada)
        {
            
                var listaTarefas = ObterListaJson();
                var tarefaExistente = listaTarefas.FirstOrDefault(t => t.Id == tarefaEditada.Id);

                if (tarefaExistente != null)
                {
                    tarefaExistente.Titulo = tarefaEditada.Titulo;
                    tarefaExistente.Descricao = tarefaEditada.Descricao;
                    tarefaExistente.Status = tarefaEditada.Status;
                    SalvarListaJson(listaTarefas);

                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
        }


        [HttpGet]
        public IActionResult Apagar(int id)
        {
            var tarefa = ObterTarefaPorId(id);
            if (tarefa == null)
            {
                return NotFound();
            }
            return View(tarefa);
        }

        [HttpGet]
        public IActionResult Visualizar(int id) 
        {
            var tarefa = ObterTarefaPorId(id);
            if (tarefa == null)
            {
                return NotFound();
            }
            return View(tarefa);
        }

        private TarefasModel ObterTarefaPorId(int id)
        {
            var listaTarefas = ObterListaJson();
            return listaTarefas.FirstOrDefault(t => t.Id == id);
        }

        [HttpPost]
        public IActionResult ConfirmarApagar(int id)
        {
            var listaTarefas = ObterListaJson();
            var tarefa = listaTarefas.FirstOrDefault(w => w.Id == id);

            if (tarefa != null)
            {
                listaTarefas.Remove(tarefa);
                SalvarListaJson(listaTarefas);
            }

            return RedirectToAction("Index");
        }

        private List<TarefasModel> ObterListaJson()
        {
            List<TarefasModel> tarefasModels;

            using (StreamReader sr = new StreamReader(_caminhoArquivo))
            {
                var json = sr.ReadToEnd();
                tarefasModels = JsonSerializer.Deserialize<List<TarefasModel>>(json);
            }

            return tarefasModels ?? new List<TarefasModel>();
        }

        private void SalvarListaJson(List<TarefasModel> lista)
        {
            using (StreamWriter sw = new StreamWriter(_caminhoArquivo))
            {
                var json = JsonSerializer.Serialize(lista);
                sw.Write(json);
            }
        }

       
     

    }
}
