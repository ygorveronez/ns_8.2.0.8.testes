using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/TipoComunicacaoRastreador")]
    public class TipoComunicacaoRastreadorController : BaseController
    {
		#region Construtores

		public TipoComunicacaoRastreadorController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 70, Models.Grid.Align.left, true);

                if(ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador repTipoComunicacaoRastreador = new Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador(unitOfWork);

                List<Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador> tecnologias = repTipoComunicacaoRastreador.Consultar(descricao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repTipoComunicacaoRastreador.ContarConsulta(descricao, ativo));

                var lista = (from p in tecnologias
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador repTipoComunicacaoRastreador = new Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador(unitOfWork);

                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");

                bool ativo = Request.GetBoolParam("Ativo");

                Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador tipoComunicacaoRastreador = new Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador()
                {
                    Ativo = ativo,
                    CodigoIntegracao = codigoIntegracao,
                    Descricao = descricao
                };

                repTipoComunicacaoRastreador.Inserir(tipoComunicacaoRastreador, Auditado);

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
                Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador repTipoComunicacaoRastreador = new Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");

                bool ativo = Request.GetBoolParam("Ativo");

                Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador tipoComunicacaoRastreador = repTipoComunicacaoRastreador.BuscarPorCodigo(codigo, true);

                tipoComunicacaoRastreador.Ativo = ativo;
                tipoComunicacaoRastreador.CodigoIntegracao = codigoIntegracao;
                tipoComunicacaoRastreador.Descricao = descricao;

                repTipoComunicacaoRastreador.Atualizar(tipoComunicacaoRastreador, Auditado);

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
                Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador repTipoComunicacaoRastreador = new Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador tipoComunicacaoRastreador = repTipoComunicacaoRastreador.BuscarPorCodigo(codigo, false);

                var retorno = new
                {
                    tipoComunicacaoRastreador.Codigo,
                    tipoComunicacaoRastreador.CodigoIntegracao,
                    tipoComunicacaoRastreador.Descricao,
                    tipoComunicacaoRastreador.Ativo
                };

                return new JsonpResult(retorno);
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
                Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador repTipoComunicacaoRastreador = new Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador tecnologiaRastreador = repTipoComunicacaoRastreador.BuscarPorCodigo(codigo, true);

                repTipoComunicacaoRastreador.Deletar(tecnologiaRastreador, Auditado);

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
