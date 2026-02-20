using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Bidding.Bidding
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Bidding/AutorizacaoBidding")]
    public class AutorizacaoBiddingController : RegraAutorizacao.AutorizacaoController<
            Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite,
            Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding,
            Dominio.Entidades.Embarcador.Bidding.BiddingConvite
      >
    {
		#region Construtores

		public AutorizacaoBiddingController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                decimal spendTotal = 0;

                Repositorio.Embarcador.Bidding.Baseline repositorioBaseline = new(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingConvite repositorioBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingTransportadorRota repositorioTransportadorRota = new Repositorio.Embarcador.Bidding.BiddingTransportadorRota(unitOfWork);
                Repositorio.Embarcador.Bidding.BiddingOfertaRota repositorioBiddingOfertaRota = new Repositorio.Embarcador.Bidding.BiddingOfertaRota(unitOfWork);

                Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite = repositorioBiddingConvite.BuscarPorCodigo(codigo, false);

                if (biddingConvite == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Bidding.Baseline> baselines = repositorioBaseline.BuscarPorBiddingConvite(biddingConvite.Codigo);
                List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> rotas = repositorioBiddingOfertaRota.BuscarRotasPorBidding(biddingConvite);

                foreach (var rota in rotas)
                {
                    decimal valorTotalBaselineRota = baselines
                       .Where(baseline => baseline.BiddingOfertaRota.Codigo == rota.Codigo)
                       .Select(baseline => baseline.Valor)
                       .Sum();


                    decimal spend = valorTotalBaselineRota * rota.QuantidadeViagensPorAno;

                    spendTotal += spend;
                }

                return new JsonpResult(new
                {
                    biddingConvite.Codigo,
                    Data = biddingConvite.DataInicio.ToString("dd/MM/yyyy"),
                    Numero = biddingConvite.Codigo,
                    biddingConvite.Status,
                    SituacaoDescricao = biddingConvite.Situacao.ObterDescricao(),
                    TotalSpendBidding = spendTotal.ToString("n2")
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

        #endregion Métodos Globais Sobrescritos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingAprovacao ObterFiltrosPesquisa()
        {
            List<StatusBiddingConvite> listaSituacoes = new List<StatusBiddingConvite>();

            return new Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingAprovacao()
            {
                CpfCnpjFornecedor = Request.GetDoubleParam("Fornecedor"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataLimite = Request.GetDateTimeParam("DataLimite"),
                Numero = Request.GetIntParam("Numero"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0,
                CodigoSolicitante = Request.GetIntParam("Solicitante"),
                CodigoTipoBidding = Request.GetIntParam("TipoBidding")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("Data"))
                propriedadeOrdenar = "DataInicio";

            return propriedadeOrdenar;
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Bidding.BiddingConvite origem)
        {
            return origem.Status == StatusBiddingConvite.Aguardando;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Bidding.BiddingConvite> biddingConvites;
            bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                dynamic listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite repositorioAprovacaoAlcada = new Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite(unitOfWork);

                biddingConvites = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    biddingConvites.Remove(new Dominio.Entidades.Embarcador.Bidding.BiddingConvite() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Bidding.BiddingConvite repositorioBidding = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                biddingConvites = new List<Dominio.Entidades.Embarcador.Bidding.BiddingConvite>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    int codigoItem = (int)itemSelecionado.Codigo;
                    biddingConvites.Add(repositorioBidding.BuscarPorCodigo(codigoItem, false));
                }
            }

            return (from obj in biddingConvites select obj.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número", propriedade: "Codigo", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Descrição", propriedade: "Descricao", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Solicitante", propriedade: "Usuario", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tipo de Bidding", propriedade: "TipoBidding", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data Limite", propriedade: "Data", tamanho: 5, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite repositorio = new Repositorio.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Bidding.BiddingConvite> biddingConvites = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Bidding.BiddingConvite>();

                var lista = (
                    from obj in biddingConvites
                    select new
                    {
                        obj.Codigo,
                        Descricao = obj.Descricao,
                        Data = obj.DataLimite.ToString("dd/MM/yyyy"),
                        Situacao = obj.Status.ObterDescricao(),
                        TipoBidding = obj.TipoBidding?.Descricao ?? string.Empty,
                        Usuario = obj.Solicitante?.Nome ?? string.Empty,
                        DT_RowColor =
                                        obj.Status == StatusBiddingConvite.Aguardando ? CorGrid.Success :
                                        obj.Status == StatusBiddingConvite.Fechamento ? CorGrid.Success :
                                        obj.Status == StatusBiddingConvite.AguardandoAprovacao ? CorGrid.Amarelo :
                                        obj.Status == StatusBiddingConvite.AprovacaoRejeitada ? CorGrid.Vermelho : CorGrid.Branco
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Bidding.BiddingConvite origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Status != StatusBiddingConvite.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Bidding.BiddingConvite repositorioBiddingConvite = new Repositorio.Embarcador.Bidding.BiddingConvite(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                Servicos.Embarcador.Bidding.BiddingConviteAprovacao servicoBiddingConvite = new Servicos.Embarcador.Bidding.BiddingConviteAprovacao(unitOfWork);

                if (servicoBiddingConvite.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                {
                    origem.Status = StatusBiddingConvite.Aguardando;
                    repositorioBiddingConvite.Atualizar(origem);
                }
            }
            else
            {
                origem.Status = StatusBiddingConvite.AprovacaoRejeitada;

                repositorioBiddingConvite.Atualizar(origem);
            }
        }

        #endregion  Métodos Protegidos Sobrescritos
    }
}