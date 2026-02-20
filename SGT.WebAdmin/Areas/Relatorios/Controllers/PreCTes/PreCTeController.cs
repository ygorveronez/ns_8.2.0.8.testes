using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.PreCTes
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/PreCTes/PreCTe")]
    public class PreCTeController : BaseController
    {
        #region Construtores

        public PreCTeController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R336_PreCTes;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;
        private int UltimaColunaDinanica = 1;
        private int NumeroMaximoComplementos = 60;
        private decimal TamanhoColunasValores = (decimal)1.75;
        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pre CT-es", "PreCTes", "PreCTe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.PreCTes.PreCTe servicoRelatorioPreCTe = new Servicos.Embarcador.Relatorios.PreCTes.PreCTe(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioPreCTe.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.PreCTes.PreCTe> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAcerto);

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

                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = svcRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);
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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe()
            {
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoEstadoDestino = Request.GetStringParam("EstadoDestino") ,
                CodigoEstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigosCargas =  Request.GetListParam<int>("Carga"),
                CodigosDestinatarios = Request.GetListParam<double>("Destinatario"),
                CodigosExpedidores = Request.GetListParam<double>("Expedidor"),
                CodigosFiliais = Request.GetListParam<int>("Filial"),
                CodigosModelosVeiculos = Request.GetListParam<int>("ModeloVeiculo"),
                CodigosRecebedores = Request.GetListParam<double>("Recebedor"),
                CodigosRemetentes = Request.GetListParam<double>("Remetente"),
                CodigosTiposDeCarga = Request.GetListParam<int>("TipoCarga"),
                CodigosTiposOperacao = Request.GetListParam<int>("TipoOperacao"),
                CodigosTiposOcorrencia = Request.GetListParam<int>("TipoOcorrencia"),
                CodigosTomadores = Request.GetListParam<double>("Tomador"),
                CodigosTransportadores =  Request.GetListParam<int>("Transportador"),
                DataEmissaoFinal = Request.GetNullableDateTimeParam("DataEmissaoFinal"),
                DataEmissaoInicial = Request.GetNullableDateTimeParam("DataEmissaoInicial"),
                NumeroNFe = Request.GetStringParam("NumeroNFe"),
                PossuiFRS = Request.GetNullableBoolParam("PossuiFRS"),
                Situacao = Request.GetNullableEnumParam<SituacaoRelatorioPreCTe>("SituacaoPreCTe"),
                TipoTomador = Request.GetListEnumParam<TipoTomador>("TipoTomador"),

            };
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = repComponenteFrete.BuscarTodosAtivos();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número da Carga", "NumeroCarga", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Número CT-e", "NumeroCTe", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Filial", "FilialDescricao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilial", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação Pré CT-e", "SituacaoPreCTeDescricao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data de Emissão", "DataEmissaoFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo do Tomador", "DescricaoTipoTomador", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoDeCarga", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Modelo Veicular da Carga", "ModeloVeicular", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Modelo do Veículo", "ModeloVeiculoCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Pagamento", "DescricaoTipoPagamento", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Prestação Serviço", "ValorPrestacaoServico", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Número da Ocorrência", "Ocorrencia", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo da Ocorrência", "TipoOcorrencia", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor a Receber", "ValorReceber", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CST", "CSTFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("B.C do ICMS", "BaseCalculoICMS", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota ICMS", "AliquotaICMS", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor do ICMS", "ValorICMS", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota ISS", "AliquiotaISS", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor do ISS", "ValorISS", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Alíquota PIS", "AliquotaPIS", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor PIS", "ValorPIS", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Alíquota COFINS", "AliquotaCOFINS", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor COFINS", "ValorCOFINS", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Mercadoria", "ValorMercadoria", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tabela Frete", "TabelaFrete", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Tabela Frete Cliente", "CodigoTabelaFreteCliente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Produto Predominante", "ProdutoPredominante", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Nome Fantasia Transportador", "NomeFantasiaTransportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Razao Social Transportador", "RazaoSocialTransportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportadorFormatado", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Transportador", "UFTransportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Motorista", "Motorista", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF Motorista", "CPFMotorista", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Chave Nota Fiscal", "ChaveNotaFiscal", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Número Nota Fiscal", "NumeroNotaFiscal", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CFOP", "CFOP", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Remetente", "Remetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código remetente", "CodigoRemetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código documento remetente", "CodigoDocumentoRemetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IE Remetente", "IERemetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Localidade Remetente", "LocalidadeRemetente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Destinatário", "Destinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Destinatário", "CodigoDestinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Documento Destinatário", "CodigoDocumentoDestinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IE Destinatário", "IEDestinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Localidade Destinatário", "LocalidadeDestinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatário", "CPFCNPJDestinatario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);


            grid.AdicionarCabecalho("Recebedor", "Recebedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Recebedor", "CodigoRecebedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Documento Recebedor", "CodigoDocumentoRecebedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IE Recebedor", "IERecebedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Localidade Recebedor", "LocalidadeRecebedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ Recebedor", "CPFCNPJRecebedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Expedidor", "Expedidor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Expedidor", "CodigoExpedidor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Documento Expedidor", "CodigoDocumentoExpedidor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IE Expedidor", "IEExpedidor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Localidade Expedidor", "LocalidadeExpedidor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ Expedidor", "CPFCNPJExpedidor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);


            grid.AdicionarCabecalho("Tomador", "Tomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Tomador", "CodigoTomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código Documento Tomador", "CodigoDocumentoTomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IE Tomador", "IETomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Localidade Tomador", "LocalidadeTomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Veículo (Tração)", "VeiculoTracao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Veículo (Reboque)", "VeiculoReboque", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Início Prestação", "InicioPrestacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IBGE Início Prestação", "IBGEInicioPrestacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Início Prestação", "UFInicioPrestacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Fim Prestação", "FimPrestacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IBGE Fim Prestação", "IBGEFimPrestacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Fim Prestação", "UFFimPrestacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Peso da Carga", "PesoPedido", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Peso (KG)", "PesoKg", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Peso Líquido (KG)", "PesoLiquidoKg", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Número Folha", "NumeroFolha", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Folha", "DataFolha", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Folha Calculada", "FolhaCalculada", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Folha Atribuída", "FolhaAtribuida", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Folha Cancelada", "FolhaCancelada", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Folha Inconsistente", "FolhaInconsistente", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Inconsistência Folha", "InconsistenciaFolha", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);



            for (int i = 0; i < componentes.Count; i++)
            {
                if (i < NumeroMaximoComplementos)
                {
                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + UltimaColunaDinanica.ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum, componentes[i].Codigo);
                    else
                    {
                        bool exibirComponenteTransportador = !repOcorrencia.ExisteBloqueioTransportadorPorComponente(componentes[i].Codigo);

                        if (exibirComponenteTransportador)
                            grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + UltimaColunaDinanica.ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum, componentes[i].Codigo);
                    }

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
