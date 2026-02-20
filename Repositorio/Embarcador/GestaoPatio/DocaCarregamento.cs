using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class DocaCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento>
    {
        #region Construtores

        public DocaCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento> Consultar(SituacaoDocaCarregamento? situacao, DateTime dataInicial, DateTime dataFinal, string carga, string preCarga)
        {
            var consultaDocaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento>()
                .Where(o =>
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    (o.Carga != null && o.Carga.OcultarNoPatio == false && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                    (o.Carga == null && o.PreCarga != null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                );

            if (situacao.HasValue && situacao != SituacaoDocaCarregamento.Todos)
                consultaDocaCarregamento = consultaDocaCarregamento.Where(o => o.Situacao == situacao);

            if (dataInicial != DateTime.MinValue)
                consultaDocaCarregamento = consultaDocaCarregamento.Where(o => o.Carga.DataCarregamentoCarga.Value.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                consultaDocaCarregamento = consultaDocaCarregamento.Where(o => o.Carga.DataCarregamentoCarga.Value.Date <= dataFinal);

            if (!string.IsNullOrWhiteSpace(carga))
                consultaDocaCarregamento = consultaDocaCarregamento.Where(o => o.Carga.CodigoCargaEmbarcador == carga || o.Carga.CodigosAgrupados.Contains(carga));

            if (!string.IsNullOrWhiteSpace(preCarga))
                consultaDocaCarregamento = consultaDocaCarregamento.Where(o => o.PreCarga.NumeroPreCarga == preCarga);

            return consultaDocaCarregamento;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento BuscarPorCarga(int codigoCarga)
        {
            var consultaDocaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    o.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem
                );

            return consultaDocaCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento BuscarPorCodigo(int codigo)
        {
            var consultaDocaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaDocaCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaDocaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaDocaCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento BuscarPorLocalCarregamento(int localCarregamento)
        {
            var consultaDocaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento>()
                .Where(o =>
                    o.LocalCarregamento.Codigo == localCarregamento &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    o.Carga.EtapaFaturamentoLiberado == false &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada
                );

            return consultaDocaCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento BuscarPorNumeroDoca(string NumeroDoca)
        {
            var consultaDocaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento>()
                .Where(o =>
                    o.NumeroDoca == NumeroDoca &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    o.Carga.EtapaFaturamentoLiberado == false &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada
                );
            
            return consultaDocaCarregamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento> Consultar(SituacaoDocaCarregamento? situacao, DateTime dataInicial, DateTime dataFinal, string carga, string preCarga, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(situacao, dataInicial, dataFinal, carga, preCarga);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(SituacaoDocaCarregamento? situacao, DateTime dataInicial, DateTime dataFinal, string carga, string preCarga)
        {
            var result = Consultar(situacao, dataInicial, dataFinal, carga, preCarga);

            return result.Count();
        }

        public int ContarPorFilialELocalCarregamento(int codigoDocaCarregamento, int codigoFilial, int codigoLocalCarregamento)
        {
            var consultaDocaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoDocaCarregamento &&
                    o.LocalCarregamento.Codigo == codigoLocalCarregamento &&
                    o.FluxoGestaoPatio.Filial.Codigo == codigoFilial &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    o.FluxoGestaoPatio.DataFinalizacaoFluxo == null &&
                    (
                        (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                    )
                );

            return consultaDocaCarregamento.Count();
        }

        #endregion
    }
}
