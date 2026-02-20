using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;



namespace SGT.WebAdmin.Controllers.Acertos
{
    [CustomAuthorize("Acertos/TipoDespesa")]
    public class TipoDespesaController : BaseController
    {
        #region Construtores

        public TipoDespesaController(Conexao conexao) : base(conexao) { }

        #endregion
        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Acerto.TipoDespesaAcerto repTipoDespesa = new Repositorio.Embarcador.Acerto.TipoDespesaAcerto(unitOfWork);

                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa tipoDespesa;
                Enum.TryParse(Request.Params("TipoDespesa"), out tipoDespesa);
               
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);

                List<Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto> listaTipoDespesa = repTipoDespesa.Consultar(descricao, tipoDespesa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoDespesa.ContarConsulta(descricao, tipoDespesa));
                var lista = from p in listaTipoDespesa
                            select new
                            {
                                p.Codigo,
                                p.Descricao
                            };
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
        public async Task<IActionResult> BuscarTiposDespesas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Acerto.TipoDespesaAcerto repTipoDespesa = new Repositorio.Embarcador.Acerto.TipoDespesaAcerto(unitOfWork);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);

                List<Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto> listaTipoDespesa = repTipoDespesa.ConsultarTodosTiposDespesas(grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoDespesa.ContarTodosTiposDespesas());
                var lista = from p in listaTipoDespesa
                            select new
                            {
                                p.Codigo,
                                p.Descricao
                            };
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

                Repositorio.Embarcador.Acerto.TipoDespesaAcerto repTipoDespesa = new Repositorio.Embarcador.Acerto.TipoDespesaAcerto(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto tipoDespesa = new Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto();

                tipoDespesa.Descricao = Request.Params("Descricao");
                tipoDespesa.CodigoIntegracao = Request.Params("CodigoIntegracao");
                tipoDespesa.Observacao = Request.Params("Observacao");
                string tipoDespesaParam = Request.Params("TipoDespesa");

                if (!string.IsNullOrEmpty(tipoDespesaParam) &&
                    Enum.TryParse(tipoDespesaParam, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa tipoDespesaEnum))
                {
                    tipoDespesa.TipoDeDespesa = tipoDespesaEnum;
                }
                else
                {
                    tipoDespesa.TipoDeDespesa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa.Geral;
                }

                repTipoDespesa.Inserir(tipoDespesa, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
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
                Repositorio.Embarcador.Acerto.TipoDespesaAcerto repTipoDespesa = new Repositorio.Embarcador.Acerto.TipoDespesaAcerto(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto tipoDespesa = repTipoDespesa.BuscarPorCodigo(codigo, true);

                tipoDespesa.Descricao = Request.Params("Descricao");
                tipoDespesa.CodigoIntegracao = Request.Params("CodigoIntegracao");
                tipoDespesa.Observacao = Request.Params("Observacao");
                string tipoDespesaParam = Request.Params("TipoDespesa");

                if (!string.IsNullOrEmpty(tipoDespesaParam) &&
                    Enum.TryParse(tipoDespesaParam, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa tipoDespesaEnum))
                {
                    tipoDespesa.TipoDeDespesa = tipoDespesaEnum;
                }
                else
                {
                    tipoDespesa.TipoDeDespesa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa.Geral;
                }
                repTipoDespesa.Atualizar(tipoDespesa, Auditado);
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Acerto.TipoDespesaAcerto repTipoDespesa = new Repositorio.Embarcador.Acerto.TipoDespesaAcerto(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto tipoDespesa = repTipoDespesa.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    tipoDespesa.Codigo,
                    tipoDespesa.Descricao,
                    tipoDespesa.CodigoIntegracao,
                    tipoDespesa.Observacao,
                    TipoDespesa = tipoDespesa.TipoDeDespesa
                };
                return new JsonpResult(dynProcessoMovimento);
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
                Repositorio.Embarcador.Acerto.TipoDespesaAcerto reTipoDespesa = new Repositorio.Embarcador.Acerto.TipoDespesaAcerto(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto tipoDespesa = reTipoDespesa.BuscarPorCodigo(codigo);
                reTipoDespesa.Deletar(tipoDespesa, Auditado);
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
