using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Patrimonio
{
    [CustomAuthorize("Patrimonio/BaixaBem")]
    public class BaixaBemController : BaseController
    {
		#region Construtores

		public BaixaBemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoBem = 0;
                int.TryParse(Request.Params("Bem"), out codigoBem);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBem statusBem;
                Enum.TryParse(Request.Params("Status"), out statusBem);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Patrimônio", "Bem", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Venda", "ValorVenda", 10, Models.Grid.Align.right, true);

                Repositorio.Embarcador.Patrimonio.BemBaixa repBemBaixa = new Repositorio.Embarcador.Patrimonio.BemBaixa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Patrimonio.BemBaixa> baixasBem = repBemBaixa.Consultar(codigoEmpresa, codigoBem, statusBem, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBemBaixa.ContarConsulta(codigoEmpresa, codigoBem, statusBem));

                var lista = (from p in baixasBem
                             select new
                             {
                                 p.Codigo,
                                 Bem = p.Bem != null ? p.Bem.Descricao : string.Empty,
                                 DescricaoStatus = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBemHelper.ObterDescricao(p.Status),
                                 Data = p.Data.ToString("dd/MM/yyyy"),
                                 ValorVenda = p.ValorVenda.ToString("n2")
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

                Repositorio.Embarcador.Patrimonio.BemBaixa repBemBaixa = new Repositorio.Embarcador.Patrimonio.BemBaixa(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemBaixa bemBaixa = new Dominio.Entidades.Embarcador.Patrimonio.BemBaixa();

                PreencherBemBaixa(bemBaixa, unitOfWork);
                repBemBaixa.Inserir(bemBaixa, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Patrimonio.BemBaixa repBemBaixa = new Repositorio.Embarcador.Patrimonio.BemBaixa(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemBaixa bemBaixa = repBemBaixa.BuscarPorCodigo(codigo, true);

                PreencherBemBaixa(bemBaixa, unitOfWork);
                repBemBaixa.Atualizar(bemBaixa, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Patrimonio.BemBaixa repBemBaixa = new Repositorio.Embarcador.Patrimonio.BemBaixa(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemBaixa bemBaixa = repBemBaixa.BuscarPorCodigo(codigo);

                var dynBemBaixa = new
                {
                    bemBaixa.Codigo,
                    Data = bemBaixa.Data.ToString("dd/MM/yyyy"),
                    ValorVenda = bemBaixa.ValorVenda.ToString("n2"),
                    bemBaixa.Status,
                    Bem = bemBaixa.Bem != null ? new { bemBaixa.Bem.Codigo, bemBaixa.Bem.Descricao } : null,
                    NotaFiscal = bemBaixa.NotaFiscal != null ? new { bemBaixa.NotaFiscal.Codigo, bemBaixa.NotaFiscal.Descricao } : null,
                    Funcionario = bemBaixa.Funcionario != null ? new { bemBaixa.Funcionario.Codigo, bemBaixa.Funcionario.Descricao } : null,
                };

                return new JsonpResult(dynBemBaixa);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Patrimonio.BemBaixa repBemBaixa = new Repositorio.Embarcador.Patrimonio.BemBaixa(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemBaixa bemBaixa = repBemBaixa.BuscarPorCodigo(codigo, true);

                if (bemBaixa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repBemBaixa.Deletar(bemBaixa, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherBemBaixa(Dominio.Entidades.Embarcador.Patrimonio.BemBaixa bemBaixa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);

            int codigoEmpresa = 0;
            int.TryParse(Request.Params("NotaFiscal"), out int codigoNotaFiscal);
            int.TryParse(Request.Params("Funcionario"), out int codigoFuncionario);
            int.TryParse(Request.Params("Bem"), out int codigoBem);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            decimal.TryParse(Request.Params("ValorVenda"), out decimal valorVenda);

            DateTime.TryParse(Request.Params("Data"), out DateTime data);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBem statusBem;
            Enum.TryParse(Request.Params("Status"), out statusBem);

            bemBaixa.Status = statusBem;
            bemBaixa.ValorVenda = valorVenda;
            bemBaixa.Bem = repBem.BuscarPorCodigo(codigoBem);
            bemBaixa.Data = data;

            if (codigoNotaFiscal > 0)
                bemBaixa.NotaFiscal = repNotaFiscal.BuscarPorCodigo(codigoNotaFiscal);
            else
                bemBaixa.NotaFiscal = null;
            if (codigoFuncionario > 0)
                bemBaixa.Funcionario = repFuncionario.BuscarPorCodigo(codigoFuncionario);
            else
                bemBaixa.Funcionario = null;
            if (codigoEmpresa > 0)
                bemBaixa.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            else
                bemBaixa.Empresa = bemBaixa.Bem.Empresa;
        }

        #endregion
    }
}
