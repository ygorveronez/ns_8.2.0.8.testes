using Newtonsoft.Json;
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
    public class IntegracaoTrafegus
    {
        private string urlWebService;
        private string user;
        private string password;
        private Int64 posicao;

        private static readonly int tempoSleep = 30;
        private static IntegracaoTrafegus Instance;
        private ConcurrentDictionary<int, Task> ListaTasks;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private IntegracaoPosicao integracaoPosicao;
        private Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus;
        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus;
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
                                if (ValidarIntegracao()) {

                                    InicializarConfiguracao();

                                    InicializarPosicao();

                                    IntegrarPosicao();
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

        private void InicializarConfiguracao()
        {
            repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unidadeDeTrabalho);

            configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();

            if (configuracaoIntegracaoTrafegus == null)
            {

                configuracaoIntegracaoTrafegus = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus
                {
                    Posicao = 1,
                    Usuario = "WSWS",
                    Senha = "1010",
                    Url = "http://186.250.92.150:9090/ws_rest/public/api/"
                };
                repConfiguracaoIntegracaoTrafegus.Inserir(configuracaoIntegracaoTrafegus);
            }


            posicao = configuracaoIntegracaoTrafegus.Posicao;
            user = configuracaoIntegracaoTrafegus.Usuario;
            password = configuracaoIntegracaoTrafegus.Senha;

            string url = !String.IsNullOrWhiteSpace(configuracaoIntegracaoTrafegus?.Url) ? configuracaoIntegracaoTrafegus.Url : $"";
            string urlPosicao = $"posicaoVeiculo?IdPosicao={posicao}";

            var barra = url.Substring(url.Length - 1) == "/" ? "" : "/";

            urlWebService = url + barra + urlPosicao;
        }

        private bool ValidarIntegracao()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unidadeDeTrabalho);
            return repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trafegus) != null;
        }
        private string ObterJson()
        {
            try
            {

                WebRequest request = HttpWebRequest.Create(urlWebService);
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes($"{user}:{password}"));
                request.Method = "GET";
                string contents = "";

                using (System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using (var responseStream = resp.GetResponseStream())
                        using (var responseStreamReader = new StreamReader(responseStream))
                        {
                            contents = responseStreamReader.ReadToEnd();
                        }
                    }

                    return contents;
                }

            }
            catch(Exception e)
            {
                Servicos.Log.TratarErro(e);
                return null;
            }
        }
              
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            var resposta = ObterJson();
            var listaposicao = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            if (resposta == null)
                return listaposicao;

            var posicoes = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.PosicaoTrafegus>(resposta);

            if (posicoes?.Posicao == null && resposta.Contains("error"))
                throw  new System.Exception("Erro ao obter posições: "+resposta);


            foreach (var posicao in posicoes?.Posicao)
            {
                listaposicao.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                {
                    ID = posicao.idPosicao,
                    Data = posicao.DataTecnologia,
                    DataVeiculo = posicao.DataBordo,
                    Placa = posicao.Placa,
                    Velocidade = posicao.Velocidade ?? 0,
                    Latitude = posicao.Latitude,
                    Longitude = posicao.Longitude,
                    Ignicao = posicao.Ignicao,
                    Descricao = posicao.DescricaoSistema
                });
            }

            return listaposicao;
        }
        private void InicializarPosicao()
        {
            integracaoPosicao = new SGT.GerenciadorApp.Monitoramento.Integracao.IntegracaoPosicao(_unidadeDeTrabalho, 0);
        }

        private void AtualizarConfiguracaoPosicao(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            if (posicoes?.Count > 0)
            {
                configuracaoIntegracaoTrafegus.Posicao = posicoes.LastOrDefault().ID;
                repConfiguracaoIntegracaoTrafegus.Atualizar(configuracaoIntegracaoTrafegus);
            }
        }
        private void IntegrarPosicao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            _unidadeDeTrabalho.Start();
            try
            {
                integracaoPosicao.GravarPosicoes(posicoes);

                AtualizarConfiguracaoPosicao(posicoes);

                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception excecao)
            {
                _unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }
  
        public static IntegracaoTrafegus GetInstance()
        {
            if (Instance == null)
                Instance = new IntegracaoTrafegus();


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


