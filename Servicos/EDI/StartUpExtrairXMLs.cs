using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Servicos.EDI
{
    public class StartUpExtrairXML : ServicoBase
    {
        public StartUpExtrairXML(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        private int tempoThread = 5000;
        private int codigoFilial = 0;
        private string dataInicioDownload = string.Empty;
        private string dataFimDownload = string.Empty;

        public void Iniciar(string dataInicio, string dataFim, int tamanhoStack)
        {
            Thread thread = new Thread(new ThreadStart(ExecutarThread), tamanhoStack);
            thread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
            thread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            thread.IsBackground = true;
            dataInicioDownload = dataInicio;
            dataFimDownload = dataFim;
            thread.Start();
        }

        private void ExecutarThread()
        {
            Servicos.Log.TratarErro("Iniciou Task");
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(tempoThread);
                    
                    using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        ExtrairXMLs(unidadeDeTrabalho);
                        unidadeDeTrabalho.Dispose();
                    }
                }
                catch (System.ServiceModel.CommunicationException com)
                {
                    Servicos.Log.TratarErro("Comunication: " + com);
                    System.Threading.Thread.Sleep(tempoThread);
                }
                catch (TimeoutException ti)
                {
                    Servicos.Log.TratarErro("Time out: " + ti);
                    System.Threading.Thread.Sleep(tempoThread);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    System.Threading.Thread.Sleep(tempoThread);
                }
            }
        }

        private void ExtrairXMLs(Repositorio.UnitOfWork unitOfWork)
        {
            string StringConexao = unitOfWork.StringConexao;

            DateTime dataInicioFiltro = DateTime.MinValue;
            DateTime dataFimFiltro = DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(dataFimDownload))
                DateTime.TryParseExact(dataFimDownload, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataFimFiltro);

            if (!string.IsNullOrWhiteSpace(dataInicioDownload))
                DateTime.TryParseExact(dataInicioDownload, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataInicioFiltro);

            Servicos.CTe svCTe = new Servicos.CTe(unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

            List<int> listaCTes = null;

            Servicos.Log.TratarErro("Inicio " + dataInicioFiltro.ToString("dd/MM/yyyy") + " até " + dataFimFiltro.ToString("dd/MM/yyyy"), "ExtrairXMLs");

            for (var data = dataInicioFiltro; data <= dataFimFiltro; data = data.AddDays(1))
            {
                listaCTes = repCTe.BuscarCodigosPorStatusEPeriodo(0, new string[] { "A" }, data, data);
                Servicos.Log.TratarErro("Dia: " + data.ToString() + ": CTes: " + listaCTes.Count().ToString(), "ExtrairXMLs");
                if (listaCTes.Count > 0)
                {
                    for (var i = 0; i < listaCTes.Count(); i++)
                    {
                        Dominio.Entidades.XMLCTe xmlCTe = repXMLCTe.BuscarPorCTe(listaCTes[i], Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                        var configuracaoArquivo = Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo();
                        string caminho = configuracaoArquivo.CaminhoDestinoXML;

                        string cnpjTomador = string.Empty;// xmlCTe.CTe.TomadorPagador?.CPF_CNPJ ?? "";
                        if (string.IsNullOrWhiteSpace(cnpjTomador))
                        {
                            if (xmlCTe.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                                cnpjTomador = xmlCTe.CTe.Remetente?.CPF_CNPJ;
                            else if (xmlCTe.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                                cnpjTomador = xmlCTe.CTe.Destinatario?.CPF_CNPJ;
                            else if (xmlCTe.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                                cnpjTomador = xmlCTe.CTe.Recebedor?.CPF_CNPJ;
                            else if (xmlCTe.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                                cnpjTomador = xmlCTe.CTe.Expedidor?.CPF_CNPJ;
                            else if (xmlCTe.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                                cnpjTomador = xmlCTe.CTe.OutrosTomador?.CPF_CNPJ;
                        }

                        if (string.IsNullOrWhiteSpace(cnpjTomador))
                            cnpjTomador = "SemTomador";

                        string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cnpjTomador, xmlCTe.CTe.Chave + ".xml");

                        //Utilidades.IO.FileStorageService.Storage.WriteAllText(arquivo, xmlCTe.XML);
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                        {
                            if (xmlCTe.XMLArmazenadoEmArquivo)
                            {
                                string arquivoOrigem = svCTe.CriarERetornarCaminhoXMLCTe(xmlCTe.CTe, "A", unitOfWork);
                                if (Utilidades.IO.FileStorageService.Storage.Exists(arquivoOrigem))
                                    Utilidades.IO.FileStorageService.Storage.Copy(arquivoOrigem, arquivo);
                                else
                                    Servicos.Log.TratarErro("CTe não localizado " + arquivoOrigem, "ExtrairXMLs");
                            }
                            else if (!string.IsNullOrWhiteSpace(xmlCTe.XML))
                            {
                                Utilidades.IO.FileStorageService.Storage.WriteAllText(arquivo, xmlCTe.XML);
                            }
                            else
                                Servicos.Log.TratarErro("CTe " + xmlCTe.CTe.Chave + " sem XML.", "ExtrairXMLs");
                        }

                        unitOfWork.FlushAndClear();
                    }
                }
                unitOfWork.FlushAndClear();
            }


            Servicos.Log.TratarErro("Finalizou " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "ExtrairXMLs");

        }

    }
}

