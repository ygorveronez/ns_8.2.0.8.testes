using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Servicos.Embarcador.Integracao.Marfrig
{
    public sealed class IntegracaoOrdemEmbarqueMarfrig
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoOrdemEmbarqueMarfrig(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IntegracaoOrdemEmbarqueMarfrig(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellation)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void AdicionarMensagemAlerta(Dominio.Entidades.Embarcador.Cargas.Carga carga, string mensagem)
        {
            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);

            servicoMensagemAlerta.AdicionarMensagem(carga, TipoMensagemAlerta.AlteracaoOrdemEmbarqueNaoIntegrada, mensagem);
        }

        private void AdicionarOrdemEmbarqueHistoricoIntegracao(Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao historicoIntegracao)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(_unitOfWork);
            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao repositorioOrdemEmbarqueHistoricoIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao ordemEmbarqueHistoricoIntegracao = new Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao()
            {
                DataIntegracao = DateTime.Now,
                OrdemEmbarque = repositorioOrdemEmbarque.BuscarPorCodigo(historicoIntegracao.CodigoOrdemEmbarque, auditavel: false),
                PedidoAdicionado = (historicoIntegracao.CodigoPedidoAdicionado > 0) ? repositorioPedido.BuscarPorCodigo(historicoIntegracao.CodigoPedidoAdicionado) : null,
                PedidoRemovido = (historicoIntegracao.CodigoPedidoRemovido > 0) ? repositorioPedido.BuscarPorCodigo(historicoIntegracao.CodigoPedidoRemovido) : null,
                ProblemaIntegracao = historicoIntegracao.ProblemaIntegracao,
                SituacaoIntegracao = historicoIntegracao.SituacaoIntegracao,
                Tipo = historicoIntegracao.TipoHistoricoIntegracao,
                Usuario = (historicoIntegracao.CodigoUsuario > 0) ? repositorioUsuario.BuscarPorCodigo(historicoIntegracao.CodigoUsuario) : null,
            };

            repositorioOrdemEmbarqueHistoricoIntegracao.Inserir(ordemEmbarqueHistoricoIntegracao);
            servicoArquivoTransacao.Adicionar(ordemEmbarqueHistoricoIntegracao, historicoIntegracao.JsonRequisicao, historicoIntegracao.JsonRetorno, "json", historicoIntegracao.TipoArquivoIntegracao);
        }

        private HttpClient CriarRequisicao(string url, string apiKey)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoOrdemEmbarqueMarfrig));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("apikey", apiKey);

            return requisicao;
        }

        private void EnviarEmailAlteracaoOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque, Dominio.Entidades.Usuario usuarioEnviarEmail, string mensagem)
        {
            if ((usuarioEnviarEmail == null) || string.IsNullOrWhiteSpace(usuarioEnviarEmail.Email))
                return;

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (configuracaoEmail == null)
                return;

            string de = configuracaoEmail.Email;
            string usuario = configuracaoEmail.Email;
            string senha = configuracaoEmail.Senha;
            string[] copiaOcultaPara = new string[] { };
            string[] copiaPara = new string[] { };
            string assunto = $"Alteração na Ordem de Embarque {ordemEmbarque.Numero} ";
            string servidorSMTP = configuracaoEmail.Smtp;
            List<System.Net.Mail.Attachment> anexos = null;
            string assinatura = "";
            bool possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
            string responderPara = "";
            int porta = configuracaoEmail.PortaSmtp;
            StringBuilder corpoMensagem = new StringBuilder();

            corpoMensagem.AppendLine(@"<div style=""font-family: Arial;"">");
            corpoMensagem.AppendLine($@"    <p style=""margin:0px"">{mensagem}</p>");
            corpoMensagem.AppendLine($@"    <p style=""font-size: 12px; margin:0px"">{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");
            corpoMensagem.AppendLine("    <p></p>");
            corpoMensagem.AppendLine(@"    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>");
            corpoMensagem.AppendLine("</div>");

            if (!Servicos.Email.EnviarEmail(de, usuario, senha, usuarioEnviarEmail.Email, copiaOcultaPara, copiaPara, assunto, corpoMensagem.ToString(), servidorSMTP, out string erro, configuracaoEmail.DisplayEmail, anexos, assinatura, possuiSSL, responderPara, porta, _unitOfWork))
                Log.TratarErro($"Falha ao enviar o e-mail de alteração na ordem de embarque: {erro}");
        }

        private void EnviarEmailAlteracaoOrdemEmbarqueCancelamento(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque, Dominio.Entidades.Usuario usuarioEnviarEmail, SituacaoIntegracao situacaoIntegracao, string retornoIntegracao)
        {
            StringBuilder mensagem = new StringBuilder();

            mensagem.AppendLine($"Cancelamento {(situacaoIntegracao == SituacaoIntegracao.Integrado ? "APROVADO" : "REPROVADO")}").AppendLine();
            mensagem.AppendLine($"Carga: {ordemEmbarque.Carga.CodigoCargaEmbarcador}");
            mensagem.AppendLine($"Filial: {ordemEmbarque.Carga.Filial.Descricao}");

            if (situacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
            {
                List<string> listaRetornos = retornoIntegracao.Split('|').ToList();

                mensagem.AppendLine();
                mensagem.AppendLine($"Retornos da Integração:");

                foreach (string retorno in listaRetornos)
                    mensagem.AppendLine($" * {retorno.Trim()}");
            }

            EnviarEmailAlteracaoOrdemEmbarque(ordemEmbarque, usuarioEnviarEmail, mensagem.ToString());
        }

        private void EnviarEmailAlteracaoOrdemEmbarqueTrocaPedido(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao ordemEmbarqueHistoricoIntegracao)
        {
            StringBuilder mensagem = new StringBuilder();

            mensagem.AppendLine($"Troca de pedidos {(ordemEmbarqueHistoricoIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado ? "APROVADA" : "REPROVADA")}").AppendLine();
            mensagem.AppendLine($"Carga: {ordemEmbarqueHistoricoIntegracao.OrdemEmbarque.Carga.CodigoCargaEmbarcador}");
            mensagem.AppendLine($"Filial: {ordemEmbarqueHistoricoIntegracao.OrdemEmbarque.Carga.Filial.Descricao}");
            mensagem.AppendLine($"Pedido Adicionado: {ordemEmbarqueHistoricoIntegracao.PedidoAdicionado.NumeroPedidoEmbarcador}");
            mensagem.AppendLine($"Pedido Removido: {ordemEmbarqueHistoricoIntegracao.PedidoRemovido.NumeroPedidoEmbarcador}");

            if (ordemEmbarqueHistoricoIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
            {
                List<string> listaRetornos = ordemEmbarqueHistoricoIntegracao.ProblemaIntegracao.Split('|').ToList();

                mensagem.AppendLine();
                mensagem.AppendLine($"Retornos da Integração:");

                foreach (string retorno in listaRetornos)
                    mensagem.AppendLine($" * {retorno.Trim()}");
            }

            EnviarEmailAlteracaoOrdemEmbarque(ordemEmbarqueHistoricoIntegracao.OrdemEmbarque, ordemEmbarqueHistoricoIntegracao.Usuario, mensagem.ToString());
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMarfrig repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoMarfrig(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null)
                throw new ServicoException("Não existe configuração de integração disponível para a Marfrig.");

            return configuracaoIntegracao;
        }

        private Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ObterOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Filiais.Filial filial, NumeroReboque numeroReboque)
        {
            if (filial == null)
                return null;

            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque = repositorioOrdemEmbarque.BuscarAtivaPorCargaENumeroReboque(carga.Codigo, filial.Codigo, numeroReboque);

            return ordemEmbarque;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ObterOrdensEmbarque(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque = repositorioOrdemEmbarque.BuscarAtivasPorCarga(carga.Codigo);

            return ordensEmbarque;
        }

        private Dominio.Entidades.Veiculo ObterReboque(Dominio.Entidades.Embarcador.Cargas.Carga carga, NumeroReboque numeroReboque)
        {
            if (numeroReboque == NumeroReboque.ReboqueDois)
                return (carga.VeiculosVinculados?.Count > 1) ? carga.VeiculosVinculados.ElementAt(1) : null;

            Dominio.Entidades.Veiculo veiculo = (carga.Veiculo?.IsTipoVeiculoReboque() ?? false) ? carga.Veiculo : null;

            return carga.VeiculosVinculados?.FirstOrDefault() ?? veiculo;
        }

        private Dominio.Entidades.Veiculo ObterTracao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return (carga.Veiculo?.IsTipoVeiculoTracao() ?? false) ? carga.Veiculo : null;
        }

        private Dominio.Entidades.Embarcador.Cargas.TipoIntegracao ObterTipoIntegracaoMarfrig()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoMarfrig = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig);

            return tipoIntegracaoMarfrig;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Integração de Data Reprogramada

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.DataReprogramada ObterDataReprogramada(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.DataReprogramada dataReprogramada = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.DataReprogramada()
            {
                Pedidos = ObterDataReprogramadaPedidos(carga)
            };

            return dataReprogramada;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.DataReprogramadaPedido> ObterDataReprogramadaPedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = carga.CargaAgrupamento ?? carga;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaReferencia.Codigo);
            List<int> codigosCargaPedidos = cargaPedidos.Select(o => o.Codigo).ToList();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.DataReprogramadaPedido> dataReprogramadaPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.DataReprogramadaPedido>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repositorioCargaEntregaPedido.BuscarPorCargaPedidos(codigosCargaPedidos);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregaPedidos.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo && o.CargaEntrega.Coleta == false).Select(o => o.CargaEntrega).FirstOrDefault();

                dataReprogramadaPedidos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.DataReprogramadaPedido()
                {
                    DataReprogramada = !cargaPedido.PedidoPallet ? cargaEntrega?.DataPrevista ?? cargaPedido.Pedido.PrevisaoEntrega : null,
                    NumeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    ProtocoloPedido = cargaPedido.Pedido.Protocolo
                });
            }

            return dataReprogramadaPedidos;
        }

        #endregion Métodos Privados - Integração de Data Reprogramada

        #region Métodos Privados - Integração de Ordem de Embarque

        private Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta EnviarRequisicaoCargaCancelamentoOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque, DateTime? data, Dominio.Entidades.Usuario usuario, string motivo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta();
            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
                string url = configuracaoIntegracao.URLCancelamentoCarga + ordemEmbarque.Numero;
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracao.ApiKey);
                string motivoCancelamento = $"Cancelado em {data?.ToString("dd/MM/yyyy HH:mm") ?? ""}{(usuario != null ? $" por {usuario.Login}" : "")}: {motivo}";

                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.CancelamentoOrdemEmbarque cancelamentoOrdemEmbarque = ObterCancelamentoOrdemEmbarque(ordemEmbarque, motivoCancelamento);
                httpRequisicaoResposta.conteudoRequisicao = JsonConvert.SerializeObject(cancelamentoOrdemEmbarque, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(httpRequisicaoResposta.conteudoRequisicao, Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url) { Content = conteudoRequisicao }).Result;
                httpRequisicaoResposta.conteudoResposta = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarqueSimplificado retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarqueSimplificado>(httpRequisicaoResposta.conteudoResposta);

                httpRequisicaoResposta.httpStatusCode = retornoRequisicao.StatusCode;
                httpRequisicaoResposta.sucesso = (retornoRequisicao.StatusCode == HttpStatusCode.Accepted);

                if (retornoRequisicao.StatusCode == HttpStatusCode.Accepted)
                    httpRequisicaoResposta.mensagem = "Integração realizada e aguardando o retorno da Marfrig";
                else if (retorno == null)
                    httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a Marfrig";
                else
                    httpRequisicaoResposta.mensagem = $"{(string.IsNullOrWhiteSpace(retorno.Mensagem) ? "" : $"{retorno.Mensagem} ")}{(string.IsNullOrWhiteSpace(retorno.Detalhes) ? "" : retorno.Detalhes)}";
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.sucesso = false;
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                httpRequisicaoResposta.sucesso = false;
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a Marfrig";
            }

            return httpRequisicaoResposta;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoOrdemEmbarque ObterAlteracaoOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoOrdemEmbarque()
            {
                Cabecalho = ObterAlteracaoOrdemEmbarqueCabecalho(ordemEmbarque),
                Frete = ObterAlteracaoOrdemEmbarqueFrete(ordemEmbarque)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoOrdemEmbarqueCabecalho ObterAlteracaoOrdemEmbarqueCabecalho(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(ordemEmbarque.Carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = ordemEmbarque.Carga.CargaAgrupamento ?? ordemEmbarque.Carga;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoOrdemEmbarqueCabecalho()
            {
                DataProgramacao = cargaJanelaCarregamento?.InicioCarregamento ?? DateTime.Now,
                EmpresaCnpj = ordemEmbarque.Carga.Filial?.CNPJ ?? "",
                NumeroOrdemEmbarque = ordemEmbarque.Numero.ToInt(),
                ProtocoloCarga = ordemEmbarque.Carga.Protocolo,
                UsuarioAD = cargaReferencia.Operador?.Login ?? ""
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoOrdemEmbarqueFrete ObterAlteracaoOrdemEmbarqueFrete(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = ordemEmbarque.Carga.CargaAgrupamento ?? ordemEmbarque.Carga;
            Dominio.Entidades.Usuario motorista = repositorioCargaMotorista.BuscarPrimeiroMotoristaPorCarga(cargaReferencia.Codigo);
            Dominio.Entidades.Veiculo tracao = ObterTracao(cargaReferencia);
            Dominio.Entidades.Veiculo reboque = ObterReboque(cargaReferencia, ordemEmbarque.NumeroReboque);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoOrdemEmbarqueFrete()
            {
                MotoristaCpf = motorista?.CPF ?? "",
                TransportadorCnpj = cargaReferencia.Empresa?.CNPJ_SemFormato ?? "",
                VeiculoDolly = "",
                VeiculoReboque = reboque?.Placa ?? "",
                VeiculoTracao = tracao?.Placa ?? ""
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.CancelamentoOrdemEmbarque ObterCancelamentoOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque, string motivo)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.CancelamentoOrdemEmbarque()
            {
                Cabecalho = ObterCancelamentoOrdemEmbarqueCabecalho(ordemEmbarque, motivo),
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.CancelamentoOrdemEmbarqueCabecalho ObterCancelamentoOrdemEmbarqueCabecalho(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque, string motivo)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = ordemEmbarque.Carga.CargaAgrupamento ?? ordemEmbarque.Carga;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.CancelamentoOrdemEmbarqueCabecalho()
            {
                EmpresaCnpj = ordemEmbarque.Carga.Filial?.CNPJ ?? "",
                Motivo = motivo,
                NumeroOrdemEmbarque = ordemEmbarque.Numero.ToInt(),
                ProtocoloCarga = ordemEmbarque.Carga.Protocolo,
                UsuarioAD = cargaReferencia.Operador?.Login ?? ""
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarque ObterOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarque()
            {
                Cabecalho = ObterOrdemEmbarqueCabecalho(carga, cargaPedidos),
                Frete = ObterOrdemEmbarqueFrete(carga),
                Pedidos = ObterOrdemEmbarquePedidos(carga, cargaPedidos)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueCabecalho ObterOrdemEmbarqueCabecalho(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = carga.CargaAgrupamento ?? carga;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(cargaReferencia.Codigo);
            TipoCondicaoPagamento tipoCondicaoPagamento = carga.Carregamento?.TipoCondicaoPagamento ?? TipoCondicaoPagamento.CIF;
            Dominio.Entidades.Cliente recebedor = cargaPedidos.Where(x => x.Recebedor != null).Select(x => x.Recebedor).FirstOrDefault();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueCabecalho()
            {
                DataProgramacao = cargaJanelaCarregamento?.InicioCarregamento ?? DateTime.Now,
                EmpresaCnpj = carga.Filial?.CNPJ ?? "",
                RecebedorCNPJ = recebedor?.CPF_CNPJ_SemFormato,
                GerarValePedagio = tipoCondicaoPagamento == TipoCondicaoPagamento.CIF,
                ProtocoloCarga = carga.Protocolo,
                RetornoViagem = false,
                UsuarioAD = cargaReferencia.Operador?.Login ?? ""
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueFrete ObterOrdemEmbarqueFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = carga.CargaAgrupamento ?? carga;
            Dominio.Entidades.Veiculo tracao = ObterTracao(cargaReferencia);
            TipoCondicaoPagamento tipoCondicaoPagamento = carga.Carregamento?.TipoCondicaoPagamento ?? TipoCondicaoPagamento.CIF;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueFrete()
            {
                MotoristaCpf = cargaReferencia.Motoristas?.FirstOrDefault()?.CPF ?? "",
                TipoFrete = (int)tipoCondicaoPagamento,
                TransportadorCnpj = cargaReferencia.Empresa?.CNPJ_SemFormato ?? "",
                VeiculoDolly = "",
                VeiculoTracao = tracao?.Placa ?? ""
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarquePedido> ObterOrdemEmbarquePedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = carga.CargaAgrupamento ?? carga;
            IEnumerable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosOrdenados = cargaPedidos.OrderBy(cargaPedido => cargaPedido.OrdemEntrega);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarquePedido> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarquePedido>();
            Dominio.Entidades.Veiculo tracao = ObterTracao(cargaReferencia);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosOrdenados)
            {
                if (cargaPedido.Pedido.Filial.Codigo != carga.Filial.Codigo)
                    continue;

                Dominio.Entidades.Veiculo reboque = ObterReboque(cargaReferencia, cargaPedido.NumeroReboque);

                pedidos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarquePedido()
                {
                    EmpresaCnpj = cargaPedido.Pedido.Filial?.CNPJ ?? "",
                    MotoristaCpf = cargaReferencia.Motoristas?.FirstOrDefault()?.CPF ?? "",
                    NumeroBau = cargaPedido.NumeroReboque == NumeroReboque.ReboqueDois ? 2 : 1,
                    NumeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    PlacaVeiculo = reboque?.Placa ?? tracao?.Placa ?? "",
                    ProtocoloPedido = cargaPedido.Pedido.Protocolo
                });
            }

            return pedidos;
        }

        #endregion Métodos Privados - Integração de Ordem de Embarque

        #region Métodos Privados - Integração de Pedidos

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoPedido ObterAlteracaoPedido(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoPedido()
            {
                Cabecalho = ObterAlteracaoPedidoCabecalho(ordemEmbarque),
                Pedidos = ObterAlteracaoPedidoPedidos(pedido)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoPedidoCabecalho ObterAlteracaoPedidoCabecalho(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = ordemEmbarque.Carga.CargaAgrupamento ?? ordemEmbarque.Carga;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoPedidoCabecalho()
            {
                EmpresaCnpj = ordemEmbarque.Carga.Filial?.CNPJ ?? "",
                NumeroOrdemEmbarque = ordemEmbarque.Numero.ToInt(),
                ProtocoloCarga = ordemEmbarque.Carga.Protocolo,
                UsuarioAD = cargaReferencia.Operador?.Login ?? ""
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoPedidoPedido> ObterAlteracaoPedidoPedidos(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoPedidoPedido> pedidosAdicionados = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoPedidoPedido>();

            pedidosAdicionados.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoPedidoPedido()
            {
                EmpresaCnpj = pedido.Filial?.CNPJ ?? "",
                NumeroPedido = pedido.NumeroPedidoEmbarcador,
                ProtocoloPedido = pedido.Protocolo
            });

            return pedidosAdicionados;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.TrocaPedido ObterTrocaPedido(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoDe, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoPara)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.TrocaPedido()
            {
                Cabecalho = ObterTrocaPedidoCabecalho(ordemEmbarque),
                DePara = ObterTrocaPedidoDePara(pedidoDe, pedidoPara)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.TrocaPedidoCabecalho ObterTrocaPedidoCabecalho(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = ordemEmbarque.Carga.CargaAgrupamento ?? ordemEmbarque.Carga;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.TrocaPedidoCabecalho()
            {
                EmpresaCnpj = ordemEmbarque.Carga.Filial?.CNPJ ?? "",
                NumeroOrdemEmbarque = ordemEmbarque.Numero.ToInt(),
                ProtocoloCarga = ordemEmbarque.Carga.Protocolo,
                UsuarioAD = cargaReferencia.Operador?.Login ?? ""
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.TrocaPedidoDePara ObterTrocaPedidoDePara(Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoDe, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoPara)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.TrocaPedidoDePara()
            {
                EmpresaPedidoDeCnpj = pedidoDe.Filial?.CNPJ ?? "",
                EmpresaPedidoParaCnpj = pedidoPara.Filial?.CNPJ ?? "",
                NumeroPedidoDe = pedidoDe.NumeroPedidoEmbarcador,
                NumeroPedidoPara = pedidoPara.NumeroPedidoEmbarcador,
                ProtocoloPedidoDe = pedidoDe.Protocolo,
                ProtocoloPedidoPara = pedidoPara.Protocolo
            };
        }

        #endregion Métodos Privados - Integração de Pedidos

        #region Métodos Públicos

        public void AdicionarCargaIntegracaoOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!carga.CargaDePreCarga)
                return;

            if (!PossuiIntegracaoOrdemEmbarque(carga.Filial?.Codigo ?? 0, carga.ModeloVeicularCarga?.Codigo ?? 0))
                return;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoMarfrig = ObterTipoIntegracaoMarfrig();

            if (tipoIntegracaoMarfrig == null)
                return;

            Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorioCargaOrdemEmbarqueIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao cargaOrdemEmbarqueIntegracao = new Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao()
            {
                Carga = carga,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                TipoIntegracao = tipoIntegracaoMarfrig
            };

            repositorioCargaOrdemEmbarqueIntegracao.Inserir(cargaOrdemEmbarqueIntegracao);
        }

        public void AtualizarIntegracaoCancelamentoOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.ConfirmacaoOrdemEmbarqueRequest cancelamentoOrdemEmbarqueRetorno)
        {
            DateTime dataRetorno = cancelamentoOrdemEmbarqueRetorno.DataHora.ToNullableDateTime() ?? throw new ServicoException("A data não está em um formato correto (dd/MM/yyyy HH:mm:ss)");

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(cancelamentoOrdemEmbarqueRetorno.ProtocoloTMSCarga) ?? throw new ServicoException("A carga informada não existe no Multi Embarcador");

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao = repositorioCargaCancelamentoCargaIntegracao.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig);

            if (cargaCancelamentoCargaIntegracao != null)
            {
                if (cargaCancelamentoCargaIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                    throw new ServicoException($"A situação da integração do cancelamento da ordem de embarque ({cargaCancelamentoCargaIntegracao.DescricaoSituacaoIntegracao}) não permite a atualização");

                ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

                if (!cancelamentoOrdemEmbarqueRetorno.Validado)
                {
                    cargaCancelamentoCargaIntegracao.ProblemaIntegracao = (cancelamentoOrdemEmbarqueRetorno.Validacoes?.Count > 0) ? string.Join(" | ", cancelamentoOrdemEmbarqueRetorno.Validacoes.Select(o => o.Mensagem)) : "";
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Cancelamento da ordem de embarque integrado com sucesso";
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }

                cargaCancelamentoCargaIntegracao.DataIntegracao = dataRetorno;

                servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, string.Empty, JsonConvert.SerializeObject(cancelamentoOrdemEmbarqueRetorno, Formatting.Indented), "json", TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento);
                repositorioCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);

                List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque = ObterOrdensEmbarque(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga);

                foreach (Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque in ordensEmbarque)
                    EnviarEmailAlteracaoOrdemEmbarqueCancelamento(ordemEmbarque, cargaCancelamentoCargaIntegracao.CargaCancelamento.Usuario, cargaCancelamentoCargaIntegracao.SituacaoIntegracao, cargaCancelamentoCargaIntegracao.ProblemaIntegracao);
            }
            else
            {
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(_unitOfWork);
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao repositorioOrdemEmbarqueSituacao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque = repositorioOrdemEmbarque.BuscarAtivaPorCargaENumero(carga.Codigo, cancelamentoOrdemEmbarqueRetorno.NumeroOrdemEmbarque);

                if (ordemEmbarque == null)
                    throw new ServicoException($"Ordem de embarque ({cancelamentoOrdemEmbarqueRetorno.NumeroOrdemEmbarque}) não encontrada.");

                if (!repositorioOrdemEmbarqueSituacao.SituacaoEhEmCancelamento(ordemEmbarque.Situacao))
                    throw new ServicoException($"A ordem de embarque ({cancelamentoOrdemEmbarqueRetorno.NumeroOrdemEmbarque}) não está aguardando confirmação de cancelamento.");

                ordemEmbarque.Situacao = repositorioOrdemEmbarqueSituacao.BuscarPorCodigoIntegracao(repositorioOrdemEmbarqueSituacao.GetCodigosIntegracaoSituacaoOrdemCancelada().First());

                repositorioOrdemEmbarque.Atualizar(ordemEmbarque);

                EnviarEmailAlteracaoOrdemEmbarqueCancelamento(ordemEmbarque, ordemEmbarque.ResponsavelCancelamento, SituacaoIntegracao.Integrado, retornoIntegracao: string.Empty);
            }
        }

        public void AtualizarIntegracaoCargaOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.OrdemEmbarqueRetorno ordemEmbarqueRetorno)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(ordemEmbarqueRetorno.ProtocoloIntegracaoCarga) ?? throw new ServicoException("A carga informada não existe no Multi Embarcador");

            Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorioCargaOrdemEmbarqueIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao cargaOrdemEmbarqueIntegracao = repositorioCargaOrdemEmbarqueIntegracao.BuscarPorCarga(carga.Codigo);

            if (cargaOrdemEmbarqueIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                throw new ServicoException($"A situação da integração da ordem de embarque ({cargaOrdemEmbarqueIntegracao.DescricaoSituacaoIntegracao}) não permite a atualização");

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            DateTime dataRetorno = ordemEmbarqueRetorno.Data.ToNullableDateTime() ?? throw new ServicoException("A data não está em um formato correto (dd/MM/yyyy HH:mm:ss)");

            if (!ordemEmbarqueRetorno.Validada)
            {
                cargaOrdemEmbarqueIntegracao.ProblemaIntegracao = (ordemEmbarqueRetorno.Validacoes?.Count > 0) ? string.Join(" | ", ordemEmbarqueRetorno.Validacoes.Select(o => o.Mensagem)) : "";
                cargaOrdemEmbarqueIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            else if ((ordemEmbarqueRetorno.OrdensEmbarque?.Count ?? 0) == 0)
            {
                cargaOrdemEmbarqueIntegracao.ProblemaIntegracao = "Nenhuma ordem de embarque foi retornada";
                cargaOrdemEmbarqueIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            else
            {
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(_unitOfWork);

                foreach (Dominio.ObjetosDeValor.WebService.OrdemEmbarque.OrdemEmbarque ordemEmbarque in ordemEmbarqueRetorno.OrdensEmbarque)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = carga.CargaAgrupamento ?? carga;

                    if (string.IsNullOrWhiteSpace(ordemEmbarque.Numero))
                        throw new ServicoException($"O número da ordem de embarque deve ser informado");

                    if (repositorioOrdemEmbarque.BuscarAtivaPorCargaENumero(carga.Codigo, ordemEmbarque.Numero) != null)
                        throw new ServicoException($"A ordem de embarque {ordemEmbarque.Numero} já foi adicionada");

                    if (string.IsNullOrWhiteSpace(ordemEmbarque.Placa))
                        throw new ServicoException($"A placa da ordem de embarque {ordemEmbarque.Numero} deve ser informada");

                    Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarqueAdicionar = new Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque()
                    {
                        Carga = carga,
                        Numero = ordemEmbarque.Numero
                    };

                    bool exigirDefinicaoReboquePedido = (cargaReferencia.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false) && (cargaReferencia.ModeloVeicularCarga?.NumeroReboques > 1);

                    if ((cargaReferencia.VeiculosVinculados?.Count > 1) && (cargaReferencia.VeiculosVinculados.ElementAt(1).Placa == ordemEmbarque.Placa))
                    {
                        ordemEmbarqueAdicionar.NumeroReboque = exigirDefinicaoReboquePedido ? NumeroReboque.ReboqueDois : NumeroReboque.SemReboque;
                        ordemEmbarqueAdicionar.Veiculo = cargaReferencia.VeiculosVinculados.ElementAt(1);
                    }
                    else if ((cargaReferencia.Veiculo != null) && (cargaReferencia.Veiculo.Placa == ordemEmbarque.Placa))
                    {
                        ordemEmbarqueAdicionar.NumeroReboque = NumeroReboque.SemReboque;
                        ordemEmbarqueAdicionar.Veiculo = cargaReferencia.Veiculo;
                    }
                    else if (cargaReferencia.VeiculosVinculados != null)
                    {
                        ordemEmbarqueAdicionar.NumeroReboque = exigirDefinicaoReboquePedido ? NumeroReboque.ReboqueUm : NumeroReboque.SemReboque;
                        ordemEmbarqueAdicionar.Veiculo = (from o in cargaReferencia.VeiculosVinculados where o.Placa == ordemEmbarque.Placa select o).FirstOrDefault();
                    }

                    if (ordemEmbarqueAdicionar.Veiculo == null)
                        throw new ServicoException($"O veículo {ordemEmbarque.Placa} não existe na carga");

                    repositorioOrdemEmbarque.Inserir(ordemEmbarqueAdicionar);
                }

                cargaOrdemEmbarqueIntegracao.ProblemaIntegracao = "Ordem de embarque integrada com sucesso";
                cargaOrdemEmbarqueIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }

            cargaOrdemEmbarqueIntegracao.DataIntegracao = dataRetorno;

            servicoArquivoTransacao.Adicionar(cargaOrdemEmbarqueIntegracao, string.Empty, JsonConvert.SerializeObject(ordemEmbarqueRetorno), "json", TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento);
            repositorioCargaOrdemEmbarqueIntegracao.Atualizar(cargaOrdemEmbarqueIntegracao);
        }

        public void AtualizarIntegracaoTrocaPedido(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.ConfirmacaoTrocaPedidoOrdemEmbarqueRequest trocaPedidoOrdemEmbarqueRetorno, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao repositorioOrdemEmbarqueHistoricoIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoTroca repositorioPedidoTroca = new Repositorio.Embarcador.Pedidos.PedidoTroca(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(trocaPedidoOrdemEmbarqueRetorno.ProtocoloTMSCarga) ?? throw new ServicoException("A carga informada não existe no Multi Embarcador");
            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = carga.CargaAgrupamento ?? carga;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAdicionado = repositorioCargaPedido.BuscarPedidoPorCargaENumeroPedidoEmbarcador(cargaReferencia.Codigo, trocaPedidoOrdemEmbarqueRetorno.NumeroPedidoPara) ?? throw new ServicoException($"O pedido {trocaPedidoOrdemEmbarqueRetorno.NumeroPedidoPara} não existe no Multi Embarcador");
            Dominio.Entidades.Embarcador.Pedidos.PedidoTroca pedidoTroca = repositorioPedidoTroca.BuscarPrimeiroPorPedidoDefinitivo(pedidoAdicionado.Codigo) ?? throw new ServicoException($"O pedido {trocaPedidoOrdemEmbarqueRetorno.NumeroPedidoDe} não existe no Multi Embarcador");
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoRemovido = pedidoTroca.PedidoProvisorio;
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao ordemEmbarqueHistoricoIntegracao = repositorioOrdemEmbarqueHistoricoIntegracao.BuscarUltimoPorTrocaPedido(carga.Codigo, pedidoRemovido.Codigo, trocaPedidoOrdemEmbarqueRetorno.NumeroOrdemEmbarque) ?? throw new ServicoException($"Nenhuma integração de troca de pedido em ordem de embarque encontrada para a carga e pedido informados.");

            if (ordemEmbarqueHistoricoIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                throw new ServicoException($"A situação da integração da troca de pedido a ordem de embarque ({ordemEmbarqueHistoricoIntegracao.SituacaoIntegracao.ObterDescricao()}) não permite a atualização");

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            DateTime dataRetorno = trocaPedidoOrdemEmbarqueRetorno.DataHora.ToNullableDateTime() ?? throw new ServicoException("A data não está em um formato correto (dd/MM/yyyy HH:mm:ss)");

            if (!trocaPedidoOrdemEmbarqueRetorno.Validado)
            {
                ordemEmbarqueHistoricoIntegracao.ProblemaIntegracao = (trocaPedidoOrdemEmbarqueRetorno.Validacoes?.Count > 0) ? string.Join(" | ", trocaPedidoOrdemEmbarqueRetorno.Validacoes.Select(o => o.Mensagem)) : "";
                ordemEmbarqueHistoricoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                Carga.CargaPedido.DesfazerTrocaPedidoCarga(cargaReferencia, pedidoAdicionado, tipoServicoMultisoftware, auditado, _unitOfWork);
            }
            else
            {
                ordemEmbarqueHistoricoIntegracao.ProblemaIntegracao = "Troca de pedido da ordem de embarque integrada com sucesso";
                ordemEmbarqueHistoricoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                if (!pedidoRemovido.Provisorio)
                {
                    pedidoRemovido.PedidoTotalmenteCarregado = false;
                    repositorioPedido.Atualizar(pedidoRemovido);
                    repositorioPedidoTroca.Deletar(pedidoTroca);
                }
            }

            ordemEmbarqueHistoricoIntegracao.DataIntegracao = dataRetorno;

            servicoArquivoTransacao.Adicionar(ordemEmbarqueHistoricoIntegracao, string.Empty, JsonConvert.SerializeObject(trocaPedidoOrdemEmbarqueRetorno, Formatting.Indented), "json", TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento);
            repositorioOrdemEmbarqueHistoricoIntegracao.Atualizar(ordemEmbarqueHistoricoIntegracao);
            EnviarEmailAlteracaoOrdemEmbarqueTrocaPedido(ordemEmbarqueHistoricoIntegracao);
        }

        public void AtualizarSituacaoOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.OrdemEmbarqueSituacaoRetorno ordemEmbarqueSituacaoRetorno)
        {
            DateTime? dataAtualizacaoSituacao = ordemEmbarqueSituacaoRetorno.DataAtualizacao.ToNullableDateTime();

            if (!dataAtualizacaoSituacao.HasValue)
                throw new ServicoException("A data não está em um formato correto (dd/MM/yyyy HH:mm:ss)");

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(ordemEmbarqueSituacaoRetorno.ProtocoloIntegracaoCarga) ?? throw new ServicoException("A carga informada não existe no Multi Embarcador");

            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque = repositorioOrdemEmbarque.BuscarAtivaPorCargaENumero(carga.Codigo, ordemEmbarqueSituacaoRetorno.NumeroOrdemEmbarque) ?? throw new ServicoException("A ordem de embarque informada não existe no Multi Embarcador");

            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao repositorioOrdemEmbarqueSituacao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao ordemEmbarqueSituacao = repositorioOrdemEmbarqueSituacao.BuscarPorCodigoIntegracao(ordemEmbarqueSituacaoRetorno.ProtocoloSituacao) ?? throw new ServicoException("A situação informada não existe no Multi Embarcador");

            ordemEmbarque.Situacao = ordemEmbarqueSituacao;

            repositorioOrdemEmbarque.Atualizar(ordemEmbarque);

            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico repositorioOrdemEmbarqueSituacaoHistorico = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico ordemEmbarqueSituacaoHistorico = new Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico()
            {
                DataAtualizacao = dataAtualizacaoSituacao.Value,
                DataCriacao = DateTime.Now,
                OrdemEmbarque = ordemEmbarque,
                Situacao = ordemEmbarqueSituacao
            };

            repositorioOrdemEmbarqueSituacaoHistorico.Inserir(ordemEmbarqueSituacaoHistorico);
        }

        public void IntegrarAlteracaoOrdensEmbarque(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario usuario)
        {
            List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque = ObterOrdensEmbarque(carga);

            if (ordensEmbarque.Count == 0)
                return;

            if (!carga.CargaDePreCarga)
            {
                AdicionarMensagemAlerta(carga, "Alterações nos dados de transporte não foram integradas");
                return;
            }

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao> historicosIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao>();

            foreach (Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque in ordensEmbarque)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao historicoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao()
                {
                    CodigoOrdemEmbarque = ordemEmbarque.Codigo,
                    CodigoUsuario = usuario?.Codigo ?? 0,
                    TipoArquivoIntegracao = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                    TipoHistoricoIntegracao = TipoOrdemEmbarqueHistoricoIntegracao.AlteracaoOrdemEmbarque
                };

                try
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
                    string url = configuracaoIntegracao.URLIntegracaoAlteraCargaOrdemEmbarque + ordemEmbarque.Numero;
                    HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracao.ApiKey);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoOrdemEmbarque alteracaoOrdemEmbarque = ObterAlteracaoOrdemEmbarque(ordemEmbarque);
                    historicoIntegracao.JsonRequisicao = JsonConvert.SerializeObject(alteracaoOrdemEmbarque, Formatting.Indented);
                    StringContent conteudoRequisicao = new StringContent(historicoIntegracao.JsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                    HttpResponseMessage retornoRequisicao = requisicao.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = conteudoRequisicao }).Result;
                    historicoIntegracao.JsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarque retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarque>(historicoIntegracao.JsonRetorno);

                    if (!(retorno?.Validado ?? false))
                        throw new ServicoException($"Falha ao integrar a alteração da ordem de embarque {ordemEmbarque.Numero} com a Marfrig{(string.IsNullOrWhiteSpace(retorno?.Mensagem) ? "" : $": {retorno.Mensagem}")}");

                    historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    historicoIntegracao.ProblemaIntegracao = "Alteração da ordem de embarque integrada com sucesso";
                }
                catch (ServicoException excecao)
                {
                    historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    historicoIntegracao.ProblemaIntegracao = excecao.Message;
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);

                    historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    historicoIntegracao.ProblemaIntegracao = $"Ocorreu uma falha ao realizar a integração da ordem de embarque {ordemEmbarque.Numero} com a Marfrig";
                }

                historicosIntegracao.Add(historicoIntegracao);
            }

            string problemasIntegracao = string.Join(" | ", (from o in historicosIntegracao where o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao select o.ProblemaIntegracao));

            if (!string.IsNullOrWhiteSpace(problemasIntegracao))
            {
                _unitOfWork.Rollback();
                _unitOfWork.Start();

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao historicoIntegracao in historicosIntegracao)
                    AdicionarOrdemEmbarqueHistoricoIntegracao(historicoIntegracao);

                _unitOfWork.CommitChanges();

                throw new ServicoException(problemasIntegracao);
            }
            else
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao historicoIntegracao in historicosIntegracao)
                    AdicionarOrdemEmbarqueHistoricoIntegracao(historicoIntegracao);
            }
        }

        public string IntegrarCancelamentoOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque, string motivo, Dominio.Entidades.Usuario usuario)
        {
            if (ordemEmbarque == null)
                throw new ServicoException("Ordem de embarque não encontrada.");

            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao repositorioOrdemEmbarqueSituacao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao(_unitOfWork);

            if (repositorioOrdemEmbarqueSituacao.SituacaoEhCancelada(ordemEmbarque.Situacao))
                throw new ServicoException("A ordem de embarque já foi cancelada.");

            if (repositorioOrdemEmbarqueSituacao.SituacaoEhEmCancelamento(ordemEmbarque.Situacao))
                throw new ServicoException("O cancelamento desta ordem de embarque já foi solicitado, aguardando confirmação.");

            DateTime dataCancelamento = DateTime.Now;
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = EnviarRequisicaoCargaCancelamentoOrdemEmbarque(ordemEmbarque, dataCancelamento, usuario, motivo);

            if (!httpRequisicaoResposta.sucesso)
                throw new ServicoException(httpRequisicaoResposta.mensagem);

            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(_unitOfWork);
            Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorioCargaOrdemEmbarqueIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao cargaOrdemEmbarqueIntegracao = repositorioCargaOrdemEmbarqueIntegracao.BuscarPorCarga(ordemEmbarque.Carga.Codigo);

            ordemEmbarque.ResponsavelCancelamento = usuario;
            ordemEmbarque.Situacao = repositorioOrdemEmbarqueSituacao.BuscarPorCodigoIntegracao(repositorioOrdemEmbarqueSituacao.GetCodigosIntegracaoSituacaoOrdemEmCancelamento().First());
            cargaOrdemEmbarqueIntegracao.Cancelada = true;

            repositorioOrdemEmbarque.Atualizar(ordemEmbarque);
            repositorioCargaOrdemEmbarqueIntegracao.Atualizar(cargaOrdemEmbarqueIntegracao);

            return httpRequisicaoResposta.mensagem;
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas += 1;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
                string url = configuracaoIntegracao.URLIntegracaoFinalizaCargaOrdemEmbarque;
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracao.ApiKey);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.DataReprogramada dataReprogramada = ObterDataReprogramada(cargaIntegracao.Carga);
                jsonRequisicao = JsonConvert.SerializeObject(dataReprogramada, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarqueSimplificado retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarqueSimplificado>(jsonRetorno);

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created))
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso";
                }
                else
                {
                    cargaIntegracao.ProblemaIntegracao = (retorno == null) ? "Ocorreu uma falha ao realizar a integração com a Marfrig" : $"{(string.IsNullOrWhiteSpace(retorno.Mensagem) ? "" : $"{retorno.Mensagem} ")}{(string.IsNullOrWhiteSpace(retorno.Detalhes) ? "" : retorno.Detalhes)}";
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Marfrig";

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarCargaCancelamentoOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque = ObterOrdensEmbarque(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = ordensEmbarque.Select(o => o.Carga).Distinct().ToList();
            string mensagemErro = string.Empty;
            string mensagemSucesso = string.Empty;

            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaIntegracao.NumeroTentativas += 1;

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque primeiraOrdemEmbarquePorCarga = ordensEmbarque.Where(o => o.Carga.Codigo == carga.Codigo).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta retornoRequisicao = EnviarRequisicaoCargaCancelamentoOrdemEmbarque(primeiraOrdemEmbarquePorCarga, cargaCancelamentoCargaIntegracao.CargaCancelamento.DataCancelamento, cargaCancelamentoCargaIntegracao.CargaCancelamento.Usuario, cargaCancelamentoCargaIntegracao.CargaCancelamento.MotivoCancelamento);

                if (retornoRequisicao.sucesso)
                    mensagemSucesso = retornoRequisicao.mensagem;
                else
                    mensagemErro = retornoRequisicao.mensagem;

                servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, retornoRequisicao.conteudoRequisicao, retornoRequisicao.conteudoResposta, "json", retornoRequisicao.mensagem);
            }

            if (!string.IsNullOrWhiteSpace(mensagemErro))
            {
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mensagemErro;
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            else
            {
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mensagemSucesso;
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
            }

            repositorioCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        public void IntegrarCargaOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao cargaOrdemEmbarqueIntegracao)
        {
            Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorioCargaOrdemEmbarqueIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaOrdemEmbarqueIntegracao.Cancelada = false;
            cargaOrdemEmbarqueIntegracao.DataIntegracao = DateTime.Now;
            cargaOrdemEmbarqueIntegracao.NumeroTentativas += 1;
            cargaOrdemEmbarqueIntegracao.TipoFalhaIntegracao = TipoFalhaCargaOrdemEmbarqueIntegracao.NaoDefinida;

            try
            {
                new Veiculo.LicencaVeiculo(_unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador).ValidarCargaLicenca(cargaOrdemEmbarqueIntegracao.Carga);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
                string url = configuracaoIntegracao.URLIntegracaoCadastraCargaOrdemEmbarque;
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracao.ApiKey);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarque ordemEmbarque = ObterOrdemEmbarque(cargaOrdemEmbarqueIntegracao.Carga);
                jsonRequisicao = JsonConvert.SerializeObject(ordemEmbarque, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarqueSimplificado retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarqueSimplificado>(jsonRetorno);

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created))
                {
                    cargaOrdemEmbarqueIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                    cargaOrdemEmbarqueIntegracao.ProblemaIntegracao = "Integração realizada e aguardando o retorno da Marfrig";
                }
                else
                {
                    cargaOrdemEmbarqueIntegracao.ProblemaIntegracao = (retorno == null) ? "Ocorreu uma falha ao realizar a integração com a Marfrig" : $"{(string.IsNullOrWhiteSpace(retorno.Mensagem) ? "" : $"{retorno.Mensagem} ")}{(string.IsNullOrWhiteSpace(retorno.Detalhes) ? "" : retorno.Detalhes)}";
                    cargaOrdemEmbarqueIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(cargaOrdemEmbarqueIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                cargaOrdemEmbarqueIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaOrdemEmbarqueIntegracao.ProblemaIntegracao = excecao.Message;

                if (excecao.ErrorCode == CodigoExcecao.LicencaInvalida)
                    cargaOrdemEmbarqueIntegracao.TipoFalhaIntegracao = TipoFalhaCargaOrdemEmbarqueIntegracao.LicencaInvalida;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaOrdemEmbarqueIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaOrdemEmbarqueIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Marfrig";

                servicoArquivoTransacao.Adicionar(cargaOrdemEmbarqueIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioCargaOrdemEmbarqueIntegracao.Atualizar(cargaOrdemEmbarqueIntegracao);
        }

        public void IntegrarPedidoAdicionado(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, NumeroReboque numeroReboque, Dominio.Entidades.Usuario usuario)
        {
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque = ObterOrdemEmbarque(carga, pedido.Filial, numeroReboque);

            if (ordemEmbarque == null)
                return;

            if (!carga.CargaDePreCarga)
            {
                AdicionarMensagemAlerta(carga, $"Adição do pedido {pedido.NumeroPedidoEmbarcador} não foi integrada");
                return;
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao historicoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao()
            {
                CodigoOrdemEmbarque = ordemEmbarque.Codigo,
                CodigoPedidoAdicionado = pedido.Codigo,
                CodigoUsuario = usuario.Codigo,
                TipoArquivoIntegracao = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                TipoHistoricoIntegracao = TipoOrdemEmbarqueHistoricoIntegracao.AdicaoPedido
            };

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
                string url = configuracaoIntegracao.URLIntegracaoAdicionarPedidoOrdemEmbarque + ordemEmbarque.Numero;
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracao.ApiKey);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoPedido alteracaoPedido = ObterAlteracaoPedido(ordemEmbarque, pedido);
                historicoIntegracao.JsonRequisicao = JsonConvert.SerializeObject(alteracaoPedido, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(historicoIntegracao.JsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                historicoIntegracao.JsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarque retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarque>(historicoIntegracao.JsonRetorno);

                if (!(retorno?.Validado ?? false))
                    throw new ServicoException(retorno != null ? $"Falha ao adicionar pedido na integração com a Marfrig: {retorno.Mensagem}" : "Ocorreu uma falha ao realizar a integração com a Marfrig");

                historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                historicoIntegracao.ProblemaIntegracao = "Adição de pedido da ordem de embarque integrada com sucesso";
            }
            catch (ServicoException excecao)
            {
                historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                historicoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                historicoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Marfrig";
            }

            if (historicoIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
            {
                _unitOfWork.Rollback();
                _unitOfWork.Start();
                AdicionarOrdemEmbarqueHistoricoIntegracao(historicoIntegracao);
                _unitOfWork.CommitChanges();
                throw new ServicoException(historicoIntegracao.ProblemaIntegracao);
            }
            else
                AdicionarOrdemEmbarqueHistoricoIntegracao(historicoIntegracao);
        }

        public void IntegrarPedidoRemovido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, NumeroReboque numeroReboque, Dominio.Entidades.Usuario usuario)
        {
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque = ObterOrdemEmbarque(carga, pedido.Filial, numeroReboque);

            if (ordemEmbarque == null)
                return;

            if (!carga.CargaDePreCarga)
            {
                AdicionarMensagemAlerta(carga, $"Remoção do pedido {pedido.NumeroPedidoEmbarcador} não foi integrada");
                return;
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao historicoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao()
            {
                CodigoOrdemEmbarque = ordemEmbarque.Codigo,
                CodigoPedidoRemovido = pedido.Codigo,
                CodigoUsuario = usuario.Codigo,
                TipoArquivoIntegracao = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                TipoHistoricoIntegracao = TipoOrdemEmbarqueHistoricoIntegracao.RemocaoPedido
            };

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
                string url = configuracaoIntegracao.URLIntegracaoRemoverPedidoOrdemEmbarque + ordemEmbarque.Numero;
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracao.ApiKey);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.AlteracaoPedido alteracaoPedido = ObterAlteracaoPedido(ordemEmbarque, pedido);
                historicoIntegracao.JsonRequisicao = JsonConvert.SerializeObject(alteracaoPedido, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(historicoIntegracao.JsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url) { Content = conteudoRequisicao }).Result;
                historicoIntegracao.JsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarque retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarque>(historicoIntegracao.JsonRetorno);

                if (!(retorno?.Validado ?? false))
                    throw new ServicoException(retorno != null ? $"Falha ao excluir pedido na integração com a Marfrig: {retorno.Mensagem}" : "Ocorreu uma falha ao realizar a integração com a Marfrig");

                historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                historicoIntegracao.ProblemaIntegracao = "Remoção de pedido da ordem de embarque integrada com sucesso";
            }
            catch (ServicoException excecao)
            {
                historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                historicoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                historicoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Marfrig";
            }

            if (historicoIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
            {
                _unitOfWork.Rollback();
                _unitOfWork.Start();
                AdicionarOrdemEmbarqueHistoricoIntegracao(historicoIntegracao);
                _unitOfWork.CommitChanges();
                throw new ServicoException(historicoIntegracao.ProblemaIntegracao);
            }
            else
                AdicionarOrdemEmbarqueHistoricoIntegracao(historicoIntegracao);
        }

        public void IntegrarTrocaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoRemovido, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAdicionado, NumeroReboque numeroReboque, Dominio.Entidades.Usuario usuario)
        {
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque = ObterOrdemEmbarque(carga, pedidoAdicionado.Filial, numeroReboque);

            if (ordemEmbarque == null)
                return;

            if (!carga.CargaDePreCarga)
            {
                AdicionarMensagemAlerta(carga, $"Troca do pedido {pedidoRemovido.NumeroPedidoEmbarcador} pelo {pedidoAdicionado.NumeroPedidoEmbarcador} não foi integrada");
                return;
            }

            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao repositorioOrdemEmbarqueHistoricoIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao ultimoOrdemEmbarqueHistoricoIntegracao = repositorioOrdemEmbarqueHistoricoIntegracao.BuscarUltimoPorTrocaPedido(carga.Codigo, pedidoRemovido.Codigo, ordemEmbarque.Numero);

            if (ultimoOrdemEmbarqueHistoricoIntegracao?.SituacaoIntegracao.IntegracaoPendente() ?? false)
                throw new ServicoException("Existe uma troca aguardando o retorno da integração com a Marfrig");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao historicoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao()
            {
                CodigoOrdemEmbarque = ordemEmbarque.Codigo,
                CodigoPedidoAdicionado = pedidoAdicionado.Codigo,
                CodigoPedidoRemovido = pedidoRemovido.Codigo,
                CodigoUsuario = usuario.Codigo,
                TipoArquivoIntegracao = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                TipoHistoricoIntegracao = TipoOrdemEmbarqueHistoricoIntegracao.TrocaPedido
            };

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
                string url = configuracaoIntegracao.URLIntegracaoAlteraCargaOrdemEmbarque + ordemEmbarque.Numero;
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracao.ApiKey);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.TrocaPedido trocaPedido = ObterTrocaPedido(ordemEmbarque, pedidoRemovido, pedidoAdicionado);
                historicoIntegracao.JsonRequisicao = JsonConvert.SerializeObject(trocaPedido, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(historicoIntegracao.JsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = conteudoRequisicao }).Result;
                historicoIntegracao.JsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarqueSimplificado retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque.RetornoIntegracaoOrdemEmbarqueSimplificado>(historicoIntegracao.JsonRetorno);

                if ((retornoRequisicao.StatusCode != HttpStatusCode.Accepted && retornoRequisicao.StatusCode != HttpStatusCode.OK))
                {
                    string msgErro = "";
                    if (retorno != null)
                        msgErro = $"{(string.IsNullOrWhiteSpace(retorno.Mensagem) ? "" : $"{retorno.Mensagem} ")}{(string.IsNullOrWhiteSpace(retorno.Detalhes) ? "" : retorno.Detalhes)}";

                    if (string.IsNullOrEmpty(msgErro))
                        msgErro = "Ocorreu uma falha ao realizar a integração com a Marfrig";

                    throw new ServicoException(msgErro);
                }

                historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                historicoIntegracao.ProblemaIntegracao = "Integração realizada e aguardando o retorno da Marfrig";
            }
            catch (ServicoException excecao)
            {
                historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                historicoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                historicoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                historicoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Marfrig";
            }

            if (historicoIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
            {
                _unitOfWork.Rollback();
                _unitOfWork.Start();
                AdicionarOrdemEmbarqueHistoricoIntegracao(historicoIntegracao);
                _unitOfWork.CommitChanges();
                throw new ServicoException(historicoIntegracao.ProblemaIntegracao);
            }
            else
                AdicionarOrdemEmbarqueHistoricoIntegracao(historicoIntegracao);
        }

        public bool PossuiIntegracaoOrdemEmbarque(int codigoFilial, int codigoModeloVeicularCarga)
        {
            Repositorio.Embarcador.Filiais.FilialModeloVeicularCarga repositorioFilialModeloVeicularCarga = new Repositorio.Embarcador.Filiais.FilialModeloVeicularCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga filialModeloVeicularCarga = repositorioFilialModeloVeicularCarga.BuscarPorFilialEModeloVeicularCarga(codigoFilial, codigoModeloVeicularCarga);

            return filialModeloVeicularCarga?.IntegrarOrdemEmbarque ?? false;
        }

        public bool PossuiOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(_unitOfWork);

            return repositorioOrdemEmbarque.ExistePorCarga(carga.Codigo);
        }

        public bool PossuiOrdemEmbarqueAguardandoRetornoIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorioCargaOrdemEmbarqueIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(_unitOfWork);

            return repositorioCargaOrdemEmbarqueIntegracao.ExistePorCargaAguardandoRetorno(carga.Codigo);
        }

        public bool PossuiOrdemEmbarqueSituacaoNaoPermiteDesagendar(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(_unitOfWork);
            Dictionary<string, string> situacoes = new Dictionary<string, string>();

            situacoes.Add("6", "Fechada");
            situacoes.Add("13", "Liberada para Saída");
            situacoes.Add("15", "Em Faturamento");
            situacoes.Add("16", "Em Certificação");
            situacoes.Add("19", "Faturamento Concluído");
            situacoes.Add("37", "Check Out do Veículo Realizado");
            situacoes.Add("38", "Check Out do Veículo Extornado");

            return repositorioOrdemEmbarque.ExistePorCargaESituacoes(carga.Codigo, situacoes.Keys.ToList());
        }

        #endregion Métodos Públicos
    }
}
