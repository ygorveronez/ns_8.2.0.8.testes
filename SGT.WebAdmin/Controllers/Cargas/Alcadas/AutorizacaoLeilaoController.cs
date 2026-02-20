using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Cargas/AutorizacaoLeilao")]
    public class AutorizacaoLeilaoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao,
        Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao,
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento
    >
    {
		#region Construtores

		public AutorizacaoLeilaoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> ReprocessarCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaJanelaCarregamento.SituacaoCotacao != SituacaoCargaJanelaCarregamentoCotacao.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();

                new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacaoAprovacao(unitOfWork).CriarAprovacao(cargaJanelaCarregamento, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = cargaJanelaCarregamento.SituacaoCotacao != SituacaoCargaJanelaCarregamentoCotacao.SemRegraAprovacao });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar o interesse na carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplasCotacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<int> codigos = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao repositorioAprovacao = new Repositorio.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacaoAprovacao servicoAprovacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacaoAprovacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = repositorioAprovacao.BuscarSemRegraAprovacaoPorCodigos(codigos);
                int totalRegrasReprocessadas = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamento)
                {
                    servicoAprovacao.CriarAprovacao(cargaJanelaCarregamento, TipoServicoMultisoftware);

                    if (cargaJanelaCarregamento.SituacaoCotacao != SituacaoCargaJanelaCarregamentoCotacao.SemRegraAprovacao)
                        totalRegrasReprocessadas++;
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegrasReprocessadas = totalRegrasReprocessadas });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar os interesses nas cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(cargaJanelaCarregamento.Carga.Codigo, cargaJanelaCarregamento.TransportadorCotacao, retornarCargasOriginais: false);
                List<int> codigosCargaJanelaCarregamentoTransportador = cargasJanelaCarregamentoTransportador.Select(o => o.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete> componentesFrete = repositorioCargaJanelaCarregamentoTransportadorComponenteFrete.BuscarPorCargasJanelaCarregamentoTransportador(codigosCargaJanelaCarregamentoTransportador);
                bool possuiCargasVinculadas = cargasJanelaCarregamentoTransportador.Count > 1;
                List<dynamic> valoresFrete = new List<dynamic>();
                decimal valorTotalFrete = 0m;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                {
                    valoresFrete.Add(new
                    {
                        DescricaoValor = possuiCargasVinculadas ? $"Valor do Frete ({cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador})" : "Valor do Frete",
                        Valor = cargaJanelaCarregamentoTransportador.ValorFreteTransportador.ToString("n2")
                    });

                    decimal valorComponentesFrete = componentesFrete.Where(o => o.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaCarregamentoTransportador.Codigo).Select(o => o.ValorComponente).Sum();

                    if (!(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento?.BloquearComponentesDeFreteJanelaCarregamentoTransportador ?? true))
                        valoresFrete.Add(new
                        {
                            DescricaoValor = possuiCargasVinculadas ? $"Valor dos Componentes ({cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador})" : "Valor dos Componentes",
                            Valor = valorComponentesFrete.ToString("n2")
                        });

                    valorTotalFrete += (cargaJanelaCarregamentoTransportador.ValorFreteTransportador + valorComponentesFrete);
                }

                valoresFrete.Add(new
                {
                    DescricaoValor = "Valor Total de Frete",
                    Valor = valorTotalFrete.ToString("n2")
                });

                return new JsonpResult(new
                {
                    cargaJanelaCarregamento.Codigo,
                    Situacao = cargaJanelaCarregamento.SituacaoCotacao,
                    SituacaoDescricao = cargaJanelaCarregamento.SituacaoCotacao.ObterDescricao(),
                    Transportador = cargaJanelaCarregamento.TransportadorCotacao.Descricao,
                    ValoresFrete = valoresFrete
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

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaLeilaoAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaLeilaoAprovacao()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                SituacaoCotacao = Request.GetNullableEnumParam<SituacaoCargaJanelaCarregamentoCotacao>("Situacao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CodigoCargaEmbarcador")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Situacao")
                return "SituacaoCotacao";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos SobrescritosS

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            return cargaJanelaCarregamento.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaLeilaoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao repositorioAprovacaoAlcada = new Repositorio.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao(unitOfWork);

                listaCargaJanelaCarregamento = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    listaCargaJanelaCarregamento.Remove(new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                listaCargaJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    listaCargaJanelaCarregamento.Add(repositorioCargaJanelaCarregamento.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return (from cargaJanelaCarregamento in listaCargaJanelaCarregamento select cargaJanelaCarregamento.Codigo).ToList();
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

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número da Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "TransportadorCotacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação da Aprovação", "Situacao", 15, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaLeilaoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao repositorio = new Repositorio.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

                var listaCargaJanelaCarregamentoRetornar = (
                    from cargaJanelaCarregamento in listaCargaJanelaCarregamento
                    select new
                    {
                        cargaJanelaCarregamento.Codigo,
                        CodigoCargaEmbarcador = cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador ?? "",
                        TransportadorCotacao = cargaJanelaCarregamento.TransportadorCotacao.Descricao,
                        Situacao = cargaJanelaCarregamento.SituacaoCotacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaJanelaCarregamentoRetornar);
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaJanelaCarregamento.SituacaoCotacao != SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(cargaJanelaCarregamento.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);


            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacaoAprovacao(unitOfWork).LiberarProximaPrioridadeAprovacao(cargaJanelaCarregamento, TipoServicoMultisoftware))
                {
                    cargaJanelaCarregamento.SituacaoCotacao = SituacaoCargaJanelaCarregamentoCotacao.Aprovada;
                    cargaJanelaCarregamento.ProcessoCotacaoFinalizada = true;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, "Interesse na carga aprovado", unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);
                    Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unitOfWork, configuracaoEmbarcador, null);
                    Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
                    Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                    Servicos.Embarcador.Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Servicos.Embarcador.Hubs.JanelaCarregamento();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(cargaJanelaCarregamento.Carga.Codigo, cargaJanelaCarregamento.TransportadorCotacao, retornarCargasOriginais: true);
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = cargasJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.Codigo == cargaJanelaCarregamento.Codigo).FirstOrDefault();

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                    {
                        if (((cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.PermitirTransportadorInformarValorFrete) || cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CargaLiberadaCotacao) && cargaJanelaCarregamentoTransportador.ValorFreteTransportador > 0)
                            servicoCargaJanelaCarregamentoTransportador.DefinirTransportadorComValorFreteInformado(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware, Usuario);
                        else
                            servicoCargaJanelaCarregamentoTransportador.DefinirTransportadorSemValorFreteInformado(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware, Usuario);

                        servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento);

                    }

                    servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaSemTransportadorEscolhido(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento);
                    servicoCargaJanelaCarregamentoNotificacao.NotificarCotacaoComTransportadorEscolhido(cargaJanelaCarregamentoTransportadorReferencia, cargasJanelaCarregamentoTransportador, TipoServicoMultisoftware);
                }
            }
            else
            {
                cargaJanelaCarregamento.SituacaoCotacao = SituacaoCargaJanelaCarregamentoCotacao.Reprovada;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, "Interesse na carga reprovado", unitOfWork);
            }

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
        }

        #endregion
    }
}