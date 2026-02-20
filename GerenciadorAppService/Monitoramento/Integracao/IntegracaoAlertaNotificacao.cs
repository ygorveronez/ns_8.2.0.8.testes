using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AdminMultisoftware.Dominio.Enumeradores;
using Repositorio;
using Servicos;

namespace SGT.GerenciadorApp.Monitoramento.Integracao
{
    public class IntegracaoAlertaNotificacao
    {
        private static readonly int Tempo = 5000;
        private ConcurrentDictionary<int, Task> ListaTasks;
        private static IntegracaoAlertaNotificacao Instance;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        public static IntegracaoAlertaNotificacao GetInstance()
        {
            if (Instance == null)
                Instance = new IntegracaoAlertaNotificacao();

            return Instance;
        }
        public void QueueItem(int idEmpresa, string stringConexao, string stringConexaoAdmin, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (!ListaTasks.ContainsKey(idEmpresa))
                this.IniciarThread(idEmpresa, stringConexao, stringConexaoAdmin, tipoServicoMultisoftware);
        }
        private void IniciarThread(int idEmpresa, string stringConexao, string stringConexaoAdmin, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var filaConsulta = new ConcurrentQueue<int>();

            filaConsulta.Enqueue(idEmpresa);

            Task task = new Task(() =>
            {

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (NHibernate.ISession session = Repositorio.SessionHelper.OpenSession(stringConexao))
                        {
                            using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(session))
                            {
                                _unidadeDeTrabalho = unidadeDeTrabalho;
                                if (ValidarIntegracao())
                                {
                                    SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Inicio", this.GetType().Name);
                                    GerarAlertaNotificacao(unidadeDeTrabalho, stringConexao, tipoServicoMultisoftware);
                                    SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Fim\r\n", this.GetType().Name);
                                }

                                unidadeDeTrabalho.Dispose();
                            }
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(Tempo);

                        if (!filaConsulta.TryDequeue(out idEmpresa))
                        {
                            Servicos.Log.TratarErro("Task parou a execução");
                            break;
                        }
                    }
                    catch (TaskCanceledException abort)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de integração da aleta cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de integração da alerta cancelada: ", abortThread.ToString()));
                        break;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        System.Threading.Thread.Sleep(Tempo);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task de integrações à fila.");
        }
        private bool ValidarIntegracao()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unidadeDeTrabalho);
            var ConfiguracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            return ConfiguracaoEmbarcador.PossuiMonitoramento;
        }
        private void EnviarEmailNotificacao(string email, string mensagem, string assunto, string stringConexao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (string.IsNullOrEmpty(email))
                return;

            try
            {
                Servicos.Email serEmail = new Email(stringConexao);

                serEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, email, "", "", assunto, mensagem, string.Empty, null, "", true, string.Empty, 0, unidadeTrabalho);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

        }
        private void GerarAlertaNotificacao(UnitOfWork unidadeDeTrabalho, string stringConexao, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var repAlertaMonitorNotificacao = new Repositorio.Embarcador.Logistica.AlertaMonitorNotificacao(unidadeDeTrabalho);

            var repAlertaNotificacao = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTratativa(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa> monitoramentoEventoTratativas = repAlertaNotificacao.BuscarTodos();

            var repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertasEmAberto = repAlertaMonitor.BuscarTodosEmAberto();

            var repCargaResponsavel = new Repositorio.Embarcador.Cargas.CargaResponsavel(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.CargaResponsavel> cargasResponsavel = repCargaResponsavel.BuscarTodosResponsaveis();

            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Processando alertas em aberto", this.GetType().Name);

            foreach (var alerta in alertasEmAberto)
            {
                var ultimaNotificacao = alerta.AlertaMonitorNotificacao.LastOrDefault();
                var sequencia = 1;


                if (ultimaNotificacao == null)
                {

                    // Inicializa controle de notificcao
                    var novaNotificacao = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao
                    {
                        Data = DateTime.Now,
                        Sequencia = 0,
                        AlertaMonitor = alerta
                    };

                    repAlertaMonitorNotificacao.Inserir(novaNotificacao);
                    continue;
                }

                DateTime dataUltimaNotificacao = DateTime.Now;
                if (ultimaNotificacao != null)
                {
                    sequencia = ultimaNotificacao.Sequencia + 1;
                    dataUltimaNotificacao = ultimaNotificacao.Data;
                }

                var monitoramentoEventoTratativa = (from conf in monitoramentoEventoTratativas
                                                    where conf.Sequencia == sequencia && conf.MonitoramentoEvento.TipoAlerta == alerta.TipoAlerta
                                                    select conf).FirstOrDefault();


                if (monitoramentoEventoTratativa != null)
                {
                    if ((!monitoramentoEventoTratativa.EnvioEmail) && (!monitoramentoEventoTratativa.EnvioEmailTransportador))
                        continue;

                    var tempoNotificacao = DateTime.Now > dataUltimaNotificacao.AddMinutes(monitoramentoEventoTratativa.TempoEmMinutos);

                    if (tempoNotificacao)
                    {
                        List<Dominio.Entidades.Usuario> funcionarios = cargasResponsavel.Where(cr => cr.CategoriaResponsavel.Codigo == monitoramentoEventoTratativa.CategoriaResponsavel.Codigo && cr.Filiais
                                                                      .Where(crf => crf.Codigo == alerta?.Carga?.Filial.Codigo).Any()).Select(o => o?.Funcionario).Distinct().ToList();

                        string assunto = "Alerta " + alerta.Descricao;
                        string mensagem = $@"Veiculo: {alerta?.Veiculo?.Placa}  
                                                 Alerta: {alerta.Codigo} - {alerta.Descricao} 
                                                 Data: {alerta.Data.ToString()} 
                                                 Sequencia: {sequencia}   
                                                 Valor: {alerta?.ValorAlerta}";

                        foreach (var funcionario in funcionarios)
                        {
                            if (funcionario != null)
                            {
                                EnviarEmailNotificacao(funcionario?.Email, mensagem, assunto, stringConexao, unidadeDeTrabalho);

                                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Envio email  alerta {alerta.Codigo}  {alerta.Descricao} - {sequencia}  para {funcionario?.Email} ", this.GetType().Name);
                            }
                        }

                        if (monitoramentoEventoTratativa.EnvioEmailTransportador)
                        {
                            EnviarEmailNotificacao(alerta?.Carga?.Empresa.Email, mensagem, assunto, stringConexao, unidadeDeTrabalho);
                            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Envio email alerta para transportador {alerta.Codigo}  {alerta.Descricao} - {sequencia}  para {alerta?.Carga?.Empresa.Email} ", this.GetType().Name);
                        }

                        var novaNotificacao = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitorNotificacao
                        {
                            Data = DateTime.Now,
                            Sequencia = sequencia,
                            AlertaMonitor = alerta
                        };

                        repAlertaMonitorNotificacao.Inserir(novaNotificacao);
                    }

                }

            }

        }
    }

}

