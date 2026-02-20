using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/AvaliacaoEntrega")]
    public class AvaliacaoEntregaController : BaseController
    {
		#region Construtores

		public AvaliacaoEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaAvaliacaoEntrega filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Carga, "Carga", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Cliente", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoEntrega", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.AvaliacaoEntrega.SituacaoAvaliacao, "Avaliacao", 15, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega = repositorioCargaEntrega.ConsultarAvaliacaoEntrega(filtrosPesquisa, grid.ObterParametrosConsulta());
                grid.setarQuantidadeTotal(repositorioCargaEntrega.ContarConsultaAvaliacaoEntrega(filtrosPesquisa));

                var lista = (from obj in listaCargaEntrega
                             select new
                             {
                                 obj.Codigo,
                                 Carga = obj.Carga.CodigoCargaEmbarcador,
                                 Cliente = obj.Cliente?.Descricao ?? string.Empty,
                                 SituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaHelper.ObterDescricao(obj.Situacao),
                                 Avaliacao = obj.DataAvaliacao.HasValue ? Localization.Resources.Cargas.AvaliacaoEntrega.Respondida : Localization.Resources.Cargas.AvaliacaoEntrega.NaoRespondida,
                             }
                    ).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacaoAnexo, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacaoAnexo, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao>(unitOfWork);

                int codigoCargaEntrega = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega, false);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacaoAnexo> listaAnexo = repositorioAnexo.BuscarPorEntidades(cargaEntrega?.Avaliacoes?.Select(x => x.Codigo).ToList() ?? new List<int>());

                dynamic retorno = new
                {
                    Codigo = cargaEntrega.Codigo,
                    Carga = cargaEntrega.Carga.CodigoCargaEmbarcador,
                    Destinatario = cargaEntrega.Cliente?.Descricao ?? string.Empty,
                    SituacaoAvaliacao = ObterDescricaoSituacao(cargaEntrega),
                    Entregue = cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue,
                    AvaliacaoRespondida = cargaEntrega.DataAvaliacao.HasValue,
                    AvaliacaoGeral = cargaEntrega.AvaliacaoGeral,
                    Avaliacoes = (from obj in cargaEntrega.Avaliacoes
                                  select new
                                  {
                                      Codigo = obj.Codigo,
                                      Ordem = obj.Ordem,
                                      Titulo = obj.Titulo,
                                      Conteudo = obj.Conteudo,
                                      Resposta = obj.Resposta,
                                      Anexos = (from anexo in listaAnexo
                                                where anexo.EntidadeAnexo.Codigo == obj.Codigo
                                                select new
                                                {
                                                    anexo.Codigo,
                                                    anexo.Descricao,
                                                    anexo.NomeArquivo
                                                }).ToList()
                                  }).ToList(),
                    MotivoAvaliacao = cargaEntrega.MotivoAvaliacao,
                    Observacao = cargaEntrega.ObservacaoAvaliacao,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InserirAvaliacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                int codigoCargaEntrega = Request.GetIntParam("CodigoCargaEntrega");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (cargaEntrega.DataAvaliacao.HasValue)
                    return new JsonpResult(false, true, "A avaliação dessa entrega já foi feita.");

                if (cargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                    return new JsonpResult(false, true, "Situação da entrega não permite informar a avaliação.");

                Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao = Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);

                unitOfWork.Start();

                List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.CargaEntregaAvaliacaoPergunta> avaliacoesRetorno = new List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.CargaEntregaAvaliacaoPergunta>();
                PreencheEntidadeAvaliacaoEntrega(ref cargaEntrega, ref avaliacoesRetorno, configuracao, unitOfWork);

                repositorioCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(avaliacoesRetorno);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarConfiguracaoPerguntasAvaliacaoEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao repositorioPortalClientePerguntaAvaliacao = new Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao(unitOfWork);
                Repositorio.Embarcador.Cargas.MotivoAvaliacao repositorioMotivoAvaliacao = new Repositorio.Embarcador.Cargas.MotivoAvaliacao(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao = Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> motivosAvaliacao = repositorioMotivoAvaliacao.BuscarMotivosAtivos();

                Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.QuestionarioView configuracaoQuestionario = new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.QuestionarioView()
                {
                    HabilitarAvaliacao = configuracao.HabilitarAvaliacao,
                    HabilitarAvaliacaoQuestionario = configuracao.TipoAvaliacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAvaliacaoPortalCliente.Individual,
                    HabilitarAnexos = configuracao.PermitirAdicionarAnexos,
                    MotivosAvaliacao = (from motivo in motivosAvaliacao
                                        select new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.MotivosView
                                        {
                                            Valor = motivo.Codigo,
                                            Descricao = motivo.Descricao
                                        }).ToList(),
                    Perguntas = new List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.PerguntasView>() { }
                };

                if (configuracaoQuestionario.HabilitarAvaliacaoQuestionario)
                {
                    configuracaoQuestionario.Perguntas = (
                        from obj in repositorioPortalClientePerguntaAvaliacao.BuscarTodasPerguntas()
                        select new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.PerguntasView()
                        {
                            Codigo = obj.Codigo,
                            Ordem = obj.Ordem,
                            Titulo = obj.Titulo,
                            Conteudo = obj.Conteudo,
                            Resposta = 0
                        }
                    ).ToList();
                }
                else
                {
                    var perguntaPadrao = new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.PerguntasView() { Codigo = 0, Ordem = 0, Titulo = "Como você avalia sua experiência com essa entrega?", Conteudo = "", Resposta = 0 };
                    configuracaoQuestionario.Perguntas.Add(perguntaPadrao);
                }

                return new JsonpResult(configuracaoQuestionario);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheEntidadeAvaliacaoEntrega(ref Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, ref List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.CargaEntregaAvaliacaoPergunta> avaliacoesRetorno, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MotivoAvaliacao repositorioMotivoAvaliacao = new Repositorio.Embarcador.Cargas.MotivoAvaliacao(unitOfWork);

            cargaEntrega.DataAvaliacao = DateTime.Now;
            cargaEntrega.AvaliacaoGeral = Request.GetNullableIntParam("AvaliacaoGeral");
            cargaEntrega.ObservacaoAvaliacao = Request.GetStringParam("Observacao");

            int? motivoAvaliacao = Request.GetNullableIntParam("MotivoAvaliacao");

            if (configuracao.TipoAvaliacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAvaliacaoPortalCliente.Individual)
            {
                SalvarRespostasAvaliacao(cargaEntrega, ref avaliacoesRetorno, configuracao, unitOfWork);

            }
            else
            {
                SalvarRespostasAvaliacaoPadrao(cargaEntrega, configuracao, unitOfWork);
                cargaEntrega.MotivoAvaliacao = motivoAvaliacao.HasValue && motivoAvaliacao.Value > 0 ? repositorioMotivoAvaliacao.BuscarPorCodigo(motivoAvaliacao.Value, false) : null;
            }
        }

        private void SalvarRespostasAvaliacao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, ref List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.CargaEntregaAvaliacaoPergunta> avaliacoesRetorno, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao repositorioCargaEntregaAvaliacao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao(unitOfWork);
            Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao repositorioPortalClientePerguntaAvaliacao = new Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracao.BuscarConfiguracaoPadrao();

            var dynRespostas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Questionario"));

            List<Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao> perguntasDisponiveis = repositorioPortalClientePerguntaAvaliacao.BuscarTodasPerguntas();
            foreach (var resposta in dynRespostas)
            {
                int codigoPergunta = ((string)resposta.Codigo).ToInt();
                int? respostaPergunta = ((string)resposta.Resposta).ToNullableInt();

                Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao pergunta = perguntasDisponiveis.Where(o => o.Codigo == codigoPergunta).FirstOrDefault() ?? throw new ControllerException(Localization.Resources.GestaoEntregas.Acompanhamento.FalhaAoSalvarAsPerguntas);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao respostaPerguntaCargaEntrega = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao()
                {
                    CargaEntrega = cargaEntrega,
                    Ordem = pergunta.Ordem,
                    Titulo = pergunta.Titulo,
                    Conteudo = pergunta.Conteudo,
                    Resposta = respostaPergunta
                };

                repositorioCargaEntregaAvaliacao.Inserir(respostaPerguntaCargaEntrega);

                if ((respostaPerguntaCargaEntrega.CargaEntrega?.MotivoAvaliacao?.GerarAtendimentoAutomaticoQuandoAvalicaoForUmaEstrela ?? false) && respostaPerguntaCargaEntrega.Resposta == 1)
                {
                    Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objetoChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado()
                    {
                        Observacao = string.Empty,
                        MotivoChamado = respostaPerguntaCargaEntrega.CargaEntrega?.MotivoAvaliacao?.MotivoChamado ?? null,
                        Carga = cargaEntrega.Carga,
                        Empresa = cargaEntrega.Carga.Empresa,
                        Cliente = repositorioCargaPedido.BuscarPrimeiroClientePorCarga(cargaEntrega.Carga.Codigo, !configuracaoTMS.ChamadoOcorrenciaUsaRemetente),
                        TipoCliente = configuracaoTMS.ChamadoOcorrenciaUsaRemetente ? Dominio.Enumeradores.TipoTomador.Remetente : Dominio.Enumeradores.TipoTomador.Destinatario
                    };

                    Dominio.Entidades.Usuario usuario = cargaEntrega.Carga.Motoristas.FirstOrDefault();
                    if (usuario == null)
                        throw new ServicoException("Carga não possui motorista. Necessário informar antes de prosseguir.");

                    Dominio.Entidades.Embarcador.Chamados.Chamado chamado = Servicos.Embarcador.Chamado.Chamado.AbrirChamado(objetoChamado, usuario, 0, null, unitOfWork);
                }

                avaliacoesRetorno.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.CargaEntregaAvaliacaoPergunta()
                {
                    CodigoAvaliacaoEntrega = respostaPerguntaCargaEntrega.Codigo,
                    CodigoPergunta = pergunta.Codigo
                });
            }
        }

        private void SalvarRespostasAvaliacaoPadrao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao repositorioCargaEntregaAvaliacao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao(unitOfWork);

            var dynRespostas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Questionario"));

            foreach (var resposta in dynRespostas)
            {
                int? respostaPergunta = ((string)resposta.Resposta).ToNullableInt();

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao respostaPergunaCargaEntrega = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao()
                {
                    CargaEntrega = cargaEntrega,
                    Ordem = 0,
                    Titulo = "Como você avalia sua experiência com essa entrega?",
                    Conteudo = "",
                    Resposta = respostaPergunta
                };

                repositorioCargaEntregaAvaliacao.Inserir(respostaPergunaCargaEntrega);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaAvaliacaoEntrega ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaAvaliacaoEntrega filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaAvaliacaoEntrega()
            {
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                NumeroTransporte = Request.GetStringParam("NumeroTransporte"),
                Respondida = Request.GetNullableIntParam("Respondida"),
                SituacaoEntrega = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega>("SituacaoEntrega")
            };

            double CNPJDestinatario = Request.GetDoubleParam("Destinatario");
            List<double> ListaCNPJ = ObterListaCnpjCpfClientePermitidosOperadorLogistica(unitOfWork);

            filtrosPesquisa.CNPJsDestinatario = ListaCNPJ.Count > 0 ? ListaCNPJ : CNPJDestinatario > 0 ? new List<double>() { CNPJDestinatario } : null;
            return filtrosPesquisa;
        }

        private string ObterDescricaoSituacao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            if (cargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                return "Situação da entrega não permite informar a avaliação";
            else if (cargaEntrega.DataAvaliacao.HasValue)
                return "Avaliação respondida";
            else
                return "Avaliação não respondida";
        }

        #endregion
    }
}
