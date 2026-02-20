using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/DireitoFiscal")]
    public class DireitoFiscalController : BaseController
    {
		#region Construtores

		public DireitoFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("IVA", "IVA", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição","Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("ICMS/ISS", "ICMS_ISS", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("IPI", "IPI", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("PIS", "PIS", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("COFINS", "COFINS", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaDireitoFiscal filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Contabeis.DireitoFiscal repositorioDireitoFiscal = new Repositorio.Embarcador.Contabeis.DireitoFiscal(unitOfWork);
                int totalRegistro = repositorioDireitoFiscal.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal> direitosFiscais = (totalRegistro > 0) ? repositorioDireitoFiscal.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal>();

                var direitoFiscalRetornar = (
                    from direitoFiscal in direitosFiscais
                    select new
                    {
                        direitoFiscal.Codigo,
                        direitoFiscal.Descricao,
                        IVA = direitoFiscal?.ImpostoValorAgregado?.Descricao ?? string.Empty,
                        direitoFiscal.ICMS_ISS,
                        direitoFiscal.IPI,
                        direitoFiscal.PIS,
                        direitoFiscal.COFINS,
                        Situacao = direitoFiscal.Ativo ? SituacaoAtivoPesquisa.Ativo.ObterDescricao() : SituacaoAtivoPesquisa.Inativo.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(direitoFiscalRetornar);
                grid.setarQuantidadeTotal(totalRegistro);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Contabeis.DireitoFiscal repositorioDireitoFiscal = new Repositorio.Embarcador.Contabeis.DireitoFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal DireitoFiscal = repositorioDireitoFiscal.BuscarPorCodigo(codigo, false);

                if (DireitoFiscal == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    DireitoFiscal.Codigo,
                    IVA = DireitoFiscal.ImpostoValorAgregado != null ? new { DireitoFiscal.ImpostoValorAgregado.Codigo, DireitoFiscal.ImpostoValorAgregado.Descricao } : null,
                    DireitoFiscal.Descricao,
                    DireitoFiscal.ICMS_ISS,
                    DireitoFiscal.IPI,
                    DireitoFiscal.PIS,
                    DireitoFiscal.COFINS,
                    DireitoFiscal.Ativo,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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

                Repositorio.Embarcador.Contabeis.DireitoFiscal repositorioDireitoFiscal = new Repositorio.Embarcador.Contabeis.DireitoFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal DireitoFiscal = new Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal();

                PreencherDireitoFiscal(DireitoFiscal, unitOfWork);
                ValidarDireitoFiscalDuplicado(DireitoFiscal, repositorioDireitoFiscal, unitOfWork);

                repositorioDireitoFiscal.Inserir(DireitoFiscal, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                Repositorio.Embarcador.Contabeis.DireitoFiscal repositorioDireitoFiscal = new Repositorio.Embarcador.Contabeis.DireitoFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal DireitoFiscal = repositorioDireitoFiscal.BuscarPorCodigo(codigo, true);

                if (DireitoFiscal == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherDireitoFiscal(DireitoFiscal, unitOfWork);
                ValidarDireitoFiscalDuplicado(DireitoFiscal, repositorioDireitoFiscal, unitOfWork);

                repositorioDireitoFiscal.Atualizar(DireitoFiscal, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                Repositorio.Embarcador.Contabeis.DireitoFiscal repositorioDireitoFiscal = new Repositorio.Embarcador.Contabeis.DireitoFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal DireitoFiscal = repositorioDireitoFiscal.BuscarPorCodigo(codigo, true);

                if (DireitoFiscal == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioDireitoFiscal.Deletar(DireitoFiscal, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaDireitoFiscal ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaDireitoFiscal()
            {
                CodigoIVA = Request.GetIntParam("IVA"),
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricatoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void PreencherDireitoFiscal(Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal DireitoFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repIVA = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork);
            int codigoIVA = Request.GetIntParam("IVA");

            DireitoFiscal.ImpostoValorAgregado = codigoIVA > 0 ? repIVA.BuscarPorCodigo(codigoIVA) : null;
            DireitoFiscal.Descricao = Request.GetStringParam("Descricao");
            DireitoFiscal.ICMS_ISS = Request.GetStringParam("ICMS_ISS");
            DireitoFiscal.IPI = Request.GetStringParam("IPI");
            DireitoFiscal.PIS = Request.GetStringParam("PIS");
            DireitoFiscal.COFINS = Request.GetStringParam("COFINS");
            DireitoFiscal.Ativo = Request.GetBoolParam("Situacao");
        }

        private void ValidarDireitoFiscalDuplicado(Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal DireitoFiscal, Repositorio.Embarcador.Contabeis.DireitoFiscal repositorioDireitoFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            if (repositorioDireitoFiscal.ExisteDuplicado(DireitoFiscal))
                throw new ControllerException("Já existe um cadastro de Direito Fiscal com os mesmos dados");
        }

        #endregion
    }
}
