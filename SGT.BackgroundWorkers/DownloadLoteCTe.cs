using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]
    public class DownloadLoteCTe : LongRunningProcessBase<DownloadLoteCTe>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            await ProcessarDownloadXMLCTeAsync(unitOfWork);
            ProcessarDownload(unitOfWork);
        }

        private void ProcessarDownload(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.DownloadLoteCTe repositorioDownloadLoteCTe = new Repositorio.Embarcador.CTe.DownloadLoteCTe(unitOfWork);
            Repositorio.Embarcador.CTe.DownloadLoteCTeChave repositorioDownloadLoteCTeChave = new Repositorio.Embarcador.CTe.DownloadLoteCTeChave(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe downloadLoteCTe = repositorioDownloadLoteCTe.BuscarPendente();

            if (downloadLoteCTe == null)
                return;

            int codigoLote = downloadLoteCTe.Codigo;
            List<int> codigosChaves = repositorioDownloadLoteCTeChave.BuscarCodigosPendentesPorLote(codigoLote);

            string caminhoXMLs = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "DownloadLoteCTe", $"{downloadLoteCTe.Codigo}", "XML");

            string caminhoPDFs = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "DownloadLoteCTe", $"{downloadLoteCTe.Codigo}", "PDF");

            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

            for (int i = 0; i < codigosChaves.Count; i++)
            {
                Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave chaveCTe = repositorioDownloadLoteCTeChave.BuscarPorCodigo(codigosChaves[i]);

                string nomeArquivoPDF = string.Empty;
                string nomeArquivoXML = string.Empty;
                try
                {
                    string chave = Utilidades.String.OnlyNumbers(chaveCTe.Chave);
                    if (!Utilidades.Validate.ValidarChave(chave))
                    {
                        chaveCTe.MensagemFalha = "Chave " + chave + " não é valida";
                        chaveCTe.Situacao = SituacaoDownloadLoteCTe.Falha;
                        repositorioDownloadLoteCTeChave.Atualizar(chaveCTe);
                    }
                    else
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(chave);

                        if (cte != null && (cte.Status == "A" || cte.Status == "C" || cte.Status == "Z"))
                        {
                            string nomeArquivo = cte.Chave;

                            if (cte.ModeloDocumentoFiscal.Numero != "57")
                                nomeArquivo = cte.ModeloDocumentoFiscal.Numero + "_" + cte.Numero + "_" + cte.Serie.Numero + "_" + cte.ModeloDocumentoFiscal.Abreviacao + "_" + cte.Codigo;

                            string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, cte.Empresa.CNPJ, nomeArquivo) + ".pdf";

                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                            {
                                byte[] dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);

                                nomeArquivoPDF = cte.Chave + ".pdf";

                                if (!Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(caminhoPDFs, nomeArquivoPDF)))
                                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(Utilidades.IO.FileStorageService.Storage.Combine(caminhoPDFs, nomeArquivoPDF), dacte);
                            }

                            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
                            byte[] arquivo = serCTe.ObterXMLCancelamentoAutorizacao(cte, Dominio.Enumeradores.TipoXMLCTe.Autorizacao, unitOfWork);
                            if (arquivo != null)
                            {
                                nomeArquivoXML = cte.Chave + ".xml";
                                if (!Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(caminhoXMLs, nomeArquivoXML)))
                                    Utilidades.IO.FileStorageService.Storage.WriteAllText(Utilidades.IO.FileStorageService.Storage.Combine(caminhoXMLs, nomeArquivoXML), Encoding.Default.GetString(arquivo));
                            }

                            if (cte.Status == "C")
                            {
                                arquivo = serCTe.ObterXMLCancelamentoAutorizacao(cte, Dominio.Enumeradores.TipoXMLCTe.Cancelamento, unitOfWork);
                                if (arquivo != null)
                                {
                                    nomeArquivoXML = cte.Chave + "_procCancCTe.xml";
                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(caminhoXMLs, nomeArquivoXML)))
                                        Utilidades.IO.FileStorageService.Storage.WriteAllText(Utilidades.IO.FileStorageService.Storage.Combine(caminhoXMLs, nomeArquivoXML), Encoding.Default.GetString(arquivo));
                                }
                            }

                            chaveCTe.MensagemFalha = string.Empty;
                            chaveCTe.Situacao = SituacaoDownloadLoteCTe.Finalizado;
                            repositorioDownloadLoteCTeChave.Atualizar(chaveCTe);

                        }
                        else
                        {
                            chaveCTe.MensagemFalha = "CTe não localizado";
                            chaveCTe.Situacao = SituacaoDownloadLoteCTe.Falha;
                            repositorioDownloadLoteCTeChave.Atualizar(chaveCTe);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);

                    if (string.IsNullOrWhiteSpace(nomeArquivoXML) || string.IsNullOrWhiteSpace(nomeArquivoPDF) || !Utilidades.IO.FileStorageService.Storage.Exists(caminhoXMLs + nomeArquivoXML) || !Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDFs + nomeArquivoPDF))
                    {
                        chaveCTe.MensagemFalha = "Não foi possível baixar arquivos";
                        chaveCTe.Situacao = SituacaoDownloadLoteCTe.Falha;
                        repositorioDownloadLoteCTeChave.Atualizar(chaveCTe);
                    }
                    else
                    {
                        chaveCTe.MensagemFalha = string.Empty;
                        chaveCTe.Situacao = SituacaoDownloadLoteCTe.Finalizado;
                        repositorioDownloadLoteCTeChave.Atualizar(chaveCTe);
                    }
                }

                unitOfWork.FlushAndClear();
            }

            VerificarFinalizacao(codigoLote, unitOfWork);
        }

        private void VerificarFinalizacao(int codigoDownloadLoteCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.DownloadLoteCTe repositorioDownloadLoteCTe = new Repositorio.Embarcador.CTe.DownloadLoteCTe(unitOfWork);
            Repositorio.Embarcador.CTe.DownloadLoteCTeChave repositorioDownloadLoteCTeChave = new Repositorio.Embarcador.CTe.DownloadLoteCTeChave(unitOfWork);

            Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe downloadLoteCTe = repositorioDownloadLoteCTe.BuscarPorCodigo(codigoDownloadLoteCTe);

            int naoPendentes = repositorioDownloadLoteCTeChave.ContarTodosNaoPendentes(downloadLoteCTe.Codigo);
            int totalCTes = repositorioDownloadLoteCTeChave.ContarConsulta(downloadLoteCTe.Codigo);
            int falhas = repositorioDownloadLoteCTeChave.ContarFalhas(downloadLoteCTe.Codigo);

            if (naoPendentes == totalCTes)
            {
                downloadLoteCTe.Situacao = SituacaoDownloadLoteCTe.Finalizado;
                downloadLoteCTe.DataTermino = DateTime.Now;

                if (falhas > 0)
                    downloadLoteCTe.MensagemFalha = $"{falhas} CT-es do total de {totalCTes} falharam.";
                else
                    downloadLoteCTe.MensagemFalha = $"Todos os CT-es foram processados";

                repositorioDownloadLoteCTe.Atualizar(downloadLoteCTe);
            }
        }

        private async Task ProcessarDownloadXMLCTeAsync(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.EfetuarDownloadXMLCTe);
            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

            List<int> codigosCTe = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCTe.BuscarCTesSemXML(limiteRegistros));

            List<Task> tasks = new List<Task>();

            foreach (int codigo in codigosCTe)
                tasks.Add(Task.Run(() => servicoCTe.EfetuarDownloadXMLCTe(codigo, _auditado, _tipoServicoMultisoftware)));

            await Task.WhenAll(tasks);
        }

    }
}
