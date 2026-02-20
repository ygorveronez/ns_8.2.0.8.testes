using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Globalization;

namespace SGT.WebAdmin.Controllers.GestaoEntregas
{
    //[CustomAuthorize("GestaoEntregas/SuaEntrega")]
    public class SuaEntregaController : BaseController
    {
        #region Construtores

        public SuaEntregaController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAnonymous]
        public async Task<IActionResult> Rastreamento(string token)
        {
            string caminhoBaseViews = "~/Views/GestaoEntregas/";

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    MontaLayoutBase(unitOfWork);
                    DefineParametrosView(token, unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaView dataView = ObtemDadosRenderizacao(token, unitOfWork);

                    if (dataView == null)
                        return View(caminhoBaseViews + "Acompanhamento/Erro.cshtml");

                    return View(caminhoBaseViews + "Acompanhamento/Detalhe.cshtml", dataView);
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return View(caminhoBaseViews + "Acompanhamento/Erro.cshtml");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Feedback()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                string token = Request.GetStringParam("Token");

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigoRastreamento(token);
                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorPedidoRastreio(pedido.Codigo, pedido.Destinatario.CPF_CNPJ);
                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (cargaEntrega.DataAvaliacao.HasValue)
                    return new JsonpResult(false, true, Localization.Resources.GestaoEntregas.Acompanhamento.AvaliacaoDessePedidoJaFoiFeita);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaAnexo anexo = repositorioAnexo.BuscarPorCodigo(codigo, true);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>(unitOfWork);
                byte[] arquivoBinario = servicoAnexo.DownloadAnexo(anexo, unitOfWork);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, "Não foi possível encontrar o anexo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> SalvarObservacaoAvaliacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                string token = Request.GetStringParam("Token");
                string observacao = Request.GetStringParam("Observacao");

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigoRastreamento(token);
                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorPedidoRastreio(pedido.Codigo, pedido.Destinatario.CPF_CNPJ);
                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (!string.IsNullOrWhiteSpace(cargaEntrega.ObservacaoAvaliacao))
                    return new JsonpResult(false, true, Localization.Resources.GestaoEntregas.Acompanhamento.AvaliacaoDessePedidoJaFoiFeita);

                unitOfWork.Start();

                cargaEntrega.ObservacaoAvaliacao = observacao;

                repositorioCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

                Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao pergunta = perguntasDisponiveis.Where(o => o.Codigo == codigoPergunta).FirstOrDefault() ?? throw new ControllerException(Localization.Resources.GestaoEntregas.Acompanhamento.FalhaAoSalvarAsPerguntas);

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

        private Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaView ObtemDadosRenderizacao(string codigoRastreamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigoRastreamento(codigoRastreamento);

            if (pedido == null)
                return null;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorPedidoRastreio(pedido.Codigo, pedido.Destinatario.CPF_CNPJ);

            if (cargaEntrega == null)
                return null;

            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao = Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaView dataView = new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaView
            {
                Entregue = cargaEntrega.Situacao == SituacaoEntrega.Entregue,
                EnderecoCompleto = ObterEnderecoCompleto(cargaEntrega),
                Numero = pedido.NumeroPedidoEmbarcador,
                Destinatario = cargaEntrega.Cliente?.Nome ?? string.Empty,
                Localidade = cargaEntrega.Cliente?.Localidade.Descricao ?? string.Empty,
                Remetente = ObterRemetente(cargaEntrega),
                Situacao = cargaEntrega.DescricaoSituacao,
                DataIncioViagem = cargaEntrega.Carga.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataEntrega = cargaEntrega.DataFim?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Transportador = cargaEntrega.Carga.Empresa?.Descricao ?? string.Empty,
                Filial = cargaEntrega.Carga.Filial?.Descricao ?? string.Empty,
                NotasFiscais = ObterNumerosNotasFiscaisPedido(pedido.Codigo, unitOfWork),
                NumeroTransporte = ObterNumeroTransportePedido(cargaEntrega, unitOfWork),
                DataPrevisaoChegada = (cargaEntrega.DataReprogramada.HasValue && cargaEntrega.DataReprogramada > cargaEntrega.DataPrevista) ? cargaEntrega.DataReprogramada.Value.ToDateTimeString() : cargaEntrega.DataPrevista?.ToDateTimeString() ?? string.Empty,
                Volumes = configuracao.QuantidadeVolumes ? (cargaEntrega.Pedidos?.Sum(o => o.CargaPedido.Pedido.QtVolumes) ?? 0) : 0,
                PesoLiquido = configuracao.PesoLiquido ? (cargaEntrega.Pedidos?.Sum(o => o?.CargaPedido.Pedido.PesoLiquidoTotal) ?? 0) : 0,
                PesoBruto = configuracao.PesoBruto ? (cargaEntrega.Pedidos?.Sum(o => o?.CargaPedido.Pedido.PesoTotal) ?? 0) : 0,
                QuantidadeVolumesNF = ConfiguracaoEmbarcador.ExibirQuantidadeVolumesNF ? ObterVolumesNF(pedido.Codigo, unitOfWork) : 0,
                Produtos = (
                                from p in pedido.Produtos
                                select new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProduto
                                {
                                    Produto = p.Produto.CodigoProdutoEmbarcador + " - " + p.Produto.Descricao,
                                    Quantidade = p.Quantidade,
                                    PesoTotalEmbalagem = p.PesoTotalEmbalagem,
                                    PesoUnitario = p.PesoUnitario,
                                    PesoTotalProduto = p.PesoTotal,
                                    NumeroLotePedidoProdutoLote = p.Produto.Lotes.Count > 0 ? string.Join(", ", p.Produto.Lotes) : ""
                                }
                            ).ToList(),
                // Previsão de chegada por extenso
                DiaPrevisaoChegada = pedido.CotacaoPedido != null ? cargaEntrega.DataPrevista?.ToString("dd 'de' MMMM") : ((cargaEntrega.DataReprogramada.HasValue && cargaEntrega.DataReprogramada > cargaEntrega.DataPrevista) ? cargaEntrega.DataReprogramada.Value.ToString("dd 'de' MMMM") : cargaEntrega.DataPrevista?.ToString("dd 'de' MMMM")),
                HorarioPrevisaoChegada = pedido.CotacaoPedido != null ? "00:00" : cargaEntrega.DataPrevista?.ToString("HH:mm"),

                NomeMotorista = ObterNomeMotorista(cargaEntrega),
                TelefoneMotorista = string.Empty,//ObterTelefoneMotorista(cargaEntrega),
                CodigoRastreio = cargaEntrega.CodigoRastreio ?? pedido.CodigoRastreamento ?? "",

                Observacao = cargaEntrega.ObservacaoAvaliacao,
                MotivoAvaliacao = cargaEntrega.MotivoAvaliacao?.Descricao ?? string.Empty,
                Avaliacao = cargaEntrega.AvaliacaoGeral ?? 0,
                DataAgenda = pedido.PrevisaoEntrega.HasValue ? pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                Questionario = ObterQuestionario(cargaEntrega, configuracao, unitOfWork),
                Localizacao = ObterLocalizacao(pedido.Codigo, configuracao, unitOfWork),
                Historico = ObterHistoricoEntrega(pedido, cargaEntrega, unitOfWork),
                MotivosAvaliacao = ObterMotivosAvaliacao(unitOfWork),
                PlacaVeiculo = string.Empty,//cargaEntrega.Carga.Veiculo?.Placa ?? "",
                Anexos = ObterAnexos(cargaEntrega.Carga.Codigo, unitOfWork),
                ClienteExterior = ConfiguracaoEmbarcador.Pais != TipoPais.Brasil,
                HabilitarPrevisaoEntrega = configuracao.HabilitarPrevisaoEntrega,
                HabilitarObservacao = configuracao.HabilitarObservacao,
                CodigoRastreioCorreio = pedido.NumeroRastreioCorreios,
                CodigoPedidoCliente = configuracao.HabilitarNumeroPedidoCliente ? pedido.CodigoPedidoCliente ?? string.Empty : string.Empty,
                NumeroOrdemCompra = configuracao.HabilitarNumeroPedidoCliente ? cargaEntrega.Carga?.DadosSumarizados?.NumeroOrdem ?? string.Empty : string.Empty,
                CaminhoFotosEntrega = ObterFotosEntrega(cargaEntrega, unitOfWork),
                PermitirVisualizarFotosEntrega = configuracao.HabilitarVisualizacaoFotosPortal,
                HabilitarAcessoPortalMultiCliFor = configuracao.HabilitarAcessoPortalMultiCliFor,
                LinkAcessoPortalMultiCliFor = configuracao.LinkAcessoPortalMultiCliFor ?? string.Empty,
                ExibirDetalhesPedido = configuracao?.ExibirDetalhesPedido ?? false,
                ExibirHistoricoPedido = configuracao?.ExibirHistoricoPedido ?? false,
                ExibirDetalhesMotorista = configuracao?.ExibirDetalhesMotorista ?? false,
                ExibirDetalhesProduto = configuracao?.ExibirDetalhesProduto ?? false,
                ExibirProduto = configuracao?.ExibirProduto ?? false
            };

            return dataView;
        }

        private static string ObterNomeMotorista(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            return string.Join(", ", from o in cargaEntrega.Carga.Motoristas select o.Nome);
        }

        private static string ObterTelefoneMotorista(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            return string.Join(", ", (from o in cargaEntrega.Carga.Motoristas select o.Telefone_Formatado));
        }

        private string ObterRemetente(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            return (from o in cargaEntrega.Pedidos where o.CargaPedido?.Pedido?.Remetente != null select o.CargaPedido?.Pedido?.Remetente?.Nome).FirstOrDefault();
        }

        private string ObterEnderecoCompleto(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            if (cargaEntrega.ClienteOutroEndereco != null)
            {
                var enderecoCompleto = $"{cargaEntrega.ClienteOutroEndereco.Endereco}";

                if (!string.IsNullOrEmpty(cargaEntrega.ClienteOutroEndereco.Numero))
                {
                    enderecoCompleto += $", {cargaEntrega.ClienteOutroEndereco.Numero} ";
                }

                enderecoCompleto += "&bull; ";

                if (!string.IsNullOrEmpty(cargaEntrega.ClienteOutroEndereco.Complemento))
                {
                    enderecoCompleto += $"{cargaEntrega.ClienteOutroEndereco.Complemento} &bull; ";
                }

                enderecoCompleto += $"{cargaEntrega.ClienteOutroEndereco.Localidade.Descricao} &bull; {cargaEntrega.ClienteOutroEndereco.Localidade?.Estado?.Sigla}";
                return enderecoCompleto;
            }
            else
            {
                var enderecoCompleto = $"{cargaEntrega.Cliente.Endereco}";

                if (!string.IsNullOrEmpty(cargaEntrega.Cliente.Numero))
                {
                    enderecoCompleto += $", {cargaEntrega.Cliente.Numero} ";
                }

                enderecoCompleto += "&bull; ";

                if (!string.IsNullOrEmpty(cargaEntrega.Cliente.Complemento))
                {
                    enderecoCompleto += $"{cargaEntrega.Cliente.Complemento} &bull; ";
                }

                enderecoCompleto += $"{cargaEntrega.Cliente.Localidade.Descricao} &bull; {cargaEntrega.Cliente.Localidade?.Estado?.Sigla}";
                return enderecoCompleto;
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.HistoricoView> ObterHistoricoEntrega(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> historicoOcorrencia = repPedidoOcorrenciaColetaEntrega.BuscarVisiveisAoClientePorPedido(pedido.Codigo);

            return (
                from historico in historicoOcorrencia
                select new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.HistoricoView
                {
                    Data = historico.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                    Descricao = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(historico.TipoDeOcorrencia, cargaEntrega.Carga, cargaEntrega.Cliente, pedido.Remetente),
                    Local = historico.Alvo?.Nome ?? ""
                }
            ).ToList();
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

        private Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.LocalizacaoView ObterLocalizacao(int codigoPedido, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.LocalizacaoView localizacao = new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.LocalizacaoView()
            {
                ExibirMapa = configuracao.ExibirMapa,
                Latitude = 0,
                Longitude = 0
            };

            if (configuracao.ExibirMapa)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarCargaAtualPorPedido(codigoPedido);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarEntregasPorCargaPedido(cargaPedido.Codigo);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaBase = cargaEntregas?.FirstOrDefault();

                if (cargaEntregaBase != null)
                {

                    double? latitude;
                    double? longitude;

                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesPedidoEmProcesso = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue,
                    };

                    if (situacoesPedidoEmProcesso.Contains(cargaEntregaBase.Situacao))
                    {
                        Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = cargaPedido.Carga?.Veiculo?.PosicaoAtual?.FirstOrDefault();

                        if (posicaoAtual != null)
                        {
                            latitude = posicaoAtual?.Latitude;
                            longitude = posicaoAtual?.Longitude;
                        }
                        else
                        {
                            latitude = (double?)cargaEntregaBase.Carga.LatitudeInicioViagem;
                            longitude = (double?)cargaEntregaBase.Carga.LongitudeInicioViagem;
                        }
                    }
                    else
                    {
                        latitude = (double?)cargaEntregaBase.LatitudeFinalizada;
                        longitude = (double?)cargaEntregaBase.LongitudeFinalizada;
                    }

                    localizacao.Latitude = latitude ?? -0;
                    localizacao.Longitude = longitude ?? -0;

                    if (localizacao.Latitude == 0 && localizacao.Longitude == 0)
                    {
                        localizacao.ExibirMapa = false;
                    }
                    else
                    {
                        localizacao.Entregas = cargaEntregas.Select(entrega => new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.EntregaLocalizacaoView
                        {
                            Ordem = entrega.Ordem,
                            Descricao = entrega.Cliente.Nome,
                            Latitude = double.Parse(entrega.Cliente.Latitude.Replace(",", "."), CultureInfo.InvariantCulture),
                            Longitude = double.Parse(entrega.Cliente.Longitude.Replace(",", "."), CultureInfo.InvariantCulture)
                        }).ToList();
                    }
                    if (string.IsNullOrEmpty(localizacao.PolilinhaPlanejada)) localizacao.PolilinhaPlanejada = "";
                    if (string.IsNullOrEmpty(localizacao.PolilinhaRealizada)) localizacao.PolilinhaRealizada = "";
                }
            }
            return localizacao;
        }

        private string ObterNumeroTransportePedido(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorPedidos(cargaEntrega.Pedidos.Select(o => o.CargaPedido.Pedido.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = (from notas in pedidosXMLNotaFiscal select notas.XMLNotaFiscal).Distinct().ToList();
            List<string> numeroNotas = (from nota in notasFiscais where nota?.NumeroTransporte != null select nota.NumeroTransporte).ToList();

            return string.Join(", ", numeroNotas);
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
        private List<string> ObterFotosEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto> anexos = repCargaEntregaFoto.BuscarPorCargaEntrega(cargaEntrega.Codigo);

            List<string> caminhoArquivos = new List<string>();

            if (anexos?.Count() == 0)
                return caminhoArquivos;

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto anexo in anexos)
            {
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
                string extensao = Path.GetExtension(anexo.NomeArquivo);
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extensao);


                if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                {
                    byte[] arquivoBytes = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
                    string base64 = Convert.ToBase64String(arquivoBytes);

                    caminhoArquivos.Add(base64);
                }

            }

            return caminhoArquivos;
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

        private string ObterNumerosNotasFiscaisPedido(int codigoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorPedido(codigoPedido);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = (from notas in pedidosXMLNotaFiscal select notas.XMLNotaFiscal).Distinct().ToList();
            List<int> numeroNotas = (from nota in notasFiscais select nota.Numero).ToList();

            return string.Join(", ", numeroNotas);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.AnexosView> ObterAnexos(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaAnexo> anexos = repositorioAnexo.Consultar(codigoCarga, "", "", 0, 0);

            return (
                from anexo in anexos
                select new Dominio.ObjetosDeValor.Embarcador.GestaoEntregas.AnexosView()
                {
                    CodigoAnexo = anexo.Codigo,
                    DescricaoAnexo = anexo.Descricao,
                    NomeAnexo = anexo.NomeArquivo
                }
            ).ToList();
        }

        private int ObterVolumesNF(int codigoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorPedido(codigoPedido);
            if (pedidosXMLNotaFiscal != null)
            {
                return pedidosXMLNotaFiscal.Select(o => o.XMLNotaFiscal.Volumes).Sum();
            }

            return 0;
        }
    }
}
