using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Pallets/AutorizacaoTransferencia")]
    public class AutorizacaoTransferenciaController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AprovacaoAlcadaTransferenciaPallet,
        Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet,
        Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet
    >
    {
		#region Construtores

		public AutorizacaoTransferenciaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
                var transferencia = repositorio.BuscarPorCodigo(codigo);

                if (transferencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    transferencia.Codigo,
                    Data = transferencia.Solicitacao.Data.ToString("dd/MM/yyyy"),
                    Filial = transferencia.Solicitacao.Filial.Descricao,
                    Numero = transferencia.Codigo,
                    transferencia.Solicitacao.Quantidade,
                    Setor = transferencia.Solicitacao.Setor.Descricao,
                    transferencia.Situacao,
                    SituacaoDescricao = transferencia.Situacao.ObterDescricao(),
                    transferencia.Solicitacao.Solicitante,
                    Turno = transferencia.Solicitacao.Turno.Descricao
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(Models.Grid.Grid grid)
        {
            if (grid.header[grid.indiceColunaOrdena].data == "Data")
                return "Solicitacao.Data";

            if (grid.header[grid.indiceColunaOrdena].data == "Numero")
                return "Codigo";

            if (grid.header[grid.indiceColunaOrdena].data == "Quantidade")
                return "Solicitacao.Quantidade";

            if (grid.header[grid.indiceColunaOrdena].data == "Solicitante")
                return "Solicitacao.Solicitante";

            return grid.header[grid.indiceColunaOrdena].data;
        }

        private List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> Pesquisar(ref int totalRegistros, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTransferenciaPalletAprovacao()
            {
                Codigo = Request.GetIntParam("Codigo"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoSetor = Request.GetIntParam("Setor"),
                CodigoTurno = Request.GetIntParam("Turno"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Situacao = Request.GetEnumParam<SituacaoTransferenciaPallet>("Situacao")
            };
            var repositorio = new Repositorio.Embarcador.Pallets.AlcadasTransferenciaPallets.AprovacaoAlcadaTransferenciaPallet(unitOfWork);
            var listaTransferenciaPallet = repositorio.Consultar(filtrosPesquisa, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

            return listaTransferenciaPallet;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet origem)
        {
            return origem.Situacao == SituacaoTransferenciaPallet.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> transferencias;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                int totalRegistros = 0;
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));

                transferencias = Pesquisar(ref totalRegistros, propriedadeOrdenar: "Codigo", direcaoOrdenacao: "", inicioRegistros: 0, maximoRegistros: 0, unitOfWork: unitOfWork);

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    transferencias.Remove(new Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                var repositorioTransferencia = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                transferencias = new List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    transferencias.Add(repositorioTransferencia.BuscarPorCodigo((int)itemSelecionado.Codigo));
                }
            }

            return (from transferencia in transferencias select transferencia.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número", propriedade: "Numero", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data", propriedade: "Data", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situacao", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Quantidade", propriedade: "Quantidade", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Solicitante", propriedade: "Solicitante", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid);
                int totalRegistros = 0;

                var transferencias = Pesquisar(ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (
                    from transferenciaPallet in transferencias
                    select new
                    {
                        transferenciaPallet.Codigo,
                        transferenciaPallet.Solicitacao.Quantidade,
                        transferenciaPallet.Solicitacao.Solicitante,
                        Data = transferenciaPallet.Solicitacao.Data.ToString("dd/MM/yyyy"),
                        Numero = transferenciaPallet.Codigo,
                        Situacao = transferenciaPallet.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(lista);
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao == SituacaoTransferenciaPallet.AguardandoAprovacao)
            {
                var situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

                if (situacaoRegrasAutorizacao != SituacaoRegrasAutorizacao.Aguardando)
                {
                    var repositorioTransferencia = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);

                    if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
                    {
                        Servicos.Embarcador.Pallets.Transferencia servicoTransferencia = new Servicos.Embarcador.Pallets.Transferencia(unitOfWork);

                        if (servicoTransferencia.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                        {
                            origem.Situacao = SituacaoTransferenciaPallet.AguardandoRecebimento;

                            servicoTransferencia.InserirMovimentacaoEstoqueEnvio(origem);
                        }
                    }
                    else
                        origem.Situacao = SituacaoTransferenciaPallet.AprovacaoRejeitada;

                    repositorioTransferencia.Atualizar(origem);
                }
            }
        }

        #endregion
    }
}