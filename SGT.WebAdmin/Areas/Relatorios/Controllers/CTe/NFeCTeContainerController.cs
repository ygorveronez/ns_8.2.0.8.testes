using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/NFeCTeContainer")]
    public class NFeCTeContainerController : BaseController
    {
		#region Construtores

		public NFeCTeContainerController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R218_NFeCTeContainer;
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

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de NFe x CTe x Container", "CTe", "NFeCTeContainer.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.NFeCTeContainer servicoRelatorioNFeCTeContainer = new Servicos.Embarcador.Relatorios.CTes.NFeCTeContainer(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioNFeCTeContainer.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.CTe.NFeCTeContainer> listaNFeCTeContainer, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows(listaNFeCTeContainer);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer filtrosPesquisa = ObterFiltrosPesquisa();

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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioNFeCTeContainer()
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
                FoiAnulado = Request.GetEnumParam<OpcaoSimNaoPesquisa>("FoiAnulado"),
                FoiSubstituido = Request.GetEnumParam<OpcaoSimNaoPesquisa>("FoiSubstituido")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Container", "ContainerDescricao", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº OS", "NumeroOS", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Viagem", "Viagem", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Navio Transbordo", "NavioTransbordo", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Terminal Origem", "TerminalOrigem", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Porto Destino", "PortoDestino", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Terminal Destino", "TerminalDestino", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Porto Transbordo", "PortoTransbordo", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Terminal Transbordo", "TerminalTransbordo", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Container", "TipoContainer", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);

            grid.AdicionarCabecalho("Expedidor", "Expedidor", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Expedidor", "CNPJExpedidor", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Remetente", "CNPJRemetente", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Destinatário", "CNPJDestinatario", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Recebedor", "CNPJRecebedor", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Tomador", "CNPJTomador", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Nº Lacre", "NumeroLacre", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tara", "Tara", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CTe", "NumeroCTe", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Nº Controle", "NumeroControle", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("UF Início", "UFInicioPrestacao", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("UF Fim", "UFFimPrestacao", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Status CTe", "StatusCTe", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Possui CCe CTe?", "PossuiCCe", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Produto Predominante", "ProdutoPredominante", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Carga IMO", "CargaIMO", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Carga", "SituacaoCargaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Nº Nota", "NumeroNota", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série Nota", "SerieNota", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão Nota", "DataEmissaoNotaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor Nota", "ValorNota", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso Nota", "PesoNota", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Chave Nota", "ChaveNota", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);

            grid.AdicionarCabecalho("Chave CTe Aquaviário", "ChaveCTeAquaviario", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave CTe Multimodal", "ChaveCTeMultimodal", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave CTe SVM", "ChaveCTeSVM", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Proposta Comercial", "PropostaComercial", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("OOG", "OOG", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Reefer", "Reefer", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("N° Referência EDI", "NumeroReferenciaEDI", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave CTe", "ChaveCTE", _tamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Nro. Pedido Embarcador", "NumeroPedidoEmbarcador", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("NCM", "NCM", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Master BL", "MasterBL", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Embarque", "Embarque", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("DI", "NumeroDI", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Numero Controle Cliente", "NumeroControleCliente", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Booking Reference Feeder", "BookingReferenceFeeder", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Aliquota ISS", "AliquotaISS", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do ISS", "ValorISS", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do ISS Retido", "ValorISSRetido", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
