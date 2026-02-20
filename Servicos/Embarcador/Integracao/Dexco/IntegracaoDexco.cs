using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Dexco;
using Infrastructure.Services.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Dexco
{
    public class IntegracaoDexco
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDexco _configuracaoIntegracao;

        #endregion Atributos

        #region Construtores

        public IntegracaoDexco(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _configuracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoDexco(_unitOfWork).BuscarIntegracao();
        }

        #endregion Construtores

        #region Metodos Publicos

        public void IntegrarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            carregamentoIntegracao.NumeroTentativas += 1;
            carregamentoIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                if (_configuracaoIntegracao == null)
                    throw new ServicoException("Configuração com a Dexco não encontrada.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorCarregamento(carregamentoIntegracao.Carregamento.Codigo);

                if (cargas.Count == 0)
                    throw new ServicoException("Nenhuma carga encontrada para o carregamento.");

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioCarregamentoPedido.BuscarPorCarregamento(carregamentoIntegracao.Carregamento.Codigo).Select(obj => obj.Pedido).ToList();
                Dominio.Entidades.Embarcador.Cargas.Carga cargaAtualizar = cargas.Where(carga => carga.CargaAgrupada).FirstOrDefault() ?? cargas.FirstOrDefault();

                xmlRequisicao = Utilidades.XML.Serializar(ObterCreateFO(cargaAtualizar, pedidos, true));

                HttpClient client = ObterClienteRequisicao(_configuracaoIntegracao.UrlDexco, _configuracaoIntegracao.Usuario, _configuracaoIntegracao.Senha);
                StringContent dadosEnviar = new StringContent(xmlRequisicao, Encoding.UTF8, "application/xml");
                HttpResponseMessage retornoRequisicao = client.PostAsync(_configuracaoIntegracao.UrlDexco, dadosEnviar).Result;

                xmlRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                ReturnMessage retornoIntegracao = Utilidades.XML.Deserializar<ReturnMessage>(xmlRetorno);
                ReturnCreateFO pedidoNaoProcessado = retornoIntegracao.ReturnCreateFO.Where(nfe => nfe.Status == "E" || nfe.Status == "W").FirstOrDefault();

                if (pedidoNaoProcessado != null)
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    carregamentoIntegracao.ProblemaIntegracao = pedidoNaoProcessado.Message;
                }
                else
                {
                    string numeroCarga = retornoIntegracao?.ReturnCreateFO.Where(r => r.AccessKey != "9999").FirstOrDefault()?.FONumber;

                    if (string.IsNullOrWhiteSpace(numeroCarga))
                        throw new ServicoException("O número da carga não foi encontrado no retorno da Dexco");

                    cargaAtualizar.CodigoCargaEmbarcador = numeroCarga;

                    repositorioCarga.Atualizar(cargaAtualizar);
                    Auditoria.Auditoria.AuditarSemDadosUsuario(cargaAtualizar, "Número da carga alterado via integração com a Dexco", _unitOfWork);

                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    carregamentoIntegracao.ProblemaIntegracao = string.Empty;
                }

                servicoArquivoTransacao.Adicionar(carregamentoIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }
            catch (ServicoException excecao)
            {
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Dexco";

                servicoArquivoTransacao.Adicionar(carregamentoIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoPendente)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaIntegracaoPendente.NumeroTentativas++;
            cargaIntegracaoPendente.DataIntegracao = DateTime.Now;
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            try
            {
                if (_configuracaoIntegracao == null)
                    throw new ServicoException("Configuração com a Dexco não encontrada.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracaoPendente.Carga;

                if (carga == null)
                    throw new ServicoException("Carga não encontrada.");

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = carga.Pedidos.Select(obj => obj.Pedido).ToList();

                xmlRequisicao = Utilidades.XML.Serializar(ObterCreateFO(carga, pedidos, false));

                HttpClient client = ObterClienteRequisicao(_configuracaoIntegracao.UrlDexco, _configuracaoIntegracao.Usuario, _configuracaoIntegracao.Senha);
                StringContent dadosEnviar = new StringContent(xmlRequisicao, Encoding.UTF8, "application/xml");
                HttpResponseMessage retornoRequisicao = client.PostAsync(_configuracaoIntegracao.UrlDexco, dadosEnviar).Result;

                xmlRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicao.IsSuccessStatusCode)
                {
                    cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Dexco";
                }
                else
                {
                    cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracaoPendente.ProblemaIntegracao = "Integrado com Sucesso";
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, xmlRequisicao, xmlRetorno, "xml");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Dexco";

                servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, xmlRequisicao, xmlRetorno, "xml");
            }

            repCargaCargaIntegracao.Atualizar(cargaIntegracaoPendente);
        }

        #endregion

        #region Metodos Privados

        private static HttpClient ObterClienteRequisicao(string url, string usuario, string senha)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDexco));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.SetBasicAuthentication(usuario, senha);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            return client;
        }

        private CreateFO ObterCreateFO(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, bool carregamento)
        {
            CreateFO createFO = new CreateFO()
            {
                AccessKey = _configuracaoIntegracao.AccessKeyDexco,
                Fotype = _configuracaoIntegracao.FoType
            };

            createFO.ProtocoloCarga = carga.Protocolo;
            createFO.NumeroCarga = carregamento ? "" : carga.CodigoCargaEmbarcador;
            createFO.DataAgendamento = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
            createFO.FODeliveryList = pedidos.Select(pedido =>
                new FODelivery()
                {
                    Delivery = pedido.NumeroPedidoEmbarcador,
                    Cost = carregamento ? 0 : ObterValorFretePorPedido(pedido)
                }
            ).ToList();

            return createFO;
        }

        private decimal ObterValorFretePorPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Servicos.Embarcador.Carga.ICMS serRegraICMS = new Embarcador.Carga.ICMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorPedido(pedido.Codigo).FirstOrDefault();

            decimal valor = cargaPedido?.ValorFreteAPagar ?? 0;

            return valor == 0 ? 0 : serRegraICMS.ObterValorLiquido(valor, cargaPedido.PercentualAliquota);
        }

        #endregion
    }
}
