using SGT.WebAdmin.Controllers.Anexo;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/TabelaFrete", "Fretes/AutorizacaoTabelaFrete", "Fretes/ConsultaReajusteTabelaFrete")]
    public class VigenciaTabelaFreteAnexoController : AnexoController<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFreteAnexo, Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>
    {
		#region Construtores

		public VigenciaTabelaFreteAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		[AllowAuthenticate]
        public async Task<IActionResult> PesquisaAnexoAutorizacaoTabelaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Vigência", "Vigencia", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 30, Models.Grid.Align.left, false);

                int codigoTabelaFrete = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.VigenciaTabelaFreteAnexo repositorioAnexo = new Repositorio.Embarcador.Frete.VigenciaTabelaFreteAnexo(unitOfWork);
                int totalRegistros = repositorioAnexo.ContarConsultaPorAutorizacaoTabelaFrete(codigoTabelaFrete);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.VigenciaAnexo> listaVigenciaAnexo = totalRegistros > 0 ? repositorioAnexo.ConsultarPorAutorizacaoTabelaFrete(codigoTabelaFrete, grid.inicio, grid.limite) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.VigenciaAnexo>();

                var listaVigenciaAnexoRetornar = (
                    from anexo in listaVigenciaAnexo
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo,
                        anexo.Vigencia
                    }
                ).ToList();

                grid.AdicionaRows(listaVigenciaAnexoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os anexos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAnexoReajusteTabelaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Vigência", "Vigencia", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 30, Models.Grid.Align.left, false);

                int codigoReajusteTabelaFrete = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.VigenciaTabelaFreteAnexo repositorioAnexo = new Repositorio.Embarcador.Frete.VigenciaTabelaFreteAnexo(unitOfWork);
                int totalRegistros = repositorioAnexo.ContarConsultaPorReajusteTabelaFrete(codigoReajusteTabelaFrete);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.VigenciaAnexo> listaVigenciaAnexo = totalRegistros > 0 ? repositorioAnexo.ConsultarPorReajusteTabelaFrete(codigoReajusteTabelaFrete, grid.inicio, grid.limite) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.VigenciaAnexo>();

                var listaVigenciaAnexoRetornar = (
                    from anexo in listaVigenciaAnexo
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo,
                        anexo.Vigencia
                    }
                ).ToList();

                grid.AdicionaRows(listaVigenciaAnexoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os anexos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}