using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/TabelaFreteTipoOperacao")]
    public class TabelaFreteTipoOperacaoController : BaseController
    {
		#region Construtores

		public TabelaFreteTipoOperacaoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao tipoOperacaoEmissao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao)int.Parse(Request.Params("TipoOperacaoEmissao"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacaoEmissao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tabela de Frete", "TabelaFrete", 45, Models.Grid.Align.center, true);


                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "TabelaFrete")
                    propOrdena += ".Descricao";

                Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao repTabelaFreteTipoOperacao = new Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao(unitOfWork);


                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao> listaTabelaFreteTipoOperacao = repTabelaFreteTipoOperacao.Consultar(tipoOperacaoEmissao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTabelaFreteTipoOperacao.ContarConsulta(tipoOperacaoEmissao));
                var lista = (from p in listaTabelaFreteTipoOperacao
                            select new
                            {
                                p.Codigo,
                                TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissaoDescricao.RetornarDescricao(p.TipoOperacaoEmissao),
                                TabelaFrete = p.TabelaFrete.Descricao
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
                Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao tabelaFreteTipoOperacao = new Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao();
                tabelaFreteTipoOperacao.TipoOperacaoEmissao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao)int.Parse(Request.Params("TipoOperacaoEmissao"));
                tabelaFreteTipoOperacao.TabelaFrete = new Dominio.Entidades.Embarcador.Frete.TabelaFrete() { Codigo = int.Parse(Request.Params("TabelaFrete")) };
                tabelaFreteTipoOperacao.PagarPorToneladaPercentualExcedente = bool.Parse(Request.Params("PagarPorTonelada"));
                tabelaFreteTipoOperacao.PagarPorToneladaPercentualExcedenteRedespacho = bool.Parse(Request.Params("PagarPorToneladaRedespacho"));
                int codigoTabelaRedespacho = int.Parse(Request.Params("TabelaFreteRedespacho"));
                if (codigoTabelaRedespacho > 0)
                    tabelaFreteTipoOperacao.TabelaFreteRedespacho = new Dominio.Entidades.Embarcador.Frete.TabelaFrete() { Codigo = codigoTabelaRedespacho };
                else
                    tabelaFreteTipoOperacao.TabelaFreteRedespacho = null;

                Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao repTabelaFreteTipoOperacao = new Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao(unitOfWork);
                if (repTabelaFreteTipoOperacao.BuscarPorTipoOperacao(tabelaFreteTipoOperacao.TipoOperacaoEmissao) == null)
                {
                    repTabelaFreteTipoOperacao.Inserir(tabelaFreteTipoOperacao);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma configuração cadastrada para esse tipo de operação");
                }
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
                Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao repTabelaFreteTipoOperacao = new Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao tabelaFreteTipoOperacao = repTabelaFreteTipoOperacao.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));
                tabelaFreteTipoOperacao.TipoOperacaoEmissao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao)int.Parse(Request.Params("TipoOperacaoEmissao"));
                tabelaFreteTipoOperacao.TabelaFrete = new Dominio.Entidades.Embarcador.Frete.TabelaFrete() { Codigo = int.Parse(Request.Params("TabelaFrete")) };
                tabelaFreteTipoOperacao.PagarPorToneladaPercentualExcedente = bool.Parse(Request.Params("PagarPorTonelada"));
                tabelaFreteTipoOperacao.PagarPorToneladaPercentualExcedenteRedespacho = bool.Parse(Request.Params("PagarPorToneladaRedespacho"));
                int codigoTabelaRedespacho = int.Parse(Request.Params("TabelaFreteRedespacho"));
                if (codigoTabelaRedespacho > 0)
                    tabelaFreteTipoOperacao.TabelaFreteRedespacho = new Dominio.Entidades.Embarcador.Frete.TabelaFrete() { Codigo = codigoTabelaRedespacho };
                else
                    tabelaFreteTipoOperacao.TabelaFreteRedespacho = null;

                Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao tabelaFreteTipoOperacaoExiste = repTabelaFreteTipoOperacao.BuscarPorTipoOperacao(tabelaFreteTipoOperacao.TipoOperacaoEmissao);
                if (tabelaFreteTipoOperacaoExiste == null || (tabelaFreteTipoOperacaoExiste.Codigo == tabelaFreteTipoOperacao.Codigo))
                {
                    repTabelaFreteTipoOperacao.Atualizar(tabelaFreteTipoOperacao);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma configuraçãoo cadastrada para esse tipo de operação");
                }
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao repTabelaFreteTipoOperacao = new Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao tabelaFreteTipoOperacao = repTabelaFreteTipoOperacao.BuscarPorCodigo(codigo);
                var dynTabelaFreteTipoOperacao = new
                {
                    tabelaFreteTipoOperacao.Codigo,
                    tabelaFreteTipoOperacao.TipoOperacaoEmissao,
                    PagarPorTonelada = tabelaFreteTipoOperacao.PagarPorToneladaPercentualExcedente,
                    PagarPorToneladaRedespacho = tabelaFreteTipoOperacao.PagarPorToneladaPercentualExcedenteRedespacho,
                    TabelaFrete = new { tabelaFreteTipoOperacao.TabelaFrete.Codigo, tabelaFreteTipoOperacao.TabelaFrete.Descricao, tabelaFreteTipoOperacao.TabelaFrete.TipoTabelaFrete },
                    TabelaFreteRedespacho = new { Codigo = tabelaFreteTipoOperacao.TabelaFreteRedespacho != null ? tabelaFreteTipoOperacao.TabelaFreteRedespacho.Codigo : 0, Descricao = tabelaFreteTipoOperacao.TabelaFreteRedespacho != null ? tabelaFreteTipoOperacao.TabelaFreteRedespacho.Descricao : "", TipoTabelaFrete = tabelaFreteTipoOperacao.TabelaFreteRedespacho != null ? tabelaFreteTipoOperacao.TabelaFreteRedespacho.TipoTabelaFrete : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.todas }
                };
                return new JsonpResult(dynTabelaFreteTipoOperacao);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao repTabelaFreteTipoOperacao = new Repositorio.Embarcador.Frete.TabelaFreteTipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao tabelaFreteTipoOperacao = repTabelaFreteTipoOperacao.BuscarPorCodigo(codigo);
                repTabelaFreteTipoOperacao.Deletar(tabelaFreteTipoOperacao);
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
