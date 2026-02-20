using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using Repositorio.Embarcador.Configuracoes;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
	[CustomAuthorize("Configuracoes/CorAnimal")]
	public class CorAnimalController : BaseController
	{

		public CorAnimalController(Conexao conexao) : base(conexao) { }

        [AllowAuthenticate]
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
				grid.AdicionarCabecalho(Localization.Resources.Configuracoes.CorAnimal.Descricao, "Descricao", 50, Align.left, true);
				if (situacaoAtivo == SituacaoAtivoPesquisa.Todos)
					grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 20, Align.left, true);

				CorAnimalRepositorio repCorAnimal = new(workUnit);
				List<CorAnimal> cores = repCorAnimal.Consultar(codigoEmpresa, descricao, situacaoAtivo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
				grid.setarQuantidadeTotal(cores.Count);

				grid.AdicionaRows((from corAnimal in cores
								   select new
								   {
									   corAnimal.Codigo,
									   DescricaoAtivo = corAnimal.Ativo ? "Ativo" : "Inativo",
									   corAnimal.Descricao,
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
				CorAnimal corAnimal = new();
				PreencherCorAnimal(corAnimal);

				CorAnimalRepositorio repCorAnimal = new(workUnit);
				workUnit.Start();
				repCorAnimal.Inserir(corAnimal);
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

				CorAnimalRepositorio repCorAnimal = new(workUnit);
				bool corAnimalExiste = repCorAnimal.VerificarExistenciaPorCodigo(codigo);
				if (!corAnimalExiste)
					return new JsonpResult(false, "Espécie não encontrada.");

				CorAnimal corAnimal = repCorAnimal.BuscarPorCodigo(codigo, false);
				if (corAnimal == null)
					return new JsonpResult(false, "Ocorreu um erro ao carregar os dados da espécie.");

				PreencherCorAnimal(corAnimal);

				workUnit.Start();
				repCorAnimal.Atualizar(corAnimal);
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

				CorAnimalRepositorio repCorAnimal = new(workUnit);
				bool corAnimalExiste = repCorAnimal.VerificarExistenciaPorCodigo(codigo);
				if (!corAnimalExiste)
					return new JsonpResult(false, "Espécie não encontrada.");

				CorAnimal corAnimal = repCorAnimal.BuscarPorCodigo(codigo, false);

				var retorno = new
				{
					corAnimal.Codigo,
					corAnimal.Ativo,
					corAnimal.Descricao
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

				CorAnimalRepositorio repCorAnimal = new(workUnit);
				bool corAnimalExiste = repCorAnimal.VerificarExistenciaPorCodigo(codigo);
				if (!corAnimalExiste)
					return new JsonpResult(false, "Espécie não encontrada.");

				CorAnimal corAnimal = repCorAnimal.BuscarPorCodigo(codigo, false);
				if (corAnimal == null)
					return new JsonpResult(false, "Ocorreu um erro ao carregar os dados da espécie.");

				workUnit.Start();
				repCorAnimal.Deletar(corAnimal);
				workUnit.CommitChanges();

				return new JsonpResult(true);
			}
			catch (Exception ex)
			{
				workUnit.Rollback();
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
				workUnit.Dispose();
			}
		}

		private void PreencherCorAnimal(CorAnimal corAnimal)
		{
			bool.TryParse(Request.GetStringParam("Ativo"), out bool ativo);
			corAnimal.Ativo = ativo;
			corAnimal.Descricao = Request.GetStringParam("Descricao") ?? string.Empty;

			if (corAnimal.Codigo == 0)
				corAnimal.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;
		}
	}
}
