using System;

namespace Servicos.Embarcador.Integracao.MultiEmbarcador
{
    public class Canhoto
    {
        public static void IntegracarCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            canhotoIntegracao.DataIntegracao = DateTime.Now;
            canhotoIntegracao.NumeroTentativas++;

            try
            {
                InspectorBehavior inspector = new InspectorBehavior();

                ServicoSGT.NFe.NFeClient serNFeClient = ObterClientNFe(canhotoIntegracao.Canhoto.Emitente.GrupoPessoas.URLIntegracaoMultiEmbarcador, canhotoIntegracao.Canhoto.Emitente.GrupoPessoas.TokenIntegracaoMultiEmbarcador);
                
                serNFeClient.Endpoint.EndpointBehaviors.Add(inspector);

                Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoWS = new Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto();

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = canhotoIntegracao.Canhoto;

                canhotoWS.ChaveAcesso = canhoto.XMLNotaFiscal?.Chave ?? "";
                canhotoWS.DataEnvioCanhoto = canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy HH:mm:ss");
                canhotoWS.Latitude = canhoto.Latitude;
                canhotoWS.Longitude = canhoto.Longitude;
                canhotoWS.NomeImagemCanhoto = canhoto.NomeArquivo;
                //canhotoWS.Protocolo = canhoto.Codigo;
                canhotoWS.TipoCanhoto = canhoto.TipoCanhoto;
                canhotoWS.SituacaoCanhoto = canhoto.SituacaoCanhoto;
                canhotoWS.SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto;

                string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);
                string caminhoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.NomeArquivo);
                byte[] bufferCanhoto = null;
                if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                {
                    bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);
                    canhotoWS.ImagemCanhotoBase64 = Convert.ToBase64String(bufferCanhoto);

                    Servicos.ServicoSGT.NFe.RetornoOfboolean retorno = serNFeClient.EnviarDigitalizacaoCanhoto(canhotoWS);

                    if (retorno.Status)
                    {
                        canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        canhotoIntegracao.ProblemaIntegracao = "Integrado com sucesso.";
                    }
                    else
                    {
                        canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        canhotoIntegracao.ProblemaIntegracao = retorno.Mensagem;
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = canhotoIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = canhotoIntegracao.ProblemaIntegracao ?? "";
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                    canhotoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                else
                {

                    canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    canhotoIntegracao.ProblemaIntegracao = "Não foi localizado o arquivo da imagem para envio.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar";
            }
            repCanhotoIntegracao.Atualizar(canhotoIntegracao);
        }

        public static ServicoSGT.NFe.NFeClient ObterClientNFe(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "NFe.svc";

            ServicoSGT.NFe.NFeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.NFe.NFeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.NFe.NFeClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        //public static void IntegracarCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(unitOfWork);
        //    Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
        //    Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

        //    Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

        //    canhotoIntegracao.DataIntegracao = DateTime.Now;
        //    canhotoIntegracao.NumeroTentativas++;

        //    if (integracao == null && string.IsNullOrWhiteSpace(integracao.APIKeyPiracanjuba))
        //    {
        //        canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
        //        canhotoIntegracao.ProblemaIntegracao = "Não está configurada a integração com a Piracanjuba";

        //    }
        //    else
        //    {
        //        Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.CanhotoNF canhotoNF = new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.CanhotoNF();

        //        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = canhotoIntegracao.Canhoto;

        //        List<string> chavesNF = new List<string>();

        //        if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso)
        //        {
        //            var chavesCanhotoAvulso = (from pedidosXMLNotasFiscais in canhoto.CanhotoAvulso.PedidosXMLNotasFiscais select pedidosXMLNotasFiscais.XMLNotaFiscal.Chave).ToList();
        //            chavesNF.AddRange(chavesCanhotoAvulso);
        //        }
        //        else
        //            chavesNF.Add(canhoto?.XMLNotaFiscal?.Chave);

        //        if (integracao.AmbienteProducaoPiracanjuba)
        //            canhotoNF.instance = "prd";
        //        else
        //            canhotoNF.instance = "hml";

        //        //canhotoNF.chaveNF = canhoto?.XMLNotaFiscal?.Chave;
        //        canhotoNF.dataEntrega = "";
        //        canhotoNF.numCarga = canhoto.Carga?.CodigoCargaEmbarcador?.PadLeft(10, '0') ?? string.Empty;
        //        canhotoNF.dataRegistro = canhoto.DataEnvioCanhoto.ToString("yyyy-MM-dd HH:mm:ss");
        //        canhotoNF.dataEntrega = "";
        //        canhotoNF.latitude = "";
        //        canhotoNF.longitude = "";
        //        canhotoNF.recebidoPor = "";
        //        canhotoNF.documentoRecebedor = "";

        //        string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
        //        string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto);
        //        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);
        //        string caminhoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.NomeArquivo);

        //        byte[] bufferCanhoto = null;

        //        if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
        //        {
        //            canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
        //            canhotoIntegracao.ProblemaIntegracao = "Não foi localizado o arquivo da imagem para envio.";
        //        }
        //        else
        //        {
        //            bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);
        //            canhotoNF.canhoto = Convert.ToBase64String(bufferCanhoto);

        //            if (extensao == ".jpg")
        //                canhotoNF.canhotoMimeType = "image/jpeg";
        //            else if (extensao == "pdf")
        //                canhotoNF.canhotoMimeType = "application/pdf";
        //            else
        //                canhotoNF.canhotoMimeType = "image/" + extensao.Replace(".", "");

        //            try
        //            {
        //                foreach (string chaveNF in chavesNF)
        //                {
        //                    canhotoNF.chaveNF = chaveNF;
        //                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Canhoto));
        //                    client.BaseAddress = new Uri(integracao.URLIntegracaoCanhotoPiracanjuba);
        //                    client.DefaultRequestHeaders.Accept.Clear();
        //                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                    client.DefaultRequestHeaders.Add("x-api-key", integracao.APIKeyPiracanjuba);

        //                    string jsonRequest = JsonConvert.SerializeObject(canhotoNF, Formatting.Indented);
        //                    var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

        //                    var result = client.PostAsync(integracao.URLIntegracaoCanhotoPiracanjuba, content).Result;

        //                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
        //                    {
        //                        string jsonResponse = result.Content.ReadAsStringAsync().Result;

        //                        Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.RetornoCanhoto retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.RetornoCanhoto>(result.Content.ReadAsStringAsync().Result);

        //                        if (retorno.success)
        //                            canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
        //                        else
        //                            canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

        //                        canhotoIntegracao.ProblemaIntegracao = retorno.msg;

        //                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
        //                        arquivoIntegracao.Data = canhotoIntegracao.DataIntegracao;
        //                        arquivoIntegracao.Mensagem = canhotoIntegracao.ProblemaIntegracao ?? "";
        //                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

        //                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
        //                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

        //                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

        //                        canhotoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        //                    }
        //                    else
        //                    {
        //                        canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
        //                        canhotoIntegracao.ProblemaIntegracao = "Falha ao conectar no WS Piracanjuba";
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Servicos.Log.TratarErro(ex);
        //                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
        //                canhotoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar";
        //            }
        //        }
        //    }

        //    repCanhotoIntegracao.Atualizar(canhotoIntegracao);
        //}
    }
}
