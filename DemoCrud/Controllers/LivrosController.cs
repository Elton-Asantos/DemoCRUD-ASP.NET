using DemoCrud.AcessoDados;
using DemoCrud.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic; // Permite consultas dinâmicas usando strings
using System.Net;
using System.Web.Mvc;

namespace DemoCrud.Models
{
    // Controlador responsável pelas operações CRUD dos livros
    public class LivrosController : Controller
    {
        // Conexão com o banco de dados usando Entity Framework
        private LivrosContexto db = new LivrosContexto();

        // Exibe a página inicial (Index) da lista de livros
        public ActionResult Index()
        {
            return View();
        }

        // Retorna os dados em formato JSON com paginação e filtragem
        public JsonResult Listar(ParametrosPaginacao parametrosPaginacao)
        {
            // Chama o método que filtra e pagina os dados com base nos parâmetros recebidos
            DadosFiltrados dadosFiltrados = FiltrarEPaginar(parametrosPaginacao);
            return Json(dadosFiltrados, JsonRequestBehavior.AllowGet);
        }

        // Método para filtrar e paginar os livros com base nos parâmetros recebidos
        private DadosFiltrados FiltrarEPaginar(ParametrosPaginacao parametrosPaginacao)
        {
            // Recupera todos os livros e inclui os dados relacionados (Genero)
            var livros = db.Livros.Include(l => l.Genero);
            int total = livros.Count(); // Conta o total de registros antes da filtragem

            // Filtra se houver um termo de pesquisa
            if (!String.IsNullOrWhiteSpace(parametrosPaginacao.SearchPhrase))
            {
                int ano = 0;
                int.TryParse(parametrosPaginacao.SearchPhrase, out ano); // Tenta converter a pesquisa para ano

                decimal valor = 0.0m;
                decimal.TryParse(parametrosPaginacao.SearchPhrase, out valor); // Tenta converter a pesquisa para valor

                // **Linha complexa:** Usa Linq.Dynamic para filtrar com base em vários campos dinamicamente
                livros = livros.Where("Titulo.Contains(@0) OR Autor.Contains(@0) OR AnoEdicao == @1 OR Valor = @2",
                    parametrosPaginacao.SearchPhrase, ano, valor);
            }

            // **Linha complexa:** Aplica ordenação dinâmica, paginação e inclui novamente o gênero para o resultado
            var livrosPaginados = livros.Include(l => l.Genero)
                                        .OrderBy(parametrosPaginacao.CampoOrdenado) // Ordenação dinâmica
                                        .Skip((parametrosPaginacao.Current - 1) * parametrosPaginacao.RowCount) // Ignora os registros já exibidos
                                        .Take(parametrosPaginacao.RowCount); // Pega o número de registros para a página atual

            // Monta e retorna o objeto com os dados filtrados e paginados
            DadosFiltrados dadosFiltrados = new DadosFiltrados(parametrosPaginacao)
            {
                rows = livrosPaginados.ToList(),
                current = parametrosPaginacao.Current,
                rowCount = parametrosPaginacao.RowCount,
                total = total
            };
            return dadosFiltrados;
        }

        // Exibe os detalhes de um livro específico
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Retorna erro 400 se o ID for nulo

            Livro livro = db.Livros.Find(id); // Busca o livro pelo ID

            if (livro == null)
                return HttpNotFound(); // Retorna erro 404 se o livro não for encontrado

            return PartialView(livro); // Exibe os detalhes do livro
        }

        // Exibe o formulário de criação de um novo livro
        public ActionResult Create()
        {
            // **Linha importante:** Cria uma lista de gêneros para o campo de seleção no formulário
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome");
            return PartialView();
        }

        // Processa o envio do formulário de criação de um novo livro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "Id,Titulo,AnoEdicao,Valor,Autor,GeneroId")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                db.Livros.Add(livro); // Adiciona o livro ao banco
                db.SaveChanges();    // Salva alterações
                return Json(new { resultado = true, mensagem = "Livro cadastrado com sucesso" });
            }
            else
            {
                // **Linha complexa:** Captura os erros de validação para exibir ao usuário
                IEnumerable<ModelError> erros = ModelState.Values.SelectMany(item => item.Errors);
                return Json(new { resultado = false, mensagem = erros });
            }
        }

        // Exibe o formulário para editar um livro existente
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Livro livro = db.Livros.Find(id);

            if (livro == null)
                return HttpNotFound();

            // **Linha importante:** Preenche a lista de gêneros selecionando o gênero atual do livro
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return PartialView(livro);
        }

        // Processa a edição do livro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "Id,Titulo,AnoEdicao,Valor,Autor,GeneroId")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                // **Linha complexa:** Marca o estado da entidade como modificada para o Entity Framework
                db.Entry(livro).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { resultado = true, mensagem = "Livro editado com sucesso" });
            }
            else
            {
                IEnumerable<ModelError> erros = ModelState.Values.SelectMany(item => item.Errors);
                return Json(new { resultado = false, mensagem = erros });
            }
        }

        // Exibe o formulário de confirmação para deletar um livro
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Livro livro = db.Livros.Find(id);

            if (livro == null)
                return HttpNotFound();

            return PartialView(livro);
        }

        // Confirma e processa a exclusão de um livro
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Livro livro = db.Livros.Find(id);

                if (livro == null)
                    return Json(new { resultado = false, mensagem = "Livro não encontrado." });

                db.Livros.Remove(livro); // Remove o livro do banco
                db.SaveChanges();       // Salva alterações

                return Json(new { resultado = true, mensagem = "Livro excluído com sucesso" });
            }
            catch (Exception ex)
            {
                // **Linha complexa:** Captura exceções e retorna a mensagem de erro
                return Json(new { resultado = false, mensagem = ex.Message });
            }
        }

        // Libera os recursos utilizados pelo banco de dados
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose(); // Fecha a conexão com o banco

            base.Dispose(disposing);
        }
    }
}
