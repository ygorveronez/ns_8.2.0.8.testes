using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using dominio = Dominio.Entidades.Embarcador.Veiculos;

namespace Repositorio.Embarcador.Veiculos
{
    public class TabelaMediaPorSegmento:RepositorioBase<dominio.TabelaMediaPorSegmento>
    {
        public TabelaMediaPorSegmento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<dominio.TabelaMediaPorSegmento> Consultar(int codigoModelo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = SessionNHiBernate.Query<dominio.TabelaMediaPorSegmento>();

            if (codigoModelo > 0)
                query = query.Where(obj => obj.Segmento.Codigo == codigoModelo);

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

    }
}
