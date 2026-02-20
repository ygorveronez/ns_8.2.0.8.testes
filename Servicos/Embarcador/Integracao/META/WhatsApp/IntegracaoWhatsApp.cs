using Dominio.ObjetosDeValor.Embarcador.Integracao.META.WhatsApp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Integracao.META.WhatsApp
{
    public class WhatsApp
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly string _urlEnvioMensagem;

        #endregion Atributo

        #region Construtores

        public WhatsApp(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _urlEnvioMensagem = "https://graph.facebook.com/v18.0/";
        }

        #endregion Construtores

        public bool EnviarNotificaçãoWhatsApp(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrencia, List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp> templateMensagemWhatsApp, Repositorio.UnitOfWork unitOfWork, string urlRastreamento)
        {

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoWhatsApp repIntegracaoWhatsApp = new Repositorio.Embarcador.Configuracoes.IntegracaoWhatsApp(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWhatsApp integracaoWhatsApp = repIntegracaoWhatsApp.Buscar();

                if (integracaoWhatsApp == null)
                    return false;

                if (string.IsNullOrEmpty(integracaoWhatsApp.IdNumeroTelefone) || string.IsNullOrEmpty(integracaoWhatsApp.Token))
                    return false;

                string urlEnvioMensagem = $"{_urlEnvioMensagem}{integracaoWhatsApp.IdNumeroTelefone}/messages";
                string token = integracaoWhatsApp.Token;

                string numeroCelular = $"{ocorrencia.Pedido.Destinatario.Localidade?.Pais?.CodigoTelefonico ?? 0}{ocorrencia.Pedido.Destinatario?.Telefone1 ?? ""}";

                if (string.IsNullOrEmpty(numeroCelular))
                    return false;

                if (!RealizarEnvioMensagemWhatsApp(urlEnvioMensagem, token, numeroCelular, templateMensagemWhatsApp.LastOrDefault(), ocorrencia, urlRastreamento))
                    return false;

                return true;

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex.Message, "NotificaoEntregaWhatsApp");
                return false;
            }
        }

        private bool RealizarEnvioMensagemWhatsApp(string urlEnvioMensagem, string token, string numeroCelular, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp templateMensagemWhatsApp, Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrencia, string urlRastreamento)
        {
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            RequestEnvioMensagem requestEnvioMensagem = new RequestEnvioMensagem();

            if (templateMensagemWhatsApp.TipoTemplate == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTemplateWhatsApp.AcomapanhamentoEntrega)
                requestEnvioMensagem = ObterBodyEnvioMensagemAcompanhamentoEntrega(numeroCelular, templateMensagemWhatsApp, ocorrencia, urlRastreamento);

            META svcMETA = new META();
            HttpClient requisicao = svcMETA.CriarRequisicaoConexaoMeta(urlEnvioMensagem, token);

            jsonRequisicao = JsonConvert.SerializeObject(requestEnvioMensagem, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlEnvioMensagem, conteudoRequisicao).Result;
            jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                return false;

            return true;
        }

        public RequestEnvioMensagem ObterBodyEnvioMensagemAcompanhamentoEntrega(string numeroCelular, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp templateMensagemWhatsApp, Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrencia, string urlRastreamento)
        {
            List<Componente> listaComponentes = new List<Componente>();
            List<ParametrosComponente> listaparametros = new List<ParametrosComponente>();

            //Aqui é realizado a construção do objeto das variáveis na ordem que foram enviadas no template de mensagem.

            Regex regex = new Regex("#\\w+");
            List<dynamic> listaOrdemTagsParaEnvio = new List<dynamic>();

            foreach (Match match in regex.Matches(templateMensagemWhatsApp.Mensagem))
            {
                listaOrdemTagsParaEnvio.Add(new { tag = match.Value, posicao = match.Index });
            }

            listaOrdemTagsParaEnvio.OrderBy(x => x.posicao);

            foreach (dynamic tag in listaOrdemTagsParaEnvio)
            {
                if (tag.tag == "#LinkAcompanhamentoEntrega")
                {
                    ParametrosComponente parametro = new ParametrosComponente()
                    {
                        Tipo = "text",
                        Texto = urlRastreamento
                    };

                    listaparametros.Add(parametro);
                }

                if (tag.tag == "#NumeroPedido")
                {
                    ParametrosComponente parametro = new ParametrosComponente()
                    {
                        Tipo = "text",
                        Texto = ocorrencia.Pedido?.NumeroPedidoEmbarcador ?? "***"
                    };

                    listaparametros.Add(parametro);
                }

                if (tag.tag == "#NomeCliente")
                {
                    ParametrosComponente parametro = new ParametrosComponente()
                    {
                        Tipo = "text",
                        Texto = ocorrencia.Pedido.Destinatario?.Nome ?? "***"
                    };

                    listaparametros.Add(parametro);
                }
            }
            Componente componenteTemplate = new Componente()
            {
                Tipo = "body",
                Parametros = listaparametros
            };

            listaComponentes.Add(componenteTemplate);

            Idioma idioma = new Idioma()
            {
                SiglaIdioma = templateMensagemWhatsApp.Idioma
            };

            Template template = new Template()
            {
                Nome = templateMensagemWhatsApp.Nome,
                Idioma = idioma,
                Componentes = listaComponentes
            };

            RequestEnvioMensagem requestEnvioMensagem = new RequestEnvioMensagem()
            {
                ProdutoEnvioMensagem = "whatsapp",
                Para = numeroCelular,
                Tipo = "template",
                Template = template

            };

            return requestEnvioMensagem;
        }

    }


}
