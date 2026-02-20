using Servicos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SGT.GerenciadorApp.Monitoramento.Integracao
{
    public class IntegracaoSascar
    {
        Servicos.Sascar.SasIntegraWSClient sascarWSClient;
        private static readonly string Usuario = "TECCHAPECO";
        private static readonly string Senha = "Multi@Sascar2019";
        private static readonly int tempoSleep = 60;
        private static IntegracaoSascar Instance;
        private ConcurrentDictionary<int, Task> ListaTasks;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private IntegracaoPosicao integracaoPosicao;
        private Repositorio.Embarcador.Configuracoes.IntegracaoSascar repConfiguracao;
        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSascar configuracao;
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
                                    InicializarConfiguracao();

                                    InicializarWSSascar();

                                    IntegrarPosicao();
                                    SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Fim\r\n", this.GetType().Name);
                                }

                                unidadeDeTrabalho.Dispose();
                            }
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(tempoSleep * 1000);

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
                        System.Threading.Thread.Sleep(tempoSleep * 1000);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task de integrações à fila.");
        }
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            Servicos.Sascar.pacotePosicao[] respostas = sascarWSClient.obterPacotePosicoes(configuracao.Usuario, configuracao.Senha, 1);

            var listaposicao = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            if (respostas == null)
                return listaposicao;

            foreach (var posicao in respostas)
            {
                string endereco = posicao.cidade + ' ' + posicao.uf;

                if (!string.IsNullOrWhiteSpace(posicao.rua))
                    endereco = posicao.rua + endereco;

                listaposicao.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                {
                    ID = posicao.codigoMacro,
                    Data = posicao.dataPosicao,
                    DataVeiculo = posicao.dataPosicao,
                    Placa = posicao.placa,
                    IDEquipamento = posicao.idVeiculo,
                    Velocidade = posicao.velocidade,
                    Latitude = posicao.latitude,
                    Longitude = posicao.longitude,
                    Ignicao = posicao.ignicao,
                    Temperatura = posicao.temperatura1,
                    Descricao = endereco
                });
            }

            return listaposicao;
        }
        private void IntegrarPosicao()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Integrando posições", this.GetType().Name);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            _unidadeDeTrabalho.Start();
            try
            {
                integracaoPosicao.GravarPosicoes(posicoes);
                AtualizarConfiguracao(posicoes);

                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception excecao)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }
        private void InicializarPosicao()
        {
            integracaoPosicao = new SGT.GerenciadorApp.Monitoramento.Integracao.IntegracaoPosicao(_unidadeDeTrabalho, 0);
        }
        private void InicializarWSSascar()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Inicializando serviço", this.GetType().Name);
            sascarWSClient = new Servicos.Sascar.SasIntegraWSClient();
            InspectorBehavior inspector = new InspectorBehavior();
            sascarWSClient.Endpoint.EndpointBehaviors.Add(inspector);
        }
        private bool ValidarIntegracao()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unidadeDeTrabalho);
            return repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Sascar) != null;
        }
        private void InicializarConfiguracao()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Inicializando configurações", this.GetType().Name);
            repConfiguracao = new Repositorio.Embarcador.Configuracoes.IntegracaoSascar(_unidadeDeTrabalho);

            configuracao = repConfiguracao.Buscar();

            if (configuracao == null)
            {

                configuracao = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSascar
                {
                    Pacote = 1,
                    Usuario = Usuario,
                    Senha = Senha,
                    
                };
                repConfiguracao.Inserir(configuracao);
            }
        }
        private void AtualizarConfiguracao(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            if (posicoes?.Count > 0)
            {
                configuracao.Pacote = posicoes.LastOrDefault().ID;
                repConfiguracao.Atualizar(configuracao);
            }
        }
        public static IntegracaoSascar GetInstance()
        {
            if (Instance == null)
                Instance = new IntegracaoSascar();


            return Instance;
        }
        public void QueueItem(int idEmpresa, string stringConexao, string stringConexaoAdmin, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (!ListaTasks.ContainsKey(idEmpresa))
                this.IniciarThread(idEmpresa, stringConexao, stringConexaoAdmin, tipoServicoMultisoftware);
        }
    }
}