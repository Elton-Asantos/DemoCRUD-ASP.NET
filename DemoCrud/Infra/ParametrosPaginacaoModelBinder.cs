using DemoCrud.ViewModels; // Importa a classe ParametrosPaginacao usada para paginação de dados
using System; // Funcionalidades básicas do .NET, como tipos primitivos e operações básicas
using System.Collections.Generic; // Coleções genéricas, como List<T> e Dictionary<TKey,TValue>
using System.Linq; // Funcionalidades para manipulação de consultas e coleções com LINQ
using System.Web; // Classes e interfaces para trabalhar com requisições e respostas HTTP
using System.Web.Mvc; // Componentes do ASP.NET MVC, incluindo controladores e vinculação de modelos

namespace DemoCrud.Infra
{
    public class ParametrosPaginacaoModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            HttpRequestBase request = controllerContext.HttpContext.Request; // Obtém a requisição HTTP atual

            ParametrosPaginacao parametrosPaginacao = new ParametrosPaginacao(request.Form); // Cria um objeto de paginação usando os dados do formulário

            return parametrosPaginacao; // Retorna o objeto de paginação para ser usado no controlador
        }
    }
}
