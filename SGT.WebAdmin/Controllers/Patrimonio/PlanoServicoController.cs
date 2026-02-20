using Dominio.Entidades.Embarcador.Patrimonio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using Repositorio.Embarcador.Patrimonio;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Patrimonio
{
	[CustomAuthorize("Patrimonio/PlanoServico")]
	public class PlanoServicoController : BaseController
	{
		#region Construtores

		public PlanoServicoController(Conexao conexao) : base(conexao) { }

		#endregion

		public async Task<IActionResult> Pesquisar()
		{
			UnitOfWork workUnit = new(_conexao.StringConexao);

			try
			{
				SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo");
				string descricao = Request.GetStringParam("Descricao");
				int codigoEmpresa = 0;
				if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
					codigoEmpresa = this.Usuario.Empresa.Codigo;

				Grid grid = new(Request);
				grid.header = new List<Head>();
				grid.AdicionarCabecalho("Código", "Codigo", 20, Align.left, true);
				grid.AdicionarCabecalho(Localization.Resources.Patrimonio.PlanoServico.Descricao, "Descricao", 50, Align.left, true);
				grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Ativo", 20, Align.left, true);

				PlanoServicoRepositorio planoServicoRepositorio = new(workUnit);
				List<PlanoServico> planos = planoServicoRepositorio.Consultar(codigoEmpresa, descricao, situacaoAtivo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
				grid.setarQuantidadeTotal(planos.Count);

				grid.AdicionaRows((from planoServico in planos
								   select new
								   {
									   planoServico.Codigo,
									   Ativo = planoServico.Ativo ? "Ativo" : "Inativo",
									   planoServico.Descricao,
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

		public async Task<IActionResult> Adicionar()
		{
			UnitOfWork workUnit = new(_conexao.StringConexao);
			try
			{
				PlanoServico planoServico = new();
				PreencherPlanoServico(planoServico);

				PlanoServicoRepositorio repPlanoServico = new(workUnit);
				workUnit.Start();
				repPlanoServico.Inserir(planoServico);
				workUnit.CommitChanges();

				return new JsonpResult(true);
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

		public async Task<IActionResult> Atualizar()
		{
			UnitOfWork workUnit = new(_conexao.StringConexao);
			try
			{
				int.TryParse(Request.Params("Codigo"), out int codigo);
				if (codigo < 0)
					return new JsonpResult(false, "Código inválido.");

				PlanoServicoRepositorio repPlanoServico = new(workUnit);
				bool planoServicoExiste = repPlanoServico.VerificarExistenciaPorCodigo(codigo);
				if (!planoServicoExiste)
					return new JsonpResult(false, "Espécie não encontrada.");

				PlanoServico planoServico = repPlanoServico.BuscarPorCodigo(codigo, false);
				if (planoServico == null)
					return new JsonpResult(false, "Ocorreu um erro ao carregar os dados da espécie.");

				PreencherPlanoServico(planoServico);

				workUnit.Start();
				repPlanoServico.Atualizar(planoServico);
				workUnit.CommitChanges();
				return new JsonpResult(true);
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

		public async Task<IActionResult> BuscarPorCodigo()
		{
			UnitOfWork workUnit = new(_conexao.StringConexao);
			try
			{
				int.TryParse(Request.Params("Codigo"), out int codigo);
				if (codigo < 0)
					return new JsonpResult(false, "Código inválido.");

				PlanoServicoRepositorio repPlanoServico = new(workUnit);
				bool planoServicoExiste = repPlanoServico.VerificarExistenciaPorCodigo(codigo);
				if (!planoServicoExiste)
					return new JsonpResult(false, "Espécie não encontrada.");

				PlanoServico planoServico = repPlanoServico.BuscarPorCodigo(codigo, false);

				var retorno = new
				{
					planoServico.Codigo,
					planoServico.Ativo,
					planoServico.Descricao
				};

				return new JsonpResult(retorno);
			}
			catch (Exception ex)
			{
				workUnit.Rollback();
				Servicos.Log.TratarErro(ex);
				throw;
			}
			finally
			{
				workUnit.Dispose();
			}
		}

		public async Task<IActionResult> ExcluirPorCodigo()
		{
			UnitOfWork workUnit = new(_conexao.StringConexao);
			try
			{
				int.TryParse(Request.Params("Codigo"), out int codigo);
				if (codigo < 0)
					return new JsonpResult(false, "Código inválido.");

				PlanoServicoRepositorio repPlanoServico = new(workUnit);
				bool planoServicoExiste = repPlanoServico.VerificarExistenciaPorCodigo(codigo);
				if (!planoServicoExiste)
					return new JsonpResult(false, "Espécie não encontrada.");

				PlanoServico planoServico = repPlanoServico.BuscarPorCodigo(codigo, false);
				if (planoServico == null)
					return new JsonpResult(false, "Ocorreu um erro ao carregar os dados da espécie.");

				workUnit.Start();
				repPlanoServico.Deletar(planoServico);
				workUnit.CommitChanges();

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				workUnit.Rollback();
				Servicos.Log.TratarErro(ex);
				throw;
			}
			finally
			{
				workUnit.Dispose();
			}
		}

		private void PreencherPlanoServico(PlanoServico planoServico)
		{
			bool.TryParse(Request.GetStringParam("Ativo"), out bool ativo);
			planoServico.Ativo = ativo;
			planoServico.Descricao = Request.GetStringParam("Descricao") ?? string.Empty;

			if (planoServico.Codigo == 0)
				planoServico.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;
		}
	}
}
