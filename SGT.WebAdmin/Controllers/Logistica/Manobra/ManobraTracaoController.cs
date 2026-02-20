using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica.Manobra
{
    [CustomAuthorize("Logistica/ManobraTracao")]
    public class ManobraTracaoController : BaseController
    {
		#region Construtores

		public ManobraTracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoTracao = Request.GetIntParam("Tracao");
                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.AdicionarManobraTracao(codigoCentroCarregamento, codigoTracao, codigoMotorista);

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

        public async Task<IActionResult> DispararNotificacaoAlteracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobraTracao = Request.GetIntParam("ManobraTracao");
                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.NotificarManobraTracaoAlterada(codigoManobraTracao);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao notificar a alteração de tração de manobra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DispararNotificacaoRemocao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobraTracao = Request.GetIntParam("ManobraTracao");
                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.NotificarManobraTracaoRemovida(codigoManobraTracao);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao notificar a remoção de tração de manobra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobraTracao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.ManobraTracao repositorio = new Repositorio.Embarcador.Logistica.ManobraTracao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = repositorio.BuscarPorCodigo(codigoManobraTracao);

                if (manobraTracao == null)
                    return new JsonpResult(false, true, "Tração de manobra não encontrada");

                return new JsonpResult(new
                {
                    AcoesRealizadas = "",
                    Placa = manobraTracao.Tracao.Placa_Formatada,
                    Motorista = manobraTracao.Motorista?.Descricao,
                    TempoMedioAcao = "",
                    TempoTotalOcioso = "",
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes da tração de manobra.");
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
                return new JsonpResult(ObterListaManobraTracao());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as trações de manobra.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaHistorico()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaHistorico());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o histórico da tração de manobra.");
            }
        }

        public async Task<IActionResult> Remover()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobra = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.RemoverManobraTracao(codigoManobra);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover a tração de manobra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VincularManobra()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoManobra = Request.GetIntParam("Manobra");
                int codigoManobraTracao= Request.GetIntParam("ManobraTracao");

                Servicos.Embarcador.Logistica.Manobra servicoManobra = new Servicos.Embarcador.Logistica.Manobra(unitOfWork, Auditado);

                servicoManobra.VincularManobraTracao(codigoManobra, codigoManobraTracao);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao vincular a manobra a uma tração de manobra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracao()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento")
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracaoHistorico ObterFiltrosPesquisaHistorico()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracaoHistorico()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                CodigoTracao = Request.GetIntParam("Tracao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisaHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Início", "Inicio", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Fim", "Fim", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tempo", "Tempo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Ação", "Acao", 25, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracaoHistorico filtrosPesquisa = ObterFiltrosPesquisaHistorico();
                List<Dominio.Entidades.Embarcador.Logistica.ManobraTracaoHistorico> listaHistorico = new List<Dominio.Entidades.Embarcador.Logistica.ManobraTracaoHistorico>();
                int totalRegistros = 0;

                if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        DirecaoOrdenar = "desc",
                        PropriedadeOrdenar = "Codigo"
                    };
                    Repositorio.Embarcador.Logistica.ManobraTracaoHistorico repositorio = new Repositorio.Embarcador.Logistica.ManobraTracaoHistorico(unitOfWork);
                    totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                    if (totalRegistros > 0)
                        listaHistorico = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);
                }

                var listaMotivoRetornar = (
                    from historico in listaHistorico
                    select new
                    {
                        historico.Codigo,
                        Placa = historico.Tracao.Placa_Formatada,
                        Motorista = historico.Motorista.Descricao,
                        Inicio = historico.Manobra.DataInicio?.ToString("dd/MM/yyyy HH:mm"),
                        Fim = historico.Manobra.DataFim?.ToString("dd/MM/yyyy HH:mm"),
                        Tempo = historico.Manobra.ObterTempoEmManobra(),
                        Acao = historico.Manobra.Acao.Descricao,
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

        private dynamic ObterListaManobraTracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { DirecaoOrdenar = "desc", PropriedadeOrdenar = "Codigo" };
                Repositorio.Embarcador.Logistica.ManobraTracao repositorio = new Repositorio.Embarcador.Logistica.ManobraTracao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.ManobraTracao> listaManobraTracao = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.ManobraTracao>();

                var listaManobraTracaoRetornar = (
                    from manobraTracao in listaManobraTracao
                    select new
                    {
                        manobraTracao.Codigo,
                        AcaoAtual = manobraTracao.ManobraAtual?.Acao?.Descricao,
                        ClasseCor = manobraTracao.Situacao.ObterClasseCor(),
                        DescricaoSituacao = manobraTracao.Situacao.ObterDescricao(),
                        Motorista = manobraTracao.Motorista?.Descricao,
                        Placa = manobraTracao.Tracao.Placa_Formatada,
                        manobraTracao.Situacao,
                        Tracao = manobraTracao.Tracao.Codigo,
                        Transportador = manobraTracao.Motorista?.Empresa?.Descricao
                    }
                ).ToList();

                return listaManobraTracaoRetornar;
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
