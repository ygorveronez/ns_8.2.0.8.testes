using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/TakeOrPay")]
    public class TakeOrPayController : BaseController
    {
		#region Construtores

		public TakeOrPayController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R263_TakeOrPay;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Embarque Certo", "CTe", "TakeOrPay.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.TakeOrPay servicoRelatorioTakeOrPay = new Servicos.Embarcador.Relatorios.CTes.TakeOrPay(unitOfWork, TipoServicoMultisoftware, Cliente);

                var listaTakeOrPay = await servicoRelatorioTakeOrPay.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(listaTakeOrPay.Count);
                grid.AdicionaRows(listaTakeOrPay);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay filtrosPesquisa = ObterFiltrosPesquisa();

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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioTakeOrPay()
            {
                DataInicialFatura = Request.GetDateTimeParam("DataInicialFatura"),
                DataFinalFatura = Request.GetDateTimeParam("DataFinalFatura"),
                NumeroFatura = Request.GetIntParam("NumeroFatura"),
                NumeroBoleto = Request.GetStringParam("NumeroBoleto"),
                SituacaoFatura = Request.GetNullableEnumParam<SituacaoFatura>("SituacaoFatura"),
                TipoProposta = Request.GetListEnumParam<TipoPropostaMultimodal>("TipoProposta"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
                CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
                CodigoViagem = Request.GetIntParam("Viagem"),
                DataInicialPrevisaoSaidaNavio = Request.GetDateTimeParam("DataInicialPrevisaoSaidaNavio"),
                DataFinalPrevisaoSaidaNavio = Request.GetDateTimeParam("DataFinalPrevisaoSaidaNavio")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);

            grid.AdicionarCabecalho("Nº da Carga", "NumeroCarga", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);

            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomador", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Grupo Pessoa Tomador", "GrupoTomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Documento Tomador", "CodigoDocumentoTomador", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Código Navio", "CodigoNavio", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Cód. Porto Origem", "CodigoPortoOrigem", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Cód. Porto Destino", "CodigoPortoDestino", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Porto Destino", "PortoDestino", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Viagem", "Viagem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Proposta", "DescricaoTipoProposta", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);

            grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Qtd. Unidades Disponibilizadas", "QtdDisponibilizada", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. Unidades Não Embarcadas", "QtdNaoEmbarcadas", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);

            grid.AdicionarCabecalho("Nº Fatura", "NumeroFatura", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Fatura", "DataFaturaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº Boleto", "NumeroBoleto", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Dias Prazo Faturamento", "DiasPrazoFaturamento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimentoBoletoFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Email Fatura", "EmailFatura", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Previsão Saída Navio", "ETS", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportador", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Situação Fatura", "SituacaoFatura", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Observação Fatura", "ObservacaoFatura", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Unidades (Contabilidade)", "UnidadesContabilidade", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquita ICMS", "AliquotaICMSContabilidade", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor ICMS", "ValorICMSContabilidade", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("PTAX", "PTAXContabilidade", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor (USD)", "ValorUSDContabilidade", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("PO Number", "PONumberContabilidade", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
