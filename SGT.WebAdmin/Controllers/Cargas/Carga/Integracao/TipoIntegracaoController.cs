using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Integracao
{
    [CustomAuthorize(new string[] { "Cargas/TipoIntegracao" })]
    public class TipoIntegracaoController : BaseController
    {
        #region Construtores

        public TipoIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTodos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos = null;
                bool? integracaoTransportador = Request.GetNullableBoolParam("IntegracaoTransportador");

                if (!string.IsNullOrWhiteSpace(Request.Params("Tipos")))
                    tipos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>>(Request.Params("Tipos"));

                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho, cancellationToken);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = await repTipoIntegracao.BuscarPorTiposAsync(tipos, integracaoTransportador);

                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeDeTrabalho, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = await repIntegracaoIntercab.BuscarIntegracaoAsync();

                return new JsonpResult((from obj in tiposIntegracao
                                        orderby obj.Tipo
                                        select new
                                        {
                                            Codigo = obj.Tipo,
                                            Descricao = obj.Descricao,
                                            CodigoTipo = obj.Codigo,
                                            DefinirModalPeloTipoCarga = integracaoIntercab?.DefinirModalPeloTipoCarga ?? false
                                        }).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os tipos de integração.");
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                string descricao = Request.GetStringParam("Descricao");
                int ativo = Request.GetIntParam("Ativo");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Ativo");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoTipoIntegracao? grupo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoTipoIntegracao>("GrupoTipoIntegracao");

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 55, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Grupo", "DescricaoGrupo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 20, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.Consultar(descricao, status, grupo, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repTipoIntegracao.ContarConsulta(descricao, status, grupo);

                var lista = (
                    from row in tiposIntegracao
                    select new
                    {
                        row.Codigo,
                        row.Descricao,
                        row.DescricaoGrupo,
                        row.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}

