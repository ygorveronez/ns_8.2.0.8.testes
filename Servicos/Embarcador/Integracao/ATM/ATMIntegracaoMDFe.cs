using System;
using System.Linq;

namespace Servicos.Embarcador.Integracao.ATM
{
    public class ATMIntegracaoMDFe
    {
        public static void EncerrarDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoMDFe averbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            IntegracaoATM.ATMWebSvrPortTypeClient integracaoATMClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<IntegracaoATM.ATMWebSvrPortTypeClient, IntegracaoATM.ATMWebSvrPortType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.ATM_ATMWebSvrPortType, out Models.Integracao.InspectorBehavior inspector);

            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(apolice.Codigo);

            string xml = ObterXMLEncerramento(averbacao.MDFe, unitOfWork);

            IntegracaoATM.RetornoMDFe retorno = integracaoATMClient.declaraMDFe(averbacaoATM.Usuario, averbacaoATM.Senha, averbacaoATM.CodigoATM, xml);

            if (retorno.Declarado != null)
            {
                averbacao.MensagemRetorno = "Encerrado com sucesso.";
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Encerrado;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Encerramento;
            }
            else
            {
                averbacao.CodigoRetorno = retorno.Erros.FirstOrDefault().Codigo;
                averbacao.MensagemRetorno = retorno.Erros.FirstOrDefault().Descricao;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Encerramento;
            }

            averbacao.DataRetorno = DateTime.Now;

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Mensagem = "Encerramento: " + averbacao.CodigoRetorno + " - " + averbacao.MensagemRetorno,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);

            averbacao.ArquivosIntegracao.Add(averbacaoIntegracaoArquivo);

            repAverbacaoMDFe.Atualizar(averbacao);
        }

        public static void CancelarDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoMDFe averbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            IntegracaoATM.ATMWebSvrPortTypeClient integracaoATMClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<IntegracaoATM.ATMWebSvrPortTypeClient, IntegracaoATM.ATMWebSvrPortType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.ATM_ATMWebSvrPortType, out Models.Integracao.InspectorBehavior inspector);

            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(apolice.Codigo);

            string xml = ObterXMLCancelamento(averbacao.MDFe, unitOfWork);

            IntegracaoATM.RetornoMDFe retorno = integracaoATMClient.declaraMDFe(averbacaoATM.Usuario, averbacaoATM.Senha, averbacaoATM.CodigoATM, xml);

            if (retorno.Declarado != null)
            {
                averbacao.MensagemRetorno = "Cancelado com sucesso.";
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Cancelado;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Cancelamento;

            }
            else
            {
                averbacao.CodigoRetorno = retorno.Erros.FirstOrDefault().Codigo;
                averbacao.MensagemRetorno = retorno.Erros.FirstOrDefault().Descricao;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Cancelamento;
            }

            averbacao.DataRetorno = DateTime.Now;

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Mensagem = "Cancelamento: " + averbacao.CodigoRetorno + " - " + averbacao.MensagemRetorno,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);

            averbacao.ArquivosIntegracao.Add(averbacaoIntegracaoArquivo);

            repAverbacaoMDFe.Atualizar(averbacao);
        }

        public static void AverbarDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoMDFe averbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            IntegracaoATM.ATMWebSvrPortTypeClient integracaoATMClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<IntegracaoATM.ATMWebSvrPortTypeClient, IntegracaoATM.ATMWebSvrPortType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.ATM_ATMWebSvrPortType, out Models.Integracao.InspectorBehavior inspector);

            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(apolice.Codigo);

            string xml = ObterXMLAutorizacao(averbacao.MDFe, unitOfWork);

            try
            {
                IntegracaoATM.RetornoMDFe retorno = integracaoATMClient.declaraMDFe(averbacaoATM.Usuario, averbacaoATM.Senha, averbacaoATM.CodigoATM, xml);

                if (retorno.Declarado != null)
                {
                    averbacao.Protocolo = retorno.Declarado.Protocolo;
                    averbacao.DataRetorno = retorno.Declarado.dhChancela;
                    averbacao.MensagemRetorno = "Averbado com sucesso.";

                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao;
                }
                else
                {
                    averbacao.CodigoRetorno = retorno.Erros.FirstOrDefault().Codigo;
                    averbacao.MensagemRetorno = retorno.Erros.FirstOrDefault().Descricao;
                    averbacao.DataRetorno = DateTime.Now;

                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao;

                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                averbacao.CodigoRetorno = "999";
                averbacao.MensagemRetorno = "Ocorreu uma falha ao comunicar com a ATM, serviço indisponível.";
                averbacao.DataRetorno = DateTime.Now;

                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Mensagem = "Autorização: " + averbacao.CodigoRetorno + " - " + averbacao.MensagemRetorno,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);

            averbacao.ArquivosIntegracao.Add(averbacaoIntegracaoArquivo);

            repAverbacaoMDFe.Atualizar(averbacao);
        }

        private static string ObterXMLCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLMDFe repXMLMDFe = new Repositorio.XMLMDFe(unitOfWork);

            Dominio.Entidades.XMLMDFe xmlMDFe = repXMLMDFe.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento);

            if (xmlMDFe != null)
                return xmlMDFe.XML;

            return string.Empty;
        }

        private static string ObterXMLEncerramento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLMDFe repXMLMDFe = new Repositorio.XMLMDFe(unitOfWork);

            Dominio.Entidades.XMLMDFe xmlMDFe = repXMLMDFe.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Encerramento);

            if (xmlMDFe != null)
                return xmlMDFe.XML;

            return string.Empty;
        }

        private static string ObterXMLAutorizacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLMDFe repXMLMDFe = new Repositorio.XMLMDFe(unitOfWork);

            Dominio.Entidades.XMLMDFe xmlMDFe = repXMLMDFe.BuscarPorMDFe(mdfe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao);

            if (xmlMDFe != null)
                return xmlMDFe.XML;

            return string.Empty;
        }
    }
}
