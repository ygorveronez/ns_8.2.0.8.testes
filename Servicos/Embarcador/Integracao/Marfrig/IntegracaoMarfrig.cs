using Dominio.Entidades.Embarcador.Financeiro;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Marfrig
{
    public class IntegracaoMarfrig
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        private string _jsonrequest;
        private string _jsonresponse;
        private int _tentativaConsultaAcrescimosDecrescimos = 1;

        #endregion

        #region Construtores

        public IntegracaoMarfrig(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IntegracaoMarfrig(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMarfrig repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoMarfrig(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null)
                throw new ServicoException("Não existe configuração de integração disponível para a Marfrig.");

            return configuracaoIntegracao;
        }

        private static HttpClient ObterClient(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMarfrig));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.URLRequisicaoImpressaoDocumentosCarga);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Add("apikey", configuracaoIntegracao.ApiKey);

            return requisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RequisicaoImpressaoDocumentos PreencherRequisicaoImpressaoDocumentos(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente clienteFilial = repCliente.BuscarPorCPFCNPJ(cargaIntegracao.Carga.Filial?.CNPJ.ToDouble() ?? 0);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RequisicaoImpressaoDocumentos
            {
                status = 1,
                numeroOrdemEmbarque = cargaIntegracao.Carga.CodigoCargaEmbarcador.ToInt(),
                idEmpresa = clienteFilial?.CodigoIntegracao ?? string.Empty,
                mensagem = "multiembarcador",
            };
        }

        private string EnviarDocumentosParaImpressao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Servicos.Embarcador.Carga.Impressao serCargaImpressao = new Servicos.Embarcador.Carga.Impressao(_unitOfWork);

            string retorno = serCargaImpressao.EnviarDocumentosParaImpressao(carga);

            if (!string.IsNullOrWhiteSpace(retorno))
                return retorno;

            retorno = serCargaImpressao.EnviarMDFeParaImpressao(carga);

            return retorno;
        }

        private bool IsRetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created;
        }

        private bool RetornarCarga(int codigoCarga, string endPoint, ref string jsonRequest, ref string jsonResponse, out string erro)
        {
            try
            {
                erro = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);


                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

                if (listaCargaPedidos != null && listaCargaPedidos.Count > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnvioCarga envioCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnvioCarga();
                    envioCarga.cnpjFilial = Utilidades.String.OnlyNumbers(listaCargaPedidos.FirstOrDefault().Carga.Filial.CNPJ);
                    envioCarga.protocoloCarga = listaCargaPedidos.FirstOrDefault().Carga.Codigo;
                    envioCarga.statusEmissao = 1;
                    envioCarga.mensagem = "documentos emitidos com sucesso";
                    envioCarga.protocolosPedido = new List<int>();
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedidos)
                        envioCarga.protocolosPedido.Add(cargaPedido.Pedido.Codigo);

                    try
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

                        HttpClient client = ObterClient(configuracaoIntegracao);
                        client.BaseAddress = new Uri(endPoint);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        jsonRequest = JsonConvert.SerializeObject(envioCarga, Formatting.Indented);
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(endPoint, content).Result;

                        jsonResponse = result.Content.ReadAsStringAsync().Result;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoCarga retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoCarga>(result.Content.ReadAsStringAsync().Result);

                        if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
                            return true;
                        else
                        {
                            erro = retorno.Mensagem;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        erro = "Não foi possível retornar carga para Marfrig";
                        return false;
                    }
                }
                else
                {
                    erro = "Nenhuma CargaPedido localizada para retornar";
                    return false;
                }


            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                erro = "Falha ao retornar carga para Marfrig";
                return false;
            }
        }

        private bool RetornarCargaRedespacho(Dominio.Entidades.Embarcador.Cargas.Carga carga, string endPoint, ref string jsonRequest, ref string jsonResponse, out string erro)
        {
            try
            {
                erro = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
                Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(_unitOfWork);


                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                List<string> listaCTes = repCargaCTe.BuscarCodigosCTePorCarga(carga.Codigo);
                List<string> listaNFSes = repCargaCTe.BuscarCodigosStringNFSePorCarga(carga.Codigo);

                string problema = string.Empty;
                if (listaCTes.Count > 0)
                {
                    int.TryParse(listaCTes[0], out int codigoCTe); // listaCargaPedidos.FirstOrDefault().Pedido.ObservacaoCTe
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                    problema = cte?.ObservacoesGerais ?? string.Empty;
                }

                Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal = carga.Redespacho.Carga;

                if (listaCargaPedidos != null && listaCargaPedidos.Count > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnviarCTeComplementar envioCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnviarCTeComplementar()
                    {
                        ocorrenciaMultiEmbarcador = carga.Codigo.ToString(),
                        protocoloCTe = listaCTes,
                        protocoloNFSe = listaNFSes,
                        motivo = "M00005",
                        problema = problema,
                        dtOcorrencia = carga.DataCriacaoCarga.ToString("yyyy-MM-dd"),
                        horaOcorrencia = carga.DataCriacaoCarga.ToString("HH:mm"),
                        transportador = cargaOriginal?.Empresa?.CNPJ ?? string.Empty, //CNPJ do transportador da carga original
                        prestadorServico = carga.Empresa?.CNPJ ?? string.Empty, //CNPJ do transportador da carga redespachada
                        CNPJFilial = carga.Filial.CNPJ,
                        userCriacao = carga.Operador?.Login ?? string.Empty,
                        userBaixa = carga.Operador?.Login ?? string.Empty,
                        quantidadeServicos = carga.ValorFreteAPagar
                    };

                    envioCarga.notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscais>();
                    int sequencial = 0;
                    foreach (string codigoCTe in listaCTes)
                    {
                        List<Dominio.Entidades.DocumentosCTE> listaDocumentosCTe = repDocumentosCTE.BuscarPorCTe(int.Parse(codigoCTe));
                        foreach (Dominio.Entidades.DocumentosCTE documentoCTe in listaDocumentosCTe)
                        {
                            sequencial += 1;
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscais notaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscais();
                            notaFiscal.chaveNFe = documentoCTe.ChaveNFE;
                            notaFiscal.emissorDoc = documentoCTe.ChaveNFE.Substring(6, 14);
                            notaFiscal.cnpjFilial = documentoCTe.ChaveNFE.Substring(6, 14);
                            notaFiscal.numeroNF = documentoCTe.Numero.PadLeft(9, '0');
                            notaFiscal.serieNF = documentoCTe.Serie;
                            notaFiscal.sequenciaTrecho = "01";

                            notaFiscal.emissorDoc = ValidaDocumentoPorChaveNf(notaFiscal.emissorDoc);
                            notaFiscal.cnpjFilial = ValidaDocumentoPorChaveNf(notaFiscal.cnpjFilial);

                            envioCarga.notasFiscais.Add(notaFiscal);
                        }
                    }
                    foreach (string codigoNFSe in listaNFSes)
                    {
                        List<Dominio.Entidades.DocumentosCTE> listaDocumentosNFSe = repDocumentosCTE.BuscarPorCTe(int.Parse(codigoNFSe));
                        foreach (Dominio.Entidades.DocumentosCTE documentoNFSe in listaDocumentosNFSe)
                        {
                            sequencial += 1;
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscais notaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscais();
                            notaFiscal.chaveNFe = documentoNFSe.ChaveNFE;
                            notaFiscal.emissorDoc = documentoNFSe.ChaveNFE.Substring(6, 14);
                            notaFiscal.cnpjFilial = documentoNFSe.ChaveNFE.Substring(6, 14);
                            notaFiscal.numeroNF = documentoNFSe.Numero.PadLeft(9, '0');
                            notaFiscal.serieNF = documentoNFSe.Serie;
                            notaFiscal.sequenciaTrecho = "01";

                            notaFiscal.emissorDoc = ValidaDocumentoPorChaveNf(notaFiscal.emissorDoc);
                            notaFiscal.cnpjFilial = ValidaDocumentoPorChaveNf(notaFiscal.cnpjFilial);

                            envioCarga.notasFiscais.Add(notaFiscal);
                        }
                    }

                    try
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

                        HttpClient client = ObterClient(configuracaoIntegracao);
                        client.BaseAddress = new Uri(endPoint);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        jsonRequest = JsonConvert.SerializeObject(envioCarga, Formatting.Indented);
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(endPoint, content).Result;

                        jsonResponse = result.Content.ReadAsStringAsync().Result;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoCarga retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoCarga>(result.Content.ReadAsStringAsync().Result);

                        if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
                            return true;
                        else
                        {
                            erro = retorno.Mensagem;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        erro = "Não foi possível retornar carga para Marfrig";
                        return false;
                    }
                }
                else
                {
                    erro = "Nenhuma CargaPedido localizada para retornar";
                    return false;
                }


            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                erro = "Falha ao retornar carga para Marfrig";
                return false;
            }
        }

        private bool RetornarNotaDebitoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, string endPoint, ref string jsonRequest, ref string jsonResponse, out string erro, out string protocolo)
        {
            try
            {
                erro = "";
                protocolo = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(_unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);


                if (ocorrenciaCTeIntegracao != null && ocorrenciaCTeIntegracao.CargaCTe != null && ocorrenciaCTeIntegracao.CargaCTe.CTe != null)
                {
                    bool enviarTagsComTomador = ocorrenciaCTeIntegracao.CargaOcorrencia.Carga?.TipoOperacao?.ConfiguracaoIntegracao?.EnviarTagsIntegracaoMarfrigComTomadorServico ?? false;

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao cargaOcorrenciaAutorizacao = repCargaOcorrenciaAutorizacao.BuscarUltimoAprovadorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia);
                    if (cargaOcorrenciaAutorizacao == null)
                        cargaOcorrenciaAutorizacao = repCargaOcorrenciaAutorizacao.BuscarUltimoAprovadorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia);

                    Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal = ocorrenciaCTeIntegracao.CargaOcorrencia.Carga?.Redespacho != null ? ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.Redespacho.Carga : null;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnviarNotaDebito enviarND = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnviarNotaDebito();

                    enviarND.arquivo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.ArquivoND();
                    enviarND.arquivo.cnpjRemetente = ocorrenciaCTeIntegracao.CargaCTe.CTe.Remetente.CPF_CNPJ;
                    enviarND.arquivo.transportador = cargaOriginal?.Empresa != null ? cargaOriginal.Empresa?.CNPJ : ocorrenciaCTeIntegracao.CargaCTe.CTe.Empresa.CNPJ;
                    enviarND.arquivo.prestadorServico = cargaOriginal != null ? ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.Empresa?.CNPJ : "";
                    enviarND.arquivo.cnpjFilial = enviarTagsComTomador ? ocorrenciaCTeIntegracao.CargaCTe.CTe.Tomador.CPF_CNPJ : ocorrenciaCTeIntegracao.CargaOcorrencia.Carga != null ? ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.Filial.CNPJ : ocorrenciaCTeIntegracao.CargaOcorrencia.Cargas != null && ocorrenciaCTeIntegracao.CargaOcorrencia.Cargas.Count > 0 ? ocorrenciaCTeIntegracao.CargaOcorrencia.Cargas.FirstOrDefault().Filial.CNPJ : ocorrenciaCTeIntegracao.CargaCTe.CTe.Remetente.CPF_CNPJ;
                    enviarND.arquivo.numeroDocumento = ocorrenciaCTeIntegracao.CargaCTe.CTe.Numero.ToString();
                    enviarND.arquivo.serieNF = ocorrenciaCTeIntegracao.CargaCTe.CTe.Serie.Numero.ToString();
                    enviarND.arquivo.valorDocumento = ocorrenciaCTeIntegracao.CargaCTe.CTe.ValorAReceber;
                    enviarND.arquivo.dataEmissao = ocorrenciaCTeIntegracao.CargaCTe.CTe.DataEmissao.HasValue ? ocorrenciaCTeIntegracao.CargaCTe.CTe.DataEmissao.Value.ToString("yyyy-MM-dd") : DateTime.Today.ToString("yyyy-MM-dd");

                    enviarND.ocorrencia = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Ocorrencia();
                    enviarND.ocorrencia.ocorrenciaMultiembarcador = ocorrenciaCTeIntegracao.CargaOcorrencia.NumeroOcorrencia.ToString();
                    enviarND.ocorrencia.motivo = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.CodigoIntegracao;//ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.Descricao;
                    enviarND.ocorrencia.problema = ocorrenciaCTeIntegracao.CargaOcorrencia.Observacao;
                    enviarND.ocorrencia.dtOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia.ToString("yyyy-MM-dd");
                    enviarND.ocorrencia.horaOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia.ToString("HH:mm");
                    enviarND.ocorrencia.userCriacao = ocorrenciaCTeIntegracao.CargaOcorrencia.Usuario?.Login ?? string.Empty;
                    enviarND.ocorrencia.userBaixa = cargaOcorrenciaAutorizacao != null ? cargaOcorrenciaAutorizacao.Usuario?.Login ?? string.Empty : string.Empty;
                    enviarND.ocorrencia.quantidadeServicos = ocorrenciaCTeIntegracao.CargaOcorrencia.ValorOcorrencia;

                    //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.Codigo);
                    //enviarND.outros.quantidadeDocumentos = cargaPedidos.Count();

                    enviarND.outros = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OutrosND();
                    if (ocorrenciaCTeIntegracao.CargaOcorrencia.Carga != null)
                        enviarND.outros.valorCarga = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.Codigo);
                    else if (ocorrenciaCTeIntegracao.CargaOcorrencia.Cargas != null && ocorrenciaCTeIntegracao.CargaOcorrencia.Cargas.Count > 0)
                        enviarND.outros.valorCarga = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(ocorrenciaCTeIntegracao.CargaOcorrencia.Cargas.FirstOrDefault().Codigo);
                    else
                        enviarND.outros.valorCarga = ocorrenciaCTeIntegracao.CargaCTe.CTe.ValorTotalMercadoria;

                    List<Dominio.Entidades.DocumentosCTE> listaDocumentosCTe = repDocumentosCTE.BuscarPorCTe(ocorrenciaCTeIntegracao.CargaCTe.CTe.Codigo);
                    enviarND.outros.quantidadeDocumentos = listaDocumentosCTe.Count();

                    enviarND.notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscaisND>();
                    int sequencial = 0;

                    foreach (Dominio.Entidades.DocumentosCTE documentoCTe in listaDocumentosCTe)
                    {
                        sequencial += 1;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscaisND notaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscaisND();
                        notaFiscal.chaveNFe = documentoCTe.ChaveNFE;
                        notaFiscal.emissorDoc = !string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE) ? documentoCTe.ChaveNFE.Substring(6, 14) : ocorrenciaCTeIntegracao.CargaCTe.CTe.Remetente.CPF_CNPJ;
                        notaFiscal.cnpjFilial = enviarTagsComTomador ? ocorrenciaCTeIntegracao.CargaCTe.CTe.Tomador.CPF_CNPJ : !string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE) ? documentoCTe.ChaveNFE.Substring(6, 14) : ocorrenciaCTeIntegracao.CargaCTe.CTe.Remetente.CPF_CNPJ;
                        notaFiscal.numeroNF = documentoCTe.Numero.PadLeft(9, '0');
                        notaFiscal.sequencial = sequencial.ToString();
                        notaFiscal.serieNF = documentoCTe.Serie;
                        notaFiscal.tipoDoc = enviarTagsComTomador ? "NFE" : "NFS";
                        notaFiscal.sequenciaTrecho = "01";

                        enviarND.notasFiscais.Add(notaFiscal);
                    }

                    try
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

                        HttpClient client = ObterClient(configuracaoIntegracao);
                        client.BaseAddress = new Uri(endPoint);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        jsonRequest = JsonConvert.SerializeObject(enviarND, Formatting.Indented);
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(endPoint, content).Result;

                        jsonResponse = result.Content.ReadAsStringAsync().Result;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoNotaDebito retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoNotaDebito>(result.Content.ReadAsStringAsync().Result);

                        if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            if (!string.IsNullOrEmpty(retorno.idIntegracao))
                            {
                                ocorrenciaCTeIntegracao.CargaCTe.CTe.Protocolo = retorno.idIntegracao;
                                repCte.Atualizar(ocorrenciaCTeIntegracao.CargaCTe.CTe);

                                protocolo = "Protocolo: " + retorno.idIntegracao;
                            }

                            return true;
                        }
                        else
                        {
                            erro = retorno.Mensagem;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        erro = "Não foi possível retornar documento ocorrência para Marfrig";
                        return false;
                    }
                }
                else
                {
                    erro = "Nenhum documento da ocorrencia localizado para retornar";
                    return false;
                }


            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                erro = "Falha ao retornar ocorrência para Marfrig";
                protocolo = "";

                return false;
            }
        }

        private bool RetornarOcorrenciaComParcela(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, string endPoint, ref string jsonRequest, ref string jsonResponse, out string erro, out string protocolo)
        {
            try
            {
                erro = "";
                protocolo = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(_unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

                if (ocorrenciaCTeIntegracao != null && ocorrenciaCTeIntegracao.CargaCTe != null && ocorrenciaCTeIntegracao.CargaCTe.CTe != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnviarOcorrenciaComParcela enviarOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnviarOcorrenciaComParcela();
                    enviarOcorrencia.acao = "I";
                    enviarOcorrencia.dataEmissao = ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia.ToString("yyyy-MM-dd");
                    enviarOcorrencia.numeroND = ocorrenciaCTeIntegracao.CargaCTe.CTe.Numero.ToString();
                    enviarOcorrencia.serieND = ocorrenciaCTeIntegracao.CargaCTe.CTe.Serie.Numero.ToString();
                    enviarOcorrencia.cnpjRemetente = ocorrenciaCTeIntegracao.CargaCTe.CTe.Remetente.CPF_CNPJ;
                    enviarOcorrencia.cnpjTomador = ocorrenciaCTeIntegracao.CargaCTe.CTe.TomadorPagador.CPF_CNPJ ?? string.Empty;
                    enviarOcorrencia.transportador = ocorrenciaCTeIntegracao.CargaCTe.CTe.Empresa.CNPJ;
                    enviarOcorrencia.ocorrenciaMulti = ocorrenciaCTeIntegracao.CargaOcorrencia.NumeroOcorrencia.ToString();
                    enviarOcorrencia.problema = ocorrenciaCTeIntegracao.CargaOcorrencia.Observacao;

                    enviarOcorrencia.carga = ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.CodigoCargaEmbarcador;
                    enviarOcorrencia.motivo = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia?.CodigoIntegracao ?? string.Empty;
                    enviarOcorrencia.descricaomotivo = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia?.Descricao ?? string.Empty;
                    enviarOcorrencia.qtdservicos = ocorrenciaCTeIntegracao.CargaCTe.CTe.ValorAReceber;

                    enviarOcorrencia.valorDocumento = ocorrenciaCTeIntegracao.CargaCTe.CTe.ValorAReceber;
                    enviarOcorrencia.qtdParcela = ocorrenciaCTeIntegracao.CargaOcorrencia.QuantidadeParcelas > 0 ? ocorrenciaCTeIntegracao.CargaOcorrencia.QuantidadeParcelas : 1;

                    List<Dominio.Entidades.DocumentosCTE> listaDocumentosCTe = repDocumentosCTE.BuscarPorCTe(ocorrenciaCTeIntegracao.CargaCTe.CTe.Codigo);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> listaCargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo);

                    enviarOcorrencia.cteOrigem = listaCargaCTeComplementoInfo.Count > 0 ? listaCargaCTeComplementoInfo.FirstOrDefault().CargaCTeComplementado.CTe.Numero.ToString() : "";
                    enviarOcorrencia.serieOrigem = listaCargaCTeComplementoInfo.Count > 0 ? listaCargaCTeComplementoInfo.FirstOrDefault().CargaCTeComplementado.CTe.Serie.Numero.ToString() : "";
                    enviarOcorrencia.nfeOrigem = listaDocumentosCTe.Count > 0 ? listaDocumentosCTe.FirstOrDefault().SerieOuSerieDaChave : "";

                    try
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

                        HttpClient client = ObterClient(configuracaoIntegracao); client.BaseAddress = new Uri(endPoint);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        jsonRequest = JsonConvert.SerializeObject(enviarOcorrencia, Formatting.Indented);
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(endPoint, content).Result;

                        jsonResponse = result.Content.ReadAsStringAsync().Result;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoNotaDebito retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoNotaDebito>(result.Content.ReadAsStringAsync().Result);

                        if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            if (!string.IsNullOrEmpty(retorno.idIntegracao))
                            {
                                ocorrenciaCTeIntegracao.CargaCTe.CTe.Protocolo = retorno.idIntegracao;
                                repCte.Atualizar(ocorrenciaCTeIntegracao.CargaCTe.CTe);

                                protocolo = "Protocolo: " + retorno.idIntegracao;
                            }

                            return true;
                        }
                        else
                        {
                            erro = retorno.Mensagem;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        erro = "Não foi possível retornar documento ocorrência para Marfrig";
                        return false;
                    }
                }
                else
                {
                    erro = "Nenhum documento da ocorrencia localizado para retornar";
                    return false;
                }


            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                erro = "Falha ao retornar ocorrência com parcela para Marfrig";
                protocolo = "";
                return false;
            }
        }

        private bool RetornarCTeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, string endPoint, ref string jsonRequest, ref string jsonResponse, out string erro)
        {
            try
            {
                erro = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

                if (ocorrenciaCTeIntegracao != null && ocorrenciaCTeIntegracao.CargaCTe != null && ocorrenciaCTeIntegracao.CargaCTe.CTe != null)
                {
                    List<string> listaCTes = repCargaCTe.BuscarCodigosCTePorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo);
                    List<string> listaNFSes = repCargaCTe.BuscarCodigosNFSePorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo);

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao cargaOcorrenciaAutorizacao = repCargaOcorrenciaAutorizacao.BuscarUltimoAprovadorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia);
                    if (cargaOcorrenciaAutorizacao == null)
                        cargaOcorrenciaAutorizacao = repCargaOcorrenciaAutorizacao.BuscarUltimoAprovadorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia);

                    Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal = ocorrenciaCTeIntegracao.CargaOcorrencia.Carga?.Redespacho != null ? ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.Redespacho.Carga : null;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnviarCTeComplementar enviarCTeComolementar = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnviarCTeComplementar()
                    {
                        ocorrenciaMultiEmbarcador = ocorrenciaCTeIntegracao.CargaOcorrencia.NumeroOcorrencia.ToString(),
                        protocoloCTe = listaCTes, //ocorrenciaCTeIntegracao.CargaCTe.CTe.Codigo.ToString()
                        protocoloNFSe = listaNFSes,
                        motivo = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.CodigoIntegracao,//ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.Descricao,
                        problema = ocorrenciaCTeIntegracao.CargaOcorrencia.Observacao,
                        dtOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia.ToString("yyyy-MM-dd"),
                        horaOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia.ToString("HH:mm"),
                        transportador = cargaOriginal?.Empresa != null ? cargaOriginal.Empresa?.CNPJ : ocorrenciaCTeIntegracao.CargaCTe.CTe.Empresa.CNPJ,
                        prestadorServico = cargaOriginal != null ? ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.Empresa?.CNPJ : "",
                        CNPJFilial = ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.Filial.CNPJ,
                        userCriacao = ocorrenciaCTeIntegracao.CargaOcorrencia.Usuario?.Login ?? string.Empty,
                        userBaixa = cargaOcorrenciaAutorizacao != null ? cargaOcorrenciaAutorizacao.Usuario?.Login ?? string.Empty : string.Empty,
                        quantidadeServicos = ocorrenciaCTeIntegracao.CargaOcorrencia.ValorOcorrencia
                    };

                    enviarCTeComolementar.notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscais>();
                    int sequencial = 0;
                    foreach (string codigoCTe in listaCTes)
                    {
                        List<Dominio.Entidades.DocumentosCTE> listaDocumentosCTe = repDocumentosCTE.BuscarPorCTe(int.Parse(codigoCTe));
                        foreach (Dominio.Entidades.DocumentosCTE documentoCTe in listaDocumentosCTe)
                        {
                            sequencial += 1;
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscais notaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscais();
                            notaFiscal.chaveNFe = documentoCTe.ChaveNFE;
                            notaFiscal.emissorDoc = documentoCTe.ChaveNFE.Substring(6, 14);
                            notaFiscal.cnpjFilial = documentoCTe.ChaveNFE.Substring(6, 14);
                            notaFiscal.numeroNF = documentoCTe.Numero.PadLeft(9, '0');
                            notaFiscal.serieNF = documentoCTe.Serie;
                            notaFiscal.sequenciaTrecho = "01";

                            notaFiscal.emissorDoc = ValidaDocumentoPorChaveNf(notaFiscal.emissorDoc);
                            notaFiscal.cnpjFilial = ValidaDocumentoPorChaveNf(notaFiscal.cnpjFilial);

                            enviarCTeComolementar.notasFiscais.Add(notaFiscal);
                        }
                    }
                    foreach (string codigoNFSe in listaNFSes)
                    {
                        List<Dominio.Entidades.DocumentosCTE> listaDocumentosNFSe = repDocumentosCTE.BuscarPorCTe(int.Parse(codigoNFSe));
                        foreach (Dominio.Entidades.DocumentosCTE documentoNFSe in listaDocumentosNFSe)
                        {
                            sequencial += 1;
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscais notaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscais();
                            notaFiscal.chaveNFe = documentoNFSe.ChaveNFE;
                            notaFiscal.emissorDoc = documentoNFSe.ChaveNFE.Substring(6, 14);
                            notaFiscal.cnpjFilial = documentoNFSe.ChaveNFE.Substring(6, 14);
                            notaFiscal.numeroNF = documentoNFSe.Numero.PadLeft(9, '0');
                            notaFiscal.serieNF = documentoNFSe.Serie;
                            notaFiscal.sequenciaTrecho = "01";

                            notaFiscal.emissorDoc = ValidaDocumentoPorChaveNf(notaFiscal.emissorDoc);
                            notaFiscal.cnpjFilial = ValidaDocumentoPorChaveNf(notaFiscal.cnpjFilial);

                            enviarCTeComolementar.notasFiscais.Add(notaFiscal);
                        }
                    }

                    try
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

                        HttpClient client = ObterClient(configuracaoIntegracao); client.BaseAddress = new Uri(endPoint);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        jsonRequest = JsonConvert.SerializeObject(enviarCTeComolementar, Formatting.Indented);
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(endPoint, content).Result;

                        jsonResponse = result.Content.ReadAsStringAsync().Result;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoNotaDebito retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoNotaDebito>(result.Content.ReadAsStringAsync().Result);

                        if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
                            return true;
                        else
                        {
                            erro = retorno.Mensagem;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        erro = "Não foi possível retornar CTe Ocorrencia para Marfrig";
                        return false;
                    }
                }
                else
                {
                    erro = "Nenhum documento da ocorrencia localizado para retornar";
                    return false;
                }


            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                erro = "Falha ao retornar ocorrência para Marfrig";
                return false;
            }
        }

        private bool RetornarNFSeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, string endPoint, ref string jsonRequest, ref string jsonResponse, out string erro)
        {
            try
            {
                erro = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

                if (ocorrenciaCTeIntegracao != null && ocorrenciaCTeIntegracao.CargaCTe != null && ocorrenciaCTeIntegracao.CargaCTe.CTe != null)
                {
                    List<string> listaCTes = repCargaCTe.BuscarCodigosCTesPorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo);

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao cargaOcorrenciaAutorizacao = repCargaOcorrenciaAutorizacao.BuscarUltimoAprovadorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia);
                    if (cargaOcorrenciaAutorizacao == null)
                        cargaOcorrenciaAutorizacao = repCargaOcorrenciaAutorizacao.BuscarUltimoAprovadorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnviarNFSeComplementar enviarNFSeComplementar = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.EnviarNFSeComplementar()
                    {
                        transportador = ocorrenciaCTeIntegracao.CargaCTe.CTe.Empresa.CNPJ,
                        numeroDocumento = ocorrenciaCTeIntegracao.CargaCTe.CTe.Numero.ToString(),
                        dataEmissao = ocorrenciaCTeIntegracao.CargaCTe.CTe.DataEmissao.Value.ToString("yyyy-MM-dd"),
                        cnpjFilial = ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.Filial.CNPJ,
                        cnpjRemetente = ocorrenciaCTeIntegracao.CargaCTe.CTe.Remetente.CPF_CNPJ,
                        valorDocumento = ocorrenciaCTeIntegracao.CargaCTe.CTe.ValorAReceber,
                        aliquotaISS = ocorrenciaCTeIntegracao.CargaCTe.CTe.AliquotaISS,
                        iSSBaseCalculo = ocorrenciaCTeIntegracao.CargaCTe.CTe.BaseCalculoISS,
                        percentualImpRetido = ocorrenciaCTeIntegracao.CargaCTe.CTe.PercentualISSRetido,
                        valorBaseCalculoISS = ocorrenciaCTeIntegracao.CargaCTe.CTe.ValorISSRetido > 0 ? ocorrenciaCTeIntegracao.CargaCTe.CTe.BaseCalculoISS : 0,
                        valorISS = ocorrenciaCTeIntegracao.CargaCTe.CTe.ValorISS,
                        valorRetencaoISS = ocorrenciaCTeIntegracao.CargaCTe.CTe.ValorISSRetido,
                        quantidadeVolumes = 0,
                        tipoTributacao = ocorrenciaCTeIntegracao.CargaCTe.CTe.ValorISSRetido > 0 ? 1 : 0,
                        codigoVerificacao = ocorrenciaCTeIntegracao.CargaCTe.CTe.Protocolo
                    };

                    enviarNFSeComplementar.valorCarga = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(ocorrenciaCTeIntegracao.CargaOcorrencia.Carga.Codigo);
                    List<Dominio.Entidades.DocumentosCTE> listaDocumentosCTe = repDocumentosCTE.BuscarPorCTe(ocorrenciaCTeIntegracao.CargaCTe.CTe.Codigo);
                    enviarNFSeComplementar.quantidadeDocumentos = listaDocumentosCTe.Count();

                    int sequencial = 0;
                    enviarNFSeComplementar.notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscaisND>();
                    foreach (Dominio.Entidades.DocumentosCTE documentoCTe in listaDocumentosCTe)
                    {
                        sequencial += 1;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscaisND notaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.NotasFiscaisND();
                        notaFiscal.sequencial = sequencial.ToString();
                        notaFiscal.cnpjFilial = documentoCTe.ChaveNFE.Substring(6, 14);
                        notaFiscal.emissorDoc = documentoCTe.ChaveNFE.Substring(6, 14);
                        notaFiscal.serieNF = documentoCTe.Serie;
                        notaFiscal.numeroNF = documentoCTe.Numero.PadLeft(9, '0');
                        notaFiscal.chaveNFe = documentoCTe.ChaveNFE;
                        //notaFiscal.tipoDoc = "NFS";
                        //notaFiscal.sequenciaTrecho = "01";

                        notaFiscal.emissorDoc = ValidaDocumentoPorChaveNf(notaFiscal.emissorDoc);
                        notaFiscal.cnpjFilial = ValidaDocumentoPorChaveNf(notaFiscal.cnpjFilial);

                        enviarNFSeComplementar.notasFiscais.Add(notaFiscal);
                    }

                    enviarNFSeComplementar.ocorrencia = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Ocorrencia();
                    enviarNFSeComplementar.ocorrencia.ocorrenciaMultiembarcador = ocorrenciaCTeIntegracao.CargaOcorrencia.NumeroOcorrencia.ToString();
                    enviarNFSeComplementar.ocorrencia.motivo = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.CodigoIntegracao;//ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.Descricao;
                    enviarNFSeComplementar.ocorrencia.problema = ocorrenciaCTeIntegracao.CargaOcorrencia.Observacao;
                    enviarNFSeComplementar.ocorrencia.dtOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia.ToString("yyyy-MM-dd");
                    enviarNFSeComplementar.ocorrencia.horaOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia.ToString("HH:mm");
                    enviarNFSeComplementar.ocorrencia.userCriacao = ocorrenciaCTeIntegracao.CargaOcorrencia.Usuario?.Login ?? string.Empty;
                    enviarNFSeComplementar.ocorrencia.userBaixa = cargaOcorrenciaAutorizacao != null ? cargaOcorrenciaAutorizacao.Usuario?.Login ?? string.Empty : string.Empty;
                    enviarNFSeComplementar.ocorrencia.quantidadeServicos = ocorrenciaCTeIntegracao.CargaOcorrencia.ValorOcorrencia;

                    try
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

                        HttpClient client = ObterClient(configuracaoIntegracao); client.BaseAddress = new Uri(endPoint);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        jsonRequest = JsonConvert.SerializeObject(enviarNFSeComplementar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(endPoint, content).Result;

                        jsonResponse = result.Content.ReadAsStringAsync().Result;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoNotaDebito retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoNotaDebito>(result.Content.ReadAsStringAsync().Result);

                        if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
                            return true;
                        else
                        {
                            erro = retorno.Mensagem;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        erro = "Não foi possível retornar NFSe Ocorrencia para Marfrig";
                        return false;
                    }
                }
                else
                {
                    erro = "Nenhum documento da ocorrencia localizado para retornar";
                    return false;
                }


            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                erro = "Falha ao retornar ocorrência para Marfrig";
                return false;
            }
        }

        private bool IntegrarPreCalculoFrete(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao, ref string jsonRequest, ref string jsonResponse)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string endPoint = $"{configuracaoIntegracao.URLPreCalculoFrete}";
            mensagem = "";
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPreCalculoFrete repCargaPreCalculoFrete = new Repositorio.Embarcador.Cargas.CargaPreCalculoFrete(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(cargaDadosTransporteIntegracao.Carga.Codigo);

            decimal valorFretePreCalculo = 0;
            bool possuiValorFrete = carga.Pedidos.Any(o => o.ValorFrete > 0);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.PreCalculoFrete.PreCalculoFrete preCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.PreCalculoFrete.PreCalculoFrete()
            {
                protocoloCarga = carga.Protocolo,
                retorno = !possuiValorFrete && carga.MotivoPendenciaFrete != MotivoPendenciaFrete.NenhumPendencia ? carga.MotivoPendencia : "",
                pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.PreCalculoFrete.PreCalculoFretePedido>()
            };

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido Cargapedido in carga.Pedidos)
            {

                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.PreCalculoFrete.PreCalculoFretePedido preCalculoFretePedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.PreCalculoFrete.PreCalculoFretePedido()
                {
                    protocoloPedido = Cargapedido.Pedido.Protocolo,
                    valorFrete = Cargapedido.ValorFrete,
                    baseCalculo = Cargapedido.BaseCalculoICMS,
                    valorIcms = Cargapedido.ValorICMS,
                    aliquotaIcms = Cargapedido.PercentualAliquota,
                    tipoCarga = Cargapedido.Carga.TipoDeCarga.CodigoTipoCargaEmbarcador ?? "",
                    tipoOperacao = Cargapedido.Carga.TipoOperacao.CodigoIntegracao ?? "",
                    CST = Cargapedido.CST ?? ""
                };

                preCalculoFrete.pedidos.Add(preCalculoFretePedido);

                valorFretePreCalculo += Cargapedido.ValorFrete;
                if (Cargapedido == carga.Pedidos.Last())
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPreCalculoFrete cargaPreCalculoFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPreCalculoFrete
                    {
                        Carga = carga,
                        ValorFrete = valorFretePreCalculo,
                    };

                    repCargaPreCalculoFrete.Inserir(cargaPreCalculoFrete);
                }
            }

            try
            {

                HttpClient client = ObterClient(configuracaoIntegracao); client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                jsonRequest = JsonConvert.SerializeObject(preCalculoFrete, Formatting.Indented);
                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                var result = client.PostAsync(endPoint, content).Result;

                if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
                    return true;
                else
                {
                    mensagem = result.Content.ReadAsStringAsync().Result;
                    return false;
                }
            }

            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagem = "Não foi possível retornar CTe Ocorrencia para Marfrig";
                return false;
            }

        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoIntegracao RetornarTitulosIntegracao(Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber integracaoMarfigCteTitulo, Dominio.Entidades.ConhecimentoDeTransporteEletronico Documento, String endPoint, Repositorio.UnitOfWork unitOfWork, out string mensagem, bool consultaManual, Dominio.Entidades.Usuario Usuario)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoIntegracao retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoIntegracao();
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            DateTime dataEmissao = Documento.DataEmissao.Value;
            if (!string.IsNullOrEmpty(Documento.ChaveCTESubComp))
            {
                //enviar data do cte original.
                var cte = repCte.BuscarPorChave(Documento.ChaveCTESubComp);
                if (cte != null)
                    dataEmissao = cte.DataEmissao.Value;
            }

            if (Documento.Serie.Numero == 901 || Documento.Serie.Numero == 900) //ND #54966
                dataEmissao = Documento.DataEmissao.Value;

            mensagem = "";
            if (Documento == null)
            {
                mensagem = "Não localizado Documento para integração";
                retorno.situacao = SituacaoIntegracao.ProblemaIntegracao;
                return retorno;
            }

            endPoint += "?cpfCnpj=" + Documento.Empresa.RaizCnpj;
            endPoint += "&numero=" + Documento.Numero.ToString().PadLeft(9, '0');
            endPoint += "&dataEmissaoInicial=" + dataEmissao.AddDays(-5).ToString("yyyy-MM-dd"); ;
            endPoint += "&dataEmissaoFinal=" + dataEmissao.AddDays(5).ToString("yyyy-MM-dd");
            endPoint += "&tipoPessoa=J";
            endPoint += "&considerarFatura=N";
            endPoint += "&limite=10&pagina=1";
            _jsonrequest = endPoint;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

            HttpClient client = ObterClient(configuracaoIntegracao);
            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = client.GetAsync(endPoint).Result;

            if (result.IsSuccessStatusCode)
            {
                string resultadoRetorno = result.Content.ReadAsStringAsync().Result;
                _jsonresponse = resultadoRetorno;

                if (resultadoRetorno.Contains("erro"))
                {
                    mensagem = "Problema na consulta dos dados"; //resultadoRetorno.Length > 300 ? resultadoRetorno.Substring(0, 300) : resultadoRetorno;
                    retorno.situacao = SituacaoIntegracao.ProblemaIntegracao;
                    Servicos.Log.TratarErro(endPoint + " consultar/titulo Retorno: " + resultadoRetorno, "IntegracaoMarfrig");
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTituloReceber RetornoComunicacao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTituloReceber>(result.Content.ReadAsStringAsync().Result);
                    if (RetornoComunicacao.status == 1)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico documento = Documento;

                        Titulo TituloCorrespondente = null;

                        if (documento.Serie.Numero == 901) //ND 
                        {
                            TituloCorrespondente = ProcessarTituloAdianamentoDevolucao(documento, RetornoComunicacao, consultaManual, Usuario, unitOfWork);

                            if (TituloCorrespondente != null)
                            {
                                mensagem = (consultaManual ? "Consulta Manual Usuário: - " + Usuario != null ? Usuario.Nome : " - " : "Consulta via API - ") + " Título Encontrado. Situação: " + TituloCorrespondente.StatusTitulo.ObterDescricao() + " - Documento: " + documento.Numero + "-" + documento.Serie.Numero.ToString();
                                retorno.situacao = SituacaoIntegracao.AgIntegracao;

                                if (TituloCorrespondente.StatusTitulo == StatusTitulo.Quitada)
                                {
                                    retorno.situacao = SituacaoIntegracao.Integrado;
                                    integracaoMarfigCteTitulo.DataQuitacaoCadastro = DateTime.Now;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(documento.Protocolo))
                                {

                                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTitulo TituloEncontrado = null;
                                    TituloEncontrado = RetornoComunicacao.titulos.Where(obj => obj.idIntegracao == documento.Protocolo).FirstOrDefault();

                                    if (TituloEncontrado != null)
                                    {
                                        // criar titulo para o documento..
                                        if (documento.Titulo == null)
                                            ProcessarTituloIntegracao(documento, TituloEncontrado.baixa, (decimal)TituloEncontrado.valorLiquido, documento.ValorAReceber, TituloEncontrado.vencimento, unitOfWork, TituloEncontrado.chaveFatura);
                                        else
                                            AtualizarTituloIntegracao(documento.Titulo, TituloEncontrado.baixa, (decimal)TituloEncontrado.valorLiquido, documento.ValorAReceber, unitOfWork, TituloEncontrado.chaveFatura);

                                        TituloCorrespondente = documento.Titulo;

                                        mensagem = (consultaManual ? "Consulta Manual Usuário: - " + Usuario != null ? Usuario.Nome : " - " : "Consulta via API - ") + " Título Encontrado. Situação: " + TituloCorrespondente.StatusTitulo.ObterDescricao() + " - Documento: " + documento.Numero + "-" + documento.Serie.Numero.ToString();
                                        retorno.situacao = SituacaoIntegracao.AgIntegracao;

                                        if (TituloCorrespondente.StatusTitulo == StatusTitulo.Quitada)
                                        {
                                            retorno.situacao = SituacaoIntegracao.Integrado;
                                            integracaoMarfigCteTitulo.DataQuitacaoCadastro = DateTime.Now;
                                        }
                                    }
                                }

                                if (TituloCorrespondente == null)
                                {
                                    //se nao achou o titulo, verificar se ja existia antes, se ja tinha encotrado e bla bla e proceder com um possivel cancelamento se o endpoint nao o manda mais significa q esta cancelado =/
                                    retorno.situacao = SituacaoIntegracao.AgIntegracao;
                                    mensagem = (consultaManual ? "Consulta Manual Usuário: - " + (Usuario != null ? Usuario.Nome : " - ") : "Consulta via API - ") + " Título não quitado ou não encontrado. Documento: " + documento.Numero + "-" + documento.Serie.Numero.ToString();
                                }
                            }
                        }
                        else
                        {
                            if (RetornoComunicacao.titulos.Length > 0)//verificar pela Serie se é FAT ou NF (AVULSO)
                            {
                                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTitulo> faturas = RetornoComunicacao.titulos.Where(obj => obj.serie == "FAT").ToList();
                                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTitulo> titulos = RetornoComunicacao.titulos.Where(obj => obj.serie != "FAT").ToList();

                                //TITULOS EM FATURA
                                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTitulo fatura in faturas) //criar a fatura caso nao existir, e procurar os titulos por numero e fornecedor no ME e adiciona titulos ao cte Encontrado;
                                    TituloCorrespondente = ProcessarFaturaIntegracao(fatura, documento, consultaManual, Usuario, unitOfWork);

                                //TITULO AVULSO
                                if (titulos.Count > 0)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTitulo TituloEncontrado = null;
                                    TituloEncontrado = titulos.Where(obj => obj.numeroTitulo.Contains(documento.Numero.ToString()) && obj.serie == documento.Serie.Numero.ToString()).FirstOrDefault();
                                    if (TituloEncontrado != null)
                                    {
                                        // criar titulo para o documento..
                                        if (documento.Titulo == null)
                                            ProcessarTituloIntegracao(documento, TituloEncontrado.baixa, (decimal)TituloEncontrado.valorLiquido, documento.ValorAReceber, TituloEncontrado.vencimento, unitOfWork, TituloEncontrado.chaveFatura);
                                        else
                                            AtualizarTituloIntegracao(documento.Titulo, TituloEncontrado.baixa, (decimal)TituloEncontrado.valorLiquido, documento.ValorAReceber, unitOfWork, TituloEncontrado.chaveFatura);

                                        TituloCorrespondente = documento.Titulo;

                                        if (TituloEncontrado.acrescimoDecrescimo)
                                        {
                                            GerarAcrescimosDescrescimos(TituloCorrespondente, TituloEncontrado, out decimal somaAcrescimos, out decimal somaDecrescimos, unitOfWork);
                                            TituloCorrespondente.Acrescimo = somaAcrescimos;
                                            TituloCorrespondente.Desconto = somaDecrescimos;
                                            TituloCorrespondente.ValorAcrescimo = somaAcrescimos;
                                            TituloCorrespondente.ValorDesconto = somaDecrescimos;

                                            decimal valorOriginal = (decimal)TituloEncontrado.valorLiquido + somaDecrescimos - somaAcrescimos;
                                            TituloCorrespondente.ValorOriginal = valorOriginal;
                                            TituloCorrespondente.ValorTituloOriginal = valorOriginal;
                                            TituloCorrespondente.Valor = valorOriginal;

                                            TituloCorrespondente.ValorTotal = TituloCorrespondente.ValorOriginal + TituloCorrespondente.Acrescimo - TituloCorrespondente.Desconto;

                                            repTitulo.Atualizar(TituloCorrespondente);
                                        }
                                    }
                                    else
                                    {
                                        if (documento.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && !string.IsNullOrEmpty(titulos[0]?.idMulti))
                                        {
                                            string idMulti = titulos[0].idMulti;
                                            if (idMulti.Length > 23)
                                            {
                                                string numero = titulos[0]?.idMulti.Substring(3, 9);
                                                string Emissao = titulos[0]?.idMulti.Substring(15, 8);

                                                if (numero.Contains(documento.Numero.ToString()) && documento.DataEmissao.Value.ToString("yyyyMMdd") == Emissao)
                                                {
                                                    TituloEncontrado = titulos[0];

                                                    if (documento.Titulo == null)
                                                        ProcessarTituloIntegracao(documento, TituloEncontrado.baixa, (decimal)TituloEncontrado.valorLiquido, documento.ValorAReceber, TituloEncontrado.vencimento, unitOfWork, (string)TituloEncontrado.chaveFatura);
                                                    else
                                                        AtualizarTituloIntegracao(documento.Titulo, TituloEncontrado.baixa, (decimal)TituloEncontrado.valorLiquido, documento.ValorAReceber, unitOfWork, (string)TituloEncontrado.chaveFatura);

                                                    TituloCorrespondente = documento.Titulo;

                                                    if (TituloEncontrado.acrescimoDecrescimo)
                                                    {
                                                        GerarAcrescimosDescrescimos(TituloCorrespondente, TituloEncontrado, out decimal somaAcrescimos, out decimal somaDecrescimos, unitOfWork);
                                                        TituloCorrespondente.Acrescimo = somaAcrescimos;
                                                        TituloCorrespondente.Desconto = somaDecrescimos;
                                                        TituloCorrespondente.ValorAcrescimo = somaAcrescimos;
                                                        TituloCorrespondente.ValorDesconto = somaDecrescimos;

                                                        decimal valorOriginal = (decimal)TituloEncontrado.valorLiquido + somaDecrescimos - somaAcrescimos;
                                                        TituloCorrespondente.ValorOriginal = valorOriginal;
                                                        TituloCorrespondente.ValorTituloOriginal = valorOriginal;
                                                        TituloCorrespondente.Valor = valorOriginal;

                                                        TituloCorrespondente.ValorTotal = TituloCorrespondente.ValorOriginal + TituloCorrespondente.Acrescimo - TituloCorrespondente.Desconto;

                                                        repTitulo.Atualizar(TituloCorrespondente);
                                                    }
                                                }
                                            }
                                        }


                                    }
                                }

                                if (TituloCorrespondente != null)
                                {
                                    mensagem = (consultaManual ? "Consulta Manual Usuário: - " + Usuario != null ? Usuario.Nome : " - " : "Consulta via API - ") + " Título Encontrado. Situação: " + TituloCorrespondente.StatusTitulo.ObterDescricao() + " - Documento: " + documento.Numero + "-" + documento.Serie.Numero.ToString();
                                    retorno.situacao = SituacaoIntegracao.AgIntegracao;

                                    if (TituloCorrespondente.StatusTitulo == StatusTitulo.Quitada)
                                    {
                                        retorno.situacao = SituacaoIntegracao.Integrado;
                                        integracaoMarfigCteTitulo.DataQuitacaoCadastro = DateTime.Now;
                                    }
                                }
                                else
                                {
                                    //aqui se nao achou o titulo, verificar se ja existia antes, se ja tinha encotrado e bla bla e proceder com um possivel cancelamento se o endpoint nao o manda mais significa q esta cancelado =/
                                    retorno.situacao = SituacaoIntegracao.AgIntegracao;
                                    mensagem = (consultaManual ? "Consulta Manual Usuário: - " + (Usuario != null ? Usuario.Nome : " - ") : "Consulta via API - ") + " Título não quitado ou não encontrado. Documento: " + documento.Numero + "-" + documento.Serie.Numero.ToString();
                                }

                                return retorno;
                            }
                            else
                            {
                                retorno.situacao = SituacaoIntegracao.AgIntegracao;
                                mensagem = (consultaManual ? "Consulta Manual Usuário: - " + (Usuario != null ? Usuario.Nome : " - ") : "Consulta via API - ") + " Título não quitado ou não encontrado. Documento: " + documento.Numero + "-" + documento.Serie.Numero.ToString();
                                return retorno;
                            }
                        }
                    }
                    else
                    {
                        mensagem = "Problema na consulta dos dados"; //resultadoRetorno.Length > 300 ? resultadoRetorno.Substring(0, 300) : resultadoRetorno;
                        retorno.situacao = SituacaoIntegracao.ProblemaIntegracao;
                        Servicos.Log.TratarErro(endPoint + " consultar/titulo Retorno: " + resultadoRetorno, "IntegracaoMarfrig");
                    }
                }
            }
            else
            {
                //vamos tentar novamente..
                Servicos.Log.TratarErro("consultar/titulo Erro: " + result.ReasonPhrase, " IntegracaoMarfrig");
                mensagem = consultaManual ? "Consulta Manual - " : "Consulta via API - " + "SERVIÇO INDISPONÍVEL";
                retorno.situacao = SituacaoIntegracao.AgIntegracao;
                return retorno;
            }

            return retorno;
        }

        private Titulo ProcessarTituloAdianamentoDevolucao(Dominio.Entidades.ConhecimentoDeTransporteEletronico documento, Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTituloReceber RetornoComunicacao, bool consultaManual, Dominio.Entidades.Usuario usuario, UnitOfWork unitOfWork)
        {
            string url = ObterConfiguracaoIntegracao().URLConsultaAdiantamentoDevolucaoTitulo;
            string FilialEmissora = "01" + documento.Remetente.CPF_CNPJ_SemFormato.Substring(8, 4).ToString();

            // url += "fornecedor=" + RetornoComunicacao.titulos[0].fornecedor;
            //url += "&filial=" + FilialEmissora;
            //url += "&loja=" + RetornoComunicacao.titulos[0].loja;
            url += "?cpfCnpj=" + documento.Empresa.RaizCnpj;
            url += "&dataEmissaoInicial=" + (documento.DataEmissao.HasValue ? documento.DataEmissao.Value.AddDays(-2).ToString("yyyy-MM-dd") : "");
            url += "&dataEmissaoFinal=" + (documento.DataEmissao.HasValue ? documento.DataEmissao.Value.AddDays(2).ToString("yyyy-MM-dd") : "");
            url += "&numero=" + documento.Numero.ToString().PadLeft(9, '0');
            url += "&serie=" + documento.Serie.Numero;
            url += "&tipo=NDF";

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

            HttpClient client = ObterClient(configuracaoIntegracao);
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = client.GetAsync(url).Result;

            if (result.IsSuccessStatusCode)
            {

                string body = result.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTituloAdiantamentoDevolucao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTituloAdiantamentoDevolucao>(body);

                if (retorno.itens.Length > 0)
                    ProcessarTituloIntegracao(documento, retorno.itens[0].data, retorno.itens[0].valorPago, documento.ValorAReceber, RetornoComunicacao.titulos[0].vencimento, unitOfWork);
            }
            else
                Servicos.Log.TratarErro($"{url} adiantamento devolução não integrados para o título {documento.Numero}", "IntegracaoMarfrig");

            return documento.Titulo;
        }


        private void GerarAcrescimosDescrescimos(Titulo titulo, Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTitulo tituloRetornado, out decimal somaAcrescimos, out decimal somaDecrescimos, Repositorio.UnitOfWork unitOfWork)
        {
            somaAcrescimos = 0;
            somaDecrescimos = 0;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

            string uri = configuracaoIntegracao.URLConsultaAcrescimoDecrescimoTitulo;

            uri += "?fornecedor=" + tituloRetornado.fornecedor;
            uri += "&filial=" + tituloRetornado.filial;
            uri += "&loja=" + tituloRetornado.loja;
            uri += "&numero=" + tituloRetornado.numeroTitulo;
            uri += "&parcela=" + tituloRetornado.parcela;
            uri += "&serie=" + tituloRetornado.serie;
            uri += "&tipo=" + tituloRetornado.tipo;

            HttpClient client = ObterClient(configuracaoIntegracao);
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("apikey", configuracaoIntegracao.ApiKey);

            var result = client.GetAsync(uri).Result;

            if (result.IsSuccessStatusCode)
            {
                Repositorio.Embarcador.Financeiro.TituloAcrescimoDecrescimo repTituloAcrescimoDecrescimo = new Repositorio.Embarcador.Financeiro.TituloAcrescimoDecrescimo(unitOfWork);

                string body = result.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTituloAcrescimosDecrescimos retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTituloAcrescimosDecrescimos>(body);

                // Deleta os acréscimos / decréscimos já existentes
                if (titulo != null)
                {
                    var listaDeletar = repTituloAcrescimoDecrescimo.BuscarPorTitulo(titulo.Codigo);
                    foreach (var d in listaDeletar)
                        repTituloAcrescimoDecrescimo.Deletar(d);
                }

                foreach (var item in retorno.itens)
                {
                    if (titulo != null)
                    {
                        var acrescimoDescrecimo = new TituloAcrescimoDecrescimo
                        {
                            IdIntegracao = item.id,
                            Valor = item.valor,
                            Tipo = item.tipo == "D" ? TipoAcrescimoDecrescimo.Decrescimo : TipoAcrescimoDecrescimo.Acrescimo,
                            Historico = item.historico,
                            Titulo = titulo,
                            Descricao = item.descricao,
                            DataCriacao = DateTime.Now
                        };

                        repTituloAcrescimoDecrescimo.Inserir(acrescimoDescrecimo);
                    }

                    if (item.tipo == "D")
                        somaDecrescimos += item.valor;
                    else
                        somaAcrescimos += item.valor;
                }
            }
            else
                Servicos.Log.TratarErro($"{uri} Acréscimos e descontos não integrados para o título {tituloRetornado.numeroTitulo}", "IntegracaoMarfrig");
        }

        private void GerarAcrescimosDescrescimosDoTituloFatura(Titulo titulo, Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTitulo tituloRetornado, Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoNotasfiscais notaFatura, Dominio.Entidades.ConhecimentoDeTransporteEletronico CteNota, out decimal somaAcrescimos, out decimal somaDecrescimos, Repositorio.UnitOfWork unitOfWork, bool tentativa = false)
        {
            somaAcrescimos = 0;
            somaDecrescimos = 0;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
            var uri = configuracaoIntegracao.URLConsultaAcrescimoDecrescimoTitulo;

            if (tentativa)
                _tentativaConsultaAcrescimosDecrescimos += 1;

            if (_tentativaConsultaAcrescimosDecrescimos > 9)
                return;

            string FilialEmissora = "01" + CteNota.Remetente.CPF_CNPJ_SemFormato.Substring(8, 4).ToString();
            string Tipo = !string.IsNullOrEmpty(CteNota.Chave) ? "NF" : "ND";

            uri += "?fornecedor=" + tituloRetornado.fornecedor;
            uri += "&filial=" + FilialEmissora;
            if (tentativa)
                uri += "&loja=0" + _tentativaConsultaAcrescimosDecrescimos;
            else
                uri += "&loja=" + tituloRetornado.loja;
            uri += "&numero=" + notaFatura.numero;
            uri += "&serie=" + CteNota.Serie.Numero;
            uri += "&tipo=" + Tipo;



            HttpClient client = ObterClient(configuracaoIntegracao);
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = client.GetAsync(uri).Result;

            if (result.IsSuccessStatusCode)
            {
                Repositorio.Embarcador.Financeiro.TituloAcrescimoDecrescimo repTituloAcrescimoDecrescimo = new Repositorio.Embarcador.Financeiro.TituloAcrescimoDecrescimo(unitOfWork);

                string body = result.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTituloAcrescimosDecrescimos retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTituloAcrescimosDecrescimos>(body);

                if (retorno.itens.Length > 0)
                {
                    // Deleta os acréscimos / decréscimos já existentes
                    if (titulo != null)
                    {
                        var listaDeletar = repTituloAcrescimoDecrescimo.BuscarPorTitulo(titulo.Codigo);
                        foreach (var d in listaDeletar)
                            repTituloAcrescimoDecrescimo.Deletar(d);
                    }

                    foreach (var item in retorno.itens)
                    {
                        if (titulo != null)
                        {
                            var acrescimoDescrecimo = new TituloAcrescimoDecrescimo
                            {
                                IdIntegracao = item.id,
                                Valor = item.valor,
                                Tipo = item.tipo == "D" ? TipoAcrescimoDecrescimo.Decrescimo : TipoAcrescimoDecrescimo.Acrescimo,
                                Historico = item.historico,
                                Titulo = titulo,
                                Descricao = item.descricao,
                                DataCriacao = DateTime.Now
                            };

                            repTituloAcrescimoDecrescimo.Inserir(acrescimoDescrecimo);
                        }

                        if (item.tipo == "D")
                            somaDecrescimos += item.valor;
                        else
                            somaAcrescimos += item.valor;
                    }
                }
                else //não encontrou, devemos procurar em outras lojas (01,02,03,04,05,06,07,08,09) =( poise.
                {
                    if (_tentativaConsultaAcrescimosDecrescimos <= 9)
                        GerarAcrescimosDescrescimosDoTituloFatura(titulo, tituloRetornado, notaFatura, CteNota, out somaAcrescimos, out somaDecrescimos, unitOfWork, true);
                }
            }
            else
                Servicos.Log.TratarErro($"{uri} Acréscimos e descontos não integrados para o título {tituloRetornado.numeroTitulo}", "IntegracaoMarfrig");
        }


        private Titulo ProcessarFaturaIntegracao(Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoCteTitulo retornoIntegracaoTitulo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteDocumento, bool consultaManual, Dominio.Entidades.Usuario Usuario, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);

            List<DocumentoFaturamento> documentosFaturamentoFatura = new List<DocumentoFaturamento>();
            Titulo TituloCorrespondente = null;

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorNumero(retornoIntegracaoTitulo.numeroTitulo.ToInt());
            if (fatura == null)
            {
                fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura()
                {
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado,
                    Empresa = cteDocumento.Empresa,
                    Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fechamento,
                    Numero = !string.IsNullOrWhiteSpace(retornoIntegracaoTitulo.chaveFatura) ? retornoIntegracaoTitulo.chaveFatura.Substring(14, 9).ToInt() : retornoIntegracaoTitulo.numeroTitulo.ToInt(),
                    DataFatura = DateTime.Now,
                    TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa,
                    Cliente = cteDocumento.Tomador.Cliente,
                    Transportador = cteDocumento.Empresa,
                    NovoModelo = true,
                    ControleNumeracao = repFatura.BuscarProximoControleNumeracao(),
                    Total = (decimal)retornoIntegracaoTitulo.valorLiquido,
                    TotalLiquido = (decimal)retornoIntegracaoTitulo.valorLiquido,
                    DataFechamento = DateTime.Parse(retornoIntegracaoTitulo.vencimento)
                };

                repFatura.Inserir(fatura);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoNotasfiscais notas in retornoIntegracaoTitulo.notasFiscais)
            {
                // para cada nota fiscal retornada encontrar o DocumentoFaturamento atraves do seu CTe (por cnpj e numero), vincular o documento a nova fatura, alem de criar o titulo ou atualizar e verificar a integracao do titulo... (mee traabaieraaa)
                Dominio.Entidades.ConhecimentoDeTransporteEletronico CteNota = null;

                if (!string.IsNullOrEmpty(notas.chaveDocumento))
                    CteNota = repCte.BuscarPorChave(notas.chaveDocumento);
                else
                    CteNota = repCte.BuscarPorNumeroSerieEEmpresa(notas.numero.ToInt(), 0, fatura.Empresa.Codigo);

                if (CteNota != null)
                {
                    DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(CteNota.Codigo);
                    if (documentoFaturamento != null)
                    {
                        //verificar se tem fatura documento..
                        if (!repFaturaDocumento.ExisteNaFatura(fatura.Codigo, documentoFaturamento.Codigo))
                        {
                            Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = new Dominio.Entidades.Embarcador.Fatura.FaturaDocumento
                            {
                                Fatura = fatura,
                                Documento = documentoFaturamento,
                                ValorACobrar = documentoFaturamento.ValorAFaturar,
                                TituloGerado = true,
                                ValorTotalACobrar = documentoFaturamento.ValorAFaturar
                            };
                            repFaturaDocumento.Inserir(faturaDocumento);
                        }

                        documentosFaturamentoFatura.Add(documentoFaturamento);

                        if (CteNota.Titulo == null)
                            ProcessarTituloIntegracao(CteNota, retornoIntegracaoTitulo.baixa, (decimal)notas.valor, CteNota.ValorAReceber, retornoIntegracaoTitulo.vencimento, unidadeTrabalho);
                        else
                            AtualizarTituloIntegracao(CteNota.Titulo, retornoIntegracaoTitulo.baixa, (decimal)notas.valor, CteNota.ValorAReceber, unidadeTrabalho);

                        if (retornoIntegracaoTitulo.acrescimoDecrescimo && (CteNota.Titulo.ValorOriginal - CteNota.Titulo.ValorPago - CteNota.Titulo.Desconto + CteNota.Titulo.Acrescimo) != 0) //AcrescimosDescrescimos para titulos da fatura
                        {
                            _tentativaConsultaAcrescimosDecrescimos = 1;
                            GerarAcrescimosDescrescimosDoTituloFatura(CteNota.Titulo, retornoIntegracaoTitulo, notas, CteNota, out decimal somaAcrescimos, out decimal somaDecrescimos, unidadeTrabalho);
                            CteNota.Titulo.Acrescimo = somaAcrescimos;
                            CteNota.Titulo.Desconto = somaDecrescimos;
                        }

                        ProcessarObjetoIntegracaoTituloFatura(CteNota, consultaManual, Usuario, fatura.Numero, unidadeTrabalho);
                    }

                    if (CteNota.Titulo != null && CteNota.Codigo == cteDocumento.Codigo) //encontrou o titulo correspondente
                        TituloCorrespondente = CteNota.Titulo;
                }
            }

            if (documentosFaturamentoFatura.Count > 0)
            {
                fatura.DataInicial = documentosFaturamentoFatura.Min(o => o.DataEmissao);
                fatura.DataFinal = documentosFaturamentoFatura.Max(o => o.DataEmissao);
            }

            if (retornoIntegracaoTitulo.acrescimoDecrescimo) //AcrescimosDescrescimos para a fatura
            {
                GerarAcrescimosDescrescimos(null, retornoIntegracaoTitulo, out decimal somaAcrescimos, out decimal somaDecrescimos, unidadeTrabalho);
                fatura.Acrescimo = somaAcrescimos;
                fatura.Desconto = somaDecrescimos;
            }

            repFatura.Atualizar(fatura);

            return TituloCorrespondente;
        }

        private void AtualizarTituloIntegracao(Titulo titulo, string dataBaixa, decimal valorLiquido, decimal valorDocumento, Repositorio.UnitOfWork unitOfWork, string chaveFatura = "")
        {
            // verificar se existe titulo para o CTe, senao criar.
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            if (string.IsNullOrEmpty(dataBaixa)) // Se já foi dado baixa
            {
                titulo.ValorPago = 0;
                titulo.ValorPendente = valorDocumento;
                titulo.StatusTitulo = StatusTitulo.EmAberto;
            }
            else
            {
                titulo.ValorPago = valorLiquido;
                titulo.ValorPendente = 0;
                titulo.StatusTitulo = StatusTitulo.Quitada;
                titulo.DataLiquidacao = DateTime.Parse(dataBaixa);
            }

            if (!string.IsNullOrWhiteSpace(chaveFatura))
                titulo.NumeroFatura = chaveFatura.Substring(14, 9).ToInt();

            if (titulo.Codigo > 0)
                repTitulo.Atualizar(titulo);
        }

        private void ProcessarTituloIntegracao(Dominio.Entidades.ConhecimentoDeTransporteEletronico Cte, string dataBaixa, decimal valorLiquido, decimal valorDocumento, string vencimento, Repositorio.UnitOfWork unitOfWork, string chaveFatura = "")
        {
            // verificar se existe titulo para o CTe, senao criar.
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Titulo Titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

            if (string.IsNullOrEmpty(dataBaixa)) // Se já foi dado baixa
            {
                Titulo.ValorPago = 0;
                Titulo.ValorPendente = valorDocumento;
                Titulo.StatusTitulo = StatusTitulo.EmAberto;
            }
            else
            {
                Titulo.ValorPago = valorLiquido;
                Titulo.ValorPendente = 0;
                Titulo.StatusTitulo = StatusTitulo.Quitada;
                Titulo.DataLiquidacao = DateTime.Parse(dataBaixa);
            }

            Titulo.DataVencimento = DateTime.Parse(vencimento);
            Titulo.DataEmissao = Cte.DataEmissao;
            Titulo.Historico = "GERADO PELA INTEGRACAO DE TITULOS " + Cte.Numero + " E SÉRIE " + Cte.Serie.Numero.ToString();
            Titulo.Pessoa = Cte.Tomador.Cliente;
            Titulo.GrupoPessoas = Titulo.Pessoa.GrupoPessoas;
            Titulo.Sequencia = 1;
            Titulo.DataAlteracao = DateTime.Now;
            Titulo.TipoTitulo = TipoTitulo.Receber;

            // Valores
            Titulo.ValorOriginal = valorDocumento;
            Titulo.ValorTituloOriginal = valorDocumento;
            Titulo.Valor = valorDocumento;

            Titulo.ValorTotal = Titulo.ValorOriginal + Titulo.Acrescimo - Titulo.Desconto;

            Titulo.NumeroDocumentoTituloOriginal = Cte.Numero.ToString();
            Titulo.ConhecimentoDeTransporteEletronico = Cte;
            Titulo.Observacao = "CT-e Nº: " + Cte.Numero;
            Titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
            Titulo.TipoAmbiente = Cte.TipoAmbiente;

            Titulo.Usuario = Cte.Usuario;
            Titulo.DataLancamento = DateTime.Now;
            Titulo.NumeroFatura = chaveFatura.Substring(14, 9).ToInt();

            Titulo.Empresa = Cte.Empresa;

            if (Titulo.Codigo > 0)
                repTitulo.Atualizar(Titulo);
            else
            {
                repTitulo.Inserir(Titulo);
                Cte.Titulo = Titulo;
                repCte.Atualizar(Cte);
            }

        }

        private void ProcessarObjetoIntegracaoTituloFatura(Dominio.Entidades.ConhecimentoDeTransporteEletronico Cte, bool consultaManual, Dominio.Entidades.Usuario Usuario, int numeroFatura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber repIntegracaoMarfrigCte = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber(unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceberArquivos repIntegracaoMarfrigCteTitulosReceberArquivos = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceberArquivos(unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber integracaoCteTituloReceber = repIntegracaoMarfrigCte.BuscarPorCodigoCTe(Cte.Codigo);

            if (integracaoCteTituloReceber != null)
            {
                if (Cte.Titulo.StatusTitulo == StatusTitulo.Quitada)
                {
                    integracaoCteTituloReceber.Situacao = SituacaoIntegracao.Integrado;
                    if (!integracaoCteTituloReceber.DataQuitacaoCadastro.HasValue)
                        integracaoCteTituloReceber.DataQuitacaoCadastro = DateTime.Now;
                }
            }
            else
            {
                integracaoCteTituloReceber = new Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber();
                integracaoCteTituloReceber.CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico { Codigo = Cte.Codigo };
                integracaoCteTituloReceber.NumeroTentativas = 0;
                integracaoCteTituloReceber.DataConsulta = DateTime.Now;
                integracaoCteTituloReceber.DataCadastro = DateTime.Now;
                integracaoCteTituloReceber.Situacao = SituacaoIntegracao.AgIntegracao;

                if (Cte.Titulo.StatusTitulo == StatusTitulo.Quitada)
                {
                    integracaoCteTituloReceber.Situacao = SituacaoIntegracao.Integrado;
                    integracaoCteTituloReceber.DataQuitacaoCadastro = DateTime.Now;
                }
            }

            integracaoCteTituloReceber.Retorno = (consultaManual ? "Consulta Manual Usuário: " + Usuario != null ? Usuario.Nome : " - " : "Consulta via API - ") + "Título Encontrado em FATURA Nº: " + numeroFatura.ToString() + " Situação: " + Cte.Titulo.StatusTitulo.ObterDescricao() + " - Documento: " + Cte.Numero + "-" + Cte.Serie.Numero.ToString();

            Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo();
            arquivoIntegracao.Mensagem = integracaoCteTituloReceber.Retorno;
            arquivoIntegracao.Data = DateTime.Now;
            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(_jsonrequest, "json", unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(_jsonresponse, "json", unitOfWork);
            repIntegracaoMarfrigCteTitulosReceberArquivos.Inserir(arquivoIntegracao);

            if (integracaoCteTituloReceber.ArquivosTransacao == null)
                integracaoCteTituloReceber.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo>();

            integracaoCteTituloReceber.ArquivosTransacao.Add(arquivoIntegracao);

            if (integracaoCteTituloReceber.Codigo > 0)
                repIntegracaoMarfrigCte.Atualizar(integracaoCteTituloReceber);
            else
                repIntegracaoMarfrigCte.Inserir(integracaoCteTituloReceber);

        }

        private int diferencaDiasUteis(DateTime dataInicial, DateTime dataFinal)
        {
            var dias = 0;
            var diascount = 0;
            dias = dataInicial.Subtract(dataFinal).Days;

            if (dias < 0)
                dias = dias * -1;

            for (int i = 1; i <= dias; i++)
            {
                dataInicial = dataInicial.AddDays(1);

                if (dataInicial.DayOfWeek != DayOfWeek.Sunday &&
                    dataInicial.DayOfWeek != DayOfWeek.Saturday)
                    diascount++;
            }
            return diascount;
        }

        private string ValidaDocumentoPorChaveNf(string documento)
        {
            if (documento.StartsWith("000"))
            {
                documento = documento.Substring(3);

                bool documentoValido = Utilidades.Validate.ValidarCPF(documento);
                if (!documentoValido)
                {
                    documento = documento.PadLeft(documento.Length + 3, '0');
                    documentoValido = Utilidades.Validate.ValidarCNPJ(documento);
                    documento = documentoValido ? documento : string.Empty;
                }
            }

            return documento;
        }

        #endregion Métodos Privados

        #region Métodos Publicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);


            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
                string mensagemErro = string.Empty;
                bool integracaoRealizadaComSucesso = false;

                if (cargaIntegracao.Carga.Redespacho == null)
                    integracaoRealizadaComSucesso = RetornarCarga(cargaIntegracao.Carga.Codigo, configuracaoIntegracao.URLIntegracaoEmissaoDocumentos, ref jsonRequisicao, ref jsonRetorno, out mensagemErro);
                else
                    integracaoRealizadaComSucesso = RetornarCargaRedespacho(cargaIntegracao.Carga, configuracaoIntegracao.URLRedespachoCarga, ref jsonRequisicao, ref jsonRetorno, out mensagemErro);

                if (integracaoRealizadaComSucesso)
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = string.Empty;
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = mensagemErro;
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
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Marfrig.";

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            ocorrenciaIntegracao.NumeroTentativas += 1;
            ocorrenciaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
                string mensagemErro = string.Empty;
                string protocolo = string.Empty;

                bool integracaoRealizadaComSucesso = false;

                if (ocorrenciaIntegracao.CargaOcorrencia.TipoOcorrencia.ExibirParcelasNaAprovacao)
                    integracaoRealizadaComSucesso = RetornarOcorrenciaComParcela(ocorrenciaIntegracao, $"{configuracaoIntegracao.URLIntegraRetornoOcorrenciaNotaDebitoParcelamento}", ref jsonRequisicao, ref jsonRetorno, out mensagemErro, out protocolo);
                else if ((ocorrenciaIntegracao.CargaOcorrencia.TipoOcorrencia.OrigemOcorrencia == OrigemOcorrencia.PorPeriodo) && (ocorrenciaIntegracao.CargaOcorrencia.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros))
                    integracaoRealizadaComSucesso = RetornarNotaDebitoOcorrencia(ocorrenciaIntegracao, $"{configuracaoIntegracao.URLIntegraRetornoOcorrenciaNotaDebito}", ref jsonRequisicao, ref jsonRetorno, out mensagemErro, out protocolo);
                else if (ocorrenciaIntegracao.CargaOcorrencia.TipoOcorrencia.OrigemOcorrencia == OrigemOcorrencia.PorPeriodo)
                    mensagemErro = "Ocorrência por período não possui integração de retorno para documentos diferentes de ND/NC disponível";
                else if (ocorrenciaIntegracao.CargaOcorrencia.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                    integracaoRealizadaComSucesso = RetornarNotaDebitoOcorrencia(ocorrenciaIntegracao, $"{configuracaoIntegracao.URLIntegraRetornoOcorrenciaNotaDebito}", ref jsonRequisicao, ref jsonRetorno, out mensagemErro, out protocolo);
                else if ((ocorrenciaIntegracao.CargaCTe.CTe?.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe) || (ocorrenciaIntegracao.CargaCTe.CTe?.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe))
                    integracaoRealizadaComSucesso = RetornarCTeOcorrencia(ocorrenciaIntegracao, $"{configuracaoIntegracao.URLIntegraRetornoOcorrenciaCteComplementar}", ref jsonRequisicao, ref jsonRetorno, out mensagemErro);
                //else if (ocorrenciaIntegracao.CargaCTe.CTe?.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                //{
                //    //integracaoRealizadaComSucesso = RetornarNFSeOcorrencia(integracao, $"{configuracaoIntegracao.Url}nota-fiscal-servico-despesa", ref jsonRequisicao, ref jsonRetorno, out mensagemErro);
                //    //Conforme passado pela Fabiana em reunião não vai mais ser utilizado o método "nota-fiscal-servico-despesa", Marfrig vai alterar o método "ctecomplementar" para receber as NFSe complementares
                //    mensagemErro = "NFSe não disponível para retorno de integração";
                //}
                else
                    mensagemErro = "Modelo de documento não disponível para retorno de integração";

                if (integracaoRealizadaComSucesso)
                {
                    ocorrenciaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    if (!string.IsNullOrEmpty(protocolo))
                        ocorrenciaIntegracao.ProblemaIntegracao = protocolo;
                    else
                        ocorrenciaIntegracao.ProblemaIntegracao = "";
                }
                else
                {
                    ocorrenciaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    ocorrenciaIntegracao.ProblemaIntegracao = mensagemErro;
                }

                servicoArquivoTransacao.Adicionar(ocorrenciaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                ocorrenciaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Marfrig.";

                servicoArquivoTransacao.Adicionar(ocorrenciaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioOcorrenciaIntegracao.Atualizar(ocorrenciaIntegracao);
        }

        public void CriarIntegracaoCteTitulosReceber(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoTransporte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber repIntegracaoMarfrigCteTitulo = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber(unitOfWork);

            List<int> codConhecimentoTransporteNaoQuitados = repConhecimentoTransporte.BuscarCTesSemTitulosOuNaoQuitadosEmConciliacao(configuracaoIntegracao.DataCorteCriacaoIntegracaoTituloMarfrig);
            foreach (int codigoCte in codConhecimentoTransporteNaoQuitados)
            {
                try
                {
                    unitOfWork.Start();

                    bool existeIntegracao = repIntegracaoMarfrigCteTitulo.ExisteIntegracaoPorCTE(codigoCte);

                    if (existeIntegracao)
                        continue;
                    else
                    {
                        //ainda nao existe vamos criar pra depois consultar;
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber integracaoMarfigCteTituloReceber = new Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber();
                        integracaoMarfigCteTituloReceber.CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico { Codigo = codigoCte };
                        integracaoMarfigCteTituloReceber.NumeroTentativas = 0;
                        integracaoMarfigCteTituloReceber.DataConsulta = DateTime.Now;
                        integracaoMarfigCteTituloReceber.Situacao = SituacaoIntegracao.AgIntegracao;
                        integracaoMarfigCteTituloReceber.DataCadastro = DateTime.Now;
                        integracaoMarfigCteTituloReceber.Retorno = "";

                        repIntegracaoMarfrigCteTitulo.Inserir(integracaoMarfigCteTituloReceber);
                    }

                    unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao);
                }
            }

        }

        public void IntegrarCTeTituloReceber(Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber IntegracaoCteTituloReceber, bool consultaManual, Dominio.Entidades.Usuario Usuario)
        {
            Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber repIntegracaoMarfrigCteTitulosReceber = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber(_unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceberArquivos repIntegracaoMarfrigCteTitulosReceberArquivos = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceberArquivos(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            _jsonrequest = string.Empty;
            _jsonresponse = string.Empty;
            string mensagem = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

                IntegracaoCteTituloReceber.NumeroTentativas += 1;
                IntegracaoCteTituloReceber.DataConsulta = DateTime.Now;

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCte.BuscarPorCodigo(IntegracaoCteTituloReceber.CTe.Codigo);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.CteTitulosReceber.RetornoIntegracao retorno = RetornarTitulosIntegracao(IntegracaoCteTituloReceber, cte, configuracaoIntegracao.URLConsultaTitulo, _unitOfWork, out mensagem, consultaManual, Usuario);

                IntegracaoCteTituloReceber.Situacao = retorno.situacao;
                if (retorno.situacao == SituacaoIntegracao.Integrado)
                    IntegracaoCteTituloReceber.Retorno = (consultaManual ? "Consulta Manual Usuário: " + Usuario != null ? Usuario.Nome : "" : "Consulta via API - ") + "Integração efetuada com Sucesso Documento: " + IntegracaoCteTituloReceber.CTe.Numero + "-" + IntegracaoCteTituloReceber.CTe.Serie.Numero.ToString() + " Situação: " + IntegracaoCteTituloReceber.CTe.Titulo.StatusTitulo.ObterDescricao();
                else
                    IntegracaoCteTituloReceber.Retorno = mensagem;

                Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo();
                arquivoIntegracao.Data = IntegracaoCteTituloReceber.DataConsulta;
                arquivoIntegracao.Mensagem = IntegracaoCteTituloReceber.Retorno;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(_jsonrequest, "json", _unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(_jsonresponse, "json", _unitOfWork);

                repIntegracaoMarfrigCteTitulosReceberArquivos.Inserir(arquivoIntegracao);

                if (IntegracaoCteTituloReceber.ArquivosTransacao == null)
                    IntegracaoCteTituloReceber.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo>();

                IntegracaoCteTituloReceber.ArquivosTransacao.Add(arquivoIntegracao);
            }
            catch (ServicoException excecao)
            {
                IntegracaoCteTituloReceber.Situacao = SituacaoIntegracao.ProblemaIntegracao;
                IntegracaoCteTituloReceber.Retorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                IntegracaoCteTituloReceber.Retorno = "Ocorreu uma falha ao comunicar com o Web Service da Marfrig. Consulta Títulos.";
                IntegracaoCteTituloReceber.Situacao = SituacaoIntegracao.ProblemaIntegracao;

                Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo();
                arquivoIntegracao.Data = IntegracaoCteTituloReceber.DataConsulta;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(_jsonrequest, "json", _unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(_jsonresponse, "json", _unitOfWork);
                repIntegracaoMarfrigCteTitulosReceberArquivos.Inserir(arquivoIntegracao);

                if (IntegracaoCteTituloReceber.ArquivosTransacao == null)
                    IntegracaoCteTituloReceber.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo>();

                IntegracaoCteTituloReceber.ArquivosTransacao.Add(arquivoIntegracao);
            }

            repIntegracaoMarfrigCteTitulosReceber.Atualizar(IntegracaoCteTituloReceber);

        }

        public void IntegrarCargaPreCalculoFrete(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            if (cargaDadosTransporteIntegracao.Carga.CargaPossuiPreCalculoFrete && !cargaDadosTransporteIntegracao.Carga.CalculandoFrete)
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);

                cargaDadosTransporteIntegracao.NumeroTentativas += 1;
                cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

                string jsonRequisicao = string.Empty;
                string jsonRetorno = string.Empty;
                try
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

                    if (IntegrarPreCalculoFrete(out string mensagem, cargaDadosTransporteIntegracao, configuracaoIntegracao, ref jsonRequisicao, ref jsonRetorno))
                    {
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração realizada com sucesso.";
                    }
                    else
                    {
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem;
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
                catch (ServicoException excecao)
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);

                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Marfrig.";
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork);
                arquivoIntegracao.Data = DateTime.Now;
                arquivoIntegracao.Mensagem = cargaDadosTransporteIntegracao.ProblemaIntegracao;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
            }
        }

        public void IntegrarImpressaoDocumentos(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

                if (string.IsNullOrWhiteSpace(cargaIntegracao.Carga.NumeroImpressora) && !(cargaIntegracao.Carga.Filial?.NumeroUnidadeImpressao > 0))
                    throw new ServicoException("Não há impressora configurada para esta ação.");

                HttpClient client = ObterClient(configuracaoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RequisicaoImpressaoDocumentos requisicao = PreencherRequisicaoImpressaoDocumentos(cargaIntegracao);

                jsonRequisicao = JsonConvert.SerializeObject(requisicao);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = client.PostAsync(configuracaoIntegracao.URLRequisicaoImpressaoDocumentosCarga, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.RetornoImpressaoDocumentos retornoIntegracao = JsonConvert.DeserializeObject<RetornoImpressaoDocumentos>(jsonRetorno);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    if (!retornoIntegracao.success)
                        throw new ServicoException(retornoIntegracao.errors?.FirstOrDefault());

                    string retorno = EnviarDocumentosParaImpressao(cargaIntegracao.Carga);

                    if (!string.IsNullOrWhiteSpace(retorno))
                        throw new ServicoException(retorno);

                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Integrado com sucesso!";

                    servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
                }
                else
                {
                    if (retornoIntegracao != null && !string.IsNullOrWhiteSpace(retornoIntegracao.data?.mensagem))
                        throw new ServicoException(retornoIntegracao.data?.mensagem);
                    else
                        throw new ServicoException("Ocorreu uma falha ao integrar Impressão de Documentos.");
                }
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Marfrig.";

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber> ObterListaFiltradaCTeAConsultar(List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber> ListaIntegrar)
        {
            List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber> listaRetornar = new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber>();

            foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber integracao in ListaIntegrar)
            {
                if (integracao.CTe.Titulo == null)
                {
                    listaRetornar.Add(integracao);
                    continue;
                }

                //REGRAS
                //2.1 - Consultar 7 dias antes do vencimento(para validar se houve alguma alteração do título antes do vencimento);
                //2.2 - Toda terça - feira da semana do vencimento do título(caso não quitado);
                //2.3 - Na data do vencimento(caso não quitado);
                //2.4 - Toda terça após a data do vencimento(caso não quitado);

                if (integracao.CTe.Titulo.DataVencimento.HasValue && integracao.CTe.Titulo.DataVencimento.Value != DateTime.MinValue)
                {
                    DateTime dataVenciamento = integracao.CTe.Titulo.DataVencimento.Value;
                    if (dataVenciamento > DateTime.Now && (dataVenciamento - DateTime.Now.Date).TotalDays == 7)
                    {
                        listaRetornar.Add(integracao);
                        continue;
                    }

                    if (integracao.CTe.Titulo.StatusTitulo != StatusTitulo.Quitada)
                    {
                        //no dia do vencimento.
                        if (integracao.CTe.Titulo.DataVencimento.Value.Date == DateTime.Now.Date)
                        {
                            listaRetornar.Add(integracao);
                            continue;
                        }

                        //se ja venceu, toda terça vai consultar.
                        if (DateTime.Now > dataVenciamento && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            listaRetornar.Add(integracao);
                            continue;
                        }
                        else
                        {
                            //ainda nao venceu, mas esta na semana do vencimento, vai consultar na terça
                            if ((dataVenciamento.Date - DateTime.Now.Date).TotalDays <= 7 && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
                            {
                                listaRetornar.Add(integracao);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        //titulo ja quitado mas com data de quitacao menor que 3 dias de hoje.
                        if (integracao.DataQuitacaoCadastro.HasValue && diferencaDiasUteis(integracao.DataQuitacaoCadastro.Value, DateTime.Now) < 4)
                        {
                            listaRetornar.Add(integracao);
                            continue;
                        }
                    }
                }
                else
                {
                    listaRetornar.Add(integracao);
                    continue;
                }
            }

            return listaRetornar;
        }

        #endregion Métodos Publicos
    }

}


