using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/ImportacaoPedido")]
    public class ImportacaoPedidoController : BaseController
    {
		#region Construtores

		public ImportacaoPedidoController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Planilha, "Planilha", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Linhas, "QuantidadeLinhas", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.DataImportacao, "DataImportacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Usuario, "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.InicioProcessamento, "DataInicioProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.FimProcessamento, "DataFimProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Tempo, "Tempo", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Situacao, "DescricaoSituacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Mensagem, "Mensagem", 15, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoPedido filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int total = repImportacaoPedido.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido> lista = total > 0 ? repImportacaoPedido.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido>();

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
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaConsultarImportacoes);
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
                
                Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedido = repImportacaoPedido.BuscarPorCodigo(codigo);
                if (importacaoPedido == null) return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.ImportacaoNaoEncontrada);

                Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha repImportacaoPedidoLinha = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> linhas = repImportacaoPedidoLinha.BuscarPorImportacaoPedido(codigo);
                int totalLinhas = linhas.Count();
                if (totalLinhas > 0)
                {
                    Models.Grid.Grid grid = new Models.Grid.Grid() { header = new List<Models.Grid.Head>() };
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Numero, "Numero");
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Carga, "Carga");
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Pedido, "Pedido");
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Situacao, "DescricaoSituacao");
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Mensagem, "Mensagem");
                    int totalColunas = linhas[0].Colunas.Count();
                    for (int i = 1; i <= totalColunas; i++)
                    {
                        grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Coluna + i, "Coluna" + i);
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
                    return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaGerarArquivo);
                }
                else
                {
                    return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.NenhumaLinhaEncontrada);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaConsultar);
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
                if (grid == null) return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.ImportacaoNaoEncontrada);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarLinhas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarLinhas(codigo);
                if (grid == null) return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.ImportacaoNaoEncontrada);

                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaGerarArquivo);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaExportarLinhas);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Colunas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarColunas(codigo);
                if (grid == null) return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.LinhaNaoEncontrada);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaConsultarColunas);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarColunas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarColunas(codigo);
                if (grid == null) return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.LinhaNaoEncontrada);

                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaExportarColunas);
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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nome = Request.GetStringParam("Nome");
                string dados = Request.Params("Dados");

                if (!Servicos.Embarcador.Pedido.ImportacaoPedido.GerarImportacaoPedido(out Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno, nome, dados, Usuario, Auditado, unitOfWork))
                    return new JsonpResult(false, retorno.MensagemAviso);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaImportarArquivo);
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
                Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha repImportacaoPedidoLinha = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha(unitOfWork);
                Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna repImportacaoPedidoLinhaColuna = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedido = repImportacaoPedido.BuscarPorCodigo(codigo);
                if (importacaoPedido == null) return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.ImportacaoNaoEncontrada); 
                
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> importacaoPedidoLinha = repImportacaoPedidoLinha.BuscarPorImportacaoPedido(codigo);
                if (importacaoPedidoLinha?.Count > 0)
                {
                    foreach (var linha in importacaoPedidoLinha)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> importacaoPedidoLinhaColuna = repImportacaoPedidoLinhaColuna.BuscarPorLinha(linha.Codigo);

                        if (importacaoPedidoLinhaColuna.Any())
                            repImportacaoPedidoLinhaColuna.Deletar(importacaoPedidoLinhaColuna);

                        repImportacaoPedidoLinha.Deletar(linha);
                    }
                }

                repImportacaoPedido.Deletar(importacaoPedido);
                unitOfWork.CommitChanges();
                return new JsonpResult(true, Localization.Resources.Pedidos.ImportacaoPedido.ImportacaoPlanilhaExcluidaSucesso);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaExcluirPlanilhaImportada);
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
                Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedido = repImportacaoPedido.BuscarPorCodigo(codigo);

                if (importacaoPedido == null) return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.ImportacaoNaoEncontrada);
                if (importacaoPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente) return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.PossivelCancelarApenasImportacoesPendentes);

                importacaoPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Cancelado;
                importacaoPedido.Mensagem = "Por " + Usuario.Nome + " em " + DateTime.Now;
                repImportacaoPedido.Atualizar(importacaoPedido);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, Localization.Resources.Pedidos.ImportacaoPedido.ImportacaoPlanilhaCanceladaSucesso);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaCancelarPlanilhaImportada);
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
                Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedido = repImportacaoPedido.BuscarPorCodigo(codigo);

                if (importacaoPedido == null) return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.ImportacaoNaoEncontrada);
                if (importacaoPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro && importacaoPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso) return new JsonpResult(false, "É possivel reprocessar apenas importações que já foram processadas.");

                Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha repImportacaoPedidoLinha = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> linhas = repImportacaoPedidoLinha.BuscarPorImportacaoPedido(codigo);
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
                repImportacaoPedido.Atualizar(importacaoPedido);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, Localization.Resources.Pedidos.ImportacaoPedido.PlanilhaMarcadaComoPendente);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.ImportacaoPedido.OcorreuFalhaReprocessarPlanilha);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoPedido ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportacaoPedido()
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
                Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedido = repImportacaoPedido.BuscarPorCodigo(codigo);
                if (importacaoPedido == null) return null;

                Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha repImportacaoPedidoLinha = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> lista = repImportacaoPedidoLinha.BuscarPorImportacaoPedido(codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Numero, "Numero", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Pedido, "Pedido", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Carga, "Carga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Situacao, "DescricaoSituacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.ImportacaoPedido.Mensagem, "Mensagem", 45, Models.Grid.Align.left, false);

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
                Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha repImportacaoPedidoLinha = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha importacaoPedidoLinha = repImportacaoPedidoLinha.BuscarPorCodigo(codigo, false);
                if (importacaoPedidoLinha == null) return null;

                Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna repImportacaoPedidoLinhaColuna = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> lista = repImportacaoPedidoLinhaColuna.BuscarPorLinha(codigo);

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
