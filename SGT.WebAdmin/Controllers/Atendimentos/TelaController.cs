using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Atendimentos
{
    [CustomAuthorize("Atendimentos/Tela")]
    public class TelaController : BaseController
    {
		#region Construtores

		public TelaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.AtendimentoTela repTela = new Repositorio.Embarcador.Atendimento.AtendimentoTela(unitOfWork);

                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
                Enum.TryParse(Request.Params("Status"), out status);

                int codigoSistema = int.Parse(Request.Params("Sistema"));
                int codigoModulo = int.Parse(Request.Params("Modulo"));

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
                grid.AdicionarCabecalho("CodigoModulo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Sistema", "Sistema", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Módulo", "Modulo", 15, Models.Grid.Align.left, false);
                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTela> listaTela = repTela.Consultar(TipoServicoMultisoftware, descricao, status, codigoSistema, codigoModulo, empresa, empresaPai, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTela.ContarConsulta(TipoServicoMultisoftware, descricao, status, codigoSistema, codigoModulo, empresa, empresaPai));
                var lista = (from p in listaTela
                             select new
                             {
                                 p.Codigo,
                                 CodigoSistema = p.AtendimentoSistema != null ? p.AtendimentoSistema.Codigo : 0,
                                 CodigoModulo = p.AtendimentoModulo != null ? p.AtendimentoModulo.Codigo : 0,
                                 p.Descricao,
                                 Sistema = p.AtendimentoSistema != null ? p.AtendimentoSistema.Descricao : string.Empty,
                                 Modulo = p.AtendimentoModulo != null ? p.AtendimentoModulo.Descricao : string.Empty,
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

                Repositorio.Embarcador.Atendimento.AtendimentoTela repTela = new Repositorio.Embarcador.Atendimento.AtendimentoTela(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTela tela = new Dominio.Entidades.Embarcador.Atendimento.AtendimentoTela();
                Repositorio.Embarcador.Atendimento.AtendimentoSistema repSistema = new Repositorio.Embarcador.Atendimento.AtendimentoSistema(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoModulo repModulo = new Repositorio.Embarcador.Atendimento.AtendimentoModulo(unitOfWork);

                int codigoSistema = int.Parse(Request.Params("Sistema"));
                int codigoModulo = int.Parse(Request.Params("Modulo"));

                bool status;
                bool.TryParse(Request.Params("Status"), out status);

                tela.Descricao = Request.Params("Descricao");
                tela.Status = status;
                tela.AtendimentoSistema = repSistema.BuscarPorCodigo(codigoSistema);
                tela.AtendimentoModulo = repModulo.BuscarPorCodigo(codigoModulo);
                tela.Empresa = this.Usuario.Empresa;

                repTela.Inserir(tela);
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
                Repositorio.Embarcador.Atendimento.AtendimentoTela repTela = new Repositorio.Embarcador.Atendimento.AtendimentoTela(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTela tela = repTela.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));
                Repositorio.Embarcador.Atendimento.AtendimentoSistema repSistema = new Repositorio.Embarcador.Atendimento.AtendimentoSistema(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoModulo repModulo = new Repositorio.Embarcador.Atendimento.AtendimentoModulo(unitOfWork);

                int codigoSistema = int.Parse(Request.Params("Sistema"));
                int codigoModulo = int.Parse(Request.Params("Modulo"));

                bool status;
                bool.TryParse(Request.Params("Status"), out status);

                tela.Descricao = Request.Params("Descricao");
                tela.Status = status;
                tela.AtendimentoSistema = repSistema.BuscarPorCodigo(codigoSistema);
                tela.AtendimentoModulo = repModulo.BuscarPorCodigo(codigoModulo);
                tela.Empresa = this.Usuario.Empresa;

                repTela.Atualizar(tela);
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
                Repositorio.Embarcador.Atendimento.AtendimentoTela repTela = new Repositorio.Embarcador.Atendimento.AtendimentoTela(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTela tela = repTela.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    tela.Codigo,
                    tela.Descricao,
                    tela.Status,
                    Sistema = tela.AtendimentoSistema != null ? new { Codigo = tela.AtendimentoSistema.Codigo, Descricao = tela.AtendimentoSistema.Descricao } : null,
                    Modulo = tela.AtendimentoModulo != null ? new { Codigo = tela.AtendimentoModulo.Codigo, Descricao = tela.AtendimentoModulo.Descricao } : null
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
                Repositorio.Embarcador.Atendimento.AtendimentoTela repTela = new Repositorio.Embarcador.Atendimento.AtendimentoTela(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTela tela = repTela.BuscarPorCodigo(codigo);
                repTela.Deletar(tela);
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
