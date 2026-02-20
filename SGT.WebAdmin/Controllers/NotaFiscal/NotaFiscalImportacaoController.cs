using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize(new string[] { "Pedidos/Pedido", "NotasFiscais/NotaFiscalImportacao" })]
    public class NotaFiscalImportacaoController : BaseController
    {
		#region Construtores

		public NotaFiscalImportacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> ConfiguracaoImportacaoNotaFiscal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.NotaFiscal.NotaFiscalImportacao servicoNotaFiscalImportacao = new Servicos.Embarcador.NotaFiscal.NotaFiscalImportacao(unitOfWork, Auditado, TipoServicoMultisoftware, ConfiguracaoEmbarcador, Usuario);

                return new JsonpResult(servicoNotaFiscalImportacao.ObterConfiguracaoImportacao().ToList());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarNotaFiscal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.NotaFiscal.NotaFiscalImportacao servicoNotaFiscalImportacao = new Servicos.Embarcador.NotaFiscal.NotaFiscalImportacao(unitOfWork, Auditado, TipoServicoMultisoftware, ConfiguracaoEmbarcador, Usuario);

                Dominio.ObjetosDeValor.Embarcador.Importacao.ParametrosImportacao parametrosImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.ParametrosImportacao();
                parametrosImportacao.Usuario = Usuario;
                parametrosImportacao.Linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(Request.GetStringParam("Dados"));
                parametrosImportacao.Nome = Request.GetStringParam("Nome");
                parametrosImportacao.Guid = Request.GetStringParam("ArquivoSalvoComo");

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoNotaFiscalImportacao.ImportarNotasFiscais(parametrosImportacao);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                Repositorio.Embarcador.Pedidos.XMLNotaFiscalImportacao repositorioXmlNotaFiscalImportacao = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalImportacao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalImportacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioXmlNotaFiscalImportacao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao> listaRetorno = totalRegistros > 0 ? repositorioXmlNotaFiscalImportacao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao>();

                grid.setarQuantidadeTotal(totalRegistros);

                var lista = (from p in listaRetorno
                             select new
                             {
                                 p.Codigo,
                                 Data = p.Data.ToString("dd/MM/yyyy HH:mm"),
                                 QuantidadeImportada = p.QuantidadeRegistrosImportados.ToString(),
                                 QuantidadeTotal = p.QuantidadeRegistrosTotal.ToString(),
                                 p.NomeArquivo,
                                 p.ImportacaoNotaFiscal?.Mensagem,
                                 Situacao = p.ImportacaoNotaFiscal?.Situacao.ObterDescricao(),
                                 DT_RowColor = p.ImportacaoNotaFiscal?.Situacao.ObterCorLinha(),
                                 TempoProcessamento = (p.ImportacaoNotaFiscal?.Tempo.HasValue ?? false) ? p.ImportacaoNotaFiscal.Tempo.Value.ToString(@"hh\:mm") : ""
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar importações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nome Arquivo", "NomeArquivo", 35, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data", "Data", 35, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Quantidade Importada", "QuantidadeImportada", 35, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Quantidade Total", "QuantidadeTotal", 35, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Mensagem", "Mensagem", 35, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 35, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tempo de Processamento", "TempoProcessamento", 35, Models.Grid.Align.center, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalImportacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalImportacao()
            {
                DataInicio = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataFinal"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal")
            };
        }

        #endregion
    }
}
