using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos.AlteracaoPedido
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Pedidos/AutorizacaoAlteracaoPedido")]
    public class AutorizacaoAlteracaoPedidoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido,
        Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido,
        Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido
    >
    {
		#region Construtores

		public AutorizacaoAlteracaoPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> RegrasAprovacaoTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Transportador", "Transportador", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador repositorioAprovacaoTransportador = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador> aprovacoes = repositorioAprovacaoTransportador.BuscarPorAlteracaoPedido(codigo);
                var lista = (
                    from aprovacao in aprovacoes
                    select new
                    {
                        Transportador = aprovacao.Transportador.Descricao,
                        Situacao = aprovacao.Situacao.ObterDescricao(),
                        DT_RowColor = aprovacao.Situacao.ObterCorGrid()
                    }
                ).ToList();

                grid.setarQuantidadeTotal(lista.Count());
                grid.AdicionaRows(lista);

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

        #endregion

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorio = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (alteracaoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    alteracaoPedido.Codigo,
                    alteracaoPedido.Pedido.NumeroPedidoEmbarcador,
                    Filial = alteracaoPedido.Pedido.Filial?.Descricao,
                    SituacaoAlteracaoPedido = alteracaoPedido.Situacao,
                    TipoCarga = alteracaoPedido.Pedido.TipoDeCarga?.Descricao,
                    TipoOperacao = alteracaoPedido.Pedido.TipoOperacao?.Descricao,
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

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacao()
            {
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                SituacaoAlteracaoPedido = Request.GetNullableEnumParam<SituacaoAlteracaoPedido>("SituacaoAlteracaoPedido")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Filial")
                return "Pedido.Filial.Descricao";

            if (propriedadeOrdenar == "TipoCarga")
                return "Pedido.TipoDeCarga.Descricao";

            if (propriedadeOrdenar == "TipoOperacao")
                return "Pedido.TipoOperacao.Descricao";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido origem)
        {
            return origem.Situacao == SituacaoAlteracaoPedido.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido> alteracoesPedidos;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido repositorioAprovacaoAlcada = new Repositorio.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido(unitOfWork);

                alteracoesPedidos = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    alteracoesPedidos.Remove(new Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorioAlteracaoPedido = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                alteracoesPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    alteracoesPedidos.Add(repositorioAlteracaoPedido.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return (from alteracaoPedido in alteracoesPedidos select alteracaoPedido.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número do Pedido", propriedade: "NumeroPedidoEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tipo de Carga", propriedade: "TipoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Operação", propriedade: "TipoOperacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação Alteração Pedido", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido repositorio = new Repositorio.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido> alteracoesPedidos = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido>();

                var lista = (
                    from alteracaoPedido in alteracoesPedidos
                    select new
                    {
                        alteracaoPedido.Codigo,
                        alteracaoPedido.Pedido.NumeroPedidoEmbarcador,
                        Filial = alteracaoPedido.Pedido.Filial?.Descricao,
                        Situacao = alteracaoPedido.Situacao.ObterDescricao(),
                        TipoCarga = alteracaoPedido.Pedido.TipoDeCarga?.Descricao,
                        TipoOperacao = alteracaoPedido.Pedido.TipoOperacao?.Descricao
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

        protected override void PreencherDadosRejeicaoAprovacao(Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido aprovacao, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoMotivoRejeicao = Request.GetIntParam("Motivo");
            Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido repositorioMotivoRejeicao = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido motivoRejeicao = repositorioMotivoRejeicao.BuscarPorCodigo(codigoMotivoRejeicao, auditavel: false);

            if (motivoRejeicao == null)
                throw new ControllerException("Motivo é obrigatório.");

            aprovacao.Data = DateTime.Now;
            aprovacao.Situacao = SituacaoAlcadaRegra.Rejeitada;
            aprovacao.Motivo = motivoRejeicao.Descricao;
            aprovacao.OrigemAprovacao.MotivoRejeicao = motivoRejeicao;
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao != SituacaoAlteracaoPedido.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                Servicos.Embarcador.Pedido.AlteracaoPedido servicoAlteracaoPedido = new Servicos.Embarcador.Pedido.AlteracaoPedido(unitOfWork);

                if (servicoAlteracaoPedido.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                {
                    origem.Situacao = SituacaoAlteracaoPedido.AguardandoAprovacaoTransportador;

                    servicoAlteracaoPedido.VerificarAprovacaoTransportador(origem, TipoServicoMultisoftware);
                }
            }
            else
                origem.Situacao = SituacaoAlteracaoPedido.Reprovada;

            Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorioAlteracaoPedido = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(unitOfWork);

            repositorioAlteracaoPedido.Atualizar(origem);
        }

        #endregion
    }
}