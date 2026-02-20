using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Text;


namespace EmissaoCTe.API
{
    public class ServicoEnvioFTP
    {
        private int Tempo = 3600000; //60 MINUTOS

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaConsultaArquivosFTP;
        private static ServicoEnvioFTP Instance;

        public static ServicoEnvioFTP GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoEnvioFTP();

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
                Tempo = 60000; //1 MINITO
#endif

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            EnviarFTP(unidadeDeTrabalho, stringConexao);
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(Tempo);

                        if (!filaConsulta.TryDequeue(out idEmpresa))
                        {
                            Servicos.Log.TratarErro("Task FTP parou a execução", "EnvioFTP");
                            break;
                        }

                    }
                    catch (TaskCanceledException abort)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task FTP de consulta de objetos cancelada: ", abort.ToString()), "EnvioFTP");
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread FTP de consulta de objetos cancelada: ", abortThread), "EnvioFTP");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "EnvioFTP");
                        System.Threading.Thread.Sleep(Tempo);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task FTP à fila.", "EnvioFTP");
        }

        private void EnviarFTP(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ConfiguracaoFTP repConfiguracaoFTP = new Repositorio.ConfiguracaoFTP(unitOfWork);
            Repositorio.EnvioFTP repEnvioFTP = new Repositorio.EnvioFTP(unitOfWork);

            List<Dominio.Entidades.ConfiguracaoFTP> configuracoesFTP = repConfiguracaoFTP.BuscarTodasEnvio();

            foreach (Dominio.Entidades.ConfiguracaoFTP configFTP in configuracoesFTP)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorConfiguracao(configFTP.Configuracao.Codigo);
                if (empresa != null)
                {
                    string host = configFTP.Host;
                    string usuario = configFTP.Usuario;
                    string senha = configFTP.Senha;
                    string porta = configFTP.Porta;
                    string diretorio = configFTP.Diretorio + (configFTP.Diretorio.EndsWith("/") ? "" : "/");
                    bool passivo = configFTP.Passivo;
                    bool sFTP = configFTP.Seguro;
                    bool SSL = configFTP.SSL;
                    string erro;

                    if (Servicos.FTP.TestarConexao(host, porta, diretorio, usuario, senha, passivo, SSL, out erro, sFTP))
                    {
                        DateTime dataInicio = repEnvioFTP.BuscarUltimoEnvio(empresa.Codigo, configFTP.Cliente.CPF_CNPJ, configFTP.Tipo);
                        if (dataInicio <= DateTime.MinValue)
                            dataInicio = DateTime.Now.AddDays(-1);

                        Dominio.Entidades.EnvioFTP envioFTP = new Dominio.Entidades.EnvioFTP();
                        envioFTP.Empresa = empresa;
                        if (configFTP.Cliente != null)
                            envioFTP.Cliente = configFTP.Cliente;
                        envioFTP.DataFiltro = DateTime.Now;
                        envioFTP.Status = Dominio.Enumeradores.StatusEnvioFTP.Pendente;
                        envioFTP.Tipo = configFTP.Tipo;
                        if (configFTP.LayoutEDI != null)
                            envioFTP.LayoutEDI = configFTP.LayoutEDI;
                        repEnvioFTP.Inserir(envioFTP);

                        string mensagemEnvio = string.Empty;

                        if (EnviarArquivosFTP(empresa, configFTP, dataInicio, envioFTP.DataFiltro, ref mensagemEnvio, unitOfWork, stringConexao))
                        {
                            envioFTP.Status = Dominio.Enumeradores.StatusEnvioFTP.Sucesso;
                            envioFTP.Mensagem = mensagemEnvio;
                            envioFTP.DataEnvio = DateTime.Now;
                            repEnvioFTP.Atualizar(envioFTP);
                        }
                        else
                        {
                            envioFTP.Status = Dominio.Enumeradores.StatusEnvioFTP.Erro;
                            envioFTP.Mensagem = mensagemEnvio;
                            envioFTP.DataEnvio = DateTime.Now;
                            repEnvioFTP.Atualizar(envioFTP);
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Não foi possível conectar ao FTP: " + host + " - " + erro, "EnvioFTP");
                    }
                }
            }
        }

        private bool EnviarArquivosFTP(Dominio.Entidades.Empresa empresa, Dominio.Entidades.ConfiguracaoFTP config, DateTime dataInicio, DateTime dataFim, ref string mensagemRetorno, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
                Repositorio.OcorrenciaDeCTe repOcorrenciaCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);
                Repositorio.OcorrenciaDeNFSe repOcorrenciaDeNFSe = new Repositorio.OcorrenciaDeNFSe(unitOfWork);
                Repositorio.OcorrenciaDeNFe repOcorrenciaDeNFe = new Repositorio.OcorrenciaDeNFe(unitOfWork);

                Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unitOfWork);
                Servicos.Embarcador.Integracao.EDI.OCOREN svcOCOREN = new Servicos.Embarcador.Integracao.EDI.OCOREN();

                if (config.Tipo == Dominio.Enumeradores.TipoArquivoFTP.EnvioCONEMB)
                {
                    mensagemRetorno = "CONEMB não configurado.";
                    return false;
                    //List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTEs = repCTe.BuscarPorEmpresaRemetenteData(empresa.Codigo, config.Cliente.CPF_CNPJ_SemFormato, dataReferencia);

                    //if (listaCTEs != null && listaCTEs.Count > 0)
                    //{
                    //    for (var i = 0; i < listaCTEs.Count; i++)
                    //    {

                    //    }
                    //    return true;
                    //}
                    //else
                    //{
                    //    mensagemRetorno = "Sem CTes pendentes para envio.";
                    //    return false;
                    //}
                }
                else if (config.Tipo == Dominio.Enumeradores.TipoArquivoFTP.EnvioOCORENCTe)
                {
                    List<Dominio.Entidades.OcorrenciaDeCTe> listaOcorrenciasCTe = repOcorrenciaCTe.BuscarPorEmpresaRemetenteData(empresa.Codigo, config.Cliente.CPF_CNPJ_SemFormato, dataInicio, dataFim);
                    if (listaOcorrenciasCTe == null || listaOcorrenciasCTe.Count == 0)
                        listaOcorrenciasCTe = repOcorrenciaCTe.BuscarPorEmpresaTomadorData(empresa.Codigo, config.Cliente.CPF_CNPJ_SemFormato, dataInicio, dataFim);

                    if (listaOcorrenciasCTe != null && listaOcorrenciasCTe.Count > 0)
                    {
                        svcEDI = new Servicos.GeracaoEDI(unitOfWork, config.LayoutEDI, empresa, new string[] { "A", "S" }, listaOcorrenciasCTe);
                        MemoryStream ediOcorenCTe = svcEDI.GerarArquivo();
                        if (ediOcorenCTe != null)
                        {
                            string nomeArquivo = string.Empty;
                            if (!string.IsNullOrWhiteSpace(config.LayoutEDI.Nomenclatura))
                                nomeArquivo = svcEDI.ObterNomenclaturaLayoutEDI(config.LayoutEDI.Nomenclatura, empresa, null, string.Empty, DateTime.Now);
                            if (string.IsNullOrWhiteSpace(nomeArquivo))
                                nomeArquivo = "OCOREN_" + dataFim.ToString("ddMMyyHHmmss");

                            if (!Servicos.FTP.EnviarArquivo(ediOcorenCTe, nomeArquivo + ".txt", config.Host, config.Porta, config.Diretorio, config.Usuario, config.Senha, config.Passivo, config.SSL, out mensagemRetorno, config.Seguro))
                                return false;
                        }
                        else
                        {
                            mensagemRetorno = "EDI não foi gerado.";
                            return false;
                        }
                    }
                    else
                        mensagemRetorno = "Não possui ocorrências pendentes.";

                    return true;
                }
                else if (config.Tipo == Dominio.Enumeradores.TipoArquivoFTP.EnvioOCORENNFSe)
                {
                    List<Dominio.Entidades.OcorrenciaDeNFSe> listaOcorrenciasNFSe = repOcorrenciaDeNFSe.BuscarPorEmpresaTomadorData(empresa.Codigo, config.Cliente.CPF_CNPJ_SemFormato, dataInicio, dataFim);
                    if (listaOcorrenciasNFSe != null && listaOcorrenciasNFSe.Count > 0)
                    {
                        Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN edi = svcOCOREN.ConverterParaOCOREN(listaOcorrenciasNFSe, unitOfWork);

                        Servicos.GeracaoEDI svcEDINFSe = new Servicos.GeracaoEDI(unitOfWork, config.LayoutEDI, empresa);
                        MemoryStream ediOcorenNFSee = svcEDINFSe.GerarArquivoRecursivo(edi);
                        if (ediOcorenNFSee != null)
                        {
                            string nomeArquivo = string.Empty;
                            if (!string.IsNullOrWhiteSpace(config.LayoutEDI.Nomenclatura))
                                nomeArquivo = svcEDI.ObterNomenclaturaLayoutEDI(config.LayoutEDI.Nomenclatura, empresa, null, string.Empty, DateTime.Now);
                            if (string.IsNullOrWhiteSpace(nomeArquivo))
                                nomeArquivo = "OCOREN_" + dataFim.ToString("ddMMyyHHmmss");

                            if (!Servicos.FTP.EnviarArquivo(ediOcorenNFSee, nomeArquivo + ".txt", config.Host, config.Porta, config.Diretorio, config.Usuario, config.Senha, config.Passivo, config.SSL, out mensagemRetorno, config.Seguro))
                                return false;
                        }
                        else
                        {
                            mensagemRetorno = "EDI não foi gerado.";
                            return false;
                        }
                    }
                    else
                        mensagemRetorno = "Não possui ocorrências pendentes.";

                    return true;
                }
                else if (config.Tipo == Dominio.Enumeradores.TipoArquivoFTP.EnvioOCORENNFe)
                {
                    List<Dominio.Entidades.OcorrenciaDeNFe> listaOcorrenciasNFe = null;
                    if (config.UtilizarContratanteComoTomador)
                        listaOcorrenciasNFe = repOcorrenciaDeNFe.BuscarPorEmpresaContratanteData(empresa.Codigo, config.Cliente.CPF_CNPJ, dataInicio, dataFim);
                    else
                        listaOcorrenciasNFe = repOcorrenciaDeNFe.BuscarPorEmpresaTomadorData(empresa.Codigo, config.Cliente.CPF_CNPJ, dataInicio, dataFim);
                    if (listaOcorrenciasNFe != null && listaOcorrenciasNFe.Count > 0)
                    {
                        Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN edi = svcOCOREN.ConverterParaOCOREN(listaOcorrenciasNFe, unitOfWork);

                        Servicos.GeracaoEDI svcEDINFSe = new Servicos.GeracaoEDI(unitOfWork, config.LayoutEDI, empresa);
                        MemoryStream ediOcorenNFSee = svcEDINFSe.GerarArquivoRecursivo(edi);
                        if (ediOcorenNFSee != null)
                        {
                            string nomeArquivo = string.Empty;
                            if (!string.IsNullOrWhiteSpace(config.LayoutEDI.Nomenclatura))
                                nomeArquivo = svcEDI.ObterNomenclaturaLayoutEDI(config.LayoutEDI.Nomenclatura, empresa, null, string.Empty, DateTime.Now);
                            if (string.IsNullOrWhiteSpace(nomeArquivo))
                                nomeArquivo = "OCOREN_" + dataFim.ToString("ddMMyyHHmmss");

                            if (!Servicos.FTP.EnviarArquivo(ediOcorenNFSee, nomeArquivo + ".txt", config.Host, config.Porta, config.Diretorio, config.Usuario, config.Senha, config.Passivo, config.SSL, out mensagemRetorno, config.Seguro))
                                return false;
                        }
                        else
                        {
                            mensagemRetorno = "EDI não foi gerado.";
                            return false;
                        }
                    }
                    else
                        mensagemRetorno = "Não possui ocorrências pendentes.";

                    return true;
                }
                else if (config.Tipo == Dominio.Enumeradores.TipoArquivoFTP.EnvioXMLCTe)
                {
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTEs = repCTe.BuscarPorEmpresaRemetenteData(empresa.Codigo, config.Cliente.CPF_CNPJ_SemFormato, dataInicio, dataFim);

                    if (listaCTEs != null && listaCTEs.Count > 0)
                    {
                        for (var i = 0; i < listaCTEs.Count; i++)
                        {
                            Dominio.Entidades.XMLCTe xmlCTe = repXMLCTe.BuscarPorCTe(listaCTEs[i].Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                            if (xmlCTe != null && !string.IsNullOrWhiteSpace(xmlCTe.XML))
                            {
                                MemoryStream xml = new MemoryStream(Encoding.UTF8.GetBytes(xmlCTe.XML));

                                if (!Servicos.FTP.EnviarArquivo(xml, listaCTEs[i].Chave + ".xml", config.Host, config.Porta, config.Diretorio, config.Usuario, config.Senha, config.Passivo, config.SSL, out mensagemRetorno, config.Seguro))
                                    return false;
                            }
                        }
                    }
                    else
                        mensagemRetorno = "Sem CTes pendentes para envio.";

                    return true;
                }

                mensagemRetorno = "Tipo não configurado.";
                return false;
            }
            catch (Exception ex)
            {
                mensagemRetorno = ex.Message;
                Servicos.Log.TratarErro(ex, "EnvioFTP");
                return false;
            }
        }
    }
}
