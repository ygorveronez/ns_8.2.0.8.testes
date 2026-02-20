using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/NFe/NotasEmitidasAdministrativo")]
    public class NotasEmitidasAdministrativoController : BaseController
    {
		#region Construtores

		public NotasEmitidasAdministrativoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R022_NotasEmitidasAdministrativo;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Notas Emitidas Administrativo", "NFe", "NotasEmitidasAdministrativo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                int numeroInicial = 0, numeroFinal = 0, serie = 0, empresa, formaEmissao = 0;
                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);
                int.TryParse(Request.Params("Serie"), out serie);
                int.TryParse(Request.Params("Empresa"), out empresa);
                int.TryParse(Request.Params("NotaImportada"), out formaEmissao);

                DateTime dataInicial, dataFinal, dataProcessamento, dataSaida, dataInicialCadastro, dataFinalCadastro; ;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);
                DateTime.TryParseExact(Request.Params("DataInicialCadastro"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialCadastro);
                DateTime.TryParseExact(Request.Params("DataFinalCadastro"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalCadastro);
                DateTime.TryParseExact(Request.Params("DataProcessamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataProcessamento);
                DateTime.TryParseExact(Request.Params("DataSaida"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataSaida);

                Dominio.Enumeradores.StatusNFe status;
                Dominio.Enumeradores.TipoEmissaoNFe tipoEmissao;
                Enum.TryParse(Request.Params("Status"), out status);
                Enum.TryParse(Request.Params("TipoEmissao"), out tipoEmissao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa statusEmpresa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;
                Enum.TryParse(Request.Params("StatusEmpresa"), out statusEmpresa);

                bool.TryParse(Request.Params("SomenteClientesComEmissao"), out bool exibirSomenteClientesComEmissao);

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = grid.header[grid.indiceColunaOrdena].data;

                IList<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidasAdministrativo> listaNotasEmitidas = repNotaFiscal.RelatorioNotasEmitidasAdministrativo(statusEmpresa, this.Usuario.Empresa.Codigo, numeroInicial, numeroFinal, serie, empresa, dataInicial, dataFinal, dataProcessamento, dataSaida, status, tipoEmissao, formaEmissao, dataInicialCadastro, dataFinalCadastro, this.Usuario.Empresa.TipoAmbiente, exibirSomenteClientesComEmissao, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNotaFiscal.ContarRelatorioNotasEmitidasAdministrativo(statusEmpresa, this.Usuario.Empresa.Codigo, numeroInicial, numeroFinal, serie, empresa, dataInicial, dataFinal, dataProcessamento, dataSaida, status, tipoEmissao, formaEmissao, dataInicialCadastro, dataFinalCadastro, this.Usuario.Empresa.TipoAmbiente, exibirSomenteClientesComEmissao));
                grid.AdicionaRows(listaNotasEmitidas);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int numeroInicial = 0, numeroFinal = 0, serie = 0, empresa, formaEmissao = 0;
                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);
                int.TryParse(Request.Params("Serie"), out serie);
                int.TryParse(Request.Params("Empresa"), out empresa);
                int.TryParse(Request.Params("NotaImportada"), out formaEmissao);

                DateTime dataInicial, dataFinal, dataProcessamento, dataSaida, dataInicialCadastro, dataFinalCadastro;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);
                DateTime.TryParseExact(Request.Params("DataInicialCadastro"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialCadastro);
                DateTime.TryParseExact(Request.Params("DataFinalCadastro"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalCadastro);
                DateTime.TryParseExact(Request.Params("DataProcessamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataProcessamento);
                DateTime.TryParseExact(Request.Params("DataSaida"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataSaida);

                Dominio.Enumeradores.StatusNFe status;
                Dominio.Enumeradores.TipoEmissaoNFe tipoEmissao;
                Enum.TryParse(Request.Params("Status"), out status);
                Enum.TryParse(Request.Params("TipoEmissao"), out tipoEmissao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa statusEmpresa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;
                Enum.TryParse(Request.Params("StatusEmpresa"), out statusEmpresa);

                bool.TryParse(Request.Params("SomenteClientesComEmissao"), out bool exibirSomenteClientesComEmissao);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);


                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioNotasEmitidasAdministrativo(statusEmpresa, codigoEmpresa, tipoAmbiente, numeroInicial, numeroFinal, serie, empresa, dataInicial, dataFinal, dataProcessamento, dataSaida, status, tipoEmissao, formaEmissao, dataInicialCadastro, dataFinalCadastro, exibirSomenteClientesComEmissao, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioNotasEmitidasAdministrativo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa statusEmpresa, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int numeroInicial, int numeroFinal, int serie, int empresa, DateTime dataInicial, DateTime dataFinal, DateTime dataProcessamento, DateTime dataSaida, Dominio.Enumeradores.StatusNFe status, Dominio.Enumeradores.TipoEmissaoNFe tipoEmissao, int formaEmissao, DateTime dataInicialCadastro, DateTime dataFinalCadastro, bool exibirSomenteClientesComEmissao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidasAdministrativo> listaNotasEmitidas = repNotaFiscal.RelatorioNotasEmitidasAdministrativo(statusEmpresa, codigoEmpresa, numeroInicial, numeroFinal, serie, empresa, dataInicial, dataFinal, dataProcessamento, dataSaida, status, tipoEmissao, formaEmissao, dataInicialCadastro, dataFinalCadastro, tipoAmbiente, exibirSomenteClientesComEmissao, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                if (numeroInicial > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInicial", numeroInicial.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInicial", false));

                if (numeroFinal > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFinal", numeroFinal.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFinal", false));

                if (serie > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Serie", serie.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Serie", false));

                if (empresa > 0)
                {
                    Dominio.Entidades.Empresa empresaFilha = repEmpresa.BuscarPorCodigo(empresa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", "(" + empresaFilha.CNPJ_Formatado + ") " + empresaFilha.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

                if (dataInicial != DateTime.MinValue || dataFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataInicial != DateTime.MinValue ? dataInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFinal != DateTime.MinValue ? "até " + dataFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", false));

                if (dataProcessamento != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataProcessamento", dataProcessamento.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataProcessamento", false));

                if (dataSaida != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataSaida", dataSaida.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataSaida", false));

                if ((int)status > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", status.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", false));

                if ((int)tipoEmissao >= 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoEmissao", tipoEmissao.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoEmissao", false));

                if (exibirSomenteClientesComEmissao)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteClientesComEmissao", "Sim", true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteClientesComEmissao", false));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/NFe/NotasEmitidasAdministrativo",parametros,relatorioControleGeracao, relatorioTemp, listaNotasEmitidas, unitOfWork, identificacaoCamposRPT);
                await unitOfWork.DisposeAsync();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo Empresa Pai", "CodigoEmpresaPai", 2, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Empresa Pai", "NomeEmpresaPai", 9, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CNPJ Empresa Pai", "CNPJEmpresaPai", 5, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Codigo Empresa", "CodigoEmpresa", 2, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Empresa", "NomeEmpresa", 13, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Cadastro", "DataCadastro", 6, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CNPJ Empresa", "CNPJEmpresa", 5, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Inutilizadas", "QtdInutilizadas", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Canceladas", "QtdCanceladas", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Em Digitação", "QtdEmitidas", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Processadas", "QtdProcessadas", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Denegadas", "QtdDenegadas", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Rejeitadas", "QtdRejeitadas", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Autorizadas", "QtdAutorizadas", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("CCes", "QtdCCe", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("NFS-e", "QtdNFSe", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Boletos", "QtdBoletos", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("NF Destinada", "QtdNFDestinada", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Qtd. Documentos Entrada", "QtdDocumentoEntrada", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Qtd. MDF-e", "QtdMDFe", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Tot. Documentos", "TotalDocumentos", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Vlr. Plano", "ValorPlano", 4, Models.Grid.Align.right, true, false);

            return grid;
        }

        #endregion
    }
}
