using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.SuperApp
{
    public class IntegracaoSuperApp : RepositorioBase<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp>
    {
        #region Construtores
        public IntegracaoSuperApp(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public IntegracaoSuperApp(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        #endregion

        #region Métodos Públicos
        public List<int> BuscarIntegracoesPendentes(List<TipoEventoApp> tiposEventos, int maximoRegistos, int? codigoIntegracao = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp>().Where(i => (i.SituacaoProcessamento == SituacaoProcessamentoIntegracao.AguardandoProcessamento ||
                           (i.SituacaoProcessamento == SituacaoProcessamentoIntegracao.ErroProcessamento && i.NumeroTentativas < 3) ||
                           (i.Codigo == (codigoIntegracao ?? 0))) && tiposEventos.Contains(i.TipoEvento));
            return query.Take(maximoRegistos).Select(x => x.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp> BuscarPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp>();
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp> result = from obj in query select obj;
            result = result.Where(obj => obj.Carga.Codigo == carga && obj.CargaEntrega != null);
            return result.ToList();
        }


        public Task<List<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp>> ConsultarAsync(FiltroPesquisaConsultaMonitorIntegracoesSuperApp filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp> consultaIntegracoesSuperApp = Consultar(filtroPesquisa);
            consultaIntegracoesSuperApp = consultaIntegracoesSuperApp
                .Fetch(integracao => integracao.Carga)
                .Fetch(integracao => integracao.CargaEntrega).ThenFetch(entrega => entrega.Cliente);

            return ObterListaAsync(consultaIntegracoesSuperApp, parametrosConsulta);
        }

        public Task<int> ContarConsultaAsync(FiltroPesquisaConsultaMonitorIntegracoesSuperApp filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp> consultaIntegracoesSuperApp = Consultar(filtroPesquisa);

            return consultaIntegracoesSuperApp.CountAsync(CancellationToken);
        }
        #endregion

        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp> Consultar(FiltroPesquisaConsultaMonitorIntegracoesSuperApp filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp> consultaIntegracoesSuperApp = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp>();

            if (filtroPesquisa.DataInicioRecebimento.HasValue)
                consultaIntegracoesSuperApp = consultaIntegracoesSuperApp
                    .Where(integracao => integracao.DataRecebimento >= filtroPesquisa.DataInicioRecebimento.Value);

            if (filtroPesquisa.DataFimRecebimento.HasValue)
                consultaIntegracoesSuperApp = consultaIntegracoesSuperApp
                    .Where(integracao => integracao.DataRecebimento <= filtroPesquisa.DataFimRecebimento.Value);

            if (filtroPesquisa.SituacaoIntegracao != null)
                consultaIntegracoesSuperApp = consultaIntegracoesSuperApp.Where(integracao => integracao.SituacaoProcessamento == filtroPesquisa.SituacaoIntegracao);

            if (filtroPesquisa.CodigoCargaEmbarcador > 0)
                consultaIntegracoesSuperApp = consultaIntegracoesSuperApp.Where(integracao => integracao.Carga.Codigo == filtroPesquisa.CodigoCargaEmbarcador);

            if (filtroPesquisa.TipoEventoApp != null)
                consultaIntegracoesSuperApp = consultaIntegracoesSuperApp.Where(integracao => integracao.TipoEvento == filtroPesquisa.TipoEventoApp);

            if (filtroPesquisa.CodigoTransportador > 0)
                consultaIntegracoesSuperApp = consultaIntegracoesSuperApp.Where(integracao => integracao.Carga.Empresa.Codigo == filtroPesquisa.CodigoTransportador);

            return consultaIntegracoesSuperApp;
        }
        #endregion
    }



}
