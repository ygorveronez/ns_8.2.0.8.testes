using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class SeparacaoMercadoria : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria>
    {
        #region Construtores

        public SeparacaoMercadoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSeparacaoMercadoria filtrosPesquisa)
        {
            var consultaSeparacaoMercadoria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria>()
                .Where(o =>
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    (o.Carga != null && o.Carga.OcultarNoPatio == false && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                    (o.Carga == null && o.PreCarga != null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                );

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaSeparacaoMercadoria = consultaSeparacaoMercadoria.Where(o => o.Carga.DataCarregamentoCarga.Value.Date >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaSeparacaoMercadoria = consultaSeparacaoMercadoria.Where(o => o.Carga.DataCarregamentoCarga.Value.Date <= filtrosPesquisa.DataLimite.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                consultaSeparacaoMercadoria = consultaSeparacaoMercadoria.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga || o.Carga.CodigosAgrupados.Contains(filtrosPesquisa.NumeroCarga));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaSeparacaoMercadoria = consultaSeparacaoMercadoria.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return consultaSeparacaoMercadoria;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria BuscarPorCodigo(int codigo)
        {
            var consultaSeparacaoMercadoria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria>()
                .Where(o => o.Codigo == codigo);

            return consultaSeparacaoMercadoria.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaSeparacaoMercadoria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaSeparacaoMercadoria.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSeparacaoMercadoria filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSeparacaoMercadoria = Consultar(filtrosPesquisa);

            consultaSeparacaoMercadoria = consultaSeparacaoMercadoria
                .Fetch(o => o.FluxoGestaoPatio)
                .Fetch(o => o.Carga).ThenFetch(o => o.DadosSumarizados)
                .Fetch(o => o.Carga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.Carga).ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.Carga).ThenFetch(o => o.TipoOperacao)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.DadosSumarizados)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.TipoOperacao);

            return ObterLista(consultaSeparacaoMercadoria, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSeparacaoMercadoria filtrosPesquisa)
        {
            var consultaSeparacaoMercadoria = Consultar(filtrosPesquisa);

            return consultaSeparacaoMercadoria.Count();
        }

        #endregion
    }
}
