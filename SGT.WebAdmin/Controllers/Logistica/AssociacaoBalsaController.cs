using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System.Text;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Logistica/AssociacaoBalsa")]
    public class AssociacaoBalsaController : BaseController
    {
        #region Construtores

        public AssociacaoBalsaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.AssociacaoBalsa repositorioAssociacaoBalsa = new Repositorio.Embarcador.Logistica.AssociacaoBalsa(unitOfWork);

                Models.Grid.Grid grid = ObterGridPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAssociacaoBalsa filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int total = repositorioAssociacaoBalsa.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa> lista = total > 0 ? repositorioAssociacaoBalsa.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa>();

                grid.AdicionaRows(ObterListaRetorno(lista));
                grid.setarQuantidadeTotal(total);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as importações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                Repositorio.Embarcador.Logistica.AssociacaoBalsa repositorioAssociacaoBalsa = new Repositorio.Embarcador.Logistica.AssociacaoBalsa(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repositorioViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.ContainerTipo repositorioTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repositorioContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Navio repositorioNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repositorioPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                DateTime dataAtual = DateTime.Now;
                StringBuilder erros = new StringBuilder();

                unitOfWork.Start();

                for (int i = 0; i < files.Count; i++)
                {
                    Servicos.DTO.CustomFile file = files[i];

                    string extensao = Path.GetExtension(file.FileName).ToLowerInvariant();
                    string nomeArquivo = file.FileName;
                    string caminhoArquivo = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "AssociacaoBalsa", "Importar" });

                    string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                    string caminhoPlanilha = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, guidArquivo + extensao);
                    file.SaveAs(caminhoPlanilha);

                    if (extensao != ".xls" && extensao != ".xlsx" && extensao != ".xlsm")
                    {
                        InserirRegistroErro(null, "Extensão do arquivo inválida da planilha " + nomeArquivo + ". Selecione um arquivo com a extensão .xls ou .xlsx.",
                            caminhoPlanilha, dataAtual, nomeArquivo, 0, ref erros, repositorioAssociacaoBalsa);
                        continue;
                    }

                    ExcelPackage package = null;
                    try
                    {
                        package = new ExcelPackage(file.InputStream);
                    }
                    catch
                    {
                        InserirRegistroErro(null, "Não foi encontrada nenhuma configuração para a importação da planilha " + nomeArquivo + " , favor verifique o layout.",
                            caminhoPlanilha, dataAtual, nomeArquivo, 0, ref erros, repositorioAssociacaoBalsa);
                        continue;
                    }

                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    int qtdLinhas = 0;

                    for (int l = 1; l <= worksheet.Dimension.End.Row; l++)
                    {
                        bool linhaVazia = true;

                        for (int j = 1; j <= worksheet.Dimension.End.Column; j++)
                        {
                            if (worksheet.Cells[l, j].Value != null && !string.IsNullOrEmpty(worksheet.Cells[l, j].Text))
                            {
                                linhaVazia = false;
                                break;
                            }
                        }

                        if (!linhaVazia)
                        {
                            qtdLinhas++;
                        }
                    }

                    bool cabecalhosPreenchidos = !string.IsNullOrWhiteSpace(worksheet.Cells[1, 1].Text) &&
                                                         !string.IsNullOrWhiteSpace(worksheet.Cells[1, 13].Text) &&
                                                         !string.IsNullOrWhiteSpace(worksheet.Cells[1, 14].Text);

                    if (!cabecalhosPreenchidos)
                    {
                        InserirRegistroErro(null, "A planilha importada não possui nenhum layout configurado, por gentileza verifique as colunas e suas posições.",
                              caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repositorioAssociacaoBalsa);
                        package.Dispose();
                        continue;
                    }

                    int contagemLinhasImportacao = 0;

                    Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa associacaoBalsaImportacao = new Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa();
                    associacaoBalsaImportacao.AssociacaoBalsas = new List<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa>();

                    for (var j = 1; j <= qtdLinhas; j++)
                    {
                        if (j < 2)
                            continue;

                        bool colunasPreenchidas = !string.IsNullOrWhiteSpace(worksheet.Cells[j, 1].Text) &&
                                                                  !string.IsNullOrWhiteSpace(worksheet.Cells[j, 13].Text) &&
                                                                  !string.IsNullOrWhiteSpace(worksheet.Cells[j, 14].Text);

                        if (!colunasPreenchidas)
                            continue;

                        contagemLinhasImportacao++;

                        string numeroContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 1].Text);
                        string numeroBooking = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 2].Text);
                        string viagemNavioDirecao = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 3].Text);
                        string codigoIntegracaoNavio = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 4].Text);
                        string codigoIntegracaoNavioTransbordo = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 5].Text);
                        string portoOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 6].Text);
                        string portoDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 7].Text);
                        string portoTransbordo = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 8].Text);
                        string descricaoTipoContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 9].Text);
                        string descricaoTomador = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 10].Text);
                        string numeroControle = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 11].Text);
                        string descricaoTipoOperacao = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 12].Text);
                        string numeroCarga = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 13].Text);
                        string balsaCodigoIMO = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 14].Text);

                        if (string.IsNullOrEmpty(numeroContainer))
                        {
                            InserirRegistroErro(associacaoBalsaImportacao, "É obrigatório informar o Número Container.",
                                caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repositorioAssociacaoBalsa);
                            continue;
                        }

                        if (string.IsNullOrEmpty(balsaCodigoIMO))
                        {
                            InserirRegistroErro(associacaoBalsaImportacao, "É obrigatório informar a Balsa.",
                                caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repositorioAssociacaoBalsa);
                            continue;
                        }

                        if (string.IsNullOrEmpty(numeroCarga))
                        {
                            InserirRegistroErro(associacaoBalsaImportacao, "É obrigatório informar o número da Carga.",
                                caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repositorioAssociacaoBalsa);
                            continue;
                        }

                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorNumeroCargaNumeroContainer(numeroCarga, numeroContainer);

                        if (cargaPedido == null)
                        {
                            InserirRegistroErro(associacaoBalsaImportacao, "Carga com documentação emitida não identificada com esse número de carga e número de container.",
                                caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repositorioAssociacaoBalsa);
                            continue;
                        }

                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
                        Dominio.Entidades.Embarcador.Pedidos.Container container = repositorioContainer.BuscarPorNumero(numeroContainer);
                        Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio = repositorioViagem.BuscarPorDescricao(viagemNavioDirecao);
                        Dominio.Entidades.Embarcador.Pedidos.Navio navio = repositorioNavio.BuscarPorCodigoIntegracao(codigoIntegracaoNavio);
                        Dominio.Entidades.Embarcador.Pedidos.Navio navioTransbordo = repositorioNavio.BuscarPorCodigoIntegracao(codigoIntegracaoNavioTransbordo);
                        Dominio.Entidades.Embarcador.Pedidos.Porto origem = repositorioPorto.BuscarPorDescricao(portoOrigem);
                        Dominio.Entidades.Embarcador.Pedidos.Porto destino = repositorioPorto.BuscarPorDescricao(portoDestino);
                        Dominio.Entidades.Embarcador.Pedidos.Porto porTransbordo = repositorioPorto.BuscarPorDescricao(portoTransbordo);
                        Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer = repositorioTipoContainer.BuscarPorDescricao(descricaoTipoContainer);
                        Dominio.Entidades.Cliente tomador = repositorioCliente.BuscarPorNome(descricaoTomador);
                        string numeroControlePedido = repositorioPedido.BuscarPorNumeroControle(pedido.Codigo);
                        string numeroBookingPedido = repositorioPedido.BuscarPrimeiroPorNumeroBooking(pedido.Codigo);
                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorDescricao(descricaoTipoOperacao);
                        Dominio.Entidades.Embarcador.Pedidos.Navio balsa = repositorioNavio.BuscarPorCodigoIMO(balsaCodigoIMO);

                        Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa associacaoBalsa = new Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa()
                        {
                            Usuario = this.Usuario,
                            Planilha = nomeArquivo,
                            CaminhoPlanilha = caminhoPlanilha,
                            QuantidadeLinhas = 0,
                            DataImportacao = dataAtual,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente,
                            Mensagem = "Processando, aguarde...",
                            DataInicioProcessamento = null,
                            DataFimProcessamento = null,
                            Carga = carga,
                            Container = container,
                            PedidoViagemNavio = pedidoViagemNavio,
                            Navio = navio,
                            NavioTransbordo = navioTransbordo,
                            PortoOrigem = origem,
                            PortoDestino = destino,
                            PortoTransbordo = porTransbordo,
                            ContainerTipo = tipoContainer,
                            Tomador = tomador,
                            TipoOperacao = tipoOperacao,
                            Balsa = balsa,
                            NumeroControle = numeroControlePedido,
                            Booking = numeroBookingPedido
                        };
                        repositorioAssociacaoBalsa.Inserir(associacaoBalsa);

                        associacaoBalsaImportacao.AssociacaoBalsas.Add(associacaoBalsa);
                    }

                    package.Dispose();

                    InserirRegistroPai(associacaoBalsaImportacao, repositorioAssociacaoBalsa, caminhoPlanilha, dataAtual, nomeArquivo, contagemLinhasImportacao);
                }

                unitOfWork.CommitChanges();

                if (erros.Length > 0)
                    return new JsonpResult(false, true, erros.ToString());
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo, verifique se os layouts estão dentro do padrão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.AssociacaoBalsa repAssociacaoBalsa = new Repositorio.Embarcador.Logistica.AssociacaoBalsa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa importacao = repAssociacaoBalsa.BuscarPorCodigo(codigo);

                if (importacao == null)
                    return new JsonpResult(false, "Importação não encontrada.");

                repAssociacaoBalsa.Deletar(importacao);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Importação de planilha excluída com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir a planilha importada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Reprocessar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.AssociacaoBalsa repAssociacaoBalsa = new Repositorio.Embarcador.Logistica.AssociacaoBalsa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa importacao = repAssociacaoBalsa.BuscarPorCodigo(codigo);

                if (importacao == null)
                    return new JsonpResult(false, "Importação não encontrada.");

                if (importacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro && importacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso)
                    return new JsonpResult(false, "É possivel reprocessar apenas importações que já foram processadas.");

                importacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente;
                importacao.DataInicioProcessamento = null;
                importacao.DataFimProcessamento = null;
                importacao.TotalSegundosProcessamento = null;
                importacao.Mensagem = null;

                repAssociacaoBalsa.Atualizar(importacao);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Planilha marcada como pendente para reprocessamento.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar a planilha importada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> Download()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.AssociacaoBalsa repAssociacaoBalsa = new Repositorio.Embarcador.Logistica.AssociacaoBalsa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa importacao = repAssociacaoBalsa.BuscarPorCodigo(codigo);

                if (importacao == null)
                    return new JsonpResult(false, "Importação não encontrada.");

                byte[] arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(importacao.CaminhoPlanilha);
                return Arquivo(arquivo, "plain/text", importacao.Planilha);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao baixar a planilha.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Colunas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarColunas(codigo);
                if (grid == null) return new JsonpResult(false, "Linha não encontrada.");
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as colunas da linha da importação de pedidos.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarColunas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarColunas(codigo);
                if (grid == null) return new JsonpResult(false, "Linha não encontrada.");

                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, grid.extensaoCSV, $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar as colunas da linha da importação de pedidos.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.AssociacaoBalsa repAssociacaoBalsa = new Repositorio.Embarcador.Logistica.AssociacaoBalsa(unitOfWork);

                int codigoAssociacaoBalsaPai = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa associacaoBalsa = repAssociacaoBalsa.BuscarPorCodigo(codigoAssociacaoBalsaPai);

                if (associacaoBalsa == null)
                    return new JsonpResult(false, "Registro não foi encontrado.");

                Models.Grid.Grid grid = ObterGridPesquisaDetalhes();
                int total = associacaoBalsa.AssociacaoBalsas.Count;

                grid.AdicionaRows(ObterListaRetornoDetalhes(associacaoBalsa.AssociacaoBalsas.ToList()));
                grid.setarQuantidadeTotal(total);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as importações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.AssociacaoBalsa repAssociacaoBalsa = new Repositorio.Embarcador.Logistica.AssociacaoBalsa(unitOfWork);

                Grid grid = ObterGridPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAssociacaoBalsa filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int total = repAssociacaoBalsa.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa> lista = total > 0 ? repAssociacaoBalsa.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa>();

                grid.AdicionaRows(ObterListaRetorno(lista));
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, grid.extensaoCSV, grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as importações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.AssociacaoBalsa repAssociacaoBalsa = new Repositorio.Embarcador.Logistica.AssociacaoBalsa(unitOfWork);

                int codigoAssociacaoBalsa = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa associacaoBalsa = repAssociacaoBalsa.BuscarPorCodigo(codigoAssociacaoBalsa);

                if (associacaoBalsa == null)
                    return new JsonpResult(false, "Registro não foi encontrado.");

                Models.Grid.Grid grid = ObterGridPesquisaDetalhes();
                int total = associacaoBalsa.AssociacaoBalsas.Count;

                grid.AdicionaRows(ObterListaRetornoDetalhes(associacaoBalsa.AssociacaoBalsas.ToList()));
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, grid.extensaoCSV, grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as importações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string FormatarMensagem(string mensagem)
        {
            return !string.IsNullOrWhiteSpace(mensagem) ? mensagem.Split('/').FirstOrDefault() : string.Empty;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAssociacaoBalsa ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAssociacaoBalsa filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAssociacaoBalsa()
            {
                CodigoUsuario = Request.GetIntParam("Funcionario"),
                Planilha = Request.GetStringParam("Planilha"),
                DataImportacaoInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataImportacaoFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido>("Situacao"),
                Mensagem = Request.GetStringParam("Mensagem"),
                Booking = Request.GetStringParam("Booking"),
                EntidadePai = true,
            };

            return filtrosPesquisa;
        }

        private Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Situacao", false);
            grid.AdicionarCabecalho("Planilha", "Planilha", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Linhas", "QuantidadeLinhas", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Data da importação", "DataImportacao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Início do processamento", "DataInicioProcessamento", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Fim do processamento", "DataFimProcessamento", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Tempo", "Tempo", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Mensagem", "Mensagem", 15, Models.Grid.Align.left, false);
            return grid;
        }

        private Grid ObterGridPesquisaDetalhes()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Situacao", false);
            grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nº da OS", "NumeroOS", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Nº Booking", "Booking", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Nº Container", "Container", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Mensagem", "Mensagem", 20, Models.Grid.Align.left, false);
            return grid;
        }

        private dynamic ObterListaRetornoDetalhes(List<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa> lista)
        {
            var dynLista = (from row in lista
                            select new
                            {
                                row.Codigo,
                                row.Situacao,
                                Numero = row.NumeroControle ?? "0",
                                Carga = row.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                row.Carga?.Pedidos?.First()?.Pedido?.NumeroOS,
                                row.Booking,
                                Container = row.Container?.Descricao ?? string.Empty,
                                DescricaoSituacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedidoHelper.ObterDescricao(row.Situacao),
                                row.Mensagem,
                            }
           ).ToList();

            return dynLista;
        }

        private void InserirRegistroPai(Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa associacaoBalsa, Repositorio.Embarcador.Logistica.AssociacaoBalsa repositorioAssociacaoBalsa, string caminhoPlanilha, DateTime dataAtual, string nomeArquivo, int qtdLinhas)
        {
            associacaoBalsa.CaminhoPlanilha = caminhoPlanilha;
            associacaoBalsa.DataFimProcessamento = dataAtual;
            associacaoBalsa.DataImportacao = dataAtual;
            associacaoBalsa.Mensagem = "Processando, aguarde...";
            associacaoBalsa.Planilha = nomeArquivo;
            associacaoBalsa.DataInicioProcessamento = dataAtual;
            associacaoBalsa.QuantidadeLinhas = qtdLinhas;
            associacaoBalsa.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente;
            associacaoBalsa.Usuario = this.Usuario;
            associacaoBalsa.EntidadePai = true;

            repositorioAssociacaoBalsa.Inserir(associacaoBalsa);
        }

        private void InserirRegistroErro(Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa associacaoBalsaPai, string msgErro, string caminhoPlanilha, DateTime dataAtual, string nomeArquivo, int qtdLinhas, ref StringBuilder erros, Repositorio.Embarcador.Logistica.AssociacaoBalsa repositorioAssociacaoBalsa)
        {
            Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa associacaoBalsa = new Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa()
            {
                CaminhoPlanilha = caminhoPlanilha,
                DataFimProcessamento = dataAtual,
                DataImportacao = dataAtual,
                Mensagem = msgErro,
                Planilha = nomeArquivo,
                DataInicioProcessamento = dataAtual,
                QuantidadeLinhas = qtdLinhas,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro,
                Usuario = this.Usuario,
                EntidadePai = associacaoBalsaPai == null,
            };

            repositorioAssociacaoBalsa.Inserir(associacaoBalsa);
            erros.Append(msgErro);

            if (associacaoBalsaPai != null)
                associacaoBalsaPai.AssociacaoBalsas.Add(associacaoBalsa);
        }

        private dynamic ObterListaRetorno(List<Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa> lista)
        {
            var dynLista = (from row in lista
                            select new
                            {
                                row.Codigo,
                                row.Planilha,
                                row.QuantidadeLinhas,
                                row.DataImportacao,
                                Usuario = row.Usuario?.Nome ?? "",
                                row.DataInicioProcessamento,
                                row.DataFimProcessamento,
                                Tempo = row.Tempo(),
                                row.Situacao,
                                DescricaoSituacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedidoHelper.ObterDescricao(row.Situacao),
                                row.Mensagem,
                                MensagemResumida = FormatarMensagem(row.Mensagem),
                                DT_RowColor = row.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro ? "#C16565" : (row.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso ? "#DFF0D8" : ""),
                                DT_FontColor = row.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro ? "#FFFFFF" : "",
                            }
           ).ToList();

            return dynLista;
        }

        private Models.Grid.Grid ConsultarColunas(int codigo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.AssociacaoBalsa repAssociacaoBalsa = new Repositorio.Embarcador.Logistica.AssociacaoBalsa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa importacao = repAssociacaoBalsa.BuscarPorCodigo(codigo);

                if (importacao == null)
                    return null;

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.AssociacaoBalsa.LinhaImportacao> lista = ConverterImportacaoLinhas(importacao);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nome campo", "NomeCampo", 30, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor", "Valor", 70, Models.Grid.Align.left, false);

                var listaRetornar = (
                    from row in lista
                    select new
                    {
                        row.Codigo,
                        row.NomeCampo,
                        row.Valor
                    }
                ).ToList();
                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(listaRetornar.Count());

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.AssociacaoBalsa.LinhaImportacao> ConverterImportacaoLinhas(Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa importacao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.AssociacaoBalsa.LinhaImportacao> linhas = new()
            {
                ConverterLinha(nameof(importacao.Container.Numero), importacao.Container?.Numero),
                ConverterLinha(nameof(importacao.Booking), importacao.Booking),
                ConverterLinha(nameof(importacao.PedidoViagemNavio), importacao.PedidoViagemNavio?.CodigoIntegracao),
                ConverterLinha(nameof(importacao.NavioTransbordo), importacao.NavioTransbordo?.CodigoIntegracao),
                ConverterLinha(nameof(importacao.PortoOrigem), importacao.PortoOrigem?.Descricao),
                ConverterLinha(nameof(importacao.PortoDestino), importacao.PortoDestino?.Descricao),
                ConverterLinha(nameof(importacao.PortoTransbordo), importacao.PortoTransbordo?.Descricao),
                ConverterLinha(nameof(importacao.ContainerTipo), importacao.ContainerTipo?.Descricao),
                ConverterLinha(nameof(importacao.Tomador), importacao.Tomador?.Nome),
                ConverterLinha(nameof(importacao.NumeroControle), importacao.NumeroControle),
                ConverterLinha(nameof(importacao.TipoOperacao), importacao.TipoOperacao?.Descricao),
                ConverterLinha(nameof(importacao.Carga), importacao.Carga?.CodigoCargaEmbarcador),
                ConverterLinha(nameof(importacao.Balsa), importacao.Balsa?.CodigoIMO)
            };

            return linhas;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.AssociacaoBalsa.LinhaImportacao ConverterLinha(string nomeCampo, string valor)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.AssociacaoBalsa.LinhaImportacao()
            {
                Codigo = Guid.NewGuid().ToString().Replace("-", ""),
                NomeCampo = nomeCampo,
                Valor = valor,
            };
        }

        #endregion
    }
}