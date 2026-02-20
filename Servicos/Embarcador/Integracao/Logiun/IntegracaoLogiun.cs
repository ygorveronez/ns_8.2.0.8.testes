using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Logiun
{
    public class IntegracaoLogiun
    {
        public static bool Realizarlogin(Repositorio.UnitOfWork unitOfWork, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, out string msgRetorno, out string token)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            msgRetorno = "";
            token = "";

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoLogiun || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioLogiun) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaLogiun))
            {
                msgRetorno = "Não existe configuração de integração disponível para a Logiun.";
                return false;
            }

            string urlWebService = configuracaoIntegracao.URLHomologacaoLogiun;
            if (tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                urlWebService = configuracaoIntegracao.URLProducaoLogiun;

            urlWebService += "/token";
            using (WebClient client = new WebClient())
            {
                client.QueryString.Add("username", configuracaoIntegracao.UsuarioLogiun);
                client.QueryString.Add("password", configuracaoIntegracao.SenhaLogiun);
                client.QueryString.Add("grant_type", "password");
                try
                {
                    var data = client.UploadValues(urlWebService, "POST", client.QueryString);
                    var responseString = UnicodeEncoding.UTF8.GetString(data);

                    if (responseString != "Credenciais inválidas")
                    {
                        token = responseString;
                        msgRetorno = "Conectado com sucesso";
                        return true;
                    }
                    else
                    {
                        token = "";
                        msgRetorno = responseString;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    token = "";
                    msgRetorno = ex.Message;
                    return false;
                }
            }
        }

        public static bool TestarConexaoLogin(Repositorio.UnitOfWork unitOfWork, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string token, out string msgRetorno)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            msgRetorno = "";

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoLogiun || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioLogiun) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaLogiun))
            {
                msgRetorno = "Não existe configuração de integração disponível para a Logiun.";
                return false;
            }

            string urlWebService = configuracaoIntegracao.URLHomologacaoLogiun;
            if (tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                urlWebService = configuracaoIntegracao.URLProducaoLogiun;

            urlWebService += "/helloworld";

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Authorization", "Bearer " + token);
                var data = client.DownloadString(urlWebService);
                var responseString = (string)data;

                if (!string.IsNullOrWhiteSpace(responseString) && responseString.Contains("Hello"))
                    return true;
                else
                {
                    msgRetorno = "Falha ao testar conexão com Logiun";
                    return false;
                }
            }
        }

        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCteParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoLogiun || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioLogiun) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaLogiun))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Logiun.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            if (cargaIntegracao.Carga.TipoOperacao == null || string.IsNullOrWhiteSpace(cargaIntegracao.Carga.TipoOperacao?.ProdutoLogiun) || string.IsNullOrWhiteSpace(cargaIntegracao.Carga.TipoOperacao?.CNPJTransportadoraLogiun))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Tipo de operação da carga não configurada corretamente.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            Dominio.Entidades.Usuario motorista = null;
            if (cargaIntegracao.Carga.Motoristas != null && cargaIntegracao.Carga.Motoristas.Count > 0)
                motorista = cargaIntegracao.Carga.Motoristas.FirstOrDefault();

            if (motorista == null)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Motorista não informado.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            Dominio.Entidades.Veiculo veiculo = null;
            if (cargaIntegracao.Carga.Veiculo != null)
                veiculo = cargaIntegracao.Carga.Veiculo;

            if (veiculo == null)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Veículo não informado.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            string msgRetorno = "";
            string token = "";
            if (!Realizarlogin(unitOfWork, cargaIntegracao.Carga.Empresa.TipoAmbiente, out msgRetorno, out token))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Falha ao realizar login. " + msgRetorno;
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            string urlWebService = configuracaoIntegracao.URLHomologacaoLogiun;

            if (cargaIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                urlWebService = configuracaoIntegracao.URLProducaoLogiun;

            urlWebService += "/transferencia";
            string produtoLogiun = cargaIntegracao.Carga.TipoOperacao?.ProdutoLogiun ?? "";
            bool todasNotasEnviadas = true;//não pode mudar para false devido a tratativa de vários envios por nota na carga
            string msgErro = "";
            string arquivosEnvios = "";
            string arquivosRetornos = "";

            if (cargaIntegracao.Carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada
                || cargaIntegracao.Carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho
                || cargaIntegracao.Carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> ctesAnteriores = repPedidoCteParaSubContratacao.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                foreach (var cte in ctesAnteriores)
                {
                    if (cte.CTeTerceiro.Emitente != null && cte.CTeTerceiro.Destinatario != null && cargaIntegracao.Carga.Empresa != null && cte.CTeTerceiro.QuantidadePaletes > 0 && ((int)cte.CTeTerceiro.QuantidadePaletes > 0))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.Transferencia transferencia = new Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.Transferencia()
                        {
                            CnpjEmpresaOrigem = cte.CTeTerceiro.Emitente?.CPF_CNPJ_SemFormato,
                            RazaoSocialOrigem = cte.CTeTerceiro.Emitente?.Nome,
                            CnpjEmpresaDestino = cte.CTeTerceiro.Destinatario?.CPF_CNPJ_SemFormato,
                            RazaoSocialDestino = cte.CTeTerceiro.Destinatario?.Nome,
                            CnpjEmpresaTransportadora = cargaIntegracao.Carga.TipoOperacao?.CNPJTransportadoraLogiun,
                            RazaoSocialTransportadora = cargaIntegracao.Carga.Empresa?.RazaoSocial,
                            DataTransferencia = cargaIntegracao.Carga.DataCarregamentoCarga.HasValue ? cargaIntegracao.Carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-ddTHH:mm:sszzz") : "",
                            NumeroViagem = cargaIntegracao.Carga.CodigoCargaEmbarcador,
                            DocumentoMotorista = motorista?.CPF,
                            NomeMotorista = motorista?.Nome,
                            PlacaVeiculo = veiculo?.Placa,
                            ValorFrete = cargaIntegracao.Carga.ValorAReceberCTes.HasValue ? cargaIntegracao.Carga.ValorAReceberCTes.Value : (decimal)0,
                            Observacao = "",
                            Embalagens = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.Embalagem>()
                        };
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.Embalagem embalagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.Embalagem()
                        {
                            CodigoEmbalagem = produtoLogiun,
                            NumeroNotaFiscal = cte.CTeTerceiro.Numero.ToString(),
                            SerieNotaFiscal = cte.CTeTerceiro.Serie.ToString(),
                            Quantidade = (int)cte.CTeTerceiro.QuantidadePaletes,
                            QuantidadeAvaria = 0,
                            ValorUnitarioEmbalagem = (decimal)0
                        };
                        transferencia.Embalagens.Add(embalagem);
                        using (WebClient client = new WebClient())
                        {
                            string jsonPost = JsonConvert.SerializeObject(transferencia, Formatting.Indented);
                            arquivosEnvios = jsonPost;
                            client.Headers.Add("Authorization", "Bearer " + token);
                            client.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                            client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            try
                            {
                                var data = client.UploadString(urlWebService, jsonPost);

                                var responseString = (string)data;
                                arquivosRetornos = " " + responseString;

                                if (!string.IsNullOrWhiteSpace(responseString) && responseString.Contains("sucesso"))
                                {
                                    if (todasNotasEnviadas)
                                        todasNotasEnviadas = true;
                                }
                                else
                                {
                                    msgErro = " " + arquivosRetornos;
                                    todasNotasEnviadas = false;
                                }

                                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosEnvios, "json", unitOfWork);
                                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                                arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;

                                if (todasNotasEnviadas)
                                {
                                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                    cargaIntegracao.ProblemaIntegracao = "Sucesso";
                                    arquivoIntegracao.Mensagem = "Sucesso";
                                    cargaIntegracao.Protocolo = "";
                                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosRetornos, "json", unitOfWork);
                                }
                                else
                                {
                                    arquivosRetornos = " " + msgErro;
                                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                    cargaIntegracao.ProblemaIntegracao = "Falha ao processar. " + msgErro;
                                    arquivoIntegracao.Mensagem = "Falha ao processar. " + msgErro;
                                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosRetornos, "json", unitOfWork);
                                }

                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                                repCargaIntegracao.Atualizar(cargaIntegracao);
                            }
                            catch (WebException ex)
                            {
                                arquivosRetornos += " " + ex.Message;
                                todasNotasEnviadas = false;

                                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                                {
                                    var resp = (HttpWebResponse)ex.Response;
                                    if (resp.StatusCode == HttpStatusCode.BadRequest)
                                    {
                                        using (StreamReader r = new StreamReader(ex.Response.GetResponseStream()))
                                        {
                                            msgErro += " " + r.ReadToEnd();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasCargas = repXMLNotaFiscal.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                
                foreach (var nota in notasCargas)
                {
                    if (nota.Emitente != null && nota.Destinatario != null && cargaIntegracao.Carga.Empresa != null && nota.QuantidadePallets > 0 && ((int)nota.QuantidadePallets > 0))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.Transferencia transferencia = new Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.Transferencia()
                        {
                            CnpjEmpresaOrigem = nota.Emitente?.CPF_CNPJ_SemFormato,
                            RazaoSocialOrigem = nota.Emitente?.Nome,
                            CnpjEmpresaDestino = nota.Destinatario?.CPF_CNPJ_SemFormato,
                            RazaoSocialDestino = nota.Destinatario?.Nome,
                            CnpjEmpresaTransportadora = cargaIntegracao.Carga.TipoOperacao?.CNPJTransportadoraLogiun,
                            RazaoSocialTransportadora = cargaIntegracao.Carga.Empresa?.RazaoSocial,
                            DataTransferencia = cargaIntegracao.Carga.DataCarregamentoCarga.HasValue ? cargaIntegracao.Carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-ddTHH:mm:sszzz") : "",
                            NumeroViagem = cargaIntegracao.Carga.CodigoCargaEmbarcador,
                            DocumentoMotorista = motorista?.CPF,
                            NomeMotorista = motorista?.Nome,
                            PlacaVeiculo = veiculo?.Placa,
                            ValorFrete = cargaIntegracao.Carga.ValorAReceberCTes.HasValue ? cargaIntegracao.Carga.ValorAReceberCTes.Value : (decimal)0,
                            Observacao = "",
                            Embalagens = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.Embalagem>()
                        };
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.Embalagem embalagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.Embalagem()
                        {
                            CodigoEmbalagem = produtoLogiun,
                            NumeroNotaFiscal = nota.Numero.ToString(),
                            SerieNotaFiscal = nota.Serie.ToString(),
                            Quantidade = (int)nota.QuantidadePallets,
                            QuantidadeAvaria = 0,
                            ValorUnitarioEmbalagem = (decimal)0
                        };
                        transferencia.Embalagens.Add(embalagem);
                        using (WebClient client = new WebClient())
                        {
                            string jsonPost = JsonConvert.SerializeObject(transferencia, Formatting.Indented);
                            arquivosEnvios = jsonPost;
                            client.Headers.Add("Authorization", "Bearer " + token);
                            client.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                            client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            try
                            {
                                var data = client.UploadString(urlWebService, jsonPost);

                                var responseString = (string)data;
                                arquivosRetornos = " " + responseString;

                                if (!string.IsNullOrWhiteSpace(responseString) && responseString.Contains("sucesso"))
                                {
                                    if (todasNotasEnviadas)
                                        todasNotasEnviadas = true;
                                }
                                else
                                {
                                    msgErro = " " + arquivosRetornos;
                                    todasNotasEnviadas = false;
                                }

                                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosEnvios, "json", unitOfWork);
                                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                                arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;

                                if (todasNotasEnviadas)
                                {
                                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                    cargaIntegracao.ProblemaIntegracao = "Sucesso";
                                    arquivoIntegracao.Mensagem = "Sucesso";
                                    cargaIntegracao.Protocolo = "";
                                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosRetornos, "json", unitOfWork);
                                }
                                else
                                {
                                    arquivosRetornos = " " + msgErro;
                                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                    cargaIntegracao.ProblemaIntegracao = "Falha ao processar. " + msgErro;
                                    arquivoIntegracao.Mensagem = "Falha ao processar. " + msgErro;
                                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosRetornos, "json", unitOfWork);
                                }

                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                                repCargaIntegracao.Atualizar(cargaIntegracao);
                            }
                            catch (WebException ex)
                            {
                                arquivosRetornos += " " + ex.Message;
                                todasNotasEnviadas = false;

                                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                                {
                                    var resp = (HttpWebResponse)ex.Response;
                                    if (resp.StatusCode == HttpStatusCode.BadRequest)
                                    {
                                        using (StreamReader r = new StreamReader(ex.Response.GetResponseStream()))
                                        {
                                            msgErro += " " + r.ReadToEnd();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!todasNotasEnviadas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosEnvios, "json", unitOfWork);
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                arquivosRetornos += " " + msgErro;

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Falha ao processar. " + msgErro;

                arquivoIntegracao.Mensagem = "Falha ao processar. " + msgErro;
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosRetornos, "json", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                repCargaIntegracao.Atualizar(cargaIntegracao);
            }
            else
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaIntegracao.ProblemaIntegracao = "Sucesso";
                repCargaIntegracao.Atualizar(cargaIntegracao);
            }
        }

        public static void CancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, Repositorio.UnitOfWork unitOfWork, string motivoCancelamento, DateTime dataCancelamento)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoAnterior = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo, cargaCancelamentoIntegracao.TipoIntegracao.Codigo);

            if (cargaIntegracaoAnterior == null || configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoLogiun || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioLogiun) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaLogiun) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoLogiun))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Logiun.";
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
                return;
            }

            cargaCancelamentoIntegracao.NumeroTentativas += 1;
            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;

            string msgRetorno = "";
            string token = "";
            if (!Realizarlogin(unitOfWork, cargaCancelamentoIntegracao.CargaCancelamento.Carga.Empresa.TipoAmbiente, out msgRetorno, out token))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Falha ao realizar login. " + msgRetorno;
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
                return;
            }

            string urlWebService = configuracaoIntegracao.URLHomologacaoLogiun;

            if (cargaCancelamentoIntegracao.CargaCancelamento.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                urlWebService = configuracaoIntegracao.URLProducaoLogiun;

            urlWebService += "/transferenciacancelar";
            string produtoLogiun = cargaCancelamentoIntegracao.CargaCancelamento.Carga.TipoOperacao?.ProdutoLogiun ?? "";

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasCargas = repXMLNotaFiscal.BuscarPorCarga(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo);
            bool todasNotasEnviadas = true;
            string msgErro = "";
            string arquivosEnvios = "";
            string arquivosRetornos = "";
            foreach (var nota in notasCargas)
            {
                if (nota.Emitente != null && nota.Destinatario != null && cargaCancelamentoIntegracao.CargaCancelamento.Carga.Empresa != null && nota.QuantidadePallets > 0 && ((int)nota.QuantidadePallets > 0))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.CancelamentoTransferencia cancelamentoTransferencia = new Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun.CancelamentoTransferencia()
                    {
                        CnpjEmpresaOrigem = nota.Emitente?.CPF_CNPJ_SemFormato,
                        CnpjEmpresaDestino = nota.Destinatario?.CPF_CNPJ_SemFormato,
                        CnpjEmpresaTransportadora = cargaCancelamentoIntegracao.CargaCancelamento.Carga.TipoOperacao?.CNPJTransportadoraLogiun,
                        DataTransferencia = cargaCancelamentoIntegracao.CargaCancelamento.Carga.DataCarregamentoCarga.HasValue ? cargaCancelamentoIntegracao.CargaCancelamento.Carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-ddTHH:mm:sszzz") : "",
                        NumeroViagem = cargaCancelamentoIntegracao.CargaCancelamento.Carga.CodigoCargaEmbarcador,
                        MotivoCancelamento = motivoCancelamento
                    };

                    using (WebClient client = new WebClient())
                    {
                        string jsonPost = JsonConvert.SerializeObject(cancelamentoTransferencia, Formatting.Indented);
                        arquivosEnvios += " " + jsonPost;
                        client.Headers.Add("Authorization", "Bearer " + token);
                        client.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        try
                        {
                            var data = client.UploadString(urlWebService, jsonPost);

                            var responseString = (string)data;
                            arquivosRetornos += " " + responseString;

                            if (!string.IsNullOrWhiteSpace(responseString) && responseString.Contains("sucesso"))
                            {
                                if (todasNotasEnviadas)
                                    todasNotasEnviadas = true;
                            }
                            else
                            {
                                msgErro += " " + arquivosRetornos;
                                todasNotasEnviadas = false;
                            }
                        }
                        catch (WebException ex)
                        {
                            arquivosRetornos += " " + ex.Message;
                            todasNotasEnviadas = false;

                            if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                            {
                                var resp = (HttpWebResponse)ex.Response;
                                if (resp.StatusCode == HttpStatusCode.BadRequest)
                                {
                                    using (StreamReader r = new StreamReader(ex.Response.GetResponseStream()))
                                    {
                                        msgErro += " " + r.ReadToEnd();
                                    }
                                }
                            }
                        }
                    }
                }
            }


            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosEnvios, "json", unitOfWork);
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            arquivoIntegracao.Data = cargaCancelamentoIntegracao.DataIntegracao;

            if (todasNotasEnviadas)
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Sucesso";
                arquivoIntegracao.Mensagem = "Sucesso";
                cargaCancelamentoIntegracao.Protocolo = "";
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosRetornos, "json", unitOfWork);
            }
            else
            {
                arquivosRetornos += " " + msgErro;
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Falha ao processar. " + msgErro;
                arquivoIntegracao.Mensagem = "Falha ao processar. " + msgErro;
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosRetornos, "json", unitOfWork);
            }

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaCancelamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
        }
        public class LoggingHandler : DelegatingHandler
        {
            public LoggingHandler(HttpMessageHandler innerHandler)
                : base(innerHandler)
            {
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Servicos.Log.TratarErro("Request: " + request.ToString(), "IntegracaoLogin");

                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                Servicos.Log.TratarErro("Response: " + response.ToString(), "IntegracaoLogin");

                return response;
            }
        }
    }
}
