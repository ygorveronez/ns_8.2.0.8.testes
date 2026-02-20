using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas
{
    [CustomAuthorize("Configuracoes/ConfiguracaoTaxaDescarga")]
    public class ConfiguracaoTaxaDescargaController : BaseController
    {
		#region Construtores

		public ConfiguracaoTaxaDescargaController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 40, Models.Grid.Align.left, true);


                Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoTaxaDescarga filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga repConfiguracaoTaxaDescarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga(unitOfWork);
                int totalRegistro = repConfiguracaoTaxaDescarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga> configuracaoTaxaDescargas = (totalRegistro > 0) ? repConfiguracaoTaxaDescarga.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga>();

                var retorno = (
                    from configuracao in configuracaoTaxaDescargas
                    select new
                    {
                        configuracao.Codigo,
                        configuracao.Descricao,
                        Situacao = configuracao.Ativa ? SituacaoAtivaPesquisa.Ativa.ObterDescricao() : SituacaoAtivaPesquisa.Inativa.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(retorno);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga repConfiguracaoTaxaDescarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga configuracaoTaxaDescarga = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga();

                PreencherEntidade(configuracaoTaxaDescarga, unitOfWork);

                repConfiguracaoTaxaDescarga.Inserir(configuracaoTaxaDescarga);

                SalvarConfiguracaoTaxaDescargaAjudantes(configuracaoTaxaDescarga, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes repConfiguracaoTaxaDescargaAjudantes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga repConfiguracaoTaxaDescarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga configuracaoTaxaDescarga = repConfiguracaoTaxaDescarga.BuscarPorCodigo(codigo);

                if (configuracaoTaxaDescarga == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                SalvarConfiguracaoTaxaDescargaAjudantes(configuracaoTaxaDescarga, unitOfWork);

                PreencherEntidade(configuracaoTaxaDescarga, unitOfWork);

                repConfiguracaoTaxaDescarga.Atualizar(configuracaoTaxaDescarga);

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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes repConfiguracaoTaxaDescargaAjudantes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga repConfiguracaoTaxaDescarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga configuracaoTaxaDescarga = repConfiguracaoTaxaDescarga.BuscarPorCodigo(codigo);

                if (configuracaoTaxaDescarga == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes> configuracaoTaxaDescargaAjudantes = repConfiguracaoTaxaDescargaAjudantes.BuscarPorConfiguracaoTaxaDescarga(configuracaoTaxaDescarga.Codigo);

                var dynConfiguracaoTaxaDescarga = new
                {
                    configuracaoTaxaDescarga.Codigo,
                    configuracaoTaxaDescarga.Descricao,
                    configuracaoTaxaDescarga.Ativa,
                    ConfiguracaoTaxaDescargaAjudantes = (from o in configuracaoTaxaDescargaAjudantes
                                                     select new
                                                     {
                                                         o.Codigo,
                                                         TipoDescricao = o.Tipo.ObterDescricao() ?? "",
                                                         o.Tipo,
                                                         o.QuantidadeInicial,
                                                         o.QuantidadeFinal,
                                                         o.QuantidadeAjudantes,
                                                     }).ToList(),

                };
                return new JsonpResult(dynConfiguracaoTaxaDescarga);
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

                Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes repConfiguracaoTaxaDescargaAjudantes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga repConfiguracaoTaxaDescarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga configuracaoTaxaDescarga = repConfiguracaoTaxaDescarga.BuscarPorCodigo(codigo);

                if (configuracaoTaxaDescarga == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes> configuracaoTaxaDescargaAjudantes = repConfiguracaoTaxaDescargaAjudantes.BuscarPorConfiguracaoTaxaDescarga(configuracaoTaxaDescarga.Codigo);

                foreach (var configuracao in configuracaoTaxaDescargaAjudantes)
                {
                    repConfiguracaoTaxaDescargaAjudantes.Deletar(configuracao);
                }

                repConfiguracaoTaxaDescarga.Deletar(configuracaoTaxaDescarga);

                unitOfWork.CommitChanges();

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

        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoTaxaDescarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoTaxaDescarga()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<SituacaoAtivaPesquisa>("Situacao"),

            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga configuracaoTaxaDescarga, Repositorio.UnitOfWork unitOfWork)
        {
            configuracaoTaxaDescarga.Descricao = Request.GetStringParam("Descricao");
            configuracaoTaxaDescarga.Ativa = Request.GetBoolParam("Situacao");
        }

        private void SalvarConfiguracaoTaxaDescargaAjudantes(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga configuracaoTaxaDescarga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes repConfiguracaoTaxaDescargaAjudantes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes> configuracoesTaxaDescargaAjudantes = repConfiguracaoTaxaDescargaAjudantes.BuscarPorConfiguracaoTaxaDescarga(configuracaoTaxaDescarga.Codigo);


            foreach (var configuracao in configuracoesTaxaDescargaAjudantes)
            {
                repConfiguracaoTaxaDescargaAjudantes.Deletar(configuracao);
            }

            dynamic dadosAjudantes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("dadosAjudantes"));

            foreach (var dadoAjudante in dadosAjudantes)
            {

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes configuracaoTaxaDescargaAjudantes = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes
                {
                    ConfiguracaoTaxaDescarga = configuracaoTaxaDescarga,
                    QuantidadeAjudantes = (int)dadoAjudante.QuantidadeAjudantes,
                    QuantidadeFinal = (int)dadoAjudante.QuantidadeFinal,
                    QuantidadeInicial = (int)dadoAjudante.QuantidadeInicial,
                    Tipo = dadoAjudante.Tipo
                };

                repConfiguracaoTaxaDescargaAjudantes.Inserir(configuracaoTaxaDescargaAjudantes);
            }

        }

        private string ObterPropriedadeOrdenar(string prop)
        {
            return prop;
        }
        #endregion
    }
}
