using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/TecnologiaRastreador")]
    public class TecnologiaRastreadorController : BaseController
    {
		#region Construtores

		public TecnologiaRastreadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TecnologiaRastreador.NomeDaConta, "NomeConta", 30, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Veiculos.TecnologiaRastreador repTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador> tecnologias = repTecnologiaRastreador.Consultar(descricao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTecnologiaRastreador.ContarConsulta(descricao, ativo));

                var lista = (from p in tecnologias
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.NomeConta,
                                 p.DescricaoAtivo
                             }).ToList();

                grid.AdicionaRows(lista);

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
                string descricao = Request.Params("Descricao");
                string nomeConta = Request.Params("NomeConta");
                string codigoIntegracao = Request.Params("CodigoIntegracao");
                bool ativo = Request.GetBoolParam("Ativo");

                Repositorio.Embarcador.Veiculos.TecnologiaRastreador repTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador = new Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador()
                {
                    Ativo = ativo,
                    CodigoIntegracao = codigoIntegracao,
                    Descricao = descricao,
                    NomeConta = nomeConta
                };

                repTecnologiaRastreador.Inserir(tecnologiaRastreador, Auditado);

                SalvarCodigosIntegracoes(tecnologiaRastreador, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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
                int codigo = Request.GetIntParam("Codigo");
                string descricao = Request.Params("Descricao");
                string nomeConta = Request.Params("NomeConta");
                string codigoIntegracao = Request.Params("CodigoIntegracao");
                bool ativo = Request.GetBoolParam("Ativo");

                Repositorio.Embarcador.Veiculos.TecnologiaRastreador repTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador = repTecnologiaRastreador.BuscarPorCodigo(codigo, true);

                tecnologiaRastreador.Ativo = ativo;
                tecnologiaRastreador.CodigoIntegracao = codigoIntegracao;
                tecnologiaRastreador.Descricao = descricao;
                tecnologiaRastreador.NomeConta = nomeConta;

                repTecnologiaRastreador.Atualizar(tecnologiaRastreador, Auditado);

                SalvarCodigosIntegracoes(tecnologiaRastreador, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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

                Repositorio.Embarcador.Veiculos.TecnologiaRastreador repTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);
                Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao repTecnologiaRastreadorCodigoIntegracao = new Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador = repTecnologiaRastreador.BuscarPorCodigo(codigo, false);
                List<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao> tecnologiaRastreadorTiposIntegracoes = repTecnologiaRastreadorCodigoIntegracao.BuscarPorTecnologiaRastreador(tecnologiaRastreador);

                var retorno = new
                {
                    tecnologiaRastreador.Codigo,
                    tecnologiaRastreador.CodigoIntegracao,
                    tecnologiaRastreador.Descricao,
                    tecnologiaRastreador.NomeConta,
                    tecnologiaRastreador.Ativo,
                    CodigoIntegracaoA52 = tecnologiaRastreadorTiposIntegracoes.Where(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52).Select(o => o.CodigoIntegracao).FirstOrDefault()
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
                Repositorio.Embarcador.Veiculos.TecnologiaRastreador repTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador = repTecnologiaRastreador.BuscarPorCodigo(codigo, true);

                repTecnologiaRastreador.Deletar(tecnologiaRastreador, Auditado);

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

        #region Métodos Privados

        #region Integrações

        private void SalvarCodigosIntegracoes(Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador, Repositorio.UnitOfWork unitOfWork)
        {
            SalvarCodigoIntegracaoA52(tecnologiaRastreador, unitOfWork);
        }

        private void SalvarCodigoIntegracaoA52(Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao repTecnologiaRastreadorCodigoIntegracao = new Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52);

            if (tipoIntegracao == null)
                return;

            Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao tecnologiaRastreadorCodigoIntegracao = repTecnologiaRastreadorCodigoIntegracao.BuscarPorTecnologiaRastreadorETipoIntegracao(tecnologiaRastreador, tipoIntegracao);

            if (tecnologiaRastreadorCodigoIntegracao == null)
            {
                tecnologiaRastreadorCodigoIntegracao = new Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao()
                {
                    TecnologiaRastreador = tecnologiaRastreador,
                    TipoIntegracao = tipoIntegracao
                };
            }

            tecnologiaRastreadorCodigoIntegracao.CodigoIntegracao = Request.GetStringParam("CodigoIntegracaoA52");

            if (tecnologiaRastreadorCodigoIntegracao.Codigo > 0)
                repTecnologiaRastreadorCodigoIntegracao.Atualizar(tecnologiaRastreadorCodigoIntegracao);
            else
                repTecnologiaRastreadorCodigoIntegracao.Inserir(tecnologiaRastreadorCodigoIntegracao);
        }

        #endregion

        #endregion
    }
}
