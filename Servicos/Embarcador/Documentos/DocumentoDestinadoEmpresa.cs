using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Documentos;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using ICSharpCode.SharpZipLib.Zip;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Documentos
{
    public class DocumentoDestinadoEmpresa : ServicoBase
    {
        public DocumentoDestinadoEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Metodos Publicos

        public static Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoConsultaStatusSefaz ObterDocumentoDestinadoEmpresaPorChave(string chave, List<string> cnpjsNaoImportarTransporte, int codigoEmpresa, string stringConexao, string caminhoDocumentosFiscais = null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);

            var retorno = new Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoConsultaStatusSefaz()
            {
                HouveConsula = false,
                DocumentoCancelado = false,
                Mensagem = "Não foi possível efetuar a consulta na SEFAZ"
            };

            if (string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                caminhoDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            var documento = repDocumentoDestinadoEmpresa.BuscarNFePorChave(chave);

            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
            {
                retorno.HouveConsula = false;
                retorno.Mensagem = "Certificado não encontrado";
                return retorno;
            }

            string cStat = "";
            string motivo = "";

            MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeInt retornoSefazNFe = null;

            if (chave.Substring(20, 2) == "55") //NFe
            {
                try
                {
                    retornoSefazNFe = ConsultarDocumentosFiscaisNFe(unidadeTrabalho, empresa.CNPJ, 0, (MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TCodUfIBGE)empresa.Localidade.Estado.CodigoIBGE, empresa.NomeCertificado, empresa.SenhaCertificado, empresa.NomeCertificadoKeyVault, false, (int)empresa.TipoAmbiente, chave);
                }
                catch (ServicoException ex)
                {
                    Log.GravarAdvertencia(ex.Message);
                    retorno.HouveConsula = false;
                    retorno.Mensagem = ex.Message;
                    return retorno;
                }
                catch (Exception ex)
                {
                    retorno.HouveConsula = false;
                    retorno.Mensagem = ex.Message;
                    return retorno;
                }

                cStat = retornoSefazNFe?.cStat ?? "";
                motivo = retornoSefazNFe?.xMotivo ?? "";
            }
            else // CTe
            {
                var NSU = documento?.NumeroSequencialUnico ?? 0;
                if (NSU > 0)
                {
                    MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeInt retornoSefazCTe = null;

                    try
                    {
                        Dominio.Enumeradores.TipoAmbiente tipoAmbiente = empresa.TipoAmbiente;
                        retornoSefazCTe = ConsultarDocumentosFiscaisCTe(unidadeTrabalho, empresa.CNPJ, NSU,
                            tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.TAmb.Item1 : MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.TAmb.Item2,
                            (MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.TCodUfIBGE)empresa.Localidade.Estado.CodigoIBGE, empresa.NomeCertificado, empresa.NomeCertificadoKeyVault,
                            empresa.SenhaCertificado, tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? empresa.Localidade.Estado.SefazCTe.UrlDistribuicaoDFe : empresa.Localidade.Estado.SefazCTeHomologacao.UrlDistribuicaoDFe, true);
                    }
                    catch (ServicoException ex)
                    {
                        Log.GravarAdvertencia(ex.Message);
                        retorno.HouveConsula = false;
                        retorno.Mensagem = ex.Message;
                        return retorno;
                    }
                    catch (Exception ex)
                    {
                        retorno.HouveConsula = false;
                        retorno.Mensagem = ex.Message;
                        return retorno;
                    }

                    cStat = retornoSefazCTe?.cStat ?? "";
                    motivo = retornoSefazCTe?.xMotivo ?? "";

                    //ajuste do objeto CTeDistribuicaoDFe para NFeDistribuicaoDFe, objeto é o "mesmo" reaproveitando os mesmos metodos de DFe
                    if (retornoSefazCTe != null)
                    {
                        retornoSefazNFe = new MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeInt();
                        retornoSefazNFe.loteDistDFeInt = new MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeInt();
                        retornoSefazNFe.loteDistDFeInt.docZip = new List<MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip>().ToArray();
                        var listaZip = retornoSefazCTe.loteDistDFeInt.docZip;
                        if (listaZip != null)
                        {
                            for (var i = 0; i < listaZip.Length; i++)
                            {
                                retornoSefazNFe.loteDistDFeInt.docZip[i].NSU = listaZip[i].NSU;
                                retornoSefazNFe.loteDistDFeInt.docZip[i].Value = listaZip[i].Value;
                                retornoSefazNFe.loteDistDFeInt.docZip[i].schema = listaZip[i].schema;
                            }
                        }
                    }
                }
                else // Cte só possui consulta na sefaz por NSU, não possui por chave
                {
                    retorno.HouveConsula = false;
                    retorno.Mensagem = "Não é possivel consultar o status do CT-e informado, NSU do CT-e não encontrado no repositório.";
                }
            }

            if (retornoSefazNFe != null)
            {
                retorno.HouveConsula = true;
                if (cStat == "138")
                {
                    retorno.Mensagem = "O status do documento é: AUTORIZADO";

                    if (retornoSefazNFe.loteDistDFeInt != null)
                    {

                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                        Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unidadeTrabalho);
                        Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto repDocumentoDestinadoEmpresaProduto = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto(unidadeTrabalho);
                        Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unidadeTrabalho);
                        Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
                        Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeTrabalho);
                        Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unidadeTrabalho);

                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();

                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };

                        if (retornoSefazNFe.loteDistDFeInt != null && retornoSefazNFe.loteDistDFeInt.docZip != null)
                        {
                            foreach (var doc in retornoSefazNFe.loteDistDFeInt.docZip)
                            {
                                ProcessarDocumentosDestinados(doc, codigoEmpresa, configuracaoTMS, unidadeTrabalho, empresa, cnpjsNaoImportarTransporte, caminhoDocumentosFiscais, tipoServicoMultisoftware,
                                repVeiculo, repDocumentoDestinadoEmpresaProduto, repTabelaValores, repAbastecimento, repVeiculoMotorista, configuracaoAbastecimento, auditado);
                            }
                        }

                    }
                }
                else if (cStat == "137")
                {
                    retorno.Mensagem = "Nenhum documento localizado na SEFAZ com a chave informada.";
                }
                else if (cStat == "653")
                {
                    retorno.Mensagem = "O status do documento é: CANCELADO";
                    retorno.DocumentoCancelado = true;
                    //Atualizar a nota como cancelada
                    if (documento != null && documento.Cancelado == false)
                    {
                        documento.Cancelado = true;
                        repDocumentoDestinadoEmpresa.Atualizar(documento);
                        new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento,
                            documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoNFe : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe);
                    }
                }
                else
                {
                    retorno.Mensagem = cStat + " " + motivo;
                }
            }

            return retorno;
        }

        public static bool ObterDocumentosDestinadosEmpresa(int codigoEmpresa, string stringConexao, List<string> cnpjsNaoImportarTransporte, out string msgErro, out string codigoStatusRetornoSefaz, long numeroNSU = 0, string caminhoDocumentosFiscais = null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null, string chaveAcesso = null, bool utilizaWebConfig = false)
        {
            msgErro = string.Empty;
            codigoStatusRetornoSefaz = string.Empty;

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);

            if (string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                caminhoDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto repDocumentoDestinadoEmpresaProduto = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto(unidadeTrabalho);
            Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unidadeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeTrabalho);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado repConfigDocumentoDestinado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado configuracaoDocumentoDestinado = repConfigDocumentoDestinado.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };

            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
            {
                msgErro = "Empresa ou Certificado não encontrado";
                return false;
            }

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa configuracaoDocumentoDestinadoEmpresa = repConfiguracaoDocumentoDestinadoEmpresa.BuscarPorEmpresa(codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe);

            if (configuracaoDocumentoDestinadoEmpresa == null)
            {
                configuracaoDocumentoDestinadoEmpresa = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa()
                {
                    Empresa = empresa,
                    ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe,
                    EmConsulta = true
                };

                repConfiguracaoDocumentoDestinadoEmpresa.Inserir(configuracaoDocumentoDestinadoEmpresa);
            }
            else
            {
                configuracaoDocumentoDestinadoEmpresa.EmConsulta = true;
                repConfiguracaoDocumentoDestinadoEmpresa.Atualizar(configuracaoDocumentoDestinadoEmpresa);
            }

            long ultimoNSU = 0;
            long maxNSU = 0L;

            if (numeroNSU > 0)
            {
                ultimoNSU = numeroNSU;
                maxNSU = numeroNSU;
            }
            else
            {
                //comentado este trecho, ocorre timeout quando tem muitos registros
                //ultimoNSU = repDocumentoDestinadoEmpresa.BuscarUltimoNSUEmpresa(codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe);
                //if (ultimoNSU < configuracaoDocumentoDestinadoEmpresa.UltimoNSU)
                ultimoNSU = configuracaoDocumentoDestinadoEmpresa.UltimoNSU;
            }

            MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeInt retorno = null;

            do
            {
                try
                {
                    if (utilizaWebConfig)
                        retorno = MultiSoftware.NFe.NFeDistribuicaoDFe.Servicos.DistribuicaoDFe.ConsultarDocumentosFiscais(empresa.CNPJ, ultimoNSU, (MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TCodUfIBGE)empresa.Localidade.Estado.CodigoIBGE, empresa.NomeCertificado, empresa.SenhaCertificado, numeroNSU > 0 ? true : false, empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? 1 : 2);
                    else
                        retorno = ConsultarDocumentosFiscaisNFe(unidadeTrabalho, empresa.CNPJ, ultimoNSU, (MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TCodUfIBGE)empresa.Localidade.Estado.CodigoIBGE, empresa.NomeCertificado, empresa.SenhaCertificado, empresa.NomeCertificadoKeyVault, numeroNSU > 0 ? true : false, empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? 1 : 2, chaveAcesso);

                    codigoStatusRetornoSefaz = retorno.cStat;
                }
                catch (ServicoException ex)
                {
                    Log.GravarAdvertencia(ex.Message);
                    msgErro = ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    msgErro = ex.Message;
                    return false;
                }

                if (numeroNSU <= 0)
                {
                    maxNSU = long.Parse(retorno.maxNSU);
                    ultimoNSU = long.Parse(retorno.ultNSU);
                }

                if (retorno.cStat == "138")
                {
                    foreach (MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe in retorno.loteDistDFeInt.docZip)
                    {
                        Int64 iddocumentoxml = 0;
                        string xmlDocumento = string.Empty;

                        try
                        {
                            if (numeroNSU <= 0 && repDocumentoDestinadoEmpresa.ExistePorNSU(empresa.Codigo, long.Parse(dfe.NSU), Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe))
                                continue;

                            using (MemoryStream compressedStream = new MemoryStream(dfe.Value))
                            {
                                using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                                {
                                    using (MemoryStream resultStream = new MemoryStream())
                                    {
                                        zipStream.CopyTo(resultStream);

                                        resultStream.Position = 0;

                                        StreamReader reader = new StreamReader(resultStream);
                                        xmlDocumento = reader.ReadToEnd();

                                        GravarXmlDistribucaoDFe(empresa, null, dfe, null, resultStream, SituacaoXml.Importado, unidadeTrabalho, xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha, out iddocumentoxml);

                                        resultStream.Position = 0;

                                        if (dfe.schema == "resNFe_v1.00.xsd")
                                        {
                                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe));

                                            MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe resNFe = (ser.Deserialize(resultStream) as MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe);

                                            SalvarResNFe(dfe, resNFe, codigoEmpresa, unidadeTrabalho, configuracaoTMS);
                                        }
                                        else if (dfe.schema == "procNFe_v3.10.xsd")
                                        {
                                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc));

                                            string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());

                                            MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfeProc = (ser.Deserialize(resultStream) as MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc);
                                            string placaObservacao = "";
                                            string kmObservacao = "";
                                            string horimetroObservacao = "";
                                            string chassiObservacao = "";

                                            if (SalvarProcNFe(dfe, nfeProc, empresa, unidadeTrabalho, cnpjsNaoImportarTransporte, out Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado, out kmObservacao, out placaObservacao, out horimetroObservacao, out chassiObservacao))
                                            {
                                                try
                                                {
                                                    resultStream.Position = 0;

                                                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "NFe", "nfeProc");

                                                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, nfeProc.protNFe.infProt.chNFe + ".xml");

                                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());

                                                    Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica servicoNotaFiscal = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica();

                                                    servicoNotaFiscal.GerarRegistroNotaFiscal(nfeProc, unidadeTrabalho, empresa);

                                                    if (configuracaoTMS.CriarNotaFiscalTransportePorDocumentoDestinado)
                                                        CriarXMLNotaFiscal(documentoDestinado, unidadeTrabalho, tipoServicoMultisoftware);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Servicos.Log.TratarErro("Não foi possível salvar a NF-e v3.00 da consulta de documentos destinados: " + ex.ToString());
                                                }
                                            }
                                        }
                                        else if (dfe.schema == "procNFe_v4.00.xsd")
                                        {
                                            try
                                            {
                                                XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc));

                                                string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());
                                                string placaObservacao = "";
                                                string kmObservacao = "";
                                                string horimetroObservacao = "";
                                                string chassiObservacao = "";

                                                MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfeProc = (ser.Deserialize(resultStream) as MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc);
                                                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica servicoNotaFiscal = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica();

                                                if (SalvarProcNFe(dfe, nfeProc, empresa, unidadeTrabalho, cnpjsNaoImportarTransporte, tipoServicoMultisoftware, out Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado, out kmObservacao, out placaObservacao, out horimetroObservacao, out chassiObservacao))
                                                {
                                                    try
                                                    {
                                                        resultStream.Position = 0;

                                                        string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "NFe", "nfeProc");

                                                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, nfeProc.protNFe.infProt.chNFe + ".xml");

                                                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                                            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());

                                                        servicoNotaFiscal.GerarRegistroNotaFiscal(nfeProc, unidadeTrabalho, empresa);

                                                        Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
                                                        Dominio.Entidades.Veiculo veiculoObs = null;

                                                        if (!string.IsNullOrWhiteSpace(placaObservacao))
                                                            veiculoObs = repVeiculo.BuscarPlaca(placaObservacao);
                                                        else if (!string.IsNullOrWhiteSpace(chassiObservacao))
                                                            veiculoObs = repVeiculo.BuscarPorChassi(chassiObservacao);

                                                        if (veiculoObs != null && documentoDestinado.Emitente != null && documentoDestinado.Emitente.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe.HasValue && documentoDestinado.Emitente.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe.Value && !repAbastecimento.ContemAbastecimentoPorChave(documentoDestinado.Chave))
                                                        {
                                                            List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto> produtos = repDocumentoDestinadoEmpresaProduto.BuscarPorDocumento(documentoDestinado.Codigo);

                                                            if (produtos != null && produtos.Count > 0)
                                                            {
                                                                for (int i = 0; i < produtos.Count; i++)
                                                                {
                                                                    if (!string.IsNullOrWhiteSpace(produtos[i].cProd))
                                                                    {
                                                                        Dominio.Entidades.Produto produto = repTabelaValores.BuscarProdutoPorPessoa(produtos[i].cProd, documentoDestinado.Emitente.CPF_CNPJ);
                                                                        if (produto != null && !repAbastecimento.AbastecimentoDuplicado(documentoDestinado.DataEmissao.Value, documentoDestinado.Numero.ToString(), documentoDestinado.Emitente?.CPF_CNPJ ?? 0d, produto.Codigo, produtos[i].qCom, produtos[i].vUnCom))
                                                                        {
                                                                            Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();
                                                                            abastecimento.DocumentoDestinadoEmpresa = documentoDestinado;
                                                                            abastecimento.Veiculo = veiculoObs;
                                                                            abastecimento.Kilometragem = kmObservacao.ToInt();
                                                                            abastecimento.Equipamento = null;
                                                                            abastecimento.Horimetro = horimetroObservacao.ToInt();
                                                                            Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

                                                                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
                                                                            if (produto.CodigoNCM.StartsWith("271121") || produto.CodigoNCM.StartsWith("271019") || produto.CodigoNCM.StartsWith("271012"))
                                                                                tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                                                                            else
                                                                                tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;

                                                                            abastecimento.Motorista = null;
                                                                            abastecimento.Posto = documentoDestinado.Emitente;
                                                                            abastecimento.Produto = produto;
                                                                            abastecimento.TipoAbastecimento = tipoAbastecimento;
                                                                            abastecimento.Litros = produtos[i].qCom;
                                                                            abastecimento.ValorUnitario = produtos[i].vUnCom;
                                                                            abastecimento.Status = "A";
                                                                            abastecimento.Situacao = "A";
                                                                            abastecimento.DataAlteracao = DateTime.Now;
                                                                            abastecimento.Data = documentoDestinado.DataEmissao.Value;
                                                                            abastecimento.Documento = documentoDestinado.Numero.ToString();
                                                                            abastecimento.ChaveNotaFiscal = documentoDestinado.Chave;
                                                                            abastecimento.TipoRecebimentoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoAbastecimento.ImportacaoXML;
                                                                            if (abastecimento.Veiculo != null)
                                                                                abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(abastecimento.Veiculo.Codigo);

                                                                            if (abastecimento.Motorista == null && abastecimento.Equipamento != null)
                                                                            {
                                                                                Dominio.Entidades.Veiculo veiculoEquipamento = repVeiculo.BuscarPorEquipamento(abastecimento.Equipamento.Codigo);
                                                                                Dominio.Entidades.Usuario MotoristaEquipamento = veiculoEquipamento != null ? repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoEquipamento.Codigo) : null;

                                                                                if (veiculoEquipamento != null && MotoristaEquipamento != null)
                                                                                    abastecimento.Motorista = MotoristaEquipamento;
                                                                                else if (veiculoEquipamento != null)
                                                                                {
                                                                                    Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(veiculoEquipamento.Codigo);
                                                                                    if (veiculoTracao != null)
                                                                                        abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                                                                                }
                                                                            }
                                                                            else if (abastecimento.Motorista == null && abastecimento.Veiculo != null)
                                                                            {
                                                                                Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(abastecimento.Veiculo.Codigo);
                                                                                if (veiculoTracao != null)
                                                                                    abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                                                                            }
                                                                            abastecimento.TipoMovimento = configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto;

                                                                            Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unidadeTrabalho, abastecimento.Veiculo, null, configuracaoTMS);
                                                                            repAbastecimento.Inserir(abastecimento);

                                                                            Servicos.Auditoria.Auditoria.Auditar(auditado, abastecimento, null, "Abastecimento inserido por uma nota fiscal recebida dos destinados", unidadeTrabalho);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (configuracaoTMS.CriarNotaFiscalTransportePorDocumentoDestinado)
                                                            CriarXMLNotaFiscal(documentoDestinado, unidadeTrabalho, tipoServicoMultisoftware);
                                                        else if (tipoServicoMultisoftware.HasValue && documentoDestinado.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeTransporte)
                                                            VincularDocumentoACarga(documentoDestinado, unidadeTrabalho, tipoServicoMultisoftware.Value, configuracaoTMS);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Servicos.Log.TratarErro("Não foi possível salvar a NF-e v4.00 da consulta de documentos destinados: " + ex.ToString());
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Servicos.Log.TratarErro("Não foi possível processar o XML da NF-e v4.00 da consulta de documentos destinados: " + ex.ToString());
                                            }
                                        }
                                        else if (dfe.schema == "resEvento_v1.00.xsd")
                                        {
                                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento));

                                            MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento resEvento = (ser.Deserialize(resultStream) as MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento);

                                            if (cnpjsNaoImportarTransporte == null || !cnpjsNaoImportarTransporte.Contains(resEvento.Item))
                                                SalvarResEvento(dfe, resEvento, codigoEmpresa, unidadeTrabalho);
                                        }
                                        else if (dfe.schema == "procEventoNFe_v1.00.xsd")
                                        {
                                            bool salvarXML = true;
                                            string chave = "";
                                            resultStream.Position = 0;
                                            string prefixo = "";
                                            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(resultStream);
                                            System.Xml.Linq.XNamespace ns = doc.Root.Name.Namespace;

                                            string tpEvento = doc.Descendants(ns + "infEvento").FirstOrDefault()?.Element(ns + "tpEvento")?.Value ?? string.Empty;

                                            resultStream.Position = 0;

                                            if (tpEvento == "210200" || tpEvento == "210210" || tpEvento == "210220" || tpEvento == "210240") //Manifestação do Destinatário
                                            {
                                                XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.Evento.ManifestacaoDestinatario.EventoProcessado.TProcEvento));

                                                MultiSoftware.NFe.Evento.ManifestacaoDestinatario.EventoProcessado.TProcEvento procEvento = (ser.Deserialize(resultStream) as MultiSoftware.NFe.Evento.ManifestacaoDestinatario.EventoProcessado.TProcEvento);

                                                if (cnpjsNaoImportarTransporte == null || !cnpjsNaoImportarTransporte.Contains(procEvento.evento.infEvento.Item))
                                                    SalvarProcEvento(dfe, procEvento, codigoEmpresa, unidadeTrabalho);

                                                salvarXML = false;
                                            }
                                            else if (tpEvento == "110111") //Cancelamento
                                            {

                                                XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.Evento.Cancelamento.Retorno.TProcEvento));
                                                MultiSoftware.NFe.Evento.Cancelamento.Retorno.TProcEvento procEvento = (ser.Deserialize(resultStream) as MultiSoftware.NFe.Evento.Cancelamento.Retorno.TProcEvento);
                                                chave = procEvento.evento.infEvento.chNFe;

                                                // if (!cnpjsNaoImportarTransporte.Contains(procEvento.evento.infEvento.Item))
                                                long documentoCodigo = SalvarProcEvento(dfe, procEvento, codigoEmpresa, unidadeTrabalho);
                                                prefixo = "-" + documentoCodigo.ToString() + "-can";
                                            }
                                            else if (tpEvento == "110110") //CC-e
                                            {
                                                prefixo = "-cce";
                                                XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.Evento.CartaCorrecaoEletronica.EventoProcessado.TProcEvento));
                                                MultiSoftware.NFe.Evento.CartaCorrecaoEletronica.EventoProcessado.TProcEvento procEvento = (ser.Deserialize(resultStream) as MultiSoftware.NFe.Evento.CartaCorrecaoEletronica.EventoProcessado.TProcEvento);
                                                chave = procEvento.evento.infEvento.chNFe;

                                                if (cnpjsNaoImportarTransporte == null || !cnpjsNaoImportarTransporte.Contains(procEvento.evento.infEvento.Item))
                                                {
                                                    long documentoCodigo = SalvarProcEvento(dfe, procEvento, codigoEmpresa, unidadeTrabalho);
                                                    prefixo = "-" + documentoCodigo.ToString() + "-can";
                                                }

                                            }
                                            else
                                            {
                                                Servicos.Log.GravarInfo("Tipo de evento não implementado. " + tpEvento);
                                                salvarXML = false;
                                            }

                                            if (salvarXML)
                                            {
                                                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "NFe", "nfeProc");

                                                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, chave + prefixo + ".xml");

                                                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());

                                            }
                                        }
                                        else
                                        {
                                            Servicos.Log.GravarInfo("Schema não implementado. " + dfe.schema);
                                        }

                                        AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.Importado, iddocumentoxml, unidadeTrabalho, "XMl Importado", xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex.ToString(), "NotasDestinadas");
                            msgErro = "Ocorreu uma falha ao consultar os documentos destinados na SEFAZ.";
                            AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.ProblemaImportacao, iddocumentoxml, unidadeTrabalho, (msgErro + ex.ToString()), xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha);
                            return false;
                        }
                    }
                }
                else
                {
                    Servicos.Log.GravarInfo(@$"NFeDistribuicaoDFe, CNPJ Empresa: {empresa.CNPJ_Formatado} cStat: {retorno.cStat}, xMotivo: {retorno.xMotivo}.", "NotasDestinadas");

                    if (retorno.cStat == "656")
                        msgErro = $"CNPJ Empresa: {empresa.CNPJ_Formatado}, Retorno Sefaz: {retorno.cStat} {retorno.xMotivo} - Aguarde 60 minutos para realizar nova consulta.";
                    else
                        msgErro = $"CNPJ Empresa: {empresa.CNPJ_Formatado}, Retorno Sefaz: {retorno.cStat} {retorno.xMotivo}";

                    return false;
                }

                if (!string.IsNullOrEmpty(chaveAcesso))
                    return true;

                if (numeroNSU <= 0)
                {
                    configuracaoDocumentoDestinadoEmpresa.UltimoNSU = ultimoNSU;
                    repConfiguracaoDocumentoDestinadoEmpresa.Atualizar(configuracaoDocumentoDestinadoEmpresa);

                    repConfiguracaoDocumentoDestinadoEmpresa = null;
                    repDocumentoDestinadoEmpresa = null;
                    unidadeTrabalho.Dispose();
                    unidadeTrabalho = null;

                    GC.Collect();

                    unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
                    repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
                    repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unidadeTrabalho);
                }

            } while (ultimoNSU < maxNSU && retorno.cStat == "138");

            configuracaoDocumentoDestinadoEmpresa.UltimaConsulta = DateTime.Now;
            configuracaoDocumentoDestinadoEmpresa.EmConsulta = false;
            repConfiguracaoDocumentoDestinadoEmpresa.Atualizar(configuracaoDocumentoDestinadoEmpresa);

            return true;
        }

        public static void ReprocessarDestinados(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml documentoDestinadoXML, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado conf)
        {
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                string nsu = "";

                byte[] byteArray = Encoding.UTF8.GetBytes(documentoDestinadoXML.ConteudoXml);
                MemoryStream xml = new MemoryStream(byteArray);
                xml.Position = 0;

                try
                {
                    switch (documentoDestinadoXML.NomeSchema)
                    {
                        case "resNFe_v1.00.xsd":
                            MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe resNFev1 = (new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe)).Deserialize(xml) as MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe);
                            SalvarResNFe(null, resNFev1, documentoDestinadoXML.Empresa.Codigo, unidadeDeTrabalho, configuracaoTMS, documentoDestinadoXML.NumeroSequencialUnico.ToString());
                            break;
                        case "procNFe_v3.10.xsd":
                            MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc procNFev3 = (new XmlSerializer(typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)).Deserialize(xml) as MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc);
                            break;
                        case "procNFe_v4.00.xsd":
                            MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc procNFev4 = (new XmlSerializer(typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)).Deserialize(xml) as MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc);
                            break;
                        case "resEvento_v1.00.xsd":
                            MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento resEventov1 = (new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento)).Deserialize(xml) as MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento);
                            break;
                        case "procEventoNFe_v1.00.xsd":
                            System.Xml.Linq.XDocument docProcEventoNFev1 = System.Xml.Linq.XDocument.Load(xml);
                            string tpEventoProcEventoNFev1 = docProcEventoNFev1.Descendants(docProcEventoNFev1.Root.Name.Namespace + "infEvento").FirstOrDefault()?.Element(docProcEventoNFev1.Root.Name.Namespace + "tpEvento")?.Value ?? string.Empty;
                            break;
                        case "procCTe_v4.00.xsd":
                            MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc procCTev4 = (new XmlSerializer(typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)).Deserialize(xml) as MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc);
                            SalvarProcCTe(null, procCTev4, xml, documentoDestinadoXML.Empresa, unidadeDeTrabalho, tipoServicoMultisoftware, false, documentoDestinadoXML.NumeroSequencialUnico);
                            break;
                        case "procCTe_v3.00.xsd":
                            MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc procCTev3 = (new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)).Deserialize(xml) as MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc);
                            break;
                        case "procCTeOS_v3.00.xsd":
                            MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc procCTeOSv3 = (new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc)).Deserialize(xml) as MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc);
                            break;
                        case "procCTeOS_v4.00.xsd":
                            MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc procCTeOSv4 = (new XmlSerializer(typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc)).Deserialize(xml) as MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc);
                            break;
                        case "procEventoCTe_v4.00.xsd":
                            System.Xml.Linq.XDocument docrocEventoCTev4 = System.Xml.Linq.XDocument.Load(xml);
                            string tpEventoProcEventoCTev4 = docrocEventoCTev4.Descendants(docrocEventoCTev4.Root.Name.Namespace + "infEvento").FirstOrDefault()?.Element(docrocEventoCTev4.Root.Name.Namespace + "tpEvento")?.Value ?? string.Empty;
                            break;
                        case "procEventoCTe_v3.00.xsd":
                            System.Xml.Linq.XDocument docProcEventoCTev3 = System.Xml.Linq.XDocument.Load(xml);
                            string tpEventoProcEventoCTev3 = docProcEventoCTev3.Descendants(docProcEventoCTev3.Root.Name.Namespace + "infEvento").FirstOrDefault()?.Element(docProcEventoCTev3.Root.Name.Namespace + "tpEvento")?.Value ?? string.Empty;
                            break;
                        case "procMDFe_v3.00.xsd":
                            MultiSoftware.MDFe.v300.mdfeProc procMDFev3 = (new XmlSerializer(typeof(MultiSoftware.MDFe.v300.mdfeProc)).Deserialize(xml) as MultiSoftware.MDFe.v300.mdfeProc);
                            break;
                        case "procEventoMDFe_v3.00.xsd":
                            System.Xml.Linq.XDocument docProcEventoMDFev3 = System.Xml.Linq.XDocument.Load(xml);
                            string tpEventoProcEventoMDFev3 = docProcEventoMDFev3.Descendants(docProcEventoMDFev3.Root.Name.Namespace + "infEvento").FirstOrDefault()?.Element(docProcEventoMDFev3.Root.Name.Namespace + "tpEvento")?.Value ?? string.Empty;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Erro ao Deserializar documentos destinados: " + ex.ToString());
                }

                AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.Importado, documentoDestinadoXML.Codigo, unidadeDeTrabalho, "XMl Importado", "", conf.NaoSalvarXmlApenasNaFalha);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao reprocessar destinados documentos destinados: " + ex.ToString());
            }
        }

        public static void VerificarCaixaDeEntrada(int codigoEmpresa, string stringConexao, out string msgErro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            msgErro = "";

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork);

            List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> emails = repConfigEmailDocTransporte.BuscarEmailLerDocumentos(codigoEmpresa);

            foreach (Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail in emails)
            {
                Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                serEmail.ReceberEmail(configEmail, tipoServicoMultisoftware, configEmail.Email, configEmail.Senha, configEmail.Pop3, configEmail.RequerAutenticacaoPop3, configEmail.PortaPop3, unitOfWork);
            }
        }

        public static void VerificarEmails(int codigoEmpresa, string stringConexao, out string msgErro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            msgErro = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Email.EmailCaixaEntrada repEmailCaixaEntrada = new Repositorio.Embarcador.Email.EmailCaixaEntrada(unitOfWork);
            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada> emails = repEmailCaixaEntrada.BuscarPorTipoServico(tipoServicoMultisoftware, 0, 100);

            foreach (Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada email in emails)
            {
                if (email.Anexos != null && email.Anexos.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Email.EmailAnexos anexo in email.Anexos)
                    {
                        string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                        string caminhoAnexos = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "Entrada");
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminhoAnexos, anexo.GuidNomeArquivo + extensao);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            if (anexo.ArquivoZipado)
                            {
                                Servicos.Log.TratarErro("Assunto email " + email.Assunto, "XMLEmail");
                                Servicos.Log.TratarErro("O arquivo (Enviado por anexo com o nome " + anexo.NomeArquivo + ") está compactado, por favor envie os arquivos descompactados ou compactados na extensão .zip .", "XMLEmail");
                            }
                            else
                            {
                                if (extensao == ".xml")
                                {
                                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation)))
                                    {
                                        try
                                        {
                                            System.IO.StreamReader stReaderXML = new StreamReader(memoryStream);

                                            Servicos.Log.TratarErro("Iniciando Convesão de NF-e. Nome arquivo:" + anexo.NomeArquivo + " GUID: " + anexo.GuidNomeArquivo, "XMLEmail");
                                            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa xmlNotaFiscal = serNFe.BuscarDadosNotaFiscalDestinada(stReaderXML, unitOfWork, null);

                                            if (xmlNotaFiscal == null)
                                            {
                                                Servicos.Log.TratarErro("Iniciando Convesão de CT-e. Nome arquivo:" + anexo.NomeArquivo + " GUID: " + anexo.GuidNomeArquivo, "XMLEmail");
                                                xmlNotaFiscal = serNFe.BuscarDadosCTeDestinada(stReaderXML, unitOfWork, null);
                                                if (xmlNotaFiscal != null)
                                                {
                                                    string caminhoDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                                                    caminhoDocumentosFiscais = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "CTe", xmlNotaFiscal.Empresa.CNPJ_SemFormato);

                                                    caminhoDocumentosFiscais = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, xmlNotaFiscal.Chave + ".xml");

                                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDocumentosFiscais))
                                                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoDocumentosFiscais, memoryStream.ToArray());
                                                }
                                                else
                                                    Servicos.Log.TratarErro("Não foi possível converter o arquivo do e-mail para NF-e e CT-e", "XMLEmail");
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    string caminhoDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                                                    caminhoDocumentosFiscais = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "NFe", "nfeProc");

                                                    caminhoDocumentosFiscais = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, xmlNotaFiscal.Chave + ".xml");

                                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDocumentosFiscais))
                                                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoDocumentosFiscais, memoryStream.ToArray());
                                                }
                                                catch (Exception ex)
                                                {
                                                    Servicos.Log.TratarErro("Não foi possível salvar a NF-e da consulta de documentos destinados: " + ex.ToString(), "XMLEmail");
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Servicos.Log.TratarErro("assunto email " + email.Assunto, "XMLEmail");
                                            Servicos.Log.TratarErro("erro: " + ex, "XMLEmail");
                                        }
                                    }
                                }
                            }

                            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                                Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                    }

                    email.Anexos.Clear();

                    repEmailCaixaEntrada.Deletar(email);

                }
                else
                {
                    repEmailCaixaEntrada.Deletar(email);
                }
            }
        }

        public static bool EnviarIMPUTDocumentosDestinadosEmpresa(int codigoEmpresa, string stringConexao, out string msgErro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            msgErro = "";

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);

            string caminhoDocumentosIMPUT = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosINPUT;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados repIntegracaoFTPDocumentosDestinados = new Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
            {
                msgErro = "Empresa ou Certificado não encontrado";
                return false;
            }
            Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados integracaoFTP = repIntegracaoFTPDocumentosDestinados.BuscarPorEmpresa(codigoEmpresa);
            if (integracaoFTP == null || string.IsNullOrWhiteSpace(integracaoFTP.DiretorioInput) || string.IsNullOrWhiteSpace(integracaoFTP.DiretorioOutput) || string.IsNullOrWhiteSpace(integracaoFTP.DiretorioXML))
            {
                msgErro = "Integração da empresa não configurada";
                return false;
            }
            if (string.IsNullOrEmpty(caminhoDocumentosIMPUT))
            {
                msgErro = "Caminho IMPUT não cadastrado";
                return false;
            }
            Auditado.IP = integracaoFTP.EnderecoFTP;
            try
            {
                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosEnvioIntegracao = repDocumentoDestinadoEmpresa.BuscarPendentesEnvioIntegracao(codigoEmpresa);
                if (documentosEnvioIntegracao.Count() > 0)
                {
                    StreamWriter x;
                    MemoryStream arquivoINPUT = new MemoryStream();

                    string siglaEmpresa = !string.IsNullOrWhiteSpace(empresa?.CodigoEmpresa ?? "") ? empresa.CodigoEmpresa : "";
                    string nomeArquivo = siglaEmpresa + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Utilidades.String.OnlyNumbers(documentosEnvioIntegracao.Count().ToString("n0")) + ".txt";
                    string caminhoNome = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosIMPUT, nomeArquivo);
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoNome))
                        nomeArquivo = siglaEmpresa + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Utilidades.String.OnlyNumbers(documentosEnvioIntegracao.Count().ToString("n0")) + "_9.txt";

                    x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

                    foreach (var documento in documentosEnvioIntegracao)
                    {
                        x.WriteLine(documento.Chave + ";" +
                            documento.DataEmissao.Value.ToString("dd/MM/yyyy hh:MM") + ";" +
                            documento.Valor.ToString("n2") + ";" +
                            documento.CPFCNPJEmitente + ";" +
                            documento.Protocolo + ";" +
                            (documento.Cancelado ? "Cancelado" : "Autorizada"));

                        documento.GerouArquivoIntegracao = true;
                        repDocumentoDestinadoEmpresa.Atualizar(documento);
                        try
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Gerou arquivo de integração via FTP. Nome do arquivo: " + caminhoNome, unidadeTrabalho);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Problemas ao auditar integração via FTP: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                        }

                    }
                    x.Flush();

                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoNome, arquivoINPUT.ToArray());

                    string mensagemRetorno = string.Empty;
                    if (!Servicos.FTP.EnviarArquivo(arquivoINPUT, nomeArquivo, integracaoFTP.EnderecoFTP, integracaoFTP.Porta, integracaoFTP.DiretorioInput, integracaoFTP.Usuario, integracaoFTP.Senha, integracaoFTP.Passivo, integracaoFTP.SSL, out mensagemRetorno, integracaoFTP.UtilizarSFTP))
                    {
                        Servicos.Log.TratarErro("Problemas ao enviar arquivo por FTP: " + mensagemRetorno, "IntegracaoDocumentosDestinados");
                        foreach (var documento in documentosEnvioIntegracao)
                        {
                            documento.GerouArquivoIntegracao = false;
                            repDocumentoDestinadoEmpresa.Atualizar(documento);
                            try
                            {
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "ERRO DO FTP. Não gerou arquivo de integração via FTP.", unidadeTrabalho);
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro("Problemas ao auditar integração via FTP: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível realizar a integração por FTP: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                return false;
            }

            return true;
        }

        public static bool ProcessarXMLDocumentosDestinadosEmpresa(int codigoEmpresa, string stringConexao, out string msgErro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            msgErro = "";

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);

            string caminhoXML = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados repIntegracaoFTPDocumentosDestinados = new Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
            {
                msgErro = "Empresa ou Certificado não encontrado";
                return false;
            }
            Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados integracaoFTP = repIntegracaoFTPDocumentosDestinados.BuscarPorEmpresa(codigoEmpresa);
            if (integracaoFTP == null || string.IsNullOrWhiteSpace(integracaoFTP.DiretorioImputXML))
            {
                msgErro = "Integração imput da empresa não configurada";
                return false;
            }
            Auditado.IP = integracaoFTP.EnderecoFTP;
            try
            {
                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosPententes = repDocumentoDestinadoEmpresa.BuscarPendentesEnvioImputIntegracao(codigoEmpresa);

                string msgErroXML = string.Empty;
                bool moverArquivoXML = true;

                foreach (var documento in documentosPententes)
                {
                    if (documento != null)
                    {
                        //aqui envia o xml para o ftp
                        msgErroXML = string.Empty;
                        string localXML = string.Empty;
                        if (documento.ModeloDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe)
                            localXML = Utilidades.IO.FileStorageService.Storage.Combine(caminhoXML, "NFe", "nfeProc", documento.Chave + ".xml");
                        else
                            localXML = Utilidades.IO.FileStorageService.Storage.Combine(caminhoXML, "CTe", empresa.CNPJ_SemFormato, documento.Chave + ".xml");

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(localXML) && (documento.ModeloDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe))
                        {
                            Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documento.Empresa.Codigo, stringConexao, null, out msgErroXML, out string codigoStatusRetornoSefaz, documento.NumeroSequencialUnico, null, tipoServicoMultisoftware);
                            if (!Utilidades.IO.FileStorageService.Storage.Exists(localXML))
                            {
                                Servicos.Log.TratarErro("Não foi possível realizar o download do XML: " + msgErroXML, "IntegracaoDocumentosDestinados");
                                moverArquivoXML = false;
                            }
                        }
                        if (Utilidades.IO.FileStorageService.Storage.Exists(localXML))
                        {
                            string mensagemRetorno = string.Empty;
                            if (!Servicos.FTP.EnviarArquivo(new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(localXML)), documento.Chave + ".xml", integracaoFTP.EnderecoFTP, integracaoFTP.Porta, integracaoFTP.DiretorioImputXML, integracaoFTP.Usuario, integracaoFTP.Senha, integracaoFTP.Passivo, integracaoFTP.SSL, out mensagemRetorno, integracaoFTP.UtilizarSFTP))
                            {
                                Servicos.Log.TratarErro("Problemas ao enviar arquivo imput por FTP: " + mensagemRetorno, "IntegracaoDocumentosDestinados");
                                moverArquivoXML = false;
                            }
                        }
                        else
                            moverArquivoXML = false;

                        if (moverArquivoXML)
                        {
                            documento.EnviouXMLImputIntegracao = true;
                            repDocumentoDestinadoEmpresa.Atualizar(documento);
                            try
                            {
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Enviar arquivo XML via imput FTP.", unidadeTrabalho);
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro("Problemas ao auditar Enviar arquivo XML via imput FTP: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                            }
                        }
                        else
                        {
                            documento.EnviouXMLImputIntegracao = false;
                            documento.TentativasEnvioImputIntegracao += 1;
                            repDocumentoDestinadoEmpresa.Atualizar(documento);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Não foi possível enviar o XML via imput ao FTP pois o mesmo ainda não foi recebido.", unidadeTrabalho);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível realizar a integração por do OUTPUT FTP: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                return false;
            }

            return true;
        }


        public static bool ProcessarOUTPUTDocumentosDestinadosEmpresa(int codigoEmpresa, string stringConexao, out string msgErro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            msgErro = "";

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);

            string caminhoDocumentosOUTPUT = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosOUTPUT;
            string caminhoXML = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados repIntegracaoFTPDocumentosDestinados = new Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
            {
                msgErro = "Empresa ou Certificado não encontrado";
                return false;
            }
            Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados integracaoFTP = repIntegracaoFTPDocumentosDestinados.BuscarPorEmpresa(codigoEmpresa);
            if (integracaoFTP == null || string.IsNullOrWhiteSpace(integracaoFTP.DiretorioInput) || string.IsNullOrWhiteSpace(integracaoFTP.DiretorioOutput) || string.IsNullOrWhiteSpace(integracaoFTP.DiretorioXML))
            {
                msgErro = "Integração da empresa não configurada";
                return false;
            }
            if (string.IsNullOrEmpty(caminhoDocumentosOUTPUT))
            {
                msgErro = "Caminho OUTPUT não cadastrado";
                return false;
            }
            if (string.IsNullOrEmpty(caminhoXML))
            {
                msgErro = "Caminho XML não cadastrado";
                return false;
            }
            Auditado.IP = integracaoFTP.EnderecoFTP;
            try
            {
                string erro = string.Empty;
                Servicos.FTP.DownloadArquivosPasta(integracaoFTP.EnderecoFTP, integracaoFTP.Porta, integracaoFTP.DiretorioOutput, integracaoFTP.Usuario, integracaoFTP.Senha, integracaoFTP.Passivo, integracaoFTP.SSL, caminhoDocumentosOUTPUT, out erro, integracaoFTP.UtilizarSFTP, false, "", true, false, true);
                if (!string.IsNullOrWhiteSpace(erro))
                {
                    Servicos.Log.TratarErro("Erro ao baixar arquivos OUTPUT do FTP: " + erro, "IntegracaoDocumentosDestinados");
                }

                IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoDocumentosOUTPUT, "*.txt", SearchOption.TopDirectoryOnly).AsParallel();

                string chaveNFe;
                string msgErroXML = string.Empty;
                bool moverArquivoXML = true;
                foreach (string arquivo in arquivos)
                {
                    string fileName = Path.GetFileName(arquivo);
                    moverArquivoXML = true;
                    chaveNFe = Utilidades.String.OnlyNumbers(fileName);
                    if (chaveNFe.Length == 44)
                    {
                        Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarNFePorChave(chaveNFe);
                        if (documento != null)
                        {
                            //aqui fazer a manifestação
                            if (documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada)
                            {
                                try
                                {
                                    if (!documento.EnviouXMLIntegracao)
                                        Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Emissão de Manifestação da Ciência da Operação.", unidadeTrabalho);
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("Problemas ao auditar Emissão de Manifestação da Ciência da Operação: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                                }

                                if (EmitirManifestacao(ref documento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.CienciaOperacao, "Solicitado por Integração em FTP", unidadeTrabalho))
                                    repDocumentoDestinadoEmpresa.Atualizar(documento);

                                try
                                {
                                    if (!documento.EnviouXMLIntegracao)
                                        Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Emissão de Manifestação da Confirmação da Operação.", unidadeTrabalho);
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("Problemas ao auditar Emissão de Manifestação da Confirmação da Operação.: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                                }

                                if (EmitirManifestacao(ref documento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.ConfirmadaOperacao, "Solicitado por Integração em FTP", unidadeTrabalho))
                                    repDocumentoDestinadoEmpresa.Atualizar(documento);
                            }

                            //aqui envia o xml para o ftp
                            msgErroXML = string.Empty;
                            string localXML = string.Empty;
                            if (documento.ModeloDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe)
                                localXML = Utilidades.IO.FileStorageService.Storage.Combine(caminhoXML, "NFe", "nfeProc", documento.Chave + ".xml");
                            else
                                localXML = Utilidades.IO.FileStorageService.Storage.Combine(caminhoXML, "CTe", empresa.CNPJ_SemFormato, documento.Chave + ".xml");

                            if (!Utilidades.IO.FileStorageService.Storage.Exists(localXML) && (documento.ModeloDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe))
                            {
                                Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documento.Empresa.Codigo, stringConexao, null, out msgErroXML, out string codigoStatusRetornoSefaz, documento.NumeroSequencialUnico, null, tipoServicoMultisoftware);
                                if (!Utilidades.IO.FileStorageService.Storage.Exists(localXML))
                                {
                                    Servicos.Log.TratarErro("Não foi possível realizar o download do XML: " + msgErroXML, "IntegracaoDocumentosDestinados");
                                    moverArquivoXML = false;
                                }
                            }
                            if (Utilidades.IO.FileStorageService.Storage.Exists(localXML))
                            {
                                string mensagemRetorno = string.Empty;
                                if (!Servicos.FTP.EnviarArquivo(new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(localXML)), chaveNFe + ".xml", integracaoFTP.EnderecoFTP, integracaoFTP.Porta, integracaoFTP.DiretorioXML, integracaoFTP.Usuario, integracaoFTP.Senha, integracaoFTP.Passivo, integracaoFTP.SSL, out mensagemRetorno, integracaoFTP.UtilizarSFTP))
                                {
                                    Servicos.Log.TratarErro("Problemas ao enviar arquivo OUTPUT por FTP: " + mensagemRetorno, "IntegracaoDocumentosDestinados");
                                    moverArquivoXML = false;
                                }
                            }
                            else
                                moverArquivoXML = false;

                            if (moverArquivoXML)
                            {
                                documento.EnviouXMLIntegracao = true;
                                repDocumentoDestinadoEmpresa.Atualizar(documento);
                                try
                                {
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Enviar arquivo XML via FTP.", unidadeTrabalho);
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("Problemas ao auditar Enviar arquivo XML via FTP: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                                }
                            }
                        }
                    }
                    if (moverArquivoXML)
                    {
                        MoverParaPastaProcessados(Guid.NewGuid().ToString() + fileName, caminhoDocumentosOUTPUT, arquivo);
                        Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                    }
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível realizar a integração por do OUTPUT FTP: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                return false;
            }

            return true;
        }

        private static bool EmitirManifestacao(ref Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario tipoManifestacao, string justificativa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario manifestacao = Servicos.Embarcador.Documentos.ManifestacaoDestinatario.EmitirManifestacaoDestinatario(documentoDestinado.Empresa.Codigo, documentoDestinado.Chave, tipoManifestacao, justificativa, unidadeDeTrabalho);
                documentoDestinado.Manifestacoes.Add(manifestacao);
                if (manifestacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusManifestacaoDestinatario.Autorizado)
                {
                    switch (manifestacao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.CienciaOperacao:
                            documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.CienciaOperacao;
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.ConfirmadaOperacao:
                            documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.ConfirmadaOperacao;
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.Desconhecida:
                            documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.Desconhecida;
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.OperacaoNaoRealizada:
                            documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.OperacaoNaoRealizada;
                            break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível realizar a manifestação da NFe por integração: " + ex.ToString());
                return false;
            }
        }

        private static void MoverParaPastaProcessados(string nomeArquivo, string caminhoArmazenamento, string fullName)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Utilidades.IO.FileStorageService.Storage.ReadAllText(fullName));

            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArmazenamento, "Processados", nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoCompleto, bytes);
        }
        public static bool ValidaUltimaConsultaNotas(int codigoEmpresa, out string msgErro, Repositorio.UnitOfWork unitOfWork, int? tempoIntervaloRequisicao = null)
        {
            msgErro = "";

            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa configuracaoDocumentoDestinadoEmpresa = repConfiguracaoDocumentoDestinadoEmpresa.BuscarPorEmpresa(codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe);

            if (empresa == null)
            {
                msgErro = "Erro ao buscar informações.";
                return false;
            }

            if (configuracaoDocumentoDestinadoEmpresa == null)
                return true;

            if (!tempoIntervaloRequisicao.HasValue)
                tempoIntervaloRequisicao = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().IntervaloDocumentosFiscaisEmbarcador;

            if (tempoIntervaloRequisicao == 0)
            {
                msgErro = "Nenhuma configuração cadastrada.";
                return false;
            }

            if (configuracaoDocumentoDestinadoEmpresa.UltimaConsulta.HasValue && DateTime.Now.AddMinutes(-tempoIntervaloRequisicao.Value) < configuracaoDocumentoDestinadoEmpresa.UltimaConsulta)
            {
                msgErro = "Retorno Sefaz: 656 Rejeicao: Consumo indevido - Aguarde " + tempoIntervaloRequisicao.ToString() + " minutos para realizar nova consulta.";
                return false;
            }

            return true;
        }

        public static bool ProcessarXMLDocumentosDestinadosCTeEmpresa(int codigoEmpresa, string stringConexao, ref string mensagemRetorno, string caminhoDocumentosFiscais, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            long numeroNSU = repDocumentoDestinadoEmpresa.BuscarPrimeiroNSU(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe);

            IEnumerable<string> files = Utilidades.IO.FileStorageService.Storage.GetFiles(Servicos.FS.GetPath(@"C:\XMLs\"));

            bool salvoComSucesso = false;
            int contador = 0;
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                salvoComSucesso = false;
                using (MemoryStream resultStream = new MemoryStream())
                {
                    XDocument doc = null;
                    int inicioXML = 0;
                    using System.IO.Stream fileStream = Utilidades.IO.FileStorageService.Storage.OpenRead(file);
                    using System.IO.Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(file);

                    fileStream.CopyTo(resultStream);

                    resultStream.Position = 0;

                    XmlSerializer ser3 = new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc));
                    XmlSerializer ser2 = new XmlSerializer(typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc));
                    XmlSerializer ser1 = new XmlSerializer(typeof(MultiSoftware.CTe.v104.ConhecimentoDeTransporteProcessado.cteProc));

                    string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cte3Proc = null;
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cte2Proc = null;
                    MultiSoftware.CTe.v104.ConhecimentoDeTransporteProcessado.cteProc cte1Proc = null;

                    try
                    {
                        doc = XDocument.Load(stream);
                    }
                    catch (System.Exception e) 
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao carregar XML de documento destinado empresa: {e.ToString()}", "CatchNoAction");
                    }
                    if (doc == null)
                    {
                        inicioXML = ("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>".Length * 2) - 2;
                        stream.Position = inicioXML;
                        doc = XDocument.Load(stream);
                    }
                    XNamespace ns = doc.Root.Name.Namespace;
                    string versao = (from ele in doc.Descendants(ns + "infCte") select ele.Attribute("versao").Value).FirstOrDefault();

                    if (versao == "3.00")
                    {
                        try
                        {
                            cte3Proc = (ser3.Deserialize(resultStream) as MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Não foi possível salvar CT-e da pasta de importação: " + ex.ToString());
                            salvoComSucesso = false;
                        }
                    }
                    else if (versao == "2.00")
                    {
                        try
                        {
                            cte2Proc = (ser2.Deserialize(resultStream) as MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Não foi possível salvar CT-e da pasta de importação: " + ex.ToString());
                            salvoComSucesso = false;
                        }
                    }
                    else if (versao == "1.04")
                    {
                        try
                        {
                            cte1Proc = (ser1.Deserialize(resultStream) as MultiSoftware.CTe.v104.ConhecimentoDeTransporteProcessado.cteProc);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Não foi possível salvar CT-e da pasta de importação: " + ex.ToString());
                            salvoComSucesso = false;
                        }
                    }
                    else
                        salvoComSucesso = false;

                    numeroNSU -= 1;
                    if (cte2Proc == null && cte3Proc == null && cte1Proc == null)
                    {
                        salvoComSucesso = false;
                    }
                    else if (cte1Proc != null)
                    {
                        if (SalvarProcCTe(null, cte1Proc, resultStream, empresa, unidadeTrabalho, tipoServicoMultisoftware, true, numeroNSU, "23643315"))
                        {
                            try
                            {
                                resultStream.Position = 0;

                                string caminho = string.Empty;

                                if (!string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                                    caminho = caminhoDocumentosFiscais;
                                else
                                    caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", empresa.CNPJ, cte1Proc.protCTe.infProt.chCTe + ".xml");

                                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());
                                salvoComSucesso = true;
                            }
                            catch (Exception ex)
                            {
                                salvoComSucesso = false;
                                Servicos.Log.TratarErro("Não foi possível salvar CT-e da pasta de importação: " + ex.ToString());
                            }
                        }
                    }
                    else if (cte2Proc != null)
                    {
                        if (SalvarProcCTe(null, cte2Proc, resultStream, empresa, unidadeTrabalho, tipoServicoMultisoftware, true, numeroNSU, "23643315"))
                        {
                            try
                            {
                                resultStream.Position = 0;

                                string caminho = string.Empty;

                                if (!string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                                    caminho = caminhoDocumentosFiscais;
                                else
                                    caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", empresa.CNPJ, cte2Proc.protCTe.infProt.chCTe + ".xml");

                                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());
                                salvoComSucesso = true;
                            }
                            catch (Exception ex)
                            {
                                salvoComSucesso = false;
                                Servicos.Log.TratarErro("Não foi possível salvar CT-e da pasta de importação: " + ex.ToString());
                            }
                        }
                    }
                    else if (SalvarProcCTe(null, cte3Proc, resultStream, empresa, unidadeTrabalho, tipoServicoMultisoftware, true, numeroNSU, "23643315"))
                    {
                        try
                        {
                            resultStream.Position = 0;

                            string caminho = string.Empty;

                            if (!string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                                caminho = caminhoDocumentosFiscais;
                            else
                                caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", empresa.CNPJ, cte3Proc.protCTe.infProt.chCTe + ".xml");

                            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());
                            salvoComSucesso = true;
                        }
                        catch (Exception ex)
                        {
                            Utilidades.IO.FileStorageService.Storage.Move(file, Servicos.FS.GetPath(@"C:\XMLs\Rejeitados\") + fileName);
                            Servicos.Log.TratarErro("Não foi possível salvar CT-e da pasta de importação: " + ex.ToString());
                        }
                    }

                    fileStream.Close();
                    resultStream.Close();
                    stream.Close();
                }
                if (salvoComSucesso)
                    Utilidades.IO.FileStorageService.Storage.Move(file, Servicos.FS.GetPath(@"C:\XMLs\Processados\") + fileName);
                else
                    Utilidades.IO.FileStorageService.Storage.Move(file, Servicos.FS.GetPath(@"C:\XMLs\Rejeitados\") + fileName);

                contador += 1;
                if ((contador % 10) == 0)
                {
                    unidadeTrabalho.FlushAndClear();

                    unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
                }
            }

            return true;
        }

        public static bool ObterDocumentosDestinadosCTeEmpresa(int codigoEmpresa, string urlSefaz, string stringConexao, long numeroNSU, ref string mensagemRetorno, out string codigoStatusRetornoSefaz, string caminhoDocumentosFiscais, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            codigoStatusRetornoSefaz = string.Empty;

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado repConfigDocumentoDestinado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado configuracaoDocumentoDestinado = repConfigDocumentoDestinado.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            if (string.IsNullOrWhiteSpace(urlSefaz))
            {
                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    if (empresa.Localidade.Estado.SefazCTe == null)
                    {
                        mensagemRetorno = "Estado sem Sefaz configurado.";
                        return false;
                    }
                    urlSefaz = empresa.Localidade.Estado.SefazCTe.UrlDistribuicaoDFe;
                }
                else
                {
                    if (empresa.Localidade.Estado.SefazCTeHomologacao == null)
                    {
                        mensagemRetorno = "Estado sem Sefaz Homologação configurado.";
                        return false;
                    }
                    urlSefaz = empresa.Localidade.Estado.SefazCTeHomologacao.UrlDistribuicaoDFe;
                }
            }

            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
            {
                mensagemRetorno = $"Empresa {empresa.CNPJ_Formatado}: Empresa/Certificado digital não configurado.";
                return false;
            }

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa configuracaoDocumentoDestinadoEmpresa = repConfiguracaoDocumentoDestinadoEmpresa.BuscarPorEmpresa(codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe);

            if (configuracaoDocumentoDestinadoEmpresa == null)
            {
                configuracaoDocumentoDestinadoEmpresa = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa();
                configuracaoDocumentoDestinadoEmpresa.Empresa = empresa;
                configuracaoDocumentoDestinadoEmpresa.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;

                repConfiguracaoDocumentoDestinadoEmpresa.Inserir(configuracaoDocumentoDestinadoEmpresa);
            }

            long ultimoNSU = 0;

            ultimoNSU = configuracaoDocumentoDestinadoEmpresa.UltimoNSU; //repDocumentoDestinadoEmpresa.BuscarUltimoNSUEmpresa(codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe);

            //if (ultimoNSU < configuracaoDocumentoDestinadoEmpresa.UltimoNSU)
            //    ultimoNSU = configuracaoDocumentoDestinadoEmpresa.UltimoNSU;

            long maxNSU = 0L;

            if (numeroNSU > 0)
            {
                ultimoNSU = numeroNSU;
                maxNSU = numeroNSU;
            }


            //#if DEBUG
            //            if (numeroNSU == 0)
            //                ultimoNSU = 1571708;
            //#endif


            MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeInt retorno = null;

            Dominio.Enumeradores.TipoAmbiente tipoAmbiente = empresa.TipoAmbiente; //Dominio.Enumeradores.TipoAmbiente.Producao;
            int quantidadeDfesObtidos = 0;
            do
            {
                try
                {
                    retorno = ConsultarDocumentosFiscaisCTe(unidadeTrabalho, empresa.CNPJ, ultimoNSU, tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.TAmb.Item1 : MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.TAmb.Item2, (MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.TCodUfIBGE)empresa.Localidade.Estado.CodigoIBGE, empresa.NomeCertificado, empresa.SenhaCertificado, empresa.NomeCertificadoKeyVault, urlSefaz, numeroNSU > 0 ? true : false);
                    codigoStatusRetornoSefaz = retorno.cStat;
                }
                catch (ServicoException ex)
                {
                    Log.GravarAdvertencia(ex.Message);
                    mensagemRetorno = ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    mensagemRetorno = ex.Message;
                    return false;
                }

                if (numeroNSU <= 0)
                {
                    maxNSU = long.Parse(retorno.maxNSU);
                    ultimoNSU = long.Parse(retorno.ultNSU);
                }

                if (retorno.cStat == "138")
                {
                    quantidadeDfesObtidos += retorno.loteDistDFeInt.docZip.Length;
                    foreach (MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe in retorno.loteDistDFeInt.docZip)
                    {
                        Int64 iddocumentoxml = 0;
                        string xmlDocumento = string.Empty;

                        using (MemoryStream compressedStream = new MemoryStream(dfe.Value))
                        {
                            using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                            {
                                using (MemoryStream resultStream = new MemoryStream())
                                {
                                    zipStream.CopyTo(resultStream);

                                    resultStream.Position = 0;

                                    StreamReader reader = new StreamReader(resultStream);
                                    xmlDocumento = reader.ReadToEnd();

                                    GravarXmlDistribucaoDFe(empresa, dfe, null, null, resultStream, SituacaoXml.Importado, unidadeTrabalho, xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha, out iddocumentoxml);

                                    resultStream.Position = 0;

                                    if (dfe.schema == "procCTe_v4.00.xsd")
                                    {
                                        XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc));

                                        string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());

                                        MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (ser.Deserialize(resultStream) as MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc);

                                        try
                                        {
                                            if (!SalvarProcCTe(dfe, cteProc, resultStream, empresa, unidadeTrabalho, tipoServicoMultisoftware))
                                                throw new Exception("Não foi possivel criar a entidade documento destinado");

                                            resultStream.Position = 0;

                                            string caminho = string.Empty;

                                            if (!string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                                                caminho = caminhoDocumentosFiscais;
                                            else
                                                caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", empresa.CNPJ, cteProc.protCTe.infProt.chCTe + ".xml");

                                            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());
                                        }
                                        catch (Exception ex)
                                        {
                                            AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.ProblemaImportacao, iddocumentoxml, unidadeTrabalho, "Não foi possível salvar o CT-e 4.0 NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha);
                                            Servicos.Log.TratarErro("Não foi possível salvar o CT-e 4.0 NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), "BuscarCTEsDestinados");
                                            continue;
                                        }

                                    }
                                    else if (dfe.schema == "procCTe_v3.00.xsd")
                                    {
                                        XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc));

                                        string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());

                                        MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (ser.Deserialize(resultStream) as MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc);

                                        try
                                        {
                                            if (!SalvarProcCTe(dfe, cteProc, resultStream, empresa, unidadeTrabalho, tipoServicoMultisoftware))
                                                throw new Exception("Não foi possivel criar a entidade documento destinado");

                                            resultStream.Position = 0;

                                            string caminho = string.Empty;

                                            if (!string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                                                caminho = caminhoDocumentosFiscais;
                                            else
                                                caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", empresa.CNPJ, cteProc.protCTe.infProt.chCTe + ".xml");

                                            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());
                                        }
                                        catch (Exception ex)
                                        {
                                            AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.ProblemaImportacao, iddocumentoxml, unidadeTrabalho, "Não foi possível salvar o CT-e 3.0 NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha);
                                            Servicos.Log.TratarErro("Não foi possível salvar o CT-e 3.0 NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), "BuscarCTEsDestinados");
                                            continue;
                                        }

                                    }
                                    else if (dfe.schema == "procCTeOS_v3.00.xsd")
                                    {
                                        XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc));

                                        string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());

                                        MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc cteProc = (ser.Deserialize(resultStream) as MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc);

                                        try
                                        {
                                            if (!SalvarProcCTeOsV3(dfe, cteProc, resultStream, empresa, unidadeTrabalho, caminhoDocumentosFiscais))
                                                throw new Exception("Não foi possivel criar a entidade documento destinado");

                                        }
                                        catch (Exception ex)
                                        {
                                            AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.ProblemaImportacao, iddocumentoxml, unidadeTrabalho, "Não foi possível salvar o CT-eOS 3.0 NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha);
                                            Servicos.Log.TratarErro("Não foi possível salvar o CT-eOS 3.0 NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), "BuscarCTEsDestinados");
                                            continue;
                                        }

                                    }
                                    else if (dfe.schema == "procCTeOS_v4.00.xsd")
                                    {
                                        XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc));

                                        string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());

                                        MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc cteProc = (ser.Deserialize(resultStream) as MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc);

                                        try
                                        {
                                            if (!SalvarProcCTeOsV4(dfe, cteProc, resultStream, empresa, unidadeTrabalho, caminhoDocumentosFiscais, tipoServicoMultisoftware))
                                                throw new Exception("Não foi possivel criar a entidade documento destinado");

                                        }
                                        catch (Exception ex)
                                        {
                                            AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.ProblemaImportacao, iddocumentoxml, unidadeTrabalho, "Não foi possível salvar o CT-eOS 4.0 NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha);
                                            Servicos.Log.TratarErro("Não foi possível salvar o CT-eOS 4.0 NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), "BuscarCTEsDestinados");
                                            continue;
                                        }

                                    }
                                    else if (dfe.schema == "procEventoCTe_v4.00.xsd")
                                    {

                                        resultStream.Position = 0;

                                        System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(resultStream);
                                        System.Xml.Linq.XNamespace ns = doc.Root.Name.Namespace;

                                        string tpEvento = doc.Descendants(ns + "infEvento").FirstOrDefault()?.Element(ns + "tpEvento")?.Value ?? string.Empty;

                                        resultStream.Position = 0;

                                        if (tpEvento == "110111") //Cancelamento 4.0
                                        {
                                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.CTe.v400.Eventos.procEventoCTe));

                                            MultiSoftware.CTe.v400.Eventos.procEventoCTe procEvento = (ser.Deserialize(resultStream) as MultiSoftware.CTe.v400.Eventos.procEventoCTe);

                                            try
                                            {
                                                if (!SalvarProcEventoCTe(dfe, procEvento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe, empresa.Codigo, unidadeTrabalho))
                                                    throw new Exception("Não foi possivel criar a entidade documento destinado");

                                                resultStream.Position = 0;

                                                string caminho = string.Empty;

                                                if (!string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                                                    caminho = caminhoDocumentosFiscais;
                                                else
                                                    caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                                                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", empresa.CNPJ, procEvento.eventoCTe.infEvento.chCTe + "_procCancCTe.xml");

                                                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());
                                            }
                                            catch (Exception ex)
                                            {
                                                AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.ProblemaImportacao, iddocumentoxml, unidadeTrabalho, "Não foi possível salvar o Cancelamento de CT-e NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha);
                                                Servicos.Log.TratarErro("Não foi possível salvar o Cancelamento de CT-e NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), "BuscarCTEsDestinados");
                                                continue;
                                            }

                                        }
                                        else
                                        {
                                            Servicos.Log.GravarInfo("Evento não implementado DFe CTe. " + tpEvento, "BuscarCTEsDestinados");

                                            //110110 = CCe
                                            //110113 = EPEC
                                            //110160 = Registr do MultiModal
                                            //110170 = Informações da GTV

                                            //310620 = Registro de Passagem
                                            //510620 = Registro de Passagem Automatico
                                            //310610 = MDFe Autorizado
                                            //310611 = MDFe Cancelado

                                            //240130 = CTe Complementar Autorizado
                                            //240131 = CTe Complentar Cancelado
                                            //240140 = CTe Substituição Autorizado
                                            //240150 = CTe Anulação Autorizado
                                            //240160 = Liberaçao EPEC
                                            //240170 = Liberaçao Cancelamento

                                            //440130 = CTe Redespacho Autorizado
                                            //440140 = CTe Redespacho Intermediario Autorizado
                                            //440150 = CTe Subcontratação Autorizado
                                            //440160 = Autorizado Serviço Vinculado MultiModal

                                            //610110 = Prestação de Serviço em Desacordo
                                        }
                                    }
                                    else if (dfe.schema == "procEventoCTe_v3.00.xsd")
                                    {
                                        resultStream.Position = 0;

                                        System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(resultStream);
                                        System.Xml.Linq.XNamespace ns = doc.Root.Name.Namespace;

                                        string tpEvento = doc.Descendants(ns + "infEvento").FirstOrDefault()?.Element(ns + "tpEvento")?.Value ?? string.Empty;

                                        resultStream.Position = 0;

                                        if (tpEvento == "110111") //Cancelamento 3.0
                                        {
                                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.CTe.v300.Eventos.procEventoCTe));

                                            MultiSoftware.CTe.v300.Eventos.procEventoCTe procEvento = (ser.Deserialize(resultStream) as MultiSoftware.CTe.v300.Eventos.procEventoCTe);

                                            try
                                            {
                                                if (!SalvarProcEventoCTe(dfe, procEvento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe, empresa.Codigo, unidadeTrabalho))
                                                    throw new Exception("Não foi possivel criar a entidade documento destinado");

                                                resultStream.Position = 0;

                                                string caminho = string.Empty;

                                                if (!string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                                                    caminho = caminhoDocumentosFiscais;
                                                else
                                                    caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                                                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", empresa.CNPJ, procEvento.eventoCTe.infEvento.chCTe + "_procCancCTe.xml");

                                                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());
                                            }
                                            catch (Exception ex)
                                            {
                                                AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.ProblemaImportacao, iddocumentoxml, unidadeTrabalho, "Não foi possível salvar o Cancelamento de CT-e NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha);
                                                Servicos.Log.TratarErro("Não foi possível salvar o Cancelamento de CT-e NSU: " + dfe.NSU + " na consulta de CTe destinados: " + ex.ToString(), "BuscarCTEsDestinados");
                                                continue;
                                            }

                                        }
                                        else
                                        {
                                            Servicos.Log.GravarInfo("Evento não implementado DFe CTe. " + tpEvento, "BuscarCTEsDestinados");

                                            //110110 = CCe
                                            //110113 = EPEC
                                            //110160 = Registr do MultiModal
                                            //110170 = Informações da GTV

                                            //310620 = Registro de Passagem
                                            //510620 = Registro de Passagem Automatico
                                            //310610 = MDFe Autorizado
                                            //310611 = MDFe Cancelado

                                            //240130 = CTe Complementar Autorizado
                                            //240131 = CTe Complentar Cancelado
                                            //240140 = CTe Substituição Autorizado
                                            //240150 = CTe Anulação Autorizado
                                            //240160 = Liberaçao EPEC
                                            //240170 = Liberaçao Cancelamento

                                            //440130 = CTe Redespacho Autorizado
                                            //440140 = CTe Redespacho Intermediario Autorizado
                                            //440150 = CTe Subcontratação Autorizado
                                            //440160 = Autorizado Serviço Vinculado MultiModal

                                            //610110 = Prestação de Serviço em Desacordo
                                        }
                                    }
                                    else
                                    {
                                        Servicos.Log.GravarInfo("Schema não implementado DFe CTe: " + dfe.schema, "BuscarCTEsDestinados");
                                    }

                                    AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.Importado, iddocumentoxml, unidadeTrabalho, "XMl Importado", xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Servicos.Log.GravarInfo(string.Format(@"CTeDistribuicaoDFe, CNPJ Filial: {0} cStat: {1}, xMotivo: {2}.", empresa.CNPJ, retorno.cStat, retorno.xMotivo), "BuscarCTEsDestinados");
                    if (retorno.cStat == "656")
                        mensagemRetorno = "Retorno Sefaz: " + retorno.cStat + " " + retorno.xMotivo + " - Aguarde 60 minutos para realizar nova consulta.";
                    else
                        mensagemRetorno = "Retorno Sefaz: " + retorno.cStat + " " + retorno.xMotivo;
                    return false;
                }

                if (numeroNSU <= 0)
                {
                    configuracaoDocumentoDestinadoEmpresa.UltimoNSU = ultimoNSU;

                    repConfiguracaoDocumentoDestinadoEmpresa.Atualizar(configuracaoDocumentoDestinadoEmpresa);

                    repConfiguracaoDocumentoDestinadoEmpresa = null;
                    unidadeTrabalho.Dispose();
                    unidadeTrabalho = null;

                    GC.Collect();

                    unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
                    repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unidadeTrabalho);
                }

            } while (ultimoNSU < maxNSU && retorno.cStat == "138");

            mensagemRetorno = "Documentos Fiscais recebidos: " + quantidadeDfesObtidos;

            return true;
        }

        public static bool ObterDocumentosDestinadosMDFeEmpresa(int codigoEmpresa, string urlSefaz, string stringConexao, long numeroNSU, ref string mensagemRetorno, string caminhoDocumentosFiscais, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado repConfigDocumentoDestinado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado configuracaoDocumentoDestinado = repConfigDocumentoDestinado.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            if (string.IsNullOrWhiteSpace(urlSefaz))
            {
                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    if (empresa.Localidade.Estado.SefazMDFe == null)
                    {
                        mensagemRetorno = "Estado sem Sefaz configurado.";
                        return false;
                    }
                    urlSefaz = empresa.Localidade.Estado.SefazMDFe.UrlDistribuicaoDFe;
                }
                else
                {
                    if (empresa.Localidade.Estado.SefazMDFeHomologacao == null)
                    {
                        mensagemRetorno = "Estado sem Sefaz Homologação configurado.";
                        return false;
                    }
                    urlSefaz = empresa.Localidade.Estado.SefazMDFeHomologacao.UrlDistribuicaoDFe;
                }
            }

            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
            {
                mensagemRetorno = $"Empresa {empresa.CNPJ_Formatado}: Empresa/Certificado digital não configurado.";
                return false;
            }

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa configuracaoDocumentoDestinadoEmpresa = repConfiguracaoDocumentoDestinadoEmpresa.BuscarPorEmpresa(codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.MDFe);

            if (configuracaoDocumentoDestinadoEmpresa == null)
            {
                configuracaoDocumentoDestinadoEmpresa = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa();
                configuracaoDocumentoDestinadoEmpresa.Empresa = empresa;
                configuracaoDocumentoDestinadoEmpresa.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.MDFe;

                repConfiguracaoDocumentoDestinadoEmpresa.Inserir(configuracaoDocumentoDestinadoEmpresa);
            }

            long ultimoNSU = 0;

            ultimoNSU = configuracaoDocumentoDestinadoEmpresa.UltimoNSU;

            long maxNSU = 0L;

            if (numeroNSU > 0)
            {
                ultimoNSU = numeroNSU;
                maxNSU = numeroNSU;
            }

            MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.retDistDFeInt retorno = null;

            Dominio.Enumeradores.TipoAmbiente tipoAmbiente = empresa.TipoAmbiente;

            do
            {
                try
                {
                    retorno = ConsultarDocumentosFiscaisMDFe(unidadeTrabalho, empresa.CNPJ, ultimoNSU, tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.TAmb.Item1 : MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.TAmb.Item2, (MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.TCodUfIBGE)empresa.Localidade.Estado.CodigoIBGE, empresa.NomeCertificado, empresa.SenhaCertificado, empresa.NomeCertificadoKeyVault, urlSefaz, numeroNSU > 0 ? true : false);
                }
                catch (ServicoException ex)
                {
                    Log.GravarAdvertencia(ex.Message);
                    mensagemRetorno = ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    mensagemRetorno = ex.Message;
                    return false;
                }

                if (numeroNSU <= 0)
                {
                    maxNSU = long.Parse(retorno.maxNSU);
                    ultimoNSU = long.Parse(retorno.ultNSU);
                }

                if (retorno.cStat == "138")
                {
                    foreach (MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe in retorno.loteDistDFeInt.docZip)
                    {
                        Int64 iddocumentoxml = 0;
                        string xmlDocumento = string.Empty;

                        using (MemoryStream compressedStream = new MemoryStream(dfe.Value))
                        {
                            using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                            {
                                using (MemoryStream resultStream = new MemoryStream())
                                {
                                    zipStream.CopyTo(resultStream);

                                    resultStream.Position = 0;

                                    StreamReader reader = new StreamReader(resultStream);
                                    xmlDocumento = reader.ReadToEnd();

                                    GravarXmlDistribucaoDFe(empresa, null, null, dfe, resultStream, SituacaoXml.ProblemaImportacao, unidadeTrabalho, xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha, out iddocumentoxml);

                                    resultStream.Position = 0;

                                    if (dfe.schema == "procMDFe_v3.00.xsd")
                                    {
                                        XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.MDFe.v300.mdfeProc));

                                        string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());

                                        MultiSoftware.MDFe.v300.mdfeProc mdfeProc = (ser.Deserialize(resultStream) as MultiSoftware.MDFe.v300.mdfeProc);

                                        if (SalvarProcMDFe(dfe, mdfeProc, resultStream, empresa, unidadeTrabalho, tipoServicoMultisoftware, configuracao.GerarCargaMDFeDestinado))
                                        {
                                            try
                                            {
                                                resultStream.Position = 0;

                                                string caminho = string.Empty;

                                                if (!string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                                                    caminho = caminhoDocumentosFiscais;
                                                else
                                                    caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                                                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "MDFe", empresa.CNPJ, mdfeProc.protMDFe.infProt.chMDFe + ".xml");

                                                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());
                                            }
                                            catch (Exception ex)
                                            {
                                                Servicos.Log.TratarErro("Não foi possível salvar MDF-e da consulta de documentos destinados: " + ex.ToString());
                                            }
                                        }
                                    }
                                    else if (dfe.schema == "procEventoMDFe_v3.00.xsd")
                                    {
                                        resultStream.Position = 0;

                                        System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(resultStream);
                                        System.Xml.Linq.XNamespace ns = doc.Root.Name.Namespace;

                                        string tpEvento = doc.Descendants(ns + "infEvento").FirstOrDefault()?.Element(ns + "tpEvento")?.Value ?? string.Empty;

                                        resultStream.Position = 0;

                                        if (tpEvento == "110111" || tpEvento == "110112") //Cancelamento e Encerramento
                                        {
                                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.MDFe.v300.TProcEvento));

                                            MultiSoftware.MDFe.v300.TProcEvento procEvento = (ser.Deserialize(resultStream) as MultiSoftware.MDFe.v300.TProcEvento);

                                            if (SalvarProcEventoMDFe(dfe, procEvento, (tpEvento == "110111" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDFECancelado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.EncerramentoMDFe), empresa.Codigo, unidadeTrabalho))
                                            {
                                                try
                                                {
                                                    resultStream.Position = 0;

                                                    string caminho = string.Empty;

                                                    if (!string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                                                        caminho = caminhoDocumentosFiscais;
                                                    else
                                                        caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                                                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "MDFe", empresa.CNPJ);

                                                    if (tpEvento == "110111")
                                                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, procEvento.eventoMDFe.infEvento.chMDFe + "_procCancMDFe.xml");
                                                    else
                                                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, procEvento.eventoMDFe.infEvento.chMDFe + "_procEncMDFe.xml");

                                                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());
                                                }
                                                catch (Exception ex)
                                                {
                                                    Servicos.Log.TratarErro("Não foi possível salvar MDF-e da consulta de documentos destinados: " + ex.ToString());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Servicos.Log.TratarErro("Evento não implementado DFe MDFe. " + tpEvento, "DestinadosMDFe");
                                        }
                                    }
                                    else
                                    {
                                        Servicos.Log.TratarErro("Schema não implementado DFe MDFe. " + dfe.schema, "DestinadosMDFe");
                                    }

                                    AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml.Importado, iddocumentoxml, unidadeTrabalho, "XMl Importado", xmlDocumento, configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Servicos.Log.TratarErro("Retorno " + retorno.cStat + " " + retorno.xMotivo, "DestinadosMDFe");
                    if (retorno.cStat == "656")
                        mensagemRetorno = "Retorno Sefaz: " + retorno.cStat + " " + retorno.xMotivo + " - Aguarde 60 minutos para realizar nova consulta.";
                    else
                        mensagemRetorno = "Retorno Sefaz: " + retorno.cStat + " " + retorno.xMotivo;
                    return false;
                }

                if (numeroNSU <= 0)
                {
                    configuracaoDocumentoDestinadoEmpresa.UltimoNSU = ultimoNSU;

                    repConfiguracaoDocumentoDestinadoEmpresa.Atualizar(configuracaoDocumentoDestinadoEmpresa);

                    repConfiguracaoDocumentoDestinadoEmpresa = null;
                    unidadeTrabalho.Dispose();
                    unidadeTrabalho = null;

                    GC.Collect();

                    unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
                    repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unidadeTrabalho);
                }

            } while (ultimoNSU < maxNSU && retorno.cStat == "138");

            return true;
        }

        public static List<object> ObterObjetoValorParaEmissao(int codigoEmpresa, List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosSelecionados, Dominio.Entidades.Usuario usuario, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            List<object> nfesConvertidas = new List<object>();

            Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc");

            for (var i = 0; i < documentosSelecionados.Count; i++)
            {
                string caminhoNFe = Utilidades.IO.FileStorageService.Storage.Combine(caminho, documentosSelecionados[i].Chave + ".xml");
                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoNFe))
                    continue;

                Stream xml = Utilidades.IO.FileStorageService.Storage.OpenRead(caminhoNFe);
                object documento = svcNFe.ObterDocumentoPorXML(xml, codigoEmpresa, usuario, unitOfWork);
                if (documento != null)
                    nfesConvertidas.Add(documento);
            }

            return nfesConvertidas;
        }

        public MemoryStream ObterLoteDeXMLNFe(List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentoDestinados, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
            foreach (Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado in documentoDestinados)
            {
                string caminhoXML = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");
                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                {
                    DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, StringConexao, null, out string msgErro, out string codigoStatusRetornoSefaz, documentoDestinado.NumeroSequencialUnico, null, null, documentoDestinado.Chave);
                }

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                {
                    byte[] arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoXML);
                    ZipEntry entry = new ZipEntry(string.Concat("NFe", documentoDestinado.Chave, ".xml"));
                    entry.DateTime = DateTime.Now;
                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(arquivo, 0, arquivo.Length);
                    zipOStream.CloseEntry();
                }
            }

            documentoDestinados = null;

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        public MemoryStream ObterLoteDeXMLCTe(List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentoDestinados, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
            foreach (Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado in documentoDestinados)
            {
                string caminhoXML = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", documentoDestinado.Empresa.CNPJ, documentoDestinado.Chave + ".xml");
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                {
                    byte[] arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoXML);
                    ZipEntry entry = new ZipEntry(string.Concat("CTe", documentoDestinado.Chave, ".xml"));
                    entry.DateTime = DateTime.Now;
                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(arquivo, 0, arquivo.Length);
                    zipOStream.CloseEntry();
                }
            }

            documentoDestinados = null;

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        public static string ObterCaminhoDocumentoDestinado(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

            string modelo = documentoDestinado.Chave.Substring(20, 2);

            if (modelo == "57")
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", documentoDestinado.Empresa.CNPJ, documentoDestinado.Chave + ".xml");
            else if (modelo == "55")
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");
            else
                caminho = string.Empty;

            return caminho;
        }

        public static Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ObterXMLNotaFiscal(string cpfCnpjEmitente, int numeroNota, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa = repDocumentoDestinadoEmpresa.BuscarPorNumeroEEmitente(numeroNota, cpfCnpjEmitente);

            if (documentoDestinadoEmpresa == null)
                return null;

            return ObterXMLNotaFiscal(documentoDestinadoEmpresa, unitOfWork, tipoServicoMultisoftware);
        }

        public static Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ObterXMLNotaFiscal(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repControleNotaDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            string caminhoXML = ObterCaminhoDocumentoDestinado(documentoDestinadoEmpresa, unitOfWork);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                return null;

            try
            {
                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe();

                using (Stream fs = Utilidades.IO.FileStorageService.Storage.OpenRead(caminhoXML))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        string stringNotaFiscal = sr.ReadToEnd();

                        if (serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, sr, unitOfWork, null, true, false, false, tipoServicoMultisoftware, false, configuracao?.UtilizarValorFreteNota ?? false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                        {
                            repXMLNotaFiscal.Inserir(xmlNotaFiscal);

                            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao controleNotaDevolucao = repControleNotaDevolucao.BuscarPorChave(xmlNotaFiscal.Chave);
                            if (controleNotaDevolucao != null)
                            {
                                controleNotaDevolucao.XMLNotaFiscal = xmlNotaFiscal;
                                controleNotaDevolucao.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusControleNotaDevolucao.ComNotaFiscal;

                                repControleNotaDevolucao.Atualizar(controleNotaDevolucao);
                            }
                            serNFe.SalvarProdutosNota(stringNotaFiscal, xmlNotaFiscal, null, tipoServicoMultisoftware.HasValue ? tipoServicoMultisoftware.Value : AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork);
                            return xmlNotaFiscal;
                        }
                        else
                        {
                            Servicos.Log.TratarErro(caminhoXML);
                            Servicos.Log.TratarErro(erro);
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(caminhoXML);
                Servicos.Log.TratarErro(ex);
                //Utilidades.IO.FileStorageService.Storage.Delete(caminhoXML);
                return null;
            }
        }

        public static void CriarXMLNotaFiscal(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware)
        {
            if (documentoDestinadoEmpresa.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeTransporte)
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (repXMLNotaFiscal.ExistePorChave(documentoDestinadoEmpresa.Chave))
                return;

            if (ObterXMLNotaFiscal(documentoDestinadoEmpresa, unitOfWork, tipoServicoMultisoftware) == null)
                Servicos.Log.TratarErro($"Não foi possível criar o registro XMLNotaFiscal para a nota {documentoDestinadoEmpresa.Chave}.");
        }

        public static string AprovarEmissaoDesacordo(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado, Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo motivo, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento)
        {
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
            Repositorio.Embarcador.Documentos.HistoricoIrregularidade repHistoricoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

            Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade svcIrregularidade = new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);

            Dominio.Entidades.Estado estadoEmissorDocumento = repEstado.BuscarPorIBGE(int.Parse(documentoDestinado.Chave.Substring(0, 2)));
            string url = "";
            string mensagemRetorno = "";

            if (estadoEmissorDocumento == null)
                return "Estado do emissor do documento não encontrado.";


            if (documentoDestinado.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
            {
                if (estadoEmissorDocumento.SefazCTe == null)
                    return "Estado do emissor do documento sem Sefaz configurado.";
                url = estadoEmissorDocumento.SefazCTe.UrlRecepcaoEvento;
            }
            else
            {
                if (estadoEmissorDocumento.SefazCTeHomologacao == null)
                    return "Estado do emissor do documento sem Sefaz Homologação configurado.";
                url = estadoEmissorDocumento.SefazCTeHomologacao.UrlRecepcaoEvento;
            }

            if (string.IsNullOrWhiteSpace(url))
                return "Sefaz do emissor do documento sem URL de Recepção de Evento configurado.";

            try
            {

                if (Servicos.Embarcador.Documentos.DesacordoPrestacaoServicoCTe.EmitirDesacordoServico(documentoDestinado.Empresa.Codigo, documentoDestinado.Chave, string.Empty, url, ref mensagemRetorno, unitOfWork))
                {
                    documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.DescordoServico;
                    Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historico = repHistoricoIrregularidade.BuscarAtualPorControleDocumento(controleDocumento.Codigo);

                    if (historico?.Irregularidade == null)
                        svcIrregularidade.CriarIrregularidadeIndividualDesacordo(controleDocumento);

                    if ((documentoDestinado.Empresa?.EmissaoDocumentosForaDoSistema ?? false) && (motivo?.SubstituiCTe ?? false))
                        RemoverDocumentoDaCarga(documentoDestinado.Chave, unitOfWork);

                    repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, documentoDestinado, null, "Emitiu desacordo.", unitOfWork);
                    return string.Empty;
                }
                else
                    return mensagemRetorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                if (ex.Message.Contains("Certificado"))
                    return ex.Message;
                else
                    return "Não foi possível aprovar o desacordo";
            }
        }

        public StatusNotaRetornoSefaz ConsultarStatusNotaSefazPorChave(Repositorio.UnitOfWork unitOfWork, ConsultaNotaSefaz dadosConsultar)
        {
            MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeInt retornoSefazNFe = ConsultarDocumentosFiscaisNFe(unitOfWork, dadosConsultar.CNPJ, 0, dadosConsultar.CodigoIBGE, dadosConsultar.NomeCertificado, dadosConsultar.SenhaCertificado, dadosConsultar.NomeCertificadoKeyVault, false, dadosConsultar.TipoAmbiente, dadosConsultar.Chave);
            return StatusNotaRetornoSefazHelper.ObterStatusSefas(retornoSefazNFe?.cStat ?? "");
        }

        public static void BuscarCTEsDestinados(string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {

            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.BuscarQueNaoImportaDocumentoDestinadoTransporte();
                List<string> cnpjsNaoImportar = (from obj in grupoPessoas select obj.Clientes.Select(o => o.CPF_CNPJ_SemFormato)).SelectMany(o => o).ToList();
                List<int> codigosEmpresas = new List<int>();

                codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasSincronismoDocumentosDestinados();

                foreach (int codigoEmpresa in codigosEmpresas)
                {
                    try
                    {
                        Servicos.Log.TratarErro("Buscando empresa " + codigoEmpresa.ToString(), "BuscarCTEsDestinados");
                        string msgErro = string.Empty;

                        if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosCTeEmpresa(codigoEmpresa, string.Empty, stringConexao, 0, ref msgErro, out string codigoStatusRetornoSefaz, null, tipoServicoMultisoftware))
                        {
                            Servicos.Log.TratarErro("Erro consultando documentos CT-e: " + msgErro, "BuscarCTEsDestinados");
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "BuscarCTEsDestinados");
                        continue;
                    }
                }

                unitOfWork.Dispose();
                unitOfWork = null;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "BuscarCTEsDestinados");
            }
            finally
            {

            }

        }

        public static void BuscarDocumentosDestinados(string stringConexao)
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.BuscarQueNaoImportaDocumentoDestinadoTransporte();
                List<string> cnpjsNaoImportar = (from obj in grupoPessoas select obj.Clientes.Select(o => o.CPF_CNPJ_SemFormato)).SelectMany(o => o).ToList();
                List<int> codigosEmpresas = new List<int>();

                codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasSincronismoDocumentosDestinados();

                foreach (int codigoEmpresa in codigosEmpresas)
                {
                    Servicos.Log.TratarErro("Buscando empresa " + codigoEmpresa.ToString(), "BuscarDocumentosDestinados");
                    try
                    {
                        if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(codigoEmpresa, stringConexao, cnpjsNaoImportar, out string msgErro, out string codigoStatusRetornoSefaz, 0))
                        {
                            Servicos.Log.TratarErro("Erro consultando documentos: " + msgErro, "BuscarDocumentosDestinados");
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Erro buscando empresa " + codigoEmpresa.ToString() + " - " + ex, "BuscarDocumentosDestinados");
                    }
                }

                unitOfWork.Dispose();
                unitOfWork = null;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {

            }
        }

        public static void BuscarIntegracaoDocumentosDestinados(string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                List<int> codigosEmpresas = new List<int>();
                codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasIntegracaoDocumentosDestinados();
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null;
                foreach (int codigoEmpresa in codigosEmpresas)
                {
                    Servicos.Log.TratarErro("Inicio empresa: " + codigoEmpresa.ToString(), "IntegracaoDocumentosDestinados");

                    Auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                    Auditado.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    Auditado.Integradora = null;
                    Auditado.IP = "";
                    Auditado.Texto = "";
                    Auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                    Auditado.Usuario = null;

                    if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.EnviarIMPUTDocumentosDestinadosEmpresa(codigoEmpresa, stringConexao, out string msgErro, Auditado))
                        Servicos.Log.TratarErro("Erro Enviar IMPUT: " + msgErro, "IntegracaoDocumentosDestinados");
                    if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ProcessarOUTPUTDocumentosDestinadosEmpresa(codigoEmpresa, stringConexao, out string msgErroOUTPUT, Auditado, tipoServicoMultisoftware))
                        Servicos.Log.TratarErro("Erro Processar OUTPUT: " + msgErroOUTPUT, "IntegracaoDocumentosDestinados");
                    if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ProcessarXMLDocumentosDestinadosEmpresa(codigoEmpresa, stringConexao, out string msgErroOUTPUTImp, Auditado, tipoServicoMultisoftware))
                        Servicos.Log.TratarErro("Erro Processar OUTPUT: " + msgErroOUTPUTImp, "IntegracaoDocumentosDestinados");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoDocumentosDestinados");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWork = null;
            }


        }


        #endregion

        #region Metodos Privados NFe

        private static void SalvarResNFe(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe resNFe, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, string nsu = "")
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(codigoEmpresa, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada }, resNFe.chNFe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.Empresa = new Dominio.Entidades.Empresa() { Codigo = codigoEmpresa };
                documento.DataIntegracao = DateTime.Now;
            }

            documento.Chave = resNFe.chNFe;
            documento.CPFCNPJEmitente = resNFe.Item;
            documento.DataAutorizacao = DateTime.ParseExact(resNFe.dhRecbto, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.DataEmissao = DateTime.ParseExact(resNFe.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.IEEmitente = resNFe.IE;
            documento.NomeEmitente = resNFe.xNome;
            documento.Numero = int.Parse(resNFe.chNFe.Substring(25, 9));
            documento.Serie = int.Parse(resNFe.chNFe.Substring(22, 3));
            documento.NumeroSequencialUnico = !string.IsNullOrWhiteSpace(nsu) ? long.Parse(nsu) : long.Parse(dfe.NSU);
            documento.Protocolo = resNFe.nProt;
            documento.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada;
            documento.TipoOperacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe)(int)resNFe.tpNF;
            documento.Valor = decimal.Parse(resNFe.vNF, cultura);
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe;
            if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                documento.Cancelado = true;

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            if (configuracaoTMS != null && configuracaoTMS.InformarCienciaOperacaoDocumentoDestinado && documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada && (documento.Manifestacoes == null || documento.Manifestacoes.Count == 0))
            {
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                Servicos.Embarcador.Documentos.ManifestacaoDestinatario.EmitirManifestacaoDestinatario(out string erro, documento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.CienciaOperacao, string.Empty, unidadeTrabalho, auditado);
            }
        }

        private static bool SalvarProcNFe(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfeProc, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, List<string> cnpjsNaoImportarTransporte, out Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento, out string kmObservacao, out string placaObservacao, out string horimetroObservacao, out string chassiObservacao)
        {
            documento = null;
            kmObservacao = "";
            placaObservacao = "";
            horimetroObservacao = "";
            chassiObservacao = "";

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento;
            if (nfeProc.NFe?.infNFe?.dest?.Item == empresa.CNPJ)
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada;
            else if (nfeProc.NFe?.infNFe?.transp?.transporta?.Item == empresa.CNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeTransporte;
                if (cnpjsNaoImportarTransporte != null && cnpjsNaoImportarTransporte.Count() > 0 && !string.IsNullOrWhiteSpace(nfeProc.NFe?.infNFe?.emit?.Item) && cnpjsNaoImportarTransporte.Contains(nfeProc.NFe?.infNFe?.emit?.Item))
                    return false;
            }
            else
                throw new Exception("Consulta de notas emitidas na SEFAZ: A empresa não é nem destinatário e nem transportador da NF-e processada. Chave: " + nfeProc.protNFe.infProt.chNFe);

            //#66774 Ajustar Depois
            //if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
            //{
            //    if (string.IsNullOrEmpty(nfeProc?.protNFe?.infProt?.Id ?? ""))
            //        throw new Exception("Campo ID do infProt não foi informado");

            //    if (string.IsNullOrEmpty(nfeProc?.versao ?? string.Empty))
            //        throw new Exception("Versão do nfeProc não informada");
            //}

            documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(empresa.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, nfeProc.protNFe.infProt.chNFe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.Empresa = empresa;
                documento.DataIntegracao = DateTime.Now;
            }
            documento.Chave = nfeProc.protNFe.infProt.chNFe;
            documento.CPFCNPJEmitente = nfeProc.NFe.infNFe.emit.Item;
            documento.DataAutorizacao = DateTime.ParseExact(nfeProc.protNFe.infProt.dhRecbto, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.DataEmissao = DateTime.ParseExact(nfeProc.NFe.infNFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.IEEmitente = nfeProc.NFe.infNFe.emit.IE;
            documento.NomeEmitente = nfeProc.NFe.infNFe.emit.xNome;
            documento.Numero = int.Parse(nfeProc.NFe.infNFe.ide.nNF);
            documento.Serie = int.Parse(nfeProc.NFe.infNFe.ide.serie);
            documento.NumeroSequencialUnico = long.Parse(dfe.NSU);
            documento.Protocolo = nfeProc.protNFe.infProt.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe)(int)nfeProc.NFe.infNFe.ide.tpNF;
            documento.Valor = decimal.Parse(nfeProc.NFe.infNFe.total.ICMSTot.vNF, cultura);
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe;
            documento.Placa = ObterPlacaVeiculo(nfeProc.NFe?.infNFe?.transp);

            documento.UFDestinatario = nfeProc.NFe.infNFe.dest.enderDest.UF.ToString("g");

            if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                documento.Cancelado = true;

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);



            new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);

            return true;
        }

        private static bool SalvarProcNFe(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfeProc, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, List<string> cnpjsNaoImportarTransporte, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware, out Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento, out string kmObservacao, out string placaObservacao, out string horimetroObservacao, out string chassiObservacao)
        {
            documento = null;
            kmObservacao = "";
            placaObservacao = "";
            horimetroObservacao = "";
            chassiObservacao = "";

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new Servicos.Embarcador.Financeiro.DocumentoEntrada(unidadeTrabalho);

            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto repDocumentoDestinadoEmpresaProduto = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento;
            if (nfeProc.NFe?.infNFe?.dest?.Item == empresa.CNPJ)
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada;
            else if (nfeProc.NFe?.infNFe?.transp?.transporta?.Item == empresa.CNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeTransporte;
                if (cnpjsNaoImportarTransporte != null && cnpjsNaoImportarTransporte.Count() > 0 && !string.IsNullOrWhiteSpace(nfeProc.NFe?.infNFe?.emit?.Item) && cnpjsNaoImportarTransporte.Contains(nfeProc.NFe?.infNFe?.emit?.Item))
                    return false;
            }
            else
                throw new Exception("Consulta de notas emitidas na SEFAZ: A empresa não é nem destinatário e nem transportador da NF-e processada. Chave: " + nfeProc.protNFe.infProt.chNFe);

            //#66774 Ajustar Depois
            //if (string.IsNullOrEmpty(nfeProc.protNFe.infProt.Id))
            //    throw new Exception("Campo ID do infProt não foi informado");

            //if (string.IsNullOrEmpty(nfeProc.versao))
            //    throw new Exception("Versão do nfeProc não informada");

            documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(empresa.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, nfeProc.protNFe.infProt.chNFe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.Empresa = empresa;
                documento.DataIntegracao = DateTime.Now;
            }
            documento.Chave = nfeProc.protNFe.infProt.chNFe;
            documento.CPFCNPJEmitente = nfeProc.NFe.infNFe.emit.Item;
            documento.DataAutorizacao = DateTime.ParseExact(nfeProc.protNFe.infProt.dhRecbto, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.DataEmissao = DateTime.ParseExact(nfeProc.NFe.infNFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.IEEmitente = nfeProc.NFe.infNFe.emit.IE;
            documento.NomeEmitente = nfeProc.NFe.infNFe.emit.xNome;
            documento.Numero = int.Parse(nfeProc.NFe.infNFe.ide.nNF);
            documento.Serie = int.Parse(nfeProc.NFe.infNFe.ide.serie);
            documento.NumeroSequencialUnico = long.Parse(dfe.NSU);
            documento.Protocolo = nfeProc.protNFe.infProt.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe)(int)nfeProc.NFe.infNFe.ide.tpNF;
            documento.Valor = decimal.Parse(nfeProc.NFe.infNFe.total.ICMSTot.vNF, cultura);
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe;
            documento.Placa = ObterPlacaVeiculo(nfeProc.NFe?.infNFe?.transp);
            documento.Observacao = (nfeProc.NFe?.infNFe?.infAdic?.infAdFisco ?? "");
            documento.Observacao += " " + (nfeProc.NFe?.infNFe?.infAdic?.infCpl ?? "");
            documento.UFDestinatario = nfeProc.NFe.infNFe.dest.enderDest.UF.ToString("g");
            documento.Emitente = !string.IsNullOrEmpty(documento.CPFCNPJEmitente) && documento.CPFCNPJEmitente.ToDouble() > 0 ? repCliente.BuscarPorCPFCNPJ(documento.CPFCNPJEmitente.ToDouble()) : null;

            if (documento.Emitente != null)
                svcDocumentoEntrada.RetornaTagAbastecimento(out kmObservacao, out placaObservacao, out horimetroObservacao, out chassiObservacao, nfeProc, documento.Emitente);

            if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                documento.Cancelado = true;

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
            {
                repDocumentoDestinadoEmpresa.Inserir(documento);

                if (nfeProc.NFe.infNFe.det != null && nfeProc.NFe.infNFe.det.Length > 0)
                {
                    for (int i = 0; i < nfeProc.NFe.infNFe.det.Length; i++)
                    {
                        if (nfeProc.NFe.infNFe.det[i].prod != null)
                        {
                            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto prod = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto()
                            {
                                cProd = nfeProc.NFe.infNFe.det[i].prod.cProd,
                                DocumentoDestinadoEmpresa = documento,
                                qCom = nfeProc.NFe.infNFe.det[i].prod.qCom != null ? decimal.Parse(nfeProc.NFe.infNFe.det[i].prod.qCom, cultura) : 0m,
                                uCom = nfeProc.NFe.infNFe.det[i].prod.uCom,
                                vProd = nfeProc.NFe.infNFe.det[i].prod.vProd != null ? decimal.Parse(nfeProc.NFe.infNFe.det[i].prod.vProd, cultura) : 0m,
                                vUnCom = nfeProc.NFe.infNFe.det[i].prod.vUnCom != null ? decimal.Parse(nfeProc.NFe.infNFe.det[i].prod.vUnCom, cultura) : 0m,
                                xProd = nfeProc.NFe.infNFe.det[i].prod.xProd,
                                NCM = nfeProc.NFe.infNFe.det[i].prod.NCM
                            };
                            repDocumentoDestinadoEmpresaProduto.Inserir(prod);
                        }
                    }
                }
            }

            new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);

            return true;
        }

        private static void VincularDocumentoACarga(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Pedido.NotaFiscal(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Texto = "Adicionado via documento destinado"
            };

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);

            if (!double.TryParse(documentoDestinadoEmpresa.CPFCNPJEmitente, out double cpfCnpjEmitente))
                return;

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = ObterXMLNotaFiscal(documentoDestinadoEmpresa, unitOfWork, tipoServicoMultisoftware);

            if (xmlNotaFiscal == null)
                return;

            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroControlePedido))
                serCargaNotaFiscal.VincularNotaFiscalAPedidosPorNumeroControle(xmlNotaFiscal, configuracaoTMS, auditado, tipoServicoMultisoftware);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> cargaPedidoXMLNotaFiscaisParciais = repCargaPedidoXMLNotaFiscalParcial.BuscarPorNumeroSemCarga(documentoDestinadoEmpresa.Numero, cpfCnpjEmitente);
            if (!cargaPedidoXMLNotaFiscaisParciais.Any())
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            List<int> codigosCargas = new List<int>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial in cargaPedidoXMLNotaFiscaisParciais)
            {
                cargaPedidoXMLNotaFiscalParcial.XMLNotaFiscal = xmlNotaFiscal;
                xmlNotaFiscal.TipoNotaFiscalIntegrada = cargaPedidoXMLNotaFiscalParcial.TipoNotaFiscalIntegrada;

                repCargaPedidoXMLNotaFiscalParcial.Atualizar(cargaPedidoXMLNotaFiscalParcial);
                repXMLNotaFiscal.Atualizar(xmlNotaFiscal);

                serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedidoXMLNotaFiscalParcial.CargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoDeCarga);

                Servicos.Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, "Adicionado via Documentos Destinados", unitOfWork);

                if (configuracaoTMS.AvancarEtapaDocumentosEmissaoAoVincularTodasNotasParciaisCarga && !codigosCargas.Contains(cargaPedidoXMLNotaFiscalParcial.CargaPedido.Carga.Codigo))
                    codigosCargas.Add(cargaPedidoXMLNotaFiscalParcial.CargaPedido.Carga.Codigo);
            }

            if (configuracaoTMS.AvancarEtapaDocumentosEmissaoAoVincularTodasNotasParciaisCarga)
            {
                foreach (int codigoCarga in codigosCargas)
                {
                    if (repCargaPedidoXMLNotaFiscalParcial.VerificarSeExisteNotaParcialSemNota(codigoCarga))
                        continue;

                    unitOfWork.FlushAndClear();

                    unitOfWork.Start();

                    if (!Servicos.Embarcador.Carga.Carga.AvancarEtapaDocumentosEmissaoCarga(out string erro, codigoCarga, false, configuracaoTMS, tipoServicoMultisoftware, unitOfWork))
                        Servicos.Log.TratarErro($"Não foi possível avançar automaticamente a etapa de documentos para emissão da carga (código {codigoCarga}), motivo: {erro}");

                    unitOfWork.CommitChanges();
                }
            }
        }

        private static string ObterPlacaVeiculo(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.Items != null)
                {
                    string placa = string.Empty;

                    foreach (var item in infNFeTransp.Items)
                        if (item.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscal.TVeiculo))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TVeiculo veiculo = (MultiSoftware.NFe.v400.NotaFiscal.TVeiculo)item;
                            return veiculo.placa;
                        }
                }
            }
            return string.Empty;
        }

        private static string ObterPlacaVeiculo(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.Items != null)
                {
                    string placa = string.Empty;

                    foreach (var item in infNFeTransp.Items)
                        if (item.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscal.TVeiculo))
                        {
                            MultiSoftware.NFe.v310.NotaFiscal.TVeiculo veiculo = (MultiSoftware.NFe.v310.NotaFiscal.TVeiculo)item;
                            return veiculo.placa;
                        }
                }
            }
            return string.Empty;
        }

        private static void SalvarResEvento(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento resEvento, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CCe;

            bool eventoImplementado = true;

            if (resEvento.tpEvento == "110110")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CCe;
            else if (resEvento.tpEvento == "110111")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoNFe;
            else if (resEvento.tpEvento == "610600")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizacaoCTe;
            else if (resEvento.tpEvento == "610601")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe;
            else if (resEvento.tpEvento == "610610")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizacaoMDFe;
            else if (resEvento.tpEvento == "610611")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoMDFe;
            else if (resEvento.tpEvento == "610614")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizacaoMDFeComCTe;
            else if (resEvento.tpEvento == "610550")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFeRFID;
            else if (resEvento.tpEvento == "610500")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFe;
            else if (resEvento.tpEvento == "610501")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoPassagemNFe;
            else if (resEvento.tpEvento == "610514")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFePropagadoPeloMDFeOuCTe;
            else if (resEvento.tpEvento == "610554")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFeAutomaticoPeloMDFeOuCTe;
            else if (resEvento.tpEvento == "610615")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoMDFeAutorizadoComCTe;
            else if (resEvento.tpEvento == "610552")
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemAutomaticoMDFe;
            else
            {
                Servicos.Log.GravarInfo("Retorno para o tipo de evento não implementado: " + resEvento.tpEvento + " " + resEvento.xEvento);
                eventoImplementado = false;
            }

            if (eventoImplementado)
            {
                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(codigoEmpresa, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, resEvento.chNFe);

                if (documento == null)
                {
                    documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                    documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                    documento.Empresa = new Dominio.Entidades.Empresa() { Codigo = codigoEmpresa };
                    documento.DataIntegracao = DateTime.Now;
                }

                documento.Chave = resEvento.chNFe;
                documento.CPFCNPJEmitente = resEvento.Item;
                documento.DataAutorizacao = DateTime.ParseExact(resEvento.dhRecbto, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
                documento.DataEmissao = DateTime.ParseExact(resEvento.dhEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
                documento.Numero = int.Parse(resEvento.chNFe.Substring(25, 9));
                documento.Serie = int.Parse(resEvento.chNFe.Substring(22, 3));
                documento.NumeroSequencialUnico = long.Parse(dfe.NSU);
                documento.NumeroSequencialEvento = int.Parse(resEvento.nSeqEvento);
                documento.Protocolo = resEvento.nProt;
                documento.TipoDocumento = tipoDocumento;
                documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Saida;
                documento.DescricaoEvento = resEvento.xEvento;
                documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe;
                if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                    documento.Cancelado = true;

                if (documento.Codigo > 0)
                    repDocumentoDestinadoEmpresa.Atualizar(documento);
                else
                    repDocumentoDestinadoEmpresa.Inserir(documento);

                if (tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoNFe)
                    new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);
            }
        }

        private static long SalvarProcEvento(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.NFe.Evento.Cancelamento.Retorno.TProcEvento procEvento, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.Embarcador.Pedido.NotaFiscal serPedidoNotaFiscal = new Pedido.NotaFiscal(unidadeTrabalho);
            Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica servicoNotaFiscal = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unidadeTrabalho);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);


            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(codigoEmpresa, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoNFe }, procEvento.retEvento.infEvento.chNFe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.Empresa = new Dominio.Entidades.Empresa() { Codigo = codigoEmpresa };
                documento.DataIntegracao = DateTime.Now;
            }

            documento.Chave = procEvento.retEvento.infEvento.chNFe;
            documento.CPFCNPJEmitente = procEvento.retEvento.infEvento.Item;
            documento.DataAutorizacao = DateTime.ParseExact(procEvento.retEvento.infEvento.dhRegEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.DataEmissao = DateTime.ParseExact(procEvento.evento.infEvento.dhEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.Numero = int.Parse(procEvento.retEvento.infEvento.chNFe.Substring(25, 9));
            documento.Serie = int.Parse(procEvento.retEvento.infEvento.chNFe.Substring(22, 3));
            documento.NumeroSequencialUnico = long.Parse(dfe.NSU);
            documento.NumeroSequencialEvento = int.Parse(procEvento.retEvento.infEvento.nSeqEvento);
            documento.Protocolo = procEvento.retEvento.infEvento.nProt;
            documento.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoNFe;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Saida;
            documento.DescricaoEvento = procEvento.retEvento.infEvento.xEvento;
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe;

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoOriginario = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(documento.Empresa.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada }, documento.Chave);

            if (documentoOriginario != null)
            {
                documentoOriginario.Cancelado = true;
                documentoOriginario.GerouArquivoIntegracao = false;
                repDocumentoDestinadoEmpresa.Atualizar(documentoOriginario);
            }

            serPedidoNotaFiscal.SalvarEventoNFParaEmissaoCancelada(documento.Chave);
            servicoNotaFiscal.AtualizarStatusCanceladaNotaFiscal(documento.Chave, unidadeTrabalho);

            new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoNFe);

            return documento.Codigo;
        }

        private static void SalvarProcEvento(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.NFe.Evento.ManifestacaoDestinatario.EventoProcessado.TProcEvento procEvento, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(codigoEmpresa, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDe }, procEvento.retEvento.infEvento.chNFe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.Empresa = new Dominio.Entidades.Empresa() { Codigo = codigoEmpresa };
                documento.DataIntegracao = DateTime.Now;
            }

            documento.Chave = procEvento.retEvento.infEvento.chNFe;
            documento.CPFCNPJEmitente = procEvento.retEvento.infEvento.Item;
            documento.DataAutorizacao = DateTime.ParseExact(procEvento.retEvento.infEvento.dhRegEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.DataEmissao = DateTime.ParseExact(procEvento.evento.infEvento.dhEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.Numero = int.Parse(procEvento.retEvento.infEvento.chNFe.Substring(25, 9));
            documento.Serie = int.Parse(procEvento.retEvento.infEvento.chNFe.Substring(22, 3));
            documento.NumeroSequencialUnico = long.Parse(dfe.NSU);
            documento.NumeroSequencialEvento = int.Parse(procEvento.retEvento.infEvento.nSeqEvento);
            documento.Protocolo = procEvento.retEvento.infEvento.nProt;
            documento.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDe;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Saida;
            documento.DescricaoEvento = procEvento.retEvento.infEvento.xEvento;
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe;

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

        }

        private static long SalvarProcEvento(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.NFe.Evento.CartaCorrecaoEletronica.EventoProcessado.TProcEvento procEvento, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(codigoEmpresa, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDe }, procEvento.retEvento.infEvento.chNFe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.Empresa = new Dominio.Entidades.Empresa() { Codigo = codigoEmpresa };
                documento.DataIntegracao = DateTime.Now;
            }

            documento.Chave = procEvento.retEvento.infEvento.chNFe;
            documento.CPFCNPJEmitente = procEvento.retEvento.infEvento.Item;
            documento.DataAutorizacao = DateTime.ParseExact(procEvento.retEvento.infEvento.dhRegEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.DataEmissao = DateTime.ParseExact(procEvento.evento.infEvento.dhEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.Numero = int.Parse(procEvento.retEvento.infEvento.chNFe.Substring(25, 9));
            documento.Serie = int.Parse(procEvento.retEvento.infEvento.chNFe.Substring(22, 3));
            documento.NumeroSequencialUnico = long.Parse(dfe.NSU);
            documento.NumeroSequencialEvento = int.Parse(procEvento.retEvento.infEvento.nSeqEvento);
            documento.Protocolo = procEvento.retEvento.infEvento.nProt;
            documento.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CCe;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Saida;
            documento.DescricaoEvento = procEvento.retEvento.infEvento.xEvento;
            documento.Correcao = procEvento.evento.infEvento.detEvento.xCorrecao;
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe;

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, documento.TipoDocumento);

            return documento.Codigo;
        }

        #endregion

        #region Metodos Privados CTe

        private static bool SalvarProcCTeOsV3(MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc cteProc, MemoryStream resultStream, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, string caminhoDocumentosFiscais, long numeroNSU = 0)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);

            //Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unidadeTrabalho);
            //Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeOSDestinadoTomador;
            string cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTeOS.infCte.toma.Item);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(empresa.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, cteProc.protCTe.infProt.chCTe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.DataIntegracao = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(cnpjTomador))
                    documento.Empresa = repEmpresa.BuscarPorCNPJ(cnpjTomador);
                if (documento.Empresa == null)
                    documento.Empresa = empresa;
            }

            documento.Chave = cteProc.protCTe.infProt.chCTe;
            documento.CPFCNPJEmitente = cteProc.CTeOS.infCte.emit.CNPJ;
            documento.DataAutorizacao = DateTime.ParseExact(cteProc.protCTe.infProt.dhRecbto, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            try
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTeOS.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            }
            catch
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTeOS.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);
            }

            documento.IEEmitente = cteProc.CTeOS.infCte.emit.IE;
            documento.NomeEmitente = cteProc.CTeOS.infCte.emit.xNome;
            documento.Numero = int.Parse(cteProc.CTeOS.infCte.ide.nCT);
            documento.Serie = int.Parse(cteProc.CTeOS.infCte.ide.serie);
            if (documento.NumeroSequencialUnico == 0)
                documento.NumeroSequencialUnico = dfe != null ? long.Parse(dfe.NSU) : numeroNSU;
            documento.Protocolo = cteProc.protCTe.infProt.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
            documento.Valor = decimal.Parse(cteProc.CTeOS.infCte.vPrest.vRec, cultura);
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
            if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                documento.Cancelado = true;

            documento.CPFCNPJTomador = cteProc.CTeOS.infCte.toma.Item;
            documento.NomeTomador = cteProc.CTeOS.infCte.toma.xNome;

            if (configuracaoTMS?.GrupoPessoasDocumentosDestinados != null)
            {
                string raizTomador = Utilidades.String.OnlyNumbers(documento.CPFCNPJTomador).Remove(8, 6);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(raizTomador);

                if (grupoPessoas == null || grupoPessoas.Codigo != configuracaoTMS.GrupoPessoasDocumentosDestinados.Codigo)
                    return false;
            }

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            SalvarXmlCTe(resultStream, empresa, cteProc.protCTe.infProt.chCTe, caminhoDocumentosFiscais, unidadeTrabalho);

            new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
            if (new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho).ExistePorTipo(TipoIntegracao.Unilever))
            {
                Servicos.Log.TratarErro("1 - Criando CT-e OS");
                cte = servicoGestaoDocumento.CriarCTe(cteProc, resultStream);
                Servicos.Log.TratarErro($"1 - CT-e OS Finalizado = {cte.Codigo}");
            }


            return true;
        }



        private static bool SalvarProcCTeOsV4(MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc cteProc, MemoryStream resultStream, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, string caminhoDocumentosFiscais, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, long numeroNSU = 0)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);
            Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeOSDestinadoTomador;
            string cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTeOS.infCte.toma.Item);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(empresa.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, cteProc.protCTe.infProt.chCTe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.DataIntegracao = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(cnpjTomador))
                    documento.Empresa = repEmpresa.BuscarPorCNPJ(cnpjTomador);
                if (documento.Empresa == null)
                    documento.Empresa = empresa;
            }

            documento.Chave = cteProc.protCTe.infProt.chCTe;
            documento.CPFCNPJEmitente = cteProc.CTeOS.infCte.emit.CNPJ;
            documento.DataAutorizacao = DateTime.ParseExact(cteProc.protCTe.infProt.dhRecbto, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            try
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTeOS.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            }
            catch
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTeOS.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);
            }

            documento.IEEmitente = cteProc.CTeOS.infCte.emit.IE;
            documento.NomeEmitente = cteProc.CTeOS.infCte.emit.xNome;
            documento.Numero = int.Parse(cteProc.CTeOS.infCte.ide.nCT);
            documento.Serie = int.Parse(cteProc.CTeOS.infCte.ide.serie);
            if (documento.NumeroSequencialUnico == 0)
                documento.NumeroSequencialUnico = dfe != null ? long.Parse(dfe.NSU) : numeroNSU;
            documento.Protocolo = cteProc.protCTe.infProt.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
            documento.Valor = decimal.Parse(cteProc.CTeOS.infCte.vPrest.vRec, cultura);
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
            if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                documento.Cancelado = true;

            documento.CPFCNPJTomador = cteProc.CTeOS.infCte.toma.Item;
            documento.NomeTomador = cteProc.CTeOS.infCte.toma.xNome;

            if (configuracaoTMS?.GrupoPessoasDocumentosDestinados != null)
            {
                string raizTomador = Utilidades.String.OnlyNumbers(documento.CPFCNPJTomador).Remove(8, 6);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(raizTomador);

                if (grupoPessoas == null || grupoPessoas.Codigo != configuracaoTMS.GrupoPessoasDocumentosDestinados.Codigo)
                    return false;
            }

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            SalvarXmlCTe(resultStream, empresa, cteProc.protCTe.infProt.chCTe, caminhoDocumentosFiscais, unidadeTrabalho);

            new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                Servicos.Log.TratarErro("2 - Criando CT-e OS");
                cte = servicoGestaoDocumento.CriarCTe(cteProc, resultStream);
                Servicos.Log.TratarErro($"2 - CT-e OS Finalizado = {cte.Codigo}");
            }

            return true;
        }

        private static bool SalvarProcCTe(MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc, MemoryStream resultStream, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool importandoPeloXML = false, long numeroNSU = 0, string raizCNPJ = "")
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);

            Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unidadeTrabalho);
            Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
            Dominio.Enumeradores.TipoTomador tipoTomador = ObterTipoTomador(cteProc.CTe.infCte.ide);

            string cnpjTomador = "";

            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && (empresa.CNPJ == cteProc.CTe.infCte.rem?.Item || cteProc.CTe.infCte.rem?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.rem?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && (empresa.CNPJ == cteProc.CTe.infCte.dest?.Item || cteProc.CTe.infCte.dest?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.dest?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && (empresa.CNPJ == cteProc.CTe.infCte.exped?.Item || cteProc.CTe.infCte.exped?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.exped?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && (empresa.CNPJ == cteProc.CTe.infCte.receb?.Item || cteProc.CTe.infCte.receb?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.receb?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.rem?.Item || cteProc.CTe.infCte.rem?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.rem?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.dest?.Item || cteProc.CTe.infCte.dest?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.dest?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.exped?.Item || cteProc.CTe.infCte.exped?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.exped?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.receb?.Item || cteProc.CTe.infCte.receb?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.receb?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.emit.CNPJ || cteProc.CTe.infCte.emit.CNPJ.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.emit?.CNPJ);
            }
            else if (cteProc.CTe.infCte.autXML != null && cteProc.CTe.infCte.autXML.Where(o => o.Item == empresa.CNPJ).Count() > 0)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
            }
            else
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(empresa.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, cteProc.protCTe.infProt.chCTe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.DataIntegracao = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(cnpjTomador))
                    documento.Empresa = repEmpresa.BuscarPorCNPJ(cnpjTomador);
                if (documento.Empresa == null)
                    documento.Empresa = empresa;
            }
            documento.Chave = cteProc.protCTe.infProt.chCTe;
            documento.CPFCNPJEmitente = cteProc.CTe.infCte.emit.CNPJ;
            documento.DataAutorizacao = cteProc.protCTe.infProt.dhRecbto; //DateTime.ParseExact(nfeProc.protNFe.infProt.dhRecbto, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            try
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            }
            catch
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);
            }
            documento.IEEmitente = cteProc.CTe.infCte.emit.IE;
            documento.NomeEmitente = cteProc.CTe.infCte.emit.xNome;
            documento.Numero = int.Parse(cteProc.CTe.infCte.ide.nCT);
            documento.Serie = int.Parse(cteProc.CTe.infCte.ide.serie);
            if (documento.NumeroSequencialUnico == 0)
                documento.NumeroSequencialUnico = dfe != null ? long.Parse(dfe.NSU) : numeroNSU;
            documento.Protocolo = cteProc.protCTe.infProt.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
            documento.Valor = decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultura);
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
            if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                documento.Cancelado = true;

            if (cteProc.CTe.infCte.rem != null)
            {
                documento.CPFCNPJRemetente = cteProc.CTe.infCte.rem.Item;
                documento.NomeRemetente = cteProc.CTe.infCte.rem.xNome;
            }

            if (cteProc.CTe.infCte.dest != null)
            {
                documento.CPFCNPJDestinatario = cteProc.CTe.infCte.dest.Item;
                documento.NomeDestinatario = cteProc.CTe.infCte.dest.xNome;
                documento.UFDestinatario = cteProc.CTe.infCte.dest.enderDest.UF.ToString("g");
            }

            if (cteProc.CTe.infCte.ide.Item == null)
            {
                if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.dest.xNome;
                }
                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.exped.xNome;
                }
                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.receb.xNome;
                }
                else
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.rem.xNome;
                }
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item;

                documento.CPFCNPJTomador = tomador.Item;
                documento.NomeTomador = tomador.xNome;
            }
            else
            {
                MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)cteProc.CTe.infCte.ide.Item;

                if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.rem.xNome;
                }
                else if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.exped.xNome;
                }
                else if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.receb.xNome;
                }
                else if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.dest.xNome;
                }
            }

            //Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ();

            if (configuracaoTMS?.GrupoPessoasDocumentosDestinados != null)
            {
                string raizTomador = Utilidades.String.OnlyNumbers(documento.CPFCNPJTomador).Remove(8, 6);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(raizTomador);

                if (grupoPessoas == null || grupoPessoas.Codigo != configuracaoTMS.GrupoPessoasDocumentosDestinados.Codigo)
                    return false;
            }

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);

            if (cteProc.CTe.infCte.Item.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm))
                ObterInformacoesCTeNormal(documento, (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Item, unidadeTrabalho);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !importandoPeloXML)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.PreCTe preCTe = serCargaPreCTe.BuscarCargaPreCTe(cteProc, tipoServicoMultisoftware, configuracaoTMS, null, unidadeTrabalho);

                if (preCTe == null || (preCTe.CargaCTe == null && preCTe.CargaCTe?.CargaCTeComplementoInfo == null))
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoGestaoDocumento.CriarCTe(cteProc, resultStream);

                    if (cte != null)
                        servicoGestaoDocumento.CriarInconsitencia(cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.SemCarga, tipoServicoMultisoftware);
                }
                else
                {
                    if (preCTe != null && preCTe.CargaCTe != null)
                    {
                        if (preCTe.CargaCTe.CTe == null)
                        {
                            string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(resultStream, preCTe.CargaCTe.PreCTe, preCTe.CargaCTe, unidadeTrabalho, configuracaoTMS, tipoServicoMultisoftware);
                            if (retorno.Length == 0)
                            {
                                if (preCTe.CargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                                    serCargaPreCTe.SetarDocumentoOriginario(preCTe.CargaCTe.CTe, unidadeTrabalho);

                                serCargaPreCTe.VerificarEnviouTodosDocumentos(unidadeTrabalho, preCTe.CargaCTe.Carga, tipoServicoMultisoftware, configuracaoTMS);
                                servicoGestaoDocumento.RemoverInconsitencia(preCTe.CargaCTe.CTe);
                            }
                            else
                                Servicos.Log.TratarErro("Retorno CT-e " + documento?.Chave + ": " + retorno, "VinculoDestinadosPreCTeCarga");
                        }
                        else
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoGestaoDocumento.CriarCTe(cteProc, resultStream);

                            if (cte != null && cte.Chave != preCTe.CargaCTe.CTe.Chave)
                            {
                                string detalhes = "Carga " + preCTe.CargaCTe.Carga.Codigo + " já possui CTe.";
                                servicoGestaoDocumento.CriarInconsitencia(cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CargaJaPossuiCTe, tipoServicoMultisoftware, detalhes);
                            }
                        }
                    }
                    else
                    {
                        string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(resultStream, preCTe.CargaCTeComplementoInfo.PreCTe, preCTe.CargaCTeComplementoInfo, unidadeTrabalho, configuracaoTMS, tipoServicoMultisoftware);

                        if (retorno.Length == 0)
                            Servicos.Embarcador.Carga.PreCTe.VerificarEnviouTodosPreDocumentos(preCTe.CargaCTeComplementoInfo.CargaOcorrencia, unidadeTrabalho);
                        else
                            Servicos.Log.TratarErro("Retorno CT-e Complementar" + retorno, "XMLEmail");
                    }
                }
            }

            return true;
        }

        private static bool SalvarProcCTe(MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc, MemoryStream resultStream, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool importandoPeloXML = false, long numeroNSU = 0, string raizCNPJ = "")
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);

            Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unidadeTrabalho);
            Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
            Dominio.Enumeradores.TipoTomador tipoTomador = ObterTipoTomador(cteProc.CTe.infCte.ide);

            string cnpjTomador = "";

            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && (empresa.CNPJ == cteProc.CTe.infCte.rem?.Item || cteProc.CTe.infCte.rem?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.rem?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && (empresa.CNPJ == cteProc.CTe.infCte.dest?.Item || cteProc.CTe.infCte.dest?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.dest?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && (empresa.CNPJ == cteProc.CTe.infCte.exped?.Item || cteProc.CTe.infCte.exped?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.exped?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && (empresa.CNPJ == cteProc.CTe.infCte.receb?.Item || cteProc.CTe.infCte.receb?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.receb?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.rem?.Item || cteProc.CTe.infCte.rem?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.rem?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.dest?.Item || cteProc.CTe.infCte.dest?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.dest?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.exped?.Item || cteProc.CTe.infCte.exped?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.exped?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.receb?.Item || cteProc.CTe.infCte.receb?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.receb?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.emit.Item || cteProc.CTe.infCte.emit.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.emit?.Item);
            }
            else if (cteProc.CTe.infCte.autXML != null && cteProc.CTe.infCte.autXML.Where(o => o.Item == empresa.CNPJ).Count() > 0)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
            }
            else
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(empresa.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, cteProc.protCTe.infProt.chCTe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.DataIntegracao = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(cnpjTomador))
                    documento.Empresa = repEmpresa.BuscarPorCNPJ(cnpjTomador);
                if (documento.Empresa == null)
                    documento.Empresa = empresa;
            }
            documento.Chave = cteProc.protCTe.infProt.chCTe;
            documento.CPFCNPJEmitente = cteProc.CTe.infCte.emit.Item;
            documento.DataAutorizacao = DateTime.ParseExact(cteProc.protCTe.infProt.dhRecbto, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            try
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            }
            catch
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);
            }
            documento.IEEmitente = cteProc.CTe.infCte.emit.IE;
            documento.NomeEmitente = cteProc.CTe.infCte.emit.xNome;
            documento.Numero = int.Parse(cteProc.CTe.infCte.ide.nCT);
            documento.Serie = int.Parse(cteProc.CTe.infCte.ide.serie);
            if (documento.NumeroSequencialUnico == 0)
                documento.NumeroSequencialUnico = dfe != null ? long.Parse(dfe.NSU) : numeroNSU;
            documento.Protocolo = cteProc.protCTe.infProt.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
            documento.Valor = decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultura);
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
            if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                documento.Cancelado = true;

            if (cteProc.CTe.infCte.rem != null)
            {
                documento.CPFCNPJRemetente = cteProc.CTe.infCte.rem.Item;
                documento.NomeRemetente = cteProc.CTe.infCte.rem.xNome;
            }

            if (cteProc.CTe.infCte.dest != null)
            {
                documento.CPFCNPJDestinatario = cteProc.CTe.infCte.dest.Item;
                documento.NomeDestinatario = cteProc.CTe.infCte.dest.xNome;
                documento.UFDestinatario = cteProc.CTe.infCte.dest.enderDest.UF.ToString("g");
            }

            if (cteProc.CTe.infCte.ide.Item == null)
            {
                if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.dest.xNome;
                }
                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.exped.xNome;
                }
                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.receb.xNome;
                }
                else
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.rem.xNome;
                }
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item;

                documento.CPFCNPJTomador = tomador.Item;
                documento.NomeTomador = tomador.xNome;
            }
            else
            {
                MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)cteProc.CTe.infCte.ide.Item;

                if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.rem.xNome;
                }
                else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.exped.xNome;
                }
                else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.receb.xNome;
                }
                else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                {
                    documento.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                    documento.NomeTomador = cteProc.CTe.infCte.dest.xNome;
                }
            }

            //Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ();

            if (configuracaoTMS?.GrupoPessoasDocumentosDestinados != null)
            {
                string raizTomador = Utilidades.String.OnlyNumbers(documento.CPFCNPJTomador).Remove(8, 6);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(raizTomador);

                if (grupoPessoas == null || grupoPessoas.Codigo != configuracaoTMS.GrupoPessoasDocumentosDestinados.Codigo)
                    return false;
            }

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);

            if (cteProc.CTe.infCte.Items.FirstOrDefault()?.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm))
                ObterInformacoesCTeNormal(documento, (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)cteProc.CTe.infCte.Items.First(), unidadeTrabalho);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !importandoPeloXML)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.PreCTe preCTe = serCargaPreCTe.BuscarCargaPreCTe(cteProc, tipoServicoMultisoftware, configuracaoTMS, null, unidadeTrabalho);

                if (preCTe == null || (preCTe.CargaCTe == null && preCTe.CargaCTe?.CargaCTeComplementoInfo == null))
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoGestaoDocumento.CriarCTe(cteProc, resultStream);

                    if (cte != null)
                        servicoGestaoDocumento.CriarInconsitencia(cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.SemCarga, tipoServicoMultisoftware);
                }
                else
                {
                    if (preCTe != null && preCTe.CargaCTe != null)
                    {
                        if (preCTe.CargaCTe.CTe == null)
                        {
                            string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(resultStream, preCTe.CargaCTe.PreCTe, preCTe.CargaCTe, unidadeTrabalho, configuracaoTMS, tipoServicoMultisoftware);
                            if (retorno.Length == 0)
                            {
                                if (preCTe.CargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                                    serCargaPreCTe.SetarDocumentoOriginario(preCTe.CargaCTe.CTe, unidadeTrabalho);

                                serCargaPreCTe.VerificarEnviouTodosDocumentos(unidadeTrabalho, preCTe.CargaCTe.Carga, tipoServicoMultisoftware, configuracaoTMS);
                                servicoGestaoDocumento.RemoverInconsitencia(preCTe.CargaCTe.CTe);
                            }
                            else
                                Servicos.Log.TratarErro("Retorno CT-e " + documento?.Chave + ": " + retorno, "VinculoDestinadosPreCTeCarga");
                        }
                        else
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoGestaoDocumento.CriarCTe(cteProc, resultStream);

                            if (cte != null && cte.Chave != preCTe.CargaCTe.CTe.Chave)
                            {
                                string detalhes = "Carga " + preCTe.CargaCTe.Carga.Codigo + " já possui CTe.";
                                servicoGestaoDocumento.CriarInconsitencia(cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.CargaJaPossuiCTe, tipoServicoMultisoftware, detalhes);
                            }
                        }
                    }
                    else
                    {
                        string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(resultStream, preCTe.CargaCTeComplementoInfo.PreCTe, preCTe.CargaCTeComplementoInfo, unidadeTrabalho, configuracaoTMS, tipoServicoMultisoftware);

                        if (retorno.Length == 0)
                            Servicos.Embarcador.Carga.PreCTe.VerificarEnviouTodosPreDocumentos(preCTe.CargaCTeComplementoInfo.CargaOcorrencia, unidadeTrabalho);
                        else
                            Servicos.Log.TratarErro("Retorno CT-e Complementar" + retorno, "XMLEmail");
                    }
                }
            }

            return true;
        }

        private static void ObterInformacoesCTeNormal(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm infCTeNormal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (infCTeNormal != null)
                SalvarInformacoesDocumentos(documento, infCTeNormal.infDoc, unidadeDeTrabalho);
        }

        private static void ObterInformacoesCTeNormal(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm infCTeNormal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (infCTeNormal != null)
                SalvarInformacoesDocumentos(documento, infCTeNormal.infDoc, unidadeDeTrabalho);
        }

        private static void SalvarInformacoesDocumentos(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDoc infDoc, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (infDoc != null)
            {
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaNotasCTe repDocumentoDestinadoEmpresaNotasCTe = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaNotasCTe(unidadeDeTrabalho);
                Repositorio.ModeloDocumentoFiscal repModeloDocumento = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);

                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                foreach (var item in infDoc.Items)
                {
                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaNotasCTe documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaNotasCTe();
                    Type tipoItem = item.GetType();
                    if (tipoItem == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)item;

                        documento.BaseCalculoICMS = decimal.Parse(nf.vBC, cultura);
                        documento.BaseCalculoICMSST = decimal.Parse(nf.vBCST, cultura);
                        documento.CFOP = nf.nCFOP;
                        documento.DocumentoDestinadoEmpresa = documentoDestinado;
                        documento.DataEmissao = DateTime.ParseExact(nf.dEmi, "yyyy-MM-dd", null);

                        string modelo = nf.mod.ToString("D");
                        documento.ModeloDocumentoFiscal = repModeloDocumento.BuscarPorModelo(modelo.Length == 1 ? string.Format("{0:00}", int.Parse(modelo)) : modelo);

                        documento.Numero = nf.nDoc;
                        documento.Peso = nf.nPeso != null ? decimal.Parse(nf.nPeso, cultura) : 0m;
                        documento.Serie = nf.serie;
                        documento.Valor = decimal.Parse(nf.vNF, cultura);
                        documento.ValorICMS = decimal.Parse(nf.vICMS, cultura);
                        documento.ValorICMSST = decimal.Parse(nf.vST, cultura);
                        documento.ValorProdutos = decimal.Parse(nf.vProd, cultura);

                        repDocumentoDestinadoEmpresaNotasCTe.Inserir(documento);
                    }
                    else if (tipoItem == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)item;

                        documento.ChaveNFE = nfe.chave;

                        DateTime dataEmissao = new DateTime();

                        if (DateTime.TryParseExact(nfe.chave.Substring(2, 4), "yyMM", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                            documento.DataEmissao = dataEmissao;
                        else
                            documento.DataEmissao = DateTime.Now;

                        documento.Numero = int.Parse(nfe.chave.Substring(25, 9)).ToString();
                        documento.Serie = int.Parse(nfe.chave.Substring(22, 3)).ToString();
                        documento.DocumentoDestinadoEmpresa = documentoDestinado;
                        documento.ModeloDocumentoFiscal = repModeloDocumento.BuscarPorModelo("55");

                        repDocumentoDestinadoEmpresaNotasCTe.Inserir(documento);
                    }
                    else if (tipoItem == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outros = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)item;

                        documento.DocumentoDestinadoEmpresa = documentoDestinado;

                        if (outros.dEmi != null)
                            documento.DataEmissao = DateTime.ParseExact(outros.dEmi, "yyyy-MM-dd", null);
                        else
                            documento.DataEmissao = DateTime.Now;

                        documento.Numero = outros.nDoc != null ? outros.nDoc : string.Empty;
                        Dominio.Entidades.ModeloDocumentoFiscal modelo = repModeloDocumento.BuscarPorModelo(outros.tpDoc.ToString("D"));
                        if (modelo == null)
                            modelo = repModeloDocumento.BuscarPorModelo("99");
                        documento.ModeloDocumentoFiscal = modelo;
                        documento.Valor = outros.vDocFisc != null ? decimal.Parse(outros.vDocFisc, cultura) : 0;

                        repDocumentoDestinadoEmpresaNotasCTe.Inserir(documento);
                    }
                }
            }
        }

        private static void SalvarInformacoesDocumentos(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDoc infDoc, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (infDoc != null)
            {
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaNotasCTe repDocumentoDestinadoEmpresaNotasCTe = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaNotasCTe(unidadeDeTrabalho);
                Repositorio.ModeloDocumentoFiscal repModeloDocumento = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);

                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                foreach (var item in infDoc.Items)
                {
                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaNotasCTe documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaNotasCTe();
                    Type tipoItem = item.GetType();
                    if (tipoItem == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)item;

                        documento.BaseCalculoICMS = decimal.Parse(nf.vBC, cultura);
                        documento.BaseCalculoICMSST = decimal.Parse(nf.vBCST, cultura);
                        documento.CFOP = nf.nCFOP;
                        documento.DocumentoDestinadoEmpresa = documentoDestinado;
                        documento.DataEmissao = DateTime.ParseExact(nf.dEmi, "yyyy-MM-dd", null);

                        string modelo = nf.mod.ToString("D");
                        documento.ModeloDocumentoFiscal = repModeloDocumento.BuscarPorModelo(modelo.Length == 1 ? string.Format("{0:00}", int.Parse(modelo)) : modelo);

                        documento.Numero = nf.nDoc;
                        documento.Peso = nf.nPeso != null ? decimal.Parse(nf.nPeso, cultura) : 0m;
                        documento.Serie = nf.serie;
                        documento.Valor = decimal.Parse(nf.vNF, cultura);
                        documento.ValorICMS = decimal.Parse(nf.vICMS, cultura);
                        documento.ValorICMSST = decimal.Parse(nf.vST, cultura);
                        documento.ValorProdutos = decimal.Parse(nf.vProd, cultura);

                        repDocumentoDestinadoEmpresaNotasCTe.Inserir(documento);
                    }
                    else if (tipoItem == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)item;

                        documento.ChaveNFE = nfe.chave;

                        DateTime dataEmissao = new DateTime();

                        if (DateTime.TryParseExact(nfe.chave.Substring(2, 4), "yyMM", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                            documento.DataEmissao = dataEmissao;
                        else
                            documento.DataEmissao = DateTime.Now;

                        documento.Numero = int.Parse(nfe.chave.Substring(25, 9)).ToString();
                        documento.Serie = int.Parse(nfe.chave.Substring(22, 3)).ToString();
                        documento.DocumentoDestinadoEmpresa = documentoDestinado;
                        documento.ModeloDocumentoFiscal = repModeloDocumento.BuscarPorModelo("55");

                        repDocumentoDestinadoEmpresaNotasCTe.Inserir(documento);
                    }
                    else if (tipoItem == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outros = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)item;

                        documento.DocumentoDestinadoEmpresa = documentoDestinado;

                        if (outros.dEmi != null)
                            documento.DataEmissao = DateTime.ParseExact(outros.dEmi, "yyyy-MM-dd", null);
                        else
                            documento.DataEmissao = DateTime.Now;

                        documento.Numero = outros.nDoc != null ? outros.nDoc : string.Empty;
                        Dominio.Entidades.ModeloDocumentoFiscal modelo = repModeloDocumento.BuscarPorModelo(outros.tpDoc.ToString("D"));
                        if (modelo == null)
                            modelo = repModeloDocumento.BuscarPorModelo("99");
                        documento.ModeloDocumentoFiscal = modelo;
                        documento.Valor = outros.vDocFisc != null ? decimal.Parse(outros.vDocFisc, cultura) : 0;

                        repDocumentoDestinadoEmpresaNotasCTe.Inserir(documento);
                    }
                }
            }
        }

        private static bool SalvarProcCTe(MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.CTe.v104.ConhecimentoDeTransporteProcessado.cteProc cteProc, MemoryStream resultStream, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool importandoPeloXML = false, long numeroNSU = 0, string raizCNPJ = "")
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);

            Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
            Dominio.Enumeradores.TipoTomador tipoTomador = ObterTipoTomador(cteProc.CTe.infCte.ide);

            string cnpjTomador = "";

            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && (empresa.CNPJ == cteProc.CTe.infCte.rem?.Item || cteProc.CTe.infCte.rem?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.rem?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && (empresa.CNPJ == cteProc.CTe.infCte.dest?.Item || cteProc.CTe.infCte.dest?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.dest?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && (empresa.CNPJ == cteProc.CTe.infCte.exped?.Item || cteProc.CTe.infCte.exped?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.exped?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && (empresa.CNPJ == cteProc.CTe.infCte.receb?.Item || cteProc.CTe.infCte.receb?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.receb?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.rem?.Item || cteProc.CTe.infCte.rem?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.rem?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.dest?.Item || cteProc.CTe.infCte.dest?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.dest?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.exped?.Item || cteProc.CTe.infCte.exped?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.exped?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.receb?.Item || cteProc.CTe.infCte.receb?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.receb?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.emit.CNPJ || cteProc.CTe.infCte.emit.CNPJ.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.emit?.CNPJ);
            }
            else
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(empresa.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, cteProc.protCTe.infProt.chCTe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.DataIntegracao = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(cnpjTomador))
                    documento.Empresa = repEmpresa.BuscarPorCNPJ(cnpjTomador);
                if (documento.Empresa == null)
                    documento.Empresa = empresa;

            }
            documento.Chave = cteProc.protCTe.infProt.chCTe;
            documento.CPFCNPJEmitente = cteProc.CTe.infCte.emit.CNPJ;
            documento.DataAutorizacao = cteProc.protCTe.infProt.dhRecbto;
            try
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            }
            catch
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);
            }
            documento.IEEmitente = cteProc.CTe.infCte.emit.IE;
            documento.NomeEmitente = cteProc.CTe.infCte.emit.xNome;
            documento.Numero = int.Parse(cteProc.CTe.infCte.ide.nCT);
            documento.Serie = int.Parse(cteProc.CTe.infCte.ide.serie);
            if (documento.NumeroSequencialUnico == 0)
                documento.NumeroSequencialUnico = dfe != null ? long.Parse(dfe.NSU) : numeroNSU;
            documento.Protocolo = cteProc.protCTe.infProt.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
            documento.Valor = decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultura);
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
            if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                documento.Cancelado = true;

            if (cteProc.CTe.infCte.rem != null)
            {
                documento.CPFCNPJRemetente = cteProc.CTe.infCte.rem.Item;
                documento.NomeRemetente = cteProc.CTe.infCte.rem.xNome;
            }

            if (cteProc.CTe.infCte.dest != null)
            {
                documento.CPFCNPJDestinatario = cteProc.CTe.infCte.dest.Item;
                documento.NomeDestinatario = cteProc.CTe.infCte.dest.xNome;
                documento.UFDestinatario = cteProc.CTe.infCte.dest.enderDest.UF.ToString("g");
            }

            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
            {
                documento.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                documento.NomeTomador = cteProc.CTe.infCte.dest.xNome;
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
            {
                documento.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                documento.NomeTomador = cteProc.CTe.infCte.exped.xNome;
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
            {
                documento.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                documento.NomeTomador = cteProc.CTe.infCte.receb.xNome;
            }
            else
            {
                documento.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                documento.NomeTomador = cteProc.CTe.infCte.rem.xNome;
            }

            //Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ();

            if (configuracaoTMS?.GrupoPessoasDocumentosDestinados != null)
            {
                string raizTomador = Utilidades.String.OnlyNumbers(documento.CPFCNPJTomador).Remove(8, 6);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(raizTomador);

                if (grupoPessoas == null || grupoPessoas.Codigo != configuracaoTMS.GrupoPessoasDocumentosDestinados.Codigo)
                    return false;
            }

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            if (tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe)
                new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);

            return true;
        }

        private static bool SalvarProcCTe(MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc, MemoryStream resultStream, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool importandoPeloXML = false, long numeroNSU = 0, string raizCNPJ = "")
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);

            Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
            Dominio.Enumeradores.TipoTomador tipoTomador = ObterTipoTomador(cteProc.CTe.infCte.ide);
            string cnpjTomador = "";

            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && (empresa.CNPJ == cteProc.CTe.infCte.rem?.Item || cteProc.CTe.infCte.rem?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.rem?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && (empresa.CNPJ == cteProc.CTe.infCte.dest?.Item || cteProc.CTe.infCte.dest?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.dest?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && (empresa.CNPJ == cteProc.CTe.infCte.exped?.Item || cteProc.CTe.infCte.exped?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.exped?.Item);
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && (empresa.CNPJ == cteProc.CTe.infCte.receb?.Item || cteProc.CTe.infCte.receb?.Item.Substring(0, 8) == raizCNPJ))
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.receb?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.rem?.Item || cteProc.CTe.infCte.rem?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.rem?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.dest?.Item || cteProc.CTe.infCte.dest?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.dest?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.exped?.Item || cteProc.CTe.infCte.exped?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.exped?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.receb?.Item || cteProc.CTe.infCte.receb?.Item.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.receb?.Item);
            }
            else if (empresa.CNPJ == cteProc.CTe.infCte.emit.CNPJ || cteProc.CTe.infCte.emit.CNPJ.Substring(0, 8) == raizCNPJ)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente;
                cnpjTomador = Utilidades.String.OnlyNumbers(cteProc.CTe.infCte.emit?.CNPJ);
            }
            else if (cteProc.CTe.infCte.autXML != null && cteProc.CTe.infCte.autXML.Where(o => o.Item == empresa.CNPJ).Count() > 0)
            {
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
            }
            else
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(empresa.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, cteProc.protCTe.infProt.chCTe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.DataIntegracao = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(cnpjTomador))
                    documento.Empresa = repEmpresa.BuscarPorCNPJ(cnpjTomador);
                if (documento.Empresa == null)
                    documento.Empresa = empresa;
            }
            documento.Chave = cteProc.protCTe.infProt.chCTe;
            documento.CPFCNPJEmitente = cteProc.CTe.infCte.emit.CNPJ;
            documento.DataAutorizacao = cteProc.protCTe.infProt.dhRecbto;
            try
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            }
            catch
            {
                documento.DataEmissao = DateTime.ParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);
            }
            documento.IEEmitente = cteProc.CTe.infCte.emit.IE;
            documento.NomeEmitente = cteProc.CTe.infCte.emit.xNome;
            documento.Numero = int.Parse(cteProc.CTe.infCte.ide.nCT);
            documento.Serie = int.Parse(cteProc.CTe.infCte.ide.serie);
            if (documento.NumeroSequencialUnico == 0)
                documento.NumeroSequencialUnico = dfe != null ? long.Parse(dfe.NSU) : numeroNSU;
            documento.Protocolo = cteProc.protCTe.infProt.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
            documento.Valor = decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultura);
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
            if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                documento.Cancelado = true;

            if (cteProc.CTe.infCte.rem != null)
            {
                documento.CPFCNPJRemetente = cteProc.CTe.infCte.rem.Item;
                documento.NomeRemetente = cteProc.CTe.infCte.rem.xNome;
            }

            if (cteProc.CTe.infCte.dest != null)
            {
                documento.CPFCNPJDestinatario = cteProc.CTe.infCte.dest.Item;
                documento.NomeDestinatario = cteProc.CTe.infCte.dest.xNome;
                documento.UFDestinatario = cteProc.CTe.infCte.dest.enderDest.UF.ToString("g");
            }

            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
            {
                documento.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                documento.NomeTomador = cteProc.CTe.infCte.dest.xNome;
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
            {
                documento.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                documento.NomeTomador = cteProc.CTe.infCte.exped.xNome;
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
            {
                documento.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                documento.NomeTomador = cteProc.CTe.infCte.receb.xNome;
            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros && cteProc.CTe.infCte.ide.Item != null)
            {
                documento.CPFCNPJTomador = ((MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item).Item;
                documento.NomeTomador = ((MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item).xNome;
            }
            else
            {
                documento.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                documento.NomeTomador = cteProc.CTe.infCte.rem.xNome;
            }

            //Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ();

            if (configuracaoTMS?.GrupoPessoasDocumentosDestinados != null)
            {
                string raizTomador = Utilidades.String.OnlyNumbers(documento.CPFCNPJTomador).Remove(8, 6);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(raizTomador);

                if (grupoPessoas == null || grupoPessoas.Codigo != configuracaoTMS.GrupoPessoasDocumentosDestinados.Codigo)
                    return false;
            }

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            if (tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe)
                new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);

            return true;
        }

        private static bool SalvarProcEventoCTe(MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.CTe.v300.Eventos.procEventoCTe procEvento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);


            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(codigoEmpresa, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, procEvento.retEventoCTe.infEvento.chCTe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.Empresa = new Dominio.Entidades.Empresa() { Codigo = codigoEmpresa };
                documento.DataIntegracao = DateTime.Now;
            }

            documento.Chave = procEvento.retEventoCTe.infEvento.chCTe;
            documento.CPFCNPJEmitente = procEvento.retEventoCTe.infEvento.chCTe.Substring(6, 14);
            documento.DataAutorizacao = DateTime.ParseExact(procEvento.retEventoCTe.infEvento.dhRegEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.DataEmissao = DateTime.ParseExact(procEvento.eventoCTe.infEvento.dhEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.Numero = int.Parse(procEvento.retEventoCTe.infEvento.chCTe.Substring(25, 9));
            documento.Serie = int.Parse(procEvento.retEventoCTe.infEvento.chCTe.Substring(22, 3));
            documento.NumeroSequencialUnico = long.Parse(dfe.NSU);
            documento.NumeroSequencialEvento = int.Parse(procEvento.retEventoCTe.infEvento.nSeqEvento);
            documento.Protocolo = procEvento.retEventoCTe.infEvento.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
            documento.DescricaoEvento = procEvento.retEventoCTe.infEvento.xEvento;
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            if (tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe)
                new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoOriginario = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(documento.Empresa.Codigo,
                                                                                                                                                              new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] {
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTerceiro,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload
                                                                                                                                                              },
                                                                                                                                                              documento.Chave);

            if (documentoOriginario != null && tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe)
            {
                documentoOriginario.Cancelado = true;
                documentoOriginario.GerouArquivoIntegracao = false;

                repDocumentoDestinadoEmpresa.Atualizar(documentoOriginario);
            }

            CancelarCTesTerceiros(documento.Chave, unidadeTrabalho);

            return true;
        }

        private static bool SalvarProcEventoCTe(MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.CTe.v400.Eventos.procEventoCTe procEvento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(codigoEmpresa, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, procEvento.retEventoCTe.infEvento.chCTe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.Empresa = new Dominio.Entidades.Empresa() { Codigo = codigoEmpresa };
                documento.DataIntegracao = DateTime.Now;
            }

            documento.Chave = procEvento.retEventoCTe.infEvento.chCTe;
            documento.CPFCNPJEmitente = procEvento.retEventoCTe.infEvento.chCTe.Substring(6, 14);
            documento.DataAutorizacao = DateTime.ParseExact(procEvento.retEventoCTe.infEvento.dhRegEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.DataEmissao = DateTime.ParseExact(procEvento.eventoCTe.infEvento.dhEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.Numero = int.Parse(procEvento.retEventoCTe.infEvento.chCTe.Substring(25, 9));
            documento.Serie = int.Parse(procEvento.retEventoCTe.infEvento.chCTe.Substring(22, 3));
            documento.NumeroSequencialUnico = long.Parse(dfe.NSU);
            documento.NumeroSequencialEvento = int.Parse(procEvento.retEventoCTe.infEvento.nSeqEvento);
            documento.Protocolo = procEvento.retEventoCTe.infEvento.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
            documento.DescricaoEvento = procEvento.retEventoCTe.infEvento.xEvento;
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);

            if (tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe)
                new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unidadeTrabalho).GerarIntegracaoDocumentoDestinado(documento, tipoDocumento);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoOriginario = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(documento.Empresa.Codigo,
                                                                                                                                                              new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] {
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTerceiro,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload
                                                                                                                                                              },
                                                                                                                                                              documento.Chave);

            if (documentoOriginario != null && tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe)
            {
                documentoOriginario.Cancelado = true;
                documentoOriginario.GerouArquivoIntegracao = false;

                repDocumentoDestinadoEmpresa.Atualizar(documentoOriginario);
            }

            CancelarCTesTerceiros(documento.Chave, unidadeTrabalho);

            return true;
        }

        private static Dominio.Enumeradores.TipoTomador ObterTipoTomador(MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde)
        {
            if (infCTeIde.Item != null)
            {
                Type tipoTomador = infCTeIde.Item.GetType();
                if (tipoTomador == typeof(MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIdeToma03))
                {
                    MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIdeToma03 tomador = (MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIdeToma03)infCTeIde.Item;
                    if (tomador.toma == MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIdeToma03Toma.Item0)
                        return Dominio.Enumeradores.TipoTomador.Remetente;
                    else if (tomador.toma == MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIdeToma03Toma.Item1)
                        return Dominio.Enumeradores.TipoTomador.Expedidor;
                    else if (tomador.toma == MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIdeToma03Toma.Item2)
                        return Dominio.Enumeradores.TipoTomador.Recebedor;
                    else if (tomador.toma == MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIdeToma03Toma.Item3)
                        return Dominio.Enumeradores.TipoTomador.Destinatario;
                }
                else if (tipoTomador == typeof(MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                {
                    MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v104.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)infCTeIde.Item;
                    return Dominio.Enumeradores.TipoTomador.Outros;
                }
            }
            return Dominio.Enumeradores.TipoTomador.Remetente;
        }

        private static Dominio.Enumeradores.TipoTomador ObterTipoTomador(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde)
        {
            if (infCTeIde.Item != null)
            {
                Type tipoTomador = infCTeIde.Item.GetType();
                if (tipoTomador == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)infCTeIde.Item;
                    if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                        return Dominio.Enumeradores.TipoTomador.Remetente;
                    else if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                        return Dominio.Enumeradores.TipoTomador.Expedidor;
                    else if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                        return Dominio.Enumeradores.TipoTomador.Recebedor;
                    else if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                        return Dominio.Enumeradores.TipoTomador.Destinatario;
                }
                else if (tipoTomador == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)infCTeIde.Item;
                    return Dominio.Enumeradores.TipoTomador.Outros;
                }
            }
            return Dominio.Enumeradores.TipoTomador.Remetente;
        }

        private static Dominio.Enumeradores.TipoTomador ObterTipoTomador(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde)
        {
            if (infCTeIde.Item != null)
            {
                Type tipoTomador = infCTeIde.Item.GetType();
                if (tipoTomador == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)infCTeIde.Item;
                    if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                        return Dominio.Enumeradores.TipoTomador.Remetente;
                    else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                        return Dominio.Enumeradores.TipoTomador.Expedidor;
                    else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                        return Dominio.Enumeradores.TipoTomador.Recebedor;
                    else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                        return Dominio.Enumeradores.TipoTomador.Destinatario;
                }
                else if (tipoTomador == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)infCTeIde.Item;
                    return Dominio.Enumeradores.TipoTomador.Outros;
                }
            }
            return Dominio.Enumeradores.TipoTomador.Remetente;
        }

        private static Dominio.Enumeradores.TipoTomador ObterTipoTomador(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde)
        {
            if (infCTeIde.Item != null)
            {
                Type tipoTomador = infCTeIde.Item.GetType();
                if (tipoTomador == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma03))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma03 tomador = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma03)infCTeIde.Item;
                    if (tomador.toma == MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma03Toma.Item0)
                        return Dominio.Enumeradores.TipoTomador.Remetente;
                    else if (tomador.toma == MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma03Toma.Item1)
                        return Dominio.Enumeradores.TipoTomador.Expedidor;
                    else if (tomador.toma == MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma03Toma.Item2)
                        return Dominio.Enumeradores.TipoTomador.Recebedor;
                    else if (tomador.toma == MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma03Toma.Item3)
                        return Dominio.Enumeradores.TipoTomador.Destinatario;
                }
                else if (tipoTomador == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)infCTeIde.Item;
                    return Dominio.Enumeradores.TipoTomador.Outros;
                }
            }
            return Dominio.Enumeradores.TipoTomador.Remetente;
        }

        private static void CancelarCTesTerceiros(string chaveCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiros = repCTeTerceiro.BuscarTodosPorChave(chaveCTe);

            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesTerceiros)
            {
                cteTerceiro.SituacaoSEFAZ = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada;

                repCTeTerceiro.Atualizar(cteTerceiro);
            }
        }

        private static void RemoverDocumentoDaCarga(string chaveCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorChaveCTe(chaveCTe);

            if (cargaCTe == null)
                return;

            cargaCTe.CTe = null;
            cargaCTe.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos;

            repCarga.Atualizar(cargaCTe.Carga);
            repCargaCTe.Atualizar(cargaCTe);
        }


        private static void SalvarXmlCTe(MemoryStream resultStream, Dominio.Entidades.Empresa empresa, string chaveCte, string caminhoDocumentosFiscais, Repositorio.UnitOfWork unitOfWork)
        {
            resultStream.Position = 0;

            string caminho = string.Empty;

            if (!string.IsNullOrWhiteSpace(caminhoDocumentosFiscais))
                caminho = caminhoDocumentosFiscais;
            else
                caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", empresa.CNPJ, chaveCte + ".xml");

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());

        }
        #endregion

        #region Metodos Privados MDFe
        private static bool SalvarProcMDFe(MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.MDFe.v300.mdfeProc mdfeProc, MemoryStream resultStream, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool gerarCargaMDFeDestinado, bool importandoPeloXML = false, long numeroNSU = 0, string raizCNPJ = "")
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            Servicos.Embarcador.CTe.CTEsImportados servicoCTeImportado = new Servicos.Embarcador.CTe.CTEsImportados(unidadeTrabalho, tipoServicoMultisoftware, default);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDFeDestinado;

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(empresa.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, mdfeProc.protMDFe.infProt.chMDFe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.DataIntegracao = DateTime.Now;
                if (documento.Empresa == null)
                    documento.Empresa = empresa;
            }
            documento.Chave = mdfeProc.protMDFe.infProt.chMDFe;
            documento.CPFCNPJEmitente = mdfeProc.MDFe.infMDFe.emit.Item;
            try
            {
                documento.DataAutorizacao = DateTime.ParseExact(mdfeProc.protMDFe.infProt.dhRecbto, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            }
            catch
            {
                documento.DataAutorizacao = DateTime.ParseExact(mdfeProc.protMDFe.infProt.dhRecbto, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);
            }
            try
            {
                documento.DataEmissao = DateTime.ParseExact(mdfeProc.MDFe.infMDFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            }
            catch
            {
                documento.DataEmissao = DateTime.ParseExact(mdfeProc.MDFe.infMDFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);
            }
            documento.IEEmitente = mdfeProc.MDFe.infMDFe.emit.IE;
            documento.NomeEmitente = mdfeProc.MDFe.infMDFe.emit.xNome;
            documento.Numero = int.Parse(mdfeProc.MDFe.infMDFe.ide.nMDF);
            documento.Serie = int.Parse(mdfeProc.MDFe.infMDFe.ide.serie);
            if (documento.NumeroSequencialUnico == 0)
                documento.NumeroSequencialUnico = dfe != null ? long.Parse(dfe.NSU) : numeroNSU;
            documento.Protocolo = mdfeProc.protMDFe.infProt.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
            documento.Valor = decimal.Parse(mdfeProc.MDFe.infMDFe.tot.vCarga, cultura);
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.MDFe;
            if (!string.IsNullOrWhiteSpace(documento.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documento.Chave))
                documento.Cancelado = true;

            documento.CPFCNPJRemetente = mdfeProc.MDFe.infMDFe.emit.Item;
            documento.NomeRemetente = mdfeProc.MDFe.infMDFe.emit.xNome;

            if (mdfeProc.MDFe.infMDFe.infModal != null)
            {
                MultiSoftware.MDFe.v300.ModalRodoviario.rodo rodo = null;
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.MDFe.v300.ModalRodoviario.rodo));

                try
                {
                    byte[] data = Encoding.Default.GetBytes(mdfeProc.MDFe.infMDFe.infModal.Any.OuterXml);
                    using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length))
                        rodo = (MultiSoftware.MDFe.v300.ModalRodoviario.rodo)serializer.Deserialize(memStream);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar modal rodoviário MDFe com encoding padrão: {ex.ToString()}", "CatchNoAction");
                }

                if (rodo == null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(mdfeProc.MDFe.infMDFe.infModal.Any.OuterXml);
                    using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length))
                        rodo = (MultiSoftware.MDFe.v300.ModalRodoviario.rodo)serializer.Deserialize(memStream);
                }

                if (rodo != null && rodo.infANTT != null && rodo.infANTT.infContratante != null && rodo.infANTT.infContratante.Length > 0)
                {
                    documento.CPFCNPJTomador = rodo.infANTT.infContratante.FirstOrDefault().Item;
                    documento.CPFCNPJDestinatario = rodo.infANTT.infContratante.FirstOrDefault().Item;
                    if (!string.IsNullOrWhiteSpace(documento.CPFCNPJTomador))
                    {
                        double.TryParse(documento.CPFCNPJTomador, out double cnpjCPF);
                        if (cnpjCPF > 0)
                        {
                            Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(cnpjCPF);
                            if (pessoa != null)
                            {
                                documento.NomeTomador = pessoa.Nome;
                                documento.NomeDestinatario = pessoa.Nome;
                                documento.UFDestinatario = pessoa.Localidade.Estado.Sigla;
                            }
                        }
                    }
                }
            }

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);


            if (gerarCargaMDFeDestinado)
            {
                try
                {
                    servicoCTeImportado.ProcessarXMLMDFeAsync(resultStream, null).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("erro: " + ex, "GerarCargaMDFe");
                }
            }

            return true;
        }

        private static bool SalvarProcEventoMDFe(MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe, MultiSoftware.MDFe.v300.TProcEvento procEvento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumento, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(codigoEmpresa, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { tipoDocumento }, procEvento.retEventoMDFe.infEvento.chMDFe);

            if (documento == null)
            {
                documento = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                documento.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documento.Empresa = new Dominio.Entidades.Empresa() { Codigo = codigoEmpresa };
                documento.DataIntegracao = DateTime.Now;
            }

            documento.Chave = procEvento.retEventoMDFe.infEvento.chMDFe;
            documento.CPFCNPJEmitente = procEvento.retEventoMDFe.infEvento.chMDFe.Substring(6, 14);
            documento.DataAutorizacao = DateTime.ParseExact(procEvento.retEventoMDFe.infEvento.dhRegEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.DataEmissao = DateTime.ParseExact(procEvento.eventoMDFe.infEvento.dhEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documento.Numero = int.Parse(procEvento.retEventoMDFe.infEvento.chMDFe.Substring(25, 9));
            documento.Serie = int.Parse(procEvento.retEventoMDFe.infEvento.chMDFe.Substring(22, 3));
            documento.NumeroSequencialUnico = long.Parse(dfe.NSU);
            documento.NumeroSequencialEvento = int.Parse(procEvento.retEventoMDFe.infEvento.nSeqEvento);
            documento.Protocolo = procEvento.retEventoMDFe.infEvento.nProt;
            documento.TipoDocumento = tipoDocumento;
            documento.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
            documento.DescricaoEvento = procEvento.retEventoMDFe.infEvento.xEvento;
            documento.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.MDFe;

            if (tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.EncerramentoMDFe)
            {
                documento.Encerrado = true;
                documento.GerouArquivoIntegracao = false;
            }

            if (documento.Codigo > 0)
                repDocumentoDestinadoEmpresa.Atualizar(documento);
            else
                repDocumentoDestinadoEmpresa.Inserir(documento);


            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoOriginario = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(documento.Empresa.Codigo,
                                                                                                                                                              new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] {
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDFeDestinado,
                                                                                                                                                                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload
                                                                                                                                                              },
                                                                                                                                                              documento.Chave);

            if (documentoOriginario != null && tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDFECancelado)
            {
                documentoOriginario.Cancelado = true;
                documentoOriginario.GerouArquivoIntegracao = false;
                repDocumentoDestinadoEmpresa.Atualizar(documentoOriginario);
            }
            else if (documentoOriginario != null && tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.EncerramentoMDFe)
            {
                documentoOriginario.Encerrado = true;
                documentoOriginario.GerouArquivoIntegracao = false;
                repDocumentoDestinadoEmpresa.Atualizar(documentoOriginario);
            }

            return true;
        }

        private static void ProcessarDocumentosDestinados(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip dfe,
            int codigoEmpresa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unidadeTrabalho,
            Dominio.Entidades.Empresa empresa, List<string> cnpjsNaoImportarTransporte, string caminhoDocumentosFiscais, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware,
            Repositorio.Veiculo repVeiculo, Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto repDocumentoDestinadoEmpresaProduto, Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repTabelaValores,
            Repositorio.Abastecimento repAbastecimento, Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento,
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {

            using (MemoryStream compressedStream = new MemoryStream(dfe.Value))
            {
                using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    using (MemoryStream resultStream = new MemoryStream())
                    {
                        zipStream.CopyTo(resultStream);

                        resultStream.Position = 0;

                        if (dfe.schema == "resNFe_v1.00.xsd")
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe));

                            MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe resNFe = (ser.Deserialize(resultStream) as MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe);

                            SalvarResNFe(dfe, resNFe, codigoEmpresa, unidadeTrabalho, configuracaoTMS);
                        }
                        else if (dfe.schema == "procNFe_v3.10.xsd")
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc));

                            string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());

                            MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfeProc = (ser.Deserialize(resultStream) as MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc);
                            string placaObservacao = "";
                            string kmObservacao = "";
                            string horimetroObservacao = "";
                            string chassiObservacao = "";

                            if (SalvarProcNFe(dfe, nfeProc, empresa, unidadeTrabalho, cnpjsNaoImportarTransporte, out Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado, out kmObservacao, out placaObservacao, out horimetroObservacao, out chassiObservacao))
                            {
                                try
                                {
                                    resultStream.Position = 0;

                                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "NFe", "nfeProc", nfeProc.protNFe.infProt.chNFe + ".xml");

                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());

                                    if (configuracaoTMS.CriarNotaFiscalTransportePorDocumentoDestinado)
                                        CriarXMLNotaFiscal(documentoDestinado, unidadeTrabalho, tipoServicoMultisoftware);

                                    Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica servicoNotaFiscal = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica();

                                    servicoNotaFiscal.GerarRegistroNotaFiscal(nfeProc, unidadeTrabalho, empresa);
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("Não foi possível salvar a NF-e v3.00 da consulta de documentos destinados: " + ex.ToString());
                                }
                            }
                        }
                        else if (dfe.schema == "procNFe_v4.00.xsd")
                        {
                            try
                            {
                                XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc));

                                string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());
                                string placaObservacao = "";
                                string kmObservacao = "";
                                string horimetroObservacao = "";
                                string chassiObservacao = "";

                                MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfeProc = (ser.Deserialize(resultStream) as MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc);

                                if (SalvarProcNFe(dfe, nfeProc, empresa, unidadeTrabalho, cnpjsNaoImportarTransporte, tipoServicoMultisoftware, out Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado, out kmObservacao, out placaObservacao, out horimetroObservacao, out chassiObservacao))
                                {
                                    try
                                    {
                                        resultStream.Position = 0;

                                        string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "NFe", "nfeProc", nfeProc.protNFe.infProt.chNFe + ".xml");

                                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, resultStream.ToArray());

                                        Dominio.Entidades.Veiculo veiculoObs = null;

                                        if (!string.IsNullOrWhiteSpace(placaObservacao))
                                            veiculoObs = repVeiculo.BuscarPlaca(placaObservacao);
                                        else if (!string.IsNullOrWhiteSpace(chassiObservacao))
                                            veiculoObs = repVeiculo.BuscarPorChassi(chassiObservacao);

                                        Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica servicoNotaFiscal = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica();


                                        if (veiculoObs != null && documentoDestinado.Emitente != null && documentoDestinado.Emitente.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe.HasValue && documentoDestinado.Emitente.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe.Value && !repAbastecimento.ContemAbastecimentoPorChave(documentoDestinado.Chave))
                                        {

                                            List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto> produtos = repDocumentoDestinadoEmpresaProduto.BuscarPorDocumento(documentoDestinado.Codigo);
                                            if (produtos != null && produtos.Count > 0)
                                            {
                                                for (int i = 0; i < produtos.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(produtos[i].cProd))
                                                    {
                                                        Dominio.Entidades.Produto produto = repTabelaValores.BuscarProdutoPorPessoa(produtos[i].cProd, documentoDestinado.Emitente.CPF_CNPJ);
                                                        if (produto != null && !repAbastecimento.AbastecimentoDuplicado(documentoDestinado.DataEmissao.Value, documentoDestinado.Numero.ToString(), documentoDestinado.Emitente?.CPF_CNPJ ?? 0d, produto.Codigo, produtos[i].qCom, produtos[i].vUnCom))
                                                        {
                                                            Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();
                                                            abastecimento.DocumentoDestinadoEmpresa = documentoDestinado;
                                                            abastecimento.Veiculo = veiculoObs;
                                                            abastecimento.Kilometragem = kmObservacao.ToInt();
                                                            abastecimento.Equipamento = null;
                                                            abastecimento.Horimetro = horimetroObservacao.ToInt();
                                                            Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

                                                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
                                                            if (produto.CodigoNCM.StartsWith("271121") || produto.CodigoNCM.StartsWith("271019") || produto.CodigoNCM.StartsWith("271012"))
                                                                tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                                                            else
                                                                tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;

                                                            abastecimento.Motorista = null;
                                                            abastecimento.Posto = documentoDestinado.Emitente;
                                                            abastecimento.Produto = produto;
                                                            abastecimento.TipoAbastecimento = tipoAbastecimento;
                                                            abastecimento.Litros = produtos[i].qCom;
                                                            abastecimento.ValorUnitario = produtos[i].vUnCom;
                                                            abastecimento.Status = "A";
                                                            abastecimento.Situacao = "A";
                                                            abastecimento.DataAlteracao = DateTime.Now;
                                                            abastecimento.Data = documentoDestinado.DataEmissao.Value;
                                                            abastecimento.Documento = documentoDestinado.Numero.ToString();
                                                            abastecimento.ChaveNotaFiscal = documentoDestinado.Chave;
                                                            abastecimento.TipoRecebimentoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoAbastecimento.ImportacaoXML;
                                                            if (abastecimento.Veiculo != null)
                                                                abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(abastecimento.Veiculo.Codigo);

                                                            if (abastecimento.Motorista == null && abastecimento.Equipamento != null)
                                                            {
                                                                Dominio.Entidades.Veiculo veiculoEquipamento = repVeiculo.BuscarPorEquipamento(abastecimento.Equipamento.Codigo);
                                                                Dominio.Entidades.Usuario MotoristaEquipamento = veiculoEquipamento != null ? repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoEquipamento.Codigo) : null;

                                                                if (veiculoEquipamento != null && MotoristaEquipamento != null)
                                                                    abastecimento.Motorista = MotoristaEquipamento;
                                                                else if (veiculoEquipamento != null)
                                                                {
                                                                    Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(veiculoEquipamento.Codigo);
                                                                    if (veiculoTracao != null)
                                                                        abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                                                                }
                                                            }
                                                            else if (abastecimento.Motorista == null && abastecimento.Veiculo != null)
                                                            {
                                                                Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(abastecimento.Veiculo.Codigo);
                                                                if (veiculoTracao != null)
                                                                    abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                                                            }
                                                            abastecimento.TipoMovimento = configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto;

                                                            Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unidadeTrabalho, abastecimento.Veiculo, null, configuracaoTMS);
                                                            repAbastecimento.Inserir(abastecimento);

                                                            Servicos.Auditoria.Auditoria.Auditar(auditado, abastecimento, null, "Abastecimento inserido por uma nota fiscal recebida dos destinados", unidadeTrabalho);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (configuracaoTMS.CriarNotaFiscalTransportePorDocumentoDestinado)
                                            CriarXMLNotaFiscal(documentoDestinado, unidadeTrabalho, tipoServicoMultisoftware);
                                        else if (tipoServicoMultisoftware.HasValue && documentoDestinado.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeTransporte)
                                            VincularDocumentoACarga(documentoDestinado, unidadeTrabalho, tipoServicoMultisoftware.Value, configuracaoTMS);

                                        servicoNotaFiscal.GerarRegistroNotaFiscal(nfeProc, unidadeTrabalho, empresa);
                                    }
                                    catch (Exception ex)
                                    {
                                        Servicos.Log.TratarErro("Não foi possível salvar a NF-e v4.00 da consulta de documentos destinados: " + ex.ToString());
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro("Não foi possível processar o XML da NF-e v4.00 da consulta de documentos destinados: " + ex.ToString());
                            }
                        }
                        else if (dfe.schema == "resEvento_v1.00.xsd")
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento));

                            MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento resEvento = (ser.Deserialize(resultStream) as MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento);

                            if (cnpjsNaoImportarTransporte == null || !cnpjsNaoImportarTransporte.Contains(resEvento.Item))
                                SalvarResEvento(dfe, resEvento, codigoEmpresa, unidadeTrabalho);
                        }
                        else if (dfe.schema == "procEventoNFe_v1.00.xsd")
                        {
                            resultStream.Position = 0;

                            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(resultStream);
                            System.Xml.Linq.XNamespace ns = doc.Root.Name.Namespace;

                            string tpEvento = doc.Descendants(ns + "infEvento").FirstOrDefault()?.Element(ns + "tpEvento")?.Value ?? string.Empty;

                            resultStream.Position = 0;

                            if (tpEvento == "210200" || tpEvento == "210210" || tpEvento == "210220" || tpEvento == "210240") //Manifestação do Destinatário
                            {
                                XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.Evento.ManifestacaoDestinatario.EventoProcessado.TProcEvento));

                                MultiSoftware.NFe.Evento.ManifestacaoDestinatario.EventoProcessado.TProcEvento procEvento = (ser.Deserialize(resultStream) as MultiSoftware.NFe.Evento.ManifestacaoDestinatario.EventoProcessado.TProcEvento);

                                if (cnpjsNaoImportarTransporte == null || !cnpjsNaoImportarTransporte.Contains(procEvento.evento.infEvento.Item))
                                    SalvarProcEvento(dfe, procEvento, codigoEmpresa, unidadeTrabalho);
                            }
                            else if (tpEvento == "110111") //Cancelamento
                            {
                                XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.Evento.Cancelamento.Retorno.TProcEvento));

                                MultiSoftware.NFe.Evento.Cancelamento.Retorno.TProcEvento procEvento = (ser.Deserialize(resultStream) as MultiSoftware.NFe.Evento.Cancelamento.Retorno.TProcEvento);

                                // if (!cnpjsNaoImportarTransporte.Contains(procEvento.evento.infEvento.Item))
                                SalvarProcEvento(dfe, procEvento, codigoEmpresa, unidadeTrabalho);
                            }
                            else if (tpEvento == "110110") //CC-e
                            {
                                XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.Evento.CartaCorrecaoEletronica.EventoProcessado.TProcEvento));

                                MultiSoftware.NFe.Evento.CartaCorrecaoEletronica.EventoProcessado.TProcEvento procEvento = (ser.Deserialize(resultStream) as MultiSoftware.NFe.Evento.CartaCorrecaoEletronica.EventoProcessado.TProcEvento);

                                if (cnpjsNaoImportarTransporte == null || !cnpjsNaoImportarTransporte.Contains(procEvento.evento.infEvento.Item))
                                    SalvarProcEvento(dfe, procEvento, codigoEmpresa, unidadeTrabalho);
                            }
                            else
                            {
                                Servicos.Log.TratarErro("Tipo de evento não implementado. " + tpEvento);
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Schema não implementado. " + dfe.schema);
                        }
                    }
                }
            }


        }

        #endregion

        public static MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeInt ConsultarDocumentosFiscaisNFe(Repositorio.UnitOfWork unidadeTrabalho, string cnpj, long ultNSU, MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TCodUfIBGE cUFAutor, string caminhoCertificado, string senhaCertificado, string nomeCertificadoKeyVault, bool consultaUnicoNSU = false, int tipoAmbiente = 1, string chaveAcesso = null)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = GetCertificado(caminhoCertificado, senhaCertificado, nomeCertificadoKeyVault);

                dynamic item1 = null;

                if (!string.IsNullOrEmpty(chaveAcesso))
                    item1 = new MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.distDFeIntConsChNFe() { chNFe = chaveAcesso };
                else if (consultaUnicoNSU)
                    item1 = new MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.distDFeIntConsNSU() { NSU = string.Format("{0:000000000000000}", ultNSU) };
                else
                    item1 = new MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.distDFeIntDistNSU() { ultNSU = string.Format("{0:000000000000000}", ultNSU) };

                MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.distDFeInt infoDFe = new MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.distDFeInt()
                {
                    cUFAutor = cUFAutor,
                    Item = cnpj,
                    ItemElementName = cnpj.Length >= 14 ? MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.ItemChoiceType.CNPJ : MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.ItemChoiceType.CPF,
                    tpAmb = tipoAmbiente == 1 ? MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TAmb.Item1 : MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TAmb.Item2,
                    versao = !string.IsNullOrEmpty(chaveAcesso) ? MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TVerDistDFe.Item101 : MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TVerDistDFe.Item100,
                    Item1 = item1
                };

                using (MultiSoftware.NFe.ServicoNFeDistribuicaoDFe.NFeDistribuicaoDFeSoapClient svcDistribuicaoDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<MultiSoftware.NFe.ServicoNFeDistribuicaoDFe.NFeDistribuicaoDFeSoapClient, MultiSoftware.NFe.ServicoNFeDistribuicaoDFe.NFeDistribuicaoDFeSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.NFeDistribuicaoDFe))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (TextWriter streamWriter = new StreamWriter(memoryStream))
                        {
                            XmlSerializer xmlSerializerEnvio = new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.distDFeInt));
                            xmlSerializerEnvio.Serialize(streamWriter, infoDFe);

                            XElement dadosEnvio = XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));

                            svcDistribuicaoDFe.ClientCredentials.ClientCertificate.Certificate = certificado;

                            XElement dadosRetorno = svcDistribuicaoDFe.nfeDistDFeInteresse(dadosEnvio);

                            XmlSerializer xmlSerializerRetorno = new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeInt));
                            return (MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeInt)xmlSerializerRetorno.Deserialize(dadosRetorno.CreateReader());
                        }
                    }
                }
            }
            catch (ServicoException ex)
            {
                throw new ServicoException("Empresa " + cnpj + "," + ex.Message);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                throw new Exception("Falha ao consultar notas fiscais destinadas à empresa " + cnpj + ".", ex);
            }
        }

        public static MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeInt ConsultarDocumentosFiscaisCTe(Repositorio.UnitOfWork unidadeTrabalho, string cnpj, long ultNSU, MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.TAmb tipoAmbiente, MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.TCodUfIBGE cUFAutor, string caminhoCertificado, string senhaCertificado, string nomeCertificadoKeyVault, string urlSefaz, bool consultaUnicoNSU = false)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = GetCertificado(caminhoCertificado, senhaCertificado, nomeCertificadoKeyVault);

                dynamic item1 = null;

                if (consultaUnicoNSU)
                    item1 = new MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.distDFeIntConsNSU() { NSU = string.Format("{0:000000000000000}", ultNSU) };
                else
                    item1 = new MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.distDFeIntDistNSU() { ultNSU = string.Format("{0:000000000000000}", ultNSU) };

                MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.distDFeInt infoDFe = new MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.distDFeInt()
                {
                    cUFAutor = cUFAutor,
                    Item = cnpj,
                    ItemElementName = MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.ItemChoiceType.CNPJ,
                    tpAmb = tipoAmbiente,
                    versao = MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.TVerDistDFe.Item100,
                    Item1 = item1
                };

                using (MultiSoftware.CTe.ServicoCTeDistribuicaoDFe.CTeDistribuicaoDFeSoapClient svcDistribuicaoDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<MultiSoftware.CTe.ServicoCTeDistribuicaoDFe.CTeDistribuicaoDFeSoapClient, MultiSoftware.CTe.ServicoCTeDistribuicaoDFe.CTeDistribuicaoDFeSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.CTeDistribuicaoDFe))
                {
                    svcDistribuicaoDFe.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlSefaz);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (TextWriter streamWriter = new StreamWriter(memoryStream))
                        {
                            XmlSerializer xmlSerializerEnvio = new XmlSerializer(typeof(MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.distDFeInt));
                            xmlSerializerEnvio.Serialize(streamWriter, infoDFe);

                            XElement dadosEnvio = XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));

                            svcDistribuicaoDFe.ClientCredentials.ClientCertificate.Certificate = certificado;

                            XElement dadosRetorno = svcDistribuicaoDFe.cteDistDFeInteresse(dadosEnvio);

                            XmlSerializer xmlSerializerRetorno = new XmlSerializer(typeof(MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeInt));
                            return (MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeInt)xmlSerializerRetorno.Deserialize(dadosRetorno.CreateReader());
                        }
                    }
                }
            }
            catch (ServicoException ex)
            {
                throw new ServicoException("Empresa " + cnpj + "," + ex.Message);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                throw new Exception("Falha ao consultar na sefaz os Ctes destinadas à empresa " + cnpj + ". ", ex);
            }
        }

        public static MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.retDistDFeInt ConsultarDocumentosFiscaisMDFe(Repositorio.UnitOfWork unidadeTrabalho, string cnpj, long ultNSU, MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.TAmb tipoAmbiente, MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.TCodUfIBGE cUFAutor, string caminhoCertificado, string senhaCertificado, string nomeCertificadoKeyVault, string urlSefaz, bool consultaUnicoNSU = false)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = GetCertificado(caminhoCertificado, senhaCertificado, nomeCertificadoKeyVault);

                dynamic item1 = null;

                if (consultaUnicoNSU)
                    item1 = new MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.distDFeIntConsNSU() { NSU = string.Format("{0:000000000000000}", ultNSU) };
                else
                    item1 = new MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.distDFeIntDistNSU() { ultNSU = string.Format("{0:000000000000000}", ultNSU) };

                MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.distDFeInt infoDFe = new MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.distDFeInt()
                {
                    Item = cnpj,
                    ItemElementName = MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.ItemChoiceType.CNPJ,
                    tpAmb = tipoAmbiente,
                    versao = MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.TVerDistDFe.Item100,
                    Item1 = item1
                };

                MultiSoftware.MDFe.ServicoMDFeDistribuicaoDFe.mdfeCabecMsg mdfeCabecMsg = new MultiSoftware.MDFe.ServicoMDFeDistribuicaoDFe.mdfeCabecMsg()
                {
                    cUF = cUFAutor.ToString("D"),
                    versaoDados = "1.00"
                };

                using (MultiSoftware.MDFe.ServicoMDFeDistribuicaoDFe.MDFeDistribuicaoDFeSoap12Client svcDistribuicaoDFe = new MultiSoftware.MDFe.ServicoMDFeDistribuicaoDFe.MDFeDistribuicaoDFeSoap12Client())
                {
                    svcDistribuicaoDFe.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlSefaz);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        XmlSerializer xmlSerializerEnvio = new XmlSerializer(typeof(MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.distDFeInt));

                        xmlSerializerEnvio.Serialize(memoryStream, infoDFe);

                        memoryStream.Position = 0;

                        XmlDocument doc = new XmlDocument();
                        doc.Load(memoryStream);

                        svcDistribuicaoDFe.ClientCredentials.ClientCertificate.Certificate = certificado;

                        XmlNode dadosRetorno = svcDistribuicaoDFe.mdfeDistDFeInteresse(ref mdfeCabecMsg, doc.DocumentElement);

                        XmlSerializer xmlSerializerRetorno = new XmlSerializer(typeof(MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.retDistDFeInt));
                        using (TextReader reader = new StringReader(dadosRetorno.OuterXml))
                        {
                            MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.retDistDFeInt result = (MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.retDistDFeInt)xmlSerializerRetorno.Deserialize(reader);

                            return result;
                        }
                    }
                }
            }
            catch (ServicoException ex)
            {
                throw new ServicoException("Empresa " + cnpj + ", " + ex.Message);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                throw new Exception("Falha ao consultar MDFe destinadas à empresa " + cnpj + ".", ex);
            }
        }

        private static X509Certificate2 GetCertificado(string caminhoCertificado, string senhaCertificado, string nomeCertificadoKeyVault)
        {
            Servicos.SecretManagement.ISecretManager secretManager = new Servicos.SecretManagement.AzureKeyVaultSecretManager();
            X509Certificate2 certificado = null;
            if (!string.IsNullOrEmpty(nomeCertificadoKeyVault))
                certificado = secretManager.GetCertificate(nomeCertificadoKeyVault);

            if (certificado == null)
            {
                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCertificado))
                    throw new ServicoException($"certificado digital não configurado em {caminhoCertificado}.");

                certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);
            }

            if (certificado == null)
                throw new ServicoException($"certificado digital não configurado em {caminhoCertificado}");

            if (certificado.NotAfter < DateTime.Now || certificado.NotBefore > DateTime.Now)
                throw new ServicoException($"certificado expirado em {caminhoCertificado}.");

            return certificado;
        }

        private static void GravarXmlDistribucaoDFe(Dominio.Entidades.Empresa empresa,
                                                    MultiSoftware.CTe.CTeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip retdistdfeintlotedistdfeintdoczipCTe,
                                                    MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip retdistdfeintlotedistdfeintdoczipNFe,
                                                    MultiSoftware.MDFe.MDFeDistribuicaoDFe.DFe.retDistDFeIntLoteDistDFeIntDocZip retdistdfeintlotedistdfeintdoczipMDFe,
                                                    Stream xml,
                                                    SituacaoXml situacao,
                                                    Repositorio.UnitOfWork unidadeTrabalho,
                                                    string xmlString,
                                                    bool salvarXMl,
                                                    out Int64 idDocumentoXml)
        {
            idDocumentoXml = 0;
            try
            {
                List<string> schemas = new List<string> { "resNFe_v1.00.xsd",
                                                          "procNFe_v3.10.xsd",
                                                          "procNFe_v4.00.xsd",
                                                          "resEvento_v1.00.xsd",
                                                          "procEventoNFe_v1.00.xsd",
                                                          "procCTe_v4.00.xsd",
                                                          "procCTe_v3.00.xsd",
                                                          "procCTeOS_v3.00.xsd",
                                                          "procCTeOS_v4.00.xsd",
                                                          "procEventoCTe_v4.00.xsd",
                                                          "procEventoCTe_v3.00.xsd",
                                                          "procMDFe_v3.00.xsd",
                                                          "procEventoMDFe_v3.00.xsd"
                                                         };

                string chave = "";
                TipoDocumentoDestinadoEmpresa tipoDocumento = new TipoDocumentoDestinadoEmpresa();
                string tipoEvento = "";
                string versao = "";
                string mensagem = "";
                string schema = "";
                long nsu = 0;
                xml.Position = 0;

                if ((retdistdfeintlotedistdfeintdoczipCTe != null && schemas.Any(p => retdistdfeintlotedistdfeintdoczipCTe.schema.Contains(p))) ||
                    (retdistdfeintlotedistdfeintdoczipNFe != null && schemas.Any(p => retdistdfeintlotedistdfeintdoczipNFe.schema.Contains(p))) ||
                    (retdistdfeintlotedistdfeintdoczipMDFe != null && schemas.Any(p => retdistdfeintlotedistdfeintdoczipMDFe.schema.Contains(p)))
                    )
                {
                    if (retdistdfeintlotedistdfeintdoczipCTe != null)
                    {
                        nsu = long.Parse(retdistdfeintlotedistdfeintdoczipCTe.NSU);
                        schema = retdistdfeintlotedistdfeintdoczipCTe.schema;
                    }

                    if (retdistdfeintlotedistdfeintdoczipNFe != null)
                    {
                        nsu = long.Parse(retdistdfeintlotedistdfeintdoczipNFe.NSU);
                        schema = retdistdfeintlotedistdfeintdoczipNFe.schema;
                    }

                    if (retdistdfeintlotedistdfeintdoczipMDFe != null)
                    {
                        nsu = long.Parse(retdistdfeintlotedistdfeintdoczipMDFe.NSU);
                        schema = retdistdfeintlotedistdfeintdoczipMDFe.schema;
                    }

                    try
                    {
                        switch (schema)
                        {
                            case "resNFe_v1.00.xsd":
                                MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe resNFev1 = (new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe)).Deserialize(xml) as MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe);
                                chave = resNFev1.chNFe;
                                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada;
                                tipoEvento = "";
                                versao = resNFev1.versao == MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TVerResNFe.Item100 ? "1.00" : "não cadastrada";
                                break;
                            case "procNFe_v3.10.xsd":
                                MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc procNFev3 = (new XmlSerializer(typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)).Deserialize(xml) as MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc);
                                chave = procNFev3.protNFe.infProt.chNFe;

                                if (procNFev3.NFe?.infNFe?.dest?.Item == empresa.CNPJ)
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada;
                                else if (procNFev3.NFe?.infNFe?.transp?.transporta?.Item == empresa.CNPJ)
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeTransporte;
                                else
                                    mensagem = "Consulta de notas emitidas na SEFAZ: A empresa não é nem destinatário e nem transportador da NF-e processada. Chave: " + procNFev3.protNFe.infProt.chNFe;

                                tipoEvento = "";
                                versao = procNFev3.versao;
                                break;
                            case "procNFe_v4.00.xsd":
                                MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc procNFev4 = (new XmlSerializer(typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)).Deserialize(xml) as MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc);
                                chave = procNFev4.protNFe.infProt.chNFe;

                                if (procNFev4.NFe?.infNFe?.dest?.Item == empresa.CNPJ)
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada;
                                else if (procNFev4.NFe?.infNFe?.transp?.transporta?.Item == empresa.CNPJ)
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeTransporte;
                                else
                                    mensagem = "Consulta de notas emitidas na SEFAZ: A empresa não é nem destinatário e nem transportador da NF-e processada. Chave: " + procNFev4.protNFe.infProt.chNFe;

                                tipoEvento = "";
                                versao = procNFev4.versao;
                                break;
                            case "resEvento_v1.00.xsd":
                                MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento resEventov1 = (new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento)).Deserialize(xml) as MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resEvento);
                                chave = resEventov1.chNFe;

                                if (resEventov1.tpEvento == "110110")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CCe;
                                else if (resEventov1.tpEvento == "110111")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoNFe;
                                else if (resEventov1.tpEvento == "610600")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizacaoCTe;
                                else if (resEventov1.tpEvento == "610601")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe;
                                else if (resEventov1.tpEvento == "610610")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizacaoMDFe;
                                else if (resEventov1.tpEvento == "610611")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoMDFe;
                                else if (resEventov1.tpEvento == "610614")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizacaoMDFeComCTe;
                                else if (resEventov1.tpEvento == "610550")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFeRFID;
                                else if (resEventov1.tpEvento == "610500")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFe;
                                else if (resEventov1.tpEvento == "610501")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoPassagemNFe;
                                else if (resEventov1.tpEvento == "610514")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFePropagadoPeloMDFeOuCTe;
                                else if (resEventov1.tpEvento == "610554")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemNFeAutomaticoPeloMDFeOuCTe;
                                else if (resEventov1.tpEvento == "610615")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoMDFeAutorizadoComCTe;
                                else if (resEventov1.tpEvento == "610552")
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.PassagemAutomaticoMDFe;
                                else
                                    mensagem = "Evento não implementado. " + resEventov1;

                                tipoEvento = resEventov1.tpEvento;
                                versao = resEventov1.versao == MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TVerResEvento.Item100 ? "1.00" : "não cadastrada";
                                break;
                            case "procEventoNFe_v1.00.xsd":
                                System.Xml.Linq.XDocument docProcEventoNFev1 = System.Xml.Linq.XDocument.Load(xml);
                                string tpEventoProcEventoNFev1 = docProcEventoNFev1.Descendants(docProcEventoNFev1.Root.Name.Namespace + "infEvento").FirstOrDefault()?.Element(docProcEventoNFev1.Root.Name.Namespace + "tpEvento")?.Value ?? string.Empty;
                                xml.Position = 0;
                                tipoEvento = tpEventoProcEventoNFev1;
                                if (tpEventoProcEventoNFev1 == "210200" || tpEventoProcEventoNFev1 == "210210" || tpEventoProcEventoNFev1 == "210220" || tpEventoProcEventoNFev1 == "210240") //Manifestação do Destinatário
                                {
                                    MultiSoftware.NFe.Evento.ManifestacaoDestinatario.EventoProcessado.TProcEvento procEventoNFev1 = (new XmlSerializer(typeof(MultiSoftware.NFe.Evento.ManifestacaoDestinatario.EventoProcessado.TProcEvento)).Deserialize(xml) as MultiSoftware.NFe.Evento.ManifestacaoDestinatario.EventoProcessado.TProcEvento);
                                    chave = procEventoNFev1.retEvento.infEvento.chNFe;
                                    tipoDocumento = TipoDocumentoDestinadoEmpresa.MDe;
                                    versao = procEventoNFev1.versao;
                                }
                                else if (tpEventoProcEventoNFev1 == "110111") //Cancelamento
                                {
                                    MultiSoftware.NFe.Evento.Cancelamento.Retorno.TProcEvento procEventoNFev1 = (new XmlSerializer(typeof(MultiSoftware.NFe.Evento.Cancelamento.Retorno.TProcEvento)).Deserialize(xml) as MultiSoftware.NFe.Evento.Cancelamento.Retorno.TProcEvento);
                                    chave = procEventoNFev1.retEvento.infEvento.chNFe;
                                    tipoDocumento = TipoDocumentoDestinadoEmpresa.CancelamentoNFe;
                                    versao = procEventoNFev1.versao;
                                }
                                else if (tpEventoProcEventoNFev1 == "110110") //CC-e
                                {
                                    MultiSoftware.NFe.Evento.CartaCorrecaoEletronica.EventoProcessado.TProcEvento procEventoNFev1 = (new XmlSerializer(typeof(MultiSoftware.NFe.Evento.CartaCorrecaoEletronica.EventoProcessado.TProcEvento)).Deserialize(xml) as MultiSoftware.NFe.Evento.CartaCorrecaoEletronica.EventoProcessado.TProcEvento);
                                    chave = procEventoNFev1.retEvento.infEvento.chNFe;
                                    tipoDocumento = TipoDocumentoDestinadoEmpresa.CCe;
                                    versao = procEventoNFev1.versao;
                                }
                                else
                                    mensagem = "Evento não implementado. " + tpEventoProcEventoNFev1;

                                break;
                            case "procCTe_v4.00.xsd":
                                MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc procCTev4 = (new XmlSerializer(typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)).Deserialize(xml) as MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc);
                                chave = procCTev4.protCTe.infProt.chCTe;

                                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
                                Dominio.Enumeradores.TipoTomador tipoTomador = ObterTipoTomador(procCTev4.CTe.infCte.ide);

                                string cnpjTomador = "";

                                if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && (empresa.CNPJ == procCTev4.CTe.infCte.rem?.Item || procCTev4.CTe.infCte.rem?.Item.Substring(0, 8) == ""))
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                    cnpjTomador = Utilidades.String.OnlyNumbers(procCTev4.CTe.infCte.rem?.Item);
                                }
                                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && (empresa.CNPJ == procCTev4.CTe.infCte.dest?.Item || procCTev4.CTe.infCte.dest?.Item.Substring(0, 8) == ""))
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                    cnpjTomador = Utilidades.String.OnlyNumbers(procCTev4.CTe.infCte.dest?.Item);
                                }
                                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && (empresa.CNPJ == procCTev4.CTe.infCte.exped?.Item || procCTev4.CTe.infCte.exped?.Item.Substring(0, 8) == ""))
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                    cnpjTomador = Utilidades.String.OnlyNumbers(procCTev4.CTe.infCte.exped?.Item);
                                }
                                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && (empresa.CNPJ == procCTev4.CTe.infCte.receb?.Item || procCTev4.CTe.infCte.receb?.Item.Substring(0, 8) == ""))
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                    cnpjTomador = Utilidades.String.OnlyNumbers(procCTev4.CTe.infCte.receb?.Item);
                                }
                                else if (empresa.CNPJ == procCTev4.CTe.infCte.rem?.Item || procCTev4.CTe.infCte.rem?.Item.Substring(0, 8) == "")
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                                    cnpjTomador = Utilidades.String.OnlyNumbers(procCTev4.CTe.infCte.rem?.Item);
                                }
                                else if (empresa.CNPJ == procCTev4.CTe.infCte.dest?.Item || procCTev4.CTe.infCte.dest?.Item.Substring(0, 8) == "")
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                                    cnpjTomador = Utilidades.String.OnlyNumbers(procCTev4.CTe.infCte.dest?.Item);
                                }
                                else if (empresa.CNPJ == procCTev4.CTe.infCte.exped?.Item || procCTev4.CTe.infCte.exped?.Item.Substring(0, 8) == "")
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                                    cnpjTomador = Utilidades.String.OnlyNumbers(procCTev4.CTe.infCte.exped?.Item);
                                }
                                else if (empresa.CNPJ == procCTev4.CTe.infCte.receb?.Item || procCTev4.CTe.infCte.receb?.Item.Substring(0, 8) == "")
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                                    cnpjTomador = Utilidades.String.OnlyNumbers(procCTev4.CTe.infCte.receb?.Item);
                                }
                                else if (empresa.CNPJ == procCTev4.CTe.infCte.emit.Item || procCTev4.CTe.infCte.emit.Item.Substring(0, 8) == "")
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente;
                                    cnpjTomador = Utilidades.String.OnlyNumbers(procCTev4.CTe.infCte.emit?.Item);
                                }
                                else if (procCTev4.CTe.infCte.autXML != null && procCTev4.CTe.infCte.autXML.Where(o => o.Item == empresa.CNPJ).Count() > 0)
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
                                }
                                else
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;

                                tipoEvento = "";
                                versao = procCTev4.versao;
                                break;
                            case "procCTe_v3.00.xsd":
                                MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc procCTev3 = (new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)).Deserialize(xml) as MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc);
                                chave = procCTev3.protCTe.infProt.chCTe;

                                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
                                Dominio.Enumeradores.TipoTomador procCTev3tipoTomador = ObterTipoTomador(procCTev3.CTe.infCte.ide);

                                string procCTev3cnpjTomador = "";

                                if (procCTev3tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && (empresa.CNPJ == procCTev3.CTe.infCte.rem?.Item || procCTev3.CTe.infCte.rem?.Item.Substring(0, 8) == ""))
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                    procCTev3cnpjTomador = Utilidades.String.OnlyNumbers(procCTev3.CTe.infCte.rem?.Item);
                                }
                                else if (procCTev3tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && (empresa.CNPJ == procCTev3.CTe.infCte.dest?.Item || procCTev3.CTe.infCte.dest?.Item.Substring(0, 8) == ""))
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                    procCTev3cnpjTomador = Utilidades.String.OnlyNumbers(procCTev3.CTe.infCte.dest?.Item);
                                }
                                else if (procCTev3tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && (empresa.CNPJ == procCTev3.CTe.infCte.exped?.Item || procCTev3.CTe.infCte.exped?.Item.Substring(0, 8) == ""))
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                    procCTev3cnpjTomador = Utilidades.String.OnlyNumbers(procCTev3.CTe.infCte.exped?.Item);
                                }
                                else if (procCTev3tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && (empresa.CNPJ == procCTev3.CTe.infCte.receb?.Item || procCTev3.CTe.infCte.receb?.Item.Substring(0, 8) == ""))
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                    procCTev3cnpjTomador = Utilidades.String.OnlyNumbers(procCTev3.CTe.infCte.receb?.Item);
                                }
                                else if (empresa.CNPJ == procCTev3.CTe.infCte.rem?.Item || procCTev3.CTe.infCte.rem?.Item.Substring(0, 8) == "")
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                                    procCTev3cnpjTomador = Utilidades.String.OnlyNumbers(procCTev3.CTe.infCte.rem?.Item);
                                }
                                else if (empresa.CNPJ == procCTev3.CTe.infCte.dest?.Item || procCTev3.CTe.infCte.dest?.Item.Substring(0, 8) == "")
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                                    procCTev3cnpjTomador = Utilidades.String.OnlyNumbers(procCTev3.CTe.infCte.dest?.Item);
                                }
                                else if (empresa.CNPJ == procCTev3.CTe.infCte.exped?.Item || procCTev3.CTe.infCte.exped?.Item.Substring(0, 8) == "")
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                                    procCTev3cnpjTomador = Utilidades.String.OnlyNumbers(procCTev3.CTe.infCte.exped?.Item);
                                }
                                else if (empresa.CNPJ == procCTev3.CTe.infCte.receb?.Item || procCTev3.CTe.infCte.receb?.Item.Substring(0, 8) == "")
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                                    procCTev3cnpjTomador = Utilidades.String.OnlyNumbers(procCTev3.CTe.infCte.receb?.Item);
                                }
                                else if (empresa.CNPJ == procCTev3.CTe.infCte.emit.CNPJ || procCTev3.CTe.infCte.emit.CNPJ.Substring(0, 8) == "")
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente;
                                    procCTev3cnpjTomador = Utilidades.String.OnlyNumbers(procCTev3.CTe.infCte.emit?.CNPJ);
                                }
                                else if (procCTev3.CTe.infCte.autXML != null && procCTev3.CTe.infCte.autXML.Where(o => o.Item == empresa.CNPJ).Count() > 0)
                                {
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload;
                                }
                                else
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;

                                tipoEvento = "";
                                versao = procCTev3.versao;
                                break;
                            case "procCTeOS_v3.00.xsd":
                                MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc procCTeOSv3 = (new XmlSerializer(typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc)).Deserialize(xml) as MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteOSProc);
                                chave = procCTeOSv3.protCTe.infProt.chCTe;
                                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeOSDestinadoTomador;
                                tipoEvento = "";
                                versao = procCTeOSv3.versao;
                                break;
                            case "procCTeOS_v4.00.xsd":
                                MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc procCTeOSv4 = (new XmlSerializer(typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc)).Deserialize(xml) as MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteOSProc);
                                chave = procCTeOSv4.protCTe.infProt.chCTe;
                                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeOSDestinadoTomador;
                                tipoEvento = "";
                                versao = procCTeOSv4.versao;
                                break;
                            case "procEventoCTe_v4.00.xsd":
                                System.Xml.Linq.XDocument docrocEventoCTev4 = System.Xml.Linq.XDocument.Load(xml);
                                string tpEventoProcEventoCTev4 = docrocEventoCTev4.Descendants(docrocEventoCTev4.Root.Name.Namespace + "infEvento").FirstOrDefault()?.Element(docrocEventoCTev4.Root.Name.Namespace + "tpEvento")?.Value ?? string.Empty;
                                xml.Position = 0;
                                tipoEvento = tpEventoProcEventoCTev4;
                                if (tpEventoProcEventoCTev4 == "110111") //Cancelamento 4.0
                                {
                                    MultiSoftware.CTe.v400.Eventos.procEventoCTe procEventoCTev4 = (new XmlSerializer(typeof(MultiSoftware.CTe.v400.Eventos.procEventoCTe)).Deserialize(xml) as MultiSoftware.CTe.v400.Eventos.procEventoCTe);
                                    chave = procEventoCTev4.retEventoCTe.infEvento.chCTe;
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe;
                                    versao = procEventoCTev4.versao;
                                }
                                else
                                    mensagem = "Evento não implementado. " + tpEventoProcEventoCTev4;

                                break;
                            case "procEventoCTe_v3.00.xsd":
                                System.Xml.Linq.XDocument docProcEventoCTev3 = System.Xml.Linq.XDocument.Load(xml);
                                string tpEventoProcEventoCTev3 = docProcEventoCTev3.Descendants(docProcEventoCTev3.Root.Name.Namespace + "infEvento").FirstOrDefault()?.Element(docProcEventoCTev3.Root.Name.Namespace + "tpEvento")?.Value ?? string.Empty;
                                xml.Position = 0;
                                tipoEvento = tpEventoProcEventoCTev3;
                                if (tpEventoProcEventoCTev3 == "110111") //Cancelamento 3.0
                                {
                                    MultiSoftware.CTe.v300.Eventos.procEventoCTe procEventoCTev3 = (new XmlSerializer(typeof(MultiSoftware.CTe.v300.Eventos.procEventoCTe)).Deserialize(xml) as MultiSoftware.CTe.v300.Eventos.procEventoCTe);
                                    chave = procEventoCTev3.retEventoCTe.infEvento.chCTe;
                                    tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe;
                                    versao = procEventoCTev3.versao;
                                }
                                else
                                    mensagem = "Evento não implementado. " + tpEventoProcEventoCTev3;

                                break;
                            case "procMDFe_v3.00.xsd":
                                MultiSoftware.MDFe.v300.mdfeProc procMDFev3 = (new XmlSerializer(typeof(MultiSoftware.MDFe.v300.mdfeProc)).Deserialize(xml) as MultiSoftware.MDFe.v300.mdfeProc);
                                chave = procMDFev3.protMDFe.infProt.chMDFe;
                                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDFeDestinado;
                                tipoEvento = "";
                                versao = procMDFev3.versao;
                                break;
                            case "procEventoMDFe_v3.00.xsd":
                                System.Xml.Linq.XDocument docProcEventoMDFev3 = System.Xml.Linq.XDocument.Load(xml);
                                string tpEventoProcEventoMDFev3 = docProcEventoMDFev3.Descendants(docProcEventoMDFev3.Root.Name.Namespace + "infEvento").FirstOrDefault()?.Element(docProcEventoMDFev3.Root.Name.Namespace + "tpEvento")?.Value ?? string.Empty;
                                xml.Position = 0;
                                tipoEvento = tpEventoProcEventoMDFev3;
                                if (tpEventoProcEventoMDFev3 == "110111" || tpEventoProcEventoMDFev3 == "110112") //Cancelamento e Encerramento
                                {
                                    MultiSoftware.MDFe.v300.TProcEvento procEventoMDFev3 = (new XmlSerializer(typeof(MultiSoftware.MDFe.v300.TProcEvento)).Deserialize(xml) as MultiSoftware.MDFe.v300.TProcEvento);
                                    chave = procEventoMDFev3.retEventoMDFe.infEvento.chMDFe;
                                    tipoDocumento = (tpEventoProcEventoMDFev3 == "110111" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDFECancelado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.EncerramentoMDFe);
                                    versao = procEventoMDFev3.versao;
                                }
                                else
                                    mensagem = "Evento não implementado. " + tpEventoProcEventoMDFev3;

                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        situacao = SituacaoXml.ProblemaImportacao;
                        mensagem = "Erro ao Deserializar documentos destinados: " + ex.ToString();
                        Servicos.Log.TratarErro("Erro ao Deserializar documentos destinados: " + ex.ToString());
                    }

                    Repositorio.Embarcador.Documentos.DocumentoDestinadoXml repDocumentoDestinadoXml = new Repositorio.Embarcador.Documentos.DocumentoDestinadoXml(unidadeTrabalho);

                    if (!repDocumentoDestinadoXml.ExistePorNSU(empresa.Codigo, nsu))
                    {
                        Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml documentoXml = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml();
                        documentoXml.Empresa = new Dominio.Entidades.Empresa() { Codigo = empresa.Codigo };
                        documentoXml.DataIntegracao = DateTime.Now;
                        documentoXml.NomeSchema = schema;
                        documentoXml.NumeroSequencialUnico = nsu;
                        documentoXml.Chave = chave;
                        documentoXml.TipoDocumento = tipoDocumento;
                        documentoXml.TipoEvento = tipoEvento;
                        documentoXml.Versao = versao;
                        documentoXml.Mensagem = mensagem;

                        if (salvarXMl)
                            documentoXml.ConteudoXml = xmlString;

                        documentoXml.SituacaoXml = situacao;

                        repDocumentoDestinadoXml.Inserir(documentoXml);

                        idDocumentoXml = documentoXml.Codigo;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao Salvar DocumentoDestinadoXml documentos destinados: " + ex.ToString());
            }
        }

        private static void AtualizarSituacaoDocumentoDestinadoXml(SituacaoXml situacao, Int64 idDocumentoXMl, Repositorio.UnitOfWork unidadeTrabalho, string mensagem, string xml, bool salvarXml)
        {
            if (idDocumentoXMl > 0)
            {
                try
                {
                    Repositorio.Embarcador.Documentos.DocumentoDestinadoXml repDocumentoDestinadoXml = new Repositorio.Embarcador.Documentos.DocumentoDestinadoXml(unidadeTrabalho);
                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml documentoXml = repDocumentoDestinadoXml.BuscarPorCodigo(idDocumentoXMl, false);
                    if (documentoXml != null)
                    {
                        documentoXml.SituacaoXml = situacao;

                        if (situacao == SituacaoXml.ProblemaImportacao)
                        {
                            documentoXml.Mensagem = mensagem;
                            if (xml != string.Empty)
                                documentoXml.ConteudoXml = xml;
                        }
                        else if (!salvarXml)
                            documentoXml.ConteudoXml = "";


                        repDocumentoDestinadoXml.Atualizar(documentoXml);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Erro ao Atualizar DocumentoDestinadoXml documentos destinados: " + ex.ToString());
                }
            }
        }
    }
}

