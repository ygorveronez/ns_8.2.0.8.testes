using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/TipoBidding")]
    public class TipoBiddingController : BaseController
    {
		#region Construtores

		public TipoBiddingController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBidding filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código", "CodigoIntegracao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Exibir Grid de Rank", "Rank", 20, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Bidding.TipoBidding repTipoBidding = new Repositorio.Embarcador.Bidding.TipoBidding(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Bidding.TipoBidding> tipoBiddings = repTipoBidding.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repTipoBidding.ContarConsulta(filtrosPesquisa));

                var lista = (from p in tipoBiddings
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoIntegracao,
                                 p.Descricao,
                                 p.DescricaoStatus,
                                 Rank = p.ExibirRankOfertas
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

                Repositorio.Embarcador.Bidding.TipoBidding repTipoBidding = new Repositorio.Embarcador.Bidding.TipoBidding(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.TipoBidding tipoBidding = new Dominio.Entidades.Embarcador.Bidding.TipoBidding();

                PreencherTipoBidding(tipoBidding, unitOfWork);

                repTipoBidding.Inserir(tipoBidding, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { tipoBidding.Codigo });
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

                Repositorio.Embarcador.Bidding.TipoBidding repTipoBidding = new Repositorio.Embarcador.Bidding.TipoBidding(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.TipoBidding tipoBidding = repTipoBidding.BuscarPorCodigo(codigo, true);

                if (tipoBidding == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherTipoBidding(tipoBidding, unitOfWork);

                repTipoBidding.Atualizar(tipoBidding, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

                Repositorio.Embarcador.Bidding.TipoBidding repTipoBidding = new Repositorio.Embarcador.Bidding.TipoBidding(unitOfWork);
                Repositorio.Embarcador.Bidding.TipoBiddingAnexo repositorioTipoBiddingAnexoAnexo = new Repositorio.Embarcador.Bidding.TipoBiddingAnexo(unitOfWork);

                Dominio.Entidades.Embarcador.Bidding.TipoBidding tipoBidding = repTipoBidding.BuscarPorCodigo(codigo, false);

                if (tipoBidding == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Bidding.TipoBiddingAnexo> anexos = repositorioTipoBiddingAnexoAnexo.BuscarPorTipoBidding(tipoBidding.Codigo);

                var dynTipoBidding = new
                {
                    tipoBidding.Codigo,
                    tipoBidding.Descricao,
                    tipoBidding.CodigoIntegracao,
                    tipoBidding.Status,
                    tipoBidding.ExibirRankOfertas,
                    tipoBidding.NaoIncluirImpostoValorTotalOferta,
                    tipoBidding.NaoPossuiPedagioFluxoOferta,
                    tipoBidding.PermitirOfertasComponentes,
                    ComponentesFrete = (
                        from obj in tipoBidding.ComponentesFrete
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }).ToList(),
                    Anexos = (
                        from obj in anexos
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                        }
                    ).ToList()
                };

                return new JsonpResult(dynTipoBidding);
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

                Repositorio.Embarcador.Bidding.TipoBidding repTipoBidding = new Repositorio.Embarcador.Bidding.TipoBidding(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.TipoBidding tipoBidding = repTipoBidding.BuscarPorCodigo(codigo, true);

                if (tipoBidding == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repTipoBidding.Deletar(tipoBidding, Auditado);

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

        public async Task<IActionResult> VerificarAnexoTipoBidding()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Bidding.TipoBiddingAnexo repositorioTipoBiddingAnexo = new Repositorio.Embarcador.Bidding.TipoBiddingAnexo(unitOfWork);

                int codigoTipoBidding = Request.GetIntParam("CodigoTipoBidding");

                List<Dominio.Entidades.Embarcador.Bidding.TipoBiddingAnexo> tipoBiddingAnexo = repositorioTipoBiddingAnexo.BuscarPorTipoBidding(codigoTipoBidding);

                var retorno = new
                {
                    Anexos = (
                        from o in tipoBiddingAnexo
                        select new
                        {
                            o.Codigo,
                            o.Descricao,
                            o.NomeArquivo,
                        }
                    ).ToList(),
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar os Anexos do Tipo Bidding.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarQuestionarioTipoBidding()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao repositorioBiddingChecklistQuestionarioPadrao = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao(unitOfWork);

                int codigoTipoBidding = Request.GetIntParam("CodigoTipoBidding");

                List<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao> biddingChecklistQuestionarioPadrao = repositorioBiddingChecklistQuestionarioPadrao.BuscarPorTipoBidding(codigoTipoBidding);

                if (biddingChecklistQuestionarioPadrao == null)
                    return new JsonpResult(false, "");

                var retorno = new
                {
                    Questionarios = (
                        from questionario in biddingChecklistQuestionarioPadrao
                        select new
                        {
                            Codigo = Guid.NewGuid().ToString().Replace("-", ""),
                            questionario.Descricao,
                            Requisito = questionario.Requisito.ObterDescricao(),
                            ChecklistAnexo = from ChecklistAnexo in questionario.Anexos
                                select new
                                {
                                    Codigo = Guid.NewGuid().ToString().Replace("-", ""),
                                    ChecklistAnexo.Descricao,
                                    ChecklistAnexo.NomeArquivo
                                }
                        }
                    ).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar os Anexos do Tipo Bidding.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherTipoBidding(Dominio.Entidades.Embarcador.Bidding.TipoBidding tipoBidding, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            List<int> codigosComponentesFrete = Request.GetListParam<int>("ComponentesFrete");
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentesFrete = codigosComponentesFrete.Count > 0 ? repComponenteFrete.BuscarPorCodigos(codigosComponentesFrete) : new List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete>();

            tipoBidding.Descricao = Request.GetStringParam("Descricao");
            tipoBidding.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tipoBidding.Status = Request.GetBoolParam("Status");
            tipoBidding.ExibirRankOfertas = Request.GetBoolParam("ExibirRankOfertas");
            tipoBidding.NaoIncluirImpostoValorTotalOferta = Request.GetBoolParam("NaoIncluirImpostoValorTotalOferta");
            tipoBidding.NaoPossuiPedagioFluxoOferta = Request.GetBoolParam("NaoPossuiPedagioFluxoOferta");
            tipoBidding.PermitirOfertasComponentes = Request.GetBoolParam("PermitirOfertasComponentes");
            tipoBidding.ComponentesFrete = componentesFrete;

            if (tipoBidding.Codigo == 0)
                tipoBidding.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBidding ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBidding()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0
            };
        }

        #endregion
    }
}
