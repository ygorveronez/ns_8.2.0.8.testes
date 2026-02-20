using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/FreteTerceirizadoPorCTe")]
    public class FreteTerceirizadoPorCTeController : BaseController
    {
		#region Construtores

		public FreteTerceirizadoPorCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R321_FreteTerceirizadoPorCTe;

        private int UltimaColunaDinanica = 1;
        private int NumeroMaximoComplementos = 15;

        private readonly decimal TamanhoColunaPequena = 1.75m;
        private readonly decimal TamanhoColunaGrande = 5.50m;
        private readonly decimal TamanhoColunaMedia = 3m;

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Fretes Terceirizados por CT-e", "Fretes", "FreteTerceirizadoPorCTe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "ContratoFrete", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(await GridPadrao(unitOfWork, cancellationToken), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Fretes.FreteTerceirizadoPorCTe servicoRelatorioFreteTerceirizadoPorCTe = new Servicos.Embarcador.Relatorios.Fretes.FreteTerceirizadoPorCTe(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioFreteTerceirizadoPorCTe.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoPorCTe> listaFrete, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaFrete);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
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

        private async Task<Models.Grid.Grid> GridPadrao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Contrato de Frete", "ContratoFrete", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CIOT", "NumeroCIOT", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Pedido Embarcador", "NumeroPedidoEmbarcador", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CT-e", "NumeroCTes", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("MDF-es", "NumeroMDFes", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);

            grid.AdicionarCabecalho("CPF/CNPJ Terceiro", "CPFCNPJTerceiroFormatado", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Terceiro", "Terceiro", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Dt. Nasc. Terceiro", "DataNascimentoTerceiro", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("PIS/PASEP Terceiro", "PISPASEPTerceiro", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Regime Tributário Terceiro", "RegimeTributarioTerceiroFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Pessoa Terceiro", "TipoPessoaTerceiroFormatado", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Loc. Remetente", "LocalidadeRemetente", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Loc. Destinatário", "LocalidadeDestinatario", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Vl. do ICMS", "ValorICMS", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Vl. do Frete", "ValorFrete", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Vl. a Receber", "ValorReceber", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("% Adiantamento", "PercentualAdiantamento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Vl. Adiantamento", "ValorAdiantamento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("% Abastecimento", "PercentualAbastecimento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Vl. Abastecimento", "ValorAbastecimento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false);
            grid.AdicionarCabecalho("Frete Sem Abastecimento", "ValorFreteMenosAbastecimento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false);
            grid.AdicionarCabecalho("Vl. Pago", "ValorPago", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Vl. Saldo", "ValorSaldo", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Vl. Bruto", "ValorBruto", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Vl. INSS", "ValorINSS", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Vl. SEST", "ValorSEST", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Vl. SENAT", "ValorSENAT", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Vl. IRRF", "ValorIRRF", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Acréscimos", "ValorAcrescimos", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Descontos", "ValorDescontos", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Encerramento", "DataEncerramento", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Dt. Autorização Pgto.", "DataAutorizacaoPagamento", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Dt. Vcto. Adiantamento", "DataVencimentoAdiantamento", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Dt. Pgto. Adiantamento", "DataPagamentoAdiantamento", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Dt. Vcto. Saldo", "DataVencimentoValor", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Dt. Pgto. Saldo", "DataPagamentoValor", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Títulos", "Titulos", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Segmento Veíc.", "SegmentoVeiculo", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Motoristas", "Motorista", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("RG", "RG", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Dt. Saque. Adiantamento", "DataSaqueAdiantamento", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Vl. Saque. Adiantamento", "ValorSaqueAdiantamento", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo documento", "TipoDocumento", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("CPF/CNPJ Filial Emissora", "CPFCNPJFilialEmissoraFormatado", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Filial Emissora", "FilialEmissora", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("UF Filial Emissora", "UFFilialEmissora", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("CNPJ Empresa/Filial", "CNPJEmpresa", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Empresa/Filial", "Empresa", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            }

            grid.AdicionarCabecalho("Situação", "DescricaoSituacaoContratoFrete", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Aprovação", "DataAprovacaoFormatada", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Dt. Abertura CIOT", "DataAberturaCIOT", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Dt. Encerramento CIOT", "DataEncerramentoCIOT", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("% Variação", "PercentualVariacao", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Centro Resultado", "CentroResultado", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Pedágio", "ValorPedagio", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Tipo Fornecedor", "TipoFornecedor", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Mês Competência", "MesCompetencia", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Ano Competência", "AnoCompetencia", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Descrição do serviço", "DescricaoServico", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Verba", "Verba", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Centro Resultado Empresa", "CentroResultadoEmpresa", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Município Lançamento", "MunicipioLancamento", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Estabelecimento", "CodigoEstabelecimento", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código da Empresa", "CodigoEmpresa", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Protocolo - CIOT", "ProtocoloCIOT", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Cód. Verificador CIOT", "CodigoVerificadorCIOT", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Expedidor", "Expedidor", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Localidade Expedidor", "LocalidadeExpedidor", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("UF Expedidor", "UFExpedidor", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Série CT-e", "SerieCTes", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Status CT-e", "StatusCTes", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("B.C. do ICMS", "BCICMS", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota ICMS", "AliquotaICMS", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso (Kg)", "PesoKg", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false);
            grid.AdicionarCabecalho("Nº Vale Pedágio", "NumeroValePedagio", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Prod. Predominante", "ProdutoPredominante", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            if (ConfiguracaoEmbarcador.SolicitarValorFretePorTonelada)
            {
                grid.AdicionarCabecalho("Vl. Negociado Pedido", "ValorFreteNegociado", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
                grid.AdicionarCabecalho("Vl. Terceiro Pedido", "ValorFreteTerceiro", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            }

            grid.AdicionarCabecalho("Total de Descontos", "TotalDescontos", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false);
            grid.AdicionarCabecalho("Saldo Frete", "SaldoFrete", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false);

            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetente", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatario", "CPFCNPJDestinatario", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF's Motoristas", "CPFMotorista", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Pedágio Manual", "ValorPedagioManual", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);

            //Colunas montadas dinamicamente
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = await repComponenteFrete.BuscarTodosAtivosAsync();
            for (int i = 0; i < componentes.Count; i++)
            {
                if (i < NumeroMaximoComplementos)
                {
                    grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + UltimaColunaDinanica.ToString(), TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false, componentes[i].Codigo);

                    UltimaColunaDinanica++;
                }
                else
                    break;
            }

            grid.AdicionarCabecalho("Tomador", "Tomador", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomadorFormatado", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor líquido sem adiantamento", "ValorLiquidoSemAdiantamento", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Subcontratação", "ValorSubcontratacao", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação da carga", "SituacaoCargaDescricao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Local Empresa/Filial", "LocalidadeEmpresaFilial", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor a Receber da Carga", "ValorReceberCTes", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor Total NF", "ValorTotalProdutosNotaFiscal", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Doc. Anterior", "NumeroDocAnterior", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Banco", "Banco", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Agência", "Agencia", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Conta", "Conta", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo da Conta", "TipoDaConta", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Titular", "Titular", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Pago Rateado", "ValorPagoPorCTeFormatado", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe()
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
                StatusCTe = Request.GetStringParam("StatusCTe"),
                NumeroCIOT = Request.GetStringParam("NumeroCIOT"),
                TipoCTe = Request.GetListEnumParam<TipoCTE>("TipoCTe"),
                TiposCargaTerceiros = Request.GetEnumParam<TiposCargaTerceiros>("TiposCargaTerceiros"),
                TipoProprietario = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo>("TipoProprietario"),
            };
        }

        #endregion
    }
}
