using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete.ImportacaoTabelaFrete
{
    public class ImportacaoTabelaFreteLayout : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout>
    {
        public ImportacaoTabelaFreteLayout(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout> BuscarLayoutsPorTabelaFrete(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            query = query.Where(o => o.Codigo == codigoTabelaFrete);

            return query.SelectMany(o => o.Layouts).ToList();
        }

    }
}
