using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/SpedFiscal")]
    public class SpedFiscalController : BaseController
    {
		#region Construtores

		public SpedFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.SpedFiscal repSpedFiscal = new Repositorio.Embarcador.NotaFiscal.SpedFiscal(unitOfWork);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal statusArquivoSpedFiscal;
                Enum.TryParse(Request.Params("Status"), out statusArquivoSpedFiscal);

                int empresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CodigoStatus", false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 25, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DescricaoTipo")
                    propOrdenar = "Tipo";
                else if (propOrdenar == "DescricaoStatus")
                    propOrdenar = "Status";

                List<Dominio.Entidades.Embarcador.NotaFiscal.SpedFiscal> listaSpedFiscal = repSpedFiscal.Consultar(dataInicial, dataFinal, statusArquivoSpedFiscal, empresa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSpedFiscal.ContarConsulta(dataInicial, dataFinal, statusArquivoSpedFiscal, empresa));
                var lista = (from p in listaSpedFiscal
                             select new
                             {
                                 p.Codigo,
                                 DataInicial = p.DataInicial > DateTime.MinValue ? p.DataInicial.Value.ToString("dd/MM/yyyy") : p.DataInicialEntrada > DateTime.MinValue ? p.DataInicialEntrada.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataFinal = p.DataFinal > DateTime.MinValue ? p.DataFinal.Value.ToString("dd/MM/yyyy") : p.DataFinalEntrada > DateTime.MinValue ? p.DataFinalEntrada.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 p.DescricaoTipo,
                                 p.DescricaoStatus,
                                 CodigoStatus = p.StatusArquivo,
                                 Retorno = p.ComRetorno
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.NotaFiscal.SpedFiscal repSpedFiscal = new Repositorio.Embarcador.NotaFiscal.SpedFiscal(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.SpedFiscal spedFiscal = new Dominio.Entidades.Embarcador.NotaFiscal.SpedFiscal();

                int empresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa.Codigo;
                else
                    int.TryParse(Request.Params("Empresa"), out empresa);

                int codigoDocumentoEntrada = 0;
                int.TryParse(Request.Params("DocumentoEntrada"), out codigoDocumentoEntrada);

                bool comExtensaoCFOP, comNFSePropria, comInventario, comRessarcimentoICMS, comD160, comD170;
                bool.TryParse(Request.Params("ComExtensaoCFOP"), out comExtensaoCFOP);
                bool.TryParse(Request.Params("ComNFSePropria"), out comNFSePropria);
                bool.TryParse(Request.Params("ComInventario"), out comInventario);
                bool.TryParse(Request.Params("ComRessarcimentoICMS"), out comRessarcimentoICMS);
                bool.TryParse(Request.Params("ComD160"), out comD160);
                bool.TryParse(Request.Params("ComD170"), out comD170);

                DateTime dataInicial, dataFinal, dataInventario;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);
                DateTime.TryParse(Request.Params("DataInventario"), out dataInventario);
                DateTime.TryParse(Request.Params("DataInicialFinalizacao"), out DateTime dataInicialFinalizacao);
                DateTime.TryParse(Request.Params("DataFinalFinalizacao"), out DateTime dataFinalFinalizacao);
                DateTime.TryParse(Request.Params("DataInicialEntrada"), out DateTime dataInicialEntrada);
                DateTime.TryParse(Request.Params("DataFinalEntrada"), out DateTime dataFinalEntrada);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal tipoMovimentoSpedFiscal;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimentoSpedFiscal);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoSPEDFiscal situacaoDocumentoSPEDFiscal;
                Enum.TryParse(Request.Params("SituacaoDocumentoSPEDFiscal"), out situacaoDocumentoSPEDFiscal);

                spedFiscal.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                spedFiscal.TipoMovimento = tipoMovimentoSpedFiscal;
                spedFiscal.StatusArquivo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.AguardandoGeracao;

                if (dataInicial > DateTime.MinValue)
                    spedFiscal.DataInicial = dataInicial;
                if (dataFinal > DateTime.MinValue)
                    spedFiscal.DataFinal = dataFinal;
                if (dataInventario > DateTime.MinValue)
                    spedFiscal.DataInventario = dataInventario;
                spedFiscal.ComExtensaoCFOP = comExtensaoCFOP;
                spedFiscal.ComNFSePropria = comNFSePropria;
                spedFiscal.ComIntentario = comInventario;
                spedFiscal.ComBlocoK = Request.GetBoolParam("ComBlocoK");
                spedFiscal.ComRessarcimentoICMS = comRessarcimentoICMS;
                spedFiscal.ComD160 = comD160;
                spedFiscal.ComD170 = comD170;
                spedFiscal.DocumentoEntrada = codigoDocumentoEntrada > 0 ? repDocumentoEntradaTMS.BuscarPorCodigo(codigoDocumentoEntrada) : null;
                spedFiscal.SituacaoDocumentoSPEDFiscal = situacaoDocumentoSPEDFiscal;
                if (dataInicialFinalizacao > DateTime.MinValue)
                    spedFiscal.DataInicialFinalizacao = dataInicialFinalizacao;
                if (dataFinalFinalizacao > DateTime.MinValue)
                    spedFiscal.DataFinalFinalizacao = dataFinalFinalizacao;
                if (dataInicialEntrada > DateTime.MinValue)
                    spedFiscal.DataInicialEntrada = dataInicialEntrada;
                if (dataFinalEntrada > DateTime.MinValue)
                    spedFiscal.DataFinalEntrada = dataFinalEntrada;

                if ((!spedFiscal.DataInicial.HasValue || !spedFiscal.DataFinal.HasValue) && (!spedFiscal.DataInicialEntrada.HasValue || !spedFiscal.DataFinalEntrada.HasValue))
                    return new JsonpResult(false, "Favor informe ao menus um período. Emissão ou Entrada.");

                SalvarDocumentos(ref spedFiscal, unitOfWork);

                repSpedFiscal.Inserir(spedFiscal, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadTXT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoSpedFiscal;
                int.TryParse(Request.Params("Codigo"), out codigoSpedFiscal);

                if (codigoSpedFiscal > 0)
                {
                    Repositorio.Embarcador.NotaFiscal.SpedFiscal repSpedFiscal = new Repositorio.Embarcador.NotaFiscal.SpedFiscal(unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.SpedFiscal spedFiscal = repSpedFiscal.BuscarPorCodigo(codigoSpedFiscal);

                    if (spedFiscal != null)
                    {
                        byte[] data = repSpedFiscal.ObterTXTGerado(spedFiscal);

                        if (data != null)
                            return Arquivo(data, "text/txt", string.Concat("SpedFiscal_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
                    }
                }

                return new JsonpResult(false, false, "TXT não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do TXT.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        private void SalvarDocumentos(ref Dominio.Entidades.Embarcador.NotaFiscal.SpedFiscal sped, Repositorio.UnitOfWork unidadeTrabalho)
        {

            dynamic documentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("DocumentosEntrada"));

            if (sped.Documentos == null)
            {
                sped.Documentos = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS>();
            }

            foreach (var doc in documentos)
            {
                int codigo = (int)doc.Documento.Codigo;
                if (!sped.Documentos.Any(o => o.Codigo == codigo))
                    sped.Documentos.Add(new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS() { Codigo = codigo });
            }
        }
    }
}
