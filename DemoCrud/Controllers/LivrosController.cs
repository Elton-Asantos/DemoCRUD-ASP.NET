using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemoCrud.AcessoDados;
using DemoCrud.Models;

// Define o namespace do controlador
namespace DemoCrud.Models
{
    public class LivrosController : Controller
    {
        // Instância do contexto do banco de dados para interagir com a tabela "Livros"
        private LivrosContexto db = new LivrosContexto();

        // GET: Livros
        public ActionResult Index()
        {
            // Retorna a view da página inicial dos livros
            return View();
        }


        public JsonResult Listar(string searchPhrase, int current = 1, int rowCount = 5)
        {
            string chave = Request.Form.AllKeys.Where(k => k.StartsWith("sort")).First();
            string ordenacao = Request[chave];
            string campo = chave.Replace("sort[", String.Empty).Replace("]", String.Empty);

            // Obtém todos os livros com seus respectivos gêneros

            var livros = db.Livros.Include(l => l.Genero);

            int total = livros.Count();

            if (!String.IsNullOrWhiteSpace(searchPhrase))
            {
                int ano = 0;
                int.TryParse(searchPhrase, out ano);

                decimal valor = 0.0m;
                decimal.TryParse(searchPhrase, out valor);

                livros = livros.Where("Titulo.Contains(@0) OR Autor.Contains(@0) OR AnoEdicao == @1 OR Valor = @2", searchPhrase, ano, valor);
            }

           
            string campoOrdenacao = String.Format("{0} {1}", campo, ordenacao);


            // Aplica ordenação por título e faz a paginação
            var livrosPaginados = livros.OrderBy(campoOrdenacao)
            .Skip((current- 1) * rowCount) // Pula os primeiros registros conforme a página
            .Take(rowCount); // Seleciona a quantidade de registros desejada


            return Json(new {
                rows = livrosPaginados.ToList(),
                current = current,
                rowCount = rowCount,
                total = total


            }
            , JsonRequestBehavior.AllowGet);
        }


        // GET: Detalhes do livro (exibe informações de um livro específico)
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                // Retorna um erro HTTP 400 (Bad Request) caso o ID não seja fornecido
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca o livro pelo ID fornecido
            Livro livro = db.Livros.Find(id);

            if (livro == null)
            {
                // Retorna erro HTTP 404 (Not Found) caso o livro não seja encontrado
                return HttpNotFound();
            }

            // Retorna a view com os detalhes do livro
            return PartialView(livro);
        }

        // GET: Criar um novo livro
        public ActionResult Create()
        {
            // Preenche a ViewBag com a lista de gêneros para o dropdown na view
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome");
            return PartialView();
        }

        // POST: Criar um novo livro (processa os dados do formulário)
        [HttpPost]
        [ValidateAntiForgeryToken] // Proteção contra ataques CSRF
        public ActionResult Create([Bind(Include = "Id,Titulo,AnoEdicao,Valor,Autor,GeneroId")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                // Adiciona o livro ao banco de dados e salva as alterações
                db.Livros.Add(livro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Caso ocorra um erro, recarrega a lista de gêneros e retorna à view
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return View(livro);
        }

        // GET: Editar um livro existente
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca o livro pelo ID fornecido
            Livro livro = db.Livros.Find(id);

            if (livro == null)
            {
                return HttpNotFound();
            }

            // Preenche a ViewBag com a lista de gêneros
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return PartialView(livro);
        }

        // POST: Editar um livro existente (processa os dados do formulário)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Titulo,AnoEdicao,Valor,Autor,GeneroId")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                // Atualiza o estado do livro no banco e salva as alterações
                db.Entry(livro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Caso ocorra um erro, recarrega a lista de gêneros e retorna à view
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return View(livro);
        }

        // GET: Excluir um livro
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca o livro pelo ID fornecido
            Livro livro = db.Livros.Find(id);

            if (livro == null)
            {
                return HttpNotFound();
            }

            // Retorna a view com o livro a ser excluído
            return PartialView(livro);
        }

        // POST: Confirmação da exclusão de um livro
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Busca o livro pelo ID e remove do banco de dados
            Livro livro = db.Livros.Find(id);
            db.Livros.Remove(livro);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Libera os recursos do contexto do banco de dados
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
