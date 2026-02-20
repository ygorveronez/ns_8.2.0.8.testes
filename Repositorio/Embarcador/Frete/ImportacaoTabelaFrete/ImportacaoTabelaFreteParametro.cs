using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete.ImportacaoTabelaFrete
{
    public class ImportacaoTabelaFreteParametro : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro>
    {
        public ImportacaoTabelaFreteParametro(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro> BuscarPorImportacaoTabelaFrete(int codigoImportacaoTabelaFrete)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro>();

            query = query.Where(o => o.ImportacaoTabelaFrete.Codigo == codigoImportacaoTabelaFrete);

            return query.ToList();
        }


        public void DeletarPorImportacao(int codigoImportacaoTabelaFrete)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE FROM ImportacaoTabelaFreteParametro  WHERE ImportacaoTabelaFrete = :codigo")
                    .SetInt32("codigo", codigoImportacaoTabelaFrete)
                    .ExecuteUpdate();
        }

    }
}
