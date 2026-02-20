using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.MICDTA
{
    public class IntegracaoMICDTA
    {
        #region Métodos Públicos
        public static void IntegrarMICDTA(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, string mensagem = "")
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoMicDta repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoMicDta(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.ObjetosDeValor.Embarcador.Localidade.DadosPaisOrigemDestino dadosPaisOrigemDestino = repCargaPedido.BuscarDadosPaisOrigemDestinoPorCarga(cargaCargaIntegracao.Carga);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(cargaCargaIntegracao.Carga.TipoOperacao.Codigo);

            cargaCargaIntegracao.NumeroTentativas += 1;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCargaIntegracao.ProblemaIntegracao = string.Empty;

            if (!(tipoOperacao?.IntegrarMICDTAComSiscomex ?? false))
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

            cargaCargaIntegracao.NumeroMICDTA = "";
            cargaCargaIntegracao.SiglaPaisOrigemMICDTA = dadosPaisOrigemDestino?.AbreviacaoOrigem ?? "BR";

            if (cargaCargaIntegracao.SiglaPaisOrigemMICDTA == "BR" && (dadosPaisOrigemDestino?.AbreviacaoDestino ?? "BR") != "BR")
                cargaCargaIntegracao.NumeroLicencaTNTIMICDTA = dadosPaisOrigemDestino?.LicencaTNTIDestino ?? "5525";
            else
                cargaCargaIntegracao.NumeroLicencaTNTIMICDTA = dadosPaisOrigemDestino?.LicencaTNTIOrigem ?? "5525";

            if (!string.IsNullOrWhiteSpace(cargaCargaIntegracao.NumeroLicencaTNTIMICDTA) && cargaCargaIntegracao.NumeroLicencaTNTIMICDTA.Contains("/"))
                cargaCargaIntegracao.NumeroLicencaTNTIMICDTA = cargaCargaIntegracao.NumeroLicencaTNTIMICDTA.Split('/').FirstOrDefault();
            if (cargaCargaIntegracao.NumeroSequencialMICDTA <= 0)
                cargaCargaIntegracao.NumeroSequencialMICDTA = repCargaIntegracao.ProximoNumeroSequencialMICDTA(cargaCargaIntegracao.SiglaPaisOrigemMICDTA, cargaCargaIntegracao.NumeroLicencaTNTIMICDTA);

            cargaCargaIntegracao.NumeroMICDTA = cargaCargaIntegracao.SiglaPaisOrigemMICDTA + cargaCargaIntegracao.NumeroLicencaTNTIMICDTA + cargaCargaIntegracao.NumeroSequencialMICDTA.ToString().PadLeft(5, '0');

            repCargaIntegracao.Atualizar(cargaCargaIntegracao);

            if (!(tipoOperacao?.IntegrarMICDTAComSiscomex ?? false))
                return;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = configuracaoIntegracao.URL + configuracaoIntegracao.MetodoManifestacaoEmbarca;
            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.MICDTA.RetornoToken token = ObterToken(configuracaoIntegracao, cargaCargaIntegracao.Carga.Empresa, out mensagemErro);
            if (token == null || string.IsNullOrWhiteSpace(token.token) || string.IsNullOrWhiteSpace(token.xCSRFToken))
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMICDTA));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            client.DefaultRequestHeaders.Add("Authorization", token.setToken);
            client.DefaultRequestHeaders.Add("X-CSRF-Token", token.xCSRFToken);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            try
            {
                jsonRequest = GerarObjetoMICDTA(cargaCargaIntegracao, configuracaoIntegracao, unitOfWork);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/xml");
                var result = client.PostAsync(endPoint, content).Result;

                jsonResponse = result.Content.ReadAsStringAsync().Result;
                jsonResponse = jsonResponse.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n", "");

                if (result.IsSuccessStatusCode)
                {
                    situacaoIntegracao = true;
                    mensagemErro = string.Empty;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "xml", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                }
                else
                {
                    mensagemErro = Utilidades.XML.ObterConteudoTag(jsonResponse, tag: "message") ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = Utilidades.XML.ObterConteudoTag(jsonResponse, tag: "descricao") ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha no Schema com a MIC/DTA.";
                    else
                        mensagemErro = "Retorno MIC/DTA: " + mensagemErro;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "xml", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                    repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoMICDTA");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoMICDTA");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoMICDTA");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da MIC/DTA.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "xml", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "xml", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }

            if (!situacaoIntegracao)
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
            else
            {
                cargaCargaIntegracao.ProblemaIntegracao = string.Empty;
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
        }

        #endregion

        #region Métodos Privados

        private static Dominio.ObjetosDeValor.Embarcador.MICDTA.RetornoToken ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta integracaoMicDta, Dominio.Entidades.Empresa empresa, out string erro)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                erro = string.Empty;
                Dominio.ObjetosDeValor.Embarcador.MICDTA.RetornoToken token = new Dominio.ObjetosDeValor.Embarcador.MICDTA.RetornoToken();

                string postBody = integracaoMicDta.URL + "/portal/api/autenticar";
                string senhaCertificado = empresa.SenhaCertificado;
                string caminhoCertificado = empresa.NomeCertificado;

                //caminhoCertificado = "C:\\Empresas\\75818849000176\\Certificado\\75818849000176.pfx";
                //senhaCertificado = "C0rd#3";

                HttpWebRequest client = (HttpWebRequest)WebRequest.Create(postBody);

                client.Method = System.Net.WebRequestMethods.Http.Post;
                client.ProtocolVersion = HttpVersion.Version10;
                client.ContentType = "application/json";
                client.KeepAlive = false;
                client.Headers["Role-Type"] = "TRANSPORT";

                var certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

                client.ClientCertificates.Add(certificado);
                client.PreAuthenticate = true;

                string retorno = "";
                HttpWebResponse objResponse = (HttpWebResponse)client.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    retorno = sr.ReadToEnd();
                    sr.Close();
                }
                token = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.MICDTA.RetornoToken>(retorno);
                token.setToken = objResponse.Headers["Set-Token"];
                token.xCSRFToken = token.token;

                if (string.IsNullOrWhiteSpace(token.token) || string.IsNullOrWhiteSpace(token.setToken))
                {
                    Servicos.Log.TratarErro(postBody, "IntegracaoMICDTA");
                    Servicos.Log.TratarErro(JsonConvert.SerializeObject(token, Formatting.Indented), "IntegracaoMICDTA");

                    erro = "MIC/DTA não retornou Token, " + token.message;
                }

                return token;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoMICDTA");

                erro = "Não foi possível obter token MIC/DTA";
                return null;
            }
        }

        private static string GerarObjetoMICDTA(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta integracaoMicDta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoFronteira repositorioPedidoFronteira = new Repositorio.Embarcador.Pedidos.PedidoFronteira(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Veiculo veiculo = cargaCargaIntegracao.Carga.Veiculo;
            Dominio.Entidades.Usuario motorista = cargaCargaIntegracao.Carga.Motoristas?.FirstOrDefault() ?? null;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCargaIntegracao.Carga.Pedidos?.FirstOrDefault() ?? null;
            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = cargaPedido.ApoliceSeguroAverbacao?.FirstOrDefault()?.ApoliceSeguro ?? null;
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notaFiscais = repPedidoXMLNotaFiscal.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);
            List<Dominio.Entidades.Cliente> listaFronteiras = repositorioPedidoFronteira.BuscarFronteirasPorPedido(cargaPedido.Pedido.Codigo);

            Dominio.ObjetosDeValor.Embarcador.MICDTA.Manifestacao manifestacao = new Dominio.ObjetosDeValor.Embarcador.MICDTA.Manifestacao();
            manifestacao.identificacaoManifestacao = cargaCargaIntegracao.Carga.CodigoCargaEmbarcador.PadLeft(10, '0');

            Dominio.Entidades.Cliente pedidoFronteiraOrigem = null;
            Dominio.Entidades.Cliente pedidoFronteiraDestino = null;

            string produtoProdominante = repCargaCTe.BuscarProdutoPredominantePorCarga(cargaCargaIntegracao.Carga.Codigo, "A");

            if (listaFronteiras.Count > 0)
            {
                pedidoFronteiraOrigem = listaFronteiras.FirstOrDefault();
                pedidoFronteiraDestino = listaFronteiras.LastOrDefault();
            }
            else if (cargaPedido.Pedido.Fronteira != null)
            {
                pedidoFronteiraOrigem = cargaPedido.Pedido.Fronteira;
                pedidoFronteiraDestino = cargaPedido.Pedido.Fronteira;
            }

            Dominio.Entidades.Cliente pessoaFronteira = null;
            if (pedidoFronteiraOrigem != null)
            {
                //if ((cargaPedido.Pedido.Fronteira?.Localidade?.Pais?.Abreviacao ?? "") == "BR") // REFATORAÇÃO DE FRONTEIRAS
                //    pessoaFronteira = repCliente.BuscarFronteiraPorLocalidade(cargaPedido.Pedido.Fronteira?.FronteiraOutroLado?.Localidade?.Codigo ?? 0);
                //else
                pessoaFronteira = repCliente.BuscarFronteiraPorLocalidade(pedidoFronteiraOrigem?.Localidade?.Codigo ?? 0);
            }

            Dominio.Entidades.Localidade destino;
            Dominio.Entidades.Localidade origem;
            if ((cargaPedido.Destino?.Pais?.Abreviacao ?? "") == "BR")
            {
                //if ((cargaPedido.Pedido.Fronteira?.Localidade?.Pais?.Abreviacao ?? "") == "BR") // REFATORAÇÃO DE FRONTEIRAS
                //    destino = cargaPedido.Pedido.Fronteira?.FronteiraOutroLado?.Localidade;
                //else
                destino = pedidoFronteiraDestino?.Localidade;
            }
            else
                destino = cargaPedido.Pedido.Destino;

            if ((cargaPedido.Pedido.Origem?.Pais?.Abreviacao ?? "") == "BR")
            {
                //if ((cargaPedido.Pedido.Fronteira?.Localidade?.Pais?.Abreviacao ?? "") == "BR")  // REFATORAÇÃO DE FRONTEIRAS
                //    origem = cargaPedido.Pedido.Fronteira?.FronteiraOutroLado?.Localidade;
                //else
                origem = pedidoFronteiraOrigem?.Localidade;
            }
            else
                origem = cargaPedido.Pedido.Origem;

            int qtdVolumes = cargaCargaIntegracao.Carga.DadosSumarizados.QuantidadeVolumes.HasValue ? cargaCargaIntegracao.Carga.DadosSumarizados.QuantidadeVolumes.Value : 0;
            if (qtdVolumes <= 0)
                qtdVolumes = notaFiscais.Sum(c => c.XMLNotaFiscal.Volumes);

            manifestacao.infoGeral = new Dominio.ObjetosDeValor.Embarcador.MICDTA.InfoGeral()
            {
                cnpjManifestador = cargaCargaIntegracao.Carga.Empresa.CNPJ_SemFormato,
                paisDestino = destino?.Pais?.Abreviacao ?? "",
                cidadeDestino = destino?.Descricao ?? "",
                indTransitoAduaneiroInternacional = "S",
                docTransporte = new Dominio.ObjetosDeValor.Embarcador.MICDTA.DocTransporte()
                {
                    numero = cargaCargaIntegracao.Carga.CodigoCargaEmbarcador.PadLeft(10, '0'),
                    dataEmissao = cargaCargaIntegracao.Carga.DataCriacaoCarga.ToString("yyyy-MM-dd")
                },
                localSaida = new Dominio.ObjetosDeValor.Embarcador.MICDTA.LocalSaida()
                {
                    codigoURF = pessoaFronteira != null ? pessoaFronteira.CodigoURFAduaneiro : origem?.CodigoURF ?? "",
                    codigoRA = pessoaFronteira != null ? pessoaFronteira.CodigoRAAduaneiro : origem?.CodigoRA ?? ""
                },
                observacoes = "XML MIC/DTA Pre-ACD"
            };
            if (veiculo != null)
            {
                manifestacao.veiculo = new Dominio.ObjetosDeValor.Embarcador.MICDTA.Veiculo()
                {
                    chassi = veiculo.Chassi,
                    anoFabricacao = veiculo.AnoFabricacao,
                    marca = veiculo.Marca?.Descricao ?? "",
                    capacidadeTracao = veiculo.CapacidadeKG > 0 ? (veiculo.CapacidadeKG / 1000).ToString("n1").Replace(",", ".") : "1.0",
                    truck = new Dominio.ObjetosDeValor.Embarcador.MICDTA.Truck()
                    {
                        placa = veiculo.Placa,
                        tara = veiculo.Tara.ToString("D")
                    },
                    condutor = motorista != null ? new Dominio.ObjetosDeValor.Embarcador.MICDTA.Condutor()
                    {
                        numeroCpf = motorista.CPF
                    } : null,
                    proprietario = veiculo.Proprietario != null ? new Dominio.ObjetosDeValor.Embarcador.MICDTA.Proprietario()
                    {
                        numeroCpf = veiculo.Proprietario.Tipo == "F" ? veiculo.Proprietario.CPF_CNPJ_SemFormato : "",
                        numeroCnpj = veiculo.Proprietario.Tipo == "J" ? veiculo.Proprietario.CPF_CNPJ_SemFormato : "",
                    } : null
                };
            }
            manifestacao.transportador = new Dominio.ObjetosDeValor.Embarcador.MICDTA.Transportador()
            {
                numeroApoliceSeguro = apoliceSeguro?.NumeroApolice ?? "",
                dataVencimentoApolice = apoliceSeguro?.FimVigencia.ToString("yyyy-MM-dd"),
                brasileiroProprio = new Dominio.ObjetosDeValor.Embarcador.MICDTA.BrasileiroProprio()
                {
                    licencaTNTI = !string.IsNullOrWhiteSpace(origem?.Pais?.LicencaTNTI ?? "") ? origem?.Pais?.LicencaTNTI ?? "" : integracaoMicDta.LicencaTNTI,
                    dataVencimentoLicenca = origem?.Pais != null && origem.Pais.VencimentoLicencaTNTI.HasValue ? origem.Pais.VencimentoLicencaTNTI.Value.ToString("yyyy-MM-dd") : integracaoMicDta.VencimentoLicencaTNTI.HasValue ? integracaoMicDta.VencimentoLicencaTNTI.Value.ToString("yyyy-MM-dd") : ""
                }
            };
            manifestacao.carga = new Dominio.ObjetosDeValor.Embarcador.MICDTA.Carga()
            {
                consignatario = new Dominio.ObjetosDeValor.Embarcador.MICDTA.Consignatario()
                {
                    indConsignadoAOrdem = "S",
                    pais = cargaPedido.ObterTomador()?.Localidade?.Pais?.Abreviacao ?? ""
                },
                remetente = new Dominio.ObjetosDeValor.Embarcador.MICDTA.Remetente()
                {
                    numeroCnpj = cargaPedido.Pedido.Remetente?.CPF_CNPJ_SemFormato ?? ""
                },
                destinatario = new Dominio.ObjetosDeValor.Embarcador.MICDTA.Destinatario()
                {
                    nome = cargaPedido.Pedido.Destinatario?.Nome ?? "",
                    endereco = (cargaPedido.Pedido.Destinatario?.Endereco ?? "") + ", " + (cargaPedido.Pedido.Destinatario?.Localidade?.Descricao ?? "") + ", " + (cargaPedido.Pedido.Destinatario?.Localidade?.Pais?.Descricao ?? "")
                },
                codigoAduanaDestino = pessoaFronteira != null ? pessoaFronteira.CodigoAduaneiro : pedidoFronteiraDestino?.CodigoAduanaDestino ?? "",
                nomeAduanaDestino = pessoaFronteira != null ? pessoaFronteira.Nome : pedidoFronteiraDestino?.Descricao ?? "",
                paisOrigemMercadorias = cargaPedido.Origem?.Pais?.Abreviacao ?? "",
                valorFOTMercadorias = string.Format("{0:0.00}", cargaCargaIntegracao.Carga.DadosSumarizados.ValorTotalProdutos).Replace(",", "."),
                moedaValorFOT = "220",
                valorSeguro = string.Format("{0:0.00}", apoliceSeguro?.ValorLimiteApolice ?? 0m).Replace(",", "."),
                moedaValorSeguro = "220",
                codigoTiposVolumes = "01",
                nomeTiposVolumes = "Caixas",
                qtdeVolumes = qtdVolumes,
                pesoBruto = string.Format("{0:0.000}", cargaCargaIntegracao.Carga.DadosSumarizados.PesoTotal).Replace(",", "."),
                descricaoMercadorias = !string.IsNullOrWhiteSpace(cargaPedido.Pedido.ProdutoPredominante) ? cargaPedido.Pedido.ProdutoPredominante : produtoProdominante,
                documentosAnexos = "Certificado de Origem"
            };

            manifestacao.carga.nfes = new List<Dominio.ObjetosDeValor.Embarcador.MICDTA.nfe>();
            if (notaFiscais != null && notaFiscais.Count > 0)
            {
                foreach (var notaFiscal in notaFiscais)
                {
                    Dominio.ObjetosDeValor.Embarcador.MICDTA.nfe nfe = new Dominio.ObjetosDeValor.Embarcador.MICDTA.nfe()
                    {
                        chaveAcesso = !string.IsNullOrWhiteSpace(notaFiscal.XMLNotaFiscal.Chave) ? notaFiscal.XMLNotaFiscal.Chave : notaFiscal.XMLNotaFiscal.Numero.ToString("D").PadLeft(44, '0')
                        //nfe = new Dominio.ObjetosDeValor.Embarcador.MICDTA.ChaveNota()
                        //{
                        //    chaveAcesso = !string.IsNullOrWhiteSpace(notaFiscal.XMLNotaFiscal.Chave) ? notaFiscal.XMLNotaFiscal.Chave : notaFiscal.XMLNotaFiscal.Numero.ToString("D").PadLeft(44, '0')
                        //}
                    };
                    manifestacao.carga.nfes.Add(nfe);
                }
            }

            string objetoXml = XML.ConvertObjectToXMLString(manifestacao);
            objetoXml = objetoXml.Replace("</Manifestacao>", "</manifestacao>");
            objetoXml = objetoXml.Replace("<Manifestacao>", "<manifestacao>");

            objetoXml = $@"<ManifestacoesExportacaoPreACDMicDTO xmlns=""http://www.pucomex.serpro.gov.br/cct"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.pucomex.serpro.gov.br/cct ManifestacaoExportacaoPreACDMic.xsd"">" + objetoXml + $@"</ManifestacoesExportacaoPreACDMicDTO>";

            return objetoXml;
        }

        #endregion
    }
}
