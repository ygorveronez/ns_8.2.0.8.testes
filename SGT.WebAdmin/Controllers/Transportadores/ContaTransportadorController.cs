using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/ContaTransportador")]
    public class ContaTransportadorController : BaseController
    {
		#region Construtores

		public ContaTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaContaTransportador filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Banco, "Banco", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.NumeroAgencia, "NumeroAgencia", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.NumeroConta, "NumeroConta", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CodigoBanco", false);
                grid.AdicionarCabecalho("DigitoAgencia", false);

                Repositorio.Embarcador.Transportadores.ContaTransportador repContaTransportador = new Repositorio.Embarcador.Transportadores.ContaTransportador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Transportadores.ContaTransportador> contaTransportadors = repContaTransportador.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repContaTransportador.ContarConsulta(filtrosPesquisa));

                var lista = (from p in contaTransportadors
                             select new
                             {
                                 p.Codigo,
                                 Banco = p.Banco?.Descricao,
                                 p.NumeroAgencia,
                                 p.NumeroConta,
                                 CodigoBanco = p.Banco?.Codigo,
                                 p.DigitoAgencia
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultar);
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

                Repositorio.Embarcador.Transportadores.ContaTransportador repContaTransportador = new Repositorio.Embarcador.Transportadores.ContaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.ContaTransportador contaTransportador = new Dominio.Entidades.Embarcador.Transportadores.ContaTransportador();

                PreencherContaTransportador(contaTransportador, unitOfWork);

                repContaTransportador.Inserir(contaTransportador, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAdicionar);
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Transportadores.ContaTransportador repContaTransportador = new Repositorio.Embarcador.Transportadores.ContaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.ContaTransportador contaTransportador = repContaTransportador.BuscarPorCodigo(codigo, true);

                PreencherContaTransportador(contaTransportador, unitOfWork);

                repContaTransportador.Atualizar(contaTransportador, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAtualizar);
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Transportadores.ContaTransportador repContaTransportador = new Repositorio.Embarcador.Transportadores.ContaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.ContaTransportador contaTransportador = repContaTransportador.BuscarPorCodigo(codigo, false);

                var dynContaTransportador = new
                {
                    contaTransportador.Codigo,
                    contaTransportador.NumeroAgencia,
                    contaTransportador.DigitoAgencia,
                    contaTransportador.NumeroConta,
                    contaTransportador.TipoContaBanco,
                    Banco = contaTransportador.Banco != null ? new { contaTransportador.Banco.Codigo, contaTransportador.Banco.Descricao } : null
                };

                return new JsonpResult(dynContaTransportador);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Transportadores.ContaTransportador repContaTransportador = new Repositorio.Embarcador.Transportadores.ContaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.ContaTransportador contaTransportador = repContaTransportador.BuscarPorCodigo(codigo, true);

                if (contaTransportador == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repContaTransportador.Deletar(contaTransportador, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, false, Localization.Resources.Transportadores.Transportador.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoEmOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherContaTransportador(Dominio.Entidades.Embarcador.Transportadores.ContaTransportador contaTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

            int codigoBanco = Request.GetIntParam("Banco");

            contaTransportador.NumeroAgencia = Request.GetStringParam("NumeroAgencia");
            contaTransportador.DigitoAgencia = Request.GetStringParam("DigitoAgencia");
            contaTransportador.NumeroConta = Request.GetStringParam("NumeroConta");
            contaTransportador.TipoContaBanco = Request.GetEnumParam<TipoContaBanco>("TipoContaBanco");

            contaTransportador.Banco = codigoBanco > 0 ? repBanco.BuscarPorCodigo(codigoBanco) : null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaContaTransportador ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaContaTransportador()
            {
                CodigoBanco = Request.GetIntParam("Banco"),
                NumeroAgencia = Request.GetStringParam("NumeroAgencia"),
                NumeroConta = Request.GetStringParam("NumeroConta")
            };
        }

        #endregion
    }
}
