using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Integracao.Qbit
{
    public sealed class IntegracaoQbit
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoQbit(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void ConsultarPesagensPlaca(Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            if (string.IsNullOrEmpty(integracao.URLQbit))
            {
                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPesagem.ProblemaIntegracao = "Não foi configurada a integração com a Qbit, por favor verifique.";
                return;
            }

            ServicoQbit.PortariaClient portariaClient = ObterClient(integracao.URLQbit);

            integracaoPesagem.DataIntegracao = DateTime.Now;
            integracaoPesagem.NumeroTentativas += 1;

            InspectorBehavior inspector = new InspectorBehavior();
            portariaClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem;
            bool sucesso = false;

            try
            {
                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = integracaoPesagem.Pesagem;
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = integracaoPesagem.Pesagem.Guarita;

                string xmlRetorno = portariaClient.ConsultarPesagensPlaca(pesagem.DataPesagem.Date.AddDays(-1), pesagem.DataPesagem.Date.AddDays(1), guarita.Carga.Veiculo?.Placa, guarita.NumeroNfProdutor);

                //Retornos diferentes quando é erro ou sucesso
                if (!xmlRetorno.Contains("<ListaPesagem xmlns"))//Se não contém a lista, tem erro
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Qbit.RetornoConsultaPesagemErro retornoConsultaPesagem = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.Integracao.Qbit.RetornoConsultaPesagemErro>(xmlRetorno);

                    throw new ServicoException(retornoConsultaPesagem.CodigoRetorno + " - " + retornoConsultaPesagem.DescricaoRetorno);
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Qbit.RetornoConsultaPesagem retornoConsultaPesagem = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.Integracao.Qbit.RetornoConsultaPesagem>(xmlRetorno);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Qbit.RetornoConsultaPesagemLista retorno = retornoConsultaPesagem.Pesagens.LastOrDefault();
                    pesagem.CodigoPesagem = retorno.Codigo.Trim();

                    if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.PesagemInicial)
                    {
                        if ((retorno.DataPesagemBruto > DateTime.MinValue) && ((retorno.DataPesagemBruto < retorno.DataPesagemTara) || (retorno.DataPesagemTara == DateTime.MinValue)))
                        {
                            pesagem.PesoInicial = retorno.PesoBruto;
                            guarita.PesagemInicial = retorno.PesoBruto;
                        }
                        else
                        {
                            pesagem.PesoInicial = retorno.PesoTara;
                            guarita.PesagemInicial = retorno.PesoTara;
                        }

                        if (pesagem.PesoInicial <= 0)
                            throw new ServicoException("Peso Inicial retornou zerado da balança.");

                        pesagem.StatusBalanca = StatusBalanca.TicketCriado;
                        guarita.OrigemPesagemInicial = OrigemPesagemGuarita.Integracao;
                    }
                    else if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.PesagemFinal)
                    {
                        if ((retorno.DataPesagemBruto > DateTime.MinValue) && (retorno.DataPesagemBruto > retorno.DataPesagemTara))
                        {
                            pesagem.PesoFinal = retorno.PesoBruto;
                            guarita.PesagemFinal = retorno.PesoBruto;
                        }
                        else
                        {
                            pesagem.PesoFinal = retorno.PesoTara;
                            guarita.PesagemFinal = retorno.PesoTara;
                        }

                        if (pesagem.PesoFinal <= 0)
                            throw new ServicoException("Peso Final retornou zerado da balança.");

                        guarita.OrigemPesagemFinal = OrigemPesagemGuarita.Integracao;

                        Servicos.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem servicoConfiguracaoPesagem = new GestaoPatio.ConfiguracaoToleranciaPesagem(_unitOfWork);
                        servicoConfiguracaoPesagem.ValidarPesagem(guarita);
                    }

                    if (pesagem.PesoFinal > 0)
                        pesagem.StatusBalanca = StatusBalanca.Encerrado;

                    repPesagem.Atualizar(pesagem);
                    repCargaJanelaCarregamentoGuarita.Atualizar(guarita);

                    mensagem = "Integração realizada com sucesso.";
                    sucesso = true;
                }
            }
            catch (ServicoException ex)
            {
                mensagem = ex.Message;
                sucesso = false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falha ao realizar a integração.";
            }

            AdicionarArquivoTransacao(integracaoPesagem, inspector, mensagem);
            integracaoPesagem.ProblemaIntegracao = mensagem;

            if (!sucesso)
                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            else
                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.Integrado;

            repPesagemIntegracao.Atualizar(integracaoPesagem);
        }

        #endregion

        #region Métodos Privados

        private static ServicoQbit.PortariaClient ObterClient(string url)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new ServicoQbit.PortariaClient(binding, endpointAddress);
        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem, InspectorBehavior inspector, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(inspector.LastRequestXML) && string.IsNullOrWhiteSpace(inspector.LastResponseXML))
                return;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            arquivoIntegracao.ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            arquivoIntegracao.ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
            arquivoIntegracao.Data = integracaoPesagem.DataIntegracao;
            arquivoIntegracao.Mensagem = mensagem;
            arquivoIntegracao.Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (integracaoPesagem.ArquivosTransacao == null)
                integracaoPesagem.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            integracaoPesagem.ArquivosTransacao.Add(arquivoIntegracao);
        }

        #endregion
    }
}
