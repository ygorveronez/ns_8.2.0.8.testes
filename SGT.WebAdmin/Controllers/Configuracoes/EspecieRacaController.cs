using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using Repositorio.Embarcador.Configuracoes;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
	[CustomAuthorize("Configuracoes/EspecieRaca")]
	public class EspecieRacaController : BaseController
	{
		public EspecieRacaController(Conexao conexao) : base(conexao) { }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
		{
			UnitOfWork workUnit = new(_conexao.StringConexao);

			try
			{
				int codigoEspecie = Request.GetIntParam("Especie");
				string descricaoRaca = Request.GetStringParam("DescricaoRaca");
				SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam<SituacaoAtivoPesquisa>("AtivoRaca");
				int codigoEmpresa = 0;
				if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
					codigoEmpresa = this.Usuario.Empresa.Codigo;

				Grid grid = new(Request);
				grid.header = new List<Head>();
				grid.AdicionarCabecalho("Código", "Codigo", 20, Align.left, true);
				grid.AdicionarCabecalho(Localization.Resources.Configuracoes.EspecieRaca.Raca, "Descricao", 50, Align.left, true);
				if (situacaoAtivo == SituacaoAtivoPesquisa.Todos)
					grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 20, Align.left, true);
				grid.AdicionarCabecalho("CodigoEspecie", false);
				grid.AdicionarCabecalho("DescricaoEspecie", false);

				EspecieRacaRepositorio especieRacaRepositorio = new(workUnit);
				List<EspecieRaca> racas = especieRacaRepositorio.Consultar(codigoEmpresa, codigoEspecie, descricaoRaca, string.Empty, situacaoAtivo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
				grid.setarQuantidadeTotal(racas.Count);

				grid.AdicionaRows((from raca in racas
								   select new
								   {
									   raca.Codigo,
									   raca.Descricao,
									   CodigoEspecie = raca.Especie != null ? raca.Especie?.Codigo : null,
									   DescricaoEspecie = raca.Especie != null ? raca.Especie?.Descricao : null,
									   DescricaoAtivo = raca.Ativo ? "Ativo" : "Inativo",
								   }).ToList());

				return new JsonpResult(grid);

			}
			catch (Exception ex)
			{
				workUnit.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
			}
			finally
			{
				workUnit.Dispose();
			}
		}

		public async Task<IActionResult> PesquisarRacaPorEspecie()
		{
			UnitOfWork workUnit = new(_conexao.StringConexao);

			try
			{
				int codigoEspecie = Request.GetIntParam("CodigoEspecie");
				if (codigoEspecie <= 0)
					return new JsonpResult(false, "Selecione uma espécie primeiro!");

				int codigoEmpresa = 0;
				if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
					codigoEmpresa = this.Usuario.Empresa.Codigo;

				string descricaoRaca = Request.GetStringParam("DescricaoRaca");

				SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam<SituacaoAtivoPesquisa>("AtivoRaca");

				Grid grid = new(Request);
				grid.header = new List<Head>();
				grid.AdicionarCabecalho("Código", "Codigo", 20, Align.left, true);
				grid.AdicionarCabecalho(Localization.Resources.Configuracoes.EspecieRaca.Raca, "Descricao", 50, Align.left, true);
				if (situacaoAtivo == SituacaoAtivoPesquisa.Todos)
					grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 20, Align.left, true);

				EspecieRacaRepositorio especieRacaRepositorio = new(workUnit);
				List<EspecieRaca> especies = especieRacaRepositorio.Consultar(codigoEmpresa, codigoEspecie, descricaoRaca, string.Empty, situacaoAtivo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
				grid.setarQuantidadeTotal(especies.Count);

				grid.AdicionaRows((from especie in especies
								   select new
								   {
									   especie.Codigo,
									   DescricaoAtivo = especie.Ativo ? "Ativo" : "Inativo",
									   especie.Descricao,
								   }).ToList());

				return new JsonpResult(grid);

			}
			catch (Exception ex)
			{
				workUnit.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
			}
			finally
			{
				workUnit.Dispose();
			}
		}
	}
}
