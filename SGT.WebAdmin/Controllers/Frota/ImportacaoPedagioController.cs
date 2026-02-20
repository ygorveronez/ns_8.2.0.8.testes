using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/ImportacaoPedagio")]
    public class ImportacaoPedagioController : BaseController
    {
		#region Construtores

		public ImportacaoPedagioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                Repositorio.Embarcador.Frota.ImportacaoPedagio repositorioImportacaoPedagio = new Repositorio.Embarcador.Frota.ImportacaoPedagio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaImportacaoPedagio filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int total = repositorioImportacaoPedagio.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio> lista = total > 0 ? repositorioImportacaoPedagio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio>();

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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as importações de pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nome = Request.GetStringParam("Nome");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> dados = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(Request.Params("Dados"));

                if (dados.Count == 0)
                    return new JsonpResult(false, "Nenhum dado encontrado no arquivo.");


                Servicos.Embarcador.Frota.ImportacaoPedagio servicoImportacaoPedagio = new Servicos.Embarcador.Frota.ImportacaoPedagio(unitOfWork, Usuario);
               
                return new JsonpResult(servicoImportacaoPedagio.GerarImportacaoPedagio(dados, nome));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a planilha para importação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            return new JsonpResult(ObterConfiguracaoImportacao());
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Linhas()
        {
            try
            {
                Models.Grid.Grid grid = ConsultarLinhas();

                if (grid == null)
                    return new JsonpResult(false, "Importação não encontrada.");

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as linhas da importação de pedidos.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Colunas()
        {
            try
            {
                Models.Grid.Grid grid = ConsultarColunas();

                if (grid == null)
                    return new JsonpResult(false, "Linha não encontrada.");

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as colunas da linha da importação de pedidos.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.ImportacaoPedagio repositorioImportacao = new Repositorio.Embarcador.Frota.ImportacaoPedagio(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio importacao = repositorioImportacao.BuscarPorCodigo(codigo);

                if (importacao == null)
                    return new JsonpResult(false, "Importação não encontrada.");
                
                if (importacao.Situacao != SituacaoImportacaoPedagio.Pendente) 
                    return new JsonpResult(false, "É possivel cancelar apenas importações que estão pendentes.");

                importacao.Situacao = SituacaoImportacaoPedagio.Cancelado;
                importacao.Mensagem = $"Por {Usuario.Nome} em {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}";
                repositorioImportacao.Atualizar(importacao);

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

            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.ImportacaoPedagio repositorioImportacao = new Repositorio.Embarcador.Frota.ImportacaoPedagio(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio importacao = repositorioImportacao.BuscarPorCodigo(codigo);

                if (importacao == null) 
                    return new JsonpResult(false, "Importação não encontrada.");

                if (importacao.Situacao != SituacaoImportacaoPedagio.Erro && importacao.Situacao != SituacaoImportacaoPedagio.Sucesso) 
                    return new JsonpResult(false, "É possivel reprocessar apenas importações que já foram processadas.");

                Repositorio.Embarcador.Frota.ImportacaoPedagioLinha repositorioLinha = new Repositorio.Embarcador.Frota.ImportacaoPedagioLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha> linhas = repositorioLinha.BuscarPorImportacaoPedagio(codigo);

                int total = linhas.Count();

                for (int i = 0; i < total; i++)
                {
                    if (linhas[i].Pedagio == null)
                    {
                        linhas[i].Situacao = SituacaoImportacaoPedagio.Pendente;
                        linhas[i].Mensagem = null;
                    }
                }

                importacao.Situacao = SituacaoImportacaoPedagio.Pendente;
                importacao.DataInicioProcessamento = null;
                importacao.DataFimProcessamento = null;
                importacao.Mensagem = null;
                repositorioImportacao.Atualizar(importacao);

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

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoesImportacao = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoesImportacao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Placa", Propriedade = "Placa", Tamanho = 200, Obrigatorio = true });
            configuracoesImportacao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Valor", Propriedade = "Valor", Tamanho = 200, Obrigatorio = true });
            configuracoesImportacao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Data Passagem", Propriedade = "DataPassagem", Tamanho = 200, Obrigatorio = true });
            configuracoesImportacao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Hora Passagem", Propriedade = "HoraPassagem", Tamanho = 200, Obrigatorio = true });
            configuracoesImportacao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Tipo", Propriedade = "Tipo", Tamanho = 200, Obrigatorio = true });
            configuracoesImportacao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Praça", Propriedade = "Praca", Tamanho = 200 });
            configuracoesImportacao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Rodovia", Propriedade = "Rodovia", Tamanho = 200 });
            configuracoesImportacao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 200 });
            configuracoesImportacao.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "CPFMotorista", Propriedade = "CPFMotorista", Tamanho = 200 });

            return configuracoesImportacao;
        }

        private Models.Grid.Grid ObterGridPesquisa()
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

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaImportacaoPedagio ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaImportacaoPedagio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaImportacaoPedagio()
            {
                CodigoUsuario = Request.GetIntParam("Funcionario"),
                Planilha = Request.GetStringParam("Planilha"),
                DataImportacaoInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataImportacaoFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio>("Situacao"),
                Mensagem = Request.GetStringParam("Mensagem"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ConsultarLinhas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("CodigoImportacao");

                Repositorio.Embarcador.Frota.ImportacaoPedagioLinha repositorioLinhas = new Repositorio.Embarcador.Frota.ImportacaoPedagioLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha> lista = repositorioLinhas.BuscarPorImportacaoPedagio(codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 45, Models.Grid.Align.left, false);

                var listaRetornar = (
                    from row in lista
                    select new
                    {
                        row.Codigo,
                        row.Situacao,
                        row.Numero,
                        DescricaoSituacao = row.Situacao.ObterDescricao(),
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

        private Models.Grid.Grid ConsultarColunas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("CodigoLinha");

                Repositorio.Embarcador.Frota.ImportacaoPedagioLinhaColuna repositorioColuna = new Repositorio.Embarcador.Frota.ImportacaoPedagioLinhaColuna(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna> lista = repositorioColuna.BuscarPorLinha(codigo);

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
