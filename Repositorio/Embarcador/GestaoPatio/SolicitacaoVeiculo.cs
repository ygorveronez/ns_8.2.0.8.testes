using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class SolicitacaoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo>
    {
        #region Construtores

        public SolicitacaoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSolicitacaoVeiculo filtrosPesquisa)
        {
            var consultaSolicitacaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo>()
                .Where(o =>
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    (o.Carga != null && o.Carga.OcultarNoPatio == false && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                    (o.Carga == null && o.PreCarga != null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                );

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaSolicitacaoVeiculo = consultaSolicitacaoVeiculo.Where(o => o.Carga.DataCarregamentoCarga.Value.Date >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaSolicitacaoVeiculo = consultaSolicitacaoVeiculo.Where(o => o.Carga.DataCarregamentoCarga.Value.Date <= filtrosPesquisa.DataLimite.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                consultaSolicitacaoVeiculo = consultaSolicitacaoVeiculo.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga || o.Carga.CodigosAgrupados.Contains(filtrosPesquisa.NumeroCarga));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaSolicitacaoVeiculo = consultaSolicitacaoVeiculo.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return consultaSolicitacaoVeiculo;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo BuscarPorCodigo(int codigo)
        {
            var consultaSolicitacaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo>()
                .Where(o => o.Codigo == codigo);

            return consultaSolicitacaoVeiculo.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaSolicitacaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaSolicitacaoVeiculo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSolicitacaoVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSolicitacaoVeiculo = Consultar(filtrosPesquisa);

            consultaSolicitacaoVeiculo = consultaSolicitacaoVeiculo
                .Fetch(o => o.FluxoGestaoPatio)
                .Fetch(o => o.Carga).ThenFetch(o => o.DadosSumarizados)
                .Fetch(o => o.Carga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.Carga).ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.Carga).ThenFetch(o => o.TipoOperacao)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.DadosSumarizados)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.TipoOperacao);

            return ObterLista(consultaSolicitacaoVeiculo, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSolicitacaoVeiculo filtrosPesquisa)
        {
            var consultaSolicitacaoVeiculo = Consultar(filtrosPesquisa);

            return consultaSolicitacaoVeiculo.Count();
        }

        #endregion
    }
}
