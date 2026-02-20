using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTConfiguracaoFinanceira : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira>
    {
        public CIOTConfiguracaoFinanceira(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira> BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira BuscarPorConfiguracaoCIOTETipoPagamento(int codigoConfiguracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT tipoPagamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao && obj.TipoPagamento == tipoPagamento);

            return query.FirstOrDefault();
        }
    }
}
