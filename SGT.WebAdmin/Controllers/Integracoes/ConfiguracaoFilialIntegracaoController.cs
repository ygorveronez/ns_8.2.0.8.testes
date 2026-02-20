using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Integracoes
{

    [CustomAuthorize("Integracoes/ConfiguracaoFilialIntegracao")]
    public class ConfiguracaoFilialIntegracaoController : BaseController
    {
        #region Construtores

        public ConfiguracaoFilialIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar(CancellationToken cancellationToken)
        {
            try
            {
                return new JsonpResult(await ObterGridPesquisa(cancellationToken));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao repConfiguracaoIntegracaoFilial = new Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao configuracaoIntegracaoFilial = new Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao();

                await PreencherEntidade(configuracaoIntegracaoFilial, unitOfWork, cancellationToken);
                await ValidarEntidade(configuracaoIntegracaoFilial, unitOfWork, cancellationToken);

                await repConfiguracaoIntegracaoFilial.InserirAsync(configuracaoIntegracaoFilial);

                await SalvarTipoIntegracao(configuracaoIntegracaoFilial, unitOfWork, cancellationToken);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao repConfiguracaoFilialIntegracao = new Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao(unitOfWork, cancellationToken);

                await unitOfWork.StartAsync(cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao configuracaoIntegracaoFilial = await repConfiguracaoFilialIntegracao.BuscarPorCodigoAsync(codigo, true);

                if (configuracaoIntegracaoFilial == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                await PreencherEntidade(configuracaoIntegracaoFilial, unitOfWork, cancellationToken);
                await ValidarEntidade(configuracaoIntegracaoFilial, unitOfWork, cancellationToken);

                await repConfiguracaoFilialIntegracao.AtualizarAsync(configuracaoIntegracaoFilial);

                await SalvarTipoIntegracao(configuracaoIntegracaoFilial, unitOfWork, cancellationToken);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao repConfiguracaoFilialIntegracao = new Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao(unitOfWork, cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao configuracaoIntegracaoFilial = await repConfiguracaoFilialIntegracao.BuscarPorCodigoAsync(codigo, false);
                if (configuracaoIntegracaoFilial == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    configuracaoIntegracaoFilial.Codigo,
                    configuracaoIntegracaoFilial.DescricaoSituacao,
                    TipoOperacao = new { Codigo = configuracaoIntegracaoFilial.TipoOperacao != null ? configuracaoIntegracaoFilial.TipoOperacao.Codigo : 0, configuracaoIntegracaoFilial.TipoOperacao?.Descricao },
                    Filial = new { Codigo = configuracaoIntegracaoFilial.Filial != null ? configuracaoIntegracaoFilial.Filial.Codigo : 0, configuracaoIntegracaoFilial.Filial?.Descricao },
                    TiposIntegracao = (
                        from obj in configuracaoIntegracaoFilial.TiposIntegracao
                        select new
                        {
                            obj.Codigo,
                            obj.Tipo,
                            Descricao = obj.Tipo.ObterDescricao(),
                        }).ToList(),
                };

                return new JsonpResult(retorno);
            }
            catch (BaseException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarIntegracoes(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tipoIntegracoes = await repTipoIntegracao.BuscarTodosAtivosAsync();

                var retornoIntegracoes = (
                        from obj in tipoIntegracoes
                        select new
                        {
                            obj.Codigo,
                            obj.Tipo,
                            Descricao = obj.Tipo.ObterDescricao(),
                        }
                    ).ToList();
                return new JsonpResult(new
                {
                    Integracoes = retornoIntegracoes
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoBuscarIntegracoes);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao repConfiguracaoFilialIntegracao = new Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao(unitOfWork, cancellationToken);

                await unitOfWork.StartAsync(cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao configuracaoIntegracaoFilial = await repConfiguracaoFilialIntegracao.BuscarPorCodigoAsync(codigo, true);

                if (configuracaoIntegracaoFilial == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                await repConfiguracaoFilialIntegracao.DeletarAsync(configuracaoIntegracaoFilial);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Chamado.MotivoChamado.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculo);

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.MotivoChamado.OcorreUmaFalhaAoRemoverDados);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task<Grid> ObterGridPesquisa(CancellationToken cancellationToken)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao repConfiguracaoIntegracaoFilial = new Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao(unitOfWork, cancellationToken);

            Grid grid = ObterGridPesquisaConfiguracaoFilialIntegracao();

            Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaConfiguracaoFilialIntegracao filtrosPesquisa = ObterFiltrosPesquisa();

            int totalRegistros = await repConfiguracaoIntegracaoFilial.ContarConsultaAsync(filtrosPesquisa);

            if (totalRegistros == 0)
            {
                grid.AdicionaRows(new List<dynamic>() { });
                return grid;
            }

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarOuAgrupar);
            List<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao> listaConfiguracores = await repConfiguracaoIntegracaoFilial.ConsultarAsync(filtrosPesquisa, parametrosConsulta);

            var retorno = (from configuracao in listaConfiguracores
                           select new
                           {
                               configuracao.Codigo,
                               Filial = configuracao.Filial.Descricao ?? string.Empty,
                               TipoOperacao = configuracao.TipoOperacao?.Descricao ?? string.Empty,
                               Situacao = configuracao.DescricaoSituacao
                           }).ToList();

            grid.AdicionaRows(retorno);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Grid ObterGridPesquisaConfiguracaoFilialIntegracao()
        {
            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 35, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 35, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 25, Models.Grid.Align.left, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaConfiguracaoFilialIntegracao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaConfiguracaoFilialIntegracao()
            {
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoFilial = Request.GetIntParam("Filial"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao"),
            };
        }

        private async Task PreencherEntidade(Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao configuracaoIntegracaoFilial, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);

            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoFilial = Request.GetIntParam("Filial");

            configuracaoIntegracaoFilial.Ativo = Request.GetBoolParam("Situacao");
            configuracaoIntegracaoFilial.Filial = await repFilial.BuscarPorCodigoAsync(codigoFilial);
            configuracaoIntegracaoFilial.TipoOperacao = codigoTipoOperacao > 0 ? await repTipoOperacao.BuscarPorCodigoAsync(codigoTipoOperacao) : null;
        }

        private async Task ValidarEntidade(Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao configuracaoIntegracaoFilial, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao repConfiguracao = new Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao configDuplicada = await repConfiguracao.BuscarConfiguracaoDuplicada(configuracaoIntegracaoFilial);
            if (configuracaoIntegracaoFilial.Ativo && configDuplicada != null)
                throw new ControllerException("Já existe uma configuração de integração com os dados informados.");
        }

        private async Task SalvarTipoIntegracao(Dominio.Entidades.Embarcador.Integracao.ConfiguracaoFilialIntegracao configuracaoIntegracaoFilial, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork, cancellationToken);

            dynamic tiposIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposIntegracao"));

            if (configuracaoIntegracaoFilial.TiposIntegracao == null)
            {
                configuracaoIntegracaoFilial.TiposIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoIntegracao in tiposIntegracao)
                    codigos.Add((int)tipoIntegracao.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposDeletar = configuracaoIntegracaoFilial.TiposIntegracao.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoDeletar in tiposDeletar)
                {
                    configuracaoIntegracaoFilial.TiposIntegracao.Remove(tipoIntegracaoDeletar);
                    codigos.Remove(tipoIntegracaoDeletar.Codigo);
                }
            }
            ;

            foreach (dynamic tipoIntegracao in tiposIntegracao)
            {
                int codigo = 0;
                codigo = tipoIntegracao.Codigo;

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoAdicionar = await repTipoIntegracao.BuscarPorCodigoAsync(codigo, false);

                if (!configuracaoIntegracaoFilial.TiposIntegracao.Any(o => o.Codigo == (int)tipoIntegracao.Codigo))
                    configuracaoIntegracaoFilial.TiposIntegracao.Add(tipoIntegracaoAdicionar);
            }
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
