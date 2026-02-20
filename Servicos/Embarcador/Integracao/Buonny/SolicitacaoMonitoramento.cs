using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Servicos.Embarcador.Integracao.Buonny
{
    public sealed class SolicitacaoMonitoramento
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private Dominio.Entidades.Embarcador.Configuracoes.Integracao _configuracaoIntegracao;

        #endregion Atributos Privados

        #region Construtores

        public SolicitacaoMonitoramento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion Construtores

        #region Métodos Privados

        private void IntegrarCargaBuonny(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            ObterConfiguracao();

            if (_configuracaoIntegracao == null || string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLHomologacaoBuonny) || string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLProducaoBuonny))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Buonny.";

                repCargaIntegracao.Atualizar(cargaIntegracao);

                return;
            }

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string codigoSM = string.Empty;
            string mensagemRetorno;
            string xmlRequisicao = "";
            string xmlResposta = "";

            try
            {
                string urlWebService = cargaIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? _configuracaoIntegracao.URLProducaoBuonny : _configuracaoIntegracao.URLHomologacaoBuonny;

                ServicoBuonny.viagem viagem = ObterObjetoViagem(cargaIntegracao.Carga);
                DefinirViagemComoOperacaoCriacao(viagem);

                xmlRequisicao = GerarXMLRequisicaoSolicitarMonitoramento(viagem, urlWebService);
                WebRequest requisicao = CriaRequisicao(urlWebService, "POST", xmlRequisicao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

                xmlResposta = ObterRetorno(retornoRequisicao);

                ServicoBuonny.viagem_result viagem_result = new ServicoBuonny.viagem_result()
                {
                    codigo_sm = Utilidades.XML.ObterConteudoTag(xmlResposta, tag: "codigo_sm") ?? string.Empty,
                    erro = Utilidades.XML.ObterConteudoTag(xmlResposta, tag: "erro")
                };

                ExtrairInformacoesRetornoBuonny(viagem_result, null, out mensagemRetorno, out codigoSM);
            }
            catch (Exception ex)
            {
                mensagemRetorno = ex.InnerException?.Message ?? ex.Message;
                Servicos.Log.TratarErro(ex);
            }

            string mensagemIntegracao = mensagemRetorno + " - Código SM: " + codigoSM + ".";

            if (string.IsNullOrWhiteSpace(codigoSM))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemRetorno, 300);
            }
            else
            {
                cargaIntegracao.Protocolo = codigoSM;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaIntegracao.ProblemaIntegracao = Utilidades.String.Left(mensagemIntegracao, 300);
            }

            if (string.IsNullOrWhiteSpace(cargaIntegracao.ProblemaIntegracao))
                cargaIntegracao.ProblemaIntegracao = "";

            servicoArquivoTransacao.Adicionar(cargaIntegracao, xmlRequisicao, xmlResposta, "xml");
            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        private void ObterConfiguracao()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            _configuracaoIntegracao = configuracaoIntegracao;
        }

        private ServicoBuonny.viagem ObterObjetoViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.Pedido primeiroPedido = cargasPedido.FirstOrDefault().Pedido;
            Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault();

            DateTime dataPrevisaoFinal = cargasPedido.Max(cp => cp.Pedido.PrevisaoEntrega) ?? DateTime.Now.AddHours(1);
            List<string> placas = new List<string>();

            if (carga.Veiculo != null)
                placas.Add(carga.Veiculo.Placa);

            if (carga.VeiculosVinculados?.Count > 0)
                placas.AddRange(carga.VeiculosVinculados.Select(reboque => reboque.Placa));

            ServicoBuonny.viagem viagem = new ServicoBuonny.viagem()
            {
                autenticacao = new ServicoBuonny.viagemAutenticacao()
                {
                    token = carga.Filial?.IntegracaoBuonny?.Token ?? _configuracaoIntegracao.TokenBuonny
                },
                cnpj_cliente = carga.Filial?.IntegracaoBuonny?.CNPJCliente ?? _configuracaoIntegracao.CNPJClienteBuonny,
                cnpj_embarcador = carga.Filial?.IntegracaoBuonny?.CNPJCliente ?? _configuracaoIntegracao.CNPJClienteBuonny,
                cnpj_transportador = carga.Empresa.CNPJ,
                cnpj_gerenciadora_de_risco = !string.IsNullOrWhiteSpace(_configuracaoIntegracao.CNPJGerenciadoraDeRiscoBuonny) ? _configuracaoIntegracao.CNPJGerenciadoraDeRiscoBuonny : "00000000000000",
                pedido_cliente = Utilidades.String.Left(carga.CodigoCargaEmbarcador, 20),
                tipo_de_transporte = carga.TipoOperacao?.CodigoIntegracaoGerenciadoraRisco ?? string.Empty,
                motorista = new ServicoBuonny.viagemMotorista()
                {
                    cpf = motorista?.CPF ?? "",
                    nome = motorista?.Nome ?? "",
                    telefone = Utilidades.String.OnlyNumbers(motorista?.Telefone ?? ""),
                    radio = ""
                },
                veiculos = placas.ToArray(),
                origem = ObterObjetoOrigemViagem(primeiroPedido.Remetente),
                data_previsao_inicio = DateTime.Now.AddMinutes(10), //dataPrevisaoInicial.AddMinutes(-10),
                data_previsao_fim = dataPrevisaoFinal.AddMinutes(10),
                itinerario = ObterListaItinerarios(cargasPedido),
                monitorar_retorno = (carga.TipoOperacao.ConfiguracaoIntegracao?.DefinirParaNaoMonitorarRetornoIntegracaoBounny ?? false) ? false : (carga.Rota != null && carga.Rota.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem) || (carga.TipoOperacao?.MonitorarRetornoCargaBuonny ?? false),
            };

            return viagem;
        }

        private ServicoBuonny.viagemOrigem ObterObjetoOrigemViagem(Dominio.Entidades.Cliente origem)
        {
            return new ServicoBuonny.viagemOrigem()
            {
                codigo_externo = origem.CPF_CNPJ_SemFormato,
                descricao = Utilidades.String.Left(origem.Nome, 50),
                logradouro = Utilidades.String.Left(origem.Endereco, 100),
                numero = Utilidades.String.Left(Utilidades.String.OnlyNumbers(origem.Numero), 5),
                complemento = Utilidades.String.Left(origem.Complemento, 10),
                bairro = Utilidades.String.Left(origem.Bairro, 50),
                cidade = Utilidades.String.Left(origem.Localidade.Descricao, 50),
                estado = origem.Localidade.Estado.Sigla
            };
        }

        private ServicoBuonny.viagemAlvo[] ObterListaItinerarios(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido)
        {
            List<Dominio.Entidades.Cliente> destinatarios = cargasPedido.Where(o => o.Pedido.Destinatario != null).Select(o => o.Pedido.Destinatario).Distinct().ToList();

            List<ServicoBuonny.viagemAlvo> itinerarios = new List<ServicoBuonny.viagemAlvo>();

            //List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaNotas = cargasPedido.SelectMany(o => o.NotasFiscais).DistinctBy(nf => nf.XMLNotaFiscal).ToList();

            int tempo = 0;

            foreach (Dominio.Entidades.Cliente destinatario in destinatarios)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = (from cp in cargasPedido where cp.Pedido.Destinatario?.Codigo == destinatario.Codigo select cp).OrderBy(cp => cp.PrevisaoEntrega).FirstOrDefault();

                var xmlNotaFiscal = cargaPedido.NotasFiscais?.FirstOrDefault()?.XMLNotaFiscal;
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaNotas = cargaPedido.NotasFiscais?.ToList();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasFiltrados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                if (xmlNotaFiscal == null)
                {
                    cargasFiltrados = (from cp in cargasPedido where cp.Pedido.Destinatario?.Codigo == destinatario.Codigo select cp).ToList();
                    listaNotas.Add((from cp in cargasFiltrados where cp.NotasFiscais != null & cp.NotasFiscais.Count > 0 select cp.NotasFiscais?.FirstOrDefault()).FirstOrDefault());
                }

                itinerarios.Add(
                    ObterObjetoItinerario(cargaPedido.Pedido.Destinatario, cargaPedido.PrevisaoEntrega ?? DateTime.Now.AddMinutes(30 + tempo), "3", cargaPedido.Carga.TipoDeCarga?.IdentificacaoMercadoriaInfolog, listaNotas)
                );

                tempo += 1;
            }

            return itinerarios.ToArray();
        }

        private ServicoBuonny.viagemAlvo ObterObjetoItinerario(Dominio.Entidades.Cliente cliente, DateTime dataPrevisaoChegada, string tipoParada, string tipoProduto, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaNotas)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoBuonny.viagemAlvoCarga[] notasDaCarga = listaNotas.Count > 0 ? new ServicoBuonny.viagemAlvoCarga[listaNotas.Count] : null;
            int n = 0;
            foreach (var notaFiscal in listaNotas)
            {
                notasDaCarga[0] = new ServicoBuonny.viagemAlvoCarga()
                {
                    nf = notaFiscal?.XMLNotaFiscal.Numero.ToString() ?? "1",
                    serie_nf = !string.IsNullOrWhiteSpace(notaFiscal?.XMLNotaFiscal.SerieOuSerieDaChave ?? "") ? notaFiscal?.XMLNotaFiscal.SerieOuSerieDaChave : "0",
                    peso = notaFiscal?.XMLNotaFiscal.Peso ?? 0,
                    tipo_produto = string.IsNullOrWhiteSpace(tipoProduto) ? "33" : tipoProduto,
                    valor_total_nf = notaFiscal?.XMLNotaFiscal.Valor ?? 0,
                    volume = notaFiscal?.XMLNotaFiscal.Volumes.ToString() ?? "1"
                };
            }

            ServicoBuonny.viagemAlvo itinerario = new ServicoBuonny.viagemAlvo
            {
                bairro = cliente.Bairro,
                cep = cliente.CEP,
                cidade = cliente.Localidade.Descricao,
                complemento = cliente.Complemento,
                dados_da_carga = notasDaCarga,
                descricao = cliente.Nome,
                email = cliente.Email,
                estado = cliente.Localidade.Estado.Sigla,
                telefone = cliente.Telefone1,
                tipo_parada = tipoParada,
                codigo_externo = cliente.CPF_CNPJ_SemFormato,

                /**
                 * #16434
                 * 
                 * Os CAVALOS da Buonny mapearam esses campos DATETIME
                 * mas esperam a informação formatada
                 * 
                 * Caso atualizar as referencias desse WSDL, favor
                 * alterar a referência e definir como string
                 * 
                 * Trocar a propriedade de DateTime para string
                 * Remover DataType="time" do XmlElementAttribute
                 */
                //janela_inicio = dataPrevisaoChegada.AddHours(-1),
                previsao_de_chegada = dataPrevisaoChegada,
                //janela_fim = dataPrevisaoChegada.AddHours(1)
            };

            return itinerario;
        }

        private void DefinirViagemComoOperacaoCriacao(ServicoBuonny.viagem viagem)
        {
            viagem.operacao_sm = ServicoBuonny.viagemOperacao_sm.I;
        }

        private string GerarXMLRequisicaoSolicitarMonitoramento(ServicoBuonny.viagem viagem, string urlWebService)
        {
            string objetoXml = XML.ConvertObjectToXMLString(viagem);

            string pattern = @"https?://([\w-]+\.)+[\w-]+\.com\.br";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(urlWebService);

            string urlBaseNameSpaceXML = match.Value;

            objetoXml = objetoXml.Replace(" xmlns=\"https://api.buonny.com.br/portal/wsdl/buonny\"", $" xmlns=\"{urlBaseNameSpaceXML}\"");
            objetoXml = objetoXml.Replace(" xmlns=\"https://api.buonny.com.br/portal/soap/buonny_soap\"", $" xmlns=\"{urlBaseNameSpaceXML}/portal/soap/buonny_soap\"");
            objetoXml = objetoXml.Replace("<viagem>", $"<viagem xmlns=\"{urlBaseNameSpaceXML}/portal/wsdl/buonny\">");

            return $@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Header>
    <Action s:mustUnderstand=""0"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"" />
  </s:Header>
  <s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    {objetoXml}
  </s:Body>
</s:Envelope>";
        }

        private void DefinirViagemComoOperacaoCancelamento(ServicoBuonny.viagem viagem)
        {
            viagem.operacao_sm = ServicoBuonny.viagemOperacao_sm.C;
        }

        private void ExtrairInformacoesRetornoBuonny(ServicoBuonny.viagem_result response, Servicos.Models.Integracao.InspectorBehavior inspector, out string mensagemRetorno, out string codigoSM)
        {
            mensagemRetorno = "";
            codigoSM = "";

            if (response != null)
            {
                mensagemRetorno = response.erro;
                codigoSM = response.codigo_sm;

                return;
            }

            if (inspector != null)
            {
                XDocument doc = XDocument.Parse(inspector.LastResponseXML);

                mensagemRetorno = doc.Descendants("viagem_result").Select(o => o.Element("erro").Value).FirstOrDefault();
                codigoSM = doc.Descendants("viagem_result").Select(o => o.Element("codigo_sm").Value).FirstOrDefault();
            }
        }

        private WebRequest CriaRequisicao(string url, string metodo, string body, List<(string Nome, string Valor)> headers = null, string contentType = "application/json")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebRequest requisicao = WebRequest.Create(url);

            byte[] byteArrayDadosRequisicao = Encoding.ASCII.GetBytes(body);

            requisicao.Method = metodo;
            requisicao.ContentType = contentType;
            requisicao.Timeout = 60 * 1000;

            foreach (var header in (headers ?? new List<(string Nome, string Valor)>()))
                requisicao.Headers[header.Nome] = header.Valor;

            requisicao.ContentLength = byteArrayDadosRequisicao.Length;

            System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream();
            streamDadosRequisicao.Write(byteArrayDadosRequisicao, 0, byteArrayDadosRequisicao.Length);
            streamDadosRequisicao.Close();

            return requisicao;
        }

        private HttpWebResponse ExecutarRequisicao(WebRequest request)
        {
            try
            {
                WebResponse retornoRequisicao = request.GetResponse();
                return (HttpWebResponse)retornoRequisicao;
            }
            catch (WebException webException)
            {
                if (webException.Response == null)
                    throw new Exception("Falha ao processar o retorno da API");

                return (HttpWebResponse)webException.Response;
            }
        }

        private string ObterRetorno(HttpWebResponse response)
        {
            string jsonDadosRetornoRequisicao;
            using (System.IO.Stream streamDadosRetornoRequisicao = response.GetResponseStream())
            {
                System.IO.StreamReader leitorDadosRetornoRequisicao = new System.IO.StreamReader(streamDadosRetornoRequisicao);
                jsonDadosRetornoRequisicao = leitorDadosRetornoRequisicao.ReadToEnd();
                leitorDadosRetornoRequisicao.Close();
            }

            response.Close();

            return jsonDadosRetornoRequisicao.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n", "");
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            if (!cargaIntegracao.TipoIntegracao.IntegrarComPlataformaNstech)
                this.IntegrarCargaBuonny(cargaIntegracao);
            else
            {
                Servicos.Embarcador.Integracao.Nstech.IntegracaoSM svcIntegracaoSMNstech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoSM(_tipoServicoMultisoftware, _unitOfWork);
                svcIntegracaoSMNstech.IntegrarSM(cargaIntegracao);
            }
        }

        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao integracaoPendente)
        {
            integracaoPendente.NumeroTentativas += 1;
            integracaoPendente.DataIntegracao = DateTime.Now;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            ObterConfiguracao();

            if (_configuracaoIntegracao == null || string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLHomologacaoBuonny) || string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLProducaoBuonny))
            {
                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Não existe configuração de integração disponível para a Buonny.";

                repCargaCancelamentoCargaIntegracao.Atualizar(integracaoPendente);

                return;
            }

            string xmlRequisicao = "";
            string codigoSM = string.Empty;
            string mensagemRetorno;
            string xmlResposta = "";

            try
            {
                string urlWebService = integracaoPendente.CargaCancelamento.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? _configuracaoIntegracao.URLProducaoBuonny : _configuracaoIntegracao.URLHomologacaoBuonny;

                ServicoBuonny.viagem viagem = ObterObjetoViagem(integracaoPendente.CargaCancelamento.Carga);
                DefinirViagemComoOperacaoCancelamento(viagem);

                xmlRequisicao = GerarXMLRequisicaoSolicitarMonitoramento(viagem, urlWebService);
                WebRequest requisicao = CriaRequisicao(urlWebService, "POST", xmlRequisicao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

                xmlResposta = ObterRetorno(retornoRequisicao);

                ServicoBuonny.viagem_result viagem_result = new ServicoBuonny.viagem_result()
                {
                    codigo_sm = Utilidades.XML.ObterConteudoTag(xmlResposta, tag: "codigo_sm") ?? string.Empty,
                    erro = Utilidades.XML.ObterConteudoTag(xmlResposta, tag: "erro")
                };

                ExtrairInformacoesRetornoBuonny(viagem_result, null, out mensagemRetorno, out codigoSM);
            }
            catch (Exception ex)
            {
                mensagemRetorno = ex.InnerException?.Message ?? ex.Message;
                Servicos.Log.TratarErro(ex);
            }

            string mensagemIntegracao = mensagemRetorno + " - Código SM: " + codigoSM + ".";

            if (string.IsNullOrWhiteSpace(codigoSM))
            {
                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = Utilidades.String.Left(mensagemRetorno, 300);
            }
            else
            {
                integracaoPendente.Protocolo = codigoSM;
                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                integracaoPendente.ProblemaIntegracao = Utilidades.String.Left(mensagemIntegracao, 300);
            }

            if (string.IsNullOrWhiteSpace(integracaoPendente.ProblemaIntegracao))
                integracaoPendente.ProblemaIntegracao = "";

            servicoArquivoTransacao.Adicionar(integracaoPendente, xmlRequisicao, xmlResposta, "xml");

            repCargaCancelamentoCargaIntegracao.Atualizar(integracaoPendente);
        }

        #endregion Métodos Públicos
    }
}