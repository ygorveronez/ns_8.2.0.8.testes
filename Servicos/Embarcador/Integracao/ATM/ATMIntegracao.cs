using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.ATM
{
    public class ATMIntegracao
    {
        #region Métodos Públicos

        public static void CancelarAverbacaoDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            IntegracaoATM.ATMWebSvrPortTypeClient integracaoATMClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<IntegracaoATM.ATMWebSvrPortTypeClient, IntegracaoATM.ATMWebSvrPortType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.ATM_ATMWebSvrPortType, out Models.Integracao.InspectorBehavior inspector);

            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(apolice.Codigo);

            string xml;
            if (averbacao.XMLNotaFiscal != null)
                xml = ObterXMLCancelamentoNFe(averbacao.XMLNotaFiscal);
            else
                xml = ObterXMLCancelamento(averbacao.CTe, averbacaoATM.AverbaComoEmbarcador, averbacao.Protocolo, unitOfWork);

            try
            {
                if (averbacao.CTe != null && averbacao.CTe.OcorreuSinistroAvaria)
                {
                    averbacao.CodigoRetorno = "";
                    averbacao.MensagemRetorno = "Conhecimento com sinitro/avaria registrado, não é possível cancelar a sua averbação.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                }
                else
                {
                    IntegracaoATM.Retorno retorno;
                    if (averbacao.XMLNotaFiscal != null)
                        retorno = integracaoATMClient.averbaNFe(averbacaoATM.Usuario, averbacaoATM.Senha, averbacaoATM.CodigoATM, xml);
                    else
                        retorno = integracaoATMClient.averbaCTe(averbacaoATM.Usuario, averbacaoATM.Senha, averbacaoATM.CodigoATM, xml);

                    if (retorno.Averbado != null)
                    {
                        averbacao.MensagemRetorno = "Cancelado com sucesso.";
                        averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado;
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                    }
                    else
                    {
                        averbacao.CodigoRetorno = retorno.Erros.FirstOrDefault().Codigo;
                        averbacao.MensagemRetorno = retorno.Erros.FirstOrDefault().Descricao;
                        averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                    }

                    Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo();
                    averbacaoIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
                    averbacaoIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
                    averbacaoIntegracaoArquivo.Data = DateTime.Now;
                    averbacaoIntegracaoArquivo.Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno;
                    averbacaoIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
                    averbacao.ArquivosTransacaoCancelamento.Add(averbacaoIntegracaoArquivo);
                }

                tentativas = 0;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (averbacao.tentativasIntegracao >= 1)
                {
                    averbacao.CodigoRetorno = "999";
                    averbacao.MensagemRetorno = "O Serviço da AT&M não está disponível no momento.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                    averbacao.tentativasIntegracao = 0;
                }
                else
                {
                    averbacao.tentativasIntegracao++;
                    tentativas++;
                }
            }

            averbacao.DataRetorno = DateTime.Now;
            repAverbacaoCTe.Atualizar(averbacao);

            if (averbacao.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado && averbacao.Tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento && averbacao.Carga != null && repCargaPedido.ContemProvedorOS(averbacao.Carga.Codigo))
            {
                int codigoAverbacao = averbacao.Codigo;
                string protocolo = averbacao.Protocolo;
                string numeroAverbacao = averbacao.Averbacao;

                Task.Run(() => EnviarEmailAverbacao(codigoAverbacao, protocolo, numeroAverbacao, stringConexao, true));
            }
        }

        public static void AverbarDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            IntegracaoATM.ATMWebSvrPortTypeClient integracaoATMClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<IntegracaoATM.ATMWebSvrPortTypeClient, IntegracaoATM.ATMWebSvrPortType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.ATM_ATMWebSvrPortType, out Models.Integracao.InspectorBehavior inspector);

            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(apolice.Codigo);

            Servicos.Log.TratarErro($"AverbarDocumento INICIO - CAA_CODIGO = {averbacaoATM.Codigo}", "AverbacaoATM");

            string xml;
            if (averbacao.XMLNotaFiscal != null)
                xml = ObterXMLAutorizacaoNFe(averbacao.XMLNotaFiscal);
            else
                xml = ObterXMLAutorizacaoATM(averbacao.CTe, averbacaoATM.AverbaComoEmbarcador, unitOfWork, averbacao.Forma == Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria, averbacaoATM);

            try
            {
                IntegracaoATM.Retorno retorno;
                if (averbacao.XMLNotaFiscal != null)
                    retorno = integracaoATMClient.averbaNFe(averbacaoATM.Usuario, averbacaoATM.Senha, averbacaoATM.CodigoATM, xml);
                else
                    retorno = integracaoATMClient.averbaCTe(averbacaoATM.Usuario, averbacaoATM.Senha, averbacaoATM.CodigoATM, xml);

                if (retorno.Averbado != null)
                {
                    averbacao.Protocolo = retorno.Averbado.Protocolo;
                    IntegracaoATM.DadosSeguro[] dadosSeguro = retorno.Averbado.DadosSeguro;
                    averbacao.Averbacao = dadosSeguro?.FirstOrDefault()?.NumeroAverbacao ?? string.Empty;
                    averbacao.MensagemRetorno = "Averbado com sucesso.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                }
                else
                {
                    averbacao.CodigoRetorno = retorno.Erros.FirstOrDefault().Codigo;
                    averbacao.MensagemRetorno = retorno.Erros.FirstOrDefault().Descricao;
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;

                    if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                        Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoDiversas(averbacao.Carga?.CodigoCargaEmbarcador, "warning", 9999, retorno.Erros.FirstOrDefault().Descricao, "Averbação ATM", unitOfWork);
                }

                Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo();
                averbacaoIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
                averbacaoIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
                averbacaoIntegracaoArquivo.Data = DateTime.Now;
                averbacaoIntegracaoArquivo.Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno;
                averbacaoIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
                averbacao.ArquivosTransacao.Add(averbacaoIntegracaoArquivo);
                tentativas = 0;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($" - CAA_CODIGO = {averbacaoATM.Codigo} - " + ex.Message, "AverbacaoATM");

                if (averbacao.tentativasIntegracao >= 1)
                {
                    averbacao.CodigoRetorno = "999";
                    averbacao.MensagemRetorno = "O Serviço da AT&M não está disponível no momento.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                    averbacao.tentativasIntegracao = 0;

                    if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                        Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoDiversas(averbacao.Carga?.CodigoCargaEmbarcador, "warning", 9999, "Serviço ATM não disponível.", "Averbação ATM", unitOfWork);
                }
                else
                {
                    averbacao.tentativasIntegracao++;
                    tentativas++;
                }
            }

            averbacao.DataRetorno = DateTime.Now;
            repAverbacaoCTe.Atualizar(averbacao);

            if (averbacao.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso && averbacao.Tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao && averbacao.Carga != null && repCargaPedido.ContemProvedorOS(averbacao.Carga.Codigo))
            {
                int codigoAverbacao = averbacao.Codigo;
                string protocolo = averbacao.Protocolo;
                string numeroAverbacao = averbacao.Averbacao;

                Task.Run(() => EnviarEmailAverbacao(codigoAverbacao, protocolo, numeroAverbacao, stringConexao, false));
            }

            Servicos.Log.TratarErro($"AverbarDocumento FIM - CAA_CODIGO = {averbacaoATM.Codigo}", "AverbacaoATM");
        }

        public static string ObterXMLCancelamento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool averbarComoEmbarcador, string protocolo, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

            string xml = "";
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe || averbarComoEmbarcador)
            {
                StringBuilder stXML = new StringBuilder();
                stXML.Append(@"<retCancCTe xmlns='http://www.portalfiscal.inf.br/cte' versao='1.04'>");
                stXML.Append("<infCanc>");
                stXML.Append("<cStat>101</cStat>");
                stXML.Append("<dhEmi>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhEmi>");
                stXML.Append("<dhRecbto>" + cte.DataCancelamento.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhRecbto>");
                stXML.Append("<nProt>" + protocolo + "</nProt>");
                stXML.Append("</infCanc>");
                stXML.Append("</retCancCTe>");
                xml = stXML.ToString();
            }
            else
            {
                Servicos.CTe svcCTE = new Servicos.CTe(unitOfWork);
                xml = svcCTE.ObterStringXMLCancelamento(cte, unitOfWork);
            }

            return xml;
        }

        public static string ObterXMLAutorizacaoATM(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool averbarComoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool averbacaoProvisoria, Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM)
        {
            string xml = "";

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe || averbarComoEmbarcador || averbacaoProvisoria)
            {
                Servicos.Log.TratarErro($"ObterXMLAutorizacaoATM - Documento não CT-e, CAA_CODIGO = {averbacaoATM.Codigo} - Chave CT-e: {cte.Chave} - CON_CODIGO = {cte.Codigo}", "AverbacaoATM");

                if ((averbacaoATM?.VersaoLayoutATMOutrosDocumentos ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumVersaoLayoutATM.Versao200) == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumVersaoLayoutATM.Versao200)
                    xml = ObterXMLAutorizacaoATM_Versao200(cte, averbarComoEmbarcador, averbacaoProvisoria, unitOfWork);
                else
                    xml = ObterXMLAutorizacaoATM_Versao400(cte, averbarComoEmbarcador, averbacaoProvisoria, unitOfWork);
            }
            else
            {
                Servicos.Log.TratarErro($"ObterXMLAutorizacaoATM - Documento igual CT-e, CAA_CODIGO = {averbacaoATM.Codigo} - Chave CT-e: {cte.Chave} - CON_CODIGO = {cte.Codigo}", "AverbacaoATM");

                Servicos.CTe svcCTE = new Servicos.CTe(unitOfWork);
                xml = svcCTE.ObterStringXMLAutorizacao(cte, unitOfWork);
            }

            return xml;
        }

        public static void AverbarNFSe(Dominio.Entidades.AverbacaoNFSe averbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.AverbacaoNFSe repAverbacaoNFSe = new Repositorio.AverbacaoNFSe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);

            IntegracaoATM.ATMWebSvrPortTypeClient integracaoATMClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<IntegracaoATM.ATMWebSvrPortTypeClient, IntegracaoATM.ATMWebSvrPortType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.ATM_ATMWebSvrPortType, out Models.Integracao.InspectorBehavior inspector);

            string xml = ObterXMLAutorizacaoATM(averbacao.NFSe, unitOfWork);

            IntegracaoATM.Retorno retorno = integracaoATMClient.averbaCTe(averbacao.Usuario, averbacao.Senha, averbacao.CodigoUsuario, xml);

            if (retorno.Averbado != null)
            {
                averbacao.Protocolo = retorno.Averbado.Protocolo;
                if (retorno.Averbado.DadosSeguro != null)
                {
                    IntegracaoATM.DadosSeguro[] dadosSeguro = retorno.Averbado.DadosSeguro;
                    averbacao.Averbacao = dadosSeguro.FirstOrDefault().NumeroAverbacao;
                }
                averbacao.MensagemRetorno = "Averbado com sucesso.";

                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;

            }
            else
            {
                averbacao.CodigoRetorno = retorno.Erros.FirstOrDefault().Codigo;
                averbacao.MensagemRetorno = retorno.Erros.FirstOrDefault().Descricao;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
            }

            averbacao.DataRetorno = DateTime.Now;

            Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo();
            averbacaoIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            averbacaoIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            averbacaoIntegracaoArquivo.Data = DateTime.Now;
            averbacaoIntegracaoArquivo.Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno;
            averbacaoIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
            if (averbacao.ArquivosTransacao == null)
                averbacao.ArquivosTransacao = new System.Collections.Generic.List<Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo>();
            averbacao.ArquivosTransacao.Add(averbacaoIntegracaoArquivo);

            repAverbacaoNFSe.Atualizar(averbacao);
        }

        public static void AverbarNFe(Dominio.Entidades.AverbacaoNFe averbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.AverbacaoNFe repAverbacaoNFe = new Repositorio.AverbacaoNFe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);

            IntegracaoATM.ATMWebSvrPortTypeClient integracaoATMClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<IntegracaoATM.ATMWebSvrPortTypeClient, IntegracaoATM.ATMWebSvrPortType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.ATM_ATMWebSvrPortType, out Models.Integracao.InspectorBehavior inspector);

            string xml = averbacao.XMLNotaFiscal.XML;

            IntegracaoATM.Retorno retorno = integracaoATMClient.averbaNFe(averbacao.Usuario, averbacao.Senha, averbacao.CodigoUsuario, xml);

            if (retorno.Averbado != null)
            {
                averbacao.Protocolo = retorno.Averbado.Protocolo;
                if (retorno.Averbado.DadosSeguro != null)
                {
                    IntegracaoATM.DadosSeguro[] dadosSeguro = retorno.Averbado.DadosSeguro;
                    averbacao.Averbacao = dadosSeguro.FirstOrDefault().NumeroAverbacao;
                }
                averbacao.MensagemRetorno = "Averbado com sucesso.";

                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;

            }
            else
            {
                averbacao.CodigoRetorno = retorno.Erros.FirstOrDefault().Codigo;
                averbacao.MensagemRetorno = retorno.Erros.FirstOrDefault().Descricao;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
            }

            averbacao.DataRetorno = DateTime.Now;

            Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo();
            averbacaoIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            averbacaoIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            averbacaoIntegracaoArquivo.Data = DateTime.Now;
            averbacaoIntegracaoArquivo.Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno;
            averbacaoIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
            if (averbacao.ArquivosTransacao == null)
                averbacao.ArquivosTransacao = new System.Collections.Generic.List<Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo>();
            averbacao.ArquivosTransacao.Add(averbacaoIntegracaoArquivo);

            repAverbacaoNFe.Atualizar(averbacao);
        }

        public static void CancelarNFSe(Dominio.Entidades.AverbacaoNFSe averbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.AverbacaoNFSe repAverbacaoNFSe = new Repositorio.AverbacaoNFSe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.AverbacaoNFSe averbacaoAutorizadora = repAverbacaoNFSe.BuscarPorNFSesTipoESituacao(averbacao.NFSe.Codigo, Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao, Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso);

            IntegracaoATM.ATMWebSvrPortTypeClient integracaoATMClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<IntegracaoATM.ATMWebSvrPortTypeClient, IntegracaoATM.ATMWebSvrPortType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.ATM_ATMWebSvrPortType, out Models.Integracao.InspectorBehavior inspector);

            string xml = ObterXMLCancelamento(averbacao.NFSe, averbacaoAutorizadora != null ? averbacaoAutorizadora.Protocolo : string.Empty, unitOfWork);
            IntegracaoATM.Retorno retorno = integracaoATMClient.averbaCTe(averbacao.Usuario, averbacao.Senha, averbacao.CodigoUsuario, xml);

            if (retorno.Averbado != null)
            {
                averbacao.MensagemRetorno = "Cancelado com sucesso.";
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;

            }
            else
            {
                if (retorno.Erros != null)
                {
                    averbacao.CodigoRetorno = retorno.Erros.FirstOrDefault().Codigo;
                    averbacao.MensagemRetorno = retorno.Erros.FirstOrDefault().Descricao;
                }
                else
                {
                    averbacao.CodigoRetorno = "8888";
                    averbacao.MensagemRetorno = "Não foi possível cancelar.";
                }
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
            }

            averbacao.DataRetorno = DateTime.Now;

            Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo();
            averbacaoIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            averbacaoIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            averbacaoIntegracaoArquivo.Data = DateTime.Now;
            averbacaoIntegracaoArquivo.Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno;
            averbacaoIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
            if (averbacao.ArquivosTransacaoCancelamento == null)
                averbacao.ArquivosTransacaoCancelamento = new System.Collections.Generic.List<Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo>();
            averbacao.ArquivosTransacaoCancelamento.Add(averbacaoIntegracaoArquivo);

            repAverbacaoNFSe.Atualizar(averbacao);
        }

        public static void EnviarEmailAverbacao(int codigoAverbacao, string protocolo, string numeroAverbacao, string stringConexao, bool averbacaoCancelada)
        {
            Dominio.Entidades.AverbacaoCTe averbacao = null;
            List<Dominio.Entidades.Cliente> provedorOS = null;
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = null;
            Repositorio.UnitOfWork unitOfWork = null;
            try
            {
                unitOfWork = new Repositorio.UnitOfWork(stringConexao);
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
                averbacao = repAverbacaoCTe.BuscarPorCodigo(codigoAverbacao);
                provedorOS = averbacao.Carga.Pedidos.Select(o => o.Pedido.ProvedorOS).ToList();
                if (email == null)
                {
                    Servicos.Log.TratarErro("Não há um e-mail configurado para realizar o envio.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "EnviarEmailAverbacao");
                return;
            }

            if (averbacao.CTe == null)
                return;

            try
            {
                string mensagemErro = "Erro ao enviar e-mail";
                string assunto = "Reenvio de OS " + (string.Join(", ", averbacao.Carga.Pedidos?.Select(o => o.Pedido.NumeroOS).ToList()) ?? "") + " com dados de averbação";
                string mensagemEmail = string.Empty;

                mensagemEmail = "<br/>Segue dados da Averbação:<br/><br/>";

                mensagemEmail += "<script src='https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js'></script>";
                mensagemEmail += "<script src='http://www.developerdan.com/table-to-json/javascripts/jquery.tabletojson.min.js'></script>";
                mensagemEmail += "<table id='tabela' style='width:100%; align='center'; border='1'; cellpadding='0'; cellspacing='0';>";
                mensagemEmail += "<thead>";
                mensagemEmail += "<th>Container</th>";
                if (averbacao.Forma != Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria)
                    mensagemEmail += "<th>Chave CTMC</th>";
                mensagemEmail += "<th>Remetente</th>";
                mensagemEmail += "<th>Destinatário</th>";
                mensagemEmail += "<th>NF</th>";
                mensagemEmail += "<th>Nº Averbação</th>";
                mensagemEmail += "<th>Data Averbação</th>";
                mensagemEmail += "<th>Tipo Averbação</th>";
                mensagemEmail += "</thead><tbody>";

                if (averbacao.CTe.Containers != null && averbacao.CTe.Containers.Count > 0)
                {
                    foreach (var container in averbacao.CTe.Containers)
                    {
                        mensagemEmail += "<tr>";
                        mensagemEmail += "<td>" + (container.Container?.Numero ?? "CNTR A DEFINIR") + "</td>";
                        if (averbacao.Forma != Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria)
                            mensagemEmail += "<td>" + averbacao.CTe.Chave + "</td>";

                        mensagemEmail += "<td>" + averbacao.CTe.Remetente.Descricao + "</td>";
                        mensagemEmail += "<td>" + averbacao.CTe.Destinatario.Descricao + "</td>";
                        mensagemEmail += "<td>" + averbacao.CTe.NumeroNotas + "</td>";

                        if (!averbacaoCancelada)
                        {
                            mensagemEmail += "<td>" + averbacao.Averbacao + "</td>";
                            mensagemEmail += "<td>" + (averbacao.DataRetorno.HasValue ? averbacao.DataRetorno.Value.ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm")) + "</td>";
                        }
                        else
                        {
                            mensagemEmail += "<td><span style='color: red'>CANCELADO</span></td>";
                            mensagemEmail += "<td>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "</td>";
                        }
                        if (averbacao.Forma != Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria)
                            mensagemEmail += "<td>Definitiva</td>";
                        else
                            mensagemEmail += "<td>Provisória</td>";

                        mensagemEmail += "</tr>";
                    }
                }
                else
                {
                    mensagemEmail += "<tr>";
                    mensagemEmail += "<td>" + "CNTR A DEFINIR" + "</td>";
                    if (averbacao.Forma != Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria)
                        mensagemEmail += "<td>" + averbacao.CTe.Chave + "</td>";
                    mensagemEmail += "<td>" + averbacao.CTe.Remetente.Descricao + "</td>";
                    mensagemEmail += "<td>" + averbacao.CTe.Destinatario.Descricao + "</td>";
                    mensagemEmail += "<td>" + averbacao.CTe.NumeroNotas + "</td>";
                    if (!averbacaoCancelada)
                    {
                        mensagemEmail += "<td>" + averbacao.Averbacao + "</td>";
                        mensagemEmail += "<td>" + (averbacao.DataRetorno.HasValue ? averbacao.DataRetorno.Value.ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm")) + "</td>";
                    }
                    else
                    {
                        mensagemEmail += "<td><span style='color: red'>CANCELADO</span></td>";
                        mensagemEmail += "<td>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "</td>";
                    }

                    if (averbacao.Forma != Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria)
                        mensagemEmail += "<td>Definitiva</td>";
                    else
                        mensagemEmail += "<td>Provisória</td>";

                    mensagemEmail += "</tr>";
                }
                mensagemEmail += "</tbody></table>";

                mensagemEmail += "<br/><br/>E -mail enviado automaticamente. Por favor, não responda.";
                List<string> emails = new List<string>();
                foreach (var pessoa in provedorOS)
                {
                    if (!string.IsNullOrWhiteSpace(pessoa.Email))
                        emails.AddRange(pessoa.Email.Split(';').ToList());

                    foreach (var outroEmail in pessoa.Emails)
                    {
                        if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A" && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Administrativo)
                            emails.Add(outroEmail.Email);
                    }
                }
                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();

                    if (averbacao.Forma != Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria)
                    {
                        string nomeArquivo = "";
                        byte[] data = null;
                        if (averbacao.CTe.ModeloDocumentoFiscal.Numero == "39")
                        {
                            nomeArquivo = averbacao.CTe.Numero.ToString() + "_" + averbacao.CTe.Serie.Numero.ToString() + ".xml";
                            Servicos.NFSe svcNFSe = new Servicos.NFSe();
                            data = svcNFSe.ObterXMLAutorizacaoCTe(averbacao.CTe.Codigo, unitOfWork);
                            if (data != null)
                            {
                                Stream stream = new MemoryStream(data);
                                attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo));
                            }
                        }
                        else
                        {
                            nomeArquivo = string.Concat(averbacao.CTe.Chave, ".xml");
                            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                            data = svcCTe.ObterXMLAutorizacao(averbacao.CTe, unitOfWork);
                            if (data != null)
                            {
                                Stream stream = new MemoryStream(data);
                                attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo));
                            }
                        }
                    }
                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail,
                                attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp);
                    if (!sucesso)
                        Servicos.Log.TratarErro("Problemas ao enviar a averbação para o provedor por e-mail: " + mensagemErro, "EnviarEmailAverbacao");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "EnviarEmailAverbacao");
            }

        }

        #endregion

        #region Métodos Privados

        private static string ObterXMLAutorizacaoATM(Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unitOfWork)
        {
            string xml = "";

            StringBuilder stXML = new StringBuilder();
            stXML.Append("<cteProc><CTe><infCte><ide>");
            stXML.Append("<mod>98</mod>");
            stXML.Append("<serie>u</serie>");
            stXML.Append("<nCT>" + nfse.Numero + "</nCT>");
            stXML.Append("<dhEmi>" + nfse.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhEmi>");
            stXML.Append("<tpAmb>" + (int)nfse.Ambiente + "</tpAmb>");
            stXML.Append("<tpCTe>0</tpCTe>");
            stXML.Append("<modal>01</modal>");
            stXML.Append("<tpServ>0</tpServ>");
            stXML.Append("<cMunIni>" + nfse.LocalidadePrestacaoServico.CodigoIBGE + "</cMunIni>");
            stXML.Append("<UFIni>" + nfse.LocalidadePrestacaoServico.Estado.Sigla + "</UFIni>");
            stXML.Append("<cMunFim>" + nfse.LocalidadePrestacaoServico.CodigoIBGE + "</cMunFim>");
            stXML.Append("<UFFim>" + nfse.LocalidadePrestacaoServico.Estado.Sigla + "</UFFim>");

            stXML.Append("<toma4>");
            stXML.Append("<toma>4</toma>");
            stXML.Append("<CNPJ>" + nfse.Tomador.CPF_CNPJ + "</CNPJ>");
            stXML.Append("<enderToma>");
            stXML.Append("<cMun>" + nfse.Tomador.Localidade.CodigoIBGE + "</cMun>");
            stXML.Append("<UF>" + nfse.Tomador.Localidade.Estado.Sigla + "</UF>");
            stXML.Append("<cPais>" + nfse.Tomador.Localidade.Pais.Codigo + "</cPais>");
            stXML.Append("</enderToma>");
            stXML.Append("</toma4>");

            stXML.Append("</ide>");

            stXML.Append("<emit>");
            stXML.Append("<CNPJ>" + nfse.Empresa.CNPJ_SemFormato + "</CNPJ>");
            stXML.Append("<enderEmit>");
            stXML.Append("<cMun>" + nfse.Empresa.Localidade.CodigoIBGE + "</cMun>");
            stXML.Append("<UF>" + nfse.Empresa.Localidade.Estado.Sigla + "</UF>");
            stXML.Append("</enderEmit>");
            stXML.Append("</emit>");

            stXML.Append("<rem>");
            stXML.Append("<CNPJ>" + nfse.Tomador.CPF_CNPJ + "</CNPJ>");
            stXML.Append("<enderReme>");
            stXML.Append("<cMun>" + nfse.Tomador.Localidade.CodigoIBGE + "</cMun>");
            stXML.Append("<UF>" + nfse.Tomador.Localidade.Estado.Sigla + "</UF>");
            stXML.Append("<cPais>" + nfse.Tomador.Localidade.Pais.Codigo + "</cPais>");
            stXML.Append("</enderReme>");
            stXML.Append("</rem>");

            stXML.Append("<dest>");
            stXML.Append("<CNPJ>" + nfse.Tomador.CPF_CNPJ + "</CNPJ>");
            stXML.Append("<enderDest>");
            stXML.Append("<cMun>" + nfse.Tomador.Localidade.CodigoIBGE + "</cMun>");
            stXML.Append("<UF>" + nfse.Tomador.Localidade.Estado.Sigla + "</UF>");
            stXML.Append("<cPais>" + nfse.Tomador.Localidade.Pais.Codigo + "</cPais>");
            stXML.Append("</enderDest>");
            stXML.Append("</dest>");

            stXML.Append("<infCTeNorm><infCarga>");

            string userInfo = "en-US";
            decimal valorMercadoria = 0;
            if (nfse.Documentos != null && nfse.Documentos.Count > 0)
                valorMercadoria = (from obj in nfse.Documentos select obj.Valor).Sum();

            stXML.Append("<vCarga>" + valorMercadoria.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</vCarga>");

            stXML.Append("</infCarga>");

            stXML.Append("</infCTeNorm>");
            stXML.Append("</infCte></CTe></cteProc>");

            xml = stXML.ToString();

            return xml;
        }

        private static string ObterXMLCancelamento(Dominio.Entidades.NFSe nfse, string protocolo, Repositorio.UnitOfWork unitOfWork)
        {
            string xml = "";

            StringBuilder stXML = new StringBuilder();
            stXML.Append(@"<retCancCTe xmlns='http://www.portalfiscal.inf.br/cte' versao='1.04'>");
            stXML.Append("<infCanc>");
            stXML.Append("<cStat>101</cStat>");
            stXML.Append("<dhEmi>" + nfse.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhEmi>");
            stXML.Append("<dhRecbto>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhRecbto>");
            stXML.Append("<nProt>" + protocolo + "</nProt>");
            stXML.Append("</infCanc>");
            stXML.Append("</retCancCTe>");
            xml = stXML.ToString();

            return xml;
        }

        private static string ObterXMLAutorizacaoNFe(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal)
        {
            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.XML))
                return xmlNotaFiscal.XML;

            Dominio.Entidades.Cliente emitente = xmlNotaFiscal.Emitente;
            Dominio.Entidades.Cliente destinatario = xmlNotaFiscal.Destinatario;
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            StringBuilder stXML = new StringBuilder();

            stXML.Append(@"<nfeProc versao='4.00' xmlns='http://www.portalfiscal.inf.br/nfe'>");
            stXML.Append("<NFe>");
            stXML.Append($"<infNFe versao='4.00' Id='NFe{xmlNotaFiscal.Chave}'>");

            stXML.Append("<ide>");
            stXML.Append($"<cUF>{emitente.Localidade.Estado.CodigoIBGE}</cUF>");
            stXML.Append("<cNF>00000000</cNF>");
            stXML.Append($"<natOp>{(!string.IsNullOrWhiteSpace(xmlNotaFiscal.NaturezaOP) ? xmlNotaFiscal.NaturezaOP : "NATUREZA FICTICIA")}</natOp>");
            stXML.Append("<mod>55</mod>");
            stXML.Append($"<serie>{xmlNotaFiscal.Serie}</serie>");
            stXML.Append($"<nNF>{xmlNotaFiscal.Numero}</nNF>");
            stXML.Append($"<dhEmi>{xmlNotaFiscal.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss-03:00")}</dhEmi>");
            stXML.Append($"<dhSaiEnt>{xmlNotaFiscal.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss-03:00")}</dhSaiEnt>");
            stXML.Append($"<tpNF>{xmlNotaFiscal.TipoOperacaoNotaFiscal.ToString("d")}</tpNF>");
            stXML.Append($"<idDest>{(emitente.Localidade.Estado.Sigla == destinatario.Localidade.Estado.Sigla ? "1" : "2")}</idDest>");
            stXML.Append($"<cMunFG>{emitente.Localidade.CodigoIBGE}</cMunFG>");
            stXML.Append("<tpImp>1</tpImp>");
            stXML.Append($"<tpEmis>{xmlNotaFiscal.TipoEmissao.ToString("d")}</tpEmis>");
            stXML.Append($"<cDV>{Utilidades.Chave.ObterDigitoVerificador(xmlNotaFiscal.Chave)}</cDV>");
            stXML.Append("<tpAmb>1</tpAmb>");
            stXML.Append("<finNFe>1</finNFe>");
            stXML.Append("<indFinal>0</indFinal>");
            stXML.Append("<indPres>9</indPres>");
            stXML.Append("<procEmi>0</procEmi>");
            stXML.Append("<verProc>4.00</verProc>");
            stXML.Append("</ide>");

            // compl
            if (xmlNotaFiscal.CTEs != null && xmlNotaFiscal.CTEs.Count > 0 && xmlNotaFiscal.CTEs[0].CargaCTes != null && xmlNotaFiscal.CTEs[0].CargaCTes.Count > 0)
            {
                if (xmlNotaFiscal.CTEs[0].CargaCTes[0]?.Carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ValorContainerAverbacao != null)
                {
                    stXML.Append("<compl>");
                    stXML.Append("<ObsCont xCampo=\"ValorContainer\">");
                    stXML.Append("<xTexto>" + xmlNotaFiscal.CTEs[0].CargaCTes[0]?.Carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ValorContainerAverbacao.ToString("n2") + "</xTexto>");
                    stXML.Append("</ObsCont>");
                    stXML.Append("</compl>");
                }
            }

            stXML.Append("<emit>");
            stXML.Append($"<CNPJ>{emitente.CPF_CNPJ_SemFormato}</CNPJ>");
            stXML.Append($"<xNome>{emitente.Nome}</xNome>");
            if (!string.IsNullOrWhiteSpace(emitente.NomeFantasia))
                stXML.Append($"<xFant>{emitente.NomeFantasia}</xFant>");
            stXML.Append("<enderEmit>");
            stXML.Append($"<xLgr>{emitente.Endereco}</xLgr>");
            stXML.Append($"<nro>{emitente.Numero}</nro>");
            stXML.Append($"<xBairro>{emitente.Bairro}</xBairro>");
            stXML.Append($"<cMun>{emitente.Localidade.CodigoIBGE}</cMun>");
            stXML.Append($"<xMun>{emitente.Localidade.Descricao}</xMun>");
            stXML.Append($"<UF>{emitente.Localidade.Estado.Sigla}</UF>");
            stXML.Append($"<CEP>{emitente.CEP}</CEP>");
            stXML.Append("<cPais>1058</cPais>");
            stXML.Append("<xPais>BRASIL</xPais>");
            stXML.Append("</enderEmit>");
            stXML.Append($"<IE>{emitente.IE_RG}</IE>");
            stXML.Append("<CRT>3</CRT>");
            stXML.Append("</emit>");

            stXML.Append("<dest>");
            if (destinatario.Tipo.Equals("F"))
                stXML.Append($"<CPF>{destinatario.CPF_CNPJ_SemFormato}</CPF>");
            else
                stXML.Append($"<CNPJ>{destinatario.CPF_CNPJ_SemFormato}</CNPJ>");
            stXML.Append($"<xNome>{destinatario.Nome}</xNome>");
            stXML.Append("<enderDest>");
            stXML.Append($"<xLgr>{destinatario.Endereco}</xLgr>");
            stXML.Append($"<nro>{destinatario.Numero}</nro>");
            stXML.Append($"<xBairro>{destinatario.Bairro}</xBairro>");
            stXML.Append($"<cMun>{destinatario.Localidade.CodigoIBGE}</cMun>");
            stXML.Append($"<xMun>{destinatario.Localidade.Descricao}</xMun>");
            stXML.Append($"<UF>{destinatario.Localidade.Estado.Sigla}</UF>");
            stXML.Append($"<CEP>{destinatario.CEP}</CEP>");
            stXML.Append("<cPais>1058</cPais>");
            stXML.Append("<xPais>BRASIL</xPais>");
            stXML.Append("</enderDest>");
            stXML.Append($"<indIEDest>{(destinatario.Tipo.Equals("F") ? "9" : "1")}</indIEDest>");
            stXML.Append($"<IE>{destinatario.IE_RG}</IE>");
            stXML.Append("</dest>");

            //Produto
            stXML.Append("<det nItem='1'>");
            stXML.Append("<prod>");
            stXML.Append("<cProd>0000</cProd>");
            stXML.Append("<cEAN>SEM GTIN</cEAN>");
            stXML.Append($"<xProd>{(!string.IsNullOrWhiteSpace(xmlNotaFiscal.Produto) ? xmlNotaFiscal.Produto : "PRODUTO FICTICIO")}</xProd>");
            stXML.Append($"<NCM>{(!string.IsNullOrWhiteSpace(xmlNotaFiscal.NCM) ? xmlNotaFiscal.NCM.PadRight(8, '0') : "00000000")}</NCM>");
            stXML.Append($"<CFOP>{(!string.IsNullOrWhiteSpace(xmlNotaFiscal.CFOP) ? xmlNotaFiscal.CFOP : "5102")}</CFOP>");
            stXML.Append("<uCom>UN</uCom>");
            stXML.Append("<qCom>1.0000</qCom>");
            stXML.Append($"<vUnCom>{xmlNotaFiscal.ValorTotalProdutos.ToString("n10", culture)}</vUnCom>");
            stXML.Append($"<vProd>{xmlNotaFiscal.ValorTotalProdutos.ToString("n2", culture)}</vProd>");
            stXML.Append("<cEANTrib>SEM GTIN</cEANTrib>");
            stXML.Append("<uTrib>UN</uTrib>");
            stXML.Append("<qTrib>1.0000</qTrib>");
            stXML.Append($"<vUnTrib>{xmlNotaFiscal.ValorTotalProdutos.ToString("n10", culture)}</vUnTrib>");
            stXML.Append("<indTot>1</indTot>");
            stXML.Append("</prod>");
            stXML.Append("<imposto>");
            stXML.Append("<vTotTrib>0.00</vTotTrib>");
            stXML.Append("<ICMS>");
            stXML.Append("<ICMS90>");
            stXML.Append("<orig>0</orig>");
            stXML.Append("<CST>90</CST>");
            if (xmlNotaFiscal.ValorICMS > 0 || xmlNotaFiscal.ValorST == 0)
            {
                stXML.Append("<modBC>3</modBC>");
                stXML.Append($"<vBC>{xmlNotaFiscal.BaseCalculoICMS.ToString("n2", culture)}</vBC>");
                stXML.Append($"<pICMS>{(xmlNotaFiscal.BaseCalculoICMS > 0 ? (xmlNotaFiscal.ValorICMS / xmlNotaFiscal.BaseCalculoICMS) * 100 : 0).ToString("n4", culture)}</pICMS>");
                stXML.Append($"<vICMS>{xmlNotaFiscal.ValorICMS.ToString("n2", culture)}</vICMS>");
            }
            if (xmlNotaFiscal.ValorST > 0)
            {
                stXML.Append("<modBCST>3</modBCST>");
                stXML.Append($"<vBCST>{xmlNotaFiscal.BaseCalculoST.ToString("n2", culture)}</vBCST>");
                stXML.Append("<pICMSST>0.0000</pICMSST>");
                stXML.Append($"<vICMSST>{xmlNotaFiscal.ValorST.ToString("n2", culture)}</vICMSST>");
            }
            stXML.Append("</ICMS90>");
            stXML.Append("</ICMS>");
            if (xmlNotaFiscal.ValorIPI > 0)
            {
                stXML.Append("<IPI>");
                stXML.Append("<cEnq>999</cEnq>");
                stXML.Append("<IPITrib>");
                stXML.Append("<CST>99</CST>");
                stXML.Append("<vBC>0.00</vBC>");
                stXML.Append("<pIPI>0.0000</pIPI>");
                stXML.Append($"<vIPI>{xmlNotaFiscal.ValorIPI.ToString("n2", culture)}</vIPI>");
                stXML.Append("</IPITrib>");
                stXML.Append("</IPI>");
            }
            stXML.Append("<PIS>");
            stXML.Append("<PISOutr>");
            stXML.Append("<CST>99</CST>");
            stXML.Append("<vBC>0.00</vBC>");
            stXML.Append("<pPIS>0.0000</pPIS>");
            stXML.Append($"<vPIS>{xmlNotaFiscal.ValorPIS.ToString("n2", culture)}</vPIS>");
            stXML.Append("</PISOutr>");
            stXML.Append("</PIS>");
            stXML.Append("<COFINS>");
            stXML.Append("<COFINSOutr>");
            stXML.Append("<CST>99</CST>");
            stXML.Append("<vBC>0.00</vBC>");
            stXML.Append("<pCOFINS>0.0000</pCOFINS>");
            stXML.Append($"<vCOFINS>{xmlNotaFiscal.ValorCOFINS.ToString("n2", culture)}</vCOFINS>");
            stXML.Append("</COFINSOutr>");
            stXML.Append("</COFINS>");
            stXML.Append("</imposto>");
            stXML.Append("</det>");

            stXML.Append("<total>");
            stXML.Append("<ICMSTot>");
            stXML.Append($"<vBC>{xmlNotaFiscal.BaseCalculoICMS.ToString("n2", culture)}</vBC>");
            stXML.Append($"<vICMS>{xmlNotaFiscal.ValorICMS.ToString("n2", culture)}</vICMS>");
            stXML.Append("<vICMSDeson>0.00</vICMSDeson>");
            stXML.Append("<vFCPUFDest>0.00</vFCPUFDest>");
            stXML.Append("<vICMSUFDest>0.00</vICMSUFDest>");
            stXML.Append("<vICMSUFRemet>0.00</vICMSUFRemet>");
            stXML.Append("<vFCP>0.00</vFCP>");
            stXML.Append($"<vBCST>{xmlNotaFiscal.BaseCalculoST.ToString("n2", culture)}</vBCST>");
            stXML.Append($"<vST>{xmlNotaFiscal.ValorST.ToString("n2", culture)}</vST>");
            stXML.Append("<vFCPST>0.00</vFCPST>");
            stXML.Append("<vFCPSTRet>0.00</vFCPSTRet>");
            stXML.Append($"<vProd>{xmlNotaFiscal.ValorTotalProdutos.ToString("n2", culture)}</vProd>");
            stXML.Append($"<vFrete>{xmlNotaFiscal.ValorFrete.ToString("n2", culture)}</vFrete>");
            stXML.Append($"<vSeg>{xmlNotaFiscal.ValorSeguro.ToString("n2", culture)}</vSeg>");
            stXML.Append($"<vDesc>{xmlNotaFiscal.ValorDesconto.ToString("n2", culture)}</vDesc>");
            stXML.Append($"<vII>{xmlNotaFiscal.ValorImpostoImportacao.ToString("n2", culture)}</vII>");
            stXML.Append($"<vIPI>{xmlNotaFiscal.ValorIPI.ToString("n2", culture)}</vIPI>");
            stXML.Append("<vIPIDevol>0.00</vIPIDevol>");
            stXML.Append($"<vPIS>{xmlNotaFiscal.ValorPIS.ToString("n2", culture)}</vPIS>");
            stXML.Append($"<vCOFINS>{xmlNotaFiscal.ValorCOFINS.ToString("n2", culture)}</vCOFINS>");
            stXML.Append($"<vOutro>{xmlNotaFiscal.ValorOutros.ToString("n2", culture)}</vOutro>");
            stXML.Append($"<vNF>{xmlNotaFiscal.Valor.ToString("n2", culture)}</vNF>");
            stXML.Append("<vTotTrib>0.00</vTotTrib>");
            stXML.Append("</ICMSTot>");
            stXML.Append("</total>");

            stXML.Append("<transp>");
            stXML.Append("<modFrete>9</modFrete>");
            stXML.Append("</transp>");

            stXML.Append("<pag>");
            stXML.Append("<detPag>");
            stXML.Append("<tPag>99</tPag>");
            stXML.Append($"<vPag>{xmlNotaFiscal.Valor.ToString("n2", culture)}</vPag>");
            stXML.Append("</detPag>");
            stXML.Append("</pag>");

            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.Observacao))
            {
                stXML.Append("<infAdic>");
                stXML.Append($"<infCpl>{xmlNotaFiscal.Observacao}</infCpl>");
                stXML.Append("</infAdic>");
            }

            stXML.Append("</infNFe>");
            stXML.Append("</NFe>");

            stXML.Append("<protNFe versao='4.00'>");
            stXML.Append("<infProt>");
            stXML.Append("<tpAmb>1</tpAmb>");
            stXML.Append("<verAplic>MULTITMS</verAplic>");
            stXML.Append($"<chNFe>{xmlNotaFiscal.Chave}</chNFe>");
            stXML.Append($"<dhRecbto>{xmlNotaFiscal.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss-03:00")}</dhRecbto>");
            stXML.Append($"<nProt>{xmlNotaFiscal.Numero}</nProt>");
            stXML.Append("<digVal>cVZDlpo5r7er5youE9Zu3VrNqAQ=</digVal>");
            stXML.Append("<cStat>100</cStat>");
            stXML.Append("<xMotivo>Autorizado o uso da NF-e</xMotivo>");
            stXML.Append("</infProt>");
            stXML.Append("</protNFe>");

            stXML.Append("</nfeProc>");

            return stXML.ToString();
        }

        private static string ObterXMLCancelamentoNFe(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal)
        {
            Dominio.Entidades.Cliente emitente = xmlNotaFiscal.Emitente;
            Dominio.Entidades.Cliente destinatario = xmlNotaFiscal.Destinatario;

            StringBuilder stXML = new StringBuilder();

            stXML.Append(@"<procEventoNFe versao='1.00' xmlns='http://www.portalfiscal.inf.br/nfe'>");
            stXML.Append("<evento versao='1.00'>");
            stXML.Append($"<infEvento Id='ID110111{xmlNotaFiscal.Chave}01'>");
            stXML.Append($"<cOrgao>{emitente.Localidade.Estado.CodigoIBGE}</cOrgao>");
            stXML.Append("<tpAmb>1</tpAmb>");
            stXML.Append($"<CNPJ>{emitente.CPF_CNPJ_SemFormato}</CNPJ>");
            stXML.Append($"<chNFe>{xmlNotaFiscal.Chave}</chNFe>");
            stXML.Append($"<dhEvento>{xmlNotaFiscal.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss-03:00")}</dhEvento>");
            stXML.Append("<tpEvento>110111</tpEvento>");
            stXML.Append("<nSeqEvento>1</nSeqEvento>");
            stXML.Append("<verEvento>1.00</verEvento>");
            stXML.Append("<detEvento versao='1.00'>");
            stXML.Append("<descEvento>Cancelamento</descEvento>");
            stXML.Append($"<nProt>{xmlNotaFiscal.Numero}</nProt>");
            stXML.Append("<xJust>Cancelamento da nota</xJust>");
            stXML.Append("</detEvento>");
            stXML.Append("</infEvento>");
            stXML.Append("</evento>");

            stXML.Append("<retEvento versao='1.00'>");
            stXML.Append("<infEvento>");
            stXML.Append("<tpAmb>1</tpAmb>");
            stXML.Append("<verAplic>MULTITMS</verAplic>");
            stXML.Append($"<cOrgao>{emitente.Localidade.Estado.CodigoIBGE}</cOrgao>");
            stXML.Append("<cStat>135</cStat>");
            stXML.Append("<xMotivo>Evento registrado e vinculado a NF-e</xMotivo>");
            stXML.Append($"<chNFe>{xmlNotaFiscal.Chave}</chNFe>");
            stXML.Append("<tpEvento>110111</tpEvento>");
            stXML.Append("<nSeqEvento>1</nSeqEvento>");
            stXML.Append($"<CNPJDest>{destinatario.CPF_CNPJ_SemFormato}</CNPJDest>");
            stXML.Append($"<dhRegEvento>{xmlNotaFiscal.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss-03:00")}</dhRegEvento>");
            stXML.Append($"<nProt>{xmlNotaFiscal.Numero}</nProt>");
            stXML.Append("</infEvento>");
            stXML.Append("</retEvento>");

            stXML.Append("</procEventoNFe>");

            return stXML.ToString();
        }

        private static string ObterXMLAutorizacaoATM_Versao200(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool averbarComoEmbarcador, bool averbacaoProvisoria, Repositorio.UnitOfWork unitOfWork)
        {
            string userInfo = "en-US";

            string modelo = "99";
            if (averbacaoProvisoria)
                modelo = "97";
            else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                modelo = "98";
            else if (cte.ModeloDocumentoFiscal.DocumentoTipoCRT)
                modelo = "93";

            StringBuilder stXML = new StringBuilder();


            if (averbacaoProvisoria)
            {
                stXML.Append("<cteProc xmlns=\"http://www.portalfiscal.inf.br/cte\" versao=\"3.00\">");
                stXML.Append("<CTe xmlns=\"http://www.portalfiscal.inf.br/cte\">");
                stXML.Append("<infCte Id=\"" + (cte.NumeroSequencialCRT > 0 ? cte.NumeroSequencialCRT.ToString("D") : cte.Numero.ToString("D")) + "\" versao=\"3.00\"><ide>");
            }
            else
                stXML.Append("<cteProc><CTe><infCte><ide>");

            if (averbacaoProvisoria)
            {
                stXML.Append("<cUF>" + (cte.Empresa?.Localidade?.Estado?.CodigoIBGE.ToString("D") ?? "") + "</cUF>");
                stXML.Append("<cCT>00000000</cCT>");
                stXML.Append("<CFOP>9999</CFOP>");
                stXML.Append("<natOp>Transporte</natOp>");
                stXML.Append("<forPag>2</forPag>");
            }

            stXML.Append("<mod>" + modelo + "</mod>");

            if (averbacaoProvisoria)
                stXML.Append("<serie>1</serie>");
            else
                stXML.Append("<serie>0</serie>");

            stXML.Append("<nCT>" + (cte.NumeroSequencialCRT > 0 ? cte.NumeroSequencialCRT.ToString() : cte.Numero.ToString()) + "</nCT>");
            stXML.Append("<dhEmi>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhEmi>");

            if (averbacaoProvisoria)
            {
                stXML.Append("<tpImp>1</tpImp>");
                stXML.Append("<tpEmis>1</tpEmis>");
                stXML.Append("<cDV>9</cDV>");
            }

            stXML.Append("<tpAmb>" + (int)cte.TipoAmbiente + "</tpAmb>");
            stXML.Append("<tpCTe>" + (int)cte.TipoCTE + "</tpCTe>");

            if (averbacaoProvisoria)
            {
                stXML.Append("<procEmi>0</procEmi>");
                stXML.Append("<verProc>3.00</verProc>");
                stXML.Append("<refCTE>00000000000000000000000000000000000000000000</refCTE>");
                stXML.Append("<cMunEnv>" + (cte.Empresa?.Localidade?.CodigoIBGE.ToString("D") ?? "") + "</cMunEnv>");
                stXML.Append("<xMunEnv>" + (cte.Empresa?.Localidade?.Descricao ?? "") + "</xMunEnv>");
                stXML.Append("<UFEnv>" + (cte.Empresa?.Localidade?.Estado?.Sigla ?? "") + "</UFEnv>");
            }

            stXML.Append("<modal>01</modal>");
            stXML.Append("<tpServ>" + (int)cte.TipoServico + "</tpServ>");
            stXML.Append("<cMunIni>" + cte.LocalidadeInicioPrestacao.CodigoIBGE + "</cMunIni>");
            stXML.Append("<UFIni>" + cte.LocalidadeInicioPrestacao.Estado.Sigla + "</UFIni>");
            stXML.Append("<cMunFim>" + cte.LocalidadeTerminoPrestacao.CodigoIBGE + "</cMunFim>");
            stXML.Append("<UFFim>" + cte.LocalidadeTerminoPrestacao.Estado.Sigla + "</UFFim>");

            if (averbacaoProvisoria)
                stXML.Append("<retira>0</retira>");

            if (averbacaoProvisoria)
            {
                stXML.Append("<toma4>");
                stXML.Append("<toma>4</toma>");
                stXML.Append("<CNPJ>" + (cte.Empresa?.CNPJ_SemFormato ?? "") + "</CNPJ>");
                stXML.Append("<IE>" + (cte.Empresa?.InscricaoEstadual ?? "") + "</IE>");
                stXML.Append("<xNome>" + (cte.Empresa?.RazaoSocial ?? "") + "</xNome>");
                stXML.Append("<xFant>" + (cte.Empresa?.NomeFantasia ?? "") + "</xFant>");
                stXML.Append("<enderToma>");
                stXML.Append("<xLgr>" + (cte.Empresa?.Endereco ?? "") + "</xLgr>");
                stXML.Append("<nro>" + (cte.Empresa?.Numero ?? "") + "</nro>");
                stXML.Append("<xBairro>" + (cte.Empresa?.Bairro ?? "") + "</xBairro>");
                stXML.Append("<cMun>" + (cte.Empresa?.Localidade?.CodigoIBGE.ToString("D") ?? "") + "</cMun>");
                stXML.Append("<xMun>" + (cte.Empresa?.Localidade?.Descricao ?? "") + "</xMun>");
                stXML.Append("<CEP>" + (Utilidades.String.OnlyNumbers(cte.Empresa?.CEP ?? "")) + "</CEP>");
                stXML.Append("<UF>" + (cte.Empresa?.Localidade?.Estado?.Sigla ?? "") + "</UF>");
                stXML.Append("<cPais>1058</cPais>");
                stXML.Append("<xPais>BRASIL</xPais>");
                stXML.Append("</enderToma>");
                stXML.Append("</toma4>");
            }
            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                stXML.Append("<toma4>");
                stXML.Append("<toma>4</toma>");

                if (cte.TomadorPagador.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                    stXML.Append("<CPF>" + cte.TomadorPagador.CPF_CNPJ_SemFormato + "</CPF>");
                else
                    stXML.Append("<CNPJ>" + cte.TomadorPagador.CPF_CNPJ_SemFormato + "</CNPJ>");

                stXML.Append("<enderToma>");

                if (cte.TomadorPagador.Exterior)
                {
                    stXML.Append("<cMun>9999999</cMun>");
                    stXML.Append("<UF>EX</UF>");
                    stXML.Append("<cPais>" + (cte.TomadorPagador.Pais?.Codigo ?? 0) + "</cPais>");
                }
                else
                {
                    stXML.Append("<cMun>" + cte.TomadorPagador.Localidade.CodigoIBGE + "</cMun>");
                    stXML.Append("<UF>" + cte.TomadorPagador.Localidade.Estado.Sigla + "</UF>");
                    stXML.Append("<cPais>" + cte.TomadorPagador.Localidade.Pais.Codigo + "</cPais>");
                }

                stXML.Append("</enderToma>");
                stXML.Append("</toma4>");
            }
            else
                stXML.Append("<toma03><toma>" + (int)cte.TipoTomador + "</toma></toma03>");

            stXML.Append("</ide>");

            StringBuilder stXMLcompl = new StringBuilder();
            // compl
            if (cte.CargaCTes?.Count > 0 && cte.CargaCTes[0]?.Carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ValorContainerAverbacao != null)
            {
                stXMLcompl.Append("<ObsCont xCampo=\"ValorContainer\">");
                stXMLcompl.Append("<xTexto>" + cte.CargaCTes[0]?.Carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ValorContainerAverbacao.ToString("n2") + "</xTexto>");
                stXMLcompl.Append("</ObsCont>");
            }

            if (cte.ValorImpostoSuspenso > 0)
            {
                stXMLcompl.Append("<ObsCont xCampo=\"$BEN\">");
                stXMLcompl.Append("<xTexto>" + cte.ValorImpostoSuspenso.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</xTexto>");
                stXMLcompl.Append("</ObsCont>");
            }

            if (stXMLcompl.Length > 0)
            {
                stXML.Append("<compl>");
                stXML.Append(stXMLcompl.ToString());
                stXML.Append("</compl>");
            }


            stXML.Append("<emit>");
            if (averbarComoEmbarcador)
            {
                stXML.Append("<CNPJ>" + cte.TomadorPagador.CPF_CNPJ_SemFormato + "</CNPJ>");
                stXML.Append("<enderEmit>");
                stXML.Append("<cMun>" + cte.TomadorPagador.Localidade.CodigoIBGE + "</cMun>");
                stXML.Append("<UF>" + cte.TomadorPagador.Localidade.Estado.Sigla + "</UF>");
                stXML.Append("</enderEmit>");
            }
            else
            {
                stXML.Append("<CNPJ>" + cte.Empresa.CNPJ_SemFormato + "</CNPJ>");
                stXML.Append("<enderEmit>");
                stXML.Append("<cMun>" + cte.Empresa.Localidade.CodigoIBGE + "</cMun>");
                stXML.Append("<UF>" + cte.Empresa.Localidade.Estado.Sigla + "</UF>");
                stXML.Append("</enderEmit>");
            }
            stXML.Append("</emit>");

            stXML.Append("<rem>");

            if (cte.Remetente.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                stXML.Append("<CPF>" + cte.Remetente.CPF_CNPJ_SemFormato + "</CPF>");
            else
                stXML.Append("<CNPJ>" + cte.Remetente.CPF_CNPJ_SemFormato + "</CNPJ>");

            stXML.Append("<enderReme>");

            if (cte.Remetente.Exterior)
            {
                stXML.Append("<cMun>9999999</cMun>");
                stXML.Append("<UF>EX</UF>");
                stXML.Append("<cPais>" + (cte.Remetente.Pais?.Codigo ?? 0) + "</cPais>");
            }
            else
            {
                stXML.Append("<cMun>" + cte.Remetente.Localidade.CodigoIBGE + "</cMun>");
                stXML.Append("<UF>" + cte.Remetente.Localidade.Estado.Sigla + "</UF>");
                stXML.Append("<cPais>" + cte.Remetente.Localidade.Pais.Codigo + "</cPais>");
            }

            stXML.Append("</enderReme>");
            stXML.Append("</rem>");

            stXML.Append("<dest>");

            if (cte.Destinatario.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                stXML.Append("<CPF>" + cte.Destinatario.CPF_CNPJ_SemFormato + "</CPF>");
            else
                stXML.Append("<CNPJ>" + cte.Destinatario.CPF_CNPJ_SemFormato + "</CNPJ>");

            stXML.Append("<enderDest>");

            if (cte.Destinatario.Exterior)
            {
                stXML.Append("<cMun>9999999</cMun>");
                stXML.Append("<UF>EX</UF>");
                stXML.Append("<cPais>" + (cte.Destinatario.Pais?.Codigo ?? 0) + "</cPais>");
            }
            else
            {
                stXML.Append("<cMun>" + cte.Destinatario.Localidade.CodigoIBGE + "</cMun>");
                stXML.Append("<UF>" + cte.Destinatario.Localidade.Estado.Sigla + "</UF>");
                stXML.Append("<cPais>" + cte.Destinatario.Localidade.Pais.Codigo + "</cPais>");
            }

            stXML.Append("</enderDest>");
            stXML.Append("</dest>");

            if (cte.Expedidor != null)
            {
                stXML.Append("<exped>");

                if (cte.Expedidor.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                    stXML.Append("<CPF>" + cte.Expedidor.CPF_CNPJ_SemFormato + "</CPF>");
                else
                    stXML.Append("<CNPJ>" + cte.Expedidor.CPF_CNPJ_SemFormato + "</CNPJ>");

                stXML.Append("<enderExped>");

                if (cte.Expedidor.Exterior)
                {
                    stXML.Append("<cMun>9999999</cMun>");
                    stXML.Append("<UF>EX</UF>");
                    stXML.Append("<cPais>" + (cte.Expedidor.Pais?.Codigo ?? 0) + "</cPais>");
                }
                else
                {
                    stXML.Append("<cMun>" + cte.Expedidor.Localidade.CodigoIBGE + "</cMun>");
                    stXML.Append("<UF>" + cte.Expedidor.Localidade.Estado.Sigla + "</UF>");
                    stXML.Append("<cPais>" + cte.Expedidor.Localidade.Pais.Codigo + "</cPais>");
                }

                stXML.Append("</enderExped>");
                stXML.Append("</exped>");
            }

            if (cte.Recebedor != null)
            {
                stXML.Append("<receb>");

                if (cte.Recebedor.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                    stXML.Append("<CPF>" + cte.Recebedor.CPF_CNPJ_SemFormato + "</CPF>");
                else
                    stXML.Append("<CNPJ>" + cte.Recebedor.CPF_CNPJ_SemFormato + "</CNPJ>");

                stXML.Append("<enderReceb>");

                if (cte.Recebedor.Exterior)
                {
                    stXML.Append("<cMun>9999999</cMun>");
                    stXML.Append("<UF>EX</UF>");
                    stXML.Append("<cPais>" + (cte.Recebedor.Pais?.Codigo ?? 0) + "</cPais>");
                }
                else
                {
                    stXML.Append("<cMun>" + cte.Recebedor.Localidade.CodigoIBGE + "</cMun>");
                    stXML.Append("<UF>" + cte.Recebedor.Localidade.Estado.Sigla + "</UF>");
                    stXML.Append("<cPais>" + cte.Recebedor.Localidade.Pais.Codigo + "</cPais>");
                }

                stXML.Append("</enderReceb>");
                stXML.Append("</receb>");
            }

            stXML.Append("<infCTeNorm><infCarga>");

            stXML.Append("<vCarga>" + CalcularValorCarga(cte).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</vCarga>");
            if (averbacaoProvisoria)
            {
                stXML.Append("<proPred>" + cte.ProdutoPredominante + "</proPred>");
                if (cte.QuantidadesCarga != null && cte.QuantidadesCarga.Count > 0)
                {
                    foreach (var volume in cte.QuantidadesCarga)
                    {
                        stXML.Append("<infQ>");
                        stXML.Append("<cUnid>" + volume.UnidadeMedida + "</cUnid>");
                        stXML.Append("<tpMed>" + volume.Tipo + "</tpMed>");
                        stXML.Append("<qCarga>" + volume.Quantidade.ToString("N4", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</qCarga>");
                        stXML.Append("</infQ>");
                    }
                }
            }

            stXML.Append("</infCarga>");

            if (cte.Seguros.Count > 0)
            {
                Dominio.Entidades.SeguroCTE seguro = cte.Seguros.FirstOrDefault();
                stXML.Append("<seg>");
                stXML.Append("<respSeg>" + (int)seguro.Tipo + "</respSeg>");
                stXML.Append("<vCarga>" + CalcularValorTotalMercadoria(cte, seguro.Valor, true, unitOfWork).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</vCarga>");
                stXML.Append("</seg>");
            }

            stXML.Append("</infCTeNorm>");
            stXML.Append("</infCte></CTe>");
            if (averbacaoProvisoria)
            {
                stXML.Append("<protCTe xmlns=\"http://www.portalfiscal.inf.br/cte\" versao=\"3.00\">");
                stXML.Append("<infProt Id=\"CTe" + (cte.NumeroSequencialCRT > 0 ? cte.NumeroSequencialCRT.ToString("D") : cte.Numero.ToString("D")) + "\">");
                stXML.Append("<tpAmb>1</tpAmb>");
                stXML.Append("<verAplic>MULTITMS</verAplic>");
                stXML.Append("<chCTe>" + (cte.NumeroSequencialCRT > 0 ? cte.NumeroSequencialCRT.ToString("D") : cte.Numero.ToString("D")) + "</chCTe>");
                stXML.Append("<dhRecbto>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhRecbto>");
                stXML.Append("<nProt>" + (cte.NumeroSequencialCRT > 0 ? cte.NumeroSequencialCRT.ToString("D") : cte.Numero.ToString("D")) + "</nProt>");
                stXML.Append("<digVal>MFbZGM/wM5sXrPHX51zBzATM8lY=</digVal>");
                stXML.Append("<cStat>100</cStat>");
                stXML.Append("<xMotivo>Autorizado o uso do CT-e</xMotivo>");
                stXML.Append("</infProt>");
                stXML.Append("</protCTe>");
            }
            stXML.Append("</cteProc>");

            return stXML.ToString();
        }

        private static string ObterXMLAutorizacaoATM_Versao400(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool averbarComoEmbarcador, bool averbacaoProvisoria, Repositorio.UnitOfWork unitOfWork)
        {
            string userInfo = "en-US";

            string modelo = "99";
            if (averbacaoProvisoria)
                modelo = "97";
            else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                modelo = "98";
            else if (cte.ModeloDocumentoFiscal.DocumentoTipoCRT)
                modelo = "93";

            StringBuilder stXML = new StringBuilder();

            stXML.Append("<cteProc versao=\"4.00\" xmlns=\"http://www.portalfiscal.inf.br/cte\">");
            stXML.Append("<CTe xmlns=\"http://www.portalfiscal.inf.br/cte\">");

            if (averbacaoProvisoria)
                stXML.Append("<infCte versao=\"4.00\" Id=\"" + (cte.NumeroSequencialCRT > 0 ? cte.NumeroSequencialCRT.ToString("D") : cte.Numero.ToString("D")) + "\">");
            else
                stXML.Append("<infCte versao=\"4.00\" Id=\"CTe00000000000000000000000000000000000000000000\">");

            stXML.Append("<ide>");

            if (averbacaoProvisoria)
            {
                stXML.Append("<cUF>" + (cte.Empresa?.Localidade?.Estado?.CodigoIBGE.ToString("D") ?? "") + "</cUF>");
                stXML.Append("<cCT>00000000</cCT>");
                stXML.Append("<CFOP>9999</CFOP>");
                stXML.Append("<natOp>Transporte</natOp>");
            }

            stXML.Append("<mod>" + modelo + "</mod>");

            if (averbacaoProvisoria)
                stXML.Append("<serie>1</serie>");
            else
                stXML.Append("<serie>0</serie>");

            stXML.Append("<nCT>" + (cte.NumeroSequencialCRT > 0 ? cte.NumeroSequencialCRT.ToString() : cte.Numero.ToString()) + "</nCT>");
            stXML.Append("<dhEmi>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhEmi>");

            if (averbacaoProvisoria)
            {
                stXML.Append("<tpImp>1</tpImp>");
                stXML.Append("<tpEmis>1</tpEmis>");
                stXML.Append("<cDV>9</cDV>");
            }

            stXML.Append("<tpAmb>" + (int)cte.TipoAmbiente + "</tpAmb>");
            stXML.Append("<tpCTe>" + (int)cte.TipoCTE + "</tpCTe>");

            if (averbacaoProvisoria)
            {
                stXML.Append("<procEmi>0</procEmi>");
                stXML.Append("<verProc>3.00</verProc>");
                stXML.Append("<refCTE>00000000000000000000000000000000000000000000</refCTE>");
                stXML.Append("<cMunEnv>" + (cte.Empresa?.Localidade?.CodigoIBGE.ToString("D") ?? "") + "</cMunEnv>");
                stXML.Append("<xMunEnv>" + (cte.Empresa?.Localidade?.Descricao ?? "") + "</xMunEnv>");
                stXML.Append("<UFEnv>" + (cte.Empresa?.Localidade?.Estado?.Sigla ?? "") + "</UFEnv>");
            }

            stXML.Append("<modal>01</modal>");
            stXML.Append("<tpServ>" + (int)cte.TipoServico + "</tpServ>");
            stXML.Append("<cMunIni>" + cte.LocalidadeInicioPrestacao.CodigoIBGE + "</cMunIni>");
            stXML.Append("<UFIni>" + cte.LocalidadeInicioPrestacao.Estado.Sigla + "</UFIni>");
            stXML.Append("<cMunFim>" + cte.LocalidadeTerminoPrestacao.CodigoIBGE + "</cMunFim>");
            stXML.Append("<UFFim>" + cte.LocalidadeTerminoPrestacao.Estado.Sigla + "</UFFim>");

            if (averbacaoProvisoria)
                stXML.Append("<retira>0</retira>");

            if (averbacaoProvisoria)
            {
                stXML.Append("<toma4>");
                stXML.Append("<toma>4</toma>");
                stXML.Append("<CNPJ>" + (cte.Empresa?.CNPJ_SemFormato ?? "") + "</CNPJ>");
                stXML.Append("<IE>" + (cte.Empresa?.InscricaoEstadual ?? "") + "</IE>");
                stXML.Append("<xNome>" + (cte.Empresa?.RazaoSocial ?? "") + "</xNome>");
                stXML.Append("<xFant>" + (cte.Empresa?.NomeFantasia ?? "") + "</xFant>");
                stXML.Append("<enderToma>");
                stXML.Append("<xLgr>" + (cte.Empresa?.Endereco ?? "") + "</xLgr>");
                stXML.Append("<nro>" + (cte.Empresa?.Numero ?? "") + "</nro>");
                stXML.Append("<xBairro>" + (cte.Empresa?.Bairro ?? "") + "</xBairro>");
                stXML.Append("<cMun>" + (cte.Empresa?.Localidade?.CodigoIBGE.ToString("D") ?? "") + "</cMun>");
                stXML.Append("<xMun>" + (cte.Empresa?.Localidade?.Descricao ?? "") + "</xMun>");
                stXML.Append("<CEP>" + (Utilidades.String.OnlyNumbers(cte.Empresa?.CEP ?? "")) + "</CEP>");
                stXML.Append("<UF>" + (cte.Empresa?.Localidade?.Estado?.Sigla ?? "") + "</UF>");
                stXML.Append("<cPais>1058</cPais>");
                stXML.Append("<xPais>BRASIL</xPais>");
                stXML.Append("</enderToma>");
                stXML.Append("</toma4>");
            }
            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                stXML.Append("<toma4>");
                stXML.Append("<toma>4</toma>");

                if (cte.TomadorPagador.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                    stXML.Append("<CPF>" + cte.TomadorPagador.CPF_CNPJ_SemFormato + "</CPF>");
                else
                    stXML.Append("<CNPJ>" + cte.TomadorPagador.CPF_CNPJ_SemFormato + "</CNPJ>");

                stXML.Append("<enderToma>");

                if (cte.TomadorPagador.Exterior)
                {
                    stXML.Append("<cMun>9999999</cMun>");
                    stXML.Append("<UF>EX</UF>");
                    stXML.Append("<cPais>" + (cte.TomadorPagador.Pais?.Codigo ?? 0) + "</cPais>");
                }
                else
                {
                    stXML.Append("<cMun>" + cte.TomadorPagador.Localidade.CodigoIBGE + "</cMun>");
                    stXML.Append("<UF>" + cte.TomadorPagador.Localidade.Estado.Sigla + "</UF>");
                    stXML.Append("<cPais>" + cte.TomadorPagador.Localidade.Pais.Codigo + "</cPais>");
                }

                stXML.Append("</enderToma>");
                stXML.Append("</toma4>");
            }
            else
                stXML.Append("<toma03><toma>" + (int)cte.TipoTomador + "</toma></toma03>");

            stXML.Append("</ide>");

            StringBuilder stXMLcompl = new StringBuilder();
            // compl
            if (cte.CargaCTes?.Count > 0 && cte.CargaCTes[0]?.Carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ValorContainerAverbacao != null)
            {
                stXMLcompl.Append("<ObsCont xCampo=\"ValorContainer\">");
                stXMLcompl.Append("<xTexto>" + cte.CargaCTes[0]?.Carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ValorContainerAverbacao.ToString("n2") + "</xTexto>");
                stXMLcompl.Append("</ObsCont>");
            }

            if (cte.ValorImpostoSuspenso > 0)
            {
                stXMLcompl.Append("<ObsCont xCampo=\"$BEN\">");
                stXMLcompl.Append("<xTexto>" + cte.ValorImpostoSuspenso.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</xTexto>");
                stXMLcompl.Append("</ObsCont>");
            }

            Dominio.Entidades.SeguroCTE seguro = null;
            if (cte.Seguros.Count > 0)
                seguro = cte.Seguros.FirstOrDefault();

            string cnpjCpfCodigoResponsavelSeguro = ObterResponsavelSeguro(cte, seguro);
            if (seguro != null && !string.IsNullOrEmpty(cnpjCpfCodigoResponsavelSeguro))
            {
                stXMLcompl.Append("<ObsCont xCampo=\"RESPSEG\">");
                stXMLcompl.Append("<xTexto>" + cnpjCpfCodigoResponsavelSeguro + "</xTexto>");
                stXMLcompl.Append("</ObsCont>");
            }

            if (stXMLcompl.Length > 0)
            {
                stXML.Append("<compl>");
                stXML.Append(stXMLcompl.ToString());
                stXML.Append("</compl>");
            }


            stXML.Append("<emit>");
            if (averbarComoEmbarcador)
            {
                stXML.Append("<CNPJ>" + cte.TomadorPagador.CPF_CNPJ_SemFormato + "</CNPJ>");
                stXML.Append("<enderEmit>");
                stXML.Append("<cMun>" + cte.TomadorPagador.Localidade.CodigoIBGE + "</cMun>");
                stXML.Append("<UF>" + cte.TomadorPagador.Localidade.Estado.Sigla + "</UF>");
                stXML.Append("</enderEmit>");
            }
            else
            {
                if (!string.IsNullOrEmpty(cte.Empresa.CNPJ_SemFormato))
                    stXML.Append("<CNPJ>" + cte.Empresa.CNPJ_SemFormato + "</CNPJ>");
                stXML.Append("<enderEmit>");
                stXML.Append("<cMun>" + cte.Empresa.Localidade.CodigoIBGE + "</cMun>");
                stXML.Append("<UF>" + cte.Empresa.Localidade.Estado.Sigla + "</UF>");
                stXML.Append("</enderEmit>");
            }
            stXML.Append("</emit>");

            stXML.Append("<rem>");

            if (cte.Remetente.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                stXML.Append("<CPF>" + cte.Remetente.CPF_CNPJ_SemFormato + "</CPF>");
            else
                stXML.Append("<CNPJ>" + cte.Remetente.CPF_CNPJ_SemFormato + "</CNPJ>");

            stXML.Append("<enderReme>");

            if (cte.Remetente.Exterior)
            {
                stXML.Append("<cMun>9999999</cMun>");
                stXML.Append("<UF>EX</UF>");
                stXML.Append("<cPais>" + (cte.Remetente.Pais?.Codigo ?? 0) + "</cPais>");
            }
            else
            {
                stXML.Append("<cMun>" + cte.Remetente.Localidade.CodigoIBGE + "</cMun>");
                stXML.Append("<UF>" + cte.Remetente.Localidade.Estado.Sigla + "</UF>");
                stXML.Append("<cPais>" + cte.Remetente.Localidade.Pais.Codigo + "</cPais>");
            }

            stXML.Append("</enderReme>");
            stXML.Append("</rem>");

            stXML.Append("<dest>");

            if (cte.Destinatario.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                stXML.Append("<CPF>" + cte.Destinatario.CPF_CNPJ_SemFormato + "</CPF>");
            else
                stXML.Append("<CNPJ>" + cte.Destinatario.CPF_CNPJ_SemFormato + "</CNPJ>");

            stXML.Append("<enderDest>");

            if (cte.Destinatario.Exterior)
            {
                stXML.Append("<cMun>9999999</cMun>");
                stXML.Append("<UF>EX</UF>");
                stXML.Append("<cPais>" + (cte.Destinatario.Pais?.Codigo ?? 0) + "</cPais>");
            }
            else
            {
                stXML.Append("<cMun>" + cte.Destinatario.Localidade.CodigoIBGE + "</cMun>");
                stXML.Append("<UF>" + cte.Destinatario.Localidade.Estado.Sigla + "</UF>");
                stXML.Append("<cPais>" + cte.Destinatario.Localidade.Pais.Codigo + "</cPais>");
            }

            stXML.Append("</enderDest>");
            stXML.Append("</dest>");

            if (cte.Expedidor != null)
            {
                stXML.Append("<exped>");

                if (cte.Expedidor.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                    stXML.Append("<CPF>" + cte.Expedidor.CPF_CNPJ_SemFormato + "</CPF>");
                else
                    stXML.Append("<CNPJ>" + cte.Expedidor.CPF_CNPJ_SemFormato + "</CNPJ>");

                stXML.Append("<enderExped>");

                if (cte.Expedidor.Exterior)
                {
                    stXML.Append("<cMun>9999999</cMun>");
                    stXML.Append("<UF>EX</UF>");
                    stXML.Append("<cPais>" + (cte.Expedidor.Pais?.Codigo ?? 0) + "</cPais>");
                }
                else
                {
                    stXML.Append("<cMun>" + cte.Expedidor.Localidade.CodigoIBGE + "</cMun>");
                    stXML.Append("<UF>" + cte.Expedidor.Localidade.Estado.Sigla + "</UF>");
                    stXML.Append("<cPais>" + cte.Expedidor.Localidade.Pais.Codigo + "</cPais>");
                }

                stXML.Append("</enderExped>");
                stXML.Append("</exped>");
            }

            if (cte.Recebedor != null)
            {
                stXML.Append("<receb>");

                if (cte.Recebedor.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                    stXML.Append("<CPF>" + cte.Recebedor.CPF_CNPJ_SemFormato + "</CPF>");
                else
                    stXML.Append("<CNPJ>" + cte.Recebedor.CPF_CNPJ_SemFormato + "</CNPJ>");

                stXML.Append("<enderReceb>");

                if (cte.Recebedor.Exterior)
                {
                    stXML.Append("<cMun>9999999</cMun>");
                    stXML.Append("<UF>EX</UF>");
                    stXML.Append("<cPais>" + (cte.Recebedor.Pais?.Codigo ?? 0) + "</cPais>");
                }
                else
                {
                    stXML.Append("<cMun>" + cte.Recebedor.Localidade.CodigoIBGE + "</cMun>");
                    stXML.Append("<UF>" + cte.Recebedor.Localidade.Estado.Sigla + "</UF>");
                    stXML.Append("<cPais>" + cte.Recebedor.Localidade.Pais.Codigo + "</cPais>");
                }

                stXML.Append("</enderReceb>");
                stXML.Append("</receb>");
            }

            stXML.Append("<infCTeNorm><infCarga>");

            stXML.Append("<vCarga>" + CalcularValorCarga(cte).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</vCarga>");

            if (averbacaoProvisoria)
            {
                stXML.Append("<proPred>" + cte.ProdutoPredominante + "</proPred>");
                if (cte.QuantidadesCarga != null && cte.QuantidadesCarga.Count > 0)
                {
                    foreach (var volume in cte.QuantidadesCarga)
                    {
                        stXML.Append("<infQ>");
                        stXML.Append("<cUnid>" + volume.UnidadeMedida + "</cUnid>");
                        stXML.Append("<tpMed>" + volume.Tipo + "</tpMed>");
                        stXML.Append("<qCarga>" + volume.Quantidade.ToString("N4", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</qCarga>");
                        stXML.Append("</infQ>");
                    }
                }
            }

            if (seguro != null)
            {
                stXML.Append("<vCargaAverb>");
                stXML.Append(CalcularValorTotalMercadoria(cte, 0m, false, unitOfWork).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", ""));
                stXML.Append("</vCargaAverb>");
            }

            stXML.Append("</infCarga>");
            stXML.Append("</infCTeNorm>");
            stXML.Append("</infCte></CTe>");

            if (averbacaoProvisoria)
            {
                stXML.Append("<protCTe xmlns=\"http://www.portalfiscal.inf.br/cte\" versao=\"3.00\">");
                stXML.Append("<infProt Id=\"CTe" + (cte.NumeroSequencialCRT > 0 ? cte.NumeroSequencialCRT.ToString("D") : cte.Numero.ToString("D")) + "\">");
                stXML.Append("<tpAmb>1</tpAmb>");
                stXML.Append("<verAplic>MULTITMS</verAplic>");
                stXML.Append("<chCTe>" + (cte.NumeroSequencialCRT > 0 ? cte.NumeroSequencialCRT.ToString("D") : cte.Numero.ToString("D")) + "</chCTe>");
                stXML.Append("<dhRecbto>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhRecbto>");
                stXML.Append("<nProt>" + (cte.NumeroSequencialCRT > 0 ? cte.NumeroSequencialCRT.ToString("D") : cte.Numero.ToString("D")) + "</nProt>");
                stXML.Append("<digVal>MFbZGM/wM5sXrPHX51zBzATM8lY=</digVal>");
                stXML.Append("<cStat>100</cStat>");
                stXML.Append("<xMotivo>Autorizado o uso do CT-e</xMotivo>");
                stXML.Append("</infProt>");
                stXML.Append("</protCTe>");
            }

            stXML.Append("</cteProc>");

            return stXML.ToString();
        }

        private static string ObterResponsavelSeguro(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.SeguroCTE seguro)
        {
            string retorno = string.Empty;

            switch (seguro.Tipo)
            {
                case Dominio.Enumeradores.TipoSeguro.Remetente:
                    retorno = cte.Remetente.CPF_CNPJ_SemFormato;
                    break;
                case Dominio.Enumeradores.TipoSeguro.Expedidor:
                    retorno = cte.Expedidor.CPF_CNPJ_SemFormato;
                    break;
                case Dominio.Enumeradores.TipoSeguro.Recebedor:
                    retorno = cte.Recebedor.CPF_CNPJ_SemFormato;
                    break;
                case Dominio.Enumeradores.TipoSeguro.Destinatario:
                    retorno = cte.Destinatario.CPF_CNPJ_SemFormato;
                    break;
                case Dominio.Enumeradores.TipoSeguro.Emitente_CTE:
                    retorno = cte.Empresa.CNPJ;
                    break;
                case Dominio.Enumeradores.TipoSeguro.Tomador_Servico:
                    retorno = cte.Tomador.CPF_CNPJ_SemFormato;
                    break;
                default:
                    retorno = cte.Tomador.CPF_CNPJ_SemFormato;
                    break;
            }

            return retorno;
        }

        private static decimal CalcularValorTotalMercadoria(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, decimal valorSeguro, bool versao200, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            if (cte.ModeloDocumentoFiscal.DocumentoTipoCRT)
            {
                Dominio.Enumeradores.TipoPagamento tipoPagamento = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarTipoPagamentoPedidoPorCTe(cte.Codigo);

                decimal valorMercadoria = CalcularValorMercadoria(cte);

                if (tipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                {
                    decimal valorFrete = cte.ValorTotalMoeda ?? 0m;
                    return valorMercadoria + valorFrete;
                }
                else if (tipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                    return valorMercadoria;

                return 0m;
            }

            if (versao200)
                return (valorSeguro + (cte.ModeloDocumentoFiscal.DocumentoTipoCRT && cte.ValorImpostoSuspenso > 0 ? cte.ValorImpostoSuspenso : 0));

            decimal valorCommodities = 0m;

            if (cte.ValorCommodities > 0)
                valorCommodities = cte.ValorCommodities;
            else if (cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0)
                valorCommodities = cte.XMLNotaFiscais.FirstOrDefault().ValorCommodities;

            return cte.ValorTotalMercadoria + valorCommodities;
        }

        private static decimal CalcularValorCarga(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte.ModeloDocumentoFiscal.DocumentoTipoCRT)
                return CalcularValorMercadoria(cte);

            return cte.ValorTotalMercadoria;
        }

        private static decimal CalcularValorMercadoria(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            decimal valorCotacaoMoeda = cte.ValorCotacaoMoeda.HasValue && cte.ValorCotacaoMoeda.Value > 0m ? cte.ValorCotacaoMoeda.Value : 1m;
            return cte.ValorTotalMercadoria / valorCotacaoMoeda;
        }

        #endregion
    }
}

