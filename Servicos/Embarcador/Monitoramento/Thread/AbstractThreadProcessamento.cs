using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento
{

    public abstract class AbstractThreadProcessamento : AbstractThread
    {

        #region Métodos protegidos

        protected System.Threading.Thread IniciarThread(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, string arquivoNivelLog, int tempoSleep, CancellationToken? cancellationToken = null)
        {

            System.Threading.Thread task = new System.Threading.Thread(() =>
              {
                  System.Threading.Thread.CurrentThread.Name = this.GetType().Name;

                  while ((!cancellationToken?.IsCancellationRequested) ?? true)
                  {
                      try
                      {
                          CarregarNivelLogDoArquivo(arquivoNivelLog);
                          DateTime inicio = DateTime.UtcNow;
                          Log("Inicio");

                          using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                          {
                              if (PossuiMonitoramento(unitOfWork))
                              {
                                  Executar(unitOfWork, stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware);
                              }
                              else
                              {
                                  Log($"Configuracao \"Possui monitoramento\" desativada");
                              }
                          }

                          Log("Fim", inicio, 0, true);

                          System.Threading.Thread.Sleep(tempoSleep * 1000);

                      }
                      catch (TaskCanceledException abort)
                      {
                          Servicos.Log.TratarErro(string.Concat("Task de monitoramento de posicoes cancelada: ", abort.ToString()));
                          break;
                      }
                      catch (System.Threading.ThreadAbortException abortThread)
                      {
                          Servicos.Log.TratarErro(string.Concat("Task de monitoramento de posicoes cancelada: ", abortThread.ToString()));
                          break;
                      }
                      catch (Exception ex)
                      {
                          Servicos.Log.TratarErro(ex);
                          System.Threading.Thread.Sleep(tempoSleep * 1000);
                      }
                  }
              });

            // Inicia a task
            task.Start();

            return task;
        }

        #endregion

        #region Métodos protegidos abstratos

        protected abstract void Executar(Repositorio.UnitOfWork unitOfWork, string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware);

        protected abstract void Parar();

        #endregion


        #region Métodos protegidos

        protected DadosComplexosMonitoramentos CarregarDadosComplexosDoMonitoramento(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota)
        {
            Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
            Repositorio.Embarcador.Logistica.PermanenciaSubarea repPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            List<int> codigosMonitoramento = monitoramentosParaAtualizarRota.Select(obj => obj.Monitoramento?.Codigo ?? 0).ToList();
            List<int> codigosCarga = monitoramentosParaAtualizarRota.Select(obj => obj.Monitoramento?.Carga?.Codigo ?? 0).ToList();
            List<long> codigosPosicao = monitoramentosParaAtualizarRota.Select(obj => obj.Posicao?.Codigo ?? 0).ToList();

            return new DadosComplexosMonitoramentos()
            {
                _codigosMonitoramento = codigosMonitoramento,
                _codigosCarga = codigosCarga,
                _codigosPosicao = codigosPosicao,
                _ListStatusViagemTodosMonitoramentos = repMonitoramentoHistoricoStatusViagem.BuscarPorMonitoramentos(codigosMonitoramento),
                _ListTodosCargaRotaFretePontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargas(codigosCarga),
                _ListPermanenciasClientesTodos = repPermanenciaCliente.BuscarPorCargas(codigosCarga),
                _ListPermanenciasSubareasTodos = repPermanenciaSubarea.BuscarPorCargas(codigosCarga),
                _CargaEntregasTodos = BuscarCargaEntregaPorCargas(unitOfWork, codigosCarga),
                _cargaJanelaCarregamentosTodos = BuscarJanelaCarregamentoPorCargas(unitOfWork, codigosCarga),
                _cargaJanelasDescarregamentosTodos = BuscarJanelasDescarregamentoPorCargas(unitOfWork, codigosCarga),
                _posicoesAlvoTodos = BuscarPosicaoAlvosPorPosicoes(unitOfWork, codigosPosicao),
                _posicoesAlvoSubareaTodos = BuscarPosicaoAlvosSubareasPorPosicoes(unitOfWork, codigosPosicao),
                _clientesOrigemTodos = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDasCargasPeloPedido(unitOfWork, codigosCarga),
            };
        }

        protected List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarCargaEntregaPorCargas(Repositorio.UnitOfWork unitOfWork, List<int> codigoCargas)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = new();
            if (codigoCargas.Count > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                cargaEntregas = repCargaEntrega.BuscarPorCargas(codigoCargas);
            }
            return cargaEntregas;
        }

        protected List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarJanelaCarregamentoPorCargas(Repositorio.UnitOfWork unitOfWork, List<int> codigosCarga)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargaJanelaCarregamento = new();
            if (codigosCarga.Count > 0)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCargas(codigosCarga);
            }
            return cargaJanelaCarregamento;
        }

        protected List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarJanelasDescarregamentoPorCargas(Repositorio.UnitOfWork unitOfWork, List<int> codigosCarga)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento = new();
            if (codigosCarga.Count > 0)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                cargaJanelasDescarregamento = repCargaJanelaDescarregamento.BuscarTodasPorCargas(codigosCarga, false);
            }
            return cargaJanelasDescarregamento;
        }

        protected List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> BuscarPosicaoAlvosPorPosicoes(Repositorio.UnitOfWork unitOfWork, List<long> codigosPosicao)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos = new();
            if (codigosPosicao.Count > 0)
            {
                Repositorio.Embarcador.Logistica.PosicaoAlvo repPosicaoAlvo = new Repositorio.Embarcador.Logistica.PosicaoAlvo(unitOfWork);
                posicaoAlvos = repPosicaoAlvo.BuscarPorPosicao(codigosPosicao);
            }
            return posicaoAlvos;
        }

        protected IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo> BuscarPosicaoAlvosPorClienteVeiculo(Repositorio.UnitOfWork unitOfWork, List<Tuple<int, string, DateTime, DateTime>> tuplaClienteVeiculo)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo>();
            if (tuplaClienteVeiculo.Count > 0)
            {
                Repositorio.Embarcador.Logistica.PosicaoAlvo repPosicaoAlvo = new Repositorio.Embarcador.Logistica.PosicaoAlvo(unitOfWork);
                posicaoAlvos = repPosicaoAlvo.BuscarPorTuplaClienteVeiculo(tuplaClienteVeiculo);
            }
            return posicaoAlvos;
        }

        protected IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo> BuscarPosicaoAlvosSubAreaPorClienteVeiculo(Repositorio.UnitOfWork unitOfWork, List<Tuple<int, string, DateTime, DateTime>> tuplaClienteVeiculo)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo>();
            if (tuplaClienteVeiculo.Count > 0)
            {
                Repositorio.Embarcador.Logistica.PosicaoAlvo repPosicaoAlvo = new Repositorio.Embarcador.Logistica.PosicaoAlvo(unitOfWork);
                posicaoAlvos = repPosicaoAlvo.BuscarPorTuplaClienteSubareaVeiculo(tuplaClienteVeiculo);
            }
            return posicaoAlvos;
        }

        protected List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> BuscarPosicaoAlvosSubareasPorPosicoes(Repositorio.UnitOfWork unitOfWork, List<long> codigosPosicao)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvosSubareas = new();
            if (codigosPosicao.Count > 0)
            {
                Repositorio.Embarcador.Logistica.PosicaoAlvoSubarea repPosicaoAlvoSubarea = new Repositorio.Embarcador.Logistica.PosicaoAlvoSubarea(unitOfWork);
                posicaoAlvosSubareas = repPosicaoAlvoSubarea.BuscarPorPosicao(codigosPosicao);
            }
            return posicaoAlvosSubareas;
        }

        #endregion
    }


    #region Classes de Processamento
    public class MonitoramentoPendente
    {
        public DateTime DataInicial;
        public DateTime DataFim;
        public Dominio.Entidades.Embarcador.Logistica.Monitoramento Monitoramento;
        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> MonitoramentoVeiculos;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RotaRealizadaCompilada;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RespostaRotaRealizada;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RespostaRotaAteOrigem;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RespostaRotaAteDestino;
        public Dominio.Entidades.Embarcador.Logistica.Posicao Posicao;
        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> PosicaoAlvos;
        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> PosicaoAlvoSubareas;
        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> PosicoesVeiculo;
        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem StatusViagem;
        public DateTime DataInicialStatusViagem;
        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> PontosDePassagens;
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> CargaEntregas;
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> PrevisoesCargaEntrega;
        public Dominio.Entidades.Cliente? ClienteOrigem;
        public decimal Percentual;
        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> HistoricosStatusViagem;
    }

    public class CargaPendente
    {
        public Dominio.Entidades.Embarcador.Cargas.Carga Carga;
        public DateTime DataPrevisao;
    }

    public class DadosComplexosMonitoramentos
    {
        public List<int> _codigosMonitoramento;
        public List<int> _codigosCarga;
        public List<long> _codigosPosicao;
        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> _ListStatusViagemTodosMonitoramentos;
        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> _ListTodosCargaRotaFretePontosPassagem;
        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> _ListPermanenciasClientesTodos;
        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> _ListPermanenciasSubareasTodos;
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> _CargaEntregasTodos;
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> _cargaJanelaCarregamentosTodos;
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> _cargaJanelasDescarregamentosTodos;
        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> _posicoesAlvoTodos;
        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> _posicoesAlvoSubareaTodos;
        public List<Tuple<int, Dominio.Entidades.Cliente>> _clientesOrigemTodos;
    }
    #endregion
}