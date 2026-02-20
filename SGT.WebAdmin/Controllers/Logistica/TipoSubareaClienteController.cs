using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Logistica/TipoSubareaCliente")]
    public class TipoSubareaClienteController : BaseController
    {
		#region Construtores

		public TipoSubareaClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Logistica.TipoSubareaCliente repTipoSubareaCliente = new Repositorio.Embarcador.Logistica.TipoSubareaCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente> tipos = repTipoSubareaCliente.Consultar(descricao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoSubareaCliente.ContarConsulta(descricao, ativo));

                var lista = (from p in tipos
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaClienteHelper.ObterDescricao(p.Tipo),
                                 p.DescricaoAtivo
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
                string descricao = Request.Params("Descricao");
                bool ativo = Request.GetBoolParam("Ativo");
                bool permiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea = Request.GetBoolParam("PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente) Request.GetIntParam("Tipo");
                Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente tipoSubareaCliente = new Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente()
                {
                    Ativo = ativo,
                    Descricao = descricao,
                    Tipo = tipo,
                    DataCadastro = DateTime.Now,
                    PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea = permiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea
                };

                Repositorio.Embarcador.Logistica.TipoSubareaCliente repTipoSubareaCliente = new Repositorio.Embarcador.Logistica.TipoSubareaCliente(unitOfWork);
                repTipoSubareaCliente.Inserir(tipoSubareaCliente, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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
                Repositorio.Embarcador.Logistica.TipoSubareaCliente repTipoSubareaCliente = new Repositorio.Embarcador.Logistica.TipoSubareaCliente(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                string descricao = Request.Params("Descricao");
                bool ativo = Request.GetBoolParam("Ativo");
                bool permiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea = Request.GetBoolParam("PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente)Request.GetIntParam("Tipo");
                Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente tipoSubareaCliente = repTipoSubareaCliente.BuscarPorCodigo(codigo, true);
                if (tipoSubareaCliente == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                tipoSubareaCliente.Ativo = ativo;
                tipoSubareaCliente.Descricao = descricao;
                tipoSubareaCliente.Tipo = tipo;
                tipoSubareaCliente.PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea = permiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea;
                repTipoSubareaCliente.Atualizar(tipoSubareaCliente, Auditado);
                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
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
                Repositorio.Embarcador.Logistica.TipoSubareaCliente repTipoSubareaCliente = new Repositorio.Embarcador.Logistica.TipoSubareaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente tipoSubareaCliente = repTipoSubareaCliente.BuscarPorCodigo(codigo, false);
                return new JsonpResult(new
                {
                    tipoSubareaCliente.Codigo,
                    tipoSubareaCliente.Descricao,
                    tipoSubareaCliente.Ativo,
                    tipoSubareaCliente.Tipo,
                    tipoSubareaCliente.PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea
            });
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.TipoSubareaCliente repTipoSubareaCliente = new Repositorio.Embarcador.Logistica.TipoSubareaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente tipoSubareaCliente = repTipoSubareaCliente.BuscarPorCodigo(codigo, false);
                if (tipoSubareaCliente == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                repTipoSubareaCliente.Deletar(tipoSubareaCliente, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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
    }
}
