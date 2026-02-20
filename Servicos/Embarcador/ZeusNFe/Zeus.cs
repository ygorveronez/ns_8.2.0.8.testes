using AdminMultisoftware.Dominio.Enumeradores;
using DFe.Classes.Flags;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Newtonsoft.Json.Linq;
using NFe.Classes;
using NFe.Classes.Informacoes;
using NFe.Classes.Informacoes.Cobranca;
using NFe.Classes.Informacoes.Destinatario;
using NFe.Classes.Informacoes.Detalhe;
using NFe.Classes.Informacoes.Detalhe.ProdEspecifico;
using NFe.Classes.Informacoes.Detalhe.Tributacao;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal.Tipos;
using NFe.Classes.Informacoes.Emitente;
using NFe.Classes.Informacoes.Identificacao;
using NFe.Classes.Informacoes.Identificacao.Tipos;
using NFe.Classes.Informacoes.Observacoes;
using NFe.Classes.Informacoes.Pagamento;
using NFe.Classes.Informacoes.Total;
using NFe.Classes.Informacoes.Transporte;
using NFe.Classes.Servicos.Tipos;
using NFe.Danfe.Base.NFCe;
using NFe.Danfe.Base;
using NFe.Danfe.Nativo;
using NFe.Danfe.OpenFast.NFCe;
using NFe.Danfe.OpenFast.NFe;
using NFe.Servicos;
using NFe.Servicos.Retorno;
using NFe.Utils;
using NFe.Utils.Consulta;
using NFe.Utils.Email;
using NFe.Utils.InformacoesSuplementares;
using NFe.Utils.NFe;
using Servicos.Extensions;
using Shared.NFe.Classes.Informacoes.InfRespTec;
using Shared.NFe.Classes.Informacoes.Intermediador;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using NFe.Danfe.Nativo.NFCe;
using teste = Zeus;

namespace Zeus.Embarcador.ZeusNFe
{
    public class Zeus
    {
        private ConfiguracaoApp _configuracoes;

        public Zeus()
        {

        }

        #region Métodos do Assinador A3

        public string AssinarXMLNFe(string cnpjEmpresa, string arquivoXML, string serial, int codigoNFe, string diretorioRaiz)
        {
            try
            {
                var arquivoNota = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, codigoNFe.ToString() + "-Assinar.xml");

                Utilidades.IO.FileStorageService.Storage.WriteLine(arquivoNota, arquivoXML);

                NFe.Classes.NFe _nfe;
                _nfe = new NFe.Classes.NFe().CarregarDeArquivoXml(arquivoNota);
                CriarConfiguracaoPadrao(serial, _nfe, "", "", diretorioRaiz);
                _nfe.Assina();

                _nfe.SalvarArquivoXml(diretorioRaiz + "notafiscal" + (!string.IsNullOrWhiteSpace(_nfe.infNFe.emit.CPF) ? _nfe.infNFe.emit.CPF : _nfe.infNFe.emit.CNPJ) +
                    "-" + _nfe.infNFe.ide.nNF + "Assinado.xml");

                return _nfe.ObterXmlString();

            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;
                else
                    return "Problemas ao criar e enviar NF-e";
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe EnviarNFe(string xmlAssinado, int codigoNFe, string serial, string reciboAnterior, string codigoStatusAnterior, string cIdToken, string csc, string diretorioRaiz)
        {
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe ret = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe();
            try
            {
                NFe.Classes.NFe _nfe;

                string arquivoNota = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, codigoNFe.ToString() + "-Assinado.xml");

                Utilidades.IO.FileStorageService.Storage.WriteLine(arquivoNota, xmlAssinado);

                _nfe = new NFe.Classes.NFe().CarregarDeArquivoXml(arquivoNota);

                CriarConfiguracaoPadrao(serial, _nfe, cIdToken, csc, diretorioRaiz);

                ModeloDocumento modNF = _nfe.infNFe.ide.mod;
                if (modNF == ModeloDocumento.NFCe && _nfe.infNFe.versao == "3.10")
                    _nfe.infNFeSupl = new infNFeSupl() { qrCode = _nfe.infNFeSupl.ObterUrlQrCode(_nfe, VersaoQrCode.QrCodeVersao1, cIdToken, csc) };
                else if (modNF == ModeloDocumento.NFCe && _nfe.infNFe.versao == "4.00")
                    _nfe.infNFeSupl = new infNFeSupl()
                    {
                        qrCode = _nfe.infNFeSupl.ObterUrlQrCode(_nfe, VersaoQrCode.QrCodeVersao2, cIdToken, csc),
                        urlChave = _nfe.infNFeSupl.ObterUrl(_nfe.infNFe.ide.tpAmb, _nfe.infNFe.ide.cUF, TipoUrlConsultaPublica.UrlConsulta, VersaoServico.Versao400, VersaoQrCode.QrCodeVersao2)
                    };

                ServicosNFe servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                RetornoNFeAutorizacao retornoEnvio = servicoNFe.NFeAutorizacao(codigoNFe, modNF == ModeloDocumento.NFCe ? IndicadorSincronizacao.Sincrono : IndicadorSincronizacao.Assincrono, new List<NFe.Classes.NFe> { _nfe }, false);

                string numeroRecibo = retornoEnvio.Retorno.infRec?.nRec ?? string.Empty;
                if (retornoEnvio.Retorno.cStat == 103 || retornoEnvio.Retorno.cStat == 104)
                {
                    NFe.Classes.Protocolo.protNFe protocoloNFe;
                    string versao, xMotivo;
                    int cStat;
                    if (modNF == ModeloDocumento.NFCe)
                    {
                        protocoloNFe = retornoEnvio.Retorno.protNFe;
                        versao = retornoEnvio.Retorno.versao;
                        xMotivo = retornoEnvio.Retorno.xMotivo;
                        cStat = retornoEnvio.Retorno.cStat;
                    }
                    else
                    {
                        RetornoNFeRetAutorizacao retornoConsulta;
                        if (((codigoStatusAnterior == "105") || (codigoStatusAnterior == "103" || retornoEnvio.Retorno.cStat == 104)) & !string.IsNullOrWhiteSpace(reciboAnterior))
                            retornoConsulta = servicoNFe.NFeRetAutorizacao(reciboAnterior);
                        else
                        {
                            Thread.Sleep(5000);
                            retornoConsulta = servicoNFe.NFeRetAutorizacao(numeroRecibo);
                        }

                        protocoloNFe = retornoConsulta.Retorno.protNFe?.Count > 0 ? retornoConsulta.Retorno.protNFe[0] : null;
                        versao = retornoConsulta.Retorno.versao;
                        xMotivo = retornoConsulta.Retorno.xMotivo;
                        cStat = retornoConsulta.Retorno.cStat;
                    }

                    if (protocoloNFe != null)
                    {
                        if (protocoloNFe.infProt.cStat != 100 && protocoloNFe.infProt.cStat != 150)
                        {
                            if (protocoloNFe.infProt.cStat == 204)
                            {
                                RetornoNfeConsultaProtocolo retornoDuplicidade = servicoNFe.NfeConsultaProtocolo(protocoloNFe.infProt.chNFe);

                                if (retornoDuplicidade.Retorno.cStat == 100 || retornoDuplicidade.Retorno.cStat == 150)
                                {
                                    nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = retornoDuplicidade.Retorno.protNFe, versao = retornoDuplicidade.Retorno.versao };

                                    ret.nRec = numeroRecibo;
                                    ret.cStat = retornoDuplicidade.Retorno.cStat.ToString();
                                    ret.xMotivo = retornoDuplicidade.Retorno.xMotivo;
                                    ret.Status = Dominio.Enumeradores.StatusNFe.Autorizado;
                                    ret.chNFe = retornoDuplicidade.Retorno.chNFe;
                                    ret.nProt = retornoDuplicidade.Retorno.protNFe.infProt.nProt;
                                    ret.dhRecbto = retornoDuplicidade.Retorno.protNFe.infProt.dhRecbto.DateTime;
                                    ret.XML = nfeproc.ObterXmlString();
                                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                                    return ret;
                                }
                                else if (protocoloNFe.infProt.cStat == 105 || protocoloNFe.infProt.xMotivo == "Lote em processamento")
                                {
                                    ret.nRec = numeroRecibo;
                                    ret.cStat = protocoloNFe.infProt.cStat.ToString();
                                    ret.xMotivo = protocoloNFe.infProt.xMotivo;
                                    ret.Status = Dominio.Enumeradores.StatusNFe.EmProcessamento;
                                    ret.chNFe = "";
                                    ret.nProt = "";
                                    ret.dhRecbto = null;
                                    ret.XML = "";
                                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                                    return ret;
                                }
                                else
                                {
                                    ret.nRec = numeroRecibo;
                                    ret.cStat = retornoDuplicidade.Retorno.cStat.ToString();
                                    ret.xMotivo = retornoDuplicidade.Retorno.xMotivo;
                                    ret.Status = Dominio.Enumeradores.StatusNFe.Rejeitado;
                                    ret.chNFe = "";
                                    ret.nProt = "";
                                    ret.dhRecbto = null;
                                    ret.XML = "";
                                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                                    return ret;
                                }
                            }
                            else if (protocoloNFe.infProt.cStat == 110)
                            {
                                nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = protocoloNFe, versao = versao };

                                ret.nRec = numeroRecibo;
                                ret.cStat = protocoloNFe.infProt.cStat.ToString();
                                ret.xMotivo = protocoloNFe.infProt.xMotivo;
                                ret.Status = Dominio.Enumeradores.StatusNFe.Denegado;
                                ret.chNFe = protocoloNFe.infProt?.chNFe;
                                ret.nProt = protocoloNFe.infProt?.nProt;
                                ret.dhRecbto = protocoloNFe.infProt?.dhRecbto.DateTime;
                                ret.XML = nfeproc.ObterXmlString();
                                ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                                return ret;
                            }
                            else if (protocoloNFe.infProt.cStat == 105 || protocoloNFe.infProt.xMotivo == "Lote em processamento")
                            {
                                ret.nRec = numeroRecibo;
                                ret.cStat = protocoloNFe.infProt.cStat.ToString();
                                ret.xMotivo = protocoloNFe.infProt.xMotivo;
                                ret.Status = Dominio.Enumeradores.StatusNFe.EmProcessamento;
                                ret.chNFe = "";
                                ret.nProt = "";
                                ret.dhRecbto = null;
                                ret.XML = "";
                                ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                                return ret;
                            }
                            else if (protocoloNFe.infProt.cStat == 302 || protocoloNFe.infProt.cStat == 205 || protocoloNFe.infProt.xMotivo.ToUpper().Contains("DENEGADO"))
                            {
                                nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = protocoloNFe, versao = versao };

                                ret.nRec = numeroRecibo;
                                ret.cStat = protocoloNFe.infProt.cStat.ToString();
                                ret.xMotivo = protocoloNFe.infProt.xMotivo;
                                ret.Status = Dominio.Enumeradores.StatusNFe.Denegado;
                                ret.chNFe = protocoloNFe.infProt?.chNFe;
                                ret.nProt = protocoloNFe.infProt?.nProt;
                                ret.dhRecbto = protocoloNFe.infProt?.dhRecbto.DateTime;
                                ret.XML = nfeproc.ObterXmlString();
                                ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                                return ret;
                            }
                            else
                            {
                                ret.nRec = numeroRecibo;
                                ret.cStat = protocoloNFe.infProt.cStat.ToString();
                                ret.xMotivo = protocoloNFe.infProt.xMotivo;
                                ret.Status = Dominio.Enumeradores.StatusNFe.Rejeitado;
                                ret.chNFe = "";
                                ret.nProt = "";
                                ret.dhRecbto = null;
                                ret.XML = "";
                                ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                                return ret;
                            }
                        }
                        else if (protocoloNFe.infProt.cStat == 105 || protocoloNFe.infProt.xMotivo == "Lote em processamento")
                        {
                            ret.nRec = numeroRecibo;
                            ret.cStat = protocoloNFe.infProt.cStat.ToString();
                            ret.xMotivo = protocoloNFe.infProt.xMotivo;
                            ret.Status = Dominio.Enumeradores.StatusNFe.EmProcessamento;
                            ret.chNFe = "";
                            ret.nProt = "";
                            ret.dhRecbto = null;
                            ret.XML = "";
                            ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                            return ret;
                        }
                        else if (protocoloNFe.infProt.cStat == 302 || protocoloNFe.infProt.cStat == 205 || protocoloNFe.infProt.xMotivo.ToUpper().Contains("DENEGADO"))
                        {
                            nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = protocoloNFe, versao = versao };

                            ret.nRec = numeroRecibo;
                            ret.cStat = protocoloNFe.infProt.cStat.ToString();
                            ret.xMotivo = protocoloNFe.infProt.xMotivo;
                            ret.Status = Dominio.Enumeradores.StatusNFe.Denegado;
                            ret.chNFe = protocoloNFe.infProt?.chNFe;
                            ret.nProt = protocoloNFe.infProt?.nProt;
                            ret.dhRecbto = protocoloNFe.infProt?.dhRecbto.DateTime;
                            ret.XML = nfeproc.ObterXmlString();
                            ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                            return ret;
                        }
                        else
                        {
                            nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = protocoloNFe, versao = versao };

                            ret.nRec = numeroRecibo;
                            ret.cStat = protocoloNFe.infProt.cStat.ToString();
                            ret.xMotivo = protocoloNFe.infProt.xMotivo;
                            ret.Status = Dominio.Enumeradores.StatusNFe.Autorizado;
                            ret.chNFe = protocoloNFe.infProt.chNFe;
                            ret.nProt = protocoloNFe.infProt.nProt;
                            ret.dhRecbto = protocoloNFe.infProt.dhRecbto.DateTime;
                            ret.XML = nfeproc.ObterXmlString();
                            ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                            return ret;
                        }
                    }
                    else if (retornoEnvio.Retorno.cStat == 105 || xMotivo == "Lote em processamento")//Em Processamento
                    {
                        ret.nRec = numeroRecibo;
                        ret.cStat = cStat.ToString();
                        ret.xMotivo = xMotivo;
                        ret.Status = Dominio.Enumeradores.StatusNFe.EmProcessamento;
                        ret.chNFe = "";
                        ret.nProt = "";
                        ret.dhRecbto = null;
                        ret.XML = "";
                        ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                        return ret;
                    }
                    else if (retornoEnvio.Retorno.cStat == 302 || retornoEnvio.Retorno.cStat == 205 || xMotivo.ToUpper().Contains("DENEGADO"))
                    {
                        nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = protocoloNFe, versao = versao };

                        ret.nRec = numeroRecibo;
                        ret.cStat = cStat.ToString();
                        ret.xMotivo = xMotivo;
                        ret.Status = Dominio.Enumeradores.StatusNFe.EmProcessamento;
                        ret.chNFe = protocoloNFe.infProt?.chNFe;
                        ret.nProt = protocoloNFe.infProt?.nProt;
                        ret.dhRecbto = protocoloNFe.infProt?.dhRecbto.DateTime;
                        ret.XML = nfeproc.ObterXmlString();
                        ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                        return ret;
                    }
                    else
                    {
                        ret.nRec = numeroRecibo;
                        ret.cStat = cStat.ToString();
                        ret.xMotivo = xMotivo;
                        ret.Status = Dominio.Enumeradores.StatusNFe.Rejeitado;
                        ret.chNFe = "";
                        ret.nProt = "";
                        ret.dhRecbto = null;
                        ret.XML = "";
                        ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                        return ret;
                    }
                }
                else if (retornoEnvio.Retorno.cStat == 105 || retornoEnvio.Retorno.xMotivo == "Lote em processamento")//Em Processamento
                {
                    ret.nRec = numeroRecibo;
                    ret.cStat = retornoEnvio.Retorno.cStat.ToString();
                    ret.xMotivo = retornoEnvio.Retorno.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.EmProcessamento;
                    ret.chNFe = "";
                    ret.nProt = "";
                    ret.dhRecbto = null;
                    ret.XML = "";
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                    return ret;
                }
                else if (retornoEnvio.Retorno.cStat == 302 || retornoEnvio.Retorno.cStat == 205 || retornoEnvio.Retorno.xMotivo.ToUpper().Contains("DENEGADO"))
                {
                    nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = retornoEnvio.Retorno.protNFe, versao = retornoEnvio.Retorno.versao };

                    ret.nRec = numeroRecibo;
                    ret.cStat = retornoEnvio.Retorno.cStat.ToString();
                    ret.xMotivo = retornoEnvio.Retorno.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.Denegado;
                    ret.chNFe = retornoEnvio.Retorno.protNFe?.infProt?.chNFe;
                    ret.nProt = retornoEnvio.Retorno.protNFe?.infProt?.nProt;
                    ret.dhRecbto = retornoEnvio.Retorno.protNFe?.infProt?.dhRecbto.DateTime;
                    ret.XML = nfeproc.ObterXmlString();
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                    return ret;
                }
                else if (retornoEnvio.Retorno.protNFe != null && retornoEnvio.Retorno.protNFe.infProt != null)
                {
                    ret.nRec = "";
                    ret.cStat = retornoEnvio.Retorno.protNFe.infProt.cStat.ToString();
                    ret.xMotivo = retornoEnvio.Retorno.protNFe.infProt.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.Rejeitado;
                    ret.chNFe = "";
                    ret.nProt = "";
                    ret.dhRecbto = null;
                    ret.XML = "";
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                    return ret;
                }
                else
                {
                    ret.nRec = "";
                    ret.cStat = retornoEnvio.Retorno.cStat.ToString();
                    ret.xMotivo = retornoEnvio.Retorno.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.Rejeitado;
                    ret.chNFe = "";
                    ret.nProt = "";
                    ret.dhRecbto = null;
                    ret.XML = "";
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;

                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret.nRec = "";
                ret.cStat = "";
                ret.Status = Dominio.Enumeradores.StatusNFe.Rejeitado;
                ret.chNFe = "";
                ret.nProt = "";
                ret.dhRecbto = null;
                ret.XML = "";
                ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Distribuicao;
                ret.xMotivo = "Problemas ao enviar NF-e " + ex.Message;
                return ret;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe CancelarNFe(DateTime dataEmissao, string xmlNotaFiscal, string codigoNFe, string sequencia, string protocolo, string chave, string justificativa, string cnpj, string serial, string diretorioRaiz)
        {
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe ret = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe();

            NFe.Classes.NFe _nfe;
            var arquivoNota = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, codigoNFe.ToString() + "-Assinado.xml");
            Utilidades.IO.FileStorageService.Storage.WriteLine(arquivoNota, xmlNotaFiscal);

            _nfe = new NFe.Classes.NFe().CarregarDeArquivoXml(arquivoNota);

            if (dataEmissao == DateTime.MinValue)
                dataEmissao = DateTime.Now;

            CriarConfiguracaoPadrao(serial, _nfe, "", "", diretorioRaiz);
            try
            {
                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);

                justificativa = justificativa.Trim().TrimEnd().TrimStart().Replace("º", "").Replace("ª", "").Replace("\n", " - ");
                var retornoConsulta = servicoNFe.RecepcaoEventoCancelamento(Convert.ToInt32(codigoNFe), Convert.ToInt16(sequencia), protocolo, chave, justificativa, Utilidades.String.OnlyNumbers(cnpj), dataEmissao);

                if (retornoConsulta.Retorno.cStat == 101 || retornoConsulta.Retorno.cStat == 151 || retornoConsulta.Retorno.cStat == 218 || retornoConsulta.Retorno.cStat == 420)
                {
                    ret.nRec = "";
                    ret.cStat = retornoConsulta.Retorno.cStat.ToString();
                    ret.xMotivo = retornoConsulta.Retorno.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.Cancelado;
                    ret.chNFe = "";
                    ret.nProt = "";
                    ret.dhRecbto = null;
                    ret.XML = retornoConsulta.EnvioStr;
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Cancelamento;
                    ret.Justificativa = justificativa;

                    return ret;
                }
                else if (retornoConsulta.Retorno.retEvento[0].infEvento.cStat == 135 || retornoConsulta.Retorno.retEvento[0].infEvento.cStat == 573)
                {
                    ret.nRec = "";
                    ret.cStat = retornoConsulta.Retorno.retEvento[0].infEvento.cStat.ToString();
                    ret.xMotivo = retornoConsulta.Retorno.retEvento[0].infEvento.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.Cancelado;
                    ret.chNFe = "";
                    ret.nProt = "";
                    ret.dhRecbto = null;
                    ret.XML = retornoConsulta.EnvioStr;
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Cancelamento;
                    ret.Justificativa = justificativa;

                    return ret;
                }
                else
                {
                    ret.nRec = "";
                    ret.cStat = retornoConsulta.Retorno.retEvento[0].infEvento.cStat.ToString();
                    ret.xMotivo = retornoConsulta.Retorno.retEvento[0].infEvento.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.Autorizado;
                    ret.chNFe = "";
                    ret.nProt = "";
                    ret.dhRecbto = null;
                    ret.XML = "";
                    ret.Justificativa = "";
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Cancelamento;

                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret.nRec = "";
                ret.cStat = "";
                ret.Status = Dominio.Enumeradores.StatusNFe.Autorizado;
                ret.chNFe = "";
                ret.nProt = "";
                ret.dhRecbto = null;
                ret.XML = "";
                ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Cancelamento;
                ret.xMotivo = "Problemas ao cancelar NF-e " + ex.Message;
                return ret;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe InutilizarNFe(string cnpjCpf, string anoAtual, string serieNFe, string numero, string justificativa, string serial, int tpAmb, int cUF, string modelo, string diretorioRaiz)
        {
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe ret = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe();

            _configuracoes = new ConfiguracaoApp
            {
                Emitente = new emit
                {
                    CNPJ = Utilidades.String.OnlyNumbers(cnpjCpf).Length == 11 ? null : Utilidades.String.OnlyNumbers(cnpjCpf),
                    CPF = Utilidades.String.OnlyNumbers(cnpjCpf).Length == 11 ? Utilidades.String.OnlyNumbers(cnpjCpf) : null,
                    enderEmit = new enderEmit
                    {
                        CEP = "00000000"
                    }
                },
                EnderecoEmitente = new enderEmit
                {
                    CEP = "00000000"
                }
            };

            _configuracoes.CfgServico = ConfiguracaoServico.Instancia;
            _configuracoes.CfgServico.tpAmb = (TipoAmbiente)tpAmb;
            _configuracoes.CfgServico.tpEmis = TipoEmissao.teNormal;
            _configuracoes.CfgServico.cUF = (DFe.Classes.Entidades.Estado)cUF;
            if (!string.IsNullOrWhiteSpace(modelo) && modelo == "65")
                _configuracoes.CfgServico.ModeloDocumento = ModeloDocumento.NFCe;
            else
                _configuracoes.CfgServico.ModeloDocumento = ModeloDocumento.NFe;
            _configuracoes.CfgServico.Certificado.Serial = serial;
            _configuracoes.CfgServico.Certificado.Senha = null;
            _configuracoes.CfgServico.Certificado.Arquivo = null;

            _configuracoes.CfgServico.VersaoNfceAministracaoCSC = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeAutorizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaCadastro = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaDest = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaProtocolo = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeDistribuicaoDFe = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeDownloadNF = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeInutilizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeRecepcao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeRetAutorizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeRetRecepcao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeStatusServico = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoCceCancelamento = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoEpec = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoManifestacaoDestinatario = VersaoServico.Versao400;

            _configuracoes.CfgServico.DiretorioSalvarXml = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, "XML NF-e");
            _configuracoes.CfgServico.DiretorioSchemas = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, "Schemas");
            _configuracoes.CfgServico.SalvarXmlServicos = false;
            _configuracoes.CfgServico.TimeOut = 160000;

            _configuracoes.ConfiguracaoDanfeNfe = new NFe.Danfe.Base.NFe.ConfiguracaoDanfeNfe();

            _configuracoes.ConfiguracaoEmail = new ConfiguracaoEmail("nfe@commerce.inf.br", "cesaoexp18", "Envio de NF-e de ", "Teste", "smtp.commerce.inf.br", 587, false, true, 16000); //Não é mais utilizado, carrega da config empresa

            var htmlEmail = "<html>" +
                            "</html>";

            _configuracoes.ConfiguracaoEmail.Assincrono = false;
            _configuracoes.ConfiguracaoEmail.Mensagem = htmlEmail;
            _configuracoes.ConfiguracaoEmail.MensagemEmHtml = true;
            _configuracoes.ConfiguracaoEmail.Timeout = 160000;

            _configuracoes.ConfiguracaoCsc = new ConfiguracaoCsc("000001", "");

            try
            {
                justificativa = justificativa.Trim().TrimEnd().TrimStart();
                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                var retornoConsulta = servicoNFe.NfeInutilizacao(Utilidades.String.OnlyNumbers(cnpjCpf), Convert.ToInt16(anoAtual),
                    _configuracoes.CfgServico.ModeloDocumento, Convert.ToInt16(serieNFe), Convert.ToInt32(numero),
                    Convert.ToInt32(numero), justificativa);

                if (retornoConsulta.Retorno.infInut.cStat == 102 || retornoConsulta.Retorno.infInut.cStat == 135 || retornoConsulta.Retorno.infInut.cStat == 206 || retornoConsulta.Retorno.infInut.cStat == 256 || retornoConsulta.Retorno.infInut.cStat == 563 || retornoConsulta.Retorno.infInut.cStat == 662)
                {
                    ret.nRec = "";
                    ret.cStat = retornoConsulta.Retorno.infInut.cStat.ToString();
                    ret.xMotivo = retornoConsulta.Retorno.infInut.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.Inutilizado;
                    ret.chNFe = "";
                    ret.nProt = "";
                    ret.dhRecbto = null;
                    ret.XML = retornoConsulta.EnvioStr;
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Inutilizacao;
                    ret.Justificativa = justificativa;

                    return ret;
                }
                else
                {

                    ret.nRec = "";
                    ret.cStat = retornoConsulta.Retorno.infInut.cStat.ToString();
                    ret.xMotivo = retornoConsulta.Retorno.infInut.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.Emitido;
                    ret.chNFe = "";
                    ret.nProt = "";
                    ret.dhRecbto = null;
                    ret.XML = "";
                    ret.Justificativa = "";
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Inutilizacao;

                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret.nRec = "";
                ret.cStat = "";
                ret.Status = Dominio.Enumeradores.StatusNFe.Emitido;
                ret.chNFe = "";
                ret.nProt = "";
                ret.dhRecbto = null;
                ret.XML = "";
                ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Inutilizacao;
                ret.xMotivo = "Problemas ao inutlizar NF-e " + ex.Message;
                return ret;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe CartaCorrecaoNFe(DateTime dataEmissao, string xmlNotaFiscal, string codigoNFe, string sequencia, string chave, string correcao, string cnpj, string serial, string diretorioRaiz)
        {
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe ret = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe();

            NFe.Classes.NFe _nfe;
            var arquivoNota = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, "XML NF-e", codigoNFe.ToString() + "-Assinado.xml");
            Utilidades.IO.FileStorageService.Storage.WriteLine(arquivoNota, xmlNotaFiscal);

            _nfe = new NFe.Classes.NFe().CarregarDeArquivoXml(arquivoNota);

            if (dataEmissao == DateTime.MinValue)
                dataEmissao = DateTime.Now;

            CriarConfiguracaoPadrao(serial, _nfe, "", "", diretorioRaiz);
            try
            {
                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);

                correcao = correcao.Trim().TrimEnd().TrimStart();
                var retornoConsulta = servicoNFe.RecepcaoEventoCartaCorrecao(Convert.ToInt32(codigoNFe), Convert.ToInt16(sequencia), chave, correcao.Replace("º", "").Replace("ª", "").Replace("\n", " - "), Utilidades.String.OnlyNumbers(cnpj), dataEmissao);

                if (retornoConsulta.Retorno.retEvento[0].infEvento.cStat == 135)
                {
                    ret.nRec = "";
                    ret.cStat = retornoConsulta.Retorno.retEvento[0].infEvento.cStat.ToString();
                    ret.xMotivo = retornoConsulta.Retorno.retEvento[0].infEvento.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.Autorizado;
                    ret.chNFe = "";
                    ret.nProt = retornoConsulta.Retorno.retEvento[0].infEvento.nProt;
                    ret.dhRecbto = null;
                    ret.XML = retornoConsulta.EnvioStr;
                    ret.Justificativa = correcao;
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.CartaCorrecao;

                    return ret;
                }
                else
                {
                    ret.nRec = "";
                    ret.cStat = retornoConsulta.Retorno.retEvento[0].infEvento.cStat.ToString();
                    ret.xMotivo = retornoConsulta.Retorno.retEvento[0].infEvento.xMotivo;
                    ret.Status = Dominio.Enumeradores.StatusNFe.Autorizado;
                    ret.chNFe = "";
                    ret.nProt = "";
                    ret.dhRecbto = null;
                    ret.XML = "";
                    ret.Justificativa = "";
                    ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.CartaCorrecao;

                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret.nRec = "";
                ret.cStat = "";
                ret.Status = Dominio.Enumeradores.StatusNFe.Autorizado;
                ret.chNFe = "";
                ret.nProt = "";
                ret.dhRecbto = null;
                ret.XML = "";
                ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.CartaCorrecao;
                ret.xMotivo = "Problemas ao gerar carta de correção da NF-e " + ex.Message;
                return ret;
            }
        }

        #endregion

        #region Métodos Públicos

        public string CriarEnviarNFe(int codigoNFe, Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, string relatorio, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario, string modelo, int tipoEmissao = 1, bool comEnvioEmail = true, bool gerarTitulo = true, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = null, string urlBase = "")
        {
            try
            {
                #region Cria e Envia NFe

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

                ModeloDocumento modNF = ModeloDocumento.NFe;
                if (modelo == "65")
                    modNF = ModeloDocumento.NFCe;

                CriarConfiguracaoPadrao(nfe, modNF, unitOfWork);

                NFe.Classes.NFe _nfe;

                string identificacaoEmitente;
                try
                {
                    _nfe = GetNf(nfe, modNF, VersaoServico.Versao400, unitOfWork, (TipoEmissao)tipoEmissao);

                    identificacaoEmitente = !string.IsNullOrWhiteSpace(_nfe.infNFe.emit.CPF) ? _nfe.infNFe.emit.CPF : _nfe.infNFe.emit.CNPJ;
                    _nfe.SalvarArquivoXml(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e" }) + "\\notafiscal" + identificacaoEmitente + "-" + _nfe.infNFe.ide.nNF + ".xml");
                }
                catch (Exception ex)
                {
                    SalvarStatusNota(nfe, unitOfWork, ex.Message, '0', Dominio.Enumeradores.StatusNFe.Rejeitado, "", "", null);
                    return ex.Message;
                }

                if (string.IsNullOrWhiteSpace(nfe.Empresa.NomeCertificado))
                {
                    string retornoAssinatura = ValidarDocumentosAguardandoAssinatura(nfe, Dominio.Enumeradores.StatusNFe.AguardandoAssinar, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(retornoAssinatura))
                        return retornoAssinatura;

                    SalvarStatusNota(nfe, unitOfWork, "Aguardando Assinatura do XML", 0, Dominio.Enumeradores.StatusNFe.AguardandoAssinar, "", "", null);
                    SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.XMLSemAssinatura, _nfe.ObterXmlString());
                    return "";
                }
                else if (nfe.Empresa.DataFinalCertificado < DateTime.Now)
                    return "Seu Certificado Digital está vencido! Favor atualizar o mesmo no sistema para prosseguir.";

                _nfe.Assina();

                if (modNF == ModeloDocumento.NFCe) //Versão 4.00
                {
                    _nfe.infNFeSupl = new infNFeSupl()
                    {
                        qrCode = _nfe.infNFeSupl.ObterUrlQrCode(_nfe, VersaoQrCode.QrCodeVersao2, _configuracoes.ConfiguracaoCsc.CIdToken, _configuracoes.ConfiguracaoCsc.Csc),
                        urlChave = _nfe.infNFeSupl.ObterUrl(_nfe.infNFe.ide.tpAmb, _nfe.infNFe.ide.cUF, TipoUrlConsultaPublica.UrlConsulta, VersaoServico.Versao400, VersaoQrCode.QrCodeVersao2)
                    };
                    _nfe.SalvarArquivoXml(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e" }) + "\\notafiscalmod65-" + identificacaoEmitente + "-" + _nfe.infNFe.ide.nNF + ".xml");
                }

                try
                {
                    _nfe.Valida();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    if (!string.IsNullOrEmpty(ex.Message))
                    {
                        SalvarStatusNota(nfe, unitOfWork, ex.Message, 0, Dominio.Enumeradores.StatusNFe.Rejeitado, "", "", null);
                        return ex.Message;
                    }
                    else
                        return "Problemas ao criar e enviar NF-e";
                }

                ServicosNFe servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);
                Servicos.SMS srvSMS = new Servicos.SMS(unitOfWork);

                RetornoNFeAutorizacao retornoEnvio = servicoNFe.NFeAutorizacao(codigoNFe, modNF == ModeloDocumento.NFCe ? IndicadorSincronizacao.Sincrono : IndicadorSincronizacao.Assincrono, new List<NFe.Classes.NFe> { _nfe }, false);

                if (_nfe.infNFe != null && _nfe.infNFe.emit != null && _nfe.infNFe.Id != null && !string.IsNullOrWhiteSpace(retornoEnvio.RetornoCompletoStr))
                {
                    Utilidades.IO.FileStorageService.Storage.WriteLine(Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e", "notafiscal" + identificacaoEmitente + "-" + _nfe.infNFe.Id + "-RETORNO-1.xml"), retornoEnvio.RetornoCompletoStr);
                }

                string numeroRecibo = retornoEnvio.Retorno.infRec?.nRec ?? string.Empty;
                if (retornoEnvio.Retorno.cStat == 103 || retornoEnvio.Retorno.cStat == 104)//Lote recebido
                {
                    NFe.Classes.Protocolo.protNFe protocoloNFe;
                    string versao, xMotivo;
                    int cStat;
                    if (modNF == ModeloDocumento.NFCe)
                    {
                        protocoloNFe = retornoEnvio.Retorno.protNFe;
                        versao = retornoEnvio.Retorno.versao;
                        xMotivo = retornoEnvio.Retorno.xMotivo;
                        cStat = retornoEnvio.Retorno.cStat;
                    }
                    else
                    {
                        RetornoNFeRetAutorizacao retornoConsulta;
                        if (((nfe.UltimoStatus == "105") || (nfe.UltimoStatus == "103" || retornoEnvio.Retorno.cStat == 104)) & !string.IsNullOrWhiteSpace(nfe.NumeroRecibo))
                            retornoConsulta = servicoNFe.NFeRetAutorizacao(nfe.NumeroRecibo);
                        else
                        {
                            Thread.Sleep(5000);
                            retornoConsulta = servicoNFe.NFeRetAutorizacao(numeroRecibo);
                        }

                        Utilidades.IO.FileStorageService.Storage.WriteLine(Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e", "notafiscal" + identificacaoEmitente + "-" + _nfe.infNFe.Id + "-RETORNO-2.xml"), retornoConsulta.RetornoCompletoStr);

                        protocoloNFe = retornoConsulta.Retorno.protNFe?.Count > 0 ? retornoConsulta.Retorno.protNFe[0] : null;
                        versao = retornoConsulta.Retorno.versao;
                        xMotivo = retornoConsulta.Retorno.xMotivo;
                        cStat = retornoConsulta.Retorno.cStat;
                    }

                    if (protocoloNFe != null)
                    {
                        if (protocoloNFe.infProt.cStat != 100 && protocoloNFe.infProt.cStat != 150)//Autorizada
                        {
                            if (protocoloNFe.infProt.cStat == 204)//Duplicidade
                            {
                                RetornoNfeConsultaProtocolo retornoDuplicidade = servicoNFe.NfeConsultaProtocolo(protocoloNFe.infProt.chNFe);

                                Utilidades.IO.FileStorageService.Storage.WriteLine(Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e", "notafiscal" + identificacaoEmitente + "-" + _nfe.infNFe.ide.nNF + "-RETORNO-3.xml"), retornoDuplicidade.RetornoCompletoStr);

                                if (retornoDuplicidade.Retorno.cStat == 100 || retornoDuplicidade.Retorno.cStat == 150)
                                {
                                    SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, retornoDuplicidade.Retorno.cStat.ToString());
                                    SalvarStatusNota(nfe, unitOfWork, retornoDuplicidade.Retorno.xMotivo, retornoDuplicidade.Retorno.cStat, Dominio.Enumeradores.StatusNFe.Autorizado, retornoDuplicidade.Retorno.chNFe, retornoDuplicidade.Retorno.protNFe.infProt.nProt, retornoDuplicidade.Retorno.protNFe.infProt.dhRecbto.DateTime);
                                    nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = retornoDuplicidade.Retorno.protNFe, versao = retornoDuplicidade.Retorno.versao };
                                    SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Distribuicao, nfeproc.ObterXmlString());
                                    MovimentarEstoqueNota(codigoNFe, unitOfWork, nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida, tipoServicoMultisoftware);
                                    if (modNF == ModeloDocumento.NFe)
                                    {
                                        if (nfe.NaturezaDaOperacao.GeraTitulo && gerarTitulo)
                                            GerarTitulosNota(codigoNFe, unitOfWork, nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida);
                                        FinalizarPedidos(codigoNFe, unitOfWork);
                                        GerarDANFECrystal(nfe, usuario, caminhoRelatoriosEmbarcador, unitOfWork);
                                        if (comEnvioEmail)
                                            EnviarEmailNFe(codigoNFe, unitOfWork, relatorio, caminhoRelatoriosEmbarcador, usuario, urlBase);
                                        if (nfe.Empresa.ArmazenarDanfeParaSMS)
                                            serNotaFiscalEletronica.GerarDanfeParaSMS(unitOfWork, nfe, usuario);
                                        if (nfe.Empresa.AtivarEnvioDanfeSMS && clienteAcesso != null)
                                            return srvSMS.EnviarMensagem(nfe, clienteAcesso.URLAcesso, unitOfWork);
                                    }
                                    else if (modNF == ModeloDocumento.NFCe && gerarTitulo)
                                    {
                                        if (gerarTitulo)
                                            GerarTitulosNotaConsumidor(codigoNFe, unitOfWork);
                                        FinalizarPedidos(codigoNFe, unitOfWork);
                                        GerarDANFENFCe(codigoNFe, unitOfWork);
                                        if (comEnvioEmail)
                                            EnviarEmailNFCe(codigoNFe, unitOfWork);
                                    }
                                    return "";
                                }
                                else if (protocoloNFe.infProt.cStat == 105 || protocoloNFe.infProt.xMotivo == "Lote em processamento")//Em Processamento
                                {
                                    SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, protocoloNFe.infProt.cStat.ToString());
                                    SalvarStatusNota(nfe, unitOfWork, protocoloNFe.infProt.xMotivo, protocoloNFe.infProt.cStat, Dominio.Enumeradores.StatusNFe.EmProcessamento, "", "", null);
                                    return protocoloNFe.infProt.xMotivo;
                                }
                                else
                                {
                                    SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, retornoDuplicidade.Retorno.cStat.ToString());
                                    SalvarStatusNota(nfe, unitOfWork, retornoDuplicidade.Retorno.xMotivo, retornoDuplicidade.Retorno.cStat, Dominio.Enumeradores.StatusNFe.Rejeitado, "", "", null);
                                    return retornoDuplicidade.Retorno.xMotivo;
                                }
                            }
                            else if (protocoloNFe.infProt.cStat == 110)//Denegada
                            {
                                SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, protocoloNFe.infProt.cStat.ToString());

                                SalvarStatusNota(nfe, unitOfWork, protocoloNFe.infProt.xMotivo, protocoloNFe.infProt.cStat, Dominio.Enumeradores.StatusNFe.Denegado, protocoloNFe?.infProt?.chNFe, protocoloNFe?.infProt?.nProt, protocoloNFe?.infProt?.dhRecbto.DateTime);
                                nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = protocoloNFe, versao = versao };
                                SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Distribuicao, nfeproc.ObterXmlString());

                                return protocoloNFe.infProt.xMotivo;
                            }
                            else if (protocoloNFe.infProt.cStat == 302 || protocoloNFe.infProt.cStat == 205 || protocoloNFe.infProt.xMotivo.ToUpper().Contains("DENEGADO"))
                            {
                                SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, protocoloNFe.infProt.cStat.ToString());
                                SalvarStatusNota(nfe, unitOfWork, protocoloNFe.infProt.xMotivo, protocoloNFe.infProt.cStat, Dominio.Enumeradores.StatusNFe.Denegado, protocoloNFe?.infProt?.chNFe, protocoloNFe?.infProt?.nProt, protocoloNFe?.infProt?.dhRecbto.DateTime);
                                nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = protocoloNFe, versao = versao };
                                SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Distribuicao, nfeproc.ObterXmlString());

                                return protocoloNFe.infProt.xMotivo;
                            }
                            else if (protocoloNFe.infProt.cStat == 105 || protocoloNFe.infProt.xMotivo == "Lote em processamento")//Em Processamento
                            {
                                SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, protocoloNFe.infProt.cStat.ToString());
                                SalvarStatusNota(nfe, unitOfWork, protocoloNFe.infProt.xMotivo, protocoloNFe.infProt.cStat, Dominio.Enumeradores.StatusNFe.EmProcessamento, "", "", null);
                                return protocoloNFe.infProt.xMotivo;
                            }
                            else
                            {
                                SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, protocoloNFe.infProt.cStat.ToString());
                                SalvarStatusNota(nfe, unitOfWork, protocoloNFe.infProt.xMotivo, protocoloNFe.infProt.cStat, Dominio.Enumeradores.StatusNFe.Rejeitado, "", "", null);
                                return protocoloNFe.infProt.xMotivo;
                            }
                        }
                        else if (protocoloNFe.infProt.cStat == 105 || protocoloNFe.infProt.xMotivo == "Lote em processamento")//Em Processamento
                        {
                            SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, protocoloNFe.infProt.cStat.ToString());
                            SalvarStatusNota(nfe, unitOfWork, protocoloNFe.infProt.xMotivo, protocoloNFe.infProt.cStat, Dominio.Enumeradores.StatusNFe.EmProcessamento, "", "", null);
                            return protocoloNFe.infProt.xMotivo;
                        }
                        else if (protocoloNFe.infProt.cStat == 302 || protocoloNFe.infProt.cStat == 205 || protocoloNFe.infProt.xMotivo.ToUpper().Contains("DENEGADO"))
                        {
                            SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, protocoloNFe.infProt.cStat.ToString());
                            SalvarStatusNota(nfe, unitOfWork, protocoloNFe.infProt.xMotivo, protocoloNFe.infProt.cStat, Dominio.Enumeradores.StatusNFe.Denegado, protocoloNFe?.infProt?.chNFe, protocoloNFe?.infProt?.nProt, protocoloNFe?.infProt?.dhRecbto.DateTime);
                            nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = protocoloNFe, versao = versao };
                            SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Distribuicao, nfeproc.ObterXmlString());

                            return protocoloNFe.infProt.xMotivo;
                        }
                        else
                        {
                            SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, protocoloNFe.infProt.cStat.ToString());
                            SalvarStatusNota(nfe, unitOfWork, protocoloNFe.infProt.xMotivo, protocoloNFe.infProt.cStat, Dominio.Enumeradores.StatusNFe.Autorizado, protocoloNFe.infProt.chNFe, protocoloNFe.infProt.nProt, protocoloNFe.infProt.dhRecbto.DateTime);
                            nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = protocoloNFe, versao = versao };
                            SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Distribuicao, nfeproc.ObterXmlString());
                            MovimentarEstoqueNota(codigoNFe, unitOfWork, nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida, tipoServicoMultisoftware);
                            if (modNF == ModeloDocumento.NFe)
                            {
                                if (nfe.NaturezaDaOperacao.GeraTitulo && gerarTitulo)
                                    GerarTitulosNota(codigoNFe, unitOfWork, nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida);
                                FinalizarPedidos(codigoNFe, unitOfWork);
                                GerarDANFECrystal(nfe, usuario, caminhoRelatoriosEmbarcador, unitOfWork);
                                if (comEnvioEmail)
                                    EnviarEmailNFe(codigoNFe, unitOfWork, relatorio, caminhoRelatoriosEmbarcador, usuario, urlBase);
                                if (nfe.Empresa.ArmazenarDanfeParaSMS)
                                    serNotaFiscalEletronica.GerarDanfeParaSMS(unitOfWork, nfe, usuario);
                                if (nfe.Empresa.AtivarEnvioDanfeSMS && clienteAcesso != null)
                                    return srvSMS.EnviarMensagem(nfe, clienteAcesso.URLAcesso, unitOfWork);
                            }
                            else if (modNF == ModeloDocumento.NFCe)
                            {
                                if (gerarTitulo)
                                    GerarTitulosNotaConsumidor(codigoNFe, unitOfWork);
                                FinalizarPedidos(codigoNFe, unitOfWork);
                                GerarDANFENFCe(codigoNFe, unitOfWork);
                                if (comEnvioEmail)
                                    EnviarEmailNFCe(codigoNFe, unitOfWork);
                            }
                            return "";
                        }
                    }
                    else if (retornoEnvio.Retorno.cStat == 105 || xMotivo == "Lote em processamento")//Em Processamento
                    {
                        SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, retornoEnvio.Retorno.cStat.ToString());
                        SalvarStatusNota(nfe, unitOfWork, xMotivo, cStat, Dominio.Enumeradores.StatusNFe.EmProcessamento, "", "", null);
                        return xMotivo;
                    }
                    else if (retornoEnvio.Retorno.cStat == 302 || retornoEnvio.Retorno.cStat == 205 || xMotivo.ToUpper().Contains("DENEGADO"))
                    {
                        SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, retornoEnvio.Retorno.cStat.ToString());
                        SalvarStatusNota(nfe, unitOfWork, protocoloNFe.infProt.xMotivo, protocoloNFe.infProt.cStat, Dominio.Enumeradores.StatusNFe.Denegado, protocoloNFe?.infProt?.chNFe, protocoloNFe?.infProt?.nProt, protocoloNFe?.infProt?.dhRecbto.DateTime);
                        nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = protocoloNFe, versao = versao };
                        SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Distribuicao, nfeproc.ObterXmlString());

                        return xMotivo;
                    }
                    else
                    {
                        SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, retornoEnvio.Retorno.cStat.ToString());
                        SalvarStatusNota(nfe, unitOfWork, xMotivo, cStat, Dominio.Enumeradores.StatusNFe.Rejeitado, "", "", null);
                        return xMotivo;
                    }
                }
                else if (retornoEnvio.Retorno.cStat == 105 || retornoEnvio.Retorno.xMotivo == "Lote em processamento")//Em Processamento
                {
                    SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, retornoEnvio.Retorno.cStat.ToString());
                    SalvarStatusNota(nfe, unitOfWork, retornoEnvio.Retorno.xMotivo, retornoEnvio.Retorno.cStat, Dominio.Enumeradores.StatusNFe.EmProcessamento, "", "", null);
                    return retornoEnvio.Retorno.xMotivo;
                }
                else if (retornoEnvio.Retorno.cStat == 302 || retornoEnvio.Retorno.cStat == 205 || retornoEnvio.Retorno.xMotivo.ToUpper().Contains("DENEGADO"))
                {
                    SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, retornoEnvio.Retorno.cStat.ToString());
                    SalvarStatusNota(nfe, unitOfWork, retornoEnvio.Retorno.xMotivo, retornoEnvio.Retorno.cStat, Dominio.Enumeradores.StatusNFe.Denegado, retornoEnvio.Retorno.protNFe?.infProt?.chNFe, retornoEnvio.Retorno.protNFe?.infProt?.nProt, retornoEnvio.Retorno.protNFe?.infProt?.dhRecbto.DateTime);
                    nfeProc nfeproc = new nfeProc { NFe = _nfe, protNFe = retornoEnvio.Retorno.protNFe, versao = retornoEnvio.Retorno.versao };
                    SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Distribuicao, nfeproc.ObterXmlString());

                    return retornoEnvio.Retorno.xMotivo;
                }
                else if (retornoEnvio.Retorno.protNFe != null && retornoEnvio.Retorno.protNFe.infProt != null)
                {
                    SalvarNumeroReciboNota(nfe, unitOfWork, "", retornoEnvio.Retorno.protNFe.infProt.cStat.ToString());
                    SalvarStatusNota(nfe, unitOfWork, retornoEnvio.Retorno.protNFe.infProt.xMotivo, retornoEnvio.Retorno.protNFe.infProt.cStat, Dominio.Enumeradores.StatusNFe.Rejeitado, "", "", null);
                    return retornoEnvio.Retorno.protNFe.infProt.xMotivo;
                }
                else
                {
                    SalvarNumeroReciboNota(nfe, unitOfWork, numeroRecibo, retornoEnvio.Retorno.cStat.ToString());
                    SalvarStatusNota(nfe, unitOfWork, retornoEnvio.Retorno.xMotivo, retornoEnvio.Retorno.cStat, Dominio.Enumeradores.StatusNFe.Rejeitado, "", "", null);
                    return retornoEnvio.Retorno.xMotivo;
                }

                #endregion
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    return ex.Message;
                }
                else
                    return "Problemas ao criar e enviar NF-e";
            }
        }

        public string VisualizarDANFE(int codigoNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarPorNota(codigoNFe);

            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            CriarConfiguracaoPadrao(nfe, ModeloDocumento.NFe, unitOfWork);

            if (arquivo != null)
            {
                try
                {
                    var xmlNota = arquivo.XMLDistribuicao;
                    if (!string.IsNullOrWhiteSpace(xmlNota))
                    {
                        var proc = new nfeProc().CarregarDeXmlString(xmlNota);
                        var danfe = new DanfeFrNfe(proc, _configuracoes.ConfiguracaoDanfeNfe);
                        //danfe.Visualizar(false);
                        danfe.ExportarPdf();
                        return "";
                    }
                    else
                        return "Nenhum xml de distribuição da nota localizado.";
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(ex.Message))
                        return ex.Message;
                    else
                        return "Problemas ao criar DANFE da nota selecionada.";
                }
            }
            else
                return "Nenhuma nota localizada.";
        }

        public string GerarDANFENFCe(int codigoNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarPorNota(codigoNFe);

            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            CriarConfiguracaoPadrao(nfe, ModeloDocumento.NFCe, unitOfWork);

            if (arquivo == null)
                return "Nenhuma nota localizada.";

            try
            {
                string arquivoXml = arquivo.XMLDistribuicao;
                if (string.IsNullOrEmpty(arquivoXml))
                    return "Nenhum arquivo XML localizado";
                nfeProc proc = new nfeProc().CarregarDeXmlString(arquivoXml);
                if (proc.NFe.infNFe.ide.mod != ModeloDocumento.NFCe)
                    throw new Exception("O XML informado não é um NFCe!");

                //ConfiguracaoDanfeNfce configuracaoDanfeNfce = new ConfiguracaoDanfeNfce(NfceDetalheVendaNormal.UmaLinha, NfceDetalheVendaContigencia.UmaLinha);
                //DanfeFrNfce danfe = new DanfeFrNfce(proc, configuracaoDanfeNfce, _configuracoes.ConfiguracaoCsc.CIdToken, _configuracoes.ConfiguracaoCsc.Csc);

                //var diretorioRaiz = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e" });
                //            danfe.ExportarPdf(diretorioRaiz + "\\" + nfe.Chave + "NFCe.pdf");

                DanfeNativoNfce impr = new DanfeNativoNfce(arquivoXml,
                    _configuracoes.ConfiguracaoDanfeNfce.VersaoQrCode,
                    null,
                    _configuracoes.ConfiguracaoCsc.CIdToken,
                    _configuracoes.ConfiguracaoCsc.Csc,
                    0 /*troco*//*, "Arial Black"*/);

                var diretorioRaiz = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e" });
                //impr.Imprimir(null, diretorioRaiz + "\\" + nfe.Chave + "NFCe.pdf");

                impr.GerarImagem(Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, nfe.Chave + "NFCe.png"), System.Drawing.Imaging.ImageFormat.Png);
                byte[] imagemEmBytes = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, nfe.Chave + "NFCe.png"));
                var pdf = impr.ConverterImagemParaPdfBytes(imagemEmBytes);
                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, nfe.Chave + "NFCe.pdf"), pdf);

                return "";
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;

                return "Problemas ao criar a DANFCE da nota selecionada.";
            }
        }

        public string GerarDANFECrystal(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Dominio.Entidades.Usuario usuario, string caminhoRelatoriosEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.UnitOfWork unidadeDeTrabaho = new Repositorio.UnitOfWork(unitOfWork.StringConexao);

            Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unidadeDeTrabaho);
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = repRelatorio.BuscarPorTitulo("DANFE");
            string novoCaminhoPDF = "";
            if (relatorioOrigem != null)
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unidadeDeTrabaho);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = new Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio();
                dynRelatorio.Codigo = relatorioOrigem.Codigo;
                dynRelatorio.CodigoControleRelatorios = relatorioOrigem.CodigoControleRelatorios;
                dynRelatorio.Titulo = relatorioOrigem.Titulo;
                dynRelatorio.Descricao = relatorioOrigem.Descricao;
                dynRelatorio.Padrao = relatorioOrigem.Padrao;
                dynRelatorio.ExibirSumarios = relatorioOrigem.ExibirSumarios;
                dynRelatorio.CortarLinhas = relatorioOrigem.CortarLinhas;
                dynRelatorio.FundoListrado = relatorioOrigem.FundoListrado;
                dynRelatorio.TamanhoPadraoFonte = relatorioOrigem.TamanhoPadraoFonte;
                dynRelatorio.FontePadrao = relatorioOrigem.FontePadrao;
                dynRelatorio.AgruparRelatorio = false;
                dynRelatorio.PropriedadeAgrupa = relatorioOrigem.PropriedadeAgrupa;
                dynRelatorio.OrdemAgrupamento = relatorioOrigem.OrdemAgrupamento;
                dynRelatorio.PropriedadeOrdena = relatorioOrigem.PropriedadeOrdena;
                dynRelatorio.OrdemOrdenacao = relatorioOrigem.OrdemOrdenacao;
                dynRelatorio.TipoArquivoRelatorio = Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
                dynRelatorio.OrientacaoRelatorio = relatorioOrigem.OrientacaoRelatorio;
                dynRelatorio.Grid = "{\"draw\":0,\"inicio\":0,\"limite\":0,\"indiceColunaOrdena\":0,\"dirOrdena\":\"desc\",\"recordsTotal\":0,\"recordsFiltered\":0,\"group\":{\"enable\":false,\"propAgrupa\":null,\"dirOrdena\":null},\"header\":[{\"title\":\"Cód. Produto\",\"data\":\"CodigoProduto\",\"width\":\"10%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":0,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Descrição dos Produtos/Serviços\",\"data\":\"DescricaoItem\",\"width\":\"25%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-left\",\"tabletHide\":false,\"phoneHide\":false,\"position\":1,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"NCM\",\"data\":\"CodigoNCMItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":2,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"CST/CSOSN\",\"data\":\"DescricaoCSTItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":3,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"CFOP\",\"data\":\"CodigoCFOPItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":4,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Unid.\",\"data\":\"DescricaoUnidadeItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":5,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Quantidade\",\"data\":\"QuantidadeItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":6,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"V. Unitário\",\"data\":\"ValorUnitarioItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":7,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"V. Total\",\"data\":\"ValorTotalItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":8,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"B.C. ICMS\",\"data\":\"BCICMSItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":9,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Valor ICMS\",\"data\":\"ValorICMSItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":10,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Valor IPI\",\"data\":\"ValorIPIItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":11,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"% ICMS\",\"data\":\"AliquotaICMSItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":12,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"% IPI\",\"data\":\"AliquotaIPIItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":13,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0}],\"data\":null,\"dataSumarizada\":null,\"order\":[{\"column\":0,\"dir\":\"desc\"}]}";
                dynRelatorio.Report = relatorioOrigem.Codigo;
                dynRelatorio.NovoRelatorio = true;

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, usuario, dynRelatorio.TipoArquivoRelatorio, unidadeDeTrabaho);

                //unidadeDeTrabaho.CommitChanges();

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware.MultiTMS);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = null;

                relatorioTemp.PropriedadeOrdena = propOrdena;

                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unidadeDeTrabaho);
                serNotaFiscalEletronica.GerarRelatorioDANFE(nfe, agrupamentos, relatorioControleGeracao, relatorioTemp, unidadeDeTrabaho.StringConexao);

                Thread.Sleep(5000);

                string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, relatorioControleGeracao.GuidArquivo) + ".pdf";
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                {
                    novoCaminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, nfe.Chave + ".pdf");
                    if (Utilidades.IO.FileStorageService.Storage.Exists(novoCaminhoPDF))
                        Utilidades.IO.FileStorageService.Storage.Delete(novoCaminhoPDF);
                    Utilidades.IO.FileStorageService.Storage.Copy(caminhoArquivo, novoCaminhoPDF);

                    string caminhoRelatorio = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabaho).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), nfe.Chave + ".pdf");

                    Utilidades.IO.FileStorageService.Storage.Copy(caminhoArquivo, caminhoRelatorio);
                }
            }

            return novoCaminhoPDF;
        }

        public string GerarDANFENFeDocumentoDestinados(string CaminhoXML, string CaminhoDANFE, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                var arquivoXml = Utilidades.IO.FileStorageService.Storage.ReadAllText(CaminhoXML);
                if (string.IsNullOrEmpty(arquivoXml))
                    return "Nenhum arquivo XML localizado";

                arquivoXml = Regex.Replace(arquivoXml, @"<comb>.*?</comb>", string.Empty); //Remove dados de tags de combustíveis
                arquivoXml = Regex.Replace(arquivoXml, @"<veicProd>.*?</veicProd>", string.Empty); //Remove dados de tags de veículo proprio
                arquivoXml = Regex.Replace(arquivoXml, @"<detExport>.*?</detExport>", string.Empty); //Remove dados de tags de veículo proprio

                var proc = DFe.Utils.FuncoesXml.XmlStringParaClasse<nfeProc>(arquivoXml);

                //var proc = new nfeProc().CarregarDeXmlString(arquivoXml);
                var danfe = new DanfeFrNfe(proc, new NFe.Danfe.Base.NFe.ConfiguracaoDanfeNfe(), null);
                byte[] pdf = danfe.ExportarPdf();

                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(CaminhoDANFE, pdf);

                return "";
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;
                else
                    return "Problemas ao criar a DANFE da nota selecionada.";
            }
        }

        public static bool GerarDANFE(out string erro, string arquivoXML, string caminhoDANFE, bool objetoJson, bool nfeSemProc)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            if (string.IsNullOrWhiteSpace(arquivoXML))
            {
                erro = "Nenhum arquivo XML localizado.";
                return false;
            }

            nfeProc proc;
            NFe.Classes.NFe nfe;
            DanfeFrNfe danfe;

            if (!objetoJson)
            {
                arquivoXML = arquivoXML.Replace(" </nro>", "</nro>").Replace("<fone></fone>", "");
                if (nfeSemProc)
                {
                    nfe = new NFe.Classes.NFe().CarregarDeXmlString(arquivoXML);
                    danfe = new DanfeFrNfe(nfe, new NFe.Danfe.Base.NFe.ConfiguracaoDanfeNfe(), null);
                }
                else
                {
                    proc = new nfeProc().CarregarDeXmlString(arquivoXML);
                    danfe = new DanfeFrNfe(proc, new NFe.Danfe.Base.NFe.ConfiguracaoDanfeNfe());
                }
            }
            else
            {
                dynamic nfeZeus = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(arquivoXML);
                string objetoConvertido = nfeZeus.nfeProc.ToString();
                objetoConvertido = objetoConvertido.Replace(System.Environment.NewLine, "");

                if (objetoConvertido.Contains("VOLUME[S]"))
                    objetoConvertido = objetoConvertido.Replace("VOLUME[S]", "VOLUME");

                if (objetoConvertido.Contains("detPag"))
                {
                    Regex[] regexes = new Regex[]
                   {
                        new Regex("^.*Pag.*$", RegexOptions.IgnoreCase),
                   };

                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }

                if (objetoConvertido.Contains("dDesemb") && objetoConvertido.Contains("xLocDesemb"))
                {
                    Regex[] regexes = new Regex[]
                    {
                        new Regex("^.*DI.*$", RegexOptions.IgnoreCase),
                    };

                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }

                if (objetoConvertido.Contains("rastro") && objetoConvertido.Contains("nLote"))
                {
                    Regex[] regexes = new Regex[]
                    {
                        new Regex("^.*rastro.*$", RegexOptions.IgnoreCase),
                    };

                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }

                if (objetoConvertido.Contains("comb") && objetoConvertido.Contains("cProdANP"))
                {
                    Regex[] regexes = new Regex[]
                    {
                        new Regex("^.*comb.*$", RegexOptions.IgnoreCase),
                    };

                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }

                if (objetoConvertido.Contains("NFref") && objetoConvertido.Contains("refNFe"))
                {
                    Regex[] regexes = new Regex[]
                    {
                        new Regex("^.*NFref.*$", RegexOptions.IgnoreCase),
                    };

                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }
                else
                 if (objetoConvertido.Contains("NFref"))
                {
                    Regex[] regexes = new Regex[]
                    {
                        new Regex("^.*NFref.*$", RegexOptions.IgnoreCase),
                    };

                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }

                if (objetoConvertido.Contains("lacres") && objetoConvertido.Contains("nLacre"))
                {
                    Regex[] regexes = new Regex[]
                    {
                        new Regex("^.*lacres.*$", RegexOptions.IgnoreCase),
                    };
                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }

                if (objetoConvertido.Contains("detExport") && objetoConvertido.Contains("nDraw"))
                {
                    Regex[] regexes = new Regex[]
                    {
                        new Regex("^.*detExport.*$", RegexOptions.IgnoreCase),
                    };
                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }
                else if (objetoConvertido.Contains("detExport"))
                {
                    Regex[] regexes = new Regex[]
                    {
                        new Regex("^.*detExport.*$", RegexOptions.IgnoreCase),
                    };
                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }

                if (objetoConvertido.Contains("veicProd"))
                {
                    Regex[] regexes = new Regex[]
                    {
                        new Regex("^.*veicProd.*$", RegexOptions.IgnoreCase),
                    };
                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }

                if (objetoConvertido.Contains("NVE"))
                {
                    Regex[] regexes = new Regex[]
                    {
                        new Regex("^.*NVE.*$", RegexOptions.IgnoreCase),
                    };
                    objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                }

                try
                {
                    proc = Newtonsoft.Json.JsonConvert.DeserializeObject<nfeProc>(objetoConvertido);
                }
                catch
                {
                    if (objetoConvertido.Contains("infAdProd"))
                    {
                        Regex[] regexes = new Regex[]
                        {
                        new Regex("^.*infAdProd.*$", RegexOptions.IgnoreCase),
                        };
                        objetoConvertido = RemoveSensitiveProperties(objetoConvertido, regexes);
                    }
                    proc = Newtonsoft.Json.JsonConvert.DeserializeObject<nfeProc>(objetoConvertido);
                }

                if (proc.NFe.infNFe != null && proc.NFe.infNFe.infAdic != null)
                    proc.NFe.infNFe.infAdic.infCpl = "NFE GERADA PELA SERPRO " + proc.NFe.infNFe?.infAdic?.infCpl ?? "";

                danfe = new DanfeFrNfe(proc, new NFe.Danfe.Base.NFe.ConfiguracaoDanfeNfe());
            }

            byte[] pdf = danfe.ExportarPdf();

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoDANFE, pdf);

            erro = string.Empty;
            return true;
        }

        public string EnviarEmailNFCe(int codigoNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unitOfWork);

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarPorNota(codigoNFe);
            if (arquivo == null)
                return "Nenhuma nota localizada.";

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            double cpfCnpjConsumidorFinal = !string.IsNullOrWhiteSpace(nfe.CPFCNPJConsumidorFinal) ? nfe.CPFCNPJConsumidorFinal.ToDouble() : 0d;
            if (cpfCnpjConsumidorFinal == 0d)
                return "Essa nota não possui cliente";

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjConsumidorFinal);
            if (cliente == null)
                return "O cliente não está cadastrado";

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(nfe.Empresa.Codigo);
            if (email == null)
                throw new Exception("Não há um e-mail configurado para realizar o envio.");

            CriarConfiguracaoPadrao(nfe, ModeloDocumento.NFCe, unitOfWork);

            try
            {
                List<string> emails = new List<string>();

                if (!string.IsNullOrWhiteSpace(cliente.Email))
                    emails.AddRange(cliente.Email.Split(';').ToList());

                Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(nfe.Empresa.Codigo, cliente.CPF_CNPJ);
                if (dadosCliente != null && !string.IsNullOrWhiteSpace(dadosCliente.Email))
                    emails.AddRange(dadosCliente.Email.Split(';').ToList());

                foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail in cliente.Emails)
                {
                    if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A" && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Cobranca
                        && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Financeiro)
                        emails.AddRange(outroEmail.Email.Split(';').ToList());
                }

                if (emails.Count == 0)
                    return "Nenhum e-mail informado no cadastro do cliente.";

                string xmlNota = arquivo.XMLDistribuicao;
                string xmlCancelamento = arquivo.XMLCancelamento;

                if (string.IsNullOrWhiteSpace(xmlNota))
                    return "Nenhum xml de distribuição da nota localizado.";

                byte[] arquivoPdf = ObterPdfDANFCe(codigoNFe, nfe.Chave, unitOfWork, out string nomeArquivoPdf, out string retorno);
                if (!string.IsNullOrWhiteSpace(retorno))
                    return retorno;

                List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();

                attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(arquivoPdf), nomeArquivoPdf));
                attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(Encoding.UTF8.GetBytes(xmlNota)), string.Concat(nfe.Chave, ".xml")));

                if (!string.IsNullOrWhiteSpace(xmlCancelamento))
                    attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(Encoding.UTF8.GetBytes(xmlCancelamento)), string.Concat(nfe.Chave, "-Cancelamento.xml")));

                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    _configuracoes.ConfiguracaoEmail.Mensagem += "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");

                emails = emails.Distinct().ToList();
                string mensagemErro = "Erro ao enviar e-mail";
                bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, null, emails.ToArray(), _configuracoes.ConfiguracaoEmail.Assunto, _configuracoes.ConfiguracaoEmail.Mensagem, email.Smtp, out mensagemErro,
                    email.DisplayEmail, attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, nfe.Empresa.Codigo);
                if (!sucesso)
                    throw new Exception("Problemas ao enviar a Nota Fiscal por e-mail: " + mensagemErro);

                return string.Empty;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;

                return "Problemas ao enviar a DANFCe da nota selecionada.";
            }
        }
        public string EnviarEmailNFe(int codigoNFe, Repositorio.UnitOfWork unitOfWork, string relatorio, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario, string urlBase)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unitOfWork);

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarPorNota(codigoNFe);
            if (arquivo == null)
                return "Nenhuma nota localizada.";

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(nfe.Empresa.Codigo);
            if (email == null)
                throw new Exception("Não há um e-mail configurado para realizar o envio.");

            CriarConfiguracaoPadrao(nfe, ModeloDocumento.NFe, unitOfWork);

            try
            {
                Dominio.Entidades.Cliente cliente = nfe.Cliente;

                List<string> emails = new List<string>();

                string grupoPessoasModeloDocumentoEmail = cliente.GrupoPessoas?.EmailsModeloDocumento?.Where(o => o.ModeloDocumentoFiscal.Numero == "55").Select(o => o.Emails).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(grupoPessoasModeloDocumentoEmail))
                    emails.AddRange(grupoPessoasModeloDocumentoEmail.Split(';').ToList());
                else if (!string.IsNullOrWhiteSpace(cliente.Email))
                    emails.AddRange(cliente.Email.Split(';').ToList());

                Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(nfe.Empresa.Codigo, cliente.CPF_CNPJ);
                if (dadosCliente != null && !string.IsNullOrWhiteSpace(dadosCliente.Email))
                    emails.AddRange(dadosCliente.Email.Split(';').ToList());

                if (!string.IsNullOrWhiteSpace(cliente.EmailContador))
                    emails.AddRange(cliente.EmailContador.Split(';').ToList());

                if (cliente.Emails?.Count() > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail in cliente.Emails)
                    {
                        if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A" && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Cobranca
                            && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Financeiro)
                            emails.AddRange(outroEmail.Email.Split(';').ToList());
                    }
                }

                string diretorioRaiz = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e" });

                string xmlNota = arquivo.XMLDistribuicao;
                string xmlCancelamento = arquivo.XMLCancelamento;
                nfeProc proc = new nfeProc().CarregarDeXmlString(xmlNota);
                string arquivoXML = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, nfe.Chave + ".xml");
                string arquivoXMLCancelamento = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, nfe.Chave + "-Cancelado.xml");

                if (string.IsNullOrWhiteSpace(xmlNota))
                    return "Nenhum xml de distribuição da nota localizado.";

                proc.SalvarArquivoXml(arquivoXML);

                if (!string.IsNullOrWhiteSpace(xmlCancelamento))
                {
                    Utilidades.IO.FileStorageService.Storage.WriteLine(arquivoXMLCancelamento, xmlCancelamento);
                }

                if (emails == null || emails.Count == 0)
                    throw new ArgumentException("Nenhum e-mail informado no cadastro do cliente.");

                if (usuario != null && usuario.Empresa != null)
                {
                    if (!string.IsNullOrWhiteSpace(usuario.Empresa.Email) && usuario.Empresa.StatusEmail == "A")
                        emails.AddRange(usuario.Empresa.Email.Split(';').ToList());
                    if (!string.IsNullOrWhiteSpace(usuario.Empresa.EmailAdministrativo) && usuario.Empresa.StatusEmailAdministrativo == "A")
                        emails.AddRange(usuario.Empresa.EmailAdministrativo.Split(';').ToList());
                    if (!string.IsNullOrWhiteSpace(usuario.Empresa.EmailContador) && usuario.Empresa.StatusEmailContador == "A")
                        emails.AddRange(usuario.Empresa.EmailContador.Split(';').ToList());
                }

                string novoCaminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, nfe.Chave + ".pdf");
                if (!Utilidades.IO.FileStorageService.Storage.Exists(novoCaminhoPDF))
                    novoCaminhoPDF = GerarDANFECrystal(nfe, usuario, caminhoRelatoriosEmbarcador, unitOfWork);

                //Enviar e-mail pelo serviço padrão Multi
                List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                attachments.Add(new System.Net.Mail.Attachment(arquivoXML));
                if (Utilidades.IO.FileStorageService.Storage.Exists(arquivoXMLCancelamento))
                    attachments.Add(new System.Net.Mail.Attachment(arquivoXMLCancelamento));
                if (Utilidades.IO.FileStorageService.Storage.Exists(novoCaminhoPDF))
                    attachments.Add(new System.Net.Mail.Attachment(novoCaminhoPDF));

                string mensagemErro = "Erro ao enviar e-mail";
                if (!unitOfWork.IsActiveTransaction())
                {
                    unitOfWork = new Repositorio.UnitOfWork(unitOfWork.StringConexao);
                    unitOfWork.Start();
                }
                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    _configuracoes.ConfiguracaoEmail.Mensagem += "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPrimeiroPorNota(codigoNFe);

                if (titulo != null && !string.IsNullOrEmpty(urlBase))
                {
                    string portalClienteCodigo = Servicos.Embarcador.Financeiro.Titulo.ObterPortalClienteCodigo(titulo, repTitulo);
                    string portalClienteUrl = Servicos.Embarcador.Financeiro.Titulo.ObterURLPortalClienteCodigo(urlBase, portalClienteCodigo);
                    _configuracoes.ConfiguracaoEmail.Mensagem += $"<br/><br/>Link de acesso aos dados da compra: <a href=\"{portalClienteUrl}\" title=\"Dados da compra\">{portalClienteUrl}</a>";
                }

                emails = emails.Distinct().ToList();
                bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, null, emails.ToArray(), _configuracoes.ConfiguracaoEmail.Assunto, _configuracoes.ConfiguracaoEmail.Mensagem, email.Smtp, out mensagemErro, email.DisplayEmail,
                    attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, nfe.Empresa.Codigo);
                if (!sucesso)
                    throw new Exception("Problemas ao enviar a Nota Fiscal por e-mail: " + mensagemErro);

                unitOfWork.CommitChanges();

                return "";
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;
                else
                    return "Problemas ao criar DANFE da nota selecionada.";
            }
        }
        public string EnviarEmailCCe(int codigoNFe, int codigoCCe, Repositorio.UnitOfWork unitOfWork, string relatorio, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao repNotaFiscalCartaCorrecao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unitOfWork);

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarUltimoPorNota(codigoNFe);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao carta = repNotaFiscalCartaCorrecao.BuscarPorCodigo(codigoCCe);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNotaFiscal.BuscarPorCodigo(codigoNFe);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(nfe.Empresa.Codigo);

            if (email == null)
                throw new Exception("Não há um e-mail configurado para realizar o envio.");

            CriarConfiguracaoPadrao(nfe, ModeloDocumento.NFe, unitOfWork);

            string htmlEmail = "<html>" +
                            "   <h3> Carta de Correção Eletrônica (CC-e)</h3>" +
                            "   <h4> Prezado Cliente " + nfe.Cliente.Nome + " </h4>" +
                            "   <p> Segue anexo a Carta de Correção Eletrônica(xml e pdf).</p>" +
                            "   <p>" +
                            "       <br>Em face do que determina a legislacao fiscal vigente, vimos pela presente comunicar-lhe(s) " +
                            "       <br>que a Nota Fiscal Eletronica em Referencia identificada pela Chave de Acesso " + nfe.Chave +
                            "       <br>contem a(s) irregularidade(s) que apontamos, cuja correcao solicitamos seja providenciada imediatamente:  " +
                            "       <br>" + carta.Mensagem +
                            "   </p>" +
                            "Commerce Sistemas LTDA" +
                            "</html>";

            _configuracoes.ConfiguracaoEmail.Assunto = "Carta de Correção NFe: " + nfe.Numero.ToString();
            _configuracoes.ConfiguracaoEmail.Mensagem = htmlEmail;

            if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                _configuracoes.ConfiguracaoEmail.Mensagem += "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");

            if (arquivo == null || string.IsNullOrWhiteSpace(arquivo.XMLCartaCorrecao))
                return "Nenhuma carta de correção localizada.";

            try
            {
                Dominio.Entidades.Cliente cliente = nfe.Cliente;

                List<string> emails = new List<string>();
                if (!string.IsNullOrWhiteSpace(cliente.Email))
                    emails.AddRange(cliente.Email.Split(';').ToList());

                Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(nfe.Empresa.Codigo, cliente.CPF_CNPJ);
                if (dadosCliente != null && !string.IsNullOrWhiteSpace(dadosCliente.Email))
                    emails.AddRange(dadosCliente.Email.Split(';').ToList());

                if (!string.IsNullOrWhiteSpace(cliente.EmailContador))
                    emails.AddRange(cliente.EmailContador.Split(';').ToList());

                if (cliente.Emails?.Count() > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail in cliente.Emails)
                    {
                        if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A" && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Cobranca
                            && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Financeiro)
                            emails.AddRange(outroEmail.Email.Split(';').ToList());
                    }
                }

                string diretorioRaiz = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e" });
                string xmlNota = arquivo.XMLCartaCorrecao;
                string arquivoXML = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, nfe.Chave + "CCe.xml");

                if (string.IsNullOrWhiteSpace(xmlNota))
                    return "Nenhum xml de carta de correção da nota localizado.";

                Utilidades.IO.FileStorageService.Storage.WriteLine(arquivoXML, xmlNota);

                if (emails == null || emails.Count == 0)
                    throw new ArgumentException("Nenhum e-mail informado no cadastro do cliente.");

                if (usuario != null && usuario.Empresa != null)
                {
                    if (!string.IsNullOrWhiteSpace(usuario.Empresa.Email) && usuario.Empresa.StatusEmail == "A")
                        emails.AddRange(usuario.Empresa.Email.Split(';').ToList());
                    if (!string.IsNullOrWhiteSpace(usuario.Empresa.EmailAdministrativo) && usuario.Empresa.StatusEmailAdministrativo == "A")
                        emails.AddRange(usuario.Empresa.EmailAdministrativo.Split(';').ToList());
                    if (!string.IsNullOrWhiteSpace(usuario.Empresa.EmailContador) && usuario.Empresa.StatusEmailContador == "A")
                        emails.AddRange(usuario.Empresa.EmailContador.Split(';').ToList());
                }

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = repRelatorio.BuscarPorTitulo("Carta de Correção Eletrônica - CCe");
                string novoCaminhoPDF = "";
                if (relatorioOrigem != null)
                {
                    Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = new Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio();
                    dynRelatorio.Codigo = relatorioOrigem.Codigo;
                    dynRelatorio.CodigoControleRelatorios = relatorioOrigem.CodigoControleRelatorios;
                    dynRelatorio.Titulo = relatorioOrigem.Titulo;
                    dynRelatorio.Descricao = relatorioOrigem.Descricao;
                    dynRelatorio.Padrao = relatorioOrigem.Padrao;
                    dynRelatorio.ExibirSumarios = relatorioOrigem.ExibirSumarios;
                    dynRelatorio.CortarLinhas = relatorioOrigem.CortarLinhas;
                    dynRelatorio.FundoListrado = relatorioOrigem.FundoListrado;
                    dynRelatorio.TamanhoPadraoFonte = relatorioOrigem.TamanhoPadraoFonte;
                    dynRelatorio.FontePadrao = relatorioOrigem.FontePadrao;
                    dynRelatorio.AgruparRelatorio = false;
                    dynRelatorio.PropriedadeAgrupa = relatorioOrigem.PropriedadeAgrupa;
                    dynRelatorio.OrdemAgrupamento = relatorioOrigem.OrdemAgrupamento;
                    dynRelatorio.PropriedadeOrdena = relatorioOrigem.PropriedadeOrdena;
                    dynRelatorio.OrdemOrdenacao = relatorioOrigem.OrdemOrdenacao;
                    dynRelatorio.TipoArquivoRelatorio = Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
                    dynRelatorio.OrientacaoRelatorio = relatorioOrigem.OrientacaoRelatorio;
                    dynRelatorio.Grid = "{\"draw\":0,\"inicio\":0,\"limite\":0,\"indiceColunaOrdena\":0,\"dirOrdena\":\"desc\",\"recordsTotal\":0,\"recordsFiltered\":0,\"group\":{\"enable\":false,\"propAgrupa\":null,\"dirOrdena\":null},\"header\":[{\"title\":\"Cód. Produto\",\"data\":\"CodigoProduto\",\"width\":\"10%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":0,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Descrição dos Produtos/Serviços\",\"data\":\"DescricaoItem\",\"width\":\"25%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-left\",\"tabletHide\":false,\"phoneHide\":false,\"position\":1,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"NCM\",\"data\":\"CodigoNCMItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":2,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"CST/CSOSN\",\"data\":\"DescricaoCSTItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":3,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"CFOP\",\"data\":\"CodigoCFOPItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":4,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Unid.\",\"data\":\"DescricaoUnidadeItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":5,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Quantidade\",\"data\":\"QuantidadeItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":6,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"V. Unitário\",\"data\":\"ValorUnitarioItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":7,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"V. Total\",\"data\":\"ValorTotalItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":8,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"B.C. ICMS\",\"data\":\"BCICMSItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":9,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Valor ICMS\",\"data\":\"ValorICMSItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":10,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Valor IPI\",\"data\":\"ValorIPIItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":11,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"% ICMS\",\"data\":\"AliquotaICMSItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":12,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"% IPI\",\"data\":\"AliquotaIPIItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":13,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0}],\"data\":null,\"dataSumarizada\":null,\"order\":[{\"column\":0,\"dir\":\"desc\"}]}";
                    dynRelatorio.Report = relatorioOrigem.Codigo;
                    dynRelatorio.NovoRelatorio = true;

                    Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                    unitOfWork.CommitChanges();

                    Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware.MultiTMS);

                    string propOrdena = relatorioTemp.PropriedadeOrdena;

                    List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = null;

                    relatorioTemp.PropriedadeOrdena = propOrdena;

                    Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);
                    serNotaFiscalEletronica.GerarRelatorioCCeNFe(carta, usuario.Empresa.Codigo, agrupamentos, relatorioControleGeracao, relatorioTemp, unitOfWork.StringConexao);

                    string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, relatorioControleGeracao.GuidArquivo) + ".pdf";
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                    {
                        novoCaminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, nfe.Chave + "CCe.pdf");
                        if (Utilidades.IO.FileStorageService.Storage.Exists(novoCaminhoPDF))
                            Utilidades.IO.FileStorageService.Storage.Delete(novoCaminhoPDF);
                        Utilidades.IO.FileStorageService.Storage.Copy(caminhoArquivo, novoCaminhoPDF);
                    }
                }

                //Enviar e-mail pelo serviço padrão Multi
                List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                attachments.Add(new System.Net.Mail.Attachment(arquivoXML));
                if (Utilidades.IO.FileStorageService.Storage.Exists(novoCaminhoPDF))
                    attachments.Add(new System.Net.Mail.Attachment(novoCaminhoPDF));

                string mensagemErro = "Erro ao enviar e-mail";
                if (!unitOfWork.IsActiveTransaction())
                {
                    unitOfWork = new Repositorio.UnitOfWork(unitOfWork.StringConexao);
                    unitOfWork.Start();
                }

                bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, null, emails.ToArray(), _configuracoes.ConfiguracaoEmail.Assunto, _configuracoes.ConfiguracaoEmail.Mensagem, email.Smtp, out mensagemErro, email.DisplayEmail,
                    attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, nfe.Empresa.Codigo);
                if (!sucesso)
                    throw new Exception("Problemas ao enviar a CCe da Nota Fiscal por e-mail: " + mensagemErro);

                unitOfWork.CommitChanges();

                return "";
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;
                else
                    return "Problemas ao criar CCe da nota selecionada.";
            }
        }

        public string InutilizarNFe(int codigoNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            if (!string.IsNullOrWhiteSpace(nfe.Empresa.Tipo) && nfe.Empresa.Tipo.Equals("F"))
                throw new ServicoException("Somente é permitida a inutilização para emitentes Pessoa Jurídica!");

            if (nfe.Status == Dominio.Enumeradores.StatusNFe.Inutilizado)
                throw new ServicoException("A Nota Fiscal já está Inutilizada! Não sendo possível inutilizar novamente.");

            if (nfe.ModeloNotaFiscal == "65")
                CriarConfiguracaoPadrao(nfe, ModeloDocumento.NFCe, unitOfWork);
            else
                CriarConfiguracaoPadrao(nfe, ModeloDocumento.NFe, unitOfWork);
            try
            {
                var anoAtual = Convert.ToString(DateTime.Now.Year).Substring(2, 2);

                if (string.IsNullOrWhiteSpace(nfe.Empresa.NomeCertificado))
                {
                    var retornoAssinatura = ValidarDocumentosAguardandoAssinatura(nfe, Dominio.Enumeradores.StatusNFe.AguardandoInutilizarAssinar, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(retornoAssinatura))
                        return retornoAssinatura;

                    SalvarStatusNota(nfe, unitOfWork, "Aguardando Inutilizacao do XML", 0, Dominio.Enumeradores.StatusNFe.AguardandoInutilizarAssinar, "", "", null);
                    string xmlSalvar = nfe.Empresa.CNPJ_Formatado + "|" + anoAtual + "|" + Convert.ToInt16(nfe.EmpresaSerie.Numero) + "|" + nfe.Numero + "|" + "FAIXA DE NUMERACAO NAO SERA MAIS UTILIZADA";
                    SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.XMLInutilizacaoNaoAssinado, xmlSalvar);
                    return "";
                }
                else if (nfe.Empresa.DataFinalCertificado < DateTime.Now)
                    throw new ServicoException("Seu Certificado Digital está vencido! Favor atualizar o mesmo no sistema para prosseguir.");

                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);

                var retornoConsulta = servicoNFe.NfeInutilizacao(Utilidades.String.OnlyNumbers(nfe.Empresa.CNPJ_Formatado), Convert.ToInt16(anoAtual),
                    nfe.ModeloNotaFiscal == "65" ? ModeloDocumento.NFCe : ModeloDocumento.NFe, Convert.ToInt16(nfe.EmpresaSerie.Numero), Convert.ToInt32(nfe.Numero),
                    Convert.ToInt32(nfe.Numero), "FAIXA DE NUMERACAO NAO SERA MAIS UTILIZADA");

                if (retornoConsulta.Retorno.infInut.cStat == 102 || retornoConsulta.Retorno.infInut.cStat == 135 || retornoConsulta.Retorno.infInut.cStat == 206 || retornoConsulta.Retorno.infInut.cStat == 256 || retornoConsulta.Retorno.infInut.cStat == 563 || retornoConsulta.Retorno.infInut.cStat == 662)
                {
                    SalvarStatusNota(nfe, unitOfWork, retornoConsulta.Retorno.infInut.xMotivo, retornoConsulta.Retorno.infInut.cStat, Dominio.Enumeradores.StatusNFe.Inutilizado, "", "", null);
                    SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Inutilizacao, retornoConsulta.EnvioStr);
                    SalvarInutilizacaoNota(nfe, unitOfWork, retornoConsulta.Retorno.infInut.dhRecbto);

                    return "";
                }
                else
                {
                    SalvarStatusNota(nfe, unitOfWork, retornoConsulta.Retorno.infInut.xMotivo, retornoConsulta.Retorno.infInut.cStat, Dominio.Enumeradores.StatusNFe.Emitido, "", "", null);
                    return retornoConsulta.Retorno.infInut.xMotivo;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;
                else
                    return "Problemas ao criar DANFE da nota selecionada.";
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe InutilizarFaixaNFe(string serieNFe, string numeroInicial, string numeroFinal, string justificativa, int cUF, string modelo, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe ret = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe();

            CriarConfiguracaoPadrao(empresa, ModeloDocumento.NFe, unitOfWork);

            try
            {
                var anoAtual = Convert.ToString(DateTime.Now.Year).Substring(2, 2);

                if (empresa.DataFinalCertificado < DateTime.Now)
                    throw new ServicoException("Seu Certificado Digital está vencido! Favor atualizar o mesmo no sistema para prosseguir.");

                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);

                var retornoConsulta = servicoNFe.NfeInutilizacao(Utilidades.String.OnlyNumbers(empresa.CNPJ_Formatado), Convert.ToInt16(anoAtual),
                    modelo == "65" ? ModeloDocumento.NFCe : ModeloDocumento.NFe, Convert.ToInt16(serieNFe), Convert.ToInt32(numeroInicial),
                    Convert.ToInt32(numeroFinal), justificativa);

                ret.nRec = "";
                ret.cStat = retornoConsulta.Retorno.infInut.cStat.ToString();
                ret.xMotivo = retornoConsulta.Retorno.infInut.xMotivo;
                ret.Status = Dominio.Enumeradores.StatusNFe.Emitido;
                ret.chNFe = "";
                ret.nProt = retornoConsulta.Retorno.infInut.nProt;
                ret.dhRecbto = null;
                ret.XML = "";
                ret.TipoArquivoXML = Dominio.Enumeradores.TipoArquivoXML.Inutilizacao;
                return ret;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    ret.xMotivo = ret.cStat + " - " + ex.Message;

                return ret;
            }
        }

        public string CancelarNFe(DateTime dataEmissao, int codigoNFe, Repositorio.UnitOfWork unitOfWork, string justificativa, string relatorio, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            if (repTitulo.ContemTitulosPagosNotaFiscal(codigoNFe))
                throw new ServicoException("Esta Nota Fiscal já possui títulos quitados! Favor reverta as baixas dos mesmos.");

            if (dataEmissao == DateTime.MinValue)
                throw new ServicoException("Por favor informe a data do cancelamento.");

            if (nfe.Status == Dominio.Enumeradores.StatusNFe.Cancelado)
                throw new ServicoException("A Nota Fiscal já está Cancelada! Não sendo possível cancelar novamente.");

            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
            Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoMensalClienteServico = repFaturamentoMensalClienteServico.BuscarPorNFe(codigoNFe);

            if (faturamentoMensalClienteServico != null && faturamentoMensalClienteServico.FaturamentoMensal.StatusFaturamentoMensal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado)
                throw new ServicoException("Favor Finalizar o Faturamento Mensal que contêm essa NF-e antes de Cancelar");

            justificativa = justificativa.Trim().TrimEnd().TrimStart().Replace("º", "").Replace("ª", "").Replace("\n", " - ");

            if (string.IsNullOrWhiteSpace(nfe.Empresa.NomeCertificado))
            {
                var retornoAssinatura = ValidarDocumentosAguardandoAssinatura(nfe, Dominio.Enumeradores.StatusNFe.AguardandoCancelarAssinar, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retornoAssinatura))
                    return retornoAssinatura;

                SalvarStatusNota(nfe, unitOfWork, "Aguardando Cancelamento do XML", 0, Dominio.Enumeradores.StatusNFe.AguardandoCancelarAssinar, "", "", null);
                string xmlSalvar = nfe.Codigo + "|" + "1" + "|" + nfe.Protocolo + "|" + nfe.Chave + "|" + justificativa + "|" + nfe.Empresa.CNPJ_Formatado + "|" + dataEmissao.ToString("dd/MM/yyyy HH:mm");
                SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.XMLCancelamentoNaoAssinado, xmlSalvar);
                return "";
            }
            else if (nfe.Empresa.DataFinalCertificado < DateTime.Now)
                throw new ServicoException("Seu Certificado Digital está vencido! Favor atualizar o mesmo no sistema para prosseguir.");

            ModeloDocumento modNFe;
            if (nfe.ModeloNotaFiscal == "55")
                modNFe = ModeloDocumento.NFe;
            else
                modNFe = ModeloDocumento.NFCe;

            CriarConfiguracaoPadrao(nfe, modNFe, unitOfWork);
            try
            {
                ServicosNFe servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);
                Servicos.SMS srvSMS = new Servicos.SMS(unitOfWork);

                var retornoConsulta = servicoNFe.RecepcaoEventoCancelamento(Convert.ToInt32(nfe.Codigo),
                    Convert.ToInt16(1), nfe.Protocolo, nfe.Chave, justificativa, Utilidades.String.OnlyNumbers(nfe.Empresa.CNPJ_Formatado), dataEmissao);

                if (retornoConsulta.Retorno.cStat == 101 || retornoConsulta.Retorno.cStat == 151 || retornoConsulta.Retorno.cStat == 218 || retornoConsulta.Retorno.cStat == 420)
                {
                    SalvarStatusNota(nfe, unitOfWork, retornoConsulta.Retorno.xMotivo, retornoConsulta.Retorno.cStat, Dominio.Enumeradores.StatusNFe.Cancelado, "", "", null);
                    var procevento = retornoConsulta.ProcEventosNFe[0];
                    var proceventoXmlString = procevento.ObterXmlString().Replace("<retEvento>", "").Replace("</retEvento>", "").Replace("</nProt></infEvento>", "</nProt></infEvento></retEvento>");
                    SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Cancelamento, proceventoXmlString);
                    SalvarCancelamentoNota(nfe, unitOfWork, DateTime.Now, justificativa, nfe.Protocolo);
                    MovimentarEstoqueNota(codigoNFe, unitOfWork, nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Entrada, tipoServicoMultisoftware);
                    if (modNFe == ModeloDocumento.NFe)
                    {
                        ReverteTituloNota(codigoNFe, unitOfWork);
                        EstornarPedidos(codigoNFe, unitOfWork);
                        GerarDANFECrystal(nfe, usuario, caminhoRelatoriosEmbarcador, unitOfWork);
                        EnviarEmailNFe(codigoNFe, unitOfWork, relatorio, caminhoRelatoriosEmbarcador, usuario, "");
                        if (nfe.Empresa.ArmazenarDanfeParaSMS)
                            serNotaFiscalEletronica.GerarDanfeParaSMS(unitOfWork, nfe, usuario);
                        if (nfe.Empresa.AtivarEnvioDanfeSMS)
                            return srvSMS.EnviarMensagem(nfe, clienteAcesso.URLAcesso, unitOfWork);
                    }
                    else
                    {
                        EstornarPedidos(codigoNFe, unitOfWork);
                        ReverteTituloNotaConsumidor(codigoNFe, unitOfWork);
                        EnviarEmailNFCe(codigoNFe, unitOfWork);
                    }
                    return "";
                }
                else if (retornoConsulta.Retorno.retEvento[0].infEvento.cStat == 135 || retornoConsulta.Retorno.retEvento[0].infEvento.cStat == 155 || retornoConsulta.Retorno.retEvento[0].infEvento.cStat == 573)
                {
                    SalvarStatusNota(nfe, unitOfWork, retornoConsulta.Retorno.retEvento[0].infEvento.xMotivo, retornoConsulta.Retorno.retEvento[0].infEvento.cStat, Dominio.Enumeradores.StatusNFe.Cancelado, "", "", null);
                    var procevento = retornoConsulta.ProcEventosNFe[0];
                    var proceventoXmlString = procevento.ObterXmlString().Replace("<retEvento>", "").Replace("</retEvento>", "").Replace("</nProt></infEvento>", "</nProt></infEvento></retEvento>");
                    SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Cancelamento, proceventoXmlString);
                    SalvarCancelamentoNota(nfe, unitOfWork, DateTime.Now, justificativa, nfe.Protocolo);
                    MovimentarEstoqueNota(codigoNFe, unitOfWork, nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Entrada, tipoServicoMultisoftware);
                    if (modNFe == ModeloDocumento.NFe)
                    {
                        ReverteTituloNota(codigoNFe, unitOfWork);
                        EstornarPedidos(codigoNFe, unitOfWork);
                        GerarDANFECrystal(nfe, usuario, caminhoRelatoriosEmbarcador, unitOfWork);
                        EnviarEmailNFe(codigoNFe, unitOfWork, relatorio, caminhoRelatoriosEmbarcador, usuario, "");
                        if (nfe.Empresa.ArmazenarDanfeParaSMS)
                            serNotaFiscalEletronica.GerarDanfeParaSMS(unitOfWork, nfe, usuario);
                        if (nfe.Empresa.AtivarEnvioDanfeSMS)
                            return srvSMS.EnviarMensagem(nfe, clienteAcesso.URLAcesso, unitOfWork);
                    }
                    else
                    {
                        ReverteTituloNotaConsumidor(codigoNFe, unitOfWork);
                        EstornarPedidos(codigoNFe, unitOfWork);
                        EnviarEmailNFCe(codigoNFe, unitOfWork);
                    }
                    return "";
                }
                else
                {
                    SalvarStatusNota(nfe, unitOfWork, retornoConsulta.Retorno.retEvento[0].infEvento.xMotivo, retornoConsulta.Retorno.retEvento[0].infEvento.cStat, Dominio.Enumeradores.StatusNFe.Autorizado, "", "", null);
                    return retornoConsulta.Retorno.retEvento[0].infEvento.xMotivo;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;
                else
                    return "Problemas ao cancelar a nota selecionada.";
            }
        }

        public string CartaCorrecaoNFe(DateTime dataEmissao, int codigoNFe, string correcao, Repositorio.UnitOfWork unitOfWork, string relatorio, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao repNotaFiscalCartaCorrecao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            if (dataEmissao == DateTime.MinValue)
                return "Por favor informe a data da CCe.";

            int sequenciaEvento = repNotaFiscalCartaCorrecao.BuscarQuantidadeCartaCorrecao(codigoNFe);
            correcao = correcao.Trim().TrimEnd().TrimStart();

            if (string.IsNullOrWhiteSpace(nfe.Empresa.NomeCertificado))
            {
                var retornoAssinatura = ValidarDocumentosAguardandoAssinatura(nfe, Dominio.Enumeradores.StatusNFe.AguardandoCartaCorrecaoAssinar, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retornoAssinatura))
                    return retornoAssinatura;

                SalvarStatusNota(nfe, unitOfWork, "Aguardando Carta Correção do XML", 0, Dominio.Enumeradores.StatusNFe.AguardandoCartaCorrecaoAssinar, "", "", null);
                string xmlSalvar = nfe.Codigo + "|" + sequenciaEvento + "|" + nfe.Chave + "|" + correcao + "|" + nfe.Empresa.CNPJ_Formatado + "|" + dataEmissao.ToString("dd/MM/yyyy HH:mm");
                SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.XMLCartaCorrecaoNaoAssinado, xmlSalvar);
                return "";
            }
            else if (nfe.Empresa.DataFinalCertificado < DateTime.Now)
                return "Seu Certificado Digital está vencido! Favor atualizar o mesmo no sistema para prosseguir.";

            CriarConfiguracaoPadrao(nfe, ModeloDocumento.NFe, unitOfWork);
            try
            {
                //_configuracoes.CfgServico.ValidarSchemas = false;
                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);

                //Servicos.Log.TratarErro("0-cartacorretao.txt");

                var retornoConsulta = servicoNFe.RecepcaoEventoCartaCorrecao(nfe.Codigo, Convert.ToInt16(sequenciaEvento), nfe.Chave, correcao.Replace("º", "").Replace("ª", "").Replace("\n", " - "), nfe.Empresa.CNPJ_SemFormato);

                //Servicos.Log.TratarErro("1-cartacorretao.txt");

                if (retornoConsulta.Retorno.retEvento[0].infEvento.cStat == 135)
                {
                    //Servicos.Log.TratarErro("retorno sucesso.txt");
                    var procevento = retornoConsulta.ProcEventosNFe[0];
                    var proceventoXmlString = procevento.ObterXmlString().Replace("<retEvento>", "").Replace("</retEvento>", "").Replace("</nProt></infEvento>", "</nProt></infEvento></retEvento>");

                    SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.CartaCorrecao, proceventoXmlString);
                    int codigoCCe = SalvarCartaCorrecaoNota(nfe, correcao, unitOfWork, DateTime.Now, retornoConsulta.Retorno.retEvento[0].infEvento.nProt, nfe.Codigo.ToString());
                    EnviarEmailCCe(codigoNFe, codigoCCe, unitOfWork, relatorio, caminhoRelatoriosEmbarcador, usuario);
                    return "";
                }
                else
                {
                    //Servicos.Log.TratarErro("retorno erro carta corretao.txt");
                    //Servicos.Log.TratarErro("EnvioStr-" + retornoConsulta.EnvioStr);
                    //Servicos.Log.TratarErro("RetornoStr-" + retornoConsulta.RetornoStr);
                    //Servicos.Log.TratarErro("RetornoCompletoStr-" + retornoConsulta.RetornoCompletoStr);
                    return retornoConsulta.Retorno.retEvento[0].infEvento.xMotivo;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;
                else
                    return "Problemas ao criar DANFE da nota selecionada.";
            }
        }

        public string SalvarNFeAnterior(int codigoEmpresa, System.IO.Stream xml, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            XDocument doc = XDocument.Load(xml);

            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            doc.WriteTo(xw);

            string arquivoXML = sw.ToString();

            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);

            try
            {
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();
                var proc = new nfeProc().CarregarDeXmlString(arquivoXML);

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente;
                if (proc.NFe.infNFe.ide.tpAmb == TipoAmbiente.Homologacao)
                    tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Homologacao;
                else
                    tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao;

                string modeloNF = "55";
                if (proc.NFe.infNFe.ide.mod == ModeloDocumento.NFe)
                    modeloNF = "55";
                else
                    modeloNF = "65";

                if (repNFe.NumeracaoJaEmitida(codigoEmpresa, (int)proc.NFe.infNFe.ide.nNF, proc.NFe.infNFe.ide.serie, tipoAmbiente, modeloNF))
                    return "Número " + proc.NFe.infNFe.ide.nNF.ToString() + " e série " + proc.NFe.infNFe.ide.serie.ToString() + " já lançada no sistema.";

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal();
                nfe.ModeloNotaFiscal = modeloNF;
                nfe.Numero = (int)proc.NFe.infNFe.ide.nNF;
                nfe.EmpresaSerie = CadastrarSerieEmpresa(proc.NFe.infNFe.ide.serie, unitOfWork, codigoEmpresa);//repEmpresaSerie.BuscarPorSerie(codigoEmpresa, proc.NFe.infNFe.ide.serie, Dominio.Enumeradores.TipoSerie.NFe);
                if (nfe.EmpresaSerie == null)
                    return "Série " + proc.NFe.infNFe.ide.serie.ToString() + " de NFe não cadastrada.";
                if (proc.NFe.infNFe.ide.tpNF == TipoNFe.tnEntrada)
                    nfe.TipoEmissao = Dominio.Enumeradores.TipoEmissaoNFe.Entrada;
                else
                    nfe.TipoEmissao = Dominio.Enumeradores.TipoEmissaoNFe.Saida;
                DateTime dataEmissao = proc.NFe.infNFe.ide.dhEmi.DateTime;


                nfe.DataEmissao = dataEmissao;
                DateTime dataSaida;
                if (proc.NFe.infNFe.ide.dhSaiEnt.HasValue)
                    dataSaida = proc.NFe.infNFe.ide.dhSaiEnt.Value.DateTime;
                else
                    dataSaida = DateTime.Now;
                nfe.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                if ((Utilidades.String.OnlyNumbers(nfe.Empresa.CNPJ) != proc.NFe.infNFe.emit.CNPJ && nfe.Empresa.Tipo.Equals("J")) ||
                    (Utilidades.String.OnlyNumbers(nfe.Empresa.CNPJ) != proc.NFe.infNFe.emit.CPF && nfe.Empresa.Tipo.Equals("F")))
                    return "Nota fiscal não foi emitida pela empresa selecionada.";

                nfe.DataSaida = dataSaida;
                nfe.Chave = Utilidades.String.OnlyNumbers(proc.NFe.infNFe.Id);
                nfe.Status = Dominio.Enumeradores.StatusNFe.Autorizado;
                nfe.Protocolo = proc.protNFe != null ? proc.protNFe.infProt.nProt : "SEM PROTOCOLO";
                nfe.DataProcessamento = proc.protNFe != null ? proc.protNFe.infProt.dhRecbto.DateTime : DateTime.Now;
                nfe.DataPrestacaoServico = dataEmissao;
                nfe.TipoAmbiente = tipoAmbiente;

                if (proc.NFe.infNFe.ide.finNFe == FinalidadeNFe.fnAjuste)
                    nfe.Finalidade = Dominio.Enumeradores.FinalidadeNFe.Ajuste;
                else if (proc.NFe.infNFe.ide.finNFe == FinalidadeNFe.fnComplementar)
                    nfe.Finalidade = Dominio.Enumeradores.FinalidadeNFe.Complementar;
                else if (proc.NFe.infNFe.ide.finNFe == FinalidadeNFe.fnDevolucao)
                    nfe.Finalidade = Dominio.Enumeradores.FinalidadeNFe.Devolucao;
                else if (proc.NFe.infNFe.ide.finNFe == FinalidadeNFe.fnNormal)
                    nfe.Finalidade = Dominio.Enumeradores.FinalidadeNFe.Normal;

                if (proc.NFe.infNFe.ide.indPres == PresencaComprador.pcInternet)
                    nfe.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.Internet;
                else if (proc.NFe.infNFe.ide.indPres == PresencaComprador.pcNao)
                    nfe.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.NaoSeAplica;
                else if (proc.NFe.infNFe.ide.indPres == PresencaComprador.pcEntregaDomicilio)
                    nfe.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.NFCe;
                else if (proc.NFe.infNFe.ide.indPres == PresencaComprador.pcOutros)
                    nfe.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.Outros;
                else if (proc.NFe.infNFe.ide.indPres == PresencaComprador.pcPresencial)
                    nfe.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.Presencial;
                else if (proc.NFe.infNFe.ide.indPres == PresencaComprador.pcTeleatendimento)
                    nfe.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.Teleatendimento;

                nfe.Cliente = CadastrarPessoa(proc.NFe.infNFe.dest, unitOfWork);
                if (nfe.Cliente == null && proc.NFe.infNFe.dest != null)
                    return "Não foi possível localizar/cadastrar o cliente " + proc.NFe.infNFe.dest.xNome + ".";
                if (proc.NFe.infNFe.dest != null)
                {
                    nfe.NaturezaDaOperacao = CadastrarNaturezaOperacao(proc.NFe.infNFe.ide.natOp, proc.NFe.infNFe.dest.enderDest.UF, nfe.Empresa.Localidade.Estado.Sigla, unitOfWork, codigoEmpresa);
                    if (nfe.NaturezaDaOperacao == null)
                        return "Empresa não possui natureza de operação cadastrada.";
                    nfe.Atividade = nfe.Cliente.Atividade;
                    nfe.LocalidadePrestacaoServico = nfe.Cliente.Localidade;
                }

                nfe.BCICMS = proc.NFe.infNFe.total.ICMSTot.vBC;
                nfe.ValorICMS = proc.NFe.infNFe.total.ICMSTot.vICMS;
                nfe.ICMSDesonerado = proc.NFe.infNFe.total.ICMSTot.vICMSDeson ?? 0;
                nfe.ValorII = proc.NFe.infNFe.total.ICMSTot.vII;
                nfe.BCICMSST = proc.NFe.infNFe.total.ICMSTot.vBCST;
                nfe.ValorProdutos = proc.NFe.infNFe.total.ICMSTot.vProd;
                nfe.ValorFrete = proc.NFe.infNFe.total.ICMSTot.vFrete;
                nfe.ValorSeguro = proc.NFe.infNFe.total.ICMSTot.vSeg;
                nfe.ValorDesconto = proc.NFe.infNFe.total.ICMSTot.vDesc;
                nfe.ValorOutrasDespesas = proc.NFe.infNFe.total.ICMSTot.vOutro;
                nfe.ValorIPI = proc.NFe.infNFe.total.ICMSTot.vIPI;
                nfe.ValorTotalNota = proc.NFe.infNFe.total.ICMSTot.vNF;
                nfe.ValorServicos = proc.NFe.infNFe.total.ISSQNtot != null ? proc.NFe.infNFe.total.ISSQNtot.vServ ?? 0 : 0;
                nfe.BCISSQN = proc.NFe.infNFe.total.ISSQNtot != null ? proc.NFe.infNFe.total.ISSQNtot.vBC ?? 0 : 0;
                nfe.ValorISSQN = proc.NFe.infNFe.total.ISSQNtot != null ? proc.NFe.infNFe.total.ISSQNtot.vISS ?? 0 : 0;
                nfe.BCDeducao = proc.NFe.infNFe.total.ISSQNtot != null ? proc.NFe.infNFe.total.ISSQNtot.vDeducao ?? 0 : 0;
                nfe.ValorOutrasRetencoes = proc.NFe.infNFe.total.ISSQNtot != null ? proc.NFe.infNFe.total.ISSQNtot.vOutro ?? 0 : 0;
                nfe.ValorDescontoIncondicional = proc.NFe.infNFe.total.ISSQNtot != null ? proc.NFe.infNFe.total.ISSQNtot.vDescIncond ?? 0 : 0;
                nfe.ValorDescontoCondicional = proc.NFe.infNFe.total.ISSQNtot != null ? proc.NFe.infNFe.total.ISSQNtot.vDescCond ?? 0 : 0;
                nfe.ValorRetencaoISS = proc.NFe.infNFe.total.ISSQNtot != null ? proc.NFe.infNFe.total.ISSQNtot.vISSRet ?? 0 : 0;
                //nfe.BCPIS = proc.NFe.infNFe.total.ICMSTot.;
                nfe.ValorPIS = proc.NFe.infNFe.total.ICMSTot.vPIS;
                //nfe.BCCOFINS = baseCOFINS;
                nfe.ValorCOFINS = proc.NFe.infNFe.total.ICMSTot.vCOFINS;
                nfe.ValorFCP = proc.NFe.infNFe.total.ICMSTot.vFCPUFDest ?? 0;
                nfe.ValorICMSDestino = proc.NFe.infNFe.total.ICMSTot.vFCPUFDest ?? 0;
                nfe.ValorICMSRemetente = proc.NFe.infNFe.total.ICMSTot.vICMSUFRemet ?? 0;

                nfe.TipoFrete = Dominio.Enumeradores.ModalidadeFrete.SemFrete;

                if (proc.NFe.infNFe.infAdic != null)
                {
                    nfe.ObservacaoNota = proc.NFe.infNFe.infAdic.infCpl;
                    nfe.ObservacaoTributaria = proc.NFe.infNFe.infAdic.infAdFisco;
                }

                if (proc.NFe.infNFe.exporta != null)
                {
                    nfe.UFEmbarque = proc.NFe.infNFe.exporta.UFSaidaPais;
                    nfe.LocalEmbarque = proc.NFe.infNFe.exporta.xLocExporta;
                    nfe.LocalDespacho = proc.NFe.infNFe.exporta.xLocDespacho;
                }

                if (proc.NFe.infNFe.compra != null)
                {
                    nfe.InformacaoCompraContrato = proc.NFe.infNFe.compra.xCont;
                    nfe.InformacaoCompraPedido = proc.NFe.infNFe.compra.xPed;
                    nfe.InformacaoCompraNotaEmpenho = proc.NFe.infNFe.compra.xNEmp;
                }

                repNFe.Inserir(nfe);

                if (proc.NFe.infNFe.det != null && proc.NFe.infNFe.det.Count > 0)
                {
                    for (int i = 0; i < proc.NFe.infNFe.det.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos();
                        if (proc.NFe.infNFe.det[i].prod.NCM == "00" || proc.NFe.infNFe.det[i].prod.NCM == "99")
                        {
                            item.Servico = CadastrarServico(proc.NFe.infNFe.det[i].prod, proc.NFe.infNFe.det[i].imposto.ISSQN.vAliq, proc.NFe.infNFe.det[i].imposto.ISSQN.cListServ, unitOfWork, codigoEmpresa);
                            if (item.Servico == null)
                                return "Serviço " + proc.NFe.infNFe.det[i].prod.xProd + " não localizado/cadastrado.";
                        }
                        else
                        {
                            item.Produto = CadastrarProduto(proc.NFe.infNFe.det[i].prod, unitOfWork, codigoEmpresa, ncmsAbastecimento, tipoServicoMultisoftware, configuracao);
                            if (item.Produto == null)
                                return "Produto " + proc.NFe.infNFe.det[i].prod.xProd + " não localizado/cadastrado.";
                        }

                        item.CFOP = CadastrarCFOP(proc.NFe.infNFe.det[i].prod.CFOP, unitOfWork, codigoEmpresa, proc.NFe.infNFe.ide.tpNF == TipoNFe.tnEntrada ? Dominio.Enumeradores.TipoCFOP.Entrada : Dominio.Enumeradores.TipoCFOP.Saida);
                        if (item.CFOP == null)
                            return "CFOP " + proc.NFe.infNFe.det[i].prod.CFOP + " não localizado/cadastrado.";
                        item.Quantidade = proc.NFe.infNFe.det[i].prod.qCom;
                        item.ValorUnitario = proc.NFe.infNFe.det[i].prod.vUnCom;
                        item.ValorTotal = proc.NFe.infNFe.det[i].prod.vProd;
                        item.ValorICMSST = 0;
                        item.ValorIPI = 0;
                        item.NotaFiscal = nfe;

                        if (proc.NFe.infNFe.det[i].imposto.IPI != null)
                        {
                            if (proc.NFe.infNFe.det[i].imposto.IPI.TipoIPI.GetType().Name == "IPITrib" && ((IPITrib)proc.NFe.infNFe.det[i].imposto.IPI.TipoIPI) != null)
                            {
                                if (((IPITrib)proc.NFe.infNFe.det[i].imposto.IPI.TipoIPI).pIPI != null)
                                    item.AliquotaIPI = ((IPITrib)proc.NFe.infNFe.det[i].imposto.IPI.TipoIPI).pIPI ?? 0;
                                else
                                    item.AliquotaIPI = 0;
                            }
                            else
                                item.AliquotaIPI = 0;
                        }
                        else
                            item.AliquotaIPI = 0;

                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMS00))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00;
                            item.AliquotaICMS = ((ICMS00)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).pICMS;
                            item.BCICMS = ((ICMS00)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).vBC;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMS10))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10;
                            item.AliquotaICMS = ((ICMS10)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).pICMS;
                            item.BCICMS = ((ICMS10)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).vBC;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMS20))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20;
                            item.AliquotaICMS = ((ICMS20)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).pICMS;
                            item.BCICMS = ((ICMS20)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).vBC;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMS30))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30;
                            item.AliquotaICMS = 0;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMS40))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40;
                            item.AliquotaICMS = 0;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMS51))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51;
                            item.AliquotaICMS = ((ICMS51)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).pICMS ?? 0;
                            item.BCICMS = ((ICMS51)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).vBC ?? 0;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMS60))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60;
                            item.AliquotaICMS = 0;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMS70))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70;
                            item.AliquotaICMS = ((ICMS70)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).pICMS;
                            item.BCICMS = ((ICMS70)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).vBC;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMS90))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90;
                            item.AliquotaICMS = ((ICMS90)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).pICMS ?? 0;
                            item.BCICMS = ((ICMS90)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).vBC ?? 0;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN101))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101;
                            item.AliquotaICMS = 0;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN102))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102;
                            item.AliquotaICMS = 0;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN201))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201;
                            item.AliquotaICMS = 0;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN202))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202;
                            item.AliquotaICMS = 0;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN500))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500;
                            item.AliquotaICMS = 0;
                        }
                        if (proc.NFe.infNFe.det[i].imposto.ICMS != null && proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN900))
                        {
                            item.CSTICMS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900;
                            item.AliquotaICMS = ((ICMSSN900)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).pICMS ?? 0;
                            item.BCICMS = ((ICMSSN900)proc.NFe.infNFe.det[i].imposto.ICMS.TipoICMS).vBC ?? 0;
                        }
                        else
                            item.AliquotaICMS = 0;


                        repNotaFiscalProdutos.Inserir(item);
                    }
                }
                SalvarXMLNota(nfe, unitOfWork, Dominio.Enumeradores.TipoArquivoXML.Distribuicao, arquivoXML);

                return "";
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;
                else
                    return "Problemas ao gravar a NFe anterior.";
            }
        }

        public string SalvarRetornoEnvioNFe(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe retorno, int codigoNFe, Repositorio.UnitOfWork unitOfWork, bool gerarTitulo = true)
        {
            try
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);
                TipoServicoMultisoftware tipoServicoMultisoftware = TipoServicoMultisoftware.MultiNFe;

                int status = 0;
                int.TryParse(retorno.cStat, out status);
                if (!string.IsNullOrWhiteSpace(retorno.nRec))
                    SalvarNumeroReciboNota(nfe, unitOfWork, retorno.nRec, retorno.cStat);

                if (retorno.Status == Dominio.Enumeradores.StatusNFe.Autorizado && retorno.TipoArquivoXML == Dominio.Enumeradores.TipoArquivoXML.CartaCorrecao)
                {
                    SalvarXMLNota(nfe, unitOfWork, retorno.TipoArquivoXML, retorno.XML);
                    SalvarStatusNota(nfe, unitOfWork, retorno.xMotivo, status, retorno.Status, retorno.chNFe, "", retorno.dhRecbto);
                    int codigoCCe = SalvarCartaCorrecaoNota(nfe, retorno.Justificativa, unitOfWork, DateTime.Now, retorno.nProt, nfe.Codigo.ToString());
                    return "";
                }

                SalvarStatusNota(nfe, unitOfWork, retorno.xMotivo, status, retorno.Status, retorno.chNFe, retorno.nProt, retorno.dhRecbto);
                if (retorno.Status == Dominio.Enumeradores.StatusNFe.Inutilizado)
                {
                    SalvarXMLNota(nfe, unitOfWork, retorno.TipoArquivoXML, retorno.XML);
                    SalvarInutilizacaoNota(nfe, unitOfWork, DateTime.Now);
                    return "";
                }
                else if (retorno.Status == Dominio.Enumeradores.StatusNFe.Cancelado)
                {
                    SalvarXMLNota(nfe, unitOfWork, retorno.TipoArquivoXML, retorno.XML);
                    SalvarCancelamentoNota(nfe, unitOfWork, DateTime.Now, retorno.Justificativa, nfe.Protocolo);
                    if (nfe.ModeloNotaFiscal == "55")
                    {
                        MovimentarEstoqueNota(codigoNFe, unitOfWork, nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Entrada, tipoServicoMultisoftware);
                        ReverteTituloNota(codigoNFe, unitOfWork);
                        EstornarPedidos(codigoNFe, unitOfWork);
                    }
                    else
                    {
                        ReverteTituloNotaConsumidor(codigoNFe, unitOfWork);
                        EstornarPedidos(codigoNFe, unitOfWork);
                    }
                    return "";
                }
                else if (retorno.cStat == "100" || retorno.cStat == "150")
                {
                    SalvarXMLNota(nfe, unitOfWork, retorno.TipoArquivoXML, retorno.XML);
                    if (nfe.ModeloNotaFiscal == "55")
                    {
                        MovimentarEstoqueNota(codigoNFe, unitOfWork, nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida, tipoServicoMultisoftware);
                        if (nfe.NaturezaDaOperacao.GeraTitulo && gerarTitulo)
                            GerarTitulosNota(codigoNFe, unitOfWork, nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida);
                        FinalizarPedidos(codigoNFe, unitOfWork);
                    }
                    else
                    {
                        if (gerarTitulo)
                            GerarTitulosNotaConsumidor(codigoNFe, unitOfWork);
                        FinalizarPedidos(codigoNFe, unitOfWork);
                        GerarDANFENFCe(codigoNFe, unitOfWork);
                    }
                    return "";
                }
                else
                    return "Rejeição: (" + retorno.cStat + ") " + retorno.xMotivo;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;
                else
                    return "Problemas ao salvar status da NF-e";
            }
        }

        public static List<Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus> EnviarEmailPDFTodosDocumentos(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, out string msgAuditoria, string urlAcesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao formaEnvioDocumentacao, List<int> codigosCTes, string stringConexao, int codigoUsuario, string caminhoRelatorios, string caminhoArquivos, string diretorioDocumentosFiscais, string emails, string assunto, string corpoEmail, string nomeArquivo, string caminhoArquivosAnexos, Repositorio.UnitOfWork unidadeTrabalho, bool envioDocumentacaoAFRMM, bool enviarParaFTP, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM configuracaoDocumentacao, string diretorioFTP, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteEnvio = null)
        {
            msgAuditoria = "";
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unidadeTrabalho);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            //Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Notificacao.Notificacao serNotificaocao = new Servicos.Embarcador.Notificacao.Notificacao(unidadeTrabalho.StringConexao, null, 0, "");
            Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
            Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeTrabalho);
            Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);
            Servicos.CCe svcCCe = new Servicos.CCe(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus> listaRetorno = new List<Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus>();

            if (string.IsNullOrWhiteSpace(emails) && !enviarParaFTP)
            {
                msgAuditoria = "Nenhum e-mail informado.";
                listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.Nenhum });
                return listaRetorno;
            }

            if (email == null)
            {
                Servicos.Log.TratarErro("Não há um e-mail configurado para realizar o envio.", "EnviarEmailPDFTodosDocumentos");
                msgAuditoria = "Não há um e-mail configurado para realizar o envio.";
                listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.Nenhum });
                return listaRetorno;
            }

            try
            {
                List<string> guidsAnexosCTe = new List<string>();
                List<string> anexosCarga = new List<string>();
                List<string> ctesAnexo = new List<string>();
                List<byte[]> sourceFiles = new List<byte[]>();
                string guidArquivo = (Guid.NewGuid().ToString());
                string guidXML = (Guid.NewGuid().ToString());
                bool contemArquivoCompactado = false;
                Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();

                List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                if (codigosCTes != null && codigosCTes.Count > 0)
                    codigosCTes = codigosCTes.Distinct().ToList();

                // TODO: ToList cast
                List<(int CodigoCTe, bool ImprimirTabelaTemperaturaVersoCTe)> ctesImprimirTabelaTemperatura = repCTe.BuscarInformacaoImpressaoTabelaTemperaturaVersoCTe(codigosCTes).ToList();
                bool imprimirTabelaTemperaturaNoVersoCTe = false;
                foreach (var codigoCTe in codigosCTes)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                    if (cte != null && (cte.Status == "A" || cte.Status == "C" || cte.Status == "K" || cte.Status == "Z" || cte.Status == "F") && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        string chaveCTe = cte.Chave;
                        byte[] pdf = null;
                        bool enviarPDFCTe = formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.Padrao || formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.PDFCTeNotas
                            || formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.PDFCTeNotasXMLCTeNotas || formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.SomentePDFCTe;
                        bool enviarPDFNotas = formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.Padrao || formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.PDFCTeNotas
                            || formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.PDFCTeNotasXMLCTeNotas || formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.SomentePDFNotas;
                        bool enviarXMLCTe = formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.PDFCTeNotasXMLCTeNotas || formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.SomenteXMLCTe;
                        bool enviarXMLNFe = formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.PDFCTeNotasXMLCTeNotas || formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.SomenteXMLNotas;

                        //Adiciona o PDF do CT-e
                        if (enviarPDFCTe || envioDocumentacaoAFRMM)
                        {
                            imprimirTabelaTemperaturaNoVersoCTe = ctesImprimirTabelaTemperatura.Exists(cteImprimirTabelaTemperatura => cteImprimirTabelaTemperatura.CodigoCTe == cte.Codigo && cteImprimirTabelaTemperatura.ImprimirTabelaTemperaturaVersoCTe);
                            pdf = RetornarPDFCTe(cte, configuracaoTMS, unidadeTrabalho, caminhoRelatorios, imprimirTabelaTemperaturaNoVersoCTe);
                            if (pdf != null)
                            {
                                sourceFiles.Add(pdf);
                                if (envioDocumentacaoAFRMM)
                                {
                                    Stream stream = new MemoryStream(pdf);
                                    nomeArquivo = "";
                                    if (cte.NumeroControle.StartsWith("SVM"))
                                        nomeArquivo = (!string.IsNullOrWhiteSpace(cte.NumeroManifestoTransbordo) ? cte.NumeroManifestoTransbordo : cte.NumeroManifesto) + "-SVM" + cte.Chave;
                                    else
                                        nomeArquivo = (!string.IsNullOrWhiteSpace(cte.NumeroManifestoTransbordo) ? cte.NumeroManifestoTransbordo : cte.NumeroManifesto) + "-CTe" + cte.Chave;
                                    if (!ctesAnexo.Contains(cte.Chave) || ctesAnexo.Count == 0)
                                    {
                                        ctesAnexo.Add(cte.Chave);
                                        attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo + ".pdf"));
                                    }
                                }
                            }

                            //Adiciona o PDF da última carta de correção
                            Dominio.ObjetosDeValor.Relatorios.Relatorio arquivoCCe = null;
                            Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarUltimaCCeAutorizadaPorCTe(cte.Codigo);
                            if (cce != null)
                            {
                                arquivoCCe = svcCCe.ObterRelatorio(cce, unidadeTrabalho);
                                if (arquivoCCe != null && arquivoCCe.Arquivo != null)
                                {
                                    sourceFiles.Add(arquivoCCe.Arquivo);
                                    if (envioDocumentacaoAFRMM)
                                    {
                                        Stream stream = new MemoryStream(arquivoCCe.Arquivo);
                                        nomeArquivo = (!string.IsNullOrWhiteSpace(cte.NumeroManifestoTransbordo) ? cte.NumeroManifestoTransbordo : cte.NumeroManifesto) + "-CCe" + cte.Chave;
                                        attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo + ".pdf"));
                                    }
                                }
                            }

                            //Adiciona o PDF dos CTes complementares
                            if (cte.PossuiCTeComplementar)
                            {
                                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesComplementares = repCTe.BuscarCTeComplementaresPorChave(cte.Chave);
                                if (ctesComplementares != null && ctesComplementares.Count > 0)
                                {
                                    foreach (var cteComplementar in ctesComplementares)
                                    {
                                        if (cteComplementar != null && (cteComplementar.Status == "A" || cteComplementar.Status == "C" || cteComplementar.Status == "K" || cteComplementar.Status == "Z" || cteComplementar.Status == "F") && cteComplementar.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                        {
                                            imprimirTabelaTemperaturaNoVersoCTe = ctesImprimirTabelaTemperatura.Exists(cteImprimirTabelaTemperatura => cteImprimirTabelaTemperatura.CodigoCTe == cte.Codigo && cteImprimirTabelaTemperatura.ImprimirTabelaTemperaturaVersoCTe);

                                            pdf = RetornarPDFCTe(cteComplementar, configuracaoTMS, unidadeTrabalho, caminhoRelatorios, imprimirTabelaTemperaturaNoVersoCTe);
                                            if (pdf != null)
                                            {
                                                sourceFiles.Add(pdf);
                                                if (envioDocumentacaoAFRMM)
                                                {
                                                    Stream stream = new MemoryStream(pdf);
                                                    nomeArquivo = "";
                                                    if (cteComplementar.NumeroControle.StartsWith("SVM"))
                                                        nomeArquivo = (!string.IsNullOrWhiteSpace(cteComplementar.NumeroManifestoTransbordo) ? cteComplementar.NumeroManifestoTransbordo : cteComplementar.NumeroManifesto) + "-SVM" + cteComplementar.Chave;
                                                    else
                                                        nomeArquivo = (!string.IsNullOrWhiteSpace(cteComplementar.NumeroManifestoTransbordo) ? cteComplementar.NumeroManifestoTransbordo : cteComplementar.NumeroManifesto) + "-CTe" + cteComplementar.Chave;
                                                    if (!ctesAnexo.Contains(cteComplementar.Chave) || ctesAnexo.Count == 0)
                                                    {
                                                        ctesAnexo.Add(cteComplementar.Chave);
                                                        attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo + ".pdf"));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (enviarXMLCTe)
                        {
                            if (cte.ModeloDocumentoFiscal.Numero == "39")
                            {
                                nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".xml";
                                Servicos.NFSe svcNFSe = new Servicos.NFSe();
                                byte[] data = svcNFSe.ObterXMLAutorizacaoCTe(cte.Codigo, unidadeTrabalho);
                                if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                {
                                    conteudoCompactar.Add(nomeArquivo, data);
                                    contemArquivoCompactado = true;
                                }
                            }
                            else
                            {
                                nomeArquivo = string.Concat(cte.Chave, ".xml");
                                byte[] data = svcCTe.ObterXMLAutorizacao(cte, unidadeTrabalho);
                                if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                {
                                    conteudoCompactar.Add(nomeArquivo, data);
                                    contemArquivoCompactado = true;
                                }
                            }
                        }


                        if (enviarPDFNotas || enviarXMLNFe || envioDocumentacaoAFRMM)
                        {
                            //Adiciona os PDF's das notas fiscais
                            if (cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0)
                            {
                                var notasFiscaisOrdenadas = cte.XMLNotaFiscais.OrderBy(o => o.Numero).ToList();
                                foreach (var notaFiscal in notasFiscaisOrdenadas)
                                {
                                    if (string.IsNullOrWhiteSpace(notaFiscal.Chave) || notaFiscal.Chave.Length != 44)
                                        continue;

                                    try
                                    {
                                        string xmlNotaFiscal = RetornarXMLNotaFiscal(notaFiscal, diretorioDocumentosFiscais, unidadeTrabalho);
                                        if (!string.IsNullOrWhiteSpace(xmlNotaFiscal))
                                        {
                                            if (enviarPDFNotas || envioDocumentacaoAFRMM)
                                            {
                                                string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "DANFE Documentos Emitidos");

                                                caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDANFE, notaFiscal.Chave + ".pdf");

                                                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                                {
                                                    if (notaFiscal.XML.Contains("</nfeProc>"))
                                                        Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminhoDANFE, false, false);
                                                    else if (notaFiscal.XML.Contains("</NFe>"))
                                                        Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminhoDANFE, false, true);
                                                    else
                                                        Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminhoDANFE, true, false);
                                                }

                                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                                {
                                                    pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE);
                                                    if (pdf != null)
                                                    {
                                                        sourceFiles.Add(pdf);
                                                        if (envioDocumentacaoAFRMM || envioDocumentacaoAFRMM)
                                                        {
                                                            Stream stream = new MemoryStream(pdf);
                                                            nomeArquivo = (!string.IsNullOrWhiteSpace(cte.NumeroManifestoTransbordo) ? cte.NumeroManifestoTransbordo : cte.NumeroManifesto) + "-NFe" + notaFiscal.Chave;
                                                            attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo + ".pdf"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.Chave, "EnviarEmailPDFTodosDocumentos");
                                                        msgAuditoria = "Não foi possível gerar a DANFE da NF-e " + notaFiscal.Chave;
                                                        Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgAuditoria, unidadeTrabalho);
                                                        listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = notaFiscal.Chave, CTe = cte, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.XMLNotaFiscal });
                                                    }
                                                }
                                                else
                                                {
                                                    Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.Chave, "EnviarEmailPDFTodosDocumentos");
                                                    msgAuditoria = "Não foi possível gerar a DANFE da NF-e " + notaFiscal.Chave;
                                                    Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgAuditoria, unidadeTrabalho);
                                                    listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = notaFiscal.Chave, CTe = cte, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.XMLNotaFiscal });
                                                }
                                            }
                                            if (enviarXMLNFe)
                                            {
                                                byte[] bytes = Encoding.ASCII.GetBytes(xmlNotaFiscal);
                                                nomeArquivo = string.Concat((!string.IsNullOrWhiteSpace(notaFiscal.Chave) ? notaFiscal.Chave : notaFiscal.Numero.ToString()), ".xml");
                                                if (bytes != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                                {
                                                    conteudoCompactar.Add(nomeArquivo, bytes);
                                                    contemArquivoCompactado = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.Chave, "EnviarEmailPDFTodosDocumentos");
                                            msgAuditoria = "Não foi possível gerar a DANFE da NF-e " + notaFiscal.Chave;
                                            Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgAuditoria, unidadeTrabalho);
                                            listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = notaFiscal.Chave, CTe = cte, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.XMLNotaFiscal });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.Chave + " " + ex, "EnviarEmailPDFTodosDocumentos");
                                        msgAuditoria = "Não foi possível gerar a DANFE da NF-e " + notaFiscal.Chave;
                                        Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgAuditoria, unidadeTrabalho);
                                        listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = notaFiscal.Chave, CTe = cte, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.XMLNotaFiscal });
                                    }
                                }
                            }
                            else if (cte.Documentos != null && cte.Documentos.Count > 0)
                            {
                                var notasFiscaisOrdenadas = cte.Documentos.OrderBy(o => o.Numero).ToList();
                                foreach (var notaFiscal in notasFiscaisOrdenadas)
                                {
                                    if (string.IsNullOrWhiteSpace(notaFiscal.ChaveNFE) || notaFiscal.ChaveNFE.Length != 44)
                                        continue;

                                    try
                                    {
                                        string xmlNotaFiscal = ObterNotaFiscalPorSerpro(notaFiscal.ChaveNFE, unidadeTrabalho);
                                        if (!string.IsNullOrWhiteSpace(xmlNotaFiscal))
                                        {
                                            if (enviarPDFNotas)
                                            {
                                                string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "DANFE Documentos Emitidos", notaFiscal.ChaveNFE + ".pdf");

                                                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                                {
                                                    Zeus.GerarDANFE(out string erro, xmlNotaFiscal, caminhoDANFE, true, false);
                                                }

                                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                                {
                                                    pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE);
                                                    if (pdf != null)
                                                    {
                                                        sourceFiles.Add(pdf);
                                                        if (envioDocumentacaoAFRMM)
                                                        {
                                                            Stream stream = new MemoryStream(pdf);
                                                            nomeArquivo = (!string.IsNullOrWhiteSpace(cte.NumeroManifestoTransbordo) ? cte.NumeroManifestoTransbordo : cte.NumeroManifesto) + "-NFe" + notaFiscal.ChaveNFE;
                                                            attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo + ".pdf"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE, "EnviarEmailPDFTodosDocumentos");
                                                        msgAuditoria = "Não foi possível gerar a DANFE da NF-e " + notaFiscal.ChaveNFE;
                                                        Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgAuditoria, unidadeTrabalho);
                                                        listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = notaFiscal.ChaveNFE, CTe = cte, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.DocumentosCTE });
                                                    }
                                                }
                                                else
                                                {
                                                    Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE, "EnviarEmailPDFTodosDocumentos");
                                                    msgAuditoria = "Não foi possível gerar a DANFE da NF-e " + notaFiscal.ChaveNFE;
                                                    Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgAuditoria, unidadeTrabalho);
                                                    listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = notaFiscal.ChaveNFE, CTe = cte, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.DocumentosCTE });
                                                }
                                            }
                                            if (enviarXMLNFe)
                                            {
                                                byte[] bytes = Encoding.ASCII.GetBytes(xmlNotaFiscal);
                                                nomeArquivo = string.Concat((!string.IsNullOrWhiteSpace(notaFiscal.ChaveNFE) ? notaFiscal.ChaveNFE : notaFiscal.Numero.ToString()), ".xml");
                                                if (bytes != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                                {
                                                    conteudoCompactar.Add(nomeArquivo, bytes);
                                                    contemArquivoCompactado = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE, "EnviarEmailPDFTodosDocumentos");
                                            msgAuditoria = "Não foi possível gerar a DANFE da NF-e " + notaFiscal.ChaveNFE;
                                            Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgAuditoria, unidadeTrabalho);
                                            listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = notaFiscal.ChaveNFE, CTe = cte, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.DocumentosCTE });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE + " " + ex, "EnviarEmailPDFTodosDocumentos");
                                        msgAuditoria = "Não foi possível gerar a DANFE da NF-e " + notaFiscal.ChaveNFE;
                                        Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgAuditoria, unidadeTrabalho);
                                        listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = notaFiscal.ChaveNFE, CTe = cte, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.DocumentosCTE });
                                    }
                                }
                            }

                        }

                        if (enviarPDFCTe && !envioDocumentacaoAFRMM)
                        {
                            //CTes anteriores
                            if (cte.DocumentosOriginarios != null && cte.DocumentosOriginarios.Count > 0)
                            {
                                foreach (var cteAnterior in cte.DocumentosOriginarios)
                                {
                                    if (cteAnterior != null && cteAnterior.CTe != null && (cteAnterior.CTe.Status == "A" || cteAnterior.CTe.Status == "C" || cteAnterior.CTe.Status == "K" || cteAnterior.CTe.Status == "Z" || cteAnterior.CTe.Status == "F") && cteAnterior.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                    {
                                        if (!cteAnterior.CTe.NumeroControle.StartsWith("SVM") || !envioDocumentacaoAFRMM)
                                        {
                                            try
                                            {
                                                imprimirTabelaTemperaturaNoVersoCTe = ctesImprimirTabelaTemperatura.Exists(cteImprimirTabelaTemperatura => cteImprimirTabelaTemperatura.CodigoCTe == cteAnterior.CTe.Codigo && cteImprimirTabelaTemperatura.ImprimirTabelaTemperaturaVersoCTe);
                                                pdf = RetornarPDFCTe(cteAnterior.CTe, configuracaoTMS, unidadeTrabalho, caminhoRelatorios, imprimirTabelaTemperaturaNoVersoCTe);
                                                if (pdf != null)
                                                {
                                                    sourceFiles.Add(pdf);
                                                    if (envioDocumentacaoAFRMM)
                                                    {
                                                        Stream stream = new MemoryStream(pdf);
                                                        nomeArquivo = "";
                                                        if (cteAnterior.CTe.NumeroControle.StartsWith("SVM"))
                                                            nomeArquivo = (!string.IsNullOrWhiteSpace(cteAnterior.CTe.NumeroManifestoTransbordo) ? cteAnterior.CTe.NumeroManifestoTransbordo : cteAnterior.CTe.NumeroManifesto) + "-SVM" + cteAnterior.CTe.Chave;
                                                        else
                                                            nomeArquivo = (!string.IsNullOrWhiteSpace(cteAnterior.CTe.NumeroManifestoTransbordo) ? cteAnterior.CTe.NumeroManifestoTransbordo : cteAnterior.CTe.NumeroManifesto) + "-CTe" + cteAnterior.CTe.Chave;
                                                        if (!ctesAnexo.Contains(cteAnterior.CTe.Chave) || ctesAnexo.Count == 0)
                                                        {
                                                            ctesAnexo.Add(cteAnterior.CTe.Chave);
                                                            attachments.Add(new System.Net.Mail.Attachment(stream, nomeArquivo + ".pdf"));
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Servicos.Log.TratarErro("CTe " + chaveCTe + " " + ex, "EnviarEmailPDFTodosDocumentos");
                                                msgAuditoria = "Não foi possível gerar a DACTE da CT-e " + cteAnterior.CTe.Chave;
                                                Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgAuditoria, unidadeTrabalho);
                                                listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = cteAnterior.CTe.Chave, CTe = cte, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.CTE });
                                            }
                                        }
                                    }
                                }
                            }

                            //Anexos vinculados a carga
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> anexos = repCargaCTe.BuscarAnexosPorCTe(cte.Codigo);
                            if (!envioDocumentacaoAFRMM && anexos != null && anexos.Count > 0)
                            {
                                foreach (var anexo in anexos)
                                {
                                    string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                                    if (!guidsAnexosCTe.Contains(anexo.GuidArquivo))
                                    {
                                        guidsAnexosCTe.Add(anexo.GuidArquivo);

                                        string caminho = caminhoArquivosAnexos;
                                        string nomeArquivoAnexo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}{extencao}");

                                        if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivoAnexo))
                                        {
                                            anexosCarga.Add(nomeArquivoAnexo);
                                            //pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivoAnexo);
                                            //if (pdf != null)
                                            //    sourceFiles.Add(pdf);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                byte[] pdfAgrupado = null;
                if ((sourceFiles != null && sourceFiles.Count > 0) || contemArquivoCompactado || anexosCarga.Count > 0 || envioDocumentacaoAFRMM)
                {
                    if (sourceFiles != null && sourceFiles.Count > 0 && !envioDocumentacaoAFRMM)
                        pdfAgrupado = svcDACTE.MergeFiles(sourceFiles);
                    if (pdfAgrupado != null || contemArquivoCompactado || anexosCarga.Count > 0 || envioDocumentacaoAFRMM)
                    {
                        string caminhoArquivo = "";
                        string caminhoArquivoXML = "";

                        if (!envioDocumentacaoAFRMM)
                        {
                            string pastaRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador;

                            if (pdfAgrupado != null)
                            {
                                caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, guidArquivo + ".pdf");
                                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoArquivo, pdfAgrupado);
                            }

                            if (anexosCarga.Count > 0)
                            {
                                foreach (var anexo in anexosCarga)
                                {
                                    if (Utilidades.IO.FileStorageService.Storage.Exists(anexo))
                                    {
                                        string caminhoArquivoAnexo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, Path.GetFileName(anexo));
                                        Utilidades.IO.FileStorageService.Storage.Copy(anexo, caminhoArquivoAnexo);
                                    }
                                }
                            }

                            if (contemArquivoCompactado)
                            {
                                using (MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar))
                                {
                                    caminhoArquivoXML = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, guidXML + ".rar");
                                    using (Stream fs = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoArquivoXML))
                                    {
                                        arquivoCompactado.CopyTo(fs);
                                        fs.Flush();
                                    }
                                }
                            }
                        }

                        if (enviarParaFTP && configuracaoDocumentacao != null && attachments != null && attachments.Count > 0)
                        {
                            bool sucesso = false;
                            string mensagemErro = "";
                            foreach (var anexo in attachments)
                            {
                                var memoryStream = new MemoryStream();
                                anexo.ContentStream.CopyTo(memoryStream);
                                sucesso = Servicos.FTP.EnviarArquivo(memoryStream, anexo.Name, configuracaoDocumentacao.EnderecoFTP, configuracaoDocumentacao.PortaFTP, diretorioFTP, configuracaoDocumentacao.UsuarioFTP, configuracaoDocumentacao.SenhaFTP,
                                    configuracaoDocumentacao.FTPPassivo, configuracaoDocumentacao.SSL, out mensagemErro, configuracaoDocumentacao.SFTP, false);
                            }
                            if (!sucesso)
                            {
                                Servicos.Log.TratarErro("Problemas ao enviar o cte para o FTP: " + mensagemErro, "EnviarEmailPDFTodosDocumentos");
                                msgAuditoria = "Problemas ao enviar o cte para o FTP: " + mensagemErro;
                                if (cteEnvio != null)
                                    listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = cteEnvio.Chave, CTe = cteEnvio, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.CTE });
                                else
                                    listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.Nenhum });
                            }
                            else
                            {
                                msgAuditoria = "Envio do Documento ao FTP com sucesso.";
                                listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = true, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.Nenhum });
                            }
                        }
                        else if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo) || Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoXML) || anexosCarga.Count > 0 || envioDocumentacaoAFRMM)
                        {
                            List<string> emailsEnvio = new List<string>();
                            string mensagemErro = "Erro ao enviar e-mail";
                            emailsEnvio.AddRange(emails.Split(';').ToList());
                            emailsEnvio = emailsEnvio.Distinct().ToList();
                            if (emailsEnvio.Count > 0)
                            {
                                if (!envioDocumentacaoAFRMM)
                                {
                                    corpoEmail += "Link para downlado do(s) Arquivo(s): <br/>";
                                    string linkDownload = "";

                                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                                    {
                                        linkDownload = "http://" + urlAcesso + "/ImpressaoLoteCarga/AnexoEmail?Anexo=pdf&guid=" + guidArquivo;
                                        linkDownload = EncurtadorUrl(linkDownload);
                                        if (string.IsNullOrWhiteSpace(linkDownload))
                                            linkDownload = urlAcesso + "/ImpressaoLoteCarga/AnexoEmail?Anexo=pdf&guid=" + guidArquivo;
                                        if (!string.IsNullOrWhiteSpace(linkDownload))
                                            corpoEmail += " PDF: " + linkDownload + " <br/>";
                                    }

                                    if (contemArquivoCompactado)
                                    {
                                        linkDownload = "";
                                        linkDownload = "http://" + urlAcesso + "/ImpressaoLoteCarga/AnexoEmail?Anexo=rar&guid=" + guidXML;
                                        linkDownload = EncurtadorUrl(linkDownload);
                                        if (string.IsNullOrWhiteSpace(linkDownload))
                                            linkDownload = urlAcesso + "/ImpressaoLoteCarga/AnexoEmail?Anexo=rar&guid=" + guidXML;
                                        if (!string.IsNullOrWhiteSpace(linkDownload))
                                            corpoEmail += " XMLs: " + linkDownload + " <br/>";
                                    }

                                    if (anexosCarga.Count > 0)
                                    {
                                        foreach (var anexo in anexosCarga)
                                        {
                                            if (Utilidades.IO.FileStorageService.Storage.Exists(anexo))
                                            {
                                                linkDownload = "";
                                                linkDownload = "http://" + urlAcesso + "/ImpressaoLoteCarga/AnexoEmail?Anexo=" + Path.GetExtension(anexo) + "&guid=" + Path.GetFileNameWithoutExtension(anexo);
                                                linkDownload = EncurtadorUrl(linkDownload);
                                                if (string.IsNullOrWhiteSpace(linkDownload))
                                                    linkDownload = urlAcesso + "/ImpressaoLoteCarga/AnexoEmail?Anexo=" + Path.GetExtension(anexo) + "&guid=" + Path.GetFileNameWithoutExtension(anexo);
                                                if (!string.IsNullOrWhiteSpace(linkDownload))
                                                    corpoEmail += " Anexo: " + linkDownload + " <br/>";
                                            }
                                        }
                                    }

                                    if (string.IsNullOrWhiteSpace(assunto))
                                        assunto = "Envio automático de CT-e";
                                    if (string.IsNullOrWhiteSpace(corpoEmail))
                                        corpoEmail = "Envio automático de CT-e";
                                }

                                bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsEnvio.ToArray(), null, assunto, corpoEmail, email.Smtp, out mensagemErro, email.DisplayEmail,
                                envioDocumentacaoAFRMM ? attachments : null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho);

                                if (!sucesso)
                                {
                                    Servicos.Log.TratarErro("Problemas ao enviar o cte em lote: " + mensagemErro, "EnviarEmailPDFTodosDocumentos");
                                    msgAuditoria = "Problemas ao enviar o cte em lote: " + mensagemErro;
                                    if (cteEnvio != null)
                                        listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, ChaveDocumento = cteEnvio.Chave, CTe = cteEnvio, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.CTE });
                                    else
                                        listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.Nenhum });
                                }
                                else
                                {
                                    msgAuditoria = "Envio o PDF em lote para E-mail(s): " + string.Join(", ", emailsEnvio.ToArray());
                                    listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = true, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.Nenhum });
                                }
                            }
                        }
                    }
                }

                if (listaRetorno.Count == 0)
                {
                    msgAuditoria = "Nenhum documento selecionado";
                    listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.Nenhum });
                    return listaRetorno;
                }
                else
                {
                    return listaRetorno;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "EnviarEmailPDFTodosDocumentos");
                msgAuditoria = "Falha ao processar o envio em lote " + ex.Message;
                listaRetorno.Add(new Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus { Sucesso = false, TipoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoGerarPDFZeus.Nenhum });
                return listaRetorno;
            }
            finally
            {
                //unidadeTrabalho.Dispose();
            }

        }

        public static void GerarPDFTodosDocumentos(long codigoImpressaoLote, List<int> codigosCTes, string stringConexao, int codigoUsuario, string caminhoRelatorios, string caminhoArquivos, string diretorioDocumentosFiscais, string paginaExecucao, string caminhoArquivosAnexos)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);

            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unidadeTrabalho);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unidadeTrabalho);
            Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unidadeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote repEnvioDocumentacaoLote = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote(unidadeTrabalho);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote envioDocumentacaoLote = null;
            if (codigoImpressaoLote > 0)
                envioDocumentacaoLote = repEnvioDocumentacaoLote.BuscarPorCodigo(codigoImpressaoLote);

            Servicos.Embarcador.Notificacao.Notificacao serNotificaocao = new Servicos.Embarcador.Notificacao.Notificacao(unidadeTrabalho.StringConexao, null, 0, "");
            Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
            Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeTrabalho);
            Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);
            Servicos.CCe svcCCe = new Servicos.CCe(unidadeTrabalho);

            string guidArquivo = (Guid.NewGuid().ToString());
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao = GerarRelatorioControle(guidArquivo, usuario, unidadeTrabalho);
            string chaveCTe = "";
            bool imprimirTabelaTemperaturaNoVersoCTe = false;

            try
            {
                List<string> guidsAnexosCTe = new List<string>();
                List<byte[]> sourceFiles = new List<byte[]>();
                // TODO: ToList cast
                List<(int CodigoCTe, bool ImprimirTabelaTemperaturaVersoCTe)> ctesImprimirTabelaTemperatura = repCTe.BuscarInformacaoImpressaoTabelaTemperaturaVersoCTe(codigosCTes).ToList();

                for (int i = 0; i < codigosCTes.Count; i++)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigosCTes[i]);
                    if (cte != null && (cte.Status == "A" || cte.Status == "C" || cte.Status == "K" || cte.Status == "Z" || cte.Status == "F") && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        chaveCTe = cte.Chave;
                        byte[] pdf = null;

                        imprimirTabelaTemperaturaNoVersoCTe = ctesImprimirTabelaTemperatura.Exists(cteImprimirTabelaTemperatura => cteImprimirTabelaTemperatura.CodigoCTe == cte.Codigo && cteImprimirTabelaTemperatura.ImprimirTabelaTemperaturaVersoCTe);

                        //Adiciona o PDF do CT-e
                        pdf = RetornarPDFCTe(cte, configuracaoTMS, unidadeTrabalho, caminhoRelatorios, imprimirTabelaTemperaturaNoVersoCTe);
                        if (pdf != null)
                            sourceFiles.Add(pdf);

                        //Adiciona o PDF da última carta de correção
                        Dominio.ObjetosDeValor.Relatorios.Relatorio arquivoCCe = null;
                        Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarUltimaCCeAutorizadaPorCTe(cte.Codigo);
                        if (cce != null)
                        {
                            arquivoCCe = svcCCe.ObterRelatorio(cce, unidadeTrabalho);
                            if (arquivoCCe != null)
                                sourceFiles.Add(arquivoCCe.Arquivo);
                        }

                        //Adiciona o PDF dos CTes complementares
                        if (cte.PossuiCTeComplementar)
                        {
                            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesComplementares = repCTe.BuscarCTeComplementaresPorChave(cte.Chave);
                            if (ctesComplementares != null && ctesComplementares.Count > 0)
                            {
                                foreach (var cteComplementar in ctesComplementares)
                                {
                                    if (cteComplementar != null && (cteComplementar.Status == "A" || cteComplementar.Status == "C" || cteComplementar.Status == "K" || cteComplementar.Status == "Z" || cteComplementar.Status == "F") && cteComplementar.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                    {
                                        pdf = RetornarPDFCTe(cteComplementar, configuracaoTMS, unidadeTrabalho, caminhoRelatorios, imprimirTabelaTemperaturaNoVersoCTe: false);
                                        if (pdf != null)
                                            sourceFiles.Add(pdf);
                                    }
                                }
                            }
                        }

                        //Adiciona os PDF's das notas fiscais
                        if (cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0)
                        {
                            var notasFiscaisOrdenadas = cte.XMLNotaFiscais.OrderBy(o => o.Numero).ToList();
                            foreach (var notaFiscal in notasFiscaisOrdenadas)
                            {
                                try
                                {
                                    string xmlNotaFiscal = RetornarXMLNotaFiscal(notaFiscal, diretorioDocumentosFiscais, unidadeTrabalho);
                                    if (!string.IsNullOrWhiteSpace(xmlNotaFiscal))
                                    {
                                        string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "DANFE Documentos Emitidos", notaFiscal.Chave + ".pdf");

                                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                        {
                                            if (notaFiscal.XML.Contains("</nfeProc>"))
                                                Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminhoDANFE, false, false);
                                            else if (notaFiscal.XML.Contains("</NFe>"))
                                                Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminhoDANFE, false, true);
                                            else
                                                Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminhoDANFE, true, false);
                                        }

                                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                        {
                                            pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE);
                                            if (pdf != null)
                                                sourceFiles.Add(pdf);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("CTe " + chaveCTe + ex, "ImpressaoDocumentos");
                                }
                            }
                        }
                        else if (cte.Documentos != null && cte.Documentos.Count > 0)
                        {
                            var notasFiscaisOrdenadas = cte.Documentos.OrderBy(o => o.Numero).ToList();
                            foreach (var notaFiscal in notasFiscaisOrdenadas)
                            {
                                if (string.IsNullOrWhiteSpace(notaFiscal.ChaveNFE) || notaFiscal.ChaveNFE.Length != 44)
                                    continue;
                                try
                                {
                                    string xmlNotaFiscal = ObterNotaFiscalPorSerpro(notaFiscal.ChaveNFE, unidadeTrabalho);
                                    if (!string.IsNullOrWhiteSpace(xmlNotaFiscal))
                                    {
                                        string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "DANFE Documentos Emitidos", notaFiscal.ChaveNFE + ".pdf");

                                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                        {
                                            Zeus.GerarDANFE(out string erro, xmlNotaFiscal, caminhoDANFE, true, false);
                                        }

                                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                        {
                                            pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE);
                                            if (pdf != null)
                                                sourceFiles.Add(pdf);
                                            else
                                            {
                                                Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE, "ImpressaoDocumentos");
                                            }
                                        }
                                        else
                                        {
                                            Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE, "ImpressaoDocumentos");
                                        }
                                    }
                                    else
                                    {
                                        Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE, "ImpressaoDocumentos");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE + " " + ex, "ImpressaoDocumentos");
                                }
                            }
                        }

                        //CTes anteriores
                        try
                        {
                            if (cte.DocumentosOriginarios != null && cte.DocumentosOriginarios.Count > 0)
                            {
                                foreach (var cteAnterior in cte.DocumentosOriginarios)
                                {
                                    if (cteAnterior != null && cteAnterior.CTe != null && (cteAnterior.CTe.Status == "A" || cteAnterior.CTe.Status == "C" || cteAnterior.CTe.Status == "K" || cteAnterior.CTe.Status == "Z" || cteAnterior.CTe.Status == "F") && cteAnterior.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                    {
                                        try
                                        {
                                            pdf = RetornarPDFCTe(cteAnterior.CTe, configuracaoTMS, unidadeTrabalho, caminhoRelatorios, imprimirTabelaTemperaturaNoVersoCTe: false);
                                            if (pdf != null)
                                                sourceFiles.Add(pdf);
                                        }
                                        catch (Exception ex)
                                        {
                                            Servicos.Log.TratarErro("CTe " + chaveCTe + ex, "ImpressaoDocumentos");
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("CTe " + chaveCTe + ex, "ImpressaoDocumentos");
                        }

                        //Anexos vinculados a carga
                        try
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> anexos = repCargaCTe.BuscarAnexosPorCTe(cte.Codigo);
                            if (anexos != null && anexos.Count > 0)
                            {
                                foreach (var anexo in anexos)
                                {
                                    string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                                    if (extencao.ToLower().Contains(".pdf") && !guidsAnexosCTe.Contains(anexo.GuidArquivo))
                                    {
                                        guidsAnexosCTe.Add(anexo.GuidArquivo);

                                        string caminho = caminhoArquivosAnexos;
                                        string nomeArquivoAnexo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}{extencao}");

                                        if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivoAnexo))
                                        {
                                            pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivoAnexo);
                                            if (pdf != null)
                                            {
                                                sourceFiles.Add(pdf);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("CTe " + chaveCTe + ex, "ImpressaoDocumentos");
                        }
                    }

                    unidadeTrabalho.FlushAndClear();
                }

                usuario = repUsuario.BuscarPorCodigo(codigoUsuario);
                byte[] pdfAgrupado = null;
                if (sourceFiles != null && sourceFiles.Count > 0)
                {
                    List<List<byte[]>> lotesSourceFiles = new List<List<byte[]>>();

                    if (sourceFiles.Count > 10)
                        lotesSourceFiles.AddRange(sourceFiles.DividirListaEmLotes(300));
                    else
                        lotesSourceFiles.Add(sourceFiles);

                    foreach (List<byte[]> lote in lotesSourceFiles)
                    {
                        if (lotesSourceFiles.IndexOf(lote) > 0)
                        {
                            guidArquivo = (Guid.NewGuid().ToString());
                            controleGeracao = GerarRelatorioControle(guidArquivo, usuario, unidadeTrabalho);
                        }

                        pdfAgrupado = svcDACTE.MergeFiles(lote);
                        if (pdfAgrupado != null)
                        {
                            string pastaRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador;
                            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, guidArquivo + ".pdf");

                            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoArquivo, pdfAgrupado);
                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                            {
                                controleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(controleGeracao.Codigo);
                                controleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.Gerado;
                                controleGeracao.DataFinalGeracao = DateTime.Now;
                                repRelatorioControleGeracao.Atualizar(controleGeracao);

                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.pdf;
                                //"Cargas/Carga"
                                serNotificaocao.GerarNotificacao(controleGeracao.Usuario, controleGeracao.Codigo, paginaExecucao, string.Format(Localization.Resources.Zeus.Zeus.RelatorioDisponivelDownload, controleGeracao.Titulo, controleGeracao.DataFinalGeracao.ToString("dd/MM/yyyy HH:mm")), icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.relatorio, 0, unidadeTrabalho);

                                if (envioDocumentacaoLote != null)
                                {
                                    envioDocumentacaoLote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Finalizado;
                                    envioDocumentacaoLote.Retorno = "O Relatorio (" + controleGeracao.Titulo + " " + controleGeracao.DataFinalGeracao.ToString("dd/MM/yyyy HH:mm") + ") está disponível para download.";
                                    repEnvioDocumentacaoLote.Atualizar(envioDocumentacaoLote);
                                }
                            }
                            else
                            {
                                controleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(controleGeracao.Codigo);
                                controleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.FalhaAoGerar;
                                controleGeracao.DataFinalGeracao = DateTime.Now;
                                repRelatorioControleGeracao.Atualizar(controleGeracao);

                                serNotificaocao.GerarNotificacao(controleGeracao.Usuario, controleGeracao.Codigo, controleGeracao.GuidArquivo, string.Format(Localization.Resources.Zeus.Zeus.NaoFoiPossivelGerarPDFAgrupador, controleGeracao.Titulo), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.relatorio, 0, unidadeTrabalho);

                                if (envioDocumentacaoLote != null)
                                {
                                    envioDocumentacaoLote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Falha;
                                    envioDocumentacaoLote.Retorno = "Não foi possível gerar o PDF agrupador (" + controleGeracao.Titulo + ").";
                                    repEnvioDocumentacaoLote.Atualizar(envioDocumentacaoLote);
                                }
                            }
                        }
                        else
                        {
                            controleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(controleGeracao.Codigo);
                            controleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.FalhaAoGerar;
                            controleGeracao.DataFinalGeracao = DateTime.Now;
                            repRelatorioControleGeracao.Atualizar(controleGeracao);

                            serNotificaocao.GerarNotificacao(controleGeracao.Usuario, controleGeracao.Codigo, controleGeracao.GuidArquivo, string.Format(Localization.Resources.Zeus.Zeus.NaoFoiPossivelAgruparDocumentos, controleGeracao.Titulo), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.relatorio, 0, unidadeTrabalho);

                            if (envioDocumentacaoLote != null)
                            {
                                envioDocumentacaoLote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Falha;
                                envioDocumentacaoLote.Retorno = "Não foi possível agrupar os documentos (" + controleGeracao.Titulo + ").";
                                repEnvioDocumentacaoLote.Atualizar(envioDocumentacaoLote);
                            }
                        }
                    }
                }
                else
                {
                    controleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(controleGeracao.Codigo);
                    controleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.FalhaAoGerar;
                    controleGeracao.DataFinalGeracao = DateTime.Now;
                    repRelatorioControleGeracao.Atualizar(controleGeracao);

                    serNotificaocao.GerarNotificacao(controleGeracao.Usuario, controleGeracao.Codigo, controleGeracao.GuidArquivo, string.Format(Localization.Resources.Zeus.Zeus.NaoFoiEncontradoDocumentoValidoCarga, controleGeracao.Titulo), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.relatorio, 0, unidadeTrabalho);
                    if (envioDocumentacaoLote != null)
                    {
                        envioDocumentacaoLote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Falha;
                        envioDocumentacaoLote.Retorno = "Não foi encontrado nenhum documento válido desta carga (" + controleGeracao.Titulo + ").";
                        repEnvioDocumentacaoLote.Atualizar(envioDocumentacaoLote);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("CTe " + chaveCTe + ex, "ImpressaoDocumentos");
                controleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(controleGeracao.Codigo);
                controleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.FalhaAoGerar;
                controleGeracao.DataFinalGeracao = DateTime.Now;
                repRelatorioControleGeracao.Atualizar(controleGeracao);

                serNotificaocao.GerarNotificacao(controleGeracao.Usuario, controleGeracao.Codigo, controleGeracao.GuidArquivo, string.Format(Localization.Resources.Zeus.Zeus.OcorreuFalhaGerarRelatorio, controleGeracao.Titulo), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.relatorio, 0, unidadeTrabalho);

                if (envioDocumentacaoLote != null)
                {
                    envioDocumentacaoLote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Falha;
                    envioDocumentacaoLote.Retorno = "Ocorreu uma falha ao gerar o realtório (" + controleGeracao.Titulo + ").";
                    repEnvioDocumentacaoLote.Atualizar(envioDocumentacaoLote);
                }
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }

        }

        // Esse método é uma cópia do método acima, porém, retorna os bytes do arquivo
        public static byte[] GerarPDFTodosDocumentosEObterBytes(List<int> codigosCTes, int codigoUsuario, string caminhoRelatorios, string caminhoArquivos, string diretorioDocumentosFiscais, string caminhoArquivosAnexos, Repositorio.UnitOfWork unidadeTrabalho, int codigoCarga)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unidadeTrabalho);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unidadeTrabalho);
            Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote repEnvioDocumentacaoLote = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Notificacao.Notificacao serNotificaocao = new Servicos.Embarcador.Notificacao.Notificacao(unidadeTrabalho.StringConexao, null, 0, "");
            Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
            Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeTrabalho);
            Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);
            Servicos.CCe svcCCe = new Servicos.CCe(unidadeTrabalho);
            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeTrabalho);

            string guidArquivo = (Guid.NewGuid().ToString());
            string chaveCTe = "";
            bool imprimirTabelaTemperaturaNoVersoCTe = false;
            try
            {
                List<string> guidsAnexosCTe = new List<string>();
                List<byte[]> sourceFiles = new List<byte[]>();
                // TODO: ToList cast
                List<(int CodigoCTe, bool ImprimirTabelaTemperaturaVersoCTe)> ctesImprimirTabelaTemperatura = repCTe.BuscarInformacaoImpressaoTabelaTemperaturaVersoCTe(codigosCTes).ToList();

                for (int i = 0; i < codigosCTes.Count; i++)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigosCTes[i]);
                    if (cte != null && (cte.Status == "A" || cte.Status == "C" || cte.Status == "K" || cte.Status == "Z" || cte.Status == "F") && (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe))
                    {
                        byte[] pdf = null;

                        if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                        {
                            chaveCTe = cte.Chave;

                            imprimirTabelaTemperaturaNoVersoCTe = ctesImprimirTabelaTemperatura.Exists(cteImprimirTabelaTemperatura => cteImprimirTabelaTemperatura.CodigoCTe == cte.Codigo && cteImprimirTabelaTemperatura.ImprimirTabelaTemperaturaVersoCTe);
                            //Adiciona o PDF do CT-e
                            pdf = RetornarPDFCTe(cte, configuracaoTMS, unidadeTrabalho, caminhoRelatorios, imprimirTabelaTemperaturaNoVersoCTe);
                            if (pdf != null)
                                sourceFiles.Add(pdf);

                            //Adiciona o PDF da última carta de correção
                            Dominio.ObjetosDeValor.Relatorios.Relatorio arquivoCCe = null;
                            Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarUltimaCCeAutorizadaPorCTe(cte.Codigo);
                            if (cce != null)
                            {
                                arquivoCCe = svcCCe.ObterRelatorio(cce, unidadeTrabalho);
                                if (arquivoCCe != null)
                                    sourceFiles.Add(arquivoCCe.Arquivo);
                            }

                            //Adiciona o PDF dos CTes complementares
                            if (cte.PossuiCTeComplementar)
                            {
                                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesComplementares = repCTe.BuscarCTeComplementaresPorChave(cte.Chave);
                                if (ctesComplementares != null && ctesComplementares.Count > 0)
                                {
                                    foreach (var cteComplementar in ctesComplementares)
                                    {
                                        if (cteComplementar != null && (cteComplementar.Status == "A" || cteComplementar.Status == "C" || cteComplementar.Status == "K" || cteComplementar.Status == "Z" || cteComplementar.Status == "F") && cteComplementar.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                        {
                                            pdf = RetornarPDFCTe(cteComplementar, configuracaoTMS, unidadeTrabalho, caminhoRelatorios, imprimirTabelaTemperaturaNoVersoCTe: false);
                                            if (pdf != null)
                                                sourceFiles.Add(pdf);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            byte[] arquivo = servicoNFSe.ObterDANFSECTe(cte.Codigo);

                            if (arquivo != null)
                                sourceFiles.Add(arquivo);
                        }

                        //Adiciona os PDF's das notas fiscais
                        if (cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0)
                        {
                            var notasFiscaisOrdenadas = cte.XMLNotaFiscais.OrderBy(o => o.Numero).ToList();
                            foreach (var notaFiscal in notasFiscaisOrdenadas)
                            {
                                try
                                {
                                    string xmlNotaFiscal = RetornarXMLNotaFiscal(notaFiscal, diretorioDocumentosFiscais, unidadeTrabalho);
                                    if (!string.IsNullOrWhiteSpace(xmlNotaFiscal))
                                    {
                                        string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "DANFE Documentos Emitidos", notaFiscal.Chave + ".pdf");

                                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                        {
                                            if (notaFiscal.XML.Contains("</nfeProc>"))
                                                Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminhoDANFE, false, false);
                                            else if (notaFiscal.XML.Contains("</NFe>"))
                                                Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminhoDANFE, false, true);
                                            else
                                                Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminhoDANFE, true, false);
                                        }

                                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                        {
                                            pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE);
                                            if (pdf != null)
                                                sourceFiles.Add(pdf);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("CTe " + chaveCTe + ex, "ImpressaoDocumentos");
                                }
                            }
                        }
                        else if (cte.Documentos != null && cte.Documentos.Count > 0)
                        {
                            var notasFiscaisOrdenadas = cte.Documentos.OrderBy(o => o.Numero).ToList();
                            foreach (var notaFiscal in notasFiscaisOrdenadas)
                            {
                                if (string.IsNullOrWhiteSpace(notaFiscal.ChaveNFE) || notaFiscal.ChaveNFE.Length != 44)
                                    continue;
                                try
                                {
                                    string xmlNotaFiscal = ObterNotaFiscalPorSerpro(notaFiscal.ChaveNFE, unidadeTrabalho);
                                    if (!string.IsNullOrWhiteSpace(xmlNotaFiscal))
                                    {
                                        string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "DANFE Documentos Emitidos", notaFiscal.ChaveNFE + ".pdf");

                                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                        {
                                            Zeus.GerarDANFE(out string erro, xmlNotaFiscal, caminhoDANFE, true, false);
                                        }

                                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                                        {
                                            pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE);
                                            if (pdf != null)
                                                sourceFiles.Add(pdf);
                                            else
                                            {
                                                Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE, "ImpressaoDocumentos");
                                            }
                                        }
                                        else
                                        {
                                            Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE, "ImpressaoDocumentos");
                                        }
                                    }
                                    else
                                    {
                                        Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE, "ImpressaoDocumentos");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("CTe " + chaveCTe + " NFe" + notaFiscal.ChaveNFE + " " + ex, "ImpressaoDocumentos");
                                }
                            }
                        }

                        //CTes anteriores
                        try
                        {
                            if (cte.DocumentosOriginarios != null && cte.DocumentosOriginarios.Count > 0)
                            {
                                foreach (var cteAnterior in cte.DocumentosOriginarios)
                                {
                                    if (cteAnterior != null && cteAnterior.CTe != null && (cteAnterior.CTe.Status == "A" || cteAnterior.CTe.Status == "C" || cteAnterior.CTe.Status == "K" || cteAnterior.CTe.Status == "Z" || cteAnterior.CTe.Status == "F") && cteAnterior.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                    {
                                        try
                                        {
                                            pdf = RetornarPDFCTe(cteAnterior.CTe, configuracaoTMS, unidadeTrabalho, caminhoRelatorios, imprimirTabelaTemperaturaNoVersoCTe: false);
                                            if (pdf != null)
                                                sourceFiles.Add(pdf);
                                        }
                                        catch (Exception ex)
                                        {
                                            Servicos.Log.TratarErro("CTe " + chaveCTe + ex, "ImpressaoDocumentos");
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("CTe " + chaveCTe + ex, "ImpressaoDocumentos");
                        }

                        //Anexos vinculados a carga
                        try
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> anexos = repCargaCTe.BuscarAnexosPorCTe(cte.Codigo);
                            if (anexos != null && anexos.Count > 0)
                            {
                                foreach (var anexo in anexos)
                                {
                                    string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                                    if (extencao.ToLower().Contains(".pdf") && !guidsAnexosCTe.Contains(anexo.GuidArquivo))
                                    {
                                        guidsAnexosCTe.Add(anexo.GuidArquivo);

                                        string caminho = caminhoArquivosAnexos;
                                        string nomeArquivoAnexo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}{extencao}");

                                        if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivoAnexo))
                                        {
                                            pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivoAnexo);
                                            if (pdf != null)
                                            {
                                                sourceFiles.Add(pdf);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("CTe " + chaveCTe + ex, "ImpressaoDocumentos");
                        }

                        //MDFe
                        try
                        {
                            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
                            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repCargaMDFe.BuscarPorCarga(codigoCarga);
                            foreach (var cargaMDFe in cargaMDFes)
                            {
                                byte[] arquivo = null;
                                if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoRelatorios))
                                {
                                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa?.CNPJ, cargaMDFe?.MDFe?.Chave) + ".pdf";
                                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                        arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                                }

                                if (arquivo != null)
                                    sourceFiles.Add(arquivo);
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro($"CTe {chaveCTe} MDFe {ex} ", "ImpressaoDocumentos");
                        }
                    }

                    unidadeTrabalho.FlushAndClear();
                }

                byte[] pdfAgrupado = null;
                if (sourceFiles != null && sourceFiles.Count > 0)
                {
                    pdfAgrupado = svcDACTE.MergeFiles(sourceFiles);
                    return pdfAgrupado;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public byte[] ObterPdfDANFCe(int codigoNFe, string chave, Repositorio.UnitOfWork unitOfWork, out string nomeArquivo, out string mensagem)
        {
            mensagem = null;

            nomeArquivo = chave + "NFCe.pdf";

            string diretorio = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e" }), nomeArquivo);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(diretorio))
            {
                mensagem = GerarDANFENFCe(codigoNFe, unitOfWork);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return null;
            }

            if (!Utilidades.IO.FileStorageService.Storage.Exists(diretorio))
            {
                mensagem = "DANFE não encontrada no Servidor! Favor contatar o suporte.";
                return null;
            }

            return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(diretorio);
        }

        #endregion

        #region Métodos Privados

        private static string EncurtadorUrl(string url)
        {
            try
            {
                string urlMontada = string.Format("http://tinyurl.com/api-create.php?url={0}", url);

                var client = new WebClient();

                string response = client.DownloadString(urlMontada);

                client.Dispose();

                return response;
            }
            catch (WebException ex)
            {
                //Log.TratarErro(ex);
                return string.Empty;
            }

        }

        private static string RemoveSensitiveProperties(string json, IEnumerable<Regex> regexes)
        {
            JToken token = JToken.Parse(json);
            RemoveSensitiveProperties(token, regexes);
            return token.ToString();
        }

        private static void RemoveSensitiveProperties(JToken token, IEnumerable<Regex> regexes)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (JProperty prop in token.Children<JProperty>().ToList())
                {
                    bool removed = false;
                    foreach (Regex regex in regexes)
                    {
                        if (regex.IsMatch(prop.Name))
                        {
                            prop.Remove();
                            removed = true;
                            break;
                        }
                    }
                    if (!removed)
                    {
                        RemoveSensitiveProperties(prop.Value, regexes);
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (JToken child in token.Children())
                {
                    RemoveSensitiveProperties(child, regexes);
                }
            }
        }

        private static byte[] RetornarPDFCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unidadeTrabalho, string caminhoRelatorios, bool imprimirTabelaTemperaturaNoVersoCTe)
        {
            Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);

            string nomeArquivo = cte.Chave;

            if (configuracaoTMS.GerarPDFCTeCancelado && cte.Status == "C" && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                nomeArquivo = nomeArquivo + "_Canc";

            if (cte.Status == "F")
                nomeArquivo = nomeArquivo + "_FSDA";

            string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, cte.Empresa.CNPJ, nomeArquivo) + ".pdf";

            byte[] pdf = null;
            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                pdf = svcDACTE.GerarPorProcesso(cte.Codigo, null, configuracaoTMS.GerarPDFCTeCancelado);
            else
                pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);


            if (imprimirTabelaTemperaturaNoVersoCTe)
            {
                byte[] versoCTe = ReportRequest.WithType(ReportType.RegistroTemperaturaETrocaDeGelo)
                    .WithExecutionType(ExecutionType.Sync)
                    .CallReport()
                    .GetContentFile();

                if (versoCTe != null && pdf != null)
                {
                    List<byte[]> sourceFiles = new List<byte[]>();
                    sourceFiles.Add(pdf);
                    sourceFiles.Add(versoCTe);
                    return svcDACTE.MergeFiles(sourceFiles);
                }
            }

            return pdf;
        }

        private static Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao GerarRelatorioControle(string guidArquivo, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unidadeTrabalho);
            Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R207_DocumentosCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            if (relatorio == null)
            {
                relatorio = new Dominio.Entidades.Embarcador.Relatorios.Relatorio()
                {
                    CodigoControleRelatorios = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R207_DocumentosCarga,
                    Titulo = "Documentos da Carga",
                    Descricao = "Documentos da Carga",
                    CaminhoRelatorio = "",
                    ArquivoRelatorio = "",
                    Ativo = true,
                    Padrao = true,
                    PadraoMultisoftware = true,
                    ExibirSumarios = false,
                    CortarLinhas = false,
                    FundoListrado = false,
                    TamanhoPadraoFonte = 10,
                    FontePadrao = "Arial",
                    PropriedadeOrdena = "",
                    PropriedadeAgrupa = "",
                    OrdemAgrupamento = "",
                    OrdemOrdenacao = "",
                    OrientacaoRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato
                };
                repRelatorio.Inserir(relatorio);
            }

            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao = new Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao()
            {
                CodigoEntidade = 0,
                DataInicioGeracao = DateTime.Now,
                DataFinalGeracao = DateTime.Now,
                GuidArquivo = guidArquivo,
                Relatorio = relatorio,
                SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao,
                TipoArquivoRelatorio = Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
                Titulo = "Documentos da Carga",
                Usuario = usuario
            };
            repRelatorioControleGeracao.Inserir(controleGeracao);

            return controleGeracao;
        }

        public static string RetornarXMLNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, string diretorioDocumentosFiscais, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repositorioDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

            if (!string.IsNullOrWhiteSpace(notaFiscal.XML) && (notaFiscal.XML.Contains("</NFe>") || notaFiscal.XML.Contains("</nfeProc>") || notaFiscal.XML.Contains("nfeProc")))
                return notaFiscal.XML;
            else if (!string.IsNullOrWhiteSpace(notaFiscal.Chave) && notaFiscal.Chave.Length == 44)
            {
                string notaSerpro = ObterNotaFiscalPorSerpro(notaFiscal.Chave, unitOfWork);
                if (!string.IsNullOrWhiteSpace(notaSerpro))
                {
                    notaFiscal.XML = notaSerpro;
                    repXMLNotaFiscal.Atualizar(notaFiscal);
                    return notaSerpro;
                }
                else
                    return "";
            }
            else
                return "";
        }

        private static string ObterNotaFiscalPorSerpro(string chave, Repositorio.UnitOfWork unitOfWork)
        {
            string msgRetorno = "";
            string token = "";
            string arquivoNotaFiscal = "";
            if (!Servicos.Embarcador.Integracao.Serpro.IntegracaoSerpro.Realizarlogin(unitOfWork, out msgRetorno, out token, false))
            {
                Servicos.Log.TratarErro(msgRetorno);
                return "";
            }
            else
            {
                arquivoNotaFiscal = Servicos.Embarcador.Integracao.Serpro.IntegracaoSerpro.BaixarJSONPelaChave(unitOfWork, out msgRetorno, token, chave);
                if (string.IsNullOrWhiteSpace(arquivoNotaFiscal))
                {
                    Servicos.Log.TratarErro(msgRetorno);
                    return "";
                }
                else
                {
                    return arquivoNotaFiscal;
                }
            }
        }

        #endregion

        #region Métodos Protegidos de Geração de NF-e/NFC-e

        protected virtual void MovimentarEstoqueNota(int codigoNFe, Repositorio.UnitOfWork unitOfWork, bool tipoSaida, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);

            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> listaItens = repNotaFiscalProdutos.BuscarPorNota(codigoNFe);
            for (int i = 0; i < listaItens.Count; i++)
            {
                var item = listaItens[i];
                if (item.Produto != null && item.CFOP != null && item.CFOP.GeraEstoque)
                {
                    decimal quantidadeMovimentacao = item.Quantidade;
                    if (item.Produto.ProdutoKIT != null ? item.Produto.ProdutoKIT.Value : false)
                    {
                        var composicoes = item.Produto.Composicoes;
                        if (composicoes.Count > 0)
                        {
                            foreach (var composicao in composicoes)
                            {
                                decimal movimentacaoInsumo = quantidadeMovimentacao * composicao.Quantidade;
                                AtualizarEstoqueProdutoKit(codigoNFe, movimentacaoInsumo, 0, tipoSaida, composicao.Insumo, item.NotaFiscal.Empresa, unitOfWork, tipoServicoMultisoftware, item.LocalArmazenamento);
                            }
                        }
                        else
                            AtualizarEstoqueProdutoKit(codigoNFe, quantidadeMovimentacao, item.ValorUnitario, tipoSaida, item.Produto, item.NotaFiscal.Empresa, unitOfWork, tipoServicoMultisoftware, item.LocalArmazenamento);
                    }
                    else
                        AtualizarEstoqueProdutoKit(codigoNFe, quantidadeMovimentacao, item.ValorUnitario, tipoSaida, item.Produto, item.NotaFiscal.Empresa, unitOfWork, tipoServicoMultisoftware, item.LocalArmazenamento);
                }
            }
        }
        protected virtual void AtualizarEstoqueProdutoKit(int codigoNFe, decimal quantidadeMovimentacao, decimal custoUnitario, bool tipoSaida, Dominio.Entidades.Produto produto, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = null)
        {
            Dominio.Enumeradores.TipoMovimento tipoMovimento;
            if (tipoSaida)
                tipoMovimento = Dominio.Enumeradores.TipoMovimento.Saida;
            else
                tipoMovimento = Dominio.Enumeradores.TipoMovimento.Entrada;

            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);
            servicoEstoque.MovimentarEstoque(out string erro, produto, quantidadeMovimentacao, tipoMovimento, "NFS", Convert.ToString(codigoNFe), custoUnitario, empresa, DateTime.Now, tipoServicoMultisoftware, null, localArmazenamento);
        }

        protected virtual void FinalizarPedidos(int codigoNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalPedido repNotaFiscalPedido = new Repositorio.Embarcador.NotaFiscal.NotaFiscalPedido(unitOfWork);
            Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalPedido> listaNotaFiscalPedido = repNotaFiscalPedido.BuscarPorNota(codigoNFe);

            if (listaNotaFiscalPedido != null)
            {
                for (int i = 0; i < listaNotaFiscalPedido.Count; i++)
                {
                    listaNotaFiscalPedido[i].PedidoVenda.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda.Faturada;
                    repPedidoVenda.Atualizar(listaNotaFiscalPedido[i].PedidoVenda);
                }
            }
        }
        protected virtual void EstornarPedidos(int codigoNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalPedido repNotaFiscalPedido = new Repositorio.Embarcador.NotaFiscal.NotaFiscalPedido(unitOfWork);
            Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalPedido> listaNotaFiscalPedido = repNotaFiscalPedido.BuscarPorNota(codigoNFe);

            if (listaNotaFiscalPedido != null)
            {
                for (int i = 0; i < listaNotaFiscalPedido.Count; i++)
                {
                    listaNotaFiscalPedido[i].PedidoVenda.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda.Aberta;
                    repPedidoVenda.Atualizar(listaNotaFiscalPedido[i].PedidoVenda);
                }
            }
        }

        protected virtual void GerarTitulosNota(int codigoNFe, Repositorio.UnitOfWork unitOfWork, bool tipoAReceber)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela repNotaFiscalParcela = new Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela> listaParcelas = repNotaFiscalParcela.BuscarPorNota(codigoNFe);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);

            if (listaParcelas != null)
            {
                for (int i = 0; i < listaParcelas.Count; i++)
                {
                    if (listaParcelas[i].NotaFiscal.NaturezaDaOperacao.GeraTitulo)
                    {
                        if (!repTitulo.ContemTituloDuplicado(listaParcelas[i].NotaFiscal.Codigo, listaParcelas[i].DataVencimento.Value, listaParcelas[i].NotaFiscal.Cliente.CPF_CNPJ, listaParcelas[i].Valor, 0, listaParcelas[i].Sequencia))
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                            titulo.Acrescimo = listaParcelas[i].Acrescimo;
                            titulo.DataEmissao = listaParcelas[i].DataEmissao;
                            titulo.DataVencimento = listaParcelas[i].DataVencimento;
                            titulo.DataProgramacaoPagamento = listaParcelas[i].DataVencimento;
                            titulo.Desconto = listaParcelas[i].Desconto;
                            titulo.Historico = "GERADO A PARTIR DA NOTA FISCAL DE NÚMERO " + listaParcelas[i].NotaFiscal.Numero + " E SÉRIE " + listaParcelas[i].NotaFiscal.EmpresaSerie.Numero;
                            titulo.Pessoa = listaParcelas[i].NotaFiscal.Cliente;
                            titulo.GrupoPessoas = listaParcelas[i].NotaFiscal.Cliente.GrupoPessoas;
                            titulo.Sequencia = listaParcelas[i].Sequencia;
                            titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                            titulo.DataAlteracao = DateTime.Now;
                            if (tipoAReceber)
                                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                            else
                                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                            titulo.ValorOriginal = listaParcelas[i].Valor;
                            titulo.ValorPago = 0;
                            titulo.ValorPendente = listaParcelas[i].Valor;
                            titulo.NotaFiscal = listaParcelas[i].NotaFiscal;
                            titulo.Empresa = listaParcelas[i].NotaFiscal.Empresa;
                            titulo.ValorTituloOriginal = titulo.ValorOriginal;
                            titulo.TipoDocumentoTituloOriginal = "NF-e";
                            titulo.NumeroDocumentoTituloOriginal = listaParcelas[i].NotaFiscal.Numero.ToString();
                            if (listaParcelas[i].NotaFiscal.NaturezaDaOperacao.TipoMovimento != null)
                                titulo.TipoMovimento = listaParcelas[i].NotaFiscal.NaturezaDaOperacao.TipoMovimento;
                            titulo.TipoAmbiente = listaParcelas[i].NotaFiscal.TipoAmbiente;
                            titulo.FormaTitulo = listaParcelas[i].Forma;

                            titulo.DataLancamento = DateTime.Now;
                            titulo.Usuario = listaParcelas[i].NotaFiscal?.Usuario;

                            repTitulo.Inserir(titulo);

                            if (listaParcelas[i].NotaFiscal.NaturezaDaOperacao.TipoMovimento != null)
                                servProcessoMovimento.GerarMovimentacao(listaParcelas[i].NotaFiscal.NaturezaDaOperacao.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "TÍTULO " + titulo.Sequencia.ToString() + " NFe " + listaParcelas[i].NotaFiscal.Numero.ToString(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaSaida, TipoServicoMultisoftware.MultiNFe, 0, null, null, titulo.Codigo);
                        }
                    }
                }
            }
        }
        protected virtual void GerarTitulosNotaConsumidor(int codigoNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela repNotaFiscalParcela = new Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);
            Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);
            Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal = repNotaFiscal.BuscarPorCodigo(codigoNFe);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela> listaParcelas = repNotaFiscalParcela.BuscarPorNota(codigoNFe);

            Dominio.Entidades.Cliente cliente = null;
            if (!string.IsNullOrWhiteSpace(notaFiscal.CPFCNPJConsumidorFinal))
            {
                cliente = repCliente.BuscarPorCPFCNPJ(Double.Parse(notaFiscal.CPFCNPJConsumidorFinal));
            }
            if (cliente == null && notaFiscal.Empresa.GerarParcelaAutomaticamente)
            {
                cliente = repCliente.BuscarPorCPFCNPJ(Double.Parse(notaFiscal.Empresa.CNPJ));
                if (cliente == null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa clienteEmpresa = serEmpresa.ConverterObjetoEmpresa(notaFiscal.Empresa);
                    serCliente.ConverterParaTransportadorTerceiro(clienteEmpresa, "Empresa", unitOfWork);
                    cliente = repCliente.BuscarPorCPFCNPJ(Double.Parse(notaFiscal.Empresa.CNPJ));
                }
            }

            int countTitulosInseridos = 0;
            if (listaParcelas != null && cliente != null && notaFiscal.Empresa.NaturezaDaOperacaoNFCe != null && notaFiscal.Empresa.NaturezaDaOperacaoNFCe.GeraTitulo)
            {
                for (int i = 0; i < listaParcelas.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                    titulo.Acrescimo = listaParcelas[i].Acrescimo;
                    titulo.DataEmissao = listaParcelas[i].DataEmissao;
                    titulo.DataVencimento = listaParcelas[i].DataVencimento;
                    titulo.DataProgramacaoPagamento = listaParcelas[i].DataVencimento;
                    titulo.Desconto = listaParcelas[i].Desconto;
                    titulo.Historico = "GERADO A PARTIR DA NOTA FISCAL DE CONSUMIDOR DE NÚMERO " + listaParcelas[i].NotaFiscal.Numero + " E SÉRIE " + listaParcelas[i].NotaFiscal.EmpresaSerie.Numero;
                    titulo.Pessoa = cliente;
                    titulo.GrupoPessoas = cliente.GrupoPessoas;
                    titulo.Sequencia = listaParcelas[i].Sequencia;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                    titulo.ValorOriginal = listaParcelas[i].Valor;
                    titulo.ValorPago = 0;
                    titulo.ValorPendente = listaParcelas[i].Valor;
                    titulo.NotaFiscal = listaParcelas[i].NotaFiscal;
                    titulo.Empresa = listaParcelas[i].NotaFiscal.Empresa;
                    titulo.ValorTituloOriginal = titulo.ValorOriginal;
                    titulo.TipoDocumentoTituloOriginal = "NFC-e";
                    titulo.NumeroDocumentoTituloOriginal = listaParcelas[i].NotaFiscal.Numero.ToString();
                    if (notaFiscal.Empresa.NaturezaDaOperacaoNFCe.TipoMovimento != null)
                        titulo.TipoMovimento = notaFiscal.Empresa.NaturezaDaOperacaoNFCe.TipoMovimento;
                    else
                        titulo.TipoMovimento = null;
                    titulo.TipoAmbiente = listaParcelas[i].NotaFiscal.TipoAmbiente;
                    titulo.FormaTitulo = listaParcelas[i].Forma;

                    titulo.DataLancamento = DateTime.Now;
                    titulo.Usuario = listaParcelas[i].NotaFiscal?.Usuario;

                    repTitulo.Inserir(titulo);

                    if (titulo.TipoMovimento != null)
                        servProcessoMovimento.GerarMovimentacao(titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "TÍTULO " + titulo.Sequencia.ToString() + " NFCe " + listaParcelas[i].NotaFiscal.Numero.ToString(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaSaida, TipoServicoMultisoftware.MultiNFe, 0, null, null, titulo.Codigo);
                    countTitulosInseridos = countTitulosInseridos + 1;
                }
            }

            //Gera e quita título automaticamente caso não tenha parcela
            if (listaParcelas.Count == 0 && cliente != null && notaFiscal.Empresa.TipoPagamentoRecebimento != null && notaFiscal.Empresa.EmitirVendaPrazoNFCe
                && notaFiscal.Empresa.NaturezaDaOperacaoNFCe != null && notaFiscal.Empresa.NaturezaDaOperacaoNFCe.TipoMovimento != null && notaFiscal.Empresa.NaturezaDaOperacaoNFCe.GeraTitulo)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                titulo.DataEmissao = notaFiscal.DataEmissao;
                titulo.DataVencimento = notaFiscal.DataEmissao;
                titulo.DataProgramacaoPagamento = notaFiscal.DataEmissao;
                titulo.Pessoa = cliente;
                titulo.GrupoPessoas = cliente.GrupoPessoas;
                titulo.Sequencia = 1;
                titulo.ValorOriginal = notaFiscal.ValorTotalNota;
                titulo.ValorPendente = 0;
                titulo.ValorPago = notaFiscal.ValorTotalNota;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
                titulo.DataAlteracao = DateTime.Now;
                titulo.Historico = "GERADO A PARTIR DA NOTA FISCAL DE CONSUMIDOR DE NÚMERO " + notaFiscal.Numero + " E SÉRIE " + notaFiscal.EmpresaSerie.Numero;
                titulo.Empresa = notaFiscal.Empresa;
                titulo.ValorTituloOriginal = titulo.ValorOriginal;
                titulo.TipoDocumentoTituloOriginal = "NFC-e";
                titulo.NumeroDocumentoTituloOriginal = notaFiscal.Numero.ToString();
                titulo.TipoAmbiente = notaFiscal.TipoAmbiente;
                titulo.TipoMovimento = notaFiscal.Empresa.NaturezaDaOperacaoNFCe.TipoMovimento;
                titulo.DataLiquidacao = notaFiscal.DataEmissao;
                titulo.DataBaseLiquidacao = notaFiscal.DataEmissao;
                titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                titulo.NotaFiscal = notaFiscal;

                titulo.DataLancamento = DateTime.Now;
                titulo.Usuario = notaFiscal.Usuario;

                repTitulo.Inserir(titulo);

                servProcessoMovimento.GerarMovimentacao(notaFiscal.Empresa.NaturezaDaOperacaoNFCe.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(),
                    "Título " + titulo.Sequencia.ToString() + " NFC-e " + notaFiscal.Numero.ToString(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaSaida,
                    TipoServicoMultisoftware.MultiNFe, 0, null, null, titulo.Codigo);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa();
                tituloBaixa.DataBaixa = notaFiscal.DataEmissao;
                tituloBaixa.DataBase = notaFiscal.DataEmissao;
                tituloBaixa.DataOperacao = DateTime.Now;
                tituloBaixa.Numero = 1;
                tituloBaixa.Observacao = "Gerado automaticamente pela NFC-e nº " + notaFiscal.Numero.ToString();
                tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada;
                tituloBaixa.Sequencia = 1;
                tituloBaixa.Valor = titulo.ValorOriginal;
                tituloBaixa.TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                tituloBaixa.Pessoa = cliente;
                tituloBaixa.Titulo = titulo;
                tituloBaixa.TipoPagamentoRecebimento = notaFiscal.Empresa.TipoPagamentoRecebimento;
                tituloBaixa.ModeloAntigo = true;

                repTituloBaixa.Inserir(tituloBaixa);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                tituloAgrupado.TituloBaixa = tituloBaixa;
                tituloAgrupado.Titulo = titulo;
                tituloAgrupado.DataBaixa = notaFiscal.DataEmissao.Value;
                tituloAgrupado.DataBase = notaFiscal.DataEmissao.Value;

                repTituloBaixaAgrupado.Inserir(tituloAgrupado);

                servBaixaTituloReceber.GeraReverteMovimentacaoFinanceira(out string erro, tituloBaixa.Codigo, unitOfWork, unitOfWork.StringConexao, TipoServicoMultisoftware.MultiNFe, false, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta, repMovimentoFinanceiro.BuscarContaDebitoTitulo(titulo.Codigo));
            }
            else if (countTitulosInseridos == 0 && notaFiscal.Empresa.NaturezaDaOperacaoNFCe != null && notaFiscal.Empresa.NaturezaDaOperacaoNFCe.TipoMovimento != null && notaFiscal.Empresa.NaturezaDaOperacaoNFCe.GeraTitulo)
            {
                servProcessoMovimento.GerarMovimentacao(notaFiscal.Empresa.NaturezaDaOperacaoNFCe.TipoMovimento, notaFiscal.DataEmissao.Value, notaFiscal.ValorTotalNota, notaFiscal.Numero.ToString(),
                    " NFC-e " + notaFiscal.Numero.ToString(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaSaida, TipoServicoMultisoftware.MultiNFe);
            }
        }

        protected virtual void ReverteTituloNota(int codigoNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTitulo.BuscarPorNota(codigoNFe);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);

            for (int i = 0; i < listaTitulo.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];

                titulo.DataAlteracao = DateTime.Now;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                titulo.DataCancelamento = DateTime.Now.Date;

                repTitulo.Atualizar(titulo);

                if (titulo.TipoMovimento != null)
                    servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, titulo.ValorOriginal, titulo.Codigo.ToString(), "REVERSÃO DO TÍTULO " + titulo.Sequencia.ToString() + " NFe " + titulo.NotaFiscal.Numero.ToString(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaSaida, TipoServicoMultisoftware.MultiNFe, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito);
            }
        }
        protected virtual void ReverteTituloNotaConsumidor(int codigoNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal = repNotaFiscal.BuscarPorCodigo(codigoNFe);
            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTitulo.BuscarPorNota(codigoNFe);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);

            for (int i = 0; i < listaTitulo.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];

                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                titulo.DataAlteracao = DateTime.Now;
                titulo.DataCancelamento = DateTime.Now.Date;

                repTitulo.Atualizar(titulo);

                if (titulo.TipoMovimento != null && notaFiscal.Empresa.TipoPagamentoRecebimento != null)
                    servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, titulo.ValorOriginal, titulo.Codigo.ToString(), "REVERSÃO DO TÍTULO " + titulo.Sequencia.ToString() + " NFCe " + titulo.NotaFiscal.Numero.ToString(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaSaida, TipoServicoMultisoftware.MultiNFe, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito);
            }

            if (listaTitulo.Count == 0 && notaFiscal.Empresa.NaturezaDaOperacaoNFCe != null
                && notaFiscal.Empresa.NaturezaDaOperacaoNFCe.TipoMovimento != null && notaFiscal.Empresa.NaturezaDaOperacaoNFCe.GeraTitulo)
            {
                servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, notaFiscal.ValorTotalNota, notaFiscal.Numero.ToString(),
                    " REVERSÃO DE MOVIMENTAÇÃO DE NFCe " + notaFiscal.Numero.ToString(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaSaida, TipoServicoMultisoftware.MultiNFe, 0,
                    notaFiscal.Empresa.NaturezaDaOperacaoNFCe.TipoMovimento.PlanoDeContaDebito, notaFiscal.Empresa.NaturezaDaOperacaoNFCe.TipoMovimento.PlanoDeContaCredito);
            }
        }

        protected virtual void SalvarCancelamentoNota(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork, DateTime dataProcessamento, string justificativa, string protocolo)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalCancelamento repNotaFiscalCancelamento = new Repositorio.Embarcador.NotaFiscal.NotaFiscalCancelamento(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCancelamento cancelar = repNotaFiscalCancelamento.BuscarPorNota(nfe.Codigo);
            if (cancelar == null)
                cancelar = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCancelamento();
            justificativa = justificativa.Trim().TrimEnd().TrimStart();
            cancelar.Chave = nfe.Chave;
            cancelar.DataProcessamento = dataProcessamento;
            cancelar.Justificativa = justificativa;
            cancelar.NotaFiscal = nfe;
            cancelar.Protocolo = protocolo;
            cancelar.Status = Dominio.Enumeradores.StatusNFe.Cancelado;

            if (cancelar.Codigo > 0)
                repNotaFiscalCancelamento.Atualizar(cancelar);
            else
                repNotaFiscalCancelamento.Inserir(cancelar);
        }
        protected virtual void SalvarInutilizacaoNota(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork, DateTime? dataRecibo)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizar repNotaFiscalInutilizar = new Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizar(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizar inutilizar = repNotaFiscalInutilizar.BuscarPorNota(nfe.Codigo);
            if (inutilizar == null)
                inutilizar = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizar();

            inutilizar.DataProcessamento = dataRecibo;
            inutilizar.Empresa = nfe.Empresa;
            inutilizar.EmpresaSerie = nfe.EmpresaSerie;
            inutilizar.Justificativa = "FAIXA DE NUMERACAO NAO SERA MAIS UTILIZADA";
            inutilizar.Modelo = "55";
            inutilizar.NotaFiscal = nfe;
            inutilizar.NumeroFinal = nfe.Numero;
            inutilizar.NumeroInicial = nfe.Numero;
            inutilizar.Status = Dominio.Enumeradores.StatusNFe.Inutilizado;

            if (inutilizar.Codigo > 0)
                repNotaFiscalInutilizar.Atualizar(inutilizar);
            else
                repNotaFiscalInutilizar.Inserir(inutilizar);
        }
        protected virtual int SalvarCartaCorrecaoNota(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, string correcao, Repositorio.UnitOfWork unitOfWork, DateTime dataRecibo, string protocolo, string numeroLote)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao repNotaFiscalCartaCorrecao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao(unitOfWork);

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao cartaCorrecao = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao();
            cartaCorrecao.DataProcessamento = dataRecibo;
            cartaCorrecao.Mensagem = correcao.Trim().TrimEnd().TrimStart();
            cartaCorrecao.NotaFiscal = nfe;
            cartaCorrecao.Status = Dominio.Enumeradores.StatusNFe.Autorizado;
            cartaCorrecao.Protocolo = protocolo;
            cartaCorrecao.NumeroLote = numeroLote;

            if (cartaCorrecao.Codigo > 0)
                repNotaFiscalCartaCorrecao.Atualizar(cartaCorrecao);
            else
                repNotaFiscalCartaCorrecao.Inserir(cartaCorrecao);

            return cartaCorrecao.Codigo;
        }
        protected virtual void SalvarXMLNota(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork, Dominio.Enumeradores.TipoArquivoXML tipoArquivo, string xml)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarPorNota(nfe.Codigo);
            if (arquivo == null)
                arquivo = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos();
            arquivo.NotaFiscal = nfe;

            if (tipoArquivo == Dominio.Enumeradores.TipoArquivoXML.Distribuicao)
                arquivo.XMLDistribuicao = xml;
            else if (tipoArquivo == Dominio.Enumeradores.TipoArquivoXML.Cancelamento)
                arquivo.XMLCancelamento = xml;
            else if (tipoArquivo == Dominio.Enumeradores.TipoArquivoXML.CartaCorrecao)
                arquivo.XMLCartaCorrecao = xml;
            else if (tipoArquivo == Dominio.Enumeradores.TipoArquivoXML.Inutilizacao)
                arquivo.XMLInutilizacao = xml;
            else if (tipoArquivo == Dominio.Enumeradores.TipoArquivoXML.XMLSemAssinatura)
                arquivo.XMLNaoAssinado = xml;
            else if (tipoArquivo == Dominio.Enumeradores.TipoArquivoXML.XMLCartaCorrecaoNaoAssinado)
                arquivo.XMLCartaCorrecaoNaoAssinado = xml;
            else if (tipoArquivo == Dominio.Enumeradores.TipoArquivoXML.XMLCancelamentoNaoAssinado)
                arquivo.XMLCancelamentoNaoAssinado = xml;
            else if (tipoArquivo == Dominio.Enumeradores.TipoArquivoXML.XMLInutilizacaoNaoAssinado)
                arquivo.XMLInutilizacaoNaoAssinado = xml;

            if (arquivo.Codigo > 0)
                repNotaFiscalArquivos.Atualizar(arquivo);
            else
                repNotaFiscalArquivos.Inserir(arquivo);
        }
        protected virtual void SalvarNumeroReciboNota(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork, string recibo, string numeroStatus)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nota = repNotaFiscal.BuscarPorCodigo(nfe.Codigo);
            nota.NumeroRecibo = recibo;
            nota.UltimoStatus = numeroStatus;
            repNotaFiscal.Atualizar(nota);
        }
        protected virtual void SalvarStatusNota(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork, string motivo, int codigoStatus, Dominio.Enumeradores.StatusNFe statusNFe, string chaveNFe, string protocolo, DateTime? dataProcessamento)
        {
            Repositorio.Embarcador.NotaFiscal.RetornoSefaz repRetornoSefaz = new Repositorio.Embarcador.NotaFiscal.RetornoSefaz(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz retornoSefaz = repRetornoSefaz.ContemRetornoSefazAtivo(motivo);

            Repositorio.Embarcador.NotaFiscal.NotaFiscalStatus repNotaFiscalStatus = new Repositorio.Embarcador.NotaFiscal.NotaFiscalStatus(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalStatus status = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalStatus();

            status.Data = DateTime.Now;
            status.Mensagem = (retornoSefaz?.AbreviacaoRetornoSefaz ?? "") + "- Retorno da SEFAZ: " + motivo;
            status.NotaFiscal = nfe;
            status.Observacao = Convert.ToString(codigoStatus);
            status.Status = statusNFe;

            repNotaFiscalStatus.Inserir(status);

            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nota = repNotaFiscal.BuscarPorCodigo(nfe.Codigo);
            nota.Status = statusNFe;
            nota.UltimoStatusSEFAZ = (retornoSefaz?.AbreviacaoRetornoSefaz ?? "") + "- Retorno da SEFAZ: " + motivo;
            if (!string.IsNullOrWhiteSpace(chaveNFe))
                nota.Chave = chaveNFe;
            if (!string.IsNullOrWhiteSpace(protocolo))
                nota.Protocolo = protocolo;
            if (dataProcessamento != null && dataProcessamento > DateTime.MinValue)
                nota.DataProcessamento = dataProcessamento;
            repNotaFiscal.Atualizar(nota);
        }

        protected virtual string ValidarDocumentosAguardandoAssinatura(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Dominio.Enumeradores.StatusNFe statusNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

            int quantidadeNotasPendentes = repNotaFiscal.QuantidadeNotaFiscalAguardandoAssinatura(nfe.Empresa.Codigo, statusNFe);
            if (quantidadeNotasPendentes >= 5)
            {
                if (statusNFe == Dominio.Enumeradores.StatusNFe.AguardandoCancelarAssinar)
                    return "Favor abrir o Assinador do Multi NF-e e assinar os Cancelamentos pendentes antes de cancelar essa nota";
                else if (statusNFe == Dominio.Enumeradores.StatusNFe.AguardandoCartaCorrecaoAssinar)
                    return "Favor abrir o Assinador do Multi NF-e e assinar as Cartas de Correções pendentes antes de gerar a CC-e dessa nota";
                else if (statusNFe == Dominio.Enumeradores.StatusNFe.AguardandoInutilizarAssinar)
                    return "Favor abrir o Assinador do Multi NF-e e assinar as Inutilizações pendentes antes de inutilizar essa nota";
                else
                    return "Favor abrir o Assinador do Multi NF-e e assinar as Notas pendentes antes de emitir essa nota";
            }

            return string.Empty;
        }

        #endregion Métodos Protegidos de Geração de NF-e/NFC-e

        #region Configurações Padrões

        protected virtual void CriarConfiguracaoPadrao(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ModeloDocumento modelo, Repositorio.UnitOfWork unitOfWork)
        {
            _configuracoes = new ConfiguracaoApp
            {
                Emitente = new emit
                {
                    CNAE = nfe.Empresa.CNAE,
                    CNPJ = !string.IsNullOrWhiteSpace(nfe.Empresa.Tipo) && nfe.Empresa.Tipo.Equals("F") ? null : nfe.Empresa.CNPJ,
                    CPF = !string.IsNullOrWhiteSpace(nfe.Empresa.Tipo) && nfe.Empresa.Tipo.Equals("F") ? nfe.Empresa.CNPJ : null,
                    CRT = nfe.Empresa.OptanteSimplesNacional && nfe.Empresa.OptanteSimplesNacionalComExcessoReceitaBruta ? CRT.SimplesNacionalExcessoSublimite : nfe.Empresa.OptanteSimplesNacional ? CRT.SimplesNacional : CRT.RegimeNormal,
                    enderEmit = new enderEmit
                    {
                        CEP = nfe.Empresa.CEP,
                        cMun = nfe.Empresa.Localidade.CodigoIBGE,
                        cPais = nfe.Empresa.Localidade.Pais.Codigo,
                        fone = null,// !string.IsNullOrWhiteSpace(nfe.Empresa.Telefone) ? Convert.ToInt64(Utilidades.String.OnlyNumbers(nfe.Empresa.Telefone)) : 0,
                        nro = nfe.Empresa.Numero,
                        UF = (DFe.Classes.Entidades.Estado)nfe.Empresa.Localidade.Estado.CodigoIBGE,
                        xBairro = nfe.Empresa.Bairro,
                        xCpl = !string.IsNullOrWhiteSpace(nfe.Empresa.Complemento) ? nfe.Empresa.Complemento.Trim().TrimEnd() : null,
                        xLgr = nfe.Empresa.Endereco,
                        xMun = nfe.Empresa.Localidade.Descricao,
                        xPais = nfe.Empresa.Localidade.Pais.Nome
                    },
                    IE = nfe.Empresa.InscricaoEstadual,
                    IM = !string.IsNullOrWhiteSpace(nfe.Empresa.InscricaoMunicipal) ? nfe.Empresa.InscricaoMunicipal : "ISENTO",
                    xFant = nfe.Empresa.NomeFantasia,
                    xNome = nfe.Empresa.RazaoSocial
                },
                EnderecoEmitente = new enderEmit
                {
                    CEP = nfe.Empresa.CEP,
                    cMun = nfe.Empresa.Localidade.CodigoIBGE,
                    cPais = nfe.Empresa.Localidade.Pais.Codigo,
                    fone = null,// !string.IsNullOrWhiteSpace(nfe.Empresa.Telefone) ? Convert.ToInt64(Utilidades.String.OnlyNumbers(nfe.Empresa.Telefone)) : 0,
                    nro = nfe.Empresa.Numero,
                    UF = (DFe.Classes.Entidades.Estado)nfe.Empresa.Localidade.Estado.CodigoIBGE,
                    xBairro = nfe.Empresa.Bairro,
                    xCpl = !string.IsNullOrWhiteSpace(nfe.Empresa.Complemento) ? nfe.Empresa.Complemento.Trim().TrimEnd() : null,
                    xLgr = nfe.Empresa.Endereco,
                    xMun = nfe.Empresa.Localidade.Descricao,
                    xPais = nfe.Empresa.Localidade.Pais.Nome
                }
            };

            _configuracoes.CfgServico = ConfiguracaoServico.Instancia;
            _configuracoes.CfgServico.tpAmb = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? TipoAmbiente.Homologacao : TipoAmbiente.Producao;
            _configuracoes.CfgServico.tpEmis = TipoEmissao.teNormal;
            _configuracoes.CfgServico.cUF = (DFe.Classes.Entidades.Estado)nfe.Empresa.Localidade.Estado.CodigoIBGE;
            _configuracoes.CfgServico.ModeloDocumento = modelo;

            _configuracoes.CfgServico.Certificado.TipoCertificado = DFe.Utils.TipoCertificado.A1Arquivo;
            _configuracoes.CfgServico.Certificado.Arquivo = nfe.Empresa.NomeCertificado;
            _configuracoes.CfgServico.Certificado.Senha = nfe.Empresa.SenhaCertificado;


            _configuracoes.CfgServico.VersaoNfceAministracaoCSC = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeAutorizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaCadastro = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaDest = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaProtocolo = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeDistribuicaoDFe = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeDownloadNF = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeInutilizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeRecepcao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeRetAutorizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeRetRecepcao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeStatusServico = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoCceCancelamento = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoEpec = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoManifestacaoDestinatario = VersaoServico.Versao400;

            _configuracoes.CfgServico.DiretorioSalvarXml = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e" });
            _configuracoes.CfgServico.DiretorioSchemas = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Schemas" });
            _configuracoes.CfgServico.SalvarXmlServicos = false;
            _configuracoes.CfgServico.TimeOut = 160000;

            //if (nfe.Empresa.Localidade.Estado.Sigla == "MG")
            //{
            //    ServicePointManager.Expect100Continue = true;
            //    ServicePointManager.CheckCertificateRevocationList = false;
            //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            //}

            _configuracoes.ConfiguracaoDanfeNfe = new NFe.Danfe.Base.NFe.ConfiguracaoDanfeNfe();

            //_configuracoes.ConfiguracaoEmail = new ConfiguracaoEmail("nfe@commerce.inf.br", "cesaoexp18", "Envio de NF-e de " + nfe.Empresa.RazaoSocial + " nº " + nfe.Numero, "Teste", "smtp.commerce.inf.br", 587, false, true, 16000); //Não é mais utilizado, carrega da config empresa

            string htmlEmail;
            string assunto;
            if (nfe.Cliente != null)
            {
                assunto = "Envio de NF-e de " + nfe.Empresa.RazaoSocial + " nº " + nfe.Numero;
                htmlEmail = "<html>" +
                                "   <h3> Nota Fiscal Eletrônica (NF-e)</h3>" +
                                "   <h4> Prezado Cliente " + nfe.Cliente.Nome + " </h4>" +
                                "   <p> Segue anexo a Nota Fiscal Eletrônica (xml e pdf).</p>" +
                                "   <p>" +
                                "       <b> Chave de acesso:</b> " + nfe.Chave +
                                "       <br><b> Número:</b> " + nfe.Numero +
                                "       <br><b> Valor Total:</b> " + nfe.ValorTotalNota.ToString("n2") +
                                "   </p>" +
                                "   <p> Consulte a autencidade de sua NFe acessando <a href='https://www.nfe.fazenda.gov.br/portal/consultaRecaptcha.aspx?tipoConsulta=completa&tipoConteudo=XbSeqxE8pl8='> Consulta NFe Completa</a> </p>" +
                                "Commerce Sistemas LTDA" +
                                "</html>";
            }
            else
            {
                assunto = "Envio de NFC-e de " + nfe.Empresa.RazaoSocial + " nº " + nfe.Numero;
                htmlEmail = "<html>" +
                                "   <h3> Nota Fiscal de Consumidor Final Eletrônica (NFC-e)</h3>" +
                                "   <h4> Prezado Cliente " + nfe.NomeConsumidorFinal + " </h4>" +
                                "   <p> Segue anexo a Nota Fiscal de Consumidor Final Eletrônica (pdf e xml).</p>" +
                                "   <p>" +
                                "       <b> Chave de acesso:</b> " + nfe.Chave +
                                "       <br><b> Número:</b> " + nfe.Numero +
                                "       <br><b> Valor Total:</b> " + nfe.ValorTotalNota.ToString("n2") +
                                "   </p>" +
                                "Commerce Sistemas LTDA" +
                                "</html>";
            }

            _configuracoes.ConfiguracaoEmail.Assunto = assunto;
            _configuracoes.ConfiguracaoEmail.Assincrono = false;
            _configuracoes.ConfiguracaoEmail.Mensagem = htmlEmail;
            _configuracoes.ConfiguracaoEmail.MensagemEmHtml = true;
            _configuracoes.ConfiguracaoEmail.Timeout = 160000;

            if (!string.IsNullOrWhiteSpace(nfe.Empresa.IdCSCNFCe))
                _configuracoes.ConfiguracaoCsc = new ConfiguracaoCsc(nfe.Empresa.IdTokenNFCe, nfe.Empresa.IdCSCNFCe);
            else
                _configuracoes.ConfiguracaoCsc = new ConfiguracaoCsc("000001", "");
        }

        protected virtual void CriarConfiguracaoPadrao(Dominio.Entidades.Empresa empresa, ModeloDocumento modelo, Repositorio.UnitOfWork unitOfWork)
        {
            _configuracoes = new ConfiguracaoApp
            {
                Emitente = new emit
                {
                    CNAE = empresa.CNAE,
                    CNPJ = !string.IsNullOrWhiteSpace(empresa.Tipo) && empresa.Tipo.Equals("F") ? null : empresa.CNPJ,
                    CPF = !string.IsNullOrWhiteSpace(empresa.Tipo) && empresa.Tipo.Equals("F") ? empresa.CNPJ : null,
                    CRT = empresa.OptanteSimplesNacional && empresa.OptanteSimplesNacionalComExcessoReceitaBruta ? CRT.SimplesNacionalExcessoSublimite : empresa.OptanteSimplesNacional ? CRT.SimplesNacional : CRT.RegimeNormal,
                    enderEmit = new enderEmit
                    {
                        CEP = empresa.CEP,
                        cMun = empresa.Localidade.CodigoIBGE,
                        cPais = empresa.Localidade.Pais.Codigo,
                        fone = null,
                        nro = empresa.Numero,
                        UF = (DFe.Classes.Entidades.Estado)empresa.Localidade.Estado.CodigoIBGE,
                        xBairro = empresa.Bairro,
                        xCpl = !string.IsNullOrWhiteSpace(empresa.Complemento) ? empresa.Complemento.Trim().TrimEnd() : null,
                        xLgr = empresa.Endereco,
                        xMun = empresa.Localidade.Descricao,
                        xPais = empresa.Localidade.Pais.Nome
                    },
                    IE = empresa.InscricaoEstadual,
                    IM = !string.IsNullOrWhiteSpace(empresa.InscricaoMunicipal) ? empresa.InscricaoMunicipal : "ISENTO",
                    xFant = empresa.NomeFantasia,
                    xNome = empresa.RazaoSocial
                },
                EnderecoEmitente = new enderEmit
                {
                    CEP = empresa.CEP,
                    cMun = empresa.Localidade.CodigoIBGE,
                    cPais = empresa.Localidade.Pais.Codigo,
                    fone = null,
                    nro = empresa.Numero,
                    UF = (DFe.Classes.Entidades.Estado)empresa.Localidade.Estado.CodigoIBGE,
                    xBairro = empresa.Bairro,
                    xCpl = !string.IsNullOrWhiteSpace(empresa.Complemento) ? empresa.Complemento.Trim().TrimEnd() : null,
                    xLgr = empresa.Endereco,
                    xMun = empresa.Localidade.Descricao,
                    xPais = empresa.Localidade.Pais.Nome
                }
            };

            _configuracoes.CfgServico = ConfiguracaoServico.Instancia;
            _configuracoes.CfgServico.tpAmb = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? TipoAmbiente.Homologacao : TipoAmbiente.Producao;
            _configuracoes.CfgServico.tpEmis = TipoEmissao.teNormal;
            _configuracoes.CfgServico.cUF = (DFe.Classes.Entidades.Estado)empresa.Localidade.Estado.CodigoIBGE;
            _configuracoes.CfgServico.ModeloDocumento = modelo;
            _configuracoes.CfgServico.Certificado.Serial = empresa.SerieCertificado;
            _configuracoes.CfgServico.Certificado.Senha = empresa.SenhaCertificado;
            _configuracoes.CfgServico.Certificado.Arquivo = empresa.NomeCertificado;

            _configuracoes.CfgServico.VersaoNfceAministracaoCSC = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeAutorizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaCadastro = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaDest = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaProtocolo = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeDistribuicaoDFe = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeDownloadNF = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeInutilizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeRecepcao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeRetAutorizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeRetRecepcao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeStatusServico = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoCceCancelamento = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoEpec = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoManifestacaoDestinatario = VersaoServico.Versao400;

            _configuracoes.CfgServico.DiretorioSalvarXml = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "XML NF-e" });
            _configuracoes.CfgServico.DiretorioSchemas = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Schemas" });
            _configuracoes.CfgServico.SalvarXmlServicos = false;
            _configuracoes.CfgServico.TimeOut = 160000;

            _configuracoes.ConfiguracaoDanfeNfe = new NFe.Danfe.Base.NFe.ConfiguracaoDanfeNfe();

            //string htmlEmail;
            //string assunto;
            //if (nfe.Cliente != null)
            //{
            //    assunto = "Envio de NF-e de " + nfe.Empresa.RazaoSocial + " nº " + nfe.Numero;
            //    htmlEmail = "<html>" +
            //                    "   <h3> Nota Fiscal Eletrônica (NF-e)</h3>" +
            //                    "   <h4> Prezado Cliente " + nfe.Cliente.Nome + " </h4>" +
            //                    "   <p> Segue anexo a Nota Fiscal Eletrônica (xml e pdf).</p>" +
            //                    "   <p>" +
            //                    "       <b> Chave de acesso:</b> " + nfe.Chave +
            //                    "       <br><b> Número:</b> " + nfe.Numero +
            //                    "       <br><b> Valor Total:</b> " + nfe.ValorTotalNota.ToString("n2") +
            //                    "   </p>" +
            //                    "   <p> Consulte a autencidade de sua NFe acessando <a href='https://www.nfe.fazenda.gov.br/portal/consultaRecaptcha.aspx?tipoConsulta=completa&tipoConteudo=XbSeqxE8pl8='> Consulta NFe Completa</a> </p>" +
            //                    "Commerce Sistemas LTDA" +
            //                    "</html>";
            //}
            //else
            //{
            //    assunto = "Envio de NFC-e de " + nfe.Empresa.RazaoSocial + " nº " + nfe.Numero;
            //    htmlEmail = "<html>" +
            //                    "   <h3> Nota Fiscal de Consumidor Final Eletrônica (NFC-e)</h3>" +
            //                    "   <h4> Prezado Cliente " + nfe.NomeConsumidorFinal + " </h4>" +
            //                    "   <p> Segue anexo a Nota Fiscal de Consumidor Final Eletrônica (pdf e xml).</p>" +
            //                    "   <p>" +
            //                    "       <b> Chave de acesso:</b> " + nfe.Chave +
            //                    "       <br><b> Número:</b> " + nfe.Numero +
            //                    "       <br><b> Valor Total:</b> " + nfe.ValorTotalNota.ToString("n2") +
            //                    "   </p>" +
            //                    "Commerce Sistemas LTDA" +
            //                    "</html>";
            //}

            //_configuracoes.ConfiguracaoEmail.Assunto = assunto;
            //_configuracoes.ConfiguracaoEmail.Assincrono = false;
            //_configuracoes.ConfiguracaoEmail.Mensagem = htmlEmail;
            //_configuracoes.ConfiguracaoEmail.MensagemEmHtml = true;
            //_configuracoes.ConfiguracaoEmail.Timeout = 160000;

            //if (!string.IsNullOrWhiteSpace(nfe.Empresa.IdCSCNFCe))
            //    _configuracoes.ConfiguracaoCsc = new ConfiguracaoCsc(nfe.Empresa.IdTokenNFCe, nfe.Empresa.IdCSCNFCe);
            //else
            //    _configuracoes.ConfiguracaoCsc = new ConfiguracaoCsc("000001", "");
        }

        protected virtual void CriarConfiguracaoPadrao(string serial, NFe.Classes.NFe nfe, string cIdToken, string csc, string diretorioRaiz)
        {
            _configuracoes = new ConfiguracaoApp
            {
                Emitente = new emit
                {
                    CNAE = nfe.infNFe.emit.CNAE,
                    CNPJ = nfe.infNFe.emit.CNPJ,
                    CPF = nfe.infNFe.emit.CPF,
                    CRT = nfe.infNFe.emit.CRT,
                    enderEmit = new enderEmit
                    {
                        CEP = nfe.infNFe.emit.enderEmit.CEP,
                        cMun = nfe.infNFe.emit.enderEmit.cMun,
                        cPais = nfe.infNFe.emit.enderEmit.cPais,
                        fone = null,// !string.IsNullOrWhiteSpace(nfe.Empresa.Telefone) ? Convert.ToInt64(Utilidades.String.OnlyNumbers(nfe.Empresa.Telefone)) : 0,
                        nro = nfe.infNFe.emit.enderEmit.nro,
                        UF = nfe.infNFe.emit.enderEmit.UF,
                        xBairro = nfe.infNFe.emit.enderEmit.xBairro,
                        xCpl = !string.IsNullOrWhiteSpace(nfe.infNFe.emit.enderEmit.xCpl) ? nfe.infNFe.emit.enderEmit.xCpl.Trim().TrimEnd() : null,
                        xLgr = nfe.infNFe.emit.enderEmit.xLgr,
                        xMun = nfe.infNFe.emit.enderEmit.xMun,
                        xPais = nfe.infNFe.emit.enderEmit.xPais,
                    },
                    IE = nfe.infNFe.emit.IE,
                    IEST = !string.IsNullOrWhiteSpace(nfe.infNFe.emit.IEST) ? nfe.infNFe.emit.IEST : null,
                    IM = !string.IsNullOrWhiteSpace(nfe.infNFe.emit.IM) ? nfe.infNFe.emit.IM : "ISENTO",
                    xFant = nfe.infNFe.emit.xFant,
                    xNome = nfe.infNFe.emit.xNome
                },
                EnderecoEmitente = new enderEmit
                {
                    CEP = nfe.infNFe.emit.enderEmit.CEP,
                    cMun = nfe.infNFe.emit.enderEmit.cMun,
                    cPais = nfe.infNFe.emit.enderEmit.cPais,
                    fone = null,// !string.IsNullOrWhiteSpace(nfe.Empresa.Telefone) ? Convert.ToInt64(Utilidades.String.OnlyNumbers(nfe.Empresa.Telefone)) : 0,
                    nro = nfe.infNFe.emit.enderEmit.nro,
                    UF = nfe.infNFe.emit.enderEmit.UF,
                    xBairro = nfe.infNFe.emit.enderEmit.xBairro,
                    xCpl = !string.IsNullOrWhiteSpace(nfe.infNFe.emit.enderEmit.xCpl) ? nfe.infNFe.emit.enderEmit.xCpl.Trim().TrimEnd() : null,
                    xLgr = nfe.infNFe.emit.enderEmit.xLgr,
                    xMun = nfe.infNFe.emit.enderEmit.xMun,
                    xPais = nfe.infNFe.emit.enderEmit.xPais,
                }
            };

            _configuracoes.CfgServico = ConfiguracaoServico.Instancia;
            _configuracoes.CfgServico.tpAmb = nfe.infNFe.ide.tpAmb;
            _configuracoes.CfgServico.tpEmis = TipoEmissao.teNormal;
            _configuracoes.CfgServico.cUF = nfe.infNFe.ide.cUF;
            _configuracoes.CfgServico.ModeloDocumento = nfe.infNFe.ide.mod;//ModeloDocumento.NFe;
            _configuracoes.CfgServico.Certificado.Serial = serial;
            _configuracoes.CfgServico.Certificado.Senha = null;
            _configuracoes.CfgServico.Certificado.Arquivo = null;

            _configuracoes.CfgServico.VersaoNfceAministracaoCSC = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeAutorizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaCadastro = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaDest = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeConsultaProtocolo = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeDistribuicaoDFe = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeDownloadNF = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeInutilizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeRecepcao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNFeRetAutorizacao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeRetRecepcao = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoNfeStatusServico = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoCceCancelamento = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoEpec = VersaoServico.Versao400;
            _configuracoes.CfgServico.VersaoRecepcaoEventoManifestacaoDestinatario = VersaoServico.Versao400;

            _configuracoes.CfgServico.DiretorioSalvarXml = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, "XML NF-e");
            _configuracoes.CfgServico.DiretorioSchemas = Utilidades.IO.FileStorageService.Storage.Combine(diretorioRaiz, "Schemas");
            _configuracoes.CfgServico.SalvarXmlServicos = false;
            _configuracoes.CfgServico.TimeOut = 160000;

            _configuracoes.ConfiguracaoDanfeNfe = new NFe.Danfe.Base.NFe.ConfiguracaoDanfeNfe();

            _configuracoes.ConfiguracaoEmail = new ConfiguracaoEmail("nfe@commerce.inf.br", "cesaoexp18", "Envio de NF-e de ", "Teste", "smtp.commerce.inf.br", 587, false, true, 16000); //Não é mais utilizado, carrega da config empresa

            var htmlEmail = "<html>" +
                            "</html>";

            _configuracoes.ConfiguracaoEmail.Assincrono = false;
            _configuracoes.ConfiguracaoEmail.Mensagem = htmlEmail;
            _configuracoes.ConfiguracaoEmail.MensagemEmHtml = true;
            _configuracoes.ConfiguracaoEmail.Timeout = 160000;

            if (!string.IsNullOrWhiteSpace(cIdToken))
                _configuracoes.ConfiguracaoCsc = new ConfiguracaoCsc(cIdToken, csc);
            else
                _configuracoes.ConfiguracaoCsc = new ConfiguracaoCsc("000001", "");
        }

        #endregion

        #region Métodos de cadastros automáticos

        public Dominio.Entidades.Cliente CadastrarPessoa(dest destinatario, Repositorio.UnitOfWork unitOfWork)
        {
            if (destinatario != null)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Dominio.Entidades.Cliente pessoa = null;
                if (!string.IsNullOrWhiteSpace(destinatario.CNPJ))
                    pessoa = repCliente.BuscarPorCPFCNPJ(Convert.ToDouble(destinatario.CNPJ));
                else if (!string.IsNullOrWhiteSpace(destinatario.CPF))
                    pessoa = repCliente.BuscarPorCPFCNPJ(Convert.ToDouble(destinatario.CPF));
                else if (!string.IsNullOrWhiteSpace(destinatario.enderDest?.xLgr))
                    pessoa = repCliente.BuscarPorNomeEndereco(destinatario.xNome, destinatario.enderDest?.xLgr ?? "");
                if (pessoa != null)
                    return pessoa;
                else
                {
                    pessoa = new Dominio.Entidades.Cliente();
                    if (!string.IsNullOrWhiteSpace(destinatario.CNPJ))
                    {
                        pessoa.CPF_CNPJ = Convert.ToDouble(destinatario.CNPJ);
                        pessoa.Tipo = "J";
                    }
                    else if (!string.IsNullOrWhiteSpace(destinatario.CPF))
                    {
                        pessoa.CPF_CNPJ = Convert.ToDouble(destinatario.CPF);
                        pessoa.Tipo = "F";
                    }
                    else
                    {
                        pessoa.CPF_CNPJ = repCliente.BuscarPorProximoExterior();
                        pessoa.Tipo = "E";
                    }

                    pessoa.Nome = destinatario.xNome;
                    pessoa.NomeFantasia = destinatario.xNome;
                    pessoa.Endereco = destinatario.enderDest.xLgr;
                    pessoa.Numero = destinatario.enderDest.nro;
                    pessoa.Complemento = !string.IsNullOrWhiteSpace(destinatario.enderDest.xCpl) ? destinatario.enderDest.xCpl.Trim().TrimEnd() : null;
                    pessoa.Bairro = destinatario.enderDest.xBairro;
                    pessoa.Localidade = repLocalidade.BuscarPorCodigoIBGE((int)destinatario.enderDest.cMun);
                    if (pessoa.Localidade == null)
                        return null;
                    pessoa.CEP = destinatario.enderDest.CEP;
                    if (!string.IsNullOrWhiteSpace(destinatario.IE))
                        pessoa.IE_RG = destinatario.IE;
                    else
                        pessoa.IE_RG = "ISENTO";
                    if (string.IsNullOrWhiteSpace(pessoa.CEP))
                        pessoa.CEP = pessoa.Localidade.CEP;
                    pessoa.InscricaoMunicipal = destinatario.IM;
                    pessoa.Atividade = repAtividade.BuscarPrimeiraAtividade();
                    pessoa.Telefone1 = destinatario.enderDest.fone.ToString();
                    pessoa.TipoLogradouro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Outros;
                    pessoa.TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Outros;
                    pessoa.TipoEmail = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Outros;
                    pessoa.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                    pessoa.Email = !string.IsNullOrWhiteSpace(destinatario.email) && !destinatario.email.Contains(";") ? destinatario.email.Trim().TrimEnd() : null;
                    if (destinatario.indIEDest == indIEDest.ContribuinteICMS)
                        pessoa.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                    else if (destinatario.indIEDest == indIEDest.Isento)
                        pessoa.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteIsento;
                    else if (destinatario.indIEDest == indIEDest.NaoContribuinte)
                        pessoa.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;

                    if (pessoa.Tipo == "J" && pessoa.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(pessoa.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            pessoa.GrupoPessoas = grupoPessoas;
                        }
                    }
                    pessoa.Ativo = true;
                    repCliente.Inserir(pessoa);

                    return pessoa;
                }
            }
            else
            {
                return null;
            }
        }
        public Dominio.Entidades.Embarcador.NotaFiscal.Servico CadastrarServico(prod item, decimal aliquotaISS, string codigoServico, Repositorio.UnitOfWork unitOfWork, int codigoEmpresa)
        {
            if (item != null)
            {
                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = null;
                servico = repServico.BuscarPorCodigoIntegracao(item.cProd, codigoEmpresa);

                if (servico != null)
                    return servico;
                else
                {
                    servico = new Dominio.Entidades.Embarcador.NotaFiscal.Servico();
                    servico.Descricao = item.xProd;
                    servico.DescricaoNFE = item.xProd;
                    servico.ValorVenda = item.vUnCom;
                    servico.AliquotaISS = aliquotaISS;
                    servico.CodigoServico = servico.CodigoServicoParaEnum(codigoServico);
                    servico.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    servico.Status = true;

                    repServico.Inserir(servico);

                    return servico;
                }
            }
            else
                return null;
        }
        public Dominio.Entidades.Produto CadastrarProduto(prod item, Repositorio.UnitOfWork unitOfWork, int codigoEmpresa, List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (item == null)
                return null;

            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigoIntegracao(codigoEmpresa, item.cProd);

            if (produto != null)
                return produto;
            else
            {
                produto = new Dominio.Entidades.Produto();
                produto.Descricao = item.xProd;
                produto.DescricaoNotaFiscal = item.xProd;
                produto.CodigoProduto = item.cProd;
                produto.CodigoNCM = item.NCM;
                produto.UnidadeDeMedida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade;
                produto.Status = "A";
                produto.CategoriaProduto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.MercadoriaRevenda;
                produto.CodigoCEST = item.CEST;
                produto.UltimoCusto = 0;
                produto.CustoMedio = 0;
                produto.MargemLucro = 100;
                produto.ValorVenda = item.vUnCom;
                produto.OrigemMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem0;
                produto.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (ncmsAbastecimento != null && ncmsAbastecimento.Count() > 0 && !string.IsNullOrWhiteSpace(produto.CodigoNCM))
                {
                    if (ncmsAbastecimento.Where(o => produto.CodigoNCM.Contains(o.NCM)).Count() > 0)
                        produto.ProdutoCombustivel = true;
                    else
                        produto.ProdutoCombustivel = false;
                }
                else
                    produto.ProdutoCombustivel = false;

                repProduto.Inserir(produto);

                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);
                servicoEstoque.AdicionarEstoque(produto, produto.Empresa, tipoServicoMultisoftware, configuracao);

                return produto;
            }
        }
        public Dominio.Entidades.EmpresaSerie CadastrarSerieEmpresa(int numeroSerie, Repositorio.UnitOfWork unitOfWork, int codigoEmpresa)
        {
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.EmpresaSerie serie = null;
            serie = repEmpresaSerie.BuscarPorSerie(codigoEmpresa, numeroSerie, Dominio.Enumeradores.TipoSerie.NFe);

            if (serie != null)
                return serie;
            else
            {
                serie = new Dominio.Entidades.EmpresaSerie();
                serie.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                serie.Numero = numeroSerie;
                serie.Status = "A";
                serie.Tipo = Dominio.Enumeradores.TipoSerie.NFe;

                repEmpresaSerie.Inserir(serie);

                return serie;
            }
        }
        public Dominio.Entidades.NaturezaDaOperacao CadastrarNaturezaOperacao(string descricao, string estadoCliente, string estadoEmpresa, Repositorio.UnitOfWork unitOfWork, int codigoEmpresa)
        {
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.NaturezaDaOperacao natureza = null;
            natureza = repNaturezaDaOperacao.BuscarNaturezaNFe(descricao, codigoEmpresa);

            if (natureza != null)
                return natureza;
            else
            {
                natureza = new Dominio.Entidades.NaturezaDaOperacao();
                natureza.Descricao = descricao;
                natureza.Status = "A";
                natureza.DentroEstado = estadoCliente == estadoEmpresa;
                natureza.GeraTitulo = true;
                natureza.Garantia = false;
                natureza.Demonstracao = false;
                natureza.Bonificacao = false;
                natureza.Outras = false;
                natureza.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                repNaturezaDaOperacao.Inserir(natureza);

                return natureza;
            }
        }
        public Dominio.Entidades.CFOP CadastrarCFOP(int codigoCFOP, Repositorio.UnitOfWork unitOfWork, int codigoEmpresa, Dominio.Enumeradores.TipoCFOP tipoCFOP)
        {
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.CFOP cfop = null;
            cfop = repCFOP.BuscarPorCFOPEmpresa(codigoCFOP, codigoEmpresa);

            if (cfop != null)
                return cfop;
            else
            {
                cfop = new Dominio.Entidades.CFOP();
                cfop.CodigoCFOP = codigoCFOP;
                cfop.Status = "A";
                cfop.Tipo = tipoCFOP;
                cfop.GeraEstoque = true;
                cfop.Descricao = codigoCFOP.ToString();

                cfop.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                repCFOP.Inserir(cfop);

                return cfop;
            }
        }

        #endregion

        #region Métodos de preenchimento do Componente

        protected virtual NFe.Classes.NFe GetNf(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ModeloDocumento modelo, VersaoServico versao, Repositorio.UnitOfWork unitOfWork, TipoEmissao tipoEmissao)
        {
            var nf = new NFe.Classes.NFe { infNFe = GetInf(nfe, modelo, versao, unitOfWork, tipoEmissao) };
            return nf;
        }
        protected virtual infNFe GetInf(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ModeloDocumento modelo, VersaoServico versao, Repositorio.UnitOfWork unitOfWork, TipoEmissao tipoEmissao)
        {
            infNFe infNFe = new infNFe
            {
                versao = Conversao.VersaoServicoParaString(versao),
                ide = GetIdentificacao(nfe, modelo, versao, unitOfWork, tipoEmissao),
                emit = GetEmitente(nfe, modelo, unitOfWork),
                dest = GetDestinatario(nfe, versao, modelo),
                entrega = nfe.UtilizarEnderecoEntrega ? GetEntrega(nfe) : null,
                retirada = nfe.UtilizarEnderecoRetirada ? GetRetirada(nfe) : null,
                transp = GetTransporte(nfe)
            };

            if (nfe.Empresa.Localidade.Estado.Sigla == "BA")
            {
                if (infNFe.ide.mod == ModeloDocumento.NFe && versao >= VersaoServico.Versao310)
                    infNFe.autXML = GetAutXML(nfe, "13937073000156");
            }
            else
            {
                if (infNFe.ide.mod == ModeloDocumento.NFe && versao >= VersaoServico.Versao310)
                    infNFe.autXML = GetAutXML(nfe, "16482040000157");
            }

            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> itens = repNotaFiscalProdutos.BuscarPorNotaOrdemDescricao(nfe.Codigo);

            for (var i = 0; i < itens.Count; i++)
            {
                infNFe.det.Add(GetDetalhe(itens[i], i, infNFe.emit.CRT, modelo, unitOfWork));
            }

            infNFe.total = GetTotal(nfe, versao, infNFe.det);

            if (infNFe.ide.mod == ModeloDocumento.NFe & versao >= VersaoServico.Versao310)
                infNFe.cobr = GetCobranca(nfe, infNFe.total.ICMSTot, unitOfWork);
            if (versao == VersaoServico.Versao400)
                infNFe.pag = GetPagamentoVersao400(nfe, infNFe.total.ICMSTot, infNFe.cobr);
            //if (infNFe.ide.mod == ModeloDocumento.NFCe)
            //    infNFe.pag = GetPagamento(nfe, infNFe.total.ICMSTot); //NFCe Somente  na 3.10

            if (!string.IsNullOrWhiteSpace(nfe.ObservacaoTributaria) || !string.IsNullOrWhiteSpace(nfe.ObservacaoNota))
                infNFe.infAdic = new infAdic()
                {
                    infCpl = !string.IsNullOrWhiteSpace(nfe.ObservacaoNota) ? Utilidades.String.RemoveDiacritics((nfe.ObservacaoNota.Trim().Replace(System.Environment.NewLine, " "))).Trim().TrimEnd().TrimStart() : null,
                    infAdFisco = !string.IsNullOrWhiteSpace(nfe.ObservacaoTributaria) ? Utilidades.String.RemoveDiacritics((nfe.ObservacaoTributaria.Trim().Replace(System.Environment.NewLine, " "))).Trim().TrimEnd().TrimStart() : null
                };
            if (infNFe.infAdic != null)
            {
                if (infNFe.infAdic.infAdFisco != null && !string.IsNullOrWhiteSpace(infNFe.infAdic.infAdFisco))
                    infNFe.infAdic.infAdFisco = infNFe.infAdic.infAdFisco.Replace("º", "").Replace("ª", "").Replace("\n", " - ");
                if (infNFe.infAdic.infCpl != null && !string.IsNullOrWhiteSpace(infNFe.infAdic.infCpl))
                    infNFe.infAdic.infCpl = infNFe.infAdic.infCpl.Replace("º", "").Replace("ª", "").Replace("\n", " - ");
            }

            if (!string.IsNullOrWhiteSpace(nfe.UFEmbarque) && !string.IsNullOrWhiteSpace(nfe.LocalEmbarque))
            {
                infNFe.exporta = new exporta();
                infNFe.exporta.UFSaidaPais = nfe.UFEmbarque;
                infNFe.exporta.xLocExporta = Utilidades.String.RemoveDiacritics(nfe.LocalEmbarque);
                if (!string.IsNullOrWhiteSpace(nfe.LocalDespacho))
                    infNFe.exporta.xLocDespacho = Utilidades.String.RemoveDiacritics(nfe.LocalDespacho);
            }

            if (!string.IsNullOrWhiteSpace(nfe.InformacaoCompraContrato) || !string.IsNullOrWhiteSpace(nfe.InformacaoCompraPedido) || !string.IsNullOrWhiteSpace(nfe.InformacaoCompraNotaEmpenho))
            {
                infNFe.compra = new compra();
                if (!string.IsNullOrWhiteSpace(nfe.InformacaoCompraContrato))
                    infNFe.compra.xCont = Utilidades.String.RemoveDiacritics(nfe.InformacaoCompraContrato);

                if (!string.IsNullOrWhiteSpace(nfe.InformacaoCompraPedido))
                    infNFe.compra.xPed = Utilidades.String.RemoveDiacritics(nfe.InformacaoCompraPedido);

                if (!string.IsNullOrWhiteSpace(nfe.InformacaoCompraNotaEmpenho))
                    infNFe.compra.xNEmp = Utilidades.String.RemoveDiacritics(nfe.InformacaoCompraNotaEmpenho);
            }

            if (versao >= VersaoServico.Versao400)
            {
                infNFe.infRespTec = GetResponsavelTecnico(infNFe, nfe.Empresa.Localidade.Estado.Sigla, nfe.TipoAmbiente, unitOfWork);
                infNFe.infIntermed = GetIntermediadorTransacao(infNFe.ide, nfe.Intermediador);
            }

            return infNFe;
        }
        protected virtual ide GetIdentificacao(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ModeloDocumento modelo, VersaoServico versao, Repositorio.UnitOfWork unitOfWork, TipoEmissao tipoEmissao)
        {
            var ide = new ide
            {
                cUF = (DFe.Classes.Entidades.Estado)nfe.Empresa.Localidade.Estado.CodigoIBGE,
                natOp = nfe.NaturezaDaOperacao != null ? Utilidades.String.RemoveDiacritics(nfe.NaturezaDaOperacao.Descricao.Trim()) : "VENDA PARA CONSUMIDOR FINAL",
                //indPag = nfe.ParcelasNFe == null || nfe.ParcelasNFe.Count() == 0 ? IndicadorPagamento.ipOutras : nfe.ParcelasNFe.Count() == 1 ? IndicadorPagamento.ipVista : IndicadorPagamento.ipPrazo,//IndicadorPagamento.ipVista,
                mod = modelo,
                serie = nfe.EmpresaSerie.Numero,
                nNF = nfe.Numero,

                tpNF = nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida ? TipoNFe.tnSaida : TipoNFe.tnEntrada,
                cMunFG = nfe.Empresa.Localidade.CodigoIBGE,
                tpEmis = tipoEmissao,
                tpImp = TipoImpressao.tiRetrato,
                cNF = "9" + nfe.Codigo.ToString(),
                tpAmb = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? TipoAmbiente.Homologacao : TipoAmbiente.Producao,
                finNFe = nfe.Finalidade == Dominio.Enumeradores.FinalidadeNFe.Normal ? FinalidadeNFe.fnNormal : nfe.Finalidade == Dominio.Enumeradores.FinalidadeNFe.Ajuste ? FinalidadeNFe.fnAjuste : nfe.Finalidade == Dominio.Enumeradores.FinalidadeNFe.Complementar ? FinalidadeNFe.fnComplementar : FinalidadeNFe.fnDevolucao,
                verProc = "MultiTMS 4.00"
            };

            if (ide.tpEmis != tipoEmissao)
            {
                ide.dhCont = DateTime.Now;
                ide.xJust = "TESTE DE CONTIGÊNCIA PARA NFe/NFCe";
            }

            #region V2.00

            if (versao == VersaoServico.Versao200)
            {
                ide.dEmi = DateTime.Today; //Mude aqui para enviar a nfe vinculada ao EPEC, V2.00
                ide.dSaiEnt = DateTime.Today;
            }

            #endregion

            #region V3.00

            if (versao < VersaoServico.Versao310) return ide;
            if (modelo == ModeloDocumento.NFe)
                ide.idDest = nfe.Cliente.Localidade.Estado.Sigla == "EX" ? DestinoOperacao.doExterior : nfe.Empresa.Localidade.Estado.Sigla == nfe.Cliente.Localidade.Estado.Sigla || (nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.Presencial && nfe.Cliente.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte) ? DestinoOperacao.doInterna : DestinoOperacao.doInterestadual;
            else
                ide.idDest = DestinoOperacao.doInterna;

            if (nfe.DataEmissao != null && nfe.DataEmissao.HasValue && ide.mod == ModeloDocumento.NFe)
                ide.dhEmi = nfe.DataEmissao.Value;
            else
                ide.dhEmi = DateTime.Now;

            //Mude aqui para enviar a nfe vinculada ao EPEC, V3.10
            if (ide.mod == ModeloDocumento.NFe)
            {
                if (nfe.DataSaida != null && nfe.DataSaida.HasValue)
                    ide.dhSaiEnt = nfe.DataSaida.Value;
                else
                    ide.dhSaiEnt = DateTime.Today;
                //ide.dhSaiEnt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
            }
            else
                ide.tpImp = TipoImpressao.tiNFCe;
            ide.procEmi = ProcessoEmissao.peAplicativoContribuinte;
            if (nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && nfe.Cliente != null &&
                    (nfe.Cliente.Localidade.Estado.Sigla == "SP" || nfe.Cliente.Localidade.Estado.Sigla == "GO" || nfe.Cliente.Localidade.Estado.Sigla == "PA" || nfe.Cliente.Localidade.Estado.Sigla == "MT" || nfe.Cliente.Localidade.Estado.Sigla == "MG" || nfe.Cliente.Localidade.Estado.Sigla == "RN"
                    || nfe.Cliente.Localidade.Estado.Sigla == "AM" || nfe.Cliente.Localidade.Estado.Sigla == "BA" || nfe.Cliente.Localidade.Estado.Sigla == "MS" || nfe.Cliente.Localidade.Estado.Sigla == "AL" || nfe.Cliente.Localidade.Estado.Sigla == "SE" || nfe.Cliente.Localidade.Estado.Sigla == "CE" || nfe.Cliente.Localidade.Estado.Sigla == "MA"))
            {
                ide.indFinal = ConsumidorFinal.cfConsumidorFinal;
            }
            else if (modelo == ModeloDocumento.NFe)
                ide.indFinal = nfe.Cliente.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte ? ConsumidorFinal.cfConsumidorFinal : ConsumidorFinal.cfNao; //NFCe: Tem que ser consumidor Final
            else
                ide.indFinal = ConsumidorFinal.cfConsumidorFinal;
            ide.indPres = nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.Internet ? PresencaComprador.pcInternet :
                nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.NaoSeAplica ? PresencaComprador.pcNao :
                nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.NFCe ? PresencaComprador.pcPresencial :
                nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.Outros ? PresencaComprador.pcOutros :
                nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.Presencial ? PresencaComprador.pcPresencial :
                nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.PresencialForaEmpresa ? PresencaComprador.pcPresencialForaEstabelecimento :
                PresencaComprador.pcTeleatendimento; //NFCe: deve ser 1 ou 4

            if (nfe.IndicadorIntermediador.HasValue)
                ide.indIntermed = nfe.IndicadorIntermediador.Value == Dominio.Enumeradores.IndicadorIntermediadorNFe.SitePlataformaTerceiros ? IndicadorIntermediador.iiSitePlataformaTerceiros : IndicadorIntermediador.iiSemIntermediador;

            ide.NFref = GerReferencia(nfe, unitOfWork);

            #endregion

            return ide;
        }
        protected virtual List<NFref> GerReferencia(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalReferencia repNotaFiscalReferencia = new Repositorio.Embarcador.NotaFiscal.NotaFiscalReferencia(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalReferencia referencias = repNotaFiscalReferencia.BuscarPorNota(nfe.Codigo);
            if (referencias != null)
            {
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                var referencia = new List<NFref>
                {
                    new NFref {
                        refECF = referencias.TipoDocumento == Dominio.Enumeradores.TipoDocumentoReferenciaNFe.CupomFiscal ? new refECF { mod = referencias.Modelo, nCOO = Convert.ToInt32(Utilidades.String.OnlyNumbers(referencias.COO)), nECF = Convert.ToInt32(Utilidades.String.OnlyNumbers(referencias.NumeroECF)) } : null,
                        refNF =  referencias.TipoDocumento == Dominio.Enumeradores.TipoDocumentoReferenciaNFe.NFModelo1 ?  new refNF { AAMM = referencias.DataEmissao.Value.ToString("yyMM"), CNPJ = Utilidades.String.OnlyNumbers(referencias.CNPJEmitente), cUF = (DFe.Classes.Entidades.Estado)repEstado.BuscarPorSigla(referencias.UF).CodigoIBGE, mod = referencias.Modelo.ToEnum<NFe.Classes.Informacoes.Identificacao.Tipos.refMod>(), nNF = Convert.ToInt32(Utilidades.String.OnlyNumbers(referencias.Numero)), serie = Convert.ToInt32(Utilidades.String.OnlyNumbers(referencias.Serie))  } : null,
                        refNFe = referencias.TipoDocumento == Dominio.Enumeradores.TipoDocumentoReferenciaNFe.NF ?  referencias.Chave : null,
                        refNFP = referencias.TipoDocumento == Dominio.Enumeradores.TipoDocumentoReferenciaNFe.NFProdutorRural ?  new refNFP { AAMM = referencias.DataEmissao.Value.ToString("yyMM"), CNPJ = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(referencias.CNPJEmitente)) ? Utilidades.String.OnlyNumbers(referencias.CNPJEmitente) : null, cUF = (DFe.Classes.Entidades.Estado)repEstado.BuscarPorSigla(referencias.UF).CodigoIBGE, mod = referencias.Modelo, nNF = Convert.ToInt32(Utilidades.String.OnlyNumbers(referencias.Numero)), serie = Convert.ToInt32(Utilidades.String.OnlyNumbers(referencias.Serie)), CPF = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(referencias.CPFEmitente)) ?  Utilidades.String.OnlyNumbers(referencias.CPFEmitente) : null, IE = referencias.IEEmitente  } : null,
                        refCTe = referencias.TipoDocumento == Dominio.Enumeradores.TipoDocumentoReferenciaNFe.CTe ? referencias.Chave : null
                    }
                };
                return referencia;
            }
            else
                return null;
        }
        protected virtual emit GetEmitente(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ModeloDocumento modelo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorInscricaoST repTransportadorInscricaoST = new Repositorio.Embarcador.Transportadores.TransportadorInscricaoST(unitOfWork);

            Dominio.Entidades.Empresa empresa = nfe.Empresa;
            Dominio.Entidades.Embarcador.Transportadores.TransportadorInscricaoST transportadorInscricaoST = modelo == ModeloDocumento.NFe ? repTransportadorInscricaoST.BuscarPorEmpresaEEstado(empresa.Codigo, nfe.Cliente.Localidade.Estado.Sigla) : null;

            emit emit = new emit
            {
                IEST = transportadorInscricaoST?.InscricaoST ?? null,
                IM = !string.IsNullOrWhiteSpace(empresa.InscricaoMunicipal) ? empresa.InscricaoMunicipal : "ISENTO",
                xNome = Utilidades.String.RemoveDiacritics(empresa.RazaoSocial),
                xFant = Utilidades.String.RemoveDiacritics(empresa.NomeFantasia),
                IE = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(empresa.InscricaoEstadual)) ? Utilidades.String.OnlyNumbers(empresa.InscricaoEstadual) : "ISENTO",
                CNPJ = !string.IsNullOrWhiteSpace(empresa.Tipo) && empresa.Tipo.Equals("F") ? null : empresa.CNPJ,
                CPF = !string.IsNullOrWhiteSpace(empresa.Tipo) && empresa.Tipo.Equals("F") ? empresa.CNPJ : null,
                CNAE = !string.IsNullOrWhiteSpace(empresa.CNAE) ? Utilidades.String.OnlyNumbers(empresa.CNAE) : null,
                CRT = empresa.OptanteSimplesNacional && empresa.OptanteSimplesNacionalComExcessoReceitaBruta ? CRT.SimplesNacionalExcessoSublimite : empresa.OptanteSimplesNacional ? CRT.SimplesNacional : CRT.RegimeNormal
            };
            emit.enderEmit = GetEnderecoEmitente(nfe);
            return emit;
        }
        protected virtual enderEmit GetEnderecoEmitente(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var enderEmit = new enderEmit
            {
                xLgr = !string.IsNullOrWhiteSpace(nfe.Empresa.Endereco) ? Utilidades.String.RemoveDiacritics(nfe.Empresa.Endereco.Trim()) : string.Empty,
                nro = !string.IsNullOrWhiteSpace(nfe.Empresa.Numero) ? Utilidades.String.RemoveDiacritics(nfe.Empresa.Numero.Trim()) : string.Empty,
                xCpl = !string.IsNullOrWhiteSpace(nfe.Empresa.Complemento) ? Utilidades.String.RemoveDiacritics(nfe.Empresa.Complemento.Trim()) : null,
                xBairro = Utilidades.String.RemoveDiacritics(nfe.Empresa.Bairro),
                cMun = nfe.Empresa.Localidade.CodigoIBGE,
                xMun = Utilidades.String.RemoveDiacritics(nfe.Empresa.Localidade.Descricao),
                UF = (DFe.Classes.Entidades.Estado)nfe.Empresa.Localidade.Estado.CodigoIBGE,
                CEP = nfe.Empresa.CEP,
                fone = null,// !string.IsNullOrWhiteSpace(nfe.Empresa.Telefone) ? Convert.ToInt64(Utilidades.String.OnlyNumbers(nfe.Empresa.Telefone)) : 0,
                cPais = nfe.Empresa.Localidade.Pais.Codigo,
                xPais = nfe.Empresa.Localidade.Pais.Nome
            };
            return enderEmit;
        }
        protected virtual dest GetDestinatario(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, VersaoServico versao, ModeloDocumento modelo)
        {
            if (modelo == ModeloDocumento.NFe)
            {
                var dest = new dest(versao)
                {
                    CNPJ = nfe.Cliente.Localidade.Estado.Sigla == "EX" ? null : nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "99999999000191" : nfe.Cliente.Tipo == "J" ? Convert.ToString(Utilidades.String.OnlyNumbers(nfe.Cliente.CPF_CNPJ_Formatado)) : null,
                    email = !string.IsNullOrWhiteSpace(nfe.Cliente.Email) && !nfe.Cliente.Email.Contains(";") ? nfe.Cliente.Email.Trim().TrimEnd() : null,
                    CPF = nfe.Cliente.Localidade.Estado.Sigla == "EX" ? null : nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? null : nfe.Cliente.Tipo == "F" ? Convert.ToString(Utilidades.String.OnlyNumbers(nfe.Cliente.CPF_CNPJ_Formatado)) : null,
                    idEstrangeiro = nfe.Cliente.Localidade.Estado.Sigla == "EX" ? nfe.Cliente.RG_Passaporte.ToString() : nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? null : nfe.Cliente.Tipo == "E" && !string.IsNullOrWhiteSpace(nfe.Cliente.RG_Passaporte) ? nfe.Cliente.RG_Passaporte.ToString() : null,
                    IE = nfe.Cliente.Localidade.Estado.Sigla == "EX" || nfe.Cliente.IE_RG == "ISENTO" ? null : nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? null : nfe.Cliente.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteIsento ? null : !string.IsNullOrWhiteSpace(nfe.Cliente.IE_RG) ? nfe.Cliente.IE_RG != "ISENTO" ? Utilidades.String.OnlyNumbers(nfe.Cliente.IE_RG) : nfe.Cliente.IE_RG : null,
                    IM = nfe.Cliente.Localidade.Estado.Sigla == "EX" ? null : nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? null : !string.IsNullOrWhiteSpace(nfe.Cliente.InscricaoMunicipal) ? nfe.Cliente.InscricaoMunicipal : null,
                    ISUF = nfe.Cliente.Localidade.Estado.Sigla == "EX" ? null : nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? null : !string.IsNullOrWhiteSpace(nfe.Cliente.InscricaoSuframa) ? nfe.Cliente.InscricaoSuframa : null
                };
                string nomeDestinatario = nfe.Cliente.Nome;
                if (nomeDestinatario.Length > 60)
                    nomeDestinatario = nomeDestinatario.Substring(0, 60);
                nomeDestinatario = Utilidades.String.RemoveDiacritics(nomeDestinatario.Trim());
                dest.xNome = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL" : nomeDestinatario;
                dest.enderDest = GetEnderecoDestinatario(nfe);

                if (versao < VersaoServico.Versao310)
                    return dest;

                if (nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && nfe.Cliente != null &&
                    (nfe.Cliente.Localidade.Estado.Sigla == "SP" || nfe.Cliente.Localidade.Estado.Sigla == "GO" || nfe.Cliente.Localidade.Estado.Sigla == "PA" || nfe.Cliente.Localidade.Estado.Sigla == "MT" || nfe.Cliente.Localidade.Estado.Sigla == "MG" || nfe.Cliente.Localidade.Estado.Sigla == "RN"
                    || nfe.Cliente.Localidade.Estado.Sigla == "AM" || nfe.Cliente.Localidade.Estado.Sigla == "BA" || nfe.Cliente.Localidade.Estado.Sigla == "MS" || nfe.Cliente.Localidade.Estado.Sigla == "AL" || nfe.Cliente.Localidade.Estado.Sigla == "SE" || nfe.Cliente.Localidade.Estado.Sigla == "CE" || nfe.Cliente.Localidade.Estado.Sigla == "MA"))
                {
                    dest.indIEDest = indIEDest.NaoContribuinte;
                    dest.IE = null;
                }
                else
                    dest.indIEDest = nfe.Cliente.Localidade.Estado.Sigla == "EX" ? indIEDest.NaoContribuinte : nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? indIEDest.Isento : nfe.Cliente.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS ? indIEDest.ContribuinteICMS : nfe.Cliente.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteIsento ? indIEDest.Isento : indIEDest.NaoContribuinte; //NFCe: Tem que ser não contribuinte V3.00 Somente

                return dest;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(nfe.CPFCNPJConsumidorFinal))
                {
                    string nomeDestinatario = "";
                    if (!string.IsNullOrWhiteSpace(nfe.NomeConsumidorFinal))
                    {
                        nomeDestinatario = nfe.NomeConsumidorFinal;
                        if (nomeDestinatario.Length > 60)
                            nomeDestinatario = nomeDestinatario.Substring(0, 60);
                    }
                    var dest = new dest(versao)
                    {
                        CNPJ = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "99999999000191" : Utilidades.String.OnlyNumbers(nfe.CPFCNPJConsumidorFinal).Length == 14 ? Utilidades.String.OnlyNumbers(nfe.CPFCNPJConsumidorFinal) : null,
                        CPF = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? null : Utilidades.String.OnlyNumbers(nfe.CPFCNPJConsumidorFinal).Length == 11 ? Utilidades.String.OnlyNumbers(nfe.CPFCNPJConsumidorFinal) : null,
                        xNome = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL" : !string.IsNullOrWhiteSpace(nomeDestinatario) ? nomeDestinatario : null,
                        indIEDest = indIEDest.NaoContribuinte
                    };
                    return dest;
                }
                else
                    return null;
            }
        }

        protected virtual entrega GetEntrega(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var entre = new entrega()
            {
                cMun = nfe.LocalidadeEntrega?.CodigoIBGE ?? 0,
                CNPJ = nfe.ClienteEntrega?.Tipo != "F" ? nfe.ClienteEntrega?.CPF_CNPJ_SemFormato ?? string.Empty : null,
                CPF = nfe.ClienteEntrega?.Tipo == "F" ? nfe.ClienteEntrega?.CPF_CNPJ_SemFormato ?? string.Empty : null,
                nro = nfe.EntregaNumeroLogradouro ?? string.Empty,
                UF = nfe.LocalidadeEntrega?.Estado?.Sigla ?? string.Empty,
                xBairro = nfe.EntregaBairro ?? string.Empty,
                xCpl = nfe.EntregaComplementoLogradouro ?? string.Empty,
                xLgr = nfe.EntregaLogradouro ?? string.Empty,
                xMun = nfe.LocalidadeEntrega?.Descricao ?? string.Empty,
                CEP = !string.IsNullOrWhiteSpace(nfe.LocalidadeEntrega?.CEP ?? string.Empty) ? nfe.LocalidadeEntrega.CEP.ObterSomenteNumeros().ToLong() : 0
            };

            return entre;
        }

        protected virtual retirada GetRetirada(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var ret = new retirada()
            {
                cMun = nfe.LocalidadeRetirada?.CodigoIBGE ?? 0,
                CNPJ = nfe.ClienteRetirada?.Tipo != "F" ? nfe.ClienteRetirada?.CPF_CNPJ_SemFormato ?? string.Empty : null,
                CPF = nfe.ClienteRetirada?.Tipo == "F" ? nfe.ClienteRetirada?.CPF_CNPJ_SemFormato ?? string.Empty : null,
                nro = nfe.RetiradaNumeroLogradouro ?? string.Empty,
                UF = nfe.LocalidadeRetirada?.Estado?.Sigla ?? string.Empty,
                xBairro = nfe.RetiradaBairro ?? string.Empty,
                xCpl = nfe.RetiradaComplementoLogradouro ?? string.Empty,
                xLgr = nfe.RetiradaLogradouro ?? string.Empty,
                xMun = nfe.LocalidadeRetirada?.Descricao ?? string.Empty,
                CEP = !string.IsNullOrWhiteSpace(nfe.LocalidadeRetirada?.CEP ?? string.Empty) ? nfe.LocalidadeRetirada.CEP.ObterSomenteNumeros().ToLong() : 0
            };

            return ret;
        }

        protected virtual enderDest GetEnderecoDestinatario(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var enderDest = new enderDest
            {
                xLgr = !string.IsNullOrWhiteSpace(nfe.Cliente.Endereco) ? Utilidades.String.RemoveDiacritics(nfe.Cliente.Endereco.Trim()) : string.Empty,
                nro = !string.IsNullOrWhiteSpace(nfe.Cliente.Numero) ? Utilidades.String.RemoveDiacritics(nfe.Cliente.Numero.Trim()) : string.Empty,
                xBairro = !string.IsNullOrWhiteSpace(nfe.Cliente.Bairro) ? Utilidades.String.RemoveDiacritics(nfe.Cliente.Bairro.Trim()) : string.Empty,
                cMun = nfe.Cliente.Localidade.CodigoIBGE,
                xMun = Utilidades.String.RemoveDiacritics(nfe.Cliente.Localidade.Descricao),
                UF = nfe.Cliente.Localidade.Estado.Sigla,
                CEP = Utilidades.String.OnlyNumbers(nfe.Cliente.CEP ?? "").PadLeft(8, '0'),
                cPais = nfe.Cliente.Localidade.Pais?.Codigo ?? 0,
                xPais = nfe.Cliente.Localidade.Pais?.Nome ?? "",
                fone = null,// !string.IsNullOrWhiteSpace(nfe.Cliente.Telefone1) ? Convert.ToInt64(Utilidades.String.OnlyNumbers(nfe.Cliente.Telefone1)) : 0,
                xCpl = !string.IsNullOrWhiteSpace(nfe.Cliente.Complemento) ? Utilidades.String.RemoveDiacritics(nfe.Cliente.Complemento.Trim()) : null
            };
            return enderDest;
        }
        protected virtual det GetDetalhe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item, int i, CRT crt, ModeloDocumento modelo, Repositorio.UnitOfWork unitOfWork)
        {
            var det = new det
            {
                nItem = i + 1,
                prod = GetProduto(item, i + 1, modelo, unitOfWork),
                imposto = new imposto
                {
                    vTotTrib = item.ValorImpostoIBPT
                }
            };

            if (modelo == ModeloDocumento.NFe || modelo == ModeloDocumento.NFCe)
            {
                if (item.Servico != null)
                {
                    det.imposto.ISSQN = new NFe.Classes.Informacoes.Detalhe.Tributacao.Municipal.ISSQN()
                    {
                        cListServ = item.Servico.NumeroCodigoServico,
                        cMun = item.NotaFiscal.LocalidadePrestacaoServico.CodigoIBGE,
                        cMunFG = item.NotaFiscal.LocalidadePrestacaoServico.CodigoIBGE,
                        cPais = item.NotaFiscal.LocalidadePrestacaoServico.Pais?.Codigo ?? 1058,
                        cServico = item.Servico.NumeroCodigoServico,
                        indISS = (NFe.Classes.Informacoes.Detalhe.Tributacao.Municipal.IndicadorISS)(int)item.ExigibilidadeISS + 1,
                        indIncentivo = (NFe.Classes.Informacoes.Detalhe.Tributacao.Municipal.indIncentivo)(item.IncentivoFiscal == true ? 0 : 1),
                        nProcesso = !string.IsNullOrWhiteSpace(item.ProcessoJudicial) ? item.ProcessoJudicial : null,
                        vAliq = item.AliquotaISS,
                        vBC = item.BaseISS,
                        vDeducao = item.BCDeducao > 0 ? item.BCDeducao : null,
                        vDescCond = item.DescontoCondicional > 0 ? item.DescontoCondicional : null,
                        vDescIncond = item.DescontoIncondicional > 0 ? item.DescontoIncondicional : null,
                        vISSQN = item.ValorISS,
                        vISSRet = item.RetencaoISS > 0 ? item.RetencaoISS : null,
                        vOutro = item.OutrasRetencoes > 0 ? item.OutrasRetencoes : null
                    };
                }

                if (item.Produto != null && item.CSTIPI != null)
                {
                    if (item.CSTIPI == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST53 || item.CSTIPI == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST51 || item.CSTIPI == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST52)
                    {
                        det.imposto.IPI = new IPI()
                        {
                            cEnq = !string.IsNullOrWhiteSpace(item.Produto.CodigoEnquadramentoIPI) && Int16.TryParse(Utilidades.String.OnlyNumbers(item.Produto.CodigoEnquadramentoIPI), out Int16 enqIPI) ? Convert.ToInt16(Utilidades.String.OnlyNumbers(item.Produto.CodigoEnquadramentoIPI)) : 999,
                            TipoIPI = new IPINT() { CST = RetornaCSTIPI(item.CSTIPI) }
                        };
                    }
                    else if (item.CSTIPI == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST03)
                    {
                        det.imposto.IPI = new IPI()
                        {
                            //clEnq = "9", Não tem mais na versão 4.00
                            cEnq = !string.IsNullOrWhiteSpace(item.Produto.CodigoEnquadramentoIPI) && Int16.TryParse(Utilidades.String.OnlyNumbers(item.Produto.CodigoEnquadramentoIPI), out Int16 enqIPI) ? Convert.ToInt16(Utilidades.String.OnlyNumbers(item.Produto.CodigoEnquadramentoIPI)) : 999,
                            TipoIPI = new IPINT() { CST = RetornaCSTIPI(item.CSTIPI) }
                        };
                    }
                    else
                    {
                        det.imposto.IPI = new IPI()
                        {
                            cEnq = !string.IsNullOrWhiteSpace(item.Produto.CodigoEnquadramentoIPI) && Int16.TryParse(Utilidades.String.OnlyNumbers(item.Produto.CodigoEnquadramentoIPI), out Int16 enqIPI) ? Convert.ToInt16(Utilidades.String.OnlyNumbers(item.Produto.CodigoEnquadramentoIPI)) : 999,
                            TipoIPI = new IPITrib() { CST = RetornaCSTIPI(item.CSTIPI), pIPI = item.AliquotaIPI, vBC = item.BCIPI, vIPI = item.ValorIPI }
                        };
                    }
                }
                else
                {
                    det.imposto.IPI = null;
                }

                if (item.CSTCOFINS != null)
                {
                    if (item.Produto != null)
                    {
                        if ((item.AliquotaCOFINS > 0 && item.ValorCOFINS > 0) || (item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01))
                        {
                            if (item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01)
                            {
                                det.imposto.COFINS = new COFINS()
                                {
                                    TipoCOFINS = new COFINSAliq { CST = RetornaCSTCOFINS(item.CSTCOFINS), pCOFINS = item.AliquotaCOFINS, vBC = item.BCCOFINS, vCOFINS = item.ValorCOFINS }
                                };
                            }
                            else
                            {
                                det.imposto.COFINS = new COFINS()
                                {
                                    TipoCOFINS = new COFINSOutr { CST = RetornaCSTCOFINS(item.CSTCOFINS), pCOFINS = item.AliquotaCOFINS, vBC = item.BCCOFINS, vCOFINS = item.ValorCOFINS }
                                };
                            }
                        }
                        else
                        {
                            if (item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03)
                            {
                                det.imposto.COFINS = new COFINS()
                                {
                                    TipoCOFINS = new COFINSQtde { CST = RetornaCSTCOFINS(item.CSTCOFINS), qBCProd = item.BCCOFINS, vAliqProd = item.AliquotaCOFINS, vCOFINS = item.ValorCOFINS }
                                };
                            }
                            else if (item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01 || item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02 || item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03 || item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04 || item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06 || item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07 || item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08 || item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09)
                            {
                                det.imposto.COFINS = new COFINS()
                                {
                                    TipoCOFINS = new COFINSNT { CST = RetornaCSTCOFINS(item.CSTCOFINS) }
                                };
                            }
                            else
                            {
                                det.imposto.COFINS = new COFINS()
                                {
                                    TipoCOFINS = new COFINSOutr { CST = RetornaCSTCOFINS(item.CSTCOFINS), pCOFINS = 0, qBCProd = null, vAliqProd = null, vBC = 0, vCOFINS = 0 }
                                };
                            }
                        }
                    }
                    else if (item.Servico != null)
                    {
                        if (item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49 || item.CSTCOFINS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99)
                        {
                            det.imposto.COFINS = new COFINS()
                            {
                                TipoCOFINS = new COFINSOutr { CST = RetornaCSTCOFINS(item.CSTCOFINS), pCOFINS = item.AliquotaCOFINS, vBC = item.BCCOFINS, vCOFINS = item.ValorCOFINS }
                            };
                        }
                        else
                        {
                            det.imposto.COFINS = new COFINS()
                            {
                                TipoCOFINS = new COFINSAliq { CST = RetornaCSTCOFINS(item.CSTCOFINS), pCOFINS = item.AliquotaCOFINS, vBC = item.BCCOFINS, vCOFINS = item.ValorCOFINS }
                            };
                        }
                    }
                    else
                        det.imposto.COFINS = null;
                }
                else
                {
                    det.imposto.COFINS = null;
                }

                if (item.CSTPIS != null)
                {
                    if (item.Produto != null)
                    {
                        if ((item.AliquotaPIS > 0 && item.ValorPIS > 0) || (item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01))
                        {
                            if (item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01)
                            {
                                det.imposto.PIS = new PIS()
                                {
                                    TipoPIS = new PISAliq { CST = RetornaCSTPIS(item.CSTPIS), pPIS = item.AliquotaPIS, vBC = item.BCPIS, vPIS = item.ValorPIS }
                                };
                            }
                            else
                            {
                                det.imposto.PIS = new PIS()
                                {
                                    TipoPIS = new PISOutr { CST = RetornaCSTPIS(item.CSTPIS), pPIS = item.AliquotaPIS, vBC = item.BCPIS, vPIS = item.ValorPIS }
                                };
                            }
                        }
                        else
                        {
                            if (item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03)
                            {
                                det.imposto.PIS = new PIS()
                                {
                                    TipoPIS = new PISQtde { CST = RetornaCSTPIS(item.CSTPIS), qBCProd = item.BCPIS, vAliqProd = item.AliquotaPIS, vPIS = item.ValorPIS }
                                };
                            }
                            else if (item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01 || item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02 || item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03 || item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04 || item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06 || item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07 || item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08 || item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09)
                            {
                                det.imposto.PIS = new PIS()
                                {
                                    TipoPIS = new PISNT { CST = RetornaCSTPIS(item.CSTPIS) }
                                };
                            }
                            else
                            {
                                det.imposto.PIS = new PIS()
                                {
                                    TipoPIS = new PISOutr { CST = RetornaCSTPIS(item.CSTPIS), pPIS = 0, qBCProd = null, vAliqProd = null, vBC = 0, vPIS = 0 }
                                };
                            }
                        }
                    }
                    else if (item.Servico != null)
                    {
                        if (item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49 || item.CSTPIS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99)
                        {
                            det.imposto.PIS = new PIS()
                            {
                                TipoPIS = new PISOutr { CST = RetornaCSTPIS(item.CSTPIS), pPIS = item.AliquotaPIS, vBC = item.BCPIS, vPIS = item.ValorPIS }
                            };
                        }
                        else
                        {
                            det.imposto.PIS = new PIS()
                            {
                                TipoPIS = new PISAliq { CST = RetornaCSTPIS(item.CSTPIS), pPIS = item.AliquotaPIS, vBC = item.BCPIS, vPIS = item.ValorPIS }
                            };
                        }
                    }
                    else
                        det.imposto.PIS = null;
                }
                else
                {
                    det.imposto.PIS = null;
                }

                if (item.Produto != null && item.CSTICMS != null)
                {
                    det.imposto.ICMS = new ICMS()
                    {
                        TipoICMS = RetornaImpostoICMS(item.CSTICMS, item)
                    };
                }
                else
                {
                    det.imposto.ICMS = null;
                }

                if (item.Produto != null && item.PercentualPartilha > 0 && item.AliquotaICMSDestino > 0 && item.AliquotaICMSInterno > 0)
                {
                    det.imposto.ICMSUFDest = new ICMSUFDest()
                    {
                        vBCFCPUFDest = item.BCFCPDestino > 0 ? item.BCFCPDestino : null,
                        pFCPUFDest = item.AliquotaFCP > 0 ? item.AliquotaFCP : null,
                        pICMSInter = item.AliquotaICMSInterno,
                        pICMSInterPart = item.PercentualPartilha,
                        pICMSUFDest = item.AliquotaICMSDestino,
                        vBCUFDest = item.BCICMSDestino,
                        vFCPUFDest = item.ValorFCP > 0 ? item.ValorFCP : null,
                        vICMSUFDest = item.ValorICMSDestino,
                        vICMSUFRemet = item.ValorICMSRemetente
                    };
                }
                else
                {
                    det.imposto.ICMSUFDest = null;
                }

                if (item.Produto != null && !string.IsNullOrWhiteSpace(item.NumeroDocImportacao))
                {
                    det.imposto.II = new II()
                    {
                        vBC = item.BaseII,
                        vDespAdu = item.ValorDespesaII,
                        vII = item.ValorII,
                        vIOF = item.ValorIOFII
                    };
                }
                else
                {
                    det.imposto.II = null;
                }

                if (item.ValorIPIDevolvido > 0 || item.PercentualIPIDevolvido > 0)
                {
                    det.impostoDevol = new impostoDevol()
                    {
                        IPI = new IPIDevolvido() { vIPIDevol = item.ValorIPIDevolvido },
                        pDevol = item.PercentualIPIDevolvido > 0 ? item.PercentualIPIDevolvido : 100
                    };
                }

                if (!string.IsNullOrWhiteSpace(item.InformacoesAdicionais))
                {
                    det.infAdProd = item.InformacoesAdicionais.Trim();
                }
            }

            return det;
        }
        protected virtual ICMSBasico RetornaImpostoICMS(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? cst, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item)
        {
            MotivoDesoneracaoIcms? motivoDesoneracao = null;
            if (item.MotivoDesoneracao > 0)
                motivoDesoneracao = (MotivoDesoneracaoIcms)item.MotivoDesoneracao;
            OrigemMercadoria origemMercadoria;
            if (item.OrigemMercadoria != null)
                origemMercadoria = (OrigemMercadoria)item.OrigemMercadoria;
            else if (item.Produto.OrigemMercadoria != null)
                origemMercadoria = (OrigemMercadoria)item.Produto.OrigemMercadoria;
            else
                origemMercadoria = OrigemMercadoria.OmNacional;

            decimal? pMVASTItem = null;
            DeterminacaoBaseIcmsSt determinacaoBaseIcmsSt = DeterminacaoBaseIcmsSt.DbisPrecoTabelado;
            if (item.MVAICMSST > 0)
            {
                pMVASTItem = item.MVAICMSST;
                determinacaoBaseIcmsSt = DeterminacaoBaseIcmsSt.DbisMargemValorAgregado;
            }

            bool itemComICMSEfetivo = item.BCICMSEfetivo > 0 || item.ReducaoBCICMSEfetivo > 0 || item.AliquotaICMSEfetivo > 0 || item.ValorICMSEfetivo > 0;

            if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101)
                return new ICMSSN101 { CSOSN = Csosnicms.Csosn101, orig = origemMercadoria, pCredSN = item.AliquotaICMSSimples, vCredICMSSN = item.ValorICMSSimples };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102)
                return new ICMSSN102 { CSOSN = Csosnicms.Csosn102, orig = origemMercadoria };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103)
                return new ICMSSN102 { CSOSN = Csosnicms.Csosn103, orig = origemMercadoria };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300)
                return new ICMSSN102 { CSOSN = Csosnicms.Csosn300, orig = origemMercadoria };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400)
                return new ICMSSN102 { CSOSN = Csosnicms.Csosn400, orig = origemMercadoria };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201)
                return new ICMSSN201
                {
                    CSOSN = Csosnicms.Csosn201,
                    orig = origemMercadoria,
                    pCredSN = item.AliquotaICMSSimples,
                    vCredICMSSN = item.ValorICMSSimples,
                    modBCST = determinacaoBaseIcmsSt,
                    pICMSST = item.AliquotaICMSST,
                    pMVAST = pMVASTItem,
                    pRedBCST = item.ReducaoBCICMSST,
                    vBCST = item.BCICMSST,
                    vICMSST = item.ValorICMSST,
                    vBCFCPST = item.BCFCPICMSST > 0 ? item.BCFCPICMSST : null,
                    pFCPST = item.PercentualFCPICMSST > 0 ? item.PercentualFCPICMSST : null,
                    vFCPST = item.ValorFCPICMSST > 0 ? item.ValorFCPICMSST : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202 || cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203)
                return new ICMSSN202
                {
                    CSOSN = cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203 ? Csosnicms.Csosn203 : Csosnicms.Csosn202,
                    orig = origemMercadoria,
                    modBCST = determinacaoBaseIcmsSt,
                    pICMSST = item.AliquotaICMSST,
                    pMVAST = pMVASTItem,
                    pRedBCST = item.ReducaoBCICMSST,
                    vBCST = item.BCICMSST,
                    vICMSST = item.ValorICMSST,
                    vBCFCPST = item.BCFCPICMSST > 0 ? item.BCFCPICMSST : null,
                    pFCPST = item.PercentualFCPICMSST > 0 ? item.PercentualFCPICMSST : null,
                    vFCPST = item.ValorFCPICMSST > 0 ? item.ValorFCPICMSST : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500)
                return new ICMSSN500
                {
                    CSOSN = Csosnicms.Csosn500,
                    orig = origemMercadoria,
                    vBCSTRet = item.BCICMSSTRetido ?? 0,
                    pST = item.AliquotaICMSSTRetido ?? 0,
                    vICMSSubstituto = item.ValorICMSSTSubstituto ?? 0,
                    vICMSSTRet = item.ValorICMSSTRetido ?? 0,

                    pRedBCEfet = itemComICMSEfetivo ? (decimal?)(item.ReducaoBCICMSEfetivo ?? 0) : null,
                    vBCEfet = itemComICMSEfetivo ? (decimal?)(item.BCICMSEfetivo ?? 0) : null,
                    pICMSEfet = itemComICMSEfetivo ? (decimal?)(item.AliquotaICMSEfetivo ?? 0) : null,
                    vICMSEfet = itemComICMSEfetivo ? (decimal?)(item.ValorICMSEfetivo ?? 0) : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900)
                return new ICMSSN900
                {
                    CSOSN = Csosnicms.Csosn900,
                    orig = origemMercadoria,
                    modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                    modBCST = determinacaoBaseIcmsSt,
                    pCredSN = item.AliquotaICMSSimples,
                    pICMS = item.AliquotaICMS,
                    pICMSST = item.AliquotaICMSST,
                    pMVAST = pMVASTItem,
                    pRedBC = item.ReducaoBCICMS,
                    pRedBCST = item.ReducaoBCICMSST,
                    vBC = item.BCICMS,
                    vBCST = item.BCICMSST,
                    vCredICMSSN = item.ValorICMSSimples,
                    vICMS = item.ValorICMS,
                    vICMSST = item.ValorICMSST,
                    vBCFCPST = item.BCFCPICMSST > 0 ? item.BCFCPICMSST : null,
                    pFCPST = item.PercentualFCPICMSST > 0 ? item.PercentualFCPICMSST : null,
                    vFCPST = item.ValorFCPICMSST > 0 ? item.ValorFCPICMSST : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00)
                return new ICMS00
                {
                    CST = Csticms.Cst00,
                    orig = origemMercadoria,
                    modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                    pICMS = item.AliquotaICMS,
                    vBC = item.BCICMS,
                    vICMS = item.ValorICMS,
                    pFCP = item.PercentualFCPICMS > 0 ? item.PercentualFCPICMS : null,
                    vFCP = item.ValorFCPICMS > 0 ? item.ValorFCPICMS : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10)
                return new ICMS10
                {
                    CST = Csticms.Cst10,
                    orig = origemMercadoria,
                    modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                    pICMS = item.AliquotaICMS,
                    vBC = item.BCICMS,
                    vICMS = item.ValorICMS,
                    modBCST = determinacaoBaseIcmsSt,
                    pICMSST = item.AliquotaICMSST,
                    pMVAST = pMVASTItem,
                    pRedBCST = item.ReducaoBCICMSST,
                    vBCST = item.BCICMSST,
                    vICMSST = item.ValorICMSST,
                    vBCFCP = item.BCFCPICMS > 0 ? item.BCFCPICMS : null,
                    pFCP = item.PercentualFCPICMS > 0 ? item.PercentualFCPICMS : null,
                    vFCP = item.ValorFCPICMS > 0 ? item.ValorFCPICMS : null,
                    vBCFCPST = item.BCFCPICMSST > 0 ? item.BCFCPICMSST : null,
                    pFCPST = item.PercentualFCPICMSST > 0 ? item.PercentualFCPICMSST : null,
                    vFCPST = item.ValorFCPICMSST > 0 ? item.ValorFCPICMSST : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20)
                return new ICMS20
                {
                    CST = Csticms.Cst20,
                    orig = origemMercadoria,
                    modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                    pICMS = item.AliquotaICMS,
                    vBC = item.BCICMS,
                    vICMS = item.ValorICMS,
                    vICMSDeson = item.ValorICMSDesonerado > 0 ? item.ValorICMSDesonerado : null,
                    pRedBC = item.ReducaoBCICMS,
                    motDesICMS = motivoDesoneracao,
                    vBCFCP = item.BCFCPICMS > 0 ? item.BCFCPICMS : null,
                    pFCP = item.PercentualFCPICMS > 0 ? item.PercentualFCPICMS : null,
                    vFCP = item.ValorFCPICMS > 0 ? item.ValorFCPICMS : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30)
                return new ICMS30
                {
                    CST = Csticms.Cst30,
                    orig = origemMercadoria,
                    modBCST = determinacaoBaseIcmsSt,
                    pICMSST = item.AliquotaICMSST,
                    pMVAST = pMVASTItem,
                    pRedBCST = item.ReducaoBCICMSST,
                    vBCST = item.BCICMSST,
                    vICMSST = item.ValorICMSST,
                    motDesICMS = motivoDesoneracao,
                    vICMSDeson = item.ValorICMSDesonerado > 0 ? item.ValorICMSDesonerado : null,
                    vBCFCPST = item.BCFCPICMSST > 0 ? item.BCFCPICMSST : null,
                    pFCPST = item.PercentualFCPICMSST > 0 ? item.PercentualFCPICMSST : null,
                    vFCPST = item.ValorFCPICMSST > 0 ? item.ValorFCPICMSST : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40)
                return new ICMS40 { CST = Csticms.Cst40, orig = origemMercadoria, motDesICMS = motivoDesoneracao, vICMSDeson = item.ValorICMSDesonerado > 0 ? item.ValorICMSDesonerado : null };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41)
                return new ICMS40 { CST = Csticms.Cst41, orig = origemMercadoria, motDesICMS = motivoDesoneracao, vICMSDeson = item.ValorICMSDesonerado > 0 ? item.ValorICMSDesonerado : null };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50)
                return new ICMS40 { CST = Csticms.Cst50, orig = origemMercadoria, motDesICMS = motivoDesoneracao, vICMSDeson = item.ValorICMSDesonerado > 0 ? item.ValorICMSDesonerado : null };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51)
                return new ICMS51
                {
                    CST = Csticms.Cst51,
                    orig = origemMercadoria,
                    modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                    pICMS = item.AliquotaICMS,
                    vBC = item.BCICMS,
                    vICMS = item.ValorICMS,
                    pRedBC = item.ReducaoBCICMS,
                    pDif = item.AliquotaICMSOperacao > 0 ? item.AliquotaICMSOperacao : null,
                    vICMSDif = item.ValorICMSDiferido > 0 ? item.ValorICMSDiferido : null,
                    vICMSOp = item.ValorICMSOperacao > 0 ? item.ValorICMSOperacao : null,
                    vBCFCP = item.BCFCPICMS > 0 ? item.BCFCPICMS : null,
                    pFCP = item.PercentualFCPICMS > 0 ? item.PercentualFCPICMS : null,
                    vFCP = item.ValorFCPICMS > 0 ? item.ValorFCPICMS : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60 && (item.BCICMSSTDestino > 0 || item.ValorICMSSTDestino > 0))
                return new ICMSST
                {
                    CST = Csticms.Cst60,
                    orig = origemMercadoria,
                    vBCSTRet = item.BCICMSSTRetido ?? 0,
                    pST = item.AliquotaICMSSTRetido ?? 0,
                    vICMSSubstituto = item.ValorICMSSTSubstituto ?? 0,
                    vICMSSTRet = item.ValorICMSSTRetido ?? 0,
                    vBCSTDest = item.BCICMSSTDestino ?? 0,
                    vICMSSTDest = item.ValorICMSSTDestino ?? 0,

                    pRedBCEfet = itemComICMSEfetivo ? (decimal?)(item.ReducaoBCICMSEfetivo ?? 0) : null,
                    vBCEfet = itemComICMSEfetivo ? (decimal?)(item.BCICMSEfetivo ?? 0) : null,
                    pICMSEfet = itemComICMSEfetivo ? (decimal?)(item.AliquotaICMSEfetivo ?? 0) : null,
                    vICMSEfet = itemComICMSEfetivo ? (decimal?)(item.ValorICMSEfetivo ?? 0) : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST61)
                return new ICMS61
                {
                    CST = Csticms.Cst61,
                    orig = origemMercadoria,
                    qBCMonoRet = item.ValorTotal,
                    adRemICMSRet = item.AliquotaRemICMSRet,
                    vICMSMonoRet = item.ValorICMSMonoRet
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60)
                return new ICMS60
                {
                    CST = Csticms.Cst60,
                    orig = origemMercadoria,
                    vBCSTRet = item.BCICMSSTRetido ?? 0,
                    pST = item.AliquotaICMSSTRetido ?? 0,
                    vICMSSubstituto = item.ValorICMSSTSubstituto ?? 0,
                    vICMSSTRet = item.ValorICMSSTRetido ?? 0,

                    pRedBCEfet = itemComICMSEfetivo ? (decimal?)(item.ReducaoBCICMSEfetivo ?? 0) : null,
                    vBCEfet = itemComICMSEfetivo ? (decimal?)(item.BCICMSEfetivo ?? 0) : null,
                    pICMSEfet = itemComICMSEfetivo ? (decimal?)(item.AliquotaICMSEfetivo ?? 0) : null,
                    vICMSEfet = itemComICMSEfetivo ? (decimal?)(item.ValorICMSEfetivo ?? 0) : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70)
                return new ICMS70
                {
                    CST = Csticms.Cst70,
                    orig = origemMercadoria,
                    modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                    pICMS = item.AliquotaICMS,
                    vBC = item.BCICMS,
                    vICMS = item.ValorICMS,
                    modBCST = determinacaoBaseIcmsSt,
                    pICMSST = item.AliquotaICMSST,
                    pMVAST = pMVASTItem,
                    pRedBCST = item.ReducaoBCICMSST,
                    vBCST = item.BCICMSST,
                    vICMSST = item.ValorICMSST,
                    vICMSDeson = item.ValorICMSDesonerado > 0 ? item.ValorICMSDesonerado : null,
                    pRedBC = item.ReducaoBCICMS,
                    motDesICMS = motivoDesoneracao,
                    vBCFCP = item.BCFCPICMS > 0 ? item.BCFCPICMS : null,
                    pFCP = item.PercentualFCPICMS > 0 ? item.PercentualFCPICMS : null,
                    vFCP = item.ValorFCPICMS > 0 ? item.ValorFCPICMS : null,
                    vBCFCPST = item.BCFCPICMSST > 0 ? item.BCFCPICMSST : null,
                    pFCPST = item.PercentualFCPICMSST > 0 ? item.PercentualFCPICMSST : null,
                    vFCPST = item.ValorFCPICMSST > 0 ? item.ValorFCPICMSST : null
                };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90)
                return new ICMS90
                {
                    CST = Csticms.Cst90,
                    orig = origemMercadoria,
                    modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                    pICMS = item.AliquotaICMS,
                    vBC = item.BCICMS,
                    vICMS = item.ValorICMS,
                    modBCST = determinacaoBaseIcmsSt,
                    pICMSST = item.AliquotaICMSST,
                    pMVAST = pMVASTItem,
                    pRedBCST = item.ReducaoBCICMSST,
                    vBCST = item.BCICMSST,
                    vICMSST = item.ValorICMSST,
                    vICMSDeson = item.ValorICMSDesonerado > 0 ? item.ValorICMSDesonerado : null,
                    pRedBC = item.ReducaoBCICMS,
                    motDesICMS = motivoDesoneracao,
                    vBCFCP = item.BCFCPICMS > 0 ? item.BCFCPICMS : null,
                    pFCP = item.PercentualFCPICMS > 0 ? item.PercentualFCPICMS : null,
                    vFCP = item.ValorFCPICMS > 0 ? item.ValorFCPICMS : null,
                    vBCFCPST = item.BCFCPICMSST > 0 ? item.BCFCPICMSST : null,
                    pFCPST = item.PercentualFCPICMSST > 0 ? item.PercentualFCPICMSST : null,
                    vFCPST = item.ValorFCPICMSST > 0 ? item.ValorFCPICMSST : null
                };
            else
                return null;
        }
        protected virtual CSTPIS RetornaCSTPIS(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? cst)
        {
            if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01)
                return CSTPIS.pis01;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02)
                return CSTPIS.pis02;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03)
                return CSTPIS.pis03;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04)
                return CSTPIS.pis04;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05)
                return CSTPIS.pis05;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06)
                return CSTPIS.pis06;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07)
                return CSTPIS.pis07;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08)
                return CSTPIS.pis08;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09)
                return CSTPIS.pis09;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49)
                return CSTPIS.pis49;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50)
                return CSTPIS.pis50;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51)
                return CSTPIS.pis51;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52)
                return CSTPIS.pis52;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53)
                return CSTPIS.pis53;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54)
                return CSTPIS.pis54;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55)
                return CSTPIS.pis55;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56)
                return CSTPIS.pis56;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60)
                return CSTPIS.pis60;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61)
                return CSTPIS.pis61;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62)
                return CSTPIS.pis62;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63)
                return CSTPIS.pis63;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64)
                return CSTPIS.pis64;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65)
                return CSTPIS.pis65;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66)
                return CSTPIS.pis66;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67)
                return CSTPIS.pis67;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70)
                return CSTPIS.pis70;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71)
                return CSTPIS.pis71;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72)
                return CSTPIS.pis72;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73)
                return CSTPIS.pis73;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74)
                return CSTPIS.pis74;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75)
                return CSTPIS.pis75;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98)
                return CSTPIS.pis98;
            else
                return CSTPIS.pis99;
        }
        protected virtual CSTCOFINS RetornaCSTCOFINS(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? cst)
        {
            if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01)
                return CSTCOFINS.cofins01;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02)
                return CSTCOFINS.cofins02;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03)
                return CSTCOFINS.cofins03;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04)
                return CSTCOFINS.cofins04;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05)
                return CSTCOFINS.cofins05;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06)
                return CSTCOFINS.cofins06;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07)
                return CSTCOFINS.cofins07;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08)
                return CSTCOFINS.cofins08;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09)
                return CSTCOFINS.cofins09;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49)
                return CSTCOFINS.cofins49;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50)
                return CSTCOFINS.cofins50;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51)
                return CSTCOFINS.cofins51;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52)
                return CSTCOFINS.cofins52;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53)
                return CSTCOFINS.cofins53;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54)
                return CSTCOFINS.cofins54;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55)
                return CSTCOFINS.cofins55;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56)
                return CSTCOFINS.cofins56;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60)
                return CSTCOFINS.cofins60;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61)
                return CSTCOFINS.cofins61;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62)
                return CSTCOFINS.cofins62;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63)
                return CSTCOFINS.cofins63;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64)
                return CSTCOFINS.cofins64;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65)
                return CSTCOFINS.cofins65;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66)
                return CSTCOFINS.cofins66;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67)
                return CSTCOFINS.cofins67;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70)
                return CSTCOFINS.cofins70;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71)
                return CSTCOFINS.cofins71;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72)
                return CSTCOFINS.cofins72;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73)
                return CSTCOFINS.cofins73;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74)
                return CSTCOFINS.cofins74;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75)
                return CSTCOFINS.cofins75;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98)
                return CSTCOFINS.cofins98;
            else
                return CSTCOFINS.cofins99;
        }
        protected virtual CSTIPI RetornaCSTIPI(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI? cst)
        {
            if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST00)
                return CSTIPI.ipi00;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST01)
                return CSTIPI.ipi01;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST02)
                return CSTIPI.ipi02;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST03)
                return CSTIPI.ipi03;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST04)
                return CSTIPI.ipi04;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST05)
                return CSTIPI.ipi05;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST49)
                return CSTIPI.ipi49;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST50)
                return CSTIPI.ipi50;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST51)
                return CSTIPI.ipi51;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST52)
                return CSTIPI.ipi52;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST53)
                return CSTIPI.ipi53;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST54)
                return CSTIPI.ipi54;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST55)
                return CSTIPI.ipi55;
            else
                return CSTIPI.ipi99;
        }
        protected virtual prod GetProduto(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item, int i, ModeloDocumento modelo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutosLotes repLotes = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutosLotes(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes> listaLote = repLotes.BuscarPorNota(item.NotaFiscal.Codigo);
            if (listaLote?.Count > 0)
                listaLote = listaLote.Where(o => o.NotaFiscalProdutos.Codigo == item.Codigo).ToList();

            prod p = new prod
            {
                cProd = !string.IsNullOrWhiteSpace(item.CodigoItem) ? Utilidades.String.RemoveDiacritics(item.CodigoItem).Trim().TrimEnd().TrimStart() : item.Produto != null ? !string.IsNullOrWhiteSpace(item.Produto.CodigoProduto) ? item.Produto.CodigoProduto : item.Produto.Codigo.ToString() : item.Servico.Codigo.ToString(),
                cEAN = item.Produto != null && !string.IsNullOrWhiteSpace(item.Produto.CodigoBarrasEAN) && (item.Produto.CodigoBarrasEAN.Length == 8 || item.Produto.CodigoBarrasEAN.Length == 12 || item.Produto.CodigoBarrasEAN.Length == 13 || item.Produto.CodigoBarrasEAN.Length == 14) ? item.Produto.CodigoBarrasEAN : "SEM GTIN",
                xProd = item.NotaFiscal.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && modelo == ModeloDocumento.NFCe ? "NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL" :
                    !string.IsNullOrWhiteSpace(item.DescricaoItem) ? Utilidades.String.RemoveDiacritics(item.DescricaoItem).Trim().TrimEnd().TrimStart() :
                    item.Produto != null ? !string.IsNullOrWhiteSpace(item.Produto.DescricaoNotaFiscal.Trim()) ? Utilidades.String.RemoveDiacritics(item.Produto.DescricaoNotaFiscal.Trim()) :
                    Utilidades.String.RemoveDiacritics(item.Produto.Descricao.Trim()) : !string.IsNullOrWhiteSpace(item.Servico.DescricaoNFE.Trim()) ?
                    Utilidades.String.RemoveDiacritics(item.Servico.DescricaoNFE.Trim()) : Utilidades.String.RemoveDiacritics(item.Servico.Descricao.Trim()),
                NCM = item.Produto != null ? item.Produto.CodigoNCM : "00",
                CFOP = item.CFOP != null ? item.CFOP.CodigoCFOP : 5101,
                uCom = item.Produto != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla(item.Produto.UnidadeDeMedida) : "SERV",
                qCom = item.Quantidade,
                vUnCom = item.ValorUnitario,
                vProd = item.ValorTotal,
                vDesc = item.ValorDesconto,
                cEANTrib = !string.IsNullOrWhiteSpace(item.CodigoEANTributavel) ? item.CodigoEANTributavel.Trim() : item.Produto != null && !string.IsNullOrWhiteSpace(item.Produto.CodigoBarrasEAN) && (item.Produto.CodigoBarrasEAN.Length == 8 || item.Produto.CodigoBarrasEAN.Length == 12 || item.Produto.CodigoBarrasEAN.Length == 13 || item.Produto.CodigoBarrasEAN.Length == 14) ? item.Produto.CodigoBarrasEAN : "SEM GTIN",
                uTrib = item.UnidadeDeMedidaTributavel != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla(item.UnidadeDeMedidaTributavel) : item.Produto != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla(item.Produto.UnidadeDeMedida) : "SERV",
                qTrib = item.QuantidadeTributavel > 0 ? item.QuantidadeTributavel.Value : item.Quantidade,
                vUnTrib = item.ValorUnitarioTributavel > 0 ? item.ValorUnitarioTributavel.Value : item.QuantidadeTributavel > 0 ? (item.ValorTotal / item.QuantidadeTributavel.Value) : item.ValorUnitario,
                indTot = IndicadorTotal.ValorDoItemCompoeTotalNF,
                CEST = item.Produto != null && !string.IsNullOrWhiteSpace(item.Produto.CodigoCEST) ? item.Produto.CodigoCEST : null,
                indEscala = item.IndicadorEscalaRelevante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.ProduzidoEscalaNaoRelevante ? NFe.Classes.Informacoes.Detalhe.indEscala.N : item.IndicadorEscalaRelevante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorEscalaRelevante.ProduzidoEscalaRelevante ? NFe.Classes.Informacoes.Detalhe.indEscala.S : null,
                cBenef = !string.IsNullOrWhiteSpace(item.CodigoBeneficioFiscal) ? item.CodigoBeneficioFiscal.Trim() : null,
                CNPJFab = !string.IsNullOrWhiteSpace(item.CNPJFabricante) ? Utilidades.String.OnlyNumbers(item.CNPJFabricante).Trim() : null,
                EXTIPI = null,
                nFCI = !string.IsNullOrWhiteSpace(item.CodigoNFCI) ? Utilidades.String.OnlyNumbers(item.CodigoNFCI).Trim() : null,
                nItemPed = !string.IsNullOrWhiteSpace(item.NumeroItemOrdemCompra) && Int32.TryParse(Utilidades.String.OnlyNumbers(item.NumeroItemOrdemCompra), out Int32 numeroItemCompra) ? Convert.ToInt32(Utilidades.String.OnlyNumbers(item.NumeroItemOrdemCompra)) : 0,
                detExport = InformarExportacaoItem(item),
                DI = !string.IsNullOrWhiteSpace(item.LocalDesembaraco) && item.DataRegistroImportacao.HasValue ? InformarDeclaracaoImportacao(item) : null,
                nRECOPI = null,
                NVE = null,
                rastro = listaLote?.Any() ?? false ? GetRastreabilidade(listaLote) : null,
                ProdutoEspecifico = GetProdutoEspecifico(item, listaLote?.Any() ?? false),
                vFrete = item.ValorFrete,
                vOutro = item.ValorOutrasDespesas,
                vSeg = item.ValorSeguro,
                xPed = !string.IsNullOrWhiteSpace(item.NumeroOrdemCompra) ? item.NumeroOrdemCompra : null
            };
            return p;
        }
        protected virtual List<NFe.Classes.Informacoes.Detalhe.DeclaracaoImportacao.DI> InformarDeclaracaoImportacao(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item)
        {
            var listaDI = new List<NFe.Classes.Informacoes.Detalhe.DeclaracaoImportacao.DI>();
            var di = new NFe.Classes.Informacoes.Detalhe.DeclaracaoImportacao.DI();

            var vAdi = new List<NFe.Classes.Informacoes.Detalhe.DeclaracaoImportacao.adi> { GetAdi() };
            di.adi = vAdi;
            di.cExportador = "1";//item.NotaFiscal.Empresa.CNPJ_SemFormato;
            di.CNPJ = Utilidades.String.OnlyNumbers(item.CNPJAdquirente); //item.NotaFiscal.Empresa.CNPJ_SemFormato;
            di.dDesemb = item.DataDesembaraco.Value;
            di.dDI = item.DataRegistroImportacao.Value;
            di.nDI = item.NumeroDocImportacao;
            di.tpIntermedio = TipoIntermediacao.ContaPropria;
            di.tpViaTransp = (TipoTransporteInternacional)item.ViaTransporteII;
            di.UFDesemb = item.UFDesembaraco;
            di.UFTerceiro = item.NotaFiscal.Empresa.Localidade.Estado.Sigla;
            di.vAFRMM = 0;
            di.xLocDesemb = item.LocalDesembaraco;

            listaDI.Add(di);

            return listaDI;
        }
        protected virtual NFe.Classes.Informacoes.Detalhe.DeclaracaoImportacao.adi GetAdi()
        {
            var v = new NFe.Classes.Informacoes.Detalhe.DeclaracaoImportacao.adi
            {
                cFabricante = "1",
                nAdicao = 1,
                nDraw = null,
                nSeqAdic = 1,
                vDescDI = 1
            };

            return v;
        }
        protected virtual ICMSBasico InformarICMS(Csticms CST, VersaoServico versao)
        {
            var icms20 = new ICMS20
            {
                orig = OrigemMercadoria.OmNacional,
                CST = Csticms.Cst20,
                modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                vBC = 1,
                pICMS = 17,
                vICMS = 0.17m,
                motDesICMS = MotivoDesoneracaoIcms.MdiTaxi
            };
            if (versao >= VersaoServico.Versao310)
                icms20.vICMSDeson = 0.10m; //V3.00 ou maior Somente

            switch (CST)
            {
                case Csticms.Cst00:
                    return new ICMS00
                    {
                        CST = Csticms.Cst00,
                        modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                        orig = OrigemMercadoria.OmNacional,
                        pICMS = 17,
                        vBC = 1,
                        vICMS = 0.17m
                    };
                case Csticms.Cst20:
                    return icms20;
                    //Outros casos aqui
            }

            return new ICMS10();
        }
        protected virtual ICMSBasico InformarCSOSN(Csosnicms CST)
        {
            switch (CST)
            {
                case Csosnicms.Csosn101:
                    return new ICMSSN101
                    {
                        CSOSN = Csosnicms.Csosn101,
                        orig = OrigemMercadoria.OmNacional
                    };
                case Csosnicms.Csosn102:
                    return new ICMSSN102
                    {
                        CSOSN = Csosnicms.Csosn102,
                        orig = OrigemMercadoria.OmNacional
                    };
                //Outros casos aqui
                default:
                    return new ICMSSN201();
            }
        }
        protected virtual total GetTotal(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, VersaoServico versao, List<det> produtos)
        {
            var icmsTot = new ICMSTot
            {
                vProd = produtos.Sum(p => p.prod.vProd),
                vNF = nfe.ValorTotalNota,//produtos.Sum(p => p.prod.vProd) - produtos.Sum(p => p.prod.vDesc ?? 0) + produtos.Sum(p => p.prod.vFrete ?? 0) + produtos.Sum(p => p.prod.vSeg ?? 0) + produtos.Sum(p => p.prod.vOutro ?? 0),
                vDesc = nfe.ValorDesconto,//produtos.Sum(p => p.prod.vDesc ?? 0),
                vOutro = nfe.ValorOutrasDespesas,// produtos.Sum(p => p.prod.vOutro ?? 0),
                vTotTrib = nfe.ValorImpostoIBPT,//produtos.Sum(p => p.imposto.vTotTrib ?? 0),
                vSeg = nfe.ValorSeguro,//produtos.Sum(p => p.prod.vSeg ?? 0),
                vFrete = nfe.ValorFrete,// produtos.Sum(p => p.prod.vFrete ?? 0)
                vICMSUFDest = nfe.ValorICMSDestino,
                vICMSUFRemet = nfe.ValorICMSRemetente,
                vFCPUFDest = nfe.ValorFCP
            };
            var issqnTot = new ISSQNtot
            {
                cRegTrib = RegTribISSQN.RTISSSociedadeProfissionais,
                dCompet = nfe.DataPrestacaoServico.Value.ToString("yyyy-MM-dd"),
                vServ = 0,
                vBC = 0,
                vCOFINS = 0,
                vDeducao = 0,
                vDescCond = 0,
                vDescIncond = 0,
                vISS = 0,
                vISSRet = 0,
                vOutro = 0,
                vPIS = 0
            };

            var comISS = false;

            if (versao >= VersaoServico.Versao310)
                icmsTot.vICMSDeson = 0;
            if (versao == VersaoServico.Versao400)
            {
                icmsTot.vFCP = 0;
                icmsTot.vFCPST = 0;
                icmsTot.vFCPSTRet = 0;
                icmsTot.vIPIDevol = 0;
                icmsTot.qBCMonoRet = 0;
                icmsTot.vICMSMonoRet = 0;
            }

            foreach (var produto in produtos)
            {
                if (produto.imposto.IPI != null && produto.imposto.IPI.TipoIPI.GetType() == typeof(IPITrib))
                    icmsTot.vIPI = icmsTot.vIPI + (((IPITrib)produto.imposto.IPI.TipoIPI).vIPI ?? 0);
                else
                    icmsTot.vIPI = icmsTot.vIPI + 0;

                if (produto.imposto.PIS != null && produto.imposto.PIS.TipoPIS.GetType() == typeof(PISOutr) && produto.imposto.ISSQN == null)
                    icmsTot.vPIS = icmsTot.vPIS + (((PISOutr)produto.imposto.PIS.TipoPIS).vPIS ?? 0);
                else
                    icmsTot.vPIS = icmsTot.vPIS + 0;

                if (produto.imposto.COFINS != null && produto.imposto.COFINS.TipoCOFINS.GetType() == typeof(COFINSOutr) && produto.imposto.ISSQN == null)
                    icmsTot.vCOFINS = icmsTot.vCOFINS + (((COFINSOutr)produto.imposto.COFINS.TipoCOFINS).vCOFINS ?? 0);
                else
                    icmsTot.vCOFINS = icmsTot.vCOFINS + 0;

                if (produto.imposto.PIS != null && produto.imposto.PIS.TipoPIS.GetType() == typeof(PISAliq) && produto.imposto.ISSQN == null)
                    icmsTot.vPIS = icmsTot.vPIS + ((PISAliq)produto.imposto.PIS.TipoPIS).vPIS;
                else
                    icmsTot.vPIS = icmsTot.vPIS + 0;

                if (produto.imposto.COFINS != null && produto.imposto.COFINS.TipoCOFINS.GetType() == typeof(COFINSAliq) && produto.imposto.ISSQN == null)
                    icmsTot.vCOFINS = icmsTot.vCOFINS + ((COFINSAliq)produto.imposto.COFINS.TipoCOFINS).vCOFINS;
                else
                    icmsTot.vCOFINS = icmsTot.vCOFINS + 0;

                if (produto.imposto.II != null && produto.imposto.II.GetType() == typeof(II))
                    icmsTot.vII = icmsTot.vII + produto.imposto.II.vII;
                else
                    icmsTot.vII = icmsTot.vII + 0;

                if (produto.imposto.ICMS != null)
                {
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS00))
                    {
                        icmsTot.vBC = icmsTot.vBC + ((ICMS00)produto.imposto.ICMS.TipoICMS).vBC;
                        icmsTot.vICMS = icmsTot.vICMS + ((ICMS00)produto.imposto.ICMS.TipoICMS).vICMS;
                        icmsTot.vFCP = icmsTot.vFCP + (((ICMS00)produto.imposto.ICMS.TipoICMS).vFCP ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS10))
                    {
                        icmsTot.vBC = icmsTot.vBC + ((ICMS10)produto.imposto.ICMS.TipoICMS).vBC;
                        icmsTot.vICMS = icmsTot.vICMS + ((ICMS10)produto.imposto.ICMS.TipoICMS).vICMS;
                        icmsTot.vBCST = icmsTot.vBCST + ((ICMS10)produto.imposto.ICMS.TipoICMS).vBCST;
                        icmsTot.vST = icmsTot.vST + ((ICMS10)produto.imposto.ICMS.TipoICMS).vICMSST;
                        icmsTot.vFCP = icmsTot.vFCP + (((ICMS10)produto.imposto.ICMS.TipoICMS).vFCP ?? 0);
                        icmsTot.vFCPST = icmsTot.vFCPST + (((ICMS10)produto.imposto.ICMS.TipoICMS).vFCPST ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS20))
                    {
                        icmsTot.vICMSDeson = icmsTot.vICMSDeson + (((ICMS20)produto.imposto.ICMS.TipoICMS).vICMSDeson ?? 0);
                        icmsTot.vBC = icmsTot.vBC + ((ICMS20)produto.imposto.ICMS.TipoICMS).vBC;
                        icmsTot.vICMS = icmsTot.vICMS + ((ICMS20)produto.imposto.ICMS.TipoICMS).vICMS;
                        icmsTot.vFCP = icmsTot.vFCP + (((ICMS20)produto.imposto.ICMS.TipoICMS).vFCP ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS30))
                    {
                        icmsTot.vICMSDeson = icmsTot.vICMSDeson + (((ICMS30)produto.imposto.ICMS.TipoICMS).vICMSDeson ?? 0);
                        icmsTot.vBCST = icmsTot.vBCST + ((ICMS30)produto.imposto.ICMS.TipoICMS).vBCST;
                        icmsTot.vST = icmsTot.vST + ((ICMS30)produto.imposto.ICMS.TipoICMS).vICMSST;
                        icmsTot.vFCPST = icmsTot.vFCPST + (((ICMS30)produto.imposto.ICMS.TipoICMS).vFCPST ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS40))
                    {
                        icmsTot.vICMSDeson = icmsTot.vICMSDeson + (((ICMS40)produto.imposto.ICMS.TipoICMS).vICMSDeson ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS51))
                    {
                        icmsTot.vBC = icmsTot.vBC + (((ICMS51)produto.imposto.ICMS.TipoICMS).vBC ?? 0);
                        icmsTot.vICMS = icmsTot.vICMS + (((ICMS51)produto.imposto.ICMS.TipoICMS).vICMS ?? 0);
                        icmsTot.vFCP = icmsTot.vFCP + (((ICMS51)produto.imposto.ICMS.TipoICMS).vFCP ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS61))
                    {
                        icmsTot.qBCMonoRet = icmsTot.qBCMonoRet + produto.prod.vProd;
                        icmsTot.vICMSMonoRet = icmsTot.vICMSMonoRet + (((ICMS61)produto.imposto.ICMS.TipoICMS).vICMSMonoRet ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS70))
                    {
                        icmsTot.vICMSDeson = icmsTot.vICMSDeson + (((ICMS70)produto.imposto.ICMS.TipoICMS).vICMSDeson ?? 0);
                        icmsTot.vBC = icmsTot.vBC + ((ICMS70)produto.imposto.ICMS.TipoICMS).vBC;
                        icmsTot.vICMS = icmsTot.vICMS + ((ICMS70)produto.imposto.ICMS.TipoICMS).vICMS;
                        icmsTot.vBCST = icmsTot.vBCST + ((ICMS70)produto.imposto.ICMS.TipoICMS).vBCST;
                        icmsTot.vST = icmsTot.vST + ((ICMS70)produto.imposto.ICMS.TipoICMS).vICMSST;
                        icmsTot.vFCP = icmsTot.vFCP + (((ICMS70)produto.imposto.ICMS.TipoICMS).vFCP ?? 0);
                        icmsTot.vFCPST = icmsTot.vFCPST + (((ICMS70)produto.imposto.ICMS.TipoICMS).vFCPST ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS90))
                    {
                        icmsTot.vICMSDeson = icmsTot.vICMSDeson + (((ICMS90)produto.imposto.ICMS.TipoICMS).vICMSDeson ?? 0);
                        icmsTot.vBC = icmsTot.vBC + (((ICMS90)produto.imposto.ICMS.TipoICMS).vBC ?? 0);
                        icmsTot.vICMS = icmsTot.vICMS + (((ICMS90)produto.imposto.ICMS.TipoICMS).vICMS ?? 0);
                        icmsTot.vBCST = icmsTot.vBCST + (((ICMS90)produto.imposto.ICMS.TipoICMS).vBCST ?? 0);
                        icmsTot.vST = icmsTot.vST + (((ICMS90)produto.imposto.ICMS.TipoICMS).vICMSST ?? 0);
                        icmsTot.vFCP = icmsTot.vFCP + (((ICMS90)produto.imposto.ICMS.TipoICMS).vFCP ?? 0);
                        icmsTot.vFCPST = icmsTot.vFCPST + (((ICMS90)produto.imposto.ICMS.TipoICMS).vFCPST ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN201))
                    {
                        icmsTot.vBCST = icmsTot.vBCST + ((ICMSSN201)produto.imposto.ICMS.TipoICMS).vBCST;
                        icmsTot.vST = icmsTot.vST + ((ICMSSN201)produto.imposto.ICMS.TipoICMS).vICMSST;
                        icmsTot.vFCPST = icmsTot.vFCPST + (((ICMSSN201)produto.imposto.ICMS.TipoICMS).vFCPST ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN202))
                    {
                        icmsTot.vBCST = icmsTot.vBCST + ((ICMSSN202)produto.imposto.ICMS.TipoICMS).vBCST;
                        icmsTot.vST = icmsTot.vST + ((ICMSSN202)produto.imposto.ICMS.TipoICMS).vICMSST;
                        icmsTot.vFCPST = icmsTot.vFCPST + (((ICMSSN202)produto.imposto.ICMS.TipoICMS).vFCPST ?? 0);
                    }
                    if (produto.imposto.ICMS != null && produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN900))
                    {
                        icmsTot.vBC = icmsTot.vBC + (((ICMSSN900)produto.imposto.ICMS.TipoICMS).vBC ?? 0);
                        icmsTot.vICMS = icmsTot.vICMS + (((ICMSSN900)produto.imposto.ICMS.TipoICMS).vICMS ?? 0);
                        icmsTot.vBCST = icmsTot.vBCST + (((ICMSSN900)produto.imposto.ICMS.TipoICMS).vBCST ?? 0);
                        icmsTot.vST = icmsTot.vST + (((ICMSSN900)produto.imposto.ICMS.TipoICMS).vICMSST ?? 0);
                        icmsTot.vFCPST = icmsTot.vFCPST + (((ICMSSN900)produto.imposto.ICMS.TipoICMS).vFCPST ?? 0);
                    }
                }
                else
                {
                    icmsTot.vICMSDeson = icmsTot.vICMSDeson + 0;
                    icmsTot.vBC = icmsTot.vBC + 0;
                    icmsTot.vICMS = icmsTot.vICMS + 0;
                    icmsTot.vBCST = icmsTot.vBCST + 0;
                    icmsTot.vST = icmsTot.vST + 0;
                }

                if (produto.imposto.ISSQN != null && produto.imposto.ISSQN.GetType() == typeof(NFe.Classes.Informacoes.Detalhe.Tributacao.Municipal.ISSQN))
                {
                    comISS = true;
                    issqnTot.vBC = issqnTot.vBC + produto.imposto.ISSQN.vBC;
                    issqnTot.vCOFINS = 0;
                    issqnTot.vDeducao = issqnTot.vDeducao + produto.imposto.ISSQN.vDeducao;
                    issqnTot.vDescCond = issqnTot.vDescCond + produto.imposto.ISSQN.vDescCond;
                    issqnTot.vDescIncond = issqnTot.vDescIncond + produto.imposto.ISSQN.vDescIncond;
                    issqnTot.vISS = issqnTot.vISS + produto.imposto.ISSQN.vISSQN;
                    issqnTot.vISSRet = issqnTot.vISSRet + produto.imposto.ISSQN.vISSRet;
                    issqnTot.vOutro = issqnTot.vOutro + produto.imposto.ISSQN.vOutro;
                    issqnTot.vPIS = 0;
                    issqnTot.vServ = issqnTot.vServ + produto.prod.vProd;

                    if (produto.imposto.PIS != null && produto.imposto.PIS.TipoPIS.GetType() == typeof(PISOutr))
                        issqnTot.vPIS = issqnTot.vPIS + (((PISOutr)produto.imposto.PIS.TipoPIS).vPIS ?? 0);

                    if (produto.imposto.COFINS != null && produto.imposto.COFINS.TipoCOFINS.GetType() == typeof(COFINSOutr))
                        issqnTot.vCOFINS = issqnTot.vCOFINS + (((COFINSOutr)produto.imposto.COFINS.TipoCOFINS).vCOFINS ?? 0);

                    if (produto.imposto.PIS != null && produto.imposto.PIS.TipoPIS.GetType() == typeof(PISAliq))
                        issqnTot.vPIS = issqnTot.vPIS + ((PISAliq)produto.imposto.PIS.TipoPIS).vPIS;

                    if (produto.imposto.COFINS != null && produto.imposto.COFINS.TipoCOFINS.GetType() == typeof(COFINSAliq))
                        issqnTot.vCOFINS = issqnTot.vCOFINS + ((COFINSAliq)produto.imposto.COFINS.TipoCOFINS).vCOFINS;

                    if (issqnTot.vPIS == 0)
                        issqnTot.vPIS = null;
                    if (issqnTot.vCOFINS == 0)
                        issqnTot.vCOFINS = null;
                }

                if (produto.impostoDevol != null)
                    icmsTot.vIPIDevol = icmsTot.vIPIDevol + (produto.impostoDevol.IPI).vIPIDevol;
            }

            if (comISS)
            {
                issqnTot.vBC = issqnTot.vBC > 0 ? issqnTot.vBC : null;
                issqnTot.vISS = issqnTot.vISS > 0 ? issqnTot.vISS : null;
            }

            icmsTot.vProd = icmsTot.vProd - issqnTot.vServ ?? 0;
            //icmsTot.vNF = icmsTot.vNF + icmsTot.vST + icmsTot.vIPI;

            var t = new total { ICMSTot = icmsTot, ISSQNtot = comISS ? issqnTot : null };
            return t;
        }
        protected virtual transp GetTransporte(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var volumes = new List<vol> { GetVolume(nfe) };

            var t = new transp
            {
                modFrete = nfe.TipoFrete == Dominio.Enumeradores.ModalidadeFrete.Destinatario ? ModalidadeFrete.mfContaDestinatario :
                nfe.TipoFrete == Dominio.Enumeradores.ModalidadeFrete.Emitente ? ModalidadeFrete.mfContaEmitenteOumfContaRemetente :
                nfe.TipoFrete == Dominio.Enumeradores.ModalidadeFrete.Terceiros ? ModalidadeFrete.mfContaTerceiros :
                nfe.TipoFrete == Dominio.Enumeradores.ModalidadeFrete.ProprioDestinatario ? ModalidadeFrete.mfProprioContaDestinatario :
                nfe.TipoFrete == Dominio.Enumeradores.ModalidadeFrete.ProprioRemetente ? ModalidadeFrete.mfProprioContaRemente :
                ModalidadeFrete.mfSemFrete,
                vol = volumes,
                transporta = nfe.LocalidadeTranspMunicipio != null && !string.IsNullOrWhiteSpace(nfe.TranspCNPJCPF) ? new transporta
                {
                    CNPJ = nfe.TranspCNPJCPF.Length == 14 ? nfe.TranspCNPJCPF : null,
                    CPF = nfe.TranspCNPJCPF.Length == 11 ? nfe.TranspCNPJCPF : null,
                    IE = !string.IsNullOrWhiteSpace(nfe.TranspIE) && nfe.TranspIE != "ISENTO" && nfe.TranspIE != "isento" ? Utilidades.String.OnlyNumbers(nfe.TranspIE) : null,
                    UF = !string.IsNullOrWhiteSpace(nfe.TranspUF) ? nfe.TranspUF : null,
                    xEnder = !string.IsNullOrWhiteSpace(nfe.TranspEndereco) ? Utilidades.String.RemoveDiacritics(nfe.TranspEndereco).Trim() : null,
                    xMun = nfe.LocalidadeTranspMunicipio != null && !string.IsNullOrWhiteSpace(nfe.LocalidadeTranspMunicipio.Descricao) ? Utilidades.String.RemoveDiacritics(nfe.LocalidadeTranspMunicipio.Descricao).Trim() : null,
                    xNome = nfe.TranspNome != null ? Utilidades.String.RemoveDiacritics(nfe.TranspNome).Trim() : null
                } : null,
                veicTransp = !string.IsNullOrWhiteSpace(nfe.TranspPlacaVeiculo) ? new veicTransp
                {
                    placa = !string.IsNullOrWhiteSpace(nfe.TranspPlacaVeiculo) ? nfe.TranspPlacaVeiculo : null,
                    RNTC = !string.IsNullOrWhiteSpace(nfe.TranspANTTVeiculo) ? nfe.TranspANTTVeiculo.Trim() : null,
                    UF = !string.IsNullOrWhiteSpace(nfe.TranspUFVeiculo) ? nfe.TranspUFVeiculo : null
                } : null
            };

            return t;
        }
        protected virtual List<autXML> GetAutXML(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, string CNPJFixo)
        {
            Dominio.Entidades.Empresa empresa = nfe.Empresa;
            if (nfe.Cliente.CPF_CNPJ_SemFormato.Equals(CNPJFixo) && string.IsNullOrWhiteSpace(empresa.CNPJContabilidade) && string.IsNullOrWhiteSpace(empresa.CPFContabilidade))
                return null;

            List<autXML> listAutXML = new List<autXML>();
            if (!nfe.Cliente.CPF_CNPJ_SemFormato.Equals(CNPJFixo))
                listAutXML.Add(new autXML { CNPJ = CNPJFixo });

            if (!string.IsNullOrWhiteSpace(empresa.CNPJContabilidade))
                listAutXML.Add(new autXML { CNPJ = empresa.CNPJContabilidade });

            if (!string.IsNullOrWhiteSpace(empresa.CPFContabilidade))
                listAutXML.Add(new autXML { CPF = empresa.CPFContabilidade });

            return listAutXML;
        }
        protected virtual vol GetVolume(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var v = new vol
            {
                esp = string.IsNullOrWhiteSpace(nfe.TranspEspecie) ? null : nfe.TranspEspecie,
                marca = string.IsNullOrWhiteSpace(nfe.TranspMarca) ? null : nfe.TranspMarca,
                nVol = string.IsNullOrWhiteSpace(nfe.TranspVolume) ? null : nfe.TranspVolume,
                pesoB = nfe.TranspPesoBruto,
                pesoL = nfe.TranspPesoLiquido,
                qVol = !string.IsNullOrWhiteSpace(nfe.TranspQuantidade) ? Convert.ToInt32(Utilidades.String.OnlyNumbers(nfe.TranspQuantidade)) : 0
            };
            if (!string.IsNullOrWhiteSpace(v.esp) || !string.IsNullOrWhiteSpace(v.marca) || !string.IsNullOrWhiteSpace(v.nVol) || v.pesoB > 0 || v.pesoL > 0 || v.qVol > 0)
                return v;
            else
                return null;
        }
        protected virtual cobr GetCobranca(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ICMSTot icmsTot, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela repNotaFiscalParcela = new Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela> notaFiscalParcelas = repNotaFiscalParcela.BuscarPorNota(nfe.Codigo);

            if (icmsTot.vNF > 0 && notaFiscalParcelas.Count() > 0)
            {
                var c = new cobr();
                c = new cobr
                {
                    fat = new fat
                    {
                        nFat = Convert.ToString(nfe.Numero),
                        vOrig = icmsTot.vNF,
                        vDesc = icmsTot.vNF - notaFiscalParcelas.Select(o => o.Valor).Sum(),
                        vLiq = notaFiscalParcelas.Select(o => o.Valor).Sum()
                    }
                };

                c.dup = new List<dup>();
                notaFiscalParcelas = notaFiscalParcelas.OrderBy(x => x.Sequencia).ToList();
                foreach (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela parcela in notaFiscalParcelas)
                {
                    c.dup.Add(new dup
                    {
                        nDup = parcela.Sequencia.ToString().PadLeft(3, '0'), //Convert.ToString(nfe.Numero) + " / " + Convert.ToString(i + 1),
                        vDup = parcela.Valor,
                        dVenc = parcela.DataVencimento.Value
                    });
                }

                return c;
            }
            else
                return null;
        }

        //protected virtual List<pag> GetPagamento(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ICMSTot icmsTot)
        //{
        //    var p = new List<pag>
        //    {
        //        new pag {tPag = FormaPagamento.fpDinheiro, vPag = nfe.ValorTotalNota},
        //    };
        //    return p;
        //}
        protected virtual List<pag> GetPagamentoVersao400(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ICMSTot icmsTot, cobr cobr)
        {
            IndicadorPagamentoDetalhePagamento? indicadorPagamento = null;
            if (cobr != null && cobr.dup.Count() == 1)
                indicadorPagamento = IndicadorPagamentoDetalhePagamento.ipDetPgVista;
            else if (cobr != null && cobr.dup.Count() > 1)
                indicadorPagamento = IndicadorPagamentoDetalhePagamento.ipDetPgPrazo;

            var p = new pag
            {
                detPag = new List<detPag>
                {
                    (nfe.Finalidade == Dominio.Enumeradores.FinalidadeNFe.Ajuste || nfe.Finalidade == Dominio.Enumeradores.FinalidadeNFe.Devolucao) ? new detPag {indPag = indicadorPagamento, tPag = FormaPagamento.fpSemPagamento, vPag = 0 } :
                    (nfe.ModeloNotaFiscal == "55" && cobr != null && cobr.dup.Count() > 0) ? new detPag {indPag = indicadorPagamento, tPag = FormaPagamento.fpDuplicataMercantil, vPag = nfe.ValorTotalNota} :
                    nfe.ModeloNotaFiscal == "65" ?  new detPag {indPag = indicadorPagamento, tPag = nfe.FormaPagamento.HasValue ? (FormaPagamento)nfe.FormaPagamento : FormaPagamento.fpDinheiro, vPag = nfe.ValorTotalNota} :
                    new detPag {indPag = indicadorPagamento, tPag = nfe.FormaPagamento.HasValue ? (FormaPagamento)nfe.FormaPagamento : FormaPagamento.fpDinheiro, vPag = nfe.ValorTotalNota}
                }
            };

            return new List<pag> { p };
        }

        protected virtual List<rastro> GetRastreabilidade(List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes> lotes)
        {
            var r = new List<rastro>();
            for (int i = 0; i < lotes.Count(); i++)
            {
                r.Add(new rastro
                {
                    nLote = lotes[i].NumeroLote,
                    qLote = lotes[i].QuantidadeLote,
                    dFab = lotes[i].DataFabricacao,
                    dVal = lotes[i].DataValidade,
                    cAgreg = !string.IsNullOrWhiteSpace(lotes[i].CodigoAgregacao) ? lotes[i].CodigoAgregacao : null
                });
            }

            return r;
        }

        protected virtual infRespTec GetResponsavelTecnico(infNFe infNFe, string UFEmitente, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConfiguracaoCSRT repConfiguracaoCSRT = new Repositorio.ConfiguracaoCSRT(unitOfWork);
            Dominio.Entidades.ConfiguracaoCSRT configuracaoCSRT = repConfiguracaoCSRT.BuscarPorEstado(UFEmitente);

            if (configuracaoCSRT != null && ((tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && configuracaoCSRT.HabilitaProducao)
                || (tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao && configuracaoCSRT.HabilitaHomologacao)))
            {
                DFe.Utils.DadosChaveFiscal chaveFiscal = DFe.Utils.ChaveFiscal.ObterChave(infNFe.ide.cUF, infNFe.ide.dEmi, !string.IsNullOrWhiteSpace(infNFe.emit.CNPJ) ? infNFe.emit.CNPJ : infNFe.emit.CPF, infNFe.ide.mod, infNFe.ide.serie, infNFe.ide.nNF, (int)infNFe.ide.tpEmis, 9);

                var i = new infRespTec
                {
                    CNPJ = "13969629000196",
                    xContato = "TECCHAPECO SISTEMAS",
                    email = "tech@multisoftware.com.br",
                    fone = "04930259500",
                    idCSRT = !string.IsNullOrWhiteSpace(configuracaoCSRT.idCSRT) ? configuracaoCSRT.idCSRT.ToNullableInt() : null,
                    hashCSRT = !string.IsNullOrWhiteSpace(configuracaoCSRT.CSRT) ? Utilidades.Calc.ObterBase64Sha1DeString(configuracaoCSRT.CSRT + Utilidades.String.OnlyNumbers(chaveFiscal.Chave)) : null
                };

                return i;
            }
            else
                return null;
        }

        protected virtual List<NFe.Classes.Informacoes.Detalhe.Exportacao.detExport> InformarExportacaoItem(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item)
        {
            if (string.IsNullOrWhiteSpace(item.NumeroDrawback) && string.IsNullOrWhiteSpace(item.NumeroRegistroExportacao) && string.IsNullOrWhiteSpace(item.ChaveAcessoExportacao))
                return null;

            List<NFe.Classes.Informacoes.Detalhe.Exportacao.detExport> listaDetExport = new List<NFe.Classes.Informacoes.Detalhe.Exportacao.detExport>();
            NFe.Classes.Informacoes.Detalhe.Exportacao.detExport detExport = new NFe.Classes.Informacoes.Detalhe.Exportacao.detExport();

            detExport.nDraw = !string.IsNullOrWhiteSpace(item.NumeroDrawback) ? item.NumeroDrawback : null;

            if (!string.IsNullOrWhiteSpace(item.NumeroRegistroExportacao) || !string.IsNullOrWhiteSpace(item.ChaveAcessoExportacao))
            {
                detExport.exportInd = new NFe.Classes.Informacoes.Detalhe.Exportacao.exportInd
                {
                    nRE = item.NumeroRegistroExportacao,
                    chNFe = item.ChaveAcessoExportacao,
                    qExport = item.Quantidade
                };
            }

            listaDetExport.Add(detExport);

            return listaDetExport;
        }

        protected virtual infIntermed GetIntermediadorTransacao(ide ide, Dominio.Entidades.Cliente intermediador)
        {
            if (intermediador == null)
                return null;

            if (ide.indIntermed.HasValue && ide.indIntermed == IndicadorIntermediador.iiSitePlataformaTerceiros)
            {
                string nomeIntermediador = intermediador.Nome;
                if (nomeIntermediador.Length > 60)
                    nomeIntermediador = nomeIntermediador.Substring(0, 60);
                nomeIntermediador = Utilidades.String.RemoveDiacritics(nomeIntermediador.Trim());

                infIntermed infIntermed = new infIntermed
                {
                    CNPJ = intermediador.CPF_CNPJ_SemFormato,
                    idCadIntTran = nomeIntermediador
                };

                return infIntermed;
            }
            else
                return null;
        }

        protected virtual List<ProdutoEspecifico> GetProdutoEspecifico(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item, bool gerarMedicamento)
        {
            List<ProdutoEspecifico> produtosEspecificos = new List<ProdutoEspecifico>();

            if (!string.IsNullOrWhiteSpace(item.CodigoANP))
            {
                produtosEspecificos.Add(new comb
                {
                    cProdANP = item.CodigoANP,
                    descANP = Utilidades.String.RemoveDiacritics(item.DescricaoItem).Trim().TrimEnd().TrimStart(),
                    UFCons = item.NotaFiscal.Cliente != null ? item.NotaFiscal.Cliente.Localidade.Estado.Sigla : item.NotaFiscal.Empresa.Localidade.Estado.Sigla,
                    pGLP = item.PercentualGLP > 0 ? item.PercentualGLP : null,
                    pGNn = item.PercentualGNN > 0 ? item.PercentualGNN : null,
                    pGNi = item.PercentualGNI > 0 ? item.PercentualGNI : null,
                    vPart = item.ValorPartidaANP > 0 ? item.ValorPartidaANP : null,
                    pBio = item.PercentualMisturaBiodiesel > 0 ? item.PercentualMisturaBiodiesel : null,
                    origComb =
                 ((item.CSTICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST61) || (item.PercentualOrigemComb.HasValue && item.PercentualOrigemComb.Value > 0)) ? new List<origComb>()
                 {
                     new origComb()
                     {
                         cUFOrig = (DFe.Classes.Entidades.Estado)(item.NotaFiscal.Empresa?.Localidade?.Estado?.CodigoIBGE ?? 0),
                         indImport = item.Produto != null && item.Produto.IndicadorImportacaoCombustivel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorImportacaoCombustivel.Importado ? 1 : 0,
                         pOrig = item.PercentualOrigemComb.HasValue && item.PercentualOrigemComb.Value > 0 ? item.PercentualOrigemComb.Value : 0
                     }
                 } : null
                });
            }

            if (gerarMedicamento && !string.IsNullOrWhiteSpace(item.Produto.CodigoAnvisa))
            {
                produtosEspecificos.Add(new med { cProdANVISA = item.Produto.CodigoAnvisa, vPMC = item.ValorUnitario });
            }

            if (produtosEspecificos.Count == 0)
                return null;

            return produtosEspecificos;
        }

        #endregion Métodos de preenchimento do Componente
    }
}
