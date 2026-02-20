using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/SpedPISCOFINS")]
    public class SpedPISCOFINSController : BaseController
    {
		#region Construtores

		public SpedPISCOFINSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.SpedPISCOFINS repSpedPISCOFINS = new Repositorio.Embarcador.NotaFiscal.SpedPISCOFINS(unitOfWork);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal statusArquivoSpedPISCOFINS;
                Enum.TryParse(Request.Params("Status"), out statusArquivoSpedPISCOFINS);

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

                List<Dominio.Entidades.Embarcador.NotaFiscal.SpedPISCOFINS> listaSpedPISCOFINS = repSpedPISCOFINS.Consultar(dataInicial, dataFinal, statusArquivoSpedPISCOFINS, empresa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSpedPISCOFINS.ContarConsulta(dataInicial, dataFinal, statusArquivoSpedPISCOFINS, empresa));
                var lista = (from p in listaSpedPISCOFINS
                             select new
                             {
                                 p.Codigo,
                                 DataInicial = p.DataInicial.ToString("dd/MM/yyyy"),
                                 DataFinal = p.DataFinal.ToString("dd/MM/yyyy"),
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

                Repositorio.Embarcador.NotaFiscal.SpedPISCOFINS repSpedPISCOFINS = new Repositorio.Embarcador.NotaFiscal.SpedPISCOFINS(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.SpedPISCOFINS spedPISCOFINS = new Dominio.Entidades.Embarcador.NotaFiscal.SpedPISCOFINS();

                int empresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa.Codigo;
                else
                    int.TryParse(Request.Params("Empresa"), out empresa);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal tipoMovimentoSpedPISCOFINS;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimentoSpedPISCOFINS);

                spedPISCOFINS.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                spedPISCOFINS.TipoMovimento = tipoMovimentoSpedPISCOFINS;
                spedPISCOFINS.StatusArquivo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.AguardandoGeracao;

                spedPISCOFINS.DataInicial = dataInicial;
                spedPISCOFINS.DataFinal = dataFinal;

                repSpedPISCOFINS.Inserir(spedPISCOFINS, Auditado);
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
                int codigoSpedPISCOFINS;
                int.TryParse(Request.Params("Codigo"), out codigoSpedPISCOFINS);

                if (codigoSpedPISCOFINS > 0)
                {
                    Repositorio.Embarcador.NotaFiscal.SpedPISCOFINS repSpedPISCOFINS = new Repositorio.Embarcador.NotaFiscal.SpedPISCOFINS(unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.SpedPISCOFINS spedPISCOFINS = repSpedPISCOFINS.BuscarPorCodigo(codigoSpedPISCOFINS);

                    if (spedPISCOFINS != null)
                    {
                        byte[] data = repSpedPISCOFINS.ObterTXTGerado(spedPISCOFINS);

                        if (data != null)
                            return Arquivo(data, "text/txt", string.Concat("SpedPISCOFINS_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
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
    }
}
