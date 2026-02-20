using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Documentos
{
    [CustomAuthorize(new string[] { "BuscarNFSPendentes", "ConsultarCargaNFS" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaNFSController : BaseController
    {
        #region Construtores

        public CargaNFSController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCargaDocumentoParaEmissaoNFSManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codCarga = 0;
                int.TryParse(Request.Params("Carga"), out codCarga);

                int numeroDocumento = 0;
                int.TryParse(Request.Params("NumeroDoc"), out numeroDocumento);

                if (numeroDocumento == 0)
                {
                    int.TryParse(Request.Params("NumeroDocumento"), out numeroDocumento);
                }



                double cpfCnpjDestinatario = 0;
                double.TryParse(Request.Params("Destinatario"), out cpfCnpjDestinatario);

                bool apenasSemOcorrencias = true;
                bool pesquisaOcorrencia = Request.GetBoolParam("PesquisaNaOcorrencia", false);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Chave", "Chave", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Doc.", "Abreviacao", 5, Models.Grid.Align.center, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 13, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Peso", "Peso", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("NFS", "NFS", 6, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                //bool nfAtivas = true;
                //if (codCarga > 0)
                //{
                //    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                //    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                //    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                //        nfAtivas = false;
                //}

                if (propOrdenar == "Remetente" || propOrdenar == "Destinatario")
                    propOrdenar += ".Nome";

                if (propOrdenar == "Destino")
                    propOrdenar = "Destinatario.Localidade.Descricao";



                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSmanual = repCargaDocumentoParaEmissaoNFSManual.ConsultarDocumentoParaEmissaoNFSManual(codCarga, numeroDocumento, cpfCnpjDestinatario, apenasSemOcorrencias, pesquisaOcorrencia ? ConfiguracaoEmbarcador.GerarOcorrenciaParaCargaAgrupada : carga.CargaAgrupada, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaDocumentoParaEmissaoNFSManual.ContarDocumentoParaEmissaoNFSManual(codCarga, numeroDocumento, cpfCnpjDestinatario, apenasSemOcorrencias, pesquisaOcorrencia ? ConfiguracaoEmbarcador.GerarOcorrenciaParaCargaAgrupada : carga.CargaAgrupada));
                var dynXmlNotaFiscal = (from obj in cargaDocumentosParaEmissaoNFSmanual
                                        select new
                                        {
                                            obj.Codigo,
                                            obj.Numero,
                                            obj.Chave,
                                            obj.ModeloDocumentoFiscal.Abreviacao,
                                            Remetente = obj.Remetente.Nome + "(" + obj.Remetente.CPF_CNPJ_Formatado + ")",
                                            Destinatario = obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")",
                                            Destino = obj.Destinatario.Localidade.DescricaoCidadeEstado,
                                            ValorFrete = obj.ValorFrete.ToString(),
                                            Peso = obj.Peso.ToString(),
                                            NFS = obj.CTe != null ? obj.CTe.Numero.ToString() : "",
                                            DT_RowColor = obj.CTe != null ? "#dff0d8" : "#fcf8e3"
                                        }).ToList();

                grid.AdicionaRows(dynXmlNotaFiscal);
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

    }
}
