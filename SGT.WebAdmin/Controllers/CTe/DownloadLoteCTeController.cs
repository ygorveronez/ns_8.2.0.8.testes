using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Entidades.Embarcador.CTe;
using ICSharpCode.SharpZipLib.Zip;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/DownloadLoteCTe")]
    public class DownloadLoteCTeController : BaseController
    {
        #region Construtores

        public DownloadLoteCTeController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoCTe(unitOfWork);

            unitOfWork.Dispose();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);;
                string dados = Request.Params("Dados");
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = serCTe.ImportarCTesLote(Auditado, dados, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                List<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe> lotesCTe;
                int totalRegistros = 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("SituacaoCodigo", false);
                grid.AdicionarCabecalho("Lote N°", "Codigo", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Solicitação", "DataSolicitacao", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Termino", "DataTermino", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Mensagem", "MensagemFalha", 12, Models.Grid.Align.center, true);

                ExecutarPesquisa(out totalRegistros, out lotesCTe, "Codigo", "asc", grid.inicio, grid.limite, unitOfWork);
                grid.setarQuantidadeTotal(totalRegistros);

                var lista = (from lote in lotesCTe
                             select new
                             {
                                 lote.Codigo,
                                 DataSolicitacao = lote.DataSolicitacao.ToString("dd/MM/yyyy HH:mm"),
                                 lote.MensagemFalha,
                                 Situacao = lote.Situacao.ObterDescricao(),
                                 SituacaoCodigo = (int)lote.Situacao,
                                 DataTermino = lote.Situacao != SituacaoDownloadLoteCTe.Pendente ? lote.DataTermino?.ToString("dd/MM/yyyy HH:mm") : "",
                                 DT_RowColor = lote.Situacao.ObterCorLinha()
                             });

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaCTesLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave> ctes;
                int totalRegistros = 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Chave", "Chave", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Mensagem Falha", "MensagemFalha", 12, Models.Grid.Align.left, true);

                ExecutarPesquisaChaves(out totalRegistros, out ctes, unitOfWork);
                grid.setarQuantidadeTotal(totalRegistros);

                var lista = (from cte in ctes
                             select new
                             {
                                 cte.Codigo,
                                 cte.Chave,
                                 Situacao = cte.Situacao.ObterDescricao(),
                                 cte.MensagemFalha,
                                 DT_RowColor = cte.Situacao.ObterCorLinha()
                             });

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.CTe.DownloadLoteCTe repositorioDownloadLoteCTe = new Repositorio.Embarcador.CTe.DownloadLoteCTe(unitOfWork);
                Repositorio.Embarcador.CTe.DownloadLoteCTeChave repositorioDownloadLoteCTeChave = new Repositorio.Embarcador.CTe.DownloadLoteCTeChave(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe downloadLoteCTe = repositorioDownloadLoteCTe.BuscarPorCodigo(codigo, true);

                if (downloadLoteCTe == null)
                    return new JsonpResult(false, "Lote não encontrado.");

                if (downloadLoteCTe.Situacao != SituacaoDownloadLoteCTe.Pendente)
                    return new JsonpResult(false, "O Lote não está pendente.");

                downloadLoteCTe.Situacao = SituacaoDownloadLoteCTe.Cancelado;
                repositorioDownloadLoteCTe.Atualizar(downloadLoteCTe);

                List<DownloadLoteCTeChave> listaChaves = repositorioDownloadLoteCTeChave.Consultar(codigo);

                foreach (DownloadLoteCTeChave cteChave in listaChaves)
                {
                    cteChave.Situacao = SituacaoDownloadLoteCTe.Cancelado;
                    repositorioDownloadLoteCTeChave.Atualizar(cteChave);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.CTe.DownloadLoteCTe repositorioDownloadLoteCTe = new Repositorio.Embarcador.CTe.DownloadLoteCTe(unitOfWork);
                Repositorio.Embarcador.CTe.DownloadLoteCTeChave repositorioDownloadLoteCTeChave = new Repositorio.Embarcador.CTe.DownloadLoteCTeChave(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe downloadLoteCTe = repositorioDownloadLoteCTe.BuscarPorCodigo(codigo, true);

                if (downloadLoteCTe == null)
                    return new JsonpResult(false, "Lote não encontrado.");

                if (downloadLoteCTe.Situacao != SituacaoDownloadLoteCTe.Cancelado && downloadLoteCTe.Situacao != SituacaoDownloadLoteCTe.Falha)
                    return new JsonpResult(false, "O Lote não está cancelado ou com falha.");

                downloadLoteCTe.Situacao = SituacaoDownloadLoteCTe.Pendente;
                downloadLoteCTe.DataSolicitacao = System.DateTime.Now;
                repositorioDownloadLoteCTe.Atualizar(downloadLoteCTe);

                List<DownloadLoteCTeChave> listaChaves = repositorioDownloadLoteCTeChave.Consultar(codigo);

                foreach (DownloadLoteCTeChave cteChave in listaChaves)
                {
                    cteChave.Situacao = SituacaoDownloadLoteCTe.Pendente;
                    repositorioDownloadLoteCTeChave.Atualizar(cteChave);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadLoteXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "DownloadLoteCTe", $"{codigo}", "XML");

                Repositorio.Embarcador.CTe.DownloadLoteCTeChave repositorioDownloadLoteCTeChave = new Repositorio.Embarcador.CTe.DownloadLoteCTeChave(unitOfWork);
                List<DownloadLoteCTeChave> listaChaves = repositorioDownloadLoteCTeChave.Consultar(codigo);

                MemoryStream fZip = new MemoryStream();
                ZipOutputStream zipOStream = new ZipOutputStream(fZip);
                zipOStream.SetLevel(9);


                foreach (DownloadLoteCTeChave CTechave in listaChaves)
                {
                    string caminhoXML = Utilidades.IO.FileStorageService.Storage.Combine(caminho, CTechave.Chave) + ".xml";

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                    {
                        byte[] dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoXML);

                        string nomeArquivoDownload = CTechave.Chave + ".xml";

                        ZipEntry entry = new ZipEntry(nomeArquivoDownload);
                        entry.DateTime = System.DateTime.Now;
                        zipOStream.PutNextEntry(entry);
                        zipOStream.Write(dacte, 0, dacte.Length);
                        zipOStream.CloseEntry();
                    }
                }

                zipOStream.IsStreamOwner = false;
                zipOStream.Close();

                fZip.Position = 0;

                return Arquivo(fZip, "application/zip", "XML dos CT-es do Lote N° " + codigo + ".zip");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha no download.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadLotePDF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "DownloadLoteCTe", $"{codigo}", "PDF");

                Repositorio.Embarcador.CTe.DownloadLoteCTeChave repositorioDownloadLoteCTeChave = new Repositorio.Embarcador.CTe.DownloadLoteCTeChave(unitOfWork);
                List<DownloadLoteCTeChave> listaChaves = repositorioDownloadLoteCTeChave.Consultar(codigo);

                MemoryStream fZip = new MemoryStream();
                ZipOutputStream zipOStream = new ZipOutputStream(fZip);
                zipOStream.SetLevel(9);


                foreach (DownloadLoteCTeChave CTechave in listaChaves)
                {
                    string caminhoXML = Utilidades.IO.FileStorageService.Storage.Combine(caminho, CTechave.Chave) + ".pdf";

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                    {
                        byte[] dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoXML);

                        string nomeArquivoDownload = CTechave.Chave + ".pdf";

                        ZipEntry entry = new ZipEntry(nomeArquivoDownload);
                        entry.DateTime = System.DateTime.Now;
                        zipOStream.PutNextEntry(entry);
                        zipOStream.Write(dacte, 0, dacte.Length);
                        zipOStream.CloseEntry();
                    }
                }

                zipOStream.IsStreamOwner = false;
                zipOStream.Close();

                fZip.Position = 0;

                return Arquivo(fZip, "application/zip", "PDF dos CT-es do Lote N° " + codigo + ".zip");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha no download.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void ExecutarPesquisa(out int totalRegistros, out List<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe> lotesCTe, string indiceColunaOrdena, string gridOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.DownloadLoteCTe repositorioDownloadLoteCTe = new Repositorio.Embarcador.CTe.DownloadLoteCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaLoteCTe filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaLoteCTe()
            {
                Situacao = Request.GetEnumParam<SituacaoDownloadLoteCTe>("Situacao"),
                DataSolicitacao = Request.GetNullableDateTimeParam("DataImportacaoInicial"),
                DataTermino = Request.GetNullableDateTimeParam("DataImportacaoFinal")
            };

            lotesCTe = repositorioDownloadLoteCTe.Consultar(filtrosPesquisa, indiceColunaOrdena, gridOrdena, inicio, limite);
            totalRegistros = repositorioDownloadLoteCTe.ContarConsulta(filtrosPesquisa);
        }

        private void ExecutarPesquisaChaves(out int totalRegistros, out List<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave> ctes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.DownloadLoteCTeChave repositorioDownloadLoteCTeChave = new Repositorio.Embarcador.CTe.DownloadLoteCTeChave(unitOfWork);

            int codigo = Request.GetIntParam("Codigo");

            ctes = repositorioDownloadLoteCTeChave.Consultar(codigo);
            totalRegistros = repositorioDownloadLoteCTeChave.ContarConsulta(codigo);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoCTe(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Chaves CTe", Propriedade = "Chave", Tamanho = 100 },
            };

            return configuracoes;
        }

        #endregion
    }
}
