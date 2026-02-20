using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ControleCarregamento")]
    public class ControleCarregamentoController : BaseController
    {
		#region Construtores

		public ControleCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ChegadaDoca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoControleCarregamento = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.ControleCarregamento servicoControleCarregamento = new Servicos.Embarcador.Logistica.ControleCarregamento(unitOfWork);

                servicoControleCarregamento.ChegadaDoca(codigoControleCarregamento);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar a chegada na doca.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoControleCarregamento = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.ControleCarregamento servicoControleCarregamento = new Servicos.Embarcador.Logistica.ControleCarregamento(unitOfWork);

                servicoControleCarregamento.Finalizar(codigoControleCarregamento);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar o início do carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> IniciarCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoControleCarregamento = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.ControleCarregamento servicoControleCarregamento = new Servicos.Embarcador.Logistica.ControleCarregamento(unitOfWork);

                servicoControleCarregamento.Iniciar(codigoControleCarregamento);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar o início do carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
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

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento()
            {
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Situacoes = Request.GetListEnumParam<SituacaoControleCarregamento>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Data de Carregamento", "DataCarregamento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "CodigoEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.center, true, false, false, false, true);
                grid.AdicionarCabecalho("Local", "Local", 10, Models.Grid.Align.center, true, false, false, false, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.ControleCarregamento repositorioControleCarregamento = new Repositorio.Embarcador.Logistica.ControleCarregamento(unitOfWork);
                int totalRegistros = repositorioControleCarregamento.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.ControleCarregamento> listaControleCarregamento = totalRegistros > 0 ? repositorioControleCarregamento.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.ControleCarregamento>();

                var listaControleCarregamentoRetornar = (
                    from controleCarregamento in listaControleCarregamento
                    select new
                    {
                        controleCarregamento.Codigo,
                        controleCarregamento.Situacao,
                        CodigoEmbarcador = controleCarregamento.JanelaCarregamento.Carga?.CodigoCargaEmbarcador ?? controleCarregamento.JanelaCarregamento.PreCarga?.NumeroPreCarga,
                        DataCarregamento = controleCarregamento.JanelaCarregamento.DataCarregamentoProgramada.ToString("dd/MM/yyyy HH:mm"),
                        DescricaoSituacao = controleCarregamento.Situacao.ObterDescricao(),
                        Local = controleCarregamento.JanelaCarregamento.PreCarga?.LocalCarregamento?.DescricaoAcao,
                        Motorista = controleCarregamento.JanelaCarregamento.CargaBase.ListaMotorista?.FirstOrDefault()?.Nome,
                        Placa = controleCarregamento.JanelaCarregamento.CargaBase.RetornarPlacas,
                        Transportador = controleCarregamento.JanelaCarregamento.CargaBase.Empresa?.Descricao,
                        DT_RowColor = controleCarregamento.Situacao.ObterCorLinha(),
                        DT_FontColor = controleCarregamento.Situacao.ObterCorFonte()
                    }
                ).ToList();

                grid.AdicionaRows(listaControleCarregamentoRetornar);
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

        private string ObterPlacas(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            List<string> placas = new List<string>();

            if (veiculo != null)
                placas.Add(veiculo.Placa);

            if (veiculosVinculados != null)
                placas.AddRange(veiculosVinculados.Select(o => o.Placa));

            return string.Join(", ", placas);
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        #endregion
    }
}
