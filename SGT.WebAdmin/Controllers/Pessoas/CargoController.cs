using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/Cargo")]
    public class CargoController : BaseController
    {
		#region Construtores

		public CargoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Pessoas.Cargo cargo = new Dominio.Entidades.Embarcador.Pessoas.Cargo();
                Repositorio.Embarcador.Pessoas.Cargo repCargo = new Repositorio.Embarcador.Pessoas.Cargo(unitOfWork);

                PreencherCargo(cargo);

                decimal valorEquivalente = cargo.AdvertenciaEquivalente + cargo.MediaEquivalente + cargo.SinistroEquivalente;
                if (valorEquivalente > 100m && valorEquivalente != 0m)
                    return new JsonpResult(false, true, "A soma dos valores equivalente não devem ser maior que 100.");

                unitOfWork.Start();

                repCargo.Inserir(cargo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                Repositorio.Embarcador.Pessoas.Cargo repositorio = new Repositorio.Embarcador.Pessoas.Cargo(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.Cargo cargo = repositorio.BuscarPorCodigo(codigo);

                if (cargo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherCargo(cargo);

                decimal valorEquivalente = cargo.AdvertenciaEquivalente + cargo.MediaEquivalente + cargo.SinistroEquivalente;
                if (valorEquivalente > 100m && valorEquivalente != 0m)
                    return new JsonpResult(false, true, "A soma dos valores equivalente não devem ser maior que 100.");

                unitOfWork.Start();

                repositorio.Atualizar(cargo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Pessoas.Cargo repositorio = new Repositorio.Embarcador.Pessoas.Cargo(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.Cargo cargo = repositorio.BuscarPorCodigo(codigo);

                if (cargo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    cargo.Codigo,
                    cargo.Descricao,
                    cargo.ValorFaturamento,
                    cargo.ValorBonificacao,
                    cargo.DescricaoAtivo,
                    cargo.ComissaoPadrao,
                    cargo.MediaEquivalente,
                    cargo.SinistroEquivalente,
                    cargo.AdvertenciaEquivalente
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.Cargo repositorio = new Repositorio.Embarcador.Pessoas.Cargo(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.Cargo cargo = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (cargo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(cargo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherCargo(Dominio.Entidades.Embarcador.Pessoas.Cargo cargo)
        {
            cargo.Descricao = Request.GetNullableStringParam("Descricao") ?? throw new ControllerException("A descrição é obrigatória.");
            cargo.ValorFaturamento = Request.GetDecimalParam("ValorFaturamento");
            cargo.ValorBonificacao = Request.GetDecimalParam("ValorBonificacao");
            cargo.Ativo = Request.GetBoolParam("Ativo");

            cargo.ComissaoPadrao = Request.GetDecimalParam("ComissaoPadrao");
            cargo.MediaEquivalente = Request.GetDecimalParam("MediaEquivalente");
            cargo.SinistroEquivalente = Request.GetDecimalParam("SinistroEquivalente");
            cargo.AdvertenciaEquivalente = Request.GetDecimalParam("AdvertenciaEquivalente");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");

                SituacaoAtivoPesquisa? ativo = null;
                SituacaoAtivoPesquisa ativoAux;
                if (Enum.TryParse(Request.Params("Ativo"), out ativoAux))
                    ativo = ativoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cargo.ValorDeFaturamentoMinimo, "ValorFaturamento", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cargo.ValorBonificacao, "ValorBonificacao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoAtivo", 50, Models.Grid.Align.left, true);


                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pessoas.Cargo repositorio = new Repositorio.Embarcador.Pessoas.Cargo(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, ativo);
                List<Dominio.Entidades.Embarcador.Pessoas.Cargo> listaCargo = totalRegistros > 0 ? repositorio.Consultar(descricao, ativo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pessoas.Cargo>();

                var listaCargoRetornar = (
                    from cargo in listaCargo
                    select new
                    {
                        cargo.Codigo,
                        cargo.Descricao,
                        cargo.ValorFaturamento,
                        cargo.ValorBonificacao,
                        cargo.DescricaoAtivo,
                    }
                ).ToList();

                grid.AdicionaRows(listaCargoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
