using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class ConfiguracaoGeralTipoPagamentoCIOT : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT>
    {
        public ConfiguracaoGeralTipoPagamentoCIOT(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT> BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT>();

            return query.ToList();
        }
    }
}