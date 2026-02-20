using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/Manobra")]
    public class ManobraController : BaseController
    {
		#region Construtores

		public ManobraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                int codigoLocalDestino = Request.GetIntParam("LocalDestino");
                int codigoManobraAcao = Request.GetIntParam("ManobraAcao");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.AdicionarManobra(codigoCentroCarregamento, codigoVeiculo, codigoManobraAcao, codigoLocalDestino);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a manobra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DispararNotificacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobra = Request.GetIntParam("Manobra");
                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.NotificarManobraAlterada(codigoManobra);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao notificar a alteração de manobra");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobra = Request.GetIntParam("Codigo");
                int codigoLocal = Request.GetIntParam("Local");
                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.FinalizarManobra(codigoManobra, codigoLocal);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover a reserva da manobra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobra = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.ManobraHistorico repositorioHistorico = new Repositorio.Embarcador.Logistica.ManobraHistorico(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.ManobraHistorico> listaHistorico = repositorioHistorico.BuscarPorManobra(codigoManobra);

                var listaHistoricoRetornar = (
                    from historico in listaHistorico
                    select new
                    {
                        DataOrdenar = historico.Data.ToString("yyyyMMddHHmm"),
                        Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                        historico.Descricao,
                        Usuario = historico.Usuario?.Descricao
                    }
                ).ToList();

                return new JsonpResult(listaHistoricoRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o histórico da manobra.");
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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as manobras.");
            }
        }

        public async Task<IActionResult> Remover()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobra = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.RemoverManobra(codigoManobra);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover a manobra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverManobraTracaoVinculada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobra = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.RemoverManobraTracaoVinculada(codigoManobra);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover a manobra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverReserva()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobra = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.RemoverReservaManobra(codigoManobra);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover a reserva da manobra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool IsManobraEmAtraso(Dominio.Entidades.Embarcador.Logistica.Manobra manobra, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao> listaCentroCarregamentoManobraAcao)
        {
            if (manobra.Situacao != SituacaoManobra.AguardandoManobra)
                return false;

            int tempoToleranciaInicioManobra = (
                from centroCarregamentoManobraAcao in listaCentroCarregamentoManobraAcao
                where centroCarregamentoManobraAcao.Acao.Codigo == manobra.Acao.Codigo
                select centroCarregamentoManobraAcao.TempoToleranciaInicioManobra
            ).FirstOrDefault() ?? 0;

            if (tempoToleranciaInicioManobra == 0)
                return false;

            return manobra.DataCriacao.AddMinutes(tempoToleranciaInicioManobra) <= DateTime.Now;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobra ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobra()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Situacoes = Request.GetNullableListParam<SituacaoManobra>("Situacao")
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
                grid.AdicionarCabecalho("DataCriacao", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Tração", "Tracao", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Reboques", "Reboques", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Modelo", "ModeloVeicularCarga", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 9, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tempo", "Tempo", 9, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Ação", "Acao", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Local Atual", "LocalAtual", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", "Destino", 9, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobra filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamentoManobraAcao repositorioCentroCarregamentoManobraAcao = new Repositorio.Embarcador.Logistica.CentroCarregamentoManobraAcao(unitOfWork);
                int totalRegistros = repositorioManobra.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.Manobra> listaManobra = totalRegistros > 0 ? repositorioManobra.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.Manobra>();
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao> listaCentroCarregamentoManobraAcao = totalRegistros > 0 ? repositorioCentroCarregamentoManobraAcao.BuscarPorCentroCarregamento(filtrosPesquisa.CodigoCentroCarregamento) : new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao>();

                var listaMotivoRetornar = (
                    from manobra in listaManobra
                    select new
                    {
                        manobra.Codigo,
                        manobra.DataCriacao,
                        DescricaoSituacao = IsManobraEmAtraso(manobra, listaCentroCarregamentoManobraAcao) ? "Em Atraso" : manobra.Situacao.ObterDescricao(),
                        Tracao = manobra.Tracao?.Placa_Formatada,
                        Reboques = string.Join(", ", (from reboque in manobra.Reboques select reboque.Placa_Formatada)),
                        ModeloVeicularCarga = manobra.ObterModeloVeicularCarga()?.Descricao,
                        manobra.Situacao,
                        Transportador = manobra.Empresa?.Descricao,
                        Tempo = manobra.ObterTempo(),
                        Acao = manobra.Acao.Descricao,
                        LocalAtual = manobra.ObterLocalAtual()?.DescricaoAcao ?? "",
                        Destino = manobra.LocalDestino?.DescricaoAcao ?? "",
                        DT_FontColor = manobra.Situacao.ObterCorFonte(),
                        DT_RowColor = IsManobraEmAtraso(manobra, listaCentroCarregamentoManobraAcao) ? "#df80ff" : manobra.Situacao.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoRetornar);
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
