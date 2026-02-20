using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.BloqueioEmissaoPorNaoConformidade
{
    [CustomAuthorize("NotasFiscais/BloqueioEmissaoPorNaoConformidade")]
    public class BloqueioEmissaoPorNaoConformidadeController : BaseController
    {
		#region Construtores

		public BloqueioEmissaoPorNaoConformidadeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade repositorioConversaoUnidadeMedida = new Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaBloqueioEmissaoPorNaoConformidade filtrosPesquisa = ObterFiltroPesquisaBloqueioEmissaoPorNaoConformidade();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo de Operacão", "TipoOperacao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Tipo de Não Conformidade", "TipoNaoConformidade", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioConversaoUnidadeMedida.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade> listaConversaoUnidadeMedida = totalRegistros > 0 ? repositorioConversaoUnidadeMedida.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade>();

                grid.AdicionaRows(
                    from o in listaConversaoUnidadeMedida
                    select new
                    {
                        o.Codigo,
                        TipoNaoConformidade = o.TipoNaoConformidade?.Descricao ?? string.Empty, 
                        TipoOperacao = o.TiposOperacao != null && o.TiposOperacao.Count > 0 ? string.Join(", ", o.TiposOperacao.Select(obj => obj.Descricao).ToList()) : string.Empty,
                        Situacao = o.Situacao == true ? "Ativo" : "Inativo",
                   }); ;

                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade repBloqueioEmissaoPorNaoConformidade = new Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade(unitOfWork);

                unitOfWork.Start();
                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade bloqueioEmissaoPorNaoConformidade = repBloqueioEmissaoPorNaoConformidade.BuscarPorCodigo(codigo, true);

                // Valida
                if (bloqueioEmissaoPorNaoConformidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(bloqueioEmissaoPorNaoConformidade, unitOfWork);

                // Persiste dados

                repBloqueioEmissaoPorNaoConformidade.Atualizar(bloqueioEmissaoPorNaoConformidade, Auditado);
                SalvarTiposOperacao(bloqueioEmissaoPorNaoConformidade, unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade repBloqueioEmissaoPorNaoConformidade = new Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade(unitOfWork);

                unitOfWork.Start();
                // Busca informacoes
                Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade bloqueioEmissaoPorNaoConformidade = new Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade();

                PreencherEntidade(bloqueioEmissaoPorNaoConformidade, unitOfWork);

                // Persiste dados
                repBloqueioEmissaoPorNaoConformidade.Inserir(bloqueioEmissaoPorNaoConformidade, Auditado);
                SalvarTiposOperacao(bloqueioEmissaoPorNaoConformidade, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade repBloqueioEmissaoPorNaoConformidade = new Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade bloqueioEmissaoPorNaoConformidade = repBloqueioEmissaoPorNaoConformidade.BuscarPorCodigo(codigo);

                if (bloqueioEmissaoPorNaoConformidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                repBloqueioEmissaoPorNaoConformidade.Deletar(bloqueioEmissaoPorNaoConformidade, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade repConversaoUnidadeMedida = new Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade bloqueioEmissaoPorNaoConformidade = repConversaoUnidadeMedida.BuscarPorCodigo(codigo, true);

                if (bloqueioEmissaoPorNaoConformidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                dynamic dynPedido = new
                {
                    TipoNaoConformidade = new { Codigo = bloqueioEmissaoPorNaoConformidade.TipoNaoConformidade?.Codigo ?? 0, Descricao = bloqueioEmissaoPorNaoConformidade.TipoNaoConformidade?.Descricao ?? string.Empty },
                    Situacao = bloqueioEmissaoPorNaoConformidade.Situacao,
                    TiposOperacao = (from obj in bloqueioEmissaoPorNaoConformidade.TiposOperacao
                                     select new
                                     {
                                         Codigo = obj.Codigo,
                                         Descricao = obj.Descricao,
                                     }).ToList(),
                };

                return new JsonpResult(dynPedido);
            }

            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }

            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metódos Privados

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaBloqueioEmissaoPorNaoConformidade ObterFiltroPesquisaBloqueioEmissaoPorNaoConformidade()
        {
            return new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaBloqueioEmissaoPorNaoConformidade()
            {
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                Situacao = Request.GetNullableBoolParam("Situacao"),
                CodigoTipoNaoConformidade = Request.GetIntParam("TipoNaoConformidade"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade bloqueioEmissaoPorNaoConformidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade repositorioBloqueioEmissaoPorNaoConformidade = new Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade(unitOfWork);

            bloqueioEmissaoPorNaoConformidade.Situacao = Request.GetBoolParam("Situacao");
            int itemNaoConformidade = Request.GetIntParam("TipoNaoConformidade");

            bloqueioEmissaoPorNaoConformidade.TipoNaoConformidade = (itemNaoConformidade > 0) ? new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork).BuscarPorCodigo(itemNaoConformidade) : null;

            if (repositorioBloqueioEmissaoPorNaoConformidade.ExisteRegraDuplicada(bloqueioEmissaoPorNaoConformidade.TipoNaoConformidade.Codigo, bloqueioEmissaoPorNaoConformidade.Codigo))
                throw new ControllerException("Já existe uma regra cadastrada para esse tipo de Não Conformidade.");
        }

        private void SalvarTiposOperacao(Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade bloqueioEmissaoPorNaoConformidadeTipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade repositorioBloqueioEmissaoPorNaoConformidade = new Repositorio.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade(unidadeDeTrabalho);

            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));

            if (bloqueioEmissaoPorNaoConformidadeTipoOperacao.TiposOperacao != null && bloqueioEmissaoPorNaoConformidadeTipoOperacao.TiposOperacao.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoOperacao in tiposOperacao)
                    codigos.Add((int)tipoOperacao.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposDeletar = bloqueioEmissaoPorNaoConformidadeTipoOperacao.TiposOperacao.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoDeletar in tiposDeletar)
                    bloqueioEmissaoPorNaoConformidadeTipoOperacao.TiposOperacao.Remove(tipoOperacaoDeletar);
            }
            else
                bloqueioEmissaoPorNaoConformidadeTipoOperacao.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            foreach (var tipoOperacao in tiposOperacao)
            {

                int.TryParse((string)tipoOperacao.Codigo, out int codigoTipoOperacao);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao existeTipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (existeTipoOperacao == null)
                    continue;

                bool existeTipoOperacaoCadastradaBloqueio = bloqueioEmissaoPorNaoConformidadeTipoOperacao.TiposOperacao.Any(o => o.Codigo == existeTipoOperacao.Codigo);

                if (!existeTipoOperacaoCadastradaBloqueio)
                    bloqueioEmissaoPorNaoConformidadeTipoOperacao.TiposOperacao.Add(existeTipoOperacao);
            }

            repositorioBloqueioEmissaoPorNaoConformidade.Atualizar(bloqueioEmissaoPorNaoConformidadeTipoOperacao);
        }

        #endregion
    }

}

