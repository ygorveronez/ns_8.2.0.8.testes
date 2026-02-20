using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pessoa
{
    public class GrupoPessoaTipoCargaEmbarcador
    {
        public static bool BuscarTiposCargaEmbarcador(out string erro, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string url, string token, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            ServicoSGT.TipoCarga.TipoCargaClient svcTipoCarga = ObterClientTipoCarga(url, token);

            ServicoSGT.TipoCarga.RetornoOfArrayOfTipoCargaEmbarcadorYCIsJlsP retorno = svcTipoCarga.BuscarTiposCargasDisponiveis();

            if (!retorno.Status)
            {
                erro = retorno.Mensagem;
                return false;
            }

            Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador repGrupoPessoasTipoCargaEmbarcador = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga repGrupoPessoasTipoCargaEmbarcadorTipoCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcadorTipoCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador> tiposCargaVinculados = repGrupoPessoasTipoCargaEmbarcador.BuscarPorGrupoPessoas(grupoPessoas.Codigo);

            unitOfWork.Start();

            foreach (ServicoSGT.TipoCarga.TipoCargaEmbarcador tipoCargaEmbarcador in retorno.Objeto)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador tipoVinculado = tiposCargaVinculados.Where(o => o.CodigoTipoCargaEmbarcador == tipoCargaEmbarcador.CodigoIntegracao).FirstOrDefault();

                if (tipoVinculado != null)
                    continue;

                tipoVinculado = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador()
                {
                    CodigoTipoCargaEmbarcador = tipoCargaEmbarcador.CodigoIntegracao,
                    DescricaoTipoCargaEmbarcador = tipoCargaEmbarcador.Descricao,
                    GrupoPessoas = grupoPessoas
                };

                repGrupoPessoasTipoCargaEmbarcador.Inserir(tipoVinculado, auditado);
            }

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasTipoCargaEmbarcador tipoVinculado in tiposCargaVinculados)
            {
                if (!retorno.Objeto.Any(o => o.CodigoIntegracao == tipoVinculado.CodigoTipoCargaEmbarcador))
                {
                    repGrupoPessoasTipoCargaEmbarcadorTipoCarga.DeletarPorTipoCargaEmbarcador(tipoVinculado.Codigo);
                    repGrupoPessoasTipoCargaEmbarcador.Deletar(tipoVinculado, auditado);
                }
            }

            unitOfWork.CommitChanges();

            erro = string.Empty;
            return true;
        }

        #region Métodos Privados

        public static ServicoSGT.TipoCarga.TipoCargaClient ObterClientTipoCarga(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "TipoCarga.svc";

            ServicoSGT.TipoCarga.TipoCargaClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.TipoCarga.TipoCargaClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.TipoCarga.TipoCargaClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        #endregion
    }
}
