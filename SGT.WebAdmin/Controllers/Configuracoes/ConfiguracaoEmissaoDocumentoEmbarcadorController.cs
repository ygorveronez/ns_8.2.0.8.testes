using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoEmissaoDocumentoEmbarcador")]
    public class ConfiguracaoEmissaoDocumentoEmbarcadorController : BaseController
    {
		#region Construtores

		public ConfiguracaoEmissaoDocumentoEmbarcadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltrosPesquisaConfiguracaoEmissaoDocumentoEmbarcador filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Cliente", "Cliente", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 40, Models.Grid.Align.left, true);


                Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador repConfiguracaoEmissaoDocumentoEmbarcador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador> configuracoesEmissaoDocumentosEmbarcador = repConfiguracaoEmissaoDocumentoEmbarcador.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repConfiguracaoEmissaoDocumentoEmbarcador.ContarConsulta(filtrosPesquisa));

                var lista = (from p in configuracoesEmissaoDocumentosEmbarcador
                             select new
                             {
                                 p.Codigo,
                                 Cliente = p.Cliente.NomeCNPJ,
                                 TipoOperacao = p.TipoOperacao.Descricao
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

                Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador repConfiguracaoEmissaoDocumentoEmbarcador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador configuracaoEmissaoDocumentoEmbarcador = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador();

                PreencherConfiguracaoEmissaoDocumentoEmbarcador(configuracaoEmissaoDocumentoEmbarcador, unitOfWork);

                string erro = string.Empty;
                if (!ValidarRegistroDuplicado(configuracaoEmissaoDocumentoEmbarcador.Cliente.CPF_CNPJ, configuracaoEmissaoDocumentoEmbarcador.TipoOperacao.Codigo, configuracaoEmissaoDocumentoEmbarcador.Codigo, unitOfWork, out erro))
                    return new JsonpResult(false, true, erro);

                repConfiguracaoEmissaoDocumentoEmbarcador.Inserir(configuracaoEmissaoDocumentoEmbarcador, Auditado);

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

                Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador repConfiguracaoEmissaoDocumentoEmbarcador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador configuracaoEmissaoDocumentoEmbarcador = repConfiguracaoEmissaoDocumentoEmbarcador.BuscarPorCodigo(codigo, true);

                if (configuracaoEmissaoDocumentoEmbarcador == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                
                PreencherConfiguracaoEmissaoDocumentoEmbarcador(configuracaoEmissaoDocumentoEmbarcador, unitOfWork);

                string erro = string.Empty;
                if (!ValidarRegistroDuplicado(configuracaoEmissaoDocumentoEmbarcador.Cliente.CPF_CNPJ, configuracaoEmissaoDocumentoEmbarcador.TipoOperacao.Codigo, configuracaoEmissaoDocumentoEmbarcador.Codigo, unitOfWork, out erro))
                    return new JsonpResult(false, true, erro);

                repConfiguracaoEmissaoDocumentoEmbarcador.Atualizar(configuracaoEmissaoDocumentoEmbarcador, Auditado);

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

                Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador repConfiguracaoEmissaoDocumentoEmbarcador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador configuracaoEmissaoDocumentoEmbarcador = repConfiguracaoEmissaoDocumentoEmbarcador.BuscarPorCodigo(codigo, false);

                if (configuracaoEmissaoDocumentoEmbarcador == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynConfiguracaoEmissaoDocumentoEmbarcador = new
                {
                    configuracaoEmissaoDocumentoEmbarcador.Codigo,
                    Cliente = new { Codigo = configuracaoEmissaoDocumentoEmbarcador.Cliente.Codigo, Descricao = configuracaoEmissaoDocumentoEmbarcador.Cliente.NomeCNPJ },
                    TipoOperacao = new { Codigo = configuracaoEmissaoDocumentoEmbarcador.TipoOperacao.Codigo, Descricao = configuracaoEmissaoDocumentoEmbarcador.TipoOperacao.Descricao }
                };

                return new JsonpResult(dynConfiguracaoEmissaoDocumentoEmbarcador);
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

                Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador repConfiguracaoEmissaoDocumentoEmbarcador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador configuracaoEmissaoDocumentoEmbarcador = repConfiguracaoEmissaoDocumentoEmbarcador.BuscarPorCodigo(codigo, true);

                if (configuracaoEmissaoDocumentoEmbarcador == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repConfiguracaoEmissaoDocumentoEmbarcador.Deletar(configuracaoEmissaoDocumentoEmbarcador, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

        private void PreencherConfiguracaoEmissaoDocumentoEmbarcador(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador configuracaoEmissaoDocumentoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacao"));
            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Cliente"));
            
            configuracaoEmissaoDocumentoEmbarcador.Cliente = cliente;
            configuracaoEmissaoDocumentoEmbarcador.TipoOperacao = tipoOperacao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltrosPesquisaConfiguracaoEmissaoDocumentoEmbarcador ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltrosPesquisaConfiguracaoEmissaoDocumentoEmbarcador()
            {
                CPFCNPJCliente = Request.GetDoubleParam("Cliente"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao")
            };
        }

        private bool ValidarRegistroDuplicado(double cfpcnpjCliente, int codigoTipoOperacao, int codigo, Repositorio.UnitOfWork unitOfWork, out string strRetorno)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador repConfiguracaoEmissaoDocumentoEmbarcador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador(unitOfWork);

            strRetorno = string.Empty;

            if (repConfiguracaoEmissaoDocumentoEmbarcador.RegistroDuplicado(cfpcnpjCliente, codigoTipoOperacao, codigo))
            {
                strRetorno = "Já existe registro com este Cliente e Tipo de Operação.";
                return false;
            }

            return true;
        }

        #endregion
    }
}
