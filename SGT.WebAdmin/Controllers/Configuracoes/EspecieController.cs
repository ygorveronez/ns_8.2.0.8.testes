using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using Repositorio.Embarcador.Configuracoes;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
	[CustomAuthorize("Configuracoes/Especie")]
	public class EspecieController : BaseController
	{
		#region Construtores

		public EspecieController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
		{
			UnitOfWork workUnit = new(_conexao.StringConexao);

			try
			{
				string descricaoEspecie = Request.GetStringParam("DescricaoEspecie");
				string descricaoRaca = Request.GetStringParam("DescricaoRaca");
				int codigoRaca = Request.GetIntParam("Raca");

				SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo");

				int codigoEmpresa = 0;
				if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
					codigoEmpresa = this.Usuario.Empresa.Codigo;

				Grid grid = new(Request);
				grid.header = new List<Head>();
				grid.AdicionarCabecalho("Código", "Codigo", 20, Align.left, true);
				grid.AdicionarCabecalho(Localization.Resources.Configuracoes.Especie.DescricaoEspecie, "Descricao", 50, Align.left, true);
				if (situacaoAtivo == SituacaoAtivoPesquisa.Todos)
					grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 20, Align.left, true);
				grid.AdicionarCabecalho("CodigoRaca", false);
				grid.AdicionarCabecalho("DescricaoRaca", false);


				EspecieRepositorio repEspecie = new(workUnit);
				List<Especie> especies = repEspecie.Consultar(codigoEmpresa, descricaoEspecie, descricaoRaca, codigoRaca, situacaoAtivo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
				grid.setarQuantidadeTotal(especies.Count);

				grid.AdicionaRows((from especie in especies
								   select new
								   {
									   especie.Codigo,
									   especie.Descricao,
									   CodigoRaca = especie.Racas != null ? (especie.Racas?.Count() == 1 ? especie.Racas?.FirstOrDefault().Codigo : 0) : null,
									   DescricaoRaca = especie.Racas != null ? (especie.Racas?.Count() == 1 ? especie.Racas?.FirstOrDefault().Descricao : string.Empty) : null,
									   DescricaoAtivo = especie.Ativo ? "Ativo" : "Inativo",
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

		public async Task<IActionResult> BuscarPorCodigo()
		{
			UnitOfWork workUnit = new UnitOfWork(_conexao.StringConexao);
			try
			{
				int.TryParse(Request.Params("Codigo"), out int codigo);
				EspecieRepositorio especieRepositorio = new(workUnit);
				Especie especie = especieRepositorio.BuscarPorCodigo(codigo, false);
				int posicaoGrid = 1;
				var dynEspecie = new
				{
					especie.Codigo,
					especie.Descricao,
					especie.Ativo,
					Racas = (from obj in especie.Racas
							   select new
							   {
								   PosicaoGrid = posicaoGrid++,
								   obj.Codigo,
								   obj.Descricao,
								   obj.Ativo,
							   }).ToList()
				};

				return new JsonpResult(dynEspecie);
			}
			catch (Exception ex)
			{
				Servicos.Log.TratarErro(ex);
				return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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
				Especie especie = new();
				PreencherEspecie(especie);
				EspecieRepositorio especieRepositorio = new(workUnit);

				workUnit.Start();
				especieRepositorio.Inserir(especie);
				SalvarRacas(especie, workUnit);
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

				EspecieRepositorio repEspecie = new(workUnit);
				bool especieExiste = repEspecie.VerificarExistenciaPorCodigo(codigo);
				if (!especieExiste)
					return new JsonpResult(false, "Espécie não encontrada.");

				Especie especie = repEspecie.BuscarPorCodigo(codigo, false);
				if (especie == null)
					return new JsonpResult(false, "Ocorreu um erro ao carregar os dados da espécie.");

				PreencherEspecie(especie);

				workUnit.Start();
				SalvarRacas(especie, workUnit);				
				repEspecie.Atualizar(especie);
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

		public async Task<IActionResult> ExcluirPorCodigo()
		{
			UnitOfWork workUnit = new(_conexao.StringConexao);
			try
			{
				int.TryParse(Request.Params("Codigo"), out int codigo);
				if (codigo <= 0)
					return new JsonpResult(false, "Código inválido.");

				EspecieRepositorio especieRepositorio = new(workUnit);
				bool especieExiste = especieRepositorio.VerificarExistenciaPorCodigo(codigo);
				if (!especieExiste)
					return new JsonpResult(false, "Espécie não encontrada.");

				Especie especie = especieRepositorio.BuscarPorCodigo(codigo, false);
				if (especie == null)
					return new JsonpResult(false, "Ocorreu um erro ao carregar os dados da espécie.");

                EspecieRacaRepositorio especieRacaRepositorio = new(workUnit);

                workUnit.Start();
                especieRacaRepositorio.Deletar(especie.Racas.ToList());
                especieRepositorio.Deletar(especie);
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

		private void PreencherEspecie(Especie especie)
		{
			bool.TryParse(Request.GetStringParam("Ativo"), out bool ativo);
			especie.Ativo = ativo;
			especie.Descricao = Request.GetStringParam("DescricaoEspecie") ?? string.Empty;

			if (especie.Codigo == 0)
				especie.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;

			string descricaoRaca = Request.GetStringParam("DescricaoRaca") ?? string.Empty;
		}

		private void SalvarRacas(Especie especie, UnitOfWork workUnit)
		{
			EspecieRacaRepositorio especieRacaRepositorio = new(workUnit);

			dynamic dynRacas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Racas"));

			if (especie.Racas == null || especie.Racas.Count == 0)
				especie.Racas = new List<EspecieRaca>();
			else
			{
				List<int> codigos = new List<int>();

				foreach (var raca in dynRacas)
					if (raca.Codigo != null)
						codigos.Add((int)raca.Codigo);

				List<EspecieRaca> racasDeletar = (from obj in especie.Racas where !codigos.Contains(obj.Codigo) select obj).ToList();

				for (var i = 0; i < racasDeletar.Count; i++)
				{
					EspecieRaca especieRaca = racasDeletar[i];
					Servicos.Auditoria.Auditoria.Auditar(Auditado, especie, "Removeu a raça da espécie " + especie.Descricao, workUnit);
					especieRacaRepositorio.Deletar(especieRaca, Auditado);
				}
			}

			foreach (var dynRaca in dynRacas)
			{
				int.TryParse((string)dynRaca.Codigo, out int codigoRaca);
				EspecieRaca especieRaca = codigoRaca > 0 ? especieRacaRepositorio.BuscarPorCodigo(codigoRaca, true) ?? new() : new();

				bool.TryParse((string)dynRaca.Ativo, out bool ativo);
				especieRaca.Descricao = (string)dynRaca.Descricao;
				especieRaca.Ativo = ativo;
				especieRaca.Especie = especie;

				if (especieRaca.Codigo > 0)
					especieRacaRepositorio.Atualizar(especieRaca, Auditado);
				else
				{
					Servicos.Auditoria.Auditoria.Auditar(Auditado, especie, $"Adicionou a raça { especieRaca.Descricao } da espécie { especie.Descricao }", workUnit);
					especieRacaRepositorio.Inserir(especieRaca, Auditado);
				}
			}
		}
	}
}
