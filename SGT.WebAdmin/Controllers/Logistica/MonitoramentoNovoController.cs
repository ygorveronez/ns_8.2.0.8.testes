using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/MonitoramentoNovo", "TorreControle/AcompanhamentoCarga")]
    public class MonitoramentoNovoController : MonitoramentoControllerBase
    {
        #region Construtores

        public MonitoramentoNovoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGrid());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaMapa()
        {
            try
            {
                return ObterDadosMapaSemLimites();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repositorioRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R343_MonitoramentoNovo, TipoServicoMultisoftware);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                if (relatorio == null)
                    relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R343_MonitoramentoNovo, TipoServicoMultisoftware, "Monitoramento Novo", "Monitoramento", "", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = new Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio()
                {
                    Report = relatorio.Codigo,
                    Codigo = relatorio.Codigo,
                    Titulo = relatorio.Titulo,
                    CortarLinhas = relatorio.CortarLinhas,
                    Descricao = relatorio.Descricao,
                    ExibirSumarios = relatorio.ExibirSumarios,
                    FontePadrao = relatorio.FontePadrao,
                    PropriedadeAgrupa = relatorio.PropriedadeAgrupa,
                    OrdemAgrupamento = relatorio.OrdemAgrupamento,
                    PropriedadeOrdena = relatorio.PropriedadeOrdena,
                    OrdemOrdenacao = "asc",
                    FundoListrado = false,
                    OrientacaoRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem,
                    TamanhoPadraoFonte = 10,
                    Padrao = true,
                    OcultarDetalhe = false,
                    RelatorioParaTodosUsuarios = false,
                    NovaPaginaAposAgrupamento = false,
                    Grid = Request.Params("GridRelatorio")
                };

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorio, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = ObterFiltrosPesquisa(configuracaoMonitoramento, configuracao, unitOfWork);

                svcRelatorio.AdicionarRelatorioParaGeracao(relatorio, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, Dominio.Enumeradores.TipoArquivoRelatorio.CSV, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Legendas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                List<dynamic> listGrupos = new List<dynamic>();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento)
                {
                    Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGrupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> gruposTipoOperacao = repGrupoTipoOperacao.BuscarAtivos();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao grupo in gruposTipoOperacao)
                    {
                        listGrupos.Add(new
                        {
                            grupo.Descricao,
                            grupo.Cor
                        });
                    }
                }
                else
                {
                    Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);
                    Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem> gruposStatusViagem = repMonitoramentoGrupoStatusViagem.BuscarAtivos();
                    foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem grupo in gruposStatusViagem)
                    {
                        listGrupos.Add(new
                        {
                            grupo.Descricao,
                            grupo.Cor
                        });
                    }
                }

                // Categorias das pessoas
                Repositorio.Embarcador.Pessoas.CategoriaPessoa repoCategoria = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> listCategorias = repoCategoria.BuscarTodos();

                return new JsonpResult(new
                {
                    Grupos = listGrupos,
                    Categorias = listCategorias
                });

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterLocaisRaioProximidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.Locais repositorioLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                Repositorio.Embarcador.Logistica.RaioProximidade repositorioRaioProximidade = new Repositorio.Embarcador.Logistica.RaioProximidade(unitOfWork);

                List<dynamic> dynlocais = JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("LocaisRaioProximidade"));
                List<int> codigosLocais = new List<int>();

                foreach (dynamic d in dynlocais)
                {
                    if (d.Codigo > 0)
                        codigosLocais.Add((int)d.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Logistica.Locais> locais = repositorioLocais.BuscarPorCodigos(codigosLocais);
                List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> raiosProximidade = repositorioRaioProximidade.BuscarPorCodigosLocais(codigosLocais);
                List<dynamic> coordenadasLocal = new List<dynamic>();


                foreach (Dominio.Entidades.Embarcador.Logistica.Locais local in locais)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.AreaLocal> areasLocal = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.AreaLocal>>(local.Area);

                    foreach (AreaLocal area in areasLocal)
                        coordenadasLocal.Add(new { Codigo = local.Codigo, Area = area });

                }

                coordenadasLocal = coordenadasLocal.Where(coordenada => coordenada.Area.type == "marker").ToList();

                var retorno = new
                {
                    Locais = (
                    from local in locais
                    select new
                    {
                        local.Codigo,
                        local.Descricao,
                        Coordenadas = coordenadasLocal.Where(coordenada => coordenada.Codigo == local.Codigo),
                        local.Tipo,
                        Raios = (
                        from raio in raiosProximidade
                        select new
                        {
                            CodigoLocal = raio.Local.Codigo,
                            raio.Raio,
                            raio.Cor,
                            raio.Identificacao
                        }).Where(r => r.CodigoLocal == local.Codigo).ToList(),
                    }).Where(local => local.Tipo == TipoLocal.RaioProximidade).ToList(),
                };


                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesPedidos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido> listaDetalhesPedidos = await servicoPedido.ObterDetalhesPedidosAsync(codigoCarga, unitOfWork);
                return new JsonpResult(listaDetalhesPedidos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarDetalhesPedidos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = await repositorioConfiguracaoAquivo.BuscarPrimeiroRegistroAsync();
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                List<Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido> listaDetalhesPedidos = await servicoPedido.ObterDetalhesPedidosAsync(codigoCarga, unitOfWork);
                if (listaDetalhesPedidos.Count == 0) throw new ControllerException("Não foram encontrados detalhes para a carga informada.");

                string guidArquivo = Guid.NewGuid().ToString() + ".xlsx";
                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoRelatoriosEmbarcador, guidArquivo);
                FileInfo fileInfo = new FileInfo(caminho);

                List<string> headersProdutos = new()
                {
                    "Farol",
                    "Protocolo Pedido",
                    "Carga",
                    "Peso Total",
                    "Tipo de Operação",
                    "Modelo Veicular",
                    "Carga Critica",
                    "Pedido Crítico",
                    "Filial",
                    "Escritorio de Venda",
                    "Canal de Venda",
                    "Notas Fiscais",
                    "Tipo de Mercado",
                    "Equipe de Venda",
                    "Canal de Entrega",
                    "Item",
                    "Destinatário",
                    "Destino",
                    "Data de Faturamento",
                    "Data de Entrega",
                    "Previsão Entrega",
                    "Data de Entrega Ajustada",
                    "Data de Entrega Atualizada (ETA)",
                    "Última Observação",
                    "Código Integração Produto",
                    "Descrição Produto"
                };
                List<string> headersOcorrencias = new()
                {
                    "Protocolo Pedido",
                    "Carga",
                    "Ocorrência",
                    "Latitude",
                    "Longitude",
                    "Data da Ocorrência",
                    "Data Posição",
                    "Data Previsão Reprogramada",
                    "Tempo Percurso",
                    "Distancia Até o Destino",
                    "Origem"
                };

                List<string> headersOcorrenciasComerciais = new()
                {
                    "Protocolo Pedido",
                    "Carga",
                    "Ocorrência",
                    "Latitude",
                    "Longitude",
                    "Data da Ocorrência",
                    "Data Posição",
                    "Data Previsão Reprogramada",
                    "Tempo Percurso",
                    "Distancia Até o Destino",
                    "Origem",
                    "Natureza",
                    "Grupo de Ocorrência",
                    "Razão",
                    "Nota Fiscal de Devolução",
                    "Solicitação do Cliente",
                };

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Pedidos");

                    //Preenche o cabeçalho da planilha.
                    foreach ((string header, int i) in headersProdutos.Select((header, i) => (header, i)))
                        worksheet.Cells[1, i + 1].Value = header;

                    int linhaIndexProdutos = 2;
                    //Preenche o conteúdo da planilha
                    foreach (Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido pedido in listaDetalhesPedidos)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Produto produto in pedido.Produtos)
                        {
                            worksheet.Cells[linhaIndexProdutos, 01].Value = pedido.CargaEntrega?.FarolStatus ?? "#a19d9c";
                            worksheet.Cells[linhaIndexProdutos, 02].Value = pedido.ProtocoloIntegracao.ToString() ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 03].Value = pedido.Carga?.CodigoCargaEmbarcador ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 04].Value = pedido.PesoTotal ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 05].Value = pedido.TipoOperacao?.Descricao ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 06].Value = pedido.ModeloVeicular?.Descricao ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 07].Value = pedido.Carga?.CargaCriticaDescricao ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 08].Value = pedido.PedidoCritico ? "Sim" : "Não";
                            worksheet.Cells[linhaIndexProdutos, 09].Value = pedido.Filial?.Descricao ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 10].Value = pedido.EscritorioVenda ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 11].Value = pedido.CanalVenda ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 12].Value = pedido.NotasFiscais.Count > 0 ? string.Join(",", pedido.NotasFiscais.Select(nota => nota.NumeroNota).ToList()) : "-";
                            worksheet.Cells[linhaIndexProdutos, 13].Value = pedido.TipoMercadoria ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 14].Value = pedido.EquipeVendas ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 15].Value = pedido.CanalEntrega ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 16].Value = pedido.Adicional7 ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 17].Value = pedido.Destinatario?.Nome ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 18].Value = pedido.Destinatario?.Endereco?.EnderecoCompleto ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 19].Value = pedido.DataFaturamento ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 20].Value = pedido.CargaEntrega?.DataEntregaFormatada ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 21].Value = pedido.CargaEntrega?.DataPrevisaoEntregaFormatada ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 22].Value = pedido.CargaEntrega?.DataPrevisaoEntregaAjustadaFormatada ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 23].Value = pedido.CargaEntrega?.DataReprogramadaFormatada ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 24].Value = pedido.Observacao ?? "-";
                            worksheet.Cells[linhaIndexProdutos, 25].Value = produto.CodigoProdutoEmbarcador;
                            worksheet.Cells[linhaIndexProdutos, 26].Value = produto.Descricao;

                            linhaIndexProdutos++;
                        }
                    }

                    worksheet = package.Workbook.Worksheets.Add("Ocorrências");

                    //Preenche o cabeçalho da planilha.
                    foreach ((string header, int i) in headersOcorrencias.Select((header, i) => (header, i)))
                        worksheet.Cells[1, i + 1].Value = header;

                    int linhaIndexOcorrencias = 2;
                    //Preenche o conteúdo da planilha
                    foreach ((Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido pedido, int i) in listaDetalhesPedidos.Select((produto, i) => (produto, i)))
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Ocorrencia ocorrencia in pedido.Ocorrencias)
                        {
                            worksheet.Cells[linhaIndexOcorrencias, 01].Value = pedido.ProtocoloIntegracao.ToString() ?? "-";
                            worksheet.Cells[linhaIndexOcorrencias, 02].Value = pedido.Carga?.CodigoCargaEmbarcador ?? "-";
                            worksheet.Cells[linhaIndexOcorrencias, 03].Value = ocorrencia.Descricao;
                            worksheet.Cells[linhaIndexOcorrencias, 04].Value = ocorrencia.Latitude;
                            worksheet.Cells[linhaIndexOcorrencias, 05].Value = ocorrencia.Longitude;
                            worksheet.Cells[linhaIndexOcorrencias, 06].Value = ocorrencia.DataOcorrencia;
                            worksheet.Cells[linhaIndexOcorrencias, 07].Value = ocorrencia.DataPosicao;
                            worksheet.Cells[linhaIndexOcorrencias, 08].Value = ocorrencia.DataReprogramada;
                            worksheet.Cells[linhaIndexOcorrencias, 09].Value = ocorrencia.TempoPercurso;
                            worksheet.Cells[linhaIndexOcorrencias, 10].Value = ocorrencia.Distancia;
                            worksheet.Cells[linhaIndexOcorrencias, 11].Value = ocorrencia.Origem;

                            linhaIndexOcorrencias++;
                        }
                    }

                    worksheet = package.Workbook.Worksheets.Add("Ocorrências Comerciais");

                    //Preenche o cabeçalho da planilha.
                    foreach ((string header, int i) in headersOcorrenciasComerciais.Select((header, i) => (header, i)))
                        worksheet.Cells[1, i + 1].Value = header;

                    int linhaIndexOcorrenciasComerciais = 2;
                    //Preenche o conteúdo da planilha
                    foreach ((Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido pedido, int i) in listaDetalhesPedidos.Select((produto, i) => (produto, i)))
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Ocorrencia ocorrencia in pedido.Ocorrencias)
                        {
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 01].Value = pedido.ProtocoloIntegracao.ToString() ?? "-";
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 02].Value = pedido.Carga?.CodigoCargaEmbarcador ?? "-";
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 03].Value = ocorrencia.Descricao;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 04].Value = ocorrencia.Latitude;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 05].Value = ocorrencia.Longitude;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 06].Value = ocorrencia.DataOcorrencia;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 07].Value = ocorrencia.DataPosicao;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 08].Value = ocorrencia.DataReprogramada;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 09].Value = ocorrencia.TempoPercurso;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 10].Value = ocorrencia.Distancia;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 11].Value = ocorrencia.Origem;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 11].Value = ocorrencia.Natureza;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 11].Value = ocorrencia.GrupoOcorrencia;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 11].Value = ocorrencia.SolicitacaoCliente;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 11].Value = ocorrencia.NotaFiscalDevolucao;
                            worksheet.Cells[linhaIndexOcorrenciasComerciais, 11].Value = ocorrencia.Razao;

                            linhaIndexOcorrenciasComerciais++;
                        }
                    }

                    package.Save();
                }

                if (!(await Utilidades.IO.FileStorageService.Storage.ExistsAsync(caminho, cancellationToken)))
                    throw new ControllerException("Ocorreu uma falha ao exportar o arquivo.");

                byte[] arquivo = await Utilidades.IO.FileStorageService.Storage.ReadAllBytesAsync(caminho, cancellationToken);
                await Utilidades.IO.FileStorageService.Storage.DeleteIfExistsAsync(caminho, cancellationToken);

                return Arquivo(arquivo, "application/vnd.ms-excel", $"Detalhes dos Pedidos Carga {listaDetalhesPedidos[0].Carga.CodigoCargaEmbarcador}.xlsx");
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarConfiguracaoUsuarioMonitoramentoMapa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa repositorioConfiguracaoUsuarioMonitoramentoMapa = new Repositorio.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa(unitOfWork);

                Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa configuracaoWidgetAcompanhamentoCargaUsuario = await repositorioConfiguracaoUsuarioMonitoramentoMapa.BuscarConfiguracaoPorUsuarioAsync(this.Usuario.Codigo);

                if (configuracaoWidgetAcompanhamentoCargaUsuario != null)
                {
                    configuracaoWidgetAcompanhamentoCargaUsuario.ConfiguracaoExibicaoIndicadores = Request.GetStringParam("ConfiguracaoIndicadorUsuarioMonitoramentoMapa");
                    configuracaoWidgetAcompanhamentoCargaUsuario.ConfiguracaoExibicaoLegendaMapa = Request.GetStringParam("ConfiguracaoLegendaUsuarioMonitoramentoMapa");

                    await repositorioConfiguracaoUsuarioMonitoramentoMapa.AtualizarAsync(configuracaoWidgetAcompanhamentoCargaUsuario);
                }
                else
                {
                    configuracaoWidgetAcompanhamentoCargaUsuario = new Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa()
                    {
                        Usuario = this.Usuario,
                        ConfiguracaoExibicaoIndicadores = Request.GetStringParam("ConfiguracaoIndicadorUsuarioMonitoramentoMapa"),
                        ConfiguracaoExibicaoLegendaMapa = Request.GetStringParam("ConfiguracaoLegendaUsuarioMonitoramentoMapa"),
                    };

                    await repositorioConfiguracaoUsuarioMonitoramentoMapa.InserirAsync(configuracaoWidgetAcompanhamentoCargaUsuario);

                }

                return new JsonpResult(true, Localization.Resources.Gerais.Geral.ConfiguracoesSalvasComSucesso);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoSalvar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracaoUsuarioMonitoramentoMapa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa repositorioConfiguracaoUsuarioMonitoramentoMapa = new Repositorio.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa(unitOfWork);

                Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoUsuarioMonitoramentoMapa configuracaoWidgetAcompanhamentoCargaUsuario = await repositorioConfiguracaoUsuarioMonitoramentoMapa.BuscarConfiguracaoPorUsuarioAsync(this.Usuario.Codigo);

                var retorno = new
                {
                    configuracaoWidgetAcompanhamentoCargaUsuario?.ConfiguracaoExibicaoIndicadores,
                    configuracaoWidgetAcompanhamentoCargaUsuario?.ConfiguracaoExibicaoLegendaMapa
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoSalvar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosFiltroCarrosselMonitoramentoMapa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterDadosFiltroCarrossel();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos privados

        private Models.Grid.Grid ObterGrid()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = ObterFiltrosPesquisa(configuracaoMonitoramento, configuracao, unitOfWork);

                return ObterGridPesquisa(Request, this.Usuario.Codigo, filtrosPesquisa, configuracao, configuracaoMonitoramento, unitOfWork, this.TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                throw new Exception(Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        private IActionResult ObterDadosMapaSemLimites()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = ObterFiltrosPesquisa(configuracaoMonitoramento, configuracao, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

                int totalRegistros = repositorioMonitoramento.ContarConsultaView(filtrosPesquisa);

                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> retornoConsulta = totalRegistros > 0 ? repositorioMonitoramento.ConsultarMonitoramentoSimplificadoMapa(filtrosPesquisa, null) : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();

                if (filtrosPesquisa?.LocaisRaioProximidade.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.Locais> locais = filtrosPesquisa.RaiosProximidade.Select(x => x.Local).Distinct().ToList();
                    List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> raios = filtrosPesquisa.RaiosProximidade;

                    foreach (Monitoramento monitoramento in retornoConsulta)
                    {
                        Dominio.Entidades.Embarcador.Logistica.RaioProximidade raioVeiculoEmAreaRaio = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarRaioVeiculoEmAreaRaioProximidade(raios.ToArray(), monitoramento.Latitude, monitoramento.Longitude);
                        if (raioVeiculoEmAreaRaio != null)
                        {
                            monitoramento.CodigoLocalRaiosProximidade = raioVeiculoEmAreaRaio.Local.Codigo;
                            monitoramento.DescricaoLocalRaiosProximidade = raioVeiculoEmAreaRaio.Local.Descricao;
                            monitoramento.CorLocalRaiosProximidade = raioVeiculoEmAreaRaio.Cor;
                            monitoramento.CodigoRaioProximidade = raioVeiculoEmAreaRaio.Codigo;
                            monitoramento.DescricaoRaioProximidade = raioVeiculoEmAreaRaio.Identificacao;
                            monitoramento.RaioRaioProximidade = raioVeiculoEmAreaRaio.Raio;
                        }
                    }
                }

                return new JsonpResult(retornoConsulta);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                throw new Exception(Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        private IActionResult ObterDadosFiltroCarrossel()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = ObterFiltrosPesquisa(configuracaoMonitoramento, configuracao, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

                int totalRegistros = repositorioMonitoramento.ContarConsultaView(filtrosPesquisa);

                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> retornoConsulta = totalRegistros > 0 ? repositorioMonitoramento.ConsultarContadorFiltroCarrosselSimplificado(filtrosPesquisa, null) : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();

                var retorno = (from obj in retornoConsulta
                               select new
                               {
                                   obj.Codigo,
                                   obj.Status,
                                   obj.StatusDescricao,
                                   StatusViagem = obj.StatusViagem != null ? obj.StatusViagem : "Nenhum",
                                   obj.CorStatusViagem,
                                   obj.TiporRegraViagem,
                                   obj.TipoOperacao,
                                   obj.GrupoStatusViagemCor,
                                   obj.GrupoStatusViagemCodigo,
                                   GrupoStatusViagemDescricao = obj.GrupoStatusViagemDescricao != null ? obj.GrupoStatusViagemDescricao : "Nenhum",
                                   obj.PossuiAlertaEmAberto,
                                   obj.Carga,
                                   obj.SituacaoCarga,
                                   SituacaoCargaDescricao = obj.SituacaoCarga.ObterDescricao(),
                                   SituacaoCargaCor = obj.SituacaoCarga.ObterCorMonitoramento(),
                                   obj.TendenciaProximaParada,
                                   obj.CorTendenciaEntrega,
                                   obj.RastreadorOnlineOffline,
                                   TendenciaEntregaDescricao = obj.TendenciaProximaParadaDescricao != "-" ? obj.TendenciaProximaParadaDescricao : "Nenhum",
                                   UtilizaGrupoStatusViagem = configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento && configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem,
                                   obj.CodigoGrupoTipoOperacao,
                                   obj.GrupoTipoOperacao,
                                   obj.GrupoTipoOperacaoCor
                               }).ToList();


                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                throw new Exception(Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento ObterFiltrosPesquisa(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                RastreadorOnlineOffline = Request.GetNullableBoolParam("RastreadorOnlineOffline"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                DescricaoAlerta = Request.GetStringParam("DescricaoAlerta"),
                Status = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus>("MonitoramentoStatus"),
                CodigosStatusViagem = Request.GetListParam<int>("StatusViagem"),
                CodigosGrupoTipoOperacao = Request.GetListParam<int>("GrupoTipoOperacao"),
                GrupoTipoOperacaoIndicador = Request.GetNullableIntParam("GrupoTipoOperacaoIndicador"), 
                CodigosTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? new List<int>() { this.Empresa.Codigo } : Request.GetListParam<int>("Transportador"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                SomenteRastreados = Request.GetBoolParam("SomenteRastreados"),
                SomenteUltimoPorCarga = Request.GetBoolParam("SomenteUltimoPorCarga"),
                FiltroCliente = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente>("FiltroCliente"),
                Cliente = Request.GetDoubleParam("Cliente"),
                CodigoCategoriaPessoa = Request.GetIntParam("CategoriaPessoa"),
                CodigoFuncionarioVendedor = Request.GetIntParam("FuncionarioVendedor"),
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                DataEntregaPedidoInicio = Request.GetDateTimeParam("DataEntregaPedidoInicio"),
                DataEntregaPedidoFinal = Request.GetDateTimeParam("DataEntregaPedidoFinal"),
                PrevisaoEntregaInicio = Request.GetDateTimeParam("PrevisaoEntregaInicio"),
                PrevisaoEntregaFinal = Request.GetDateTimeParam("PrevisaoEntregaFinal"),
                CodigosExpedidores = Request.GetListParam<long>("Expedidor"),
                VeiculosComContratoDeFrete = Request.GetBoolParam("VeiculosComContratoDeFrete"),
                CodigosOrigem = Request.GetListParam<int>("Origem"),
                CodigosDestinos = Request.GetListParam<int>("Destino"),
                CodigoClienteDestino = Request.GetListParam<double>("ClienteDestino"),
                CodigoClienteOrigem = Request.GetListParam<double>("ClienteOrigem"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                EstadosOrigem = Request.GetListParam<string>("EstadoOrigem"),
                EstadosDestino = Request.GetListParam<string>("EstadoDestino"),
                DataEmissaoNFeFim = Request.GetDateTimeParam("DataEmissaoNFeFim"),
                DataEmissaoNFeInicio = Request.GetDateTimeParam("DataEmissaoNFeInicio"),
                CodigosResponsavelVeiculo = Request.GetListParam<int>("ResponsavelVeiculo"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                CodigosFronteiraRotaFrete = Request.GetListParam<double>("FronteiraRotaFrete"),
                CodigosPaisDestino = Request.GetListParam<int>("PaisDestino"),
                CodigosPaisOrigem = Request.GetListParam<int>("PaisOrigem"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                ApenasMonitoramentosCriticos = Request.GetBoolParam("ApenasMonitoramentosCriticos"),
                VeiculosEmLocaisTracking = Request.GetBoolParam("VeiculosEmLocaisTracking"),
                LocaisTracking = Request.GetListParam<int>("LocaisTracking"),
                DataInicioCarregamento = Request.GetDateTimeParam("DataInicioCarregamento"),
                DataFimCarregamento = Request.GetDateTimeParam("DataFimCarregamento"),
                PossuiRecebedor = Request.GetNullableBoolParam("PossuiRecebedor"),
                PossuiExpedidor = Request.GetNullableBoolParam("PossuiExpedidor"),
                Destinatario = Request.GetListParam<double>("Destinatario"),
                Recebedores = Request.GetListParam<double>("Recebedores"),
                CodigoCargaEmbarcadorMulti = Request.GetListParam<int>("CodigoCargaEmbarcadorMulti"),
                CodigosTiposTrecho = Request.GetListParam<int>("TipoTrecho"),
                InicioViagemPrevistaInicial = Request.GetDateTimeParam("InicioViagemPrevistaInicial"),
                InicioViagemPrevistaFinal = Request.GetDateTimeParam("InicioViagemPrevistaFinal"),
                CodigoMotorista = Request.GetIntParam("CodigoMotorista"),
                Remetente = Request.GetListParam<double>("Remetente"),
                TiposCarga = Request.GetListParam<int>("TipoCarga"),
                Produtos = Request.GetListParam<int>("Produtos"),
                DataRealEntrega = Request.GetDateTimeParam("DataRealEntrega"),
                LocaisRaioProximidade = Request.GetListParam<int>("LocaisRaioProximidade"),
                MostrarRaiosProximidade = Request.GetNullableBoolParam("MostrarRaiosProximidade"),
                DataAgendamentoPedidoInicial = Request.GetDateTimeParam("DataAgendamentoPedidoInicial"),
                DataAgendamentoPedidoFinal = Request.GetDateTimeParam("DataAgendamentoPedidoFinal"),
                DataColetaPedidoInicial = Request.GetDateTimeParam("DataColetaPedidoInicial"),
                DataColetaPedidoFinal = Request.GetDateTimeParam("DataColetaPedidoFinal"),
                CodigosClienteComplementar = Request.GetListParam<double>("ClientesComplementar"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                CanalVenda = Request.GetIntParam("CanalVenda"),
                SituacaoIntegracaoSM = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracaoSM"),
                VeiculoNoRaio = Request.GetBoolParam("VeiculoNoRaio"),
                CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                ModalTransporte = Request.GetEnumParam("TipoCobrancaMultimodal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.Nenhum),
                EquipeVendas = Request.GetStringParam("EquipeVendas"),
                Vendedor = Request.GetListParam<int>("Vendedor"),
                Supervisor = Request.GetListParam<int>("Supervisor"),
                TipoMercadoria = Request.GetStringParam("TipoMercadoria"),
                EscritorioVenda = Request.GetStringParam("EscritorioVenda"),
                RotaFrete = Request.GetStringParam("RotaFrete"),
                Mesoregiao = Request.GetListParam<int>("Mesoregiao"),
                Regiao = Request.GetListParam<int>("Regiao"),
                Matriz = Request.GetStringParam("Matriz"),
                SituacaoCarga = Request.GetIntParam("SituacaoCarga"),
                TendenciaEntrega = Request.GetIntParam("TendenciaEntrega"),
                ComAlerta = Request.GetNullableBoolParam("ComAlertas"),
                GrupoStatusViagem = Request.GetIntParam("GrupoStatusViagem"),
                Parqueada = Request.GetNullableBoolParam("Parqueada"),
                ColetaNoPrazo = Request.GetNullableBoolParam("ColetaNoPrazo"),
                EntregaNoPrazo = Request.GetNullableBoolParam("EntregaNoPrazo"),
                TendenciaProximaColeta = Request.GetListEnumParam<TendenciaEntrega>("TendenciaProximaColeta"),
                TendenciaProximaEntrega = Request.GetListEnumParam<TendenciaEntrega>("TendenciaProximaEntrega"),
                MonitoramentoStatusViagemTipoRegra = Request.GetIntParam("MonitoramentoStatusViagemTipoRegra"),
                TipoAlertaEvento = Request.GetListParam<int>("TipoAlertaEvento"),
                DataInicioMonitoramento = Request.GetDateTimeParam("DataMonitoramentoInicial"),
                DataFimMonitoramento = Request.GetDateTimeParam("DataMonitoramentoFinal"),
            };

            if (this.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && Usuario.ClienteFornecedor != null)
            {
                //para mostrar no portal cliente.
                filtrosPesquisa.CodigoClienteDestino = new List<double>();
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente clienteFornecedor = repositorioCliente.BuscarPorCPFCNPJ(Usuario.ClienteFornecedor.CPF_CNPJ);
                List<Dominio.Entidades.Cliente> clientesCnpjRaiz = repositorioCliente.BuscarPorRaizCNPJ(clienteFornecedor.CPF_CNPJ_SemFormato.Substring(0, 8));
                filtrosPesquisa.FiltroCliente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComColetaOuEntrega;

                if (clientesCnpjRaiz != null && clientesCnpjRaiz.Count > 1)
                {
                    foreach (Dominio.Entidades.Cliente cliente in clientesCnpjRaiz)
                        filtrosPesquisa.CodigoClienteDestino.Add(cliente.CPF_CNPJ);
                }
                else
                    filtrosPesquisa.CodigoClienteDestino.Add(Usuario.ClienteFornecedor.CPF_CNPJ);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DescricaoAlerta))
            {
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repMonitoramentoEvento.BuscarAtivo(filtrosPesquisa.DescricaoAlerta);
                if (monitoramentoEvento != null)
                {
                    filtrosPesquisa.TipoAlerta = monitoramentoEvento.TipoAlerta;
                }
            }

            List<int> codigosFilial = Request.GetListParam<int>("Filial");

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);

            if (filtrosPesquisa.CodigosTipoCarga == null || filtrosPesquisa.CodigosTipoCarga.Count == 0)
                filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);

            filtrosPesquisa.CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);

            if (filtrosPesquisa.CodigosTipoOperacao == null || filtrosPesquisa.CodigosTipoOperacao.Count == 0)
                filtrosPesquisa.CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);

            if (filtrosPesquisa.VeiculosEmLocaisTracking)
            {
                Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);

                if (filtrosPesquisa.LocaisTracking.Count > 0)
                    filtrosPesquisa.locais = repLocais.BuscarPorCodigos(filtrosPesquisa.LocaisTracking);
                else
                    filtrosPesquisa.locais = repLocais.BuscarTodos();
            }

            if ((filtrosPesquisa.MostrarRaiosProximidade ?? false) && (filtrosPesquisa.LocaisRaioProximidade.Count > 0))
            {
                Repositorio.Embarcador.Logistica.RaioProximidade repRaioProximidade = new Repositorio.Embarcador.Logistica.RaioProximidade(unitOfWork);

                if (filtrosPesquisa.LocaisRaioProximidade.Count > 0)
                    filtrosPesquisa.RaiosProximidade = repRaioProximidade.BuscarPorCodigosLocais(filtrosPesquisa.LocaisRaioProximidade);
            }

            if (configuracao != null)
            {
                filtrosPesquisa.DataBaseCalculoPrevisaoControleEntrega = configuracao.DataBaseCalculoPrevisaoControleEntrega;
                filtrosPesquisa.TelaMonitoramentoApresentarCargasQuando = configuracao.TelaMonitoramentoApresentarCargasQuando;
                filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal = configuracao.TempoSemPosicaoParaVeiculoPerderSinal;
            }

            if (configuracaoMonitoramento != null)
                filtrosPesquisa.TelaMonitoramentoFiltroFilialDaCarga = configuracaoMonitoramento.TelaMonitoramentoFiltroFilialDaCarga;

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa(HttpRequest request, int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool limitando = true)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(request)
            {
                header = new List<Models.Grid.Head>(),
                listTabs = new List<Models.Grid.Tab>(),
            };

            grid.AdicionarTab("Data", out Models.Grid.Tab tabData);
            grid.AdicionarTab("Monitoramento", out Models.Grid.Tab tabMonitoramento);
            grid.AdicionarTab("Veículo", out Models.Grid.Tab tabVeiculo);
            grid.AdicionarTab("Carga", out Models.Grid.Tab tabCarga);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataDaCarga, "DataCriacaoCarga", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho("Carga", false);
            grid.AdicionarCabecalho("DataInicioMonitoramento", false);
            grid.AdicionarCabecalho("DataFimMonitoramento", false);
            grid.AdicionarCabecalho("DataInicioMonitoramentoFormatada", false);
            grid.AdicionarCabecalho("DataFimMonitoramentoFormatada", false);
            grid.AdicionarCabecalho("Veiculo", false);
            grid.AdicionarCabecalho("Veiculos", false);
            grid.AdicionarCabecalho("Cor", false);
            grid.AdicionarCabecalho("CorStatusViagem", false);
            grid.AdicionarCabecalho("TiporRegraViagem", false);
            grid.AdicionarCabecalho("GrupoStatusViagemCor", false);
            grid.AdicionarCabecalho("GrupoStatusViagemCodigo", false);
            grid.AdicionarCabecalho("CodigoLocalRaiosProximidade", false);
            grid.AdicionarCabecalho("DescricaoLocalRaiosProximidade", false);
            grid.AdicionarCabecalho("CorLocalRaiosProximidade", false);
            grid.AdicionarCabecalho("CodigoRaioProximidade", false);
            grid.AdicionarCabecalho("DescricaoRaioProximidade", false);
            grid.AdicionarCabecalho("RaioRaioProximidade", false);
            grid.AdicionarCabecalho("GrupoStatusViagemDescricao", false);
            grid.AdicionarCabecalho("SituacaoCarga", false);
            grid.AdicionarCabecalho("DescricaoSituacaoCarga", false);
            grid.AdicionarCabecalho("CorSituacaoCarga", false);
            grid.AdicionarCabecalho("IDEquipamento", false);
            grid.AdicionarCabecalho("CodigoProximaEntrega", false);
            grid.AdicionarCabecalho("CorTendenciaEntrega", false);
            grid.AdicionarCabecalho("TendenciaEntrega", false);
            grid.AdicionarCabecalho("TendenciaProximaParada", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataDoMonitoramento, "Data", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho("Codigo", false); //Esse "Codigo" = SM, verificar commit de alteração para eventuais duvidas.
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CodigoFilial, "CodigoFilial", 5, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Filial, "Filial", 8, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Carga, "CargaEmbarcador", 6, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PesoTotal, "PesoTotalCarga", 6, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.ValorTotalNotaFiscal, "ValorTotalNFe", 8, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.ClienteOrigem, "ClienteOrigem", 6, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CidadeDestino, "CidadeDestino", 6, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Destinos, "Destinos", 10, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DestinosPontoPassagem, "DestinosPontoPassagem", 10, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Recebedor, "Recebedor", 10, Models.Grid.Align.left, false, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Expedidor, "Expedidor", 10, Models.Grid.Align.left, false, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Pedidos, "Pedidos", 5, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Ordens, "Ordens", 5, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataCarregamento, "DataCarregamentoFormatada", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PrevisaoChegadaPlanta, "DataPrevisaoChegadaPlanta", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataEntregaPedido, "DataPrevisaoEntregaPedido", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataInicioViagem, "DataInicioViagem", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataDescargaPrevista, "DataPrevisaoDescargaJanela", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.ProximoDestino, "ProximoDestino", 5, Models.Grid.Align.left, true, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CodigoIntegracaoDestino, "CodigoIntegracaoDestino", 5, Models.Grid.Align.left, true, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PrevisaoEntregaPlanejada, "DataEntregaPlanejadaProximaEntrega", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PrevisaoEntregaReprogramada, "DataEntregaReprogramadaProximaEntrega", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataSaidaDaOrigem, "DataSaidaOrigem", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataChegadaNoDestino, "DataChegadaDestino", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Coletas, "Coletas", 5, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.QuantidadesEntregas, "QuantidadeEntregas", 5, Models.Grid.Align.left, false, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DistanciaRota, "DistanciaRotaPrevistaRealizada", 5, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PorcentagemAderenciaSequencia, "AderenciaSequencia", 10, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PorcentagemAderenciaRaio, "AderenciaRaio", 10, Models.Grid.Align.left, true, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Tracao, "Tracao", 5, Models.Grid.Align.left, true, true, tabVeiculo);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Reboque, "Reboques", 5, Models.Grid.Align.left, true, true, tabVeiculo);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.VeiculoDedicado, "PossuiContratoFreteDescricao", 5, Models.Grid.Align.center, true, true, tabVeiculo);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.RazaoSocialTransportador, "RazaoSocialTransportador", 10, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NomeFantasiaTransportador, "NomeFantasiaTransportador", 10, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Motorista, "Motoristas", 10, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CPFMotorista, "CPFMotoristas", 10, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Latitude, "LatitudeFormatada", 5, Models.Grid.Align.right, false, false, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Longitude, "LongitudeFormatada", 5, Models.Grid.Align.right, false, false, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Posicao, "Posicao", 5, Models.Grid.Align.left, true, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Ignicao, "StatusIgnicao", 4, Models.Grid.Align.left, true, true, tabVeiculo);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NivelGPS, "NivelGPS", 5, Models.Grid.Align.center, true, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Velocidade, "Velocidade", 4, Models.Grid.Align.left, true, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Temperatura, "Temperatura", 5, Models.Grid.Align.center, true, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.FaixaDeTemperatura, "FaixaTemperaturaDescricao", 5, Models.Grid.Align.left, true, true, tabMonitoramento);
            grid.AdicionarCabecalho("Lead time Transportador", "DiasUteisPrazoTransportador", 5, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho("TemperaturaFaixaInicial", false);
            grid.AdicionarCabecalho("TemperaturaFaixaFinal", false);
            grid.AdicionarCabecalho("ControleDeTemperatura", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PorcentagemTemperaturaNaFaixa, "TemperaturaDentroFaixa", 7, Models.Grid.Align.left, false, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PorcentagemViagem, "PercentualViagem", 7, Models.Grid.Align.left, true, true, tabMonitoramento);
            grid.AdicionarCabecalho("N° Frota", "NumeroFrota", 7, Models.Grid.Align.left, true, true, tabVeiculo);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.KmAteDestino, "DistanciaAteDestino", 7, Models.Grid.Align.left, true, false, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.KmRodado, "DistanciaPercorrida", 7, Models.Grid.Align.left, true, false, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.KmTotal, "DistanciaTotal", 7, Models.Grid.Align.left, true, false, tabMonitoramento);
            grid.AdicionarCabecalho("DistanciaPrevistaFormatada", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Rastreador, "RastreadorOnlineOffline", 4, Models.Grid.Align.left, true, true, tabVeiculo);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataDaPosicao, "DataPosicaoAtual", 5, Models.Grid.Align.left, true, false, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.ClassificacaoRota, "ClassificacaoRota", 5, Models.Grid.Align.left, false, false, tabCarga);

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TotalDeAlertas, "TotalAlertas", 4, Models.Grid.Align.right, true, false, tabMonitoramento);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Alertas, "Alertas", 4, Models.Grid.Align.left, true, false, tabMonitoramento);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.AlertasNaoTratados, "AlertasAbertos", 4, Models.Grid.Align.left, true, true, tabMonitoramento);
            }

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataProgramadaDaColeta, "DataProgramadaColeta", 5, Models.Grid.Align.left, true, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Semaforo, "CorSemaforo", 5, Models.Grid.Align.left, false, true, tabMonitoramento);

            if (!configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento || !configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.EtapaMonitoramento, "StatusViagem", 5, Models.Grid.Align.left, true, true, tabMonitoramento);

            if (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.GrupoDeOperacao, "GrupoTipoOperacao", 5, Models.Grid.Align.left, true, true, tabCarga);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Alvo, "CategoriasAlvos", 5, Models.Grid.Align.left, true, false, tabMonitoramento);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.SituacaoMonitoramento, "StatusDescricao", 7, Models.Grid.Align.left, true, true, tabMonitoramento);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NumeroEXP, "NumeroEXP", 4, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Critico, "Critico", 5, Models.Grid.Align.center, true, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NumeroRastreador, "NumeroRastreador", 7, Models.Grid.Align.left, false, false, tabVeiculo);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TendenciaProximaParada, "TendenciaProximaParadaDescricao", 10, Models.Grid.Align.left, false, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TendenciaColeta, "TendenciaColetaDescricao", 10, Models.Grid.Align.left, false, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.ColetaNoPrazo, "PrazoColetaDescricao", 10, Models.Grid.Align.left, false, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TendenciaEntrega, "TendenciaEntregaDescricao", 10, Models.Grid.Align.left, false, true, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.EntregaNoPrazo, "PrazoEntregaDescricao", 10, Models.Grid.Align.left, false, true, tabMonitoramento);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.ResponsavelVeiculo, "ResponsavelVeiculo", 10, Models.Grid.Align.left, false, false, tabVeiculo);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CentrosDeResultados, "CentroResultado", 5, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Fronteiras, "FronteiraRotaFrete", 5, Models.Grid.Align.left, true, false, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CidadeOrigem, "CidadeOrigem", 5, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NomeRastreador, "NomeRastreador", 5, Models.Grid.Align.left, true, true, tabVeiculo);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Gerenciador, "Gerenciador", 5, Models.Grid.Align.left, true, true, tabVeiculo);
            grid.AdicionarCabecalho("TipoIntegracaoTecnologiaRastreador", false);
            grid.AdicionarCabecalho("IconeUltimoAlertaExibirTela", false);
            grid.AdicionarCabecalho("TipoUltimoAlertaMonitoramento", false);
            grid.AdicionarCabecalho("DescricaoUltimoAlertaMonitoramento", false);
            grid.AdicionarCabecalho("CorUltimoAlertaMonitoramento", false);
            grid.AdicionarCabecalho("StatusUltimoAlertaMonitoramento", false);
            grid.AdicionarCabecalho("CorStatusUltimoAlertaMonitoramento", false);
            grid.AdicionarCabecalho("DescricaoStatusUltimoAlertaMonitoramento", false);
            grid.AdicionarCabecalho("CodigoUltimoAlerta", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NotasFiscais, "NotasFiscais", 7, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Número AE/SM", "NumeroProtocoloIntegracaoCarga", 7, Models.Grid.Align.left, false, false, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.VersaoAppMotorista, "VersaoAppMotorista", 7, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NumeroPedidoNoEmbarcador, "NumeroPedidoEmbarcadorSumarizado", 7, Models.Grid.Align.left, false, false, tabCarga);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Anotacoes, "Observacao", 10, Models.Grid.Align.left, false, false, tabMonitoramento);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TempoStatus, "TempoStatusDescricao", 5, Models.Grid.Align.left, false, false, tabMonitoramento);
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataReagendamento, "DataReagendamentoFormatada", 7, Models.Grid.Align.left, false, false, tabData);

            if (filtrosPesquisa.VeiculosEmLocaisTracking)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.LocaisTracking, "LocalTrackingDescricao", 7, Models.Grid.Align.left, true, true, tabMonitoramento);

            grid.AdicionarCabecalho("Previsão Fim Viagem", "PrevisaoFimViagemFormatada", 5, Models.Grid.Align.left, false, true, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoOperacao, "TipoOperacao", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NumeroContainer, "NumeroContainer", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoTrecho, "TipoTrecho", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Previsao Termino Viagem", "PrevisaoTerminoViagemFormatada", 5, Models.Grid.Align.left, false, false, tabData);
            grid.AdicionarCabecalho("Previsao StopTracking", "PrevisaoStopTrankingFormatada", 5, Models.Grid.Align.left, false, false, tabData);
            grid.AdicionarCabecalho("Previsao Saida Destino", "PrevisaoSaidaDestinoFormatada", 5, Models.Grid.Align.left, false, false, tabData);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoCarga, "TipoCarga", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Produtos, "Produtos", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataRealEntrega, "DataRealEntrega", 5, Models.Grid.Align.left, false, false, tabData);
            grid.AdicionarCabecalho("Data/Hora primeira posição", "DataPrimeiraPosicaoFormatada", 5, Models.Grid.Align.left, true, false, tabData);
            grid.AdicionarCabecalho("Data Agendamento", "DataAgendamentoPedidoFormatada", 5, Models.Grid.Align.left, false, false, tabData);
            grid.AdicionarCabecalho("Data Coleta próxima entrega", "DataCarregamentoPedidoFormatada", 5, Models.Grid.Align.left, false, false, tabData);
            grid.AdicionarCabecalho("Data chegada na coleta", "DataChegadaColetaFormatada", 5, Models.Grid.Align.left, false, false, tabData);
            grid.AdicionarCabecalho("Data saída da coleta", "DataSaidaColetaFormatada", 5, Models.Grid.Align.left, false, false, tabData);
            grid.AdicionarCabecalho("Escritório Venda", "EscritorioVendasComplementar", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Matriz", "MatrizComplementar", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Número Pedido Cliente", "NumeroPedidoCliente", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CanalVenda, "CanalVenda", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Retorno integração SM", "RetornoIntegracaoSM", 5, Models.Grid.Align.left, false, false, tabMonitoramento);
            grid.AdicionarCabecalho("Situação integração SM", "SituacaoIntegracaoSMRetorno", 5, Models.Grid.Align.left, false, false, tabMonitoramento);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte, "ModalTransporte", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Mesoregiao, "Mesoregiao", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Regiao, "Regiao", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Latitude", false);
            grid.AdicionarCabecalho("Longitude", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Parqueada, "ParqueadaDescricao", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("DataAgendamentoParada", false);
            grid.AdicionarCabecalho("TempoPermitidoPermanenciaEmCarregamento", false);
            grid.AdicionarCabecalho("TempoPermitidoPermanenciaNoCliente", false);
            grid.AdicionarCabecalho("TempoStatusEmMinutos", false);
            grid.AdicionarCabecalho("Status", "Status", 5, Models.Grid.Align.center, false, false, tabMonitoramento);
            grid.AdicionarCabecalho("StatusRastreador", false);


            Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "MonitoramentoNovo/Pesquisa", "grid-monitoramentoNovo");
            grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(codigoUsuario, grid.modelo));

            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            int totalRegistros = repositorioMonitoramento.ContarConsultaView(filtrosPesquisa);

            if (!limitando)
                parametrosConsulta.LimiteRegistros = totalRegistros;

            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> retornoConsulta = totalRegistros > 0 ? repositorioMonitoramento.ConsultarMonitoramento(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();

            AdicionarRegistrosGrid(retornoConsulta, configuracao, configuracaoMonitoramento, grid, filtrosPesquisa);

            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> AdicionarRegistrosGrid(IList<Monitoramento> retornoConsulta, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento, Models.Grid.Grid grid, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa)
        {

            if (filtrosPesquisa?.LocaisRaioProximidade.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Logistica.Locais> locais = filtrosPesquisa.RaiosProximidade.Select(x => x.Local).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> raios = filtrosPesquisa.RaiosProximidade;

                foreach (Monitoramento monitoramento in retornoConsulta)
                {
                    Dominio.Entidades.Embarcador.Logistica.RaioProximidade raioVeiculoEmAreaRaio = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarRaioVeiculoEmAreaRaioProximidade(raios.ToArray(), monitoramento.Latitude, monitoramento.Longitude);
                    if (raioVeiculoEmAreaRaio != null)
                    {
                        monitoramento.CodigoLocalRaiosProximidade = raioVeiculoEmAreaRaio.Local.Codigo;
                        monitoramento.DescricaoLocalRaiosProximidade = raioVeiculoEmAreaRaio.Local.Descricao;
                        monitoramento.CorLocalRaiosProximidade = raioVeiculoEmAreaRaio.Cor;
                        monitoramento.CodigoRaioProximidade = raioVeiculoEmAreaRaio.Codigo;
                        monitoramento.DescricaoRaioProximidade = raioVeiculoEmAreaRaio.Identificacao;
                        monitoramento.RaioRaioProximidade = raioVeiculoEmAreaRaio.Raio;
                    }
                }
            }
            // TODO: ToList Cast
            List<Monitoramento> lista = retornoConsulta.ToList();

            // Ordenação de colunas que não são do banco de dados
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(null);
            string direcaoOrdenar = parametrosConsulta.DirecaoOrdenar?.ToLower();
            string colunaOrdenacaoManual = (parametrosConsulta.PropriedadeOrdenar != null) ? parametrosConsulta.PropriedadeOrdenar : String.Empty;
            switch (colunaOrdenacaoManual)
            {
                case "Status":
                    if (direcaoOrdenar == "asc") lista.Sort((x, y) => String.Compare(x.StatusDescricao, y.StatusDescricao)); else lista.Sort((x, y) => String.Compare(y.StatusDescricao, x.StatusDescricao));
                    break;
                case "Rastreador":
                    if (direcaoOrdenar == "asc") lista.Sort((x, y) => String.Compare(x.Rastreador, y.Rastreador)); else lista.Sort((x, y) => String.Compare(y.Rastreador, x.Rastreador));
                    break;
                case "Coletas":
                    if (direcaoOrdenar == "asc") lista.Sort((x, y) => String.Compare(x.Coletas, y.Coletas)); else lista.Sort((x, y) => String.Compare(y.Coletas, x.Coletas));
                    break;
                case "EntregasDescricao":
                    if (direcaoOrdenar == "asc") lista.Sort((x, y) => String.Compare(x.EntregasDescricao, y.EntregasDescricao)); else lista.Sort((x, y) => String.Compare(y.EntregasDescricao, x.EntregasDescricao));
                    break;
                case "AlertasAbertos":
                    if (direcaoOrdenar == "asc") lista.Sort((x, y) => String.Compare(x.IconeUltimoAlertaExibirTela, y.IconeUltimoAlertaExibirTela)); else lista.Sort((x, y) => String.Compare(y.IconeUltimoAlertaExibirTela, x.IconeUltimoAlertaExibirTela));
                    break;
            }

            grid.AdicionaRows(lista);

            return retornoConsulta;
        }
        #endregion

    }
}