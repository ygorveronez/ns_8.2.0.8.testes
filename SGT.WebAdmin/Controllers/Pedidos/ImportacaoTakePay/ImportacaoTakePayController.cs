using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace SGT.WebAdmin.Controllers.Pedidos.ImportacaoTakePay
{
    [CustomAuthorize("Pedidos/ImportacaoTakePay")]
    public class ImportacaoTakePayController : BaseController
    {
		#region Construtores

		public ImportacaoTakePayController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho("Planilha", "Planilha", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Linhas", "QuantidadeLinhas", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data da importação", "DataImportacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Início do processamento", "DataInicioProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Fim do processamento", "DataFimProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tempo", "Tempo", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 15, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay repImportacaoTakePay = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoTakePay filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int total = repImportacaoTakePay.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay> lista = total > 0 ? repImportacaoTakePay.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay>();

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
        public async Task<IActionResult> Exportar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay repImportacaoTakePay = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay importacaoPedido = repImportacaoTakePay.BuscarPorCodigo(codigo);
                if (importacaoPedido == null) return new JsonpResult(false, "Importação não encontrada.");

                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha repImportacaoTakePayLinha = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> linhas = repImportacaoTakePayLinha.BuscarPorImportacaoTakePay(codigo);
                int totalLinhas = linhas.Count();
                if (totalLinhas > 0)
                {
                    Models.Grid.Grid grid = new Models.Grid.Grid() { header = new List<Models.Grid.Head>() };
                    grid.AdicionarCabecalho("Número", "Numero");
                    grid.AdicionarCabecalho("Carga", "Carga");
                    grid.AdicionarCabecalho("Pedido", "Pedido");
                    grid.AdicionarCabecalho("Situação", "DescricaoSituacao");
                    grid.AdicionarCabecalho("Mensagem", "Mensagem");
                    int totalColunas = linhas[0].Colunas.Count();
                    for (int i = 1; i <= totalColunas; i++)
                    {
                        grid.AdicionarCabecalho("Coluna" + i, "Coluna" + i);
                    }

                    List<dynamic> lista = new List<dynamic>();
                    for (int i = 0; i < totalLinhas; i++)
                    {
                        dynamic row = new ExpandoObject();
                        var drow = row as IDictionary<String, object>;
                        drow["Numero"] = linhas[i].Numero;
                        drow["Carga"] = linhas[i].Carga?.CodigoCargaEmbarcador ?? string.Empty;
                        drow["Pedido"] = linhas[i].Pedido?.NumeroPedidoEmbarcador ?? string.Empty;
                        drow["Situacao"] = linhas[i].Situacao;
                        drow["DescricaoSituacao"] = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedidoHelper.ObterDescricao(linhas[i].Situacao);
                        drow["Mensagem"] = linhas[i].Mensagem;
                        for (int j = 0; j < totalColunas; j++)
                        {
                            drow["Coluna" + (j + 1)] = linhas[i].Colunas[j].Valor;
                        }

                        lista.Add(drow);
                    }

                    grid.AdicionaRows(lista);
                    grid.setarQuantidadeTotal(lista.Count());
                    byte[] arquivoBinario = grid.GerarExcel();
                    if (arquivoBinario != null)
                        return Arquivo(arquivoBinario, "application/octet-stream", $"importacao-pedido-{importacaoPedido.Planilha}-{codigo}-resultado.csv");
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
                }
                else
                {
                    return new JsonpResult(false, "Nenhuma linha encontrada.");
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as linhas da importação de pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Linhas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarLinhas(codigo);
                if (grid == null) return new JsonpResult(false, "Importação não encontrada.");
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as linhas da importação de pedidos.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarLinhas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarLinhas(codigo);
                if (grid == null) return new JsonpResult(false, "Importação não encontrada.");

                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar as linhas da importação de pedidos.");
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

        [AllowAuthenticate]
        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Servicos.Embarcador.Pedido.ImportacaoTakePay serImportacaoTakePay = new Servicos.Embarcador.Pedido.ImportacaoTakePay(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = serImportacaoTakePay.ConfiguracaoImportacaoTakePay(unitOfWork, TipoServicoMultisoftware);
            unitOfWork.Dispose();

            return new JsonpResult(configuracoes.ToList());
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string nome = Request.GetStringParam("Nome");
                string dados = Request.Params("Dados");

                if (!Servicos.Embarcador.Pedido.ImportacaoTakePay.GerarImportacaoTakePay(out Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno, nome, dados, Usuario, Auditado, unitOfWork))
                    return new JsonpResult(false, retorno.MensagemAviso);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
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
            unitOfWork.Start();
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay repImportacaoTakePay = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay importacaoPedido = repImportacaoTakePay.BuscarPorCodigo(codigo);
                if (importacaoPedido == null) return new JsonpResult(false, "Importação não encontrada.");
                repImportacaoTakePay.Deletar(importacaoPedido);
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
                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay repImportacaoTakePay = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay importacaoPedido = repImportacaoTakePay.BuscarPorCodigo(codigo);

                if (importacaoPedido == null) return new JsonpResult(false, "Importação não encontrada.");
                if (importacaoPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente) return new JsonpResult(false, "É possivel cancelar apenas importações que estão pendentes.");

                importacaoPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Cancelado;
                importacaoPedido.Mensagem = "Por " + Usuario.Nome + " em " + DateTime.Now;
                repImportacaoTakePay.Atualizar(importacaoPedido);
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
                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay repImportacaoTakePay = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay importacaoPedido = repImportacaoTakePay.BuscarPorCodigo(codigo);

                if (importacaoPedido == null) return new JsonpResult(false, "Importação não encontrada.");
                if (importacaoPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro && importacaoPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso) return new JsonpResult(false, "É possivel reprocessar apenas importações que já foram processadas.");

                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha repImportacaoTakePayLinha = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> linhas = repImportacaoTakePayLinha.BuscarPorImportacaoTakePay(codigo);
                int total = linhas.Count();
                for (int i = 0; i < total; i++)
                {
                    if (linhas[i].Pedido == null)
                    {
                        linhas[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente;
                        linhas[i].Mensagem = null;
                    }
                }

                importacaoPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente;
                importacaoPedido.DataInicioProcessamento = null;
                importacaoPedido.DataFimProcessamento = null;
                importacaoPedido.TotalSegundosProcessamento = null;
                importacaoPedido.Mensagem = null;
                repImportacaoTakePay.Atualizar(importacaoPedido);
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

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoTakePay ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoTakePay filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoTakePay()
            {
                CodigoUsuario = Request.GetIntParam("Funcionario"),
                Planilha = Request.GetStringParam("Planilha"),
                DataImportacaoInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataImportacaoFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido>("Situacao"),
                Mensagem = Request.GetStringParam("Mensagem"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ConsultarLinhas(int codigo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay repImportacaoTakePay = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay importacaoPedido = repImportacaoTakePay.BuscarPorCodigo(codigo);
                if (importacaoPedido == null) return null;

                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha repImportacaoTakePayLinha = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> lista = repImportacaoTakePayLinha.BuscarPorImportacaoTakePay(codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Pedido", "Pedido", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 45, Models.Grid.Align.left, false);

                var listaRetornar = (
                    from row in lista
                    select new
                    {
                        row.Codigo,
                        row.Situacao,
                        row.Numero,
                        Pedido = row.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                        Carga = row.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                        DescricaoSituacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedidoHelper.ObterDescricao(row.Situacao),
                        row.Mensagem
                    }
                ).ToList();
                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(listaRetornar.Count());

                return grid;
            }
            catch (Exception excecao)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ConsultarColunas(int codigo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha repImportacaoTakePayLinha = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha importacaoPedidoLinha = repImportacaoTakePayLinha.BuscarPorCodigo(codigo, false);
                if (importacaoPedidoLinha == null) return null;

                Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna repImportacaoTakePayLinhaColuna = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna> lista = repImportacaoTakePayLinhaColuna.BuscarPorLinha(codigo);

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
            catch (Exception excecao)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
