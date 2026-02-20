using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	public class ConferenciaFiscalController : BaseController
    {
		#region Construtores

		public ConferenciaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R276_ConferenciaFiscal;

        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Conferência Fiscal", "Financeiros", "ConferenciaFiscal.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroCIOT", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConferenciaFiscal filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.ConferenciaFiscal servicoConferenciaFiscal = new Servicos.Embarcador.Relatorios.Financeiros.ConferenciaFiscal(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoConferenciaFiscal.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConferenciaFiscal> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
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
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConferenciaFiscal filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o reltároio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data Emissão CIOT", "DataEmissaoCIOTFormatada", TamanhoColunaPequena, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Número CIOT", "NumeroCIOT", TamanhoColunaPequena, Models.Grid.Align.left, true, true);

            grid.AdicionarCabecalho("Contratante", "Contratante", TamanhoColunaGrande, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("CNPJ Contratante", "CNPJContratanteFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Contratado", "Contratado", TamanhoColunaGrande, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("CNPJ/CPF Contratado", "CPFCNPJContratadoFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Regime Tributário Terceiro", "RegimeTributarioTerceiroFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF do Contratado", "UFContratado", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Município do Contratado", "MunicipioContratado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Valor do frete contratado", "ValorFreteContratado", TamanhoColunaPequena, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor PIS", "ValorPIS", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor COFINS", "ValorCOFINS", TamanhoColunaPequena, Models.Grid.Align.right, false, false);

            grid.AdicionarCabecalho("Data Emissão CTe", "DataEmissaoCTeFormatada", TamanhoColunaPequena, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Chave de acesso CTe", "ChaveCTe", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Numero CTe", "NumeroCTe", TamanhoColunaPequena, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Série CTe", "SerieCTe", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Finalidade", "FinalidadeFormatada", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Forma", "FormaFormatada", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("CFOP CTe", "CFOPCTe", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Status CTe", "DescricaoStatusCTe", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data de cancelamento CTe", "DataCancelamentoCTeFormatada", TamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Valor Total CTe", "ValorTotalCTe", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Base ICMS", "BaseICMS", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Alíquota ICMS", "AliquotaICMS", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor ICMS", "ValorICMS", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("CST ICMS", "CSTICMS", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Chave de acesso CTe complementar", "ChaveCTeComplementar", TamanhoColunaGrande, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Emitente", "Emitente", TamanhoColunaGrande, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("CNPJ/CPF Emitente", "CPFCNPJEmitenteFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF Emitente", "UFEmitente", TamanhoColunaPequena, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Tomador", "Tomador", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("CNPJ/CPF Tomador", "CPFCNPJTomadorFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF Tomador", "UFTomador", TamanhoColunaPequena, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("CNPJ/CPF Remetente", "CPFCNPJRemetenteFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF Remetente", "UFRemetente", TamanhoColunaPequena, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Expedidor", "Expedidor", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("CNPJ/CPF Expedidor", "CPFCNPJExpedidorFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF Expedidor", "UFExpedidor", TamanhoColunaPequena, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunaGrande, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("CNPJ/CPF Destinatário", "CPFCNPJDestinatarioFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF Destinatário", "UFDestinatario", TamanhoColunaPequena, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Recebedor", "Recebedor", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("CNPJ/CPF Recebedor", "CPFCNPJRecebedorFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF Recebedor", "UFRecebedor", TamanhoColunaPequena, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("UF início da prestação", "UFInicioPrestacao", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Município início da prestação", "MunicipioInicioPrestacao", TamanhoColunaMedia, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("UF fim da prestação", "UFFimPrestacao", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Município fim da prestação", "MunicipioFimPrestacao", TamanhoColunaMedia, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Produto predominante", "ProdutoPredominante", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Unidade de medida", "DescricaoUnidadeMedida", TamanhoColunaPequena, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Cahve de acesso NFe Referenciada", "ChaveNFeReferenciada", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Número NFe Referenciada", "NumeroNFeReferenciada", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Data emissão NFe", "DataEmissaoNFeFormatada", TamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Valor NFe", "ValorNFe", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("CFOP NFe", "CFOPNFe", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Base ICMS NFe", "BaseICMSNFe", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Alíquota ICMS NFe", "AliquotaICMSNFe", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor ICMS NFe", "ValorICMSNFe", TamanhoColunaPequena, Models.Grid.Align.right, false, false);

            grid.AdicionarCabecalho("Emitente NFe", "EmitenteNota", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("CNPJ/CPF Emitente NFe", "CPFCNPJEmitenteNotaFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Destinatário NFe", "DestinatarioNota", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("CNPJ/CPF Destinatário NFe", "CPFCNPJDestinatarioNotaFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConferenciaFiscal ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConferenciaFiscal()
            {
                CpfCnpjTerceiros = Request.GetListParam<double>("Terceiro"),
                DataEmissaoContratoInicial = Request.GetDateTimeParam("DataEmissaoContratoInicial"),
                DataEmissaoContratoFinal = Request.GetDateTimeParam("DataEmissaoContratoFinal"),
                DataAprovacaoInicial = Request.GetDateTimeParam("DataAprovacaoInicial"),
                DataAprovacaoFinal = Request.GetDateTimeParam("DataAprovacaoFinal"),
                DataEncerramentoInicial = Request.GetDateTimeParam("DataEncerramentoInicial"),
                DataEncerramentoFinal = Request.GetDateTimeParam("DataEncerramentoFinal"),
                DataEncerramentoCIOTInicial = Request.GetDateTimeParam("DataEncerramentoCIOTInicial"),
                DataEncerramentoCIOTFinal = Request.GetDateTimeParam("DataEncerramentoCIOTFinal"),
                DataAberturaCIOTInicial = Request.GetDateTimeParam("DataAberturaCIOTInicial"),
                DataAberturaCIOTFinal = Request.GetDateTimeParam("DataAberturaCIOTFinal"),
                Veiculo = Request.GetIntParam("Veiculo"),
                NumeroContrato = Request.GetIntParam("NumeroContrato"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                ModeloVeicular = Request.GetIntParam("ModeloVeicular"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                Empresa = Request.GetIntParam("Empresa"),
                Situacao = Request.GetListEnumParam<SituacaoContratoFrete>("Situacao"),
                StatusCTe = Request.GetStringParam("StatusCTe")
            };
        }

        #endregion
    }
}
