using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/NotificacaoRetiradaProduto")]
    public class NotificacaoRetiradaProdutoController : BaseController
    {
		#region Construtores

		public NotificacaoRetiradaProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaNotificacaoRetiradaProduto filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Email", "Email", 55, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Situacao == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto repNotificacaoRetiradaProduto = new Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto> listaNotificacaoRetiradaProduto = repNotificacaoRetiradaProduto.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repNotificacaoRetiradaProduto.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaNotificacaoRetiradaProduto
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 Email = p.EmailFormatado,
                                 p.DescricaoSituacao
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto repNotificacaoRetiradaProduto = new Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto notificacaoRetiradaProduto = new Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto();

                PreencherNotificacaoRetiradaProduto(notificacaoRetiradaProduto);
                repNotificacaoRetiradaProduto.Inserir(notificacaoRetiradaProduto, Auditado);

                SalvarDestinatarios(notificacaoRetiradaProduto, unitOfWork);
                repNotificacaoRetiradaProduto.Atualizar(notificacaoRetiradaProduto);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto repNotificacaoRetiradaProduto = new Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto(unitOfWork);
                Repositorio.EmpresaContratoAnexo repContratoNotaFiscalAnexo = new Repositorio.EmpresaContratoAnexo(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto notificacaoRetiradaProduto = repNotificacaoRetiradaProduto.BuscarPorCodigo(codigo, true);

                if (notificacaoRetiradaProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherNotificacaoRetiradaProduto(notificacaoRetiradaProduto);
                SalvarDestinatarios(notificacaoRetiradaProduto, unitOfWork);

                repNotificacaoRetiradaProduto.Atualizar(notificacaoRetiradaProduto, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto repNotificacaoRetiradaProduto = new Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto notificacaoRetiradaProduto = repNotificacaoRetiradaProduto.BuscarPorCodigo(codigo);

                if (notificacaoRetiradaProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynContratoNotaFiscal = new
                {
                    notificacaoRetiradaProduto.Codigo,
                    notificacaoRetiradaProduto.Descricao,
                    notificacaoRetiradaProduto.Situacao,
                    notificacaoRetiradaProduto.Email,
                    Destinatarios = (from obj in notificacaoRetiradaProduto.Destinatarios
                                     select new
                                     {
                                         obj.Codigo,
                                         obj.Descricao
                                     }).ToList()
                };

                return new JsonpResult(dynContratoNotaFiscal);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto repNotificacaoRetiradaProduto = new Repositorio.Embarcador.Pedidos.NotificacaoRetiradaProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto notificacaoRetiradaProduto = repNotificacaoRetiradaProduto.BuscarPorCodigo(codigo, true);

                if (notificacaoRetiradaProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repNotificacaoRetiradaProduto.Deletar(notificacaoRetiradaProduto, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherNotificacaoRetiradaProduto(Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto notificacaoRetiradaProduto)
        {
            notificacaoRetiradaProduto.Email = Request.Params("Email");
            notificacaoRetiradaProduto.Descricao = Request.GetStringParam("Descricao");
            notificacaoRetiradaProduto.Situacao = Request.GetBoolParam("Situacao");
        }

        private void SalvarDestinatarios(Dominio.Entidades.Embarcador.Pedidos.NotificacaoRetiradaProduto notificacaoRetiradaProduto, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            if (notificacaoRetiradaProduto.Destinatarios == null)
                notificacaoRetiradaProduto.Destinatarios = new List<Dominio.Entidades.Usuario>();
            else
                notificacaoRetiradaProduto.Destinatarios.Clear();

            dynamic dynDestinatarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinatarios"));

            foreach (var dynDestinatario in dynDestinatarios)
                notificacaoRetiradaProduto.Destinatarios.Add(repUsuario.BuscarPorCodigo((int)dynDestinatario.Codigo));
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaNotificacaoRetiradaProduto ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaNotificacaoRetiradaProduto()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
            };
        }

        #endregion


    }
}
