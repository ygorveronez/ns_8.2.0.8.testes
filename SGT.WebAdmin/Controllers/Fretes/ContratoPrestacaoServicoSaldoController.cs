using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ContratoPrestacaoServicoSaldo")]
    public class ContratoPrestacaoServicoSaldoController : BaseController
    {
		#region Construtores

		public ContratoPrestacaoServicoSaldoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Frete.ContratoPrestacaoServicoSaldo servicoContratoPrestacaoServicoSaldo = new Servicos.Embarcador.Frete.ContratoPrestacaoServicoSaldo(unitOfWork, this.Usuario);
                Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados dados = new Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados()
                {
                    CodigoContratoPrestacaoServico = Request.GetIntParam("ContratoPrestacaoServico"),
                    Descricao = Request.GetStringParam("Descricao"),
                    TipoLancamento = TipoLancamento.Manual,
                    TipoMovimentacao = TipoMovimentacaoContratoPrestacaoServico.Saida,
                    Valor = Request.GetDecimalParam("Valor")
                };

                servicoContratoPrestacaoServicoSaldo.Adicionar(dados);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
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

        public async Task<IActionResult> Remover()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Frete.ContratoPrestacaoServicoSaldo servicoContratoPrestacaoServicoSaldo = new Servicos.Embarcador.Frete.ContratoPrestacaoServicoSaldo(unitOfWork);

                servicoContratoPrestacaoServicoSaldo.Remover(codigo);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico ObterContratoPrestacaoServico(int codigoContratoPrestacaoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorioContratoPrestacaoServico = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);

            return repositorioContratoPrestacaoServico.BuscarPorCodigo(codigoContratoPrestacaoServico) ?? throw new ControllerException("Contrato de prestação de serviço não encontrado");
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoSaldo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoSaldo()
            {
                CodigoContratoPrestacaoServico = Request.GetIntParam("ContratoPrestacaoServico"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                TipoLancamento = Request.GetNullableEnumParam<TipoLancamento>("TipoLancamento")
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
                grid.AdicionarCabecalho("TipoLancamento", false);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo do Lançamento", "TipoLancamentoDescricao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor Entrada", "ValorEntrada", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor Saída", "ValorSaida", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor Disponível", "ValorDisponivel", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoSaldo filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Frete.ContratoPrestacaoServicoSaldo repositorio = new Repositorio.Embarcador.Frete.ContratoPrestacaoServicoSaldo(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo> listaContratoPrestacaoServico = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo>();

                var listaContratoPrestacaoServicoRetornar = (
                    from contratoPrestacaoServico in listaContratoPrestacaoServico
                    select new
                    {
                        contratoPrestacaoServico.Codigo,
                        Data = contratoPrestacaoServico.Data.ToString("dd/MM/yyyy HH:mm"),
                        contratoPrestacaoServico.Descricao,
                        contratoPrestacaoServico.TipoLancamento,
                        TipoLancamentoDescricao = contratoPrestacaoServico.TipoLancamento.ObterDescricao(),
                        ValorEntrada = contratoPrestacaoServico.ObterValorEntrada().ToString("n2"),
                        ValorSaida = contratoPrestacaoServico.ObterValorSaida().ToString("n2"),
                        ValorDisponivel = contratoPrestacaoServico.ObterValorDisponivel().ToString("n2")
                    }
                ).ToList();

                grid.AdicionaRows(listaContratoPrestacaoServicoRetornar);
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
