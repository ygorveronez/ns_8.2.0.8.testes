using Dominio.Entidades;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Financeiro
{
    public class ConciliacaoTransportador
    {
        private Repositorio.UnitOfWork unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador configuracao;

        public ConciliacaoTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            var repConfiguracaoConciliacaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador(unitOfWork);
            configuracao = repConfiguracaoConciliacaoTransportador.BuscarConfiguracaoPadrao();
        }

        #region Métodos Públicos

        public void AdicionarCargaEmConciliacaoTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (configuracao == null || !configuracao.HabilitarGeracaoAutomatica || carga == null)
            {
                return;
            }

            var repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            var cargaCtes = repCargaCte.BuscarPorCarga(carga.Codigo);
            foreach (var cargaCte in cargaCtes)
            {
                AdicionarCTeEmConciliacaoTransportador(cargaCte.CTe);
            }
        }

        /// <summary>
        /// Verifica se o CTe precisa ser adicionado em uma Conciliação.
        /// Se necessário, cria uma Conciliação ou reutiliza uma já existente.
        /// </summary>
        /// <param name="CTe"></param>
        public void AdicionarCTeEmConciliacaoTransportador(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (configuracao == null || !configuracao.HabilitarGeracaoAutomatica || cte.ConciliacaoTransportador != null || !cte.DataEmissao.HasValue)
            {
                return;
            }

            Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador conciliacao = ObterConciliacaoParaCTe(cte);
                cte.ConciliacaoTransportador = conciliacao;

                repConhecimentoDeTransporteEletronico.Atualizar(cte);

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                throw;
            }

        }

        public void GerarPdfAnuencia(Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador conciliacao, decimal saldoTotal, int empresaCodigo)
        {
            ReportRequest.WithType(ReportType.AnuenciaTransportador)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoConciliacao", conciliacao.Codigo)
                .AddExtraData("EmpresaCodigo", empresaCodigo)
                .AddExtraData("SaldoTotal", saldoTotal.ToString("n2"))
                .CallReport();
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador ObterConciliacaoParaCTe(ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
            Repositorio.Embarcador.Financeiro.ConciliacaoTransportadorEmpresa repConciliacaoTransportadorEmpresa = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportadorEmpresa(unitOfWork);

            ObterDataInicialEFinalConciliacao(cte.DataEmissao.Value, out DateTime? dataInicial, out DateTime? dataFinal);

            if (!dataInicial.HasValue || !dataFinal.HasValue)
            {
                throw new Exception("Erro ao obter datas do começo e fim da conciliação de transportador");
            }

            if (configuracao.TipoCnpj == TipoCnpjConciliacaoTransportador.RaizCNPJ)
            {
                var conciliacao = repConciliacaoTransportador.BuscarPorRaizCnpj(cte.Empresa.RaizCnpj, dataInicial.Value, dataFinal.Value, configuracao.Periodicidade);

                if (conciliacao != null)
                {
                    repConciliacaoTransportadorEmpresa.AdicionarTransportadorNaConciliacao(cte.Empresa, conciliacao);
                    return conciliacao;
                }
            }
            else if (configuracao.TipoCnpj == TipoCnpjConciliacaoTransportador.CNPJCompleto)
            {
                var conciliacao = repConciliacaoTransportador.BuscarPorTransportador(cte.Empresa.Codigo, dataInicial.Value, dataFinal.Value, configuracao.Periodicidade);

                if (conciliacao != null)
                {
                    repConciliacaoTransportadorEmpresa.AdicionarTransportadorNaConciliacao(cte.Empresa, conciliacao);
                    return conciliacao;
                }
            }

            return CriarConciliacaoParaCte(cte, dataInicial.Value, dataFinal.Value);
        }

        private Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador CriarConciliacaoParaCte(ConhecimentoDeTransporteEletronico cte, DateTime dataInicial, DateTime dataFinal)
        {
            Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
            Repositorio.Embarcador.Financeiro.ConciliacaoTransportadorEmpresa repConciliacaoTransportadorEmpresa = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportadorEmpresa(unitOfWork);

            var conciliacao = new Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador
            {
                SituacaoConciliacaoTransportador = SituacaoConciliacaoTransportador.Aberta,
                DataInicial = dataInicial,
                DataFinal = dataFinal,
                RaizCnpj = cte.Empresa.RaizCnpj,
                DataAnuenciaDisponivel = dataFinal.AddDays(configuracao.DiasParaContestacao),
                DataAssinaturaAnuencia = null,
                Periodicidade = configuracao.Periodicidade,
            };

            repConciliacaoTransportador.Inserir(conciliacao);
            repConciliacaoTransportadorEmpresa.AdicionarTransportadorNaConciliacao(cte.Empresa, conciliacao);

            return conciliacao;
        }

        public void ObterDataInicialEFinalConciliacao(DateTime dataAtual, out DateTime? dataInicial, out DateTime? dataFinal)
        {
            dataInicial = null;
            dataFinal = null;

            switch (configuracao.Periodicidade)
            {
                case PeriodicidadeConciliacaoTransportador.Mensal:
                    dataInicial = dataAtual.FirstDayOfMonth();
                    dataFinal = dataAtual.LastDayOfMonth();
                    break;
                case PeriodicidadeConciliacaoTransportador.Anual:
                    var primeiroDiaDoAno = dataAtual.AddMonths(-dataAtual.Month + 1).FirstDayOfMonth();
                    dataInicial = primeiroDiaDoAno;
                    dataFinal = dataInicial.Value.AddYears(1).AddDays(-1);
                    break;
                case PeriodicidadeConciliacaoTransportador.Trimestral:
                    ObterDataInicialEFinalConciliacaoTrimestral(dataAtual, ref dataInicial, ref dataFinal);
                    break;
                case PeriodicidadeConciliacaoTransportador.Semestral:
                    ObterDataInicialEFinalConciliacaoSemestral(dataAtual, ref dataInicial, ref dataFinal);
                    break;
                case PeriodicidadeConciliacaoTransportador.Bimestral:
                    ObterDataInicialEFinalConciliacaoBimestral(dataAtual, ref dataInicial, ref dataFinal);
                    break;
            }
        }

        private void ObterDataInicialEFinalConciliacaoTrimestral(DateTime dataAtual, ref DateTime? dataInicial, ref DateTime? dataFinal)
        {

            int modificador = 0;
            switch (configuracao.SequenciaPeriodicidade)
            {
                case SequenciaPeriodicidadeConciliacaoTransportador.TrimestralJaneiro:
                    modificador = 0;
                    break;
                case SequenciaPeriodicidadeConciliacaoTransportador.TrimestralFevereiro:
                    modificador = 2;
                    break;
                case SequenciaPeriodicidadeConciliacaoTransportador.TrimestralMarco:
                    modificador = 1;
                    break;
            }

            int indexDaSequencia = (dataAtual.Month - 1 + modificador) / 3;
            var primeiroDiaDoAno = dataAtual.AddMonths(-dataAtual.Month + 1 - modificador).FirstDayOfMonth();

            dataInicial = primeiroDiaDoAno.AddMonths(indexDaSequencia * 3);
            dataFinal = dataInicial.Value.AddMonths(3).AddDays(-1);
        }

        private void ObterDataInicialEFinalConciliacaoSemestral(DateTime dataAtual, ref DateTime? dataInicial, ref DateTime? dataFinal)
        {

            int modificador = 0;
            switch (configuracao.SequenciaPeriodicidade)
            {
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralJaneiro:
                    modificador = 0;
                    break;
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralFevereiro:
                    modificador = 5;
                    break;
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralMarco:
                    modificador = 4;
                    break;
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralAbril:
                    modificador = 3;
                    break;
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralMaio:
                    modificador = 2;
                    break;
                case SequenciaPeriodicidadeConciliacaoTransportador.SemestralJunho:
                    modificador = 1;
                    break;
            }

            int indexDaSequencia = (dataAtual.Month - 1 + modificador) / 6;
            var primeiroDiaDoAno = dataAtual.AddMonths(-dataAtual.Month + 1 - modificador).FirstDayOfMonth();

            dataInicial = primeiroDiaDoAno.AddMonths(indexDaSequencia * 6);
            dataFinal = dataInicial.Value.AddMonths(6).AddDays(-1);
        }

        private void ObterDataInicialEFinalConciliacaoBimestral(DateTime dataAtual, ref DateTime? dataInicial, ref DateTime? dataFinal)
        {

            int modificador = 0;
            switch (configuracao.SequenciaPeriodicidade)
            {
                case SequenciaPeriodicidadeConciliacaoTransportador.BimestralJaneiro:
                    modificador = 0;
                    break;
                case SequenciaPeriodicidadeConciliacaoTransportador.BimestralFevereiro:
                    modificador = 1;
                    break;
            }

            int indexDaSequencia = (dataAtual.Month - 1 + modificador) / 2;
            var primeiroDiaDoAno = dataAtual.AddMonths(-dataAtual.Month + 1 - modificador).FirstDayOfMonth();

            dataInicial = primeiroDiaDoAno.AddMonths(indexDaSequencia * 2);
            dataFinal = dataInicial.Value.AddMonths(2).AddDays(-1);
        }

        #endregion
    }
}
