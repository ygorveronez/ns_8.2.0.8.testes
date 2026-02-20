using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Pallets/AutorizacaoAvariaPallet")]
    public class AutorizacaoAvariaPalletController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria,
        Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.RegraAutorizacaoAvaria,
        Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet
    >
    {
		#region Construtores

		public AutorizacaoAvariaPalletController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.AvariaPallet(unitOfWork);
                var avaria = repositorio.BuscarPorCodigo(codigo);

                if (avaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    avaria.Codigo,
                    Data = avaria.Data.ToString("dd/MM/yyyy"),
                    Filial = avaria.Filial?.Descricao,
                    MotivoAvaria = avaria.MotivoAvaria.Descricao,
                    Numero = avaria.Codigo,
                    Setor = avaria.Setor?.Descricao,
                    Transportador = avaria.Transportador?.Descricao,
                    avaria.Situacao,
                    SituacaoDescricao = avaria.Situacao.ObterDescricao(),
                    Solicitante = avaria.Solicitante.Descricao,
                    QuantidadesAvariadas = (
                        from quantidadeAvariada in avaria.QuantidadesAvariadas
                        select new
                        {
                            quantidadeAvariada.SituacaoDevolucaoPallet.Descricao,
                            quantidadeAvariada.Quantidade,
                            Valor = (quantidadeAvariada.SituacaoDevolucaoPallet.ValorUnitario * quantidadeAvariada.Quantidade)
                        }
                    ).ToList()
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
            if (grid.header[grid.indiceColunaOrdena].data == "Numero")
                return "Codigo";

            if (grid.header[grid.indiceColunaOrdena].data == "Filial")
                return "Filial.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "MotivoAvaria")
                return "MotivoAvaria.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "Setor")
                return "Setor.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "Transportador")
                return "Transportador.Descricao";

            return grid.header[grid.indiceColunaOrdena].data;
        }

        private List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> Pesquisar(ref int totalRegistros, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAvariaAprovacao()
            {
                Codigo = Request.GetIntParam("Codigo"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoMotivoAvaria = Request.GetIntParam("MotivoAvaria"),
                CodigoSetor = Request.GetIntParam("Setor"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Situacao = Request.GetEnumParam<SituacaoAvariaPallet>("Situacao")
            };
            var repositorio = new Repositorio.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria(unitOfWork);
            var avarias = repositorio.Consultar(filtrosPesquisa, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

            return avarias;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet origem)
        {
            return origem.Situacao == SituacaoAvariaPallet.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet> avarias;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                int totalRegistros = 0;
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));

                avarias = Pesquisar(ref totalRegistros, propriedadeOrdenar: "Codigo", direcaoOrdenacao: "", inicioRegistros: 0, maximoRegistros: 0, unitOfWork: unitOfWork);

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    avarias.Remove(new Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                var repositorioAvaria = new Repositorio.Embarcador.Pallets.AvariaPallet(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                avarias = new List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    avarias.Add(repositorioAvaria.BuscarPorCodigo((int)itemSelecionado.Codigo));
                }
            }

            return (from avaria in avarias select avaria.Codigo).ToList();
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

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(descricao: "Empresa/Filial", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                else
                {
                    grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Setor", propriedade: "Setor", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                }

                grid.AdicionarCabecalho(descricao: "Motivo Avaria", propriedade: "MotivoAvaria", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
        
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid);
                int totalRegistros = 0;

                var avarias = Pesquisar(ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (
                    from avaria in avarias
                    select new
                    {
                        avaria.Codigo,
                        Data = avaria.Data.ToString("dd/MM/yyyy"),
                        Filial = avaria.Filial?.Descricao,
                        Transportador = avaria.Transportador?.Descricao,
                        MotivoAvaria = avaria.MotivoAvaria.Descricao,
                        Numero = avaria.Codigo,
                        Setor = avaria.Setor?.Descricao,
                        Situacao = avaria.Situacao.ObterDescricao()
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao == SituacaoAvariaPallet.AguardandoAprovacao)
            {
                var situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

                if (situacaoRegrasAutorizacao != SituacaoRegrasAutorizacao.Aguardando)
                {
                    if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
                    {
                        if (new Servicos.Embarcador.Pallets.Avaria(unitOfWork).LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                        {
                            origem.Situacao = SituacaoAvariaPallet.Finalizada;

                            var servicoAvaria = new Servicos.Embarcador.Pallets.Avaria(unitOfWork);

                            servicoAvaria.InserirMovimentacaoEstoque(origem, TipoServicoMultisoftware);
                        }
                    }
                    else
                        origem.Situacao = SituacaoAvariaPallet.AprovacaoRejeitada;

                    var repositorioAvaria = new Repositorio.Embarcador.Pallets.AvariaPallet(unitOfWork);

                    repositorioAvaria.Atualizar(origem);

                    if (origem.Situacao == SituacaoAvariaPallet.Finalizada)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, $"Avaria finalizada", unitOfWork);
                }
            }
        }

        #endregion
    }
}