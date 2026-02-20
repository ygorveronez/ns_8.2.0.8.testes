using Dominio.ObjetosDeValor.Embarcador.Integracao.Senig;
using Servicos.ServicoSenig;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.Senig
{
    public sealed class AverbacaoSenig
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public AverbacaoSenig(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private RetornoAverbacao ConverterRetornoSenig(string retornoXMLWebService)
        {
            string conteudoTagReturn = Utilidades.XML.ObterConteudoTag(retornoXMLWebService, tag: "return", incluirTagNaBusca: true);

            if (string.IsNullOrWhiteSpace(conteudoTagReturn))
                return null;

            XmlSerializer serializer = new XmlSerializer(typeof(RetornoAverbacao));
            TextReader reader = new StringReader(conteudoTagReturn);
            return (RetornoAverbacao)serializer.Deserialize(reader);
        }

        private string EnviarAverbacaoSenig(Dominio.Entidades.AverbacaoCTe averbacao, bool averbarComoEmbarcador, Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            string xmlCTe = ObterXMLAutorizacaoCTe(averbacao.CTe, averbarComoEmbarcador, averbacao.Forma == Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria);
            string cnpjTransportador = averbacao.CTe.Empresa.CNPJ_SemFormato;
            string numeroCTe = averbacao.CTe.Numero.ToString();

            ServicoSenig.WebAppAverbaClient webClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoSenig.WebAppAverbaClient, ServicoSenig.IWebAppAverba>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Senig_WebAppAverba, out inspector);

            string retorno = webClient.EnviaXMLRet(xmlCTe, cnpjTransportador, numeroCTe);

            webClient.Close();

            return retorno;
        }

        private string EnviarCancelamentoAverbacaoSenig(Dominio.Entidades.AverbacaoCTe averbacao, bool averbarComoEmbarcador, Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            string xmlCTe = ObterXMLCancelamentoCTe(averbacao.CTe, averbarComoEmbarcador, averbacao.Protocolo);
            string cnpjTransportador = averbacao.CTe.Empresa.CNPJ_SemFormato;
            string numeroCTe = averbacao.CTe.Numero.ToString();

            ServicoSenig.WebAppAverbaClient webClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoSenig.WebAppAverbaClient, ServicoSenig.IWebAppAverba>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Senig_WebAppAverba, out inspector);

            string retorno = webClient.EnviaXMLCANRet(xmlCTe, cnpjTransportador, numeroCTe);

            webClient.Close();

            return retorno;
        }

        private string ObterXMLAutorizacaoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool averbarComoEmbarcador, bool averbacaoProvisoria)
        {
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(_unitOfWork);

            string xml;

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe || averbarComoEmbarcador || averbacaoProvisoria)
            {
                string modelo = "99";
                if (averbacaoProvisoria)
                    modelo = "97";
                else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    modelo = "98";

                StringBuilder stXML = new StringBuilder();

                if (averbacaoProvisoria)
                {
                    stXML.Append("<cteProc xmlns=\"http://www.portalfiscal.inf.br/cte\" versao=\"3.00\">");
                    stXML.Append("<CTe xmlns=\"http://www.portalfiscal.inf.br/cte\">");
                    stXML.Append("<infCte Id=\"" + cte.Numero.ToString("D") + "\" versao=\"3.00\"><ide>");
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

                stXML.Append("<nCT>" + cte.Numero + "</nCT>");
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
                    stXML.Append("<toma03><toma>" + (int)cte.TipoTomador + "</toma></toma03>");

                stXML.Append("</ide>");

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

                if (cte.Destinatario.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                    stXML.Append("<CPF>" + cte.Destinatario.CPF_CNPJ_SemFormato + "</CPF>");
                else
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

                    if (cte.Expedidor.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                        stXML.Append("<CPF>" + cte.Expedidor.CPF_CNPJ_SemFormato + "</CPF>");
                    else
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

                    if (cte.Recebedor.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                        stXML.Append("<CPF>" + cte.Recebedor.CPF_CNPJ_SemFormato + "</CPF>");
                    else
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
                    stXML.Append("<vCarga>" + seguro.Valor.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</vCarga>");
                    stXML.Append("</seg>");
                }

                stXML.Append("</infCTeNorm>");
                stXML.Append("</infCte></CTe>");
                if (averbacaoProvisoria)
                {
                    stXML.Append("<protCTe xmlns=\"http://www.portalfiscal.inf.br/cte\" versao=\"3.00\">");
                    stXML.Append("<infProt Id=\"CTe" + cte.Numero.ToString("D") + "\">");
                    stXML.Append("<tpAmb>1</tpAmb>");
                    stXML.Append("<verAplic>MULTITMS</verAplic>");
                    stXML.Append("<chCTe>" + cte.Numero.ToString("D") + "</chCTe>");
                    stXML.Append("<dhRecbto>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "</dhRecbto>");
                    stXML.Append("<nProt>" + cte.Numero.ToString("D") + "</nProt>");
                    stXML.Append("<digVal>MFbZGM/wM5sXrPHX51zBzATM8lY=</digVal>");
                    stXML.Append("<cStat>100</cStat>");
                    stXML.Append("<xMotivo>Autorizado o uso do CT-e</xMotivo>");
                    stXML.Append("</infProt>");
                    stXML.Append("</protCTe>");
                }
                stXML.Append("</cteProc>");

                xml = stXML.ToString();
            }
            else
            {
                Servicos.CTe svcCTE = new Servicos.CTe(_unitOfWork);
                xml = svcCTE.ObterStringXMLAutorizacao(cte, _unitOfWork);
            }

            return xml;
        }

        private string ObterXMLCancelamentoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool averbarComoEmbarcador, string protocolo)
        {
            string xml;
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
                Servicos.CTe svcCTE = new Servicos.CTe(_unitOfWork);
                xml = svcCTE.ObterStringXMLCancelamento(cte, _unitOfWork);
            }

            return xml;
        }

        private Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig BuscarConfiguracaoSenig(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice)
        {
            Repositorio.Embarcador.Seguros.AverbacaoSenig repAverbacaoSenig = new Repositorio.Embarcador.Seguros.AverbacaoSenig(_unitOfWork);

            Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig averbacao = repAverbacaoSenig.BuscarPorApolice(apolice.Codigo);

            return averbacao;
        }

        #endregion

        #region Métodos Públicos

        public void AverbarDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(_unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(_unitOfWork);

            string protocolo = "";
            string mensagemIntegracao = "";
            string codigoRetorno = "";
            bool integradoComSucesso = false;

            try
            {
                Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig averbacaoSenig = BuscarConfiguracaoSenig(apolice);

                Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
                string xmlRetorno = EnviarAverbacaoSenig(averbacao, averbacaoSenig.AverbaComoEmbarcador, inspector);

                RetornoAverbacao retornoWebService = ConverterRetornoSenig(xmlRetorno);

                if (retornoWebService != null)
                {
                    if (retornoWebService.Mensagem.Codigo == CodigoRetorno.Sucesso)
                    {
                        mensagemIntegracao = "Averbado com sucesso";
                        integradoComSucesso = true;
                    }
                    else if (retornoWebService.Mensagem.Codigo == CodigoRetorno.Invalido)
                        mensagemIntegracao = retornoWebService.Mensagem.Codigo.ObterDetalhe();
                    else if (retornoWebService.Mensagem.Codigo == CodigoRetorno.Erro)
                        mensagemIntegracao = $"{retornoWebService.Mensagem.Codigo.ObterDetalhe()} - {retornoWebService.Mensagem.Detalhes}";

                    protocolo = retornoWebService.Protocolo;
                    codigoRetorno = ((int)retornoWebService.Mensagem.Codigo).ToString();
                }
                else
                {
                    mensagemIntegracao = "Ocorreu uma falha ao processar a averbação.";
                }

                Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo
                {
                    ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork),
                    ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork),
                    Data = DateTime.Now,
                    Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
                };

                repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
                averbacao.ArquivosTransacao.Add(averbacaoIntegracaoArquivo);
                tentativas = 0;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                if (averbacao.tentativasIntegracao >= 1)
                {
                    codigoRetorno = "999";
                    mensagemIntegracao = "O serviço da Senig não está disponível no momento.";
                    integradoComSucesso = false;
                    averbacao.tentativasIntegracao = 0;
                }
                else
                {
                    averbacao.tentativasIntegracao++;
                    tentativas++;
                }

            }

            if (integradoComSucesso)
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
            else
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;

            averbacao.Protocolo = protocolo ?? averbacao.Protocolo;
            averbacao.Averbacao = protocolo ?? averbacao.Averbacao;
            averbacao.CodigoRetorno = codigoRetorno ?? averbacao.CodigoRetorno;
            averbacao.MensagemRetorno = mensagemIntegracao;
            averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
            averbacao.DataRetorno = DateTime.Now;
            repAverbacaoCTe.Atualizar(averbacao);
        }

        public void CancelarAverbacaoDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(_unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(_unitOfWork);

            string protocolo = "";
            string mensagemIntegracao = "";
            string codigoRetorno = "";
            bool integradoComSucesso = false;

            try
            {
                Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig averbacaoSenig = BuscarConfiguracaoSenig(apolice);
                Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
                string xmlRetorno = EnviarCancelamentoAverbacaoSenig(averbacao, averbacaoSenig.AverbaComoEmbarcador, inspector);

                RetornoAverbacao retornoWebService = ConverterRetornoSenig(xmlRetorno);

                if (retornoWebService != null)
                {
                    if (retornoWebService.Mensagem.Codigo == CodigoRetorno.Sucesso)
                    {
                        mensagemIntegracao = "Cancelamento de averbação enviado com sucesso";
                        integradoComSucesso = true;
                    }
                    else if (retornoWebService.Mensagem.Codigo == CodigoRetorno.Invalido)
                        mensagemIntegracao = retornoWebService.Mensagem.Codigo.ObterDetalhe();
                    else if (retornoWebService.Mensagem.Codigo == CodigoRetorno.Erro)
                        mensagemIntegracao = $"{retornoWebService.Mensagem.Codigo.ObterDetalhe()} - {retornoWebService.Mensagem.Detalhes}";

                    protocolo = retornoWebService.Protocolo;
                    codigoRetorno = ((int)retornoWebService.Mensagem.Codigo).ToString();
                }
                else
                {
                    mensagemIntegracao = "Ocorreu uma falha ao processar o cancelamento de averbação.";
                }

                Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo
                {
                    ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork),
                    ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork),
                    Data = DateTime.Now,
                    Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
                };

                repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
                averbacao.ArquivosTransacao.Add(averbacaoIntegracaoArquivo);
                tentativas = 0;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                if (averbacao.tentativasIntegracao >= 1)
                {
                    codigoRetorno = "999";
                    mensagemIntegracao = "O serviço da Senig não está disponível no momento.";
                    integradoComSucesso = false;
                    averbacao.tentativasIntegracao = 0;
                }
                else
                {
                    averbacao.tentativasIntegracao++;
                    tentativas++;
                }

            }

            if (integradoComSucesso)
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado;
            else
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;

            averbacao.Protocolo = protocolo ?? averbacao.Protocolo;
            averbacao.Averbacao = protocolo ?? averbacao.Averbacao;
            averbacao.CodigoRetorno = codigoRetorno ?? averbacao.CodigoRetorno;
            averbacao.MensagemRetorno = mensagemIntegracao;
            averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
            averbacao.DataRetorno = DateTime.Now;
            repAverbacaoCTe.Atualizar(averbacao);
        }

        #endregion
    }
}
