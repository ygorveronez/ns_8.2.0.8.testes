using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.IO.Compression;

namespace SGT.WebAdmin.Controllers.NFe
{

    public class PainelNFeTransporteController : BaseController
    {
        #region Contrutores

        public PainelNFeTransporteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        private decimal TamanhoColunasValores = (decimal)1.75;
        private decimal TamanhoColunasParticipantes = (decimal)5.50;
        private decimal TamanhoColunasLocalidades = 3;

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {

            
            Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = repositorioComponenteFrete.BuscarTodosAtivos();
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Pedido", "Pedido", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Peso Pedido", "PesoPedido", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Pedido", "ValorPedido", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Carga", "DataCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão NF-e", "DataNotaFiscal", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Rota", "Itinerario", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Código Remetente", "CodigoRemetente", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Remetente", "CNPJRemetente", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Endereço Remetente", "EnderecoRemetente", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Remetente", "GrupoRemetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Categoria Remetente", "CategoriaRemetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Código Destinatário", "CodigoDestinatario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Destinatário", "CNPJDestinatario", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Grupo Pessoa", "GrupoPessoa", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nota Fiscal", "NotasFiscais", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série Nota Fiscal", "SerieNotaFiscal", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Chave NF-e", "ChaveNFe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor NF-e", "ValorNFe", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso", "Peso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Vol.", "Volumes", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Aliquota ICMS", "AliquotaICMS", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor ICMS", "ICMS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ICMS ST", "ValorICMSST", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ISS", "ValorISS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Aliquota ISS", "AliquotaISS", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor ISS Retido", "ValorISSRetido", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("CST", "CST", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Placa", "Placa", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Quantidade Volumes", "QuantidadeVolumes", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Classificação NFe", "ClassificacaoNFeDescricao", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, ConfiguracaoEmbarcador.ExibirClassificacaoNFe);
            grid.AdicionarCabecalho("Nº do Pedido Cliente", "NumeroPedidoCliente", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Nota Fiscal Entrega", "DescricaoSituacaoNotaFiscalEntrega", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("UF Origem", "UFOrigem", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("UF Destino", "UFDestino", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Natureza Nota Fiscal", "NaturezaOP", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);

       
            grid.AdicionarCabecalho("Peso Líquido", "PesoLiquido", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Pedido NFe", "NumeroPedidoNFe", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        private Models.Grid.Grid GridNotasPendentesIntegracaoMercadoLivre()
        {

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data", "DataInclusao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "SituacaoDownloadNotasCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Mensagem", "MensagemRetorno", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, true);
           
            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigosCarga = Request.GetListParam<int>("Carga"),
                CodigosOrigem = Request.GetListParam<int>("Origem"),
                CodigosDestino = Request.GetListParam<int>("Destino"),
                CodigosGrupoPessoas = Request.GetListParam<int>("GrupoPessoas"),
                CodigosRestricoes = Request.GetListParam<int>("Restricao"),
                CodigosMotorista = Request.GetListParam<int>("Motorista"),
                CodigosNotasFiscais = Request.GetListParam<int>("NotaFiscal"),
                CpfCnpjsRemetente = Request.GetListParam<double>("Remetente"),
                CpfCnpjsDestinatario = Request.GetListParam<double>("Destinatario"),
                CpfCnpjsExpedidor = Request.GetListParam<double>("Expedidos"),
                EstadosOrigem = Request.GetListParam<string>("EstadoOrigem"),
                EstadosDestino = Request.GetListParam<string>("EstadoDestino"),
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                DataInicialEmissaoCTe = Request.GetDateTimeParam("DataInicialEmissaoCTe"),
                DataFinalEmissaoCTe = Request.GetDateTimeParam("DataFinalEmissaoCTe"),
                DataInicialEmissaoCarga = Request.GetDateTimeParam("DataInicialEmissaoCarga"),
                DataFinalEmissaoCarga = Request.GetDateTimeParam("DataFinalEmissaoCarga"),
                DataInicialPrevisaoEntregaPedido = Request.GetDateTimeParam("DataInicialPrevisaoEntregaPedido"),
                DataFinalPrevisaoEntregaPedido = Request.GetDateTimeParam("DataFinalPrevisaoEntregaPedido"),
                DataInicialInicioViagemPlanejada = Request.GetDateTimeParam("DataInicialInicioViagemPlanejada"),
                DataFinalInicioViagemPlanejada = Request.GetDateTimeParam("DataFinalInicioViagemPlanejada"),
                TipoLocalPrestacao = Request.GetEnumParam<TipoLocalPrestacao>("TipoLocalPrestacao"),
                SituacaoFatura = Request.GetNullableEnumParam<SituacaoFatura>("SituacaoFatura"),
                TiposCTe = Request.GetListEnumParam<TipoCTE>("TipoCTe"),
                QuantidadeVolumesInicial = Request.GetIntParam("QuantidadeVolumesInicial"),
                QuantidadeVolumesFinal = Request.GetIntParam("QuantidadeVolumesFinal"),
                ClassificacaoNFe = Request.GetEnumParam<ClassificacaoNFe>("ClassificacaoNFe"),
                NotasFiscaisSemCarga = Request.GetNullableBoolParam("NotasFiscaisSemCarga"),
                CargaTransbordo = Request.GetNullableBoolParam("CargaTransbordo"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                Situacoes = Request.GetListEnumParam<SituacaoCarga>("Situacoes"),
                SituacoesCargaMercante = Request.GetListEnumParam<SituacaoCargaMercante>("SituacaoCargaMercante"),
                SituacoesEntrega = Request.GetListEnumParam<SituacaoEntrega>("SituacoesEntrega"),
                FiltroDinamico = Request.GetStringParam("FiltroDinamico"),
                NumeroCanhoto = Request.GetIntParam("Canhoto"),
                StatusEntrega = Request.GetNullableEnumParam<SituacaoEntrega>("StatusEntrega"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                PossuiExpedidor = Request.GetIntParam("PossuiExpedidor"),
                PossuiRecebedor = Request.GetIntParam("PossuiRecebedor"),
                DataPrevisaoCargaEntregaInicial = Request.GetDateTimeParam("DataPrevisaoCargaEntregaInicial"),
                DataPrevisaoCargaEntregaFinal = Request.GetDateTimeParam("DataPrevisaoCargaEntregaFinal"),
                CodigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork)
            };

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosTipoCarga = Request.GetListParam<int>("TipoCarga");
            List<int> codigosTipoOperacao = Request.GetListParam<int>("TipoOperacao");

            filtrosPesquisa.CodigosFilial = codigosFilial.Count > 0 ? codigosFilial : ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = codigosTipoCarga.Count > 0 ? codigosTipoCarga : ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoOperacao = codigosTipoOperacao.Count > 0 ? codigosTipoOperacao : ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe ObterFiltros(int codigoCarga)
        {
            Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe();
            filtrosPesquisa.CodigosCarga = new List<int> { codigoCarga };
            filtrosPesquisa.CodigosTransportador = new List<int>();
            filtrosPesquisa.CodigosGrupoPessoas = new List<int>();
            filtrosPesquisa.CodigosOrigem = new List<int>();
            filtrosPesquisa.CodigosDestino = new List<int>();
            filtrosPesquisa.CodigosFilial = new List<int>();
            filtrosPesquisa.CpfCnpjsRemetente = new List<double>();
            filtrosPesquisa.CpfCnpjsDestinatario = new List<double>();
            filtrosPesquisa.CpfCnpjsExpedidor = new List<double>();
            filtrosPesquisa.EstadosOrigem = new List<string>();
            filtrosPesquisa.EstadosDestino = new List<string>();
            filtrosPesquisa.CodigosTipoCarga = new List<int>();
            filtrosPesquisa.CodigosTipoOperacao = new List<int>();
            filtrosPesquisa.CodigosRestricoes = new List<int>();
            filtrosPesquisa.CodigosNotasFiscais = new List<int>();
            filtrosPesquisa.CodigosMotorista = new List<int>();
            filtrosPesquisa.TiposCTe = new List<Dominio.Enumeradores.TipoCTE>();
            filtrosPesquisa.Situacoes = new List<SituacaoCarga>();
            filtrosPesquisa.SituacoesCargaMercante = new List<SituacaoCargaMercante>();
            filtrosPesquisa.SituacoesEntrega = new List<SituacaoEntrega>();
            filtrosPesquisa.CodigosFiliais = new List<int>();
            filtrosPesquisa.CodigosRecebedores = new List<double>();

            return filtrosPesquisa;
        }

        private bool IsZipEmpty(MemoryStream memoryStream)
        {
            //Reposiciona o Stream no início
            memoryStream.Position = 0;

            using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read, true))
            {
                return zipArchive.Entries.Count == 0;
            }
        }

        private bool ValidaEtapaCarga(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga)
        {
            return (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova != situacaoCarga &&
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete != situacaoCarga &&
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada != situacaoCarga &&
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada != situacaoCarga);
        }

        #endregion

        #region Métodos Publicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPadrao(unitOfWork);
                grid.scrollHorizontal = IsLayoutClienteAtivo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "PainelNFeTransporte/Pesquisa", "grid-painel-nfe-transporte");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Models.Grid.Relatorio gridlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFes.NFes servicoRelatorioNFes = new Servicos.Embarcador.Relatorios.NFes.NFes(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioNFes.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.NFes> listaNFes, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows(listaNFes);

                return await Task.FromResult<IActionResult>(new JsonpResult(grid));

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return await Task.FromResult<IActionResult>(new JsonpResult(false, "Ocorreu uma falha ao consultar."));
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargaNotasPendentesIntegracao(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridNotasPendentesIntegracaoMercadoLivre();
                grid.scrollHorizontal = IsLayoutClienteAtivo(unitOfWork);

                int CodigoCarga = Request.GetIntParam("Carga");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre SituacaoDownload = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre>("SituacaoDownload");

                Models.Grid.Relatorio gridlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                List<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre> listCargaNotasPendentesIntegracaoMercadoLivre = new List<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre>();
                Repositorio.Embarcador.PainelNFeTransporte.CargaNotasPendentesIntegracaoMercadoLivre repCargaNotasPendentesIntegracaoMercadoLivre = new Repositorio.Embarcador.PainelNFeTransporte.CargaNotasPendentesIntegracaoMercadoLivre(unitOfWork, cancellationToken);

                listCargaNotasPendentesIntegracaoMercadoLivre = await repCargaNotasPendentesIntegracaoMercadoLivre.BuscarPorCargaSituacaoAsync(CodigoCarga, SituacaoDownload);

                grid.setarQuantidadeTotal(listCargaNotasPendentesIntegracaoMercadoLivre.Count());
                grid.AdicionaRows(listCargaNotasPendentesIntegracaoMercadoLivre);

                return await Task.FromResult<IActionResult>(new JsonpResult(grid));

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return await Task.FromResult<IActionResult>(new JsonpResult(false, "Ocorreu uma falha ao consultar."));
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                Models.Grid.Relatorio relatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = GridPadrao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<PropriedadeAgrupamento> agrupamentos = relatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFes.NFes servicoRelatorioNFes = new Servicos.Embarcador.Relatorios.NFes.NFes(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioNFes.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.NFes> listaNFes, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows(listaNFes);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return await Task.FromResult<IActionResult>(new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo."));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return await Task.FromResult<IActionResult>(new JsonpResult(false, "Ocorreu uma falha ao exportar."));
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarCargaSemNota(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga();
                Repositorio.Embarcador.Cargas.Carga repcarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                carga = repcarga.PesquisaPorCodigoEmbarcador(codigoCarga.ToString());

                if (carga == null)
                    return await Task.FromResult<IActionResult>(new JsonpResult(true, $"Carga não encontrada com número {codigoCarga}."));

                if (!ValidaEtapaCarga(carga.SituacaoCarga))
                    return await Task.FromResult<IActionResult>(new JsonpResult(true, $"A Carga {codigoCarga} só pode ser adicionada após a etapa 3."));

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorCarga(carga.Codigo).FirstOrDefault();
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(carga.TipoOperacao.Codigo);

                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = ObterFiltros(carga.Codigo);
                Servicos.Embarcador.Relatorios.NFes.NFes servicoRelatorioNFes = new Servicos.Embarcador.Relatorios.NFes.NFes(unitOfWork, TipoServicoMultisoftware, Cliente);

                Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre cargaNotasPendentesIntegracaoMercadoLivre = new Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre();
                Repositorio.Embarcador.PainelNFeTransporte.CargaNotasPendentesIntegracaoMercadoLivre repCargaNotasPendentesIntegracaoMercadoLivre = new Repositorio.Embarcador.PainelNFeTransporte.CargaNotasPendentesIntegracaoMercadoLivre(unitOfWork, cancellationToken);

                if (await repCargaNotasPendentesIntegracaoMercadoLivre.ExisteCargaParaDownloadDeNotaAsync(carga.Codigo))
                    return await Task.FromResult<IActionResult>(new JsonpResult(true, $"A Carga {codigoCarga} já foi adicionada para o Download das Notas."));

                if (cargaIntegracaoMercadoLivre == null)
                {
                    if (tipoOperacao.TipoIntegracaoMercadoLivre == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre.HandlingUnit)
                    {
                        return await Task.FromResult<IActionResult>(new JsonpResult(true, $"A Carga {codigoCarga} não possui Handling Unit para realizar o Download das Notas na API do Mercado Livre."));
                    }
                    else
                    {
                        return await Task.FromResult<IActionResult>(new JsonpResult(true, $"A Carga {codigoCarga} não possui o Rota e Facility o Download das Notas na API do Mercado Livre."));
                    }
                }

                cargaNotasPendentesIntegracaoMercadoLivre.Carga = carga;
                cargaNotasPendentesIntegracaoMercadoLivre.SituacaoDownloadNotas = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Pendente;
                cargaNotasPendentesIntegracaoMercadoLivre.DataInclusao = DateTime.Now;
                cargaNotasPendentesIntegracaoMercadoLivre.MensagemRetorno = string.Empty;

                await repCargaNotasPendentesIntegracaoMercadoLivre.InserirAsync(cargaNotasPendentesIntegracaoMercadoLivre, Auditado, null, $"Carga {carga.CodigoCargaEmbarcador} adicionada para download das notas via API do Mercado Livre");

                return await Task.FromResult<IActionResult>(new JsonpResult(true));
                

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return await Task.FromResult<IActionResult>(new JsonpResult(false, "Ocorreu uma falha ao Adicionar a carga para o Download das Notas."));
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReprocessarCargaNotasPendentes(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre cargaNotasPendentesIntegracaoMercadoLivre = new Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre();
                Repositorio.Embarcador.PainelNFeTransporte.CargaNotasPendentesIntegracaoMercadoLivre repCargaNotasPendentesIntegracaoMercadoLivre = new Repositorio.Embarcador.PainelNFeTransporte.CargaNotasPendentesIntegracaoMercadoLivre(unitOfWork, cancellationToken);

                cargaNotasPendentesIntegracaoMercadoLivre = await repCargaNotasPendentesIntegracaoMercadoLivre.BuscarPorCodigoAsync(codigo, true);

                if(cargaNotasPendentesIntegracaoMercadoLivre.SituacaoDownloadNotas == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Concluido)
                    return await Task.FromResult<IActionResult>(new JsonpResult(true, "Não é possível reprocessar a carga com a situação concluído"));

                cargaNotasPendentesIntegracaoMercadoLivre.SituacaoDownloadNotas = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Pendente;
                
                await repCargaNotasPendentesIntegracaoMercadoLivre.AtualizarAsync(cargaNotasPendentesIntegracaoMercadoLivre, Auditado, null, $"Carga código {codigo} reprocessada");
                return await Task.FromResult<IActionResult>(new JsonpResult(true));
  
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return await Task.FromResult<IActionResult>(new JsonpResult(false, "Ocorreu uma falha ao reprocessar a carga."));
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadLoteXML(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.NFe.NFe servicoNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Models.Grid.Relatorio relatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = GridPadrao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = relatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFes.NFes servicoRelatorioNFes = new Servicos.Embarcador.Relatorios.NFes.NFes(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioNFes.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.NFes> listaNFes, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                if (countRegistros == 0)
                    return new JsonpResult(false, true, "Nenhum registro encontrado para o download em lote do XML.");

                if (countRegistros > 500)
                    return new JsonpResult(false, true, "Quantidade de NF-es para geração de lote inválida (" + countRegistros + "). É permitido o download de um lote com o máximo de 500 NF-es.");

                List<int> listaCodigoNFe = listaNFes.Select(c => c.Codigo).ToList();

                MemoryStream loteNotasXML = await servicoNFe.ObterLoteDeXML(listaCodigoNFe, unitOfWork);

                if(IsZipEmpty(loteNotasXML))
                    return new JsonpResult(false, true, "Nenhum XML encontrado para o download em lote.");

                return Arquivo(loteNotasXML, "application/zip", "LoteXMLNFe.zip");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do Lote de XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadLoteXMLDanfe(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.NFe.NFe servicoNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Models.Grid.Relatorio relatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = GridPadrao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = relatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFes.NFes servicoRelatorioNFes = new Servicos.Embarcador.Relatorios.NFes.NFes(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioNFes.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.NFes> listaNFes, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                if (countRegistros == 0)
                    return new JsonpResult(false, true, "Nenhum registro encontrado para o download em lote da DANFE.");

                if (countRegistros > 500)
                    return new JsonpResult(false, true, "Quantidade de NF-es para geração de lote inválida (" + countRegistros + "). É permitido o download de um lote com o máximo de 500 NF-es.");

                List<int> listaCodigoNFe = listaNFes.Select(c => c.Codigo).ToList();

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaXML = await repositorioXMLNotaFiscal.BuscarListaXMLPorCodigoAsync(listaCodigoNFe);

                MemoryStream danfes = new MemoryStream();

                string caminhoArquivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos;

                using (ZipOutputStream zipOStream = new ZipOutputStream(danfes))
                {
                    zipOStream.SetLevel(9);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xml in listaXML)
                    {
                        if (xml.Chave != null && !string.IsNullOrEmpty(xml.Chave))
                        {
                            if (xml.XML != null && !string.IsNullOrEmpty(xml.XML))
                            {
                                string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, "DANFE Documentos Emitidos", xml.Chave + ".pdf");

                                if (!string.IsNullOrWhiteSpace(xml.XML) && xml.XML.Contains("</nfeProc>"))
                                {
                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, xml.XML, caminhoDANFE, false, false))
                                        return new JsonpResult(false, true, erro);
                                }
                                else if (!string.IsNullOrWhiteSpace(xml.XML) && xml.XML.Contains("</NFe>"))
                                {
                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, xml.XML, caminhoDANFE, false, true))
                                        return new JsonpResult(false, true, erro);
                                }
                                else if (!string.IsNullOrWhiteSpace(xml.XML) && xml.XML.Contains("nfeProc"))
                                {
                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, xml.XML, caminhoDANFE, true, false))
                                        return new JsonpResult(false, true, erro);
                                }

                                byte[] damdfe = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE);

                                ZipEntry entry = new ZipEntry(string.Concat(xml.Chave, ".pdf"))
                                {
                                    DateTime = DateTime.Now
                                };

                                zipOStream.PutNextEntry(entry);
                                zipOStream.Write(damdfe, 0, damdfe.Length);
                                zipOStream.CloseEntry();
                            }
                        }
                    }

                    zipOStream.IsStreamOwner = false;
                    zipOStream.Close();
                }

                danfes.Position = 0;

                if (IsZipEmpty(danfes))
                    return new JsonpResult(false, true, "Nenhuma DANFE encontrada para o download em lote.");

                return Arquivo(danfes, "application/zip", "LoteDANFe.zip");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do Lote de XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
