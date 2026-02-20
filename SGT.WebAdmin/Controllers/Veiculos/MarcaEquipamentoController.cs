using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    public class MarcaEquipamentoController : BaseController
    {
		#region Construtores

		public MarcaEquipamentoController(Conexao conexao) : base(conexao) { }

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

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Veiculos.MarcaEquipamento repMarcaEquipamento = new Repositorio.Embarcador.Veiculos.MarcaEquipamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento> listaMarcaEquipamento = repMarcaEquipamento.Consultar(descricao, ativo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMarcaEquipamento.ContarConsulta(descricao, ativo));

                var lista = (from p in listaMarcaEquipamento
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

                Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento marcaEquipamento = new Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento();
                marcaEquipamento.Ativo = bool.Parse(Request.Params("Ativo"));
                marcaEquipamento.Descricao = Request.Params("Descricao");
                marcaEquipamento.Observacao = Request.Params("Observacao");

                repMarcaEquipamento.Inserir(marcaEquipamento, Auditado);

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
                Repositorio.Embarcador.Veiculos.MarcaEquipamento repMarcaEquipamento = new Repositorio.Embarcador.Veiculos.MarcaEquipamento(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento marcaEquipamento = repMarcaEquipamento.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                marcaEquipamento.Ativo = bool.Parse(Request.Params("Ativo"));
                marcaEquipamento.Descricao = Request.Params("Descricao");
                marcaEquipamento.Observacao = Request.Params("Observacao");

                repMarcaEquipamento.Atualizar(marcaEquipamento, Auditado);

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
                Repositorio.Embarcador.Veiculos.MarcaEquipamento repMarcaEquipamento = new Repositorio.Embarcador.Veiculos.MarcaEquipamento(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento marcaEquipamento = repMarcaEquipamento.BuscarPorCodigo(codigo);

                var dynMarcaEquipamento = new
                {
                    marcaEquipamento.Ativo,
                    marcaEquipamento.Codigo,
                    marcaEquipamento.Descricao,
                    marcaEquipamento.Observacao
                };

                return new JsonpResult(dynMarcaEquipamento);
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
                Repositorio.Embarcador.Veiculos.MarcaEquipamento repMarcaEquipamento = new Repositorio.Embarcador.Veiculos.MarcaEquipamento(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento marcaEquipamento = repMarcaEquipamento.BuscarPorCodigo(codigo);

                repMarcaEquipamento.Deletar(marcaEquipamento, Auditado);

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
