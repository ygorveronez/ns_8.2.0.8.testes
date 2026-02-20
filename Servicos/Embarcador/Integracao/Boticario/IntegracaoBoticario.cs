using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Boticario
{
    public class IntegracaoBoticario
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private Dominio.Entidades.Embarcador.Configuracoes.Integracao _configuracaoIntegracao;

        #endregion

        #region Construtores

        public IntegracaoBoticario(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.Integracao ObterConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao != null) return _configuracaoIntegracao;

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (!(configuracaoIntegracao?.PossuiIntegracaoBoticario ?? false) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoBoticario))
                throw new ServicoException("Não existe configuração de integração disponível para a Boticário.");

            _configuracaoIntegracao = configuracaoIntegracao;

            return configuracaoIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoCTe ObterBaseIntegracaoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCte.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCte.CTe;

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao contaContabil = repCargaPedidoContaContabilContabilizacao.BuscarFirstOrDefaultPorCargaPedido(cargaPedido.Codigo);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoCTe()
            {
                ChaveCTe = cte.Chave,
                Numero = cte.Numero.ToString(),
                Protocolo = cte.Codigo,
                TipoProcesso = carga.TipoOperacao != null ? carga.TipoOperacao.RemessaSAP.ToString().PadLeft(2, '0') : string.Empty,
                CentroCusto = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.CentroCusto()
                {
                    Codigo = cargaPedido?.CentroResultado?.PlanoContabilidade,
                    NumeroContaRazao = contaContabil?.PlanoConta?.PlanoContabilidade,
                },
                PEP = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.PEP()
                {
                    Codigo = cargaPedido?.ElementoPEP ?? string.Empty,
                },
                Transporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.Transporte()
                {
                    CustoPedagio = cargaCte.Componentes.Where(c => c.TipoComponenteFrete == TipoComponenteFrete.PEDAGIO).Sum(c => c.ValorComponente),
                    ValorFrete = cte.ValorFrete
                }
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoCTe ObterDadosIntegracaoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoCTe dados = ObterBaseIntegracaoCTe(cargaCte);

            dados.Triangulacoes = (from nota in cargaCte.NotasFiscais
                                   where nota.PedidoXMLNotaFiscal != null && nota.PedidoXMLNotaFiscal.XMLNotaFiscal != null
                                   select new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.Triangulacao
                                   {
                                       ChaveRemessa = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave,
                                       ChaveVenda = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.ChaveVenda,
                                   }).ToList();

            return dados;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoCTe ObterDadosIntegracaoCancelamentoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoCTe dados = ObterBaseIntegracaoCTe(cargaCte);

            dados.TipoProcesso = "99";

            return dados;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoComplementoCTe ObterBaseIntegracaoComplementoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteComlemento, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteComplementado)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCteComlemento.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplemento = cargaCteComlemento.CTe;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = cargaCteComplementado.CTe;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoComplementoCTe()
            {
                ChaveCTe = cteComplemento.Chave,
                ChaveCTeComplementado = cteComplementado.Chave,
                Numero = cteComplemento.Numero.ToString(),
                Protocolo = cteComplemento.Codigo,
                TipoProcesso = carga.TipoOperacao != null ? carga.TipoOperacao.RemessaSAP.ToString().PadLeft(2, '0') : string.Empty,
                Transporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.Transporte()
                {
                    CustoPedagio = cargaCteComlemento.Componentes.Where(c => c.TipoComponenteFrete == TipoComponenteFrete.PEDAGIO).Sum(c => c.ValorComponente),
                    ValorFrete = cteComplemento.ValorAReceber
                }
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoComplementoCTe ObterDadosIntegracaoComplementoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteComlemento, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteComplementado)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoComplementoCTe dados = ObterBaseIntegracaoComplementoCTe(cargaCteComlemento, cargaCteComplementado);

            return dados;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoComplementoCTe ObterDadosIntegracaoCancelamentoComplementoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteComlemento, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteComplementado)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoComplementoCTe dados = ObterBaseIntegracaoComplementoCTe(cargaCteComlemento, cargaCteComplementado);

            dados.TipoProcesso = "99";

            return dados;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.IntegracaoSequenciaZonaTransporte ObterDadosIntegracaoSequenciaZonaTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool incluirChaveNF)
        {
            Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault() ?? null;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.IntegracaoSequenciaZonaTransporte dados = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.IntegracaoSequenciaZonaTransporte()
            {
                NumeroCargaConsolidada = carga.CodigoCargaEmbarcador,
                Transportadora = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.Transportadora()
                {
                    CodigoIntegracao = carga.Empresa.CodigoIntegracao ?? "",
                    Cnpj = carga.Empresa.CNPJ ?? ""
                },
                Coleta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.Coleta()
                {
                    Data = carga.DataCarregamentoCarga?.ToString("yyyy-MM-dd"),
                    Hora = carga.DataCarregamentoCarga?.ToString("HH:mm:ss")
                },
                Motorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.Motorista()
                {
                    Cpf = motorista?.CPF,
                    Nome = motorista?.Nome
                },
                Caminhao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.Caminhao()
                {
                    Placa = carga.Veiculo?.Placa
                }
            };

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            int count = 0;
            dados.Cargas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.Carga>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                count++;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.Carga objetoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.Carga()
                {
                    Ordem = count.ToString().PadLeft(3, '0'),
                    NumeroCarga = carga.CodigoCargaEmbarcador,
                    NumeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    ProtocoloCarga = carga.Protocolo.ToString(),
                    ProtocoloPedido = cargaPedido.Pedido.Protocolo.ToString()
                };
                dados.Cargas.Add(objetoCarga);
            }
            if (incluirChaveNF)
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
                List<string> chavesNF = repXMLNotaFiscal.BuscarPorChavesDasNotasNaCarga(carga.Codigo);
                if (chavesNF != null && chavesNF.Count > 0)
                {
                    dados.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.NotasFiscais>();
                    foreach (string chaveNF in chavesNF)
                    {
                        var notafiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.NotasFiscais();
                        notafiscal.ChaveNFe = chaveNF;
                        dados.NotasFiscais.Add(notafiscal);
                    }
                }
            }

            return dados;
        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao ocorrenciaCTeCancelamentoIntegracao, string jsonDadosRequisicao, string jsonDadosRetornoRequisicao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo repOcorrenciaCTeCancelamentoIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRetornoRequisicao, "json", _unitOfWork),
                Data = ocorrenciaCTeCancelamentoIntegracao.DataIntegracao,
                Mensagem = ocorrenciaCTeCancelamentoIntegracao.ProblemaIntegracao,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
            };

            repOcorrenciaCTeCancelamentoIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (ocorrenciaCTeCancelamentoIntegracao.ArquivosTransacao == null)
                ocorrenciaCTeCancelamentoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo>();

            ocorrenciaCTeCancelamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }


        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, string mensagem, string jsonDadosRequisicao, string jsonDadosRetornoRequisicao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRetornoRequisicao, "json", _unitOfWork),
                Data = cargaCargaIntegracao.DataIntegracao,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaCargaIntegracao.ArquivosTransacao == null)
                cargaCargaIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }



        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, string jsonDadosRequisicao, string jsonDadosRetornoRequisicao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repositorioOcorrenciaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRetornoRequisicao, "json", _unitOfWork),
                Data = ocorrenciaCTeIntegracao.DataIntegracao,
                Mensagem = ocorrenciaCTeIntegracao.ProblemaIntegracao,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
            };

            repositorioOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (ocorrenciaCTeIntegracao.ArquivosTransacao == null)
                ocorrenciaCTeIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>();

            ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao, string jsonDadosRequisicao, string jsonDadosRetornoRequisicao, string mensagemRetorno)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRetornoRequisicao, "json", _unitOfWork),
                Data = cargaCTeIntegracao.DataIntegracao,
                Mensagem = string.IsNullOrWhiteSpace(mensagemRetorno) ? "Integrado com sucesso" : mensagemRetorno,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaCTeIntegracao.ArquivosTransacao == null)
                cargaCTeIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCargaCTeIntegraca, string jsonDadosRequisicao, string jsonDadosRetornoRequisicao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRetornoRequisicao, "json", _unitOfWork),
                Data = cargaCancelamentoCargaCTeIntegraca.DataIntegracao,
                Mensagem = cargaCancelamentoCargaCTeIntegraca.ProblemaIntegracao,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaCancelamentoCargaCTeIntegraca.ArquivosTransacao == null)
                cargaCancelamentoCargaCTeIntegraca.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaCancelamentoCargaCTeIntegraca.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, string jsonDadosRequisicao, string jsonDadosRetornoRequisicao, string mensagem)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonDadosRetornoRequisicao, "json", _unitOfWork),
                Data = cargaDadosTransporteIntegracao.DataIntegracao,
                Mensagem = !string.IsNullOrWhiteSpace(mensagem) ? mensagem : cargaDadosTransporteIntegracao.ProblemaIntegracao,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaDadosTransporteIntegracao.ArquivosTransacao == null)
                cargaDadosTransporteIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private HttpResponseMessage CriaRequisicao(string url, string body, List<(string Nome, string Valor)> headers = null)
        {

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

            var httpClient = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBoticario));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            httpClient.DefaultRequestHeaders.Add("X-IBM-Client-Id", configuracaoIntegracao.IntegracaoBoticarioClientId);
            httpClient.DefaultRequestHeaders.Add("X-IBM-Client-secret", configuracaoIntegracao.IntegracaoBoticarioClientSecret);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Nome, header.Valor);
                }
            }
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            return httpClient.PostAsync(configuracaoIntegracao.URLIntegracaoBoticario, content).Result;
        }

        private Dictionary<string, string> ObterJsonRetorno(string response)
        {
            Dictionary<string, string> dadosRetornoRequisicao = new Dictionary<string, string>();

            try
            {
                dadosRetornoRequisicao = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }

            return dadosRetornoRequisicao;
        }

        private bool IsRetornoSucesso(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.Created;
        }
        private bool IsRetornoSucesso(int statusCode)
        {
            return (statusCode == 200 || statusCode == 201);
        }

        private string ObterMensagemDeErro(string corpoResposta, HttpStatusCode statusCode)
        {
            try
            {
                var erroResponse = JsonConvert.DeserializeObject<dynamic>(corpoResposta);
                string faultstring = erroResponse.fault?.faultstring ?? string.Empty;
                string errorcode = erroResponse.fault?.detail?.errorcode ?? string.Empty;
                if (string.IsNullOrEmpty(errorcode))
                {
                    Dictionary<string, string> dadosRetornoRequisicao = ObterJsonRetorno(corpoResposta);
                    if (dadosRetornoRequisicao.ContainsKey("moreInformation"))
                        return $"{dadosRetornoRequisicao["moreInformation"]}. HTTP Status {(int)statusCode}";
                    else if (dadosRetornoRequisicao.ContainsKey("mensagem"))
                        return $"{dadosRetornoRequisicao["mensagem"]}. HTTP Status {(int)statusCode}";
                    else if (dadosRetornoRequisicao.ContainsKey("HttpMensagem"))
                        return $"{dadosRetornoRequisicao["HttpMensagem"]}. HTTP Status {(int)statusCode}";
                    else
                        return $"Ocorreu uma falha ao realizar a integração com a Boticário. HTTP Status {(int)statusCode}";
                }
                return $"{faultstring} - {errorcode}. HTTP Status {(int)statusCode} ";
            }
            catch (Exception)
            {
                return $"Ocorreu uma falha ao realizar a integração com a Boticário. HTTP Status {(int)statusCode}";
            }
        }

        private void IntegrarComplemento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoComplementoCTe dadosRequisicao = ObterDadosIntegracaoComplementoCTe(integracao.CargaCTe, integracao.CargaCTe.CargaCTeComplementoInfo.CargaCTeComplementado);

            string jsonDadosRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            var headers = new List<(string Nome, string Valor)>() {
                ValueTuple.Create("unidadeNegocio", UnidadeNegocioBoticario.GrupoBoticario.ObterSigla())
            };

            using (HttpResponseMessage resposta = CriaRequisicao(configuracaoIntegracao.URLIntegracaoBoticario, jsonDadosRequisicao, headers))
            {
                string jsonDadosRetornoRequisicao = resposta.Content.ReadAsStringAsync().Result;

                string mensagemErroIntegracao = string.Empty;

                if (!IsRetornoSucesso(resposta.StatusCode))
                    mensagemErroIntegracao = ObterMensagemDeErro(jsonDadosRetornoRequisicao, resposta.StatusCode);

                AdicionarArquivoTransacao(integracao, jsonDadosRequisicao, jsonDadosRetornoRequisicao);

                if (!string.IsNullOrWhiteSpace(mensagemErroIntegracao))
                    throw new ServicoException(mensagemErroIntegracao);
            }
        }

        private void IntegrarCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoCTe dadosRequisicao = ObterDadosIntegracaoCTe(cargaCTeIntegracao.CargaCTe);

            string jsonDadosRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            var headers = new List<(string Nome, string Valor)>()
            {
                ValueTuple.Create("unidadeNegocio", UnidadeNegocioBoticario.GrupoBoticario.ObterSigla())
            };

            using (HttpResponseMessage resposta = CriaRequisicao(configuracaoIntegracao.URLIntegracaoBoticario, jsonDadosRequisicao, headers))
            {

                string jsonDadosRetornoRequisicao = resposta.Content.ReadAsStringAsync().Result;

                string mensagemErroIntegracao = string.Empty;
                HttpStatusCode statusCode = resposta.StatusCode;

                if (!IsRetornoSucesso(statusCode))
                {
                    mensagemErroIntegracao = ObterMensagemDeErro(jsonDadosRetornoRequisicao, statusCode);
                }

                AdicionarArquivoTransacao(cargaCTeIntegracao, jsonDadosRequisicao, jsonDadosRetornoRequisicao, mensagemErroIntegracao);

                if (!string.IsNullOrWhiteSpace(mensagemErroIntegracao))
                    throw new ServicoException(mensagemErroIntegracao);
            }
        }

        private void IntegrarCancelamentoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCTeCancelamentoIntegracao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoCTe dadosRequisicao = ObterDadosIntegracaoCancelamentoCTe(cargaCTeCancelamentoIntegracao.CargaCTe);

            string jsonDadosRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            var headers = new List<(string Nome, string Valor)>() {
                ValueTuple.Create("unidadeNegocio", UnidadeNegocioBoticario.GrupoBoticario.ObterSigla())
            };

            using (HttpResponseMessage resposta = CriaRequisicao(configuracaoIntegracao.URLIntegracaoBoticario, jsonDadosRequisicao, headers))
            {
                string jsonDadosRetornoRequisicao = resposta.Content.ReadAsStringAsync().Result;

                string mensagemErroIntegracao = string.Empty;

                if (!IsRetornoSucesso(resposta.StatusCode))
                    mensagemErroIntegracao = ObterMensagemDeErro(jsonDadosRetornoRequisicao, resposta.StatusCode);

                AdicionarArquivoTransacao(cargaCTeCancelamentoIntegracao, jsonDadosRequisicao, jsonDadosRetornoRequisicao);

                if (!string.IsNullOrWhiteSpace(mensagemErroIntegracao))
                    throw new ServicoException(mensagemErroIntegracao);
            }
        }

        private void IntegrarCancelamentoComplemento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao integracao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.IntegracaoComplementoCTe dadosRequisicao = ObterDadosIntegracaoCancelamentoComplementoCTe(integracao.OcorrenciaCTeIntegracao.CargaCTe, integracao.OcorrenciaCTeIntegracao.CargaCTe.CargaCTeComplementoInfo.CargaCTeComplementado);

            string jsonDadosRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            var headers = new List<(string Nome, string Valor)>() {
                ValueTuple.Create("unidadeNegocio", UnidadeNegocioBoticario.GrupoBoticario.ObterSigla())
            };

            using (HttpResponseMessage resposta = CriaRequisicao(configuracaoIntegracao.URLIntegracaoBoticario, jsonDadosRequisicao, headers))
            {
                string jsonDadosRetornoRequisicao = resposta.Content.ReadAsStringAsync().Result;

                string mensagemErroIntegracao = string.Empty;

                if (!IsRetornoSucesso(resposta.StatusCode))
                    mensagemErroIntegracao = ObterMensagemDeErro(jsonDadosRetornoRequisicao, resposta.StatusCode);

                AdicionarArquivoTransacao(integracao, jsonDadosRequisicao, jsonDadosRetornoRequisicao);

                if (!string.IsNullOrWhiteSpace(mensagemErroIntegracao))
                    throw new ServicoException(mensagemErroIntegracao);
            }
        }

        private void IntegrarSequenciaZonaTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, out string jsonRequisicao, out string jsonRetorno, out string mensagemIntegracao, bool incluirChaveNF = false)
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

            string authToken = ObterToken(configuracaoIntegracao);
            if (string.IsNullOrWhiteSpace(authToken))
                throw new ServicoException("Não retornou o token de integração.");

            if (carga.Empresa == null)
                throw new ServicoException("Transportadora não informada na carga.");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte.IntegracaoSequenciaZonaTransporte dadosRequisicao = ObterDadosIntegracaoSequenciaZonaTransporte(carga, incluirChaveNF);

            jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            HttpClient requisicao = CriarRequisicao(configuracaoIntegracao.URLEnvioSequenciaBoticario, authToken);

            StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLEnvioSequenciaBoticario, conteudoRequisicao).Result;
            jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
            mensagemIntegracao = string.Empty;

            if (!IsRetornoSucesso(retornoRequisicao.StatusCode) || !IsRetornoSucesso((int)obj.statusCode)) mensagemIntegracao = $"{obj.statusCode} - {obj.message}";

            if (!string.IsNullOrWhiteSpace(mensagemIntegracao))
                throw new ServicoException(mensagemIntegracao);
        }

        private HttpClient CriarRequisicao(string url, string accessToken = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBoticario));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(accessToken))
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return requisicao;
        }

        private string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLGerarTokenBoticario))
                return null;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBoticario));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", configuracaoIntegracao.IntegracaoBoticarioClientId },
                { "client_secret", configuracaoIntegracao.IntegracaoBoticarioClientSecret },
                { "grant_type", "client_credentials" }
            });

            HttpResponseMessage result = client.PostAsync(configuracaoIntegracao.URLGerarTokenBoticario, content).Result;
            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            if (!result.IsSuccessStatusCode)
                return null;

            dynamic obj = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            return (string)obj.access_token;
        }

        private string ObterToken(Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestSharp.RestClient(configuracaoIntegracao.UrlAutenticacao);
            client.Timeout = -1;
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", configuracaoIntegracao.ClienteId);
            request.AddParameter("client_secret", configuracaoIntegracao.ClientSecret);
            request.AddParameter("grant_type", "client_credentials");

            RestSharp.IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);

            if (string.IsNullOrWhiteSpace(retorno.access_token))
                throw new ServicoException("Não foi possível obter o Token");

            return retorno.access_token;
        }

        private HttpClient ObterHttpClient(Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            string token = ObterToken(configuracaoIntegracao);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBoticario));

            client.BaseAddress = new Uri(configuracaoIntegracao.UrlRequisicao);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.ConfiguracaoIntegracao ObterConfiguracaoAVIPED()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow repIntegracaoBoticarioFreeFlow = new Repositorio.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow configIntegracaoBoticarioFreeFlow = repIntegracaoBoticarioFreeFlow.Buscar();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.ConfiguracaoIntegracao()
            {
                UrlRequisicao = configIntegracaoBoticarioFreeFlow.URLConsultaAVIPED,
                UrlAutenticacao = configIntegracaoBoticarioFreeFlow.URLAutenticacao,
                ClienteId = configIntegracaoBoticarioFreeFlow.ClientId,
                ClientSecret = configIntegracaoBoticarioFreeFlow.ClientSecret
            };
        }

        private string XmlToJson(string xml)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);

            return JsonConvert.SerializeXmlNode(doc, Formatting.Indented);
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCTeComplemento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);

            integracao.NumeroTentativas += 1;
            integracao.DataIntegracao = DateTime.Now;

            try
            {
                IntegrarComplemento(integracao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Boticário";
            }
            finally
            {
                repositorioOcorrenciaCTeIntegracao.Atualizar(integracao);
            }
        }

        public void IntegrarCargaCTe(ref Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            cargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeIntegracao.NumeroTentativas++;

            try
            {
                IntegrarCTe(cargaCTeIntegracao);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCTeIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Boticário";
            }
        }

        public void IntegrarCancelamentoCargaCTe(ref Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCTeCancelamentoIntegracao)
        {
            cargaCTeCancelamentoIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeCancelamentoIntegracao.NumeroTentativas++;

            try
            {
                IntegrarCancelamentoCTe(cargaCTeCancelamentoIntegracao);

                cargaCTeCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCTeCancelamentoIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                cargaCTeCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeCancelamentoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Boticário";
            }
        }

        public void IntegrarCancelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao integracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repOcorrenciaCTeCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(_unitOfWork);

            integracao.NumeroTentativas += 1;
            integracao.DataIntegracao = DateTime.Now;

            try
            {
                IntegrarCancelamentoComplemento(integracao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Boticário";
            }
            finally
            {
                repOcorrenciaCTeCancelamentoIntegracao.Atualizar(integracao);
            }
        }

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            string jsonRequisicao = "";
            string jsonRetorno = "";
            string mensagemIntegracao = "";
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas += 1;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                IntegrarSequenciaZonaTransporte(cargaDadosTransporteIntegracao.Carga, out jsonRequisicao, out jsonRetorno, out mensagemIntegracao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integrado com sucesso";
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
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Boticário";
            }
            AdicionarArquivoTransacao(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, mensagemIntegracao);
            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            string jsonRequisicao = "";
            string jsonRetorno = "";
            string mensagemIntegracao = "";
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                IntegrarSequenciaZonaTransporte(cargaIntegracao.Carga, out jsonRequisicao, out jsonRetorno, out mensagemIntegracao, true);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
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
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Boticário";
            }

            AdicionarArquivoTransacao(cargaIntegracao, mensagemIntegracao, jsonRequisicao, jsonRetorno);

            repCargaCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarAVIPED(Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED aviped)
        {
            aviped.NumeroAvisoRecebimento = "";
            aviped.NumeroPedidoCompra = "";
            aviped.Mensagem = "";

            string xmlSOAP =
               $@"  <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:rfc:functions"">
                       <soapenv:Header/>
                       <soapenv:Body>
                            <urn:ZFMMM_BUSCA_AVIPED_FREEFLOW>
                                <IV_CHAVE_ACESSO>{aviped.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave}</IV_CHAVE_ACESSO>
                                <IV_PROT_PEDIDO>{aviped.CargaPedido.Pedido.Protocolo}</IV_PROT_PEDIDO>
                            </urn:ZFMMM_BUSCA_AVIPED_FREEFLOW>
                        </soapenv:Body>
                    </soapenv:Envelope>";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Senior.ConfiguracaoIntegracao configIntegracaoAVIPED = ObterConfiguracaoAVIPED();

            HttpClient cliente = ObterHttpClient(configIntegracaoAVIPED);
            HttpContent httpContent = new StringContent(xmlSOAP, Encoding.UTF8, "text/xml");

            HttpResponseMessage retornoRequisicao = cliente.PostAsync(configIntegracaoAVIPED.UrlRequisicao, httpContent).Result;
            var resultado = retornoRequisicao.Content.ReadAsStringAsync().Result;
            var resultadoJson = XmlToJson(resultado);

            try
            {
                dynamic dynResultado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resultadoJson, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                string mensagem = dynResultado["soap-env:Envelope"]["soap-env:Body"]["n0:ZFMMM_BUSCA_AVIPED_FREEFLOWResponse"]["EV_MENSAGEM"].Value;

                if (string.IsNullOrEmpty(mensagem))
                {
                    aviped.NumeroPedidoCompra = dynResultado["soap-env:Envelope"]["soap-env:Body"]["n0:ZFMMM_BUSCA_AVIPED_FREEFLOWResponse"]["EV_EBELN"].Value;
                    aviped.NumeroAvisoRecebimento = dynResultado["soap-env:Envelope"]["soap-env:Body"]["n0:ZFMMM_BUSCA_AVIPED_FREEFLOWResponse"]["EV_VBELN"].Value;
                }
                else
                    aviped.Mensagem = mensagem;
            }
            catch
            {
                aviped.Mensagem = "Erro ao integrar.";
            }
        }

        #endregion
    }
}
