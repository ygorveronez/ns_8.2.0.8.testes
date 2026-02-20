using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Atendimentos
{
    [CustomAuthorize("Atendimentos/TipoAtendimento")]
    public class TipoAtendimentoController : BaseController
    {
		#region Construtores

		public TipoAtendimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repTipoAtendimento = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);

                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
                Enum.TryParse(Request.Params("Status"), out status);

                int empresa = 0;
                int empresaPai = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    empresa = this.Usuario.Empresa.Codigo;
                    empresaPai = this.Usuario.Empresa.EmpresaPai != null ? this.Usuario.Empresa.EmpresaPai.Codigo : 0;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo> listaTipoAtendimento = repTipoAtendimento.Consultar(empresaPai, TipoServicoMultisoftware, descricao, status, empresa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoAtendimento.ContarConsulta(empresaPai, TipoServicoMultisoftware, descricao, status, empresa));
                var lista = (from p in listaTipoAtendimento
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoStatus
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

                Repositorio.Embarcador.Atendimento.AtendimentoTipo repTipoAtendimento = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo tipoAtendimento = new Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo();

                bool status, envioEmail;
                bool.TryParse(Request.Params("Status"), out status);
                bool.TryParse(Request.Params("EnvioEmail"), out envioEmail);

                tipoAtendimento.Descricao = Request.Params("Descricao");
                tipoAtendimento.Status = status;
                tipoAtendimento.EnviarEmailAutomatico = envioEmail;
                tipoAtendimento.Empresa = this.Usuario.Empresa;

                repTipoAtendimento.Inserir(tipoAtendimento);
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
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repTipoAtendimento = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo tipoAtendimento = repTipoAtendimento.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));

                bool status, envioEmail;
                bool.TryParse(Request.Params("Status"), out status);
                bool.TryParse(Request.Params("EnvioEmail"), out envioEmail);

                tipoAtendimento.Descricao = Request.Params("Descricao");
                tipoAtendimento.Status = status;
                tipoAtendimento.EnviarEmailAutomatico = envioEmail;
                tipoAtendimento.Empresa = this.Usuario.Empresa;

                repTipoAtendimento.Atualizar(tipoAtendimento);
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
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repTipoAtendimento = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo tipoAtendimento = repTipoAtendimento.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    tipoAtendimento.Codigo,
                    tipoAtendimento.Descricao,
                    tipoAtendimento.Status,
                    EnvioEmail = tipoAtendimento.EnviarEmailAutomatico
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
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repTipoAtendimento = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo tipoAtendimento = repTipoAtendimento.BuscarPorCodigo(codigo);
                repTipoAtendimento.Deletar(tipoAtendimento);
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
