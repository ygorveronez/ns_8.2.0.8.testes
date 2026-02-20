using System;
using System.Collections.Generic;
using System.Linq;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/GeneroMotivoChamado")]
	public class GeneroMotivoChamadoController : BaseController
	{
		#region Construtores

		public GeneroMotivoChamadoController(Conexao conexao) : base(conexao) { }

		#endregion

		[AllowAuthenticate]
		public async Task<IActionResult> Pesquisa()
		{
			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
			try
			{
				return new JsonpResult(ExecutaPesquisa(unitOfWork));
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
				Repositorio.Embarcador.Chamados.GeneroMotivoChamado repositorioGenero = new Repositorio.Embarcador.Chamados.GeneroMotivoChamado(unitOfWork);

				Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado genero = new Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado();

				unitOfWork.Start();

				PreencherEntidade(genero, unitOfWork);

				repositorioGenero.Inserir(genero, Auditado);

				unitOfWork.CommitChanges();

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
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
				Repositorio.Embarcador.Chamados.GeneroMotivoChamado repositorioGenero = new Repositorio.Embarcador.Chamados.GeneroMotivoChamado(unitOfWork);

				int codigoGenero = Request.GetIntParam("Codigo");

				Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado genero = repositorioGenero.BuscarPorCodigo(codigoGenero, false);

				if (genero == null)
					return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);
				
				unitOfWork.Start();

				PreencherEntidade(genero, unitOfWork);

				repositorioGenero.Atualizar(genero, Auditado);

				unitOfWork.CommitChanges();

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
				Repositorio.Embarcador.Chamados.GeneroMotivoChamado repositorioGenero = new Repositorio.Embarcador.Chamados.GeneroMotivoChamado(unitOfWork);

				int codigoGenero = Request.GetIntParam("Codigo");

				Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado genero = repositorioGenero.BuscarPorCodigo(codigoGenero, false);

				if (genero == null)
					return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

				var retorno = new
				{
					genero.Codigo,
					genero.Descricao,
					genero.CodigoIntegracao,
					genero.Status
				};

				return new JsonpResult(retorno);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
				Repositorio.Embarcador.Chamados.GeneroMotivoChamado repositorioGenero = new Repositorio.Embarcador.Chamados.GeneroMotivoChamado(unitOfWork);

				int codigoGenero = Request.GetIntParam("Codigo");

				Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado genero = repositorioGenero.BuscarPorCodigo(codigoGenero, false);

				if (genero == null)
					return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

				unitOfWork.Start();

				repositorioGenero.Deletar(genero, Auditado);

				unitOfWork.CommitChanges();

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
			}
			finally
			{
				unitOfWork.Dispose();
			}
		}

		#region Métodos Privados

		private Models.Grid.Grid GridPesquisa()
		{
			Models.Grid.Grid grid = new Models.Grid.Grid(Request)
			{
				header = new List<Models.Grid.Head>()
			};

			grid.AdicionarCabecalho("Codigo", false);
			grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

			if (Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo) == SituacaoAtivoPesquisa.Todos)
				grid.AdicionarCabecalho("Status", "DescricaoStatus", 25, Models.Grid.Align.left, true);

			return grid;
		}

		private Models.Grid.Grid ExecutaPesquisa(Repositorio.UnitOfWork unitOfWork)
		{
			Repositorio.Embarcador.Chamados.GeneroMotivoChamado repositorioGenero = new Repositorio.Embarcador.Chamados.GeneroMotivoChamado(unitOfWork);

			Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGeneroMotivoChamado filtrosPesquisa = ObterFiltrosPesquisa();

			Models.Grid.Grid grid = GridPesquisa();

			Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

			int totalRegistros = repositorioGenero.ContarConsulta(filtrosPesquisa);
			List<Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado> listaGrid = totalRegistros > 0 ? repositorioGenero.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado>();

			var lista = from obj in listaGrid
				select new
				{
					obj.Codigo,
					obj.Descricao,
					obj.CodigoIntegracao,
					obj.DescricaoStatus
				};

			grid.AdicionaRows(lista);
			grid.setarQuantidadeTotal(totalRegistros);

			return grid;
		}

		public Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGeneroMotivoChamado ObterFiltrosPesquisa()
		{
			return new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGeneroMotivoChamado()
			{
				Descricao = Request.GetStringParam("Descricao"),
				Status = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo),
			};
		}

		private string ObterPropriedadeOrdenar(string propOrdenar)
		{
			if (propOrdenar == "DescricaoStatus")
				return "Status";

			return propOrdenar;
		}

		private void PreencherEntidade(Dominio.Entidades.Embarcador.Chamados.GeneroMotivoChamado genero, Repositorio.UnitOfWork unitOfWork)
		{
			genero.Descricao = Request.GetStringParam("Descricao");
			genero.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
			genero.Status = Request.GetBoolParam("Status");
		}

		#endregion
	}
}
