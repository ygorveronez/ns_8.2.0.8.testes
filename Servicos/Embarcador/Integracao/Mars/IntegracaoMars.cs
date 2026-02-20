using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Mars
{
    public class IntegracaoMars
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars _configuracaoIntegracao;


        #endregion

        #region Construtores

        public IntegracaoMars(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Comunicação

        private HttpClient CriaRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars configuracaoIntegracao)
        {
            return CriaRequisicao(new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.ConfiguracoesAutenticacao().CarregarConfiguracaoPadrao(configuracaoIntegracao));
        }

        private HttpClient CriaRequisicao(Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.ConfiguracoesAutenticacao configuracoesAutenticacao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMars));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string token = ObterToken(configuracoesAutenticacao);
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return requisicao;
        }

        private string ObterToken(Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.ConfiguracoesAutenticacao configuracoesAutenticacao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            RestClient client = new RestClient(configuracoesAutenticacao.URLAutenticacao);
            client.Timeout = -1;
            RestRequest request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            string basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{configuracoesAutenticacao.ClientID}:{configuracoesAutenticacao.ClientSecret}"));
            request.AddHeader("Authorization", $"Basic {basicAuth}");

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);

            return retorno.access_token;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars ObterConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao != null) return _configuracaoIntegracao;

            Repositorio.Embarcador.Configuracoes.IntegracaoMars repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoMars(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAutenticacao) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoCargaCTe))
                throw new ServicoException("Não existe configuração de integração disponível para a Mars.");

            _configuracaoIntegracao = configuracaoIntegracao;

            return _configuracaoIntegracao;
        }

        private bool EnviarArquivosIntegracao(Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Requisicao requisicao, out string jsonDadosRequisicao, out string jsonDadosRetornoRequisicao)
        {
            jsonDadosRequisicao = string.Empty;
            jsonDadosRetornoRequisicao = string.Empty;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars configuracaoIntegracao = ObterConfiguracaoIntegracao();

            jsonDadosRequisicao = JsonConvert.SerializeObject(requisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            StringContent conteudoRequisicao = new StringContent(jsonDadosRequisicao, Encoding.UTF8, "application/json");

            HttpClient client = CriaRequisicao(configuracaoIntegracao);

            HttpResponseMessage retornoRequisicao = client.PostAsync(configuracaoIntegracao.URLIntegracaoCargaCTe, conteudoRequisicao).Result;

            jsonDadosRetornoRequisicao = retornoRequisicao.Content.ReadAsStringAsync().Result;
            dynamic jsonRetorno = JsonConvert.DeserializeObject<dynamic>(jsonDadosRetornoRequisicao);

            if (!IsRetornoSucesso(retornoRequisicao) || !IsRetornoValido(jsonRetorno))
                return false;

            return true;
        }

        private bool IsRetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created;
        }

        private bool IsRetornoValido(dynamic jsonRetorno)
        {
            if (jsonRetorno.status != null && jsonRetorno.message != null)
            {
                return jsonRetorno.status;
            }

            return false;
        }

        private bool IntegrarCTeProgramado(Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado, out string jsonDadosRequisicao, out string jsonDadosRetornoRequisicao)
        {
            Servicos.Embarcador.Carga.ICMS serRegraICMS = new Embarcador.Carga.ICMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = integracaoEnvioProgramado.Carga ?? integracaoEnvioProgramado.CargaOcorrencia.Carga;

            if (carga == null)
                throw new ServicoException("Carga não localizada");

            if (integracaoEnvioProgramado.CTe == null)
                throw new ServicoException("CTe não localizado");

            string codigoIntegracao = carga.Codigo.ToString();

            string tipoCusto = ObterTipoCustoDescricao(integracaoEnvioProgramado, carga);

            decimal valorFrete = (integracaoEnvioProgramado.CTe.AliquotaICMS > 0 ? serRegraICMS.ObterValorLiquido(integracaoEnvioProgramado.CTe.ValorAReceber, integracaoEnvioProgramado.CTe.AliquotaICMS) : integracaoEnvioProgramado.CTe.ValorAReceber);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Requisicao requisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Requisicao
            {
                CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador,
                CodigoIntegracao = codigoIntegracao,
                TipoCusto = tipoCusto,
                ValorFretePagar = valorFrete.ToString().Replace(',', '.')
            };

            requisicao.Documentos = ObterDocumentos(integracaoEnvioProgramado.CTe, carga);

            return EnviarArquivosIntegracao(requisicao, out jsonDadosRequisicao, out jsonDadosRetornoRequisicao);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Documento> ObterDocumentos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioPedidoXmlNotaFiscalCte = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Servicos.Embarcador.Carga.ICMS serRegraICMS = new Embarcador.Carga.ICMS(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Documento> documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Documento>();
            documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Documento
            {
                Cfop = $"{cte.CFOP.CodigoCFOP} - {cte.CFOP.Descricao ?? string.Empty}",
                ChaveAcesso = cte.ChaveAcesso,
                DataHoraEmissao = cte.DataEmissao.ToString(),
                Modelo = cte.ModeloDocumentoFiscal.Descricao,
                ProtocoloCTe = cte.Codigo.ToString(),
                NumeroCTe = cte.Numero.ToString(),
                Serie = cte.Serie.Descricao,
                ProdutoPredominante = cte.ProdutoPredominante,
                ProtocoloAutorizacao = cte.Protocolo?.ToString() ?? "",
                ValorTotalCarga = cte.ValorFrete.ToString().Replace(',', '.'),
                PesoBruto = cte.Peso.ToString().Replace(',', '.'),
                ValorTotalServico = cte.ValorAReceber.ToString().Replace(',', '.'),
                Modal = cte.ModalTransporte.Descricao.ToString(),
                InicioViagem = carga.DataInicioViagem?.ToString() ?? string.Empty,
                FimViagem = carga.DataFimViagem?.ToString() ?? string.Empty,
                InformacoesFiscais = ObterImpostosPorCTe(cte),
                Destinatario = new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Participante
                {
                    Cnpj = cte.Destinatario.CPF_CNPJ.ToString(),
                    Endereco = cte.EnderecoDestinatario?.ToString() ?? string.Empty,
                    InscricaoEstadual = cte.Destinatario.InscricaoSuframa?.ToString() ?? string.Empty,
                    Nome = cte.Destinatario.Nome,
                    Telefone = cte.Destinatario.Telefone1
                },
                Remetente = new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Participante
                {
                    Cnpj = cte.Remetente.CPF_CNPJ.ToString(),
                    Endereco = cte.EnderecoRemetente?.ToString() ?? string.Empty,
                    InscricaoEstadual = cte.Remetente.InscricaoSuframa?.ToString() ?? string.Empty,
                    Nome = cte.Remetente.Nome,
                    Telefone = cte.Remetente.Telefone1
                },
                Emitente = new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Participante
                {
                    Cnpj = cte.Tomador.CPF_CNPJ.ToString(),
                    Endereco = cte.EnderecoTomador?.ToString() ?? string.Empty,
                    InscricaoEstadual = cte.Tomador.InscricaoSuframa?.ToString() ?? string.Empty,
                    Nome = cte.Tomador.Nome,
                    Telefone = cte.Tomador.Telefone1
                },
                ComponentesValorServico = new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.ComponentesValorServico
                {
                    ValorFrete = cte.ValorFrete.ToString().Replace(',', '.'),
                    Descarga = carga.Componentes.Where(x => x.TipoComponenteFrete == TipoComponenteFrete.DESCARGA).Sum(x => x.ValorComponente).ToString().Replace(',', '.'),
                    Pedagio = carga.Componentes.Where(x => x.TipoComponenteFrete == TipoComponenteFrete.PEDAGIO).Sum(x => x.ValorComponente).ToString().Replace(',', '.'),
                    Impostos = cte.ValorICMS.ToString().Replace(',', '.'),
                    TotalCTe = (cte.AliquotaICMS > 0 ? serRegraICMS.ObterValorLiquido(cte.ValorAReceber, cte.AliquotaICMS) : cte.ValorAReceber).ToString().Replace(",", "."),
                },
                Pedagio = ObterCargaValePedagio(carga.Codigo),
                TipoCTe = cte.TipoCTE.ObterDescricao(),
                DocumentosOrigem = ObterDocumentosOrigem(repositorioPedidoXmlNotaFiscalCte.BuscarXMLNotasFiscaisPorCodigoCTe(cte.Codigo))
            });

            return documentos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.DocumentoOrigem> ObterDocumentosOrigem(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> XMLNotaFiscais)
        {
            return XMLNotaFiscais.Select(xmlNotaFiscal => new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.DocumentoOrigem
            {
                Chave = xmlNotaFiscal.Chave,
                Numero = xmlNotaFiscal.Numero.ToString(),
                Serie = xmlNotaFiscal.Serie,
                TipoDocumento = xmlNotaFiscal.TipoDocumento.ObterDescricao()
            }).ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Pedagio> ObterCargaValePedagio(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaValePedagios = repositorioCargaValePedagio.BuscarPorCarga(codigoCarga);

            return cargaValePedagios.Select(valePedagio => new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.Pedagio
            {
                Cnpj = valePedagio.CnpjMeioPagamento ?? string.Empty,
                Recibo = valePedagio.NumeroValePedagio ?? string.Empty,
                ValorPedagio = valePedagio.ValorValePedagio.ToString().Replace(',', '.')
            }).ToList();

        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.InformacoesFiscais ObterImpostosPorCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.InformacoesFiscais impostos = new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.InformacoesFiscais();

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                impostos.AliquotaIcms = cte.AliquotaISS.ToString().Replace(',', '.');
                impostos.BaseCalculo = cte.BaseCalculoISS.ToString().Replace(',', '.');
                impostos.SituacaoTributaria = cte.SubstituicaoTributaria;
                impostos.ValorIcms = cte.ValorISS.ToString().Replace(',', '.');
            }

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                impostos.AliquotaIcms = cte.AliquotaICMS.ToString().Replace(',', '.');
                impostos.BaseCalculo = cte.BaseCalculoICMS.ToString().Replace(',', '.');
                impostos.SituacaoTributaria = cte.SubstituicaoTributaria;
                impostos.ValorIcms = cte.ValorICMS.ToString().Replace(',', '.');
            }

            return impostos;
        }

        private void IntegrarCTe(Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            DateTime dataIntegracao = DateTime.Now;
            SituacaoIntegracao situacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            string problemaIntegracao = string.Empty;
            string jsonDadosRequisicao = string.Empty;
            string jsonDadosRetornoRequisicao = string.Empty;

            bool sucesso = false;

            try
            {
                sucesso = IntegrarCTeProgramado(integracaoEnvioProgramado, out jsonDadosRequisicao, out jsonDadosRetornoRequisicao);
            }
            catch (ServicoException excecao)
            {
                problemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoMars");
                problemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Mars";
            }

            integracaoEnvioProgramado.DataIntegracao = dataIntegracao;
            integracaoEnvioProgramado.NumeroTentativas++;

            if (sucesso)
            {
                integracaoEnvioProgramado.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                integracaoEnvioProgramado.ProblemaIntegracao = "Integrado com sucesso e aguardando retorno da Mars";
            }
            else
            {
                integracaoEnvioProgramado.SituacaoIntegracao = situacaoIntegracao;
                integracaoEnvioProgramado.ProblemaIntegracao = problemaIntegracao;
            }

            Log.TratarErro($"Requisição: {jsonDadosRequisicao}\nResposta: {jsonDadosRetornoRequisicao}", "IntegracaoMars");

            servicoArquivoTransacao.Adicionar(integracaoEnvioProgramado, jsonDadosRequisicao, jsonDadosRetornoRequisicao, "json", integracaoEnvioProgramado.ProblemaIntegracao);
            repositorioIntegracaoEnvioProgramado.Atualizar(integracaoEnvioProgramado);
        }

        private void AplicarFalha(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao, string mensagemFalha)
        {
            cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mensagemFalha;
        }

        #endregion

        #region Métodos Públicos       

        public void IntegrarCTes(List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado)
        {
            foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado in integracoesEnvioProgramado)
            {
                IntegrarCTe(integracaoEnvioProgramado);
            }
        }

        public static void InserirIntegracaoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars);

            new Servicos.Embarcador.Canhotos.CanhotoIntegracao(unitOfWork).CriarCanhotoIntegracaoAsync(canhoto, tipoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador).GetAwaiter().GetResult();
        }

        public void IntegrarCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repositorioCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars integracao = ObterConfiguracaoIntegracao();

            canhotoIntegracao.DataIntegracao = DateTime.Now;
            canhotoIntegracao.NumeroTentativas++;

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosPorCarga = repositorioCanhoto.BuscarPorCarga(canhotoIntegracao.Canhoto.Carga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.RequisicaoCanhoto request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.RequisicaoCanhoto();
            request.CTes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.CTe>();
            request.CodigoCargaEmbarcador = canhotoIntegracao.Canhoto.Carga.CodigoCargaEmbarcador ?? string.Empty;
            request.Data = DateTime.Now;
            request.Liberado = true;

            List<int> numerosCTes = repositorioCargaCTe.BuscarNumerosCTesPorCargaAsync(canhotoIntegracao.Canhoto.Carga.Codigo).GetAwaiter().GetResult();

            DateTime? ultimaDataDigitalizacao = canhotoIntegracao.Canhoto.DataDigitalizacao;

            foreach (int numeroCte in numerosCTes)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.CTe requisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.CTe
                {
                    Numero = numeroCte.ToString(),
                    DataUltimaDigitalizacaoCanhotos = ultimaDataDigitalizacao ?? DateTime.MinValue,
                };

                request.CTes.Add(requisicao);
            }

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                HttpClient client = CriaRequisicao(integracao);

                jsonRequest = JsonConvert.SerializeObject(request, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage result = client.PostAsync(integracao.URLIntegracaoCanhoto, content).Result;

                if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    jsonResponse = result.Content.ReadAsStringAsync().Result;
                    canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                    servicoArquivoTransacao.Adicionar(canhotoIntegracao, jsonRequest, jsonResponse, "json", canhotoIntegracao.ProblemaIntegracao ?? "");

                    Log.TratarErro($"Requisição: {jsonRequest}\nResposta: {jsonResponse}", "IntegracaoMars");
                }
                else
                {
                    canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    canhotoIntegracao.ProblemaIntegracao = "Falha ao conectar no WS Mars";

                    servicoArquivoTransacao.Adicionar(canhotoIntegracao, jsonRequest, jsonResponse, "json", canhotoIntegracao.ProblemaIntegracao ?? "");
                    Log.TratarErro($"Requisição: {jsonRequest}\nResposta: {jsonResponse}", "IntegracaoMars");
                }

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "IntegracaoMars");
                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar";

                servicoArquivoTransacao.Adicionar(canhotoIntegracao, jsonRequest, jsonResponse, "json", canhotoIntegracao.ProblemaIntegracao ?? "");
                Log.TratarErro($"Requisição: {jsonRequest}\nResposta: {jsonResponse}", "IntegracaoMars");
            }

            repositorioCanhotoIntegracao.Atualizar(canhotoIntegracao);
        }

        public async Task IntegrarCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new(_unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoIntegracao = new(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new(_unitOfWork);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaIntegracao.NumeroTentativas++;

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars integracao = ObterConfiguracaoIntegracao();

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = await repositorioCargaCancelamento.BuscarPorCargaComFilialEmpresaPedidosAsync(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.CargaCancelamento request = new(cargaCancelamento);

                HttpClient client = CriaRequisicao(new Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.ConfiguracoesAutenticacao().CarregarConfiguracaoCancelamentosCargas(integracao));

                jsonRequest = JsonConvert.SerializeObject(request, Formatting.Indented);
                StringContent content = new(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage result = await client.PostAsync(integracao.URLIntegracaoCancelamentosCargas, content);

                if (result.IsSuccessStatusCode)
                {
                    if (result.Content != null)
                        jsonResponse = await result.Content.ReadAsStringAsync();

                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso.";
                }
                else
                {
                    AplicarFalha(cargaCancelamentoCargaIntegracao, "Falha ao conectar no WS Mars.");
                }

            }
            catch (Exception ex)
            {
                AplicarFalha(cargaCancelamentoCargaIntegracao, "Ocorreu uma falha ao integrar.");
            }
            finally
            {
                servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, jsonRequest, jsonResponse, "json", cargaCancelamentoCargaIntegracao.ProblemaIntegracao);
                await repositorioCargaCancelamentoIntegracao.AtualizarAsync(cargaCancelamentoCargaIntegracao);
            }
        }

        public string ObterTipoCustoDescricao(Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string descricao = string.Empty;

            if (integracaoEnvioProgramado.CargaOcorrencia != null)
            {
                if (integracaoEnvioProgramado.CargaOcorrencia.TipoOcorrencia?.GrupoTipoDeOcorrenciaDeCTe?.CodigoIntegracao != null)
                {
                    descricao = integracaoEnvioProgramado.CargaOcorrencia.TipoOcorrencia.GrupoTipoDeOcorrenciaDeCTe.CodigoIntegracao;
                    return descricao;
                }

                descricao = Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.TipoServicoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.TipoServico.Complemento);
                return descricao;
            }

            if (carga.TipoServicoCarga == TipoServicoCarga.Redespacho || carga.TipoServicoCarga == TipoServicoCarga.Normal)
                descricao = Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.TipoServicoHelper.ObterDescricaoPorCarga(carga.TipoServicoCarga);

            if (carga.DadosSumarizados.Reentrega)
                descricao = Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.TipoServicoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.TipoServico.Reentrega);

            if (integracaoEnvioProgramado.CTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || integracaoEnvioProgramado.CTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                descricao = Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.TipoServicoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Integracao.Mars.TipoServico.NFSE);

            return descricao;
        }
        #endregion
    }
}
