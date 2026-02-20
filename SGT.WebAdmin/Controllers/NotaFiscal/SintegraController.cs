using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/Sintegra")]
    public class SintegraController : BaseController
    {
		#region Construtores

		public SintegraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.Sintegra repSintegra = new Repositorio.Embarcador.NotaFiscal.Sintegra(unitOfWork);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal statusArquivoSintegra;
                Enum.TryParse(Request.Params("Status"), out statusArquivoSintegra);

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

                List<Dominio.Entidades.Embarcador.NotaFiscal.Sintegra> listaSintegra = repSintegra.Consultar(dataInicial, dataFinal, statusArquivoSintegra, empresa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSintegra.ContarConsulta(dataInicial, dataFinal, statusArquivoSintegra, empresa));
                var lista = (from p in listaSintegra
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

                Repositorio.Embarcador.NotaFiscal.Sintegra repSintegra = new Repositorio.Embarcador.NotaFiscal.Sintegra(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.Sintegra sintegra = new Dominio.Entidades.Embarcador.NotaFiscal.Sintegra();

                int empresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa.Codigo;
                else
                    int.TryParse(Request.Params("Empresa"), out empresa);

                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

                Enum.TryParse(Request.Params("TipoMovimento"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal tipoMovimentoSintegra);

                if (!dataInicial.IsFirstDayOfMonth())
                    return new JsonpResult(false, true, "Data Inicial deve ser o primeiro dia do mês.");
                else if (!dataFinal.IsLastDayOfMonth())
                    return new JsonpResult(false, true, "Data Final deve ser o último dia do mês.");
                else if (!dataInicial.IsDateSameMonth(dataFinal))
                    return new JsonpResult(false, true, "Data Inicial e Final devem ser do mesmo mês e ano.");

                sintegra.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                sintegra.TipoMovimento = tipoMovimentoSintegra;
                sintegra.StatusArquivo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.AguardandoGeracao;

                sintegra.DataInicial = dataInicial;
                sintegra.DataFinal = dataFinal;

                repSintegra.Inserir(sintegra, Auditado);
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
                int codigoSintegra;
                int.TryParse(Request.Params("Codigo"), out codigoSintegra);

                if (codigoSintegra > 0)
                {
                    Repositorio.Embarcador.NotaFiscal.Sintegra repSintegra = new Repositorio.Embarcador.NotaFiscal.Sintegra(unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.Sintegra sintegra = repSintegra.BuscarPorCodigo(codigoSintegra);

                    if (sintegra != null)
                    {
                        byte[] data = repSintegra.ObterTXTGerado(sintegra);

                        if (data != null)
                            return Arquivo(data, "text/txt", string.Concat("Sintegra_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
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
