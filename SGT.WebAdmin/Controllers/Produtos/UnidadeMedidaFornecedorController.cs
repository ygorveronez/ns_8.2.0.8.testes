using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/UnidadeMedidaFornecedor")]
    public class UnidadeMedidaFornecedorController : BaseController
    {
		#region Construtores

		public UnidadeMedidaFornecedorController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricaoFornecedor = Request.Params("DescricaoFornecedor");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida unidadeDeMedida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida)int.Parse(Request.Params("UnidadeDeMedida"));

                int empresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição Fornecedor", "DescricaoFornecedor", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Unidade Medida", "DescricaoUnidadeDeMedida", 10, Models.Grid.Align.left, false);

                string ordenacao = grid.header[grid.indiceColunaOrdena].data;
                if (ordenacao == "DescricaoUnidadeDeMedida")
                    ordenacao = "UnidadeDeMedida";

                Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor repUnidadeMedidaFornecedor = new Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor(unitOfWork);
                List<Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor> listaUnidadeMedidaFornecedor = repUnidadeMedidaFornecedor.Consultar(empresa, descricaoFornecedor, unidadeDeMedida, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repUnidadeMedidaFornecedor.ContarConsulta(empresa, descricaoFornecedor, unidadeDeMedida));
                var lista = (from p in listaUnidadeMedidaFornecedor
                             select new
                             {
                                 p.Codigo,
                                 p.DescricaoFornecedor,
                                 p.DescricaoUnidadeDeMedida
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

                string descricaoFornecedor = Request.Params("DescricaoFornecedor");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida unidadeDeMedida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida)int.Parse(Request.Params("UnidadeDeMedida"));

                Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor repUnidadeMedidaFornecedor = new Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor unidadeMedidaFornecedor = new Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor();

                unidadeMedidaFornecedor.DescricaoFornecedor = descricaoFornecedor;
                unidadeMedidaFornecedor.UnidadeDeMedida = unidadeDeMedida;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    unidadeMedidaFornecedor.Empresa = this.Usuario.Empresa;

                repUnidadeMedidaFornecedor.Inserir(unidadeMedidaFornecedor, Auditado);

                unitOfWork.CommitChanges();
                object retorno = new
                {
                    unidadeMedidaFornecedor.Codigo
                };

                return new JsonpResult(retorno, true, "Sucesso");
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

                string descricaoFornecedor = Request.Params("DescricaoFornecedor");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida unidadeDeMedida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida)int.Parse(Request.Params("UnidadeDeMedida"));
                Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor repUnidadeMedidaFornecedor = new Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor unidadeMedidaFornecedor = repUnidadeMedidaFornecedor.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                unidadeMedidaFornecedor.DescricaoFornecedor = descricaoFornecedor;
                unidadeMedidaFornecedor.UnidadeDeMedida = unidadeDeMedida;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    unidadeMedidaFornecedor.Empresa = this.Usuario.Empresa;

                repUnidadeMedidaFornecedor.Atualizar(unidadeMedidaFornecedor, Auditado);

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
                Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor repUnidadeMedidaFornecedor = new Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor unidadeMedidaFornecedor = repUnidadeMedidaFornecedor.BuscarPorCodigo(codigo);

                var dynProduto = new
                {
                    unidadeMedidaFornecedor.Codigo,
                    unidadeMedidaFornecedor.DescricaoFornecedor,
                    unidadeMedidaFornecedor.UnidadeDeMedida
                };
                return new JsonpResult(dynProduto);
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
                Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor repUnidadeMedidaFornecedor = new Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor unidadeMedidaFornecedor = repUnidadeMedidaFornecedor.BuscarPorCodigo(codigo);
                repUnidadeMedidaFornecedor.Deletar(unidadeMedidaFornecedor, Auditado);
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
    }
}
