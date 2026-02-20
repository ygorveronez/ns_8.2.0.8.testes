using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Threading;
using System.Configuration;
using System.Reflection;

namespace SGT.GerenciadorApp.Monitoramento
{
    public static class ControleMonitoramento
    {
        //private static string stringConexaoDebug = "Data Source=191.232.235.86;Initial Catalog=ControleCte_Danone;User Id=sa;Password=Multi@2017;"; //Danone
        //private static string stringConexaoDebug = "Data Source=191.238.219.120;Initial Catalog=ControleWalmart1;User Id=sa;Password=Multi@2017;"; //Walmart
        //private static string stringConexaoDebug = "Data Source=192.168.0.125;Initial Catalog=ControleCTe;User Id=sa;Password=Multi@2017;";
        private static readonly string stringConexaoDebug = "";
        private static readonly bool homologacao = false;

        private static readonly int tempoInicioDeViagem = 60;
        private static readonly int tempoInicioDeEntrega = 60;
        private static readonly int tempoFimDeEntrega = 60;
        private static readonly int tempoFimDeViagem = 60;
        private static readonly int tempoVelocidade = 300;
        private static readonly int tempoSensorTemperaturaComProblema = 120;
        private static readonly int tempoTemperatura = 120;
        private static readonly int tempoParadaExcessiva = 120;
        private static readonly int tempoParadaNaoProgramada = 120;
        private static readonly int tempoParadaEmAreaDeRisco = 60;
        private static readonly int tempoDesvioDeRota = 300;
        private static readonly int tempoSemSinal = 60;
        private static readonly int tempoPerdaDeSinal = 60;
        private static readonly int tempoControleDistancia = 300;
        private static readonly int tempoPrevisaoEntrega = 60;
        private static readonly int tempoStatusAtual = 60;
        private static readonly int tempoAtualizarRastreamentoCarga = 300;
        private static readonly int tempoPernoite = 300;
        private static readonly int tempoDirecaoContinuaExcessiva = 300;
        private static void SetThreadMonitoramentoIntegracaoOnixSat()
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            //Somente um cliente por integracao
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);
            var codigoCliente = codigosClientes.FirstOrDefault();

