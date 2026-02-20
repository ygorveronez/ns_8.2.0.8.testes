using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Servicos.Global
{
    /// <summary>
    /// Serviço de OCR que usa a Engine versão 5 (versão com mais acertividade na leitura, ideal para cupons)
    /// (Implementação da V. 5 para arquivos PDF pendente. Para arquivos PDF, usar Servicos.Embarcador.Canhotos.LeitorOCR)
    /// </summary>
    public class ServicoOCR
    {
        readonly private  string _apiURI;
        readonly private  string _apiKey;

        public ServicoOCR(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente respositorioConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);
            var configuracaoAmbiente = respositorioConfiguracaoAmbiente.BuscarConfiguracaoPadrao();
            _apiURI = configuracaoAmbiente.APIOCRLink;
            _apiKey = configuracaoAmbiente.APIOCRKey;

        }

        public RespostaOCR ExecutarServico(byte[] imageData, bool isTable = false, bool tipoPdf = false)
        {
            RespostaOCR retornoOCR = new RespostaOCR();
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(ServicoOCR));
            requisicao.Timeout = new TimeSpan(0, 0, 300);
            try
            {
                var conteudoRequisicao = new MultipartFormDataContent
                {
                    { new StringContent(_apiKey), "apikey" },
                    { new StringContent("por"), "language" },
                    { new StringContent("true"), "detectOrientation" },
                    { new StringContent("true"), "scale" }, //Não alterar!
                    { new StringContent(isTable.ToString().ToLowerInvariant()), "isTable" }
                };

                if (!tipoPdf)
                {
                    conteudoRequisicao.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");
                    conteudoRequisicao.Add(new StringContent("5"), "OCREngine");
                }
                else
                    conteudoRequisicao.Add(new ByteArrayContent(imageData), "pdf", "document.pdf");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_apiURI, conteudoRequisicao).Result;
                byte[] byteRetornoRequisicao = retornoRequisicao.Content.ReadAsByteArrayAsync().Result;
                string conteudoRetornoRequisicao = Encoding.UTF8.GetString(byteRetornoRequisicao);

                retornoOCR = JsonConvert.DeserializeObject<RespostaOCR>(conteudoRetornoRequisicao);

                return retornoOCR;
            }
            catch (HttpRequestException ex1)
            {
                retornoOCR.OCRExitCode = 0;
                if (!retornoOCR.ErrorMessage.Any())
                    retornoOCR.ErrorMessage.Add("Erro ao enviar a requisição para o serviço OCR.");

                Log.TratarErro(ex1);
                return retornoOCR;
            }
            catch (TimeoutException ex2)
            {
                retornoOCR.OCRExitCode = 0;
                if (!retornoOCR.ErrorMessage.Any())
                    retornoOCR.ErrorMessage.Add("O serviço OCR não está disponível no momento. Tente novamente mais tarde.");

                Log.TratarErro(ex2);
                return retornoOCR;
            }
            catch (Exception ex3)
            {
                retornoOCR.OCRExitCode = 0;
                if (!retornoOCR.ErrorMessage.Any())
                    retornoOCR.ErrorMessage.Add(ex3.Message);

                Log.TratarErro(ex3);
                return retornoOCR;
            }
            finally
            {
                requisicao.Dispose();
            }

        }

    }

}
