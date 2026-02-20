using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Atendimentos
{
    [CustomAuthorize("Atendimentos/Modulo")]
    public class ModuloController : BaseController
    {
		#region Construtores

		public ModuloController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.AtendimentoModulo repModulo = new Repositorio.Embarcador.Atendimento.AtendimentoModulo(unitOfWork);

                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
                Enum.TryParse(Request.Params("Status"), out status);

                int.TryParse(Request.Params("Sistema"), out int codigoSistema);

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
                grid.AdicionarCabecalho("CodigoSistema", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Sistema", "Sistema", 25, Models.Grid.Align.left, false);
                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.Atendimento.AtendimentoModulo> listaModulo = repModulo.Consultar(TipoServicoMultisoftware, descricao, status, codigoSistema, empresa, empresaPai, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repModulo.ContarConsulta(TipoServicoMultisoftware, descricao, status, codigoSistema, empresa, empresaPai));

                var lista = (from p in listaModulo
                             select new
                             {
                                 p.Codigo,
                                 CodigoSistema = p.AtendimentoSistema != null ? p.AtendimentoSistema.Codigo : 0,
                                 p.Descricao,
                                 Sistema = p.AtendimentoSistema != null ? p.AtendimentoSistema.Descricao : string.Empty,
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

                Repositorio.Embarcador.Atendimento.AtendimentoModulo repModulo = new Repositorio.Embarcador.Atendimento.AtendimentoModulo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoModulo modulo = new Dominio.Entidades.Embarcador.Atendimento.AtendimentoModulo();
                Repositorio.Embarcador.Atendimento.AtendimentoSistema repSistema = new Repositorio.Embarcador.Atendimento.AtendimentoSistema(unitOfWork);

                int codigoSistema = int.Parse(Request.Params("Sistema"));

                bool status;
                bool.TryParse(Request.Params("Status"), out status);

                modulo.Descricao = Request.Params("Descricao");
                modulo.Status = status;
                modulo.AtendimentoSistema = repSistema.BuscarPorCodigo(codigoSistema);
                modulo.Empresa = this.Usuario.Empresa;

                repModulo.Inserir(modulo);
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
                Repositorio.Embarcador.Atendimento.AtendimentoModulo repModulo = new Repositorio.Embarcador.Atendimento.AtendimentoModulo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoModulo modulo = repModulo.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));
                Repositorio.Embarcador.Atendimento.AtendimentoSistema repSistema = new Repositorio.Embarcador.Atendimento.AtendimentoSistema(unitOfWork);

                int codigoSistema = int.Parse(Request.Params("Sistema"));

                bool status;
                bool.TryParse(Request.Params("Status"), out status);

                modulo.Descricao = Request.Params("Descricao");
                modulo.Status = status;
                modulo.AtendimentoSistema = repSistema.BuscarPorCodigo(codigoSistema);
                modulo.Empresa = this.Usuario.Empresa;

                repModulo.Atualizar(modulo);
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
                Repositorio.Embarcador.Atendimento.AtendimentoModulo repModulo = new Repositorio.Embarcador.Atendimento.AtendimentoModulo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoModulo modulo = repModulo.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    modulo.Codigo,
                    modulo.Descricao,
                    modulo.Status,
                    Sistema = modulo.AtendimentoSistema != null ? new { Codigo = modulo.AtendimentoSistema.Codigo, Descricao = modulo.AtendimentoSistema.Descricao } : null
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
                Repositorio.Embarcador.Atendimento.AtendimentoModulo repModulo = new Repositorio.Embarcador.Atendimento.AtendimentoModulo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoModulo modulo = repModulo.BuscarPorCodigo(codigo);
                repModulo.Deletar(modulo);
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
