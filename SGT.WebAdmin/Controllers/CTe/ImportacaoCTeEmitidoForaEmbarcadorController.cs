using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/ImportacaoCTeEmitidoForaEmbarcador")]
    public class ImportacaoCTeEmitidoForaEmbarcadorController : BaseController
    {
		#region Construtores

		public ImportacaoCTeEmitidoForaEmbarcadorController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho("Data Importação", "DataImportacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Início do processamento", "DataInicioProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Fim do processamento", "DataFimProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tempo", "Tempo", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 15, Models.Grid.Align.left, false);

                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaImportacaoCTeEmitidoForaEmbarcador filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int total = repImportacaoCTeEmitidoForaEmbarcador.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador> lista = total > 0 ? repImportacaoCTeEmitidoForaEmbarcador.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador>();

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
                        DescricaoSituacao = row.Situacao.ObterDescricao(),
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
                return new JsonpResult(false, "Falha ao consultar Importações");
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

                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador ImportacaoCTeEmitidoForaEmbarcador = repImportacaoCTeEmitidoForaEmbarcador.BuscarPorCodigo(codigo);
                if (ImportacaoCTeEmitidoForaEmbarcador == null) return new JsonpResult(false, "Importação não encontrada. ");

                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha repImportacaoCTeEmitidoForaEmbarcadorLinha = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha> linhas = repImportacaoCTeEmitidoForaEmbarcadorLinha.BuscarPorImportacaoCTeEmitidoForaEmbarcador(codigo);
                int totalLinhas = linhas.Count();
                if (totalLinhas > 0)
                {
                    Models.Grid.Grid grid = new Models.Grid.Grid() { header = new List<Models.Grid.Head>() };
                    grid.AdicionarCabecalho("Número", "Numero");
                    grid.AdicionarCabecalho("Número CTe emitido fora embarcador", "NumeroCTe");
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
                        drow["NumeroCTe"] = linhas[i].CTeEmitidoForaEmbarcador?.Numero.ToString() ?? string.Empty;
                        drow["Situacao"] = linhas[i].Situacao;
                        drow["DescricaoSituacao"] = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcadorHelper.ObterDescricao(linhas[i].Situacao);
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
                        return Arquivo(arquivoBinario, "application/octet-stream", $"importacao-cte-terceiro-{ImportacaoCTeEmitidoForaEmbarcador.Planilha}-{codigo}-resultado.csv");
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar Linha.");
                }
                else
                {
                    return new JsonpResult(false, "Nenhuma Linha encontrada. ");
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu falha ao Consultar. ");
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
                if (grid == null) return new JsonpResult(false, "Importação não encontrada. ");
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu falha ao consultar. ");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarLinhas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarLinhas(codigo);
                if (grid == null) return new JsonpResult(false, "Importação não encontrada. ");

                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, "Ocorreu falha ao gerar arquivo. ");

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu falha ao exportar linhas. ");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Colunas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarColunas(codigo);
                if (grid == null) return new JsonpResult(false, "Linha não encontrada. ");
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu falha ao consultar colunas. ");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarColunas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarColunas(codigo);
                if (grid == null) return new JsonpResult(false, "Linha não encontrada. ");

                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, "Ocorreu falha ao gerar arquivos. ");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu falha ao exportar colunas. ");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoCTeEmitidoForaEmbarcador();
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

                Servicos.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador servicoImportacaoCTeEmitidoForaEmbarcador = new Servicos.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador(unitOfWork);
                if (!servicoImportacaoCTeEmitidoForaEmbarcador.GerarImportacaoCTeEmitidoForaEmbarcador(out Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno, nome, dados, Usuario, Auditado))
                    return new JsonpResult(false, retorno.MensagemAviso);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu falha ao importar arquivo. ");
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
                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador ImportacaoCTeEmitidoForaEmbarcador = repImportacaoCTeEmitidoForaEmbarcador.BuscarPorCodigo(codigo);
                if (ImportacaoCTeEmitidoForaEmbarcador == null) return new JsonpResult(false, "Importação não encontrada. ");
                repImportacaoCTeEmitidoForaEmbarcador.Deletar(ImportacaoCTeEmitidoForaEmbarcador);
                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Importação de Planilha excluida com sucesso! ");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu falha ao excluir planilha importada. ");
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
                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador ImportacaoCTeEmitidoForaEmbarcador = repImportacaoCTeEmitidoForaEmbarcador.BuscarPorCodigo(codigo);

                if (ImportacaoCTeEmitidoForaEmbarcador == null) return new JsonpResult(false, "Importação não encontrada ! ");
                if (ImportacaoCTeEmitidoForaEmbarcador.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Pendente) return new JsonpResult(false, "Possivel cancelar apenas importaçoes Pendentes. ");

                ImportacaoCTeEmitidoForaEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Cancelado;
                ImportacaoCTeEmitidoForaEmbarcador.Mensagem = "Por " + Usuario.Nome + " em " + DateTime.Now;
                repImportacaoCTeEmitidoForaEmbarcador.Atualizar(ImportacaoCTeEmitidoForaEmbarcador);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Importação de Planilha cancelada com Sucesso. ");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha cancelar de planilha Importada. ");
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
                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador importacaoCTeEmitidoForaEmbarcador = repImportacaoCTeEmitidoForaEmbarcador.BuscarPorCodigo(codigo);

                if (importacaoCTeEmitidoForaEmbarcador == null) return new JsonpResult(false, "Importação não encontrada. ");
                if (importacaoCTeEmitidoForaEmbarcador.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Erro && importacaoCTeEmitidoForaEmbarcador.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Sucesso) return new JsonpResult(false, "É possivel reprocessar apenas importações que já foram processadas.");

                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha repImportacaoCTeEmitidoForaEmbarcadorLinha = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha> linhas = repImportacaoCTeEmitidoForaEmbarcadorLinha.BuscarPorImportacaoCTeEmitidoForaEmbarcador(codigo);
                int total = linhas.Count();
                for (int i = 0; i < total; i++)
                {
                    if (linhas[i].CTeEmitidoForaEmbarcador == null)
                    {
                        linhas[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Pendente;
                        linhas[i].Mensagem = null;
                    }
                }

                importacaoCTeEmitidoForaEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Pendente;
                importacaoCTeEmitidoForaEmbarcador.DataInicioProcessamento = null;
                importacaoCTeEmitidoForaEmbarcador.DataFimProcessamento = null;
                importacaoCTeEmitidoForaEmbarcador.TotalSegundosProcessamento = null;
                importacaoCTeEmitidoForaEmbarcador.Mensagem = null;
                repImportacaoCTeEmitidoForaEmbarcador.Atualizar(importacaoCTeEmitidoForaEmbarcador);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Planilha marcada como Pendente. ");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar a Planilha. ");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaImportacaoCTeEmitidoForaEmbarcador ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaImportacaoCTeEmitidoForaEmbarcador filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaImportacaoCTeEmitidoForaEmbarcador()
            {
                CodigoUsuario = Request.GetIntParam("Funcionario"),
                Planilha = Request.GetStringParam("Planilha"),
                DataImportacaoInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataImportacaoFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Situacao = Request.GetEnumParam<SituacaoImportacaoCTeEmitidoForaEmbarcador>("Situacao"),
                Mensagem = Request.GetStringParam("Mensagem"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ConsultarLinhas(int codigo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador ImportacaoCTeEmitidoForaEmbarcador = repImportacaoCTeEmitidoForaEmbarcador.BuscarPorCodigo(codigo);
                if (ImportacaoCTeEmitidoForaEmbarcador == null) return null;

                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha repImportacaoCTeEmitidoForaEmbarcadorLinha = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha> lista = repImportacaoCTeEmitidoForaEmbarcadorLinha.BuscarPorImportacaoCTeEmitidoForaEmbarcador(codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Número CTe emitido fora embarcador ", "NumeroCTe", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 45, Models.Grid.Align.left, false);

                var listaRetornar = (
                    from row in lista
                    select new
                    {
                        row.Codigo,
                        row.Situacao,
                        row.Numero,
                        NumeroCTe = row.CTeEmitidoForaEmbarcador?.Numero.ToString() ?? string.Empty,
                        DescricaoSituacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcadorHelper.ObterDescricao(row.Situacao),
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
                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha repImportacaoCTeEmitidoForaEmbarcadorLinha = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha ImportacaoCTeEmitidoForaEmbarcadorLinha = repImportacaoCTeEmitidoForaEmbarcadorLinha.BuscarPorCodigo(codigo, false);
                if (ImportacaoCTeEmitidoForaEmbarcadorLinha == null) return null;

                Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna repImportacaoCTeEmitidoForaEmbarcadorLinhaColuna = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna(unitOfWork);
                List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna> lista = repImportacaoCTeEmitidoForaEmbarcadorLinhaColuna.BuscarPorLinha(codigo);

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

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoCTeEmitidoForaEmbarcador()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Tipo (CIF = 0/FOB = 1)", Propriedade = "Tipo", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Número", Propriedade = "Numero", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Serie", Propriedade = "Serie", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Chave", Propriedade = "Chave", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Data Emissão", Propriedade = "DataEmissao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "CNPJ Transportador/Emitente", Propriedade = "CNPJTransportador", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "CPF/CNPJ Remetente", Propriedade = "CNPJRemetente", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "CPF/CNPJ Destinatário", Propriedade = "CNPJDestinatario", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "CPF/CNPJ Recebedor", Propriedade = "CNPJRecebedor", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "CPF/CNPJ Tomador", Propriedade = "CNPJTomador", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "CPF/CNPJ Expedidor", Propriedade = "CNPJExpedidor", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Município início", Propriedade = "LocalidadeOrigem", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "UF Origem", Propriedade = "UFOrigem", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Município fim", Propriedade = "LocalidadeDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = "UF Destino", Propriedade = "UFDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = "Tipo Operação", Propriedade = "TipoOperacao", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = "Qtd. CTe", Propriedade = "QuantidadeCTe", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = "Qtd. NF", Propriedade = "QuantidadeNFe", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = "Valor do Frete", Propriedade = "ValorFrete", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = "Valor Total da Mercadoria", Propriedade = "ValorTotalMercadoria", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 21, Descricao = "Peso Base de Cálculo", Propriedade = "PesoBaseCalculo", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 22, Descricao = "Base ICMS", Propriedade = "BaseICMS", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 23, Descricao = "Aliquota do ICMS", Propriedade = "AliquotaICMS", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 24, Descricao = "Valor ICMS", Propriedade = "ValorICMS", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 25, Descricao = "Total de Impostos", Propriedade = "TotalImpostos", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 26, Descricao = "Total Frete", Propriedade = "TotalFrete", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 27, Descricao = "Frete Peso", Propriedade = "FretePeso", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 28, Descricao = "GRIS + ADV", Propriedade = "GrisAdv", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 29, Descricao = "Imposto", Propriedade = "Imposto", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 30, Descricao = "Pedágio", Propriedade = "Pedagio", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 31, Descricao = "Taxas", Propriedade = "Taxas", Tamanho = 200 });

            return configuracoes;
        }

        #endregion
    }
}
