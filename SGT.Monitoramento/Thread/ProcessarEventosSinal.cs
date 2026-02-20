using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Collections.Specialized;

namespace SGT.Monitoramento.Thread
{
    public class ProcessarEventosSinal : AbstractThreadProcessarEventos
    {

        #region Atributos privados

        private static ProcessarEventosSinal Instante;

        #endregion

        #region Métodos públicos

        // Singleton
        public static ProcessarEventosSinal GetInstance(string stringConexao)
        {
            if (Instante == null) Instante = new ProcessarEventosSinal(stringConexao);
            return Instante;
        }

        #endregion

        #region Construtor privado

        private ProcessarEventosSinal(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = string.IsNullOrWhiteSpace(stringConexao) ? null : new Repositorio.UnitOfWork(stringConexao);
            base.dataAtual = DateTime.Now;
            try
            {
                base.tempoSleep = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventosSinal().TempoSleepThread;
                base.enable = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventosSinal().Ativo;
                base.limiteRegistros = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventosSinal().LimiteRegistros;
                base.arquivoNivelLog = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventosSinal().ArquivoNivelLog;
            }
            catch (Exception e)
            {
                Log(e.Message);
                throw e;
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }

        #endregion

        #region Métodos abstratos

        public override void ProcessarEventosPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            if (!configuracao.PossuiMonitoramento) return;

            // Identifica os eventos ativos s e configurados para serem processados
            List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento> eventos = CarregarEventosDeSinalAtivos(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> listaMonitoramentoEventoAtivo = repMonitoramentoEvento.BuscarTodosAtivos();
            if (eventos.Count > 0)
            {

                Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> posicoesAtuais = repPosicaoAtual.BuscarProcessarEventos();

                int total = posicoesAtuais.Count;
                Log($"{total} posicoes atuais a verificar");
                if (total > 0)
                {

                    // Extrai os veículos únicos envolvidos nas posições recebidas
                    List<int> codigosVeiculos = ObtemCodigosVeiculosDistintos(posicoesAtuais);

                    // Carrega o último alerta por veículo e evento
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas = BuscarAlertasPendentes(unitOfWork, codigosVeiculos, eventos);

                    // Busca uma lista de monitoramentos abertos durante as posições recebidas
                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosAbertos = BuscarMonitoramentosAbertos(unitOfWork, codigosVeiculos, posicoesAtuais);
                    Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento;

                    DateTime inicio;

                    try
                    {
                        for (int i = 0; i < total; i++)
                        {
                            if (posicoesAtuais[i].CodigoVeiculo != null)
                            {

                                // Busca algum monitoramento aberto
                                monitoramento = BuscarMonitoramentoEmAberto(monitoramentosAbertos, posicoesAtuais[i].CodigoVeiculo, posicoesAtuais[i].DataVeiculoPosicao);

                                // Processamento de cada um dos eventos ativos
                                ProcessarEventosAtivos(listaMonitoramentoEventoAtivo, eventos, monitoramento, posicoesAtuais[i], null, alertas, null, null, null, null, configuracao, unitOfWork);

                            }
                        }

                        inicio = DateTime.UtcNow;

                        Log("CommitChanges BuscarPosicoesProcessarEventos", inicio, 1);

                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        throw e;
                    }
                    Log($"{total} posicoes atuais verificadas com sucesso");
                }
            }
        }

        private List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento> CarregarEventosDeSinalAtivos(Repositorio.UnitOfWork unitOfWork)
        {
            DateTime inicio = DateTime.UtcNow;
            List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento> eventos = new List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento>();

            Servicos.Embarcador.Monitoramento.Eventos.PerdaDeSinal PerdaDeSinal = new Servicos.Embarcador.Monitoramento.Eventos.PerdaDeSinal(unitOfWork);
            if (PerdaDeSinal.EstaAtivo())
            {
                eventos.Add(PerdaDeSinal);
            }

            Servicos.Embarcador.Monitoramento.Eventos.SemSinal SemSinal = new Servicos.Embarcador.Monitoramento.Eventos.SemSinal(unitOfWork);
            if (SemSinal.EstaAtivo())
            {
                eventos.Add(SemSinal);
            }

            Log(eventos.Count + " eventos de sinal a processar", inicio, 1);
            return eventos;
        }

        /**
         * Busca uma lista de monitoramentos abertos no período das posições atuais
         */
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentosAbertos(Repositorio.UnitOfWork unitOfWork, List<int> codigosVeiculos, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> posicoesAtuais)
        {
            List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos = new List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            if (posicoesAtuais != null && posicoesAtuais.Count > 0 && posicoesAtuais.First().CodigoVeiculo != null && posicoesAtuais.Last().CodigoVeiculo != null)
            {
                DateTime inicio = DateTime.UtcNow, dataInicio = dataAtual.AddHours(-24), dataFim = DateTime.Now;
                Log($"BuscarMonitoramentoEmAbertoNoPeriodo {dataInicio} a {dataFim}", 2);

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

                int total = codigosVeiculos.Count;
                int limit = 1000;
                int index = 0;
                while (index < total)
                {
                    List<int> codigosVeiculosParciais = codigosVeiculos.Skip(index).Take(limit).ToList();
                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosParciais = repMonitoramento.BuscarMonitoramentoEmAbertoNoPeriodo(codigosVeiculosParciais, dataInicio, dataFim);
                    monitoramentos = monitoramentos.Concat(monitoramentosParciais).ToList();
                    index += limit;
                }
                monitoramentos.Sort((x, y) => x.DataInicio.Value.CompareTo(y.DataInicio.Value));

                Log($"BuscarMonitoramentoEmAbertoNoPeriodo {monitoramentos.Count}", inicio, 1);

            }
            return monitoramentos;
        }

        #endregion

    }
}
