using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao.Alcadas
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Escrituracao/AutorizacaoEstornoProvisao")]
    public class AutorizacaoEstornoProvisaoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente,
        Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao
    >
    {
		#region Construtores

		public AutorizacaoEstornoProvisaoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais
		[AllowAuthenticate]
        public async Task<IActionResult> Reprocessar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao repositorioEstornoProvisaoSolicitacao = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao estornoSolicitacao = repositorioEstornoProvisaoSolicitacao.BuscarPendentePorEstornoProvisao(codigo);

                if (estornoSolicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (estornoSolicitacao.Situacao != SituacaoEstornoProvisaoSolicitacao.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();

                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repEstornoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
                Servicos.Embarcador.Escrituracao.EstornoProvisaoAprovacao servicoEstornoProvisaoAprovacao = new Servicos.Embarcador.Escrituracao.EstornoProvisaoAprovacao(unitOfWork);

                servicoEstornoProvisaoAprovacao.CriarAprovacao(estornoSolicitacao, TipoServicoMultisoftware);
                repEstornoProvisao.Atualizar(estornoSolicitacao.EstornoProvisao);
                repositorioEstornoProvisaoSolicitacao.Atualizar(estornoSolicitacao);


                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = estornoSolicitacao.Situacao != SituacaoEstornoProvisaoSolicitacao.SemRegraAprovacao });
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
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                List<int> codigosEstornoProvisaoSolicitacao = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repEstornoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao repositorioEstornoProvisaoSolicitacao = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao> listaEstornoProvisaoSolicitacao = repositorioEstornoProvisaoSolicitacao.BuscarSemRegraAprovacaoPorCodigos(codigosEstornoProvisaoSolicitacao);
                Servicos.Embarcador.Escrituracao.EstornoProvisaoAprovacao servicoEstornoProvisaoAprovacao = new Servicos.Embarcador.Escrituracao.EstornoProvisaoAprovacao(unitOfWork);
                int totalRegrasReprocessadas = 0;

                foreach (Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao estornoSolicitacao in listaEstornoProvisaoSolicitacao)
                {
                    servicoEstornoProvisaoAprovacao.CriarAprovacao(estornoSolicitacao, TipoServicoMultisoftware);

                    if (estornoSolicitacao.Situacao != SituacaoEstornoProvisaoSolicitacao.SemRegraAprovacao)
                    {
                        repEstornoProvisao.Atualizar(estornoSolicitacao.EstornoProvisao);
                        repositorioEstornoProvisaoSolicitacao.Atualizar(estornoSolicitacao);
                        totalRegrasReprocessadas++;
                    }
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
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarRegras()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorioAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);

                var aprovacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (aprovacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                if (aprovacao.Situacao != SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações.");

                if (!aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo))
                    return new JsonpResult(false, "Aprovação não permite alterações.");

                aprovacao.Data = DateTime.Now;
                aprovacao.Situacao = SituacaoAlcadaRegra.Aprovada;

                if (aprovacao.TermoQuitacaoFinanceiro == null)
                    VerificarSituacaoOrigem(aprovacao.OrigemAprovacao, unitOfWork);
                else
                    AprovarAlcadaTermoQuitacao(aprovacao.TermoQuitacaoFinanceiro, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a regra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplasRegrasController()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorioAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);
                var aprovacoesPendentes = repositorioAprovacao.BuscarPendentesIncluindoTermo(codigo, this.Usuario.Codigo);

                foreach (var aprovacao in aprovacoesPendentes)
                {
                    if (aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo))
                    {
                        aprovacao.Data = DateTime.Now;
                        aprovacao.Situacao = SituacaoAlcadaRegra.Aprovada;
                        repositorioAprovacao.Atualizar(aprovacao);
                    }
                }

                var origem = (from aprovacao in aprovacoesPendentes select aprovacao.OrigemAprovacao).FirstOrDefault();
                var termos = (from aprovacao in aprovacoesPendentes select aprovacao.TermoQuitacaoFinanceiro).FirstOrDefault();

                if (origem != null)
                    VerificarSituacaoOrigem(origem, unitOfWork);
                else if (termos != null)
                    AprovarAlcadaTermoQuitacao(termos, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RegrasModificadas = aprovacoesPendentes.Count()
                });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RegrasAprovacaoController()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoUsuario = Request.GetIntParam("Usuario");

                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoUsuario", false);
                grid.AdicionarCabecalho("PodeAprovar", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (codigoUsuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Aprovação", "Data", 10, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorioAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> aprovacoes = repositorioAprovacao.BuscarDesbloqueadaIncluindoTermo(codigo, codigoUsuario);

                var lista = (
                    from aprovacao in aprovacoes
                    select new
                    {
                        aprovacao.Codigo,
                        CodigoUsuario = aprovacao.Usuario?.Codigo ?? 0,
                        Regra = aprovacao.Descricao,
                        Situacao = aprovacao.Situacao.ObterDescricao(),
                        Usuario = aprovacao.Usuario?.Nome,
                        Data = aprovacao.Data.HasValue ? aprovacao.Data.Value.ToString("dd/MM/yyyy HH:mm") : "",
                        PodeAprovar = PermitirAprovacaoOuReprovacao(this.Usuario.Codigo,aprovacao),
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
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorio = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo repositorioAnexo = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao estornoProvisaoSolicitacao = repositorio.BuscarPorCodigo(codigo);


                if (estornoProvisaoSolicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao estorno = estornoProvisaoSolicitacao.OrigemAprovacao;
                List<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo> anexos = repositorioAnexo.BuscarPorSolicitacao(estornoProvisaoSolicitacao.Codigo);

                return new JsonpResult(new
                {
                    Codigo = estorno != null ? estorno.Codigo : estornoProvisaoSolicitacao.Codigo,
                    CodigoSolicitacao = estornoProvisaoSolicitacao.Codigo,
                    NumeroProvisao = estorno != null ? estorno.Numero : estornoProvisaoSolicitacao.TermoQuitacaoFinanceiro?.NumeroTermo ?? 0,
                    Transportador = estorno != null ? estorno.Empresa?.Descricao ?? string.Empty : "",
                    Filial = estorno != null ? estorno.Filial?.Descricao ?? string.Empty : "",
                    Tomador = estorno != null ? estorno.Tomador?.Descricao ?? string.Empty : "",
                    DataInicial = estorno != null ? estorno.DataInicial.HasValue ? estorno.DataInicial.Value.ToString("g") : "" : estornoProvisaoSolicitacao.TermoQuitacaoFinanceiro.DataInicial?.ToString("g") ?? "",
                    DataFinal = estorno != null ? estorno.DataInicial.HasValue ? estorno.DataInicial.Value.ToString("g") : "" : estornoProvisaoSolicitacao.TermoQuitacaoFinanceiro.DataFinal?.ToString("g") ?? "",
                    Carga = estorno != null ? estorno.Carga?.CodigoCargaEmbarcador ?? string.Empty : "",
                    Ocorrencia = estorno != null ? estorno.CargaOcorrencia?.NumeroOcorrencia.ToString() : string.Empty,
                    ValorProvisao = estorno != null ? estorno.ValorCancelamentoProvisao.ToString() : estornoProvisaoSolicitacao.TermoQuitacaoFinanceiro.TotalGeralPagamento.ToString(),
                    ValorFrete = estorno != null ? estorno.Carga?.ValorFreteAPagar.ToString("n2") : "",
                    Anexos = (from anexo in anexos select RetornaDynAnexo(anexo)).ToList()
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

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaEstornoProvisaoAprovacao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaEstornoProvisaoAprovacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaEstornoProvisaoAprovacao()
            {
                SituacaoEstornoProvisaoSolicitacao = Request.GetNullableEnumParam<SituacaoEstornoProvisaoSolicitacao>("Situacao"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                NumeroLote = Request.GetIntParam("NumeroLote"),
                DataGeracaoLoteInicial = Request.GetDateTimeParam("DataGeracaoLoteInicial"),
                DataGeracaoLoteFinal = Request.GetDateTimeParam("DataGeracaoLoteFinal"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                NumeroProvisao = Request.GetStringParam("NumeroProvisao"),
                CodigoCarga = Request.GetIntParam("Carga")
            };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataGeracao")
                return "DataCriacao";
            return propriedadeOrdenar;
        }

        private dynamic RetornaDynAnexo(Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo anexo)
        {
            return new
            {
                Codigo = anexo.Codigo,
                NomeArquivo = anexo.NomeArquivo,
                CodigoGuid = anexo.GuidArquivo
            };
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao origem)
        {
            return true;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> estornosProvisaoSolicitacao;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaEstornoProvisaoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorioAprovacaoAlcadaCarga = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);

                estornosProvisaoSolicitacao = repositorioAprovacaoAlcadaCarga.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    estornosProvisaoSolicitacao.Remove(new Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorioEstornoProvisaoSolicitacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                estornosProvisaoSolicitacao = new List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    estornosProvisaoSolicitacao.Add(repositorioEstornoProvisaoSolicitacao.BuscarPorEstornoProvisao((int)itemSelecionado.Codigo, 0));
            }

            List<int> codigoOrigems = estornosProvisaoSolicitacao.Where(x => x.OrigemAprovacao != null).Select(x => x.Codigo).ToList();
            List<int> codigosTermos = estornosProvisaoSolicitacao.Where(x => x.TermoQuitacaoFinanceiro != null).Select(x => x.Codigo).ToList();

            if (codigosTermos.Count > 0)
                codigoOrigems.AddRange(codigosTermos);

            return codigoOrigems;
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
                grid.AdicionarCabecalho(propriedade: "ValorProvisao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "PossuiTermoQuitacao", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número Lote", propriedade: "NumeroLote", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Número Provisão", propriedade: "NumeroProvisao", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Carga", propriedade: "Carga", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Data Geração", propriedade: "DataGeracao", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);


                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaEstornoProvisaoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorio = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao repositorioEstornoProvisao = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> estornosProvisaoSolicitacao = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();
                List<(int codigoEstorno, SituacaoEstornoProvisaoSolicitacao situacao)> situacoesDasAprovacoesEstoro = repositorioEstornoProvisao.BuscarSituacaoesPorEstorno(estornosProvisaoSolicitacao.Where(x => x.OrigemAprovacao != null).Select(x => x.OrigemAprovacao.Codigo).ToList());
                var lista = (
                    from estornoProvisaoSolicitacao in estornosProvisaoSolicitacao
                    select new
                    {
                        estornoProvisaoSolicitacao.Codigo,
                        PossuiTermoQuitacao = estornoProvisaoSolicitacao.TermoQuitacaoFinanceiro != null,
                        NumeroLote = estornoProvisaoSolicitacao?.OrigemAprovacao != null ? estornoProvisaoSolicitacao?.OrigemAprovacao.Numero.ToString() : estornoProvisaoSolicitacao?.TermoQuitacaoFinanceiro?.NumeroTermo.ToString() ?? string.Empty,
                        NumeroProvisao = estornoProvisaoSolicitacao?.OrigemAprovacao != null ? string.Join(", ", estornoProvisaoSolicitacao?.OrigemAprovacao?.DocumentosProvisao?.Select(o => o.Stage?.NumeroFolha ?? string.Empty).ToList()) : estornoProvisaoSolicitacao?.TermoQuitacaoFinanceiro?.NumeroTermo.ToString() ?? "",
                        Carga = estornoProvisaoSolicitacao?.OrigemAprovacao != null ? estornoProvisaoSolicitacao?.OrigemAprovacao?.Carga?.CodigoCargaEmbarcador?.ToString() : string.Empty,
                        DataGeracao = estornoProvisaoSolicitacao?.OrigemAprovacao != null ? estornoProvisaoSolicitacao?.OrigemAprovacao?.DataCriacao.ToString("g") : estornoProvisaoSolicitacao?.TermoQuitacaoFinanceiro?.DataCriacao?.ToString("g") ?? "",
                        Situacao = estornoProvisaoSolicitacao.TermoQuitacaoFinanceiro == null ? situacoesDasAprovacoesEstoro.Where(x => x.codigoEstorno == estornoProvisaoSolicitacao.OrigemAprovacao.Codigo)?.FirstOrDefault().situacao.ObterDescricao() ?? string.Empty : estornoProvisaoSolicitacao.Situacao.ObterDescricao(),
                        ValorProvisao = estornoProvisaoSolicitacao?.OrigemAprovacao != null ? estornoProvisaoSolicitacao?.OrigemAprovacao?.ValorCancelamentoProvisao : estornoProvisaoSolicitacao?.TermoQuitacaoFinanceiro?.TotalGeralPagamento ?? 0
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception exe)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao != SituacaoCancelamentoProvisao.AgAprovacaoSolicitacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repositorioEstornoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao repositorioEstornoProvisaoSolicitacao = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao(unitOfWork);
            Servicos.Embarcador.Escrituracao.EstornoProvisaoAprovacao servicoEstornoProvisaoAprovacao = new Servicos.Embarcador.Escrituracao.EstornoProvisaoAprovacao(unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao solicitacao = repositorioEstornoProvisaoSolicitacao.BuscarPendentePorEstornoProvisao(origem.Codigo);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (!servicoEstornoProvisaoAprovacao.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                origem.Situacao = SituacaoCancelamentoProvisao.EmCancelamento;
                if (solicitacao != null)
                {
                    solicitacao.Situacao = SituacaoEstornoProvisaoSolicitacao.Aprovada;
                    repositorioEstornoProvisaoSolicitacao.Atualizar(solicitacao);
                }

                repositorioEstornoProvisao.Atualizar(origem);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Estorno da Provisão aprovado", unitOfWork);
            }
            else
            {
                origem.Situacao = SituacaoCancelamentoProvisao.SolicitacaoReprovada;
                if (solicitacao != null)
                {
                    solicitacao.Situacao = SituacaoEstornoProvisaoSolicitacao.Reprovada;
                    repositorioEstornoProvisaoSolicitacao.Atualizar(solicitacao);
                }
                repositorioEstornoProvisao.Atualizar(origem);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Estorno da Provisão reprovado", unitOfWork);
            }

        }

        #endregion

        #region Metodos Privados

        private void AprovarAlcadaTermoQuitacao(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacaoFinanceiro, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermo = new Repositorio.Embarcador.Financeiro.TermoQuitacao(unitOfWork);
            Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro servicoTermoQuitacaoFinanceiro = new Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro(unitOfWork);
            termoQuitacaoFinanceiro.SituacaoTermoQuitacao = SituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador;
            servicoTermoQuitacaoFinanceiro.ValidarAprovacaoTransportador(termoQuitacaoFinanceiro);
            repositorioTermo.Atualizar(termoQuitacaoFinanceiro);
        }

        public virtual bool PermitirAprovacaoOuReprovacao(int codigoUsuario, Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao aprovacao)
        {
            return !aprovacao.Bloqueada && (aprovacao.Usuario != null) && (aprovacao.Usuario.Codigo == codigoUsuario) && (aprovacao.Situacao == SituacaoAlcadaRegra.Pendente);
        }
        #endregion
    }
}