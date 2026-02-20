using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
	public class ContainerController : BaseController
    {
		#region Construtores

		public ContainerController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R204_Container;
        private int UltimaColunaDinanica = 1;
        private int NumeroMaximoComplementos = 60;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Containeres", "CTe", "Container.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(await ObterGridPadraoAsync(unitOfWork, cancellationToken), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

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

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.Container servicoRelatorioContainer = new Servicos.Embarcador.Relatorios.CTes.Container(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioContainer.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.CTe.Container> listaContainerCTe, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows(listaContainerCTe);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioContainer()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                NumeroNota = Request.GetIntParam("NumeroNota"),
                NumeroSerie = Request.GetIntParam("NumeroSerie"),
                SituacaoCTe = Request.GetListParam<string>("SituacaoCTe"),
                SituacaoCarga = Request.GetListEnumParam<SituacaoCarga>("SituacaoCarga"),
                SituacoesCargaMercante = Request.GetListEnumParam<SituacaoCargaMercante>("SituacaoCargaMercante"),
                TipoProposta = Request.GetListEnumParam<TipoPropostaMultimodal>("TipoProposta"),
                TipoServico = Request.GetListEnumParam<TipoServicoMultimodal>("TipoServico"),
                TipoModal = Request.GetListEnumParam<TipoModal>("TipoModal"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
                CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
                CodigoViagem = Request.GetIntParam("Viagem"),
                CodigoContainer = Request.GetIntParam("Container"),
                CodigoTerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                CodigoTerminalDestino = Request.GetIntParam("TerminalDestino"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                TiposCTe = Request.GetListEnumParam<Dominio.Enumeradores.TipoCTE>("TipoCTe"),
                VeioPorImportacao = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("VeioPorImportacao"),
                SomenteCTeSubstituido = Request.GetBoolParam("SomenteCTeSubstituido"),
                CodigoViagemTransbordo = Request.GetIntParam("ViagemTransbordo"),
                CodigoPortoTransbordo = Request.GetIntParam("PortoTransbordo"),
                CodigoBalsa = Request.GetIntParam("Balsa")
            };
        }

        private async Task<Models.Grid.Grid> ObterGridPadraoAsync(Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho, cancellationToken);
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = await repComponenteFrete.BuscarTodosAtivosAsync();

            UltimaColunaDinanica = 1;
            decimal _tamanhoColunasValores = (decimal)1.75;
            decimal _tamanhoColunasDescricoes = (decimal)3.50;

            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Container", "ContainerDescricao", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Container", "NumeroContainer", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº OS", "NumeroOS", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Viagem", "Viagem", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Código Navio", "CodigoNavio", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Navio Transbordo", "NavioTransbordo", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Balsa", "Balsa", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Cód. Porto Origem", "CodigoPortoOrigem", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Terminal Origem", "TerminalOrigem", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Cód. Porto Destino", "CodigoPortoDestino", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Porto Destino", "PortoDestino", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Terminal Destino", "TerminalDestino", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Porto Transbordo", "PortoTransbordo", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Terminal Transbordo", "TerminalTransbordo", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Peso Bruto", "PesoBruto", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Tipo Container", "TipoContainer", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Previsão Saída Navio", "ETS", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Previsão Chegada Navio", "ETA", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Previsão Chegada Navio Transbordo 1", "ETATransbordo1", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("CPF/CNPJ Expedidor", "CPFCNPJExpedidor", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Expedidor", "Expedidor", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CNPJRemetente", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Destinatário", "CNPJDestinatario", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Recebedor", "CNPJRecebedor", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomador", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Nº Lacre", "NumeroLacre", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tara", "Tara", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº CTe", "NumeroCTe", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série", "SerieCTe", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Controle", "NumeroControle", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº NFe", "NumeroNota", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. NF", "QuantidadeNota", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Código Início Prestação", "CodigoInicioPrestacao", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Início Prestação", "InicioPrestacao", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("UF Início", "UFInicioPrestacao", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Código Fim Prestação", "CodigoFimPrestacao", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Fim Prestação", "FimPrestacao", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("UF Fim", "UFFimPrestacao", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Carga IMO", "CargaIMO", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Proposta", "TipoProposta", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Proposta", "NumeroProposta", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Status CTe", "StatusCTe", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Carga", "SituacaoCargaFormatada", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Possui Notas?", "PossuiNotas", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Notas", "ValorNotas", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("PTAX", "Taxa", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("CFOP", "CFOP", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("CST", "CST", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("B.C. do ICMS", "BaseCalculoICMS", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Alíquota ICMS", "AliquotaICMS", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor ICMS", "ValorICMS", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Alíquota ISS", "AliquotaISS", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor ISS", "ValorISS", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ISS Retido", "ValorISSRetido", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Alíquota PIS", "AliquotaPIS", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota COFINS", "AliquotaCOFINS", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor a Receber", "ValorReceber", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor da Prestação", "ValorPrestacao", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Total com Imposto Parcial", "ValorSemImposto", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Nº da Carga", "NumeroCarga", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Doc", "AbreviacaoModeloDocumentoFiscal", _tamanhoColunasValores, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo do CT-e", "DescricaoTipoCTe", _tamanhoColunasValores, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo do Serviço", "DescricaoTipoServico", _tamanhoColunasValores, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Autorização", "DataAutorizacaoFormatada", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Entrega", "DataEntrega", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Coleta", "DataColeta", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Previsão de Entrega", "DataPrevistaEntrega", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportador", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Chave CTe", "ChaveCTe", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Chave CTe Multimodal", "ChaveCTeMultimodal", _tamanhoColunasDescricoes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave CTe SVM", "ChaveCTeSVM", _tamanhoColunasDescricoes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Foi Anulado", "CTeAnulado", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Foi Substituído", "SomenteCTeSubstituido", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Manifesto", "NumeroManifesto", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CE Mercante", "NumeroCEMercante", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Commodity", "Commodity", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Afretamento", "AfretamentoDescricao", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Protocolo ANTAQ", "NumeroProtocoloANTAQ", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("FFE", "FFE", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("TEU", "TEU", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Número Manifesto FEEDER", "NumeroManifestoFEEDER", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CE FEEDER", "NumeroCEFEEDER", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Número CT-e Substituto", "NumeroCTeSubstituto", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Controle CT-e Substituto", "NumeroControleCTeSubstituto", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CT-e Anulação", "NumeroCTeAnulacao", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Controle CT-e Anulação", "NumeroControleCTeAnulacao", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CT-e Complementar", "NumeroCTeComplementar", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Controle CT-e Complementar", "NumeroControleCTeComplementar", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CT-e Manual Duplicado", "NumeroCTeDuplicado", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Controle CT-e Manual Duplicado", "NumeroControleCTeDuplicado", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número CT-e Original", "NumeroCTeOriginal", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Controle CT-e Original", "NumeroControleCTeOriginal", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Data da Operação do Navio (POD)", "DataOperacaoNavioFormatada", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Booking Reference FEEDER", "BookingReferente", _tamanhoColunasDescricoes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Valor total sem Tributo", "ValorSemTributo", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Observação", "Observacao", _tamanhoColunasDescricoes, Models.Grid.Align.left, false, false, false, false, false);

            //Colunas montadas dinamicamente
            for (int i = 0; i < componentes.Count; i++)
            {
                if (i < NumeroMaximoComplementos)
                {
                    grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + UltimaColunaDinanica.ToString(), _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum, componentes[i].Codigo);

                    UltimaColunaDinanica++;
                }
                else
                    break;
            }

            return grid;
        }

        #endregion
    }
}
