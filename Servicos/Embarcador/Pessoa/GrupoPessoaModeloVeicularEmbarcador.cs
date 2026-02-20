using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pessoa
{
    public class GrupoPessoaModeloVeicularEmbarcador
    {
        public static bool BuscarModelosVeicularesEmbarcador(out string erro, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string url, string token, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            ServicoSGT.ModeloVeicular.ModeloVeicularClient svcModeloVeicular = ObterClientModeloVeicular(url, token);

            ServicoSGT.ModeloVeicular.RetornoOfArrayOfModeloVeicularYCIsJlsP retorno = svcModeloVeicular.BuscarModelosVeicularesDisponiveis();

            if (!retorno.Status)
            {
                erro = retorno.Mensagem;
                return false;
            }

            Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador repGrupoPessoasModeloVeicularEmbarcador = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador> modelosVeicularesVinculados = repGrupoPessoasModeloVeicularEmbarcador.BuscarPorGrupoPessoas(grupoPessoas.Codigo);

            unitOfWork.Start();

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular modeloVeicularEmbarcador in retorno.Objeto)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador modeloVinculado = modelosVeicularesVinculados.Where(o => o.CodigoModeloVeicularEmbarcador == modeloVeicularEmbarcador.CodigoIntegracao).FirstOrDefault();

                if (modeloVinculado != null)
                    continue;

                modeloVinculado = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador()
                {
                    CodigoModeloVeicularEmbarcador = modeloVeicularEmbarcador.CodigoIntegracao,
                    DescricaoModeloVeicularEmbarcador = modeloVeicularEmbarcador.Descricao,
                    GrupoPessoas = grupoPessoas
                };

                repGrupoPessoasModeloVeicularEmbarcador.Inserir(modeloVinculado, auditado);
            }

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcador modeloVinculado in modelosVeicularesVinculados)
            {
                if (!retorno.Objeto.Any(o => o.CodigoIntegracao == modeloVinculado.CodigoModeloVeicularEmbarcador))
                {
                    repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.DeletarPorModeloVeicularEmbarcador(modeloVinculado.Codigo);
                    repGrupoPessoasModeloVeicularEmbarcador.Deletar(modeloVinculado, auditado);
                }
            }

            unitOfWork.CommitChanges();

            erro = string.Empty;
            return true;
        }

        #region Métodos Privados

        public static ServicoSGT.ModeloVeicular.ModeloVeicularClient ObterClientModeloVeicular(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "ModeloVeicular.svc";

            ServicoSGT.ModeloVeicular.ModeloVeicularClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.ModeloVeicular.ModeloVeicularClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.ModeloVeicular.ModeloVeicularClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        #endregion
    }
}
