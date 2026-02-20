using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.GestaoEntregas
{
    [AllowAnonymous]
    public class RastreioEntregaController : BaseController
    {
		#region Construtores

		public RastreioEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Rastreamento(string token)
        {
            string caminhoBaseViews = "~/Views/GestaoEntregas/RastreioEntrega";

            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                MontaLayoutBase(unitOfWork);
                DefineParametrosView(token, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaView dataView = ObtemDadosRenderizacao(token);

                if (dataView == null)
                    return View($"{caminhoBaseViews}/Erro.cshtml");

                return View($"{caminhoBaseViews}/Detalhe.cshtml", dataView);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return View($"{caminhoBaseViews}/Erro.cshtml");
            }
        }

        public async Task<IActionResult> Feedback()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                string token = Request.GetStringParam("Token");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigoRastreio(token);
                if (cargaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaEntrega.DataAvaliacao.HasValue)
                    return new JsonpResult(false, true, "A avaliação desse pedido já foi feita.");

                Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao = Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);

                PreencheEntidade(ref cargaEntrega, configuracao, unitOfWork);

                repCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MotivoAvaliacao repMotivoAvaliacao = new Repositorio.Embarcador.Cargas.MotivoAvaliacao(unitOfWork);

            entrega.DataAvaliacao = DateTime.Now;
            entrega.ObservacaoAvaliacao = Request.GetStringParam("Observacao");

            if (configuracao.TipoAvaliacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAvaliacaoPortalCliente.Individual)
            {
                SalvarRespostasAvaliacao(entrega, configuracao, unitOfWork);
                return;
            }

            entrega.AvaliacaoGeral = Request.GetNullableIntParam("Avaliacao");
            entrega.MotivoAvaliacao = entrega.AvaliacaoGeral.Value <= 3 ? repMotivoAvaliacao.BuscarPorCodigo(Request.GetIntParam("MotivoAvaliacao"), false) : null;
        }

        private void SalvarRespostasAvaliacao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao repPortalClientePerguntaAvaliacao = new Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao repCargaEntregaAvaliacao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao(unitOfWork);

            var respostas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Questionario"));

            List<Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao> perguntasDisponiveis = repPortalClientePerguntaAvaliacao.BuscarTodasPerguntas();

            foreach (var resposta in respostas)
            {
                int codigoPergunta = ((string)resposta.Codigo).ToInt();
                int? respostaPergunta = ((string)resposta.Resposta).ToNullableInt();

                Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao pergunta = perguntasDisponiveis.Where(o => o.Codigo == codigoPergunta).FirstOrDefault() ?? throw new ControllerException("Falha ao salvar as perguntas");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao respostaPergunaCargaEntrega = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao()
                {
                    CargaEntrega = entrega,
                    Ordem = pergunta.Ordem,
                    Titulo = pergunta.Titulo,
                    Conteudo = pergunta.Conteudo,
                    Resposta = respostaPergunta
                };

                repCargaEntregaAvaliacao.Inserir(respostaPergunaCargaEntrega);
            }
        }

        private void DefineParametrosView(string token, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();
            string protocolo = (Request.IsHttps ? "https" : "http");
            if (configuracaoAmbiente?.TipoProtocolo != null && configuracaoAmbiente?.TipoProtocolo.ObterProtocolo() != "")
                protocolo = configuracaoAmbiente?.TipoProtocolo.ObterProtocolo();
            ViewBag.HTTPConnection = protocolo;
            ViewBag.Token = token;
            ViewBag.APIKeyGoogle = configuracaoIntegracao?.APIKeyGoogle ?? "AIzaSyB6e6zUspWGFYrLmABRgI3rsMss_nKW_s4";
        }

        private void MontaLayoutBase(Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.UnitOfWork adminMultisoftwareUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminMultisoftwareUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                adminMultisoftwareUnitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaView ObtemDadosRenderizacao(string codigoRastreamento)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigoRastreio(codigoRastreamento);
            if (cargaEntrega == null)
                return null;

            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao = Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaView dataView = new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaView
            {
                Entregue = cargaEntrega.Situacao == SituacaoEntrega.Entregue,
                Numero = cargaEntrega.Carga.CodigoCargaEmbarcador,
                Destinatario = cargaEntrega.Cliente?.Descricao ?? string.Empty,
                Localidade = cargaEntrega.Cliente?.Localidade.Descricao ?? string.Empty,
                Situacao = cargaEntrega.Situacao.ObterDescricaoPortalEntrega(),
                Transportador = cargaEntrega?.Carga?.Empresa?.Descricao ?? string.Empty,
                Filial = cargaEntrega?.Carga?.Filial?.Descricao ?? string.Empty,
                NotasFiscais = ObterNumerosNotasFiscaisPedido(cargaEntrega, unitOfWork),
                NumeroTransporte = ObterNumeroTransportePedido(cargaEntrega, unitOfWork),
                DataPrevisaoChegada = cargaEntrega.DataPrevista?.ToDateTimeString() ?? string.Empty,
                Volumes = cargaEntrega?.Pedidos?.Sum(o => o.CargaPedido.Pedido.QtVolumes) ?? 0,

                Observacao = cargaEntrega.ObservacaoAvaliacao,
                MotivoAvaliacao = cargaEntrega.MotivoAvaliacao?.Descricao ?? string.Empty,
                Avaliacao = cargaEntrega.AvaliacaoGeral ?? 0,
                Questionario = ObterQuestionario(cargaEntrega, configuracao, unitOfWork),
                MotivosAvaliacao = ObterMotivosAvaliacao(unitOfWork),

                Localizacao = ObterLocalizacao(cargaEntrega, configuracao, unitOfWork),
                Historico = ObterHistoricoEntrega(cargaEntrega, unitOfWork),
            };

            return dataView;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.HistoricoView> ObterHistoricoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> historicoOcorrencia = repOcorrenciaColetaEntrega.BuscarVisiveisAoClientePorCargaEntrega(cargaEntrega.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaEntrega.Pedidos?.FirstOrDefault()?.CargaPedido?.Pedido;

            return (from historico in historicoOcorrencia
                    select new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.HistoricoView
                    {
                        Data = historico.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                        Descricao = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(historico.TipoDeOcorrencia, cargaEntrega.Carga, cargaEntrega.Cliente, pedido?.Remetente),

                    }).ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.MotivosView> ObterMotivosAvaliacao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MotivoAvaliacao repMotivoAvaliacao = new Repositorio.Embarcador.Cargas.MotivoAvaliacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> motivosAvaliacao = repMotivoAvaliacao.BuscarMotivosAtivos();

            return (from motivo in motivosAvaliacao
                    select new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.MotivosView
                    {
                        Valor = motivo.Codigo,
                        Descricao = motivo.Descricao
                    }).ToList();
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.LocalizacaoView ObterLocalizacao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.LocalizacaoView localizacao = new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.LocalizacaoView()
            {
                ExibirMapa = configuracao.ExibirMapa,
                Latitude = 0,
                Longitude = 0
            };

            if (!configuracao.ExibirMapa)
                return localizacao;

            double? latitude;
            double? longitude;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesPedidoEmProcesso = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue,
            };

            if (situacoesPedidoEmProcesso.Contains(cargaEntrega.Situacao))
            {
                Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = cargaEntrega.Carga?.Veiculo?.PosicaoAtual?.FirstOrDefault();

                if (posicaoAtual != null)
                {
                    latitude = posicaoAtual?.Latitude;
                    longitude = posicaoAtual?.Longitude;
                }
                else
                {
                    latitude = (double?)cargaEntrega.Carga.LatitudeInicioViagem;
                    longitude = (double?)cargaEntrega.Carga.LongitudeInicioViagem;
                }
            }
            else
            {
                latitude = (double?)cargaEntrega.LatitudeFinalizada;
                longitude = (double?)cargaEntrega.LongitudeFinalizada;
            }

            localizacao.Latitude = latitude ?? -0;
            localizacao.Longitude = longitude ?? -0;

            if (localizacao.Latitude == 0 && localizacao.Longitude == 0)
            {
                localizacao.ExibirMapa = false;
            }
            else
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                int codigoMonitoramento = repMonitoramento.BuscarCodigoUltimoPorCarga(cargaEntrega.Carga.Codigo);
                if (codigoMonitoramento > 0)
                {
                    localizacao.PolilinhaPlanejada = repMonitoramento.BuscarPolilinhaPlanejada(codigoMonitoramento);
                    localizacao.PolilinhaRealizada = repMonitoramento.BuscarPolilinhaRealizada(codigoMonitoramento);
                }

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(cargaEntrega.Carga.Codigo);
                localizacao.Entregas = new List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaLocalizacaoView>();
                int total = cargaEntregas?.Count() ?? 0;
                for (int i = 0; i < total; i++)
                {
                    localizacao.Entregas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaLocalizacaoView
                    {
                        Ordem = cargaEntregas[i].Ordem,
                        Descricao = cargaEntregas[i].Cliente.Nome,
                        Latitude = double.Parse((cargaEntregas[i].Cliente.Latitude).Replace(",", "."), CultureInfo.InvariantCulture),
                        Longitude = double.Parse((cargaEntregas[i].Cliente.Longitude).Replace(",", "."), CultureInfo.InvariantCulture)
                    });
                }
            }

            if (string.IsNullOrEmpty(localizacao.PolilinhaPlanejada)) localizacao.PolilinhaPlanejada = "";
            if (string.IsNullOrEmpty(localizacao.PolilinhaRealizada)) localizacao.PolilinhaRealizada = "";

            return localizacao;
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.QuestionarioView ObterQuestionario(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            bool situacaoPermiteAvaliacao = cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue;

            Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.QuestionarioView questionario = new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.QuestionarioView()
            {
                HabilitarAvaliacao = configuracao.HabilitarAvaliacao && situacaoPermiteAvaliacao,
                AvaliacaoRespondia = cargaEntrega.DataAvaliacao.HasValue,
                LinkAvaliacaoExterna = configuracao.LinkAvaliacaoExterna,
                HabilitarAvaliacaoQuestionario = configuracao.TipoAvaliacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAvaliacaoPortalCliente.Individual,
                Perguntas = new List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.PerguntasView>() { }
            };

            if (questionario.HabilitarAvaliacaoQuestionario)
            {
                if (cargaEntrega.AvaliacaoGeral.HasValue)
                    questionario.Perguntas = ObterPerguntasRespondidas(cargaEntrega);
                else
                    questionario.Perguntas = ObterPerguntasParaResponder(unitOfWork);
            }

            return questionario;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.PerguntasView> ObterPerguntasRespondidas(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            return (
                from obj in cargaEntrega.Avaliacoes
                select new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.PerguntasView()
                {
                    Codigo = obj.Codigo,
                    Titulo = obj.Titulo,
                    Conteudo = obj.Conteudo,
                    Resposta = obj.Resposta
                }
            ).ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.PerguntasView> ObterPerguntasParaResponder(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao repPortalClientePerguntaAvaliacao = new Repositorio.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao(unitOfWork);

            return (
                from obj in repPortalClientePerguntaAvaliacao.BuscarTodasPerguntas()
                select new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.PerguntasView()
                {
                    Codigo = obj.Codigo,
                    Titulo = obj.Titulo,
                    Conteudo = obj.Conteudo
                }
            ).ToList();
        }

        private string ObterNumerosNotasFiscaisPedido(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorPedidos(cargaEntrega.Pedidos.Select(o => o.CargaPedido.Pedido.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = (from notas in pedidosXMLNotaFiscal select notas.XMLNotaFiscal).Distinct().ToList();
            List<int> numeroNotas = (from nota in notasFiscais select nota.Numero).ToList();

            return string.Join(", ", numeroNotas);
        }

        private string ObterNumeroTransportePedido(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorPedidos(cargaEntrega.Pedidos.Select(o => o.CargaPedido.Pedido.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = (from notas in pedidosXMLNotaFiscal select notas.XMLNotaFiscal).Distinct().ToList();
            List<string> numeroNotas = (from nota in notasFiscais select nota.NumeroTransporte).ToList();

            return string.Join(", ", numeroNotas);
        }

        #endregion
    }
}

