using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/GrupoPessoas")]
    public class GrupoPessoasModeloVeicularEmbarcadorController : BaseController
    {
		#region Construtores

		public GrupoPessoasModeloVeicularEmbarcadorController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterModelosDoEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");

                Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador repGrupoPessoasModeloVeicularEmbarcador = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador> registros = repGrupoPessoasModeloVeicularEmbarcador.BuscarPorGrupoPessoas(codigoGrupoPessoas);

                var retorno = registros.Select(o => new { o.Codigo, Descricao = o.DescricaoModeloVeicularEmbarcador }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os modelos veiculares do embarcador.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo Veicular do Embarcador", "DescricaoModeloVeicularEmbarcador", 45, Models.Grid.Align.left, true);
                

                Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga(unitOfWork);

                int countRegistros = repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ContarConsulta(codigoGrupoPessoas);
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga> registros = countRegistros > 0 ? repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.Consultar(codigoGrupoPessoas, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga>();

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows((from obj in registros
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.ModeloVeicularEmbarcador.DescricaoModeloVeicularEmbarcador,
                                       ModeloVeicular = obj.ModeloVeicular.Descricao ?? string.Empty
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os modelos veiculares configurados.");
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga modeloVeicular = repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.BuscarPorCodigo(codigo, false);

                return new JsonpResult(new
                {
                    modeloVeicular.Codigo,
                    ModeloVeicular = new { modeloVeicular.ModeloVeicular.Codigo, modeloVeicular.ModeloVeicular.Descricao },
                    ModeloVeicularEmbarcador = modeloVeicular.ModeloVeicularEmbarcador.Codigo
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o modelo veicular.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarModelosVeicularesEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");

                string url = Request.GetStringParam("URLIntegracaoMultiEmbarcador");
                string token = Request.GetStringParam("TokenIntegracaoMultiEmbarcador");

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);

                Servicos.Embarcador.Pessoa.GrupoPessoaModeloVeicularEmbarcador.BuscarModelosVeicularesEmbarcador(out string erro, grupoPessoas, url, token, unitOfWork, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");
                int codigoModeloVeicularEmbarcador = Request.GetIntParam("ModeloVeicularEmbarcador");

                Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador repGrupoPessoasModeloVeicularEmbarcador = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador grupoPessoasModeloVeicularEmbarcador = repGrupoPessoasModeloVeicularEmbarcador.BuscarPorCodigo(codigoModeloVeicularEmbarcador, false);

                if (grupoPessoasModeloVeicularEmbarcador == null)
                    return new JsonpResult(false, true, "O modelo veicular do embarcador não foi encontrado.");

                if (repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ExistePorGrupoPessoasEModeloVeicular(grupoPessoasModeloVeicularEmbarcador.GrupoPessoas.Codigo, codigoModeloVeicular))
                    return new JsonpResult(false, true, "Já existe uma configuração para este modelo veicular.");

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga()
                {
                    ModeloVeicular = repModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicular),
                    ModeloVeicularEmbarcador = grupoPessoasModeloVeicularEmbarcador
                };
                
                repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.Inserir(grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                int codigo = Request.GetIntParam("Codigo");
                int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");
                int codigoModeloVeicularEmbarcador = Request.GetIntParam("ModeloVeicularEmbarcador");

                Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador repGrupoPessoasModeloVeicularEmbarcador = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador grupoPessoasModeloVeicularEmbarcador = repGrupoPessoasModeloVeicularEmbarcador.BuscarPorCodigo(codigoModeloVeicularEmbarcador, false);

                if (grupoPessoasModeloVeicularEmbarcador == null)
                    return new JsonpResult(false, true, "O modelo veicular do embarcador não foi encontrado.");

                if (repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ExistePorGrupoPessoasEModeloVeicular(grupoPessoasModeloVeicularEmbarcador.GrupoPessoas.Codigo, codigoModeloVeicular, codigo))
                    return new JsonpResult(false, true, "Já existe uma configuração para este modelo veicular.");

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.BuscarPorCodigo(codigo, true);

                if (grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ModeloVeicular = repModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicular);
                grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ModeloVeicularEmbarcador = grupoPessoasModeloVeicularEmbarcador;

                repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.Atualizar(grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
