using Servicos.Embarcador.Carga;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 600000)]

    public class CargaFechamento : LongRunningProcessBase<CargaFechamento>
    {

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ProcessarFechamentosAguardandoRateio(unitOfWork);

            ProcessarFechamentosAguardandoCalculoFrete(unitOfWork);
        }


        private void ProcessarFechamentosAguardandoRateio(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.RateioFrete servicorateio = new Servicos.Embarcador.Carga.RateioFrete();
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new CargaDadosSumarizados(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFechamento repCargaFechamento = new Repositorio.Embarcador.Cargas.CargaFechamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaFechamento> cargaFechamentos = repCargaFechamento.BuscarAguardandoRateio();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaFechamento cargaFechamento in cargaFechamentos)
            {
                Servicos.Log.TratarErro($"Fechamento cargas aguardando Rateio Inicio | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "CargaFechamento");

                try
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaFechamento.Carga;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorCarga(carga.Codigo);
                    servicorateio.RatearValorDoFrenteEntrePedidos(carga, cargasPedido, configuracao, false, unitOfWork, _tipoServicoMultisoftware);

                    serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargasPedido, configuracao, unitOfWork, _tipoServicoMultisoftware);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoSemCobertura = repCargaPedido.BuscarCargaPedidoNaoPossuiNumeroControlePedido(carga.Codigo);
                    servicorateio.AdicionarValoresDaCargaNotasSemCobertura(carga, cargasPedidoSemCobertura, unitOfWork);

                    cargaFechamento.SituacaoFechamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaFechamento.AgCalculoFrete;

                    repCargaFechamento.Atualizar(cargaFechamento);

                    unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao);
                }

                Servicos.Log.TratarErro($"Fechamento cargas aguardando Rateio Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "CargaFechamento");
            }

        }

        private void ProcessarFechamentosAguardandoCalculoFrete(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Frete servicoFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, _tipoServicoMultisoftware);

            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrenciaValorFechamento = repositorioTipoDeOcorrenciaDeCTe.BuscarTipoOcorrenciaDiferencaValorFechamento();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Cargas.CargaFechamento repCargaFechamento = new Repositorio.Embarcador.Cargas.CargaFechamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaFechamento> cargaFechamentos = repCargaFechamento.BuscarAguardandoCalculoFrete();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaFechamento cargaFechamento in cargaFechamentos)
            {
                Servicos.Log.TratarErro($"Fechamento cargas aguardando Calculo Frete Inicio | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "CargaFechamento");

                try
                {
                    unitOfWork.Start();

                    if (cargaFechamento.Carga.ValorFrete <= 0)
                    {
                        cargaFechamento.MotivoRejeicaoCalculoFrete = "O valor do parâmetro base está zerado";
                        cargaFechamento.SituacaoFechamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaFechamento.ProblemaCalculoFrete;

                        repCargaFechamento.Atualizar(cargaFechamento);
                        unitOfWork.CommitChanges();

                        continue;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosRetorno = servicoFrete.CalcularFreteParaCargaFechamento(cargaFechamento, unitOfWork, _tipoServicoMultisoftware, configuracao);

                    if (!dadosRetorno.FreteCalculado || dadosRetorno.FreteCalculadoComProblemas)
                    {
                        cargaFechamento.MotivoRejeicaoCalculoFrete = dadosRetorno.MensagemRetorno;
                        cargaFechamento.SituacaoFechamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaFechamento.ProblemaCalculoFrete;
                    }
                    else
                        cargaFechamento.SituacaoFechamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaFechamento.Finalizado;

                    cargaFechamento.ValorRecalculado = dadosRetorno.ValorFrete;

                    repCargaFechamento.Atualizar(cargaFechamento);

                    if (cargaFechamento.ValorRecalculado > cargaFechamento.ValorFrete && tipoOcorrenciaValorFechamento != null)
                    {
                        decimal Diferenca = cargaFechamento.ValorRecalculado - cargaFechamento.ValorFrete;

                        Servicos.Embarcador.CargaOcorrencia.Ocorrencia servOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                        servOcorrencia.GerarOcorrenciaCargaValorFreteFechamento(cargaFechamento.Carga, Diferenca, tipoOcorrenciaValorFechamento, unitOfWork, _tipoServicoMultisoftware, configuracao, _clienteMultisoftware);
                    }

                    unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao);
                }

                Servicos.Log.TratarErro($"Fechamento cargas aguardando Calculo Frete Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ", "CargaFechamento");
            }

        }
    }
}