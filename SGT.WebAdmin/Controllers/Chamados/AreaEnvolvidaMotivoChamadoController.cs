using System;
using System.Collections.Generic;
using System.Linq;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/AreaEnvolvidaMotivoChamado")]
	public class AreaEnvolvidaMotivoChamadoController : BaseController
	{
		#region Construtores

		public AreaEnvolvidaMotivoChamadoController(Conexao conexao) : base(conexao) { }

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
				Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado repositorioAreaEnvolvida = new Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado(unitOfWork);

				Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado areaEnvolvida = new Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado();

				unitOfWork.Start();

				PreencherEntidade(areaEnvolvida, unitOfWork);

				repositorioAreaEnvolvida.Inserir(areaEnvolvida, Auditado);

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
				Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado repositorioAreaEnvolvida = new Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado(unitOfWork);

				int codigoAreaEnvolvida = Request.GetIntParam("Codigo");

				Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado areaEnvolvida = repositorioAreaEnvolvida.BuscarPorCodigo(codigoAreaEnvolvida, false);

				if (areaEnvolvida == null)
					return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

				unitOfWork.Start();

				PreencherEntidade(areaEnvolvida, unitOfWork);

				repositorioAreaEnvolvida.Atualizar(areaEnvolvida, Auditado);

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
				Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado repositorioAreaEnvolvida = new Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado(unitOfWork);

				int codigoAreaEnvolvida = Request.GetIntParam("Codigo");

				Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado areaEnvolvida = repositorioAreaEnvolvida.BuscarPorCodigo(codigoAreaEnvolvida, false);

				if (areaEnvolvida == null)
					return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

				var retorno = new
				{
					areaEnvolvida.Codigo,
					areaEnvolvida.Descricao,
					areaEnvolvida.CodigoIntegracao,
					areaEnvolvida.Status
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
				Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado repositorioAreaEnvolvida = new Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado(unitOfWork);

				int codigoAreaEnvolvida = Request.GetIntParam("Codigo");

				Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado areaEnvolvida = repositorioAreaEnvolvida.BuscarPorCodigo(codigoAreaEnvolvida, false);

				if (areaEnvolvida == null)
					return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

				unitOfWork.Start();

				repositorioAreaEnvolvida.Deletar(areaEnvolvida, Auditado);

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
			Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado repositorioAreaEnvolvida = new Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado(unitOfWork);

			Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAreaEnvolvidaMotivoChamado filtrosPesquisa = ObterFiltrosPesquisa();

			Models.Grid.Grid grid = GridPesquisa();

			Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

			int totalRegistros = repositorioAreaEnvolvida.ContarConsulta(filtrosPesquisa);
			List<Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado> listaGrid = totalRegistros > 0 ? repositorioAreaEnvolvida.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado>();

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

		public Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAreaEnvolvidaMotivoChamado ObterFiltrosPesquisa()
		{
			return new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAreaEnvolvidaMotivoChamado()
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

		private void PreencherEntidade(Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado areaEnvolvida, Repositorio.UnitOfWork unitOfWork)
		{
			areaEnvolvida.Descricao = Request.GetStringParam("Descricao");
			areaEnvolvida.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
			areaEnvolvida.Status = Request.GetBoolParam("Status");
		}

		#endregion
	}
}
