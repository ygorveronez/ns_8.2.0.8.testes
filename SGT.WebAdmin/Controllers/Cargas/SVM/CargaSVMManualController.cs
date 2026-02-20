using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas.SVM
{
    [CustomAuthorize(new string[] { "DownloadLoteDACTE", "DownloadLoteXML" }, "Cargas/CargaSVMManual")]
    public class CargaSVMManualController : BaseController
    {
		#region Construtores

		public CargaSVMManualController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.CTe.CTeSVMMultimodal repCTeSVMMultimodal = new Repositorio.Embarcador.CTe.CTeSVMMultimodal(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("Número", "Numero", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Controle", "NumeroControle", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Série", "Serie", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº CTM", "NumeroCTeMultiModal", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Doc.", "DocumentoDescricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("T. Pagamento", "TipoPagamentoDescricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Remetente", "RemetenteDescricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("T. Modal", "TipoModalDescricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("T. Serviço", "TipoServicoDescricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "DestinatarioDescricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", "DestinoDescricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor a Rec.", "ValorReceber", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CST", "CST", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Alíquota", "Aliquota", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "StatusDescricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 40, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int codigoCarga = Request.GetIntParam("Carga");
                int.TryParse(Request.Params("NumeroNF"), out int numeroNF);
                int.TryParse(Request.Params("NumeroDocumento"), out int numeroDocumento);
                string statusCTe = Request.Params("Status");

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "DescricaoTipoServico")
                    propOrdenacao = "TipoServico";

                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                propOrdenacao = "CTeSVM." + propOrdenacao;

                grid.setarQuantidadeTotal(repCTeSVMMultimodal.ContarConsultarSVM(codigoCarga, statusCTe, numeroNF, numeroDocumento));
                List<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal> ctes = repCTeSVMMultimodal.ConsultarSVM(codigoCarga, statusCTe, numeroNF, numeroDocumento, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                var lista = (from obj in ctes
                             select new
                             {
                                 obj.Codigo,
                                 CodigoCTE = obj.CTeSVM.Codigo,
                                 NumeroControle = obj.CTeSVM.NumeroControle,
                                 Numero = obj.CTeSVM.Numero,
                                 Serie = obj.CTeSVM.Serie.Numero,
                                 NumeroCTeMultiModal = obj.CTeMultimodal.Numero,
                                 DocumentoDescricao = obj.CTeSVM.ModeloDocumentoFiscal.Abreviacao,
                                 TipoPagamentoDescricao = obj.CTeSVM.DescricaoTipoPagamento,
                                 RemetenteDescricao = obj.CTeSVM.Remetente?.Descricao ?? obj.CTeSVM.Expedidor?.Descricao,
                                 TipoModalDescricao = obj.CTeSVM.DescricaoTipoModal,
                                 TipoServicoDescricao = obj.CTeSVM.DescricaoTipoServico,
                                 DestinatarioDescricao = obj.CTeSVM.Destinatario?.Descricao ?? obj.CTeSVM.Recebedor?.Descricao,
                                 DestinoDescricao = obj.CTeSVM.LocalidadeTerminoPrestacao.Descricao,
                                 ValorReceber = obj.CTeSVM.ValorAReceber.ToString("n2"),
                                 CST = obj.CTeSVM.CST,
                                 Aliquota = obj.CTeSVM.AliquotaICMS > 0 ? obj.CTeSVM.AliquotaICMS.ToString("n2") : obj.CTeSVM.AliquotaISS.ToString("n4"),
                                 StatusDescricao = obj.CTeSVM.DescricaoStatus,
                                 RetornoSefaz = obj.CTeSVM.MensagemRetornoSefaz
                             }).ToList();
                grid.AdicionaRows(lista);

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

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadLoteDACTE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.CTe.CTeSVMMultimodal repCTeSVMMultimodal = new Repositorio.Embarcador.CTe.CTeSVMMultimodal(unitOfWork);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCTeSVMMultimodal.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga);
                List<int> nfses = repCTeSVMMultimodal.BuscarCodigosNFSePorCarga(codigoCarga);

                if (ctes.Count <= 0 && nfses.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados documentos autorizados para esta carga.");

                if (ctes.Count + nfses.Count > 2000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 2000 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                System.IO.MemoryStream arquivo = svcCTe.ObterLoteDeDACTE(ctes, nfses, unitOfWork);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_DACTE.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de DACTE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadLoteXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
                Repositorio.Embarcador.CTe.CTeSVMMultimodal repCTeSVMMultimodal = new Repositorio.Embarcador.CTe.CTeSVMMultimodal(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> codigosCTes = repCTeSVMMultimodal.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga);
                List<int> codigosNFSes = repCTeSVMMultimodal.BuscarCodigosNFSePorCarga(codigoCarga);

                for (int i = 0; i < codigosNFSes.Count(); i++)
                    codigosCTes.Add(codigosNFSes[i]);

                if (codigosCTes.Count <= 0 && codigosNFSes.Count <= 0)
                    return new JsonpResult(false, true, "Não há CT-es/NFS-es disponíveis para esta carga.");

                //if (codigosCTes.Count > 500)
                //    return new JsonpResult(false, true, "Não é possível realizar o download de mais de 500 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                System.IO.MemoryStream arquivo = svcCTe.ObterLoteDeXML(codigosCTes, 0, unitOfWork);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_XML.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadLoteDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.CTe.CTeSVMMultimodal repCTeSVMMultimodal = new Repositorio.Embarcador.CTe.CTeSVMMultimodal(unitOfWork);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCTeSVMMultimodal.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga);

                if (ctes.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados CT-es autorizados para esta carga.");

                int codigoUsuario = this.Usuario.Codigo;
                string stringConexao = _conexao.StringConexao;
                string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos;
                string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                string caminhoArquivosAnexos = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido" });

                Task.Run(() => Zeus.Embarcador.ZeusNFe.Zeus.GerarPDFTodosDocumentos(0, ctes, stringConexao, codigoUsuario, caminhoRelatorios, caminhoArquivos, diretorioDocumentosFiscais, "Cargas/Carga", caminhoArquivosAnexos));

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote dos documentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void ObterFiltrosPesquisa()
        {

        }

        #endregion
    }
}
