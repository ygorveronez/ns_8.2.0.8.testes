using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.ConversaoUnidadeMedida
{
    [CustomAuthorize("NotasFiscais/ConversaoUnidadeMedida")]
    public class ConversaoUnidadeMedidaController : BaseController
    {
		#region Construtores

		public ConversaoUnidadeMedidaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida repositorioConversaoUnidadeMedida = new Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaConversaoUnidadeMedida filtrosPesquisa = ObterFiltrosPesquisaConversaoUnidadeMedida();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Fator", "Fator", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Unidade de Medida Destino", "UnidadeMedidaDestino", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Unidade de Medida Origem", "UnidadeMedidaOrigem", 40, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioConversaoUnidadeMedida.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida> listaConversaoUnidadeMedida = totalRegistros > 0 ? repositorioConversaoUnidadeMedida.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida>();

                grid.AdicionaRows((
                    from o in listaConversaoUnidadeMedida
                    select new
                    {
                        o.Codigo,
                        o.Descricao,
                        o.Fator,
                        UnidadeMedidaDestino = o.UnidadeMedidaDestino.ObterDescricao(),
                        UnidadeMedidaOrigem = o.UnidadeMedidaOrigem.ObterDescricao()
                    }).ToList()
                );

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
                Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida repConversaoUnidadeMedida = new Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida conversaoUnidadeMedida = repConversaoUnidadeMedida.BuscarPorCodigo(codigo, true);

                // Valida
                if (conversaoUnidadeMedida == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencherEntidade(conversaoUnidadeMedida, unitOfWork);

                // Persiste dados
                unitOfWork.Start();

                if (!VerificarDuplicidade(conversaoUnidadeMedida, repConversaoUnidadeMedida))
                    repConversaoUnidadeMedida.Atualizar(conversaoUnidadeMedida, Auditado);
                else
                    throw new ControllerException("Já existe uma conversão com esses dados");
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
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
                Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida repConversaoUnidadeMedida = new Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida conversaoUnidadeMedida = new Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida();

                // Preenche entidade com dados
                PreencherEntidade(conversaoUnidadeMedida, unitOfWork);

                // Persiste dados
                unitOfWork.Start();

                if (!VerificarDuplicidade(conversaoUnidadeMedida, repConversaoUnidadeMedida))
                    repConversaoUnidadeMedida.Inserir(conversaoUnidadeMedida, Auditado);
                else
                    throw new ControllerException("Já existe uma conversão com esses dados");

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
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
                Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida repConversaoUnidadeMedida = new Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida conversaoUnidadeMedida = repConversaoUnidadeMedida.BuscarPorCodigo(codigo);

                if (conversaoUnidadeMedida == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                repConversaoUnidadeMedida.Deletar(conversaoUnidadeMedida, Auditado);
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

                Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida repConversaoUnidadeMedida = new Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida conversaoUnidadeMedida = repConversaoUnidadeMedida.BuscarPorCodigo(codigo, true);

                if (conversaoUnidadeMedida == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                dynamic dynPedido = new
                {
                    Descricao = conversaoUnidadeMedida.Descricao,
                    Fator = conversaoUnidadeMedida.Fator,
                    UnidadeMedidaOrigem = conversaoUnidadeMedida.UnidadeMedidaOrigem,
                    UnidadeMedidaDestino = conversaoUnidadeMedida.UnidadeMedidaDestino
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

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaConversaoUnidadeMedida ObterFiltrosPesquisaConversaoUnidadeMedida()
        {
            return new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaConversaoUnidadeMedida()
            {
                Descricao = Request.GetStringParam("Descricao")
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida conversaoUnidadeMedida, Repositorio.UnitOfWork unitOfWork)
        {
            conversaoUnidadeMedida.Descricao = Request.GetStringParam("Descricao");
            conversaoUnidadeMedida.Fator = Request.GetDecimalParam("Fator");
            conversaoUnidadeMedida.UnidadeMedidaOrigem = Request.GetEnumParam<UnidadeDeMedida>("UnidadeMedidaOrigem");
            conversaoUnidadeMedida.UnidadeMedidaDestino = Request.GetEnumParam<UnidadeDeMedida>("UnidadeMedidaDestino");
        }

        private bool VerificarDuplicidade(Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida conversaoUnidadeMedida, Repositorio.Embarcador.NotaFiscal.ConversaoUnidadeMedida repConversaoUnidadeMedidal)
        {
            return repConversaoUnidadeMedidal.ExisteDuplicado(conversaoUnidadeMedida);
        }

        #endregion
    }

}

