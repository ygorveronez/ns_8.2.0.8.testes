using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ViaTransporte
{
    [CustomAuthorize("Cargas/ViaTransporte")]
    public class ViaTransporteController : BaseController
    {
		#region Construtores

		public ViaTransporteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ViaTransporte repViaTransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(unitOfWork);
                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");
                string codigoIntegracaoEnvio = Request.Params("CodigoIntegracaoEnvio");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código de Integração de Envio", "CodigoIntegracaoEnvio", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Cargas.ViaTransporte> viaTransporte = repViaTransporte.Consultar(codigoIntegracao, codigoIntegracaoEnvio, descricao, ativo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repViaTransporte.ContarConsulta(codigoIntegracao, codigoIntegracaoEnvio, descricao, ativo));

                var retorno = (from obj in viaTransporte
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.CodigoIntegracao,
                                   obj.CodigoIntegracaoEnvio,
                                   obj.DescricaoAtivo
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as vias de Transportes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            bool novo = true;
            try
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.ViaTransporte repViaTransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte = new Dominio.Entidades.Embarcador.Cargas.ViaTransporte();
                viaTransporte.Ativo = bool.Parse(Request.Params("Ativo"));
                viaTransporte.Descricao = Request.Params("Descricao");
                viaTransporte.CodigoIntegracao = Request.Params("CodigoIntegracao");
                viaTransporte.CodigoIntegracaoEnvio = Request.Params("CodigoIntegracaoEnvio");

                int tipoOperacaoPadrao = Request.GetIntParam("TipoOperacaoPadrao");

                if (tipoOperacaoPadrao > 0)
                    viaTransporte.TipoOperacaoPadrao = repTipoOperacao.BuscarPorCodigo(tipoOperacaoPadrao);
                else
                    viaTransporte.TipoOperacaoPadrao = null;

                if (!string.IsNullOrEmpty(viaTransporte.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Cargas.ViaTransporte existeViaTransporte = repViaTransporte.BuscarPorCodigoIntegracao(viaTransporte.CodigoIntegracao);
                    if (existeViaTransporte != null)
                        novo = false;
                }
              
                if (novo)
                {
                    unitOfWork.Start();
                    repViaTransporte.Inserir(viaTransporte, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, true, "Já existe uma via de transporte cadastrada para o código informado.");
            }
            catch (Exception ex)
            {
                if (novo)
                    unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.ViaTransporte repViaTransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte = repViaTransporte.BuscarPorCodigo(codigo, true);

                viaTransporte.Ativo = bool.Parse(Request.Params("Ativo"));
                viaTransporte.Descricao = Request.Params("Descricao");
                viaTransporte.CodigoIntegracao = Request.Params("CodigoIntegracao");
                viaTransporte.CodigoIntegracaoEnvio = Request.Params("CodigoIntegracaoEnvio");
                int tipoOperacaoPadrao = Request.GetIntParam("TipoOperacaoPadrao");

                if (tipoOperacaoPadrao > 0)
                    viaTransporte.TipoOperacaoPadrao = repTipoOperacao.BuscarPorCodigo(tipoOperacaoPadrao);
                else
                    viaTransporte.TipoOperacaoPadrao = null;

                bool atualizar = true;
                string compl = "cadastrada para o código informado.";

                if (!string.IsNullOrEmpty(viaTransporte.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Cargas.ViaTransporte existeViaTransporte = repViaTransporte.BuscarPorCodigoIntegracao(viaTransporte.CodigoIntegracao);
                    if (existeViaTransporte != null)
                        if (existeViaTransporte.Codigo != viaTransporte.Codigo)
                            atualizar = false;
                }

                if (atualizar)
                {
                    Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = repViaTransporte.Atualizar(viaTransporte, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe uma via de transporte" + compl);
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Cargas.ViaTransporte repViaTransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte = repViaTransporte.BuscarPorCodigo(codigo, false);

                var dynViaTransporte = new
                {
                    viaTransporte.Codigo,
                    viaTransporte.Descricao,
                    TipoOperacaoPadrao = new { Codigo = viaTransporte.TipoOperacaoPadrao?.Codigo ?? 0, Descricao = viaTransporte.TipoOperacaoPadrao?.Descricao ??  ""},
                    viaTransporte.CodigoIntegracao,
                    viaTransporte.CodigoIntegracaoEnvio,
                    viaTransporte.Ativo
                };

                return new JsonpResult(dynViaTransporte);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Cargas.ViaTransporte repViaTransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte = repViaTransporte.BuscarPorCodigo(codigo, false);
                repViaTransporte.Deletar(viaTransporte, Auditado);
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
        }

        #endregion
    }
}
