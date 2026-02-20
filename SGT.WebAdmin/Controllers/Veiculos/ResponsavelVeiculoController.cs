using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/ResponsavelVeiculo")]
    public class ResponsavelVeiculoController : BaseController
    {
		#region Construtores

		public ResponsavelVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoFuncionarioResponsavel = Request.GetIntParam("FuncionarioResponsavel");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Funcionário Responsável", "FuncionarioResponsavel", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Lançamento", "DataLancamento", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Funcionário Lançamento", "FuncionarioLancamento", 20, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Veiculos.ResponsavelVeiculo repResponsavelVeiculo = new Repositorio.Embarcador.Veiculos.ResponsavelVeiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo> responsaveis = repResponsavelVeiculo.Consultar(codigoVeiculo, codigoFuncionarioResponsavel, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repResponsavelVeiculo.ContarConsulta(codigoVeiculo, codigoFuncionarioResponsavel));

                var lista = (from p in responsaveis
                             select new
                             {
                                 p.Codigo,
                                 Veiculo = p.Veiculo?.Placa_Formatada,
                                 FuncionarioResponsavel = p.FuncionarioResponsavel?.Nome,
                                 DataLancamento = p.DataLancamento.ToString("dd/MM/yyyy HH:mm"),
                                 FuncionarioLancamento = p.FuncionarioLancamento?.Nome
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

                Repositorio.Embarcador.Veiculos.ResponsavelVeiculo repResponsavelVeiculo = new Repositorio.Embarcador.Veiculos.ResponsavelVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo responsavelVeiculo = new Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo();

                PreencherResponsavelVeiculo(responsavelVeiculo, unitOfWork);

                repResponsavelVeiculo.Inserir(responsavelVeiculo, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Veiculos/ReposnsavelVeiculo");

                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Excluir)))
                    return new JsonpResult(false, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Veiculos.ResponsavelVeiculo repResponsavelVeiculo = new Repositorio.Embarcador.Veiculos.ResponsavelVeiculo(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo responsavelVeiculo = repResponsavelVeiculo.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Veiculo Veiculo = repVeiculo.BuscarPorCodigo(responsavelVeiculo.Veiculo.Codigo, true);

                if (responsavelVeiculo == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                Veiculo.FuncionarioResponsavel = null;

                repVeiculo.Atualizar(Veiculo, Auditado);
                repResponsavelVeiculo.Deletar(responsavelVeiculo, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o registro.");
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
                Repositorio.Embarcador.Veiculos.ResponsavelVeiculo repResponsavelVeiculo = new Repositorio.Embarcador.Veiculos.ResponsavelVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo responsavelVeiculo = repResponsavelVeiculo.BuscarPorCodigo(codigo, false);

                var dynResponsavelVeiculo = new
                {
                    responsavelVeiculo.Codigo,
                    responsavelVeiculo.Observacao,
                    Veiculo = responsavelVeiculo.Veiculo != null ? new { responsavelVeiculo.Veiculo.Codigo, responsavelVeiculo.Veiculo.Descricao } : null,
                    FuncionarioResponsavel = responsavelVeiculo.FuncionarioResponsavel != null ? new { responsavelVeiculo.FuncionarioResponsavel.Codigo, Descricao = responsavelVeiculo.FuncionarioResponsavel.Nome } : null
                };

                return new JsonpResult(dynResponsavelVeiculo);
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

        #endregion

        #region Métodos Privados

        private void PreencherResponsavelVeiculo(Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo responsavelVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);

            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int codigoFuncionarioResponsavel = Request.GetIntParam("FuncionarioResponsavel");

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

            responsavelVeiculo.DataLancamento = DateTime.Now;
            responsavelVeiculo.FuncionarioLancamento = this.Usuario;

            responsavelVeiculo.Observacao = Request.GetStringParam("Observacao");
            responsavelVeiculo.Veiculo = veiculo;
            responsavelVeiculo.FuncionarioResponsavel = repFuncionario.BuscarPorCodigo(codigoFuncionarioResponsavel);

            veiculo.FuncionarioResponsavel = responsavelVeiculo.FuncionarioResponsavel;
            repVeiculo.Atualizar(veiculo);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Atualizado Funcionário Responsável", unitOfWork);
        }

        #endregion
    }
}
