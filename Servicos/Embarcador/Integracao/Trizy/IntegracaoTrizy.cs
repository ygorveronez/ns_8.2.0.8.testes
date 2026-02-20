using Dominio.Entidades.Embarcador.Pessoas;
using Dominio.Entidades.Embarcador.SuperApp;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.Relatorios;
using Infrastructure.Services.HttpClientFactory;
using MongoDB.Bson;
using Newtonsoft.Json;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace Servicos.Embarcador.Integracao.Trizy
{
    public class IntegracaoTrizy
    {
        #region Atributos
        private readonly Repositorio.UnitOfWork _unitOfWork;
        #endregion

        #region Construtores
        public IntegracaoTrizy(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Métodos Públicos

        public static void IntegrarApiEnvioComprovante(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComprovanteTrizy tipoComprovanteTrizy, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            Servicos.WebService.CTe.CTe servicoCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);
            Servicos.Embarcador.Pedido.ImpressaoPedido serImpressaoPedido = new Servicos.Embarcador.Pedido.ImpressaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            if (cargaIntegracao.Carga.ProblemaIntegracaoGrMotoristaVeiculo && !cargaIntegracao.Carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Só é possível enviar o comprovante após a GR aprovar ou a carga for liberada.";
                repCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = configuracaoIntegracao.URLTrizy;
            //endPoint = string.Concat(endPoint, "apiEnvioComprovante");
            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;

            HttpClient client = CriarHttpClient(configuracaoIntegracao, endPoint);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            int idPropostaTrizy = repCarregamentoPedido.BuscarIDPropostaTrizyPorCarregamento(cargaIntegracao.Carga.Carregamento?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio();

            parameters.num_romaneio = cargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDLoteTrizy;
            parameters.identificador = !string.IsNullOrWhiteSpace(cargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador) ? cargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador : Utilidades.String.OnlyNumbers(cargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.Protocolo.ToString("n0"));
            parameters.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
            parameters.mensagem = string.Empty;
            parameters.detalhes = string.Empty;
            parameters.cancelar_acordo = "0";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio romaneio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio();
            romaneio.module = "M3002API";
            romaneio.operation = "apiInsRomaneio";
            romaneio.parameters = parameters;

            try
            {
                jsonRequest = JsonConvert.SerializeObject(romaneio, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersComprovante parametersComprovane = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersComprovante();

                    parametersComprovane.descricao = tipoComprovanteTrizy.ObterDescricao();
                    parametersComprovane.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
                    parametersComprovane.nome_arquivo = tipoComprovanteTrizy.ObterNomeArquivo();
                    parametersComprovane.tipo_documento = tipoComprovanteTrizy.ObterNumero();

                    if (tipoComprovanteTrizy == TipoComprovanteTrizy.CTe && cte != null)
                    {
                        parametersComprovane.arquivo = servicoCTe.ObterRetornoPDF(cte, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, unitOfWork);
                        parametersComprovane.num_cte = cte.Numero;
                        parametersComprovane.id_externo = cte.Codigo;
                    }
                    else if (tipoComprovanteTrizy == TipoComprovanteTrizy.MDFe && mdfe != null)
                    {
                        parametersComprovane.arquivo = servicoMDFe.ObterDAMDFE(mdfe.Codigo, mdfe.Empresa.Codigo, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF);
                        parametersComprovane.num_mdfe = mdfe.Numero;
                        parametersComprovane.id_externo = mdfe.Codigo;
                    }
                    else
                    {
                        if (serImpressaoPedido.GerarRelatorioTMS(false, cargaIntegracao.Carga.Pedidos.FirstOrDefault()?.Pedido, false, out string msg, false, cargaIntegracao.Carga, cargaIntegracao.Carga.Codigo, true, false, unitOfWork.StringConexao, tipoServicoMultisoftware, cargaIntegracao.Carga.Empresa.NomeFantasia, cargaIntegracao.Carga.Operador, false, out string guidRelatorio, out string fileName))
                        {
                            if (!string.IsNullOrWhiteSpace(guidRelatorio))
                            {
                                string pastaRelatorios = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();
                                string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, guidRelatorio);
                                string caminhoArquivoFileName = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, fileName.Replace("-", ""));

                                //reportRetorno.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, caminhoArquivo + ".pdf");
                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo + ".pdf"))
                                {
                                    byte[] pdfRelatorioCarga = null;
                                    pdfRelatorioCarga = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivo + ".pdf");
                                    string relatorioCarga = null;

                                    if (pdfRelatorioCarga != null)
                                    {
                                        if (configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF)
                                            relatorioCarga = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, pdfRelatorioCarga));
                                        else
                                            relatorioCarga = Convert.ToBase64String(pdfRelatorioCarga);

                                        parametersComprovane.arquivo = relatorioCarga;
                                    }
                                }
                                else if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoFileName))
                                {
                                    byte[] pdfRelatorioCarga = null;
                                    pdfRelatorioCarga = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivoFileName);
                                    string relatorioCarga = null;

                                    if (pdfRelatorioCarga != null)
                                    {
                                        if (configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF)
                                            relatorioCarga = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, pdfRelatorioCarga));
                                        else
                                            relatorioCarga = Convert.ToBase64String(pdfRelatorioCarga);

                                        parametersComprovane.arquivo = relatorioCarga;
                                    }
                                }
                                parametersComprovane.id_externo = cargaIntegracao.Carga.Protocolo;
                            }
                        }
                    }
                    parametersComprovane.peso_carregado = cargaIntegracao.Carga != null && cargaIntegracao.Carga.DadosSumarizados != null && cargaIntegracao.Carga.DadosSumarizados.PesoTotal > 0 ? cargaIntegracao.Carga.DadosSumarizados.PesoTotal : cargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.PesoTotal;


                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioComprovante envioComprovante = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioComprovante();
                    envioComprovante.module = "M3002API";
                    envioComprovante.operation = "apiEnvioComprovante";
                    envioComprovante.parameters = parametersComprovane;

                    try
                    {
                        jsonRequest = JsonConvert.SerializeObject(envioComprovante, Formatting.Indented);
                        content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                        result = client.PostAsync(endPoint, content).Result;
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        if (result.IsSuccessStatusCode)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);

                            situacaoIntegracao = true;
                            mensagemErro = string.Empty;

                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                            arquivoIntegracao.Mensagem = mensagemErro;
                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                            if (retorno == null)
                                mensagemErro = result.StatusCode.ToString();
                            else
                                mensagemErro = retorno.message;
                            if (string.IsNullOrWhiteSpace(mensagemErro))
                                mensagemErro = "Falha na integração com a Trizy.";
                            else
                                mensagemErro = "Retorno Trizy: " + mensagemErro;

                            situacaoIntegracao = false;

                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                            arquivoIntegracao.Mensagem = mensagemErro;
                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaIntegracao.ProblemaIntegracao = mensagemErro;
                            repCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
                        }
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                        mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";
                        situacaoIntegracao = false;

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = mensagemErro;
                        repCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
                        return;
                    }

                    if (!situacaoIntegracao)
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = mensagemErro;
                    }
                    else
                    {
                        cargaIntegracao.ProblemaIntegracao = string.Empty;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }

                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = mensagemErro;
                    repCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            if (!situacaoIntegracao)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
            }
            else
            {
                cargaIntegracao.ProblemaIntegracao = string.Empty;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
            }

            repCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarApiMultiplasEntregas(ref Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo repPedidoIntegracaoArquivo = new Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            pedidoIntegracao.Tentativas += 1;
            pedidoIntegracao.DataEnvio = DateTime.Now;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = configuracaoIntegracao.URLTrizy;
            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersEntregas parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersEntregas();
            parameters.identificador = !string.IsNullOrWhiteSpace(pedidoIntegracao.Pedido.NumeroPedidoEmbarcador) ? pedidoIntegracao.Pedido.NumeroPedidoEmbarcador : Utilidades.String.OnlyNumbers(pedidoIntegracao.Pedido.Protocolo.ToString("n0"));
            parameters.tipo = 2;//"Tipo de lote 1-Proposta 2-Lote\"
            parameters.publica = 0;
            parameters.agencia = configuracaoIntegracao.AgenciaTrizy;
            parameters.peso_bruto = pedidoIntegracao.Pedido.PesoTotal / 1000;
            parameters.peso_bruto_min = pedidoIntegracao.Pedido.PesoTotal / 1000;
            parameters.peso_bruto_max = pedidoIntegracao.Pedido.PesoTotal / 1000;
            parameters.adiantamento = pedidoIntegracao.Pedido.PercentualAdiantamentoTerceiro;
            parameters.adiantamento_min = pedidoIntegracao.Pedido.PercentualMinimoAdiantamentoTerceiro;
            parameters.adiantamento_max = pedidoIntegracao.Pedido.PercentualMaximoAdiantamentoTerceiro;

            parameters.tipo_carreta = new string[] { "TOCO_AB" };
            if (pedidoIntegracao.Pedido.ModelosVeiculares != null && pedidoIntegracao.Pedido.ModelosVeiculares.Count > 0)
            {
                parameters.tipo_carroceria = new string[pedidoIntegracao.Pedido.ModelosVeiculares.Count];
                int i = 0;
                foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular in pedidoIntegracao.Pedido.ModelosVeiculares)
                {
                    parameters.tipo_carroceria[i] = (!string.IsNullOrWhiteSpace(modeloVeicular.CodigoIntegracao) ? modeloVeicular.CodigoIntegracao : modeloVeicular.Descricao);
                    i++;
                }
            }
            else
                parameters.tipo_carroceria = new string[] { (pedidoIntegracao.Pedido.ModeloVeicularCarga?.CodigoIntegracao ?? pedidoIntegracao.Pedido.ModeloVeicularCarga?.Descricao ?? "") };
            parameters.valor = pedidoIntegracao.Pedido.ValorFreteToneladaTerceiro;
            parameters.valor_por = 1;//1-Tonelada 2-Lotação 3-Volume
            parameters.observacao = pedidoIntegracao.Pedido.ObservacaoCTe;
            parameters.data_carregamento = pedidoIntegracao.Pedido.DataPrevisaoSaida.HasValue ? pedidoIntegracao.Pedido.DataPrevisaoSaida.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
            parameters.proposta_consolidada_id = pedidoIntegracao.Pedido.Protocolo;
            parameters.cpf_motorista = pedidoIntegracao.Pedido.Motoristas?.FirstOrDefault()?.CPF ?? "";
            parameters.pedagio_pago_embarcador = 0;//"Pedágio é pago pelo embarcador? 0-Não 1-Sim\
            parameters.excluir = 0;//"Excluir proposta? 0-Não 1-Sim\

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            double lat = Convert.ToDouble(pedidoIntegracao.Pedido.Remetente?.Latitude ?? "0", provider);
            double lng = Convert.ToDouble(pedidoIntegracao.Pedido.Remetente?.Longitude ?? "0", provider);

            parameters.origem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnderecoOrigem()
            {
                bairro = pedidoIntegracao.Pedido.Remetente?.Bairro ?? "",
                complemento = pedidoIntegracao.Pedido.Remetente?.Complemento ?? "",
                latitude = lat,
                longitude = lng,
                logradouro = pedidoIntegracao.Pedido.Remetente?.Endereco ?? "",
                municipio = pedidoIntegracao.Pedido.Remetente?.Localidade?.CodigoIBGE ?? pedidoIntegracao.Pedido.Origem?.CodigoIBGE ?? 0,
                numero = pedidoIntegracao.Pedido.Remetente?.Numero ?? "",
                uf = pedidoIntegracao.Pedido.Remetente?.Localidade?.Estado?.Sigla ?? pedidoIntegracao.Pedido.Origem?.Estado?.Sigla ?? ""
            };
            parameters.entregas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnderecoEntrega>();
            if (pedidoIntegracao.Pedido.Destinatario != null)
            {
                lat = Convert.ToDouble(pedidoIntegracao.Pedido.Destinatario?.Latitude ?? "0", provider);
                lng = Convert.ToDouble(pedidoIntegracao.Pedido.Destinatario?.Longitude ?? "0", provider);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnderecoEntrega entrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnderecoEntrega()
                {
                    bairro = pedidoIntegracao.Pedido.Destinatario?.Bairro ?? "",
                    complemento = pedidoIntegracao.Pedido.Destinatario?.Complemento ?? "",
                    latitude = lat,// pedidoIntegracao.Pedido.Destinatario?.Latitude ?? 0,
                    longitude = lng,
                    logradouro = pedidoIntegracao.Pedido.Destinatario?.Endereco ?? "",
                    municipio = pedidoIntegracao.Pedido.Destinatario?.Localidade?.CodigoIBGE ?? pedidoIntegracao.Pedido.Origem?.CodigoIBGE ?? 0,
                    numero = pedidoIntegracao.Pedido.Destinatario?.Numero ?? "",
                    uf = pedidoIntegracao.Pedido.Destinatario?.Localidade?.Estado?.Sigla ?? pedidoIntegracao.Pedido.Origem?.Estado?.Sigla ?? "",
                    data = pedidoIntegracao.Pedido.PrevisaoEntrega.HasValue ? pedidoIntegracao.Pedido.PrevisaoEntrega.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                    instrucao = "",
                    //mercadoria = pedidoIntegracao.Pedido.Produtos?.FirstOrDefault()?.Produto?.CodigoProdutoEmbarcador ?? "",
                    ordem = 1,
                    quantidade = (int)(pedidoIntegracao.Pedido.Produtos?.FirstOrDefault()?.Quantidade ?? 0),
                    mercadoria_id = pedidoIntegracao.Pedido.Produtos?.FirstOrDefault()?.Produto?.CodigoProdutoEmbarcador ?? "",
                    tipo = 1,//1-Entreg
                    unidade = 1,//0-M³ 1-KG 2-TON 3-Un 4-Lt
                    valor = 0,
                    valor_extra = 0m// pedidoIntegracao.Pedido.ValorPedagioRota
                };
                parameters.entregas.Add(entrega);
            }
            parameters.liberar_leilao = 0;
            parameters.transportadora = "";
            //parameters.leilao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Leilao();
            //parameters.campos_adicionais = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Adicionais();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.MultiplasEntregas multiplasEntregas = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.MultiplasEntregas();
            multiplasEntregas.module = "M3002API";
            multiplasEntregas.operation = "apiMultiplasEntregas";
            multiplasEntregas.parameters = parameters;

            try
            {
                jsonRequest = JsonConvert.SerializeObject(multiplasEntregas, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);

                    if (retorno != null && retorno.result != null && retorno.success)
                    {
                        situacaoIntegracao = true;
                        pedidoIntegracao.Pedido.IDPropostaTrizy = 0;
                        pedidoIntegracao.Pedido.IDLoteTrizy = retorno.result.lote_id;
                        mensagemErro = retorno.result.mensagem;
                        repPedido.Atualizar(pedidoIntegracao.Pedido);
                    }
                    else if (retorno != null && retorno.success)
                        situacaoIntegracao = true;
                    else
                        situacaoIntegracao = false;

                    mensagemErro = string.Empty;

                    Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                    arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                    pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                else
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                        if (retorno == null)
                            mensagemErro = result.StatusCode.ToString();
                        else
                            mensagemErro = retorno.message;
                    }
                    catch
                    {
                        mensagemErro = result.Content.ReadAsStringAsync().Result;
                    }
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                    arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                    pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    pedidoIntegracao.ProblemaIntegracao = mensagemErro;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
                return;
            }

            if (!situacaoIntegracao)
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
            }
            else
            {
                pedidoIntegracao.ProblemaIntegracao = string.Empty;
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            }
        }

        public static void IntegrarApiIsRomaneio(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, string mensagem = "")
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            cargaCargaIntegracao.NumeroTentativas += 1;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;
            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                if (!configuracaoIntegracao.PossuiIntegracaoTrizy || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLTrizy))
                    throw new ServicoException("Configuração da Integração Trizy não está habilitada.");

                string endPoint = configuracaoIntegracao.URLTrizy;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

                client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

                int idPropostaTrizy = repCarregamentoPedido.BuscarIDPropostaTrizyPorCarregamento(cargaCargaIntegracao.Carga.Carregamento?.Codigo ?? 0);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio();

                parameters.num_romaneio = cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDLoteTrizy;
                parameters.identificador = !string.IsNullOrWhiteSpace(cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador) ? cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador : Utilidades.String.OnlyNumbers(cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.Protocolo.ToString("n0"));
                parameters.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
                parameters.mensagem = mensagem;
                parameters.detalhes = mensagem;
                parameters.cancelar_acordo = "0";

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio romaneio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio();
                romaneio.module = "M3002API";
                romaneio.operation = "apiInsRomaneio";
                romaneio.parameters = parameters;

                jsonRequest = JsonConvert.SerializeObject(romaneio, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);

                    situacaoIntegracao = true;
                    mensagemErro = string.Empty;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    if (cargaCargaIntegracao.Carga.TipoOperacao != null && cargaCargaIntegracao.Carga.TipoOperacao.EnviarComprovantesDaCarga)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);
                        if (cargasCTe != null && cargasCTe.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
                                IntegrarApiEnvioComprovanteCTe(cargaCargaIntegracao, unitOfWork, cargaCTe.CTe);
                        }
                        List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargasMDFe = repCargaMDFe.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);
                        if (cargasMDFe != null && cargasMDFe.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargasMDFe)
                                IntegrarApiEnvioComprovanteMDFe(cargaCargaIntegracao, unitOfWork, cargaMDFe.MDFe);
                        }
                    }
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                    repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                }
            }
            catch (ServicoException excecao)
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = excecao.Message;

                situacaoIntegracao = false;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

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

        public static void IntegrarApiIsRomaneio(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, string mensagem)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(cargaCargaIntegracao.CargaCancelamento.Carga.Codigo);

            cargaCargaIntegracao.NumeroTentativas += 1;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = configuracaoIntegracao.URLTrizy;
            //endPoint = string.Concat(endPoint, "apiAgendamento");
            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            int idPropostaTrizy = repCarregamentoPedido.BuscarIDPropostaTrizyPorCarregamento(cargaCargaIntegracao.CargaCancelamento.Carga.Carregamento?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio();

            parameters.num_romaneio = cargaCargaIntegracao.CargaCancelamento.Carga.Pedidos.FirstOrDefault().Pedido.IDLoteTrizy;
            parameters.identificador = !string.IsNullOrWhiteSpace(cargaCargaIntegracao.CargaCancelamento.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador) ? cargaCargaIntegracao.CargaCancelamento.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador : Utilidades.String.OnlyNumbers(cargaCargaIntegracao.CargaCancelamento.Carga.Pedidos.FirstOrDefault().Pedido.Protocolo.ToString("n0"));
            parameters.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaCargaIntegracao.CargaCancelamento.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
            parameters.mensagem = mensagem;
            parameters.detalhes = mensagem;
            parameters.cancelar_acordo = "1";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio romaneio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio();
            romaneio.module = "M3002API";
            romaneio.operation = "apiInsRomaneio";
            romaneio.parameters = parameters;

            try
            {
                jsonRequest = JsonConvert.SerializeObject(romaneio, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);

                    situacaoIntegracao = true;
                    mensagemErro = string.Empty;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                    repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }

            if (!situacaoIntegracao)
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
            else
            {
                cargaCargaIntegracao.ProblemaIntegracao = string.Empty;
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
        }

        public static void IntegrarApiEncerraLote(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            carregamentoIntegracao.NumeroTentativas += 1;
            carregamentoIntegracao.DataIntegracao = DateTime.Now;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = configuracaoIntegracao.URLTrizy;
            //endPoint = string.Concat(endPoint, "apiAgendamento");
            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersAlteraPesoLote parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersAlteraPesoLote();
            parameters.identificador = !string.IsNullOrWhiteSpace(carregamentoIntegracao.Carregamento.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador) ? carregamentoIntegracao.Carregamento.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador : Utilidades.String.OnlyNumbers(carregamentoIntegracao.Carregamento.Pedidos.FirstOrDefault().Pedido.Protocolo.ToString("n0"));
            parameters.encerra_lote = 0;
            parameters.peso_bruto = carregamentoIntegracao.Carregamento.PesoCarregamento;
            parameters.operacao = "remover";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.AlteraPesoLote alteraPesoLote = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.AlteraPesoLote();
            alteraPesoLote.module = "M3002API";
            alteraPesoLote.operation = "apiAlteraPesoLote";
            alteraPesoLote.parameters = parameters;

            try
            {
                jsonRequest = JsonConvert.SerializeObject(alteraPesoLote, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);

                    situacaoIntegracao = true;
                    mensagemErro = string.Empty;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = carregamentoIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    carregamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = carregamentoIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    carregamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    carregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    carregamentoIntegracao.ProblemaIntegracao = mensagemErro;
                    repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = carregamentoIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                carregamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                carregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = mensagemErro;
                repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
                return;
            }

            if (!situacaoIntegracao)
            {
                carregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = mensagemErro;
                repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
            }
            else
            {
                carregamentoIntegracao.ProblemaIntegracao = string.Empty;
                carregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
            }
        }

        public static void IntegrarCargaAPPTrizy(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, string mensagem = "", AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware = null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCargaIntegracao.Carga;
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            List<int> veiculosVinculados = carga.VeiculosVinculados?.Select(o => o.Codigo)?.ToList();
            Dominio.Entidades.Embarcador.Cargas.Carga cargaPendente = repCarga.BuscarCargaViagemAberta(carga.Motoristas.Select(o => o.Codigo).FirstOrDefault(), carga.Codigo, veiculosVinculados, carga.Veiculo?.Codigo ?? 0, (configuracaoIntegracaoTrizy?.IntegrarApenasCargasComControleDeEntrega ?? false));
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            cargaCargaIntegracao.NumeroTentativas += 1;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            VersaoIntegracaoTrizy versaoIntegracaoGlobal = configuracaoIntegracaoTrizy == null || configuracaoIntegracaoTrizy.VersaoIntegracao == 0
                        ? VersaoIntegracaoTrizy.Versao1
                        : configuracaoIntegracaoTrizy.VersaoIntegracao;

            VersaoIntegracaoTrizy versaoIntegracaoOperacao = carga?.TipoOperacao?.ConfiguracaoTrizy?.VersaoIntegracao == null || carga?.TipoOperacao?.ConfiguracaoTrizy?.VersaoIntegracao == 0
                        ? VersaoIntegracaoTrizy.Versao1
                        : carga?.TipoOperacao?.ConfiguracaoTrizy?.VersaoIntegracao ?? VersaoIntegracaoTrizy.Versao1;

            VersaoIntegracaoTrizy versaoIntegracao =
                configuracaoIntegracaoTrizy?.ValidarIntegracaoPorOperacao == true
                    ? versaoIntegracaoOperacao
                    : versaoIntegracaoGlobal;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/{versaoIntegracao.ObterDescricaoRota()}/external/travel";
            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;
            string idTrizyAntigo = carga.IDIdentificacaoTrizzy;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            try
            {
                if (((carga.Motoristas == null) || !(carga.Motoristas.Any())) && (carga?.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false))
                    throw new ServicoException($"A carga não tem integração para o App Trizy para Modal Ferroviário", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroCanceladoOuAnulado);

                if ((carga.Motoristas == null) || !(carga.Motoristas.Any()))
                    throw new ServicoException($"Carga sem motorista definido", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroCanceladoOuAnulado);

                if (cargaCargaIntegracao.Carga.SituacaoCarga == SituacaoCarga.Cancelada)
                    throw new ServicoException($"Não é possível integrar uma carga cancelada com a Trizy");

                if (!(configuracaoIntegracaoTrizy?.PermitirIntegrarMultiplasCargasParaOMesmoMotorista ?? false) && !string.IsNullOrWhiteSpace(cargaPendente?.CodigoCargaEmbarcador))
                    throw new ServicoException($"A carga {cargaPendente.CodigoCargaEmbarcador} está pendente de finalização, para envio de nova carga para Trizy");

                if (!VerificarSituacaoRoteirizacao(carga, configuracaoTMS, unitOfWork, tipoServicoMultisoftware))
                    throw new ServicoException($"A Carga {carga.CodigoCargaEmbarcador} está pendente de roteirização, ao finalizar a roteirização a integração será liberada");

                if (configuracaoControleEntrega.PermitirAjustarEntregasEtapasAnterioresIntegracao)
                    ValidarNotasFiscaisControleEntrega(carga, unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega = repCargaEntrega.BuscarPorCarga(carga.Codigo);

                dynamic objEnvio = versaoIntegracao == VersaoIntegracaoTrizy.Versao1 ? ObterObjetoIntegracaoCargaAPP(configuracaoTMS, carga, listaCargaEntrega, unitOfWork, clienteMultisoftware, configuracaoIntegracaoTrizy) : ObterObjetoIntegracaoCargaAPPV3(configuracaoTMS, carga, listaCargaEntrega, unitOfWork, clienteMultisoftware, configuracaoIntegracaoTrizy);

                if (!string.IsNullOrWhiteSpace(idTrizyAntigo))
                {
                    AtualizarViagem(carga, "CANCELED", unitOfWork, null, false, null, false, null, cargaCargaIntegracao, " - Viagem cancelada via Reenvio", idViagemTrizy: idTrizyAntigo, cancelamentoPorReenvio: true);
                }

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                jsonRequest = JsonConvert.SerializeObject(objEnvio, Formatting.Indented, settings);

                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;

                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoCargaAPP retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoCargaAPP>(result.Content.ReadAsStringAsync().Result);

                    SalvarDadosRetorno(carga, retorno, listaCargaEntrega, false, unitOfWork);

                    situacaoIntegracao = true;
                    EnviarNotificacao(carga.Motoristas?.FirstOrDefault()?.CPF, "Nova carga", $"Carga {carga.CodigoCargaEmbarcador} disponível", unitOfWork);

                    mensagemErro = "Integrado com sucesso. ID trizy: " + carga.IDIdentificacaoTrizzy;
                    cargaCargaIntegracao.Protocolo = carga.IDIdentificacaoTrizzy;

                    SalvarArquivoTransacaoIntegrarCargaAPPTrizy(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro, cargaCargaIntegracao);

                    repCarga.Atualizar(carga);

                    if (configuracaoIntegracaoTrizy?.EnviarPDFDocumentosFiscais ?? false)
                    {
                        ICollection<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DocumentosFiscaisTrizy> listaDocumentosFiscais = configuracaoIntegracaoTrizy.DocumentosFiscaisEnvioPDF;
                        if ((listaDocumentosFiscais?.Count ?? 0) > 0)
                            EnviarPDFDosDocumentosFiscais(cargaCargaIntegracao, listaDocumentosFiscais, unitOfWork, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                    {
                        mensagemErro = retorno.message;
                        if (!string.IsNullOrEmpty(retorno.error))
                        {
                            if (mensagemErro.Contains("Could not create the external travel"))
                                mensagemErro = "Não foi possível integrar a carga. Motivo da falha: " + retorno.error;
                            if (mensagemErro.Contains("Could not update the travel"))
                                mensagemErro = "Não foi possível atualizar a carga. Motivo da falha: " + retorno.error;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                    Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                    Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                    situacaoIntegracao = false;
                    SalvarArquivoTransacaoIntegrarCargaAPPTrizy(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro, cargaCargaIntegracao);

                    cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                    repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                }
            }
            catch (ServicoException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroCanceladoOuAnulado)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                situacaoIntegracao = true;

                cargaCargaIntegracao.AguardarFinalizarCargaAnterior = true;
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;

                Servicos.Auditoria.Auditoria.AuditarSemDadosUsuario(cargaCargaIntegracao, cargaCargaIntegracao.ProblemaIntegracao, unitOfWork);

                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                situacaoIntegracao = false;

                cargaCargaIntegracao.AguardarFinalizarCargaAnterior = true;
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                cargaCargaIntegracao.CargaPendente = cargaPendente;

                Servicos.Auditoria.Auditoria.AuditarSemDadosUsuario(cargaCargaIntegracao, cargaCargaIntegracao.ProblemaIntegracao, unitOfWork);

                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint);
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy);
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizyCarga");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";
                situacaoIntegracao = false;

                SalvarArquivoTransacaoIntegrarCargaAPPTrizy(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro, cargaCargaIntegracao);

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
                cargaCargaIntegracao.AguardarFinalizarCargaAnterior = false;
                cargaCargaIntegracao.CargaPendente = null;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
        }

        public static void IntegrarCargaDadosTransporteAPPTrizy(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporte, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware = null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, string menssagemAdicionalHistorico = "")
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaDadosTransporte.Carga;
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            List<int> veiculosVinculados = carga.VeiculosVinculados?.Select(o => o.Codigo)?.ToList();
            Dominio.Entidades.Embarcador.Cargas.Carga cargaPendente = repCarga.BuscarCargaViagemAberta(carga.Motoristas.Select(o => o.Codigo).FirstOrDefault(), carga.Codigo, veiculosVinculados, carga.Veiculo?.Codigo ?? 0, (configuracaoIntegracaoTrizy?.IntegrarApenasCargasComControleDeEntrega ?? false));
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repConfiguracaoMotorista.BuscarConfiguracaoPadrao();

            cargaDadosTransporte.NumeroTentativas += 1;
            cargaDadosTransporte.DataIntegracao = DateTime.Now;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            VersaoIntegracaoTrizy versaoIntegracaoGlobal = configuracaoIntegracaoTrizy == null || configuracaoIntegracaoTrizy.VersaoIntegracao == 0
                        ? VersaoIntegracaoTrizy.Versao1
                        : configuracaoIntegracaoTrizy.VersaoIntegracao;

            VersaoIntegracaoTrizy versaoIntegracaoOperacao = carga?.TipoOperacao?.ConfiguracaoTrizy?.VersaoIntegracao == null || carga?.TipoOperacao?.ConfiguracaoTrizy?.VersaoIntegracao == 0
                        ? VersaoIntegracaoTrizy.Versao1
                        : carga?.TipoOperacao?.ConfiguracaoTrizy?.VersaoIntegracao ?? VersaoIntegracaoTrizy.Versao1;

            VersaoIntegracaoTrizy versaoIntegracao =
                configuracaoIntegracaoTrizy?.ValidarIntegracaoPorOperacao == true
                    ? versaoIntegracaoOperacao
                    : versaoIntegracaoGlobal;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/{versaoIntegracao.ObterDescricaoRota()}/external/travel";
            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            try
            {
                if (((carga.Motoristas == null) || !(carga.Motoristas.Any())) && (carga?.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false))
                    throw new ServicoException($"A carga não tem integração para o App Trizy para Modal Ferroviário", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroCanceladoOuAnulado);

                if ((carga.Motoristas == null) || !(carga.Motoristas.Any()))
                {
                    Auditoria.Auditoria.Auditar(new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema }, carga, "Carga sem motorista definido na integração", unitOfWork);
                    throw new ServicoException($"Carga sem motorista definido", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroCanceladoOuAnulado);
                }
                Dominio.Entidades.Usuario primeiroMotorista = carga.Motoristas.FirstOrDefault();
                if (configuracaoMotorista != null && configuracaoMotorista.NaoGerarPreTripMotoristasIgnorados && configuracaoMotorista.MotoristasIgnorados != null && configuracaoMotorista.MotoristasIgnorados.Count > 0 && configuracaoMotorista.MotoristasIgnorados.Any(obj => obj.ToLower() == primeiroMotorista.Nome.ToLower()))
                    throw new ServicoException($"A configuração atual não permite enviar esse motorista");

                if (cargaDadosTransporte.Carga.SituacaoCarga == SituacaoCarga.Cancelada)
                    throw new ServicoException($"Não é possível integrar uma carga cancelada com a Trizy");

                if (!(configuracaoIntegracaoTrizy?.PermitirIntegrarMultiplasCargasParaOMesmoMotorista ?? false) && !string.IsNullOrWhiteSpace(cargaPendente?.CodigoCargaEmbarcador))
                    throw new ServicoException($"A carga {cargaPendente.CodigoCargaEmbarcador} está pendente de finalização, para envio de nova carga para Trizy");

                if (!VerificarSituacaoRoteirizacao(carga, configuracaoTMS, unitOfWork, tipoServicoMultisoftware))
                    throw new ServicoException($"A Carga {carga.CodigoCargaEmbarcador} está pendente de roteirização, ao finalizar a roteirização a integração será liberada");

                if (repCargaCargaIntegracao.ExistePorCargaETipo(carga.Codigo, TipoIntegracao.Trizy))
                    throw new ServicoException($"Integração PréTrip ignorada pois carga não está mais na situação de integração PréTrip.");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega = repCargaEntrega.BuscarPorCarga(carga.Codigo);

                dynamic parameters = versaoIntegracao == VersaoIntegracaoTrizy.Versao1 ? ObterObjetoIntegracaoCargaAPP(configuracaoTMS, carga, listaCargaEntrega, unitOfWork, clienteMultisoftware, configuracaoIntegracaoTrizy, false, true) : ObterObjetoIntegracaoCargaAPPV3(configuracaoTMS, carga, listaCargaEntrega, unitOfWork, clienteMultisoftware, configuracaoIntegracaoTrizy, false, true);

                if (!string.IsNullOrWhiteSpace(carga.IDIdentificacaoTrizzy))
                {
                    AtualizarViagem(cargaDadosTransporte.Carga, "CANCELED", unitOfWork, null, false, cargaDadosTransporte, false, null, null, " - Removido Motorista");
                }

                jsonRequest = JsonConvert.SerializeObject(parameters, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoCargaAPP retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoCargaAPP>(result.Content.ReadAsStringAsync().Result);

                    SalvarDadosRetorno(carga, retorno, listaCargaEntrega, true, unitOfWork);

                    situacaoIntegracao = true;
                    EnviarNotificacao(carga.Motoristas?.FirstOrDefault()?.CPF, "Nova carga", $"Carga {carga.CodigoCargaEmbarcador} disponível", unitOfWork);

                    mensagemErro = "Integrado com sucesso. ID trizy: " + carga.IDIdentificacaoTrizzy;
                    cargaDadosTransporte.Protocolo = carga.IDIdentificacaoTrizzy;

                    servicoArquivoTransacao.Adicionar(cargaDadosTransporte, jsonRequest, jsonResponse, "json", mensagemErro + menssagemAdicionalHistorico);

                    if (carga.TipoOperacao != null && carga.TipoOperacao.EnviarComprovantesDaCarga)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCarga(carga.Codigo);
                        if (cargasCTe != null && cargasCTe.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
                                IntegrarCargaDadosTrasporteApiEnvioComprovanteCTe(cargaDadosTransporte, unitOfWork, cargaCTe.CTe);
                        }
                        List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargasMDFe = repCargaMDFe.BuscarPorCarga(carga.Codigo);
                        if (cargasMDFe != null && cargasMDFe.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargasMDFe)
                                IntegrarCargaDadosTrasporteApiEnvioComprovanteMDFe(cargaDadosTransporte, unitOfWork, cargaMDFe.MDFe);
                        }
                    }
                    repCarga.Atualizar(carga);
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    Log.TratarErro("URL: " + endPoint);
                    Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy);
                    Log.TratarErro("IntegracaoTrizyCarga");
                    Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                    Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                    situacaoIntegracao = false;
                    servicoArquivoTransacao.Adicionar(cargaDadosTransporte, jsonRequest, jsonResponse, "json", mensagemErro + menssagemAdicionalHistorico);

                    cargaDadosTransporte.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaDadosTransporte.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                    repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporte);
                }
            }
            catch (ServicoException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroCanceladoOuAnulado)
            {
                Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                cargaDadosTransporte.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaDadosTransporte.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                cargaDadosTransporte.AguardarFinalizarCargaAnterior = true;

                Servicos.Auditoria.Auditoria.AuditarSemDadosUsuario(cargaDadosTransporte, cargaDadosTransporte.ProblemaIntegracao, unitOfWork);

                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporte);
                return;
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;

                cargaDadosTransporte.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporte.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                cargaDadosTransporte.CargaPendente = cargaPendente;
                cargaDadosTransporte.AguardarFinalizarCargaAnterior = true;
                situacaoIntegracao = true;

                Servicos.Auditoria.Auditoria.AuditarSemDadosUsuario(cargaDadosTransporte, cargaDadosTransporte.ProblemaIntegracao, unitOfWork);

                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporte);
                return;
            }
            catch (Exception excecao)
            {
                Log.TratarErro("URL: " + endPoint);
                Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy);
                Log.TratarErro(excecao, "IntegracaoTrizyCarga");
                Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";
                situacaoIntegracao = false;

                servicoArquivoTransacao.Adicionar(cargaDadosTransporte, jsonRequest, jsonResponse, "json", mensagemErro + menssagemAdicionalHistorico);

                cargaDadosTransporte.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporte.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporte);
                return;
            }

            if (!situacaoIntegracao)
            {
                cargaDadosTransporte.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporte.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporte);
            }
            else
            {
                cargaDadosTransporte.ProblemaIntegracao = mensagemErro;
                cargaDadosTransporte.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaDadosTransporte.CargaPendente = null;
                cargaDadosTransporte.AguardarFinalizarCargaAnterior = false;
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporte);
            }
        }

        public static void IntegrarPreTripAPPTrizy(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporte, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            IntegrarCargaDadosTransporteAPPTrizy(cargaDadosTransporte, unitOfWork, clienteMultisoftware, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, " - Novo Motorista");
        }

        public static void IntegrarChamadoOcorrencia(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, EventoIntegracaoOcorrenciaTrizy tipoEvento, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/core/v1/external/occurrence/{chamado.IdOcorrenciaTrizy}";
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), endPoint);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioOcorrencia objetoEnvio = ConverterObjetoEnvioOcorrencia(tipoEvento, usuario, unitOfWork);

            try
            {
                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    EnviarNotificacao(chamado.Motorista?.CPF, "Retorno de Atendimento", $"Atendimento {chamado.Numero} retornado", unitOfWork);
                    mensagemErro = "Integrado com sucesso";
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }

                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
            }
            catch (Exception)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                return;
            }
        }

        public static void EnviarMensagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, string mensagem, DateTime dataMensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string idViagem = carga.IDIdentificacaoTrizzy;
            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v1/external/travel/{idViagem}/chat";
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnviarMensagem objetoEnvio = ConverterObjetoEnviarMensagem(mensagem, dataMensagem, carga.IDIdentificacaoTrizzy);

            try
            {
                if (string.IsNullOrWhiteSpace(idViagem))
                    throw new ServicoException("A carga não tem ID Viagem");

                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    EnviarNotificacao(carga.Motoristas?.FirstOrDefault()?.CPF, "Nova mensagem de chat", "Nova mensagem de chat", unitOfWork);
                    mensagemErro = "Integrado com sucesso";
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }

                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
            }
            catch (ServicoException excecao)
            {

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Exception: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = excecao.Message;

                return;
            }
            catch (Exception excecao)
            {

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Exception: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                return;
            }
            finally
            {
                Servicos.Log.TratarErro($"Integração com a Trizy: {mensagemErro}");
            }

        }

        public static bool AtualizarViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, string status, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaIntegracao = null, bool atualizarCargaAntiga = false, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporte = null, bool preTrip = false, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = null, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = null, string menssagemAdicionalHistorico = "", string idViagemTrizy = "", bool cancelamentoPorReenvio = false, string stringConexao = "")
        {
            //if (!string.IsNullOrEmpty(stringConexao))
            //    unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegraca = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaInteracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            if (atualizarCargaAntiga)
                AtualizarViagemAntiga(new Dominio.ObjetosDeValor.WebService.Carga.Carga { Protocolo = carga?.Codigo ?? 0, NumeroCarga = carga?.CodigoCargaEmbarcador ?? string.Empty }, unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (cargaIntegracao != null)
            {
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;
            }
            else if (cargaDadosTransporte != null)
            {
                cargaDadosTransporte.NumeroTentativas += 1;
                cargaDadosTransporte.DataIntegracao = DateTime.Now;
            }
            else if (cargaCargaIntegracao != null)
            {
                cargaCargaIntegracao.NumeroTentativas += 1;
                cargaCargaIntegracao.DataIntegracao = DateTime.Now;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (string.IsNullOrEmpty(idViagemTrizy))
                idViagemTrizy = carga?.IDIdentificacaoTrizzy ?? "";

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v1/external/travel/{idViagemTrizy}";
            string mensagemErro = string.Empty;
            bool situacaoIntegracao = false;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), endPoint);
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            dynamic objetoEnvio;

            if (preTrip && cargaEntrega != null)
            {
                endPoint += $"/stopping-point/{cargaEntrega.IdTrizy}";
                objetoEnvio = ConverterObjetoAtualizarViagemPretrip(cargaEntrega, unitOfWork);
            }
            else
            {
                objetoEnvio = ConverterObjetoAtualizarViagem(status);
            }

            try
            {
                if (string.IsNullOrWhiteSpace(idViagemTrizy))
                    throw new ServicoException("Carga sem ID Trizy", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NenhumRegistroEncontrado);
                if (cargaCargaIntegracao != null && !cargaCargaIntegracao.FinalizarCargaAnterior && !cancelamentoPorReenvio)
                    throw new ServicoException("ID Trizy já foi finalizado", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NenhumRegistroEncontrado);

                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                    if (preTrip)
                        mensagemErro = "Integrado com sucesso envio das NFe";
                    else if (cargaCargaIntegracao != null && !cancelamentoPorReenvio)
                        mensagemErro = "Pré-trip finalizado com sucesso";
                    else
                        mensagemErro = "Integrado com sucesso";

                    situacaoIntegracao = true;

                    try
                    {
                        if (cargaCargaIntegracao?.FinalizarCargaAnterior ?? false)
                            Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaParaIntegracao(carga, cargaCargaIntegracao.TipoIntegracao, unitOfWork, true, false, false, false, forcarNovaIntegracao: true);
                    }
                    catch (Exception ex)
                    {
                        Log.TratarErro(ex, "IntegracaoTrizy");
                    }

                    mensagemErro = "ID trizy: " + idViagemTrizy + " - " + mensagemErro;

                    SalvarArquivoTransacaoAtualizar(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro + menssagemAdicionalHistorico, cargaIntegracao, cargaDadosTransporte, cargaCargaIntegracao);
                    if (preTrip)
                    {
                        SalvarArquivoTransacaoIntegrarCargaAPPTrizy(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro + menssagemAdicionalHistorico, cargaCargaIntegracao);
                        return true;
                    }
                }
                else
                {
                    Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    situacaoIntegracao = false;

                    SalvarArquivoTransacaoAtualizar(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro + menssagemAdicionalHistorico, cargaIntegracao, cargaDadosTransporte, cargaCargaIntegracao);

                    if (preTrip)
                        SalvarArquivoTransacaoIntegrarCargaAPPTrizy(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro + menssagemAdicionalHistorico, cargaCargaIntegracao);

                    return SalvarRetornoErroAtualizar(repCargaIntegracao, repCargaDadosTransporteIntegraca, repCargaCargaInteracao, mensagemErro + menssagemAdicionalHistorico, cargaIntegracao, cargaDadosTransporte, cargaCargaIntegracao);
                }
            }
            catch (ServicoException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NenhumRegistroEncontrado)
            {
                Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                situacaoIntegracao = true;

            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                situacaoIntegracao = false;

                return SalvarRetornoErroAtualizar(repCargaIntegracao, repCargaDadosTransporteIntegraca, repCargaCargaInteracao, mensagemErro + menssagemAdicionalHistorico, cargaIntegracao, cargaDadosTransporte, cargaCargaIntegracao);
            }
            catch (Exception excecao)
            {
                Log.TratarErro("URL: " + endPoint, "D");
                Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                SalvarArquivoTransacaoAtualizar(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro + menssagemAdicionalHistorico, cargaIntegracao, cargaDadosTransporte, cargaCargaIntegracao);

                if (preTrip)
                    SalvarArquivoTransacaoIntegrarCargaAPPTrizy(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro + menssagemAdicionalHistorico, cargaCargaIntegracao);

                return SalvarRetornoErroAtualizar(repCargaIntegracao, repCargaDadosTransporteIntegraca, repCargaCargaInteracao, mensagemErro + menssagemAdicionalHistorico, cargaIntegracao, cargaDadosTransporte, cargaCargaIntegracao);
            }
            if (!situacaoIntegracao && cargaIntegracao != null)
            {
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                repCargaIntegracao.Atualizar(cargaIntegracao);
            }
            else if (cargaIntegracao != null)
            {
                cargaIntegracao.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return true;
            }

            if (!situacaoIntegracao && cargaDadosTransporte != null)
            {
                cargaDadosTransporte.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporte.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                repCargaDadosTransporteIntegraca.Atualizar(cargaDadosTransporte);
            }
            else if (cargaDadosTransporte != null)
            {
                cargaDadosTransporte.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                cargaDadosTransporte.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                repCargaDadosTransporteIntegraca.Atualizar(cargaDadosTransporte);
                return true;
            }

            if (!situacaoIntegracao && cargaCargaIntegracao != null)
            {
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                repCargaCargaInteracao.Atualizar(cargaCargaIntegracao);
            }
            else if (cargaCargaIntegracao != null)
            {
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro + menssagemAdicionalHistorico;
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                if (cargaCargaIntegracao.FinalizarCargaAnterior)
                    cargaCargaIntegracao.FinalizarCargaAnterior = false;
                repCargaCargaInteracao.Atualizar(cargaCargaIntegracao);
                return true;
            }


            Log.TratarErro($"Retorno Integração Trizy: {mensagemErro + menssagemAdicionalHistorico}", "IntegracaoTrizy");

            client.Dispose();

            return false;
        }

        private static void SalvarArquivoTransacaoAtualizar(ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao, string jsonRequest, string jsonResponse, string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaIntegracao = null, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporte = null, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = null)
        {
            if (cargaIntegracao != null)
            {
                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
            }
            else if (cargaDadosTransporte != null)
            {
                servicoArquivoTransacao.Adicionar(cargaDadosTransporte, jsonRequest, jsonResponse, "json", mensagemErro);
            }
            else if (cargaCargaIntegracao != null)
                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
        }

        private static void SalvarArquivoTransacaoIntegrarCargaAPPTrizy(ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao, string jsonRequest, string jsonResponse, string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = null)
        {
            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
        }

        private static bool SalvarRetornoErroAtualizar(Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaIntegracao, Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegraca, Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao, string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaIntegracao = null, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporte = null, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = null)
        {
            if (cargaIntegracao != null)
            {
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaIntegracao);
            }
            else if (cargaDadosTransporte != null)
            {
                cargaDadosTransporte.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporte.ProblemaIntegracao = mensagemErro;
                repCargaDadosTransporteIntegraca.Atualizar(cargaDadosTransporte);
            }
            else if (cargaCargaIntegracao != null)
            {
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }

            return false;
        }
        public static void IntegrarTransbordo(Dominio.Entidades.Embarcador.Cargas.Carga carga, string status, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao integracao)
        {
            Repositorio.Embarcador.Cargas.TransbordoIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.TransbordoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (integracao != null)
            {
                integracao.NumeroTentativas += 1;
                integracao.DataIntegracao = DateTime.Now;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v1/external/travel/{carga.IDIdentificacaoTrizzy}";
            string mensagemErro = string.Empty;
            bool situacaoIntegracao = false;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), endPoint);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAtualizarViagem objetoEnvio = ConverterObjetoAtualizarViagem(status);

            try
            {
                if (string.IsNullOrWhiteSpace(carga.IDIdentificacaoTrizzy))
                    throw new ServicoException("Carga sem ID Trizy");

                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";
                    situacaoIntegracao = true;

                    if (integracao != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = integracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = "Integrado com sucesso";
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        integracao.ArquivosTransacao.Add(arquivoIntegracao);

                        //Devops 9864: Ao integrar transbordo, deve liberar cargas que estavam pendentes de finalização.
                        AtualizarViagemAntiga(new Dominio.ObjetosDeValor.WebService.Carga.Carga { Protocolo = carga.Codigo, NumeroCarga = carga.CodigoCargaEmbarcador }, unitOfWork);
                    }

                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = integracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    integracao.ArquivosTransacao.Add(arquivoIntegracao);

                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    integracao.ProblemaIntegracao = mensagemErro;
                    repCargaIntegracao.Atualizar(integracao);


                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                situacaoIntegracao = false;

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(integracao);
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                if (integracao != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = integracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    integracao.ArquivosTransacao.Add(arquivoIntegracao);
                }

                return;
            }
            if (!situacaoIntegracao && integracao != null)
            {
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(integracao);
            }
            else if (integracao != null)
            {
                integracao.ProblemaIntegracao = mensagemErro;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCargaIntegracao.Atualizar(integracao);
            }

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
        }

        public static void AtualzarEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, string stringConexao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, bool finalizarEntrega = false, Repositorio.UnitOfWork unitOfWorkCandidata = null)
        {
            Repositorio.UnitOfWork unitOfWork = unitOfWorkCandidata != null ? unitOfWorkCandidata : new Repositorio.UnitOfWork(stringConexao);

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v1/external/travel/{cargaEntrega.Carga.IDIdentificacaoTrizzy}/stopping-point/{cargaEntrega.IdTrizy}";
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), endPoint);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAtualizarEntrega objetoEnvio = ConverterObjetoAtualizarEntrega(cargaEntrega, finalizarEntrega);

            try
            {
                if (string.IsNullOrWhiteSpace(cargaEntrega.IdTrizy))
                    throw new ServicoException("Carga sem ID Trizy");

                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    AtualizarTracking(cargaEntrega.Carga, unitOfWork, configuracaoTMS);

                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";
                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                return;
            }

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
        }

        public static void AlternarBloqueioParada(bool bloquear, string idCarga, string idParada, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v1/external/travel/{idCarga}/stopping-point/{idParada}/{(bloquear ? "block" : "unblock")}";
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), endPoint);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAlternarBloqueioParada objetoEnvio = GerarObjetoEnvioAlternarBloqueioParada(bloquear);

            try
            {
                if (string.IsNullOrWhiteSpace(idCarga))
                    throw new ServicoException("Carga sem ID Trizy");
                if (string.IsNullOrWhiteSpace(idParada))
                    throw new ServicoException("Carga sem ID Trizy");
                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";
                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                return;
            }

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
        }

        public static void IntegrarModeloVeicular(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade repositorioModeloVeicularCargaDivisaoCapacidade = new Repositorio.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> divisoes = repositorioModeloVeicularCargaDivisaoCapacidade.BuscarPorModeloVeicularCarga(modeloVeicularCarga.Codigo);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v1/external/load-detail-layout";
            string method = "POST";

            if (!string.IsNullOrEmpty(modeloVeicularCarga.LayoutSuperAppId))
            {
                endPoint = $"{endPoint}/{modeloVeicularCarga.LayoutSuperAppId}";
                method = "PATCH";
            }

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), endPoint);
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioModeloVeicular objetoEnvio = GerarObjetoEnvioModeloVeicular(modeloVeicularCarga, divisoes);

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, settings);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoModeloVeicular retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoModeloVeicular>(result.Content.ReadAsStringAsync().Result);
                    modeloVeicularCarga.LayoutSuperAppId = retorno.loadDetailLayout?._id;
                    repositorioModeloVeicularCarga.Atualizar(modeloVeicularCarga);
                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                return;
            }

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
        }

        public static void IntegrarTipoOcorrencia(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCte = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/core/v1/external/occurrence-category";
            string method = "POST";

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), endPoint);

            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioTipoOcorrencia objetoEnvio = GerarObjetoTipoOcorrencia(tipoOcorrencia);

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, settings);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoTipoOcorrencia retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoTipoOcorrencia>(result.Content.ReadAsStringAsync().Result);
                    tipoOcorrencia.IdSuperApp = retorno.occurrenceCategory?._id;
                    repositorioTipoDeOcorrenciaDeCte.Atualizar(tipoOcorrencia);
                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                return;
            }

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
        }

        public static void IntegrarMotivoDevolucaoEntregaEntregaParcial(Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivoDevolucaoEntrega, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repositorioMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v1/external/partial-delivery-reason";
            string method = "POST";

            if (!string.IsNullOrEmpty(motivoDevolucaoEntrega.EntregaParcialSuperAppId))
            {
                endPoint = $"{endPoint}/{motivoDevolucaoEntrega.EntregaParcialSuperAppId}";
                method = "PATCH";
            }

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), endPoint);
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioIntegrarMotivoDevolucaoEntrega objetoEnvio = GerarObjetoEnvioIntegrarMotivoDevolucaoEntrega(motivoDevolucaoEntrega);

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, settings);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoMotivoDevolucaoEntrega retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoMotivoDevolucaoEntrega>(result.Content.ReadAsStringAsync().Result);
                    if (!String.IsNullOrEmpty(retorno.partialDeliveryReason?._id))
                    {
                        motivoDevolucaoEntrega.EntregaParcialSuperAppId = retorno.partialDeliveryReason?._id;
                        repositorioMotivoDevolucaoEntrega.Atualizar(motivoDevolucaoEntrega);
                    }
                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                return;
            }

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
        }

        public static void IntegrarMotivoDevolucaoEntregaNaoEntregue(Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivoDevolucaoEntrega, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repositorioMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v1/external/not-delivered-reason";
            string method = "POST";

            if (!string.IsNullOrEmpty(motivoDevolucaoEntrega.NaoEntregaSuperAppId))
            {
                endPoint = $"{endPoint}/{motivoDevolucaoEntrega.NaoEntregaSuperAppId}";
                method = "PATCH";
            }

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), endPoint);
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioIntegrarMotivoDevolucaoEntrega objetoEnvio = GerarObjetoEnvioIntegrarMotivoDevolucaoEntrega(motivoDevolucaoEntrega);

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, settings);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoMotivoDevolucaoEntrega retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoMotivoDevolucaoEntrega>(result.Content.ReadAsStringAsync().Result);
                    if (!String.IsNullOrEmpty(retorno.notDeliveredReason?._id))
                    {
                        motivoDevolucaoEntrega.NaoEntregaSuperAppId = retorno.notDeliveredReason?._id;
                        repositorioMotivoDevolucaoEntrega.Atualizar(motivoDevolucaoEntrega);
                    }
                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                return;
            }

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
        }

        public static (string? mensagemErro, Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoChecklist? checklist) IntegrarChecklist(Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp checklist, List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> etapas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/core/v1/external/checklist-flow";
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), endPoint);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Checklist objetoEnvio = ConverterObjetoChecklist(checklist, etapas);

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, settings);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoChecklist retornoSucesso = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoChecklist>(result.Content.ReadAsStringAsync().Result);

                    return (null, retornoSucesso);

                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoChecklist retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoChecklist>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                return (mensagemErro, null);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                return (mensagemErro, null);
            }

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
            return (mensagemErro, null);
        }


        public static void EnviarNotificacao(string cnpjMotorista, string titulo, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy.Replace("api-trizy-app", "legacy/gateway")}";
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioEnviarNotificacao objetoEnvio = ConverterObjetoEnviarNotificacao(cnpjMotorista, titulo, mensagem);

            try
            {
                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";
                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";

                return;
            }

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
        }

        public string EnviarNotificacaoSuperApp(Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp notificacao, string cpfMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagemErro = string.Empty;

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repositorioConfiguracaoIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repositorioConfiguracaoIntegracaoTrizy.BuscarPrimeiroRegistro();
            string titulo = ObterDadosTags(notificacao, notificacao.NotificacaoApp.Titulo);
            string mensagem = ObterDadosTags(notificacao, notificacao.NotificacaoApp.Mensagem);
            string tipoDocumentoMotorista = configuracaoIntegracaoTrizy.TipoDocumentoPais.ObterPersonDocumentType();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/notification-manager/v1/external/notification/driver";

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnviarNotificacaoSuperApp requestBody = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnviarNotificacaoSuperApp();
            requestBody.driver.document.type = tipoDocumentoMotorista;
            requestBody.driver.document.value = cpfMotorista;
            requestBody.title = titulo;
            requestBody.message = mensagem;

            try
            {
                string jsonRequest = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                string jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                if (!result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message + " " + (retorno.error ?? string.Empty);

                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }

                notificacao.ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);
                notificacao.ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                mensagemErro = excecao.Message;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");
                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Trizy.";
            }
            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
            return mensagemErro;
        }

        public bool ValidarEtapaIntegracao(EtapaFluxoGestaoPatio etapaFluxoPatio)
        {
            switch (etapaFluxoPatio)
            {
                case EtapaFluxoGestaoPatio.CheckList:
                case EtapaFluxoGestaoPatio.DocumentosTransporte:
                    return true;
                default:
                    return false;
            }
        }

        public void Integrar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao fluxoPatioIntegrao)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioFluxoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            try
            {
                DateTime dataAvancoEtapa = fluxoPatioIntegrao.DataIntegracao;
                fluxoPatioIntegrao.NumeroTentativas++;
                fluxoPatioIntegrao.DataIntegracao = DateTime.Now;

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao = ObterConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoTrizy = ObterConfiguracaoPadraoTrizy();

                if (string.IsNullOrEmpty(configuracaoTrizy.URLEnvioEventosPatio))
                    throw new ServicoException("Url do Evento patio na integração Não Configurada");

                if (string.IsNullOrEmpty(configuracaoTrizy.TokenEnvioMS))
                    throw new ServicoException("Não existe token configurado na integração Trizy");

                string evento = fluxoPatioIntegrao.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.DocumentosTransporte ? "DOCUMENTACAO" :
                        fluxoPatioIntegrao.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.CheckList ? "CHECKLIST" : "";

                int.TryParse(fluxoPatioIntegrao.Carga.NumeroAgendamento, out int numeroAgendamento);
                jsonRequest = JsonConvert.SerializeObject(new
                {
                    agendamento_id = numeroAgendamento,
                    data = dataAvancoEtapa.ToString("yyyy-MM-dd HH:mm:ss"),
                    evento = new
                    {
                        id = evento
                    }
                });

                HttpClient cliente = ObterCliente(configuracao, configuracaoTrizy.TokenEnvioMS);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage result = cliente.PostAsync(configuracaoTrizy.URLEnvioEventosPatio, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (!result.IsSuccessStatusCode)
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    string message = retorno?.message?.error ?? string.Empty;
                    fluxoPatioIntegrao.ProblemaIntegracao = message;
                    fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    fluxoPatioIntegrao.ProblemaIntegracao = "Integração realizada com Sucesso";

                }
                servicoArquivoTransacao.Adicionar(fluxoPatioIntegrao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                fluxoPatioIntegrao.ProblemaIntegracao = ex.Message;
                fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                fluxoPatioIntegrao.ProblemaIntegracao = "Problema ao tentar integrar com ao Trizy";
                fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                servicoArquivoTransacao.Adicionar(fluxoPatioIntegrao, jsonRequest, jsonResponse, "json");
            }

            repositorioFluxoPatio.Atualizar(fluxoPatioIntegrao);

        }

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao = ObterConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoTrizy = ObterConfiguracaoPadraoTrizy();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
            cargaDadosTransporteIntegracao.NumeroTentativas++;

            try
            {
                if (!(configuracao?.PossuiIntegracaoTrizy ?? false))
                    throw new ServicoException("Não possui configuração para Trizy.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Contrato objetoContrato = ObterContrato(cargaDadosTransporteIntegracao);

                string url = $"{configuracaoTrizy.URLEnvioCarga}";
                HttpClient requisicao = ObterCliente(configuracao, configuracaoTrizy.TokenEnvioMS);

                jsonRequisicao = JsonConvert.SerializeObject(objetoContrato, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato retornoContrato = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato>(jsonRetorno);

                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = retornoContrato?.Mensagem;
                    cargaDadosTransporteIntegracao.Protocolo = retornoContrato?.Codigo.ToString();
                    cargaDadosTransporteIntegracao.CargaPedido.ProtocoloIntegracao = retornoContrato?.Codigo.ToString();

                    repositorioCargaPedido.Atualizar(cargaDadosTransporteIntegracao.CargaPedido);
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato retornoContrato = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato>(jsonRetorno);
                    throw new ServicoException($"{retornoContrato.Codigo} - {retornoContrato.Mensagem}");
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Trizy: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da Trizy";
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao = ObterConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoTrizy = ObterConfiguracaoPadraoTrizy();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            cancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cancelamentoCargaIntegracao.NumeroTentativas++;

            try
            {
                if (!(configuracao?.PossuiIntegracaoTrizy ?? false))
                    throw new ServicoException("Não possui configuração para Trizy.");


                string url = $"{configuracaoTrizy.URLEnvioCancelamentoCarga}/{cancelamentoCargaIntegracao.CargaPedido.ProtocoloIntegracao}";
                HttpClient requisicao = ObterCliente(configuracao, configuracaoTrizy.TokenEnvioMS);

                HttpResponseMessage retornoRequisicao = requisicao.DeleteAsync(url).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato retornoContrato = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato>(jsonRetorno);

                    cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cancelamentoCargaIntegracao.ProblemaIntegracao = retornoContrato?.Mensagem;
                    cancelamentoCargaIntegracao.Protocolo = retornoContrato?.Codigo.ToString();
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato retornoContrato = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato>(jsonRetorno);
                    throw new ServicoException($"{retornoContrato.Codigo} - {retornoContrato.Mensagem}");
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Trizy: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException ex)
            {
                cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoCargaIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração de Cancelamento da Trizy";
            }

            servicoArquivoTransacao.Adicionar(cancelamentoCargaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaCancelamentoCargaIntegracao.Atualizar(cancelamentoCargaIntegracao);
        }

        public static HttpClient CriarHttpClient(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, string endPoint)
        {
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);
            return client;
        }

        public async Task IntegrarCriacaoGrupoMotoristaAsync(Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao grupoMotoristasIntegracao, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repositorioGrupoMotoristasIntegracao = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas repositorioGrupoMotoristas = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas(_unitOfWork, cancellationToken);


            ArquivoTransacao<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracaoArquivos> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracaoArquivos>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao = ObterConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoTrizy = ObterConfiguracaoPadraoTrizy();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            grupoMotoristasIntegracao.DataIntegracao = DateTime.Now;
            grupoMotoristasIntegracao.NumeroTentativas++;

            try
            {
                if (!(configuracao?.PossuiIntegracaoTrizy ?? false))
                    throw new ServicoException("Não possui configuração para Trizy.");


                Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.GrupoMotoristaCreateRequest objetoRequest = await ObterGrupoMotoristaCreateRequest(grupoMotoristasIntegracao, cancellationToken);

                string url = configuracaoTrizy.URLIntegracaoGrupoMotoristas;
                HttpClient requisicao = ObterCliente(configuracao, configuracao.TokenTrizy);

                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                jsonRequisicao = JsonConvert.SerializeObject(objetoRequest, Formatting.Indented, jsonSerializerSettings);


                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = await requisicao.PostAsync(url, conteudoRequisicao, cancellationToken);
                jsonRetorno = await retornoRequisicao.Content.ReadAsStringAsync();

                if (retornoRequisicao.StatusCode == HttpStatusCode.Created)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.GrupoMotoristaCreateResponse retornoContrato = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.GrupoMotoristaCreateResponse>(jsonRetorno);

                    grupoMotoristasIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    grupoMotoristasIntegracao.ProblemaIntegracao = retornoContrato?.message;
                    grupoMotoristasIntegracao.GrupoMotoristas.CodigoIntegracao = retornoContrato?.segmentation._id.ToString();

                    grupoMotoristasIntegracao.GrupoMotoristas.Situacao = SituacaoIntegracaoGrupoMotoristas.Finalizado;

                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato retornoContrato = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato>(jsonRetorno);
                    throw new ServicoException($"{retornoContrato.Codigo} - {retornoContrato.Mensagem}");
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Trizy: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException ex)
            {
                grupoMotoristasIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                grupoMotoristasIntegracao.GrupoMotoristas.Situacao = SituacaoIntegracaoGrupoMotoristas.FalhaNasIntegracoes;
                grupoMotoristasIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                grupoMotoristasIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                grupoMotoristasIntegracao.GrupoMotoristas.Situacao = SituacaoIntegracaoGrupoMotoristas.FalhaNasIntegracoes;
                grupoMotoristasIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da Trizy";
            }

            servicoArquivoTransacao.Adicionar(grupoMotoristasIntegracao, jsonRequisicao, jsonRetorno, "json");

            await repositorioGrupoMotoristas.AtualizarAsync(grupoMotoristasIntegracao.GrupoMotoristas);

            await repositorioGrupoMotoristasIntegracao.AtualizarAsync(grupoMotoristasIntegracao);
        }


        public async Task IntegrarAtualizacaoGrupoMotoristaAsync(Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao grupoMotoristasIntegracao, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repositorioGrupoMotoristasIntegracao = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao(_unitOfWork, cancellationToken);

            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionarioAlteracao repositorioGrupoMotoristasFuncionarioAlteracao = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionarioAlteracao(_unitOfWork, cancellationToken);

            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas repositorioGrupoMotoristas = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas(_unitOfWork, cancellationToken);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracaoArquivos> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracaoArquivos>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao = ObterConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoTrizy = ObterConfiguracaoPadraoTrizy();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            grupoMotoristasIntegracao.DataIntegracao = DateTime.Now;
            grupoMotoristasIntegracao.NumeroTentativas++;

            try
            {
                if (!(configuracao?.PossuiIntegracaoTrizy ?? false))
                    throw new ServicoException("Não possui configuração para Trizy.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.GrupoMotoristaUpdateRequest objetoRequest = await ObterGrupoMotoristaUpdateRequest(grupoMotoristasIntegracao, cancellationToken);

                string url = $"{configuracaoTrizy.URLIntegracaoGrupoMotoristas}/{grupoMotoristasIntegracao.GrupoMotoristas.CodigoIntegracao}";
                HttpClient requisicao = ObterCliente(configuracao, configuracao.TokenTrizy);

                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                jsonRequisicao = JsonConvert.SerializeObject(objetoRequest, Formatting.Indented, jsonSerializerSettings);

                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
                request.Content = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = await requisicao.SendAsync(request);
                jsonRetorno = await retornoRequisicao.Content.ReadAsStringAsync();

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.GrupoMotoristaCreateResponse retornoContrato = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.GrupoMotoristaCreateResponse>(jsonRetorno);

                    grupoMotoristasIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    grupoMotoristasIntegracao.ProblemaIntegracao = retornoContrato?.message;
                    grupoMotoristasIntegracao.GrupoMotoristas.Situacao = SituacaoIntegracaoGrupoMotoristas.Finalizado;

                    await repositorioGrupoMotoristasFuncionarioAlteracao.LimparAlteracoesPorCodigoGrupoMotoristaAsync(grupoMotoristasIntegracao.GrupoMotoristas.Codigo, cancellationToken);

                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato retornoContrato = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoContrato>(jsonRetorno);
                    throw new ServicoException($"{retornoContrato.Codigo} - {retornoContrato.Mensagem}");
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Trizy: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException ex)
            {
                grupoMotoristasIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                grupoMotoristasIntegracao.GrupoMotoristas.Situacao = SituacaoIntegracaoGrupoMotoristas.FalhaNasIntegracoes;
                grupoMotoristasIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                grupoMotoristasIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                grupoMotoristasIntegracao.GrupoMotoristas.Situacao = SituacaoIntegracaoGrupoMotoristas.FalhaNasIntegracoes;
                grupoMotoristasIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da Trizy";
            }

            servicoArquivoTransacao.Adicionar(grupoMotoristasIntegracao, jsonRequisicao, jsonRetorno, "json");

            await repositorioGrupoMotoristas.AtualizarAsync(grupoMotoristasIntegracao.GrupoMotoristas);

            await repositorioGrupoMotoristasIntegracao.AtualizarAsync(grupoMotoristasIntegracao);
        }


        public async Task IntegracaoNaoImplementadaAsync(Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao grupoMotoristasIntegracao, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repositorioGrupoMotoristasIntegracao = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao(_unitOfWork, cancellationToken);

            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas repositorioGrupoMotoristas = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas(_unitOfWork, cancellationToken);

            grupoMotoristasIntegracao.DataIntegracao = DateTime.Now;
            grupoMotoristasIntegracao.NumeroTentativas++;
            grupoMotoristasIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            grupoMotoristasIntegracao.GrupoMotoristas.Situacao = SituacaoIntegracaoGrupoMotoristas.Finalizado;
            grupoMotoristasIntegracao.ProblemaIntegracao = "Integração Não Implementada";

            await repositorioGrupoMotoristas.AtualizarAsync(grupoMotoristasIntegracao.GrupoMotoristas);

            await repositorioGrupoMotoristasIntegracao.AtualizarAsync(grupoMotoristasIntegracao);
        }
        #endregion

        #region Métodos Privados - Estrutura Padrão

        private static void IntegrarApiEnvioComprovanteCTe(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);
            Servicos.WebService.CTe.CTe servicoCTe = new Servicos.WebService.CTe.CTe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComprovanteTrizy tipoComprovanteTrizy = TipoComprovanteTrizy.CTe;

            string endPoint = configuracaoIntegracao.URLTrizy;
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            int idPropostaTrizy = repCarregamentoPedido.BuscarIDPropostaTrizyPorCarregamento(cargaCargaIntegracao.Carga.Carregamento?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio();

            parameters.num_romaneio = cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDLoteTrizy;
            parameters.identificador = !string.IsNullOrWhiteSpace(cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador) ? cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador : Utilidades.String.OnlyNumbers(cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.Protocolo.ToString("n0"));
            parameters.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
            parameters.mensagem = string.Empty;
            parameters.detalhes = string.Empty;
            parameters.cancelar_acordo = "0";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio romaneio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio();
            romaneio.module = "M3002API";
            romaneio.operation = "apiInsRomaneio";
            romaneio.parameters = parameters;

            try
            {
                jsonRequest = JsonConvert.SerializeObject(romaneio, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersComprovante parametersComprovane = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersComprovante();

                    parametersComprovane.descricao = tipoComprovanteTrizy.ObterDescricao();
                    parametersComprovane.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
                    parametersComprovane.nome_arquivo = tipoComprovanteTrizy.ObterNomeArquivo();
                    parametersComprovane.tipo_documento = tipoComprovanteTrizy.ObterNumero();
                    parametersComprovane.arquivo = servicoCTe.ObterRetornoPDF(cte, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, unitOfWork);
                    parametersComprovane.num_cte = cte.Numero;
                    parametersComprovane.id_externo = cte.Codigo;
                    parametersComprovane.peso_carregado = cargaCargaIntegracao.Carga != null && cargaCargaIntegracao.Carga.DadosSumarizados != null && cargaCargaIntegracao.Carga.DadosSumarizados.PesoTotal > 0 ? cargaCargaIntegracao.Carga.DadosSumarizados.PesoTotal : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.PesoTotal;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioComprovante envioComprovante = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioComprovante();
                    envioComprovante.module = "M3002API";
                    envioComprovante.operation = "apiEnvioComprovante";
                    envioComprovante.parameters = parametersComprovane;

                    try
                    {
                        jsonRequest = JsonConvert.SerializeObject(envioComprovante, Formatting.Indented);
                        content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                        result = client.PostAsync(endPoint, content).Result;
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        if (result.IsSuccessStatusCode)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                            servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", "Envio CT-e " + cte.Numero);
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                            if (retorno == null)
                                mensagemErro = result.StatusCode.ToString();
                            else
                                mensagemErro = retorno.message;
                            if (string.IsNullOrWhiteSpace(mensagemErro))
                                mensagemErro = "Falha na integração com a Trizy.";
                            else
                                mensagemErro = "Retorno Trizy: " + mensagemErro;

                            servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", "Falha envio CT-e " + cte.Numero + " " + mensagemErro);
                        }
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                        mensagemErro = ("Falha envio CT-e " + cte.Numero + " ") + (excecao.Message.Length > 300 ? excecao.Message.Substring(0, 300) : excecao.Message);
                        servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
                        return;
                    }
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", ("Falha envio CT-e " + cte.Numero + " ") + mensagemErro);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = ("Falha envio CT-e " + cte.Numero + " ") + (excecao.Message.Length > 300 ? excecao.Message.Substring(0, 300) : excecao.Message);
                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
            }

            repCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }

        private static void IntegrarCargaDadosTrasporteApiEnvioComprovanteCTe(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);
            Servicos.WebService.CTe.CTe servicoCTe = new Servicos.WebService.CTe.CTe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComprovanteTrizy tipoComprovanteTrizy = TipoComprovanteTrizy.CTe;

            string endPoint = configuracaoIntegracao.URLTrizy;
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            int idPropostaTrizy = repCarregamentoPedido.BuscarIDPropostaTrizyPorCarregamento(cargaCargaIntegracao.Carga.Carregamento?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio();

            parameters.num_romaneio = cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDLoteTrizy;
            parameters.identificador = !string.IsNullOrWhiteSpace(cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador) ? cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador : Utilidades.String.OnlyNumbers(cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.Protocolo.ToString("n0"));
            parameters.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
            parameters.mensagem = string.Empty;
            parameters.detalhes = string.Empty;
            parameters.cancelar_acordo = "0";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio romaneio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio();
            romaneio.module = "M3002API";
            romaneio.operation = "apiInsRomaneio";
            romaneio.parameters = parameters;

            try
            {
                jsonRequest = JsonConvert.SerializeObject(romaneio, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersComprovante parametersComprovane = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersComprovante();

                    parametersComprovane.descricao = tipoComprovanteTrizy.ObterDescricao();
                    parametersComprovane.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
                    parametersComprovane.nome_arquivo = tipoComprovanteTrizy.ObterNomeArquivo();
                    parametersComprovane.tipo_documento = tipoComprovanteTrizy.ObterNumero();
                    parametersComprovane.arquivo = servicoCTe.ObterRetornoPDF(cte, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, unitOfWork);
                    parametersComprovane.num_cte = cte.Numero;
                    parametersComprovane.id_externo = cte.Codigo;
                    parametersComprovane.peso_carregado = cargaCargaIntegracao.Carga != null && cargaCargaIntegracao.Carga.DadosSumarizados != null && cargaCargaIntegracao.Carga.DadosSumarizados.PesoTotal > 0 ? cargaCargaIntegracao.Carga.DadosSumarizados.PesoTotal : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.PesoTotal;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioComprovante envioComprovante = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioComprovante();
                    envioComprovante.module = "M3002API";
                    envioComprovante.operation = "apiEnvioComprovante";
                    envioComprovante.parameters = parametersComprovane;

                    try
                    {
                        jsonRequest = JsonConvert.SerializeObject(envioComprovante, Formatting.Indented);
                        content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                        result = client.PostAsync(endPoint, content).Result;
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        if (result.IsSuccessStatusCode)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                            servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", "Envio CT-e " + cte.Numero);
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                            if (retorno == null)
                                mensagemErro = result.StatusCode.ToString();
                            else
                                mensagemErro = retorno.message;
                            if (string.IsNullOrWhiteSpace(mensagemErro))
                                mensagemErro = "Falha na integração com a Trizy.";
                            else
                                mensagemErro = "Retorno Trizy: " + mensagemErro;

                            servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", "Falha envio CT-e " + cte.Numero + " " + mensagemErro);
                        }
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                        mensagemErro = ("Falha envio CT-e " + cte.Numero + " ") + (excecao.Message.Length > 300 ? excecao.Message.Substring(0, 300) : excecao.Message);
                        servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
                        return;
                    }
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", ("Falha envio CT-e " + cte.Numero + " ") + mensagemErro);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = ("Falha envio CT-e " + cte.Numero + " ") + (excecao.Message.Length > 300 ? excecao.Message.Substring(0, 300) : excecao.Message);
                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
            }

            repCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }

        private static void IntegrarApiEnvioComprovanteMDFe(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComprovanteTrizy tipoComprovanteTrizy = TipoComprovanteTrizy.MDFe;

            string endPoint = configuracaoIntegracao.URLTrizy;
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            int idPropostaTrizy = repCarregamentoPedido.BuscarIDPropostaTrizyPorCarregamento(cargaCargaIntegracao.Carga.Carregamento?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio();

            parameters.num_romaneio = cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDLoteTrizy;
            parameters.identificador = !string.IsNullOrWhiteSpace(cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador) ? cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador : Utilidades.String.OnlyNumbers(cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.Protocolo.ToString("n0"));
            parameters.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
            parameters.mensagem = string.Empty;
            parameters.detalhes = string.Empty;
            parameters.cancelar_acordo = "0";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio romaneio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio();
            romaneio.module = "M3002API";
            romaneio.operation = "apiInsRomaneio";
            romaneio.parameters = parameters;

            try
            {
                jsonRequest = JsonConvert.SerializeObject(romaneio, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersComprovante parametersComprovane = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersComprovante();

                    parametersComprovane.descricao = tipoComprovanteTrizy.ObterDescricao();
                    parametersComprovane.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
                    parametersComprovane.nome_arquivo = tipoComprovanteTrizy.ObterNomeArquivo();
                    parametersComprovane.tipo_documento = tipoComprovanteTrizy.ObterNumero();
                    parametersComprovane.arquivo = servicoMDFe.ObterDAMDFE(mdfe.Codigo, mdfe.Empresa.Codigo, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF);
                    parametersComprovane.num_mdfe = mdfe.Numero;
                    parametersComprovane.id_externo = mdfe.Codigo;
                    parametersComprovane.peso_carregado = cargaCargaIntegracao.Carga != null && cargaCargaIntegracao.Carga.DadosSumarizados != null && cargaCargaIntegracao.Carga.DadosSumarizados.PesoTotal > 0 ? cargaCargaIntegracao.Carga.DadosSumarizados.PesoTotal : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.PesoTotal;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioComprovante envioComprovante = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioComprovante();
                    envioComprovante.module = "M3002API";
                    envioComprovante.operation = "apiEnvioComprovante";
                    envioComprovante.parameters = parametersComprovane;

                    try
                    {
                        jsonRequest = JsonConvert.SerializeObject(envioComprovante, Formatting.Indented);
                        content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                        result = client.PostAsync(endPoint, content).Result;
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        if (result.IsSuccessStatusCode)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);

                            mensagemErro = "Envio MDF-e " + mdfe.Numero + " ";
                            servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                            if (retorno == null)
                                mensagemErro = result.StatusCode.ToString();
                            else
                                mensagemErro = retorno.message;
                            if (string.IsNullOrWhiteSpace(mensagemErro))
                                mensagemErro = "Falha na integração com a Trizy.";
                            else
                                mensagemErro = "Retorno Trizy: " + mensagemErro;

                            servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", "Falha envio MDF-e " + mdfe.Numero + " " + mensagemErro);
                        }
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                        mensagemErro = ("Falha envio MDF-e " + mdfe.Numero + " ") + (excecao.Message.Length > 300 ? excecao.Message.Substring(0, 300) : excecao.Message);
                        servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
                        return;
                    }
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", ("Falha envio MDF-e " + mdfe.Numero + " ") + mensagemErro);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = ("Falha envio MDF-e " + mdfe.Numero + " ") + (excecao.Message.Length > 300 ? excecao.Message.Substring(0, 300) : excecao.Message);
                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
            }

            repCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }

        private static void IntegrarCargaDadosTrasporteApiEnvioComprovanteMDFe(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComprovanteTrizy tipoComprovanteTrizy = TipoComprovanteTrizy.MDFe;

            string endPoint = configuracaoIntegracao.URLTrizy;
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            int idPropostaTrizy = repCarregamentoPedido.BuscarIDPropostaTrizyPorCarregamento(cargaCargaIntegracao.Carga.Carregamento?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersRomaneio();

            parameters.num_romaneio = cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDLoteTrizy;
            parameters.identificador = !string.IsNullOrWhiteSpace(cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador) ? cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.NumeroPedidoEmbarcador : Utilidades.String.OnlyNumbers(cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.Protocolo.ToString("n0"));
            parameters.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
            parameters.mensagem = string.Empty;
            parameters.detalhes = string.Empty;
            parameters.cancelar_acordo = "0";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio romaneio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Romaneio();
            romaneio.module = "M3002API";
            romaneio.operation = "apiInsRomaneio";
            romaneio.parameters = parameters;

            try
            {
                jsonRequest = JsonConvert.SerializeObject(romaneio, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersComprovante parametersComprovane = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.parametersComprovante();

                    parametersComprovane.descricao = tipoComprovanteTrizy.ObterDescricao();
                    parametersComprovane.proposta_id = idPropostaTrizy > 0 ? idPropostaTrizy : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.IDPropostaTrizy;
                    parametersComprovane.nome_arquivo = tipoComprovanteTrizy.ObterNomeArquivo();
                    parametersComprovane.tipo_documento = tipoComprovanteTrizy.ObterNumero();
                    parametersComprovane.arquivo = servicoMDFe.ObterDAMDFE(mdfe.Codigo, mdfe.Empresa.Codigo, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF);
                    parametersComprovane.num_mdfe = mdfe.Numero;
                    parametersComprovane.id_externo = mdfe.Codigo;
                    parametersComprovane.peso_carregado = cargaCargaIntegracao.Carga != null && cargaCargaIntegracao.Carga.DadosSumarizados != null && cargaCargaIntegracao.Carga.DadosSumarizados.PesoTotal > 0 ? cargaCargaIntegracao.Carga.DadosSumarizados.PesoTotal : cargaCargaIntegracao.Carga.Pedidos.FirstOrDefault().Pedido.PesoTotal;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioComprovante envioComprovante = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioComprovante();
                    envioComprovante.module = "M3002API";
                    envioComprovante.operation = "apiEnvioComprovante";
                    envioComprovante.parameters = parametersComprovane;

                    try
                    {
                        jsonRequest = JsonConvert.SerializeObject(envioComprovante, Formatting.Indented);
                        content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                        result = client.PostAsync(endPoint, content).Result;
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        if (result.IsSuccessStatusCode)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);

                            mensagemErro = "Envio MDF-e " + mdfe.Numero + " ";
                            servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                            if (retorno == null)
                                mensagemErro = result.StatusCode.ToString();
                            else
                                mensagemErro = retorno.message;
                            if (string.IsNullOrWhiteSpace(mensagemErro))
                                mensagemErro = "Falha na integração com a Trizy.";
                            else
                                mensagemErro = "Retorno Trizy: " + mensagemErro;

                            servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", "Falha envio MDF-e " + mdfe.Numero + " " + mensagemErro);
                        }
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                        mensagemErro = ("Falha envio MDF-e " + mdfe.Numero + " ") + (excecao.Message.Length > 300 ? excecao.Message.Substring(0, 300) : excecao.Message);
                        servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
                        return;
                    }
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", ("Falha envio MDF-e " + mdfe.Numero + " ") + mensagemErro);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = ("Falha envio MDF-e " + mdfe.Numero + " ") + (excecao.Message.Length > 300 ? excecao.Message.Substring(0, 300) : excecao.Message);
                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);
            }

            repCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }



        #endregion

        #region Métodos Privados

        public static async Task AtualizarVeiculoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (carga == null)
                return;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v1/external/travel/{carga.IDIdentificacaoTrizzy}";
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), endPoint);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                dynamic objetoEnvio = ObterVehicleCargaAPP(carga);

                if (string.IsNullOrWhiteSpace(carga.IDIdentificacaoTrizzy))
                    throw new ServicoException("Carga sem ID Trizy");

                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.SendAsync(request);
                jsonResponse = await result.Content.ReadAsStringAsync();

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";
                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                return;
            }

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);
            SalvarArquivoTransacaoIntegrarCargaAPPTrizy(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro, cargaCargaIntegracao);

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
        }
        public static void IntegrarMDFeManual(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao cargaMDFeManualIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repCargaMDFeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            cargaMDFeManualIntegracao.NumeroTentativas++;
            cargaMDFeManualIntegracao.DataIntegracao = DateTime.Now;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCargaMDFe.BuscarCargaPorMDFe(cargaMDFeManualIntegracao.MDFe.Codigo);
            if (carga == null)
                throw new ServicoException("Carga não vinculada");

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v2/external/travel/{carga.IDIdentificacaoTrizzy}";
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), endPoint);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                dynamic objetoEnvio = ObterVehicleDriversCargaAPP(carga.Veiculo, carga.Motoristas.ToList(), configuracaoIntegracaoTrizy);

                if (string.IsNullOrWhiteSpace(carga.IDIdentificacaoTrizzy))
                    throw new ServicoException("Carga sem ID Trizy");

                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.GravarDebug("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    if (EnviarDocumentoMDFeManual(cargaMDFeManualIntegracao, carga, unitOfWork, ref mensagemErro))
                    {
                        cargaMDFeManualIntegracao.ProblemaIntegracao = mensagemErro;
                        cargaMDFeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    }
                    else
                    {
                        throw new ServicoException(mensagemErro);
                    }

                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    cargaMDFeManualIntegracao.ProblemaIntegracao = mensagemErro;
                    cargaMDFeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaMDFeManualIntegracao.ProblemaIntegracao = message;
                cargaMDFeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            }
            catch (Exception excecao)
            {
                Servicos.Log.GravarDebug("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");


                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaMDFeManualIntegracao.ProblemaIntegracao = message;
                cargaMDFeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            }
            servicoArquivoTransacao.Adicionar(cargaMDFeManualIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);

            repCargaMDFeManualIntegracao.Atualizar(cargaMDFeManualIntegracao);
        }

        public static void IntegrarMDFeManualCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao cargaMDFeManualCancelamentoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao repCargaMDFeManualCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            cargaMDFeManualCancelamentoIntegracao.NumeroTentativas++;
            cargaMDFeManualCancelamentoIntegracao.DataIntegracao = DateTime.Now;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCargaMDFe.BuscarCargaPorMDFe(cargaMDFeManualCancelamentoIntegracao.MDFe.Codigo);
            if (carga == null)
                throw new ServicoException("Carga não vinculada");

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v2/external/travel/{carga.IDIdentificacaoTrizzy}";
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), endPoint);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                dynamic objetoEnvio = ObterVehicleDriversCargaAPP(carga.Veiculo, carga.Motoristas.ToList(), configuracaoIntegracaoTrizy);

                if (string.IsNullOrWhiteSpace(carga.IDIdentificacaoTrizzy))
                    throw new ServicoException("Carga sem ID Trizy");

                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.GravarDebug("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    if (EnviarDocumentoMDFeManualCancelamento(cargaMDFeManualCancelamentoIntegracao, carga, unitOfWork, ref mensagemErro))
                    {
                        cargaMDFeManualCancelamentoIntegracao.ProblemaIntegracao = mensagemErro;
                        cargaMDFeManualCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    }
                    else
                    {
                        throw new ServicoException(mensagemErro);
                    }

                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    cargaMDFeManualCancelamentoIntegracao.ProblemaIntegracao = mensagemErro;
                    cargaMDFeManualCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaMDFeManualCancelamentoIntegracao.ProblemaIntegracao = message;
                cargaMDFeManualCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            }
            catch (Exception excecao)
            {
                Servicos.Log.GravarDebug("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");


                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaMDFeManualCancelamentoIntegracao.ProblemaIntegracao = message;
                cargaMDFeManualCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            }
            servicoArquivoTransacao.Adicionar(cargaMDFeManualCancelamentoIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);

            repCargaMDFeManualCancelamentoIntegracao.Atualizar(cargaMDFeManualCancelamentoIntegracao);
        }
        public static bool EnviarDocumentoMDFeManual(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao cargaMDFeManualIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, ref string mensagemErro)
        {
            bool sucesso = false;
            Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repCargaMDFeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/core/v1/external/document";

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                dynamic objetoEnvio = ObterDocumentsMDFeManual(carga, cargaMDFeManualIntegracao.MDFe, unitOfWork);

                if (string.IsNullOrWhiteSpace(carga.IDIdentificacaoTrizzy))
                    throw new ServicoException("Carga sem ID Trizy");

                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.GravarDebug("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    sucesso = true;
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ReturnDocumentV1 retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ReturnDocumentV1>(jsonResponse);

                    cargaMDFeManualIntegracao.CodigoExternoRetornoIntegracao = retorno?.Document?._Id ?? "";
                    mensagemErro = $"Documento {cargaMDFeManualIntegracao.CodigoExternoRetornoIntegracao} integrado com sucesso. Viagem ID Trizy: {carga.IDIdentificacaoTrizzy}";

                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    sucesso = false;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                mensagemErro = excecao.Message;
                sucesso = false;

            }
            catch (Exception excecao)
            {
                Servicos.Log.GravarDebug("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");


                mensagemErro = excecao.Message;
                sucesso = false;

            }
            servicoArquivoTransacao.Adicionar(cargaMDFeManualIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);

            repCargaMDFeManualIntegracao.Atualizar(cargaMDFeManualIntegracao);

            return sucesso;
        }
        public static bool EnviarDocumentoMDFeManualCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao cargaMDFeManualCancelamentoIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, ref string mensagemErro)
        {
            bool sucesso = false;
            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao repCargaMDFeManualCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repCargaMDFeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unitOfWork);

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao cargaMDFeManualIntegracao = repCargaMDFeManualIntegracao.BuscarPorCargaMDFeManualETipo(cargaMDFeManualCancelamentoIntegracao.CargaMDFeManualCancelamento.CargaMDFeManual.Codigo, TipoIntegracao.Trizy)?.FirstOrDefault();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (string.IsNullOrWhiteSpace(cargaMDFeManualIntegracao?.CodigoExternoRetornoIntegracao ?? ""))
                throw new ServicoException("Documento sem ID Trizy");

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/core/v1/external/document/{cargaMDFeManualIntegracao?.CodigoExternoRetornoIntegracao}";

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("DELETE"), endPoint);

                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.GravarDebug("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    sucesso = true;
                    mensagemErro = $"Documento {cargaMDFeManualIntegracao?.CodigoExternoRetornoIntegracao} integrado com sucesso. Viagem ID Trizy: {carga.IDIdentificacaoTrizzy}";

                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;

                    sucesso = false;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");
                mensagemErro = excecao.Message;
                sucesso = false;

            }
            catch (Exception excecao)
            {
                Servicos.Log.GravarDebug("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.GravarDebug("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");


                mensagemErro = excecao.Message;
                sucesso = false;

            }
            servicoArquivoTransacao.Adicionar(cargaMDFeManualCancelamentoIntegracao, jsonRequest, jsonResponse, "json", mensagemErro);

            repCargaMDFeManualCancelamentoIntegracao.Atualizar(cargaMDFeManualCancelamentoIntegracao);

            return sucesso;
        }

        public static void AtualizarTracking(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repositorioConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repositorioConfiguracaoMonitoramento.BuscarPrimeiroRegistro();
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (carga == null)
                return;

            string endPoint = $"{configuracaoIntegracao.URLTrizy}/travel-manager/v1/external/travel/{carga.IDIdentificacaoTrizzy}";
            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), endPoint);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                dynamic objetoEnvio = ConverterObjetoAtualizarTracking(carga, unitOfWork, configuracaoTMS, (configuracaoMonitoramento?.FrequenciaCapturaPosicoesAppTrizy ?? FrequenciaTrackingAppTrizy.High));

                if (string.IsNullOrWhiteSpace(carga.IDIdentificacaoTrizzy))
                    throw new ServicoException("Carga sem ID Trizy");

                jsonRequest = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.SendAsync(request).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    mensagemErro = "Integrado com sucesso";
                }
                else
                {
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a Trizy.";
                    else
                        mensagemErro = "Retorno Trizy: " + mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTrizy");

                mensagemErro = excecao.Message;
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Header: " + configuracaoIntegracao.TokenTrizy, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoTrizy");
                Servicos.Log.TratarErro("Erro: " + excecao.Message, "IntegracaoTrizy");

                return;
            }

            Servicos.Log.TratarErro($"Retorno Integração Trizy: {mensagemErro}", "IntegracaoTrizy");
        }

        private static dynamic ObterObjetoIntegracaoCargaAPP(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy, bool enviarDocumentos = true, bool preTrip = false)
        {
            dynamic obj = new ExpandoObject();
            //Instanciar Repositórios
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy repConfiguracaoTrizy = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repositorioConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteComplementar repositorioClienteComplementar = new Repositorio.Embarcador.Pessoas.ClienteComplementar(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repConfigIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

            //Obter todos os dados antes de processá-los (não se deve realizar consultas posteriormente)
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repositorioConfiguracaoMonitoramento.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy = repConfigIntegracaoTrizy.BuscarPrimeiroRegistro();

            bool utilizarExpedidor = carga.Pedidos?.FirstOrDefault()?.Expedidor != null;
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy = repConfiguracaoTrizy.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);
            if (configuracaoTrizy == null)
                throw new ServicoException($"Tipo de operação {carga.TipoOperacao?.Descricao ?? ""} não realiza integração com Trizy.");

            bool enviarPrimeiraColetaComoOrigin = (!preTrip && (configuracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente ?? false)) || (preTrip && (configuracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip ?? false));
            string dataInicioPrevistaPadrao = carga.DataCriacaoCarga.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
            string dataInicioPrevista = carga.DataInicioViagemPrevista.HasValue ? carga.DataInicioViagemPrevista.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") : dataInicioPrevistaPadrao;
            List<string> codigosOcorrencias = repTipoOcorrencia.BuscarCodigosIntegracaoAuxiliarAtivos();
            List<dynamic> stopovers = new List<dynamic>();
            dynamic destination = new ExpandoObject();
            int codigoCargaEntrega = 0;

            Dominio.Entidades.Cliente remetenteOuExpedidor = utilizarExpedidor ? carga.Pedidos?.FirstOrDefault()?.Expedidor : carga.Pedidos?.FirstOrDefault()?.Pedido?.Remetente;
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            Dominio.ObjetosDeValor.Endereco enderecoPonto = new Dominio.ObjetosDeValor.Endereco();
            MapeiaEndereco(remetenteOuExpedidor, enderecoPonto);

            if (enviarPrimeiraColetaComoOrigin && listaCargaEntrega != null && listaCargaEntrega.Exists(cargaEntrega => cargaEntrega.Coleta))
            {
                codigoCargaEntrega = listaCargaEntrega[0].Codigo;
                remetenteOuExpedidor = listaCargaEntrega[0].Cliente;
                dataInicioPrevista = ObterDataEsperada(configuracaoTrizy, listaCargaEntrega[0]);

                if (listaCargaEntrega[0].ClienteOutroEndereco != null)
                    MapeiaEndereco(listaCargaEntrega[0].ClienteOutroEndereco, enderecoPonto);

                listaCargaEntrega.RemoveAt(0);
            }
            // Temporário, até ser definido regra para cálculo de previsão de início de viagem. 19/07/2024
            else if (listaCargaEntrega != null && !listaCargaEntrega.Exists(cargaEntrega => cargaEntrega.Coleta))
                dataInicioPrevista = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");

            ProcessarEntregasCargaAPP(ref stopovers, ref destination, enviarDocumentos, listaCargaEntrega, preTrip, configuracaoTrizy, unitOfWork, dataInicioPrevista, configuracaoIntegracaoTrizy.TipoDocumentoPais, integracaoTrizy);

            Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar clienteComplementar = repositorioClienteComplementar.BuscarPorCliente(remetenteOuExpedidor?.CPF_CNPJ ?? 0);

            // Constrói a requisição de fato
            obj.externalId = carga.Protocolo.ToString() ?? string.Empty;
            obj.external = ObterExternalCargaAPP(carga, clienteMultisoftware, configuracaoTrizy, preTrip);
            obj.drivers = ObterDriversCargaAPP(carga, configuracaoIntegracaoTrizy.TipoDocumentoPais);
            obj.company = ObterCompanyCargaAPP(carga, integracao, configuracaoIntegracaoTrizy.TipoDocumentoPais);
            obj.origin = ObterPontoParadaCargaAPP(enderecoPonto, remetenteOuExpedidor, enviarPrimeiraColetaComoOrigin ? "COLLECT" : "LOAD", preTrip ? dataInicioPrevistaPadrao : dataInicioPrevista, true, preTrip, false, integracaoTrizy, null, null, carga, enviarPrimeiraColetaComoOrigin, configuracaoTrizy, dataPrevisaoEstouIndo: dataInicioPrevistaPadrao, pedidos: null, codigoCargaEntrega: codigoCargaEntrega, tipoDocumentoPaisTrizy: configuracaoIntegracaoTrizy.TipoDocumentoPais, clienteComplementar: clienteComplementar);
            obj.stopovers = stopovers;
            obj.destination = destination;
            obj.currency = "BRL";
            obj.language = "pt-BR";
            obj.alerts = ObterAlertsCargaAPP(configuracaoTrizy, preTrip);
            obj.vehicle = ObterVehicleCargaAPP(carga);
            obj.observation = "";
            if (!preTrip) obj.occurrence = ObterOccurrenceCargaAPP(codigosOcorrencias.Count > 0, codigosOcorrencias);
            obj.tracking = ObterTrackingCargaAPP(integracaoTrizy, (configuracaoMonitoramento?.FrequenciaCapturaPosicoesAppTrizy ?? FrequenciaTrackingAppTrizy.High), configuracaoTMS);
            obj.chatAvailable = configuracaoTrizy?.HabilitarChat ?? true;
            obj.additionalInformation = ObterInformacoesAdicionaisCarga(carga);
            if (carga.TipoOperacao?.ConfiguracaoTrizy?.EnviarDadosEmpresaGR ?? false)
                obj.riskManager = ObterInformacoesGR(carga);

            return obj;
        }


        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ViagemV3 ObterObjetoIntegracaoCargaAPPV3(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy, bool enviarDocumentos = true, bool preTrip = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ViagemV3 obj = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ViagemV3();
            //Instanciar Repositórios
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy repConfiguracaoTrizy = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repositorioConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteComplementar repositorioClienteComplementar = new Repositorio.Embarcador.Pessoas.ClienteComplementar(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repConfigIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
            Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa repChecklistSuperAppEtapa = new Repositorio.Embarcador.SuperApp.ChecklistSuperAppEtapa(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacaoEventoSuperApp repTipoOperacaoEventoSuperApp = new Repositorio.Embarcador.Pedidos.TipoOperacaoEventoSuperApp(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
            Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repositorioMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);

            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new(unitOfWork);
            //Obter todos os dados antes de processá-los (não se deve realizar consultas posteriormente)
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repositorioConfiguracaoMonitoramento.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy = repConfigIntegracaoTrizy.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy = repConfiguracaoTrizy.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> todosOsProdutosCargaEntrega = repositorioCargaEntregaProduto.BuscarPorCarga(carga.Codigo);

            if (configuracaoTrizy == null)
                throw new ServicoException($"Tipo de operação {carga.TipoOperacao?.Descricao ?? ""} não realiza integração com Trizy.");

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> listaTipoOperacaoEventoSuperApp = repTipoOperacaoEventoSuperApp.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0).Result;
            List<int> codigosChecklist = listaTipoOperacaoEventoSuperApp.Select(c => c.EventoSuperApp.ChecklistSuperApp?.Codigo ?? 0).ToList();
            List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaChecklistSuperAppEtapa = repChecklistSuperAppEtapa.BuscarPorChecklists(codigosChecklist);
            List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio> listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio = repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio.BuscarPorConfiguracaoTipoOperacaoTrizy(configuracaoTrizy.Codigo);

            List<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega> motivosDevolucaoEntregas = repositorioMotivoDevolucaoEntrega.BuscarAtivos();
            List<string> codigosOcorrencias = repTipoOcorrencia.BuscarIdsSuperAppAtivos();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            bool utilizarExpedidor = carga.Pedidos?.FirstOrDefault()?.Expedidor != null;
            Dominio.Entidades.Cliente remetenteOuExpedidor = utilizarExpedidor ? carga.Pedidos?.FirstOrDefault()?.Expedidor : carga.Pedidos?.FirstOrDefault()?.Pedido?.Remetente;

            bool enviarPrimeiraColetaComoOrigin = (!preTrip && (configuracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente ?? false)) || (preTrip && (configuracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip ?? false));
            string dataInicioPrevistaPadrao = carga.DataCriacaoCarga.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
            string dataInicioPrevista = carga.DataInicioViagemPrevista.HasValue ? carga.DataInicioViagemPrevista.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") : dataInicioPrevistaPadrao;

            bool devolucaoPorPeso = carga.TipoOperacao?.DevolucaoProdutosPorPeso ?? false;
            bool temItems = false;

            validarRegrasV3(listaChecklistSuperAppEtapa, configuracaoTrizy, carga);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPoint> stopovers = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPoint>();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPoint destination = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPoint();
            int codigoCargaEntrega = 0;

            Dominio.ObjetosDeValor.Endereco enderecoPonto = new Dominio.ObjetosDeValor.Endereco();
            MapeiaEndereco(remetenteOuExpedidor, enderecoPonto);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> produtosDaEntregaOrigin = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>();

            if (enviarPrimeiraColetaComoOrigin && listaCargaEntrega != null && listaCargaEntrega.Exists(cargaEntrega => cargaEntrega.Coleta))
            {
                codigoCargaEntrega = listaCargaEntrega[0].Codigo;
                remetenteOuExpedidor = listaCargaEntrega[0].Cliente;
                dataInicioPrevista = ObterDataEsperada(configuracaoTrizy, listaCargaEntrega[0]);

                if (listaCargaEntrega[0].ClienteOutroEndereco != null)
                    MapeiaEndereco(listaCargaEntrega[0].ClienteOutroEndereco, enderecoPonto);

                produtosDaEntregaOrigin = todosOsProdutosCargaEntrega.Where(produto => produto.CargaEntrega.Codigo == listaCargaEntrega[0].Codigo).ToList();

                listaCargaEntrega.RemoveAt(0);
            }
            else if (listaCargaEntrega != null && !listaCargaEntrega.Exists(cargaEntrega => cargaEntrega.Coleta))
                dataInicioPrevista = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");

            List<int> codigosCargaEntrega = listaCargaEntrega.Select(c => c.Codigo).ToList();
            List<double> listaCpfCnpj = listaCargaEntrega.Select(c => c.Cliente?.CPF_CNPJ ?? 0).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregasPedido = repCargaEntregaPedido.BuscarPorCargaEntregas(codigosCargaEntrega);

            List<int> codigosPedidos = cargaEntregasPedido.Select(c => c.CargaPedido.Pedido.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = repPedidoProduto.BuscarPorPedidos(codigosPedidos);
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = servicoControleEntrega.ObterCanhotosDaCargaEntregas(listaCargaEntrega);
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> listaClientesComplementares = repositorioClienteComplementar.BuscarPorClientesAsync(listaCpfCnpj).Result;


            ProcessarEntregasCargaAPPV3(ref stopovers, ref destination, enviarDocumentos, listaCargaEntrega, preTrip, configuracaoTrizy, unitOfWork, dataInicioPrevista, configuracaoIntegracaoTrizy.TipoDocumentoPais, integracaoTrizy, cargaEntregasPedido, produtos, canhotos, listaClientesComplementares, listaTipoOperacaoEventoSuperApp, listaChecklistSuperAppEtapa, carga.TipoOperacao.CheckListColeta, carga.TipoOperacao.CheckListEntrega, carga.ModeloVeicularCarga, todosOsProdutosCargaEntrega, devolucaoPorPeso, ref temItems);

            Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar clienteComplementar = repositorioClienteComplementar.BuscarPorCliente(remetenteOuExpedidor?.CPF_CNPJ ?? 0);

            // Constrói a requisição de fato
            obj.externalId = carga.Codigo.ToString() ?? string.Empty;
            obj.external = ObterExternalCargaAPPV3(carga, clienteMultisoftware, configuracaoTrizy, preTrip);
            obj.drivers = ObterDriversCargaAPPV3(carga, configuracaoIntegracaoTrizy.TipoDocumentoPais);
            obj.company = ObterCompanyCargaAPPV3(carga, integracao, configuracaoIntegracaoTrizy.TipoDocumentoPais);
            obj.origin = ObterPontoParadaCargaAPPV3(enderecoPonto, remetenteOuExpedidor, enviarPrimeiraColetaComoOrigin ? "COLLECT" : "LOAD", preTrip ? dataInicioPrevistaPadrao : dataInicioPrevista, true, preTrip, false, integracaoTrizy, ref temItems, null, null, carga, enviarPrimeiraColetaComoOrigin, configuracaoTrizy, dataPrevisaoEstouIndo: dataInicioPrevistaPadrao, pedidos: null, codigoCargaEntrega: codigoCargaEntrega, tipoDocumentoPaisTrizy: configuracaoIntegracaoTrizy.TipoDocumentoPais, clienteComplementar: clienteComplementar, eventosSuperApp: listaTipoOperacaoEventoSuperApp, listaChecklistSuperAppEtapa: listaChecklistSuperAppEtapa, checklistColeta: carga.TipoOperacao.CheckListColeta, checklistEntrega: carga.TipoOperacao.CheckListEntrega, modeloVeicular: carga.ModeloVeicularCarga, produtosDaEntrega: produtosDaEntregaOrigin, devolucaoPorPeso: devolucaoPorPeso);
            obj.stopovers = stopovers;
            obj.destination = destination;
            obj.currency = "BRL";
            obj.language = "pt-BR";
            obj.alerts = ObterAlertsCargaAPPV3(configuracaoTrizy, preTrip);
            obj.vehicle = ObterVehicleCargaAPPV3(carga);
            obj.tracking = ObterTrackingCargaAPPV3(integracaoTrizy, (configuracaoMonitoramento?.FrequenciaCapturaPosicoesAppTrizy ?? FrequenciaTrackingAppTrizy.High), configuracaoTMS);
            obj.chatAvailable = configuracaoTrizy?.HabilitarChat ?? true;
            obj.additionalInformation = ObterInformacoesAdicionaisCarga(carga);

            if (!carga.TipoOperacao?.ConfiguracaoTrizy?.NaoEnviarPolilinha ?? true)
            {
                obj.route = ObterRouteAppV3(cargaRotaFrete);
            }

            if (carga.TipoOperacao?.ConfiguracaoTrizy?.EnviarDadosEmpresaGR ?? false)
                obj.riskManager = ObterInformacoesGRV3(carga);

            obj.features = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ViagemV3Features()
            {
                canSearchDocument = true,
                occurrence = ObterOccurrenceCargaAPPV3(preTrip, codigosOcorrencias),
                loadDetail = ObterLoadDetailAPPV3(configuracaoTrizy, listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio, carga.ModeloVeicularCarga.LayoutSuperAppId),
                partialDelivery = ObterPartialDeliveryFeatureAPPV3(motivosDevolucaoEntregas, temItems, carga?.TipoOperacao?.ConfiguracaoTrizy?.HabilitarDevolucaoParcial ?? false),
                notDelivered = ObterNotDeliveredFeatureAPPV3(motivosDevolucaoEntregas, temItems, carga?.TipoOperacao?.ConfiguracaoTrizy?.HabilitarDevolucao ?? false),
                travelInitializationAction = configuracaoTrizy.NecessarioFinalizarOrigem ? new TravelInitializationActionFeature()
                {
                    point = 0,
                    type = "COMPLETE_POINT"
                } : null,
                documentItemValidator = temItems && carga.TipoOperacao?.ConfiguracaoTrizy?.HabilitarDevolucaoParcial == true ? "DOCUMENT_ITEM" : null
            };

            return obj;
        }

        private static List<dynamic> ObterAlertsCargaAPP(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy, bool preTrip)
        {
            List<dynamic> alerts = new List<dynamic>();
            if (!preTrip || (!(configuracaoTrizy?.EnviarMensagemAlertaPreTrip ?? false)))
                return alerts;
            alerts.Add(new
            {
                description = "Carga de Pré Trip - Deslocamento para carregar",
                title = "Alerta",
                color = "ERROR"
            });
            return alerts;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Alert> ObterAlertsCargaAPPV3(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy, bool preTrip)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Alert> alerts = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Alert>();
            if (!preTrip || (!(configuracaoTrizy?.EnviarMensagemAlertaPreTrip ?? false)))
                return alerts;
            alerts.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Alert()
            {
                description = GerarTextoInternacionalizado("Carga de Pré Trip - Deslocamento para carregar"),
                title = "Alerta",
                color = "ERROR"
            });
            return alerts;
        }

        private static dynamic ObterTrackingCargaAPP(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy, FrequenciaTrackingAppTrizy frequenciaTrackingAppTrizy, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            dynamic tracking = new
            {
                enabled = configuracaoTMS.PossuiMonitoramento,
                priority = frequenciaTrackingAppTrizy.ObterFrequencia(),
                startAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),  //carga.DataInicioViagemPrevista.HasValue ? carga.DataInicioViagemPrevista.Value.AddHours(-12).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") : dataInicioPrevistaPadrao,
                endAt = integracaoTrizy.DiasIntervaloTracking > 0 ? DateTime.Now.AddDays(integracaoTrizy.DiasIntervaloTracking).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") : DateTime.Now.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")//ObterDataEsperada(configuracaoTrizy, listaCargaEntrega != null ? listaCargaEntrega[listaCargaEntrega.Count - 1] : null)
            };

            return tracking;
        }
        private static Tracking ObterTrackingCargaAPPV3(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy, FrequenciaTrackingAppTrizy frequenciaTrackingAppTrizy, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Tracking tracking = new Tracking()
            {
                enabled = configuracaoTMS.PossuiMonitoramento,
                priority = frequenciaTrackingAppTrizy.ObterFrequencia(),
                startAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),  //carga.DataInicioViagemPrevista.HasValue ? carga.DataInicioViagemPrevista.Value.AddHours(-12).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") : dataInicioPrevistaPadrao,
                endAt = integracaoTrizy.DiasIntervaloTracking > 0 ? DateTime.Now.AddDays(integracaoTrizy.DiasIntervaloTracking).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") : DateTime.Now.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")//ObterDataEsperada(configuracaoTrizy, listaCargaEntrega != null ? listaCargaEntrega[listaCargaEntrega.Count - 1] : null)
            };

            return tracking;
        }
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Route ObterRouteAppV3(Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RouteSegment> segments = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RouteSegment>();

            segments.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RouteSegment()
            {
                polyline = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RouteSegmentPolyline()
                {
                    encoded = cargaRotaFrete.PolilinhaRota
                }
            });

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Route route = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Route()
            {
                provider = "OSRM",
                segments = segments
            };

            return route;
        }

        private static dynamic ObterVehicleCargaAPP(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            int eixos = carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 2;
            dynamic vehicle = new Vehicle()
            {
                transportMode = "TRUCK",
                axleCount = Math.Max(eixos, 2),
                plateCount = 1,
                label = carga.Veiculo?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                licensePlate = carga.Veiculo?.Placa ?? string.Empty
            };
            return vehicle;
        }
        private static dynamic ObterVehicleMDFeManual(Dominio.Entidades.Veiculo veiculo)
        {
            dynamic vehicle = new
            {
                transportMode = "TRUCK",
                axleCount = (veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 1) == 0 ? 1 : (veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 1),
                plateCount = 1,
                label = veiculo?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                licensePlate = veiculo?.Placa ?? string.Empty
            };
            return vehicle;
        }

        private static dynamic ObterVehicleDriversCargaAPP(Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Usuario> motoristas, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy)
        {
            dynamic data = new
            {
                drivers = ObterDriversMDFeManual(motoristas, configuracaoIntegracaoTrizy.TipoDocumentoPais),
                vehicle = ObterVehicleMDFeManual(veiculo),
            };

            return data;
        }
        private static dynamic ObterDocumentsMDFeManual(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork).BuscarPrimeiroRegistro();

            string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";
            string arquivo = string.Empty;

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                arquivo = Convert.ToBase64String(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF));

            dynamic data = new
            {
                travel = carga.IDIdentificacaoTrizzy,
                documentType = DocumentosFiscaisTrizy.MDFe.ObterIDTrizy(),
                file = arquivo,
                label = $"{DocumentosFiscaisTrizy.MDFe.ObterDescricao()}: Número {mdfe.Numero} - Série {mdfe.Serie?.Numero}",
                description = mdfe.Chave
            };

            return data;
        }


        private static Vehicle ObterVehicleCargaAPPV3(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            int eixos = carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 2;
            Vehicle vehicle = new Vehicle()
            {
                transportMode = "TRUCK",
                axleCount = Math.Max(eixos, 2),
                plateCount = 1,
                label = carga.Veiculo?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                licensePlate = carga.Veiculo?.Placa ?? string.Empty
            };
            return vehicle;
        }

        private static void ProcessarEntregasCargaAPP(ref List<dynamic> stopovers, ref dynamic destination, bool enviarDocumentos, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega, bool preTrip, Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy, Repositorio.UnitOfWork unitOfWork, string dataPrevisaoEstouIndo, TipoDocumentoPaisTrizy tipoDocumentoPaisTrizy, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteComplementar repositorioClienteComplementar = new Repositorio.Embarcador.Pessoas.ClienteComplementar(unitOfWork);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in listaCargaEntrega)
            {
                //Não deveria, mas pode consultar nesse caso:
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = repPedidoProduto.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                //List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = enviarDocumentos ? repNotaFiscal.BuscarXMLNotaFiscalPorCargaPedido(cargaEntrega.Pedidos?.Select(e => e.CargaPedido.Codigo).ToList() ?? new List<int>()) : null;
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaEntrega = servicoControleEntrega.ObterCanhotosDaCargaEntrega(cargaEntrega);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cargaEntrega.Pedidos?.Select(p => p.CargaPedido.Pedido).ToList();
                Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar clienteComplementar = repositorioClienteComplementar.BuscarPorCliente(cargaEntrega.Cliente?.CPF_CNPJ ?? 0);

                string dataPrevisao = ObterDataEsperada(configuracaoTrizy, cargaEntrega);
                bool adicionarInicioViagemPrimeiraEntrega = (cargaEntrega.Codigo == listaCargaEntrega[0].Codigo);
                if (string.IsNullOrEmpty(dataPrevisaoEstouIndo))
                    dataPrevisaoEstouIndo = dataPrevisao;

                // Aqui, se configura o Destino
                if (cargaEntrega.Codigo == listaCargaEntrega[listaCargaEntrega.Count - 1].Codigo)
                    destination = ObterPontoParadaCargaAPP(ObterEnderecoEntrega(cargaEntrega), cargaEntrega.Cliente, cargaEntrega.Coleta ? "COLLECT" : "DELIVER", dataPrevisao, false, preTrip, enviarDocumentos, integracaoTrizy, canhotosDaEntrega, produtos, cargaEntrega.Carga, cargaEntrega.Coleta, configuracaoTrizy, true, adicionarInicioViagemPrimeiraEntrega, dataPrevisaoEstouIndo, pedidos, cargaEntrega.Codigo, tipoDocumentoPaisTrizy, clienteComplementar);
                else // Entregas
                    stopovers.Add(ObterPontoParadaCargaAPP(ObterEnderecoEntrega(cargaEntrega), cargaEntrega.Cliente, cargaEntrega.Coleta ? "COLLECT" : "DELIVER", dataPrevisao, false, preTrip, enviarDocumentos, integracaoTrizy, canhotosDaEntrega, produtos, cargaEntrega.Carga, cargaEntrega.Coleta, configuracaoTrizy, false, adicionarInicioViagemPrimeiraEntrega, dataPrevisaoEstouIndo, pedidos, cargaEntrega.Codigo, tipoDocumentoPaisTrizy, clienteComplementar));

                // Guarda a data de previsão do ponto anterior para mandar como data do evento "Estou Indo".
                dataPrevisaoEstouIndo = dataPrevisao;
            }
        }

        private static void ProcessarEntregasCargaAPPV3(ref List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPoint> stopovers, ref Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPoint destination, bool enviarDocumentos, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega, bool preTrip, Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy, Repositorio.UnitOfWork unitOfWork, string dataPrevisaoEstouIndo, TipoDocumentoPaisTrizy tipoDocumentoPaisTrizy, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> todasCargaEntregasPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> todosProdutos, List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> todosCanhotos, List<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> todosClientesComplementares, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> listaTipoOperacaoEventoSuperApp, List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaChecklistSuperAppEtapa, Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checklistColeta, Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checklistEntrega, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> todosOsProdutosCargaEntrega, bool devolucaoPorPeso, ref bool temItems)
        {
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in listaCargaEntrega)
            {
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregasPedido = todasCargaEntregasPedido.Where(tcp => tcp.CargaEntrega.Codigo == cargaEntrega.Codigo).ToList();
                List<int> codigosEntregasPedido = cargaEntregasPedido
                .Select(cep => cep.CargaPedido.Pedido.Codigo)
                .Distinct()
                .ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = todosProdutos.Where(tp => codigosEntregasPedido.Contains(tp.Pedido.Codigo)).ToList();
                //List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = enviarDocumentos ? repNotaFiscal.BuscarXMLNotaFiscalPorCargaPedido(cargaEntrega.Pedidos?.Select(e => e.CargaPedido.Codigo).ToList() ?? new List<int>()) : null;
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaEntrega = todosCanhotos.Where(tc => codigosEntregasPedido.Contains(tc.Pedido?.Codigo ?? 0)).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cargaEntrega.Pedidos?.Select(p => p.CargaPedido.Pedido).ToList();
                Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar clienteComplementar = cargaEntrega.Cliente?.CPF_CNPJ != 0 ? todosClientesComplementares.FirstOrDefault(tcc => tcc.Cliente.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ) : null;
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> produtosDaEntrega = todosOsProdutosCargaEntrega.Where(produto => produto.CargaEntrega.Codigo == cargaEntrega.Codigo).ToList();

                string dataPrevisao = ObterDataEsperada(configuracaoTrizy, cargaEntrega);
                bool adicionarInicioViagemPrimeiraEntrega = (cargaEntrega.Codigo == listaCargaEntrega[0].Codigo);
                if (string.IsNullOrEmpty(dataPrevisaoEstouIndo))
                    dataPrevisaoEstouIndo = dataPrevisao;

                // Aqui, se configura o Destino
                if (cargaEntrega.Codigo == listaCargaEntrega[listaCargaEntrega.Count - 1].Codigo)
                    destination = ObterPontoParadaCargaAPPV3(ObterEnderecoEntrega(cargaEntrega), cargaEntrega.Cliente, cargaEntrega.Coleta ? "COLLECT" : "DELIVER", dataPrevisao, false, preTrip, enviarDocumentos, integracaoTrizy, ref temItems, canhotosDaEntrega, produtos, cargaEntrega.Carga, cargaEntrega.Coleta, configuracaoTrizy, true, adicionarInicioViagemPrimeiraEntrega, dataPrevisaoEstouIndo, pedidos, cargaEntrega.Codigo, tipoDocumentoPaisTrizy, clienteComplementar, listaTipoOperacaoEventoSuperApp, listaChecklistSuperAppEtapa, checklistColeta: checklistColeta, checklistEntrega: checklistEntrega, modeloVeicular: modeloVeicular, produtosDaEntrega: produtosDaEntrega, devolucaoPorPeso: devolucaoPorPeso);
                else // Entregas
                    stopovers.Add(ObterPontoParadaCargaAPPV3(ObterEnderecoEntrega(cargaEntrega), cargaEntrega.Cliente, cargaEntrega.Coleta ? "COLLECT" : "DELIVER", dataPrevisao, false, preTrip, enviarDocumentos, integracaoTrizy, ref temItems, canhotosDaEntrega, produtos, cargaEntrega.Carga, cargaEntrega.Coleta, configuracaoTrizy, false, adicionarInicioViagemPrimeiraEntrega, dataPrevisaoEstouIndo, pedidos, cargaEntrega.Codigo, tipoDocumentoPaisTrizy, clienteComplementar, listaTipoOperacaoEventoSuperApp, listaChecklistSuperAppEtapa, checklistColeta: checklistColeta, checklistEntrega: checklistEntrega, modeloVeicular: modeloVeicular, produtosDaEntrega: produtosDaEntrega, devolucaoPorPeso: devolucaoPorPeso));

                // Guarda a data de previsão do ponto anterior para mandar como data do evento "Estou Indo".
                dataPrevisaoEstouIndo = dataPrevisao;
            }
        }

        private static dynamic ObterPontoParadaCargaAPP(Dominio.ObjetosDeValor.Endereco endereco, Dominio.Entidades.Cliente ponto, string tipoPonto, string dataPrevisao, bool origem, bool preTrip, bool enviarDocumentos, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy, List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaEntrega = null, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = null, Dominio.Entidades.Embarcador.Cargas.Carga carga = null, bool coleta = false, Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy = null, bool destination = false, bool adicionarInicioViagemPrimeiraEntrega = false, string dataPrevisaoEstouIndo = "", List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = null, int codigoCargaEntrega = 0, TipoDocumentoPaisTrizy tipoDocumentoPaisTrizy = TipoDocumentoPaisTrizy.Brasil, Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar clienteComplementar = null)
        {
            dynamic obj = new ExpandoObject();

            if (carga == null)
                return obj;

            int quantidadeCanhotos = (ponto?.ClienteDescargas?.FirstOrDefault()?.PossuiCanhotoDeDuasOuMaisPaginas ?? false) ? (ponto?.ClienteDescargas?.FirstOrDefault()?.QuantidadeDePaginasDoCanhoto ?? 1) : 1;
            bool destinationPreTrip = destination && preTrip;
            bool naoEnviarTagValidacao = (carga.TipoOperacao?.ConfiguracaoTrizy?.NaoEnviarTagValidacao ?? false) || (canhotosDaEntrega == null || canhotosDaEntrega.Count == 0) || quantidadeCanhotos > 1;
            bool naoEnviarDocumentosFiscais = (carga.TipoOperacao?.ConfiguracaoTrizy?.NaoEnviarDocumentosFiscais ?? false);
            bool exigirInformarNumeroPacotesNaColeta = coleta && (carga.TipoOperacao?.ConfiguracaoControleEntrega?.ExigirInformarNumeroPacotesNaColetaTrizy ?? false);
            bool solicitarComprovanteColetaEntrega = (carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarComprovanteColetaEntrega ?? false);
            bool solicitarAssinaturaNaConfirmacaoDeColetaEntrega = !coleta && (carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarAssinaturaNaConfirmacaoDeColetaEntrega ?? false);
            bool SolicitarFotoComoEvidenciaOpcional = carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarFotoComoEvidenciaOpcional ?? false;
            bool SolicitarFotoComoEvidenciaObrigatoria = (carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarFotoComoEvidenciaObrigatoria ?? false);
            bool enviarEventoIniciarViagemComoObrigatorio = (preTrip && !(configuracaoTrizy?.EnviarEventoIniciarViagemComoOpcionalPreTrip ?? false)) || (!preTrip && !(configuracaoTrizy?.EnviarEventoIniciarViagemComoOpcional ?? false));
            bool enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos = carga.TipoOperacao?.ConfiguracaoTrizy?.EnviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos ?? false;
            bool enviarNomeRecebedorConfirmacao = carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega ?? false;
            bool enviarDocumentoConfirmacao = carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarDocumentoNaConfirmacaoDeColetaEntrega ?? false;
            bool identificarNotaPallet = (carga.TipoOperacao?.ConfiguracaoTrizy?.IdentificarNotaDeMercadoriaENotaDePallet ?? false);
            bool converterCanhotoParaPretoeBrancoERotacionarAutomaticamente = (carga.TipoOperacao?.ConfiguracaoTrizy?.ConverterCanhotoParaPretoeBrancoERotacionarAutomaticamente ?? false);
            bool enviarMascaraFixaParaoCanhoto = (carga.TipoOperacao?.ConfiguracaoTrizy?.EnviarMascaraFixaParaoCanhoto ?? false);
            bool enviarMascaraDinamicaParaoCanhoto = (carga.TipoOperacao?.ConfiguracaoTrizy?.EnviarMascaraDinamicaParaoCanhoto ?? false);
            bool naoPermitirVincularFotosDaGaleriaParaCanhotos = (carga.TipoOperacao?.ConfiguracaoTrizy?.NaoPermitirVincularFotosDaGaleriaParaCanhotos ?? false);
            bool solicitarApenasCanhotos = carga.TipoOperacao?.ConfiguracaoTrizy?.NaoEnviarEventosViagemColetaEntregaSolicitarApenasCanhotos ?? false;
            bool naoEnviarEventosNaOrigem = carga.TipoOperacao?.ConfiguracaoTrizy?.NaoEnviarEventosNaOrigem ?? false;

            obj.operation = tipoPonto;
            obj.expectedAt = dataPrevisao;
            if (codigoCargaEntrega > 0)
                obj.externalId = codigoCargaEntrega.ToString() + ";" + ponto.CPF_CNPJ;

            obj.point = new
            {
                location = ObterLocationCargaAPP("Point", endereco.Longitude.ToDecimal(), endereco.Latitude.ToDecimal()),
                address = new
                {
                    countryCode = "BRA",
                    country = "Brasil",
                    stateCode = endereco.Cidade.Estado.Sigla,
                    state = endereco.Cidade.Estado.Descricao,
                    city = endereco.Cidade.Descricao,
                    district = endereco.Bairro,
                    street = endereco.Logradouro,
                    postalCode = endereco.CEP,
                    houseNumber = endereco.Numero
                }
            };
            string tipoDocumento;
            if (ponto.Tipo == "E")
            {
                tipoDocumentoPaisTrizy = TipoDocumentoPaisTrizy.Estrangeiro;
                tipoDocumento = tipoDocumentoPaisTrizy.ObterPersonDocumentType();
            }
            else if (ponto.Tipo == "F")
                tipoDocumento = tipoDocumentoPaisTrizy.ObterPersonDocumentType();
            else
                tipoDocumento = tipoDocumentoPaisTrizy.ObterLegalPersonDocumentType();

            obj.client = new
            {
                name = integracaoTrizy.EnviarNomeFantasiaQuandoPossuir && !string.IsNullOrWhiteSpace(ponto.NomeFantasia)
                    ? ponto.NomeFantasia
                    : ponto.Nome,
                document = ObterDocumentCargaAPP(tipoDocumento, ponto.CPF_CNPJ_SemFormato.ToString())
            };

            if (!destinationPreTrip || (carga.TipoOperacao?.ConfiguracaoTrizy.NaoFinalizarPreTrip ?? false))
            {
                if (!(origem && naoEnviarEventosNaOrigem))
                {
                    if (!(tipoPonto == "COLLECT" && naoEnviarEventosNaOrigem))
                    {
                        obj.events = new
                        {
                            flow = "SEQUENTIAL",
                            hasDeliveryReceipt = (!origem && (configuracaoTrizy?.SolicitarComprovanteColetaEntrega ?? true)) || exigirInformarNumeroPacotesNaColeta,
                            items = new List<dynamic>()
                        };
                    }
                }
            }

            if (!origem)
            {
                if (!destinationPreTrip && enviarDocumentos && canhotosDaEntrega != null && canhotosDaEntrega.Count > 0 && !naoEnviarDocumentosFiscais)
                    obj.documents = ObterDocumentsCargaAPP(canhotosDaEntrega, enviarDocumentos, identificarNotaPallet, converterCanhotoParaPretoeBrancoERotacionarAutomaticamente, enviarMascaraFixaParaoCanhoto, enviarMascaraDinamicaParaoCanhoto, naoPermitirVincularFotosDaGaleriaParaCanhotos);
                if (!destinationPreTrip && (produtos != null && produtos.Count > 0))
                    obj.products = ObterProductsCargaAPP(produtos, enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos);

                if (!(tipoPonto == "COLLECT" && naoEnviarEventosNaOrigem))
                {
                    List<dynamic> evidences = ObterEvidencesCargaAPP(naoEnviarTagValidacao,
                    solicitarAssinaturaNaConfirmacaoDeColetaEntrega,
                    SolicitarFotoComoEvidenciaObrigatoria,
                    SolicitarFotoComoEvidenciaOpcional,
                    exigirInformarNumeroPacotesNaColeta,
                    quantidadeCanhotos,
                    solicitarComprovanteColetaEntrega,
                    enviarNomeRecebedorConfirmacao,
                    enviarDocumentoConfirmacao,
                    converterCanhotoParaPretoeBrancoERotacionarAutomaticamente,
                    enviarMascaraFixaParaoCanhoto,
                    enviarMascaraDinamicaParaoCanhoto,
                    naoPermitirVincularFotosDaGaleriaParaCanhotos,
                    solicitarApenasCanhotos);

                    if (evidences != null && evidences.Count > 0)
                        obj.evidences = evidences;


                    dynamic eventoIniciarViagem = ObterEventCargaAPP("START_TRAVEL", "Iniciar viagem", dataPrevisao, enviarEventoIniciarViagemComoObrigatorio, "CREATED");
                    dynamic eventoCheguei = ObterEventCargaAPP("START_OPERATION", "Cheguei para descarregar", dataPrevisao, true, "CREATED");
                    dynamic eventoComprovante = ObterEventCargaAPP("DELIVERY_RECEIPT", "Comprovante de entrega", dataPrevisao, true, "CREATED");
                    dynamic eventoConfirmar = ObterEventCargaAPP("END_OPERATION", "Confirmar entrega", dataPrevisao, true, "CREATED");
                    dynamic eventoEstouIndo = ObterEventCargaAPP("CUSTOM", "Estou indo", dataPrevisaoEstouIndo, true, "CREATED", TipoCustomEventAppTrizy.EstouIndo);
                    dynamic eventoSolicitacaoDataeHoraCanhoto = ObterEventCargaAPP("CUSTOM", "Preencha data e hora do canhoto", dataPrevisao, true, "CREATED", TipoCustomEventAppTrizy.SolicitacaoDataeHoraCanhoto);

                    if (coleta)
                    {
                        eventoCheguei = ObterEventCargaAPP("START_OPERATION", "Cheguei para carregar", dataPrevisao, true, "CREATED");
                        eventoComprovante = ObterEventCargaAPP("DELIVERY_RECEIPT", "Comprovante de coleta", dataPrevisao, true, "CREATED");
                        eventoConfirmar = ObterEventCargaAPP("END_OPERATION", "Confirmar coleta", dataPrevisao, true, "CREATED");
                    }

                    bool adicionarEstouIndo = false;
                    bool adicionarEventoIniciarViagem = false;

                    if (tipoPonto == "COLLECT")
                    {
                        adicionarEstouIndo = ((configuracaoTrizy?.EnviarEstouIndoColeta ?? false) && !preTrip) || (preTrip && (configuracaoTrizy?.EnviarEstouIndoColetaPreTrip ?? false));
                        adicionarEventoIniciarViagem = ((configuracaoTrizy?.EnviarInicioViagemColeta ?? false) && !preTrip) || (preTrip && (configuracaoTrizy?.EnviarIniciarViagemColetaPreTrip ?? false));
                    }
                    else if (tipoPonto == "DELIVER")
                    {
                        adicionarEstouIndo = (((configuracaoTrizy?.EnviarEstouIndoEntrega ?? false) && !preTrip) || (preTrip && (configuracaoTrizy?.EnviarEstouIndoEntregaPreTrip ?? false)));
                        adicionarEventoIniciarViagem = ((configuracaoTrizy?.EnviarInicioViagemEntrega ?? false) && !preTrip) || (preTrip && (configuracaoTrizy?.EnviarIniciarViagemEntregaPreTrip ?? false));
                    }

                    bool adicionarCheguei = false;
                    if (!preTrip)
                        adicionarCheguei = (coleta && configuracaoTrizy.EnviarChegueiParaCarregar) || (!coleta && configuracaoTrizy.EnviarChegueiParaDescarregar);
                    else
                        adicionarCheguei = (coleta && configuracaoTrizy.EnviarChegueiParaCarregarPreTrip) || (!coleta && configuracaoTrizy.EnviarChegueiParaDescarregarPreTrip);

                    bool adicionarComprovante = (preTrip && !coleta && (configuracaoTrizy?.SolicitarComprovanteEntregaSemOCRPreTrip ?? false)) || ((configuracaoTrizy?.SolicitarComprovanteColetaEntrega ?? true) && !preTrip) || (coleta && exigirInformarNumeroPacotesNaColeta) || solicitarAssinaturaNaConfirmacaoDeColetaEntrega;
                    bool adicionarConfirmar = !preTrip || (carga.TipoOperacao?.ConfiguracaoTrizy.NaoFinalizarPreTrip ?? false);
                    bool adicionarSolicitacaoDataeHoraCanhoto = !preTrip && (configuracaoTrizy?.SolicitarComprovanteColetaEntrega ?? true) && (configuracaoTrizy?.SolicitarDataeHoraDoCanhoto ?? false);

                    if (!destinationPreTrip || (carga.TipoOperacao?.ConfiguracaoTrizy.NaoFinalizarPreTrip ?? false))
                    {
                        if (solicitarApenasCanhotos)
                        {
                            if (adicionarComprovante)
                                obj.events.items.Add(eventoComprovante);
                        }
                        else
                        {
                            if (adicionarEventoIniciarViagem && adicionarInicioViagemPrimeiraEntrega)
                                obj.events.items.Add(eventoIniciarViagem);
                            if (adicionarEstouIndo)
                                obj.events.items.Add(eventoEstouIndo);
                            if (adicionarCheguei)
                                obj.events.items.Add(eventoCheguei);
                            if (adicionarSolicitacaoDataeHoraCanhoto)
                                obj.events.items.Add(eventoSolicitacaoDataeHoraCanhoto);
                            if (adicionarComprovante)
                                obj.events.items.Add(eventoComprovante);
                            if (adicionarConfirmar)
                                obj.events.items.Add(eventoConfirmar);
                        }
                    }
                }
            }
            else
            {
                if (!(origem && naoEnviarEventosNaOrigem))
                {
                    bool enviarPrimeiraColetaComoOrigin = (!preTrip && (configuracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente ?? false)) || (preTrip && (configuracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip ?? false));

                    bool adicionarCheguei = (!preTrip && (configuracaoTrizy?.EnviarChegueiParaCarregar ?? false)) || (preTrip && (configuracaoTrizy?.EnviarChegueiParaCarregarPreTrip ?? false));

                    if (enviarPrimeiraColetaComoOrigin)
                    {
                        List<dynamic> evidences = ObterEvidencesCargaAPP(naoEnviarTagValidacao,
                                               false,
                                               SolicitarFotoComoEvidenciaObrigatoria,
                                               SolicitarFotoComoEvidenciaOpcional,
                                               exigirInformarNumeroPacotesNaColeta,
                                               quantidadeCanhotos,
                                               solicitarComprovanteColetaEntrega,
                                               enviarNomeRecebedorConfirmacao,
                                               enviarDocumentoConfirmacao,
                                               converterCanhotoParaPretoeBrancoERotacionarAutomaticamente,
                                               enviarMascaraFixaParaoCanhoto,
                                               enviarMascaraDinamicaParaoCanhoto,
                                               naoPermitirVincularFotosDaGaleriaParaCanhotos,
                                               solicitarApenasCanhotos);

                        if (evidences != null && evidences.Count > 0)
                            obj.evidences = evidences;
                    }

                    bool adicionarEventoEstouIndo = false;
                    bool adicionarEventoChegueiParaColetar = false;
                    bool adicionarEventoConfirmar = false;
                    bool adicionarEventoIniciarViagem = false;
                    bool adicionarComprovanteColeta = false;

                    if (preTrip && (configuracaoTrizy?.ExigirEnvioFotosDasNotasNaOrigemPreTrip ?? false))
                        adicionarComprovanteColeta = true;
                    if (((preTrip && (configuracaoTrizy?.EnviarEstouIndoColetaPreTrip ?? false)) || (!preTrip && (configuracaoTrizy?.EnviarEstouIndoColeta ?? false))) && (tipoPonto == "COLLECT" || tipoPonto == "LOAD"))
                        adicionarEventoEstouIndo = true;
                    if (((!preTrip && (configuracaoTrizy?.EnviarInicioViagemColeta ?? false)) || (preTrip && (configuracaoTrizy?.EnviarIniciarViagemColetaPreTrip ?? false))) && (tipoPonto == "COLLECT" || tipoPonto == "LOAD"))
                        adicionarEventoIniciarViagem = true;
                    if (enviarPrimeiraColetaComoOrigin)
                        adicionarEventoConfirmar = true;
                    if (adicionarCheguei)
                        adicionarEventoChegueiParaColetar = true;

                    if (adicionarEventoEstouIndo)
                        obj.events.items.Add(ObterEventCargaAPP("CUSTOM", "Estou Indo", dataPrevisaoEstouIndo, true, "CREATED", TipoCustomEventAppTrizy.EstouIndo));
                    if (adicionarEventoIniciarViagem)
                        obj.events.items.Add(ObterEventCargaAPP("START_TRAVEL", "Iniciar viagem", dataPrevisao, enviarEventoIniciarViagemComoObrigatorio, "CREATED"));
                    if (adicionarEventoChegueiParaColetar)
                        obj.events.items.Add(ObterEventCargaAPP("START_OPERATION", "Cheguei para carregar", dataPrevisao, true, "CREATED"));
                    if (adicionarComprovanteColeta || exigirInformarNumeroPacotesNaColeta)
                        obj.events.items.Add(ObterEventCargaAPP("DELIVERY_RECEIPT", "Comprovante de coleta", dataPrevisao, true, "CREATED"));
                    if (adicionarEventoConfirmar)
                        obj.events.items.Add(ObterEventCargaAPP("END_OPERATION", "Confirmar coleta", dataPrevisao, true, "CREATED"));
                }
            }

            obj.contacts = ObterContatosInformacoesEntregaa(carga, ponto, produtos, pedidos, clienteComplementar);
            obj.additionalInformation = ObterInformacoesAdicionaisEntrega(carga, ponto, produtos, pedidos, clienteComplementar, canhotosDaEntrega);
            return obj;
        }

        private static List<LoadDetailInputs> TransformarPerguntasChecklist(Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checklist)
        {
            List<LoadDetailInputs> inputs = new List<LoadDetailInputs>();

            if (checklist == null)
                return inputs;

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes pergunta in checklist?.Perguntas)
            {
                LoadDetailInput input = new LoadDetailInput
                {
                    _id = ObjectId.GenerateNewId().ToString(),
                    required = pergunta.Obrigatorio,
                    externalInfo = new InformacaoExterna
                    {
                        id = pergunta.Codigo.ToString()
                    },
                    label = GerarTextoInternacionalizado(pergunta.Descricao),
                };
                switch (pergunta.Tipo)
                {
                    case TipoOpcaoCheckList.Aprovacao:
                    case TipoOpcaoCheckList.Informativo:
                        if (pergunta.TipoDecimal)
                        {
                            input.type = "NUMBER";
                            input.maximumFractionDigits = 2;
                        }
                        else if (pergunta.TipoData)
                        {
                            input.type = "DATE";
                        }
                        else if (pergunta.TipoHora)
                        {
                            input.type = "TIME";
                        }
                        else
                        {
                            input.type = "TEXT";
                        }
                        break;
                    case TipoOpcaoCheckList.SimNao:
                        input.type = "RADIO_GROUP";
                        input.options = new List<LabelValue>
                        {
                            new LabelValue
                            {
                                label = GerarTextoInternacionalizado("Sim"),
                                value = "SIM"
                            },
                            new LabelValue
                            {
                                label = GerarTextoInternacionalizado("Não"),
                                value = "NAO"
                            }
                        };
                        break;
                    case TipoOpcaoCheckList.Selecoes:
                    case TipoOpcaoCheckList.Escala:
                        input.type = "RADIO_GROUP";
                        break;
                    case TipoOpcaoCheckList.Opcoes:
                        input.type = "CHECKBOX";
                        break;
                    default:
                        break;
                }

                if (pergunta.Alternativas?.Any() == true)
                {
                    input.options = pergunta.Alternativas.OrderBy(a => a.Ordem).Select(a => new LabelValue { label = GerarTextoInternacionalizado(a.Descricao), value = a.CodigoIntegracao.ToString() }).ToList();
                }

                if (input.options?.Count() > 1 && pergunta.PermiteNaoAplica)
                {
                    input.options.Add(new LabelValue
                    {
                        label = GerarTextoInternacionalizado("Não se aplica"),
                        value = "N/A"
                    });
                }
                inputs.Add(new LoadDetailInputs { input = input });
            }

            return inputs;

        }
        private static List<ChecklistStep> ObterTemplateChecklistAppV3(List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaChecklistSuperAppEtapa, int codigoChecklist, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checklist, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga)
        {
            List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> etapasChecklist = listaChecklistSuperAppEtapa
                .Where(etapa => etapa.ChecklistSuperApp.Codigo == codigoChecklist)
                .ToList();

            List<ChecklistStep> listaChecklistStep = new List<ChecklistStep>();

            foreach (var etapa in etapasChecklist)
            {
                ChecklistStep step = new ChecklistStep();
                step.type = etapa.Tipo.ObterSuperAppType();
                bool temPropriedade = false;
                switch (etapa.TipoEvidencia)
                {
                    case TipoEvidenciaSuperApp.LocalizacaoCliente:
                        switch (cliente.TipoArea)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Raio:
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Ponto:
                                step.geofences = new List<ChecklistGeofence>()
                                    {
                                        new ChecklistGeofence
                                        {
                                          type = "Circle",
                                          radius = new Radius()
                                          {
                                            value = (cliente.RaioEmMetros.HasValue && cliente.RaioEmMetros.Value > 0)
                                                    ? cliente.RaioEmMetros.Value
                                                    : 50,
                                            unit = "m"
                                          },
                                          coordinates = new List<double>
                                          {
                                              double.Parse(cliente.Longitude, CultureInfo.InvariantCulture),
                                              double.Parse(cliente.Latitude, CultureInfo.InvariantCulture)
                                          },
                                          stopCondition = "ALLOWED",
                                        }
                                    };
                                step.type = "LOCATION";
                                temPropriedade = true;
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Poligono:
                                List<List<double>> coordenadas = new List<List<double>>();

                                using (JsonDocument doc = JsonDocument.Parse(cliente.Area))
                                {
                                    var root = doc.RootElement;

                                    foreach (var poligono in root.EnumerateArray())
                                    {
                                        if (poligono.TryGetProperty("paths", out var paths))
                                        {
                                            coordenadas.Clear(); // Zera para cada polígono

                                            foreach (var ponto in paths.EnumerateArray())
                                            {
                                                double lat = ponto.GetProperty("lat").GetDouble();
                                                double lng = ponto.GetProperty("lng").GetDouble();
                                                coordenadas.Add(new List<double> { lng, lat });
                                            }

                                            // Fecha o polígono: adiciona o primeiro ponto ao final se necessário
                                            if (coordenadas.Count > 0 &&
                                                (coordenadas[0][0] != coordenadas[coordenadas.Count - 1][0] || coordenadas[0][1] != coordenadas[coordenadas.Count - 1][1]))
                                            {
                                                coordenadas.Add(new List<double>(coordenadas[0]));
                                            }
                                            step.geofences = new List<ChecklistGeofence>()
                                                {
                                                    new ChecklistGeofence
                                                    {
                                                        type = "Polygon",
                                                        coordinates = new List<List<List<double>>> { coordenadas },
                                                        stopCondition = "ALLOWED"
                                                    }
                                                };
                                        }
                                    }
                                }
                                step.type = "LOCATION";
                                temPropriedade = true;
                                break;
                            default:
                                break;
                        }
                        break;
                    case TipoEvidenciaSuperApp.DetalheCarga:
                        step.indicators = new List<LoadDetailIndicator>()
                                {
                                    new LoadDetailIndicator()
                                    {
                                        title = GerarTextoInternacionalizado("Indicadores"),
                                        data = new List<LoadDetailIndicatorData>()
                                        {
                                            new LoadDetailIndicatorData()
                                            {
                                                type = "FIXED",
                                                label = GerarTextoInternacionalizado("Total"),
                                                value = modeloVeicularCarga.CapacidadePesoTransporte.ToString(),
                                            },
                                            new LoadDetailIndicatorData()
                                            {
                                                type = "DYNAMIC",
                                                label = GerarTextoInternacionalizado("Total Coletado"),
                                                value = "TOTAL",
                                            }
                                        }
                                    }
                                };
                        step.inputs = checklist != null ? TransformarPerguntasChecklist(checklist) : null;
                        temPropriedade = true;
                        break;
                    default:
                        break;
                }
                if (temPropriedade)
                {
                    step.step = etapa.IdSuperApp;
                    listaChecklistStep.Add(step);
                }
            }

            return listaChecklistStep;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPoint ObterPontoParadaCargaAPPV3(Dominio.ObjetosDeValor.Endereco endereco, Dominio.Entidades.Cliente ponto, string tipoPonto, string dataPrevisao, bool origem, bool preTrip, bool enviarDocumentos, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy, ref bool temItems, List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaEntrega = null, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = null, Dominio.Entidades.Embarcador.Cargas.Carga carga = null, bool coleta = false, Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy = null, bool destination = false, bool adicionarInicioViagemPrimeiraEntrega = false, string dataPrevisaoEstouIndo = "", List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = null, int codigoCargaEntrega = 0, TipoDocumentoPaisTrizy tipoDocumentoPaisTrizy = TipoDocumentoPaisTrizy.Brasil, Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar clienteComplementar = null, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> eventosSuperApp = null, List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaChecklistSuperAppEtapa = null, Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checklistColeta = null, Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checklistEntrega = null, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = null, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> produtosDaEntrega = null, bool devolucaoPorPeso = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPoint obj = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPoint();

            if (carga == null)
                return obj;
            // tratar depois
            // int quantidadeCanhotos = (ponto?.ClienteDescargas?.FirstOrDefault()?.PossuiCanhotoDeDuasOuMaisPaginas ?? false) ? (ponto?.ClienteDescargas?.FirstOrDefault()?.QuantidadeDePaginasDoCanhoto ?? 1) : 1;
            bool destinationPreTrip = destination && preTrip;
            bool naoEnviarDocumentosFiscais = (carga.TipoOperacao?.ConfiguracaoTrizy?.NaoEnviarDocumentosFiscais ?? false);
            bool enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos = carga.TipoOperacao?.ConfiguracaoTrizy?.EnviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos ?? false;
            bool naoEnviarEventosNaOrigem = carga.TipoOperacao?.ConfiguracaoTrizy?.NaoEnviarEventosNaOrigem ?? false;

            var checklist = tipoPonto == "DELIVER" ? checklistEntrega : checklistColeta;

            obj.operation = tipoPonto;
            obj.expectedAt = dataPrevisao;

            if (codigoCargaEntrega > 0)
                obj.externalId = codigoCargaEntrega.ToString() + ";" + ponto.CPF_CNPJ;

            obj.point = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Point()
            {
                location = ObterLocationCargaAPPV3("Point", endereco.Longitude.ToDecimal(), endereco.Latitude.ToDecimal()),
                address = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Address()
                {
                    countryCode = "BRA",
                    country = "Brasil",
                    stateCode = endereco.Cidade.Estado.Sigla,
                    state = endereco.Cidade.Estado.Descricao,
                    city = endereco.Cidade.Descricao,
                    district = endereco.Bairro,
                    street = endereco.Logradouro,
                    postalCode = endereco.CEP,
                    houseNumber = endereco.Numero
                }
            };

            obj.clients = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPointClient>();
            string tipoDocumento;
            if (ponto.Tipo == "E")
            {
                tipoDocumentoPaisTrizy = TipoDocumentoPaisTrizy.Estrangeiro;
                tipoDocumento = tipoDocumentoPaisTrizy.ObterPersonDocumentType();
            }
            else if (ponto.Tipo == "F")
                tipoDocumento = tipoDocumentoPaisTrizy.ObterPersonDocumentType();
            else
                tipoDocumento = tipoDocumentoPaisTrizy.ObterLegalPersonDocumentType();
            obj.clients.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPointClient()
            {
                company = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Company()
                {
                    name = integracaoTrizy.EnviarNomeFantasiaQuandoPossuir && !string.IsNullOrWhiteSpace(ponto.NomeFantasia)
                ? ponto.NomeFantasia
                : ponto.Nome,
                    document = ObterDocumentCargaAPPV3(tipoDocumento, ponto.CPF_CNPJ_SemFormato.ToString().ToLowerInvariant())
                }
            });

            obj.contacts = ObterContatosInformacoesEntregaa(carga, ponto, produtos, pedidos, clienteComplementar);
            obj.additionalInformation = ObterInformacoesAdicionaisEntrega(carga, ponto, produtos, pedidos, clienteComplementar, canhotosDaEntrega);

            obj.features = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPointFeatures();

            TipoParadaEventoSuperApp[] tiposParadaPermitidos = new[] { tipoPonto == "DELIVER" ? TipoParadaEventoSuperApp.Entrega : TipoParadaEventoSuperApp.Coleta, TipoParadaEventoSuperApp.Ambos };
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> eventosOperacao = eventosSuperApp.Where(evento => tiposParadaPermitidos.Contains(evento.EventoSuperApp.TipoParada)).ToList();

            if (tipoPonto == "LOAD")
            {
                eventosOperacao = eventosOperacao.Where(evento => evento.EventoSuperApp.Tipo == TipoEventoSuperApp.InicioDeViagem).ToList();
            }

            ChecklistSuperApp checklistDriverReceipt = new ChecklistSuperApp();

            if (eventosOperacao.Count >= 1 && !(origem && naoEnviarEventosNaOrigem))
            {
                obj.events = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Event();
                obj.events.flow = "SEQUENTIAL";
                obj.events.hasDeliveryReceipt = eventosOperacao.Any(evento => evento.EventoSuperApp.Tipo == TipoEventoSuperApp.EvidenciasDaEntrega);
                obj.events.items = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EventItem>();
                eventosOperacao.Sort((a, b) => a.EventoSuperApp.Ordem.CompareTo(b.EventoSuperApp.Ordem));


                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp evento in eventosOperacao)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EventItem eventItem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EventItem();

                    eventItem.type = evento.EventoSuperApp.Tipo.ObterTypeSuperApp();
                    eventItem.title = evento.EventoSuperApp.Titulo;
                    eventItem.required = evento.EventoSuperApp.Obrigatorio;
                    eventItem.externalId = evento.EventoSuperApp.TipoEventoCustomizado > 0 ? $"{evento.EventoSuperApp.TipoEventoCustomizado}" : null;
                    eventItem.checklist = evento.EventoSuperApp.Tipo != TipoEventoSuperApp.EvidenciasDaEntrega ? evento.EventoSuperApp.ChecklistSuperApp?.IdSuperApp : null;
                    eventItem.expectedAt = evento.EventoSuperApp.TipoEventoCustomizado == TipoCustomEventAppTrizy.EstouIndo ? dataPrevisaoEstouIndo : dataPrevisao;

                    if (!string.IsNullOrEmpty(eventItem.checklist) && evento.EventoSuperApp.ChecklistSuperApp?.Codigo > 0)
                    {
                        List<ChecklistStep> template = ObterTemplateChecklistAppV3(listaChecklistSuperAppEtapa, evento.EventoSuperApp.ChecklistSuperApp.Codigo, ponto, checklist, modeloVeicular);
                        if (template.Count > 0)
                        {
                            eventItem.template = template;
                        }
                    }

                    obj.events.items.Add(eventItem);

                    if (evento.EventoSuperApp.Tipo == TipoEventoSuperApp.EvidenciasDaEntrega && evento.EventoSuperApp.ChecklistSuperApp != null)
                    {
                        checklistDriverReceipt = evento.EventoSuperApp.ChecklistSuperApp;
                    }
                }
            }

            if (!destinationPreTrip && enviarDocumentos && canhotosDaEntrega != null && canhotosDaEntrega.Count > 0 && !naoEnviarDocumentosFiscais)
                obj.documents = ObterDocumentsCargaAPPV3(canhotosDaEntrega, enviarDocumentos, checklistDriverReceipt, ponto, listaChecklistSuperAppEtapa, checklist, modeloVeicular, produtosDaEntrega, devolucaoPorPeso, enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos, ref temItems, carga?.TipoOperacao?.ConfiguracaoTrizy?.HabilitarDevolucao ?? false, carga?.TipoOperacao?.ConfiguracaoTrizy?.HabilitarDevolucaoParcial ?? false);
            else if (checklistDriverReceipt != null && checklistDriverReceipt.Codigo != 0 && (canhotosDaEntrega == null || canhotosDaEntrega?.Count == 0))
            {
                obj.features.driverReceipt = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.FeatureDriverReceipt()
                {
                    enabled = true,
                    checklist = checklistDriverReceipt.IdSuperApp
                };

                List<ChecklistStep> templateDriverReceipt = ObterTemplateChecklistAppV3(listaChecklistSuperAppEtapa, checklistDriverReceipt.Codigo, ponto, checklist, modeloVeicular);
                if (templateDriverReceipt.Count > 0)
                {
                    obj.features.driverReceipt.template = templateDriverReceipt;
                }

            }

            // caso tenha evento de comprovação e não tenha documento, precisa tirar o evento.
            if (obj.events != null && (obj.documents == null || obj.documents?.Count == 0) && checklistDriverReceipt != null)
            {
                obj.events.items.RemoveAll(item => item.type == "DELIVERY_RECEIPT");
                obj.events.hasDeliveryReceipt = false;
            }

            // Caso não tenha evento de comprovação de entrega, remove os documentos.
            if (obj.events == null || !obj.events.items.Exists(item => item.type == "DELIVERY_RECEIPT"))
                obj.documents = null;

            if (!destinationPreTrip && (produtos != null && produtos.Count > 0))
                obj.products = ObterProductsCargaAPPV3(produtos, enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos);

            return obj;
        }

        private static List<Contact> ObterContatosInformacoesEntregaa(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente cliente, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, ClienteComplementar clienteComplementar)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ContatosTrizy infosContatos = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ContatosTrizy();
            infosContatos.contacts = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Contact>();

            Dominio.Entidades.Pais pais = cliente?.Localidade?.Pais;

            bool enviarContatosInformacoesEntrega = carga.TipoOperacao?.ConfiguracaoTrizy?.EnviarContatoInformacoesEntrega ?? false;
            List<EnumContatosInformacoesEntregaTrizy> contatosInformacoesAdicionais = carga.TipoOperacao?.ConfiguracaoTrizy?.ContatosInformacoesEntrega.ToList();

            if (!enviarContatosInformacoesEntrega || contatosInformacoesAdicionais == null || contatosInformacoesAdicionais.Count == 0 || pais?.Abreviacao != "BR") return infosContatos.contacts;

            if (contatosInformacoesAdicionais.Exists(info => info == EnumContatosInformacoesEntregaTrizy.TelefoneDoCliente))
            {
                string telefoneCliente = cliente.Telefone1;

                if (string.IsNullOrWhiteSpace(telefoneCliente))
                    telefoneCliente = cliente.Enderecos?.FirstOrDefault(x => !string.IsNullOrEmpty(x.Telefone))?.Telefone;

                if (!string.IsNullOrWhiteSpace(telefoneCliente))
                {
                    string telefoneCelular = ObterNumeroCelularCompleto(telefoneCliente, pais?.CodigoTelefonico.ToString());

                    infosContatos.contacts.AddRange(CriarInfosContatos("WHATSAPP", telefoneCelular, Localization.Resources.Pedidos.TipoOperacao.TelefoneDoCliente, telefoneCliente.ObterTelefoneFormatado()));
                }
            }

            if (contatosInformacoesAdicionais.Exists(info => info == EnumContatosInformacoesEntregaTrizy.TelefoneTorre) && !string.IsNullOrWhiteSpace(cliente.Telefone2))
            {
                string telefone = ObterNumeroCelularCompleto(cliente.Telefone2, pais?.CodigoTelefonico.ToString());

                infosContatos.contacts.AddRange(CriarInfosContatos("WHATSAPP", telefone, Localization.Resources.Pedidos.TipoOperacao.TelefoneTorre, cliente.Telefone2.ObterTelefoneFormatado()));
            }

            return infosContatos.contacts;
        }

        private static string ObterNumeroCelularCompleto(string telefone, string codigoPais)
        {

            if (string.IsNullOrWhiteSpace(telefone))
                return string.Empty;

            string celular = Utilidades.String.OnlyNumbers(telefone ?? string.Empty);

            if (string.IsNullOrWhiteSpace(celular))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(codigoPais))
                return celular;

            return $"+{codigoPais}{celular}";

        }

        private static List<Contact> CriarInfosContatos(string contactType, string contactValue, string contactLabel, string description)
        {
            ContactItem contactItem = new ContactItem
            {
                type = contactType,
                value = contactValue,
                label = contactLabel
            };

            Contact contact = new Contact
            {
                items = new List<ContactItem> { contactItem },
                description = description,
                label = contactLabel
            };

            ContatosTrizy contactsRoot = new ContatosTrizy
            {
                contacts = new List<Contact> { contact }
            };

            return contactsRoot.contacts;
        }

        private static List<dynamic> ObterDocumentsCargaAPP(List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaEntrega, bool enviarDocumentos, bool identificarNotaPallet, bool converterCanhotoParaPretoeBrancoERotacionarAutomaticamente, bool enviarMascaraFixaParaoCanhoto, bool enviarMascaraDinamicaParaoCanhoto, bool naoPermitirVincularFotosDaGaleriaParaCanhotos)
        {
            List<dynamic> documents = new List<dynamic>();

            if (canhotosDaEntrega != null && canhotosDaEntrega.Count > 0 && enviarDocumentos)
            {
                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotosDaEntrega)
                {
                    dynamic documento = new ExpandoObject();
                    documento.type = "NF-E";
                    if (canhoto.TipoCanhoto == TipoCanhoto.Avulso)
                    {
                        documento.key = "CA_" + canhoto.Codigo;
                        documento.identifier = canhoto.Numero.ToString();
                    }
                    else if (canhoto.TipoCanhoto == TipoCanhoto.NFe)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nf = canhoto.XMLNotaFiscal;
                        documento.key = nf.Chave;
                        documento.identifier = nf.Numero.ToString();
                        dynamic evidencias = (identificarNotaPallet && nf.TipoNotaFiscalIntegrada.HasValue && nf.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet) ?
                            ObterEvidenciasDocumento(converterCanhotoParaPretoeBrancoERotacionarAutomaticamente, enviarMascaraFixaParaoCanhoto, enviarMascaraDinamicaParaoCanhoto, naoPermitirVincularFotosDaGaleriaParaCanhotos) :
                            null;
                        if (evidencias != null)
                            documento.evidences = evidencias;
                    }
                    documento.externalId = canhoto.Codigo.ToString();

                    documents.Add(documento);
                }
            }
            else if (!enviarDocumentos)
            {
                documents.Add(new
                {
                    type = "NF-E",
                    key = string.Empty.PadLeft(44, '0'),
                    identifier = "00000000",
                    externalId = "0"
                });
            }

            return documents;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPointDocument> ObterDocumentsCargaAPPV3(List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaEntrega, bool enviarDocumentos, ChecklistSuperApp checklistDriverReceipt, Dominio.Entidades.Cliente cliente, List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaChecklistSuperAppEtapa, Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checklist, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> produtosDaEntrega, bool devolucaoPorPeso, bool enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos, ref bool temItems, bool habilitarDevolucaoTotal, bool habilitarDevolucaoParcial)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPointDocument> documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPointDocument>();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPointDocumentFeatures documentFeatures = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPointDocumentFeatures();

            if (checklistDriverReceipt != null)
            {
                documentFeatures.driverReceipt = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.FeatureDriverReceipt()
                {
                    enabled = true,
                    checklist = checklistDriverReceipt.IdSuperApp
                };

                List<ChecklistStep> template = ObterTemplateChecklistAppV3(listaChecklistSuperAppEtapa, checklistDriverReceipt.Codigo, cliente, checklist, modeloVeicular);
                if (template.Count > 0)
                {
                    documentFeatures.driverReceipt.template = template;
                }
            }

            if (canhotosDaEntrega != null && canhotosDaEntrega.Count > 0 && enviarDocumentos)
            {
                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotosDaEntrega)
                {
                    dynamic documento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPointDocument();
                    documento.type = "NF-E";
                    if (canhoto.TipoCanhoto == TipoCanhoto.Avulso)
                    {
                        documento.key = "CA_" + canhoto.Codigo;
                        documento.identifier = canhoto.Numero.ToString();
                    }
                    else if (canhoto.TipoCanhoto == TipoCanhoto.NFe)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nf = canhoto.XMLNotaFiscal;
                        documento.key = nf.Chave;
                        documento.identifier = nf.Numero.ToString();
                    }
                    documento.externalId = canhoto.Codigo.ToString();
                    documento.features = documentFeatures;
                    if (habilitarDevolucaoParcial)
                    {
                        documento.items = ObterProdutosPorNotaV3(produtosDaEntrega.Where(produto => produto.XMLNotaFiscal?.Codigo == canhoto.XMLNotaFiscal.Codigo).ToList(), devolucaoPorPeso, enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos);
                        if (documento.items?.Count > 0)
                        {
                            temItems = true;
                        }
                    }
                    documents.Add(documento);
                }
            }
            else if (!enviarDocumentos)
            {
                documents.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.StoppingPointDocument()
                {
                    type = "NF-E",
                    key = string.Empty.PadLeft(44, '0'),
                    identifier = "00000000",
                    externalId = "0",
                    features = documentFeatures
                });
            }

            return documents;
        }

        private static List<dynamic> ObterProductsCargaAPP(List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos, bool enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos)
        {
            List<dynamic> products = new List<dynamic>();

            if (produtos != null && produtos.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto in produtos)
                {
                    dynamic product = new ExpandoObject();
                    if (enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos)
                    {
                        decimal somaQuantidade = produto.Quantidade;
                        somaQuantidade = somaQuantidade < 0 ? 0 : somaQuantidade;

                        product.label = produto.Produto.Descricao + " - (CX)";
                        product.value = new { value = Math.Round(somaQuantidade, 0).ToString() };
                    }
                    else
                    {
                        decimal somaQuantidade = produto.Quantidade * produto.PesoUnitario;
                        somaQuantidade = somaQuantidade < 0 ? 0 : somaQuantidade;

                        product.label = produto.Produto.Descricao;
                        product.value = new { value = Math.Round(somaQuantidade, 2), unit = "kg" };
                    }

                    products.Add(product);
                }
            }

            return products;
        }
        private static List<StoppingPointDocumentItem> ObterProdutosPorNotaV3(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> produtosDaEntrega, bool devolucaoPorPeso, bool enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos)
        {
            List<StoppingPointDocumentItem> items = new List<StoppingPointDocumentItem>();

            if (produtosDaEntrega != null && produtosDaEntrega.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto produto in produtosDaEntrega)
                {
                    decimal quantidade = devolucaoPorPeso && !enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos ? produto.Quantidade * produto.PesoUnitario : produto.Quantidade;
                    string unidade =
                        !string.IsNullOrEmpty(produto.Produto?.UnidadeMedidaSuperApp)
                            ? produto.Produto.UnidadeMedidaSuperApp
                            : !string.IsNullOrEmpty(produto.Produto?.Unidade?.UnidadeMedidaSuperApp)
                                ? produto.Produto.Unidade.UnidadeMedidaSuperApp
                                : null;

                    StoppingPointDocumentItem item = new StoppingPointDocumentItem
                    {
                        label = GerarTextoInternacionalizado(!enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos ? produto.Produto.Descricao : produto.Produto.Descricao + " - (CX)"),
                        externalId = produto.Codigo.ToString(),
                        value = new DadosProduto
                        {
                            value = Math.Round(Math.Max(quantidade, 0), 2),
                            unit = unidade,
                        }
                    };

                    items.Add(item);
                }
            }

            return items;
        }
        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Produto> ObterProductsCargaAPPV3(List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos, bool enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Produto> products = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Produto>();

            if (produtos != null && produtos.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto in produtos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Produto product = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Produto();
                    if (enviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos)
                    {
                        decimal somaQuantidade = produto.Quantidade;
                        somaQuantidade = somaQuantidade < 0 ? 0 : somaQuantidade;

                        product.label = produto.Produto.Descricao + " - (CX)";
                        product.value = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.DadosProduto()
                        {
                            value = Math.Round(somaQuantidade, 0)
                        };
                    }
                    else
                    {
                        decimal somaQuantidade = produto.Quantidade * produto.PesoUnitario;
                        somaQuantidade = somaQuantidade < 0 ? 0 : somaQuantidade;

                        product.label = produto.Produto.Descricao;
                        product.value = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.DadosProduto()
                        {
                            value = Math.Round(somaQuantidade, 2),
                            unit = "kg"
                        };
                    }

                    products.Add(product);
                }
            }

            return products;
        }

        private static List<dynamic> ObterEvidencesCargaAPP(bool naoEnviarTagValidacao, bool solicitarAssinaturaNaConfirmacaoDeColetaEntrega, bool SolicitarFotoComoEvidenciaObrigatoria, bool SolicitarFotoComoEvidenciaOpcional, bool enviarEvidenciaDePacotes, int quantidadeCanhotos, bool solicitarComprovanteColetaEntrega, bool enviarNomeRecebedorConfirmacao, bool enviarDocumentoConfirmacao, bool converterCanhotoParaPretoeBrancoERotacionarAutomaticamente, bool enviarMascaraFixaParaoCanhoto, bool enviarMascaraDinamicaParaoCanhoto, bool naoPermitirVincularFotosDaGaleriaParaCanhotos, bool solicitarApenasCanhotos)
        {
            /********************************************************************************************************************************************************************************************************/
            /* Ao adicionar nova evidencia, verificar necessidade de validação de recebimento via webhook na função validarObrigatoriedadeDeEvidencias (Servicos.Embarcador.SuperApp.Eventos.DeliveryReceiptCreate);*/
            /********************************************************************************************************************************************************************************************************/
            List<dynamic> evidences = new List<dynamic>();
            evidences.Add(new
            {
                flow = "SEQUENTIAL",
                items = new List<dynamic>() { }
            });

            #region Definição de Evidencias
            dynamic evFotoCanhotoComValidacao = new ExpandoObject();
            evFotoCanhotoComValidacao.title = new { pt = "Foto", en = "Photo", es = "Foto" };
            evFotoCanhotoComValidacao.subtitle = new { pt = "Foto", en = "Photo", es = "Foto" };
            evFotoCanhotoComValidacao.value = new List<string>();
            evFotoCanhotoComValidacao.step = "PHOTO";
            evFotoCanhotoComValidacao.galleryPhoto = !naoPermitirVincularFotosDaGaleriaParaCanhotos;
            evFotoCanhotoComValidacao.limitPhoto = 1;
            evFotoCanhotoComValidacao.required = true;
            evFotoCanhotoComValidacao.validator = new { type = "NF-E" };
            evFotoCanhotoComValidacao.imageProcessing = enviarMascaraDinamicaParaoCanhoto;
            evFotoCanhotoComValidacao.imageGreyScale = converterCanhotoParaPretoeBrancoERotacionarAutomaticamente;
            evFotoCanhotoComValidacao.externalId = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.FotoCanhotoComValidacao.ToString();
            if (enviarMascaraFixaParaoCanhoto)
                evFotoCanhotoComValidacao.imageProportion = new { height = 60, width = 9 };

            dynamic evFotoCanhotoSemValidacao = new ExpandoObject();
            evFotoCanhotoSemValidacao.title = new { pt = "Foto", en = "Photo", es = "Foto" };
            evFotoCanhotoSemValidacao.subtitle = new { pt = "Foto", en = "Foto", es = "Foto" };
            evFotoCanhotoSemValidacao.value = new List<string>();
            evFotoCanhotoSemValidacao.step = "PHOTO";
            evFotoCanhotoSemValidacao.galleryPhoto = !naoPermitirVincularFotosDaGaleriaParaCanhotos;
            evFotoCanhotoSemValidacao.limitPhoto = quantidadeCanhotos;
            evFotoCanhotoSemValidacao.required = false;
            evFotoCanhotoSemValidacao.externalId = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.FotoCanhotoSemValidacao.ToString();

            dynamic evFotoAdicional = new ExpandoObject();
            evFotoAdicional.title = new { pt = "Foto", en = "Photo", es = "Foto" };
            evFotoAdicional.subtitle = new { pt = "Foto", en = "Foto", es = "Foto" };
            evFotoAdicional.value = new List<string>();
            evFotoAdicional.step = "PHOTO";
            evFotoAdicional.galleryPhoto = !naoPermitirVincularFotosDaGaleriaParaCanhotos;
            evFotoAdicional.limitPhoto = 99;
            evFotoAdicional.required = SolicitarFotoComoEvidenciaObrigatoria;
            evFotoAdicional.externalId = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.FotoAdicional.ToString();

            dynamic evFotoCanhotoComentario = new ExpandoObject();
            evFotoCanhotoComentario.title = new { pt = "Adicione um comentário", en = "Add a comment", es = "Añadir un comentario" };
            evFotoCanhotoComentario.subtitle = new { pt = "Comentário sobre a foto enviada", en = "Add a comment", es = "Añadir un comentario" };
            evFotoCanhotoComentario.value = new List<string>();
            evFotoCanhotoComentario.step = "TEXT";
            evFotoCanhotoComentario.required = false;
            evFotoCanhotoComentario.externalId = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.FotoCanhotoComentario.ToString();

            dynamic evAssinaturaConfirmacao = new ExpandoObject();
            evAssinaturaConfirmacao.title = new { pt = "Adicione a Assinatura do Recebedor", en = "Add the Recipient's Signature", es = "Añadir la Firma del Receptor" };
            evAssinaturaConfirmacao.subtitle = new { pt = "Assinatura do Recebedor", en = "Recipient's Signature", es = "Añadir la Firma del Receptor" };
            evAssinaturaConfirmacao.value = new List<string>();
            evAssinaturaConfirmacao.step = "SIGNATURE";
            evAssinaturaConfirmacao.required = true;
            evAssinaturaConfirmacao.externalId = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.AssinaturaConfirmacao.ToString();

            dynamic evSolicitacaoPacotesColeta = new ExpandoObject();
            evSolicitacaoPacotesColeta.title = new { pt = "Adicione a quantidade de pacotes", en = "Add a ammount of packages", es = "Añadir una quantidad" };
            evSolicitacaoPacotesColeta.subtitle = new { pt = "N°", en = "N°", es = "N°" };
            evSolicitacaoPacotesColeta.value = new List<string>();
            evSolicitacaoPacotesColeta.step = "NUMBER";
            evSolicitacaoPacotesColeta.required = true;
            evSolicitacaoPacotesColeta.externalId = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.SolicitacaoPacotesColeta.ToString();

            dynamic evNomeRecebedorConfirmacao = new ExpandoObject();
            evNomeRecebedorConfirmacao.title = new { pt = "Nome do Recebedor", en = "Recipient's Name", es = "Nombre del Receptor" };
            evNomeRecebedorConfirmacao.subtitle = new { pt = "Adicione Nome e Sobrenome do recebedor", en = "Add the Recipient's First and Last Name", es = "Añadir Nombre y Apellido del Receptor" };
            evNomeRecebedorConfirmacao.value = new List<string>();
            evNomeRecebedorConfirmacao.step = "TEXT";
            evNomeRecebedorConfirmacao.required = true;
            evNomeRecebedorConfirmacao.externalId = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.NomeRecebedorConfirmacao.ToString();

            dynamic evDocumentoConfirmacao = new ExpandoObject();
            evDocumentoConfirmacao.title = new { pt = "Adicione o número do Documento do Recebedor", en = "Add the Recipient's Document Number", es = "Añadir el número del Documento del Receptor" };
            evDocumentoConfirmacao.subtitle = new { pt = "Adicione o número do Documento do Recebedor", en = "Add the Recipient's Document Number", es = "Añadir el número del Documento del Receptor" };
            evDocumentoConfirmacao.value = new List<string>();
            evDocumentoConfirmacao.step = "NUMBER";
            evDocumentoConfirmacao.required = true;
            evDocumentoConfirmacao.externalId = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.DocumentoConfirmacao.ToString();
            #endregion

            #region Validação para Inserir Evidencias
            bool temFoto = false;

            if (solicitarApenasCanhotos)
                evidences[0].items.Add(evFotoCanhotoComValidacao);
            else
            {
                if (solicitarComprovanteColetaEntrega)
                {
                    if (naoEnviarTagValidacao)
                        evidences[0].items.Add(evFotoCanhotoSemValidacao);
                    else
                        evidences[0].items.Add(evFotoCanhotoComValidacao);
                    temFoto = true;
                }

                if (SolicitarFotoComoEvidenciaObrigatoria || SolicitarFotoComoEvidenciaOpcional)
                {
                    evidences[0].items.Add(evFotoAdicional);
                    temFoto = true;
                }

                if (temFoto)
                    evidences[0].items.Add(evFotoCanhotoComentario);

                if (solicitarAssinaturaNaConfirmacaoDeColetaEntrega)
                    evidences[0].items.Add(evAssinaturaConfirmacao);

                if (enviarEvidenciaDePacotes)
                    evidences[0].items.Add(evSolicitacaoPacotesColeta);

                if (enviarNomeRecebedorConfirmacao)
                    evidences[0].items.Add(evNomeRecebedorConfirmacao);

                if (enviarDocumentoConfirmacao)
                    evidences[0].items.Add(evDocumentoConfirmacao);

                if (evidences[0].items.Count <= 0)
                    evidences[0].items.Add(evFotoCanhotoSemValidacao);
            }

            #endregion

            return evidences;
        }

        private static List<dynamic> ObterEvidenciasDocumento(bool converterCanhotoParaPretoeBrancoERotacionarAutomaticamente, bool enviarMascaraFixaParaoCanhoto, bool enviarMascaraDinamicaParaoCanhoto, bool naoPermitirVincularFotosDaGaleriaParaCanhotos)
        {
            List<dynamic> evidences = new List<dynamic>();
            evidences.Add(new
            {
                flow = "SEQUENTIAL",
                items = new List<dynamic>() { }
            });

            dynamic evFotoCanhotoPallets = new ExpandoObject();
            evFotoCanhotoPallets.title = new { pt = "FOTO DO CANHOTO DE PALLETS", en = "PHOTO OF THE PALLET STUB", es = "FOTO DEL RECIBO DE PALETAS" };
            evFotoCanhotoPallets.subtitle = new { pt = "Envie uma foto", en = "Send a photo", es = "Envía una foto" };
            evFotoCanhotoPallets.value = new List<string>();
            evFotoCanhotoPallets.step = "PHOTO";
            evFotoCanhotoPallets.galleryPhoto = !naoPermitirVincularFotosDaGaleriaParaCanhotos;
            evFotoCanhotoPallets.limitPhoto = 1;
            evFotoCanhotoPallets.required = true;
            evFotoCanhotoPallets.imageProcessing = enviarMascaraDinamicaParaoCanhoto;
            evFotoCanhotoPallets.imageGreyScale = converterCanhotoParaPretoeBrancoERotacionarAutomaticamente;
            evFotoCanhotoPallets.externalId = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.FotoCanhotoPallet.ToString();
            if (enviarMascaraFixaParaoCanhoto)
                evFotoCanhotoPallets.imageProportion = new { height = 60, width = 9 };

            dynamic evFotoCanhotoValePallet = new ExpandoObject();
            evFotoCanhotoValePallet.title = new { pt = "FOTO DO VALE PALLETS", en = "PHOTO OF THE PALLET VOUCHER", es = "FOTO DEL VALE DE PALETAS" };
            evFotoCanhotoValePallet.subtitle = new { pt = "Envie uma foto", en = "Send a photo", es = "Envía una foto" };
            evFotoCanhotoValePallet.value = new List<string>();
            evFotoCanhotoValePallet.step = "PHOTO";
            evFotoCanhotoValePallet.galleryPhoto = !naoPermitirVincularFotosDaGaleriaParaCanhotos;
            evFotoCanhotoValePallet.limitPhoto = 2;
            evFotoCanhotoValePallet.required = false;
            evFotoCanhotoValePallet.externalId = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.FotoCanhotoValePallet.ToString();

            evidences[0].items.Add(evFotoCanhotoPallets);
            evidences[0].items.Add(evFotoCanhotoValePallet);

            return evidences;
        }

        private static dynamic ObterCompanyCargaAPP(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao, TipoDocumentoPaisTrizy tipoDocumentoPaisTrizy)
        {
            dynamic company = new
            {
                document = ObterDocumentCargaAPP(tipoDocumentoPaisTrizy.ObterLegalPersonDocumentType(), !string.IsNullOrWhiteSpace(integracao.CNPJCompanyTrizy) ? integracao.CNPJCompanyTrizy : carga.Filial?.CNPJ ?? carga.Empresa?.CNPJ ?? string.Empty),
                name = carga.Filial?.Descricao ?? carga.Empresa?.Descricao ?? string.Empty
            };
            return company;
        }

        private static Company ObterCompanyCargaAPPV3(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao, TipoDocumentoPaisTrizy tipoDocumentoPaisTrizy)
        {
            Company company = new Company()
            {
                document = ObterDocumentCargaAPPV3(tipoDocumentoPaisTrizy.ObterLegalPersonDocumentType(), !string.IsNullOrWhiteSpace(integracao.CNPJCompanyTrizy) ? integracao.CNPJCompanyTrizy : carga.Filial?.CNPJ ?? carga.Empresa?.CNPJ ?? string.Empty),
                name = carga.Filial?.Descricao ?? carga.Empresa?.Descricao ?? string.Empty
            };
            return company;
        }

        private static dynamic[] ObterDriversCargaAPP(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoDocumentoPaisTrizy tipoDocumentoPaisTrizy)
        {
            List<dynamic> drivers = new List<dynamic>() {
                new {
                    document = ObterDocumentCargaAPP(tipoDocumentoPaisTrizy.ObterPersonDocumentType(), carga.Motoristas?.FirstOrDefault()?.CPF ?? string.Empty)
                }
            };
            return drivers.ToArray();
        }
        private static dynamic[] ObterDriversMDFeManual(List<Dominio.Entidades.Usuario> motoristas, TipoDocumentoPaisTrizy tipoDocumentoPaisTrizy)
        {
            List<dynamic> drivers = new List<dynamic>();

            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                drivers.Add(new
                {
                    document = ObterDocumentCargaAPP(tipoDocumentoPaisTrizy.ObterPersonDocumentType(), motorista.CPF ?? string.Empty)
                });
            }

            return drivers.ToArray();
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Driver> ObterDriversCargaAPPV3(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoDocumentoPaisTrizy tipoDocumentoPaisTrizy)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Driver> drivers = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Driver>();
            drivers.Add(
                new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Driver()
                {
                    document = ObterDocumentCargaAPPV3(tipoDocumentoPaisTrizy.ObterPersonDocumentType(), carga.Motoristas?.FirstOrDefault()?.CPF ?? string.Empty)
                }
            );
            return drivers;
        }

        private static dynamic ObterExternalCargaAPP(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy, bool preTrip)
        {
            dynamic external = new ExpandoObject();

            external.id = ((configuracaoTrizy?.EnviarPreTripJuntoAoNumeroCarga ?? false) && preTrip) ? $"{carga.CodigoCargaEmbarcador} (Pré Trip)" : carga.CodigoCargaEmbarcador;
            external.label = "Carga";
            external.tags = new List<string>() { clienteMultisoftware?.Codigo != null ? clienteMultisoftware.Codigo.ToString() : string.Empty };

            return external;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoExterna ObterExternalCargaAPPV3(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy, bool preTrip)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoExterna external = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoExterna();

            external.id = ((configuracaoTrizy?.EnviarPreTripJuntoAoNumeroCarga ?? false) && preTrip) ? $"{carga.CodigoCargaEmbarcador} (Pré Trip)" : carga.CodigoCargaEmbarcador;
            external.label = "Carga";
            external.tags = new List<string>() { clienteMultisoftware?.Codigo != null ? clienteMultisoftware.Codigo.ToString() : string.Empty };

            return external;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioOcorrencia ConverterObjetoEnvioOcorrencia(EventoIntegracaoOcorrenciaTrizy tipoEvento, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioOcorrencia request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioOcorrencia();

            request.status = tipoEvento == EventoIntegracaoOcorrenciaTrizy.AssumirAtendimento ? "UNDER_ANALYSIS" : "RESOLVED";
            ;
            request.responsible = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Responsavel()
            {
                name = usuario.Nome,
                message = tipoEvento.ObterDescricaoEnvioIntegracao(),
                sentAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            return request;

        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnviarMensagem ConverterObjetoEnviarMensagem(string mensagem, DateTime dataMensagem, string idViagem)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnviarMensagem request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnviarMensagem();

            request.content = mensagem;
            request.sentAt = dataMensagem.ToString("yyyy-MM-dd") + "T" + dataMensagem.ToString("HH:mm:ss") + ".000-03:00";

            return request;

        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAtualizarViagem ConverterObjetoAtualizarViagem(string status)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAtualizarViagem request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAtualizarViagem();

            request.status = status;

            return request;

        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.DocumentsPreTrip ConverterObjetoAtualizarViagemPretrip(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Documento> listaDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Documento>();

            if (notasFiscais != null && notasFiscais.Count > 0 && !(cargaEntrega.Carga?.TipoOperacao?.ConfiguracaoTrizy?.NaoEnviarDocumentosFiscais ?? false))
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDistintas = notasFiscais.DistinctBy(nf => nf.Chave).ToList();
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nf in notasFiscaisDistintas)
                {
                    listaDocumentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Documento()
                    {
                        type = "NF-E",
                        key = nf.Chave,
                        identifier = nf.Numero.ToString()
                    });
                }
            }
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.DocumentsPreTrip() { documents = listaDocumentos };
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAtualizarEntrega ConverterObjetoAtualizarEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, bool finalizarEntrega = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAtualizarEntrega request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAtualizarEntrega();

            request.expectedAt = cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
                : cargaEntrega.DataPrevista.HasValue ? cargaEntrega.DataPrevista.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") : cargaEntrega.Carga.DataCriacaoCarga.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");

            if (finalizarEntrega)
            {
                request.status = "COMPLETED";
            }

            return request;

        }
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAlternarBloqueioParada GerarObjetoEnvioAlternarBloqueioParada(bool bloquear)
        {
            if (bloquear)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAlternarBloqueioParada
                {
                    title = GerarTextoInternacionalizado("Atendimento em Aberto"),
                    description = GerarTextoInternacionalizado("Existe um atendimento em aberto para esse ponto. Aguarde a resolução.")
                };
            }
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioAlternarBloqueioParada
            {
                title = GerarTextoInternacionalizado(""),
                description = GerarTextoInternacionalizado("")
            };

        }
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioModeloVeicular GerarObjetoEnvioModeloVeicular(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> divisoes)
        {
            int numeroDeLinhas = Math.Max(modeloVeicularCarga.NumeroReboques ?? 1, 1);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioModeloVeicularSlot> slots = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioModeloVeicularSlot>();

            // aqui gera os subslots -> um por Piso/Coluna
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioModeloVeicularSubSlot> subSlots = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioModeloVeicularSubSlot>();

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> divisoesCapacidade =
                divisoes?.Count > 0
                    ? divisoes.ToList()
                    : new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade>
                    {
                        new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade
                        {
                            Codigo = 0,
                            Descricao = "Piso 1 Coluna 1",
                            ModeloVeicularCarga = modeloVeicularCarga,
                            Quantidade = modeloVeicularCarga.CapacidadePesoTransporte,
                            Piso = 1,
                            Coluna = 1
                        }
                    };

            foreach (var linha in divisoesCapacidade)
            {
                // cria o subslot para o Piso/Coluna da linha
                var subSlot = new EnvioModeloVeicularSubSlot
                {
                    label = GerarTextoInternacionalizado($"Piso {linha.Piso} - Coluna {linha.Coluna}"),
                    totalCapacity = linha.Quantidade,
                    placeHolder = GerarTextoInternacionalizado(linha.Descricao)
                };

                subSlots.Add(subSlot);
            }

            var totalCapacitySlot = subSlots.Sum(s => s.totalCapacity);

            for (int i = 0; i < numeroDeLinhas; i++)
            {

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioModeloVeicularSlot slot = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioModeloVeicularSlot
                {
                    title = GerarTextoInternacionalizado($"Reboque {i + 1}"),
                    totalCapacity = totalCapacitySlot,
                    subSlot = subSlots
                };

                slots.Add(slot);
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioModeloVeicular objetoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioModeloVeicular
            {
                description = GerarTextoInternacionalizado("Selecione a área do caminhão para preencher"),
                showImage = false,
                canLoadMoreThanCapacity = modeloVeicularCarga.ToleranciaPesoExtra > 0,
                slots = slots,
            };

            return objetoRequest;

        }
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioIntegrarMotivoDevolucaoEntrega GerarObjetoEnvioIntegrarMotivoDevolucaoEntrega(Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivoDevolucaoEntrega)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioIntegrarMotivoDevolucaoEntrega()
            {
                externalId = motivoDevolucaoEntrega.Codigo.ToString(),
                source = "DOCUMENT",
                label = GerarTextoInternacionalizado(motivoDevolucaoEntrega.Descricao),
                checklist = motivoDevolucaoEntrega.ChecklistSuperApp?.IdSuperApp,
            };
        }
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioTipoOcorrencia GerarObjetoTipoOcorrencia(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioTipoOcorrencia()
            {
                externalId = tipoOcorrencia.Codigo.ToString(),
                sources = new List<string>() { "STOPOVER" },
                label = GerarTextoInternacionalizado(tipoOcorrencia.Descricao),
                checklist = tipoOcorrencia.ChecklistSuperApp?.IdSuperApp,
            };
        }
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Checklist ConverterObjetoChecklist(Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp checklistSuperApp, List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> etapasChecklist)
        {

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Checklist request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Checklist();

            request.flow = checklistSuperApp.TipoFluxo.ObterDescricaoSuperapp();
            request.label = GerarTextoInternacionalizado(checklistSuperApp.Titulo);
            request.externalInfo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoExterna();
            request.externalInfo.id = $"{checklistSuperApp.Codigo}";

            request.steps = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistStep[etapasChecklist.Count];
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            etapasChecklist.Sort((x, y) => x.Ordem.CompareTo(y.Ordem));

            for (int i = 0; i < etapasChecklist.Count; i++)
            {
                Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa etapa = etapasChecklist[i];
                request.steps[i] = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistStep();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistConfiguracoes ConfiguracoesJson = !string.IsNullOrWhiteSpace(etapa.Configuracoes) ? JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistConfiguracoes>(etapa.Configuracoes, settings) : null;
                request.steps[i].type = etapa.Tipo.ObterSuperAppType();
                request.steps[i].required = etapa.Obrigatorio;
                request.steps[i].label = GerarTextoInternacionalizado(etapa.Titulo);
                request.steps[i].helperText = ConfiguracoesJson.HelperTextEtapaChecklist != null ? GerarTextoInternacionalizado(ConfiguracoesJson.HelperTextEtapaChecklist) : null;
                if (!string.IsNullOrWhiteSpace(ConfiguracoesJson.TipoAlertaEtapaChecklist))
                {
                    request.steps[i].alert = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistStepAlert();
                    request.steps[i].alert.description = GerarTextoInternacionalizado(ConfiguracoesJson.DescricaoAlertaEtapaChecklist);
                    request.steps[i].alert.title = !string.IsNullOrWhiteSpace(ConfiguracoesJson.TituloAlertaEtapaChecklist) ? GerarTextoInternacionalizado(ConfiguracoesJson.TituloAlertaEtapaChecklist) : null;
                    request.steps[i].alert.color = !string.IsNullOrWhiteSpace(ConfiguracoesJson.TipoAlertaEtapaChecklist) ? ConfiguracoesJson.TipoAlertaEtapaChecklist : null;
                }
                request.steps[i].placeholder = !string.IsNullOrWhiteSpace(ConfiguracoesJson.PlaceHolderEtapaChecklist) ? GerarTextoInternacionalizado(ConfiguracoesJson.PlaceHolderEtapaChecklist) : null;

                switch (etapa.Tipo)
                {
                    case TipoEtapaChecklistSuperApp.Text:
                        if (ConfiguracoesJson.ValidacaoAdicionalTexto != null)
                        {
                            ValidacaoAdicionalEtapaTextoSuperApp validacaoAdicional = (ValidacaoAdicionalEtapaTextoSuperApp)ConfiguracoesJson.ValidacaoAdicionalTexto;
                            request.steps[i].validation = validacaoAdicional.ObterSuperAppType();
                        }
                        break;
                    case TipoEtapaChecklistSuperApp.Number:
                        if (ConfiguracoesJson.ValorMinimoEtapaChecklist.HasValue || ConfiguracoesJson.ValorMaximoEtapaChecklist.HasValue)
                        {
                            request.steps[i].range = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistStepMinMax();
                            request.steps[i].range.min = ConfiguracoesJson.ValorMinimoEtapaChecklist;
                            request.steps[i].range.max = ConfiguracoesJson.ValorMaximoEtapaChecklist;
                        }
                        if (ConfiguracoesJson.UtilizarNumerosDecimais ?? false)
                        {
                            request.steps[i].minimumFractionDigits = 0;
                            request.steps[i].maximumFractionDigits = 2;
                        }
                        break;
                    case TipoEtapaChecklistSuperApp.Image:
                        if (ConfiguracoesJson.QuantidadeMinimaEtapaChecklist.HasValue || ConfiguracoesJson.QuantidadeMaximaEtapaChecklist.HasValue)
                        {
                            request.steps[i].limit = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistStepMinMax();
                            request.steps[i].limit.min = ConfiguracoesJson.QuantidadeMinimaEtapaChecklist;
                            request.steps[i].limit.max = ConfiguracoesJson.QuantidadeMaximaEtapaChecklist;
                        }
                        request.steps[i].galleryEnabled = ConfiguracoesJson.GaleriaHabilitadaEtapaChecklist;
                        request.steps[i].validatedMetadataSetting = new ChecklistValidatedMetadataSetting
                        {
                            showLogo = ConfiguracoesJson.MetadadosImagemMostrarLogo,
                            showDate = ConfiguracoesJson.MetadadosImagemMostrarData,
                            showTime = ConfiguracoesJson.MetadadosImagemMostrarHora,
                            showLocation = ConfiguracoesJson.MetadadosImagemMostrarLocalizacao,
                            showDriverName = ConfiguracoesJson.MetadadosImagemMostrarNomeMotorista
                        };
                        break;
                    case TipoEtapaChecklistSuperApp.ImageValidator:
                        request.steps[i].validator = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistStepValidator[1];
                        request.steps[i].validator[0] = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistStepValidator();
                        request.steps[i].validator[0].valueType = "DOCUMENT_IDENTIFIER";

                        if (!string.IsNullOrWhiteSpace(ConfiguracoesJson.TipoProcessamentoImagemEtapaChecklist))
                        {
                            request.steps[i].imageProcessing = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistStepImageProcessing();
                            request.steps[i].imageProcessing.enabled = true;
                            request.steps[i].imageProcessing.type = ConfiguracoesJson.TipoProcessamentoImagemEtapaChecklist;
                            request.steps[i].imageProcessing.threshold = ConfiguracoesJson.ThresholdEtapaChecklist;

                        }
                        if (etapa.Tipo == TipoEtapaChecklistSuperApp.ImageValidator && ConfiguracoesJson.ModoValidacaoImagemEtapaChecklist == "CAMERA" && ConfiguracoesJson.UtilizarMascaraImagemValidatorEtapaChecklist.GetValueOrDefault())
                        {
                            request.steps[i].proportion = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistStepProportion();
                            request.steps[i].proportion.width = 1;
                            request.steps[i].proportion.height = 5;

                        }
                        request.steps[i].mode = ConfiguracoesJson.ModoValidacaoImagemEtapaChecklist;
                        if (ConfiguracoesJson.QuantidadeMinimaEtapaChecklist.HasValue || ConfiguracoesJson.QuantidadeMaximaEtapaChecklist.HasValue)
                        {
                            request.steps[i].validationLimit = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ChecklistStepMinMax();
                            request.steps[i].validationLimit.min = ConfiguracoesJson.QuantidadeMinimaEtapaChecklist;
                            request.steps[i].validationLimit.max = ConfiguracoesJson.QuantidadeMaximaEtapaChecklist;
                        }
                        break;
                    case TipoEtapaChecklistSuperApp.Timer:
                        request.steps[i].canPause = ConfiguracoesJson.PermitirPausarEtapaChecklist;
                        request.steps[i].waitInSeconds = ConfiguracoesJson.TempoEsperaEtapaChecklist;
                        break;
                    case TipoEtapaChecklistSuperApp.Location:
                        request.steps[i].geofences = new List<ChecklistGeofence>();
                        request.steps[i].geofences.Add(new ChecklistGeofence()
                        {
                            type = "Circle",
                            coordinates = new List<double>
                            {
                                -52.61737704455287,
                                -27.094412589467936
                            },
                            radius = new Radius()
                            {
                                value = 10,
                                unit = "m"
                            },
                            stopCondition = "ALLOWED"
                        });

                        request.steps[i].outsideAreaFeatures = new ChecklistOutsideAreaFeatures()
                        {
                            block = ConfiguracoesJson.LocalizacaoBloquearAvancoEtapa,
                            allowOutside = ConfiguracoesJson.LocalizacaoPodeAvancarForaRaio,
                            imageRequired = ConfiguracoesJson.LocalizacaoObrigarImagemComprovacao
                        };
                        break;
                    case TipoEtapaChecklistSuperApp.DateTime:
                        request.steps[i].useCurrentTimeAsDefault = ConfiguracoesJson.UsarDataAtualComoInicial;
                        break;
                    default:
                        break;
                }

                request.steps[i].externalInfo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoExterna();
                request.steps[i].externalInfo.id = $"{etapa.Codigo}";
                request.steps[i].externalInfo.tags = new List<string> { etapa.TipoEvidencia.ToString() };
            }

            return request;
        }

        private static dynamic ConverterObjetoAtualizarTracking(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, FrequenciaTrackingAppTrizy frequenciaTrackingAppTrizy)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarUltimaCargaEntrega(carga.Codigo);

            if (cargaEntrega == null)
                throw new ServicoException("Não encontrou a última entrega");

            dynamic request = new
            {
                tracking = new
                {
                    enabled = configuracaoTMS.PossuiMonitoramento,
                    priority = frequenciaTrackingAppTrizy.ObterFrequencia(),
                    startAt = carga.DataInicioViagem?.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") ?? carga.DataInicioViagemPrevista?.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") ?? DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),
                    endAt = cargaEntrega.DataReprogramada?.AddDays(3).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") ?? cargaEntrega.DataPrevista?.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") ?? DateTime.Now.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
                }
            };

            return request;

        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioEnviarNotificacao ConverterObjetoEnviarNotificacao(string cnpjMotorista, string titulo, string mensagem)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioEnviarNotificacao request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.EnvioEnviarNotificacao();

            request.module = "M3002API";
            request.operation = "apiInsNotificacao";
            request.parameters = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ParametrosEnviarNotificacao()
            {
                cpf = cnpjMotorista ?? string.Empty,
                titulo = titulo,
                mensagem = mensagem
            };

            return request;

        }

        private static void SalvarDadosRetorno(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoCargaAPP retorno, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega, bool preTrip, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy repConfiguracaoTrizy = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy(unitOfWork);

            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy = repConfiguracaoTrizy.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);
            bool enviarPrimeiraColetaComoOrigin = (!preTrip && (configuracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente ?? false)) || (preTrip && (configuracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip ?? false));

            if ((retorno.travel?.stopovers != null || retorno.travel?.destination != null) && listaCargaEntrega != null && listaCargaEntrega.Count > 0)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

                int count = 0;
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in listaCargaEntrega)
                {
                    unitOfWork.Flush();

                    if (cargaEntrega == null)
                        continue;

                    if (cargaEntrega == listaCargaEntrega[listaCargaEntrega.Count - 1])
                    {

                        if (string.IsNullOrWhiteSpace(retorno.travel?.destination?.client?.name))
                            continue;

                        cargaEntrega.IdTrizy = retorno.travel.destination._id;
                        repCargaEntrega.Atualizar(cargaEntrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork, configControleEntrega);

                        if (retorno.travel?.destination?.documents != null && retorno.travel?.destination?.documents.Count > 0)
                        {
                            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Documents obj in retorno.travel?.destination?.documents)
                            {
                                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;
                                if (obj.key.Contains("CA_"))
                                    canhoto = repCanhoto.BuscarPorCodigo(obj.key.Remove(0, 3).ToInt());
                                else
                                    canhoto = repCanhoto.BuscarPorChaveENFAtiva(obj.key);

                                if (canhoto == null)
                                    continue;

                                canhoto.IdTrizy = obj._id;
                                repCanhoto.Atualizar(canhoto);
                            }
                        }
                    }
                    else
                    {
                        if (retorno.travel.stopovers == null || retorno.travel.stopovers.Count == 0 || string.IsNullOrWhiteSpace(retorno.travel.stopovers[count]?._id))
                            continue;

                        cargaEntrega.IdTrizy = retorno.travel.stopovers[count]._id;
                        repCargaEntrega.Atualizar(cargaEntrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork, configControleEntrega);

                        if (retorno.travel?.stopovers[count]?.documents != null && retorno.travel?.stopovers[count]?.documents.Count > 0)
                        {
                            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Documents obj in retorno.travel?.stopovers[count]?.documents)
                            {
                                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;
                                if (obj.key.Contains("CA_"))
                                    canhoto = repCanhoto.BuscarPorCodigo(obj.key.Remove(0, 3).ToInt());
                                else
                                    canhoto = repCanhoto.BuscarPorChaveENFAtiva(obj.key);

                                if (canhoto == null)
                                    continue;

                                canhoto.IdTrizy = obj._id;
                                repCanhoto.Atualizar(canhoto);
                            }
                        }

                        count++;
                    }
                }

                if (enviarPrimeiraColetaComoOrigin && retorno.travel.origin != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaColeta = repCargaEntrega.BuscarPrimeiraCargaEntregaColetaPorCarga(carga.Codigo);
                    if (cargaEntregaColeta != null)
                        cargaEntregaColeta.IdTrizy = retorno.travel.origin._id;
                }
            }

            carga.IDIdentificacaoTrizzy = retorno.travel?._id ?? string.Empty;

        }
        private static void SalvarDadosRetornoV3(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.RetornoCargaAPP retorno, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega, bool preTrip, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy repConfiguracaoTrizy = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy(unitOfWork);

            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy = repConfiguracaoTrizy.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);
            bool enviarPrimeiraColetaComoOrigin = (!preTrip && (configuracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente ?? false)) || (preTrip && (configuracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip ?? false));

            if ((retorno.travel?.stopovers != null || retorno.travel?.destination != null) && listaCargaEntrega != null && listaCargaEntrega.Count > 0)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

                int count = 0;
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in listaCargaEntrega)
                {
                    if (cargaEntrega == null)
                        continue;

                    if (cargaEntrega == listaCargaEntrega[listaCargaEntrega.Count - 1])
                    {

                        if (string.IsNullOrWhiteSpace(retorno.travel?.destination?.client?.name))
                            continue;

                        cargaEntrega.IdTrizy = retorno.travel.destination._id;
                        repCargaEntrega.Atualizar(cargaEntrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork, configControleEntrega);

                        if (retorno.travel?.destination?.documents != null && retorno.travel?.destination?.documents.Count > 0)
                        {
                            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Documents obj in retorno.travel?.destination?.documents)
                            {
                                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;
                                if (obj.key.Contains("CA_"))
                                    canhoto = repCanhoto.BuscarPorCodigo(obj.key.Remove(0, 3).ToInt());
                                else
                                    canhoto = repCanhoto.BuscarPorChaveENFAtiva(obj.key);

                                if (canhoto == null)
                                    continue;

                                canhoto.IdTrizy = obj._id;
                                repCanhoto.Atualizar(canhoto);
                            }
                        }
                    }
                    else
                    {
                        if (retorno.travel.stopovers == null || retorno.travel.stopovers.Count == 0 || string.IsNullOrWhiteSpace(retorno.travel.stopovers[count]?._id))
                            continue;

                        cargaEntrega.IdTrizy = retorno.travel.stopovers[count]._id;
                        repCargaEntrega.Atualizar(cargaEntrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork, configControleEntrega);

                        if (retorno.travel?.stopovers[count]?.documents != null && retorno.travel?.stopovers[count]?.documents.Count > 0)
                        {
                            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Documents obj in retorno.travel?.stopovers[count]?.documents)
                            {
                                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;
                                if (obj.key.Contains("CA_"))
                                    canhoto = repCanhoto.BuscarPorCodigo(obj.key.Remove(0, 3).ToInt());
                                else
                                    canhoto = repCanhoto.BuscarPorChaveENFAtiva(obj.key);

                                if (canhoto == null)
                                    continue;

                                canhoto.IdTrizy = obj._id;
                                repCanhoto.Atualizar(canhoto);
                            }
                        }

                        count++;
                    }
                }

                if (enviarPrimeiraColetaComoOrigin && retorno.travel.origin != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaColeta = repCargaEntrega.BuscarPrimeiraCargaEntregaColetaPorCarga(carga.Codigo);
                    if (cargaEntregaColeta != null)
                        cargaEntregaColeta.IdTrizy = retorno.travel.origin._id;
                }
            }

            carga.IDIdentificacaoTrizzy = retorno.travel?._id ?? string.Empty;

        }

        /// <summary>
        /// Método para liberar integrações pendentes de finalização de viagem.
        /// Regras conforme alinhado na task #7339:
        ///     Deverá liberar a integração da carga por ordem de data de criação.
        ///     Caso haja integrações pendentes na etapa 1 (prétrip) e na etapa 6 (viagem) para mesma carga: Deverá ignorar a da etapa 1 e enviar apenas da etapa 6.
        /// </summary>
        /// <param name="cargaFinalizada">Código da Carga que teve a viagem finalizada/cancelada/transbordada</param>
        /// <param name="unitOfWork">Unidade de trabalho</param>
        public static void AtualizarViagemAntiga(Dominio.ObjetosDeValor.WebService.Carga.Carga cargaFinalizada, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga cargaLiberada = null;
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            /// Busca as integrações da Etapa 6 que estão pendentes de liberação. Libera a primeira, deixando as demais pendentes apontando para a carga liberada.
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> listaCargaIntegracao = repCargaIntegracao.BuscarPendentesAguardandoFinalizarCargaAnterior(cargaFinalizada.Protocolo);

            if (listaCargaIntegracao?.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao in listaCargaIntegracao)
                {
                    if (repCargaCancelamento.ExistePorCargaEData(DateTime.Now, cargaIntegracao.Carga.Codigo))
                    {
                        cargaIntegracao.AguardarFinalizarCargaAnterior = false;
                        cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = "Integração ignorada pois existe cancelamento registrado.";
                    }
                    else
                    {
                        if (cargaLiberada == null)
                            cargaLiberada = cargaIntegracao.Carga;

                        if (cargaLiberada.Codigo == cargaIntegracao.Carga.Codigo)
                        {
                            cargaIntegracao.AguardarFinalizarCargaAnterior = false;
                            cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                            cargaIntegracao.ProblemaIntegracao = SituacaoIntegracao.AgIntegracao.ObterDescricao();
                        }
                        else
                        {
                            cargaIntegracao.CargaPendente = cargaLiberada;
                            cargaIntegracao.AguardarFinalizarCargaAnterior = true;
                            cargaIntegracao.ProblemaIntegracao = $"A carga {cargaLiberada.CodigoCargaEmbarcador} está pendente de finalização, para envio de nova carga para Trizy";
                        }
                    }
                    cargaIntegracao.DataIntegracao = DateTime.Now;
                    repCargaIntegracao.Atualizar(cargaIntegracao);
                    try
                    {
                        Servicos.Auditoria.Auditoria.AuditarSemDadosUsuario(cargaIntegracao, cargaIntegracao.ProblemaIntegracao + (cargaLiberada != null ? $" | Liberado pela carga: {cargaFinalizada.NumeroCarga}" : ""), unitOfWork);
                    }
                    catch (Exception) { /* Enterra o erro na auditoria. */}
                }
            }

            /// Busca as integrações da Etapa 1 que estão aguardando finalização da viagem da carga.
            /// Libera a primeira integração e as demais deixa pendente para a carga liberada.
            /// Caso a primeira integração foi liberada na etapa 6, ignora a integração da etapa 1 marcando como integrado.
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> listaCargaDadosTrasporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPendentesAguardandoFinalizarCargaAnterior(cargaFinalizada.Protocolo);

            if (listaCargaDadosTrasporteIntegracao?.Count > 0)
            {
                bool houveLiberacaoEtapa6 = cargaLiberada != null;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTrasporteIntegracao in listaCargaDadosTrasporteIntegracao)
                {
                    if (repCargaCancelamento.ExistePorCargaEData(DateTime.Now, cargaDadosTrasporteIntegracao.Carga.Codigo))
                    {
                        cargaDadosTrasporteIntegracao.AguardarFinalizarCargaAnterior = false;
                        cargaDadosTrasporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaDadosTrasporteIntegracao.ProblemaIntegracao = "Integração ignorada pois existe cancelamento registrado.";
                    }
                    else
                    {
                        if (cargaLiberada == null)
                            cargaLiberada = cargaDadosTrasporteIntegracao.Carga;

                        if (cargaDadosTrasporteIntegracao.Carga.Codigo == cargaLiberada.Codigo)
                        {
                            cargaDadosTrasporteIntegracao.AguardarFinalizarCargaAnterior = false;
                            if (houveLiberacaoEtapa6)
                            {
                                cargaDadosTrasporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                                cargaDadosTrasporteIntegracao.ProblemaIntegracao = "Integração PréTrip ignorada pois existe integração de viagem (etapa 6).";
                            }
                            else
                            {
                                cargaDadosTrasporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                                cargaDadosTrasporteIntegracao.ProblemaIntegracao = SituacaoIntegracao.AgIntegracao.ObterDescricao();
                            }
                        }
                        else
                        {
                            cargaDadosTrasporteIntegracao.CargaPendente = cargaLiberada;
                            cargaDadosTrasporteIntegracao.AguardarFinalizarCargaAnterior = true;
                            cargaDadosTrasporteIntegracao.ProblemaIntegracao = $"A carga {cargaLiberada.CodigoCargaEmbarcador} está pendente de finalização, para envio de nova carga para Trizy";
                        }
                    }
                    cargaDadosTrasporteIntegracao.DataIntegracao = DateTime.Now;
                    repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTrasporteIntegracao);
                    try
                    {
                        Servicos.Auditoria.Auditoria.AuditarSemDadosUsuario(cargaDadosTrasporteIntegracao, cargaDadosTrasporteIntegracao.ProblemaIntegracao + (cargaLiberada != null ? $" | Liberado pela carga: {cargaFinalizada.NumeroCarga}" : ""), unitOfWork);
                    }
                    catch (Exception) { /* Enterra o erro na auditoria. */}
                }
            }
        }

        private Dominio.Entidades.Embarcador.Configuracoes.Integracao ObterConfiguracaoPadrao()
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null)
                throw new ServicoException("Não existe Configuração de integração com a trizy");

            return configuracaoIntegracao;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy ObterConfiguracaoPadraoTrizy()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracao = repositorioConfiguracaoIntegracao.BuscarPrimeiroRegistro();

            if (configuracaoIntegracao == null)
                throw new ServicoException("Não existe Configuração de integração com a trizy");

            return configuracaoIntegracao;
        }

        private HttpClient ObterCliente(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracao, string token)
        {
            HttpClient cliente = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return cliente;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Contrato ObterContrato(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ContratoDetalhes requisicaoContratoDetalhes = ObterContratoDetalhes(cargaDadosTransporteIntegracao);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Contrato requisicaoContrato = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Contrato()
            {
                ContratoDetalhes = requisicaoContratoDetalhes
            };

            return requisicaoContrato;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ContratoDetalhes ObterContratoDetalhes(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaDadosTransporteIntegracao.CargaPedido.Pedido;
            Dominio.Entidades.Cliente tomador = pedido.ObterTomador();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProduto = repositorioPedidoProduto.BuscarPorPedido(pedido.Codigo);

            DateTime? dataColeta = pedido.DataInicialColeta;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ContratoDetalhes requisicaoContratoDetalhes = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ContratoDetalhes()
            {
                Atributos = ObterAtributos(cargaDadosTransporteIntegracao),
                Descricao = pedido.NumeroPedidoEmbarcador + " - " + pedido.Destino.DescricaoCidadeEstado,
                Incoterm = pedido.CanalEntrega?.Descricao ?? string.Empty,
                IdentificadorExterno = pedido.NumeroPedidoEmbarcador,
                Terminal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Terminal()
                {
                    CNPJ = pedido.Remetente?.CPF_CNPJ_SemFormato.ToString() ?? string.Empty
                },
                Cliente = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ClienteContrato()
                {
                    CNPJ = pedido.Destinatario?.CPF_CNPJ_SemFormato.ToString() ?? string.Empty,
                    Nome = pedido.Destinatario?.Nome ?? string.Empty,
                    NomeFantasia = pedido.Destinatario?.NomeFantasia ?? string.Empty,
                    CEP = pedido.Destinatario?.CEP ?? string.Empty,
                },
                Operacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Operacao()
                {
                    Descricao = pedido.TipoOperacao?.Descricao ?? string.Empty,
                    Tipo = pedido.TipoOperacao?.RemessaSAP.ToString() ?? string.Empty,
                },
                Produto = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.ProdutoContrato()
                {
                    Descricao = pedidosProduto != null ? pedidosProduto.Select(o => o.Produto.Descricao).FirstOrDefault() : "",
                    NCM = pedidosProduto != null ? pedidosProduto.Select(o => o.Produto.CodigoProdutoEmbarcador).FirstOrDefault() : "",
                },
                Vigencia = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Vigencia()
                {
                    Inicio = dataColeta.HasValue ? dataColeta.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "",
                    Fim = dataColeta.HasValue ? dataColeta.Value.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss") : "",
                },
                Recursos = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Recursos()
                {
                    Peso = pedido?.PesoTotal.ToString("n0").ObterSomenteNumeros() ?? string.Empty,
                    Veiculos = "1",
                    UnidadeNegociacao = "kg",
                },
                Flags = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Flags()
                {
                    ExigeNota = "false",
                    ReutilizarNota = "false",
                    TRDisputarCota = "false",
                    CotaFlexivel = "false",
                    FracaoCotaFlexivel = "false",
                    TipoDemanda = pedido.TipoOperacao != null ? pedido.TipoOperacao.RemessaSAP == 2 ? "IN" : "OUT" : string.Empty,
                    ExigeAceiteTransp = "false",
                    ExigeAceiteCliente = "false",
                    TerminalRespTransp = pedido.CanalEntrega != null ? (pedido.CanalEntrega.Descricao == "CIF" ? "true" : "false") : string.Empty,
                },
                Emissor = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Emissor()
                {
                    Cidade = pedido.Remetente?.Localidade?.Descricao ?? string.Empty,
                    CNPJ = pedido.Remetente?.CPF_CNPJ_SemFormato.ToString() ?? string.Empty,
                    Logradouro = pedido.Remetente?.Endereco ?? string.Empty,
                    Nome = pedido.Remetente?.Nome ?? string.Empty,
                    Numero = pedido.Remetente?.NomeFantasia ?? string.Empty,
                    UF = pedido.Remetente?.Localidade?.Estado?.Sigla ?? string.Empty,
                },
                Tomador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Tomador()
                {
                    Cidade = tomador?.Localidade?.Descricao ?? string.Empty,
                    CNPJ = tomador?.CPF_CNPJ_SemFormato.ToString() ?? string.Empty,
                    Logradouro = tomador?.Endereco ?? string.Empty,
                    Nome = tomador?.Nome ?? string.Empty,
                    Numero = tomador?.NomeFantasia ?? string.Empty,
                    UF = tomador?.Localidade?.Estado?.Sigla ?? string.Empty,
                },
                Destino = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Destino()
                {
                    Cidade = pedido.Recebedor != null ? pedido.Recebedor?.Localidade?.Descricao ?? string.Empty : pedido?.EnderecoDestino?.Localidade?.Descricao ?? string.Empty,
                    CNPJ = pedido.Recebedor != null ? pedido.Recebedor.CPF_CNPJ_SemFormato.ToString() : pedido?.Destinatario?.CPF_CNPJ_SemFormato.ToString() ?? string.Empty,
                    Logradouro = pedido.Recebedor != null ? pedido.Recebedor.Endereco : pedido?.EnderecoDestino?.Endereco ?? string.Empty,
                    Nome = pedido.Recebedor != null ? pedido.Recebedor.Nome : pedido?.Destinatario?.Nome ?? string.Empty,
                    Numero = pedido.Recebedor != null ? pedido.Recebedor.NomeFantasia ?? string.Empty : pedido.Destinatario?.NomeFantasia ?? string.Empty,
                    UF = pedido.Recebedor != null ? pedido.Recebedor.Localidade?.Estado?.Sigla ?? string.Empty : pedido?.EnderecoDestino?.Localidade?.Estado?.Sigla ?? string.Empty,
                },
                Cotas = ObterCotas(cargaDadosTransporteIntegracao),
                FracoesCota = cargaDadosTransporteIntegracao.Carga.Empresa != null ? ObterFracoesCotas(cargaDadosTransporteIntegracao, dataColeta) : null,
            };

            return requisicaoContratoDetalhes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Atributos> ObterAtributos(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Atributos> atributos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Atributos>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Atributos atributoNumero = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Atributos()
            {
                Atributo = "Numero_da_Carga_Multi",
                Valor = cargaDadosTransporteIntegracao.Carga.CodigoCargaEmbarcador
            };

            atributos.Add(atributoNumero);


            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Atributos atributoProtocolo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Atributos()
            {
                Atributo = "Protocolo_Multi",
                Valor = cargaDadosTransporteIntegracao.Carga.Protocolo.ToString()
            };

            atributos.Add(atributoProtocolo);

            return atributos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Cotas> ObterCotas(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Cotas> cotas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Cotas>();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaDadosTransporteIntegracao.CargaPedido.Pedido;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Cotas cotasTrizy = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Cotas()
            {
                Data = pedido.DataInicialColeta?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty,
                Peso = pedido.PesoTotal.ToString("n0").ObterSomenteNumeros() ?? string.Empty,
            };

            cotas.Add(cotasTrizy);

            return cotas;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoAdicional> ObterInformacoesAdicionaisCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoAdicional> obj = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoAdicional>()
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoAdicional()
                {
                    label = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Linguagem()
                    {
                        pt = "TRANSPORTADORA",
                        en = "SHIPPING COMPANY",
                        es = "COMPAÑÍA DE ENVIOS",
                    },
                    description = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Linguagem()
                    {
                        pt = carga.Empresa?.RazaoSocial ?? string.Empty,
                        en = carga.Empresa?.RazaoSocial ?? string.Empty,
                        es = carga.Empresa?.RazaoSocial ?? string.Empty,
                    }
                }
            };
            return obj;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoAdicional> ObterInformacoesAdicionaisEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente cliente, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar clienteComplementar, List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaEntrega)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoAdicional> informacoesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoAdicional>();

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = new();
            if (canhotosDaEntrega != null)
            {
                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotosDaEntrega)
                {
                    if (canhoto.TipoCanhoto == TipoCanhoto.Avulso)
                        notasFiscais.AddRange(canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Select(pedido => pedido.XMLNotaFiscal).ToList());
                    else if (canhoto.TipoCanhoto == TipoCanhoto.NFe)
                        notasFiscais.Add(canhoto.XMLNotaFiscal);
                }
            }

            bool enviarInformacoesAdicionaisEntrega = carga.TipoOperacao?.ConfiguracaoTrizy?.EnviarInformacoesAdicionaisEntrega ?? false;
            List<InformacoesAdicionaisEntregaTrizy> informacoesAdicionaisEntrega = carga.TipoOperacao?.ConfiguracaoTrizy?.InformacoesAdicionaisEntrega.ToList();
            if (!enviarInformacoesAdicionaisEntrega || informacoesAdicionaisEntrega == null || informacoesAdicionaisEntrega.Count == 0) return informacoesAdicionais;

            if (informacoesAdicionaisEntrega.Exists(info => info == InformacoesAdicionaisEntregaTrizy.Fardos) && (produtos?.Count ?? 0) > 0)
            {
                string descricaoProdutos = string.Join(", ", produtos.Select(produto => $"{produto.Produto.Descricao}: {produto.Quantidade.ToString("F0", CultureInfo.InvariantCulture)} fardos"));

                if (!string.IsNullOrWhiteSpace(descricaoProdutos))
                    informacoesAdicionais.Add(CriarInformacaoAdicional(Localization.Resources.Produtos.ProdutoEmbarcador.Produtos, descricaoProdutos));
            }

            if (informacoesAdicionaisEntrega.Exists(info => info == InformacoesAdicionaisEntregaTrizy.ObservacoesDoCliente))
            {
                string observacao = !string.IsNullOrEmpty(cliente.Observacao) ? cliente.Observacao : cliente.Nome;

                informacoesAdicionais.Add(CriarInformacaoAdicional(Localization.Resources.Pedidos.TipoOperacao.ObservacoesDoCliente, observacao));
            }

            if (informacoesAdicionaisEntrega.Exists(info => info == InformacoesAdicionaisEntregaTrizy.NumeroDosPedidos) && (pedidos?.Count ?? 0) > 0)
            {
                string descricaoPedidos = string.Join(", ", pedidos.Select(pedido => pedido.NumeroPedidoEmbarcador));

                if (!string.IsNullOrWhiteSpace(descricaoPedidos))
                    informacoesAdicionais.Add(CriarInformacaoAdicional(Localization.Resources.Pedidos.Pedido.DescricaoPedidos, descricaoPedidos));
            }

            if (informacoesAdicionaisEntrega.Exists(info => info == InformacoesAdicionaisEntregaTrizy.TelefoneDoCliente))
            {
                string telefoneCliente = cliente.Enderecos?.FirstOrDefault(x => !string.IsNullOrEmpty(x.Telefone))?.Telefone;
                if (string.IsNullOrWhiteSpace(telefoneCliente))
                    telefoneCliente = cliente.Telefone1;
                informacoesAdicionais.Add(CriarInformacaoAdicional(Localization.Resources.Pedidos.TipoOperacao.TelefoneDoCliente, telefoneCliente.ObterTelefoneFormatado()));
            }

            if (informacoesAdicionaisEntrega.Exists(info => info == InformacoesAdicionaisEntregaTrizy.RegrasPallet) && clienteComplementar != null)
            {
                string regra = clienteComplementar.RegraPallet.ObterDescricao();
                if (!string.IsNullOrEmpty(regra))
                    informacoesAdicionais.Add(CriarInformacaoAdicional(Localization.Resources.Pedidos.TipoOperacao.RegrasPalletTrizy, regra));
            }

            if (informacoesAdicionaisEntrega.Exists(info => info == InformacoesAdicionaisEntregaTrizy.NotasFiscais) && notasFiscais != null)
            {
                string notas = string.Join(", ", notasFiscais.Where(nota => nota.Destinatario.Codigo == cliente.Codigo).DistinctBy(notas => notas.Numero).Select(nota => nota.Numero));

                if (!string.IsNullOrWhiteSpace(notas))
                    informacoesAdicionais.Add(CriarInformacaoAdicional("NÚMERO DA NOTA FISCAL", notas));
            }

            if (informacoesAdicionaisEntrega.Exists(info => info == InformacoesAdicionaisEntregaTrizy.RazaoSocial) && !string.IsNullOrWhiteSpace(cliente.Nome))
                informacoesAdicionais.Add(CriarInformacaoAdicional("RAZÃO SOCIAL", cliente.Nome));

            if (informacoesAdicionaisEntrega.Exists(info => info == InformacoesAdicionaisEntregaTrizy.NomeFantasia) && !string.IsNullOrWhiteSpace(cliente.NomeFantasia))
                informacoesAdicionais.Add(CriarInformacaoAdicional("NOME FANTASIA", cliente.NomeFantasia));

            return informacoesAdicionais;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoAdicional CriarInformacaoAdicional(string label, string description)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.InformacaoAdicional()
            {
                label = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Linguagem() { pt = label, en = label, es = label },
                description = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Linguagem() { pt = description, en = description, es = description }
            };
        }

        private static string ObterDataEsperada(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            int diasAdicionaisPadrao = 0;
            DateTime? dataEsperada = null;
            if (cargaEntrega != null)
            {
                DataEsperadaColetaEntregaTrizy dataEsperadaColetaEntrega = cargaEntrega.Coleta ? configuracaoTrizy.DataEsperadaParaColetas : configuracaoTrizy.DataEsperadaParaEntregas;
                switch (dataEsperadaColetaEntrega)
                {
                    case DataEsperadaColetaEntregaTrizy.DataPrevisao:
                        dataEsperada = cargaEntrega.DataPrevista;
                        break;
                    case DataEsperadaColetaEntregaTrizy.DataAgendamento:
                        dataEsperada = cargaEntrega.DataAgendamento;
                        break;
                    case DataEsperadaColetaEntregaTrizy.DataAgendamentoTransportador:
                        dataEsperada = cargaEntrega.DataPrevisaoEntregaTransportador;
                        break;
                    default:
                        dataEsperada = DateTime.Now.AddDays((cargaEntrega.Coleta ? 0 : 2));
                        break;
                }
                diasAdicionaisPadrao = cargaEntrega.Coleta ? 0 : 2;
            }
            return (dataEsperada ?? DateTime.Now.AddDays(diasAdicionaisPadrao)).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.FracoesCota> ObterFracoesCotas(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, DateTime? dataColeta)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.FracoesCota> fracoesCota = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.FracoesCota>();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaDadosTransporteIntegracao.CargaPedido.Pedido;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.FracoesCota fracoesCotasTrizy = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.FracoesCota()
            {
                Data = dataColeta.HasValue ? dataColeta.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "",
                Peso = pedido.PesoTotal.ToString("n0").ObterSomenteNumeros() ?? string.Empty,
                Transportador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Transportador()
                {
                    CNPJ = cargaDadosTransporteIntegracao.Carga.Empresa?.CNPJ_SemFormato ?? string.Empty,
                    Nome = cargaDadosTransporteIntegracao.Carga.Empresa?.RazaoSocial ?? string.Empty,
                    NomeFantasia = cargaDadosTransporteIntegracao.Carga.Empresa?.NomeFantasia ?? string.Empty,
                    CEP = cargaDadosTransporteIntegracao.Carga.Empresa?.CEP ?? string.Empty,
                }
            };

            fracoesCota.Add(fracoesCotasTrizy);

            return fracoesCota;
        }

        private string ObterDadosTags(Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp notificacao, string texto)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            int codigoCarga = notificacao.Carga?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = notificacao.Chamado;

            if (texto.Contains("#NumeroAtendimento") || texto.Contains("#ChamadoCliente") || texto.Contains("#TratativaChamado") || texto.Contains("#RetornoMotoristaChamado"))
            {
                if (chamado == null)
                {
                    Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
                    chamado = repChamado.BuscarPorCarga(codigoCarga);
                }

                if (chamado != null)
                {
                    texto = texto
                    .Replace("#NumeroAtendimento", chamado.Numero.ToString())
                    .Replace("#ChamadoCliente", chamado.Cliente?.NomeCNPJ ?? string.Empty)
                    .Replace("#TratativaChamado", chamado.TratativaDevolucao.ObterDescricao())
                    .Replace("#RetornoMotoristaChamado", chamado.ObservacaoRetornoMotorista ?? string.Empty);
                }
            }

            texto = texto
            .Replace("#NumeroRecibo", (codigoCarga > 0 ? repositorioCargaValePedagio.BuscarPorUnicaCarga(codigoCarga)?.NumeroValePedagio : string.Empty))
            .Replace("#NumeroNFe", notificacao.GestaoDadosColetaDadosNFe?.Numero.ToString() ?? string.Empty)
            .Replace("#DescricaoMotivo", notificacao.Motivo?.Descricao ?? string.Empty);

            return texto;
        }

        private static dynamic ObterDocumentCargaAPP(string type, string value) => new { type, value };
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Document ObterDocumentCargaAPPV3(string type, string value)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Document() { type = type, value = value };
        }
        private static dynamic ObterOccurrenceCargaAPP(bool enabled, List<string> categories) => new { enabled, categories };
        private static OccurrenceFeature ObterOccurrenceCargaAPPV3(bool preTrip, List<string> categories)
        {
            if (preTrip || categories.Count == 0)
            {
                return new OccurrenceFeature()
                {
                    enabled = !preTrip && categories.Count > 0
                };
            }

            List<OccurrenceFeatureCategory> categoriesApp = new List<OccurrenceFeatureCategory>();

            foreach (string category in categories)
            {
                OccurrenceFeatureCategory item = new OccurrenceFeatureCategory
                {
                    category = category
                };

                categoriesApp.Add(item);
            }

            return new OccurrenceFeature()
            {
                enabled = !preTrip && categories.Count > 0,
                categories = categoriesApp
            };
        }
        private static LoadDetail ObterLoadDetailAPPV3(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy, List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio> listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio, string layoutSuperAppId)
        {
            if (configuracaoTrizy.HabilitarEnvioRelatorio && !string.IsNullOrEmpty(layoutSuperAppId))
            {
                List<InformacaoAdicional> informacaoAdicional = listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio
                    .Select(x => new InformacaoAdicional
                    {
                        label = GerarTextoLinguagem(x.Rotulo),
                        description = GerarTextoLinguagem(x.Descricao)
                    })
                    .ToList();

                return new LoadDetail()
                {
                    enabled = true,
                    receipt = new LoadDetailReceipt()
                    {
                        enabled = true,
                        titleReceipt = GerarTextoInternacionalizado(configuracaoTrizy.TituloReciboViagem),
                        titleReport = GerarTextoInternacionalizado(configuracaoTrizy.TituloRelatorioViagem),
                        additionalInfo = informacaoAdicional,
                    },
                    layout = layoutSuperAppId
                };
            }

            return new LoadDetail()
            {
                enabled = false
            };
        }
        private static PartialDeliveryFeature ObterPartialDeliveryFeatureAPPV3(List<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega> motivosDevolucaoEntrega, bool temItems, bool habilitarDevolucaoParcial)
        {
            List<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega> motivosDevolucaoEntregaComIdTrizy = motivosDevolucaoEntrega.Where(motivo => !string.IsNullOrEmpty(motivo.EntregaParcialSuperAppId)).ToList();
            if (!temItems && habilitarDevolucaoParcial)
            {
                throw new ServicoException($"Para habilitar a devolução total é preciso ter itens na nota.");
            }

            if (!motivosDevolucaoEntregaComIdTrizy.Any() && habilitarDevolucaoParcial)
            {
                throw new ServicoException($"Para habilitar a devolução total é preciso ter motivos de devolução cadastrados e integrados com a Trizy.");
            }

            if (motivosDevolucaoEntregaComIdTrizy.Any() && temItems && habilitarDevolucaoParcial)
            {
                List<Reason> reasons = motivosDevolucaoEntregaComIdTrizy
                    .Select(motivo => new Reason
                    {
                        reason = motivo.EntregaParcialSuperAppId
                    })
                    .ToList();

                return new PartialDeliveryFeature()
                {
                    enabled = true,
                    reasons = reasons
                };
            }

            return new PartialDeliveryFeature()
            {
                enabled = false
            };
        }
        private static PartialDeliveryFeature ObterNotDeliveredFeatureAPPV3(List<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega> motivosDevolucaoEntrega, bool temItems, bool habilitarDevolucaoTotal)
        {
            List<Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega> motivosDevolucaoEntregaComIdTrizy = motivosDevolucaoEntrega.Where(motivo => !string.IsNullOrEmpty(motivo.NaoEntregaSuperAppId)).ToList();
            if (!temItems && habilitarDevolucaoTotal)
            {
                throw new ServicoException($"Para habilitar a devolução total é preciso ter itens na nota.");
            }

            if (!motivosDevolucaoEntregaComIdTrizy.Any() && habilitarDevolucaoTotal)
            {
                throw new ServicoException($"Para habilitar a devolução total é preciso ter motivos de devolução cadastrados e integrados com a Trizy.");
            }


            if (motivosDevolucaoEntregaComIdTrizy.Any() && temItems && habilitarDevolucaoTotal)
            {
                List<Reason> reasons = motivosDevolucaoEntregaComIdTrizy
                    .Select(motivo => new Reason
                    {
                        reason = motivo.NaoEntregaSuperAppId
                    })
                    .ToList();

                return new PartialDeliveryFeature()
                {
                    enabled = true,
                    reasons = reasons
                };
            }

            return new PartialDeliveryFeature()
            {
                enabled = false
            };
        }
        private static dynamic ObterLocationCargaAPP(string type, decimal longitude, decimal latitude) => new { type, coordinates = new List<decimal>() { longitude, latitude } };
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Location ObterLocationCargaAPPV3(string type, decimal longitude, decimal latitude)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Location() { type = type, coordinates = new List<decimal>() { longitude, latitude } };
        }
        private static dynamic ObterEventCargaAPP(string type, string title, string expectedAt, bool required, string status, TipoCustomEventAppTrizy externalId = TipoCustomEventAppTrizy.Nenhum) => new { type, title, expectedAt, required, status, externalId = externalId.ToString() };


        private static Dominio.ObjetosDeValor.Endereco ObterEnderecoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            Dominio.ObjetosDeValor.Endereco endereco = new Dominio.ObjetosDeValor.Endereco();

            if (cargaEntrega.ClienteOutroEndereco != null)
                MapeiaEndereco(cargaEntrega.ClienteOutroEndereco, endereco);
            else
                MapeiaEndereco(cargaEntrega.Cliente, endereco);

            return endereco;
        }

        private static void MapeiaEndereco(Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco origem, Dominio.ObjetosDeValor.Endereco destino)
        {
            destino.Logradouro = origem.Endereco;
            destino.Cidade = origem.Localidade;
            destino.Bairro = origem.Bairro;
            destino.CEP = origem.CEP;
            destino.Numero = origem.Numero;
            destino.Complemento = origem.Complemento;
            destino.Latitude = origem.Latitude;
            destino.Longitude = origem.Longitude;
        }

        private static void MapeiaEndereco(Dominio.Entidades.Cliente origem, Dominio.ObjetosDeValor.Endereco destino)
        {
            destino.Logradouro = origem.Endereco;
            destino.Cidade = origem.Localidade;
            destino.Bairro = origem.Bairro;
            destino.CEP = origem.CEP;
            destino.Numero = origem.Numero;
            destino.Complemento = origem.Complemento;
            destino.Latitude = origem.Latitude;
            destino.Longitude = origem.Longitude;
        }

        private static bool VerificarSituacaoRoteirizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga.SituacaoRoteirizacaoCarga is SituacaoRoteirizacao.SemDefinicao or SituacaoRoteirizacao.Aguardando)
                return false;

            int tentativas = 0;
            while ((carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Erro ||
                   !Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.CargaPossuiControleEntrega(carga, unitOfWork)) &&
                   tentativas < 3)
            {
                tentativas++;
                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoTMS, tipoServicoMultisoftware);
            }

            if (tentativas == 3)
                return false;

            return true;
        }

        private static void EnviarPDFDosDocumentosFiscais(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, ICollection<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DocumentosFiscaisTrizy> listaEnumDocumentosFiscais, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCargaIntegracao.Carga;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.DocumentosFiscais> listaDocumentosFiscais = new();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork).BuscarPrimeiroRegistro();
            string caminhoPDF;
            string arquivo;

            //Busca apenas os documentos que já tem gerados os PDFs.
            //Caso necessário gerar, deverá ser chamado a geração fora desse serviço de integração.
            if (listaEnumDocumentosFiscais.Contains(DocumentosFiscaisTrizy.MDFe))
            {
                Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repositorioCargaMDFe.BuscarPorCarga(carga.Codigo);
                if (cargaMDFes.Count > 0)
                {

                    List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfeLista = cargaMDFes.Where(cargaMdfe => cargaMdfe.MDFe != null && cargaMdfe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado).Select(cargaMDFe => cargaMDFe.MDFe).ToList();

                    if (mdfeLista != null && mdfeLista.Count > 0)
                    {
                        foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfeLista)
                        {
                            arquivo = string.Empty;
                            caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";
                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                arquivo = Convert.ToBase64String(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF));
                            else
                            {
                                try
                                {
                                    ReportResult resultReportApi = ReportRequest.WithType(ReportType.CargaMDFe)
                                                              .WithExecutionType(ExecutionType.Async)
                                                              .AddExtraData("codigoMDFe", mdfe.Codigo)
                                                              .AddExtraData("contingencia", false)
                                                              .CallReport();
                                    if (resultReportApi == null)
                                        throw new Exception();

                                    arquivo = Convert.ToBase64String(resultReportApi.GetContentFile());
                                }
                                catch
                                {
                                }
                            }

                            if (!string.IsNullOrEmpty(arquivo))
                                listaDocumentosFiscais.Add(new()
                                {
                                    travel = carga.IDIdentificacaoTrizzy,
                                    documentType = DocumentosFiscaisTrizy.MDFe.ObterIDTrizy(),
                                    file = arquivo,
                                    label = $"{DocumentosFiscaisTrizy.MDFe.ObterDescricao()}: Número {mdfe.Numero} - Série {mdfe.Serie?.Numero}",
                                    description = mdfe.Chave
                                });
                        }
                    }
                }
            }

            if (listaEnumDocumentosFiscais.Contains(DocumentosFiscaisTrizy.CTe))
            {
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCTe.BuscarPorCarga(carga.Codigo);
                if (cargaCTes.Count > 0)
                {

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesLista = cargaCTes
                        .Where(cargaCte => cargaCte.CTe != null)
                        .Select(cargaCte => cargaCte.CTe)
                        .ToList();

                    if (ctesLista != null && ctesLista.Any())
                    {

                        foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctesLista)
                        {
                            arquivo = string.Empty;
                            caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoRelatorios, cte.Empresa.CNPJ, cte.Chave) + ".pdf";
                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                arquivo = Convert.ToBase64String(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF));
                            else
                            {
                                try
                                {
                                    Servicos.DACTE svcDACTE = new Servicos.DACTE(unitOfWork);
                                    arquivo = Convert.ToBase64String(svcDACTE.GerarPorProcesso(cte.Codigo, null, false));
                                }
                                catch
                                {
                                }
                            }

                            if (!string.IsNullOrEmpty(arquivo))
                                listaDocumentosFiscais.Add(new()
                                {
                                    travel = carga.IDIdentificacaoTrizzy,
                                    documentType = DocumentosFiscaisTrizy.CTe.ObterIDTrizy(),
                                    file = arquivo,
                                    label = $"{DocumentosFiscaisTrizy.CTe.ObterDescricao()}: Número {cte.Numero} - Série {cte.Serie?.Numero}",
                                    description = cte.Chave
                                });
                        }
                    }

                }
            }

            if (listaEnumDocumentosFiscais.Contains(DocumentosFiscaisTrizy.NFe))
            {
                //De primeiro momento não terá envio de NFEs, pois existe clientes com muitas notas e ficaria inviável.
            }

            if (listaEnumDocumentosFiscais.Contains(DocumentosFiscaisTrizy.CIOT))
            {
                Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repositorioCargaCIOT.BuscarPorCarga(carga.Codigo);
                if (cargaCIOT != null)
                {
                    arquivo = string.Empty;
                    caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoRelatorios, carga.Empresa.CNPJ, cargaCIOT.CIOT.Numero) + ".pdf";
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        arquivo = Convert.ToBase64String(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF));
                    else
                    {
                        try
                        {
                            ReportResult resultReportApi = ReportRequest.WithType(ReportType.CIOT)
                                                            .WithExecutionType(ExecutionType.Async)
                                                            .AddExtraData("CodigoCiot", cargaCIOT.CIOT.Codigo.ToString())
                                                            .CallReport();
                            if (resultReportApi == null)
                                throw new Exception();

                            arquivo = Convert.ToBase64String(resultReportApi.GetContentFile());
                        }
                        catch
                        {
                        }
                    }
                    if (!string.IsNullOrEmpty(arquivo))
                        listaDocumentosFiscais.Add(new()
                        {
                            travel = carga.IDIdentificacaoTrizzy,
                            documentType = DocumentosFiscaisTrizy.CIOT.ObterIDTrizy(),
                            file = arquivo,
                            label = $"{DocumentosFiscaisTrizy.CIOT.ObterDescricao()}: Número {cargaCIOT.CIOT.Numero}",
                            description = cargaCIOT.CIOT.Numero
                        });
                }
            }

            if (listaEnumDocumentosFiscais.Contains(DocumentosFiscaisTrizy.VPO))
            {
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio RepositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaValePedagio = RepositorioCargaIntegracaoValePedagio.BuscarPorCarga(carga.Codigo);
                Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);

                if (cargaValePedagio.Count > 0)
                {
                    foreach (var valePedagio in cargaValePedagio)
                    {
                        arquivo = string.Empty;
                        caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoRelatorios, valePedagio?.NumeroValePedagio ?? "") + ".pdf";
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                            arquivo = Convert.ToBase64String(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF));
                        else
                        {
                            try
                            {
                                byte[] arquivoBytes = null;
                                string mensagemRetorno = servicoValePedagio.ObterArquivoValePedagio(valePedagio, ref arquivoBytes, tipoServicoMultisoftware);

                                if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                                    throw new ServicoException(mensagemRetorno);

                                if (arquivoBytes == null || arquivoBytes.Length == 0)
                                    throw new ServicoException("Arquivo do vale-pedágio não foi retornado.");

                                arquivo = Convert.ToBase64String(arquivoBytes);
                            }
                            catch
                            {
                            }
                        }
                        if (!string.IsNullOrEmpty(arquivo))
                            listaDocumentosFiscais.Add(new()
                            {
                                travel = carga.IDIdentificacaoTrizzy,
                                documentType = DocumentosFiscaisTrizy.VPO.ObterIDTrizy(),
                                file = arquivo,
                                label = $"{DocumentosFiscaisTrizy.VPO.ObterDescricao()}: Número {valePedagio.NumeroValePedagio}",
                                description = valePedagio.NumeroValePedagio
                            });
                    }
                }
            }

            if (listaDocumentosFiscais.Count == 0) return;
            EnviarDocumentos(listaDocumentosFiscais, cargaCargaIntegracao, unitOfWork);
        }

        private static void EnviarDocumentos(List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.DocumentosFiscais> listaDocumentosFiscais, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            string id = Guid.NewGuid().ToString();

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCargaIntegracao.Carga;
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string endPoint = $"{configuracaoIntegracao.URLTrizy}/core/v1/external/document";
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTrizy));
            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", configuracaoIntegracao.TokenTrizy);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            bool falhaEnvioDocumentos = false;
            try
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.DocumentosFiscais documentoFiscal in listaDocumentosFiscais)
                {
                    jsonRequest = JsonConvert.SerializeObject(documentoFiscal, Formatting.Indented);
                    StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                    HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                    jsonResponse = result.Content.ReadAsStringAsync().Result;

                    mensagemErro = $"Documento {documentoFiscal.label} integrado com sucesso. Viagem ID Trizy: {carga.IDIdentificacaoTrizzy}";

                    if (!result.IsSuccessStatusCode)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Retorno>(result.Content.ReadAsStringAsync().Result);
                        if (retorno == null)
                            mensagemErro = result.StatusCode.ToString();
                        else
                            mensagemErro = retorno.message;

                        if (mensagemErro.Contains("Could not create the document") && retorno != null)
                            mensagemErro = $"Não foi possível integrar o PDF do documento Documento {documentoFiscal.label}. Motivo da falha: " + retorno.error;

                        falhaEnvioDocumentos = true;
                    }

                    SalvarArquivoTransacaoIntegrarCargaAPPTrizy(servicoArquivoTransacao, jsonRequest, jsonResponse, "Retorno Trizy: " + mensagemErro, cargaCargaIntegracao);
                }
            }
            catch (Exception excecao)
            {
                falhaEnvioDocumentos = true;

                Servicos.Log.TratarErro($"{id} - {excecao}", "IntegracaoTrizy");
                Servicos.Log.TratarErro($"{id} - Request: " + jsonRequest, "IntegracaoTrizy");
                Servicos.Log.TratarErro($"{id} - Response: " + jsonResponse, "IntegracaoTrizy");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço de Envio do PDF dos Documentos da Trizy.";
                SalvarArquivoTransacaoIntegrarCargaAPPTrizy(servicoArquivoTransacao, jsonRequest, jsonResponse, mensagemErro, cargaCargaIntegracao);
            }

            if (falhaEnvioDocumentos)
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);

                AtualizarViagem(carga, "CANCELED", unitOfWork, null, false, null, false, null, cargaCargaIntegracao, " - Viagem cancelada via Falha no Envio de Documentos (PDF)", cancelamentoPorReenvio: true);
                throw new ServicoException("Falha envio dos documentos (PDF) para app trizy");
            }
        }

        private static dynamic ObterInformacoesGR(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return new
            {
                name = carga.TipoOperacao.ConfiguracaoTrizy.DescricaoEmpresaGR ?? string.Empty,
                document = ObterDocumentCargaAPP("CNPJ", carga.TipoOperacao.ConfiguracaoTrizy.CNPJEmpresaGR ?? string.Empty)
            };
        }
        private static Company ObterInformacoesGRV3(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return new Company()
            {
                name = carga.TipoOperacao.ConfiguracaoTrizy.DescricaoEmpresaGR ?? string.Empty,
                document = ObterDocumentCargaAPPV3("CNPJ", carga.TipoOperacao.ConfiguracaoTrizy.CNPJEmpresaGR ?? string.Empty)
            };
        }
        public void ReenviarIntegracaoTrizyDadosTransporte(int codigoCarga)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || (!configuracaoIntegracao.PossuiIntegracaoTrizy || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLTrizy)))
                return;

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> listaCargaDadosTrasporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCarga(codigoCarga, SituacaoIntegracao.ProblemaIntegracao, TipoIntegracao.Trizy);

            if (listaCargaDadosTrasporteIntegracao.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao in listaCargaDadosTrasporteIntegracao)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                cargaDadosTransporteIntegracao.NumeroTentativas = 0;
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
            }
        }

        private static void ValidarNotasFiscaisControleEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade servControleEntregaQualidade = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade(unitOfWork, null);

            int codCargaSemNota = repCarga.ValidarCargaEntregasSemNotas(carga.Codigo);

            if (codCargaSemNota > 0)
                servControleEntregaQualidade.AjustarNotasFiscaisControleEntrega(codCargaSemNota, new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema });

            int codCargaSemItemNota = repCarga.ValidarCargaEntregasSemProdutos(carga.Codigo);

            if (codCargaSemItemNota > 0)
                servControleEntregaQualidade.AjustarItensNotasFiscaisControleEntrega(codCargaSemItemNota, new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema });
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.TextoInternacionalizado GerarTextoInternacionalizado(string texto)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.TextoInternacionalizado retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.TextoInternacionalizado();
            retorno.pt = texto;
            retorno.en = texto;
            retorno.es = texto;

            return retorno;
        }
        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Linguagem GerarTextoLinguagem(string texto)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Linguagem retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.Linguagem();
            retorno.pt = texto;
            retorno.en = texto;
            retorno.es = texto;

            return retorno;
        }


        private async Task<GrupoMotoristaCreateRequest> ObterGrupoMotoristaCreateRequest(Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao grupoMotoristasIntegracao, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionario repositorioGrupoMotoristasFuncionario = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionario(_unitOfWork, cancellationToken);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.GrupoMotoristaCreateRequest objetoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.GrupoMotoristaCreateRequest();

            List<Dominio.Entidades.Usuario> funcionarios = await repositorioGrupoMotoristasFuncionario.BuscarFuncionariosPorGrupoMotoristaAsync(grupoMotoristasIntegracao.GrupoMotoristas.Codigo, cancellationToken);

            if (funcionarios.Count == 0)
                throw new ServicoException("Grupo sem motoristas");

            objetoRequest.name = grupoMotoristasIntegracao.GrupoMotoristas.Descricao;
            objetoRequest.description = grupoMotoristasIntegracao.GrupoMotoristas.Descricao;

            objetoRequest.drivers = ObterGrupoMotoristaCreateRequestFuncionarios(funcionarios);

            return objetoRequest;
        }

        private async Task<GrupoMotoristaUpdateRequest> ObterGrupoMotoristaUpdateRequest(Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao grupoMotoristasIntegracao, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionarioAlteracao repositorioGrupoMotoristasFuncionarioAlteracao = new Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionarioAlteracao(_unitOfWork, cancellationToken);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.GrupoMotoristaUpdateRequest objetoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.GrupoMotoristaUpdateRequest();

            //List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasFuncionarioAlteracao> funcionarios = await repositorioGrupoMotoristasFuncionarioAlteracao.BuscarFuncionariosAlteracaoPorGrupoMotoristaAsync(grupoMotoristasIntegracao.GrupoMotoristas.Codigo, cancellationToken);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionarioAlteracao> funcionarios = await repositorioGrupoMotoristasFuncionarioAlteracao.BuscarFuncionariosAlteracaoMotoristaAsync(grupoMotoristasIntegracao.GrupoMotoristas.Codigo, cancellationToken);

            objetoRequest.addDrivers = ObterDrivers(funcionarios.Where(w => w.Acao == GrupoMotoristaAtualizarAcao.Adicionar).ToList());

            objetoRequest.removeDrivers = ObterDrivers(funcionarios.Where(w => w.Acao == GrupoMotoristaAtualizarAcao.Deletar).ToList());

            return objetoRequest;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Driver[] ObterDrivers(List<Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasFuncionarioAlteracao> grupoMotoristasFuncionarioAlteracaos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Driver> motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Driver>();

            foreach (var item in grupoMotoristasFuncionarioAlteracaos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Driver motorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Driver();

                motorista.document = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Document()
                {
                    type = "CPF",
                    value = item.CPF
                };

                motorista.fullName = item.Nome;

                string celular = item.Celular.ObterTelefoneFormatadoCom55();

                if (!string.IsNullOrEmpty(celular))
                {
                    motorista.phones = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Phone[] {
                            new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Phone()
                            {
                                type = "CELLPHONE",
                                value = celular
                            }
                        };
                }

                motorista.workMode = "SELF_EMPLOYED";

                motoristas.Add(motorista);
            }

            return motoristas.ToArray();
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Driver[] ObterGrupoMotoristaCreateRequestFuncionarios(List<Dominio.Entidades.Usuario> funcionarios)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Driver> motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Driver>();

            foreach (Dominio.Entidades.Usuario funcionario in funcionarios)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Driver motorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Driver();

                motorista.document = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Document()
                {
                    type = "CPF",
                    value = funcionario.CPF
                };

                motorista.fullName = funcionario.Nome;

                string celular = funcionario.Celular.ObterTelefoneFormatadoCom55();

                if (!string.IsNullOrEmpty(celular))
                {
                    motorista.phones = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Phone[] {
                            new Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista.Phone()
                            {
                                type = "CELLPHONE",
                                value = celular
                            }
                        };
                }

                motorista.workMode = "SELF_EMPLOYED";

                motoristas.Add(motorista);
            }

            return motoristas.ToArray();
        }

        private static void validarRegrasV3(List<Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa> listaChecklistSuperAppEtapa, Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTrizy, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (listaChecklistSuperAppEtapa.Any(etapa => etapa.Tipo == TipoEtapaChecklistSuperApp.LoadDetail) && string.IsNullOrEmpty(carga?.ModeloVeicularCarga?.LayoutSuperAppId ?? ""))
            {
                throw new ServicoException($"Existe uma etapa de comprovação do tipo Detalhes da Carga, porém o Modelo Veicular de Carga não está cadastrado na Trizy.");
            }
            if (listaChecklistSuperAppEtapa.Any(etapa => etapa.Tipo == TipoEtapaChecklistSuperApp.LoadDetail) && !configuracaoTrizy.HabilitarEnvioRelatorio)
            {
                throw new ServicoException($"Existe uma etapa de comprovação do tipo Detalhes da Carga, porém o envio de relatório não está habilitado");
            }
        }
        #endregion
    }
}