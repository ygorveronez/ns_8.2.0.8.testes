using System;
using System.Linq;

namespace Servicos.Embarcador.Integracao.MultiEmbarcador
{
    public class CIOT
    {
        public static void IntegrarCIOT(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas += 1;

            if (cargaCIOT == null)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaIntegracao.ProblemaIntegracao = "Não foi encontrado um CIOT para integração.";

                repCargaIntegracao.Atualizar(cargaIntegracao);

                return;
            }

            if (cargaCIOT.CIOT.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "O CIOT deve estar aberto para esta integração.";

                repCargaIntegracao.Atualizar(cargaIntegracao);

                return;
            }

            if (cargaIntegracao.Carga.TipoOperacao?.HabilitarIntegracaoMultiEmbarcador != true)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "A integração com o MultiEmbarcador está desabilitada.";

                repCargaIntegracao.Atualizar(cargaIntegracao);

                return;
            }

            if (cargaIntegracao.Carga.TipoOperacao?.IntegrarCIOTMultiEmbarcador != true)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "A integração do CIOT com o MultiEmbarcador está desabilitada.";

                repCargaIntegracao.Atualizar(cargaIntegracao);

                return;
            }

            if (string.IsNullOrWhiteSpace(cargaIntegracao.Carga.TipoOperacao?.URLIntegracaoMultiEmbarcador) || string.IsNullOrWhiteSpace(cargaIntegracao.Carga.TipoOperacao?.TokenIntegracaoMultiEmbarcador))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "A configuração (token/url) da integração com o MultiEmbarcador é inválida.";

                repCargaIntegracao.Atualizar(cargaIntegracao);

                return;
            }

            string urlWebService = cargaIntegracao.Carga.TipoOperacao?.URLIntegracaoMultiEmbarcador;
            string token = cargaIntegracao.Carga.TipoOperacao?.TokenIntegracaoMultiEmbarcador;

            if (!urlWebService.EndsWith("/"))
                urlWebService += "/";

            urlWebService += "Empresa.svc";

            ServicoSGT.Empresa.EmpresaClient svcEmpresa = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoSGT.Empresa.EmpresaClient, ServicoSGT.Empresa.IEmpresa>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SGTWebService_Empresas, urlWebService, out Servicos.Models.Integracao.InspectorBehavior inspector);

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(svcEmpresa.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            string mensagem = null;
            bool sucesso = false;

            try
            {
                Servicos.Embarcador.Veiculo.Veiculo svcVeiculo = new Servicos.Embarcador.Veiculo.Veiculo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao = svcVeiculo.ConverterObjetoVeiculo(cargaIntegracao.Carga.Veiculo, cargaIntegracao.Carga.VeiculosVinculados.ToList(), cargaCIOT.CIOT.Numero, unitOfWork);

                ServicoSGT.Empresa.RetornoOfboolean retorno = svcEmpresa.SalvarVeiculo(veiculoIntegracao);

                sucesso = retorno.Status;

                if (sucesso)
                    mensagem = "Integração realizada com sucesso.";
                else
                    mensagem = retorno.Mensagem;
            }
            catch (Exception ex)
            {
                mensagem = "Ocorreu uma falha ao realizar a integração.";
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
            arquivoIntegracao.Mensagem = mensagem;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            cargaIntegracao.ProblemaIntegracao = mensagem;

            if (!sucesso)
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            else
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }
    }
}
