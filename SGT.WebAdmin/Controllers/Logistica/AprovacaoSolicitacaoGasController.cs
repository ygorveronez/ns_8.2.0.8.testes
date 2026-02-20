using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Logistica/AprovacaoSolicitacaoGas")]
    public class AprovacaoSolicitacaoGasController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas,
        Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas,
        Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas
    >
    {
		#region Construtores

		public AprovacaoSolicitacaoGasController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorio = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoGas = repositorio.BuscarPorCodigo(codigo);

                if (solicitacaoGas == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    DataBase = solicitacaoGas.DataMedicao.ToString("dd/MM/yyyy"),
                    Base = solicitacaoGas.ClienteBase?.Descricao ?? "",
                    SituacaoDescricao = solicitacaoGas.Situacao.ObterDescricao(),
                    VolumeRodoviario = $"{solicitacaoGas.VolumeRodoviarioCarregamentoProximoDiaTotal.ToString("n3")} ton",
                    DisponibilidadeTransferencia = $"{solicitacaoGas.DisponibilidadeTransferenciaProximoDiaTotal.ToString("n3")} ton"
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

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAprovacaoSolicitacaoAbastecimentoGas ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAprovacaoSolicitacaoAbastecimentoGas filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAprovacaoSolicitacaoAbastecimentoGas()
            {
                CodigoBase = Request.GetDoubleParam("Base"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                Situacao = Request.GetNullableEnumParam<SituacaoAprovacaoSolicitacaoGas>("Situacao"),
                DataSolicitacaoInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataSolicitacaoFinal = Request.GetNullableDateTimeParam("DataLimite")
            };
            
            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas origem)
        {
            return origem.Situacao == SituacaoAprovacaoSolicitacaoGas.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas> listaSolicitacaoGas;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAprovacaoSolicitacaoAbastecimentoGas filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas repositorioAprovacaoSolicitacaoGas = new Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas(unitOfWork);

                listaSolicitacaoGas = repositorioAprovacaoSolicitacaoGas.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    listaSolicitacaoGas.Remove(new Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacaoGas = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));
                
                listaSolicitacaoGas = new List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    listaSolicitacaoGas.Add(repositorioSolicitacaoGas.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return (from termoQuitacao in listaSolicitacaoGas select termoQuitacao.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data Base", "DataBase", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data/Hora Criação", "DataHoraCriacao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nome usuário solicitação", "NomeUsuarioSolicitacao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Base", "Base", 20, Models.Grid.Align.center, true);
                
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAprovacaoSolicitacaoAbastecimentoGas filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas repositorio = new Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas> listaSolicitacaoAbastecimentoGas = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();

                var listaSolicitacaoAbastecimentoRetornar = (
                    from solicitacaoGas in listaSolicitacaoAbastecimentoGas
                    select new
                    {
                        solicitacaoGas.Codigo,
                        DataBase = solicitacaoGas.DataCriacao.ToString("dd/MM/yyyy"),
                        DataHoraCriacao = solicitacaoGas.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                        NomeUsuarioSolicitacao = solicitacaoGas.Usuario.Nome,
                        Situacao = solicitacaoGas.Situacao.ObterDescricao(),
                        Base = solicitacaoGas.ClienteBase?.Descricao ?? ""
                    }
                ).ToList();

                grid.AdicionaRows(listaSolicitacaoAbastecimentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao != SituacaoAprovacaoSolicitacaoGas.AguardandoAprovacao)
                return;

            var situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                Servicos.Embarcador.Logistica.SolicitacaoAbastecimentoGas servicoSolicitacaoAbastecimentoGas = new Servicos.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);

                if (servicoSolicitacaoAbastecimentoGas.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    origem.Situacao = SituacaoAprovacaoSolicitacaoGas.Aprovada;
            }
            else
                origem.Situacao = SituacaoAprovacaoSolicitacaoGas.Reprovada;

            Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacaoAbastecimentoGas = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);

            repositorioSolicitacaoAbastecimentoGas.Atualizar(origem);

            if (origem.Situacao == SituacaoAprovacaoSolicitacaoGas.Aprovada)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, $"Solicitação de Gás Aprovada", unitOfWork);
        }

        #endregion
    }
}