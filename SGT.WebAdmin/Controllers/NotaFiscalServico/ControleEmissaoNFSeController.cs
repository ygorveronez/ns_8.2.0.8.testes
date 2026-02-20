using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscalServico
{
    [CustomAuthorize("NotaFiscalServico/ControleEmissaoNFSe")]
    public class ControleEmissaoNFSeController : BaseController
    {
		#region Construtores

		public ControleEmissaoNFSeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reemitir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigo);

                // Valida
                if (nfse == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (nfse.Empresa.Status != "A")
                    return new JsonpResult(false, true, "Empresa não está ativa para emissão de NFS-e.");

                if (nfse.Empresa.StatusFinanceiro == "B")
                    return new JsonpResult(false, true, "Empresa está com pendências, contate o setor de cadastros para maiores informações.");

                // Inicia transacao
                if (!EmitirNFSe(nfse, unitOfWork, out string msg))
                    return new JsonpResult(false, true, msg);

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDANFSE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);

                int.TryParse(Request.Params("NFSe"), out int codigoNFSe);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return new JsonpResult(true, false, "NFS-e não encontrada, atualize a página e tente novamente.");

                if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Autorizado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.Cancelado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.EmCancelamento
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.NFSeManualGerada)
                    return new JsonpResult(true, false, "O status da NFS-e não permite a geração do DANFSE.");

                byte[] danfse = null;

                if (nfse.Status == Dominio.Enumeradores.StatusNFSe.NFSeManualGerada)
                {
                    Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamento = repLancamentoNFSManual.BuscarPorNFSe(nfse.Codigo);
                    if (lancamento != null && !string.IsNullOrWhiteSpace(lancamento.DadosNFS.ImagemNFS))
                        danfse = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(lancamento.DadosNFS.ImagemNFS);
                }
                else
                {
                    danfse = svcNFSe.ObterDANFSE(nfse.Codigo);
                }

                if (danfse != null)
                    return Arquivo(danfse, "application/pdf", "NFSe_" + nfse.Numero.ToString() + ".pdf");
                else
                    return new JsonpResult(true, false, "Não foi possível gerar o DANFSE, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do DANFSE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);

                int.TryParse(Request.Params("NFSe"), out int codigoNFSe);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return new JsonpResult(true, false, "NFS-e não encontrada, atualize a página e tente novamente.");

                if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Autorizado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.Cancelado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.EmCancelamento
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.NFSeManualGerada)
                    return new JsonpResult(true, false, "O status da NFS-e não permite a geração do XML.");

                byte[] xml = null;

                if (nfse.Status == Dominio.Enumeradores.StatusNFSe.NFSeManualGerada)
                {
                    Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamento = repLancamentoNFSManual.BuscarPorNFSe(nfse.Codigo);
                    if (lancamento != null && !string.IsNullOrWhiteSpace(lancamento.DadosNFS.XML))
                        xml = System.Text.Encoding.Default.GetBytes(lancamento.DadosNFS.XML);
                }
                else
                {
                    xml = svcNFSe.ObterXML(codigoNFSe, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao);
                }

                if (xml != null)
                    return Arquivo(xml, "text/xml", string.Concat("NFSe_", nfse.Numero, ".xml"));

                return new JsonpResult(true, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(true, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            int numeroInicial = 0;
            int numeroFinal = 0;
            int serie = 0;
            int.TryParse(Request.Params("Empresa"), out int empresa);
            int.TryParse(Request.Params("NumeroCarga"), out int numeroCarga);

            List<int> series = new List<int>();
            Dominio.Enumeradores.StatusNFSe? status = null;
            if (Enum.TryParse<Dominio.Enumeradores.StatusNFSe>(Request.Params("Status"), out Dominio.Enumeradores.StatusNFSe statusAux))
                status = statusAux;
            string cnpjTomador = "";

            List<Dominio.Entidades.NFSe> listaGrid = repNFSe.Consultar(empresa, dataInicial, dataFinal, numeroInicial, numeroFinal, serie, status, series, numeroCarga, 0, "", cnpjTomador, inicio, limite);
            totalRegistros = repNFSe.ContarConsulta(empresa, dataInicial, dataFinal, numeroInicial, numeroFinal, serie, status, series, numeroCarga, 0, "", cnpjTomador);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Status,
                            obj.Numero,
                            DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                            Serie = obj.Serie.Numero,
                            Empresa = obj.Empresa?.RazaoSocial ?? string.Empty,
                            Tomador = obj.Tomador != null ? (obj.Tomador.Exterior ? obj.Tomador.NumeroDocumentoExterior : obj.Tomador.CPF_CNPJ_Formatado) + " - " + obj.Tomador.Nome : string.Empty,
                            LocalidadePrestacao = obj.LocalidadePrestacaoServico != null ? obj.LocalidadePrestacaoServico.Estado.Sigla + " / " + obj.LocalidadePrestacaoServico.Descricao : string.Empty,
                            Valor = obj.ValorServicos.ToString("n2"),
                            DescricaoStatus = !string.IsNullOrWhiteSpace(obj.DescricaoStatus) ? obj.DescricaoStatus : string.Empty,
                            MensagemRetorno = obj.RPS != null && !string.IsNullOrWhiteSpace(obj.RPS.MensagemRetorno) ? System.Web.HttpUtility.HtmlEncode(Utilidades.String.ReplaceInvalidCharacters(obj.RPS.MensagemRetorno)) : string.Empty
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "Empresa") propOrdenar = "Empresa.RazaoSocial";
            else if (propOrdenar == "EstadoCarregamento") propOrdenar = "EstadoCarregamento.Nome";
            else if (propOrdenar == "EstadoDescarregamento") propOrdenar = "EstadoDescarregamento.Nome";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Status", false);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Série", "Serie", 6, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Tomador", "Tomador", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Loc. Prest.", "LocalidadePrestacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "MensagemRetorno", 21, Models.Grid.Align.left, true);

            return grid;

        }

        private bool EmitirNFSe(Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unitOfWork, out string msg)
        { 
            msg = "";

            Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            Dominio.Enumeradores.StatusNFSe statusAnterior = nfse.Status;

            if (nfse.Status == Dominio.Enumeradores.StatusNFSe.EmDigitacao || nfse.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao)
            {
                unitOfWork.Start();

                if (svcNFSe.Emitir(nfse, unitOfWork))
                {
                    nfse.Status = Dominio.Enumeradores.StatusNFSe.Enviado;
                    repNFSe.Atualizar(nfse);

                    if (statusAnterior == Dominio.Enumeradores.StatusNFSe.Rejeicao)
                    {
                        Repositorio.Embarcador.Cargas.CargaNFS repCargaNFS = new Repositorio.Embarcador.Cargas.CargaNFS(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.CargaNFS cargaNFS = repCargaNFS.BuscarPorCodigoNFSe(nfse.Codigo);
                        if (cargaNFS != null)
                        {
                            if (cargaNFS.Carga.PossuiPendencia)
                            {
                                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                                cargaNFS.Carga.PossuiPendencia = false;
                                cargaNFS.Carga.problemaNFS = false;
                                cargaNFS.Carga.MotivoPendencia = "";
                                repCarga.Atualizar(cargaNFS.Carga);
                            }
                        }
                    }
                    unitOfWork.CommitChanges();
                    svcNFSe.AdicionarNFSeNaFilaDeConsulta(nfse, unitOfWork);
                }
                else
                {
                    unitOfWork.Rollback();

                    msg = "A NFS-e foi salva, mas ocorreu uma falha ao emitir. Atualize a página e tente novamente.";
                    return false;
                }
            }
            else
            {
                msg = "Status da NFS-e não permite emitir novamente.";
                return false;
            }

            return true;
        }
        #endregion
    }
}
