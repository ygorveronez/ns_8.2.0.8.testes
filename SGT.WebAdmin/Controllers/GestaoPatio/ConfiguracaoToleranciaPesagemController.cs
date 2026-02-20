using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/ConfiguracaoToleranciaPesagem")]
    public class ConfiguracaoToleranciaPesagemController : BaseController
    {
		#region Construtores

		public ConfiguracaoToleranciaPesagemController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroToleranciaPesagem filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", "Descricao", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("TipoRegra", "TipoRegra", 10, Models.Grid.Align.center, true);
                if (filtrosPesquisa.Situacao == SituacaoToleranciaPesagem.Todos)
                    grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem repositorioConfiguracaoToleranciaPesagem = new Repositorio.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem> configuracaesToleranciaPesagem = repositorioConfiguracaoToleranciaPesagem.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioConfiguracaoToleranciaPesagem.ContarConsulta(filtrosPesquisa));

                grid.AdicionaRows((from obj in configuracaesToleranciaPesagem
                                   select new
                                   {
                                       obj.Codigo,
                                       TipoRegra = obj.TipoRegra.ObterDescricao(),
                                       obj.Descricao,
                                       Situacao = obj.Ativo.ObterDescricaoAtiva(),
                                   }).ToList());
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem repositorioConfiguracaoToleranciaPesagem = new Repositorio.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem configuracaesToleranciaPesagem = repositorioConfiguracaoToleranciaPesagem.Consultar().FirstOrDefault();

                if (configuracaesToleranciaPesagem == null)
                    throw new Exception("Não encontrado registro.");

                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem configuracaoToleranciaPesagemRetorno = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem()
                {
                    Codigo = configuracaesToleranciaPesagem.Codigo,
                    Descricao = configuracaesToleranciaPesagem.Descricao,
                    TipoRegra = configuracaesToleranciaPesagem.TipoRegra,
                    PercentualToleranciaPesoInferior = configuracaesToleranciaPesagem.ToleranciaPesoInferior,
                    PercentualToleranciaPesoSuperior = configuracaesToleranciaPesagem.PercentualToleranciaPesoSuperior,
                    ToleranciaPesoInferior = configuracaesToleranciaPesagem.ToleranciaPesoInferior,
                    ToleranciaPesoSuperior = configuracaesToleranciaPesagem.ToleranciaPesoSuperior,
                    CodigosFiliais = configuracaesToleranciaPesagem.Filials.Select(t => new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.BuscaMultiplosConfiguracaoToleranciaPesagem { Codigo = t.Codigo, Descricao = t.Descricao }).ToList(),
                    CodigosModeloVeicular = configuracaesToleranciaPesagem.ModelosVeicularesCarga.Select(t => new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.BuscaMultiplosConfiguracaoToleranciaPesagem { Codigo = t.Codigo, Descricao = t.Descricao }).ToList(),
                    CodigosTipoCarga = configuracaesToleranciaPesagem.TiposCarga.Select(t => new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.BuscaMultiplosConfiguracaoToleranciaPesagem { Codigo = t.Codigo, Descricao = t.Descricao }).ToList(),
                    CodigosTipoOperacao = configuracaesToleranciaPesagem.TiposOperacao.Select(t => new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.BuscaMultiplosConfiguracaoToleranciaPesagem { Codigo = t.Codigo, Descricao = t.Descricao }).ToList(),
                };

                return new JsonpResult(configuracaoToleranciaPesagemRetorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem repositorioConfiguracaoToleranciaPesagem = new Repositorio.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem configuracaoToleranciaPesagem = new Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem();

                SetarFiliais(configuracaoToleranciaPesagem, unitOfWork);
                SetarModeloVeicularCarga(configuracaoToleranciaPesagem, unitOfWork);
                SetarTiposCarga(configuracaoToleranciaPesagem, unitOfWork);
                SetarTiposOperacao(configuracaoToleranciaPesagem, unitOfWork);

                PreencherConfiguracaoToleranciaPesagem(configuracaoToleranciaPesagem);

                repositorioConfiguracaoToleranciaPesagem.Inserir(configuracaoToleranciaPesagem);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar");
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


                Repositorio.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem repositorioConfiguracaoToleranciaPesagem = new Repositorio.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem configuracaoToleranciaPesagem = repositorioConfiguracaoToleranciaPesagem.BuscarPorCodigo(codigo, true);

                if (configuracaoToleranciaPesagem == null)
                    return new JsonpResult(true, false, "Registro não encontrado");

                unitOfWork.Start();

                SetarFiliais(configuracaoToleranciaPesagem, unitOfWork);
                SetarModeloVeicularCarga(configuracaoToleranciaPesagem, unitOfWork);
                SetarTiposCarga(configuracaoToleranciaPesagem, unitOfWork);
                SetarTiposOperacao(configuracaoToleranciaPesagem, unitOfWork);

                PreencherConfiguracaoToleranciaPesagem(configuracaoToleranciaPesagem);

                repositorioConfiguracaoToleranciaPesagem.Atualizar(configuracaoToleranciaPesagem);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroToleranciaPesagem ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroToleranciaPesagem()
            {
                Situacao = Request.GetEnumParam<SituacaoToleranciaPesagem>("Situacao"),
                CodigosFiliais = Request.GetListParam<int>("Filiais"),
                CodigosModeloVeicular = Request.GetListParam<int>("ModelosVeiculares"),
                CodigosTipoCarga = Request.GetListParam<int>("TiposCarga"),
                CodigosTipoOperacao = Request.GetListParam<int>("TiposOperacao"),
            };
        }

        private void PreencherConfiguracaoToleranciaPesagem(Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem configuracaoToleranciaPesagem)
        {
            string descricao = Request.Params("Descricao");
            bool ativo = Request.Params("Situacao").ToBool();

            TipoRegraAutorizacaoToleranciaPesagem tipoRegraAutorizacaoToleranciaPesagem;
            Enum.TryParse(Request.Params("TipoRegraAutorizacaoToleranciaPesagem"), out tipoRegraAutorizacaoToleranciaPesagem);

            decimal toleranciaPesoSuperior = 0, toleranciaPesoInferior = 0, percentualToleranciaPesoSuperior = 0, percentualToleranciaPesoInferior = 0;

            if (tipoRegraAutorizacaoToleranciaPesagem == TipoRegraAutorizacaoToleranciaPesagem.Peso)
            {
                decimal.TryParse(Request.Params("ToleranciaPesoSuperior"), out toleranciaPesoSuperior);
                decimal.TryParse(Request.Params("ToleranciaPesoInferior"), out toleranciaPesoInferior);
            }
            else
            {
                decimal.TryParse(Request.Params("PercentualToleranciaPesoSuperior"), out percentualToleranciaPesoSuperior);
                decimal.TryParse(Request.Params("PercentualToleranciaPesoInferior"), out percentualToleranciaPesoInferior);
            }

            configuracaoToleranciaPesagem.Descricao = descricao;
            configuracaoToleranciaPesagem.Ativo = ativo;
            configuracaoToleranciaPesagem.TipoRegra = tipoRegraAutorizacaoToleranciaPesagem;
            configuracaoToleranciaPesagem.ToleranciaPesoSuperior = toleranciaPesoSuperior;
            configuracaoToleranciaPesagem.ToleranciaPesoInferior = toleranciaPesoInferior;
            configuracaoToleranciaPesagem.PercentualToleranciaPesoInferior = percentualToleranciaPesoInferior;
            configuracaoToleranciaPesagem.PercentualToleranciaPesoSuperior = percentualToleranciaPesoSuperior;
            configuracaoToleranciaPesagem.ValidaFilial = configuracaoToleranciaPesagem.Filials?.Count > 0;
            configuracaoToleranciaPesagem.ValidaTipoCarga = configuracaoToleranciaPesagem.TiposCarga?.Count > 0;
            configuracaoToleranciaPesagem.ValidaTipoOperacao = configuracaoToleranciaPesagem.TiposOperacao?.Count > 0;
            configuracaoToleranciaPesagem.ValidaModeloVeicularCarga = configuracaoToleranciaPesagem.ModelosVeicularesCarga?.Count > 0;

        }

        private void SetarTiposCarga(Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem configuracaoToleranciaPesagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            configuracaoToleranciaPesagem.TiposCarga = repositorioTipoDeCarga.BuscarPorCodigos(ObterCodigos("TiposCarga"));
        }

        private void SetarTiposOperacao(Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem configuracaoToleranciaPesagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            configuracaoToleranciaPesagem.TiposOperacao = repositorioTipoOperacao.BuscarPorCodigos(ObterCodigos("TiposOperacao"));
        }

        private void SetarFiliais(Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem configuracaoToleranciaPesagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            configuracaoToleranciaPesagem.Filials = repositorioFilial.BuscarPorCodigos(ObterCodigos("Filial"));
        }

        private void SetarModeloVeicularCarga(Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem configuracaoToleranciaPesagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioFilial = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            configuracaoToleranciaPesagem.ModelosVeicularesCarga = repositorioFilial.BuscarPorCodigos(ObterCodigos("ModeloVeicularCarga"));
        }

        private List<int> ObterCodigos(string paramName)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.RetornoMultiplosConfiguracaoToleranciaPesagem>>(Request.Params(paramName)).Select(x => x.Codigo).ToList();
        }
        #endregion
    }
}
