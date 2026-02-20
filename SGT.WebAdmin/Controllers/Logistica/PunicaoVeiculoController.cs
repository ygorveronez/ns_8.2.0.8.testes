using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/PunicaoVeiculo")]
    public class PunicaoVeiculoController : BaseController
    {
		#region Construtores

		public PunicaoVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var punicao = new Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo();

                PreencherPunicao(punicao, unitOfWork);

                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Logistica.PunicaoVeiculo(unitOfWork);

                repositorio.Inserir(punicao, Auditado);

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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Logistica.PunicaoVeiculo(unitOfWork);
                var punicao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (punicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherPunicao(punicao, unitOfWork);

                unitOfWork.Start();

                repositorio.Atualizar(punicao, Auditado);

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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Logistica.PunicaoVeiculo(unitOfWork);
                var punicao = repositorio.BuscarPorCodigo(codigo);

                if (punicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    punicao.Codigo,
                    punicao.Ativo,
                    Veiculo = new { punicao.Veiculo.Codigo, punicao.Veiculo.Descricao },
                    Motivo = new { punicao.Motivo.Codigo, punicao.Motivo.Descricao },
                    DataInicioPunicao = punicao.DataInicioPunicao.ToString("dd/MM/yyyy HH:mm"),
                    punicao.DiasPunicao,
                    punicao.Observacao
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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Logistica.PunicaoVeiculo(unitOfWork);
                var punicao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (punicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(punicao, Auditado);

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

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherPunicao(Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo punicao, Repositorio.UnitOfWork unitOfWork)
        {
            var codigoVeiculo = Request.GetNullableIntParam("Veiculo") ?? throw new ControllerException("O veiculo deve ser informado.");
            var repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            var veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo) ?? throw new ControllerException("Veículo não encontrado.");
            var codigoMotivoPunicao = Request.GetNullableIntParam("Motivo") ?? throw new ControllerException("O motivo da punição deve ser informado.");
            var repositorioMotivoPunicao = new Repositorio.Embarcador.Logistica.MotivoPunicaoVeiculo(unitOfWork);
            var motivo = repositorioMotivoPunicao.BuscarPorCodigo(codigoMotivoPunicao) ?? throw new ControllerException("Motivo da punição não encontrado.");
            var dataInicioPunicao = Request.GetNullableDateTimeParam("DataInicioPunicao") ?? throw new ControllerException("A data de início da punição deve ser informada.");
            var diasPunicao = Request.GetIntParam("DiasPunicao");
            var observacao = Request.Params("Observacao");

            if (observacao.Length > 2000)
                throw new ControllerException("Observação não pode passar de 2000 caracteres.");

            var dataFimPunicao = dataInicioPunicao.AddDays(diasPunicao);

            if (dataFimPunicao.CompareTo(DateTime.Now) < 0)
                throw new ControllerException("o período de punição não pode estar encerrado.");

            punicao.Ativo = Request.GetBoolParam("Ativo");
            punicao.DataInicioPunicao = dataInicioPunicao;
            punicao.DiasPunicao = diasPunicao;
            punicao.Motivo = motivo;
            punicao.Observacao = observacao;
            punicao.Veiculo = veiculo;

            ValidarVeiculoFilaCarregamento(punicao, unitOfWork);
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var situacaoAtivo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo);
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Motivo", "Motivo", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Início Punição", "DataInicioPunicao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Dias Punição", "DiasPunicao", 12, Models.Grid.Align.center, false);

                if (situacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 12, Models.Grid.Align.center, true);

                var codigoVeiculo = Request.GetIntParam("Veiculo");
                var codigoMotivoPunicao = Request.GetIntParam("Motivo");
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var repositorio = new Repositorio.Embarcador.Logistica.PunicaoVeiculo(unitOfWork);
                var listaPunicao = repositorio.Consultar(codigoVeiculo, codigoMotivoPunicao, situacaoAtivo, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                var totalRegistros = repositorio.ContarConsulta(codigoVeiculo, codigoMotivoPunicao, situacaoAtivo);

                var listaPunicaoRetornar = (
                    from punicao in listaPunicao
                    select new
                    {
                        punicao.Codigo,
                        punicao.DescricaoAtivo,
                        Veiculo = punicao.Veiculo.Descricao,
                        Motivo = punicao.Motivo.Descricao,
                        DataInicioPunicao = punicao.DataInicioPunicao.ToString("dd/MM/yyyy HH:mm"),
                        punicao.DiasPunicao
                    }
                ).ToList();

                grid.AdicionaRows(listaPunicaoRetornar);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Veiculo")
                return "Veiculo.Descricao";

            if (propriedadeOrdenar == "Motivo")
                return "Motivo.Descricao";

            return propriedadeOrdenar;
        }

        private void ValidarVeiculoFilaCarregamento(Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo punicao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(TipoServicoMultisoftware));
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculo(punicao.Veiculo.Codigo, codigoMotoristaDesconsiderar: 0);

            if (filaCarregamentoVeiculo != null)
                throw new ControllerException($"O veiculo {punicao.Veiculo.Placa_Formatada} está na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}. Remova o veículo da fila para poder {(punicao.IsInitialized() ? "atualizar" : "adicionar")} a punição");
        }

        #endregion
    }
}
