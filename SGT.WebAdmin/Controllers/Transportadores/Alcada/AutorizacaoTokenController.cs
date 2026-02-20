using Dominio.Entidades.Embarcador.Transportadores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores.Alcada
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Transportadores/AutorizacaoToken")]
    public class AutorizacaoTokenController : RegraAutorizacao.AutorizacaoController<
    Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken,
    Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken,
    Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken
>
    {
		#region Construtores

		public AutorizacaoTokenController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais    

		#endregion

		#region Metodos Publicos

		public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Transportadores.SolicitacaoToken repositorioSolicitacaoToken = new Repositorio.Embarcador.Transportadores.SolicitacaoToken(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacaoToken = repositorioSolicitacaoToken.BuscarPorCodigo(codigo, false);

                if (solicitacaoToken == null)
                    return new JsonpResult(false, false, "Solicitação Token não encontrada.");

                if (solicitacaoToken.Situacao != EtapaAutorizacaoToken.SemRegraAprovacao)
                    return new JsonpResult(false, false, "A situação atual da solicitação não permite esta operação.");

                new Servicos.Embarcador.Transportadores.AutorizacaoToken(unitOfWork).CriarAprovacao(solicitacaoToken, TipoServicoMultisoftware);
                repositorioSolicitacaoToken.Atualizar(solicitacaoToken);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metodos Sobrescritos
        public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Transportadores.SolicitacaoToken RepsolicitacaoToken = new Repositorio.Embarcador.Transportadores.SolicitacaoToken(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken solicitacao = RepsolicitacaoToken.BuscarPorCodigo(codigo, false);

                var Retsolicitacao = new
                {
                    solicitacao.Codigo,
                    EtapaAutorizacao = solicitacao.Situacao.ObterDescricao(),
                    DataInicioVigencia = solicitacao.DataInicioVigencia.ToString("dd/MM/yyyy"),
                    DataFimVigencia = solicitacao.DataFimVigencia.ToString("dd/MM/yyyy"),
                    solicitacao.NumeroProtocolo,
                    MetodosSolicitados = (from obj in solicitacao.PermissoesWS select obj).ToList(),
                    Transportadores = (from obj in solicitacao.Transportadores select new {obj.Codigo, CNPJ = obj.CNPJ_Formatado, Descricao = obj.Descricao}).ToList(),
                };

                return new JsonpResult(Retsolicitacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        protected override bool IsPermitirDelegar(SolicitacaoToken origem)
        {
            return true;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken> aprovacaoSolicitacao = new List<Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken>();
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken filtrosPesquisa = ObterFiltrosPesquisaTokens();
                Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken repositorAlcadaToken = new Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken(unitOfWork);

                aprovacaoSolicitacao = repositorAlcadaToken.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });
                //Rever depois
                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    aprovacaoSolicitacao.Remove(new Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken repositorioAutorizacaoAlcadaToken = new Repositorio.Embarcador.Transportadores.Alcada.AutorizacaoAlcadaToken(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ItensSelecionados"));

                foreach (var itemSelecionado in listaItensSelecionados)
                    aprovacaoSolicitacao.Add(repositorioAutorizacaoAlcadaToken.BuscarPorCodigo((int)itemSelecionado.Codigo, false));
            }

            List<int> codigoOrigems = aprovacaoSolicitacao.Where(x => x.OrigemAprovacao != null).Select(x => x.Codigo).ToList();

            return codigoOrigems;

        }

        protected override Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };


            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken filtrosPesquisa = ObterFiltrosPesquisaTokens();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data Solicitação", "DataSolicitacao", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Data Final da Vigência", "DataFimVigencia", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Número do Protocolo", "NumeroProtocolo", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("CNPJ", "CNPJ", 12, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Etapa de Autorização", "EtapaAutorizacao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Token", "Token", 20, Models.Grid.Align.left, false);

            Repositorio.Embarcador.Transportadores.SolicitacaoToken RepSolicitacaoToken = new Repositorio.Embarcador.Transportadores.SolicitacaoToken(unitOfWork);
            int totalRegistros = RepSolicitacaoToken.ContarConsulta(filtrosPesquisa);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            List<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken> listaSolicitacoes = totalRegistros > 0 ? RepSolicitacaoToken.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken>();

            dynamic listaTokensRetornar = (
                from solicitacoes in listaSolicitacoes
                select new
                {
                    solicitacoes.Codigo,
                    solicitacoes.NumeroProtocolo,
                    DataSolicitacao = solicitacoes.DataCriacao?.ToString("dd/MM/yyyy"),
                    DataFimVigencia = solicitacoes.DataFimVigencia.ToString("dd/MM/yyyy"),
                    Transportador = solicitacoes.Transportadores.Count > 0 && solicitacoes.Transportadores != null ? string.Join(", ", solicitacoes.Transportadores.Select(x => x.RazaoSocial).ToList()) : "",
                    CNPJ = solicitacoes.Transportadores.Count > 0 && solicitacoes.Transportadores != null ? string.Join(", ", solicitacoes.Transportadores.Select(x => x.CNPJ).ToList()) : "",
                    solicitacoes.Observacao,
                    EtapaAutorizacao = solicitacoes.Situacao.ObterDescricao(),
                    Token = solicitacoes.TipoAutenticacao == TipoAutenticacao.Token ? string.Join(", ", solicitacoes.SolicitacoesTokenTransportador.Select(stt => stt.Token)) : string.Join(", ", solicitacoes.SolicitacoesTokenTransportador.Select(stt => $"{stt.Usuario?.Login} - {stt.Usuario?.Senha}" )),
                }
            ).ToList();

            grid.AdicionaRows(listaTokensRetornar);
            grid.setarQuantidadeTotal(totalRegistros);
            return grid;

        }

        protected override void VerificarSituacaoOrigem(SolicitacaoToken origem, UnitOfWork unitOfWork)
        {
            if (origem.Situacao != EtapaAutorizacaoToken.AgAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Transportadores.SolicitacaoToken repositoriSolicitacaoToken = new Repositorio.Embarcador.Transportadores.SolicitacaoToken(unitOfWork);
            Servicos.Embarcador.Transportadores.AutorizacaoToken servicoTransportadorToken = new Servicos.Embarcador.Transportadores.AutorizacaoToken(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (!servicoTransportadorToken.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                origem.Situacao = EtapaAutorizacaoToken.EmLiberacaoSistematica;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Autorização de token aprovada", unitOfWork);
            }
            else
            {
                origem.Situacao = EtapaAutorizacaoToken.SolicitacaoReprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Autorização de token reprovado", unitOfWork);
            }
            repositoriSolicitacaoToken.Atualizar(origem);
        }

        #endregion

        #region Métodos Privados
        private Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken ObterFiltrosPesquisaTokens()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken()
            {
                NumeroProtocolo = Request.GetIntParam("NumeroProtocolo"),
                Descricao = Request.GetStringParam("Descricao"),
                DataInicioVigencia = Request.GetNullableDateTimeParam("DataInicioVigencia"),
                DataFimVigencia = Request.GetNullableDateTimeParam("DataFimVigencia"),
                Usuario = Request.GetIntParam("Usuario"),
                Prioridade = Request.GetIntParam("Prioridade"),
                Situacao = Request.GetEnumParam("Situacao", SituacaoAutorizacaoToken.Todos),
                EtapaAutorizacao = Request.GetEnumParam("EtapaAutorizacao", EtapaAutorizacaoToken.Todos),
            };
        }
        #endregion
    }
}