using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.AtendimentoCRM
{
    [CustomAuthorize("AtendimentoCrm/PortifolioAtendimento")]
    public class PortifolioAtendimentoController : BaseController
    {
		#region Construtores

		public PortifolioAtendimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
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

        #region Métodos Privados

        private Models.Grid.Grid ObterGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição do Portifólio", "Descricao", 10, Models.Grid.Align.left, true);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.GrupoMotivoChamado repositorioGenero = new Repositorio.Embarcador.Chamados.GrupoMotivoChamado(unitOfWork);
            Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao repGrupoMotivoChamadoTipoIntegracao = new Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGrupoMotivoChamado filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = ObterGrid();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

            int totalRegistros = repositorioGenero.ContarConsulta(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado> listaGrid = totalRegistros > 0 ? repositorioGenero.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado>();

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                        };

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        public Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGrupoMotivoChamado ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGrupoMotivoChamado()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao"),
            };
        }

        private string ObterPropriedadeOrdenar(string propOrdenar)
        {
            return propOrdenar;
        }

        #endregion
    }
}
