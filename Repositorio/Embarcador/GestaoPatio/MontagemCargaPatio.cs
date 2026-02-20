using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class MontagemCargaPatio : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio>
    {
        #region Construtores

        public MontagemCargaPatio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaMontagemCargaPatio filtrosPesquisa)
        {
            var consultaMontagemCargaPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio>()
                .Where(o =>
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    (o.Carga != null && o.Carga.OcultarNoPatio == false && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                    (o.Carga == null && o.PreCarga != null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                );

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaMontagemCargaPatio = consultaMontagemCargaPatio.Where(o => o.Carga.DataCarregamentoCarga.Value.Date >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaMontagemCargaPatio = consultaMontagemCargaPatio.Where(o => o.Carga.DataCarregamentoCarga.Value.Date <= filtrosPesquisa.DataLimite.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                consultaMontagemCargaPatio = consultaMontagemCargaPatio.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga || o.Carga.CodigosAgrupados.Contains(filtrosPesquisa.NumeroCarga));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaMontagemCargaPatio = consultaMontagemCargaPatio.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return consultaMontagemCargaPatio;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio BuscarPorCodigo(int codigo)
        {
            var consultaMontagemCargaPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio>()
                .Where(o => o.Codigo == codigo);

            return consultaMontagemCargaPatio.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaMontagemCargaPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaMontagemCargaPatio.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaMontagemCargaPatio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMontagemCargaPatio = Consultar(filtrosPesquisa);

            consultaMontagemCargaPatio = consultaMontagemCargaPatio
                .Fetch(o => o.FluxoGestaoPatio)
                .Fetch(o => o.Carga).ThenFetch(o => o.DadosSumarizados)
                .Fetch(o => o.Carga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.Carga).ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.Carga).ThenFetch(o => o.TipoOperacao)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.DadosSumarizados)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.TipoOperacao);

            return ObterLista(consultaMontagemCargaPatio, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaMontagemCargaPatio filtrosPesquisa)
        {
            var consultaMontagemCargaPatio = Consultar(filtrosPesquisa);

            return consultaMontagemCargaPatio.Count();
        }

        #endregion
    }
}
