using Infrastructure.Services.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace Servicos.Embarcador.Integracao.GoldenService
{
    public class IntegracaoGoldenService
    {
        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);


            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoGoldenService || string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigoGoldenService) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaGoldenService) || string.IsNullOrWhiteSpace(configuracaoIntegracao.IdGoldenService) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoGoldenService))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Golden Service.";
            }

            string urlWebService = configuracaoIntegracao.URLHomologacaoGoldenService;

            if (cargaIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                urlWebService = configuracaoIntegracao.URLProducaoGoldenService;

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaIntegracao.Carga.Pedidos.FirstOrDefault();
            Dominio.Entidades.Usuario condutorPrincipal = cargaIntegracao.Carga.Motoristas.FirstOrDefault();
            Dominio.Entidades.Usuario condutorAuxiliar = cargaIntegracao.Carga.Motoristas.Count > 1 ? cargaIntegracao.Carga.Motoristas[1] : null;
            Dominio.Entidades.Usuario ajudante = cargaIntegracao.Carga.Ajudantes.FirstOrDefault();

            decimal valorTotalNotas = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(cargaIntegracao.Carga.Codigo);
            string numeroNotas = string.Join(" / ", repPedidoXMLNotaFiscal.ObterNumerosNotasPorCarga(cargaIntegracao.Carga.Codigo));

            Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Request solicitacaoMonitoramento = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Request()
            {
                Login = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.Login()
                {
                    Codigo = configuracaoIntegracao.CodigoGoldenService,
                    Id = configuracaoIntegracao.IdGoldenService,
                    Senha = configuracaoIntegracao.SenhaGoldenService
                },
                Carga = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Carga()
                {
                    TipoOperacao = cargaIntegracao.Carga.TipoOperacao?.CodigoIntegracaoGoldenService ?? string.Empty,
                    ValorMercadoria = valorTotalNotas.ToString("F2", new System.Globalization.CultureInfo("en-US")),
                    Descricao = "Notas: " + numeroNotas
                },
                Destino = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Localidade()
                {
                    Cidade = cargaPedido.Origem?.Descricao ?? string.Empty,
                    UF = cargaPedido.Origem?.Estado?.Sigla ?? string.Empty
                },
                Origem = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Localidade()
                {
                    Cidade = cargaPedido.Destino?.Descricao ?? string.Empty,
                    UF = cargaPedido.Destino?.Estado.Sigla ?? string.Empty
                },
                Embarcador = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Embarcador(cargaPedido.Pedido.Remetente),
                Servico = "01",
                Transportador = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Transportador(cargaIntegracao.Carga.Empresa),
                Viagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Viagem()
                {
                    DataInicio = cargaPedido.Pedido.DataPrevisaoSaida?.ToString("ddMMyyyy"),
                    HoraInicio = cargaPedido.Pedido.DataPrevisaoSaida?.ToString("HHmm"),
                    DataFim = cargaPedido.Pedido.PrevisaoEntrega?.ToString("ddMMyyyy"),
                    HoraFim = cargaPedido.Pedido.PrevisaoEntrega?.ToString("HHmm")
                }
            };

            if (ajudante != null)
                solicitacaoMonitoramento.Ajudante = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Ajudante(ajudante);

            if (condutorPrincipal != null)
                solicitacaoMonitoramento.Condutor1 = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Condutor(condutorPrincipal);

            if (condutorAuxiliar != null)
                solicitacaoMonitoramento.Condutor2 = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Condutor(condutorAuxiliar);

            if (cargaIntegracao.Carga.Veiculo != null)
                solicitacaoMonitoramento.Veiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Veiculo(cargaIntegracao.Carga.Veiculo, cargaIntegracao.Carga.Empresa);

            if (cargaIntegracao.Carga.VeiculosVinculados.Count > 0)
            {
                Dominio.Entidades.Veiculo carreta1 = cargaIntegracao.Carga.VeiculosVinculados.FirstOrDefault();

                solicitacaoMonitoramento.Carreta1 = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Veiculo(carreta1, cargaIntegracao.Carga.Empresa);

                if (cargaIntegracao.Carga.VeiculosVinculados.Count > 1)
                {
                    Dominio.Entidades.Veiculo carreta2 = cargaIntegracao.Carga.VeiculosVinculados.LastOrDefault();
                    solicitacaoMonitoramento.Carreta2 = new Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento.Veiculo(carreta2, cargaIntegracao.Carga.Empresa);
                }
            }

            string mensagemRetorno = string.Empty;
            string responseData = string.Empty;

            string requestData = (new StringBuilder())
                   .Append("<?xml version='1.0' encoding='iso-8859-1'?>")
                   .Append(Utilidades.XML.Serializar(solicitacaoMonitoramento).Replace("<Request>", "").Replace("</Request>", ""))
                   .ToString();

            try
            {
                HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGoldenService));
                StringContent conteudoRequisicao = new StringContent(requestData, Encoding.UTF8, "application/xml");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlWebService, conteudoRequisicao).Result;

                if (retornoRequisicao.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseData = retornoRequisicao.Content.ReadAsStringAsync().Result;

                    XDocument retorno = XDocument.Parse(responseData);

                    IEnumerable<XElement> retornoOK = retorno.Descendants("OK");
                    IEnumerable<XElement> retornoErro = retorno.Descendants("ERRO");

                    if (retornoOK.Any())
                    {
                        string codigoSM = retornoOK.Descendants("SM").First().Value;

                        IEnumerable<XElement> retornoMSG1 = retornoOK.Descendants("MSG1");

                        if (retornoMSG1.Any())
                        {
                            mensagemRetorno = retornoMSG1.FirstOrDefault().Value;

                            IEnumerable<XElement> retornoMSG2 = retornoOK.Descendants("MSG2");

                            if (retornoMSG2.Any())
                                mensagemRetorno += " / " + retornoMSG2.FirstOrDefault().Value;
                        }

                        cargaIntegracao.Protocolo = codigoSM;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = mensagemRetorno + " / SM: " + codigoSM;
                    }
                    else
                    {
                        if (retornoErro.Any())
                            mensagemRetorno = retornoErro.FirstOrDefault().Value;

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = mensagemRetorno;
                    }
                }
                else
                {
                    cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service do Golden Service.";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service do Golden Service.";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(requestData, "xml", unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(responseData, "xml", unitOfWork),
                Data = cargaIntegracao.DataIntegracao,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Mensagem = mensagemRetorno
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }
    }
}
