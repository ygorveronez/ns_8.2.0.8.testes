using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/NFe/NFes")]
    public class NFesController : BaseController
    {
        #region Construtores

        public NFesController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R089_NFes;
        private int UltimaColunaDinanica = 1;
        private int NumeroMaximoComplementos = 30;

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

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de NF-e(s)", "NFe", "NFes.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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
                grid.scrollHorizontal = IsLayoutClienteAtivo(unitOfWork);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFes.NFes servicoRelatorioNFes = new Servicos.Embarcador.Relatorios.NFes.NFes(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioNFes.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.NFes> listaNFes, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows(listaNFes);

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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
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

        [AllowAuthenticate]
        public async Task<IActionResult> GerarPlanilhaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPortalCliente();

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                parametrosConsulta.InicioRegistros = 0;
                parametrosConsulta.LimiteRegistros = 0;
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFes.NFes servicoRelatorioNFes = new Servicos.Embarcador.Relatorios.NFes.NFes(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioNFes.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.NFes> listaNFes, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(listaNFes);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", "RelatorioNFes" + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = Request.GetIntParam("Codigo");

                byte[] xml = new Servicos.Embarcador.NFe.NFe(unitOfWork).DownloadXml(codigoNFe);

                if (xml == null)
                    return new JsonpResult(false, true, "Não foi encontrado o arquivo XML da nota selecionada.");

                return Arquivo(xml, "text/xml", string.Concat("Nota_" + codigoNFe, ".xml"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> DownloadDANFE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoNFe = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repositorioXMLNotaFiscal.BuscarXMLPorCodigo(codigoNFe);

                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

                if (xmlNotaFiscal == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "DANFE Documentos Emitidos", xmlNotaFiscal.Chave + ".pdf");

                if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.XML) && xmlNotaFiscal.XML.Contains("</nfeProc>"))
                {
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, xmlNotaFiscal.XML, caminhoDANFE, false, false))
                        return new JsonpResult(false, true, erro);
                }
                else if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.XML) && xmlNotaFiscal.XML.Contains("</NFe>"))
                {
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, xmlNotaFiscal.XML, caminhoDANFE, false, true))
                        return new JsonpResult(false, true, erro);
                }
                else if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.XML) && xmlNotaFiscal.XML.Contains("nfeProc"))
                {
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, xmlNotaFiscal.XML, caminhoDANFE, true, false))
                        return new JsonpResult(false, true, erro);
                }
                else
                {
                    string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                    caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", xmlNotaFiscal.Chave + ".pdf");
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                        return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", xmlNotaFiscal.Chave + ".pdf");
                    else
                    {
                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", xmlNotaFiscal.Chave + ".xml");
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                            return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");

                        var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                        var retorno = z.GerarDANFENFeDocumentoDestinados(caminho, caminhoDANFE, unitOfWork);

                        if (retorno == "")
                            return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", xmlNotaFiscal.Chave + ".pdf");
                        else
                            return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");
                    }
                }
                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", xmlNotaFiscal.Chave + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o DANFE.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (IsLayoutClienteAtivo(unidadeDeTrabalho))
                return GridPortalCliente();

            UltimaColunaDinanica = 1;

            Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = repositorioComponenteFrete.BuscarTodosAtivos();
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Pedido", "Pedido", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Pedido", "DataPedido", TamanhoColunasLocalidades, Models.Grid.Align.center, true, false, false, false, true).DateTimeOnlyDate();
            grid.AdicionarCabecalho("Horário Pedido", "HoraPedido", TamanhoColunasLocalidades, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Peso Pedido", "PesoPedido", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Pedido", "ValorPedido", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Carga", "DataCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão CT-e", "DataEmissaoFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão NF-e", "DataNotaFiscal", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Rota", "Itinerario", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Rota da Carga", "RotaCarga", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Natureza NF", "NaturezaNF", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, true);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Filial", "Filial", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.AdicionarCabecalho("Código Transportador", "CodigoEmpresa", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("CNPJ Transportador", "CNPJEmpresa", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Transportador", "Empresa", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            }

            grid.AdicionarCabecalho("Código Remetente", "CodigoRemetente", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Remetente", "CNPJRemetente", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Endereço Remetente", "EnderecoRemetente", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Remetente", "GrupoRemetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Categoria Remetente", "CategoriaRemetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Código Destinatário", "CodigoDestinatario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Destinatário", "CNPJDestinatario", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Endereço Destinatário", "EnderecoDestinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Telefone Destinatário", "TelefoneDestinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("E-mail Destinatário", "EmailDestinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Destinatário", "GrupoDestinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Categoria Destinatário", "CategoriaDestinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Restrições", "Restricoes", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Expedidor", "NomeFantasiaExpedidor", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Código SAP Expedidor", "CodigoSapExpedidor", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Grupo Pessoa", "GrupoPessoa", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nota Fiscal", "NotasFiscais", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série Nota Fiscal", "SerieNotaFiscal", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Chave NF-e", "ChaveNFe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor NF-e", "ValorNFe", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso", "Peso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Vol.", "Volumes", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Número de Pallet", "NumeroPallet", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);

            grid.AdicionarCabecalho("Frete Valor", "Frete", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor dos Componentes", "ValorComponentes", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Frete Total Sem Imposto", "FreteTotalSemImposto", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Aliquota ICMS", "AliquotaICMS", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor ICMS", "ICMS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ICMS ST", "ValorICMSST", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ISS", "ValorISS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Aliquota ISS", "AliquotaISS", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor ISS Retido", "ValorISSRetido", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Total a Receber", "TotalReceber", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("CST", "CST", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CT-e", "CTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série CT-e", "SerieCTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo CT-e", "TipoCTeFormatado", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CFOP", "CFOP", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Frete Líquido", "FreteTotalLiquidoCTe", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Frete Total CT-e", "FreteTotalCTe", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do ICMS do CT-e", "ValorICMS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Motorista", "Motorista", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Placa", "Placa", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Capacidade Veículo", "CapacidadeVeiculo", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).NumberFormat("n2");
            grid.AdicionarCabecalho("Metro Cúbico", "MetroCubico", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Metro Cúbico NF-e", "MetroCubicoNFe", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);

            grid.AdicionarCabecalho("CST IBS/CBS", "CSTIBSCBS", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Classificação Tributária IBS/CBS", "ClassificacaoTributariaIBSCBS", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Base de Cálculo IBS/CBS", "BaseCalculoIBSCBS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota IBS Estadual", "AliquotaIBSEstadual", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percentual Redução IBS Estadual", "PercentualReducaoIBSEstadual", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Redução IBS Estadual", "ValorReducaoIBSEstadual", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor IBS Estadual", "ValorIBSEstadual", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota IBS Municipal", "AliquotaIBSMunicipal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percentual Redução IBS Municipal", "PercentualReducaoIBSMunicipal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Redução IBS Municipal", "ValorReducaoIBSMunicipal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor IBS Municipal", "ValorIBSMunicipal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota CBS", "AliquotaCBS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percentual Redução CBS", "PercentualReducaoCBS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Redução CBS", "ValorReducaoCBS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor CBS", "ValorCBS", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("Frete do Embarcador", "FreteEmbarcador", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
                grid.AdicionarCabecalho("Frete Calculado", "FreteTabelaFrete", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Produto", "Produto", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Codigo Produto", "CodigoProduto", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Codigo cEAN", "CodigocEAN", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Unidade Comercial", "UnidadeComercial", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
                grid.AdicionarCabecalho("Quantidade do Produto", "QuantidadeProduto", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            }
            else
                grid.AdicionarCabecalho("Qtd. Total Produto", "QuantidadeTotalProduto", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Valor Mercadoria CT-e", "ValorMercadoriaTotalCTe", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("CEP Destinatário", "CEPDestinatario", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("País Destinatário", "PaisDestinatario", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Previsão Entrega Pedido", "PrevisaoEntregaPedido", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Realizada Entrega", "DataRealizadaEntrega", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Atendimentos", "Atendimentos", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Situação Nota Fiscal", "SituacaoNotaFiscal", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Situação Entrega", "DescricaoSituacaoEntrega", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("SLA Documento", "SLADocumento", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Nome Vendedor", "NomeVendedor", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Telefone Vendedor", "TelefoneVendedor", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("KM Filial/Cliente", "KMFilialCliente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Mês Previsão Entrega Pedido", "MesPrevisaoEntregaPedido", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Semana Previsão Entrega Pedido", "SemanaPrevisaoEntregaPedido", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Número Remessa Espelho", "NumeroRemessaEspelho", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade Volumes", "QuantidadeVolumes", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Classificação NFe", "ClassificacaoNFeDescricao", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, ConfiguracaoEmbarcador.ExibirClassificacaoNFe);
            grid.AdicionarCabecalho("Nº do Pedido Cliente", "NumeroPedidoCliente", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Início Viagem", "DataInicioViagemFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação da Carga", "DescricaoSituacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Situação Nota Fiscal Entrega", "DataSituacaoNotaFiscalEntregaFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Situação Nota Fiscal Entrega", "DescricaoSituacaoNotaFiscalEntrega", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Observação", "ObservacaoPedido", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade de peças", "QuantidadePecas", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("ID Agrupador", "IdAgrupador", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código SAP Destinatário", "CodigoSAPDestinatario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Destino", "NumeroDestino", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("UF Origem", "UFOrigem", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("UF Destino", "UFDestino", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data da Entrega", "DataEntregaFormatada", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Prevista Carga Entrega", "DataPrevisaoCargaEntregaFormatada", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Natureza Nota Fiscal", "NaturezaOP", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Expedidor Carga", "ExpedidorCarga", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Recebedor Carga", "RecebedorCarga", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Codigo Integração Migo", "CodigoInteragracaoMigo", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Integração Migo", "DataMigoFormatado", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Codigo Integração Miro", "CodigoInteragracaoMiro", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Integração Miro", "DataMiroFormatado", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Transbordo", "Transbordo", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso Líquido", "PesoLiquido", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Pedido NFe", "NumeroPedidoNFe", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, true);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Fatura", "Fatura", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);

            //Colunas montadas dinamicamente
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                for (int i = 0; i < componentes.Count; i++)
                {
                    if (i < NumeroMaximoComplementos)
                    {
                        grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + UltimaColunaDinanica.ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum, componentes[i].Codigo);
                        UltimaColunaDinanica++;
                    }
                    else
                        break;
                }
            }

            return grid;
        }

        private Models.Grid.Grid GridPortalCliente()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>(),
                scrollHorizontal = true
            };

            grid.AdicionarCabecalho("CNPJ Remetente", "CNPJRemetente", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("CNPJ Destinatário", "CNPJDestinatario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Endereço Destinatário", "EnderecoDestinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunasLocalidades, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Peso", "Peso", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, true);
            grid.AdicionarCabecalho("Nota Fiscal", "NotasFiscais", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão NF-e", "DataNotaFiscal", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Valor NF-e", "ValorNFe", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("CT-e", "CTe", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Valor Mercadoria CT-e", "ValorMercadoriaTotalCTe", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão CT-e", "DataEmissaoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Fatura", "Fatura", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Status Entrega", "DescricaoSituacaoEntrega", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Realizada Entrega", "DataRealizadaEntrega", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Digitalização Canhoto", "DataDigitalizacaoCanhotoFormatada", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação Digitalização Canhoto", "DescricaoSituacaoDigitalizacaoCanhoto", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Motivos Atendimentos", "MotivosAtendimentos", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, true);

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

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                if (this.Usuario.ClienteFornecedor != null)
                {
                    if (this.Usuario.ClienteFornecedor.GrupoPessoas != null)
                        filtrosPesquisa.CodigoGrupoPessoaClienteFornecedor = this.Usuario.ClienteFornecedor.GrupoPessoas.Codigo;
                    else
                        filtrosPesquisa.CpfCnpjClienteFornecedor = this.Usuario.ClienteFornecedor.CPF_CNPJ;
                }

            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (!Empresa.Matriz.Any())
                {
                    Repositorio.Empresa empresa = new Repositorio.Empresa(unitOfWork);
                    List<int> codigosEmpresa = empresa.BuscarCodigoMatrizEFiliais(Usuario.Empresa?.CNPJ_SemFormato);
                    filtrosPesquisa.CodigosTransportador = codigosEmpresa?.Count > 0 ? codigosEmpresa : null;
                }
                else
                    filtrosPesquisa.CodigosTransportador = new List<int>() { Usuario.Empresa.Codigo };
            }

            return filtrosPesquisa;
        }

        #endregion
    }
}
