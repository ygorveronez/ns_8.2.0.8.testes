using Dominio.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/CTe/CustoRentabilidadeCteCrt")]
    public class CustoRentabilidadeCteCrtController : BaseController
    {
        #region Construtores

        public CustoRentabilidadeCteCrtController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R351_CustoRentabilidadeCteCrt;

        private const decimal TamanhoColunaPequena = 1.75m;
        private const decimal TamanhoColunaMedia = 3m;
        private const decimal TamanhoColunaGrande = 5.50m;

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Custos e Rentabilidade - CTE/CRT", "CTe", "CustoRentabilidadeCteCrt.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.CTes.CustoRentabilidadeCteCrt servicoRelatorioCustoRentabilidadeCteCrt = new Servicos.Embarcador.Relatorios.CTes.CustoRentabilidadeCteCrt(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCustoRentabilidadeCteCrt.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CustoRentabilidadeCteCrt> listaCTesSubcontratados, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(listaCTesSubcontratados);
                grid.setarQuantidadeTotal(countRegistros);

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

                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa = ObterFiltrosPesquisa();

                await svcRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
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

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCustoRentabilidadeCteCrt()
            {
                DataInicialEmissao = Request.GetNullableDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetNullableDateTimeParam("DataFinalEmissao"),

                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),

                Pedido = Request.GetStringParam("Pedido"),
                NotaFiscal = Request.GetStringParam("NFe"),
                Serie = Request.GetIntParam("Serie"),
                TipoServico = Request.GetListEnumParam<TipoServico>("TipoServico"),

                Situacao = Request.GetListParam<string>("Situacao"),
                CTeVinculadoACarga = Request.GetNullableBoolParam("CTeVinculadoACarga"),

                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                CpfCnpjDestinatarios = Request.GetListParam<double>("Destinatario"),
                CpfCnpjTomadores = Request.GetListParam<double>("Tomador"),

                CodigosVeiculo = Request.GetListParam<int>("Veiculo"),
                CodigosCarga = Request.GetListParam<int>("Carga"),
                CodigoFilial = Request.GetIntParam("Filial"),

                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CodigosCTe = Request.GetListParam<int>("CTe"),

                CodigosModeloDocumento = Request.GetListParam<int>("ModeloDocumento")
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("CodigoCargaOriginal", false, true);

            grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Cliente Tomador", "Tomador", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Carga de Venda", "CargaVenda", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nr. DT / CTE ", "NumeroDocumentoCTe", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Sigla da Filial", "CodigoIntegracaoFilial", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("Emissão DT/CTE", "DataEmissaoOriginal", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Bruto/CTE", "ValorBrutoOriginal", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Pis/CTE", "ValorPisOriginal", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Cofins/CTE", "ValorCofinsOriginal", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("ISS/NF", "ValorIssNfOriginal", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("ICMS/CTE", "ValorIcmsOriginal", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Líquido/CTE", "ValorLiquidoOriginal", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Transportadora", "TransportadoraOriginal", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Transportadora", "CnpjTransportadoraOriginalFormatado", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Nr. DT / Número da Provisão do sistema", "NumeroProvisao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Emissão DT Provisão", "DataEmissaoProvisao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Bruto Provisão", "ValorBrutoProvisao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Impostos Provisão", "ValorImpostoProvisao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Pis/Provisão", "ValorPisProvisao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Cofins/Provisão", "ValorCofinsProvisao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("ICMS/Provisão ", "ValorIcmsProvisao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Líquido Provisão", "ValorLiquidoProvisao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Emissão DT/CTE do Transportador", "DataEmissaoCTeEspelho", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Bruto CTE do Transportador", "ValorBrutoCTeEspelho", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Pis/CTE do Transportador", "ValorPisCTeEspelho", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Cofins/CTE do Transportador", "ValorCofinsCTeEspelho", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("ISS/NF do Transportador", "ValorIssNfEspelho", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("ICMS/CTE do Transportador", "ValorIcmsCTeEspelho", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Líquido CTE do Transportador", "ValorLiquidoCTeEspelho", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Transportadora do Espelho", "TransportadoraEspelho", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Transportadora do Espelho", "CnpjTransportadoraEspelhoFormatado", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Receita em Dolár", "ReceitaDolar", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Provisão Dolár", "ProvisaoDolar", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Despesa em Dolár", "DespesaDolar", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Taxa de Convesão da Moeda", "TaxaConversaoMoeda", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Rentabilidade", "Rentabilidade", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Rentabilidade Dolár", "RentabilidadeDolar", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
