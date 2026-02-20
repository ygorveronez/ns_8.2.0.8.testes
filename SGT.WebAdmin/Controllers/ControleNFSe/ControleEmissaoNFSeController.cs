using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ControleNFSe
{
    [CustomAuthorize("ControleNFSe/ControleEmissaoNFSe")]
    public class ControleEmissaoNFSeController : BaseController
    {
		#region Construtores

		public ControleEmissaoNFSeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                //PropOrdena(ref propOrdenar);

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

        public async Task<IActionResult> ReemitirNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigo, true);

                // Valida
                if (nfse == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                if (nfse.Empresa.Status != "A")
                    return new JsonpResult(false, true, "Empresa não está ativa para emissão de NFSe.");

                if (nfse.Empresa.StatusFinanceiro == "B")
                    return new JsonpResult(false, true, "Empresa está com pendências, contate o setor de cadastros para maiores informações.");

                // Inicia transacao
                unitOfWork.Start();
                string retorno = EmitirNFSe(nfse, unitOfWork);
                unitOfWork.CommitChanges();

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
        public async Task<IActionResult> DownloadPDFNFSe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeTrabalho);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unidadeTrabalho);
                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeTrabalho);

                int.TryParse(Request.Params("NFSe"), out int codigoNFSe);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return new JsonpResult(false, false, "NFS-e não encontrada, atualize a página e tente novamente.");

                if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Autorizado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.Cancelado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.EmCancelamento
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.NFSeManualGerada)
                    return new JsonpResult(false, false, "O status da NFS-e não permite a geração do DANFSE.");

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
                    return new JsonpResult(false, false, "Não foi possível gerar o PDF, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do PDF.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLNFSe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeTrabalho);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unidadeTrabalho);
                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeTrabalho);

                int.TryParse(Request.Params("NFSe"), out int codigoNFSe);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return new JsonpResult(false, false, "NFS-e não encontrada, atualize a página e tente novamente.");

                if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Autorizado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.Cancelado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.EmCancelamento
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.NFSeManualGerada)
                    return new JsonpResult(false, false, "O status da NFS-e não permite a geração do XML.");

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
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
            Repositorio.RPSNFSe repRetornoXMLNFSe = new Repositorio.RPSNFSe(unitOfWork);

            // Reutilizado o mesmo metodo do consultar, so foi mantido as variaveis para não ficar confuso
            // Mesmo que elas estejam apenas inicializadas
            int serie = 0, numeroInicial = 0, numeroFinal = 0;
            int[] series = new int[] { };

            bool contem = false;

            Dominio.Enumeradores.TipoCTE tipoNFSe = Dominio.Enumeradores.TipoCTE.Todos;

            // Filtros
            int.TryParse(Request.Params("Empresa"), out int empresa);
            int.TryParse(Request.Params("NumeroCarga"), out int numeroCarga);

            Dominio.Enumeradores.StatusNFSe? status = null;
            Dominio.Enumeradores.StatusNFSe statusAux;
            if (Enum.TryParse<Dominio.Enumeradores.StatusNFSe>(Request.Params("Status"), out statusAux))
                status = statusAux;

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);

            numeroInicial = Request.GetIntParam("NumeroInicial");
            numeroFinal = Request.GetIntParam("NumeroFinal");
            serie = Request.GetIntParam("Serie");
            
            unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

            List<Dominio.Entidades.NFSe> listaNFSe = repNFSe.Consultar(empresa, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, serie, status, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.NFSe).Select(o => o.Codigo).ToList(), numeroCarga, 0, "", "",  inicio, 50);
            totalRegistros = repNFSe.ContarConsulta(empresa, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, serie, status, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.NFSe).Select(o => o.Codigo).ToList(), numeroCarga, 0, "", "");

            unitOfWork.CommitChanges();
            
            var lista = from obj in listaNFSe
                        select new
                        {
                            //CodigoCriptografado = Servicos.Criptografia.Criptografar(nfse.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                            obj.Codigo,
                            obj.Numero,
                            Serie = obj.Serie.Numero,
                            Empresa = obj.Empresa != null ? obj.Empresa.RazaoSocial + " (" + obj.Empresa.CNPJ + ")" : string.Empty,
                            DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                            RPS = obj.RPS != null ? obj.RPS.Numero : 0,
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
            else if (propOrdenar == "Remetente") propOrdenar = "Remetente.Nome";
            else if (propOrdenar == "LocalidadeRemetente") propOrdenar = "Remetente.Nome.Cidade";
            else if (propOrdenar == "Destinatario") propOrdenar = "Destinatario.Nome";
            else if (propOrdenar == "LocalidadeDestinatario") propOrdenar = "Destinatario.Nome.Cidade";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Empresa", "Empresa", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tomador", "Tomador", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Loc. Prest.", "LocalidadePrestacao", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor", "Valor", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "MensagemRetorno", 11, Models.Grid.Align.left, true);
            
            return grid;

        }

        private string EmitirNFSe(Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
            Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);

            if (nfse != null)
            {
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

                        mensagem = "A NFS-e foi salva, mas ocorreu uma falha ao emitir. Atualize a página e tente novamente.";
                    }
                }
                else
                    mensagem = "Status da NFS-e não permite emitir novamente.";
            }
            else
                mensagem = "A NFS-e não encontrada para emissão.";

            return mensagem;
        }
        #endregion
    }
}
