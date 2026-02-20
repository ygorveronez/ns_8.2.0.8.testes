using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize("Cargas/Carga")]
    public class DadosEmissaoIscaController : BaseController
    {
		#region Construtores

		public DadosEmissaoIscaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarIsca()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);

                Repositorio.Embarcador.Cargas.CargaIsca repositorioCargaIsca = new Repositorio.Embarcador.Cargas.CargaIsca(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Isca repositorioIsca = new Repositorio.Embarcador.Cargas.Isca(unidadeTrabalho);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoIsca = Request.GetIntParam("Isca");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Isca isca = repositorioIsca.BuscarPorCodigo(codigoIsca, false);

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaIsca cargaIsca = new Dominio.Entidades.Embarcador.Cargas.CargaIsca
                {
                    Carga = carga,
                    Isca = isca,
                };

                repositorioCargaIsca.Inserir(cargaIsca, Auditado);

                return new JsonpResult(new
                {
                    cargaIsca.Codigo,
                    cargaIsca.Isca.CodigoIntegracao
                });
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirIscaCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);

                Repositorio.Embarcador.Cargas.CargaIsca repositorioCargaIsca = new Repositorio.Embarcador.Cargas.CargaIsca(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaIsca cargaIsca = repositorioCargaIsca.BuscarPorCodigo(codigo, false);

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(cargaIsca.Carga, unidadeTrabalho);
                
                repositorioCargaIsca.Deletar(cargaIsca, Auditado);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
