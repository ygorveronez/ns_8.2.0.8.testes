using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/GrupoPessoas")]
    public class GrupoPessoasTipoCargaEmbarcadorController : BaseController
    {
		#region Construtores

		public GrupoPessoasTipoCargaEmbarcadorController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterTiposCargaEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");

                Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador repGrupoPessoasTipoCargaEmbarcador = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador> registros = repGrupoPessoasTipoCargaEmbarcador.BuscarPorGrupoPessoas(codigoGrupoPessoas);

                var retorno = registros.Select(o => new { o.Codigo, Descricao = o.DescricaoTipoCargaEmbarcador }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os tipos de carga do embarcador.");
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
                grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Carga do Embarcador", "DescricaoTipoCargaEmbarcador", 45, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga repGrupoPessoasTipoCargaEmbarcadorTipoCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga(unitOfWork);

                int countRegistros = repGrupoPessoasTipoCargaEmbarcadorTipoCarga.ContarConsulta(codigoGrupoPessoas);
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga> registros = countRegistros > 0 ? repGrupoPessoasTipoCargaEmbarcadorTipoCarga.Consultar(codigoGrupoPessoas, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga>();

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows((from obj in registros
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.TipoCargaEmbarcador.DescricaoTipoCargaEmbarcador,
                                       TipoCarga = obj.TipoCarga.Descricao ?? string.Empty
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os tipos de cargas configurados.");
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

                Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga repGrupoPessoasTipoCargaEmbarcadorTipoCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga tipoCarga = repGrupoPessoasTipoCargaEmbarcadorTipoCarga.BuscarPorCodigo(codigo, false);

                return new JsonpResult(new
                {
                    tipoCarga.Codigo,
                    TipoCarga = new { tipoCarga.TipoCarga.Codigo, tipoCarga.TipoCarga.Descricao },
                    TipoCargaEmbarcador = tipoCarga.TipoCargaEmbarcador.Codigo
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o tipo de carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarTiposCargaEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");

                string url = Request.GetStringParam("URLIntegracaoMultiEmbarcador");
                string token = Request.GetStringParam("TokenIntegracaoMultiEmbarcador");

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);

                Servicos.Embarcador.Pessoa.GrupoPessoaTipoCargaEmbarcador.BuscarTiposCargaEmbarcador(out string erro, grupoPessoas, url, token, unitOfWork, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int codigoTipoCarga = Request.GetIntParam("TipoCarga");
                int codigoTipoCargaEmbarcador = Request.GetIntParam("TipoCargaEmbarcador");

                Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador repGrupoPessoaTipoCargaEmbarcador = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga repGrupoPessoasTipoCargaEmbarcadorTipoCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador grupoPessoasTipoCargaEmbarcador = repGrupoPessoaTipoCargaEmbarcador.BuscarPorCodigo(codigoTipoCargaEmbarcador, false);

                if (grupoPessoasTipoCargaEmbarcador == null)
                    return new JsonpResult(false, true, "O tipo de carga do embarcador não foi encontrado.");

                if (repGrupoPessoasTipoCargaEmbarcadorTipoCarga.ExistePorGrupoPessoasETipoCarga(grupoPessoasTipoCargaEmbarcador.GrupoPessoas.Codigo, codigoTipoCargaEmbarcador))
                    return new JsonpResult(false, true, "Já existe uma configuração para este tipo de carga.");

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga grupoPessoasTipoCargaEmbarcadorTipoCarga = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga()
                {
                    TipoCarga = repTipoDeCarga.BuscarPorCodigo(codigoTipoCarga),
                    TipoCargaEmbarcador = grupoPessoasTipoCargaEmbarcador
                };

                repGrupoPessoasTipoCargaEmbarcadorTipoCarga.Inserir(grupoPessoasTipoCargaEmbarcadorTipoCarga, Auditado);

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
                int codigoTipoCarga = Request.GetIntParam("TipoCarga");
                int codigoTipoCargaEmbarcador = Request.GetIntParam("TipoCargaEmbarcador");

                Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador repGrupoPessoasTipoCargaEmbarcador = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga repGrupoPessoasTipoCargaEmbarcadorTipoCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador grupoPessoasTipoCargaEmbarcador = repGrupoPessoasTipoCargaEmbarcador.BuscarPorCodigo(codigoTipoCargaEmbarcador, false);

                if (grupoPessoasTipoCargaEmbarcador == null)
                    return new JsonpResult(false, true, "O tipo de carga do embarcador não foi encontrado.");

                if (repGrupoPessoasTipoCargaEmbarcadorTipoCarga.ExistePorGrupoPessoasETipoCarga(grupoPessoasTipoCargaEmbarcador.GrupoPessoas.Codigo, codigoTipoCargaEmbarcador))
                    return new JsonpResult(false, true, "Já existe uma configuração para este tipo de carga.");

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga grupoPessoasTipoCargaEmbarcadorTipoCarga = repGrupoPessoasTipoCargaEmbarcadorTipoCarga.BuscarPorCodigo(codigo, true);

                if (grupoPessoasTipoCargaEmbarcadorTipoCarga == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                grupoPessoasTipoCargaEmbarcadorTipoCarga.TipoCarga = repTipoCarga.BuscarPorCodigo(codigoTipoCarga);
                grupoPessoasTipoCargaEmbarcadorTipoCarga.TipoCargaEmbarcador = grupoPessoasTipoCargaEmbarcador;

                repGrupoPessoasTipoCargaEmbarcadorTipoCarga.Atualizar(grupoPessoasTipoCargaEmbarcadorTipoCarga, Auditado);

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
