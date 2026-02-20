using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.MultiEmbarcador
{
    public class Veiculo
    {
        #region Métodos Públicos

        public static void IntegrarVeiculosPendentesIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> integracoes = repVeiculoIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador);

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao in integracoes)
                IntegrarVeiculoEmbarcador(integracao, unitOfWork);
        }

        public static void IntegrarVeiculoEmbarcador(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);

            veiculoIntegracao.DataIntegracao = DateTime.Now;
            veiculoIntegracao.NumeroTentativas += 1;

            if (veiculoIntegracao.GrupoPessoas == null || !(veiculoIntegracao.GrupoPessoas.UtilizaMultiEmbarcador ?? false) || !(veiculoIntegracao.GrupoPessoas.HabilitarIntegracaoVeiculoMultiEmbarcador ?? false))
            {
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = $"A integração do veículo com este embarcador ({veiculoIntegracao.GrupoPessoas?.Descricao ?? string.Empty}) está desabilitada.";

                repVeiculoIntegracao.Atualizar(veiculoIntegracao);

                return;
            }

            if (string.IsNullOrWhiteSpace(veiculoIntegracao.GrupoPessoas.URLIntegracaoMultiEmbarcador) || string.IsNullOrWhiteSpace(veiculoIntegracao.GrupoPessoas.TokenIntegracaoMultiEmbarcador))
            {
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = "A configuração (token/url) da integração com o MultiEmbarcador é inválida.";

                repVeiculoIntegracao.Atualizar(veiculoIntegracao);

                return;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            ServicoSGT.Empresa.EmpresaClient svcEmpresa = ObterClientEmpresa(veiculoIntegracao.GrupoPessoas.URLIntegracaoMultiEmbarcador, veiculoIntegracao.GrupoPessoas.TokenIntegracaoMultiEmbarcador);

            svcEmpresa.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem = null;
            bool sucesso = false;

            try
            {
                Servicos.Embarcador.Veiculo.Veiculo svcVeiculo = new Servicos.Embarcador.Veiculo.Veiculo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegrar = svcVeiculo.ConverterObjetoVeiculoEmbarcador(veiculoIntegracao.GrupoPessoas, veiculoIntegracao.Veiculo, veiculoIntegracao.Veiculo.VeiculosVinculados.ToList(), unitOfWork);

                ServicoSGT.Empresa.RetornoOfboolean retorno = svcEmpresa.SalvarVeiculo(veiculoIntegrar);

                sucesso = retorno.Status;

                if (sucesso)
                    mensagem = "Integração realizada com sucesso.";
                else
                    mensagem = retorno.Mensagem;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falha ao realizar a integração.";
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = veiculoIntegracao.DataIntegracao,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            veiculoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            veiculoIntegracao.ProblemaIntegracao = mensagem;

            if (!sucesso)
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            else
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

            repVeiculoIntegracao.Atualizar(veiculoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private static ServicoSGT.Empresa.EmpresaClient ObterClientEmpresa(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Empresa.svc";

            ServicoSGT.Empresa.EmpresaClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.Empresa.EmpresaClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.Empresa.EmpresaClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        #endregion
    }
}
