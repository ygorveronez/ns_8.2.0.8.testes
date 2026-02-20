using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Pallets/AutorizacaoDevolucaoValePallet")]
    public class AutorizacaoDevolucaoValePalletController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet,
        Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet,
        Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet
    >
    {
		#region Construtores

		public AutorizacaoDevolucaoValePalletController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(unitOfWork);
                var devolucaoValePallet = repositorio.BuscarPorCodigo(codigo);

                if (devolucaoValePallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    devolucaoValePallet.Codigo,
                    Data = devolucaoValePallet.Data.ToString("dd/MM/yyyy"),
                    Filial = devolucaoValePallet.Filial.Descricao,
                    devolucaoValePallet.Numero,
                    NumeroValePallet = devolucaoValePallet.ValePallet.Numero,
                    devolucaoValePallet.QuantidadePallets,
                    Setor = devolucaoValePallet.Setor?.Descricao,
                    devolucaoValePallet.Situacao,
                    SituacaoDescricao = devolucaoValePallet.Situacao.ObterDescricao(),
                    Transportador = devolucaoValePallet.ValePallet.Devolucao.Transportador.Descricao
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

        private void LiberarValePallet(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);

            devolucaoValePallet.ValePallet.Initialize();

            devolucaoValePallet.ValePallet.Situacao = SituacaoValePallet.AgRecolhimento;

            repositorioValePallet.Atualizar(devolucaoValePallet.ValePallet, Auditado);
        }

        private void CancelarValePallet(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);

            devolucaoValePallet.ValePallet.Initialize();

            devolucaoValePallet.ValePallet.Situacao = SituacaoValePallet.Cancelado;
            devolucaoValePallet.ValePallet.DataCancelamento = DateTime.Now;

            repositorioValePallet.Atualizar(devolucaoValePallet.ValePallet, Auditado);
        }

        private string ObterPropriedadeOrdenar(Models.Grid.Grid grid)
        {
            if (grid.header[grid.indiceColunaOrdena].data == "Filial")
                return "Filial.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "NumeroValePallet")
                return "ValePallet.Numero";

            if (grid.header[grid.indiceColunaOrdena].data == "Setor")
                return "Setor.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "Transportador")
                return "ValePallet.Devolucao.Transportador.RazaoSocial";

            return grid.header[grid.indiceColunaOrdena].data;
        }

        private List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> Pesquisar(ref int totalRegistros, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaDevolucaoValePalletAprovacao()
            {
                Codigo = Request.GetIntParam("Codigo"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoSetor = Request.GetIntParam("Setor"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Situacao = Request.GetEnumParam<SituacaoDevolucaoValePallet>("Situacao")
            };
            var repositorio = new Repositorio.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet(unitOfWork);
            var listaDevolucaoValePallet = repositorio.Consultar(filtrosPesquisa, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

            return listaDevolucaoValePallet;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet origem)
        {
            return origem.Situacao == SituacaoDevolucaoValePallet.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> devolucoesValePallet;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                int totalRegistros = 0;
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));

                devolucoesValePallet = Pesquisar(ref totalRegistros, propriedadeOrdenar: "Codigo", direcaoOrdenacao: "", inicioRegistros: 0, maximoRegistros: 0, unitOfWork: unitOfWork);

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    devolucoesValePallet.Remove(new Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                var repositorioDevolucaoValePallet = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                devolucoesValePallet = new List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    devolucoesValePallet.Add(repositorioDevolucaoValePallet.BuscarPorCodigo((int)itemSelecionado.Codigo));
                }
            }

            return (from devolucaoValePallet in devolucoesValePallet select devolucaoValePallet.Codigo).ToList();
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
                grid.AdicionarCabecalho(descricao: "Número", propriedade: "Numero", tamanho: 12, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Número do Vale Pallet", propriedade: "NumeroValePallet", tamanho: 12, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data", propriedade: "Data", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Quantidade", propriedade: "QuantidadePallets", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Setor", propriedade: "Setor", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid);
                int totalRegistros = 0;

                var devolucoesValePallet = Pesquisar(ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (
                    from devolucaoValePallet in devolucoesValePallet
                    select new
                    {
                        devolucaoValePallet.Codigo,
                        Data = devolucaoValePallet.Data.ToString("dd/MM/yyyy"),
                        Filial = devolucaoValePallet.Filial.Descricao,
                        devolucaoValePallet.QuantidadePallets,
                        devolucaoValePallet.Numero,
                        NumeroValePallet = devolucaoValePallet.ValePallet.Numero,
                        Setor = devolucaoValePallet.Setor?.Descricao,
                        Situacao = devolucaoValePallet.Situacao.ObterDescricao(),
                        Transportador = devolucaoValePallet.ValePallet.Devolucao.Transportador.Descricao
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao == SituacaoDevolucaoValePallet.AguardandoAprovacao)
            {
                var situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

                if (situacaoRegrasAutorizacao != SituacaoRegrasAutorizacao.Aguardando)
                {
                    var repositorioDevolucaoValePallet = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(unitOfWork);

                    if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
                    {
                        if (new Servicos.Embarcador.Pallets.DevolucaoValePallet(unitOfWork, Auditado).LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                        {
                            origem.Situacao = SituacaoDevolucaoValePallet.Finalizada;

                            LiberarValePallet(origem, unitOfWork);

                            var servicoDevolucaoValePallet = new Servicos.Embarcador.Pallets.DevolucaoValePallet(unitOfWork, Auditado);

                            servicoDevolucaoValePallet.InserirMovimentacaoEstoque(origem);
                        }
                    }
                    else
                    {
                        origem.Situacao = SituacaoDevolucaoValePallet.AprovacaoRejeitada;

                        CancelarValePallet(origem, unitOfWork);
                    }

                    repositorioDevolucaoValePallet.Atualizar(origem);
                }
            }
        }

        #endregion
    }
}