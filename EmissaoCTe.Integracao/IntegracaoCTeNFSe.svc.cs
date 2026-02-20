using Dominio.ObjetosDeValor.NFSe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace EmissaoCTe.Integracao
{
    public class IntegracaoCTeNFSe : IIntegracaoCTeNFSe
    {
        #region Metodos Publicos
        public RetornoDocumento<int> IntegrarDocumento(Dominio.ObjetosDeValor.CTe.CTeNFSe documento, string cnpjEmpresaAdministradora, string token)
        {
            try
            {
                Servicos.Log.TratarErro("IntegrarDocumento - Documento: " + (documento != null ? Newtonsoft.Json.JsonConvert.SerializeObject(documento) : string.Empty));
                Servicos.Log.TratarErro("IntegrarDocumento - EmpresaAdministradora: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora) ? cnpjEmpresaAdministradora : string.Empty));
                Servicos.Log.TratarErro("IntegrarDocumento - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                string NBS = System.Configuration.ConfigurationManager.AppSettings["NBS"];
                string codigoIndicadorOperacao = System.Configuration.ConfigurationManager.AppSettings["CodigoIndicadorOperacao"];

                if (documento.IBSCBS != null && string.IsNullOrEmpty(documento.IBSCBS.NBS))
                    documento.IBSCBS.NBS = NBS;

                if (documento.IBSCBS != null && string.IsNullOrEmpty(documento.IBSCBS.CodigoIndicadorOperacao))
                    documento.IBSCBS.CodigoIndicadorOperacao = codigoIndicadorOperacao;

                if (ConfigurationManager.AppSettings["GerarCTesIntegrarDocumentoPorThread"] == "SIM")
                    return this.EmitirCTeNFSePorObjetoNew(documento, cnpjEmpresaAdministradora, token);
                else
                    return this.EmitirCTeNFSePorObjeto(documento, cnpjEmpresaAdministradora, token);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha: IntegrarDocumento " + ex);

                try
                {

                    Servicos.Email svcEmail = new Servicos.Email();

                    string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    string assunto = ambiente + " - Problemas na emissão de documentos!";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<p>Atenção, problemas na emissão no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                    sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br", 587, null, 0, true, null, false);
#else
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br", 587, null, 0, true, null, false);
#endif                  
                }
                catch (Exception exptEmail)
                {
                    Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
                }

                return new RetornoDocumento<int>() { Mensagem = "Ocorreu uma falha generica ao integrar  CTe/NFSe.", Status = false };
            }
        }

        public RetornoDocumento<int> IntegrarDocumentoAguardarConfirmacao(Dominio.ObjetosDeValor.CTe.CTeNFSe documento, string cnpjEmpresaAdministradora, string token)
        {
            try
            {
                Servicos.Log.TratarErro("IntegrarDocumentoAguardarConfirmacao - Documento: " + (documento != null ? Newtonsoft.Json.JsonConvert.SerializeObject(documento) : string.Empty));
                Servicos.Log.TratarErro("IntegrarDocumentoAguardarConfirmacao - EmpresaAdministradora: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora) ? cnpjEmpresaAdministradora : string.Empty));
                Servicos.Log.TratarErro("IntegrarDocumentoAguardarConfirmacao - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                return this.EmitirCTeNFSePorObjetoAguardarConfirmacao(documento, cnpjEmpresaAdministradora, token);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha: IntegrarDocumento " + ex);

                try
                {

                    Servicos.Email svcEmail = new Servicos.Email();

                    string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    string assunto = ambiente + " - Problemas na emissão de documentos!";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<p>Atenção, problemas na emissão no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                    sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br", 587, null, 0, true, null, false);
#else
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br", 587, null, 0, true, null, false);
#endif                  
                }
                catch (Exception exptEmail)
                {
                    Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
                }

                return new RetornoDocumento<int>() { Mensagem = "Ocorreu uma falha generica ao integrar  CTe/NFSe.", Status = false };
            }
        }

        public Retorno<RetornoCTeNFSe> BuscarPorCodigoDocumento(int codigo, string documento, string token, string codificarUTF8)
        {
            if (ConfigurationManager.AppSettings["BloquearBuscarPorCodigoDocumento"] == "SIM")
                return new Retorno<RetornoCTeNFSe>() { Mensagem = "", Status = false };

            Servicos.Log.TratarErro("Codigo  " + codigo.ToString() + " Tipo " + documento, "BuscarPorCodigoDocumento");

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (string.IsNullOrWhiteSpace(codificarUTF8))
                    codificarUTF8 = "S";

                if (documento == "CTe")
                {
                    Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unidadeDeTrabalho);

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);

                    if (cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                        return new Retorno<RetornoCTeNFSe>() { Mensagem = "Token de acesso inválido.", Status = false };

                    Retorno<RetornoCTeNFSe> retorno = new Retorno<RetornoCTeNFSe> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                    Dominio.Entidades.XMLCTe xml = repXMLCTe.BuscarPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
                    Dominio.Entidades.XMLCTe xmlCancelamento = repXMLCTe.BuscarPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Cancelamento);

                    string xmlCTe = string.Empty;
                    if (xml != null)
                    {
                        if (!xml.XMLArmazenadoEmArquivo)
                            xmlCTe = xml.XML;
                        else
                        {
                            Servicos.CTe serCTe = new Servicos.CTe(unidadeDeTrabalho);

                            string caminho = serCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(xml.CTe, "A", unidadeDeTrabalho);

                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                xmlCTe = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminho);
                        }
                    }

                    string xmlCanc = string.Empty;
                    if (xmlCancelamento != null)
                    {
                        if (!xmlCancelamento.XMLArmazenadoEmArquivo)
                            xmlCanc = xml.XML;
                        else
                        {
                            Servicos.CTe serCTe = new Servicos.CTe(unidadeDeTrabalho);

                            string caminho = serCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(xmlCancelamento.CTe, "C", unidadeDeTrabalho);

                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                xmlCanc = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminho);
                        }
                    }

                    retorno.Objeto = new RetornoCTeNFSe()
                    {
                        Documento = "CTe",
                        Codigo = cte.Codigo,
                        Numero = cte.Numero,
                        DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        Serie = cte.Serie.Numero,
                        ChaveCTe = cte.Chave,
                        NumeroProtocolo = cte.Protocolo,
                        CNPJEmpresa = cte.Empresa.CNPJ,
                        Status = cte.Status,
                        Ambiente = cte.TipoAmbiente,
                        MensagemRetorno = cte.MensagemStatus != null ? cte.MensagemStatus.MensagemDoErro : cte.MensagemRetornoSefaz,
                        XML = xmlCTe,
                        XMLCancelamento = xmlCanc,
                        PDF = servicoCTe.ObterDACTE(cte.Codigo, cte.Empresa.Codigo, unidadeDeTrabalho, codificarUTF8 == "S"),
                        JustificativaCancelamento = cte.Status == "C" ? cte.ObservacaoCancelamento : string.Empty,
                        ICMS = new Dominio.ObjetosDeValor.CTe.ImpostoICMS()
                        {
                            Aliquota = cte.AliquotaICMS,
                            BaseCalculo = cte.BaseCalculoICMS,
                            CST = cte.CST == "91" ? "90" : !string.IsNullOrWhiteSpace(cte.CST) ? cte.CST : string.Empty,
                            PercentualReducaoBaseCalculo = cte.PercentualReducaoBaseCalculoICMS,
                            Valor = cte.ValorICMS,
                            ValorCreditoPresumido = cte.ValorPresumido,
                            ValorDevido = cte.ValorICMSDevido
                        },
                    };
                    return retorno;
                }
                else
                {
                    Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                    Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                    Repositorio.XMLNFSe repXMLNFSe = new Repositorio.XMLNFSe(unidadeDeTrabalho);
                    Repositorio.ItemNFSe repItemNFSe = new Repositorio.ItemNFSe(unidadeDeTrabalho);

                    Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigo);

                    if (nfse == null)
                        return new Retorno<RetornoCTeNFSe>() { Mensagem = "NFSe não localizada.", Status = false };

                    if (nfse.Empresa.EmpresaPai.Configuracao != null && nfse.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                        return new Retorno<RetornoCTeNFSe>() { Mensagem = "Token de acesso inválido.", Status = false };

                    Retorno<RetornoCTeNFSe> retorno = new Retorno<RetornoCTeNFSe> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                    Dominio.Entidades.XMLNFSe xml = repXMLNFSe.BuscarPorNFSe(nfse.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao);
                    Dominio.Entidades.XMLNFSe xmlCancelamento = repXMLNFSe.BuscarPorNFSe(nfse.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Cancelamento);

                    List<Dominio.Entidades.ItemNFSe> listaItemNFSe = repItemNFSe.BuscarPorNFSe(nfse.Codigo);

                    retorno.Objeto = new RetornoCTeNFSe()
                    {
                        Documento = "NFSe",
                        Codigo = nfse.Codigo,
                        Numero = nfse.Numero,
                        DataEmissao = nfse.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss"),
                        Serie = nfse.Serie.Numero,
                        NumeroProtocolo = nfse.CodigoVerificacao,
                        CNPJEmpresa = nfse.Empresa.CNPJ,
                        Status = nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? "A" : nfse.Status == Dominio.Enumeradores.StatusNFSe.Cancelado ? "C" : nfse.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao ? "R" : "E",
                        Ambiente = nfse.Ambiente,
                        MensagemRetorno = nfse.RPS?.MensagemRetorno != null ? nfse.RPS?.MensagemRetorno : string.Empty,
                        XML = xml != null ? xml.XML : string.Empty,
                        XMLCancelamento = xmlCancelamento != null ? xmlCancelamento.XML : string.Empty,
                        PDF = servicoNFSe.ObterDANFSEString(nfse.Codigo, unidadeDeTrabalho),
                        JustificativaCancelamento = nfse.Status == Dominio.Enumeradores.StatusNFSe.Cancelado ? nfse.JustificativaCancelamento : string.Empty,
                        AliquotaISS = nfse.AliquotaISS,
                        ValorISS = nfse.ValorISS,
                        ServicoCodigo = listaItemNFSe != null && listaItemNFSe.Count() > 0 ? listaItemNFSe.FirstOrDefault().Servico.Codigo.ToString() : string.Empty,
                        NumeroRPS = nfse.RPS?.Numero.ToString(),
                        SerieRPS = nfse.RPS?.Serie,
                        DataRPS = nfse.RPS != null && nfse.RPS.Data.HasValue ? nfse.RPS?.Data.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        NumeroNFSePrefeitura = !string.IsNullOrWhiteSpace(nfse.NumeroPrefeitura) ? nfse.NumeroPrefeitura : nfse.Numero.ToString()
                    };
                    return retorno;
                }


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<RetornoCTeNFSe>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        public Retorno<RetornoCTeNFSe> BuscarPorChaveCTe(string chaveCTe, string token, string codificarUTF8)
        {
            if (ConfigurationManager.AppSettings["BloquearBuscarPorChaveCTe"] == "SIM")
                return new Retorno<RetornoCTeNFSe>() { Mensagem = "", Status = false };

            Servicos.Log.TratarErro("ChaveCTe  " + chaveCTe, "BuscarPorChaveCTe");

            //System.Threading.Thread.Sleep(1537);

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                //unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);

                if (string.IsNullOrWhiteSpace(codificarUTF8))
                    codificarUTF8 = "S";

                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unidadeDeTrabalho);

                int codigoCTe = repCTe.BuscarCodigoPorChave(chaveCTe);

                if (codigoCTe == 0)
                    return new Retorno<RetornoCTeNFSe>() { Mensagem = "Chave CTe não localizado", Status = false };

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new Retorno<RetornoCTeNFSe>() { Mensagem = "CTe não localizado", Status = false };

                if (cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<RetornoCTeNFSe>() { Mensagem = "Token de acesso inválido.", Status = false };

                Retorno<RetornoCTeNFSe> retorno = new Retorno<RetornoCTeNFSe> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                Dominio.Entidades.XMLCTe xml = repXMLCTe.BuscarPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
                Dominio.Entidades.XMLCTe xmlCancelamento = repXMLCTe.BuscarPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Cancelamento);

                retorno.Objeto = new RetornoCTeNFSe()
                {
                    Documento = "CTe",
                    Codigo = cte.Codigo,
                    Numero = cte.Numero,
                    DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                    Serie = cte.Serie.Numero,
                    ChaveCTe = cte.Chave,
                    NumeroProtocolo = cte.Protocolo,
                    CNPJEmpresa = cte.Empresa.CNPJ,
                    Status = cte.Status,
                    Ambiente = cte.TipoAmbiente,
                    MensagemRetorno = cte.MensagemStatus != null ? cte.MensagemStatus.MensagemDoErro : cte.MensagemRetornoSefaz,
                    XML = xml != null ? xml.XML : string.Empty,
                    XMLCancelamento = xmlCancelamento != null ? xmlCancelamento.XML : string.Empty,
                    PDF = servicoCTe.ObterDACTE(cte.Codigo, cte.Empresa.Codigo, unidadeDeTrabalho, codificarUTF8 == "S"),
                    JustificativaCancelamento = cte.Status == "C" ? cte.ObservacaoCancelamento : string.Empty,
                    ICMS = new Dominio.ObjetosDeValor.CTe.ImpostoICMS()
                    {
                        Aliquota = cte.AliquotaICMS,
                        BaseCalculo = cte.BaseCalculoICMS,
                        CST = cte.CST == "91" ? "90" : !string.IsNullOrWhiteSpace(cte.CST) ? cte.CST : string.Empty,
                        PercentualReducaoBaseCalculo = cte.PercentualReducaoBaseCalculoICMS,
                        Valor = cte.ValorICMS,
                        ValorCreditoPresumido = cte.ValorPresumido,
                        ValorDevido = cte.ValorICMSDevido
                    },
                };
                return retorno;

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<RetornoCTeNFSe>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        public Retorno<int> IntegrarNFSeProcessada(Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfseProcessada, string cnpjEmpresaAdministradora, string token)
        {
            try
            {
                Servicos.Log.TratarErro("IntegrarNFSeProcessada - IntegrarNFSeProcessada: " + (nfseProcessada != null ? Newtonsoft.Json.JsonConvert.SerializeObject(nfseProcessada) : string.Empty), "IntegrarNFSeProcessada");
                Servicos.Log.TratarErro("IntegrarNFSeProcessada - EmpresaAdministradora: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora) ? cnpjEmpresaAdministradora : string.Empty), "IntegrarNFSeProcessada");
                Servicos.Log.TratarErro("IntegrarNFSeProcessada - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty), "IntegrarNFSeProcessada");

                if (nfseProcessada == null)
                    return new Retorno<int>() { Mensagem = "A NFS-e nao deve ser nula para a integração.", Status = false };

                if (nfseProcessada.Emitente == null)
                    return new Retorno<int>() { Mensagem = "O Emitente nao pode ser nulo.", Status = false };

                if (ConfigurationManager.AppSettings["GerarNFSesProcessadasPorThread"] == "SIM")
                    return this.SalvarNFSeProcessadaNew(nfseProcessada, cnpjEmpresaAdministradora, token);
                else
                    return this.SalvarNFSeProcessada(nfseProcessada, cnpjEmpresaAdministradora, token);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha NFSe " + nfseProcessada.Numero.ToString() + ": " + ex, "IntegrarNFSeProcessada");

                try
                {

                    Servicos.Email svcEmail = new Servicos.Email();

                    string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    string assunto = ambiente + " - Problemas na integração de NFSe Processada!";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<p>Atenção, problemas na integração de NFSe Processada no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                    sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br");
#else
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br");
#endif              
                }
                catch (Exception exptEmail)
                {
                    Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
                }

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha generica ao integrar a NFS-e.", Status = false };
            }
        }

        public RetornoDocumento<int> ReenviarDocumento(int codigo, string documento, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (documento == "CTe")
                {
                    Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);

                    if (cte == null)
                        return new RetornoDocumento<int>() { Mensagem = "CTe não localizado.", Status = false };

                    if (cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                        return new RetornoDocumento<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                    if (cte.Status != "S" && cte.Status != "R")
                        return new RetornoDocumento<int>() { Mensagem = "Status do CT-e não permite reenvio.", Status = false };

                    if (servicoCTe.Emitir(ref cte, unidadeDeTrabalho))
                    {
                        servicoCTe.AtualizarIntegracaoRetornoCTe(cte, unidadeDeTrabalho);
                        if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                        {
                            cte.Status = "R";
                            repCTe.Atualizar(cte);
                            return new RetornoDocumento<int>() { Mensagem = "O CT-e " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.", Status = false };
                        }
                    }
                    else
                        return new RetornoDocumento<int>() { Mensagem = "Não foi possícel reenviar CTe.", Status = false };

                    return new RetornoDocumento<int>() { Mensagem = "Reenvio CT-e realizado com sucesso.", Status = true, Objeto = cte.Codigo, Documento = "CTe" };
                }
                else
                {
                    Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                    Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

                    Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigo);

                    if (nfse == null)
                        return new RetornoDocumento<int>() { Mensagem = "NFS-e não localizada.", Status = false };

                    if (nfse.Empresa.EmpresaPai.Configuracao != null && nfse.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                        return new RetornoDocumento<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                    if (nfse.Status != Dominio.Enumeradores.StatusNFSe.EmDigitacao && nfse.Status != Dominio.Enumeradores.StatusNFSe.Rejeicao)
                        return new RetornoDocumento<int>() { Mensagem = "Status da NFs-e não permite reenvio.", Status = false };


                    if (servicoNFSe.Emitir(nfse, unidadeDeTrabalho))
                    {
                        if (!servicoNFSe.AdicionarNFSeNaFilaDeConsulta(nfse, unidadeDeTrabalho))
                            return new RetornoDocumento<int>() { Mensagem = "A NFs-e nº " + nfse.Numero.ToString() + " da empresa " + nfse.Empresa.CNPJ + " foi salva, porem, nao foi possivel adiciona-la na fila de consulta.", Status = false };
                    }
                    else
                        return new RetornoDocumento<int>() { Mensagem = "Não foi possícel reenviar NF-e.", Status = false };

                    return new RetornoDocumento<int>() { Mensagem = "Reenvio NFs-e realizado com sucesso.", Status = true, Objeto = nfse.Codigo, Documento = "NFSe" };
                }


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new RetornoDocumento<int>() { Mensagem = "Ocorreu uma falha ao reenviar integração.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public RetornoDocumento<int> ConfirmarEmissaoDocumento(int codigo, string documento, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Log.TratarErro("ConfirmarEmissaoDocumento - Codigo: " + codigo.ToString() + " - Documento: " + documento);
                if (documento == "CTe")
                {
                    Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);

                    if (cte == null)
                        return new RetornoDocumento<int>() { Mensagem = "CTe não localizado.", Status = false };

                    if (cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                        return new RetornoDocumento<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                    if (cte.Status != "P")
                        return new RetornoDocumento<int>() { Mensagem = "CT-e não esta aguardando confirmação para emissão. Status atual: " + cte.DescricaoStatus, Status = false };

                    Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);
                    Dominio.Entidades.IntegracaoCTe integracaoCTe = repIntegracaoCTe.BuscarPorCTeTipoStatus(codigo, Dominio.Enumeradores.TipoIntegracao.Emissao, Dominio.Enumeradores.StatusIntegracao.AguardandoConfirmacao);

                    if (integracaoCTe != null)
                    {
                        integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoCTe;
                        repIntegracaoCTe.Atualizar(integracaoCTe);

                        return new RetornoDocumento<int>() { Mensagem = "Confirmação realizada com sucesso.", Status = true };
                    }
                    else
                        return new RetornoDocumento<int>() { Mensagem = "Nenhuma integração aguardando confirmação encontrada para o CT-e.", Status = false };
                }
                else
                {
                    Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                    Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

                    Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigo);

                    if (nfse == null)
                        return new RetornoDocumento<int>() { Mensagem = "NFS-e não localizada.", Status = false };

                    if (nfse.Empresa.EmpresaPai.Configuracao != null && nfse.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                        return new RetornoDocumento<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                    if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Pendente)
                        return new RetornoDocumento<int>() { Mensagem = "NFS-e não esta aguardando confirmação para emissão. Status atual: " + nfse.DescricaoStatus, Status = false };

                    Repositorio.IntegracaoNFSe repIntegracaoNFSe = new Repositorio.IntegracaoNFSe(unidadeDeTrabalho);
                    Dominio.Entidades.IntegracaoNFSe integracaoNFSe = repIntegracaoNFSe.BuscarPorNFSeTipoStatus(codigo, Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao, Dominio.Enumeradores.StatusIntegracao.AguardandoConfirmacao);

                    if (integracaoNFSe != null)
                    {
                        integracaoNFSe.Status = Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoCTe;
                        repIntegracaoNFSe.Atualizar(integracaoNFSe);

                        return new RetornoDocumento<int>() { Mensagem = "Confirmação realizada com sucesso.", Status = true };
                    }
                    else
                        return new RetornoDocumento<int>() { Mensagem = "Nenhuma integração aguardando confirmação encontrada para a NFS-e.", Status = false };
                }


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new RetornoDocumento<int>() { Mensagem = "Ocorreu uma falha ao reenviar integração.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<RetornoProtocoloPorNFe>> ConsultarProtocoloPorNFe(string chaveNFe, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string codificarUTF8 = "S";

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

                if (empresaPai != null && empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<List<RetornoProtocoloPorNFe>>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (string.IsNullOrWhiteSpace(chaveNFe))
                    return new Retorno<List<RetornoProtocoloPorNFe>>() { Mensagem = "Chave de NFe não informada.", Status = false };

                if (!Utilidades.Validate.ValidarChave(Utilidades.String.OnlyNumbers(chaveNFe)))
                    return new Retorno<List<RetornoProtocoloPorNFe>>() { Mensagem = "Chave de NFe inválida.", Status = false };

                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                List<RetornoProtocoloPorNFe> listaRetornoProtocoloPorNFe = new List<RetornoProtocoloPorNFe>();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = repCTe.BuscarListaPorChaveNFe(Utilidades.String.OnlyNumbers(chaveNFe));
                for (var i = 0; i < listaCTe.Count(); i++)
                {
                    RetornoProtocoloPorNFe retornoCTE = new RetornoProtocoloPorNFe();
                    retornoCTE.Protocolo = listaCTe[i].Codigo;
                    retornoCTE.Documento = "CTe";
                    retornoCTE.StatusDocumento = listaCTe[i].DescricaoStatus;

                    listaRetornoProtocoloPorNFe.Add(retornoCTE);
                }

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                List<Dominio.Entidades.NFSe> listaNFSe = repNFSe.BuscarListaPorChaveNFe(Utilidades.String.OnlyNumbers(chaveNFe));
                for (var i = 0; i < listaNFSe.Count(); i++)
                {
                    RetornoProtocoloPorNFe retornoNFSe = new RetornoProtocoloPorNFe();
                    retornoNFSe.Protocolo = listaNFSe[i].Codigo;
                    retornoNFSe.Documento = "NFSe";
                    retornoNFSe.StatusDocumento = listaNFSe[i].DescricaoStatus;

                    listaRetornoProtocoloPorNFe.Add(retornoNFSe);
                }

                Retorno<List<RetornoProtocoloPorNFe>> retorno = new Retorno<List<RetornoProtocoloPorNFe>>();
                retorno.Mensagem = "Retorno realizado com sucesso.";
                retorno.Status = true;
                retorno.Objeto = listaRetornoProtocoloPorNFe;

                return retorno;

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<RetornoProtocoloPorNFe>>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> InformarTerminoEnvioDocumentos(Dominio.ObjetosDeValor.IntegrarCarga integrarCarga, string token)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Log.TratarErro("InformarTerminoEnvioDocumentos - IntegrarCarga: " + (integrarCarga != null ? Newtonsoft.Json.JsonConvert.SerializeObject(integrarCarga) : string.Empty), "InformarTerminoEnvioDocumentos");
                Servicos.Log.TratarErro("InformarTerminoEnvioDocumentos - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty), "InformarTerminoEnvioDocumentos");

                if (integrarCarga == null)
                {
                    Servicos.Log.TratarErro("Dados da carga nao pode ser nula para a integracao.", "InformarTerminoEnvioDocumentos");
                    return new Retorno<int>() { Mensagem = "Dados da carga nao pode ser nula para a integracao.", Status = false };
                }

                if (string.IsNullOrWhiteSpace(integrarCarga.NumeroCarga))
                {
                    Servicos.Log.TratarErro("Numero carga nao informado.", "InformarTerminoEnvioDocumentos");
                    return new Retorno<int>() { Mensagem = "Numero carga nao informado.", Status = false };
                }

                if (string.IsNullOrWhiteSpace(integrarCarga.NumeroUnidade))
                {
                    Servicos.Log.TratarErro("Numero unidade nao informado.", "InformarTerminoEnvioDocumentos");
                    return new Retorno<int>() { Mensagem = "Numero unidade nao informado.", Status = false };
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                Repositorio.IntegracaoCarga repIntegracaoCarga = new Repositorio.IntegracaoCarga(unitOfWork);
                Dominio.Entidades.IntegracaoCarga integracaoCarga = repIntegracaoCarga.BuscarPorCargaUniadade(integrarCarga.NumeroCarga, integrarCarga.NumeroUnidade);

                if (integracaoCarga == null)
                {
                    integracaoCarga = new Dominio.Entidades.IntegracaoCarga();
                    integracaoCarga.DataIntegracao = DateTime.Now;
                    integracaoCarga.NumeroDaCarga = integrarCarga.NumeroCarga;
                    integracaoCarga.NumeroDaUnidade = integrarCarga.NumeroUnidade;
                    integracaoCarga.Status = Dominio.Enumeradores.StatusIntegracaoCarga.Pendente;

                    repIntegracaoCarga.Inserir(integracaoCarga);

                    return new Retorno<int>() { Mensagem = "Integracao realizada com sucesso.", Objeto = integracaoCarga.Codigo, Status = true };
                }
                else if (integracaoCarga.Status == Dominio.Enumeradores.StatusIntegracaoCarga.Erro)
                {
                    integracaoCarga.Status = Dominio.Enumeradores.StatusIntegracaoCarga.Pendente;
                    repIntegracaoCarga.Atualizar(integracaoCarga);

                    return new Retorno<int>() { Mensagem = "Integracao realizada com sucesso.", Objeto = integracaoCarga.Codigo, Status = true };
                }
                else
                {


                    return new Retorno<int>() { Mensagem = "Integracao em duplicidade.", Objeto = integracaoCarga.Codigo, Status = false };
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha " + integrarCarga?.NumeroCarga + ": " + ex, "InformarTerminoEnvioDocumentos");

                try
                {

                    Servicos.Email svcEmail = new Servicos.Email();

                    string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    string assunto = ambiente + " - Problemas na integração InformarTerminoEnvioDocumentos!";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<p>Atenção, problemas InformarTerminoEnvioDocumentos no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                    sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br");
#else
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br");
#endif              
                }
                catch (Exception exptEmail)
                {
                    Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
                }

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha generica ao integrar Termino Envio Documentos", Status = false };
            }
        }

        public Retorno<int> SolicitarCancelamentoDocumentos(Dominio.ObjetosDeValor.IntegrarCarga integrarCarga, string justificativa, string token)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Log.TratarErro("SolicitarCancelaentoDocumentos - IntegrarCarga: " + (integrarCarga != null ? Newtonsoft.Json.JsonConvert.SerializeObject(integrarCarga) : string.Empty), "SolicitarCancelaentoDocumentos");
                Servicos.Log.TratarErro("SolicitarCancelaentoDocumentos - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty), "SolicitarCancelaentoDocumentos");

                if (integrarCarga == null)
                {
                    Servicos.Log.TratarErro("Dados da carga nao pode ser nula para a integracao.", "SolicitarCancelaentoDocumentos");
                    return new Retorno<int>() { Mensagem = "Dados da carga nao pode ser nula para a integracao.", Status = false };
                }

                if (string.IsNullOrWhiteSpace(integrarCarga.NumeroCarga))
                {
                    Servicos.Log.TratarErro("Numero carga nao informado.", "SolicitarCancelaentoDocumentos");
                    return new Retorno<int>() { Mensagem = "Numero carga nao informado.", Status = false };
                }

                if (string.IsNullOrWhiteSpace(integrarCarga.NumeroUnidade))
                {
                    Servicos.Log.TratarErro("Numero unidade nao informado.", "SolicitarCancelaentoDocumentos");
                    return new Retorno<int>() { Mensagem = "Numero unidade nao informado.", Status = false };
                }

                if (string.IsNullOrWhiteSpace(justificativa) || justificativa.Length < 20)
                {
                    Servicos.Log.TratarErro("Justificativa deve possuir pelo menos 20 Caracteres.", "SolicitarCancelaentoDocumentos");
                    return new Retorno<int>() { Mensagem = "Justificativa deve possuir pelo menos 20 Caracteres.", Status = false };
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                Repositorio.IntegracaoCarga repIntegracaoCarga = new Repositorio.IntegracaoCarga(unitOfWork);
                Dominio.Entidades.IntegracaoCarga integracaoCarga = repIntegracaoCarga.BuscarPorCargaUniadadeStatus(integrarCarga.NumeroCarga, integrarCarga.NumeroUnidade, Dominio.Enumeradores.StatusIntegracaoCarga.Gerado);

                if (integracaoCarga != null)
                {
                    integracaoCarga.Status = Dominio.Enumeradores.StatusIntegracaoCarga.PendenteCancelamento;
                    integracaoCarga.JustificativaCancelameto = justificativa.Left(200);
                    repIntegracaoCarga.Atualizar(integracaoCarga);

                    return new Retorno<int>() { Mensagem = "Integracao realizada com sucesso.", Objeto = integracaoCarga.Codigo, Status = true };
                }
                else
                {
                    integracaoCarga = repIntegracaoCarga.BuscarPorCargaUniadadeStatus(integrarCarga.NumeroCarga, integrarCarga.NumeroUnidade, Dominio.Enumeradores.StatusIntegracaoCarga.PendenteCancelamento);
                    if (integracaoCarga != null)
                        return new Retorno<int>() { Mensagem = "Cancelamento ja solicitado, em processamento.", Objeto = integracaoCarga.Codigo, Status = false };

                    integracaoCarga = repIntegracaoCarga.BuscarPorCargaUniadadeStatus(integrarCarga.NumeroCarga, integrarCarga.NumeroUnidade, Dominio.Enumeradores.StatusIntegracaoCarga.SolicitadoCancelamento);
                    if (integracaoCarga != null)
                        return new Retorno<int>() { Mensagem = "Cancelamento ja solicitado.", Objeto = integracaoCarga.Codigo, Status = false };


                    integracaoCarga = repIntegracaoCarga.BuscarPorCargaUniadadeStatus(integrarCarga.NumeroCarga, integrarCarga.NumeroUnidade, Dominio.Enumeradores.StatusIntegracaoCarga.CancelamentoNaoEfetuado);
                    if (integracaoCarga != null)
                        return new Retorno<int>() { Mensagem = "Cancelamento solicitado, não efetuado.", Objeto = integracaoCarga.Codigo, Status = false };

                    return new Retorno<int>() { Mensagem = "Carga/Unidade não localizado para cancelamento.", Objeto = 0, Status = false };
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha " + integrarCarga?.NumeroCarga + ": " + ex, "SolicitarCancelaentoDocumentos");

                try
                {

                    Servicos.Email svcEmail = new Servicos.Email();

                    string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    string assunto = ambiente + " - Problemas na integração SolicitarCancelaentoDocumentos!";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<p>Atenção, problemas SolicitarCancelaentoDocumentos no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                    sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br");
#else
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br");
#endif              
                }
                catch (Exception exptEmail)
                {
                    Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
                }

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha generica ao Solicitar Cancelamento Documentos.", Status = false };
            }
        }

        public Retorno<int> SolicitarQuitacaoCIOTCarga(Dominio.ObjetosDeValor.IntegrarCarga integrarCarga, string token)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Log.TratarErro("SolicitarQuitacaoCIOTCarga - IntegrarCarga: " + (integrarCarga != null ? Newtonsoft.Json.JsonConvert.SerializeObject(integrarCarga) : string.Empty), "SolicitarQuitacaoCIOTCarga");
                Servicos.Log.TratarErro("SolicitarQuitacaoCIOTCarga - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty), "SolicitarQuitacaoCIOTCarga");

                if (integrarCarga == null)
                {
                    Servicos.Log.TratarErro("Dados da carga nao pode ser nula para a integracao.", "SolicitarQuitacaoCIOTCarga");
                    return new Retorno<int>() { Mensagem = "Dados da carga nao pode ser nula para a integracao.", Status = false };
                }

                if (string.IsNullOrWhiteSpace(integrarCarga.NumeroCarga))
                {
                    Servicos.Log.TratarErro("Numero carga nao informado.", "SolicitarQuitacaoCIOTCarga");
                    return new Retorno<int>() { Mensagem = "Numero carga nao informado.", Status = false };
                }

                if (string.IsNullOrWhiteSpace(integrarCarga.NumeroUnidade))
                {
                    Servicos.Log.TratarErro("Numero unidade nao informado.", "SolicitarQuitacaoCIOTCarga");
                    return new Retorno<int>() { Mensagem = "Numero unidade nao informado.", Status = false };
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso invalido.", Status = false };

                Repositorio.IntegracaoCarga repIntegracaoCarga = new Repositorio.IntegracaoCarga(unitOfWork);
                Dominio.Entidades.IntegracaoCarga integracaoCarga = repIntegracaoCarga.BuscarPorCargaUniadadeStatus(integrarCarga.NumeroCarga, integrarCarga.NumeroUnidade, Dominio.Enumeradores.StatusIntegracaoCarga.Gerado);
                if (integracaoCarga == null)
                    return new Retorno<int>() { Mensagem = "Nao localizada integracao de carga com sucesso.", Status = false };

                if (integracaoCarga.CodigoCarga == 0)
                    return new Retorno<int>() { Mensagem = "Integracao da carga não gerou carga.", Status = false };

                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(integracaoCarga.CodigoCarga);//repCargaCIOT.BuscarPorNumeroCargaFilial(integrarCarga.NumeroCarga, integrarCarga.NumeroUnidade);

                if (cargaCIOT == null || cargaCIOT.CIOT == null)
                {
                    Servicos.Log.TratarErro("Nao localizado CIOT para a carga " + integrarCarga.NumeroCarga + " da filial " + integrarCarga.NumeroUnidade + ".", "SolicitarQuitacaoCIOTCarga");
                    return new Retorno<int>() { Mensagem = "Nao localizado CIOT para a carga " + integrarCarga.NumeroCarga + " da filial " + integrarCarga.NumeroUnidade + ".", Status = false };
                }

                if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)
                {
                    Servicos.Log.TratarErro("CIOT ja esta encerrado. Carga " + integrarCarga.NumeroCarga + " filial " + integrarCarga.NumeroUnidade + ".", "SolicitarQuitacaoCIOTCarga");
                    return new Retorno<int>() { Mensagem = "CIOT ja esta encerrado.", Status = true };
                }

                if (cargaCIOT.CIOT.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                {
                    Servicos.Log.TratarErro("Situacao do CIOT (" + cargaCIOT.CIOT.DescricaoSituacao + ") nao permite realizar a quitacao. Carga " + integrarCarga.NumeroCarga + " filial " + integrarCarga.NumeroUnidade + ".", "SolicitarQuitacaoCIOTCarga");
                    return new Retorno<int>() { Mensagem = "Situacao do CIOT (" + cargaCIOT.CIOT.DescricaoSituacao + ") nao permite realizar a quitacao.", Status = false };
                }

                bool sucesso = false;
                string mensagemErro = string.Empty;
                switch (cargaCIOT.CIOT.Operadora)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete:
                        Servicos.Embarcador.CIOT.EFrete serEFrete = new Servicos.Embarcador.CIOT.EFrete();
                        sucesso = serEFrete.EncerrarCIOT(cargaCIOT.CIOT, unitOfWork, out mensagemErro);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom:
                        Servicos.Embarcador.CIOT.Repom svcRepom = new Servicos.Embarcador.CIOT.Repom();
                        sucesso = svcRepom.EncerrarCIOT(cargaCIOT.CIOT, unitOfWork, out mensagemErro);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard:
                        Servicos.Embarcador.CIOT.Pamcard svcPamcard = new Servicos.Embarcador.CIOT.Pamcard();
                        sucesso = svcPamcard.EncerrarCIOT(cargaCIOT.CIOT, unitOfWork, out mensagemErro);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem:
                        Servicos.Embarcador.CIOT.Pagbem svcPagbem = new Servicos.Embarcador.CIOT.Pagbem();
                        sucesso = svcPagbem.EncerrarCIOT(cargaCIOT.CIOT, unitOfWork, out mensagemErro);
                        break;
                    default:
                        mensagemErro = "Encerramento não implementado para a operadora.";
                        break;
                }

                if (sucesso)
                    return new Retorno<int>() { Mensagem = "Quitacao realizada com sucesso.", Objeto = cargaCIOT.Carga.Codigo, Status = true };
                else
                {
                    Servicos.Log.TratarErro("Solicitacao de Quitacao do CIOT nao processada: " + mensagemErro + ". Carga " + integrarCarga.NumeroCarga + " filial " + integrarCarga.NumeroUnidade + ".", "SolicitarQuitacaoCIOTCarga");
                    return new Retorno<int>() { Mensagem = "Solicitacao de Quitacao do CIOT nao processada: " + mensagemErro, Objeto = cargaCIOT.Carga.Codigo, Status = true };
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha " + integrarCarga?.NumeroCarga + ": " + ex, "SolicitarQuitacaoCIOTCarga");

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha generica ao Solicitar Quitação Documentos.", Status = false };
            }
        }

        public Retorno<int> CancelarCTe(string cnpjEmpresaAdministradora, int codigo, string justificativa, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);


                if (string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora))
                    return new Retorno<int>() { Mensagem = "CNPJ da Empresa inválido (" + cnpjEmpresaAdministradora + ").", Status = false };

                if (string.IsNullOrWhiteSpace(justificativa) || justificativa.Trim().Length < 20)
                    return new Retorno<int>() { Mensagem = "Justificativa inválida (" + justificativa + ").", Status = false };

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);

                if (cte == null)
                    return new Retorno<int>() { Mensagem = "CT-e não encontrado.", Status = false };

                if (cte.Empresa.EmpresaPai.CNPJ != cnpjEmpresaAdministradora)
                    return new Retorno<int>() { Mensagem = "Empresa administradora inválida para a emissão do CT-e.", Status = false };

                if (cte.Empresa.EmpresaPai.Configuracao != null && token != cte.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (cte.Status != "A")
                    return new Retorno<int>() { Mensagem = "O status do CT-e não permite o cancelamento do mesmo.", Status = false };

                Dominio.Entidades.IntegracaoCTe integracao = repIntegracao.BuscarPorCTe(cte.Codigo);

                if (integracao == null)
                    return new Retorno<int>() { Mensagem = "Registro de integração do CT-e (" + cte.Chave + ") não encontrado.", Status = false };

                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cte.SistemaEmissor).CancelarCte(cte.Codigo, cte.Empresa.Codigo, justificativa, unidadeDeTrabalho))
                    return new Retorno<int>() { Mensagem = "Não foi possível enviar o CT-e (" + cte.Chave + ") para cancelamento.", Status = false };

                if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                    return new Retorno<int>() { Mensagem = "Não foi possível adicionar o CT-e (" + cte.Chave + ") na fila de consulta.", Status = false };

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                svcCTe.AtualizarIntegracaoRetornoCTe(cte, unidadeDeTrabalho);

                if (!this.AdicionarRegistroCTeIntegrado(cte, integracao.NumeroDaCarga, integracao.NumeroDaUnidade, "", "", "", Dominio.Enumeradores.TipoArquivoIntegracao.CTe, Dominio.Enumeradores.TipoIntegracao.Cancelamento, unidadeDeTrabalho))
                    return new Retorno<int>() { Mensagem = "Não foi possível adicionar o CT-e (" + cte.Chave + ") nos registros de integração, consulte o sistema da empresa." };

                return new Retorno<int>() { Mensagem = "CT-e (" + cte.Chave + ") em processo de cancelamento.", Status = true };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao cancelar o CTe: " + ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao cancelar o CT-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private Retorno<int> SalvarNFSeProcessada(Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfseProcessada, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.RPSNFSe repRPS = new Repositorio.RPSNFSe(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(nfseProcessada.Emitente.CNPJ));
                int.TryParse(nfseProcessada.Serie, out int serieNFSe);
                Dominio.Entidades.EmpresaSerie serieEmpresa = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, serieNFSe, Dominio.Enumeradores.TipoSerie.NFSe);
                if (serieEmpresa == null)
                    return new Retorno<int>() { Mensagem = "Série não liberada para receber NFSe processadae.", Status = false };

                string erros = this.ValidarNFSeProcessada(nfseProcessada, empresa, cnpjEmpresaAdministradora, token, unidadeDeTrabalho); string caminho = System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivosXMLNFeAverbacao"];

                if (!string.IsNullOrEmpty(erros))
                    return new Retorno<int>() { Mensagem = erros, Status = false };

                if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                    unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                else
                    unidadeDeTrabalho.Start();

                Dominio.Entidades.NFSe nfse = null;

                nfse = repNFSe.BuscarPorRPSSituacaoAmbiente(empresa.Codigo, nfseProcessada.NumeroRPS, Dominio.Enumeradores.StatusNFSe.AguardandoAutorizacaoRPS, nfseProcessada.Ambiente);
                if (nfse != null)
                {
                    if (!string.IsNullOrWhiteSpace(nfseProcessada.Status) && nfseProcessada.Status.Substring(0, 1) == "A")
                    {
                        if (nfseProcessada.Numero > 0)
                        {
                            nfse.Serie = serieEmpresa;
                            nfse.Numero = nfseProcessada.Numero;
                            repNFSe.Atualizar(nfse);

                            nfse.RPS.Protocolo = nfseProcessada.Protocolo;
                            repRPS.Atualizar(nfse.RPS);
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Retorno: Número da NFSe  " + nfse.Numero.ToString() + " não é valido para atualizar situação do RPS", "IntegrarNFSeProcessada");

                            unidadeDeTrabalho.Rollback();

                            return new Retorno<int>() { Mensagem = "Número da NFSe  " + nfse.Numero.ToString() + " não é valido para atualizar situação do RPS", Status = false };
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Retorno: Status " + nfseProcessada.Status + " não é valido para atualizar situação do RPS", "IntegrarNFSeProcessada");

                        unidadeDeTrabalho.Rollback();

                        return new Retorno<int>() { Mensagem = "Status " + nfseProcessada.Status + " não é valido para atualizar situação do RPS", Status = false };
                    }

                    unidadeDeTrabalho.CommitChanges();

                    Servicos.Log.TratarErro("IntegrarNFSeProcessada - : Integracao NFSe " + nfseProcessada.Numero.ToString() + " realizada com sucesso: " + nfse.Codigo.ToString(), "IntegrarNFSeProcessada");
                    return new Retorno<int>() { Mensagem = "Integracao realizada com sucesso.", Status = true, Objeto = nfse.Codigo };

                }
                else
                {
                    if (nfseProcessada.Numero > 0)
                        nfse = repNFSe.BuscarPorNumeroEStatus(empresa.Codigo, nfseProcessada.Numero, serieNFSe, null, nfseProcessada.Ambiente);
                    else
                    {
                        nfse = repNFSe.BuscarPorRPSSituacaoAmbiente(empresa.Codigo, nfseProcessada.NumeroRPS, null, nfseProcessada.Ambiente);
                    }

                    if (nfse != null && string.IsNullOrWhiteSpace(nfseProcessada.Status))
                        return new Retorno<int>() { Mensagem = "NFS-e com mesmo numero/serie ja existente.", Status = true, Objeto = nfse.Codigo };

                    if (nfse != null && nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado && !string.IsNullOrWhiteSpace(nfseProcessada.Status) && nfseProcessada.Status.Substring(0, 1) == "A")
                        return new Retorno<int>() { Mensagem = "NFS-e com mesmo numero/serie/status ja existente.", Status = true, Objeto = nfse.Codigo };

                    if (nfse != null && nfse.Status == Dominio.Enumeradores.StatusNFSe.Cancelado && !string.IsNullOrWhiteSpace(nfseProcessada.Status) && nfseProcessada.Status.Substring(0, 1) == "C")
                        return new Retorno<int>() { Mensagem = "NFS-e com mesmo numero/serie/status ja existente.", Status = true, Objeto = nfse.Codigo };

                    if (nfse != null && !string.IsNullOrWhiteSpace(nfseProcessada.Status) && nfseProcessada.Status.Substring(0, 1) == "P")
                        return new Retorno<int>() { Mensagem = "RPS NFSe já existente.", Status = true, Objeto = nfse.Codigo };

                    Dominio.Entidades.Veiculo veiculo = null;
                    if (!string.IsNullOrWhiteSpace(nfseProcessada.Placa))
                    {
                        veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, nfseProcessada.Placa);

                        if (veiculo == null)
                        {
                            veiculo = new Dominio.Entidades.Veiculo();
                            veiculo.Placa = nfseProcessada.Placa;
                            veiculo.Empresa = empresa;
                            veiculo.Ativo = true;
                            veiculo.Estado = empresa.Localidade.Estado;
                            veiculo.Tipo = "P";
                            veiculo.TipoRodado = "01";
                            veiculo.TipoCarroceria = "00";

                            repVeiculo.Inserir(veiculo);
                        }
                    }

                    Dominio.Entidades.EmpresaSerie serie = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, serieNFSe, Dominio.Enumeradores.TipoSerie.NFSe);
                    if (serie == null)
                    {
                        serie = new Dominio.Entidades.EmpresaSerie();
                        serie.Empresa = empresa;
                        serie.Numero = serieNFSe;
                        serie.Tipo = Dominio.Enumeradores.TipoSerie.NFSe;
                        serie.Status = "A";
                        repEmpresaSerie.Inserir(serie);
                    }

                    if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                    else
                        unidadeDeTrabalho.Start();

                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                    Dominio.Entidades.NFSe nfseIntegrada = null;

                    if (nfse != null && !string.IsNullOrWhiteSpace(nfseProcessada.Status) && nfseProcessada.Status.Substring(0, 1) == "C")
                    {
                        nfseIntegrada = svcNFSe.CancelarNFSeProcessada(nfse, unidadeDeTrabalho);

                        if (!this.AdicionarRegistroNFSeIntegrado(nfse, nfseProcessada.NumeroCarga, nfseProcessada.NumeroUnidade, nfseProcessada.CodigoTipoOperacao, "", JsonConvert.SerializeObject(nfseProcessada), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoNFSe.Cancelamento, unidadeDeTrabalho, nfseProcessada.Romaneio, nfseProcessada.TipoVeiculo, nfseProcessada.TipoCalculo, nfseProcessada.ValorDespesa))
                        {
                            unidadeDeTrabalho.Rollback();

                            return new Retorno<int>() { Mensagem = "Ocorreu uma falha e não foi possível adicionar a NFS-e na lista de integracoes.", Status = false };
                        }
                    }
                    else
                    {
                        nfseIntegrada = svcNFSe.GravarNFSeProcessada(nfseProcessada, empresa, veiculo, serie, unidadeDeTrabalho);

                        if (!this.AdicionarRegistroNFSeIntegrado(nfseIntegrada, nfseProcessada.NumeroCarga, nfseProcessada.NumeroUnidade, nfseProcessada.CodigoTipoOperacao, "", JsonConvert.SerializeObject(nfseProcessada), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao, unidadeDeTrabalho, nfseProcessada.Romaneio, nfseProcessada.TipoVeiculo, nfseProcessada.TipoCalculo, nfseProcessada.ValorDespesa))
                        {
                            unidadeDeTrabalho.Rollback();

                            return new Retorno<int>() { Mensagem = "Ocorreu uma falha e não foi possível adicionar a NFS-e na lista de integracoes.", Status = false };
                        }
                    }

                    unidadeDeTrabalho.CommitChanges();

                    Servicos.Log.TratarErro("IntegrarNFSeProcessada - : Integracao NFSe " + nfseProcessada.Numero.ToString() + " realizada com sucesso: " + nfseIntegrada.Codigo.ToString(), "IntegrarNFSeProcessada");
                    return new Retorno<int>() { Mensagem = "Integracao realizada com sucesso.", Status = true, Objeto = nfseIntegrada.Codigo };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha NFSe " + nfseProcessada.Numero.ToString() + ": " + ex, "IntegrarNFSeProcessada");

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha generica ao integrar a NFS-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        private Retorno<int> SalvarNFSeProcessadaNew(Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfseProcessada, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                //unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.RPSNFSe repRPS = new Repositorio.RPSNFSe(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPorCNPJ(Utilidades.String.OnlyNumbers(nfseProcessada.Emitente.CNPJ));

                string erros = this.ValidarNFSeProcessada(nfseProcessada, empresa, cnpjEmpresaAdministradora, token, unidadeDeTrabalho);

                if (!string.IsNullOrEmpty(erros))
                    return new Retorno<int>() { Mensagem = erros, Status = false };

                int.TryParse(nfseProcessada.Serie, out int serieNFSe);

                Dominio.Entidades.EmpresaSerie serie = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, serieNFSe, Dominio.Enumeradores.TipoSerie.NFSe);

                if (serie == null)
                    return new Retorno<int>() { Mensagem = "Série não liberada para receber NFSe processadae.", Status = false };

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorNumeroEStatus(empresa.Codigo, nfseProcessada.Numero, serieNFSe, null, nfseProcessada.Ambiente);

                if (nfse != null && string.IsNullOrWhiteSpace(nfseProcessada.Status))
                    return new Retorno<int>() { Mensagem = "NFS-e com mesmo numero/serie ja existente.", Status = true, Objeto = nfse.Codigo };

                if (nfse != null && nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado && !string.IsNullOrWhiteSpace(nfseProcessada.Status) && nfseProcessada.Status.Substring(0, 1) == "A")
                    return new Retorno<int>() { Mensagem = "NFS-e com mesmo numero/serie/status ja existente.", Status = true, Objeto = nfse.Codigo };

                if (nfse != null && nfse.Status == Dominio.Enumeradores.StatusNFSe.Cancelado && !string.IsNullOrWhiteSpace(nfseProcessada.Status) && nfseProcessada.Status.Substring(0, 1) == "C")
                    return new Retorno<int>() { Mensagem = "NFS-e com mesmo numero/serie/status ja existente.", Status = true, Objeto = nfse.Codigo };


                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                Dominio.Entidades.NFSe nfseIntegrada = null;

                if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                    unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                else
                    unidadeDeTrabalho.Start();

                if (nfse != null && !string.IsNullOrWhiteSpace(nfseProcessada.Status) && nfseProcessada.Status.Substring(0, 1) == "C")
                {
                    nfseIntegrada = servicoNFSe.CancelarNFSeProcessada(nfse, unidadeDeTrabalho);

                    if (!this.AdicionarRegistroNFSeIntegrado(nfse, nfseProcessada.NumeroCarga, nfseProcessada.NumeroUnidade, nfseProcessada.CodigoTipoOperacao, "", JsonConvert.SerializeObject(nfseProcessada), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoNFSe.Cancelamento, unidadeDeTrabalho, nfseProcessada.Romaneio, nfseProcessada.TipoVeiculo, nfseProcessada.TipoCalculo, nfseProcessada.ValorDespesa))
                    {
                        Servicos.Log.TratarErro("Erro ao AdicionarRegistroNFSeIntegrado", "IntegrarNFSeProcessada");

                        unidadeDeTrabalho.Rollback();

                        return new Retorno<int>() { Mensagem = "Ocorreu uma falha e não foi possível adicionar a NFS-e na lista de integracoes.", Status = false };
                    }
                }
                else
                {
                    nfseIntegrada = repNFSe.BuscarPorRPSSituacaoAmbiente(empresa.Codigo, nfseProcessada.NumeroRPS, Dominio.Enumeradores.StatusNFSe.AguardandoAutorizacaoRPS, nfseProcessada.Ambiente);
                    if (nfseIntegrada != null)
                    {
                        if (!string.IsNullOrWhiteSpace(nfseProcessada.Status) && nfseProcessada.Status.Substring(0, 1) == "A")
                        {
                            if (nfseProcessada.Numero > 0)
                            {
                                nfseIntegrada.Serie = serie;
                                nfseIntegrada.Numero = nfseProcessada.Numero;
                                nfseIntegrada.Status = Dominio.Enumeradores.StatusNFSe.Autorizado;
                                repNFSe.Atualizar(nfseIntegrada);

                                nfseIntegrada.RPS.Protocolo = nfseProcessada.Protocolo;
                                repRPS.Atualizar(nfseIntegrada.RPS);

                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorRPS(nfseIntegrada.RPS.Codigo);
                                if (cte != null)
                                {
                                    cte.Numero = nfseProcessada.Numero;
                                    cte.Serie = serie;
                                    cte.Status = "A";
                                    cte.Protocolo = nfseProcessada.Protocolo;

                                    repCTe.Atualizar(cte);
                                }
                            }
                            else
                            {
                                Servicos.Log.TratarErro("Retorno: Número da NFSe  " + nfseIntegrada.Numero.ToString() + " não é valido para atualizar situação do RPS", "IntegrarNFSeProcessada");

                                unidadeDeTrabalho.Rollback();

                                return new Retorno<int>() { Mensagem = "Número da NFSe  " + nfseIntegrada.Numero.ToString() + " não é valido para atualizar situação do RPS", Status = false };
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Retorno: Status " + nfseProcessada.Status + " não é valido para atualizar situação do RPS", "IntegrarNFSeProcessada");

                            unidadeDeTrabalho.Rollback();

                            return new Retorno<int>() { Mensagem = "Status " + nfseProcessada.Status + " não é valido para atualizar situação do RPS", Status = false };
                        }

                    }
                    else
                    {
                        //Validação se já existe o RPS
                        bool validarRPSJaIntegrado = ConfigurationManager.AppSettings["ValidarRPSJaIntegrado"] != "NAO";
                        if (validarRPSJaIntegrado && repNFSe.BuscarPorRPSSituacaoAmbiente(empresa.Codigo, nfseProcessada.NumeroRPS, null, nfseProcessada.Ambiente) != null)
                        {
                            return new Retorno<int>() { Mensagem = "RPS " + nfseProcessada.NumeroRPS + " ja integrado anteriormente.", Status = false };
                        }
                        else
                        {
                            nfseIntegrada = servicoNFSe.GravarNFSeProcessadaTemporaria(nfseProcessada, empresa, serie, unidadeDeTrabalho);

                            if (!this.AdicionarRegistroNFSeIntegrado(nfseIntegrada, nfseProcessada.NumeroCarga, nfseProcessada.NumeroUnidade, nfseProcessada.CodigoTipoOperacao, "", JsonConvert.SerializeObject(nfseProcessada), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao, unidadeDeTrabalho, nfseProcessada.Romaneio, nfseProcessada.TipoVeiculo, nfseProcessada.TipoCalculo, nfseProcessada.ValorDespesa, Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoNFSeTemporaria))
                            {
                                Servicos.Log.TratarErro("Erro: AdicionarRegistroNFSeIntegrado", "IntegrarNFSeProcessada");

                                unidadeDeTrabalho.Rollback();

                                return new Retorno<int>() { Mensagem = "Ocorreu uma falha e não foi possível adicionar a NFS-e na lista de integracoes.", Status = false };
                            }
                        }
                    }
                }

                unidadeDeTrabalho.CommitChanges();
                if (nfseProcessada.Numero > 0)
                {
                    Servicos.Log.TratarErro("IntegrarNFSeProcessada: " + nfseProcessada.Numero.ToString() + " realizada com sucesso: " + nfseIntegrada.Codigo.ToString(), "IntegrarNFSeProcessada");
                    return new Retorno<int>() { Mensagem = "Integracao NFSe realizada com sucesso.", Status = true, Objeto = nfseIntegrada.Codigo };
                }
                else
                {
                    Servicos.Log.TratarErro("IntegrarNFSeProcessada: " + nfseProcessada.NumeroRPS.ToString() + " realizada com sucesso: " + nfseIntegrada.Codigo.ToString(), "IntegrarNFSeProcessada");
                    return new Retorno<int>() { Mensagem = "Integracao RPS realizada com sucesso.", Status = true, Objeto = nfseIntegrada.Codigo };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha NFSe " + nfseProcessada.Numero.ToString() + ": " + ex, "IntegrarNFSeProcessada");

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha generica ao integrar a NFS-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        private RetornoDocumento<int> EmitirCTeNFSePorObjetoNew(Dominio.ObjetosDeValor.CTe.CTeNFSe documento, string cnpjEmpresaAdministradora, string token, int codigoCTe = 0)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {

                if (documento == null)
                    return new RetornoDocumento<int>() { Mensagem = "O CT-e nao deve ser nulo para a integracao.", Status = false };

                if (documento.Emitente == null)
                    return new RetornoDocumento<int>() { Mensagem = "O Emitente nao pode ser nulo.", Status = false };

                if (documento.CodigoIBGECidadeInicioPrestacao == 0)
                    return new RetornoDocumento<int>() { Mensagem = "O código IBGE de inicio da prestação não foi enviado.", Status = false };

                if (documento.CodigoIBGECidadeTerminoPrestacao == 0)
                    return new RetornoDocumento<int>() { Mensagem = "O código IBGE de termino da prestação não foi enviado.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(documento.Emitente.CNPJ));

                string erros = string.Empty;

                Dominio.Enumeradores.TipoDocumento tipoDocumento = Dominio.Enumeradores.TipoDocumento.CTe;

                if (documento.CodigoIBGECidadeInicioPrestacao == documento.CodigoIBGECidadeTerminoPrestacao && !empresa.Configuracao.GerarCTeIntegracaoDocumentosMunicipais)
                    tipoDocumento = Dominio.Enumeradores.TipoDocumento.NFSe;

                Dominio.Entidades.EmpresaSerie empresaSerie = null;
                if (tipoDocumento == Dominio.Enumeradores.TipoDocumento.CTe)
                    erros = this.ValidarCTe(documento, empresa, cnpjEmpresaAdministradora, token, ref empresaSerie, unidadeDeTrabalho);
                else
                    erros = this.ValidarNFSe(documento, empresa, cnpjEmpresaAdministradora, token, unidadeDeTrabalho);

                if (!string.IsNullOrEmpty(erros))
                    return new RetornoDocumento<int>() { Mensagem = erros, Status = false };

                int tipoEnvio = string.IsNullOrWhiteSpace(empresa.Configuracao?.NumeroLoteEmissaoCTe) ? 0 : int.Parse(empresa.Configuracao.NumeroLoteEmissaoCTe);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = null;

                string retorno = string.Empty;

                if (tipoDocumento == Dominio.Enumeradores.TipoDocumento.CTe)
                {
                    Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                    string bloquearIntegracaoDuplicada = ConfigurationManager.AppSettings["BloquearIntegracaoDuplicada"];
                    if (bloquearIntegracaoDuplicada == "SIM" && documento.Documentos != null && documento.Documentos.Count > 0)
                    {
                        Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
                        Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                        string chaveNFe = documento.Documentos.FirstOrDefault().ChaveNFE;
                        if (chaveNFe != null)
                        {
                            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repDocumentosCTe.BuscarCTesPorChaveNFe(chaveNFe, empresa.Codigo);

                            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNota in listaCTes)
                            {
                                if (cteNota.Status == "A" || cteNota.Status == "R" || cteNota.Status == "E" || cteNota.Status == "P")
                                {
                                    Dominio.Entidades.IntegracaoCTe integracao = repIntegracaoCTe.BuscarPorCTe(cteNota.Codigo);

                                    if (integracao != null && !string.IsNullOrWhiteSpace(integracao.Arquivo) && JsonConvert.SerializeObject(documento) == integracao.Arquivo)
                                    {
                                        cteIntegrado = integracao.CTe;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (cteIntegrado == null)
                    {
                        Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unidadeDeTrabalho);
                        Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);
                        Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                        Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                        Dominio.Entidades.ModalTransporte modal = repModalTransporte.BuscarPorCodigo(1, false);
                        Dominio.Entidades.ModeloDocumentoFiscal modelo = repModeloDocumentoFiscal.BuscarPorModelo("57");
                        if (empresaSerie == null)
                            empresaSerie = repSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.CTe); //servicoCTe.ObterSerie(empresa, empresa.Localidade.Estado.Sigla, empresa.Localidade.Estado.Sigla, empresa.Localidade.Estado.Sigla, unidadeDeTrabalho);
                        Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorNumero(5932);

                        if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                            unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                        else
                            unidadeDeTrabalho.Start();

                        Dominio.ObjetosDeValor.CTe.CTe documentoCTe = servicoCTe.ConverteObjetoCTeNFSe(documento);
                        cteIntegrado = servicoCTe.GerarCTeTemporarioIntegracao(documentoCTe, empresa, modal, modelo, empresaSerie, cfop, tipoEnvio, unidadeDeTrabalho);

                        if (!this.AdicionarRegistroCTeIntegrado(cteIntegrado, documento.NumeroCarga, documento.NumeroUnidade, documento.CodigoTipoOperacao, "", JsonConvert.SerializeObject(documento), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracao.Emissao, unidadeDeTrabalho, documento.CodigoControleInternoCliente, documento.Romaneio, documento.TipoVeiculo, documento.TipoCalculo, documento.ValorDespesa, Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoCTe))
                        {
                            unidadeDeTrabalho.Rollback();

                            return new RetornoDocumento<int>() { Mensagem = "Ocorreu uma falha e nao foi possivel adicionar o CT-e na lista de integracoes.", Status = false, Documento = "CTe" };
                        }

                        servicoCTe.SalvarIntegracaoRetornoCTe(cteIntegrado, empresa, unidadeDeTrabalho);

                        unidadeDeTrabalho.CommitChanges();

                        //Chamar fila para iniciar MultiCTe
                        this.AdicionarCTeNaFilaDeConsulta(null);
                    }
                    else if (cteIntegrado.Status == "R")
                    {
                        if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                            unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                        else
                            unidadeDeTrabalho.Start();

                        if (servicoCTe.Deletar(cteIntegrado.Codigo, cteIntegrado.Empresa.Codigo, false, unidadeDeTrabalho))
                        {
                            Dominio.ObjetosDeValor.CTe.CTe documentoCTe = servicoCTe.ConverteObjetoCTeNFSe(documento);
                            cteIntegrado = servicoCTe.GerarCTePorObjeto(documentoCTe, cteIntegrado.Codigo, unidadeDeTrabalho, "1", tipoEnvio, "E", null, 0, null, empresa);

                            unidadeDeTrabalho.CommitChanges();
                        }
                        else
                        {
                            unidadeDeTrabalho.Rollback();

                            return new RetornoDocumento<int>() { Mensagem = "Nao foi possível integrar o CT-e, verifique o status do mesmo.", Status = false };
                        }
                    }

                    if (cteIntegrado.Status == "E")
                    {
                        if (!servicoCTe.Emitir(ref cteIntegrado, unidadeDeTrabalho))
                            retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.";

                        if (!this.AdicionarCTeNaFilaDeConsulta(cteIntegrado))
                            retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.";
                    }

                    if (cteIntegrado != null)
                        Servicos.Log.TratarErro("IntegrarDocumento - Retornado CodigoCTe: " + cteIntegrado.Codigo);
                    if (string.IsNullOrWhiteSpace(retorno))
                        return new RetornoDocumento<int>() { Mensagem = "Integracao realizada com sucesso.", Status = true, Objeto = cteIntegrado.Codigo, Documento = "CTe" };
                    else
                        return new RetornoDocumento<int>() { Mensagem = retorno, Status = false, Objeto = cteIntegrado.Codigo, Documento = "CTe" };
                }
                else //Emitir NFSe a partir do objeto CTe
                {
                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                    Dominio.Enumeradores.StatusNFSe statusNFS = Dominio.Enumeradores.StatusNFSe.Pendente;

                    // Configuração para gerar uma NFs Manual (vínculo entre MultiCTe -> SGT)
                    bool emiteFora = empresa.Configuracao?.EmiteNFSeForaEmbarcador ?? false;
                    if (emiteFora)
                        statusNFS = Dominio.Enumeradores.StatusNFSe.AgGeracaoNFSeManual;

                    if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                    else
                        unidadeDeTrabalho.Start();

                    Dominio.Entidades.NFSe nfseIntegrada = svcNFSe.GerarNFSePorObjetoObjetoCTe(documento, unidadeDeTrabalho, statusNFS);

                    if (!this.AdicionarRegistroNFSeIntegrado(nfseIntegrada, documento.NumeroCarga, documento.NumeroUnidade, documento.CodigoTipoOperacao, "", JsonConvert.SerializeObject(documento), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao, unidadeDeTrabalho, documento.Romaneio, documento.TipoVeiculo, documento.TipoCalculo, documento.ValorDespesa))
                    {
                        unidadeDeTrabalho.Rollback();

                        return new RetornoDocumento<int>() { Mensagem = "Ocorreu uma falha e nao foi possivel adicionar a NFS-e na lista de integracoes.", Status = false, Documento = "NFSe" };
                    }

                    unidadeDeTrabalho.CommitChanges();

                    if (nfseIntegrada.Status != Dominio.Enumeradores.StatusNFSe.AgGeracaoNFSeManual)
                    {
                        if (!svcNFSe.Emitir(nfseIntegrada, unidadeDeTrabalho))
                            retorno += "A NFS-e nº " + nfseIntegrada.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salva, porem, ocorreu uma falha ao emiti-la.";

                        if (!svcNFSe.AdicionarNFSeNaFilaDeConsulta(nfseIntegrada, unidadeDeTrabalho))
                            retorno += "A NFS-e nº " + nfseIntegrada.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salva, porem, nao foi possivel adiciona-la na fila de consulta.";
                    }

                    if (string.IsNullOrWhiteSpace(retorno))
                        return new RetornoDocumento<int>() { Mensagem = "Integração realizada com sucesso.", Status = true, Objeto = nfseIntegrada.Codigo, Documento = "NFSe" };
                    else
                        return new RetornoDocumento<int>() { Mensagem = retorno, Status = false, Objeto = nfseIntegrada.Codigo, Documento = "NFSe" };
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("EmitirCTeNFSePorObjetoNew: " + ex);

                unidadeDeTrabalho.Rollback();

                throw;
            }
            finally
            {
                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Dispose();
            }
        }

        private RetornoDocumento<int> EmitirCTeNFSePorObjetoAguardarConfirmacao(Dominio.ObjetosDeValor.CTe.CTeNFSe documento, string cnpjEmpresaAdministradora, string token, int codigoCTe = 0)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {

                if (documento == null)
                    return new RetornoDocumento<int>() { Mensagem = "O CT-e nao deve ser nulo para a integracao.", Status = false };

                if (documento.Emitente == null)
                    return new RetornoDocumento<int>() { Mensagem = "O Emitente nao pode ser nulo.", Status = false };

                if (documento.CodigoIBGECidadeInicioPrestacao == 0)
                    return new RetornoDocumento<int>() { Mensagem = "O código IBGE de inicio da prestação não foi enviado.", Status = false };

                if (documento.CodigoIBGECidadeTerminoPrestacao == 0)
                    return new RetornoDocumento<int>() { Mensagem = "O código IBGE de termino da prestação não foi enviado.", Status = false };

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(documento.Emitente.CNPJ));

                if (empresa == null)
                    return new RetornoDocumento<int>() { Mensagem = "A empresa (" + documento.Emitente.CNPJ + ") nao foi encontrada.", Status = false };

                if (empresa.Configuracao == null)
                    return new RetornoDocumento<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") nao esta configurada.", Status = false };

                string erros = string.Empty;

                Dominio.Enumeradores.TipoDocumento tipoDocumento = Dominio.Enumeradores.TipoDocumento.CTe;

                if (documento.CodigoIBGECidadeInicioPrestacao == documento.CodigoIBGECidadeTerminoPrestacao && !empresa.Configuracao.GerarCTeIntegracaoDocumentosMunicipais)
                    tipoDocumento = Dominio.Enumeradores.TipoDocumento.NFSe;

                Dominio.Entidades.EmpresaSerie empresaSerie = null;
                if (tipoDocumento == Dominio.Enumeradores.TipoDocumento.CTe)
                    erros = this.ValidarCTe(documento, empresa, cnpjEmpresaAdministradora, token, ref empresaSerie, unidadeDeTrabalho);
                else
                    erros = this.ValidarNFSe(documento, empresa, cnpjEmpresaAdministradora, token, unidadeDeTrabalho);

                if (!string.IsNullOrEmpty(erros))
                    return new RetornoDocumento<int>() { Mensagem = erros, Status = false };

                int tipoEnvio = string.IsNullOrWhiteSpace(empresa.Configuracao?.NumeroLoteEmissaoCTe) ? 0 : int.Parse(empresa.Configuracao.NumeroLoteEmissaoCTe);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = null;

                string retorno = string.Empty;

                if (tipoDocumento == Dominio.Enumeradores.TipoDocumento.CTe)
                {
                    Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                    string bloquearIntegracaoDuplicada = ConfigurationManager.AppSettings["BloquearIntegracaoDuplicada"];
                    if (bloquearIntegracaoDuplicada == "SIM" && documento.Documentos != null && documento.Documentos.Count > 0)
                    {
                        Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
                        Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                        string chaveNFe = documento.Documentos.FirstOrDefault().ChaveNFE;
                        if (chaveNFe != null)
                        {
                            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repDocumentosCTe.BuscarCTesPorChaveNFe(chaveNFe, empresa.Codigo);

                            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNota in listaCTes)
                            {
                                if (cteNota.Status == "A" || cteNota.Status == "R" || cteNota.Status == "E" || cteNota.Status == "P")
                                {
                                    Dominio.Entidades.IntegracaoCTe integracao = repIntegracaoCTe.BuscarPorCTe(cteNota.Codigo);

                                    if (integracao != null && !string.IsNullOrWhiteSpace(integracao.Arquivo) && JsonConvert.SerializeObject(documento) == integracao.Arquivo)
                                    {
                                        cteIntegrado = integracao.CTe;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (cteIntegrado == null)
                    {
                        Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unidadeDeTrabalho);
                        Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);
                        Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                        Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                        Dominio.Entidades.ModalTransporte modal = repModalTransporte.BuscarPorCodigo(1, false);
                        Dominio.Entidades.ModeloDocumentoFiscal modelo = repModeloDocumentoFiscal.BuscarPorModelo("57");
                        if (empresaSerie == null)
                            empresaSerie = repSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.CTe);
                        Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorNumero(5932);

                        if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                            unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                        else
                            unidadeDeTrabalho.Start();

                        Dominio.ObjetosDeValor.CTe.CTe documentoCTe = servicoCTe.ConverteObjetoCTeNFSe(documento);
                        cteIntegrado = servicoCTe.GerarCTeTemporarioIntegracao(documentoCTe, empresa, modal, modelo, empresaSerie, cfop, tipoEnvio, unidadeDeTrabalho);

                        if (!this.AdicionarRegistroCTeIntegrado(cteIntegrado, documento.NumeroCarga, documento.NumeroUnidade, documento.CodigoTipoOperacao, "", JsonConvert.SerializeObject(documento), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracao.Emissao, unidadeDeTrabalho, documento.CodigoControleInternoCliente, documento.Romaneio, documento.TipoVeiculo, documento.TipoCalculo, documento.ValorDespesa, Dominio.Enumeradores.StatusIntegracao.AguardandoConfirmacao))
                        {
                            unidadeDeTrabalho.Rollback();

                            return new RetornoDocumento<int>() { Mensagem = "Ocorreu uma falha e nao foi possivel adicionar o CT-e na lista de integracoes.", Status = false, Documento = "CTe" };
                        }

                        unidadeDeTrabalho.CommitChanges();

                        //Chamar fila para iniciar MultiCTe
                        this.AdicionarCTeNaFilaDeConsulta(null);
                    }
                    else if (cteIntegrado.Status == "R" || cteIntegrado.Status == "S")
                    {
                        if (ConfigurationManager.AppSettings["NaoRegerarCTeRejeitado"] == "SIM")
                        {
                            cteIntegrado.Status = "E";
                            repCTe.Atualizar(cteIntegrado);
                        }
                        else
                        {

                            if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                                unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                            else
                                unidadeDeTrabalho.Start();

                            if (servicoCTe.Deletar(cteIntegrado.Codigo, cteIntegrado.Empresa.Codigo, false, unidadeDeTrabalho))
                            {
                                Dominio.ObjetosDeValor.CTe.CTe documentoCTe = servicoCTe.ConverteObjetoCTeNFSe(documento);
                                cteIntegrado = servicoCTe.GerarCTePorObjeto(documentoCTe, cteIntegrado.Codigo, unidadeDeTrabalho, "1", tipoEnvio, "E", null, 0, null, empresa);

                                unidadeDeTrabalho.CommitChanges();
                            }
                            else
                            {
                                unidadeDeTrabalho.Rollback();

                                return new RetornoDocumento<int>() { Mensagem = "Nao foi possível integrar o CT-e, verifique o status do mesmo.", Status = false };
                            }
                        }
                    }

                    if (cteIntegrado.Status == "E")
                    {
                        if (!servicoCTe.Emitir(ref cteIntegrado, unidadeDeTrabalho))
                            retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.";

                        if (!this.AdicionarCTeNaFilaDeConsulta(cteIntegrado))
                            retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.";
                    }

                    if (cteIntegrado != null)
                        Servicos.Log.TratarErro("IntegrarDocumento - Retornado CodigoCTe: " + cteIntegrado.Codigo);
                    if (string.IsNullOrWhiteSpace(retorno))
                        return new RetornoDocumento<int>() { Mensagem = "Integracao realizada com sucesso.", Status = true, Objeto = cteIntegrado.Codigo, Documento = "CTe" };
                    else
                        return new RetornoDocumento<int>() { Mensagem = retorno, Status = false, Objeto = cteIntegrado.Codigo, Documento = "CTe" };
                }
                else //Emitir NFSe a partir do objeto CTe
                {
                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                    if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                    else
                        unidadeDeTrabalho.Start();

                    Dominio.Entidades.NFSe nfseIntegrada = svcNFSe.GerarNFSeTemporariaPorObjetoObjetoCTe(documento, unidadeDeTrabalho);

                    if (!this.AdicionarRegistroNFSeIntegrado(nfseIntegrada, documento.NumeroCarga, documento.NumeroUnidade, documento.CodigoTipoOperacao, "", JsonConvert.SerializeObject(documento), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao, unidadeDeTrabalho, documento.Romaneio, documento.TipoVeiculo, documento.TipoCalculo, documento.ValorDespesa, Dominio.Enumeradores.StatusIntegracao.AguardandoConfirmacao))
                    {
                        unidadeDeTrabalho.Rollback();

                        return new RetornoDocumento<int>() { Mensagem = "Ocorreu uma falha e nao foi possivel adicionar a NFS-e na lista de integracoes.", Status = false, Documento = "NFSe" };
                    }

                    unidadeDeTrabalho.CommitChanges();

                    if (string.IsNullOrWhiteSpace(retorno))
                        return new RetornoDocumento<int>() { Mensagem = "Integração realizada com sucesso.", Status = true, Objeto = nfseIntegrada.Codigo, Documento = "NFSe" };
                    else
                        return new RetornoDocumento<int>() { Mensagem = retorno, Status = false, Objeto = nfseIntegrada.Codigo, Documento = "NFSe" };
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("EmitirCTeNFSePorObjetoAguardarConfirmacao: " + ex);

                unidadeDeTrabalho.Rollback();

                throw;
            }
            finally
            {
                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Dispose();
            }
        }

        private RetornoDocumento<int> EmitirCTeNFSePorObjeto(Dominio.ObjetosDeValor.CTe.CTeNFSe documento, string cnpjEmpresaAdministradora, string token, int codigoCTe = 0)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {

                if (documento == null)
                    return new RetornoDocumento<int>() { Mensagem = "O CT-e nao deve ser nulo para a integracao.", Status = false };

                if (documento.Emitente == null)
                    return new RetornoDocumento<int>() { Mensagem = "O Emitente nao pode ser nulo.", Status = false };

                if (documento.CodigoIBGECidadeInicioPrestacao == 0)
                    return new RetornoDocumento<int>() { Mensagem = "O código IBGE de inicio da prestação não foi enviado.", Status = false };

                if (documento.CodigoIBGECidadeTerminoPrestacao == 0)
                    return new RetornoDocumento<int>() { Mensagem = "O código IBGE de termino da prestação não foi enviado.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(documento.Emitente.CNPJ));

                if (empresa == null)
                    return new RetornoDocumento<int>() { Mensagem = "Transportador (" + documento.Emitente.CNPJ + ") não possui cadastro.", Status = false };

                string erros = string.Empty;

                Dominio.Enumeradores.TipoDocumento tipoDocumento = Dominio.Enumeradores.TipoDocumento.CTe;

                if (documento.CodigoIBGECidadeInicioPrestacao == documento.CodigoIBGECidadeTerminoPrestacao && !empresa.Configuracao.GerarCTeIntegracaoDocumentosMunicipais)
                    tipoDocumento = Dominio.Enumeradores.TipoDocumento.NFSe;

                Dominio.Entidades.EmpresaSerie empresaSerie = null;
                if (tipoDocumento == Dominio.Enumeradores.TipoDocumento.CTe)
                    erros = this.ValidarCTe(documento, empresa, cnpjEmpresaAdministradora, token, ref empresaSerie, unidadeDeTrabalho);
                else
                    erros = this.ValidarNFSe(documento, empresa, cnpjEmpresaAdministradora, token, unidadeDeTrabalho);

                if (!string.IsNullOrEmpty(erros))
                    return new RetornoDocumento<int>() { Mensagem = erros, Status = false };

                int tipoEnvio = string.IsNullOrWhiteSpace(empresa.Configuracao?.NumeroLoteEmissaoCTe) ? 0 : int.Parse(empresa.Configuracao.NumeroLoteEmissaoCTe);

                //if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                //    unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                //else
                //    unidadeDeTrabalho.Start();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = null;

                string retorno = string.Empty;

                if (tipoDocumento == Dominio.Enumeradores.TipoDocumento.CTe)
                {
                    Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                    string bloquearIntegracaoDuplicada = ConfigurationManager.AppSettings["BloquearIntegracaoDuplicada"];
                    if (bloquearIntegracaoDuplicada == "SIM" && documento.Documentos != null && documento.Documentos.Count > 0)
                    {
                        Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
                        Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                        string chaveNFe = documento.Documentos.FirstOrDefault().ChaveNFE;
                        if (chaveNFe != null)
                        {
                            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repDocumentosCTe.BuscarCTesPorChaveNFe(chaveNFe, empresa.Codigo);

                            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNota in listaCTes)
                            {
                                if (cteNota.Status == "A" || cteNota.Status == "R" || cteNota.Status == "E" || cteNota.Status == "P" || string.IsNullOrWhiteSpace(cteNota.Status))
                                {
                                    Dominio.Entidades.IntegracaoCTe integracao = repIntegracaoCTe.BuscarPorCTe(cteNota.Codigo);

                                    if (integracao != null && !string.IsNullOrWhiteSpace(integracao.Arquivo) && JsonConvert.SerializeObject(documento) == integracao.Arquivo)
                                    {
                                        cteIntegrado = integracao.CTe;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (cteIntegrado == null)
                    {
                        Dominio.ObjetosDeValor.CTe.CTe documentoCTe = servicoCTe.ConverteObjetoCTeNFSe(documento);

                        if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                            unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                        else
                            unidadeDeTrabalho.Start();

                        cteIntegrado = servicoCTe.GerarCTePorObjeto(documentoCTe, codigoCTe, unidadeDeTrabalho, "1", tipoEnvio, "E", null, 0, null, empresa);

                        if (!this.AdicionarRegistroCTeIntegrado(cteIntegrado, documento.NumeroCarga, documento.NumeroUnidade, documento.CodigoTipoOperacao, "", JsonConvert.SerializeObject(documento), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracao.Emissao, unidadeDeTrabalho, documento.CodigoControleInternoCliente, documento.Romaneio, documento.TipoVeiculo, documento.TipoCalculo, documento.ValorDespesa))
                        {
                            unidadeDeTrabalho.Rollback();

                            return new RetornoDocumento<int>() { Mensagem = "Ocorreu uma falha e nao foi possivel adicionar o CT-e na lista de integracoes.", Status = false, Documento = "CTe" };
                        }

                        servicoCTe.SalvarIntegracaoRetornoCTe(cteIntegrado, empresa, unidadeDeTrabalho);

                        unidadeDeTrabalho.CommitChanges();
                    }
                    else if (cteIntegrado.Status == "R" || string.IsNullOrWhiteSpace(cteIntegrado.Status))
                    {
                        if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                            unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                        else
                            unidadeDeTrabalho.Start();

                        if (servicoCTe.Deletar(cteIntegrado.Codigo, cteIntegrado.Empresa.Codigo, false, unidadeDeTrabalho))
                        {
                            Dominio.ObjetosDeValor.CTe.CTe documentoCTe = servicoCTe.ConverteObjetoCTeNFSe(documento);
                            cteIntegrado = servicoCTe.GerarCTePorObjeto(documentoCTe, cteIntegrado.Codigo, unidadeDeTrabalho, "1", tipoEnvio, "E", null, 0, null, empresa);

                            unidadeDeTrabalho.CommitChanges();
                        }
                        else
                        {
                            unidadeDeTrabalho.Rollback();

                            return new RetornoDocumento<int>() { Mensagem = "Nao foi possível integrar o CT-e, verifique o status do mesmo.", Status = false };
                        }
                    }

                    if (cteIntegrado.Status == "E")
                    {
                        if (!servicoCTe.Emitir(ref cteIntegrado, unidadeDeTrabalho))
                            retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.";

                        if (!this.AdicionarCTeNaFilaDeConsulta(cteIntegrado))
                            retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.";
                    }

                    if (string.IsNullOrWhiteSpace(retorno))
                        return new RetornoDocumento<int>() { Mensagem = "Integracao realizada com sucesso.", Status = true, Objeto = cteIntegrado.Codigo, Documento = "CTe" };
                    else
                        return new RetornoDocumento<int>() { Mensagem = retorno, Status = false, Objeto = cteIntegrado.Codigo, Documento = "CTe" };
                }
                else //Emitir NFSe a partir do objeto CTe
                {
                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                    Dominio.Enumeradores.StatusNFSe statusNFS = Dominio.Enumeradores.StatusNFSe.Pendente;

                    // Configuração para gerar uma NFs Manual (vínculo entre MultiCTe -> SGT)
                    bool emiteFora = empresa.Configuracao?.EmiteNFSeForaEmbarcador ?? false;
                    if (emiteFora)
                        statusNFS = Dominio.Enumeradores.StatusNFSe.AgGeracaoNFSeManual;

                    if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                    else
                        unidadeDeTrabalho.Start();

                    Dominio.Entidades.NFSe nfseIntegrada = svcNFSe.GerarNFSePorObjetoObjetoCTe(documento, unidadeDeTrabalho, statusNFS);

                    if (!this.AdicionarRegistroNFSeIntegrado(nfseIntegrada, documento.NumeroCarga, documento.NumeroUnidade, documento.CodigoTipoOperacao, "", JsonConvert.SerializeObject(documento), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao, unidadeDeTrabalho, documento.Romaneio, documento.TipoVeiculo, documento.TipoCalculo, documento.ValorDespesa))
                    {
                        unidadeDeTrabalho.Rollback();

                        return new RetornoDocumento<int>() { Mensagem = "Ocorreu uma falha e nao foi possivel adicionar a NFS-e na lista de integracoes.", Status = false, Documento = "NFSe" };
                    }

                    unidadeDeTrabalho.CommitChanges();

                    if (nfseIntegrada.Status != Dominio.Enumeradores.StatusNFSe.AgGeracaoNFSeManual)
                    {
                        if (!svcNFSe.Emitir(nfseIntegrada, unidadeDeTrabalho))
                            retorno += "A NFS-e nº " + nfseIntegrada.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salva, porem, ocorreu uma falha ao emiti-la.";

                        if (!svcNFSe.AdicionarNFSeNaFilaDeConsulta(nfseIntegrada, unidadeDeTrabalho))
                            retorno += "A NFS-e nº " + nfseIntegrada.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salva, porem, nao foi possivel adiciona-la na fila de consulta.";
                    }

                    if (string.IsNullOrWhiteSpace(retorno))
                        return new RetornoDocumento<int>() { Mensagem = "Integração realizada com sucesso.", Status = true, Objeto = nfseIntegrada.Codigo, Documento = "NFSe" };
                    else
                        return new RetornoDocumento<int>() { Mensagem = retorno, Status = false, Objeto = nfseIntegrada.Codigo, Documento = "NFSe" };
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("EmitirCTeNFSePorObjeto: " + ex);

                unidadeDeTrabalho.Rollback();

                throw;
            }
            finally
            {
                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Dispose();
            }
        }

        private string ValidarCTe(Dominio.ObjetosDeValor.CTe.CTeNFSe cte, Dominio.Entidades.Empresa empresa, string cnpjEmpresaAdministradora, string token, ref Dominio.Entidades.EmpresaSerie empresaSerie, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            string exigirCadastroVeiculo = System.Configuration.ConfigurationManager.AppSettings["CTeExigeVeiculoCadastro"];
            string cadastrarVeiculoAutomaticamente = System.Configuration.ConfigurationManager.AppSettings["CadastrarVeiculoAutomaticamente"];

            if (empresa == null)
                return "A empresa (" + cte.Emitente.CNPJ + ") nao foi encontrada.";

            if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora))
                return "A empresa administradora (" + cnpjEmpresaAdministradora + ") nao esta vinculada ou nao pode emitir CT-es para esta empresa (" + empresa.CNPJ + ").";

            if (empresa.EmpresaPai.Configuracao != null && token != empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                return "Token de acesso invalido.";

            if (empresa.Status != "A")
                return "A empresa (" + empresa.CNPJ + ") esta inativa.";

            if (empresa.StatusFinanceiro == "B")
                return "A empresa (" + empresa.CNPJ + ") esta com pendencias, contate o setor de cadastros para maiores informacoes.";

            if (empresa.Configuracao == null)
                return "A empresa (" + empresa.CNPJ + ") nao esta configurada.";

            if (cte.Serie > 0)
            {
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
                empresaSerie = repEmpresaSerie.BuscarPorEmpresaNumeroTipo(empresa.Codigo, cte.Serie, Dominio.Enumeradores.TipoSerie.CTe);
                if (empresaSerie == null)
                    return "Serie " + cte.Serie.ToString() + " não configurada para empresa " + empresa.CNPJ + ".";
            }

            if (cte.Numero > 0)
            {
                if (cte.Serie == 0)
                    return "Obrigatório informar a série quando o número do CTe é informado.";

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                var cteAnterior = repCTe.BuscarPorNumeroESerie(empresa.Codigo, cte.Numero, cte.Serie, "57", empresa.TipoAmbiente);
                if (cteAnterior != null)
                    return "Já existe CTe com mesmo número " + cteAnterior.Numero + " e série " + cteAnterior.Serie.Numero + " para a empresa " + empresa.CNPJ + ".";
            }

            if (!string.IsNullOrWhiteSpace(cte.ChaveCTe))
            {
                if (!Utilidades.Validate.ValidarChave(cte.ChaveCTe))
                    return "Chave do CTe é inválida (" + cte.ChaveCTe + ").";
                if (!cte.ChaveCTe.Contains(empresa.CNPJ))
                    return "CNPJ do Emissor (" + empresa.CNPJ + ") é diferente do CNPJ da Chave do CTe (" + cte.ChaveCTe + ").";
            }

            if (empresa.Configuracao.SerieInterestadual == null || empresa.Configuracao.SerieIntraestadual == null)
                return "A empresa (" + empresa.CNPJ + ") nao possui uma serie configurada para a emissao de CT-e.";

            decimal valorLimiteFrete = empresa.Configuracao != null && empresa.Configuracao.ValorLimiteFrete > 0 ? empresa.Configuracao.ValorLimiteFrete : empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.ValorLimiteFrete > 0 ? empresa.EmpresaPai.Configuracao.ValorLimiteFrete : 0;
            if (valorLimiteFrete > 0 && (cte.ValorFrete > 0 || cte.ValorAReceber > 0))
            {
                if (cte.ValorAReceber > 0 && cte.ValorAReceber > valorLimiteFrete)
                    return "Valor a Receber do Frete R$" + cte.ValorAReceber.ToString("n2", cultura) + " é maior do que o valor limite permitido.";
                if (cte.ValorFrete > 0 && cte.ValorFrete > valorLimiteFrete)
                    return "Valor do Frete R$" + cte.ValorFrete.ToString("n2", cultura) + " é maior do que o valor limite permitido.";
            }

            StringBuilder erros = new StringBuilder();

            if (cte.Remetente == null)
                erros.Append("Remetente nao pode ser nulo. ");
            else if (cte.Remetente.CodigoAtividade <= 0)
                erros.Append("Atividade do remetente invalida. ");
            else if (string.IsNullOrWhiteSpace(cte.Remetente.CPFCNPJ))
                erros.Append("CNPJ do remetente invalido. ");

            if (cte.Destinatario == null)
                erros.Append("Destinatario nao pode ser nulo. ");
            else if (cte.Destinatario.CodigoAtividade <= 0)
                erros.Append("Atividade do destinatario invalida. ");
            else if (string.IsNullOrWhiteSpace(cte.Destinatario.CPFCNPJ))
                erros.Append("CNPJ do destinatario invalido. ");

            if (cte.TomadorDoCTe == null)
                erros.Append("Tomador nao pode ser nulo. ");
            else if (cte.TomadorDoCTe.CodigoAtividade <= 0)
                erros.Append("Atividade do tomador invalida. ");

            if (cte.Expedidor != null)
            {
                if (cte.Expedidor.CodigoAtividade <= 0)
                    erros.Append("Atividade do expedidor invalida. ");
                else if (string.IsNullOrWhiteSpace(cte.Expedidor.CPFCNPJ))
                    erros.Append("CNPJ do expedidor invalido. ");
            }

            if (cte.Recebedor != null)
            {
                if (cte.Recebedor.CodigoAtividade <= 0)
                    erros.Append("Atividade do recebedor invalida. ");
                else if (string.IsNullOrWhiteSpace(cte.Recebedor.CPFCNPJ))
                    erros.Append("CNPJ do recebedor invalido. ");
            }

            if (cte.CodigoIBGECidadeInicioPrestacao <= 0)
                erros.Append("Codigo da cidade de início da prestação invalido. ");

            if (cte.CodigoIBGECidadeTerminoPrestacao <= 0)
                erros.Append("Codigo da cidade de termino da prestação invalido. ");

            List<Dominio.Entidades.Veiculo> veiculosCadastrados = new List<Dominio.Entidades.Veiculo>();

            if (cte.Veiculos != null && cte.Veiculos.Count > 0)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                foreach (var veiculo in cte.Veiculos)
                {
                    if (string.IsNullOrWhiteSpace(veiculo.Placa) || veiculo.Placa.Length != 7)
                    {
                        erros.Append("A placa do veículo invalida. ");
                    }
                    else
                    {
                        Dominio.Entidades.Veiculo veiculoCadastrado = repVeiculo.BuscarPorPlaca(empresa.Codigo, veiculo.Placa);

                        if (cadastrarVeiculoAutomaticamente == "SIM" && veiculoCadastrado == null)
                        {
                            veiculoCadastrado = new Dominio.Entidades.Veiculo();
                            veiculoCadastrado.Ativo = true;
                            veiculoCadastrado.Placa = veiculo.Placa;
                            veiculoCadastrado.Empresa = empresa;
                            veiculoCadastrado.Estado = empresa.Localidade.Estado;
                            veiculo.Renavam = string.IsNullOrWhiteSpace(veiculo.Renavam) || veiculo.Renavam.Trim().Length < 9 || veiculo.Renavam.Trim().Length > 11 ? "111111111" : veiculo.Renavam.Trim();
                            veiculoCadastrado.Tipo = !string.IsNullOrWhiteSpace(veiculo.TipoPropriedade) ? veiculo.TipoPropriedade : "P";
                            veiculoCadastrado.TipoCarroceria = !string.IsNullOrWhiteSpace(veiculo.TipoCarroceria) ? veiculo.TipoCarroceria : "02";
                            veiculoCadastrado.TipoRodado = !string.IsNullOrWhiteSpace(veiculo.TipoRodado) ? veiculo.TipoRodado : "01";
                            veiculoCadastrado.TipoVeiculo = !string.IsNullOrWhiteSpace(veiculo.TipoVeiculo) ? veiculo.TipoVeiculo : "0";
                            veiculoCadastrado.Tara = veiculo.Tara > 0 && veiculo.Tara < 999999 ? veiculo.Tara : 1000;
                            veiculoCadastrado.CapacidadeKG = veiculo.CapacidadeKG > 0 && veiculo.CapacidadeKG < 999999 ? veiculo.CapacidadeKG : 10000;
                            veiculoCadastrado.CapacidadeM3 = veiculo.CapacidadeM3 > 0 && veiculo.CapacidadeM3 < 999 ? veiculo.CapacidadeM3 : 100;
                            repVeiculo.Inserir(veiculoCadastrado);
                        }

                        if (veiculoCadastrado == null)
                        {
                            if (exigirCadastroVeiculo == "SIM")
                                erros.Append("O veiculo " + veiculo.Placa + " não possui cadastro. ");
                            else
                            {
                                if (string.IsNullOrWhiteSpace(veiculo.Renavam) || veiculo.Renavam.Trim().Length < 9 || veiculo.Renavam.Trim().Length > 11)
                                    erros.Append("O RENAVAM do veiculo (" + veiculo.Placa + ") invalido. ");

                                if (string.IsNullOrWhiteSpace(veiculo.TipoCarroceria))
                                    erros.Append("O tipo de carroceria do veeculo (" + veiculo.Placa + ") invalido. ");

                                if (string.IsNullOrWhiteSpace(veiculo.TipoPropriedade))
                                    erros.Append("O tipo de proriedade do veiculo (" + veiculo.Placa + ") invalido. ");

                                if (string.IsNullOrWhiteSpace(veiculo.TipoRodado))
                                    erros.Append("O tipo de rodado do veiculo (" + veiculo.Placa + ") invalido. ");

                                if (string.IsNullOrWhiteSpace(veiculo.TipoVeiculo))
                                    erros.Append("O tipo de veiculo do veiculo (" + veiculo.Placa + ") invalido. ");

                                if (veiculo.Tara > 999999)
                                    erros.Append("A tara do veiculo invalida (deve possuir de 1 a 6 digitos). ");

                                if (veiculo.CapacidadeKG > 999999)
                                    erros.Append("A capacidade em KG do veiculo invalida (deve possuir de 1 a 6 digitos). ");

                                if (veiculo.CapacidadeM3 > 999)
                                    erros.Append("A capacidade em M3 do veiculo invalida (deve possuir de 1 a 3 digitos). ");
                            }
                        }
                        else
                        {
                            veiculosCadastrados.Add(veiculoCadastrado);
                        }
                    }
                }
            }

            //if (veiculosCadastrados.Count() <= 0 || !(from obj in veiculosCadastrados where !string.IsNullOrWhiteSpace(obj.CPFMotorista) && !string.IsNullOrWhiteSpace(obj.NomeMotorista) select obj).Any())
            //{
            //    if (cte.Motoristas == null || cte.Motoristas.Count <= 0)
            //    {
            //        erros.Append("Motorista obrigatorio. ");
            //    }
            //    else
            //    {
            //        foreach (var motorista in cte.Motoristas)
            //        {
            //            if (string.IsNullOrWhiteSpace(motorista.CPF) || motorista.CPF.Trim().Length != 11)
            //                erros.Append("O CPF do motorista (" + motorista.CPF + ") invalido. ");

            //            if (string.IsNullOrWhiteSpace(motorista.Nome))
            //                erros.Append("O nome do motorista invalido. ");
            //        }
            //    }
            //}

            if (cte.Motoristas != null && cte.Motoristas.Count > 0)
            {
                foreach (var motorista in cte.Motoristas)
                {
                    if (string.IsNullOrWhiteSpace(motorista.CPF) || motorista.CPF.Trim().Length != 11)
                        erros.Append("O CPF do motorista (" + motorista.CPF + ") invalido. ");

                    Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                    Dominio.Entidades.Usuario motoristaCadastrado = repMotorista.BuscarMotoristaPorCPF(empresa.Codigo, motorista.CPF);

                    if (motoristaCadastrado == null && string.IsNullOrWhiteSpace(motorista.Nome))
                        erros.Append("O nome do motorista invalido. ");
                }
            }


            if (cte.NumeroCarga <= 0)
                erros.Append("Numero da carga invalido. ");

            if (string.IsNullOrWhiteSpace(cte.ProdutoPredominante))
                erros.Append("Produto predominante invalido. ");

            if (empresa.EmpresaPai.Configuracao == null || empresa.EmpresaPai.Configuracao.UtilizaTabelaDeFrete == false)
                if (cte.ValorFrete <= 0)
                    erros.Append("Valor do frete invalido. ");

            if (cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
                if (cte.PercentualICMSIncluirNoFrete <= 0m)
                    erros.Append("Percentual de ICMS a incluir no frete invalido. ");

            if (cte.Emitente.OptanteSimplesNacional == null || cte.Emitente.OptanteSimplesNacional == false)
                if (cte.ICMS != null)
                    if (!(new List<string>() { "00", "20", "40", "41", "51", "60", "90", "91", "" }.Contains(cte.ICMS.CST)))
                        erros.Append("CST do ICMS invalida. ");

            if (cte.Documentos != null && cte.Documentos.Count > 0)
            {
                List<Dominio.Enumeradores.TipoDocumentoCTe> tipos = (from obj in cte.Documentos where obj != null select obj.Tipo).Distinct().ToList();

                if (tipos.Count() > 1)
                {
                    erros.Append("Somente um tipo de documento deve ser informado para o CT-e. ");
                }
                else
                {
                    foreach (var documento in cte.Documentos)
                    {
                        if (documento == null)
                        {
                            erros.Append("O documento nao pode ser nulo. ");
                        }
                        else
                        {
                            DateTime data;
                            int inteiro;

                            if (string.IsNullOrWhiteSpace(documento.ChaveNFE) && (string.IsNullOrWhiteSpace(documento.Numero) || !int.TryParse(documento.Numero, out inteiro) || inteiro <= 0))
                                erros.Append("O numero do documento nao pode ser nulo e deve ser maior que zero. ");

                            if (string.IsNullOrWhiteSpace(documento.ChaveNFE) && (string.IsNullOrWhiteSpace(documento.DataEmissao) || !DateTime.TryParseExact(documento.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data)))
                                erros.Append("A data de emissao do documento invalida ou esta no formato incorreto (dd/MM/yyyy HH:mm:ss). ");

                            //if (documento.Valor <= 0m) removido porcausa da APTI
                            //    erros += "O valor do documento deve ser maior que zero. ";

                            if (documento.Tipo == Dominio.Enumeradores.TipoDocumentoCTe.NFe)
                            {
                                if (string.IsNullOrWhiteSpace(documento.ChaveNFE) || documento.ChaveNFE.Length != 44)
                                    erros.Append("A chave da NF-e (" + documento.ChaveNFE + ") nao pode ser nula e deve possuir 44 dígitos. ");
                            }
                            else if (documento.Tipo == Dominio.Enumeradores.TipoDocumentoCTe.NF)
                            {
                                if (string.IsNullOrWhiteSpace(documento.ModeloDocumentoFiscal) || (documento.ModeloDocumentoFiscal != "01" && documento.ModeloDocumentoFiscal != "04"))
                                    erros.Append("O modelo do documento (" + documento.ModeloDocumentoFiscal + ") invalido (01 ou 04). ");

                                if (string.IsNullOrWhiteSpace(documento.Serie) || documento.Serie.Length > 3)
                                    erros.Append("A serie do documento (" + documento.Serie + ") invalida. ");

                                if (string.IsNullOrWhiteSpace(documento.CFOP) || documento.CFOP.Length != 4)
                                    erros.Append("A CFOP do documento (" + documento.CFOP + ") invalida. ");

                                if (documento.Peso <= 0m)
                                    erros.Append("O peso do documento (" + documento.Peso.ToString("n2") + ") invalido. ");
                            }
                            else if (documento.Tipo == Dominio.Enumeradores.TipoDocumentoCTe.Outros)
                            {
                                if (string.IsNullOrWhiteSpace(documento.ModeloDocumentoFiscal) || (documento.ModeloDocumentoFiscal != "00" && documento.ModeloDocumentoFiscal != "99"))
                                    erros.Append("O modelo do documento (" + documento.ModeloDocumentoFiscal + ") invalido (00 ou 99). ");

                                if (string.IsNullOrWhiteSpace(documento.Descricao))
                                    erros.Append("A descricao do documento nao pode ser vazia ou nula. ");
                            }
                        }
                    }
                }
            }
            else
            {
                erros.Append("Nenhum documento informado. ");
            }

            if (cte.QuantidadesCarga != null && cte.QuantidadesCarga.Count > 0)
            {
                List<string> listaUnidades = new List<string>() { "00", "01", "02", "03", "04", "05" };

                foreach (var quantidade in cte.QuantidadesCarga)
                {
                    if (quantidade == null)
                    {
                        erros.Append("A quantidade da carga nao pode ser nula. ");
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(quantidade.Descricao))
                            erros.Append("A descricao da quantidade da carga nao pode ser vazia ou nula. ");

                        if (quantidade.Quantidade <= 0m)
                            erros.Append("A quantidade da quantidade da carga deve ser maior que zero. ");

                        if (!listaUnidades.Contains(quantidade.UnidadeMedida))
                            erros.Append("A unidade de medida (" + quantidade.UnidadeMedida + ") da quantidade da carga invalida. ");
                    }
                }
            }
            else
            {
                erros.Append("Nenhuma quantidade da carga informada. ");
            }

            if (cte.Seguros != null && cte.Seguros.Count > 0)
            {
                foreach (var seguro in cte.Seguros)
                {
                    if (seguro == null)
                    {
                        erros.Append("O seguro nao pode ser nulo. ");
                    }
                    else
                    {
                        if (seguro.NomeSeguradora != null && seguro.NomeSeguradora.Length > 30)
                            erros.Append("O nome da seguradora (" + seguro.NomeSeguradora + ") do seguro deve possuir ate 30 caracteres. ");

                        if (seguro.NumeroApolice != null && seguro.NumeroApolice.Length > 20)
                            erros.Append("O número da apolice (" + seguro.NumeroApolice + ") do seguro deve possuir ate 20 caracteres. ");

                        if (seguro.NumeroAverbacao != null && seguro.NumeroAverbacao.Length > 20)
                            erros.Append("O número da averbacao (" + seguro.NumeroAverbacao + ") do seguro deve possuir ate 20 caracteres. ");
                    }
                }
            }
            //Não é mais obrigatório na versão 3.0
            //else
            //{
            //    erros.Append("Seguro obrigatorio. ");
            //}

            return erros.ToString();
        }

        private string ValidarNFSe(Dominio.ObjetosDeValor.CTe.CTeNFSe nfse, Dominio.Entidades.Empresa empresa, string cnpjEmpresaAdministradora, string token, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (empresa == null)
                return "A empresa (" + nfse.Emitente.CNPJ + ") nao foi encontrada.";

            if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora))
                return "A empresa administradora (" + cnpjEmpresaAdministradora + ") nao esta vinculada ou nao pode emitir NFS-es para esta empresa (" + empresa.CNPJ + ").";

            if (empresa.EmpresaPai.Configuracao != null && token != empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                return "Token de acesso inválido.";

            if (empresa.Status != "A")
                return "A empresa (" + empresa.CNPJ + ") esta inativa.";

            if (empresa.StatusFinanceiro == "B")
                return "A empresa (" + empresa.CNPJ + ") esta com pendencias, contate o setor de cadastros para maiores informacoes.";

            if (empresa.Configuracao == null)
                return "A empresa (" + empresa.CNPJ + ") nao esta configurada.";

            if (string.IsNullOrWhiteSpace(empresa.Configuracao.SerieRPSNFSe))
                return "A empresa (" + empresa.CNPJ + ") nao possui uma serie de RPS configurada para a emissão de NFS-e.";

            if (empresa.Configuracao.SerieNFSe == null)
                return "A empresa (" + empresa.CNPJ + ") nao possui uma serie configurada para a emissao de NFS-e.";

            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
            Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, empresa.Localidade.CodigoIBGE != nfse.CodigoIBGECidadeInicioPrestacao, nfse.CodigoIBGECidadeInicioPrestacao, unidadeDeTrabalho);
            Dominio.Entidades.NaturezaNFSe naturezaMultiCTe = servicoNFSe.ObterNaturezaNFSe(empresa, empresa.Localidade.CodigoIBGE != nfse.CodigoIBGECidadeInicioPrestacao, nfse.CodigoIBGECidadeInicioPrestacao, unidadeDeTrabalho);

            if (naturezaMultiCTe == null)
                return "A empresa (" + empresa.CNPJ + ") nao possui uma natureza padrao configurada para a emissao de NFS-e.";

            if (servicoMultiCTe == null)
                return "A empresa (" + empresa.CNPJ + ") nao possui um servico padrao configurado para a emissao de NFS-e.";

            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

            //list<dominio.entidades.nfse> nfsespendentes = repnfse.buscarnfsespendentes(empresa.codigo);

            //if (nfsespendentes.count > 0)
            //{
            //    string r = "existem nfs-es pendentes para a empresa (" + empresa.cnpj + "), não sendo possível emitir uma nova nfs-e até que as mesmas sejam autorizadas, canceladas ou excluídas. protocolos: ";

            //    foreach (dominio.entidades.nfse nfsependente in nfsespendentes)
            //        r += nfsependente.codigo.tostring() + ", ";

            //    return r.substring(0, r.length - 2) + ".";
            //}

            StringBuilder erros = new StringBuilder();

            if (nfse.ValorFrete <= 0)
                erros.Append("Valor para emissão de NFSe invalido. ");

            if (servicoMultiCTe != null && ConfigurationManager.AppSettings["ValidarAliquotaISSIntegracaoNFSe"] == "SIM")
            {
                if (!string.IsNullOrWhiteSpace(nfse.ISSNFSeRetido))
                {
                    if ((servicoMultiCTe.ISSRetido && nfse.ISSNFSeRetido.ToUpper() != "SIM") || (!servicoMultiCTe.ISSRetido && nfse.ISSNFSeRetido.ToUpper() != "NAO"))
                        erros.Append("Informação da retenção de ISS esta diferente do configurado. Verifique e reenvie a integração. ");
                }

                if (nfse.ICMS != null)
                {
                    if (nfse.ICMS.Aliquota != servicoMultiCTe.Aliquota)
                        erros.Append("Alíquota ISS enviada na integração (" + nfse.ICMS.Aliquota.ToString() + ") divergente da cadastrada no MultiCTe  (" + servicoMultiCTe.Aliquota.ToString() + "). ");
                }
            }

            return erros.ToString();
        }

        private bool AdicionarRegistroCTeIntegrado(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, int numeroDaCarga, int numeroDaUnidade, string tipoOperacao, string nomeArquivo, string arquivo, Dominio.Enumeradores.TipoArquivoIntegracao tipoArquivo, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho = null, string codigoControleInternoCliente = null, string romaneio = null, string tipoVeiculo = null, string tipoCalculo = null, decimal valorDespesa = 0, Dominio.Enumeradores.StatusIntegracao status = Dominio.Enumeradores.StatusIntegracao.Pendente)
        {
            try
            {
                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.IntegracaoCTe integracao = new Dominio.Entidades.IntegracaoCTe();

                integracao.CTe = cte;
                integracao.Arquivo = arquivo;
                integracao.NumeroDaCarga = numeroDaCarga;
                integracao.NumeroDaUnidade = numeroDaUnidade;
                integracao.Status = status;
                integracao.TipoArquivo = tipoArquivo;
                integracao.NomeArquivo = nomeArquivo;
                integracao.Tipo = tipoIntegracao;
                integracao.CodigoControleInternoCliente = codigoControleInternoCliente;
                integracao.CodigoTipoOperacao = tipoOperacao;

                integracao.Romaneio = romaneio;
                integracao.TipoVeiculo = tipoVeiculo;
                integracao.TipoCalculo = tipoCalculo;
                integracao.ValorDespesa = valorDespesa;
                integracao.GerouCargaEmbarcador = false;

                repIntegracao.Inserir(integracao);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private bool AdicionarRegistroNFSeIntegrado(Dominio.Entidades.NFSe nfse, int numeroDaCarga, int numeroDaUnidade, string nomeArquivo, string tipoOperacao, string arquivo, Dominio.Enumeradores.TipoArquivoIntegracao tipoArquivo, Dominio.Enumeradores.TipoIntegracaoNFSe tipoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho, string romaneio = null, string tipoVeiculo = null, string tipoCalculo = null, decimal valorDespesa = 0, Dominio.Enumeradores.StatusIntegracao status = Dominio.Enumeradores.StatusIntegracao.Pendente)
        {
            try
            {
                Repositorio.IntegracaoNFSe repIntegracao = new Repositorio.IntegracaoNFSe(unidadeDeTrabalho);

                Dominio.Entidades.IntegracaoNFSe integracao = new Dominio.Entidades.IntegracaoNFSe();

                integracao.NFSe = nfse;
                integracao.Arquivo = arquivo;
                integracao.Status = status;
                integracao.TipoArquivo = tipoArquivo;
                integracao.NomeArquivo = nomeArquivo;
                integracao.Tipo = tipoIntegracao;
                integracao.NumeroDaCarga = numeroDaCarga;
                integracao.NumeroDaUnidade = numeroDaUnidade;
                integracao.CodigoTipoOperacao = tipoOperacao;

                integracao.Romaneio = romaneio;
                integracao.TipoVeiculo = tipoVeiculo;
                integracao.TipoCalculo = tipoCalculo;
                integracao.ValorDespesa = valorDespesa;
                integracao.GerouCargaEmbarcador = false;

                repIntegracao.Inserir(integracao);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private bool AdicionarCTeNaFilaDeConsulta(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            try
            {
                if (cte == null || cte.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    return true;

                string postData = "CodigoCTe=" + cte.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["WebServiceConsultaCTe"], "IntegracaoCTe/AdicionarNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = (System.Collections.Generic.Dictionary<string, object>)serializer.DeserializeObject(result);

                return (bool)retorno["Sucesso"];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private string ObterPDFCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho, string codificarUTF8 = "S")
        {
            if (cte.Status.Equals("A") && tipoIntegracao == Dominio.Enumeradores.TipoIntegracao.Emissao)
            {
                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                return servicoCTe.ObterDACTE(cte.Codigo, cte.Empresa.Codigo, unidadeDeTrabalho, codificarUTF8 == "S");
            }
            else
            {
                return string.Empty;
            }
        }

        private string ValidarNFSeProcessada(Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfse, Dominio.Entidades.Empresa empresa, string cnpjEmpresaAdministradora, string token, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Localidade repLocalidades = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            StringBuilder erros = new StringBuilder();

            if (empresa == null)
                return "A empresa (" + nfse.Emitente.CNPJ + ") nao foi encontrada.";

            if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora))
                erros.Append("A empresa administradora (" + cnpjEmpresaAdministradora + ") nao esta vinculada ou nao pode emitir NFS-es para esta empresa (" + empresa.CNPJ + ").");

            if (empresa.EmpresaPai.Configuracao != null && token != empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                erros.Append("Token de acesso invalido.");

            if (empresa.Status != "A")
                erros.Append("A empresa (" + empresa.CNPJ + ") esta inativa.");

            if (empresa.StatusFinanceiro == "B")
                erros.Append("A empresa (" + empresa.CNPJ + ") esta com pendencias, contate o setor de cadastros para maiores informacoes.");

            if (empresa.Configuracao == null)
                erros.Append("A empresa (" + empresa.CNPJ + ") nao esta configurada.");

            if (nfse.Natureza == null)
                erros.Append("NFS-e nao possui uma natureza enviada.");

            if (nfse.Itens == null || nfse.Itens.Count == 0)
                erros.Append("NFS-e nao possui item.");
            else
            {
                if (nfse.Itens.FirstOrDefault().Servico == null)
                    erros.Append("NFS-e nao possui um servico.");

                if (nfse.Itens.FirstOrDefault().CodigoIBGECidade <= 0)
                    erros.Append("IBGE da cidade do item esta invalido.");
                else if (repLocalidades.BuscarPorCodigoIBGE(nfse.Itens.FirstOrDefault().CodigoIBGECidade) == null)
                    erros.Append("Cidade do item não localizada pelo IBGE " + nfse.Itens.FirstOrDefault().CodigoIBGECidade + ".");

                if (nfse.Itens.FirstOrDefault().CodigoIBGECidadeIncidencia <= 0)
                    erros.Append("IBGE da cidade de incidência do item esta invalido.");
                else if (repLocalidades.BuscarPorCodigoIBGE(nfse.Itens.FirstOrDefault().CodigoIBGECidadeIncidencia) == null)
                    erros.Append("Cidade de incidência do item não localizada pelo IBGE " + nfse.Itens.FirstOrDefault().CodigoIBGECidadeIncidencia + ".");
            }

            if (string.IsNullOrWhiteSpace(nfse.Serie))
                erros.Append("NFS-e nao possui uma serie.");

            if (nfse.Numero <= 0 && nfse.Status != "PEN")
                erros.Append("NFS-e nao possui um numero.");

            if (nfse.CodigoIBGECidadePrestacaoServico <= 0)
                erros.Append("IBGE da cidade de prestacao esta invalido.");
            else if (repLocalidades.BuscarPorCodigoIBGE(nfse.CodigoIBGECidadePrestacaoServico) == null)
                erros.Append("Cidade Prestacao de serviço não localizada pelo IBGE " + nfse.CodigoIBGECidadePrestacaoServico + ".");

            if (nfse.Tomador == null || string.IsNullOrWhiteSpace(nfse.Tomador.CPFCNPJ))
                erros.Append("NFS-e nao possui um cliente Tomador.");
            else
            {
                if (string.IsNullOrWhiteSpace(nfse.Tomador.RazaoSocial)) //Se não são enviados os dados exige ter cadastro.
                {
                    Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(double.Parse(nfse.Tomador.CPFCNPJ));
                    if (tomador == null)
                        erros.Append("Cliente tomador " + nfse.Tomador.CPFCNPJ + " não possui cadastro.");
                }
            }

            if (nfse.Documentos != null && nfse.Documentos.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.NFSe.Documentos documento in nfse.Documentos)
                {
                    if (documento.EmitenteNFe != null && !string.IsNullOrWhiteSpace(documento.EmitenteNFe.CPFCNPJ))
                    {
                        if (string.IsNullOrWhiteSpace(documento.EmitenteNFe.RazaoSocial)) //Se não são enviados os dados exige ter cadastro.
                        {
                            Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(documento.EmitenteNFe.CPFCNPJ));
                            if (emitente == null)
                                erros.Append("Cliente emitente " + documento.EmitenteNFe.CPFCNPJ + " do documento " + documento.ChaveNFE + " não possui cadastro.");
                        }
                    }
                    if (documento.DestinatarioNFe != null && !string.IsNullOrWhiteSpace(documento.DestinatarioNFe.CPFCNPJ))
                    {
                        if (string.IsNullOrWhiteSpace(documento.DestinatarioNFe.RazaoSocial)) //Se não são enviados os dados exige ter cadastro.
                        {
                            Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(documento.DestinatarioNFe.CPFCNPJ));
                            if (destinatario == null)
                                erros.Append("Cliente destino " + documento.DestinatarioNFe.CPFCNPJ + " do documento " + documento.ChaveNFE + " não possui cadastro.");
                        }
                    }
                }
            }

            return erros.ToString();
        }

        #endregion
    }
}
