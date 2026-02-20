using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoGeralCarga : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga>
    {
        public ConfiguracaoGeralCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoGeralCarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public bool ExisteConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga>();

            return query.Select(o => o.ExigirConfirmacaoEtapaFreteNoFluxoNotaAposFrete).FirstOrDefault();
        }

        public bool ExisteConsiderarFilialDaTransportadoraParaCompraDoValePedagioQuandoForEFrete()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga>();

            return query.Select(o => (bool?)o.ConsiderarFilialDaTransportadoraParaCompraDoValePedagioQuandoForEFrete).FirstOrDefault() ?? false;
        }

        public bool ExisteCancelarValePedagioQuandoGerarCargaTransbordo()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga>();

            return query.Select(o => (bool?)o.CancelarValePedagioQuandoGerarCargaTransbordo).FirstOrDefault() ?? false;
        }

        public bool ExisteModeloVeicularVeiculoCargaEtapaFrete()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga>();

            return query.Select(x => (bool?)x.ValidarModeloVeicularVeiculoCargaEtapaFrete).FirstOrDefault() ?? false;
        }

        public bool ExistePermiteHabilitarContingenciaEPECAutomaticamente()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga>();

            return query.Select(x => (bool?)x.PermiteHabilitarContingenciaEPECAutomaticamente).FirstOrDefault() ?? false;
        }
    }
}