using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using Zen.Barcode;

namespace ReportApi.ReportService
{
    public sealed class DAMDFE
    {
        public byte[] Gerar(int codigoMDFe, Repositorio.UnitOfWork unidadeDeTrabalho, bool contingencia)
        {
            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null || (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                                     mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                                     mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado &&
                                     mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                                     mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                                     !contingencia))
                    return null;

                Repositorio.DocumentoMunicipioDescarregamentoMDFe repCTeMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                Repositorio.NotaFiscalEletronicaMDFe repNFeMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unidadeDeTrabalho);
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
                Repositorio.ReboqueMDFe repReboqueMDFe = new Repositorio.ReboqueMDFe(unidadeDeTrabalho);
                Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);
                Repositorio.MDFeCIOT repCIOT = new Repositorio.MDFeCIOT(unidadeDeTrabalho);
                Repositorio.ValePedagioMDFe repValePedagioMDFe = new Repositorio.ValePedagioMDFe(unidadeDeTrabalho);
                Repositorio.MDFeSeguro repSeguros = new Repositorio.MDFeSeguro(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Relatorios.DocumentoMDFe> documentos = new List<Dominio.ObjetosDeValor.Relatorios.DocumentoMDFe>();
                documentos.AddRange(repCTeMDFe.BuscarDocumentosParaDAMDFE(mdfe.Codigo));
                documentos.AddRange(repNFeMDFe.BuscarDocumentosParaDAMDFE(mdfe.Codigo));

                List<Dominio.Entidades.VeiculoMDFe> veiculos = new List<Dominio.Entidades.VeiculoMDFe>() { repVeiculoMDFe.BuscarPorMDFe(codigoMDFe) };
                List<Dominio.Entidades.ReboqueMDFe> reboques = repReboqueMDFe.BuscarPorMDFe(mdfe.Codigo);
                List<Dominio.Entidades.MotoristaMDFe> motoristas = repMotoristaMDFe.BuscarPorMDFe(mdfe.Codigo);
                List<Dominio.Entidades.ValePedagioMDFe> valesPedagios = repValePedagioMDFe.BuscarPorMDFe(mdfe.Codigo);
                List<Dominio.Entidades.MDFeSeguro> seguros = repSeguros.BuscarPorMDFe(mdfe.Codigo);
                List<Dominio.Entidades.MDFeCIOT> ciots = repCIOT.BuscarPorMDFe(mdfe.Codigo);

                string mensagemMarcaDAgua = string.Empty,
                       mensagens = string.Empty,
                       observacoes = string.Empty,
                       chaveContingencia = string.Empty;

                if (mdfe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                    mensagemMarcaDAgua = "SEM VALOR FISCAL - AMBIENTE DE HOMOLOGAÇÃO";
                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado)
                    mensagemMarcaDAgua = "SEM VALOR FISCAL - CT-e CANCELADO NA SEFAZ";

                if (contingencia)
                {
                    Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);
                    chaveContingencia = svcMDFe.GerarChaveMDFe(mdfe);
                }

                if (!(mdfe.ObservacaoContribuinte ?? "").Contains("Estados"))
                {
                    Repositorio.PercursoMDFe repPercursoMDFe = new Repositorio.PercursoMDFe(unidadeDeTrabalho);
                    List<string> estados = repPercursoMDFe.BuscarSiglaEstadoPorMDFe(mdfe.Codigo);

                    if (!string.IsNullOrWhiteSpace(mdfe.ObservacaoContribuinte))
                        mdfe.ObservacaoContribuinte += " / ";
                    else
                        mdfe.ObservacaoContribuinte = "";

                    mdfe.ObservacaoContribuinte += "Estados de Passagem: " + string.Join(", ", estados) + ".";
                }

                List<Dominio.ObjetosDeValor.Relatorios.DAMDFE> damdfes = new List<Dominio.ObjetosDeValor.Relatorios.DAMDFE>();

                Dominio.ObjetosDeValor.Relatorios.DAMDFE damdfe = new Dominio.ObjetosDeValor.Relatorios.DAMDFE()
                {
                    BairroEmitente = mdfe.Empresa.Bairro,
                    CEPEmitente = mdfe.Empresa.CEP,
                    Chave = contingencia ? chaveContingencia : mdfe.Chave,
                    CIOT = mdfe?.CIOT ?? "",
                    CNPJEmitente = mdfe.Empresa.CNPJ,
                    Codigo = mdfe.Codigo,
                    CodigoBarras = GerarBarcodeMethod(contingencia ? chaveContingencia : mdfe.Chave, Zen.Barcode.BarcodeSymbology.Code128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Bmp),
                    ComplementoEmitente = mdfe.Empresa.Complemento,
                    DataAutorizacao = mdfe.DataAutorizacao,
                    DataEmissao = mdfe.DataEmissao,
                    IEEmitente = mdfe.Empresa.InscricaoEstadual,
                    Logomarca = Utilidades.Image.GetBmpFromPath(mdfe.Empresa.CaminhoLogoDacte),
                    LogradouroEmitente = mdfe.Empresa.Endereco,
                    MarcaDAgua = !string.IsNullOrWhiteSpace(mensagemMarcaDAgua) ? Utilidades.Image.DrawTextAngle45(mensagemMarcaDAgua) : null,
                    QRCode = !string.IsNullOrWhiteSpace(mdfe.QRCode) ? Utilidades.QRcode.Gerar(mdfe.QRCode) : null,
                    Modelo = mdfe.Modelo.Numero,
                    MunicipioEmitente = mdfe.Empresa.Localidade.Descricao,
                    Numero = mdfe.Numero,
                    NumeroEmitente = mdfe.Empresa.Numero,
                    Observacoes = !contingencia ? mdfe.ObservacaoFisco + " - " + mdfe.ObservacaoContribuinte : "MDF-e impresso em CONTINGÊNCIA dia " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "   " + mdfe.ObservacaoFisco + " - " + mdfe.ObservacaoContribuinte,
                    PesoTotal = mdfe.PesoBrutoMercadoria,
                    ProtocoloAutorizacao = !contingencia ? mdfe.Protocolo : "CONTINGÊNCIA " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    RNTRCEmitente = mdfe.Empresa.RegistroANTT,
                    QuantidadeCTe = (from obj in documentos where obj.Tipo == "CT-e" select obj).Count(),
                    QuantidadeCTRC = 0,
                    QuantidadeNF = 0,
                    QuantidadeNFe = (from obj in documentos where obj.Tipo == "NF-e" select obj).Count(),
                    RazaoSocialEmitente = mdfe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? mdfe.Empresa.RazaoSocial : "MDF-E EMITIDO EM AMBIENTE DE HOMOLOGAÇÃO",
                    Serie = mdfe.Serie.Numero,
                    UFCarregamento = mdfe.EstadoCarregamento.Sigla,
                    UFEmitente = mdfe.Empresa.Localidade.Estado.Sigla,
                    ValorTotalCarga = mdfe.ValorTotalMercadoria,
                    UFDescarregamento = mdfe.EstadoDescarregamento.Sigla,
                    UnidadeMedida = mdfe.UnidadeMedidaMercadoria == Dominio.Enumeradores.UnidadeMedidaMDFe.KG ? "Kg" : "Ton"
                };

                damdfes.Add(damdfe);

                ReportViewer reportViewer = new ReportViewer();

                string arquivoDoRelatorio = "";
                if (mdfe.Versao.Equals("3.00"))
                {
                    arquivoDoRelatorio = "DAMDFERetrato300.rdlc";

                    ReportDataSource dataSourceCiots1 = new ReportDataSource();
                    dataSourceCiots1.Name = "CIOTS1";
                    dataSourceCiots1.Value = ciots.Take(2);
                    reportViewer.LocalReport.DataSources.Add(dataSourceCiots1);

                    ReportDataSource dataSourceCiots2 = new ReportDataSource();
                    dataSourceCiots2.Name = "CIOTS2";
                    dataSourceCiots2.Value = ciots.Skip(2).Take(2);
                    reportViewer.LocalReport.DataSources.Add(dataSourceCiots2);

                    ReportDataSource dataSourceValesPedagio = new ReportDataSource();
                    dataSourceValesPedagio.Name = "ValesPedagio";
                    dataSourceValesPedagio.Value = valesPedagios.Take(2);
                    reportViewer.LocalReport.DataSources.Add(dataSourceValesPedagio);

                    ReportDataSource dataSourceSeguros = new ReportDataSource();
                    dataSourceSeguros.Name = "Seguros";
                    dataSourceSeguros.Value = seguros.Take(4);
                    reportViewer.LocalReport.DataSources.Add(dataSourceSeguros);
                }
                else
                {
                    arquivoDoRelatorio = "DAMDFERetrato.rdlc";

                    ReportDataSource dataSourceValesPedagio = new ReportDataSource();
                    dataSourceValesPedagio.Name = "Seguros";
                    dataSourceValesPedagio.Value = valesPedagios.Take(1);
                    reportViewer.LocalReport.DataSources.Add(dataSourceValesPedagio);
                }

                reportViewer.LocalReport.ReportPath = Utilidades.IO.FileStorageService.Storage.Combine(ReportApi.
                    Extensions.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory), "Areas", "Relatorios", "ReportViwer", arquivoDoRelatorio);

                ReportDataSource dataSourceVeiculos = new ReportDataSource();
                dataSourceVeiculos.Name = "Veiculos";
                dataSourceVeiculos.Value = veiculos.Take(1);
                reportViewer.LocalReport.DataSources.Add(dataSourceVeiculos);

                ReportDataSource dataSourceReboques = new ReportDataSource();
                dataSourceReboques.Name = "Reboques";
                dataSourceReboques.Value = reboques.Take(3);
                reportViewer.LocalReport.DataSources.Add(dataSourceReboques);

                ReportDataSource dataSourceDocumentosFiscais1 = new ReportDataSource();
                dataSourceDocumentosFiscais1.Name = "DocumentosFiscais1";
                dataSourceDocumentosFiscais1.Value = documentos.Take(40);
                reportViewer.LocalReport.DataSources.Add(dataSourceDocumentosFiscais1);

                ReportDataSource dataSourceDocumentosFiscais2 = new ReportDataSource();
                dataSourceDocumentosFiscais2.Name = "DocumentosFiscais2";
                dataSourceDocumentosFiscais2.Value = documentos.Skip(40).Take(40);
                reportViewer.LocalReport.DataSources.Add(dataSourceDocumentosFiscais2);

                ReportDataSource dataSourceCondutores = new ReportDataSource();
                dataSourceCondutores.Name = "Condutores";
                dataSourceCondutores.Value = motoristas.Take(6);
                reportViewer.LocalReport.DataSources.Add(dataSourceCondutores);

                ReportDataSource dataSourcedamdfes = new ReportDataSource();
                dataSourcedamdfes.Name = "DAMDFE";
                dataSourcedamdfes.Value = damdfes;
                reportViewer.LocalReport.DataSources.Add(dataSourcedamdfes);

                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                warnings = null;
                streamids = null;
                mimeType = null;
                encoding = null;
                filenameExtension = null;
                reportViewer = null;
                repCTeMDFe = null;
                repNFeMDFe = null;
                repVeiculoMDFe = null;
                repReboqueMDFe = null;
                repMotoristaMDFe = null;
                repValePedagioMDFe = null;
                mdfe = null;
                documentos = null;
                veiculos = null;
                reboques = null;
                motoristas = null;
                valesPedagios = null;
                damdfes = null;
                damdfe = null;

                GC.Collect();

                return bytes;
            }
            catch (Exception ex)
            {
                throw new ServerException($"{ex.Message} - {ex.StackTrace}");
            }

        }


        static byte[] GerarBarcodeMethod(string text, BarcodeSymbology symbology, BarcodeMetrics metrics, System.Drawing.Imaging.ImageFormat format)
        {
            using (System.Drawing.Image barcode = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.Code128).Draw(text, metrics))
            {
                using (System.IO.MemoryStream mm = new System.IO.MemoryStream())
                {
                    barcode.Save(mm, format);

                    return mm.ToArray();
                }
            }
        }
    }
}