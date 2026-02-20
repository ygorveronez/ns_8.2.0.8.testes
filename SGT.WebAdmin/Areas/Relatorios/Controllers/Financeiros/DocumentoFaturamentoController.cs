using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/DocumentoFaturamento")]
    public class DocumentoFaturamentoController : BaseController
    {
		#region Construtores

		public DocumentoFaturamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R070_DocumentoFaturamento;

        private decimal TamanhoColunasValores = (decimal)1.75;
        private decimal TamanhoColunasParticipantes = (decimal)5.50;
        private decimal TamanhoColunasLocalidades = 3;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de CT-es a Receber", "Financeiros", "DocumentoFaturamento.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Numero", "desc", "", "", Codigo, unitOfWork, true, false, 8);
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
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.DocumentoFaturamento servicocRelatorioDocumento = new Servicos.Embarcador.Relatorios.Financeiros.DocumentoFaturamento(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicocRelatorioDocumento.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamento> listaDocumentoFaturamento, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaDocumentoFaturamento);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

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
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            //grid.AdicionarCabecalho("Codigo", false);

            grid.AdicionarCabecalho("Número ", "Numero", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Série", "Serie", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            grid.AdicionarCabecalho("Nº Pedido Cliente", "NumeroPedidoCliente", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Ocorrência Cliente", "NumeroOcorrenciaCliente", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Doc. Originário", "NumeroDocumentoOriginario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Doc. Originário", "DataEmissaoDocumentoOriginario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Nº Ocorrência", "NumeroOcorrencia", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Doc", "DescricaoAbreviacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho("Filial", "Filial", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CNPJ Empresa", "CNPJEmpresaFormatado", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Empresa", "Empresa", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CNPJ Tomador", "CPFCNPJTomadorFormatado", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Cidade Tomador", "CidadeTomador", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Tomador", "GrupoTomador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Remetente", "CPFCNPJRemetenteFormatado", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Expedidor", "Expedidor", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Expedidor", "CPFCNPJExpedidorFormatado", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Recebedor", "CPFCNPJRecebedorFormatado", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Destinatario", "Destinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Destinatario", "CPFCNPJDestinatarioFormatado", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Frota", "Frotas", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Veículo", "Placas", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Motorista", "Motoristas", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Autorização", "DataAutorizacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Anulação", "DataAnulacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Cancelamento", "DataCancelamento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Dt. Envio Canhoto", "DataEnvioUltimoCanhoto", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Canhotos Recebidos", "CanhotosRecebidos", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Canhotos Digitalizados", "CanhotosDigitalizados", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunasLocalidades, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("UF Origem", "UFOrigem", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunasLocalidades, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("UF Destino", "UFDestino", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Faturas", "Faturas", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Vencimentos", "VencimentosFaturas", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Emissões de Faturas", "EmissoesFaturas", TamanhoColunasValores, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Código Documento", "CodigoDocumento", TamanhoColunasValores, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("% ICMS", "AliquotaICMS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("% ISS", "AliquotaISS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor ICMS", "ValorICMS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ISS", "ValorISS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Impostos", "ValorImpostos", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Valor de Acrescimo", "ValorAcrescimo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor de Desconto", "ValorDesconto", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Pago", "ValorPago", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor em Fatura", "ValorEmFatura", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor a Faturar", "ValorAFaturar", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do Documento", "ValorDocumento", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Chave de Acesso", "ChaveAcesso", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Títulos", "NumeroTitulos", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Status dos Títulos", "StatusTitulos", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Valor Pendente", "ValorPendente", TamanhoColunasValores, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Data Liquidação", "DataLiquidacao", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo do CT-e", "DescricaoTipoCTe", TamanhoColunasLocalidades, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", TamanhoColunasLocalidades, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Tipo de Serviço", "DescricaoTipoServico", TamanhoColunasLocalidades, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Base Liquidação", "DataBaseLiquidacao", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Observação da Fatura", "ObservacaoFatura", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº Notas Fiscais", "NumeroNotaFiscal", TamanhoColunasValores, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("CT-e Anterior", "NumeroDocumentoAnterior", TamanhoColunasValores, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valores Pagos", "ValoresPagos", TamanhoColunasValores, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Data Vencimento Título", "DataVencimentoTitulo", TamanhoColunasValores, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo de operação", "TipoOperacao", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, true, false);


            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento()
            {
                CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Empresa.Codigo : Request.GetIntParam("Transportador"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigoFilial = Request.GetIntParam("Filial"),
                ModeloDocumento = Request.GetIntParam("ModeloDocumento"),
                NumeroDocumentoOriginario = Request.GetIntParam("NumeroDocumentoOriginario"),
                NumeroOcorrencia = Request.GetIntParam("NumeroOcorrencia"),
                NumeroFatura = Request.GetIntParam("NumeroFatura"),

                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjTomador = Request.GetListParam<double>("Tomador"),

                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                DataAutorizacaoInicial = Request.GetDateTimeParam("DataAutorizacaoInicial"),
                DataAutorizacaoFinal = Request.GetDateTimeParam("DataAutorizacaoFinal"),
                DataCancelamentoInicial = Request.GetDateTimeParam("DataCancelamentoInicial"),
                DataCancelamentoFinal = Request.GetDateTimeParam("DataCancelamentoFinal"),
                DataAnulacaoInicial = Request.GetDateTimeParam("DataAnulacaoInicial"),
                DataAnulacaoFinal = Request.GetDateTimeParam("DataAnulacaoFinal"),
                DataBaseLiquidacaoInicial = Request.GetDateTimeParam("DataBaseLiquidacaoInicial"),
                DataBaseLiquidacaoFinal = Request.GetDateTimeParam("DataBaseLiquidacaoFinal"),

                ValorInicial = Request.GetDecimalParam("ValorInicial"),
                ValorFinal = Request.GetDecimalParam("ValorFinal"),

                TipoPropriedadeVeiculo = Request.GetStringParam("TipoPropriedadeVeiculo"),
                EstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                EstadoDestino = Request.GetStringParam("EstadoDestino"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                NumeroOcorrenciaCliente = Request.GetStringParam("NumeroOcorrenciaCliente"),

                GruposPessoas = Request.GetListParam<int>("GruposPessoas"),
                TipoOcorrencia = Request.GetListParam<int>("TipoOcorrencia"),
                GruposPessoasDiferente = Request.GetListParam<int>("GruposPessoasDiferente"),
                TipoFaturamento = Request.GetListEnumParam<TipoFaturamentoRelatorioDocumentoFaturamento>("TipoFaturamento"),

                Situacao = Request.GetNullableEnumParam<SituacaoDocumentoFaturamento>("Situacao"),
                TipoLiquidacao = Request.GetNullableEnumParam<TipoLiquidacaoRelatorioDocumentoFaturamento>("TipoLiquidacao"),

                DocumentoComCanhotosDigitalizados = Request.GetNullableBoolParam("DocumentoComCanhotosDigitalizados"),
                DocumentoComCanhotosRecebidos = Request.GetNullableBoolParam("DocumentoComCanhotosRecebidos"),
                TipoCTe = Request.GetListParam<int>("TipoCTe"),
                DataLiquidacaoInicial = Request.GetDateTimeParam("DataLiquidacaoInicial"),
                DataLiquidacaoFinal = Request.GetDateTimeParam("DataLiquidacaoFinal"),
                TipoServico = Request.GetListParam<int>("TipoServico"),
                TipoOperacao = Request.GetStringParam("TipoOperacao")
            };
        }

        #endregion
    }
}
