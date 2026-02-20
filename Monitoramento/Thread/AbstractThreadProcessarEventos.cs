using Servicos.Embarcador.Monitoramento;
using System.Reflection;

namespace Monitoramento.Thread
{

    public abstract class AbstractThreadProcessarEventos : AbstractThreadProcessamento
    {

        #region Atributos protegidos

        protected int tempoSleep = 5;
        protected bool enable = true;
        protected int limiteRegistros = 100;
        protected string arquivoNivelLog;
        protected int limiteDiasConsulta = 0;
        protected int MinutosFiltroProcessar = 0;
        protected DateTime dataAtual;

        private static System.Threading.Thread EventosThread;

        #endregion

        #region Métodos públicos

        public System.Threading.Thread Iniciar(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancellationToken)
        {
            if (enable)
                EventosThread = base.IniciarThread(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware, arquivoNivelLog, tempoSleep, cancellationToken);

            return EventosThread;
        }

        public void Finalizar()
        {
            if (enable)
                Parar();
        }

        #endregion

        #region Métodos públicos abstratos

        public abstract void ProcessarEventosPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente);

        #endregion

        #region Implementação dos métodos abstratos

        override protected void Executar(Repositorio.UnitOfWork unitOfWork, string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {

            try
            {
                unitOfWork.Start();

                ProcessarEventosPendentes(unitOfWork, clienteMultisoftware);

                unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        override protected void Parar()
        {
            if (EventosThread != null)
            {
                EventosThread.Interrupt();
                EventosThread = null;
            }
        }

        #endregion

        #region Métodos protegidos

        /**
         * Percorre os eventos ativos e executa o método "Processar"
         */
        protected void ProcessarEventosAtivos(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> listaMonitoramentoEventoAtivo, List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento> eventos, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal> permanenciaLocais)
        {

            int total = eventos.Count;
            if (total > 0 && monitoramentoProcessarEvento != null)
            {
                DateTime inicio = DateTime.UtcNow, inicio1;

                // Processamento de cada um dos eventos
                for (int i = 0; i < total; i++)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> ListaMonitoramentoEventoAtivo = listaMonitoramentoEventoAtivo.Where(x => x.TipoAlerta == eventos[i].GetTipoAlerta()).ToList();
                    foreach (var monitoramentoEvendo in ListaMonitoramentoEventoAtivo)
                    {
                        inicio1 = DateTime.UtcNow;
                        Type thisType = eventos[i].GetType();
                        MethodInfo theMethod = thisType.GetMethod("Processar");
                        theMethod.Invoke(eventos[i], new object[] { monitoramentoEvendo, monitoramentoProcessarEvento, monitoramento, posicoesObjetoValor, alertas, entregas, codigosClientesAlvo, cargaJanelaCarregamento, cargaJanelasDescarregamento, permanenciaLocais });
                        Log(thisType.Name, inicio1, 5);
                    }

                }

                Log($"ProcessarEventos", inicio, 4);
            }
        }

        /**
         * Busca um possível monitoramento em aberto do veículo em uma determinada data
         */
        protected Dominio.Entidades.Embarcador.Logistica.Monitoramento BuscarMonitoramentoEmAberto(List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos, int? codigoVeiculo, DateTime? data)
        {
            DateTime inicio = DateTime.UtcNow;
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = null;
            if (monitoramentos != null && codigoVeiculo != null && data != null)
            {
                int total = monitoramentos.Count;
                for (int i = 0; i < total; i++)
                {
                    if (monitoramentos[i].Veiculo != null && monitoramentos[i].Veiculo.Codigo == codigoVeiculo &&
                        (
                            (
                                monitoramentos[i].Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                                monitoramentos[i].DataInicio <= data
                            )
                            ||
                            (
                                monitoramentos[i].Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado &&
                                data >= monitoramentos[i].DataInicio && data <= monitoramentos[i].DataFim
                            )
                        )
                    )
                    {
                        monitoramento = monitoramentos[i];
                        break;
                    }
                }
            }
            Log($"BuscarMonitoramentoEmAberto", inicio, 4);

            return monitoramento;
        }

        /**
         * Busca uma lista de últimos alteras para os veículos e eventos
         */
        protected List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> BuscarAlertasPendentes(Repositorio.UnitOfWork unitOfWork, List<int> codigosVeiculos, List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento> eventos)
        {
            DateTime inicio = DateTime.UtcNow;
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor>();
            if (codigosVeiculos != null && codigosVeiculos.Count > 0 && eventos != null && eventos.Count > 0)
            {
                int total = eventos.Count;
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>();
                for (int i = 0; i < total; i++)
                {
                    tiposAlerta.Add(eventos[i].GetTipoAlerta());
                }

                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                alertas = repAlertaMonitor.BuscarUltimoAlertaObjetoDeValorPorVeiculosTiposDeAlerta(codigosVeiculos, tiposAlerta);
            }
            Log($"BuscarAlertasPendentes {alertas.Count}", inicio, 1);
            return alertas;
        }

        /**
         * Extrai os códigos únicos dos veículos envolvidos nas posições recebidas
         */
        protected List<int> ObtemCodigosVeiculosDistintos(IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> processarEventos)
        {
            List<int> codigosVeiculos = new List<int>();
            int t = processarEventos.Count;
            for (int i = 0; i < t; i++)
            {
                if (processarEventos[i].CodigoPosicao != null && processarEventos[i].CodigoVeiculo != null && !codigosVeiculos.Contains(processarEventos[i].CodigoVeiculo.Value))
                {
                    codigosVeiculos.Add(processarEventos[i].CodigoVeiculo.Value);
                }
            }
            Log($"{codigosVeiculos.Count} veiculos", 1);
            return codigosVeiculos;
        }

        #endregion

        #region Métodos privados

        /**
    * Consulta todas as posições dos veículos no período
    */
        protected IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> CarregarPosicoesVeiculos(Repositorio.UnitOfWork unitOfWork, List<int> codigosVeiculos, DateTime dataInicial, DateTime dataFinal)
        {
            DateTime inicio = DateTime.UtcNow;
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor = repPosicao.BuscarWaypointsPorVeiculosDataInicialeFinal(codigosVeiculos, dataInicial, dataFinal);
            Log($"CarregarPosicoesVeiculos {posicoesObjetoValor.Count}", inicio, 1);
            return posicoesObjetoValor;
        }

        /**
         * Extrai as posições do veículo da lista de posições de todos os veículos
         */
        protected List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscarPosicoesVeiculo(int codigoVeiculo, IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculos)
        {
            DateTime inicio = DateTime.UtcNow;
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            int total = posicoesVeiculos.Count;
            for (int i = 0; i < total; i++)
            {
                if (posicoesVeiculos[i].CodigoVeiculo == codigoVeiculo)
                {
                    posicoesObjetoValor.Add(posicoesVeiculos[i]);
                }
            }
            Log($"BuscarPosicoesVeiculo {posicoesObjetoValor.Count}", inicio, 3);
            return posicoesObjetoValor;
        }

        /**
         * Extrai as permanências de locais cadastrados nos eventos
         */
        protected List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal> CarregarPermanenciaLocais(Repositorio.UnitOfWork unitOfWork, List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento> eventosAtivos)
        {
            Repositorio.Embarcador.Logistica.PermanenciaLocal permanenciaLocal = new Repositorio.Embarcador.Logistica.PermanenciaLocal(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal> permanenciasLocal = new();
            Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento evento = eventosAtivos.Where(e => e.GetTipoAlerta() == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PermanenciaNoPontoApoio).FirstOrDefault();
            List<int> codigosLocais = evento?.GetListaCodigosLocais() ?? new();

            if (codigosLocais.Count > 0)
                permanenciasLocal = permanenciaLocal.BuscarObjetoDeValorAbertaPorLocais(codigosLocais);
            return permanenciasLocal;
        }

        #endregion

    }
}
