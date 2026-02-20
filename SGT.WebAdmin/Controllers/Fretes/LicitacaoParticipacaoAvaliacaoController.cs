using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/LicitacaoParticipacaoAvaliacao")]
    public class LicitacaoParticipacaoAvaliacaoController : BaseController
    {
		#region Construtores

		public LicitacaoParticipacaoAvaliacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Aprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao = repositorioLicitacaoParticipacao.BuscarPorCodigo(codigo, auditavel: false);

                if (licitacaoParticipacao == null)
                    return new JsonpResult(false, true, $"Não foi possível encontrar a proposta.");

                if (licitacaoParticipacao.Situacao != SituacaoLicitacaoParticipacao.AguardandoRetornoOferta)
                    return new JsonpResult(false, true, $"A situação da proposta não permite aprovação.");

                unitOfWork.Start();

                licitacaoParticipacao.Situacao = SituacaoLicitacaoParticipacao.OfertaAceita;

                repositorioLicitacaoParticipacao.Atualizar(licitacaoParticipacao);

                AtualizarTabelasFreteCliente(unitOfWork, licitacaoParticipacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, licitacaoParticipacao, null, "Oferta para a licitação aceita", unitOfWork);

                unitOfWork.CommitChanges();

                try
                {
                    EnviarNotificacaoUsuarioProposta(unitOfWork, licitacaoParticipacao, string.Format(Localization.Resources.Fretes.LicitacaoParticipacaoAvaliacao.OfertaLicitacaoAceita, licitacaoParticipacao.Licitacao.Numero));
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aceitar a proposta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao> listaLicitacaoParticipacao = ObterListaLicitacaoParticipacaoSelecionadas(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao> listaLicitacaoParticipacaoFiltrada = (from licitacaoParticipacao in listaLicitacaoParticipacao where licitacaoParticipacao.Situacao == SituacaoLicitacaoParticipacao.AguardandoRetornoOferta select licitacaoParticipacao).ToList();
                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao in listaLicitacaoParticipacaoFiltrada)
                {
                    licitacaoParticipacao.Situacao = SituacaoLicitacaoParticipacao.OfertaAceita;

                    repositorioLicitacaoParticipacao.Atualizar(licitacaoParticipacao);

                    AtualizarTabelasFreteCliente(unitOfWork, licitacaoParticipacao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, licitacaoParticipacao, null, "Oferta para a licitação aceita", unitOfWork);
                }

                unitOfWork.CommitChanges();

                try
                {
                    foreach (Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao in listaLicitacaoParticipacaoFiltrada)
                    {
                        EnviarNotificacaoUsuarioProposta(unitOfWork, licitacaoParticipacao, string.Format(Localization.Resources.Fretes.LicitacaoParticipacaoAvaliacao.OfertaLicitacaoAceita, licitacaoParticipacao.Licitacao.Numero));
                    }
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                }

                return new JsonpResult(new { PropostasModificadas = listaLicitacaoParticipacaoFiltrada.Count });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aceitar múltiplas propostas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorLicitacaoParticipacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao = repositorioLicitacaoParticipacao.BuscarPorCodigo(codigo, auditavel: false);

                if (licitacaoParticipacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a proposta.");

                return new JsonpResult(new
                {
                    LicitacaoParticipacao = new {
                        licitacaoParticipacao.Situacao
                    },
                    Resumo = new {
                        licitacaoParticipacao.Licitacao.Numero,
                        licitacaoParticipacao.Licitacao.Descricao,
                        Validade = $"{licitacaoParticipacao.Licitacao.DataInicio.ToString("dd/MM/yyyy")} até {licitacaoParticipacao.Licitacao.DataFim.ToString("dd/MM/yyyy")}",
                        Situacao = licitacaoParticipacao.Situacao.ObterDescricao(),
                        Transportador = licitacaoParticipacao.Transportador.Descricao
                    }
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados da proposta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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

        public async Task<IActionResult> PesquisaOferta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLicitacaoParticipacao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao = repositorioLicitacaoParticipacao.BuscarPorCodigo(codigoLicitacaoParticipacao, auditavel: false);

                if (licitacaoParticipacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a proposta.");

                Models.Grid.Grid grid = ObterGridOferta(licitacaoParticipacao.Licitacao.TabelaFrete);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
                {
                    ParametroBase = licitacaoParticipacao.Licitacao.TabelaFrete.ParametroBase,
                    CodigoTabelaFrete = licitacaoParticipacao.Licitacao.TabelaFrete.Codigo,
                    CodigoLicitacaoParticipacao = codigoLicitacaoParticipacao
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                int totalRegistros = repositorioTabelaFreteCliente.ContarConsulta(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> lista = totalRegistros > 0 ? repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao = repositorioLicitacaoParticipacao.BuscarPorCodigo(codigo, auditavel: false);

                if (licitacaoParticipacao == null)
                    return new JsonpResult(false, true, $"Não foi possível encontrar a proposta.");

                if (licitacaoParticipacao.Situacao != SituacaoLicitacaoParticipacao.AguardandoRetornoOferta)
                    return new JsonpResult(false, true, $"A situação da proposta não permite rejeição.");

                unitOfWork.Start();

                licitacaoParticipacao.Situacao = SituacaoLicitacaoParticipacao.OfertaRecusada;
                licitacaoParticipacao.ObservacaoRetorno = Request.GetNullableStringParam("Observacao");

                repositorioLicitacaoParticipacao.Atualizar(licitacaoParticipacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, licitacaoParticipacao, null, "Oferta para a licitação rejeitada", unitOfWork);

                unitOfWork.CommitChanges();

                try
                {
                    EnviarNotificacaoUsuarioProposta(unitOfWork, licitacaoParticipacao, string.Format(Localization.Resources.Fretes.LicitacaoParticipacaoAvaliacao.OfertaLicitacaoRejeitada, licitacaoParticipacao.Licitacao.Numero));
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar a proposta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarMultiplas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao> listaLicitacaoParticipacao = ObterListaLicitacaoParticipacaoSelecionadas(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao> listaLicitacaoParticipacaoFiltrada = (from licitacaoParticipacao in listaLicitacaoParticipacao where licitacaoParticipacao.Situacao == SituacaoLicitacaoParticipacao.AguardandoRetornoOferta select licitacaoParticipacao).ToList();
                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao in listaLicitacaoParticipacaoFiltrada)
                {
                    licitacaoParticipacao.Situacao = SituacaoLicitacaoParticipacao.OfertaRecusada;

                    repositorioLicitacaoParticipacao.Atualizar(licitacaoParticipacao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, licitacaoParticipacao, null, "Oferta para a licitação rejeitada", unitOfWork);
                }

                unitOfWork.CommitChanges();

                try
                {
                    foreach (Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao in listaLicitacaoParticipacaoFiltrada)
                    {
                        EnviarNotificacaoUsuarioProposta(unitOfWork, licitacaoParticipacao, string.Format(Localization.Resources.Fretes.LicitacaoParticipacaoAvaliacao.OfertaLicitacaoRejeitada, licitacaoParticipacao.Licitacao.Numero));
                    }
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                }

                return new JsonpResult(new { PropostasModificadas = listaLicitacaoParticipacaoFiltrada.Count });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aceitar múltiplas propostas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Públicos

        private void AtualizarTabelasFreteCliente(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFreteCliente = repositorioTabelaFreteCliente.BuscarPorLicitacaoParticipacao(licitacaoParticipacao.Codigo);

            foreach (var tabelaFreteCliente in tabelasFreteCliente)
            {
                tabelaFreteCliente.Tipo = TipoTabelaFreteCliente.Calculo;

                repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);
            }
        }

        private void EnviarNotificacaoUsuarioProposta(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao, string mensagem)
        {
            var servicoNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, cliente: Cliente, tipoServicoMultisoftware: TipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacao(
                usuario: licitacaoParticipacao.Usuario,
                usuarioGerouNotificacao: licitacaoParticipacao.Licitacao.Usuario,
                codigoObjeto: licitacaoParticipacao.Codigo,
                URLPagina: "Fretes/LicitacaoParticipacaoAvaliacao",
                nota: mensagem,
                icone: IconesNotificacao.sucesso,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: TipoServicoMultisoftware,
                unitOfWork: unitOfWork
            );
        }

        private List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao> ObterListaLicitacaoParticipacaoSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao> listaLicitacaoParticipacao;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacaoParticipacao filtrosPesquisa = ObterFiltrosPesquisa();
                listaLicitacaoParticipacao = repositorioLicitacaoParticipacao.ConsultarAprovacao(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    listaLicitacaoParticipacao.Remove(new Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                listaLicitacaoParticipacao = new List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    listaLicitacaoParticipacao.Add(repositorioLicitacaoParticipacao.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return listaLicitacaoParticipacao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacaoParticipacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacaoParticipacao()
            {
                CodigoTabelaFrete = Request.GetIntParam("TabelaFrete"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DescricaoLicitacao = Request.GetNullableStringParam("DescricaoLicitacao"),
                NumeroLicitacao = Request.GetIntParam("NumeroLicitacao"),
                NumeroLicitacaoParticipacao = Request.GetIntParam("NumeroLicitacaoParticipacao"),
                Situacao = Request.GetNullableEnumParam<SituacaoLicitacaoParticipacao>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridOferta(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            int NumeroMaximoComplementos = 15;
            int UltimaColunaDinamica = 1;
            decimal TamanhoColunasValores = 1.75m;
            decimal TamanhoColunasParticipantes = 5.50m;
            decimal TamanhoColunasEnderecoParticipantes = 3m;

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Código", "CodigoIntegracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tabela de Frete", "TabelaFrete", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, true);

            if (tabelaFrete.ParametroBase.HasValue)
                grid.AdicionarCabecalho("Base (" + tabelaFrete.ParametroBase.Value.ObterDescricao() + ")", "ParametroBase", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
            
            if (tabelaFrete.NumeroEntregas.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega))
            {
                grid.AdicionarCabecalho("Entrega", "NumeroEntrega", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Entrega", "DescricaoValorEntrega", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.TipoEmbalagens.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem))
            {
                grid.AdicionarCabecalho("Tipo de Embalagem", "TipoEmbalagem", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo de Embalagem", "DescricaoValorTipoEmbalagem", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.PesosTransportados.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso))
            {
                grid.AdicionarCabecalho("Peso", "DescricaoPeso", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Peso", "DescricaoValorPeso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Distancias.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia))
            {
                grid.AdicionarCabecalho("Distância", "DescricaoDistancia", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Distância", "DescricaoValorDistancia", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.TiposCarga.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga))
            {
                grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo Carga", "DescricaoValorTipoCarga", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.ModelosReboque.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque))
            {
                grid.AdicionarCabecalho("Reboque", "ModeloReboque", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Reboque", "DescricaoValorModeloReboque", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.ModelosTracao.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao))
            {
                grid.AdicionarCabecalho("Tração", "ModeloTracao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tração", "DescricaoValorModeloTracao", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Ajudantes.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante))
            {
                grid.AdicionarCabecalho("Ajudante", "NumeroAjudante", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Ajudante", "DescricaoValorAjudante", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Pallets.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pallets))
            {
                grid.AdicionarCabecalho("Pallet", "NumeroPallets", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Pallet", "DescricaoValorPallets", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Tempos.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo))
            {
                grid.AdicionarCabecalho("Tempo", "HoraTempo", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tempo", "DescricaoValorTempo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            for (int i = 0; i < tabelaFrete.Componentes.Count; i++)
            {
                if (i < NumeroMaximoComplementos)
                {
                    grid.AdicionarCabecalho(tabelaFrete.Componentes[i].ComponenteFrete.Descricao, "DescricaoValorComponente" + UltimaColunaDinamica.ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, tabelaFrete.Componentes[i].Codigo);
                    UltimaColunaDinamica++;
                }
                else
                    break;
            }

            grid.AdicionarCabecalho("Valor Total", "ValorTotal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);

            if (tabelaFrete.PossuiMinimoGarantido)
                grid.AdicionarCabecalho("Valor Mínimo", "ValorMinimo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            if (tabelaFrete.PossuiValorMaximo)
                grid.AdicionarCabecalho("Valor Máximo", "ValorMaximo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            if (tabelaFrete.PossuiValorBase)
                grid.AdicionarCabecalho("Valor Base", "DescricaoValorBase", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            return grid;
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
                grid.AdicionarCabecalho("Ranking", "Ranking", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Licitação", "NumeroLicitacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Proposta", "NumeroLicitacaoParticipacao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Proposta", "DataEnvioOferta", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacaoParticipacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorio = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaAprovacao(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao> listaLicitacaoParticipacao = totalRegistros > 0 ? repositorio.ConsultarAprovacao(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao>();

                var listaLicitacaoRetornar = (
                    from licitacaoParticipacao in listaLicitacaoParticipacao
                    select new
                    {
                        licitacaoParticipacao.Codigo,
                        DataEnvioOferta = licitacaoParticipacao.DataEnvioOferta?.ToString("dd/MM/yyyy"),
                        NumeroLicitacao = licitacaoParticipacao.Licitacao.Numero,
                        NumeroLicitacaoParticipacao = licitacaoParticipacao.Numero,
                        Situacao = licitacaoParticipacao.Situacao.ObterDescricao(),
                        Transportador = licitacaoParticipacao.Transportador.Descricao,
                        licitacaoParticipacao.Ranking
                    }
                ).ToList();

                grid.AdicionaRows(listaLicitacaoRetornar);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "NumeroLicitacao")
                return "Licitacao.Numero";

            if (propriedadeOrdenar == "NumeroLicitacaoParticipacao")
                return "Numero";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
