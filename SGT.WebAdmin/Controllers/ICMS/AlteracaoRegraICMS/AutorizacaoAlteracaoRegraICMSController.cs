using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.ICMS.AlteracaoRegraICMS
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "ICMS/AutorizacaoAlteracaoRegraICMS")]
    public class AutorizacaoAlteracaoRegraICMSController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS,
        Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS,
        Dominio.Entidades.Embarcador.ICMS.RegraICMS
    >
    {
        #region Construtores

        public AutorizacaoAlteracaoRegraICMSController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ReprocessarRegraICMS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                int codigoRegraICMS = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.ICMS.RegraICMS repositorioRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = repositorioRegraICMS.BuscarPorCodigo(codigoRegraICMS);

                if (regraICMS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (regraICMS.SituacaoAlteracao != SituacaoAlteracaoRegraICMS.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();

                Servicos.Embarcador.ICMS.RegraICMSAprovacao servicoRegraICMSAprovacao = new Servicos.Embarcador.ICMS.RegraICMSAprovacao(unitOfWork);

                servicoRegraICMSAprovacao.CriarAprovacao(regraICMS, TipoServicoMultisoftware);
                repositorioRegraICMS.Atualizar(regraICMS);

                unitOfWork.CommitChanges();

                if (regraICMS.SituacaoAlteracao == SituacaoAlteracaoRegraICMS.Aprovada)
                    Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork).AtualizarRegrasICMS(unitOfWork);

                return new JsonpResult(new { RegraReprocessada = regraICMS.SituacaoAlteracao != SituacaoAlteracaoRegraICMS.SemRegraAprovacao });
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
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar a configuração de regra para ICMS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplasRegrasICMS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                List<int> codigosRegraICMS = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.ICMS.RegraICMS repositorioRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
                List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> listaRegraICMS = repositorioRegraICMS.BuscarSemRegraAprovacaoPorCodigos(codigosRegraICMS);
                Servicos.Embarcador.ICMS.RegraICMSAprovacao servicoRegraICMSAprovacao = new Servicos.Embarcador.ICMS.RegraICMSAprovacao(unitOfWork);
                int totalRegrasReprocessadas = 0;
                bool possuiRegraAprovada = false;

                foreach (Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS in listaRegraICMS)
                {
                    servicoRegraICMSAprovacao.CriarAprovacao(regraICMS, TipoServicoMultisoftware);

                    if (regraICMS.SituacaoAlteracao != SituacaoAlteracaoRegraICMS.SemRegraAprovacao)
                    {
                        repositorioRegraICMS.Atualizar(regraICMS);
                        totalRegrasReprocessadas++;
                    }

                    if (regraICMS.SituacaoAlteracao == SituacaoAlteracaoRegraICMS.Aprovada)
                        possuiRegraAprovada = true;
                }

                unitOfWork.CommitChanges();

                if (possuiRegraAprovada)
                    Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork).AtualizarRegrasICMS(unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as configuração de regra para ICMS.");
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
                Repositorio.Embarcador.ICMS.RegraICMS repositorio = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS = repositorio.BuscarPorCodigo(codigo);

                if (regraICMS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    regraICMS.Codigo,
                    Situacao = regraICMS.SituacaoAlteracao,
                    regraICMS.Descricao,
                    SituacaoDescricao = regraICMS.SituacaoAlteracao.ObterDescricao(),
                    UFEmitente = regraICMS.UFEmitente?.Sigla ?? "",
                    Destino = regraICMS.UFDestino != null ? regraICMS.UFDestino.Sigla + (regraICMS.Destinatario != null ? $" - {regraICMS.Destinatario.Descricao}" : (regraICMS.GrupoDestinatario != null ? $" - {regraICMS.GrupoDestinatario.Descricao}" : "")) : "" + (regraICMS.Destinatario != null ? regraICMS.Destinatario.Descricao : (regraICMS.GrupoDestinatario != null ? regraICMS.GrupoDestinatario.Descricao : "")),
                    Origem = regraICMS.UFOrigem != null ? regraICMS.UFOrigem.Sigla + (regraICMS.Remetente != null ? $" - {regraICMS.Remetente.Descricao}" : (regraICMS.GrupoRemetente != null ? $" - {regraICMS.GrupoRemetente.Descricao}" : "")) : "" + (regraICMS.Remetente != null ? regraICMS.Remetente.Descricao : (regraICMS.GrupoRemetente != null ? regraICMS.GrupoRemetente.Descricao : "")),
                    Tomador = regraICMS.UFTomador != null ? regraICMS.UFTomador.Sigla + (regraICMS.Tomador != null ? $" - {regraICMS.Tomador.Descricao}" : (regraICMS.GrupoTomador != null ? $" - {regraICMS.GrupoTomador.Descricao}" : "")) : "" + (regraICMS.Tomador != null ? regraICMS.Tomador.Descricao : (regraICMS.GrupoTomador != null ? regraICMS.GrupoTomador.Descricao : "")),
                    regraICMS.DescricaoRegra,
                    VigenciaInicio = regraICMS.VigenciaInicio?.ToString("dd/MM/yyyy") ?? "",
                    VigenciaFim = regraICMS.VigenciaFim?.ToString("dd/MM/yyyy") ?? " - ",
                    regraICMS.CST,
                    CFOP = regraICMS.CFOP?.CodigoCFOP.ToString() ?? "",
                    Aliquota = regraICMS.Aliquota?.ToString("n2") ?? "",
                    ZerarValorICMS = regraICMS.ZerarValorICMS ? "Sim" : "Não",
                    NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE ? "Sim" : "Não",
                    NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte ? "Sim" : "Não",
                    NaoIncluirICMSValorFrete = (regraICMS.NaoIncluirICMSValorFrete ?? false) ? "Sim" : "Não",
                    CNPJ = regraICMS.Empresa?.CNPJ_Formatado ?? "",
                    RazaoSocial = regraICMS.Empresa?.RazaoSocial ?? "",
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

        private Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaAlteracaoRegraICMS ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaAlteracaoRegraICMS()
            {
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Descricao = Request.GetStringParam("Descricao"),
                SituacaoAlteracao = Request.GetNullableEnumParam<SituacaoAlteracaoRegraICMS>("Situacao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.ICMS.RegraICMS origem)
        {
            return origem.SituacaoAlteracao == SituacaoAlteracaoRegraICMS.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasICMS;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaAlteracaoRegraICMS filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS repositorioAprovacaoAlcada = new Repositorio.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS(unitOfWork);

                regrasICMS = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    regrasICMS.Remove(new Dominio.Entidades.Embarcador.ICMS.RegraICMS() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.ICMS.RegraICMS repositorioRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                regrasICMS = new List<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    regrasICMS.Add(repositorioRegraICMS.BuscarPorCodigo((int)itemSelecionado.Codigo));
            }

            return (from regraICMS in regrasICMS select regraICMS.Codigo).ToList();
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
                grid.AdicionarCabecalho(descricao: "Descrição", propriedade: "Descricao", tamanho: 18, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "UF Emitente", propriedade: "UFEmitente", tamanho: 9, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Origem", propriedade: "Origem", tamanho: 9, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Destino", propriedade: "Destino", tamanho: 9, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tomador", propriedade: "Tomador", tamanho: 9, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Regra", propriedade: "DescricaoRegra", tamanho: 18, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Vig. Início", propriedade: "VigenciaInicio", tamanho: 7, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Vig. Fim", propriedade: "VigenciaFim", tamanho: 7, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 12, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaAlteracaoRegraICMS filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS repositorio = new Repositorio.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> regrasICMS = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

                var lista = (
                    from regraICMS in regrasICMS
                    select new
                    {
                        regraICMS.Codigo,
                        regraICMS.Descricao,
                        UFEmitente = regraICMS.UFEmitente?.Sigla ?? "",
                        Destino = regraICMS.UFDestino != null ? regraICMS.UFDestino.Sigla + (regraICMS.Destinatario != null ? $" - {regraICMS.Destinatario.Descricao}" : (regraICMS.GrupoDestinatario != null ? $" - {regraICMS.GrupoDestinatario.Descricao}" : "")) : "" + (regraICMS.Destinatario != null ? regraICMS.Destinatario.Descricao : (regraICMS.GrupoDestinatario != null ? regraICMS.GrupoDestinatario.Descricao : "")),
                        Origem = regraICMS.UFOrigem != null ? regraICMS.UFOrigem.Sigla + (regraICMS.Remetente != null ? $" - {regraICMS.Remetente.Descricao}" : (regraICMS.GrupoRemetente != null ? $" - {regraICMS.GrupoRemetente.Descricao}" : "")) : "" + (regraICMS.Remetente != null ? regraICMS.Remetente.Descricao : (regraICMS.GrupoRemetente != null ? regraICMS.GrupoRemetente.Descricao : "")),
                        Tomador = regraICMS.UFTomador != null ? regraICMS.UFTomador.Sigla + (regraICMS.Tomador != null ? $" - {regraICMS.Tomador.Descricao}" : (regraICMS.GrupoTomador != null ? $" - {regraICMS.GrupoTomador.Descricao}" : "")) : "" + (regraICMS.Tomador != null ? regraICMS.Tomador.Descricao : (regraICMS.GrupoTomador != null ? regraICMS.GrupoTomador.Descricao : "")),
                        regraICMS.DescricaoRegra,
                        VigenciaInicio = regraICMS.VigenciaInicio?.ToString("dd/MM/yyyy") ?? " - ",
                        VigenciaFim = regraICMS.VigenciaFim?.ToString("dd/MM/yyyy") ?? " - ",
                        Situacao = regraICMS.SituacaoAlteracao.ObterDescricao(),
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.ICMS.RegraICMS origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.SituacaoAlteracao != SituacaoAlteracaoRegraICMS.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.ICMS.RegraICMS repositorioRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);
            Servicos.Embarcador.ICMS.RegraICMSAprovacao servicoRegraICMSAprovacao = new Servicos.Embarcador.ICMS.RegraICMSAprovacao(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (!servicoRegraICMSAprovacao.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                origem.SituacaoAlteracao = SituacaoAlteracaoRegraICMS.Aprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Configuração de regra para ICMS aprovada", unitOfWork);
                new Servicos.Embarcador.Carga.ICMS().AplicarAlteracoes(origem, unitOfWork);
                Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork).AtualizarRegrasICMS(unitOfWork);
            }
            else
            {
                origem.SituacaoAlteracao = SituacaoAlteracaoRegraICMS.Reprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Configuração de regra para ICMS reprovada", unitOfWork);
            }

            EnviarEmailParaTransportador(origem, situacaoRegrasAutorizacao, unitOfWork);

            repositorioRegraICMS.Atualizar(origem);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        private void EnviarEmailParaTransportador(Dominio.Entidades.Embarcador.ICMS.RegraICMS origem, SituacaoRegrasAutorizacao situacaoRegrasAutorizacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem != null &&
                origem.TipoServicoMultisoftware.HasValue &&
                origem.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe &&
                origem.Empresa != null &&
                !string.IsNullOrWhiteSpace(origem.Empresa.Email) &&
                (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas || situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Reprovadas))
            {

                string titulo = (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas) ? "Aprovada" : "Reprovada";
                string assunto = "[Regra ICMS] " + origem.Descricao + " - " + titulo;

                List<KeyValuePair<string, string>> linhas = new List<KeyValuePair<string, string>>();
                linhas.Add(new KeyValuePair<string, string>("Descrição", origem.Descricao));
                linhas.Add(new KeyValuePair<string, string>("Regra ICMS", origem.DescricaoRegra));
                linhas.Add(new KeyValuePair<string, string>("CNPJ/CPF", origem.Empresa.CNPJ_Formatado));
                linhas.Add(new KeyValuePair<string, string>("Tranportador", origem.Empresa.Descricao));
                linhas.Add(new KeyValuePair<string, string>("Data da alteração", origem.DataAlteracao?.ToString("dd/MM/yyyy HH:mm") ?? ""));

                List<Dominio.ObjetosDeValor.Email.Mensagem> mensagens = new List<Dominio.ObjetosDeValor.Email.Mensagem>()
                {
                    new Dominio.ObjetosDeValor.Email.Mensagem
                    {
                        Destinatarios = new List<string> { origem.Empresa.Email },
                        Assunto = assunto,
                        Corpo = Servicos.Email.TemplateCorpoEmail(titulo, linhas, "Foi realizada a revisão da regra de ICMS registrada no Portal MultiTransportador:", null, "Regra ICMS", "E-mail enviado automaticamente.")
                    }
                };

                Servicos.Email.EnviarMensagensAsync(mensagens, unitOfWork);
            }
        }


        #endregion

    }
}