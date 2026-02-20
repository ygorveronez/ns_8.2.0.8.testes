using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CargaOcorrencia
{
    public sealed class RegraParcelamentoOcorrencia
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Contrutores

        public RegraParcelamentoOcorrencia(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento ObterRegraParcelamento(Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia, decimal percentualFaturamentoMedioTransportador)
        {
            Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento repositorioParcelamento = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento> parcelamentos = repositorioParcelamento.BuscarPorRegraParcelamentoOcorrencia(regraParcelamentoOcorrencia.Codigo);

            return (
                from o in parcelamentos
                where o.PercentualInicial <= percentualFaturamentoMedioTransportador && o.PercentualFinal >= percentualFaturamentoMedioTransportador
                select o
            ).FirstOrDefault();
        }

        private decimal ObterPercentualFaturamentoMedioTransportador(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia)
        {
            int totalDiasNoPeriodo = (int)regraParcelamentoOcorrencia.PeriodoFaturamento * regraParcelamentoOcorrencia.QuantidadePeriodos;
            DateTime dateFinalPeriodo = DateTime.Now.FirstDayOfMonth().AddDays(-1);
            DateTime dataInicialPeriodo = dateFinalPeriodo.AddDays(-totalDiasNoPeriodo);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            decimal valorFretePorPeriodo = repositorioCarga.BuscarValorFreteCargasFaturadasPorPeriodo(dataInicialPeriodo, dateFinalPeriodo, ocorrencia.ObterEmitenteOcorrencia().Codigo);

            if (valorFretePorPeriodo <= ocorrencia.ValorOcorrencia)
                return 100m;

            return (ocorrencia.ValorOcorrencia * 100) / valorFretePorPeriodo;
        }

        private Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia ObterRegraParcelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia repositorio = new Repositorio.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia(_unitOfWork);

            return repositorio.BuscarPrimeiraAtiva();
        }

        #endregion

        #region Métodos Públicos

        public void DefinirParcelamento(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            if ((ocorrencia.TipoOcorrencia == null) || !ocorrencia.TipoOcorrencia.ExibirParcelasNaAprovacao || !ocorrencia.TipoOcorrencia.UtilizarParcelamentoAutomatico)
                return;

            if (ocorrencia.ObterEmitenteOcorrencia() == null)
                return;

            Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia regraParcelamentoOcorrencia = ObterRegraParcelamentoOcorrencia(ocorrencia);

            if (regraParcelamentoOcorrencia == null)
                return;

            decimal percentualFaturamentoMedioTransportador = ObterPercentualFaturamentoMedioTransportador(ocorrencia, regraParcelamentoOcorrencia);
            Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento regraParcelamento = ObterRegraParcelamento(regraParcelamentoOcorrencia, percentualFaturamentoMedioTransportador);

            ocorrencia.QuantidadeParcelas = regraParcelamento?.NumeroParcelas ?? 0;
            ocorrencia.PercentualJurosParcela = regraParcelamento?.PercentualJurosParcela ?? 0m;
        }

        #endregion
    }
}
