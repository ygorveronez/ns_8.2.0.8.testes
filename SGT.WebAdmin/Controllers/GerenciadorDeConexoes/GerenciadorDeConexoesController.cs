using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GerenciadorDeConexoes
{
    public class GerenciadorDeConexoesController : BaseController
    {
		#region Construtores

		public GerenciadorDeConexoesController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAnonymous]
        public async Task<IActionResult> ValidarTokenAcessoOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string tokenAcesso = Request.GetStringParam("TOKEN_ACESSO_AUTORIZACAO_OCORRENCIA");

                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao cargaOcorrenciaAutorizacao = repositorioCargaOcorrenciaAutorizacao.BuscarPorGuid(tokenAcesso);

                ViewBag.CODIGO_OCORRENCIA_VIA_TOKEN_ACESSO_AUTORIZACAO_OCORRENCIA = cargaOcorrenciaAutorizacao.CargaOcorrencia.Codigo;

                return View("GerenciadorDeConexoes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ValidarTokenAcessoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string tokenAcesso = Request.GetStringParam("TOKEN_ACESSO_AUTORIZACAO_CARGA");

                Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorioAprovacaoAlcadaCarga = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga aprovacaoAlcadaCarga = repositorioAprovacaoAlcadaCarga.BuscarPorGuid(tokenAcesso);

                ViewBag.CODIGO_CARGA_VIA_TOKEN_ACESSO_AUTORIZACAO_CARGA = aprovacaoAlcadaCarga.OrigemAprovacao.Codigo;

                return View("GerenciadorDeConexoes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ValidarTokenAcessoTabelaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string tokenAcesso = Request.GetStringParam("TOKEN_ACESSO_AUTORIZACAO_TABELA_FRETE");

                Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repositorioAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao ajusteTabelaFreteAutorizacao = repositorioAjusteTabelaFreteAutorizacao.BuscarPorGuid(tokenAcesso);

                ViewBag.CODIGO_TABELA_FRETE_VIA_TOKEN_ACESSO_AUTORIZACAO_TABELA_FRETE = ajusteTabelaFreteAutorizacao.AjusteTabelaFrete.Codigo;

                return View("GerenciadorDeConexoes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ValidarTokenAcessoCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string tokenAcesso = Request.GetStringParam("TOKEN_ACESSO_AUTORIZACAO_CARREGAMENO");

                Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento repositorioAprovacaoAlcadaCarregamento = new Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento aprovacaoAlcadaCarregamento = repositorioAprovacaoAlcadaCarregamento.BuscarPorGuid(tokenAcesso);

                ViewBag.CODIGO_CARREGAMENTO_VIA_TOKEN_ACESSO_AUTORIZACAO_CARREGAMENTO = aprovacaoAlcadaCarregamento.OrigemAprovacao.Codigo;

                return View("GerenciadorDeConexoes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
