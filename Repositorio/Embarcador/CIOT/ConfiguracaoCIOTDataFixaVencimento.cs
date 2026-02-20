using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class ConfiguracaoCIOTDataFixaVencimento : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento>
    {
        public ConfiguracaoCIOTDataFixaVencimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento> BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.ToList();
        }
    }
}
