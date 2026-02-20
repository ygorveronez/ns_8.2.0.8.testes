using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class FluxoPatioIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao>
    {
        #region Construtores

        public FluxoPatioIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao> BuscarIntegracaoesPententes()
        {
            var consultaFluxoPatioIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao>()
                .Where(o => o.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao || (o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && o.NumeroTentativas < 3));

            return consultaFluxoPatioIntegracao.Take(50).ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var consultaFluxoPatioIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(arquivo => arquivo.Codigo == codigoArquivo));

            return consultaFluxoPatioIntegracao.FirstOrDefault();
        }

        public string BuscarPreProtocoloPorCarga(int codigoCarga)
        {
            var consultaFluxoPatioIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao>()
                .Where(o => o.Carga.Codigo == codigoCarga && (o.PreProtocolo != string.Empty || o.PreProtocolo != null));

            return consultaFluxoPatioIntegracao.Select(o => o.PreProtocolo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoPatioIntegracao filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaFluxoPatioIntegracao = Consultar(filtroPesquisa);

            return ObterLista(consultaFluxoPatioIntegracao, parametroConsulta);
        }

        public int ContarConsultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoPatioIntegracao filtroPesquisa)
        {
            var consultaFluxoPatioIntegracao = Consultar(filtroPesquisa);

            return consultaFluxoPatioIntegracao.Count();
        }

        public bool ExistePorEtapaETipoIntegracao(int codigoCarga, EtapaFluxoGestaoPatio etapaFluxoPatio, int codigoTipoIntegracao)
        {
            var consultaFluxoPatioIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.EtapaFluxoGestaoPatio == etapaFluxoPatio && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return consultaFluxoPatioIntegracao.Any();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoPatioIntegracao filtroPesquisa)
        {
            var consultaFluxoPatioIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao>();

            if (!string.IsNullOrEmpty(filtroPesquisa.NumeroCarga))
                consultaFluxoPatioIntegracao = consultaFluxoPatioIntegracao.Where(fluxo => fluxo.Carga.CodigoCargaEmbarcador == filtroPesquisa.NumeroCarga);

            if (filtroPesquisa.CodigoIntegradora > 0)
                consultaFluxoPatioIntegracao = consultaFluxoPatioIntegracao.Where(fluxo => fluxo.TipoIntegracao.Codigo == filtroPesquisa.CodigoIntegradora);

            if (filtroPesquisa.SituacaoIntegracao.HasValue)
                consultaFluxoPatioIntegracao = consultaFluxoPatioIntegracao.Where(fluxo => fluxo.SituacaoIntegracao == filtroPesquisa.SituacaoIntegracao.Value);

            if (filtroPesquisa.Data != null && filtroPesquisa.Data != DateTime.MinValue)
                consultaFluxoPatioIntegracao = consultaFluxoPatioIntegracao.Where(fluxo => fluxo.DataIntegracao == filtroPesquisa.Data);

            if (filtroPesquisa.EtapaFluxo == EtapaFluxoGestaoPatio.Todas)
                return consultaFluxoPatioIntegracao;
            
            consultaFluxoPatioIntegracao = consultaFluxoPatioIntegracao.Where(fluxo => fluxo.EtapaFluxoGestaoPatio == filtroPesquisa.EtapaFluxo);

            return consultaFluxoPatioIntegracao;
        }

        #endregion Métodos Privados
    }
}