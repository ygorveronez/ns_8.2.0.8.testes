using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using OfficeOpenXml;
using System.Text;

namespace SGT.WebAdmin.Controllers.Cargas.MontagemFeeder
{
    [CustomAuthorize("Pedidos/MontagemFeeder")]
    public class MontagemFeederController : BaseController
    {
        #region Construtores

        public MontagemFeederController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Mensagem", false);
                grid.AdicionarCabecalho("Planilha", "Planilha", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Linhas", "QuantidadeLinhas", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data da importação", "DataImportacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Início do processamento", "DataInicioProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Fim do processamento", "DataFimProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tempo", "Tempo", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "MensagemResumida", 15, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder = new Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.FiltroPesquisaMontagemFeeder filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int total = repMontagemFeeder.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder> lista = total > 0 ? repMontagemFeeder.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder>();

                var listaRetornar = (
                    from row in lista
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
                        MensagemResumida = FormatarMensagem(row.Mensagem)
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(total);

                return new JsonpResult(grid);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as importações de pedidos.");
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

                Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder = new Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.FiltroPesquisaMontagemFeeder filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int total = repMontagemFeeder.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder> lista = total > 0 ? repMontagemFeeder.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder>();

                var listaRetornar = (
                    from row in lista
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
                        row.Mensagem
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as importações de feeder.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = serPedido.ConfiguracaoImportacaoPedido(unitOfWork, TipoServicoMultisoftware);
            unitOfWork.Dispose();

            return new JsonpResult(configuracoes.ToList());
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                bool importarMesmoSemCTeAbsorvidoAnteriormente = Request.GetBoolParam("ImportarMesmoSemCTeAbsorvidoAnteriormente");
                bool importarMesmoComDocumentacaoDuplicada = Request.GetBoolParam("ImportarMesmoComDocumentacaoDuplicada");

                Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder = new Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unidadeDeTrabalho);
                DateTime dataAtual = DateTime.Now;
                StringBuilder erros = new StringBuilder();

                unidadeDeTrabalho.Start();

                for (int i = 0; i < files.Count; i++)
                {
                    Servicos.DTO.CustomFile file = files[i];
                    string extensao = Path.GetExtension(file.FileName).ToLowerInvariant();
                    string nomeArquivo = file.FileName;

                    string caminhoArquivo = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho)?.ObterConfiguracaoArquivo().CaminhoArquivos, "FEEDER", "Importar" });
                    string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    
                    string caminhoPlanilha = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, guidArquivo + extensao);
                    file.SaveAs(caminhoPlanilha);

                    if (extensao != ".xls" && extensao != ".xlsx" && extensao != ".xlsm")
                    {
                        InserirRegistroErro("Extensão do arquivo inválida da planilha " + nomeArquivo + ". Selecione um arquivo com a extensão .xls ou .xlsx.",
                            caminhoPlanilha, dataAtual, false, importarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, 0, ref erros, repMontagemFeeder, importarMesmoComDocumentacaoDuplicada);
                        continue;
                    }
                    ExcelPackage package = null;
                    try
                    {
                        package = new ExcelPackage(file.InputStream);
                    }
                    catch
                    {
                        InserirRegistroErro("Não foi encontrada nenhuma configuração para a importação da planilha " + nomeArquivo + " , favor verifique o layout.",
                            caminhoPlanilha, dataAtual, false, importarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, 0, ref erros, repMontagemFeeder, importarMesmoComDocumentacaoDuplicada);
                        continue;
                    }

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
                    int qtdLinhas = worksheet.Dimension.End.Row;

