using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/FaturamentoPorCTe")]
    public class FaturamentoPorCTeController : BaseController
    {
		#region Construtores

		public FaturamentoPorCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R217_FaturamentoPorCTe;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Faturamento por CT-e", "CTe", "FaturamentoPorCTe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.FaturamentoPorCTe servicoRelatorioFaturamentoPorCTe = new Servicos.Embarcador.Relatorios.CTes.FaturamentoPorCTe(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioFaturamentoPorCTe.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.CTe.FaturamentoPorCTe> listaFaturamentoPorCTe, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaFaturamentoPorCTe);

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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
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
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe()
            {
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                DataInicialFatura = Request.GetDateTimeParam("DataInicialFatura"),
                DataFinalFatura = Request.GetDateTimeParam("DataFinalFatura"),
                DataInicialVencimentoFatura = Request.GetDateTimeParam("DataInicialVencimentoFatura"),
                DataFinalVencimentoFatura = Request.GetDateTimeParam("DataFinalVencimentoFatura"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                NumeroFatura = Request.GetIntParam("NumeroFatura"),
                NumeroTitulo = Request.GetIntParam("NumeroTitulo"),
                NumeroBoleto = Request.GetStringParam("NumeroBoleto"),
                NFe = Request.GetStringParam("NFe"),
                SituacaoFatura = Request.GetNullableEnumParam<SituacaoFatura>("SituacaoFatura"),
                SituacaoFaturamentoCTe = Request.GetNullableEnumParam<SituacaoFaturamentoCTe>("SituacaoFaturamentoCte"),
                TipoProposta = Request.GetNullableEnumParam<TipoPropostaMultimodal>("TipoProposta"),
                VeioPorImportacao = Request.GetEnumParam<OpcaoSimNaoPesquisa>("VeioPorImportacao"),
                SomenteCTeSubstituido = Request.GetBoolParam("CTeSubstituido"),
                StatusCTe = Request.GetListParam<string>("Situacao"),
                TipoServico = Request.GetListParam<TipoServicoMultimodal>("TipoServico"),
                TipoTomador = Request.GetListParam<int>("TipoTomador"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                SituacaoCarga = Request.GetEnumParam<SituacaoCarga>("SituacaoCarga"),
                SituacoesCargaMercante = Request.GetListEnumParam<SituacaoCargaMercante>("SituacaoCargaMercante"),
                StatusTitulo = Request.GetListEnumParam<StatusTitulo>("StatusTitulo"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
                CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
                CodigoViagem = Request.GetIntParam("Viagem"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                TiposCTe = Request.GetListEnumParam<TipoCTE>("TipoCTe"),
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

            grid.AdicionarCabecalho("Número", "NumeroCTe", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série", "SerieCTe", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº da Carga", "NumeroCarga", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo do CT-e", "DescricaoTipoCTe", _tamanhoColunaPequena, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Status", "StatusCTe", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimentoBoletoFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Vencimento Títulos", "DataVencimentoTitulo", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Carga", "SituacaoCargaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Título", "DescricaoStatusTitulo", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetente", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Expedidor", "CPFCNPJExpedidor", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Expedidor", "Expedidor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Recebedor", "CPFCNPJRecebedor", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Destinatario", "CPFCNPJDestinatario", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomador", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Pessoa", "GrupoTomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Início da Prestação", "InicioPrestacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Início", "UFInicioPrestacao", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Fim da Prestação", "FimPrestacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Fim", "UFFimPrestacao", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Alíquota ICMS", "AliquotaICMS", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do ICMS", "ValorICMS", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor da Prestacao", "ValorPrestacao", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Nota Fiscal", "NumeroNotaFiscal", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Chave CTe", "ChaveCTe", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº OS", "NumeroOS", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle", "NumeroControle", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. NF", "QuantidadeNF", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Lacre", "NumeroLacre", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tara", "Tara", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Container", "Container", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Container", "TipoContainer", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Fatura", "NumeroFatura", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Fatura", "DataFaturaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº Boleto", "NumeroBoleto", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Boleto", "DataBoletoFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Porto Destino", "PortoDestino", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Porto Transbordo", "PortoTransbordo", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Navio Transbordo", "NavioTransbordo", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Data Envio Fatura", "DataEnvioFaturaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            //grid.AdicionarCabecalho("Data Vencimento Fatura", "DataVencimentoFaturaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Email Fatura", "EmailFatura", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Email Fatura", "SituacaoEmailFatura", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Faturamento a vista", "FaturamentoAVista", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Envio Boleto", "DataEnvioBoletoFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Email Boleto", "EmailBoleto", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Título", "NumeroTitulo", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Email Boleto", "SituacaoEmailBoleto", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Viagem", "Viagem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso NF", "PesoNotaFiscal", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso Bruto NF", "PesoBrutoNotaFiscal", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Dia Semana", "DiaSemana", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Dia Mês", "DiaMes", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Prazo Faturamento", "TipoPrazoFaturamentoFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("BAF", "ValorComponenteBAF", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Ad Valorem", "ValorComponenteAdValorem", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Previsão Saída Navio", "ETS", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Foi Substituído", "SomenteCTeSubstituido", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Foi Anulado", "CTeAnulado", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Proposta", "TipoProposta", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Serviço", "DescricaoTipoServico", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Número CT-e Substituto", "NumeroCTeSubstituto", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Controle CT-e Substituto", "NumeroControleCTeSubstituto", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CT-e Anulação", "NumeroCTeAnulacao", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Controle CT-e Anulação", "NumeroControleCTeAnulacao", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CT-e Complementar", "NumeroCTeComplementar", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Controle CT-e Complementar", "NumeroControleCTeComplementar", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CT-e Manual Duplicado", "NumeroCTeDuplicado", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Controle CT-e Manual Duplicado", "NumeroControleCTeDuplicado", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CT-e Original", "NumeroCTeOriginal", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Controle CT-e Original", "NumeroControleCTeOriginal", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
