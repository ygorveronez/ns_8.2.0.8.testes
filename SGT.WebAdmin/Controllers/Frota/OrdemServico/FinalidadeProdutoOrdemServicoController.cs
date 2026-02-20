using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.OrdemServico
{
    [CustomAuthorize("Frota/FinalidadeProdutoOrdemServico")]
    public class FinalidadeProdutoOrdemServicoController : BaseController
    {
		#region Construtores

		public FinalidadeProdutoOrdemServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                
                int codigoTipoMovimento;
                int.TryParse(Request.Params("TipoMovimento"), out codigoTipoMovimento);
                                
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAux;
                if (Enum.TryParse(Request.Params("Ativo"), out situacaoAux))
                    situacao = situacaoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Movimento de Uso", "TipoMovimentoUso", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Movimento de Reversão", "TipoMovimentoUso", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Ativo", 15, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "TipoMovimentoUso" || propOrdena == "TipoMovimentoReversao")
                    propOrdena += ".Descricao";

                Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico repFinalidadeProduto = new Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico(unidadeDeTrabalho);
                                
                List<Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico> listaFinalidadeProduto = repFinalidadeProduto.Consultar(descricao, situacao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repFinalidadeProduto.ContarConsulta(descricao, situacao, propOrdena));

                grid.AdicionaRows((from obj in listaFinalidadeProduto
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Descricao,
                                       TipoMovimentoUso = obj.TipoMovimentoUso.Descricao,
                                       TipoMovimentoReversao = obj.TipoMovimentoReversao.Descricao,
                                       Ativo = obj.DescricaoAtivo
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string observacao = Request.Params("Observacao");

                int codigoTipoMovimentoUso, codigoTipoMovimentoReversao;
                int.TryParse(Request.Params("TipoMovimentoUso"), out codigoTipoMovimentoUso);
                int.TryParse(Request.Params("TipoMovimentoReversao"), out codigoTipoMovimentoReversao);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico repFinalidadeProduto = new Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico finalidadeProduto = new Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico();

                finalidadeProduto.Ativo = ativo;
                finalidadeProduto.Descricao = descricao;
                finalidadeProduto.Observacao = observacao;
                finalidadeProduto.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                finalidadeProduto.TipoMovimentoUso = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUso);
                finalidadeProduto.TipoMovimentoReversao = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversao);

                repFinalidadeProduto.Inserir(finalidadeProduto);
                                
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string observacao = Request.Params("Observacao");

                int codigoTipoMovimentoUso, codigoTipoMovimentoReversao, codigo;
                int.TryParse(Request.Params("TipoMovimentoUso"), out codigoTipoMovimentoUso);
                int.TryParse(Request.Params("TipoMovimentoReversao"), out codigoTipoMovimentoReversao);
                int.TryParse(Request.Params("Codigo"), out codigo);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico repFinalidadeProduto = new Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico finalidadeProduto = repFinalidadeProduto.BuscarPorCodigo(codigo);

                finalidadeProduto.Ativo = ativo;
                finalidadeProduto.Descricao = descricao;
                finalidadeProduto.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                finalidadeProduto.Observacao = observacao;
                finalidadeProduto.TipoMovimentoUso = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUso);
                finalidadeProduto.TipoMovimentoReversao = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversao);

                repFinalidadeProduto.Atualizar(finalidadeProduto);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico repFinalidadeProduto = new Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico finalidadeProduto = repFinalidadeProduto.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    finalidadeProduto.Ativo,
                    finalidadeProduto.Codigo,
                    finalidadeProduto.Descricao,
                    finalidadeProduto.CodigoIntegracao,
                    finalidadeProduto.Observacao,
                    TipoMovimentoUso = new
                    {
                        Descricao = finalidadeProduto.TipoMovimentoUso.Descricao,
                        Codigo = finalidadeProduto.TipoMovimentoUso.Codigo
                    },
                    TipoMovimentoReversao = new
                    {
                        Descricao = finalidadeProduto.TipoMovimentoReversao.Descricao,
                        Codigo = finalidadeProduto.TipoMovimentoReversao.Codigo
                    }
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
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico repFinalidadeProduto = new Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico finalidadeProduto = repFinalidadeProduto.BuscarPorCodigo(codigo);

                repFinalidadeProduto.Deletar(finalidadeProduto);

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
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