            if (codigoCliente != null)
            {
                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                if (homologacao)
                    stringConexao = !string.IsNullOrWhiteSpace(stringConexaoDebug) ? stringConexaoDebug : App.StringConexaoHomologacao(codigoClienteMultisoftware, unitOfWorkAdmin);

                var TipoServicoMultisoftware = new AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware();
                try
                {
                    SGT.GerenciadorApp.Monitoramento.Integracao.IntegracaoOnixSat.GetInstance().QueueItem(codigoClienteMultisoftware, stringConexao, App.StringConexaoAdmin, TipoServicoMultisoftware);

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "IntegracaoOnixSat");
                }
            }
        }
        private static void SetThreadMonitoramentoIntegracaoSascar()
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            //Somente um cliente por integracao
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);
            var codigoCliente = codigosClientes.FirstOrDefault();

            if (codigoCliente != null)
            {
                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                if (homologacao)
                    stringConexao = !string.IsNullOrWhiteSpace(stringConexaoDebug) ? stringConexaoDebug : App.StringConexaoHomologacao(codigoClienteMultisoftware, unitOfWorkAdmin);

                var TipoServicoMultisoftware = new AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware();
                try
                {
                    SGT.GerenciadorApp.Monitoramento.Integracao.IntegracaoSascar.GetInstance().QueueItem(codigoClienteMultisoftware, stringConexao, App.StringConexaoAdmin, TipoServicoMultisoftware);

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "Integracao Sascar");
                }
            }
        }
        private static void SetThreadMonitoramentoIntegracaoTrafegus()
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            //Somente um cliente por integracao
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);
            var codigoCliente = codigosClientes.FirstOrDefault();

            if (codigoCliente != null)
            {
                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                if (homologacao)
                    stringConexao = !string.IsNullOrWhiteSpace(stringConexaoDebug) ? stringConexaoDebug : App.StringConexaoHomologacao(codigoClienteMultisoftware, unitOfWorkAdmin);


                var TipoServicoMultisoftware = new AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware();
                try
                {
                    SGT.GerenciadorApp.Monitoramento.Integracao.IntegracaoTrafegus.GetInstance().QueueItem(codigoClienteMultisoftware, stringConexao, App.StringConexaoAdmin, TipoServicoMultisoftware);

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "IntegracaoTrafegus");
                }
            }
        }
        private static void SetThreadMonitoramentoIntegracaoAlertas()
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);
            var TipoServicoMultisoftware = new AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware();

            foreach (string codigoCliente in codigosClientes)
            {
                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);
                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                if (homologacao)
                    stringConexao = !string.IsNullOrWhiteSpace(stringConexaoDebug) ? stringConexaoDebug : App.StringConexaoHomologacao(codigoClienteMultisoftware, unitOfWorkAdmin);

                try
                {
                    SGT.GerenciadorApp.Monitoramento.Integracao.IntegracaoAlertaNotificacao.GetInstance().QueueItem(codigoClienteMultisoftware, stringConexao, App.StringConexaoAdmin, TipoServicoMultisoftware);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "IntegracaoAlertas");
                }

            }

        }
        private static void SetThreadMonitoramentoEventos()
        {
            try
            {
                SetTimerInicioDeViagem();

                SetTimerInicioDeEntrega();

                SetTimerFimDeEntrega();

                SetTimerVelocidade();

                SetTimerTemperatura();

                SetTimerSensorTemperaturaComProblema();

                SetTimerPernoite();

                SetTimerParadaExessiva();

                SetTimerParadaNaoProgramada();

                SetTimerParadaEmAreaDeRisco();

                SetTimerDesvioDeRota();

                SetTimerSemSinal();

                SetTimerPerdaDeSinal();

                SetTimerFimDeViagem();

                SetTimerDirecaoContinuaExcessiva();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }
        private static void SetThreadMonitoramentoControles()
        {
            SetTimerStatusAtual();

            SetTimerControleDistancia();

            SetTimerPrevisaoEntregas();

            //SetTimerAtualizarRastreamentoCarga();
        }
        private static void SetTimerVelocidade()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoVelocidade = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoVelocidade), "MonitoramentoEventoVelocidade", tempoVelocidade);
                }
                catch
                {
                    SetTimerVelocidade();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerTemperatura()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoTemperatura = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoTemperatura), "MonitoramentoEventoTemperatura", tempoTemperatura);

                }
                catch
                {
                    SetTimerTemperatura();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerSensorTemperaturaComProblema()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoSensorTemperaturaComProblema = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoSensorTemperaturaComProblema), "MonitoramentoEventoSensorTemperaturaComProblema", tempoSensorTemperaturaComProblema);

                }
                catch
                {
                    SetTimerSensorTemperaturaComProblema();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerParadaExessiva()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoParadaExcessiva = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoParadaExessiva), "MonitoramentoEventoParadaExcessiva", tempoParadaExcessiva);
                }
                catch
                {
                    SetTimerParadaExessiva();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerPernoite()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoPernoite = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoPernoite), "MonitoramentoEventoPernoite", tempoPernoite);
                }
                catch
                {
                    SetTimerPernoite();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerParadaNaoProgramada()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoParadaNaoProgramada = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoParadaNaoProgramada), "MonitoramentoEventoParadaNaoProgramada", tempoParadaNaoProgramada);
                }
                catch
                {
                    SetTimerParadaNaoProgramada();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerParadaEmAreaDeRisco()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoParadaEmAreaDeRisco = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoParadaEmAreaDeRisco), "MonitoramentoEventoParadaEmAreaDeRisco", tempoParadaEmAreaDeRisco);
                }
                catch
                {
                    SetTimerParadaEmAreaDeRisco();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerDesvioDeRota()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoDesvioDeRota = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoDesvioDeRota), "MonitoramentoEventoDesvioDeRota", tempoDesvioDeRota);
                }
                catch
                {
                    SetTimerDesvioDeRota();
                }


            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerSemSinal()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoSemSinal = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoSemSinal), "MonitoramentoEventoSemSinal", tempoSemSinal);
                }
                catch
                {
                    SetTimerSemSinal();
                }


            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerPerdaDeSinal()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoPerdaDeSinal = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoPerdaDeSinal), "MonitoramentoEventoPerdaDeSinal", tempoPerdaDeSinal);
                }
                catch
                {
                    SetTimerPerdaDeSinal();
                }


            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerDirecaoContinuaExcessiva()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoDirecaoContinuaExcessiva = new System.Threading.Timer(x =>
             {
                 try
                 {
                     IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoDirecaoContinuaExcessiva), "MonitoramentoEventoDirecaoContinuaExcessiva", tempoDirecaoContinuaExcessiva);
                 }
                 catch
                 {
                     SetTimerDirecaoContinuaExcessiva();
                 }


             }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerInicioDeViagem()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoInicioDeViagem = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoInicioDeViagem), "MonitoramentoEventoInicioDeViagem", tempoInicioDeViagem);
                }
                catch
                {
                    SetTimerInicioDeViagem();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerInicioDeEntrega()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoInicioDeEntrega = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoInicioDeEntrega), "MonitoramentoEventoInicioDeEntrega", tempoInicioDeEntrega);
                }
                catch
                {
                    SetTimerInicioDeEntrega();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerFimDeEntrega()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoFimDeEntrega = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoFimDeEntrega), "MonitoramentoEventoFimDeEntrega", tempoFimDeEntrega);
                }
                catch
                {

                    SetTimerFimDeEntrega();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerFimDeViagem()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoEventoFimDeViagem = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.EventoFimDeViagem), "MonitoramentoEventoFimDeViagem", tempoFimDeViagem);
                }
                catch
                {

                    SetTimerFimDeViagem();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerControleDistancia()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoMonitoramentoControleDistancia = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.Controle.MonitoramentoControleDistancia), "MonitoramentoControleDistancia", tempoControleDistancia);
                }
                catch
                {
                    SetTimerControleDistancia();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerPrevisaoEntregas()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoMonitoramentoPrevisaoEntrega = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.Controle.MonitoramentoPrevisaoDeEntrega), "MonitoramentoPrevisaoDeEntrega", tempoPrevisaoEntrega);
                }
                catch
                {
                    SetTimerPrevisaoEntregas();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerStatusAtual()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoMonitoramentoStatusAtual = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.Controle.MonitoramentoControleStatusAtual), "StatusAtual", tempoStatusAtual);
                }
                catch
                {
                    SetTimerStatusAtual();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static void SetTimerAtualizarRastreamentoCarga()
        {
            TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

            App.ThreadIntegracaoMonitoramentoStatusAtual = new System.Threading.Timer(x =>
            {
                try
                {
                    IniciarThreadMonitoramento(typeof(SGT.GerenciadorApp.Monitoramento.Controle.MonitoramentoAtualizarRastreamentoCarga), "StatusAtual", tempoAtualizarRastreamentoCarga);
                }
                catch
                {
                    SetTimerAtualizarRastreamentoCarga();
                }

            }, null, timeToGo, Timeout.InfiniteTimeSpan);

        }
        private static bool PossuiMonitoramento(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                var ConfiguracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                return ConfiguracaoEmbarcador.PossuiMonitoramento;
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWork = null;
            }

        }
        private static void IniciarThreadMonitoramento(Type type, string Descricao, int tempoSegundos)
        {

            var tempo = tempoSegundos * 1000;
            if (homologacao)
                tempo = tempoSegundos * 1000;


            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {

                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);
                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                if (homologacao)
                    stringConexao = !string.IsNullOrWhiteSpace(stringConexaoDebug) ? stringConexaoDebug : App.StringConexaoHomologacao(codigoClienteMultisoftware, unitOfWorkAdmin);

                try
                {
                    if (PossuiMonitoramento(stringConexao))
                    {
                        while (true)
                        {
                            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
                            try
                            {
                                object instance = Activator.CreateInstance(type);
                                MethodInfo methodInfo = type.GetMethod("Iniciar");

                                methodInfo.Invoke(instance, new object[] { unitOfWork });
                            }
                            finally
                            {
                                unitOfWork.Dispose();
                                unitOfWork = null;
                                GC.Collect();
                            }
                            System.Threading.Thread.Sleep(tempo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, Descricao);
                    throw;
                }
            }
        }
        public static void IniciarMonitoramento()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Inicio do controle de monitoramento", "ControleMonitoramento");
            SetThreadMonitoramentoIntegracaoOnixSat();
            SetThreadMonitoramentoIntegracaoSascar();
            SetThreadMonitoramentoIntegracaoTrafegus();
            SetThreadMonitoramentoControles();
            SetThreadMonitoramentoEventos();
            SetThreadMonitoramentoIntegracaoAlertas();
        }
    }
}
