using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "PesquisaExtrato", "ObterSaldoPorRotatividade" }, "Pallets/AjusteSaldo")]
    public class AjusteSaldoController : BaseController
    {
		#region Construtores

		public AjusteSaldoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, false, "Não é possível ajustar o saldo de pallets para este tipo de empresa.");

                Servicos.Embarcador.Pallets.EstoquePallet servicoEstoquePallet = new Servicos.Embarcador.Pallets.EstoquePallet(unitOfWork, Auditado);

                Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                {
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoSetor = Request.GetIntParam("Setor"),
                    CodigoTransportador = Request.GetIntParam("Transportador"),
                    CpfCnpjCliente = Request.GetDoubleParam("Cliente"),
                    Observacao = Request.Params("Observacao"),
                    Quantidade = Request.GetIntParam("Quantidade"),
                    TipoLancamento = TipoLancamento.Manual,
                    TipoOperacaoMovimentacao = Request.GetEnumParam<TipoOperacaoMovimentacaoEstoquePallet>("TipoOperacaoMovimentacao")
                };

                unitOfWork.Start();

                servicoEstoquePallet.InserirMovimentacao(dadosMovimentacaoEstoque);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar o ajuste.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaExtrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Número do Documento", "NumeroDocumento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Número da Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Filial", "Filial", 16, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo de Lançamento", "TipoLancamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 32, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Entrada", "Entrada", 12, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Saída", "Saida", 12, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Saldo Total", "SaldoTotal", 12, Models.Grid.Align.right, false);

                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;
                Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Pallets.EstoquePallet repositorioEstoque = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);
                int totalRegistros = repositorioEstoque.ContarConsultaTransportador(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> movimentacoesEstoquePalletsTransportador = (totalRegistros > 0) ? repositorioEstoque.ConsultarTransportador(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows((
                    from movimentacao in movimentacoesEstoquePalletsTransportador
                    select new
                    {
                        movimentacao.Codigo,
                        Data = movimentacao.Data.ToString("dd/MM/yyyy HH:mm"),
                        NumeroDocumento = movimentacao.Fechamento?.Numero.ToString("n0") ?? string.Empty,
                        CodigoCargaEmbarcador = movimentacao.Devolucao?.CargaPedido?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                        Filial = movimentacao.Filial?.Descricao ?? string.Empty,
                        movimentacao.Observacao,
                        TipoLancamento = movimentacao.ObterTipoLancamento(),
                        Entrada = movimentacao.ObterQuantidadeEntrada(),
                        Saida = movimentacao.ObterQuantidadeSaida(),
                        movimentacao.SaldoTotal
                    }
                ).ToList());

                return new JsonpResult(grid);
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

        public async Task<IActionResult> ObterSaldoPorRotatividade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTransportador = Request.GetIntParam("Transportador");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    codigoTransportador = Empresa.Codigo;

                Repositorio.Embarcador.Pallets.EstoquePallet repositorioEstoque = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo(codigoTransportador);
                int saldo = repositorioEstoque.ObterSaldoTransportadorPorRotatividade(codigoTransportador, empresa.DiasRotatividadePallets);

                return new JsonpResult(new
                {
                    Saldo = saldo,
                    Dias = empresa.DiasRotatividadePallets
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o saldo por rotatividade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo()
            {
                CodigoTransportador = Request.GetIntParam("Transportador"),
                NumeroDocumento = Request.GetIntParam("NumeroDocumento"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoFilial = Request.GetIntParam("Filial"),
                DataMovimentoInicial = Request.GetNullableDateTimeParam("DataMovimentoInicial"),
                DataMovimentoFinal = Request.GetNullableDateTimeParam("DataMovimentoFinal"),
                TipoMovimento = Request.GetNullableEnumParam<TipoMovimentacaoEstoquePallet>("TipoMovimento")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = Empresa.Codigo;

            if (Request.GetBoolParam("RetornarSaldoFilial") && (filtrosPesquisa.CodigoTransportador > 0))
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

                filtrosPesquisa.CodigosTransportador.AddRange(repositorioEmpresa.BuscarCodigosFiliaisVinculadas(filtrosPesquisa.CodigosTransportador));
            }

            return filtrosPesquisa;
        }

        #endregion
    }
}