                    bool planilhaFeeder = (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 2].Text)).ToLower() == "booking") &&
                         (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 3].Text)).ToLower() == "proposta  agreement") &&
                          (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 4].Text)).ToLower() == "manifesto  manifest") &&
                           (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 5].Text)).ToLower() == "protocolo antaq id") &&
                            (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 6].Text)).ToLower() == "numero ce  ce number");

                    if (!planilhaFeeder)
                    {
                        planilhaFeeder = (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 2].Text)).ToLower() == "booking") &&
                         (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 3].Text)).ToLower() == "proposta") &&
                          (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 4].Text)).ToLower() == "manifesto") &&
                           (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 5].Text)).ToLower() == "protocolo antaq");
                    }

                    bool planilhaFeederSubcontratacao = (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 2].Text)).ToLower() == "booking") &&
                         (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 3].Text)).ToLower() == "proposta") &&
                          (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 4].Text)).ToLower() == "modalidade") &&
                           (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 5].Text)).ToLower() == "chave cte") &&
                            (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 6].Text)).ToLower() == "chave nfe");

                    bool planilhaFeederSubcontratacaoTipoUm = (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 1].Text)).ToLower() == "booking") &&
                        (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 2].Text)).ToLower() == "proposta") &&
                         (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 3].Text)).ToLower() == "modalidade") &&
                          (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 4].Text)).ToLower() == "chave cte") &&
                           (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(worksheet.Cells[35, 5].Text)).ToLower() == "chave nfe");

                    string strEmbarque = Utilidades.String.RemoveDiacritics(worksheet.Cells[33, 5].Text).ToLower();
                    if (planilhaFeeder)
                    {
                        if (string.IsNullOrWhiteSpace(strEmbarque) || ((strEmbarque != "sim") && (strEmbarque != "nao")))
                        {
                            InserirRegistroErro("A planilha " + nomeArquivo + " importada não possui a informação se o embarque é um afretamento.",
                                caminhoPlanilha, dataAtual, false, importarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, qtdLinhas, ref erros, repMontagemFeeder, importarMesmoComDocumentacaoDuplicada);
                            package.Dispose();
                            continue;
                        }
                    }
                    bool embarqueAfretamento = strEmbarque == "sim";

                    Dominio.Entidades.Cliente destinatario = null;
                    Dominio.Entidades.Cliente expedidor = null;
                    Dominio.Entidades.Cliente tomador = null;

                    string viagemNavioDirecao = Utilidades.String.RemoveDiacritics(worksheet.Cells[16, 3].Text);
                    string portoOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[12, 3].Text);
                    string portoDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[14, 3].Text);

                    string cnpjDestinatario = "", cnpjExpedidor = "", cnpjTomador = "";
                    if (planilhaFeeder || planilhaFeederSubcontratacao)
                    {
                        cnpjDestinatario = Utilidades.String.RemoveDiacritics(worksheet.Cells[24, 11].Text);
                        cnpjExpedidor = Utilidades.String.RemoveDiacritics(worksheet.Cells[24, 3].Text);
                        cnpjTomador = Utilidades.String.RemoveDiacritics(worksheet.Cells[30, 3].Text);
                    }
                    else if (planilhaFeederSubcontratacaoTipoUm)
                    {
                        cnpjDestinatario = Utilidades.String.RemoveDiacritics(worksheet.Cells[24, 10].Text);
                        cnpjExpedidor = Utilidades.String.RemoveDiacritics(worksheet.Cells[24, 2].Text);
                        cnpjTomador = Utilidades.String.RemoveDiacritics(worksheet.Cells[30, 2].Text);
                    }
                    else
                    {
                        InserirRegistroErro("A planilha importada não possui nenhum layout configurado, por gentileza verifique as colunas e suas posições.",
                              caminhoPlanilha, dataAtual, embarqueAfretamento, importarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, qtdLinhas, ref erros, repMontagemFeeder, importarMesmoComDocumentacaoDuplicada);
                        package.Dispose();
                        continue;
                    }

                    double.TryParse(cnpjDestinatario, out double cnpj_cpf_destinatario);
                    if (cnpj_cpf_destinatario > 0)
                        destinatario = repCliente.BuscarPorCPFCNPJ(cnpj_cpf_destinatario);

                    double.TryParse(cnpjExpedidor, out double cnpj_cpf_expedidor);
                    if (cnpj_cpf_expedidor > 0)
                        expedidor = repCliente.BuscarPorCPFCNPJ(cnpj_cpf_expedidor);

                    double.TryParse(cnpjTomador, out double cnpj_cpf_tomador);
                    if (cnpj_cpf_tomador > 0)
                        tomador = repCliente.BuscarPorCPFCNPJ(cnpj_cpf_tomador);

                    if (destinatario == null)
                    {
                        InserirRegistroErro("Destinatário de CNPJ " + cnpjDestinatario + " da planilha " + nomeArquivo + " não localizado no sistema, primeiro deve-se cadastrar o mesmo.",
                            caminhoPlanilha, dataAtual, embarqueAfretamento, importarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, qtdLinhas, ref erros, repMontagemFeeder, importarMesmoComDocumentacaoDuplicada);
                        package.Dispose();
                        continue;
                    }
                    if (planilhaFeeder || planilhaFeederSubcontratacao || planilhaFeederSubcontratacaoTipoUm)
                    {
                        Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder montagemFeeder = new Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder()
                        {
                            CaminhoPlanilha = caminhoPlanilha,
                            CNPJCPFDestinatario = cnpj_cpf_destinatario,
                            CNPJCPFExpedidor = cnpj_cpf_expedidor,
                            CNPJCPFTomador = cnpj_cpf_tomador,
                            DataFimProcessamento = null,
                            DataImportacao = dataAtual,
                            Destinatario = destinatario,
                            EmbarqueAfretamento = embarqueAfretamento,
                            Expedidor = expedidor,
                            ImportarMesmoSemCTeAbsorvidoAnteriormente = importarMesmoSemCTeAbsorvidoAnteriormente,
                            ImportarMesmoComDocumentacaoDuplicada = importarMesmoComDocumentacaoDuplicada,
                            Mensagem = "Processando, aguarde...",
                            Planilha = nomeArquivo,
                            DataInicioProcessamento = null,
                            QuantidadeLinhas = qtdLinhas,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente,
                            TipoPlanilhaFeeder = planilhaFeeder ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPlanilhaFeeder.Feeder : planilhaFeederSubcontratacao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPlanilhaFeeder.Subcontratacao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPlanilhaFeeder.SubcontratacaoTipoUm,
                            Tomador = tomador,
                            Usuario = this.Usuario,
                            PedidoViagemNavio = !string.IsNullOrWhiteSpace(viagemNavioDirecao) ? repViagem.BuscarPorDescricao(viagemNavioDirecao.ToUpper()) : null,
                            PortoDestino = !string.IsNullOrWhiteSpace(portoDestino) ? repPorto.BuscarPorCodigoIATA(portoDestino.ToUpper()) : null,
                            PortoOrigem = !string.IsNullOrWhiteSpace(portoOrigem) ? repPorto.BuscarPorCodigoIATA(portoOrigem.ToUpper()) : null
                        };
                        repMontagemFeeder.Inserir(montagemFeeder);
                    }
                    else
                    {
                        InserirRegistroErro("Não foi encontrada nenhuma configuração para a importação da planilha " + nomeArquivo + ", favor verifique o layout.",
                            caminhoPlanilha, dataAtual, embarqueAfretamento, importarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, qtdLinhas, ref erros, repMontagemFeeder, importarMesmoComDocumentacaoDuplicada);
                    }

                    package.Dispose();
                }

                unidadeDeTrabalho.CommitChanges();

                if (erros.Length > 0)
                    return new JsonpResult(false, true, erros.ToString());
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo, verifique se os layouts estão dentro do padrão Feeder.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder = new Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder importacaoPedido = repMontagemFeeder.BuscarPorCodigo(codigo);
                if (importacaoPedido == null) return new JsonpResult(false, "Importação não encontrada.");
                repMontagemFeeder.Deletar(importacaoPedido);
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
        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder = new Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder importacaoPedido = repMontagemFeeder.BuscarPorCodigo(codigo);

                if (importacaoPedido == null) return new JsonpResult(false, "Importação não encontrada.");
                if (importacaoPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente) return new JsonpResult(false, "É possivel cancelar apenas importações que estão pendentes.");

                importacaoPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Cancelado;
                importacaoPedido.Mensagem = "Por " + Usuario.Nome + " em " + DateTime.Now;
                repMontagemFeeder.Atualizar(importacaoPedido);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Importação da planilha cancelada com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar a planilha importada.");
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
            unitOfWork.Start();
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder = new Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder importacaoPedido = repMontagemFeeder.BuscarPorCodigo(codigo);

                if (importacaoPedido == null) return new JsonpResult(false, "Importação não encontrada.");
                if (importacaoPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro && importacaoPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso) return new JsonpResult(false, "É possivel reprocessar apenas importações que já foram processadas.");

                importacaoPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente;
                importacaoPedido.DataInicioProcessamento = null;
                importacaoPedido.DataFimProcessamento = null;
                importacaoPedido.TotalSegundosProcessamento = null;
                importacaoPedido.Mensagem = null;
                repMontagemFeeder.Atualizar(importacaoPedido);
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

        #endregion

        #region Métodos Privados

        private string FormatarMensagem(string mensagem)
        {
            return !string.IsNullOrWhiteSpace(mensagem) ? mensagem.Split('/').FirstOrDefault() : string.Empty;
        }
        private Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.FiltroPesquisaMontagemFeeder ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.FiltroPesquisaMontagemFeeder filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.FiltroPesquisaMontagemFeeder()
            {
                CodigoUsuario = Request.GetIntParam("Funcionario"),
                Planilha = Request.GetStringParam("Planilha"),
                DataImportacaoInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataImportacaoFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido>("Situacao"),
                Mensagem = Request.GetStringParam("Mensagem"),
                Viagem = Request.GetIntParam("Viagem"),
                PortoOrigem = Request.GetIntParam("PortoOrigem"),
                PortoDestino = Request.GetIntParam("PortoDestino"),
                NumeroBooking = Request.GetStringParam("NumeroBooking")
            };

            return filtrosPesquisa;
        }

        private void InserirRegistroErro(string msgErro, string caminhoPlanilha, DateTime dataAtual, bool embarqueAfretamento, bool importarMesmoSemCTeAbsorvidoAnteriormente, string nomeArquivo, int qtdLinhas, ref StringBuilder erros, Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder, bool importarMesmoComDocumentacaoDuplicada)
        {
            Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder montagemFeeder = new Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder()
            {
                CaminhoPlanilha = caminhoPlanilha,
                CNPJCPFDestinatario = 0,
                CNPJCPFExpedidor = 0,
                CNPJCPFTomador = 0,
                DataFimProcessamento = dataAtual,
                DataImportacao = dataAtual,
                Destinatario = null,
                EmbarqueAfretamento = embarqueAfretamento,
                Expedidor = null,
                ImportarMesmoSemCTeAbsorvidoAnteriormente = importarMesmoSemCTeAbsorvidoAnteriormente,
                ImportarMesmoComDocumentacaoDuplicada = importarMesmoComDocumentacaoDuplicada,
                Mensagem = msgErro,
                Planilha = nomeArquivo,
                DataInicioProcessamento = dataAtual,
                QuantidadeLinhas = qtdLinhas,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro,
                TipoPlanilhaFeeder = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPlanilhaFeeder.Feeder,
                Tomador = null,
                Usuario = this.Usuario
            };
            repMontagemFeeder.Inserir(montagemFeeder);
            erros.Append(msgErro);

        }

        #endregion
    }
}
