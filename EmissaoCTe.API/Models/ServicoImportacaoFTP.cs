using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;


namespace EmissaoCTe.API
{
    public class ServicoImportacaoFTP
    {
        private int Tempo = 300000; //5 minutos

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaConsultaArquivosFTP;
        private static ServicoImportacaoFTP Instance;

        public static ServicoImportacaoFTP GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoImportacaoFTP();

            return Instance;
        }

        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaConsultaArquivosFTP == null)
                ListaConsultaArquivosFTP = new ConcurrentQueue<int>();

            if (!ListaTasks.ContainsKey(idEmpresa))
            {
                this.IniciarThread(idEmpresa, stringConexao);
            }
        }

        private void IniciarThread(int idEmpresa, string stringConexao)
        {
            var filaConsulta = new ConcurrentQueue<int>();

            filaConsulta.Enqueue(idEmpresa);

            Task task = new Task(() =>
            {
#if DEBUG
                Tempo = 3000;
#endif

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            verificarFTP(unidadeDeTrabalho, stringConexao);
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(Tempo);

                        if (!filaConsulta.TryDequeue(out idEmpresa))
                        {
                            Servicos.Log.TratarErro("Task FTP parou a execução", "ImportacaoFTP");
                            break;
                        }

                    }
                    catch (TaskCanceledException abort)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task FTP de consulta de objetos cancelada: ", abort.ToString()), "ImportacaoFTP");
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread FTP de consulta de objetos cancelada: ", abortThread), "ImportacaoFTP");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "ImportacaoFTP");
                        System.Threading.Thread.Sleep(Tempo);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task FTP à fila.", "ImportacaoFTP");
        }

        private void verificarFTP(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            string diretorioDownload = System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivos"];

            if (!string.IsNullOrWhiteSpace(diretorioDownload))
                diretorioDownload = Utilidades.IO.FileStorageService.Storage.Combine(diretorioDownload, "FTP");

            if (!string.IsNullOrWhiteSpace(diretorioDownload))
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConfiguracaoFTP repConfiguracaoFTP = new Repositorio.ConfiguracaoFTP(unitOfWork);

                Servicos.Log.TratarErro("Buscando FTPs configurados", "ImportacaoFTP");
                List<Dominio.Entidades.ConfiguracaoFTP> configuracoesFTP = repConfiguracaoFTP.BuscarTodasImportacao();

                foreach (Dominio.Entidades.ConfiguracaoFTP configFTP in configuracoesFTP)
                {
                    Servicos.Log.TratarErro("Buscando empresa", "ImportacaoFTP");
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorConfiguracao(configFTP.Configuracao.Codigo);
                    if (empresa != null)
                    {
                        string host = configFTP.Host;
                        string usuario = configFTP.Usuario;
                        string senha = configFTP.Senha;
                        string porta = configFTP.Porta;
                        string diretorio = configFTP.Diretorio + (configFTP.Diretorio.EndsWith("/") ? "" : "/");
                        string diretorioImportados = diretorio + (diretorio.EndsWith("/") ? "" : "/") + "Importados/";
                        bool passivo = configFTP.Passivo;
                        bool sFTP = configFTP.Seguro;
                        bool SSL = configFTP.SSL;
                        string erro;

                        Servicos.Log.TratarErro("Testando FTP " + host, "ImportacaoFTP");

                        if (Servicos.FTP.TestarConexao(host, porta, diretorio, usuario, senha, passivo, SSL, out erro, sFTP))
                        {
                            Servicos.Log.TratarErro("Importando FTP " + host, "ImportacaoFTP");

                            // Adiciona na tabela para processamentos posteriores
                            bool usarUTF8 = configFTP.Cliente?.CPF_CNPJ == 19782476000150;

                            List<string> arquivosFTP = Servicos.FTP.DownloadArquivosPasta(host, porta, diretorio, usuario, senha, passivo, SSL, diretorioDownload, out erro, sFTP, true, diretorioImportados, true, true, false, usarUTF8);
                            if (!string.IsNullOrWhiteSpace(erro))
                                Servicos.Log.TratarErro("DownloadArquivosPasta: " + host + " - Erro: " + erro, "ImportacaoFTP");
                            if (arquivosFTP != null && arquivosFTP.Count() > 0)
                            {
                                for (var i = 0; i < arquivosFTP.Count(); i++)
                                {
                                    string caminhoArquivo = arquivosFTP[i];
                                    string arquivo = caminhoArquivo.Split('\\').LastOrDefault();
                                    string extencaoArquivo = System.IO.Path.GetExtension(arquivo).ToLower();
                                    string arquivoOriginal = diretorio + arquivo;

                                    // O nome esta sendo alterado na hora de salvar, entao o arquivo perde o nome original
                                    arquivoOriginal = "";

                                    Servicos.Log.TratarErro("Salvando arquivo " + arquivo, "ImportacaoFTP");

                                    Dominio.Entidades.LayoutEDI layoutEDI = configFTP.Tipo == Dominio.Enumeradores.TipoArquivoFTP.ImportacaoNOTFIS ? configFTP.LayoutEDI : null;
                                    InserirArquivoBaixado(empresa, configFTP, layoutEDI, arquivoOriginal, caminhoArquivo, extencaoArquivo, unitOfWork);
                                }
                            }
                            else
                            {
                                Servicos.Log.TratarErro("DownloadArquivosPasta: " + host + " - Nenhum arquivo retornado.", "ImportacaoFTP");
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Não foi possível conectar ao FTP: " + host + " - " + erro, "ImportacaoFTP");
                        }
                    }
                }
            }

            //string endereco = "sftp.natura.net";
            //string usuario = "trp0254";
            //string senha = "bqoX7XnZ";
            //string porta = "22";
            //string diretorio = "temp/testes";
            //bool passivo = true;
            //bool sFTP = true;

            //string endereco = "ftptranslog-wtool-databases-com-br.umbler.net";
            //string usuario = "translog";
            //string senha = "translog01";
            //string porta = "";
            //string diretorio = "testes";
            //bool passivo = true;
            //bool sFTP = false;
        }

        private void MoverArquivoParaPastaDeErro(string pathFolder, string pathFile)
        {
            string path = Utilidades.IO.FileStorageService.Storage.Combine(pathFolder, "Falhas", DateTime.Now.Date.ToString("dd_MM_yyyy"), Path.GetFileName(pathFile));

            Servicos.Arquivo.Mover(pathFile, path);
        }

        private void MoverArquivoParaPastaDeImportados(string pathFolder, string pathFile)
        {
            string path = Utilidades.IO.FileStorageService.Storage.Combine(pathFolder, "Importados", DateTime.Now.Date.ToString("dd_MM_yyyy"), Path.GetFileName(pathFile));

            Servicos.Arquivo.Mover(pathFile, path);
        }

        private static void InserirArquivoBaixado(Dominio.Entidades.Empresa empresa, Dominio.Entidades.ConfiguracaoFTP configFTP, Dominio.Entidades.LayoutEDI layoutEDI, string arquivoOriginal, string arquivoSalvo, string extencaoArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ImportacaoFTP repImportacaoFTP = new Repositorio.ImportacaoFTP(unitOfWork);

            Dominio.Entidades.ImportacaoFTP importacaoFTP = new Dominio.Entidades.ImportacaoFTP
            {
                Empresa = empresa,
                Cliente = configFTP.Cliente,
                TipoArquivo = configFTP.TipoArquivo,
                LayoutEDI = layoutEDI,
                DataImportacao = DateTime.Now,
                ArquivoOriginal = arquivoOriginal,
                ArquivoSalvo = arquivoSalvo,
                ExtencaoArquivo = extencaoArquivo,
                Status = Dominio.Enumeradores.StatusImportacaoFTP.Salvo,
                UtilizarContratanteComoTomador = configFTP.UtilizarContratanteComoTomador
            };

            repImportacaoFTP.Inserir(importacaoFTP);
        }

    }
}
