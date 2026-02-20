using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class LogTabelaComissaoProduto: Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto>
    {
         public LogTabelaComissaoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }


         public List<Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto> Consultar(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
         {

             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto>();

             var result = from obj in query select obj;

             return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

         }

         public int ContarConsulta()
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.LogTabelaComissaoProduto>();

             var result = from obj in query select obj;

             return result.Count();
         } 

    }
}
