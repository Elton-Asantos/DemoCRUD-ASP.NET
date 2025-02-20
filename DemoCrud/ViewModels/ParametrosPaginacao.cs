using System; // Fornece funcionalidades básicas, como tipos primitivos e operações de formatação
using System.Collections.Generic; // Oferece suporte a coleções genéricas, como List<T>
using System.Collections.Specialized; // Contém NameValueCollection, usado para armazenar pares chave/valor
using System.Linq; // Permite o uso de LINQ para consultas e manipulação de coleções
using System.Web; // Fornece classes para lidar com requisições HTTP e outros serviços web

namespace DemoCrud.ViewModels
{
    public class ParametrosPaginacao
    {
        public ParametrosPaginacao(NameValueCollection dados)
        {
            // Verifica se existe uma chave de ordenação e define um valor padrão se não houver
            string chave = dados.AllKeys.FirstOrDefault(k => k.StartsWith("sort")) ?? "Titulo";
            string ordenacao = dados[chave] ?? "asc"; // Define a ordenação padrão como ascendente
            string campo = chave.Replace("sort[", String.Empty).Replace("]", String.Empty); // Extrai o nome do campo

            // Formata o campo ordenado no padrão "Campo Ordenacao" (ex.: "Titulo asc")
            CampoOrdenado = String.Format("{0} {1}", campo, ordenacao);
            Current = int.Parse(dados["current"]); // Página atual
            RowCount = int.Parse(dados["rowCount"]); // Número de registros por página
            SearchPhrase = dados["searchPhrase"]; // Frase de pesquisa para filtro
        }

        public int Current { get; set; } // Página atual
        public int RowCount { get; set; } // Quantidade de registros por página
        public string SearchPhrase { get; set; } // Frase de pesquisa para filtragem
        public string CampoOrdenado { get; set; } // Campo e ordem de ordenação (ex.: "Titulo asc")
    }
}
