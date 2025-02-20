using DemoCrud.AcessoDados;
using DemoCrud.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;

namespace DemoCrud.Models
{
    public class LivrosController : Controller
    {
        private LivrosContexto db = new LivrosContexto();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Listar(ParametrosPaginacao parametrosPaginacao)
        {
            DadosFiltrados dadosFiltrados = FiltrarEPaginar(parametrosPaginacao);

            return Json(dadosFiltrados, JsonRequestBehavior.AllowGet);
        }

        private DadosFiltrados FiltrarEPaginar(ParametrosPaginacao parametrosPaginacao)
        {
            var livros = db.Livros.Include(l => l.Genero);
            int total = livros.Count();

            if (!String.IsNullOrWhiteSpace(parametrosPaginacao.SearchPhrase))
            {
                int ano = 0;
                int.TryParse(parametrosPaginacao.SearchPhrase, out ano);

                decimal valor = 0.0m;
                decimal.TryParse(parametrosPaginacao.SearchPhrase, out valor);

                livros = livros.Where("Titulo.Contains(@0) OR Autor.Contains(@0) OR AnoEdicao == @1 OR Valor = @2", parametrosPaginacao.SearchPhrase, ano, valor);
            }



            var livrosPaginados = livros.Include(l => l.Genero).OrderBy(parametrosPaginacao.CampoOrdenado).Skip((parametrosPaginacao.Current - 1) * parametrosPaginacao.RowCount).Take(parametrosPaginacao.RowCount);

            DadosFiltrados dadosFiltrados = new DadosFiltrados(parametrosPaginacao)
            {
                rows = livrosPaginados.ToList(),
                current = parametrosPaginacao.Current,
                rowCount = parametrosPaginacao.RowCount,
                total = total
            };
            return dadosFiltrados;
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Livro livro = db.Livros.Find(id);

            if (livro == null)
                return HttpNotFound();

            return PartialView(livro);
        }

        public ActionResult Create()
        {
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome");
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "Id,Titulo,AnoEdicao,Valor,Autor,GeneroId")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                db.Livros.Add(livro);
                db.SaveChanges();

                return Json(new { resultado = true, mensagem = "Livro cadastrado com sucesso" });
            }
            else
            {
                IEnumerable<ModelError> erros = ModelState.Values.SelectMany(item => item.Errors);
                return Json(new { resultado = false, mensagem = erros });
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Livro livro = db.Livros.Find(id);

            if (livro == null)
                return HttpNotFound();

            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return PartialView(livro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "Id,Titulo,AnoEdicao,Valor,Autor,GeneroId")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(livro).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { resultado = true, mensagem = "Livro editado com sucesso" });
            }
            else
            {
                IEnumerable<ModelError> erros = ModelState.Values.SelectMany(item => item.Errors);
                return Json(new { resultado = false, mensagem = erros });
            }
        } // Correção 2: Fechamento do método Edit (POST)

        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Livro livro = db.Livros.Find(id);

            if (livro == null)
                return HttpNotFound();

            return PartialView(livro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Livro livro = db.Livros.Find(id);

                // Correção 3: Verificação de nulo
                if (livro == null)
                    return Json(new { resultado = false, mensagem = "Livro não encontrado." });

                db.Livros.Remove(livro);
                db.SaveChanges();

                return Json(new { resultado = true, mensagem = "Livro excluído com sucesso" });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensagem = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}

