using OfficeOpenXml;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/RolagemContainer")]
    public class RolagemContainerController : BaseController
    {
        #region Construtores

        public RolagemContainerController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer = new Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer(unitOfWork);

                Models.Grid.Grid grid = ObterGridPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Carga.RolagemContainer.FiltroPesquisaRolagemContainer filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int total = repRolagemContainer.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer> lista = total > 0 ? repRolagemContainer.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer>();

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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer = new Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer(unitOfWork);

                Grid grid = ObterGridPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Carga.RolagemContainer.FiltroPesquisaRolagemContainer filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int total = repRolagemContainer.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer> lista = total > 0 ? repRolagemContainer.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer>();

                grid.AdicionaRows(ObterListaRetorno(lista));
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
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
        public async Task<IActionResult> PesquisaDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer = new Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer(unitOfWork);

                int codigoRolagemContainerPai = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer rolagemContainer = repRolagemContainer.BuscarPorCodigo(codigoRolagemContainerPai);

                if (rolagemContainer == null)
                    return new JsonpResult(false, "Registro não foi encontrado.");

                Models.Grid.Grid grid = ObterGridPesquisaDetalhes();
                int total = rolagemContainer.RolagemContainers.Count;

                grid.AdicionaRows(ObterListaRetornoDetalhes(rolagemContainer.RolagemContainers.ToList()));
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
        public async Task<IActionResult> ExportarPesquisaDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer = new Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer(unitOfWork);

                int codigoRolagemContainerPai = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer rolagemContainer = repRolagemContainer.BuscarPorCodigo(codigoRolagemContainerPai);

                if (rolagemContainer == null)
                    return new JsonpResult(false, "Registro não foi encontrado.");

                Models.Grid.Grid grid = ObterGridPesquisaDetalhes();
                int total = rolagemContainer.RolagemContainers.Count;

                grid.AdicionaRows(ObterListaRetornoDetalhes(rolagemContainer.RolagemContainers.ToList()));
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
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
        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer = new Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer(unitOfWork);

                DateTime dataAtual = DateTime.Now;
                StringBuilder erros = new StringBuilder();

                unitOfWork.Start();

                for (int i = 0; i < files.Count; i++)
                {
                    Servicos.DTO.CustomFile file = files[i];

                    string extensao = Path.GetExtension(file.FileName).ToLowerInvariant();
                    string nomeArquivo = file.FileName;
                    string caminhoArquivo = $"{Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "RolagemContainer", "Importar" })}";
                    string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                    string caminhoPlanilha = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, guidArquivo + extensao);
                    file.SaveAs(caminhoPlanilha);

                    if (extensao != ".xls" && extensao != ".xlsx" && extensao != ".xlsm")
                    {
                        InserirRegistroErro(null, "Extensão do arquivo inválida da planilha " + nomeArquivo + ". Selecione um arquivo com a extensão .xls ou .xlsx.",
                            caminhoPlanilha, dataAtual, nomeArquivo, 0, ref erros, repRolagemContainer);
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
                            caminhoPlanilha, dataAtual, nomeArquivo, 0, ref erros, repRolagemContainer);
                        continue;
                    }

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
                    int qtdLinhas = worksheet.Dimension.End.Row;

                    bool layouPlanilhaOk = (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[3, 1].Text)).ToLower() == "container") &&
                          (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[3, 4].Text)).ToLower() == "booking novo") &&
                           (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[3, 6].Text)).ToLower() == "booking anterior") &&
                           (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[3, 12].Text)).ToLower() == "vvd") &&
                            (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[3, 28].Text)).ToLower() == "pod");

                    if (!layouPlanilhaOk)
                    {
                        InserirRegistroErro(null, "A planilha importada não possui nenhum layout configurado, por gentileza verifique as colunas e suas posições.",
                              caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                        package.Dispose();
                        continue;
                    }

                    int contagemLinhasImportacao = 0;

                    var bookingsNovosAll = worksheet.Cells[4, 4, worksheet.Dimension.End.Row, 4]
                        .Select(c => c.Text)
                        .Where(c => !string.IsNullOrEmpty(c))
                        .ToList();

                    var cargasUtilizadas = new List<int>();

                    Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer rolagemContainerImportacao = new Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer();
                    rolagemContainerImportacao.RolagemContainers = new List<Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer>();

                    for (var j = 1; j <= qtdLinhas; j++)
                    {
                        if (j < 4)
                            continue;

                        bool linhaVazia = (string.IsNullOrWhiteSpace(Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 1].Text)))) &&
                         (string.IsNullOrWhiteSpace(Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 2].Text)))) &&
                          (string.IsNullOrWhiteSpace(Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 4].Text)))) &&
                           (string.IsNullOrWhiteSpace(Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 6].Text)))) &&
                           (string.IsNullOrWhiteSpace(Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 12].Text)))) &&
                            (string.IsNullOrWhiteSpace(Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 28].Text))));

                        if (linhaVazia)
                            continue;

                        contagemLinhasImportacao++;

                        string numeroContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 1].Text);
                        string clienteDescricao = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 2].Text);
                        string tipoEquipamento = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 3].Text);
                        string bookingNovo = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 4].Text);
                        string dataCriacao = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 5].Text);
                        string bookingAnterior = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 6].Text);
                        string numeroOS = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 7].Text);
                        string municipioOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 8].Text);
                        string UFOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 9].Text);
                        string portoOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 10].Text);
                        string terminalPortuarioOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 11].Text);
                        string viagemNavioDirecao = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 12].Text);
                        string portoTransbordo1 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 13].Text);
                        string terminalTransbordo1 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 14].Text);
                        string viagemTransbordo1 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 15].Text);
                        string portoTransbordo2 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 16].Text);
                        string terminalTransbordo2 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 17].Text);
                        string viagemTransbordo2 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 18].Text);
                        string portoTransbordo3 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 19].Text);
                        string terminalTransbordo3 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 20].Text);
                        string viagemTransbordo3 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 21].Text);
                        string portoTransbordo4 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 22].Text);
                        string terminalTransbordo4 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 23].Text);
                        string viagemTransbordo4 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 24].Text);
                        string portoTransbordo5 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 25].Text);
                        string terminalTransbordo5 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 26].Text);
                        string viagemTransbordo5 = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 27].Text);
                        string portoDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 28].Text);
                        string terminalPortuarioDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 29].Text);
                        string municipioDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 30].Text);
                        string UFDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[j, 31].Text);

                        Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio = null;
                        pedidoViagemNavio = repViagem.BuscarPorDescricao(viagemNavioDirecao);

                        if (pedidoViagemNavio == null)
                        {
                            InserirRegistroErro(rolagemContainerImportacao, "Não foi localizada a viagem.",
                                caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                            continue;
                        }

                        if (!repCargaPedido.ExistePorNumeroBookingContainer(bookingAnterior, numeroContainer))
                        {
                            InserirRegistroErro(rolagemContainerImportacao, "Container não identificado no booking informado.",
                               caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                            continue;
                        }

                        if (!repCargaPedido.ExisteCargaPorBookingAntigo(bookingAnterior, numeroContainer))
                        {
                            InserirRegistroErro(rolagemContainerImportacao, "Booking antigo não localizado.",
                               caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                            continue;
                        }

                        if (!repCargaPedido.ExisteCargaPorBookingNovo(bookingNovo))
                        {
                            InserirRegistroErro(rolagemContainerImportacao, "Booking novo não localizado.",
                               caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                            continue;
                        }

                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoAntigo = repCargaPedido.BuscarPorNumeroBookingNumeroContainer(bookingAnterior, numeroContainer, cargasUtilizadas);
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNovo = repCargaPedido.BuscarPorNumeroBooking(bookingNovo, cargasUtilizadas);

                        if (cargaPedidoAntigo == null)
                        {
                            InserirRegistroErro(rolagemContainerImportacao, "Booking antigo não localizado.",
                               caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                            continue;
                        }
                        if (cargaPedidoNovo == null)
                        {
                            InserirRegistroErro(rolagemContainerImportacao, "Booking novo não localizado.",
                               caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                            continue;
                        }

                        cargasUtilizadas.Add(cargaPedidoAntigo.Carga.Codigo);
                        cargasUtilizadas.Add(cargaPedidoNovo.Carga.Codigo);

                        var qtdeBookingSistema = repCargaPedido.QtdeCargasPorBookingNovo(bookingNovo);
                        var qtdeBookingPlanilha = bookingsNovosAll.Count(b => b == bookingNovo);

                        if (qtdeBookingSistema != qtdeBookingPlanilha)
                        {
                            InserirRegistroErro(rolagemContainerImportacao, $"Quantidade do booking {bookingNovo} da planilha divergente de pedidos do booking do sistema.\nQuantidade na planilha:\t{qtdeBookingPlanilha}\nQuantidade no sistema:\t{qtdeBookingSistema}\n",
                               caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                            continue;
                        }

                        if (!ValidarFreteComponentesEntreCargas(cargaPedidoAntigo, cargaPedidoNovo))
                        {
                            InserirRegistroErro(rolagemContainerImportacao, "Valores de frete ou componentes entre as cargas são diferentes.",
                               caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                            continue;
                        }

                        var validarPortos = new List<(string NomeColuna, string Descricao)> {
                            (worksheet.Cells[3, 10].Text, portoOrigem),
                            (worksheet.Cells[3, 28].Text, portoDestino),
                            (worksheet.Cells[3, 13].Text, portoTransbordo1),
                            (worksheet.Cells[3, 16].Text, portoTransbordo2),
                            (worksheet.Cells[3, 19].Text, portoTransbordo3),
                            (worksheet.Cells[3, 22].Text, portoTransbordo4),
                            (worksheet.Cells[3, 25].Text, portoTransbordo5),
                        }
                        .Where(porto => !string.IsNullOrWhiteSpace(porto.Descricao))
                        .Where(porto => repPorto.BuscarPorDescricao(porto.Descricao) == null);

                        foreach (var (NomeColuna, Descricao) in validarPortos)
                        {
                            InserirRegistroErro(rolagemContainerImportacao, $"Não foi localizado o porto da coluna [{NomeColuna}] com a descrição: {Descricao}.",
                                caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                        };

                        if (validarPortos.Any())
                            continue;

                        var validarTerminais = new List<(string NomeColuna, string Descricao)> {
                            (worksheet.Cells[3, 11].Text, terminalPortuarioOrigem),
                            (worksheet.Cells[3, 29].Text, terminalPortuarioDestino),
                            (worksheet.Cells[3, 14].Text, terminalTransbordo1),
                            (worksheet.Cells[3, 17].Text, terminalTransbordo2),
                            (worksheet.Cells[3, 20].Text, terminalTransbordo3),
                            (worksheet.Cells[3, 23].Text, terminalTransbordo4),
                            (worksheet.Cells[3, 26].Text, terminalTransbordo5)
                        }
                        .Where(terminal => !string.IsNullOrWhiteSpace(terminal.Descricao))
                        .Where(terminal => repTerminal.BuscarPorDescricao(terminal.Descricao) == null);

                        foreach (var (NomeColuna, Descricao) in validarTerminais)
                        {
                            InserirRegistroErro(rolagemContainerImportacao, $"Não foi localizado o terminal portuário da coluna [{NomeColuna}] com a descrição: {Descricao}.",
                                caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                        };

                        if (validarTerminais.Any())
                            continue;

                        if (!ValidarRotaCargas(cargaPedidoAntigo, cargaPedidoNovo, rolagemContainerImportacao, caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer))
                            continue;

                        cargaPedidoNovo.Pedido.Initialize();

                        cargaPedidoNovo.Pedido.LacreContainerUm = cargaPedidoAntigo.Pedido.LacreContainerUm;
                        cargaPedidoNovo.Pedido.LacreContainerDois = cargaPedidoAntigo.Pedido.LacreContainerDois;
                        cargaPedidoNovo.Pedido.LacreContainerTres = cargaPedidoAntigo.Pedido.LacreContainerTres;
                        cargaPedidoNovo.Pedido.TaraContainer = cargaPedidoAntigo.Pedido.TaraContainer;

                        if (string.IsNullOrWhiteSpace(cargaPedidoNovo.Pedido.TaraContainer) && cargaPedidoAntigo.Pedido?.Container != null)
                            cargaPedidoNovo.Pedido.TaraContainer = cargaPedidoAntigo.Pedido?.Container?.Tara.ToString();

                        cargaPedidoNovo.Pedido.Remetente = cargaPedidoAntigo.Pedido.Remetente;
                        cargaPedidoNovo.Pedido.Destinatario = cargaPedidoAntigo.Pedido.Destinatario;
                        cargaPedidoNovo.Pedido.Tomador = cargaPedidoAntigo.Pedido.Tomador;
                        cargaPedidoNovo.Pedido.Recebedor = cargaPedidoAntigo.Pedido.Recebedor;
                        cargaPedidoNovo.Pedido.Expedidor = cargaPedidoAntigo.Pedido.Expedidor;

                        cargaPedidoNovo.Pedido.Origem = cargaPedidoAntigo.Pedido.Origem;
                        cargaPedidoNovo.Pedido.Destino = cargaPedidoAntigo.Pedido.Destino;

                        repPedido.Atualizar(cargaPedidoNovo.Pedido, Auditado);

                        Dominio.Entidades.Cliente cliente = repCliente.BuscarPorNome(clienteDescricao);
                        Dominio.Entidades.Embarcador.Pedidos.Container container = repContainer.BuscarPorNumero(numeroContainer);
                        Dominio.Entidades.Localidade munOrigem = repLocalidade.BuscarPrimeiraPorDescricao(municipioOrigem);
                        Dominio.Entidades.Localidade munDestino = repLocalidade.BuscarPrimeiraPorDescricao(municipioDestino);
                        Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem = !string.IsNullOrWhiteSpace(terminalPortuarioOrigem) ? repTerminal.BuscarPorDescricao(terminalPortuarioOrigem) : null;
                        Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino = !string.IsNullOrWhiteSpace(terminalPortuarioDestino) ? repTerminal.BuscarPorDescricao(terminalPortuarioDestino) : null;
                        Dominio.Entidades.Embarcador.Pedidos.Porto origem = repPorto.BuscarPorDescricao(portoOrigem);
                        Dominio.Entidades.Embarcador.Pedidos.Porto destino = repPorto.BuscarPorDescricao(portoDestino);
                        Dominio.Entidades.Embarcador.Pedidos.Porto porTransbordo1 = repPorto.BuscarPorDescricao(portoTransbordo1);
                        Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedviagemTransbordo1 = repViagem.BuscarPorDescricao(viagemTransbordo1);
                        Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao termTransbordo1 = !string.IsNullOrWhiteSpace(terminalTransbordo1) ? repTerminal.BuscarPorDescricao(terminalTransbordo1) : null;
                        Dominio.Entidades.Embarcador.Pedidos.Porto porTransbordo2 = repPorto.BuscarPorDescricao(portoTransbordo2);
                        Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedviagemTransbordo2 = repViagem.BuscarPorDescricao(viagemTransbordo2);
                        Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao termTransbordo2 = !string.IsNullOrWhiteSpace(terminalTransbordo2) ? repTerminal.BuscarPorDescricao(terminalTransbordo2) : null;
                        Dominio.Entidades.Embarcador.Pedidos.Porto porTransbordo3 = repPorto.BuscarPorDescricao(portoTransbordo3);
                        Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedviagemTransbordo3 = repViagem.BuscarPorDescricao(viagemTransbordo3);
                        Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao termTransbordo3 = !string.IsNullOrWhiteSpace(terminalTransbordo3) ? repTerminal.BuscarPorDescricao(terminalTransbordo3) : null;
                        Dominio.Entidades.Embarcador.Pedidos.Porto porTransbordo4 = repPorto.BuscarPorDescricao(portoTransbordo4);
                        Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedviagemTransbordo4 = repViagem.BuscarPorDescricao(viagemTransbordo4);
                        Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao termTransbordo4 = !string.IsNullOrWhiteSpace(terminalTransbordo4) ? repTerminal.BuscarPorDescricao(terminalTransbordo4) : null;
                        Dominio.Entidades.Embarcador.Pedidos.Porto porTransbordo5 = repPorto.BuscarPorDescricao(portoTransbordo5);
                        Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedviagemTransbordo5 = repViagem.BuscarPorDescricao(viagemTransbordo5);
                        Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao termTransbordo5 = !string.IsNullOrWhiteSpace(terminalTransbordo5) ? repTerminal.BuscarPorDescricao(terminalTransbordo5) : null;

                        var cargaNovo = cargaPedidoNovo.Carga;
                        Servicos.Embarcador.Carga.CargaDadosSumarizados svcCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                        svcCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaNovo, ConfiguracaoEmbarcador, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
                        cargaPedidoNovo.Carga = cargaNovo;

                        Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer rolagemContainer = new Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer()
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
                            Container = container,
                            Cliente = cliente,
                            TipoEquipamento = tipoEquipamento,
                            BookingNovo = bookingNovo,
                            BookingAnterior = bookingAnterior,
                            NumeroOS = numeroOS,
                            MunicipioOrigem = munOrigem,
                            MunicipioDestino = munDestino,
                            UFOrigem = UFOrigem,
                            UFDestino = UFDestino,
                            TerminalOrigem = terminalOrigem,
                            TerminalDestino = terminalDestino,
                            PortoOrigem = origem,
                            PortoDestino = destino,
                            PedidoViagemNavio = pedidoViagemNavio,
                            PortoTransbordo1 = porTransbordo1,
                            PedidoViagemNavioTransbordo1 = pedviagemTransbordo1,
                            TerminalTransbordo1 = termTransbordo1,
                            PortoTransbordo2 = porTransbordo2,
                            PedidoViagemNavioTransbordo2 = pedviagemTransbordo2,
                            TerminalTransbordo2 = termTransbordo2,
                            PortoTransbordo3 = porTransbordo3,
                            PedidoViagemNavioTransbordo3 = pedviagemTransbordo3,
                            TerminalTransbordo3 = termTransbordo3,
                            PortoTransbordo4 = porTransbordo4,
                            PedidoViagemNavioTransbordo4 = pedviagemTransbordo4,
                            TerminalTransbordo4 = termTransbordo4,
                            PortoTransbordo5 = porTransbordo5,
                            PedidoViagemNavioTransbordo5 = pedviagemTransbordo5,
                            TerminalTransbordo5 = termTransbordo5,
                            CargaPedidoAntigo = cargaPedidoAntigo,
                            CargaPedido = cargaPedidoNovo,
                        };
                        repRolagemContainer.Inserir(rolagemContainer);

                        rolagemContainerImportacao.RolagemContainers.Add(rolagemContainer);
                    }
                    package.Dispose();

                    InserirRegistroPai(rolagemContainerImportacao, repRolagemContainer, caminhoPlanilha, dataAtual, nomeArquivo, contagemLinhasImportacao);
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

                Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer = new Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacao = repRolagemContainer.BuscarPorCodigo(codigo);

                if (importacao == null)
                    return new JsonpResult(false, "Importação não encontrada.");

                repRolagemContainer.Deletar(importacao);
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

                Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer = new Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacao = repRolagemContainer.BuscarPorCodigo(codigo);

                if (importacao == null)
                    return new JsonpResult(false, "Importação não encontrada.");

                if (importacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro && importacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso)
                    return new JsonpResult(false, "É possivel reprocessar apenas importações que já foram processadas.");

                importacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente;
                importacao.DataInicioProcessamento = null;
                importacao.DataFimProcessamento = null;
                importacao.TotalSegundosProcessamento = null;
                importacao.Mensagem = null;

                repRolagemContainer.Atualizar(importacao);
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

                Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer = new Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacao = repRolagemContainer.BuscarPorCodigo(codigo);

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
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar as colunas da linha da importação de pedidos.");
            }
        }

        #endregion

        #region Métodos Privados

        private string FormatarMensagem(string mensagem)
        {
            return !string.IsNullOrWhiteSpace(mensagem) ? mensagem.Split('/').FirstOrDefault() : string.Empty;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.RolagemContainer.FiltroPesquisaRolagemContainer ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.RolagemContainer.FiltroPesquisaRolagemContainer filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.RolagemContainer.FiltroPesquisaRolagemContainer()
            {
                CodigoUsuario = Request.GetIntParam("Funcionario"),
                Planilha = Request.GetStringParam("Planilha"),
                DataImportacaoInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataImportacaoFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido>("Situacao"),
                Mensagem = Request.GetStringParam("Mensagem"),
                EntidadePai = true,
            };

            return filtrosPesquisa;
        }

        private void InserirRegistroErro(Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer rolagemContainerPai, string msgErro, string caminhoPlanilha, DateTime dataAtual, string nomeArquivo, int qtdLinhas, ref StringBuilder erros, Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer)
        {
            Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer rolagemContainer = new Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer()
            {
                CaminhoPlanilha = caminhoPlanilha,
                BookingNovo = string.Empty,
                BookingAnterior = string.Empty,
                DataFimProcessamento = dataAtual,
                DataImportacao = dataAtual,
                Mensagem = msgErro,
                Planilha = nomeArquivo,
                DataInicioProcessamento = dataAtual,
                QuantidadeLinhas = qtdLinhas,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro,
                Usuario = this.Usuario,
                EntidadePai = rolagemContainerPai == null,
            };

            repRolagemContainer.Inserir(rolagemContainer);
            erros.Append(msgErro);

            if (rolagemContainerPai != null)
                rolagemContainerPai.RolagemContainers.Add(rolagemContainer);
        }

        private void InserirRegistroPai(Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer rolagemContainer, Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer, string caminhoPlanilha, DateTime dataAtual, string nomeArquivo, int qtdLinhas)
        {
            rolagemContainer.CaminhoPlanilha = caminhoPlanilha;
            rolagemContainer.BookingNovo = string.Empty;
            rolagemContainer.BookingAnterior = string.Empty;
            rolagemContainer.DataFimProcessamento = dataAtual;
            rolagemContainer.DataImportacao = dataAtual;
            rolagemContainer.Mensagem = "Processando, aguarde...";
            rolagemContainer.Planilha = nomeArquivo;
            rolagemContainer.DataInicioProcessamento = dataAtual;
            rolagemContainer.QuantidadeLinhas = qtdLinhas;
            rolagemContainer.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente;
            rolagemContainer.Usuario = this.Usuario;
            rolagemContainer.EntidadePai = true;

            repRolagemContainer.Inserir(rolagemContainer);
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

        private dynamic ObterListaRetorno(List<Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer> lista)
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

        private Grid ObterGridPesquisaDetalhes()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Situacao", false);
            grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.left, true);
            //grid.AdicionarCabecalho("Pedido", "Pedido", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Booking Anterior", "BookingAnterior", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Nº da OS", "NumeroOS", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Nº Container", "Container", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Booking Novo", "BookingNovo", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Mensagem", "Mensagem", 20, Models.Grid.Align.left, false);
            return grid;
        }

        private dynamic ObterListaRetornoDetalhes(List<Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer> lista)
        {
            var dynLista = (from row in lista
                            select new
                            {
                                row.Codigo,
                                row.Situacao,
                                Numero = row.CargaPedido?.Pedido.Numero ?? 0,
                                //Pedido = row.CargaPedido?.Pedido.NumeroPedidoEmbarcador ?? string.Empty,
                                Carga = row.CargaPedido?.Carga.CodigoCargaEmbarcador ?? string.Empty,
                                row.BookingAnterior,
                                row.NumeroOS,
                                Container = row.Container?.Descricao ?? string.Empty,
                                row.BookingNovo,
                                DescricaoSituacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedidoHelper.ObterDescricao(row.Situacao),
                                row.Mensagem,
                            }
           ).ToList();

            return dynLista;
        }

        private bool ValidarFreteComponentesEntreCargas(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaAntiga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaNova)
        {
            //if (cargaAntiga.Pedido.PedidosComponente.Count != cargaNova.Pedido.PedidosComponente.Count)
            //    return false;

            if (cargaAntiga.Pedido.ValorFreteNegociado != cargaNova.Pedido.ValorFreteNegociado)
                return false;

            //foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete componenteCargaAntiga in cargaAntiga.Pedido.PedidosComponente)
            //{
            //    if (!cargaNova.Pedido.PedidosComponente.Any(x => x.TipoComponenteFrete == componenteCargaAntiga.TipoComponenteFrete))
            //        return false;

            //    if (cargaNova.Pedido.PedidosComponente.Any(x => x.TipoComponenteFrete == componenteCargaAntiga.TipoComponenteFrete &&
            //                                                (x.ValorComponente != componenteCargaAntiga.ValorComponente || x.Percentual != componenteCargaAntiga.Percentual)))
            //        return false;
            //}

            return true;
        }

        private bool ValidarRotaCargas(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaAntiga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaNova, Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer rolagemContainerPai, string caminhoPlanilha, DateTime dataAtual, string nomeArquivo, int qtdLinhas, ref StringBuilder erros, Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer)
        {
            if (cargaAntiga.Pedido.OrigemBooking?.Codigo != cargaNova.Pedido.OrigemBooking?.Codigo)
            {
                InserirRegistroErro(rolagemContainerPai, $"A origem das cargas é diferente (carga nova: {cargaNova.Pedido.OrigemBooking?.Descricao} / carga anterior: {cargaAntiga.Pedido.OrigemBooking?.Descricao}).",
                               caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                return false;
            }

            if (cargaAntiga.Pedido.DestinoBooking?.Codigo != cargaNova.Pedido.DestinoBooking?.Codigo)
            {
                InserirRegistroErro(rolagemContainerPai, $"O destino das cargas é diferente (carga nova: {cargaNova.Pedido.DestinoBooking?.Descricao} / carga anterior: {cargaAntiga.Pedido.DestinoBooking?.Descricao}).",
                               caminhoPlanilha, dataAtual, nomeArquivo, qtdLinhas, ref erros, repRolagemContainer);
                return false;
            }

            return true;
        }

        private Models.Grid.Grid ConsultarColunas(int codigo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer = new Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacao = repRolagemContainer.BuscarPorCodigo(codigo);

                if (importacao == null)
                    return null;

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.LinhaImportacao> lista = ConverterImportacaoLinhas(importacao);

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

        private List<Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.LinhaImportacao> ConverterImportacaoLinhas(Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.LinhaImportacao> linhas = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.LinhaImportacao>();

            linhas.Add(ConverterLinha(nameof(importacao.Container), importacao.Container?.Numero));
            linhas.Add(ConverterLinha(nameof(importacao.Cliente), importacao.Cliente?.Nome));
            linhas.Add(ConverterLinha(nameof(importacao.TipoEquipamento), importacao.TipoEquipamento));
            linhas.Add(ConverterLinha(nameof(importacao.BookingNovo), importacao.BookingNovo));
            linhas.Add(ConverterLinha(nameof(importacao.BookingAnterior), importacao.BookingAnterior));
            linhas.Add(ConverterLinha(nameof(importacao.NumeroOS), importacao.NumeroOS));
            linhas.Add(ConverterLinha(nameof(importacao.MunicipioOrigem), importacao.MunicipioOrigem?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.MunicipioDestino), importacao.MunicipioDestino?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.UFOrigem), importacao.UFOrigem));
            linhas.Add(ConverterLinha(nameof(importacao.UFDestino), importacao.UFDestino));
            linhas.Add(ConverterLinha(nameof(importacao.TerminalOrigem), importacao.TerminalOrigem?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.TerminalDestino), importacao.TerminalDestino?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.PortoOrigem), importacao.PortoOrigem?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.PortoDestino), importacao.PortoDestino?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.PedidoViagemNavio), importacao.PedidoViagemNavio?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.PortoTransbordo1), importacao.PortoTransbordo1?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.PortoTransbordo2), importacao.PortoTransbordo2?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.PortoTransbordo3), importacao.PortoTransbordo3?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.PortoTransbordo4), importacao.PortoTransbordo4?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.PortoTransbordo5), importacao.PortoTransbordo5?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.NavioTransbordo1), importacao.PedidoViagemNavioTransbordo1?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.NavioTransbordo2), importacao.PedidoViagemNavioTransbordo2?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.NavioTransbordo3), importacao.PedidoViagemNavioTransbordo3?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.NavioTransbordo4), importacao.PedidoViagemNavioTransbordo4?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.NavioTransbordo5), importacao.PedidoViagemNavioTransbordo5?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.TerminalTransbordo1), importacao.TerminalTransbordo1?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.TerminalTransbordo2), importacao.TerminalTransbordo2?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.TerminalTransbordo3), importacao.TerminalTransbordo3?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.TerminalTransbordo4), importacao.TerminalTransbordo4?.Descricao));
            linhas.Add(ConverterLinha(nameof(importacao.TerminalTransbordo5), importacao.TerminalTransbordo5?.Descricao));

            return linhas;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.LinhaImportacao ConverterLinha(string nomeCampo, string valor)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.RolagemContainer.LinhaImportacao()
            {
                Codigo = Guid.NewGuid().ToString().Replace("-", ""),
                NomeCampo = nomeCampo,
                Valor = valor,
            };
        }

        #endregion
    }
}
