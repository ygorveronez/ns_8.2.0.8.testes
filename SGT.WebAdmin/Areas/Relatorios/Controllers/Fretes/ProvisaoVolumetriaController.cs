using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Fretes/ProvisaoVolumetria")]
    public class ProvisaoVolumetriaController : BaseController
    {
        #region Construtores

        public ProvisaoVolumetriaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R310_ProvisaoVolumetria;

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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Provisão e Volumetria", "Fretes", "ProvisaoVolumetria.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Fretes.ProvisaoVolumetria servicoRelatorioNFes = new Servicos.Embarcador.Relatorios.Fretes.ProvisaoVolumetria(unitOfWork, TipoServicoMultisoftware, Cliente);
                var listaNFes = await servicoRelatorioNFes.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);
                grid.setarQuantidadeTotal(listaNFes.Count);
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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
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
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = Request.GetIntParam("Codigo");

                byte[] xml = await new Servicos.Embarcador.NFe.NFe(unitOfWork, cancellationToken).DownloadXmlAsync(codigoNFe);

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

        public async Task<IActionResult> DownloadDANFE(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoNFe = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = await repositorioXMLNotaFiscal.BuscarXMLPorCodigoAsync(codigoNFe);

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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CNPJ Transportador", "CNPJEmpresaFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Empresa", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Placa", "PlacaFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão NF-e", "DataEmissaoFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Migração NF", "DataMigracaoNFFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série Nota Fiscal", "SerieNotaFiscal", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Chave NF-e", "ChaveNFe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CFOP", "CFOP", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Vol.", "Volumes", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("UF Origem", "UFOrigem", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunasLocalidades, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("UF Destino", "UFDestino", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Remetente", "CNPJRemetenteFormatado", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CNPJ Destinatário", "CNPJDestinatarioFormatado", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo do Tomador", "DescricaoTipoTomador", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Emissão CT-e", "DataEmissaoCTEFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CT-e", "CTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Série CT-e", "SerieCTe", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo CT-e", "TipoCTeFormatado", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave CTe", "ChaveCTe", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Devolução CTRC", "DataDevolucaoCTRCFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Importação", "DataImportacao", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Total a Receber", "TotalReceber", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Frete Total Sem Imposto", "FreteTotalSemImposto", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Data Aprovação", "DataAprovacaoFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nome Aprovador", "NomeAprovador", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Cód. UC", "CodUC", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("UC", "DescricaoUC", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Cód. Conta Contábil", "CodContaContabil", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Conta Contábil", "DescricaoContaContabil", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("CIA", "CIA", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Cód. Filial", "CodFilial", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Filial", "Filial", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Mercado", "Mercado", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Diretoria", "Diretoria", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            //grid.AdicionarCabecalho("Conta Gerencial", "ContaGerencial", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            //grid.AdicionarCabecalho("VA", "VA", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            //grid.AdicionarCabecalho("Doc. Enviado RI", "DocEnviadoRI", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Integração Pagamento", "DataIntegracaoPagamentoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Aprovação Pagamento", "DataAprovacaoPagamentoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Pagamento", "NumeroPagamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Pagamento", "SituacaoPagamentoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Erro Integração Pagamento", "ErroIntegracaoPagamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Pagamento", "DataPagamentoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação da Carga", "SituacaoCargaFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Pago", "Pago", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria()
            {
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigosFilial = Request.GetListParam<int>("Filial"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                DataEmissaoNFInicio = Request.GetNullableDateTimeParam("DataEmissaoNFInicio"),
                DataEmissaoNFFim = Request.GetNullableDateTimeParam("DataEmissaoNFFim"),
                DataIntegracaoPagamentoInicio = Request.GetNullableDateTimeParam("DataIntegracaoPagamentoInicio"),
                DataIntegracaoPagamentoFim = Request.GetNullableDateTimeParam("DataIntegracaoPagamentoFim"),
                DataVencimentoInicio = Request.GetNullableDateTimeParam("DataVencimentoInicio"),
                DataVencimentoFim = Request.GetNullableDateTimeParam("DataVencimentoFim"),
                TipoServicoMultisoftware = TipoServicoMultisoftware
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
