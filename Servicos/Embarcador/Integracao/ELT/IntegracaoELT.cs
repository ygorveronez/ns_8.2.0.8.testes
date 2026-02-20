using System;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Integracao.ELT
{
    public class IntegracaoELT
    {
        public static void CancelarAverbacaoDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);

            ServicoELT.ELTAverbaServiceClient svcAverbacaoELT = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoELT.ELTAverbaServiceClient, ServicoELT.IELTAverbaService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.ELT_Averba, out Servicos.Models.Integracao.InspectorBehavior inspector);
            
            System.IO.Stream xml = ObterXMLCancelamentoELT(averbacao.CTe, averbacao.Protocolo, unitOfWork);

            string cnpjSegurado = averbacao.CTe.Empresa.CNPJ_SemFormato;

            svcAverbacaoELT.FileUpload(ref cnpjSegurado, averbacao.CTe.Chave + ".xml", xml.Length, xml, out string codigoRetorno, out DateTime dataHoraAverbacao, out string protocoloAverbacao, out string mensagemRetorno, out string fileName, out string statusRetorno);

            if (codigoRetorno == "1")
            {
                averbacao.MensagemRetorno = mensagemRetorno;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
            }
            else
            {
                averbacao.CodigoRetorno = codigoRetorno;
                averbacao.MensagemRetorno = mensagemRetorno;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
            }

            averbacao.DataRetorno = DateTime.Now;

            if (averbacao.ArquivosTransacaoCancelamento != null)
            {
                Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                    Data = DateTime.Now,
                    Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
                };

                repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);

                averbacao.ArquivosTransacaoCancelamento.Add(averbacaoIntegracaoArquivo);
            }
            else
            {
                Servicos.Log.TratarErro(inspector.LastRequestXML, "CancelarAverbacaoDocumentoELT");
                Servicos.Log.TratarErro(inspector.LastResponseXML, "CancelarAverbacaoDocumentoELT");
            }

            repAverbacaoCTe.Atualizar(averbacao);
        }

        public static void AverbarDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);

            ServicoELT.ELTAverbaServiceClient svcAverbacaoELT = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoELT.ELTAverbaServiceClient, ServicoELT.IELTAverbaService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.ELT_Averba, out Servicos.Models.Integracao.InspectorBehavior inspector);

            System.IO.Stream xml = ObterXMLAutorizacaoELT(averbacao.CTe, unitOfWork);

            string cnpjSegurado = averbacao.CTe.Empresa.CNPJ_SemFormato;

            svcAverbacaoELT.FileUpload(ref cnpjSegurado, averbacao.CTe.Chave + ".xml", xml.Length, xml, out string codigoRetorno, out DateTime dataHoraAverbacao, out string protocoloAverbacao, out string mensagemRetorno, out string fileName, out string statusRetorno);

            if (codigoRetorno == "0")
            {
                averbacao.Protocolo = protocoloAverbacao;
                averbacao.Averbacao = protocoloAverbacao;
                averbacao.MensagemRetorno = mensagemRetorno;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
            }
            else
            {
                averbacao.CodigoRetorno = codigoRetorno;
                averbacao.MensagemRetorno = mensagemRetorno;
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
            }

            averbacao.DataRetorno = dataHoraAverbacao;

            if (averbacao.ArquivosTransacao != null)
            {

                Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                    Data = DateTime.Now,
                    Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
                };

                repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);

                averbacao.ArquivosTransacao.Add(averbacaoIntegracaoArquivo);               
            }
            else
            {
                Servicos.Log.TratarErro(inspector.LastRequestXML, "AverbarDocumentoELT");
                Servicos.Log.TratarErro(inspector.LastResponseXML, "AverbarDocumentoELT");
            }

            repAverbacaoCTe.Atualizar(averbacao);
        }

        private static System.IO.Stream ObterXMLCancelamentoELT(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string protocolo, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe)
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

                string xml = stXML.ToString();

                return Utilidades.String.ToStream(xml);
            }
            else
            {
                Servicos.CTe svcCTE = new Servicos.CTe(unitOfWork);
                return new System.IO.MemoryStream(svcCTE.ObterXMLCancelamento(cte, unitOfWork));
            }
        }

        private static System.IO.Stream ObterXMLAutorizacaoELT(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe)
            {
                string modelo = "99";
                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    modelo = "98";

                StringBuilder stXML = new StringBuilder();
                stXML.Append("<cteProc><CTe><infCte><ide>");
                stXML.Append("<mod>" + modelo + "</mod>");
                stXML.Append("<serie>u</serie>");
                stXML.Append("<nCT>" + cte.Numero + "</nCT>");
                stXML.Append("<dhEmi>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhEmi>");
                stXML.Append("<tpAmb>" + (int)cte.TipoAmbiente + "</tpAmb>");
                stXML.Append("<tpCTe>" + (int)cte.TipoCTE + "</tpCTe>");
                stXML.Append("<modal>01</modal>");
                stXML.Append("<tpServ>" + (int)cte.TipoServico + "</tpServ>");
                stXML.Append("<cMunIni>" + cte.LocalidadeInicioPrestacao.CodigoIBGE + "</cMunIni>");
                stXML.Append("<UFIni>" + cte.LocalidadeInicioPrestacao.Estado.Sigla + "</UFIni>");
                stXML.Append("<cMunFim>" + cte.LocalidadeTerminoPrestacao.CodigoIBGE + "</cMunFim>");
                stXML.Append("<UFFim>" + cte.LocalidadeTerminoPrestacao.Estado.Sigla + "</UFFim>");

                if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                {
                    stXML.Append("<toma4>");
                    stXML.Append("<toma>4</toma>");
                    stXML.Append("<CNPJ>" + cte.TomadorPagador.CPF_CNPJ_SemFormato + "</CNPJ>");
                    stXML.Append("<enderToma>");

                    if (cte.TomadorPagador.Exterior)
                    {
                        stXML.Append("<cMun>9999999</cMun>");
                        stXML.Append("<UF>EX</UF>");
                        stXML.Append("<cPais>" + cte.TomadorPagador.Pais.Codigo + "</cPais>");
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
                    stXML.Append("<toma3><toma>" + (int)cte.TipoTomador + "</toma></toma3>");

                stXML.Append("</ide>");

                stXML.Append("<emit>");

                stXML.Append("<CNPJ>" + cte.Empresa.CNPJ_SemFormato + "</CNPJ>");
                stXML.Append("<enderEmit>");
                stXML.Append("<cMun>" + cte.Empresa.Localidade.CodigoIBGE + "</cMun>");
                stXML.Append("<UF>" + cte.Empresa.Localidade.Estado.Sigla + "</UF>");
                stXML.Append("</enderEmit>");

                stXML.Append("</emit>");

                stXML.Append("<rem>");
                stXML.Append("<CNPJ>" + cte.Remetente.CPF_CNPJ_SemFormato + "</CNPJ>");
                stXML.Append("<enderReme>");

                if (cte.Remetente.Exterior)
                {
                    stXML.Append("<cMun>9999999</cMun>");
                    stXML.Append("<UF>EX</UF>");
                    stXML.Append("<cPais>" + cte.Remetente.Pais.Codigo + "</cPais>");
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
                stXML.Append("<CNPJ>" + cte.Destinatario.CPF_CNPJ_SemFormato + "</CNPJ>");
                stXML.Append("<enderDest>");

                if (cte.Destinatario.Exterior)
                {
                    stXML.Append("<cMun>9999999</cMun>");
                    stXML.Append("<UF>EX</UF>");
                    stXML.Append("<cPais>" + cte.Destinatario.Pais.Codigo + "</cPais>");
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
                    stXML.Append("<CNPJ>" + cte.Expedidor.CPF_CNPJ_SemFormato + "</CNPJ>");
                    stXML.Append("<enderExped>");

                    if (cte.Expedidor.Exterior)
                    {
                        stXML.Append("<cMun>9999999</cMun>");
                        stXML.Append("<UF>EX</UF>");
                        stXML.Append("<cPais>" + cte.Expedidor.Pais.Codigo + "</cPais>");
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
                    stXML.Append("<CNPJ>" + cte.Recebedor.CPF_CNPJ_SemFormato + "</CNPJ>");
                    stXML.Append("<enderReceb>");

                    if (cte.Recebedor.Exterior)
                    {
                        stXML.Append("<cMun>9999999</cMun>");
                        stXML.Append("<UF>EX</UF>");
                        stXML.Append("<cPais>" + cte.Recebedor.Pais.Codigo + "</cPais>");
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

                string userInfo = "en-US";
                stXML.Append("<vCarga>" + cte.ValorTotalMercadoria.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</vCarga>");

                stXML.Append("</infCarga>");

                if (cte.Seguros.Count > 0)
                {
                    Dominio.Entidades.SeguroCTE seguro = cte.Seguros.FirstOrDefault();
                    stXML.Append("<seg>");
                    stXML.Append("<respSeg>" + (int)seguro.Tipo + "</respSeg>");
                    stXML.Append("<vCarga>" + seguro.Valor.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</vCarga>");
                    stXML.Append("</seg>");
                }

                stXML.Append("</infCTeNorm>");
                stXML.Append("</infCte></CTe></cteProc>");

                return Utilidades.String.ToStream(stXML.ToString());
            }
            else
            {
                Servicos.CTe svcCTE = new Servicos.CTe(unitOfWork);
                return new System.IO.MemoryStream(svcCTE.ObterXMLAutorizacao(cte, unitOfWork));
            }
        }
    }
}
