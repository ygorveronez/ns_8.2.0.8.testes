using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Servicos;
using System.Threading.Tasks;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        //private static async Task<>
        //{

        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            //Frete cotação Havan
            string token = "56b7d5efc87348d69c6c283c8a7e31a3";

            InspectorBehavior inspector = new InspectorBehavior();
            WsFreteHavan.FretesClient svcFrete = new WsFreteHavan.FretesClient();
            svcFrete.Endpoint.EndpointBehaviors.Add(inspector);

            OperationContextScope scope = new OperationContextScope(svcFrete.InnerChannel);
            MessageHeader header = MessageHeader.CreateHeader("Token", "Token", token);
            OperationContext.Current.OutgoingMessageHeaders.Add(header);

            Dominio.ObjetosDeValor.WebService.Pedido.Cotacao cotacao = new Dominio.ObjetosDeValor.WebService.Pedido.Cotacao();
            cotacao.EnderecoDestino = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
            cotacao.EnderecoDestino.CEP = "89801000";
            cotacao.Expedidor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
            cotacao.Expedidor.CPFCNPJ = "79379491001406";

            cotacao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
            var produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
            produto.Altura = 0.120m;
            produto.CodigoGrupoProduto = "41237";
            produto.CodigoProduto = "46438-1";
            produto.Comprimento = 0.120m;
            produto.DescricaoGrupoProduto = "Finecasa";
            produto.DescricaoProduto = "Capsula De Nescafé Dolce Gusto 96G Nestle - Espresso";
            produto.Largura = 0.120m;
            produto.PesoUnitario = 0.185m;
            produto.Quantidade = 1;
            produto.ValorUnitario = 349.9m;

            cotacao.Produtos.Add(produto);

            cotacao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacao();
            cotacao.TipoOperacao.CodigoEmbarcador = "normal";

            //GravarLog("Inicio", "Havan");
            //for (var i = 0; i < 500; i++)
            //{
            //    GravarLog("Request " + i.ToString(), "Havan");

            //    try
            //    {
            //        Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao[]> retorno = svcFrete.ObterCotacao(cotacao);

            //        //var lastRequest = inspector.LastRequestXML;
            //        //var lastResponse = inspector.LastResponseXML;

            //        if (retorno.Status)
            //            GravarLog("Response " + i.ToString() + " sucesso.", "Havan");
            //        else
            //            GravarLog("Response " + i.ToString() + " " + retorno.Mensagem, "Havan");
            //    }
            //    catch (Exception ex)
            //    {
            //        GravarLog("Response " + i.ToString() + " erro: " + ex.Message, "Havan");
            //    }
            //}
            //GravarLog("Fim", "Havan");


            try
            {                

                List<Task<Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao[]>>> tasks = new List<Task<Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao[]>>>();

                for (var i = 0; i < 50; i++)
                {
                    async Task<Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao[]>> func()
                    {
                        return await svcFrete.ObterCotacaoAsync(cotacao);
                    }

                    tasks.Add(func());
                }

                GravarLog("Inicio", "Havan");
                Task.WaitAll(tasks.ToArray());
                GravarLog("Fim", "Havan");

                var postResponses = new List<string>();               
                var j = 1;
                GravarLog("Situações integrações", "Havan");
                foreach (var t in tasks)
                {
                    var postResponse = t; 
                    if (postResponse.Result.Status)
                        GravarLog("Response " + j.ToString() + " sucesso.", "Havan");
                    else
                        GravarLog("Response " + j.ToString()  + postResponse.Result.Mensagem, "Havan");
                    j++;
                }
            }
            catch (Exception ex)
            {
                GravarLog("Erro: " + ex.Message, "Havan");
            }

            produto.ValorUnitario = 349.9m;
            //HEINEKEN
            ////Criado objeto carga integração
            //Cargas.Ajustado.CargaIntegracao cargaIntegracao = new Cargas.Ajustado.CargaIntegracao();

            ////Criado veiculo principal
            //cargaIntegracao.Veiculo = new Cargas.Ajustado.Veiculo();
            //cargaIntegracao.Veiculo.Placa = "ASD1234";
            //cargaIntegracao.Veiculo.TipoVeiculo = Cargas.Ajustado.TipoVeiculo.Tracao;
            //cargaIntegracao.Veiculo.TipoPropriedadeVeiculo = Cargas.Ajustado.TipoPropriedadeVeiculo.Proprio;

            ////Criado lista de Reboques
            //cargaIntegracao.Veiculo.Reboques = new Cargas.Ajustado.ArrayOfReboque();

            ////Adicionado Reboque 1
            //Cargas.Ajustado.Reboque reboque1 = new Cargas.Ajustado.Reboque();
            //reboque1.Placa = "ASD1235";
            //reboque1.TipoVeiculo = Cargas.Ajustado.TipoVeiculo.Reboque;
            //reboque1.TipoPropriedadeVeiculo = Cargas.Ajustado.TipoPropriedadeVeiculo.Proprio;
            //cargaIntegracao.Veiculo.Reboques.Add(reboque1);

            ////Adicionado Reboque 2
            //Cargas.Ajustado.Reboque reboque2 = new Cargas.Ajustado.Reboque();
            //reboque2.Placa = "ASD1236";
            //reboque2.TipoVeiculo = Cargas.Ajustado.TipoVeiculo.Reboque;
            //reboque2.TipoPropriedadeVeiculo = Cargas.Ajustado.TipoPropriedadeVeiculo.Proprio;
            //cargaIntegracao.Veiculo.Reboques.Add(reboque2);

            ////Envio da integração
            //string token = "9b0e33610d1b49b680f26c9e557c3599";

            //InspectorBehavior inspector = new InspectorBehavior();
            //Cargas.Ajustado.CargasClient svcCarga = new Cargas.Ajustado.CargasClient();
            //svcCarga.Endpoint.EndpointBehaviors.Add(inspector);

            //OperationContextScope scope = new OperationContextScope(svcCarga.InnerChannel);
            //MessageHeader header = MessageHeader.CreateHeader("Token", "Token", token);
            //OperationContext.Current.OutgoingMessageHeaders.Add(header);

            //Cargas.Ajustado.RetornoOfProtocolosVQbIXuKl retorno = null;

            //try
            //{
            //    retorno = svcCarga.AdicionarCarga(cargaIntegracao);
            //}
            //catch (Exception ex)
            //{

            //}

            //var xmlEnvio = inspector.LastRequestXML;
            //var xmlRetorno = inspector.LastResponseXML;



            //MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.retDistDFeInt retorno = MultiSoftware.NFe.NFeDistribuicaoDFe.Servicos.DistribuicaoDFe.ConsultarDocumentosFiscais("82809088000151", 1, MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TCodUfIBGE.Item42, @"C:\TOMBINI & CIA. LTDA82809088000151.pfx", "1234");

            //if (retorno.cStat == "138")
            //{
            //    foreach (var dfe in retorno.loteDistDFeInt.docZip)
            //    {
            //        if (dfe.schema == "resNFe_v1.00.xsd")
            //        {
            //            using (MemoryStream compressedStream = new MemoryStream(dfe.Value))
            //            {
            //                using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            //                {
            //                    using (MemoryStream resultStream = new MemoryStream())
            //                    {
            //                        zipStream.CopyTo(resultStream);

            //                        string documento = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());

            //                        resultStream.Position = 0;

            //                        XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe));

            //                        MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe nfe = (ser.Deserialize(resultStream) as MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.resNFe);

            //                        MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Servicos.ManifestacaoDestinatario svcManifestacaoDestinatario = new MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Servicos.ManifestacaoDestinatario();

            //                        //svcManifestacaoDestinatario.EnviarManifestacaoDestinatario("82809088000151", MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TAmb.Item2, MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TEventoInfEventoTpEvento.Item210200, nfe.chNFe, @"C:\TOMBINI & CIA. LTDA82809088000151.pfx", "1234");

            //                        MultiSoftware.NFe.NFeDownloadNF.Servicos.DownloadNFe svcDownloadNFe = new MultiSoftware.NFe.NFeDownloadNF.Servicos.DownloadNFe();

            //                        MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TRetDownloadNFe retornoDownload = svcDownloadNFe.RealizarDownload("82809088000151", MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TAmb.Item1, "42160675339051000141550070006328691190648122", @"C:\TOMBINI & CIA. LTDA82809088000151.pfx", "1234");
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}




            //Servicos.Natura svcNatura = new Servicos.Natura(Conexao.StringConexao);
            //Servicos.ServicoNatura.ProcessaOcorrencias.SI_ProcessaOcorrenciasAsync_OBClient svcOcorrencia = svcNatura.ObterClientNatura<Servicos.ServicoNatura.ProcessaOcorrencias.SI_ProcessaOcorrenciasAsync_OBClient, Servicos.ServicoNatura.ProcessaOcorrencias.SI_ProcessaOcorrenciasAsync_OB>("133881059", "natura15");

            //List<Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias> ocorrencias = new List<Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias>();


            //ocorrencias.Add(new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias()
            //{
            //    chaveNFe = "35151200190373000849550010000043712284684358",
            //    codigoOcorrencia = "01",
            //    dataOcorrencia = (new DateTime(2015, 12, 02)).ToString("yyyy-MM-dd"),
            //    horaOcorrencia = (new DateTime(2015, 12, 02, 10, 10, 10)).ToString("HH:mm:ss"),
            //    textoOcorrencia = ""
            //});

            //svcOcorrencia.SI_ProcessaOcorrenciasAsync_OBAsync(new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrencias()
            //{
            //    dados = new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDados()
            //    {
            //        codTranspMatriz = "T982",
            //        documentoTransporte = "953925",
            //        ocorrencias = ocorrencias.ToArray()
            //    }
            //});

            //Servicos.ServicoNatura.ProcessaPreFatura.SI_ProcessaPreFaturaSync_OBClient svcPreFatura = svcNatura.ObterClientNatura<Servicos.ServicoNatura.ProcessaPreFatura.SI_ProcessaPreFaturaSync_OBClient, Servicos.ServicoNatura.ProcessaPreFatura.SI_ProcessaPreFaturaSync_OB>("133881059", "natura15");

            //var retorno = svcPreFatura.SI_ProcessaPreFaturaSync_OB(new Servicos.ServicoNatura.ProcessaPreFatura.DT_EnviaParamPreFatura()
            //{
            //    dados = new Servicos.ServicoNatura.ProcessaPreFatura.DT_EnviaParamPreFaturaDados()
            //    {
            //          codTranspEmitente= "T982",
            //        codTranspMatriz = "T982",
            //        numeroPreFatura = "5000000072"
            //    }
            //});

            //System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            //Servicos.ServicoNatura.ProcessaFatura.SI_ProcessaFaturaAsync_OBClient svcFatura = svcNatura.ObterClientNatura<Servicos.ServicoNatura.ProcessaFatura.SI_ProcessaFaturaAsync_OBClient, Servicos.ServicoNatura.ProcessaFatura.SI_ProcessaFaturaAsync_OB>("133881059", "natura15");

            //List<Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItens> itens = new List<Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItens>();

            //itens.Add(new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItens()
            //{
            //    chaveCTe = retorno[0].itens[0].chaveCTe,
            //    codTranspEmit = retorno[0].itens[0].codTranspEmit,
            //    dataDocFiscal = retorno[0].itens[0].dataDocFiscal,
            //    docFiscalRef = (from obj in retorno[0].itens[0].docFiscalRef
            //                    select new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensDocFiscalRef()
            //                    {
            //                        chaveNFeCTe = obj.chaveNFeCTe,
            //                        dataEmissao = obj.dataEmissao,
            //                        modelo = obj.modelo.ToString(),
            //                        numero = obj.numero,
            //                        serie = obj.serie
            //                    }).ToArray(),
            //    impostos = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensImpostos()
            //    {
            //        aliquotaCOFINS = retorno[0].itens[0].impostos.aliquotaCOFINS,
            //        aliquotaCOFINSSpecified = retorno[0].itens[0].impostos.aliquotaCOFINSSpecified,
            //        aliquotaICMS = retorno[0].itens[0].impostos.aliquotaICMS,
            //        aliquotaICMSSpecified = retorno[0].itens[0].impostos.aliquotaICMSSpecified,
            //        aliquotaICMSST = retorno[0].itens[0].impostos.aliquotaICMSST,
            //        aliquotaICMSSTSpecified = retorno[0].itens[0].impostos.aliquotaICMSSTSpecified,
            //        aliquotaISS = retorno[0].itens[0].impostos.aliquotaISS,
            //        aliquotaISSSpecified = retorno[0].itens[0].impostos.aliquotaISSSpecified,
            //        aliquotaPIS = retorno[0].itens[0].impostos.aliquotaPIS,
            //        aliquotaPISSpecified = retorno[0].itens[0].impostos.aliquotaPISSpecified,
            //        baseCOFINS = float.Parse(retorno[0].itens[0].impostos.baseCOFINS, cultura),
            //        baseCOFINSSpecified = true,
            //        baseICMS = float.Parse(retorno[0].itens[0].impostos.baseICMS, cultura),
            //        baseICMSSpecified = true,
            //        baseICMSST = float.Parse(retorno[0].itens[0].impostos.baseICMSST, cultura),
            //        baseICMSSTSpecified = true,
            //        baseISS = float.Parse(retorno[0].itens[0].impostos.baseISS, cultura),
            //        baseISSSpecified = true,
            //        basePIS = float.Parse(retorno[0].itens[0].impostos.basePIS, cultura),
            //        basePISSpecified = true,
            //        ivaICMSST = !string.IsNullOrWhiteSpace(retorno[0].itens[0].impostos.ivaICMSST) ? float.Parse(retorno[0].itens[0].impostos.ivaICMSST, cultura) : 0,
            //        ivaICMSSTSpecified = true,
            //        valorCOFINS = float.Parse(retorno[0].itens[0].impostos.valorCOFINS, cultura),
            //        valorCOFINSSpecified = true,
            //        valorICMS = float.Parse(retorno[0].itens[0].impostos.valorICMS, cultura),
            //        valorICMSSpecified = true,
            //        valorICMSST = float.Parse(retorno[0].itens[0].impostos.valorICMSST, cultura),
            //        valorICMSSTSpecified = true,
            //        valorISS = float.Parse(retorno[0].itens[0].impostos.valorISS, cultura),
            //        valorISSSpecified = true,
            //        valorPIS = float.Parse(retorno[0].itens[0].impostos.valorPIS, cultura),
            //        valorPISSpecified = true
            //    },
            //    modeloDocFiscal = retorno[0].itens[0].modeloDocFiscal,
            //    numeroDocFiscal = retorno[0].itens[0].numeroDocFiscal,
            //    serieDocFiscal = retorno[0].itens[0].serieDocFiscal,
            //    transporte = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensTransporte()
            //    {
            //        difValorFrete = float.Parse(retorno[0].itens[0].transporte.difValorFrete, cultura),
            //        difValorFreteSpecified = true,
            //        docTransporte = retorno[0].itens[0].transporte.docTransporte,
            //        valorFrete = float.Parse(retorno[0].itens[0].transporte.valorFrete, cultura)
            //    }
            //});

            //itens.Add(new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItens()
            //{
            //    chaveCTe = retorno[0].itens[1].chaveCTe,
            //    codTranspEmit = retorno[0].itens[1].codTranspEmit,
            //    dataDocFiscal = retorno[0].itens[1].dataDocFiscal,
            //    docFiscalRef = (from obj in retorno[0].itens[1].docFiscalRef
            //                    select new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensDocFiscalRef()
            //                    {
            //                        chaveNFeCTe = obj.chaveNFeCTe,
            //                        dataEmissao = obj.dataEmissao,
            //                        modelo = obj.modelo,
            //                        numero = obj.numero,
            //                        serie = obj.serie
            //                    }).ToArray(),
            //    impostos = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensImpostos()
            //    {
            //        aliquotaCOFINS = retorno[0].itens[1].impostos.aliquotaCOFINS,
            //        aliquotaCOFINSSpecified = retorno[0].itens[1].impostos.aliquotaCOFINSSpecified,
            //        aliquotaICMS = retorno[0].itens[1].impostos.aliquotaICMS,
            //        aliquotaICMSSpecified = retorno[0].itens[1].impostos.aliquotaICMSSpecified,
            //        aliquotaICMSST = retorno[0].itens[1].impostos.aliquotaICMSST,
            //        aliquotaICMSSTSpecified = retorno[0].itens[1].impostos.aliquotaICMSSTSpecified,
            //        aliquotaISS = retorno[0].itens[1].impostos.aliquotaISS,
            //        aliquotaISSSpecified = retorno[0].itens[1].impostos.aliquotaISSSpecified,
            //        aliquotaPIS = retorno[0].itens[1].impostos.aliquotaPIS,
            //        aliquotaPISSpecified = retorno[0].itens[1].impostos.aliquotaPISSpecified,
            //        baseCOFINS = float.Parse(retorno[0].itens[1].impostos.baseCOFINS, cultura),
            //        baseCOFINSSpecified = true,
            //        baseICMS = float.Parse(retorno[0].itens[1].impostos.baseICMS, cultura),
            //        baseICMSSpecified = true,
            //        baseICMSST = float.Parse(retorno[0].itens[1].impostos.baseICMSST, cultura),
            //        baseICMSSTSpecified = true,
            //        baseISS = float.Parse(retorno[0].itens[1].impostos.baseISS, cultura),
            //        baseISSSpecified = true,
            //        basePIS = float.Parse(retorno[0].itens[1].impostos.basePIS, cultura),
            //        basePISSpecified = true,
            //        ivaICMSST = !string.IsNullOrWhiteSpace(retorno[0].itens[1].impostos.ivaICMSST) ? float.Parse(retorno[0].itens[1].impostos.ivaICMSST, cultura) : 0,
            //        ivaICMSSTSpecified = true,
            //        valorCOFINS = float.Parse(retorno[0].itens[1].impostos.valorCOFINS, cultura),
            //        valorCOFINSSpecified = true,
            //        valorICMS = float.Parse(retorno[0].itens[1].impostos.valorICMS, cultura),
            //        valorICMSSpecified = true,
            //        valorICMSST = float.Parse(retorno[0].itens[1].impostos.valorICMSST, cultura),
            //        valorICMSSTSpecified = true,
            //        valorISS = float.Parse(retorno[0].itens[1].impostos.valorISS, cultura),
            //        valorISSSpecified = true,
            //        valorPIS = float.Parse(retorno[0].itens[1].impostos.valorPIS, cultura),
            //        valorPISSpecified = true
            //    },
            //    modeloDocFiscal = retorno[0].itens[1].modeloDocFiscal,
            //    numeroDocFiscal = retorno[0].itens[1].numeroDocFiscal,
            //    serieDocFiscal = retorno[0].itens[1].serieDocFiscal,
            //    transporte = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensTransporte()
            //    {
            //        difValorFrete = float.Parse(retorno[0].itens[1].transporte.difValorFrete, cultura),
            //        difValorFreteSpecified = true,
            //        docTransporte = retorno[0].itens[1].transporte.docTransporte,
            //        valorFrete = float.Parse(retorno[0].itens[1].transporte.valorFrete, cultura)
            //    }
            //});

            //itens.Add(new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItens()
            //{
            //    chaveCTe = retorno[0].itens[2].chaveCTe,
            //    codTranspEmit = retorno[0].itens[2].codTranspEmit,
            //    dataDocFiscal = retorno[0].itens[2].dataDocFiscal,
            //    docFiscalRef = (from obj in retorno[0].itens[2].docFiscalRef
            //                    select new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensDocFiscalRef()
            //                    {
            //                        chaveNFeCTe = obj.chaveNFeCTe,
            //                        dataEmissao = obj.dataEmissao,
            //                        modelo = obj.modelo,
            //                        numero = obj.numero,
            //                        serie = obj.serie
            //                    }).ToArray(),
            //    impostos = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensImpostos()
            //    {
            //        aliquotaCOFINS = retorno[0].itens[2].impostos.aliquotaCOFINS,
            //        aliquotaCOFINSSpecified = retorno[0].itens[2].impostos.aliquotaCOFINSSpecified,
            //        aliquotaICMS = retorno[0].itens[2].impostos.aliquotaICMS,
            //        aliquotaICMSSpecified = retorno[0].itens[2].impostos.aliquotaICMSSpecified,
            //        aliquotaICMSST = retorno[0].itens[2].impostos.aliquotaICMSST,
            //        aliquotaICMSSTSpecified = retorno[0].itens[2].impostos.aliquotaICMSSTSpecified,
            //        aliquotaISS = retorno[0].itens[2].impostos.aliquotaISS,
            //        aliquotaISSSpecified = retorno[0].itens[2].impostos.aliquotaISSSpecified,
            //        aliquotaPIS = retorno[0].itens[2].impostos.aliquotaPIS,
            //        aliquotaPISSpecified = retorno[0].itens[2].impostos.aliquotaPISSpecified,
            //        baseCOFINS = float.Parse(retorno[0].itens[2].impostos.baseCOFINS, cultura),
            //        baseCOFINSSpecified = true,
            //        baseICMS = float.Parse(retorno[0].itens[2].impostos.baseICMS, cultura),
            //        baseICMSSpecified = true,
            //        baseICMSST = float.Parse(retorno[0].itens[2].impostos.baseICMSST, cultura),
            //        baseICMSSTSpecified = true,
            //        baseISS = float.Parse(retorno[0].itens[2].impostos.baseISS, cultura),
            //        baseISSSpecified = true,
            //        basePIS = float.Parse(retorno[0].itens[2].impostos.basePIS, cultura),
            //        basePISSpecified = true,
            //        ivaICMSST = !string.IsNullOrWhiteSpace(retorno[0].itens[2].impostos.ivaICMSST) ? float.Parse(retorno[0].itens[2].impostos.ivaICMSST, cultura) : 0,
            //        ivaICMSSTSpecified = true,
            //        valorCOFINS = float.Parse(retorno[0].itens[2].impostos.valorCOFINS, cultura),
            //        valorCOFINSSpecified = true,
            //        valorICMS = float.Parse(retorno[0].itens[2].impostos.valorICMS, cultura),
            //        valorICMSSpecified = true,
            //        valorICMSST = float.Parse(retorno[0].itens[2].impostos.valorICMSST, cultura),
            //        valorICMSSTSpecified = true,
            //        valorISS = float.Parse(retorno[0].itens[2].impostos.valorISS, cultura),
            //        valorISSSpecified = true,
            //        valorPIS = float.Parse(retorno[0].itens[2].impostos.valorPIS, cultura),
            //        valorPISSpecified = true
            //    },
            //    modeloDocFiscal = retorno[0].itens[2].modeloDocFiscal,
            //    numeroDocFiscal = retorno[0].itens[2].numeroDocFiscal,
            //    serieDocFiscal = retorno[0].itens[2].serieDocFiscal,
            //    transporte = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensTransporte()
            //    {
            //        difValorFrete = float.Parse(retorno[0].itens[2].transporte.difValorFrete, cultura),
            //        difValorFreteSpecified = true,
            //        docTransporte = retorno[0].itens[2].transporte.docTransporte,
            //        valorFrete = float.Parse(retorno[0].itens[2].transporte.valorFrete, cultura)
            //    }
            //});

            //itens.Add(new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItens()
            //{
            //    chaveCTe = retorno[0].itens[1].chaveCTe,
            //    codTranspEmit = retorno[0].itens[1].codTranspEmit,
            //    dataDocFiscal = retorno[0].itens[1].dataDocFiscal,
            //    docFiscalRef = (from obj in retorno[0].itens[1].docFiscalRef
            //                    select new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensDocFiscalRef()
            //                    {
            //                        chaveNFeCTe = obj.chaveNFeCTe,
            //                        dataEmissao = obj.dataEmissao,
            //                        modelo = obj.modelo,
            //                        numero = obj.numero,
            //                        serie = obj.serie
            //                    }).ToArray(),
            //    impostos = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensImpostos()
            //    {
            //        aliquotaCOFINS = retorno[0].itens[1].impostos.aliquotaCOFINS,
            //        aliquotaCOFINSSpecified = retorno[0].itens[1].impostos.aliquotaCOFINSSpecified,
            //        aliquotaICMS = retorno[0].itens[1].impostos.aliquotaICMS,
            //        aliquotaICMSSpecified = retorno[0].itens[1].impostos.aliquotaICMSSpecified,
            //        aliquotaICMSST = retorno[0].itens[1].impostos.aliquotaICMSST,
            //        aliquotaICMSSTSpecified = retorno[0].itens[1].impostos.aliquotaICMSSTSpecified,
            //        aliquotaISS = retorno[0].itens[1].impostos.aliquotaISS,
            //        aliquotaISSSpecified = retorno[0].itens[1].impostos.aliquotaISSSpecified,
            //        aliquotaPIS = retorno[0].itens[1].impostos.aliquotaPIS,
            //        aliquotaPISSpecified = retorno[0].itens[1].impostos.aliquotaPISSpecified,
            //        baseCOFINS = float.Parse(retorno[0].itens[1].impostos.baseCOFINS, cultura),
            //        baseCOFINSSpecified = true,
            //        baseICMS = retorno[0].itens[1].impostos.baseICMS,
            //        baseICMSSpecified = retorno[0].itens[1].impostos.baseICMSSpecified,
            //        baseICMSST = retorno[0].itens[1].impostos.baseICMSST,
            //        baseICMSSTSpecified = retorno[0].itens[1].impostos.baseICMSSTSpecified,
            //        baseISS = retorno[0].itens[1].impostos.baseISS,
            //        baseISSSpecified = retorno[0].itens[1].impostos.baseISSSpecified,
            //        basePIS = retorno[0].itens[1].impostos.basePIS,
            //        basePISSpecified = retorno[0].itens[1].impostos.basePISSpecified,
            //        ivaICMSST = retorno[0].itens[1].impostos.ivaICMSST,
            //        ivaICMSSTSpecified = retorno[0].itens[1].impostos.ivaICMSSTSpecified,
            //        valorCOFINS = retorno[0].itens[1].impostos.valorCOFINS,
            //        valorCOFINSSpecified = retorno[0].itens[1].impostos.valorCOFINSSpecified,
            //        valorICMS = retorno[0].itens[1].impostos.valorICMS,
            //        valorICMSSpecified = retorno[0].itens[1].impostos.valorICMSSpecified,
            //        valorICMSST = retorno[0].itens[1].impostos.valorICMSST,
            //        valorICMSSTSpecified = retorno[0].itens[1].impostos.valorICMSSTSpecified,
            //        valorISS = retorno[0].itens[1].impostos.valorISS,
            //        valorISSSpecified = retorno[0].itens[1].impostos.valorISSSpecified,
            //        valorPIS = retorno[0].itens[1].impostos.valorPIS,
            //        valorPISSpecified = retorno[0].itens[1].impostos.valorPISSpecified
            //    },
            //    modeloDocFiscal = retorno[0].itens[1].modeloDocFiscal,
            //    numeroDocFiscal = retorno[0].itens[1].numeroDocFiscal,
            //    serieDocFiscal = retorno[0].itens[1].serieDocFiscal,
            //    transporte = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensTransporte()
            //    {
            //        difValorFrete = retorno[0].itens[1].transporte.difValorFrete,
            //        difValorFreteSpecified = retorno[0].itens[1].transporte.difValorFreteSpecified,
            //        docTransporte = retorno[0].itens[1].transporte.docTransporte,
            //        valorFrete = retorno[0].itens[1].transporte.valorFrete
            //    }
            //});

            //itens.Add(new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItens()
            //{
            //    chaveCTe = retorno[0].itens[2].chaveCTe,
            //    codTranspEmit = retorno[0].itens[2].codTranspEmit,
            //    dataDocFiscal = retorno[0].itens[2].dataDocFiscal,
            //    docFiscalRef = (from obj in retorno[0].itens[2].docFiscalRef
            //                    select new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensDocFiscalRef()
            //                    {
            //                        chaveNFeCTe = obj.chaveNFeCTe,
            //                        dataEmissao = obj.dataEmissao,
            //                        modelo = obj.modelo,
            //                        numero = obj.numero,
            //                        serie = obj.serie
            //                    }).ToArray(),
            //    impostos = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensImpostos()
            //    {
            //        aliquotaCOFINS = retorno[0].itens[2].impostos.aliquotaCOFINS,
            //        aliquotaCOFINSSpecified = retorno[0].itens[2].impostos.aliquotaCOFINSSpecified,
            //        aliquotaICMS = retorno[0].itens[2].impostos.aliquotaICMS,
            //        aliquotaICMSSpecified = retorno[0].itens[2].impostos.aliquotaICMSSpecified,
            //        aliquotaICMSST = retorno[0].itens[2].impostos.aliquotaICMSST,
            //        aliquotaICMSSTSpecified = retorno[0].itens[2].impostos.aliquotaICMSSTSpecified,
            //        aliquotaISS = retorno[0].itens[2].impostos.aliquotaISS,
            //        aliquotaISSSpecified = retorno[0].itens[2].impostos.aliquotaISSSpecified,
            //        aliquotaPIS = retorno[0].itens[2].impostos.aliquotaPIS,
            //        aliquotaPISSpecified = retorno[0].itens[2].impostos.aliquotaPISSpecified,
            //        baseCOFINS = retorno[0].itens[2].impostos.baseCOFINS,
            //        baseCOFINSSpecified = retorno[0].itens[2].impostos.baseCOFINSSpecified,
            //        baseICMS = retorno[0].itens[2].impostos.baseICMS,
            //        baseICMSSpecified = retorno[0].itens[2].impostos.baseICMSSpecified,
            //        baseICMSST = retorno[0].itens[2].impostos.baseICMSST,
            //        baseICMSSTSpecified = retorno[0].itens[2].impostos.baseICMSSTSpecified,
            //        baseISS = retorno[0].itens[2].impostos.baseISS,
            //        baseISSSpecified = retorno[0].itens[2].impostos.baseISSSpecified,
            //        basePIS = retorno[0].itens[2].impostos.basePIS,
            //        basePISSpecified = retorno[0].itens[2].impostos.basePISSpecified,
            //        ivaICMSST = retorno[0].itens[2].impostos.ivaICMSST,
            //        ivaICMSSTSpecified = retorno[0].itens[2].impostos.ivaICMSSTSpecified,
            //        valorCOFINS = retorno[0].itens[2].impostos.valorCOFINS,
            //        valorCOFINSSpecified = retorno[0].itens[2].impostos.valorCOFINSSpecified,
            //        valorICMS = retorno[0].itens[2].impostos.valorICMS,
            //        valorICMSSpecified = retorno[0].itens[2].impostos.valorICMSSpecified,
            //        valorICMSST = retorno[0].itens[2].impostos.valorICMSST,
            //        valorICMSSTSpecified = retorno[0].itens[2].impostos.valorICMSSTSpecified,
            //        valorISS = retorno[0].itens[2].impostos.valorISS,
            //        valorISSSpecified = retorno[0].itens[2].impostos.valorISSSpecified,
            //        valorPIS = retorno[0].itens[2].impostos.valorPIS,
            //        valorPISSpecified = retorno[0].itens[2].impostos.valorPISSpecified
            //    },
            //    modeloDocFiscal = retorno[0].itens[2].modeloDocFiscal,
            //    numeroDocFiscal = retorno[0].itens[2].numeroDocFiscal,
            //    serieDocFiscal = retorno[0].itens[2].serieDocFiscal,
            //    transporte = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensTransporte()
            //    {
            //        difValorFrete = retorno[0].itens[2].transporte.difValorFrete,
            //        difValorFreteSpecified = retorno[0].itens[2].transporte.difValorFreteSpecified,
            //        docTransporte = retorno[0].itens[2].transporte.docTransporte,
            //        valorFrete = retorno[0].itens[2].transporte.valorFrete
            //    }
            //});

            //svcFatura.SI_ProcessaFaturaAsync_OBAsync(new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFatura()
            //{
            //    dados = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDados()
            //    {
            //        codTranspMatriz = retorno[0].codTranspMatriz,
            //        dataFatura = DateTime.Now,
            //        dataPreFatura = retorno[0].dataPreFatura,
            //        dataPreFaturaSpecified = retorno[0].dataPreFaturaSpecified,
            //        dataVencFatura = new DateTime(2015, 12, 10),
            //        numeroFatura = "5",
            //        numeroPreFatura = retorno[0].numeroPreFatura,
            //        itens = itens.ToArray()
            //    }
            //});

            //Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(Conexao.StringConexao);

            //Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(160603);

            //Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            //string dacte = svcCTe.ObterDACTE(cte.Codigo, cte.Empresa.Codigo);

            //System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");

            //byte[] decodedData = encoding.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, encoding, Convert.FromBase64String(dacte))).ToArray();

            //Response.ContentType = "application/octetstream";
            //Response.AppendHeader("content-disposition", "attachment; filename=dacte.pdf");
            //Response.BinaryWrite(decodedData);

            //Response.End();

            //object xml = MultiSoftware.NFe.Servicos.Leitura.Ler(System.IO.File.OpenRead("E:\\nota natura teste.xml"));


            //Servicos.Natura svcNatura = new Servicos.Natura();

            //svcNatura.ConsultarNFesParaEmissao(3, "4170", new Repositorio.UnitOfWork(Conexao.StringConexao));

            //var serie = "43150895422218000140550050005380841257312273".Substring(23, 2);

            //ServicoOcorrenciaCTeTransportador.OcorrenciaCTeClient svcOcorrencia = new ServicoOcorrenciaCTeTransportador.OcorrenciaCTeClient();

            //var retorno = svcOcorrencia.ConsultarOcorrencia("13496023000180", "10970441000170", "1F883BAC-9506-42D1-BCAC-8A24EC3A53DA", "42130610970441000170550010000001091662589523", 0);

            //if (retorno.Status)
            //{

            //}

            //ServicoMDFeTransportador.MDFeClient svcMDFe = new ServicoMDFeTransportador.MDFeClient();

            //var retorno = svcMDFe.ObterProtocolos("13496023000180", "1F883BAC-9506-42D1-BCAC-8A24EC3A53DA", "01/01/2015", "22/06/2015");

            //var xmls = svcMDFe.ObterXML("13496023000180", "1F883BAC-9506-42D1-BCAC-8A24EC3A53DA", retorno.Objeto);

            //if (xmls.Status)
            //{

            //}


            //ServicoCTeTransportador.CTeClient svcCTe = new ServicoCTeTransportador.CTeClient();

            //var retorno = svcCTe.ObterProtocolos("13496023000180", "1F883BAC-9506-42D1-BCAC-8A24EC3A53DA", "01/06/2015", "22/06/2015");

            //var xmls = svcCTe.ObterXML("13496023000180", "1F883BAC-9506-42D1-BCAC-8A24EC3A53DA", retorno.Objeto);

            //if (xmls.Status)
            //{

            //}










            //ServicoIntegracaoNFSe.IntegracaoNFSeClient svcConsulta = new ServicoIntegracaoNFSe.IntegracaoNFSeClient();
            //WSNFSe.NotaFiscalDeServicoEletronicaClient svcNFSe = new WSNFSe.NotaFiscalDeServicoEletronicaClient();

            //var retornoCancelamento = svcNFSe.CancelarNFSe("13969629000196", 1046, "TESTE DE CANCELAMENTO WS", "D37AC5E6-BCA1-45A5-B4F8-FFAD7F4535BD");
            //var retornoConsultaCancelamento = svcConsulta.BuscarPorCodigoNFSe(1046, Dominio.Enumeradores.TipoIntegracaoNFSe.Cancelamento, Dominio.Enumeradores.TipoRetornoIntegracao.Todos, "D37AC5E6-BCA1-45A5-B4F8-FFAD7F4535BD");

            //Dominio.ObjetosDeValor.NFSe.NFSe nfse = new Dominio.ObjetosDeValor.NFSe.NFSe();

            //nfse.AliquotaISS = 1;
            //nfse.BaseCalculoISS = 100;
            //nfse.CodigoIBGECidadePrestacaoServico = 4204707;
            //nfse.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //nfse.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa() { CNPJ = "14749256000100", Atualizar = false };
            //nfse.Intermediario = null;
            //nfse.ISSRetido = false;
            //nfse.Itens = new List<Dominio.ObjetosDeValor.NFSe.Item>(){
            //    new Dominio.ObjetosDeValor.NFSe.Item (){
            //         AliquotaISS = 1,
            //          BaseCalculoISS = 100,
            //           CodigoIBGECidade = 4204707,
            //            CodigoIBGECidadeIncidencia = 4204707,
            //             CodigoPaisPrestacaoServico = 1058,
            //             Discriminacao = "TESTE EMISSAO SERVICO",
            //              ExigibilidadeISS = 0,
            //               Quantidade = 1,
            //                Servico = new Dominio.ObjetosDeValor.NFSe.Servico(){
            //                     Aliquota = 0.10m,
            //                     Descricao = "TESTE",
            //                      Numero = 1401,
            //                       CodigoTributacao = null,
            //                       CNAE = null
            //                },
            //                 ServicoPrestadoNoPais = true,
            //                  ValorServico = 100,
            //                   ValorISS = 1,
            //                    ValorTotal = 100
            //    }
            //};
            //nfse.Natureza = new Dominio.ObjetosDeValor.NFSe.Natureza()
            //{
            //    Descricao = "Natureza do Serviço",
            //    Numero = 1
            //};
            //nfse.OutrasInformacoes = "TESTE OUTRAS INFORMACOES";
            //nfse.Tomador = new Dominio.ObjetosDeValor.CTe.Cliente()
            //{
            //    Bairro = "Presidente Médici",
            //    CEP = "89801141",
            //    CodigoAtividade = 3,
            //    CodigoIBGECidade = 4204202,
            //    Complemento = "Teste",
            //    CPFCNPJ = "07211571900",
            //    Emails = "willian@multisoftware.com.br",
            //    EmailsContador = "willian@multisoftware.com.br",
            //    EmailsContato = "willian@multisofware.com.br",
            //    Endereco = "Rua 7 de Setembro",
            //    Exportacao = false,
            //    NomeFantasia = "Willian Bonho Daiprai",
            //    Numero = "1178",
            //    RazaoSocial = "Willian Bonho Daiprai",
            //    StatusEmails = true,
            //    StatusEmailsContador = true,
            //    StatusEmailsContato = true,
            //    Telefone1 = "49-9992-1865",
            //    Telefone2 = "49*3323*3902",
            //    IM = "",
            //    RGIE = ""
            //};
            //nfse.ValorISS = 1;
            //nfse.ValorServicos = 100;


            //WSEmpresa.EmpresaClient svcEmpresa = new WSEmpresa.EmpresaClient();

            //var retorno = svcEmpresa.ObterDetalhesEmpresa("13496023000180", "13969629000196", "D37AC5E6-BCA1-45A5-B4F8-FFAD7F4535BD");
            //retorno = svcEmpresa.ObterDetalhesEmpresa("13496023000180", "13969629000196", "D37AC5E6-BCA1-45A5-B4F8-FFAD7F4535BD");


            //var retorno = svcNFSe.IntegrarNFSe(nfse, "13969629000196", "D37AC5E6-BCA1-45A5-B4F8-FFAD7F4535BD");
            //var retornoExclusao = svcNFSe.ExcluirNFSe("13969629000196", 1036, "D37AC5E6-BCA1-45A5-B4F8-FFAD7F4535BD");


            //var retornoConsulta = svcConsulta.BuscarPorCodigoNFSe(retorno.Objeto, Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao, Dominio.Enumeradores.TipoRetornoIntegracao.Todos, "D37AC5E6-BCA1-45A5-B4F8-FFAD7F4535BD");

            //if (retornoConsulta.Status)
            //{

            //}


            //string token = "3d181798f84b43ff8c317ffa84ef2803";

            //System.IO.FileStream file = new FileStream("C:\\Temp\\NFe.xml", FileMode.Open);

            //SGT.WebService.NFe.Homolog.NFeClient svcNFe = new SGT.WebService.NFe.Homolog.NFeClient();
            //OperationContextScope scope = new OperationContextScope(svcNFe.InnerChannel);
            //MessageHeader header = MessageHeader.CreateHeader("Token", "Token", token);
            //OperationContext.Current.OutgoingMessageHeaders.Add(header);

            //var retorno = svcNFe.EnviarArquivoXMLNFe(file);

            //if (retorno.Status)
            //{

            //}

            //Marfrig homo
            //string token = "dca6555827fb4b9780a3798e023125f3";

            //System.IO.FileStream file = new FileStream("C:\\Temp\\teste.xml", FileMode.Open);

            //SGT.WebService.NFe.Homolog.NFeClient svcNFe = new SGT.WebService.NFe.Homolog.NFeClient();
            //OperationContextScope scope = new OperationContextScope(svcNFe.InnerChannel);
            //MessageHeader header = MessageHeader.CreateHeader("Token", "Token", token);
            //OperationContext.Current.OutgoingMessageHeaders.Add(header);

            //var retorno = svcNFe.EnviarArquivoXMLNFe(file);

            //if (retorno.Status)
            //{

            //}


            //string token = "b7690bad39c8478aa1980473999d9517";

            //SGT.WebService.Cargas.Homolog.CargasClient svcCargas = new SGT.WebService.Cargas.Homolog.CargasClient();
            //OperationContextScope scope = new OperationContextScope(svcCargas.InnerChannel);
            //MessageHeader header = MessageHeader.CreateHeader("Token", "Token", token);
            //OperationContext.Current.OutgoingMessageHeaders.Add(header);

            //Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos();
            //protocolo.protocoloIntegracaoCarga = 1898;
            //protocolo.protocoloIntegracaoPedido = 10601;

            //var retorno = svcCargas.BuscarCarga(protocolo);

            //if (retorno.Status)
            //{

            //}

            //SGT.WebService.Cargas.Homolog.CargasClient svcCarga = new SGT.WebService.Cargas.Homolog.CargasClient();


            //Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();

            //List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> listaProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();

            //Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
            //produto.CodigoGrupoProduto = "PREFRI";
            //produto.CodigoProduto = "03679";
            //produto.DescricaoGrupoProduto = "BATATA PRE-FRITA CONGELADA EMPACOTADA";
            //produto.DescricaoProduto = "BB CORTE CASEIRO 1,05KG (14PC X 1,05KG/CX 14,7KG)";
            //produto.PesoUnitario = 4020;
            //produto.Quantidade = 4025;
            //produto.ValorUnitario = 20;

            //listaProdutos.Add(produto);

            //cargaIntegracao.Produtos = listaProdutos;





            //byte[] pdf = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, Convert.FromBase64String("stringPDF"));
            //System.IO.File.WriteAllBytes("C:\\Temp\\testeNFSe.pdf", pdf);

            //Servicos.ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.ServicoMDFe.uMDFeServiceTSSoapClient();

            //Servicos.MDFe svcMDFeInterno = new Servicos.MDFe(Conexao.StringConexao);
            //Repositorio.Empresa repEmpresa = new Repositorio.Empresa(Conexao.StringConexao);

            //Servicos.ServicoMDFe.EventoEncerramentoManualMDFe evento = new Servicos.ServicoMDFe.EventoEncerramentoManualMDFe();
            //evento.Ambiente = Servicos.ServicoMDFe.TipoAmbiente.Homologacao;
            //evento.Chave = "42150413496023000180580010000001361000001365";
            //evento.CodigoMunicipioEncerramento = "3300308";
            //evento.CodigoUFEncerramento = "33";
            //evento.DataEncerramento = DateTime.Now.ToString("dd/MM/yyyy");
            //evento.DataEvento = DateTime.Now.ToString("dd/MM/yyyy");
            //evento.Empresa = svcMDFeInterno.ObterEmpresaEmitente(repEmpresa.BuscarPorCodigo(3));
            //evento.Protocolo = "942150000004622";

            //var retorno = svcMDFe.ImportarEventoEncerramentoManual(evento);

            //var reto = svcMDFe.ConsultarEventoEncerramentoManualMDFe(retorno.Valor);

            //Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica nfe = new Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica("56991441000823|233085985117|AVON COSMETICOS LTDA|RUA LAURO PINTO TOLEDO|410||PINHAL|3508405|CABREUVA|SP|13315971|1058|BRASIL||0|1|72710767|2015-05-26|35150556991441000823550010727107675261291830|121.15|3.320|1|0|ISENTO|MICHELE TAVARES DA SILVA|||RUA ORQUIDEA|00043|CASA|LOMBA DO PINHEIRO|4314902|PORTO ALEGRE|RS|91560220|1058|BRASIL||217215|312592|71883671|10|09|121,15|* S.T.P. *|546|D|D0000||||||||||||||||");

            //Servicos.Avon svcAvon = new Servicos.Avon();

            //svcAvon.GerarCTePorNFe(3, nfe,  null, new Repositorio.UnitOfWork(Conexao.StringConexao));

            //var asas = 11;
            //asas = 2;
            //asas++;

            //System.IO.FileStream file = new FileStream("E:\\00690000054362-ped-aut.txt", FileMode.Open);

            //Dominio.NDDigital.v104.Emissao.Arquivo arquivo = new Dominio.NDDigital.v104.Emissao.Arquivo(file);

            //Servicos.CTe svcCTe123 = new Servicos.CTe(unitOfWork);
            //svcCTe123.GerarCTePorTxt(arquivo.CTes[0], 3, new Repositorio.UnitOfWork(Conexao.StringConexao));

            //file.Dispose();

            //Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica nfe = new Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica("56991441000823|233085985117|AVON COSMETICOS LTDA|RUA LAURO PINTO TOLEDO|410||PINHAL|3508405|CABREUVA|SP|13315971|1058|BRASIL||0|1|329005103|2014-09-26|35140956991441000823550013290051031261291370|438.78|10007.649|1|0|ISENTO|NAIR BATISTA FERREIRA|||R AGUAPEI|00182||CAMPESTRE|3547809|STO ANDRE|SP|09070090|1058|BRASIL||113043|323562|21437516|14|15|492,29|E.LIBERADA|014|2|20210||||||||||||||||");

            //var x = 0;
            //WSImpressaoCTe.ImpressaoCTeClient svcImpressao = new WSImpressaoCTe.ImpressaoCTeClient();

            //var impressoes = svcImpressao.ObterImpressoesPendentes(1, 1);

            //var x = impressoes.Objeto;

            //ServiceReference4.IntegracaoCTeClient svcIntegracao = new ServiceReference4.IntegracaoCTeClient();

            //svcIntegracao.Buscar(Dominio.Enumeradores.StatusIntegracao.Integrado, 3, 0, 0, Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF);

            //WSImpressaoAPTI.ImpressaoCTeClient svcImpressaoCTe = new WSImpressaoAPTI.ImpressaoCTeClient();

            //WSImpressaoAPTI.RetornoOfArrayOfRetornoImpressaolQrtB7Zh impressoes = svcImpressaoCTe.ObterImpressoesPendentes(1, 1);

            //foreach (var impressao in impressoes.Objeto)
            //{

            //byte[] pdf = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding("ISO-8859-1"), Convert.FromBase64String("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GcO4w43CocOOw6NCwpvDlMKaw53DpcKyw4VTTQIrw5jCtjjDtcOwHXI7wopdDMKHI2d7wprCgWHCgGzCjFXCnm4Uw7PClhxOCE8JGnsswqgMw7zDplDDp3HCoU1qw43DrnLDmcOiwqkmwoEVbFvCnHrDuA7CuR3DhS7ChsODwpHCsz3DjcOAMEA2w4YqTzfCinlLDifChMKnBMKNPRZUBn5zwqjDs8K4w5AmwrVmd8K5bMOxVMKTw4AKwrYtTj18wofDnMKOYhfDg8Ohw4jDmcKeZsOgw7/Ck38Aw6fDpy9KDQplbmRzdHJlYW0NCmVuZG9iag0KNCAwIG9iag0KPDwgL1R5cGUgL0ZvbnQgL1N1YnR5cGUgL1R5cGUxIC9CYXNlRm9udCAvSGVsdmV0aWNhIC9FbmNvZGluZyAvV2luQW5zaUVuY29kaW5nID4+DQplbmRvYmoNCjYgMCBvYmoNCjw8IC9UeXBlIC9Gb250IC9TdWJ0eXBlIC9UeXBlMSAvQmFzZUZvbnQgL0hlbHZldGljYS1Cb2xkIC9FbmNvZGluZyAvV2luQW5zaUVuY29kaW5nID4+DQplbmRvYmoNCjkgMCBvYmoNCjw8IC9UeXBlIC9QYWdlcyAvS2lkcyBbIDIgMCBSIF0gL0NvdW50IDEgPj4NCmVuZG9iag0KMTAgMCBvYmoNCjw8IC9UeXBlIC9DYXRhbG9nIC9QYWdlcyA5IDAgUiA+Pg0KZW5kb2JqDQoxMSAwIG9iag0KPDwgL1RpdGxlIDxmZWZmMDA0NDAwNDEwMDQzMDA1NDAwNDUwMDUyMDA2NTAwNzQwMDcyMDA2MTAwNzQwMDZmPg0KL0F1dGhvciA8Pg0KL1N1YmplY3QgPD4NCi9DcmVhdG9yIChNaWNyb3NvZnQgUmVwb3J0aW5nIFNlcnZpY2VzIDEyLjAuMC4wKQ0KL1Byb2R1Y2VyIChNaWNyb3NvZnQgUmVwb3J0aW5nIFNlcnZpY2VzIFBERiBSZW5kZXJpbmcgRXh0ZW5zaW9uIDEyLjAuMC4wKQ0KL0NyZWF0aW9uRGF0ZSAoRDoyMDE1MDgyMDExNDUxNi0wMycwMCcpDQo+Pg0KZW5kb2JqDQp4cmVmDQowIDEyDQowMDAwMDAwMDAwIDY1NTM1IGYNCjAwMDAwMDAwMTAgMDAwMDAgbg0KMDAwMDAwNzk4MiAwMDAwMCBuDQowMDAwMDA4MTkxIDAwMDAwIG4NCjAwMDAwMTc5NDkgMDAwMDAgbg0KMDAwMDAxNTg5MiAwMDAwMCBuDQowMDAwMDE4MDQ5IDAwMDAwIG4NCjAwMDAwMTYwOTMgMDAwMDAgbg0KMDAwMDAwMDA2NSAwMDAwMCBuDQowMDAwMDE4MTU0IDAwMDAwIG4NCjAwMDAwMTgyMTYgMDAwMDAgbg0KMDAwMDAxODI2OSAwMDAwMCBuDQp0cmFpbGVyIDw8IC9TaXplIDEyIC9Sb290IDEwIDAgUiAvSW5mbyAxMSAwIFIgPj4NCnN0YXJ0eHJlZg0KMTg1NDgNCiUlRU9G"));
            //System.IO.File.WriteAllBytes("C:\\TesteImpressaoAPTI\\1943.pdf", pdf);


            //}

            //WSImpressaoCTe.ImpressaoCTeClient svcImpressaoCTe = new WSImpressaoCTe.ImpressaoCTeClient();

            //var retorno = svcImpressaoCTe.SolicitarImpressao(new int[] { 90334, 90335, 90336 }, "13969629000196");

            //var x = retorno.Mensagem;

            //var retorno1 = svcImpressaoCTe.ObterImpressoesPendentes(1, 90);



            //for (var i = 0; i < 100; i++)
            //{
            //    WSEmpresa.EmpresaClient svcEmpresa = new WSEmpresa.EmpresaClient();

            //    Dominio.ObjetosDeValor.Empresa empresa = new Dominio.ObjetosDeValor.Empresa()
            //    {
            //        CNPJ = "30.171.285/0001-94",
            //        ANTT = "12345678",
            //        NomeFantasia = "Teste",
            //        RazaoSocial = "Teste",
            //        Bairro = "Teste",
            //        CEP = "99999999",
            //        CodigoIBGECidade = 4204202,
            //        Endereco = "rua teste",
            //        Numero = "13212 xxx",
            //        Telefone = "49 3323 3902",
            //        InscricaoEstadual = "123456789"
            //    };

            //    WSEmpresa.RetornoOfstring retorno = svcEmpresa.SalvarEmpresaEmissora(empresa, "13969629000196");

            //    //svcEmpresa.Close();
            //    //svcEmpresa = null;
            //}


            //ServiceReference3.ConhecimentoDeTransporteEletronicoClient svcCTe = new ServiceReference3.ConhecimentoDeTransporteEletronicoClient();



            //foreach (var obj in retorno1.Objeto)
            //{
            //    var retornoImpressao = svcImpressaoCTe.Alterar(obj.CodigoImpressao, Dominio.Enumeradores.StatusImpressaoCTe.Impresso);
            //}

            //WSMDFe.ManifestoEletronicoDeDocumentosFiscaisClient svcMDFe = new WSMDFe.ManifestoEletronicoDeDocumentosFiscaisClient();

            //svcMDFe.IntegrarMDFePorCTes(new int[] { 87394, 87395 }, "13496023000180", "13969629000196");

            //System.IO.FileStream file = new FileStream("E:\\42140589637490013204550010001667261052393426-nfe.XML", FileMode.Open);

            //var ndfe = MultiSoftware.NFe.Servicos.Leitura.LerNFeProcessada(file);

            //Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            //svcCTe.GerarCTePorNFe(ndfe, 3);


            //MDFe.ManifestoEletronicoDeDocumentosFiscaisClient svcMDFe = new MDFe.ManifestoEletronicoDeDocumentosFiscaisClient();

            //Dominio.ObjetosDeValor.MDFe.MDFe mdfe = new Dominio.ObjetosDeValor.MDFe.MDFe();

            //mdfe.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            //mdfe.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
            //{
            //    CNPJ = "13496023000180",
            //    Atualizar = false
            //};

            //mdfe.UFCarregamento = "SC";

            //mdfe.UFDescarregamento = "SC";

            //mdfe.MunicipiosDeCarregamento = new List<Dominio.ObjetosDeValor.MDFe.MunicipioCarregamento>()
            //{
            //    new Dominio.ObjetosDeValor.MDFe.MunicipioCarregamento(){
            //         CodigoIBGE = 4204707
            //    }
            //};

            //mdfe.MunicipiosDeDescarregamento = new List<Dominio.ObjetosDeValor.MDFe.MunicipioDescarregamento>()
            //{
            //    new Dominio.ObjetosDeValor.MDFe.MunicipioDescarregamento(){
            //         CodigoIBGE = 4209102,
            //         Documentos = new List<Dominio.ObjetosDeValor.MDFe.DocumentoMunicipioDescarregamento>(){
            //             new Dominio.ObjetosDeValor.MDFe.DocumentoMunicipioDescarregamento(){
            //                  ChaveCTe = "42140613496023000180570090000001681000001680"
            //             },
            //             new Dominio.ObjetosDeValor.MDFe.DocumentoMunicipioDescarregamento(){
            //                  ChaveCTe = "42140613496023000180570090000001671000001674"
            //             }
            //         }
            //    }
            //};

            //mdfe.Veiculo = new Dominio.ObjetosDeValor.CTe.Veiculo()
            //{
            //    Placa = "ASD1234",
            //    Renavam = "123456789",
            //    Tara = 1000,
            //    TipoCarroceria = "00",
            //    TipoPropriedade = "P",
            //    TipoRodado = "00",
            //    TipoVeiculo = "0",
            //    UF = "SC"
            //};

            //mdfe.Motoristas = new List<Dominio.ObjetosDeValor.CTe.Motorista>()
            //{
            //    new Dominio.ObjetosDeValor.CTe.Motorista(){
            //         CPF = "07211571900",
            //         Nome = "Willian"
            //    }
            //};

            //mdfe.NumeroCarga = 1122;

            //mdfe.UnidadeMedidaMercadoria = Dominio.Enumeradores.UnidadeMedidaMDFe.KG;

            //mdfe.ValorTotalMercadoria = 1;

            //mdfe.PesoBrutoMercadoria = 1000;

            //var retorno = svcMDFe.IntegrarMDFePorObjeto(mdfe, "13969629000196");

            //IntegracaoMDFe.IntegracaoMDFeClient svcIntegracao = new IntegracaoMDFe.IntegracaoMDFeClient();

            //var retornoConsulta = svcIntegracao.BuscarPorCodigoMDFe(retorno.Objeto, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao, Dominio.Enumeradores.TipoRetornoIntegracao.XML);



            //E:\0000500479050601N.EMS

            //Repositorio.Empresa repEmpresa = new Repositorio.Empresa(Conexao.StringConexao);
            //var empresa = repEmpresa.BuscarPorCodigo(3);

            //System.IO.FileStream file = new FileStream("E:\\0000500479050601N.EMS", FileMode.Open);

            //Servicos.LeituraEDI svcLeitura = new Servicos.LeituraEDI(empresa, (from obj in empresa.LayoutsEDI where obj.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS select obj).FirstOrDefault(), file);

            //svcLeitura.GerarCTes();

            //file.Dispose();

            //ServiceReference4.IntegracaoCTeClient svcIntegracao = new ServiceReference4.IntegracaoCTeClient();

            //ServiceReference4.RetornoOfRetornoCTelQrtB7Zh integracao = svcIntegracao.BuscarPorNumeroDaNota("13496023000180", "515148", "1", Dominio.Enumeradores.TipoIntegracao.Emissao, Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF);

            //WSAptiIntegracao.IntegracaoCTeClient wsApti = new WSAptiIntegracao.IntegracaoCTeClient();
            //var retorno = wsApti.BuscarPorNumeroDaNota("02657396000170", "296683", "3", Dominio.Enumeradores.TipoIntegracao.Emissao, Dominio.Enumeradores.TipoRetornoIntegracao.XML);

            //WSApti.ConhecimentoDeTransporteEletronicoClient svcCTe = new WSApti.ConhecimentoDeTransporteEletronicoClient();

            //WSImpressaoCTe.ImpressaoCTeClient svcImpressao = new WSImpressaoCTe.ImpressaoCTeClient();

            //int count = 0;

            //do
            //{
            //    var impressoes = svcImpressao.ObterImpressoesPendentes(1, 33);

            //    count = impressoes.Objeto.Count();

            //    foreach (var impressao in impressoes.Objeto)
            //    {
            //        svcImpressao.Alterar(impressao.CodigoImpressao, Dominio.Enumeradores.StatusImpressaoCTe.Impresso);
            //    }
            //} while (count > 0);


            //ServiceReference3.ConhecimentoDeTransporteEletronicoClient svcCTe = new ServiceReference3.ConhecimentoDeTransporteEletronicoClient();

            //List<int> ctes = new List<int>();

            ////for (var i = 0; i < 50; i++)
            ////{
            //Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();

            //cte.CFOP = 5352;
            //cte.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //cte.CodigoIBGECidadeInicioPrestacao = 4204202;
            //cte.CodigoIBGECidadeTerminoPrestacao = 4204608;

            //cte.Destinatario = new Dominio.ObjetosDeValor.CTe.Cliente()
            //{
            //    Bairro = "Presidente Médici",
            //    CEP = "89801141",
            //    CodigoAtividade = 3,
            //    CodigoIBGECidade = 4204202,
            //    Complemento = "Teste",
            //    CPFCNPJ = "07211571900",
            //    Emails = "willian@multisoftware.com.br",
            //    EmailsContador = "willian@multisoftware.com.br",
            //    EmailsContato = "willian@multisofware.com.br",
            //    Endereco = "Rua 7 de Setembro",
            //    Exportacao = false,
            //    NomeFantasia = "Willian Bonho Daiprai",
            //    Numero = "1178",
            //    RazaoSocial = "Willian Bonho Daiprai",
            //    StatusEmails = true,
            //    StatusEmailsContador = true,
            //    StatusEmailsContato = true,
            //    Telefone1 = "49-9992-1865",
            //    Telefone2 = "49*3323*3902",
            //    RGIE = ""
            //};

            //cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento> { 
            //    new  Dominio.ObjetosDeValor.CTe.Documento(){ ChaveNFE = "42140406368045000108550010000594531001223251", DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ModeloDocumentoFiscal = "55", Tipo = Dominio.Enumeradores.TipoDocumentoCTe.NFe, Numero = "123"  }
            //};

            //cte.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
            //{
            //    Atualizar = false,
            //    CNPJ = "13496023000180"
            //};

            //cte.Expedidor = new Dominio.ObjetosDeValor.CTe.Cliente()
            //{
            //    Bairro = "Presidente Médici",
            //    CEP = "89801141",
            //    CodigoAtividade = 3,
            //    CodigoIBGECidade = 4204202,
            //    Complemento = "Teste",
            //    CPFCNPJ = "07211571900",
            //    Emails = "willian@multisoftware.com.br",
            //    EmailsContador = "willian@multisoftware.com.br",
            //    EmailsContato = "willian@multisofware.com.br",
            //    Endereco = "Rua 7 de Setembro",
            //    Exportacao = false,
            //    NomeFantasia = "Willian Bonho Daiprai",
            //    Numero = "1178",
            //    RazaoSocial = "Willian Bonho Daiprai",
            //    StatusEmails = true,
            //    StatusEmailsContador = true,
            //    StatusEmailsContato = true,
            //    Telefone1 = "49-9992-1865",
            //    Telefone2 = "49*3323*3902",
            //    RGIE = ""
            //};

            //cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Nao;
            //cte.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Nao;
            //cte.NumeroCarga = 33;
            //cte.NumeroUnidade = 33;
            //cte.ProdutoPredominante = "Diversos";

            //cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga>{
            //    new  Dominio.ObjetosDeValor.CTe.QuantidadeCarga(){
            //         Descricao = "Kilogramas",
            //         Quantidade =  10000,
            //         UnidadeMedida = "01"
            //    }
            //};

            //cte.Recebedor = new Dominio.ObjetosDeValor.CTe.Cliente()
            //{
            //    Bairro = "Presidente Médici",
            //    CEP = "89801141",
            //    CodigoAtividade = 3,
            //    CodigoIBGECidade = 4204202,
            //    Complemento = "Teste",
            //    CPFCNPJ = "07211571900",
            //    Emails = "willian@multisoftware.com.br",
            //    EmailsContador = "willian@multisoftware.com.br",
            //    EmailsContato = "willian@multisofware.com.br",
            //    Endereco = "Rua 7 de Setembro",
            //    Exportacao = false,
            //    NomeFantasia = "Willian Bonho Daiprai",
            //    Numero = "1178",
            //    RazaoSocial = "Willian Bonho Daiprai",
            //    StatusEmails = true,
            //    StatusEmailsContador = true,
            //    StatusEmailsContato = true,
            //    Telefone1 = "49-9992-1865",
            //    Telefone2 = "49*3323*3902",
            //    RGIE = ""
            //};

            //cte.Remetente = new Dominio.ObjetosDeValor.CTe.Cliente()
            //{
            //    Bairro = "Presidente Médici",
            //    CEP = "89801141",
            //    CodigoAtividade = 3,
            //    CodigoIBGECidade = 4204202,
            //    Complemento = "Teste",
            //    CPFCNPJ = "07211571900",
            //    Emails = "willian@multisoftware.com.br",
            //    EmailsContador = "willian@multisoftware.com.br",
            //    EmailsContato = "willian@multisoftware.com.br",
            //    Endereco = "Rua 7 de Setembro",
            //    Exportacao = false,
            //    NomeFantasia = "Willian Bonho Daiprai",
            //    Numero = "1178",
            //    RazaoSocial = "Willian Bonho Daiprai",
            //    StatusEmails = true,
            //    StatusEmailsContador = true,
            //    StatusEmailsContato = true,
            //    Telefone1 = "49-9992-1865",
            //    Telefone2 = "49*3323*3902",
            //    RGIE = ""
            //};

            //cte.Retira = Dominio.Enumeradores.OpcaoSimNao.Nao;

            //cte.Seguros = new List<Dominio.ObjetosDeValor.CTe.Seguro> {
            //    new  Dominio.ObjetosDeValor.CTe.Seguro() { NomeSeguradora = "999999999999999999999999999999", NumeroApolice = null, NumeroAverbacao = "12312312312312312312", Tipo = Dominio.Enumeradores.TipoSeguro.Remetente, Valor = 0m }
            //};

            //cte.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Sim;
            ////cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
            //cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
            //cte.TipoImpressao = Dominio.Enumeradores.TipoImpressao.Paisagem;
            //cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            //cte.TipoServico = Dominio.Enumeradores.TipoServico.Normal;
            //cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
            //cte.ValorFrete = 1500m;
            //cte.ValorTotalPrestacaoServico = 1500m;
            //cte.ValorAReceber = 1500m;
            //cte.ValorTotalMercadoria = 15000m;

            //cte.Veiculos = new List<Dominio.ObjetosDeValor.CTe.Veiculo>();

            //cte.Veiculos.Add(new Dominio.ObjetosDeValor.CTe.Veiculo()
            //{
            //    //CapacidadeKG = 1000,
            //    //CapacidadeM3 = 2,
            //    Placa = "QQQ1111",
            //    //Renavam = "123456789",
            //    //Tara = 1000,
            //    //TipoCarroceria = "00",
            //    //TipoPropriedade = "P",
            //    //TipoRodado = "00",
            //    //TipoVeiculo = "0",
            //    //UF = "SC"
            //});

            //cte.Veiculos.Add(new Dominio.ObjetosDeValor.CTe.Veiculo()
            //{
            //    //CapacidadeKG = 10000,
            //    //CapacidadeM3 = 5,
            //    Placa = "MMM1111",
            //    //Renavam = "1234567891",
            //    //Tara = 9000,
            //    //TipoCarroceria = "00",
            //    //TipoPropriedade = "P",
            //    //TipoRodado = "00",
            //    //TipoVeiculo = "0",
            //    //UF = "SC"
            //});

            //cte.Motoristas = new List<Dominio.ObjetosDeValor.CTe.Motorista>();

            //cte.Motoristas.Add(new Dominio.ObjetosDeValor.CTe.Motorista()
            //{
            //    CPF = "66666666666",
            //    Nome = "João ZZZZZ"
            //});

            //ServiceReference3.RetornoOfint retorno = svcCTe.IntegrarCTe(cte, "13969629000196");  //svcCTe.AlterarCTe(87393, cte, "13969629000196");

            //if (retorno.Status)
            //    ctes.Add(retorno.Objeto);

            //}


            //svcImpressao.SolicitarImpressao(ctes.ToArray(), "13969629000196");

            //System.IO.FileStream file = new FileStream("E:\\42140380972078000107550300005404911861067963.XML", FileMode.Open);

            //var cte = MultiSoftware.CTe.Servicos.Leitura.Ler(file);

            ////file.Dispose();

            //ServiceReference1.ConhecimentoDeTransporteEletronicoClient svc = new ServiceReference1.ConhecimentoDeTransporteEletronicoClient();

            //var retorno = svc.EnviarXMLNFeParaIntegracao(file);

            //var retorno2 = svc.IntegrarCTePorXMLNFe(retorno.Objeto, 1, System.IO.Path.GetFileName(file.Name));

            //svc.Close();
            //file.Dispose();


            //Dominio.NDDigital.v104.Emissao.Arquivo arquivo = new Dominio.NDDigital.v104.Emissao.Arquivo(file);

            //file.Dispose();

            //decimal xxxx = 0;
            //decimal.TryParse("1.000,00", out xxxx);

            //Servicos.Email svcEmail = new Servicos.Email(unitOfWork);
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.Append("<p>Seguem os dados para acesso ao sistema de emissão de CT-e:<br /><br />");
            //sb.Append("Usuário: ").Append("Teste").Append("<br />");
            //sb.Append("Senha: ").Append("123").Append("</p><br />");
            //sb.Append("Para utilizar o sistema de emissão de CT-e acesse a página <a href='http://www.multicte.com.br/'>www.multicte.com.br</a> e utilize a opção de Login.");
            //System.Text.StringBuilder ss = new System.Text.StringBuilder();
            //ss.Append("MultiSoftware - <a href='http://www.multicte.com.br/'>www.multicte.com.br</a> <br />");
            //ss.Append("Fone/Fax: (49)3311-8177 / (49)3328-5516 <br />");
            //ss.Append("Cel.: (49)9999-8880 <br />");
            //ss.Append("E-mail: cte@multisoftware.com.br / cesar@multisoftware.com.br / infra@multisoftware.com.br");
            //svcEmail.EnviarEmail("cte@multisoftware.com.br", "cte@multisoftware.com.br", "Multi@2013", "cesar@multisoftware.com.br", "", "", "MultiCTe - Dados para Acesso ao Sistema", sb.ToString(), "pod51028.outlook.com", null, ss.ToString(), true, "cte@multisoftware.com.br");



            ////ServiceReference1.ConhecimentoDeTransporteEletronicoClient svcCTe = new ServiceReference1.ConhecimentoDeTransporteEletronicoClient();
            //var x = svcCTe.BuscarLoteDeXMLPorEmpresaEPeriodo("01/05/2013", "01/06/2013", "13496023000180", "+73o22sAiX7pnkv/ov/Fu/RtNyJyfuyTpfRUP22irSU=");
            //var stringxml = "";

            //using (XmlReader xr = XmlReader.Create("C:/nfe.xml", new XmlReaderSettings() { IgnoreWhitespace = true }))
            //{
            //    using (StringWriter sw = new StringWriter())
            //    {
            //        using (XmlWriter xw = XmlWriter.Create(sw))
            //        {
            //            xw.WriteNode(xr, false);
            //        }
            //        stringxml = sw.ToString();
            //    }
            //}

            //ServiceReference3.ConhecimentoDeTransporteEletronicoClient svcCTe = new ServiceReference3.ConhecimentoDeTransporteEletronicoClient();
            //var retorno = svcCTe.IntegrarCTe(stringxml, 1);

            //ServiceReference2.EmpresaClient svcEmpresa = new ServiceReference2.EmpresaClient();

            //var aa = svcEmpresa.BuscarEmpresaAdministradora("zEOMy5FmnPAjEKh5GGA8lQ+EIB+HkfPnEDhTNQDHFrk=");
            //var x = svcEmpresa.BuscarEmpresaPai("13496023000180", "zEOMy5FmnPAjEKh5GGA8lQ+EIB+HkfPnEDhTNQDHFrk=");
            //var y = svcEmpresa.AlterarStatusEmpresaPai("13496023000180", "zEOMy5FmnPAjEKh5GGA8lQ+EIB+HkfPnEDhTNQDHFrk=", false);

            //var empresa = new ServiceReference2.Empresa();
            //empresa.ANTT = "12345678";
            //empresa.Bairro = "XXXXXXXXX";
            //empresa.CEP = "89.801-141";
            //empresa.CNAE = "";
            //empresa.CNPJ = "44.123.593/0001-09";
            //empresa.CodigoIBGECidade = 4204202;
            //empresa.Complemento = "";
            //empresa.Contato = "Willian";
            //empresa.DataEmissaoANTT = "25/12/2012";
            //empresa.DataFinalCertificado = "";
            //empresa.DataInicialCertificado = "";
            //empresa.DataValidadeANTT = "01/01/2020";
            //empresa.Email = "willianbdaiprai@gmail.com";
            //empresa.Endereco = "XXXXXXXXXXXXXXX";
            //empresa.InscricaoEstadual = "123456789";
            //empresa.NomeFantasia = "TESTE INTEGRACAO";
            //empresa.Numero = "123";
            //empresa.RazaoSocial = "TESTE INTEGRACAO";
            //empresa.Responsavel = "Willian";
            //empresa.Telefone = "4933233333";

            //var ret = svcEmpresa.SalvarEmpresaEmissora(empresa, "13969629000196", "+73o22sAiX7pnkv/ov/Fu/RtNyJyfuyTpfRUP22irSU=");
            //var ret = svcEmpresa.SalvarEmpresaPai(empresa, "zEOMy5FmnPAjEKh5GGA8lQ+EIB+HkfPnEDhTNQDHFrk=");

            //var cred = svcEmpresa.BuscarCredencial("44.825.593/0001-09", "13496023000180", "zEOMy5FmnPAjEKh5GGA8lQ+EIB+HkfPnEDhTNQDHFrk=");
        }

        private void GravarLog(string mensagem, string prefixo = "")
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                string arquivo = (string.IsNullOrWhiteSpace(prefixo) ? "" : prefixo + "-") + dateTime.Day + "-" + dateTime.Month + "-" + dateTime.Year + ".txt";
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                string file = System.IO.Path.Combine(path, arquivo);

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                System.IO.StreamWriter strw = new System.IO.StreamWriter(file, true);
                try
                {
                    strw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " " + mensagem);
                    strw.WriteLine();
                }
                catch
                {
                }
                finally
                {
                    strw.Close();
                }
            }
            catch
            {
            }
        }
    }
}