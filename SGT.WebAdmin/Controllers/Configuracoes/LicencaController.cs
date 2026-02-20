using Dominio.ObjetosDeValor.Enumerador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/Licenca")]
    public class LicencaController : BaseController
    {
		#region Construtores

		public LicencaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                int codigoEmpresa = 0;
                int tipoLicenca = Request.GetIntParam("Tipo", -1);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo de Licença", "DescricaoTipo", 20, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);

                List<Dominio.Entidades.Embarcador.Configuracoes.Licenca> listaLicenca = repLicenca.Consultar(codigoEmpresa, descricao, ativo, tipoLicenca, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repLicenca.ContarConsulta(codigoEmpresa, descricao, ativo, tipoLicenca));

                grid.AdicionaRows((from p in listaLicenca
                                   select new
                                   {
                                       p.Codigo,
                                       p.Descricao,
                                       p.DescricaoAtivo,
                                       p.DescricaoTipo,
                                       p.GerarRequisicao
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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

                Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Licenca licenca = new Dominio.Entidades.Embarcador.Configuracoes.Licenca();

                PreencherLicenca(licenca, unitOfWork);

                repLicenca.Inserir(licenca, Auditado);

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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Licenca licenca = repLicenca.BuscarPorCodigo(codigo, true);

                PreencherLicenca(licenca, unitOfWork);

                repLicenca.Atualizar(licenca, Auditado);

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

                Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Licenca licenca = repLicenca.BuscarPorCodigo(codigo, false);

                var retorno = new
                {
                    licenca.Codigo,
                    licenca.Ativo,
                    licenca.BloquearCheckListComLicencaInvalida,
                    licenca.Descricao,
                    licenca.Email,
                    licenca.Tipo,
                    licenca.GerarRequisicao
                };

                return new JsonpResult(retorno);
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

                Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Licenca licenca = repLicenca.BuscarPorCodigo(codigo, true);

                if (licenca == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repLicenca.Deletar(licenca, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    unitOfWork.Rollback();
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

        #region Métodos Privados

        private void PreencherLicenca(Dominio.Entidades.Embarcador.Configuracoes.Licenca licenca, Repositorio.UnitOfWork unitOfWork)
        {
            licenca.Descricao = Request.GetStringParam("Descricao");
            licenca.Ativo = Request.GetBoolParam("Ativo");
            licenca.Tipo = (TipoLicenca) Request.GetNullableEnumParam<TipoLicenca>("Tipo");
            licenca.Email = Request.GetStringParam("Email");
            licenca.BloquearCheckListComLicencaInvalida = Request.GetBoolParam("BloquearCheckListComLicencaInvalida");
            licenca.GerarRequisicao = Request.GetBoolParam("GerarRequisicao");

            if (licenca.Codigo == 0)
                licenca.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;
        }

        #endregion
    }
}
