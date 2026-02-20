using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.BalancaKIKI
{
    public class IntegracaoBalancaKIKI
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoBalancaKIKI(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void ConsultarPesagensPlaca(Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem)
        {
            Repositorio.Embarcador.Logistica.PesagemIntegracao repositorioPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoBalancaKIKI repositorioIntegracaoBalancaKIKI = new Repositorio.Embarcador.Configuracoes.IntegracaoBalancaKIKI(_unitOfWork);
            Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBalancaKIKI configuracaoIntegracaoBalancaKIKI = repositorioIntegracaoBalancaKIKI.Buscar();

            integracaoPesagem.DataIntegracao = DateTime.Now;
            integracaoPesagem.NumeroTentativas++;

            if (!(configuracaoIntegracaoBalancaKIKI?.PossuiIntegracao ?? false) || string.IsNullOrWhiteSpace(configuracaoIntegracaoBalancaKIKI?.URL))
            {
                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPesagem.ProblemaIntegracao = "Não foi configurada a integração com a Balança KIKI, por favor verifique.";
                return;
            }

            ServicoBalancaKIKI.ServiceClient serviceClient = ObterClient(configuracaoIntegracaoBalancaKIKI.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            serviceClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = integracaoPesagem.Pesagem;
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = integracaoPesagem.Pesagem.Guarita;

                ServicoBalancaKIKI.ListaPesagem retornoConsultaPesagem = serviceClient.ConsultarPesagensPlaca(pesagem.DataPesagem.Date.AddDays(-3), pesagem.DataPesagem.Date, guarita.Carga.Veiculo?.Placa, guarita.NumeroNfProdutor);
                //ServicoBalancaKIKI.ListaPesagem retornoConsultaPesagem = serviceClient.ConsultarPesagensPlaca("10/08/2023".ToDateTime(), "10/08/2023".ToDateTime(), "GVJ5238", 2794);//Exemplo de retorno com sucesso da NaturalOne

                if (retornoConsultaPesagem.CodigoRetorno != 1)
                    throw new ServicoException($"{retornoConsultaPesagem.CodigoRetorno} - {retornoConsultaPesagem.DescricaoRetorno}");

                ServicoBalancaKIKI.Pesagem retorno = retornoConsultaPesagem.Pesagens.LastOrDefault();
                pesagem.CodigoPesagem = retorno.Codigo.ToString();

                pesagem.PesoInicial = (decimal)retorno.PesoBruto;
                guarita.PesagemInicial = (decimal)retorno.PesoBruto;

                if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.PesagemInicial)
                {
                    if (retorno.PesoBruto <= 0)
                        throw new ServicoException("Peso Inicial (Bruto) retornou zerado da balança.");

                    pesagem.StatusBalanca = StatusBalanca.TicketCriado;
                }
                else if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.PesagemFinal)
                {
                    if (retorno.PesoTara <= 0)
                        throw new ServicoException("Peso Final (Tara) retornou zerado da balança.");

                    pesagem.PesoFinal = (decimal)retorno.PesoTara;
                    guarita.PesagemFinal = (decimal)retorno.PesoTara;
                    guarita.PorcentagemPerda = (decimal)retorno.PorcentagemRefugo;
                }

                if (pesagem.PesoFinal > 0)
                {
                    AtualizarChecklistPerguntasCarga(integracaoPesagem.Codigo, retorno);
                    pesagem.StatusBalanca = StatusBalanca.Encerrado;
                }

                repositorioPesagem.Atualizar(pesagem);
                repositorioCargaJanelaCarregamentoGuarita.Atualizar(guarita);

                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracaoPesagem.ProblemaIntegracao = "Integração realizada com sucesso";
            }
            catch (ServicoException ex)
            {
                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPesagem.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPesagem.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a KIKI.";
            }

            servicoArquivoTransacao.Adicionar(integracaoPesagem, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repositorioPesagemIntegracao.Atualizar(integracaoPesagem);
        }
        public List<string> GetTagsDeIntegracaoCheckListPerguntas(List<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistOpcoesRelacaoCampo> relacoesCampo = null)
        {
            List<string> retorno = new List<string>() { "Brix", "QtdeFcoj", "QtdeNfc", "Ratio", "Rendimento" };
            if (relacoesCampo != null && relacoesCampo.Exists(x => x.CheckListOpcaoRelacaoCampo == CheckListOpcaoRelacaoCampo.ResultadoRendimentoLaranja))
                retorno.AddRange(new List<string>(){"PesoTotalExtraidoFruta", "PesoCorretoComRefugo" });
            return retorno;
        }
        #endregion

        #region Métodos Privados


        private bool AtualizaCheckListCargaPeloValorDaTag(ServicoBalancaKIKI.Pesagem pessagemKIKI, Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta pergunta)
        {
            bool atualizou = false;
            if (pergunta.TagIntegracao == "Brix")
            {
                try
                {
                    pergunta.Observacao =  pessagemKIKI.Brix.ToString();
                    atualizou = true;
                }
                catch (Exception)
                {
                }
            }

            if (pergunta.TagIntegracao == "QtdeFcoj")
            {
                try
                {
                    pergunta.Observacao = pessagemKIKI.QtdeFcoj.ToString();
                    atualizou = true;
                }
                catch (Exception)
                {
                }
            }
            if (pergunta.TagIntegracao == "QtdeNfc")
            {
                try
                {
                    pergunta.Observacao = pessagemKIKI.QtdeNfc.ToString();
                    atualizou = true;
                }
                catch (Exception)
                {
                }
            }
            if (pergunta.TagIntegracao == "Ratio")
            {
                try
                {
                    pergunta.Observacao = pessagemKIKI.Ratio.ToString();
                    atualizou = true;
                }
                catch (Exception)
                {
                }
            }
            if (pergunta.TagIntegracao == "Rendimento")
            {
                try
                {
                    pergunta.Observacao = pessagemKIKI.Rendimento.ToString();
                    atualizou = true;
                }
                catch (Exception)
                {
                }
            }
            return atualizou;
        }


        private void AtualizarChecklistPerguntasCarga(int codigoIntegracao, ServicoBalancaKIKI.Pesagem pessagemKIKI)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta repositorioChecklistPergunta = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPergunta> CheckListCargaPergunta = repositorioChecklistPergunta.BuscarPerguntasCargaPorIdIntegracaoBalanca(codigoIntegracao);
            List<string> TagsDaIntegracao = GetTagsDeIntegracaoCheckListPerguntas();
            foreach (var pergunta in CheckListCargaPergunta.Where(x => TagsDaIntegracao.Contains(x.TagIntegracao)))
            {
                if (AtualizaCheckListCargaPeloValorDaTag(pessagemKIKI, pergunta))
                    repositorioChecklistPergunta.Atualizar(pergunta);
            }
        }

        private ServicoBalancaKIKI.ServiceClient ObterClient(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new ServicoBalancaKIKI.ServiceClient(binding, endpointAddress);
        }

        #endregion
    }
}
