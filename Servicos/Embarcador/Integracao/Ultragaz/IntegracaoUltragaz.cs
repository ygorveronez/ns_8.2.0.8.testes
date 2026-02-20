using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Integracao.Ultragaz
{
    public sealed class IntegracaoUltragaz
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ConfiguracaoIntegracao _configuracaoIntegracao = null;

        #endregion

        #region Construtores

        public IntegracaoUltragaz(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Privados Request

        private WebRequest CriaRequisicao(string url, string metodo, string body, List<(string Chave, string Valor)> headers = null, string contentType = "application/json")
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebRequest requisicao = WebRequest.Create(url);

            //byte[] byteArrayDadosRequisicao = Encoding.ASCII.GetBytes(body);
            byte[] byteArrayDadosRequisicao = Encoding.UTF8.GetBytes(body);

            requisicao.Method = metodo;
            requisicao.ContentType = contentType;

            foreach (var (Chave, Valor) in (headers ?? new List<(string Chave, string Valor)>()))
                requisicao.Headers[Chave] = Valor;

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
                    throw new ServicoException("Falha ao processar o retorno da API");

                return (HttpWebResponse)webException.Response;
            }
        }

        private string ObterResposta(HttpWebResponse response)
        {
            string jsonDadosRetornoRequisicao;
            using (System.IO.Stream streamDadosRetornoRequisicao = response.GetResponseStream())
            {
                System.IO.StreamReader leitorDadosRetornoRequisicao = new System.IO.StreamReader(streamDadosRetornoRequisicao);
                jsonDadosRetornoRequisicao = leitorDadosRetornoRequisicao.ReadToEnd();
                leitorDadosRetornoRequisicao.Close();
            }


            return jsonDadosRetornoRequisicao;
        }

        private bool IsRetornoIsRetornoSucessoSucesso(HttpWebResponse retornoRequisicao, string jsonDadosRetornoRequisicao)
        {
            if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                return true;

            if (retornoRequisicao.StatusCode == HttpStatusCode.Created)
                return true;

            return false;
        }

        #endregion Métodos Privados Request

        #region Métodos Privados Autenticação

        private string ObterBodyRequisicaoAutenticacao()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RequisicaoAutenticacao requisicaoAutenticacao = ObterObjetoRequisicaoAutenticacao();
            string body = JsonConvert.SerializeObject(requisicaoAutenticacao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });

            return body;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RespostaAutenticacao ObterBodyConvertidoRespostaAutenticacao(string body)
        {
            return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RespostaAutenticacao>(body);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RequisicaoAutenticacao ObterObjetoRequisicaoAutenticacao()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RequisicaoAutenticacao()
            {
                TipoAutenticacao = "client_credentials"
            };
        }

        private List<(string Chave, string Valor)> ObterHeadersRequisicaoAutenticacao()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ConfiguracaoIntegracao configuracao = ObterConfiguracao();

            string headerKey = "Authorization";
            string headerValue = $"Basic {Utilidades.String.Base64Encode($"{configuracao.Identificacao}:{configuracao.Senha}")}";

            var headers = new List<(string Chave, string Valor)>() {
                ValueTuple.Create(headerKey, headerValue)
            };

            return headers;
        }

        private void Autenticar(out string request, out string response)
        {
            request = "";
            response = "";

            if (CacheTokenAcesso.IsTokenCacheValido())
                return;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ConfiguracaoIntegracao configuracao = ObterConfiguracao();

            string bodyAutenticacao = ObterBodyRequisicaoAutenticacao();
            List<(string Chave, string Valor)> headersAutenticacao = ObterHeadersRequisicaoAutenticacao();

            WebRequest requisicao = CriaRequisicao(configuracao.UrlAutenticacao, "POST", bodyAutenticacao, headersAutenticacao);

            using (HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao))
            {
                string bodyResposta = ObterResposta(retornoRequisicao);

                request = bodyAutenticacao;
                response = bodyResposta;

                Log.TratarErro($"URL: {configuracao.UrlAutenticacao}\nHeaders:{JsonConvert.SerializeObject(headersAutenticacao)}\nRequest:\n{bodyAutenticacao}\n\nResponse:\n{bodyResposta}", "ULTRAGAZ");

                if (!IsRetornoIsRetornoSucessoSucesso(retornoRequisicao, bodyResposta))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ErrorResponse error = null;
                    try
                    {
                        error = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ErrorResponse>(bodyResposta);
                    }
                    catch (Exception ex) 
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de erro da API Ultragaz: {ex.ToString()}", "CatchNoAction");
                    }

                    if (error == null)
                        throw new ServicoException("Ocorreu uma falha ao autenticar com a API.");
                    else
                    {
                        string erroTratado = error.result;
                        if (error.errors != null && error.errors.Count > 0)
                            erroTratado = string.Join(";", (from obj in error.errors select obj.message).ToList());
                        throw new ServicoException(error.status + " - " + erroTratado);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RespostaAutenticacao respostaAutenticacao = ObterBodyConvertidoRespostaAutenticacao(bodyResposta);

                CacheTokenAcesso.SetCache(respostaAutenticacao.TokenAcesso, respostaAutenticacao.TempoExpiracao);
            }
        }

        #endregion Métodos Privados Autenticação

        #region Métodos Privados Integração

        private string ObterBodyRequisicaoIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Empresa transportador, Dominio.Entidades.Veiculo veiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RequisicaoIntegracao requisicaoIntegracao = ObterObjetoRequisicaoIntegracao(carga, transportador, veiculo);
            string body = JsonConvert.SerializeObject(requisicaoIntegracao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            return body;
        }

        private string ObterBodyRequisicaoIntegracao(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.DadosContabilizacao requisicaoIntegracao = ObterObjetoRequisicaoIntegracao(pagamentoIntegracao);
            string body = JsonConvert.SerializeObject(requisicaoIntegracao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            body = body.Replace("round_", "");
            return body;
        }

        private string ObterBodyRequisicaoIntegracao(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RequisicaoVeiculo requisicaoIntegracao = ObterObjetoRequisicaoIntegracao(veiculoIntegracao);
            string body = JsonConvert.SerializeObject(requisicaoIntegracao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            return body;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RequisicaoIntegracao ObterObjetoRequisicaoIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Empresa transportador, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            string cnpjFilialDestino = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo)?.Pedido?.Destinatario?.CPF_CNPJ_SemFormato;
            Dominio.Entidades.Embarcador.Filiais.Filial filialDestino = repFilial.BuscarPorCNPJ(cnpjFilialDestino);

            if (filialDestino == null)
                throw new ServicoException("Não foi encontrada a filial de destino.");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RequisicaoIntegracao()
            {
                FilialOrigemDescricao = null, // carga.Filial.Descricao
                FilialOrigemCNPJ = "0" + carga.Filial.CNPJ,
                FilialOrigemCidade = Utilidades.String.RemoveAccents(carga.Filial.Localidade.Descricao).ToUpper(),
                FilialOrigemUF = carga.Filial.Localidade.Estado.Sigla,
                FilialOrigemIBGE = carga.Filial.Localidade.CodigoIBGE.ToString(),

                FilialDestinoDescricao = null, // filialDestino.Descricao,
                FilialDestinoCNPJ = "0" + filialDestino.CNPJ,
                FilialDestinoCidade = Utilidades.String.RemoveAccents(filialDestino.Localidade.Descricao).ToUpper(),
                FilialDestinoUF = filialDestino.Localidade.Estado.Sigla,
                FilialDestinoIBGE = filialDestino.Localidade.CodigoIBGE.ToString(),

                PlacaVeiculo = veiculo.Placa,
                CNPJTransportador = "0" + transportador.CNPJ_SemFormato,
            };
        }

        private List<(string Chave, string Valor)> ObterHeadersRequisicaoIntegracao()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ConfiguracaoIntegracao configuracao = ObterConfiguracao();
            string tokenAcesso = CacheTokenAcesso.ObterToken();

            var headers = new List<(string Chave, string Valor)>() {
                ValueTuple.Create("client_id", configuracao.Identificacao),
                ValueTuple.Create("access_token", tokenAcesso),
            };

            return headers;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ValidadeVeiculoTransportador ObterInformacoesTransportadorEVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Empresa transportador, Dominio.Entidades.Veiculo veiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ConfiguracaoIntegracao configuracao = ObterConfiguracao();

            string bodyIntegracao = ObterBodyRequisicaoIntegracao(carga, transportador, veiculo);
            List<(string Chave, string Valor)> headersIntegracao = ObterHeadersRequisicaoIntegracao();

            using (HttpWebResponse retornoRequisicao = (HttpWebResponse)CriaRequisicao(configuracao.UrlIntegracao, "POST", bodyIntegracao, headersIntegracao).GetResponse())
            {
                string bodyResposta = ObterResposta(retornoRequisicao);

                Log.TratarErro($"URL: {configuracao.UrlIntegracao}\nHeaders:{JsonConvert.SerializeObject(headersIntegracao)}\nRequest:\n{bodyIntegracao}\n\nResponse:\n{bodyResposta}", "ULTRAGAZ");

                if (!IsRetornoIsRetornoSucessoSucesso(retornoRequisicao, bodyResposta))
                    throw new ServicoException("Ocorreu uma falha ao integrar com a API.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RespostaIntegracao respostaIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RespostaIntegracao>(bodyResposta);

                if (!string.IsNullOrWhiteSpace(respostaIntegracao.MensagemErro))
                    throw new ServicoException($"Ocorreu uma falha ao integrar com a API. Mensagem: {respostaIntegracao.MensagemErro}");

                return new Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ValidadeVeiculoTransportador
                {
                    TransportadorValido = respostaIntegracao.SituacaoTransportador,
                    VeiculoValido = respostaIntegracao.SituacaoVeiculo,
                };
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.DadosContabilizacao ObterObjetoRequisicaoIntegracao(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalContabilizacao repXMLNotaFiscalContabilizacao = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalContabilizacao(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.DadosContabilizacao dadosContabilizacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.DadosContabilizacao();
            dadosContabilizacao.accounting = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Accounting();
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = pagamentoIntegracao.DocumentoFaturamento.CTe;
            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();
            decimal aliquotaCOFINS = empresaPai?.Configuracao?.AliquotaCOFINS ?? 0;
            decimal aliquotaPIS = empresaPai?.Configuracao?.AliquotaPIS ?? 0;
            bool IsCTe = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe;
            bool IsNFSe = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe;
            bool IsNFS = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS;

            List<int> codigoNotas = (from obj in cte.XMLNotaFiscais select obj.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (pagamentoIntegracao.DocumentoFaturamento.CargaPagamento != null)
            {
                if (pagamentoIntegracao.DocumentoFaturamento.CargaPagamento.CargaAgrupada)
                    pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaAgrupadaECtes(codigoNotas, pagamentoIntegracao.DocumentoFaturamento.CargaPagamento.Codigo);
                else
                    pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaECtes(codigoNotas, pagamentoIntegracao.DocumentoFaturamento.CargaPagamento.Codigo);
            }
            else
            {
                if (pagamentoIntegracao.DocumentoFaturamento.LancamentoNFSManual != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentoParaEmissaoNFSManuals = repCargaDocumentoParaEmissaoNFSManual.BuscarPorLancamentoNFsManual(pagamentoIntegracao.DocumentoFaturamento.LancamentoNFSManual.Codigo);
                    pedidoXMLNotasFiscais = (from obj in cargaDocumentoParaEmissaoNFSManuals select obj.PedidoXMLNotaFiscal).Distinct().ToList();
                }
            }

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao> dadosContabilizacoes = repXMLNotaFiscalContabilizacao.BuscarPorXMLNotasFiscais(codigoNotas);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Header header = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Header();
            header.source = cte.TomadorPagador.Nome.ToLower().Contains("bahiana") ? "LOGISTIC-BA" : "LOGISTIC-UG";
            header.process_flag = 1;
            header.gl_date = pagamentoIntegracao.Pagamento.DataCriacao.ToString("yyyy-MM-dd"); //"2008-09-28",
            header.freight_flag = "N";
            header.document_type = cte.Empresa.Tipo == "F" ? "CPF" : "CGC";
            header.cnpj_issuer = cte.Empresa.CNPJ_SemFormato;
            header.invoice_num = cte.Numero;
            header.series = cte.Serie.Numero.ToString();
            header.cnpj_recipient = cte.TomadorPagador.CPF_CNPJ_SemFormato;

            if (IsCTe)
                header.invoice_amount = cte.ValorAReceber;
            else
            {
                if (cte.PercentualISSRetido == 0)
                    header.invoice_amount = cte.BaseCalculoISS;
                else if (cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && !cte.ISSRetido)
                    header.invoice_amount = cte.ValorAReceber - cte.ValorISS;
                else
                    header.invoice_amount = cte.ValorAReceber;
            }

            if (IsCTe)
                header.gross_total_amount = cte.ValorAReceber;
            else
                header.gross_total_amount = cte.BaseCalculoISS;

            header.invoice_date = cte.DataEmissao.Value.ToString("yyyy-MM-dd");
            header.freight_spec = dadosContabilizacoes.FirstOrDefault()?.CodigoTransacaoRecebimento ?? "";
            header.icms_tax = (int)cte.AliquotaICMS;

            if (IsCTe)
            {
                header.fiscal_document_model = "CT-E";
                if (cte.CST == "" || cte.CST == "40" || cte.CST == "51")
                    header.icms_type = "ISENTO";
                else if (cte.CST == "60")
                {
                    header.icms_type = "SUBSTITUTO";
                }
                else if (cte.CST == "90" || cte.CST == "91")
                {
                    if (cte.Empresa.OptanteSimplesNacional && cte.LocalidadeInicioPrestacao.Estado.Sigla == cte.Empresa.Localidade.Estado.Sigla)
                        header.icms_type = "ISENTO";
                    else
                        header.icms_type = "NORMAL";
                }
                else
                    header.icms_type = "NORMAL";
            }
            else
            {
                header.fiscal_document_model = "NFS";
                header.icms_type = "NÃO SE APLICA";
            }

            header.icms_base = Math.Round(cte.BaseCalculoICMS, 2);
            header.icms_amount = cte.ValorICMS;
            header.ipi_amount = 0;
            header.origin_state = cte.LocalidadeInicioPrestacao.Estado.Sigla;
            header.destination_state = cte.LocalidadeTerminoPrestacao.Estado.Sigla;
            header.ir_vendor = "4"; //cte.ValorBaseCalculoIR > 0 ? "1" : "4";

            header.reo_attribute4 = "1";
            if (IsNFSe || IsNFS)
            {
                header.ir_categ = "17080150";
                header.iss_city_code = cte.LocalidadeInicioPrestacao.Descricao.ToUpper();
            }
            else
            {
                header.ir_categ = null;
                header.iss_city_code = null;
            }

            header.source_items = "0";
            header.terms_date = pagamentoIntegracao.Pagamento.DataCriacao.ToString("yyyy-MM-dd");  //"2018-11-01-03:00",
            if (!string.IsNullOrWhiteSpace(cte.ChaveAcesso) && IsCTe)
                header.eletronic_invoice_key = cte.ChaveAcesso;
            else if (IsNFSe || IsNFS)
                header.eletronic_invoice_key = pedidoXMLNotasFiscais?.FirstOrDefault()?.XMLNotaFiscal.Chave ?? "";
            else
                header.eletronic_invoice_key = null;

            header.inss_base = 0;
            header.inss_tax = 0;
            header.inss_amount = 0;
            header.invoice_weight = (int)cte.Peso;

            if ((IsNFSe || IsNFS) && cte.PercentualISSRetido == 0)
            {
                header.iss_tax = 0;
                header.ISS_AMOUNT = 0;
                header.iss_base = 0;
            }
            else
            {
                header.iss_tax = cte.AliquotaISS;
                header.ISS_AMOUNT = cte.ValorISS;
                header.iss_base = cte.BaseCalculoISS;
            }

            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                header.reo_attribute2 = "1";
            else
                header.reo_attribute2 = "2";

            if (pedidoXMLNotasFiscais.FirstOrDefault()?.CargaPedido.Carga.VeiculosVinculados.Count > 0)
                header.reo_attribute5 = pedidoXMLNotasFiscais.FirstOrDefault().CargaPedido.Carga.VeiculosVinculados.FirstOrDefault().Placa;
            else
                header.reo_attribute5 = pedidoXMLNotasFiscais.FirstOrDefault()?.CargaPedido.Carga.Veiculo?.Placa ?? "";

            header.reo_attribute3 = cte.Empresa.CNPJ_SemFormato;

            header.ship_via_lookup_code = "R";
            header.receive_date = DateTime.Now.ToString("yyyy-MM-dd");
            header.reo_attribute10 = null;
            header.source_ibge_code = cte.LocalidadeInicioPrestacao.CodigoIBGE.ToString();
            header.destination_ibge_code = cte.LocalidadeTerminoPrestacao.CodigoIBGE.ToString();

            if (IsCTe)
                header.model = cte.ModeloDocumentoFiscal.Numero;
            else
                header.model = "32";

            header.protocol = cte.Codigo.ToString();
            header.reo_attribute12 = pagamentoIntegracao.Pagamento.Numero.ToString();
            header.reo_attribute13 = pagamentoIntegracao.DocumentoFaturamento?.CargaPagamento?.CodigoCargaEmbarcador ?? string.Empty;
            header.reo_attribute14 = pagamentoIntegracao.DocumentoFaturamento?.CargaPagamento?.Pedidos?.FirstOrDefault()?.DescricaoTipoPagamentoCIFFOB ?? string.Empty;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Line> lines = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Line>();
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = (from obj in pedidoXMLNotasFiscais select obj.XMLNotaFiscal).FirstOrDefault();

            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoNotaFiscal = (from obj in pedidoXMLNotasFiscais where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo select obj).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(pedidoNotaFiscal.CargaPedido.Pedido.NumeroControle) && repPedidoXMLNotaFiscal.PossuiNotaFilhoPorCargaENumeroControle(pedidoNotaFiscal.CargaPedido.Carga.Codigo, pedidoNotaFiscal.CargaPedido.Pedido.NumeroControle, xmlNotaFiscal.Codigo))
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisFilhos = repPedidoXMLNotaFiscal.BuscarPorCargaENumeroControle(pedidoNotaFiscal.CargaPedido.Carga.Codigo, pedidoNotaFiscal.CargaPedido.Pedido.NumeroControle, xmlNotaFiscal.Codigo);
                List<int> notasFilho = (from obj in pedidoXMLNotasFiscaisFilhos select obj.XMLNotaFiscal.Codigo).Distinct().ToList();

                if (notasFilho.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao> dadosContabilizacoesFilho = repXMLNotaFiscalContabilizacao.BuscarPorXMLNotasFiscais(notasFilho);
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao dadoContabil = (from obj in dadosContabilizacoes where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalFilho = pedidoXMLNotasFiscaisFilhos.FirstOrDefault();
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao dadoContabilFilho = (from obj in dadosContabilizacoesFilho where obj.XMLNotaFiscal.Codigo == pedidoXMLNotaFiscalFilho.XMLNotaFiscal.Codigo select obj).FirstOrDefault();
                    string contaTransacao = "";
                    if (!string.IsNullOrWhiteSpace(dadoContabil?.ContaTransacao))
                    {
                        string[] segmentosContaTransacao = dadoContabil.ContaTransacao.Split('.');
                        for (int i = 0; i < segmentosContaTransacao.Length; i++)
                        {
                            string segmento = segmentosContaTransacao[i];
                            if (i == 3 && dadoContabilFilho != null)
                                segmento = dadoContabilFilho.UC;

                            if (i > 0)
                                contaTransacao += '.';

                            contaTransacao += segmento;
                        }
                    }
                    header.freight_spec = dadoContabilFilho?.CodigoTransacaoRecebimento ?? "";

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Line line = gerarLineContabilizacao(cte, pedidoXMLNotaFiscalFilho.CargaPedido, dadoContabil, contaTransacao, dadoContabilFilho?.CFOPEntrada ?? "", aliquotaPIS, aliquotaCOFINS, header, cte.ValorAReceber, cte.ValorICMS, cte.ValorISS, cte.BaseCalculoICMS, cte.BaseCalculoISS, dadoContabilFilho?.ItemFrete, configuracaoFinanceiro);
                    lines.Add(line);
                }
            }
            else
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao dadoContabil = (from obj in dadosContabilizacoes where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Line line = gerarLineContabilizacao(cte, pedidoNotaFiscal.CargaPedido, dadoContabil, dadoContabil?.ContaTransacao ?? "", dadoContabil?.CFOPEntrada, aliquotaPIS, aliquotaCOFINS, header, cte.ValorAReceber, cte.ValorICMS, cte.ValorISS, cte.BaseCalculoICMS, cte.BaseCalculoISS, dadoContabil?.ItemFrete, configuracaoFinanceiro);
                lines.Add(line);
            }

            dadosContabilizacao.accounting.Header = header;
            dadosContabilizacao.accounting.Line = lines;

            return dadosContabilizacao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Line gerarLineContabilizacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao dadoContabil, string contaTransacao, string cfop, decimal aliquotaPIS, decimal aliquotaCOFINS, Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Header header, decimal valorFreteReceber, decimal valorICMS, decimal valorISS, decimal baseCalculoICMS, decimal baseCalculoISS, string itemFrete, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Line line = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.Line();
            bool IsCTe = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe;
            bool IsNFSe = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe;
            bool IsNotCTe = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe;
            bool IsNFS = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS;

            if (string.IsNullOrWhiteSpace(contaTransacao))
                line.accounting_combination = null;
            else
                line.accounting_combination = contaTransacao;

            if (cfop == "1360")
                header.icms_type = "SUBSTITUTO";

            line.cfo_code = cfop;
            line.uom = "UNIDADE";
            line.quantity = 1;
            line.unit_price = Math.Round(valorFreteReceber, 2);
            line.icms_tax_code = 2;

            if (IsCTe)
            {
                if (header.icms_type != "ISENTO")
                    line.icms_tax_code = 1;
            }
            else if ((IsNFSe || IsNFS) && cte.PercentualISSRetido > 0)
            {
                if (cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && !cte.ISSRetido)
                    line.unit_price = cte.ValorAReceber - cte.ValorISS;
                else
                    line.unit_price = cte.ValorAReceber;
            }
            else
            {
                if (cte.PercentualISSRetido == 0)
                    line.unit_price = Math.Round(cte.BaseCalculoISS, 2);
                else if (cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
                    line.unit_price = Math.Round(valorFreteReceber - valorISS, 2);
            }

            line.diff_icms_tax = 0;
            line.diff_icms_amount = 0;

            if (cte.CST != "60")
                line.icms_amount_recover = cargaPedido.ICMSPagoPorST ? valorICMS : 0;

            line.icms_base = Math.Round(baseCalculoICMS, 2);
            line.icms_tax = (int)cte.AliquotaICMS;
            line.icms_amount = valorICMS;

            if ((IsNFSe || IsNFS) && cte.PercentualISSRetido == 0)
                line.iss_base_amount = 0;
            else if (IsNotCTe)
                line.iss_base_amount = baseCalculoISS;

            line.ipi_tax = 0;
            line.ipi_amount = 0;
            line.ipi_amount_recover = 0;
            line.ipi_tax_code = 2;

            if (IsCTe || IsNFS)
            {
                if (!cte.ISSRetido)
                    line.total_amount = Math.Round(valorFreteReceber, 2);
                else
                    line.total_amount = Math.Round(valorFreteReceber + cte.ValorISS, 2);
            }
            else
                line.total_amount = Math.Round(valorFreteReceber + cte.ValorISS, 2);

            if (IsCTe || IsNFS)
                line.ipi_base_amount = line.total_amount;
            else
                line.ipi_base_amount = line.total_amount + valorISS;

            line.diff_icms_amount_recover = 0;

            if (cte.CST == "40" || cte.CST == "" || cte.CST == "41" || cte.CST == "51")
                line.tributary_status_code = "040";
            else if (cte.CST == "60")
                line.tributary_status_code = "010";
            else
            {
                if (cte.Empresa.OptanteSimplesNacional && cte.LocalidadeInicioPrestacao.Estado.Sigla == cte.Empresa.Localidade.Estado.Sigla)
                    line.tributary_status_code = "040";
                else
                    line.tributary_status_code = "000";
            }

            line.ipi_tributary_code = 49;
            if (string.IsNullOrWhiteSpace(itemFrete))
                line.item = null;
            else
                line.item = itemFrete;

            if (dadoContabil?.CalcularPisCofins ?? false)
            {
                bool naoIncluirICMSBaseCalculoPisCofins = configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins;

                if (IsCTe && header.gross_total_amount > 0 && baseCalculoICMS == 0)
                    naoIncluirICMSBaseCalculoPisCofins = true;

                if (IsCTe || IsNFS)
                {
                    if (naoIncluirICMSBaseCalculoPisCofins && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
                        line.pis_base_amount = Math.Round(line.total_amount - line.icms_amount, 2);
                    else
                    {
                        if (IsCTe)
                            line.pis_base_amount = Math.Round(baseCalculoICMS, 2);
                        else
                            line.pis_base_amount = Math.Round(baseCalculoISS, 2);
                    }

                }
                else
                    line.pis_base_amount = Math.Round(line.total_amount + valorISS, 2);

                decimal valorPIS = Math.Round(line.pis_base_amount * (aliquotaPIS / 100), 2, MidpointRounding.AwayFromZero);

                line.pis_tax_rate = aliquotaPIS;
                line.pis_amount_recover = 0;
                line.pis_amount = valorPIS;
                line.pis_tributary_code = "51";

                if (IsCTe || IsNFS)
                {
                    if (naoIncluirICMSBaseCalculoPisCofins && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
                        line.cofins_base_amount = Math.Round(line.total_amount - line.icms_amount, 2);
                    else
                    {
                        if (IsCTe)
                            line.cofins_base_amount = Math.Round(baseCalculoICMS, 2);
                        else
                            line.cofins_base_amount = Math.Round(baseCalculoISS, 2);
                    }
                }

                else
                    line.cofins_base_amount = Math.Round(line.total_amount + valorISS, 2);

                decimal valorCOFINS = Math.Round(line.cofins_base_amount * (aliquotaCOFINS / 100), 2, MidpointRounding.AwayFromZero);

                line.cofins_tax_rate = aliquotaCOFINS;
                line.cofins_amount_recover = 0;
                line.cofins_amount = valorCOFINS;
                line.cofins_tributary_code = "51";
            }
            else
            {
                line.pis_tributary_code = "70";
                line.cofins_tributary_code = "70";
            }

            if ((IsNFSe || IsNFS) && cte.PercentualISSRetido == 0)
            {
                line.iss_tax_amount = 0;
                line.iss_tax_rate = 0;
            }
            else
            {
                line.iss_tax_amount = valorISS;
                line.iss_tax_rate = cte.AliquotaISS;
            }

            line.receipt_flag = "N";
            line.ibsuf_tax = cte.AliquotaIBSEstadual;
            line.ibsuf_amount = cte.ValorIBSEstadual;
            line.ibsmun_tax = cte.AliquotaIBSMunicipal;
            line.ibsmun_amount = cte.ValorIBSMunicipal;
            line.classtrib = cte.ClassificacaoTributariaIBSCBS;
            line.cbs_tax = cte.AliquotaCBS;
            line.cbs_amount = cte.ValorCBS;
            line.ibs_cbs_tax_code = cte.CSTIBSCBS;
            line.cod_linha_xml = "1";
            line.crt = cte.Empresa.OptanteSimplesNacional ? "3" : "1";

            return line;
        }

        private void ObterInformacoesContabilizacao(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao, out string jsonRequisicao, out string jsonRetorno)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ConfiguracaoIntegracao configuracao = ObterConfiguracao();

            string bodyIntegracao = ObterBodyRequisicaoIntegracao(pagamentoIntegracao);
            List<(string Chave, string Valor)> headersIntegracao = ObterHeadersRequisicaoIntegracao();

            using (HttpWebResponse retornoRequisicao = (HttpWebResponse)CriaRequisicao(configuracao.URLContabilizacao, "POST", bodyIntegracao, headersIntegracao).GetResponse())
            {
                string bodyResposta = ObterResposta(retornoRequisicao);

                jsonRequisicao = $"Headers:{JsonConvert.SerializeObject(headersIntegracao)}\nRequest:\n{bodyIntegracao}";
                jsonRetorno = bodyResposta;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RespostaDadosContabilizacao respostaIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RespostaDadosContabilizacao>(bodyResposta);

                if (respostaIntegracao?.Erro != null && respostaIntegracao.Erro.Codigo.Equals("504"))
                    throw new ServicoException($"Servidor indisponível. Uma nova tentativa de reenvio será feita em instantes.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                if (respostaIntegracao?.Erro != null && respostaIntegracao.Erro.Status.Equals("E"))
                    throw new ServicoException($"Ocorreu uma falha ao integrar com a API. Mensagem: {respostaIntegracao.Erro.Mensagem}");

                if (!IsRetornoIsRetornoSucessoSucesso(retornoRequisicao, bodyResposta))
                    throw new ServicoException("Ocorreu uma falha ao integrar com a API.");
            }
        }

        private string ObterInformacoesVeiculo(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao, out string jsonRequisicao, out string jsonRetorno)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ConfiguracaoIntegracao configuracao = ObterConfiguracao();

            string bodyIntegracao = ObterBodyRequisicaoIntegracao(veiculoIntegracao);
            List<(string Chave, string Valor)> headersIntegracao = ObterHeadersRequisicaoIntegracao();

            using (HttpWebResponse retornoRequisicao = (HttpWebResponse)CriaRequisicao(configuracao.URLIntegracaoVeiculo, "POST", bodyIntegracao, headersIntegracao).GetResponse())
            {
                string bodyResposta = ObterResposta(retornoRequisicao);

                jsonRequisicao = $"Headers:{JsonConvert.SerializeObject(headersIntegracao)}\nRequest:\n{bodyIntegracao}";
                jsonRetorno = bodyResposta;

                if (!IsRetornoIsRetornoSucessoSucesso(retornoRequisicao, bodyResposta))
                    throw new ServicoException("Ocorreu uma falha ao integrar com a API.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RespostaRequisicaoVeiculo respostaIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RespostaRequisicaoVeiculo>(bodyResposta);

                if (respostaIntegracao == null)
                    throw new ServicoException("Não foi possível converter a resposta da API.");

                if (respostaIntegracao.Erro != null)
                    throw new ServicoException($"Erro: {respostaIntegracao.Erro.Codigo} - {respostaIntegracao.Erro.Mensagem} Detalhe: {respostaIntegracao.Erro.Detalhe}");

                return respostaIntegracao.RetornoSucesso;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RequisicaoVeiculo ObterObjetoRequisicaoIntegracao(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao)
        {
            Dominio.Entidades.Veiculo veiculo = veiculoIntegracao.Veiculo;
            Dominio.Entidades.Empresa transportador = veiculo.Empresa;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.RequisicaoVeiculo()
            {
                CnpjTransportador = transportador.CNPJ,
                RazaoSocialTransportador = transportador.RazaoSocial,
                Placa = veiculo.Placa,
                UF = veiculo.Estado.Sigla,
                CodigoIntegracaoModeloVeicular = veiculo.ModeloVeicularCarga.CodigoIntegracao,
                Ativo = veiculo.Ativo ? "Y" : "N",
                SistemaOrigem = "MLT"
            };
        }

        #endregion Métodos Privados Integração

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ConfiguracaoIntegracao ObterConfiguracao()
        {
            if (_configuracaoIntegracao != null)
                return _configuracaoIntegracao;

            Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.Buscar();

            if (!(integracao?.PossuiIntegracaoUltragaz ?? false))
                throw new ServicoException("Não foram configurados os dados de integração com a Ultragaz");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ConfiguracaoIntegracao configuracaoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ConfiguracaoIntegracao()
            {
                UrlAutenticacao = integracao.URLAutenticacaoUltragaz,
                UrlIntegracao = integracao.URLIntegracaoUltragaz,
                Identificacao = integracao.ClientIdUltragaz,
                Senha = integracao.ClientSecretUltragaz,
                URLContabilizacao = integracao.URLContabilizacaoUltragaz,
                URLIntegracaoVeiculo = integracao.URLIntegracaoVeiculoUltragaz
            };

            return _configuracaoIntegracao = configuracaoIntegracao;
        }

        private void AdicionarArquivoTransacaoIntegracaoPagamento(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao, string mensagem, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork, bool novaTentativa)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = null;

            if (novaTentativa)
                arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, pagamentoIntegracao.DataIntegracao.AddMinutes(-11), mensagem, unitOfWork);
            else
                arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, pagamentoIntegracao.DataIntegracao, mensagem, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (pagamentoIntegracao.ArquivosTransacao == null)
                pagamentoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            pagamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void AdicionarArquivoTransacaoVeiculoIntegracao(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao, string mensagem, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, veiculoIntegracao.DataIntegracao, mensagem, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (veiculoIntegracao.ArquivosTransacao == null)
                veiculoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            veiculoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo AdicionarArquivoTransacao(string jsonRequisicao, string jsonRetorno, DateTime data, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Data = data,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz.ValidadeVeiculoTransportador ValidarSituacaoTransportadorEVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Empresa transportador, Dominio.Entidades.Veiculo veiculo)
        {
            string request = "";
            string response = "";
            Autenticar(out request, out response);

            return ObterInformacoesTransportadorEVeiculo(carga, transportador, veiculo);
        }

        public void IntegrarDocumentosContabilizacao(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
            string jsonRequisicao = "";
            string jsonRetorno = "";

            pagamentoIntegracao.DataIntegracao = DateTime.Now;
            pagamentoIntegracao.NumeroTentativas++;

            bool novaTentativa = false;

            try
            {
                Autenticar(out jsonRequisicao, out jsonRetorno);
                ObterInformacoesContabilizacao(pagamentoIntegracao, out jsonRequisicao, out jsonRetorno);
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                pagamentoIntegracao.ProblemaIntegracao = "Ag retorno da Ultragaz";
            }
            catch (ServicoException ex)
            {
                if (ex.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                {
                    novaTentativa = true;

                    pagamentoIntegracao.DataIntegracao = pagamentoIntegracao.DataIntegracao.AddMinutes(11);
                    pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    pagamentoIntegracao.ProblemaIntegracao = ex.Message;
                }
                else
                {
                    pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pagamentoIntegracao.ProblemaIntegracao = ex.Message;
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar os dados da contabilização";
            }

            AdicionarArquivoTransacaoIntegracaoPagamento(pagamentoIntegracao, pagamentoIntegracao.ProblemaIntegracao, jsonRequisicao, jsonRetorno, _unitOfWork, novaTentativa);
            repPagamentoIntegracao.Atualizar(pagamentoIntegracao);
        }

        public void IntegrarVeiculo(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            veiculoIntegracao.DataIntegracao = DateTime.Now;
            veiculoIntegracao.NumeroTentativas++;

            try
            {
                Autenticar(out jsonRequisicao, out jsonRetorno);
                string mensagem = ObterInformacoesVeiculo(veiculoIntegracao, out jsonRequisicao, out jsonRetorno);

                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                veiculoIntegracao.ProblemaIntegracao = mensagem;
            }
            catch (ServicoException ex)
            {
                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar o veículo";
            }

            AdicionarArquivoTransacaoVeiculoIntegracao(veiculoIntegracao, veiculoIntegracao.ProblemaIntegracao, jsonRequisicao, jsonRetorno, _unitOfWork);
            repositorioVeiculoIntegracao.Atualizar(veiculoIntegracao);
        }

        #endregion Métodos Públicos
    }
}

