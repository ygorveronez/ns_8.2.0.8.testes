using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    public class ModeloEquipamentoController : BaseController
    {
		#region Construtores

		public ModeloEquipamentoController(Conexao conexao) : base(conexao) { }

		#endregion

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
                grid.AdicionarCabecalho("Descrição", "Descricao", 80, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("CodigoMarca", false);
                grid.AdicionarCabecalho("DescricaoMarca", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Veiculos.ModeloEquipamento repModeloEquipamento = new Repositorio.Embarcador.Veiculos.ModeloEquipamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento> listaModeloEquipamento = repModeloEquipamento.Consultar(descricao, ativo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repModeloEquipamento.ContarConsulta(descricao, ativo));

                var lista = (from p in listaModeloEquipamento
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo,
                                 CodigoMarca = p.MarcaEquipamento?.Codigo ?? 0,
                                 DescricaoMarca = p.MarcaEquipamento?.Descricao ?? "",
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

                Repositorio.Embarcador.Veiculos.MarcaEquipamento repMarcaEquipamento = new Repositorio.Embarcador.Veiculos.MarcaEquipamento(unitOfWork);
                Repositorio.Embarcador.Veiculos.ModeloEquipamento repModeloEquipamento = new Repositorio.Embarcador.Veiculos.ModeloEquipamento(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento modeloEquipamento = new Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento();
                modeloEquipamento.Ativo = bool.Parse(Request.Params("Ativo"));
                modeloEquipamento.Descricao = Request.Params("Descricao");
                modeloEquipamento.Observacao = Request.Params("Observacao");
                int codigoMarca = Request.GetIntParam("MarcaEquipamento");
                if (codigoMarca > 0)
                    modeloEquipamento.MarcaEquipamento = repMarcaEquipamento.BuscarPorCodigo(codigoMarca);
                else
                    modeloEquipamento.MarcaEquipamento = null;

                repModeloEquipamento.Inserir(modeloEquipamento, Auditado);

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
                Repositorio.Embarcador.Veiculos.ModeloEquipamento repModeloEquipamento = new Repositorio.Embarcador.Veiculos.ModeloEquipamento(unitOfWork);
                Repositorio.Embarcador.Veiculos.MarcaEquipamento repMarcaEquipamento = new Repositorio.Embarcador.Veiculos.MarcaEquipamento(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento modeloEquipamento = repModeloEquipamento.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                modeloEquipamento.Ativo = bool.Parse(Request.Params("Ativo"));
                modeloEquipamento.Descricao = Request.Params("Descricao");
                modeloEquipamento.Observacao = Request.Params("Observacao");
                int codigoMarca = Request.GetIntParam("MarcaEquipamento");
                if (codigoMarca > 0)
                    modeloEquipamento.MarcaEquipamento = repMarcaEquipamento.BuscarPorCodigo(codigoMarca);
                else
                    modeloEquipamento.MarcaEquipamento = null;

                repModeloEquipamento.Atualizar(modeloEquipamento, Auditado);

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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Veiculos.ModeloEquipamento repModeloEquipamento = new Repositorio.Embarcador.Veiculos.ModeloEquipamento(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento modeloEquipamento = repModeloEquipamento.BuscarPorCodigo(codigo);

                var dynModeloEquipamento = new
                {
                    modeloEquipamento.Ativo,
                    modeloEquipamento.Codigo,
                    modeloEquipamento.Descricao,
                    modeloEquipamento.Observacao,
                    MarcaEquipamento = modeloEquipamento.MarcaEquipamento != null ? new { Codigo = modeloEquipamento.MarcaEquipamento.Codigo, Descricao = modeloEquipamento.MarcaEquipamento.Descricao } : null
                };

                return new JsonpResult(dynModeloEquipamento);
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

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Veiculos.ModeloEquipamento repModeloEquipamento = new Repositorio.Embarcador.Veiculos.ModeloEquipamento(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento modeloEquipamento = repModeloEquipamento.BuscarPorCodigo(codigo);

                repModeloEquipamento.Deletar(modeloEquipamento, Auditado);

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


    }
}
