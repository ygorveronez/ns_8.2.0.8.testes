using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using Dominio.ObjetosDeValor.Embarcador.Integracao.META.WhatsApp;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Integracao.META.Templates
{
    public class TemplateMensagemWhatsApp
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly string _urlTemplate;

        #endregion Atributo

        #region Construtores

        public TemplateMensagemWhatsApp(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _urlTemplate = "https://graph.facebook.com/v18.0/";
        }

        #endregion Construtores

        public void CriarNovoTemplate(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp templateMensagemWhatsApp)
        {
            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoWhatsApp repIntegracaoWhatsApp = new Repositorio.Embarcador.Configuracoes.IntegracaoWhatsApp(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWhatsApp integracaoWhatsApp = repIntegracaoWhatsApp.Buscar();

                string jsonRequisicao = string.Empty;
                string jsonRetorno = string.Empty;

                if (integracaoWhatsApp == null)
                    return;

                if (string.IsNullOrEmpty(integracaoWhatsApp.Token) || string.IsNullOrEmpty(integracaoWhatsApp.IdContaWhatsAppBusiness))
                    return;

                string urlCriarTemplate = $"{_urlTemplate}{integracaoWhatsApp.IdContaWhatsAppBusiness}/message_templates";
                string token = integracaoWhatsApp.Token;


                META svcMETA = new META();
                HttpClient requisicao = svcMETA.CriarRequisicaoConexaoMeta(urlCriarTemplate, token);

                jsonRequisicao = JsonConvert.SerializeObject(ObterBodyTemplate(templateMensagemWhatsApp), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlCriarTemplate, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.NaoFoiPossivelEnviarAIntegracaoParaCriacaoDoTemplateNoMETA);
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex.Message, "CriarTemplateWhatsApp");
                throw new Exception(ex.Message);
            }

        }

        public void AtualizarStatusTemplate(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp template)
        {
            string jsonRetorno = string.Empty;

            Repositorio.Embarcador.Configuracoes.IntegracaoWhatsApp repIntegracaoWhatsApp = new Repositorio.Embarcador.Configuracoes.IntegracaoWhatsApp(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTemplateWhatsApp repConfiguracaoTemplateWhatsApp = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTemplateWhatsApp(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWhatsApp integracaoWhatsApp = repIntegracaoWhatsApp.Buscar();

            string urlLerTemplate = $"{_urlTemplate}{integracaoWhatsApp.IdContaWhatsAppBusiness}/message_templates?name={template.Nome}&limit=200";
            string token = integracaoWhatsApp.Token;

            META svcMETA = new META();
            HttpClient requisicao = svcMETA.CriarRequisicaoConexaoMeta(urlLerTemplate, token);

            HttpResponseMessage retornoRequisicao = requisicao.GetAsync(urlLerTemplate).Result;
            jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

            if (retornoRequisicao.IsSuccessStatusCode)
            {
                string statusAprovacao = string.Empty;

                foreach (var item in data.data)
                {
                    if (item.name == template.Nome)
                    {
                        statusAprovacao = item.status;
                        break;
                    }

                }

                if (!string.IsNullOrEmpty(statusAprovacao))
                {
                    if (statusAprovacao == "APPROVED")
                        template.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTemplateWhatsApp.Aprovado;
                    else if (statusAprovacao == "PENDING")
                        template.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTemplateWhatsApp.AguardandoAprovacao;
                    else if (statusAprovacao == "REJECTED")
                        template.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTemplateWhatsApp.Rejeitado;
                    else
                        template.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTemplateWhatsApp.Rejeitado;

                    repConfiguracaoTemplateWhatsApp.Atualizar(template);
                }
            }
        }

        public ObjetoCriarTemplate ObterBodyTemplate(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp templateMensagemWhatsApp)
        {
            //Para entender a construção dos objetos, leia a documentação aexada na tarefa #61273

            List<ComponentesTemplate> componentesTemplate = new List<ComponentesTemplate>();


            if (templateMensagemWhatsApp.TipoTemplate == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTemplateWhatsApp.AcomapanhamentoEntrega)
            {
                //Aqui é criado o body que é enviado no template de mensagem, cada tipo de template tem um texto e tags diferentes
                ComponentesTemplate bodyTemplateAcompanhamentoEntrega = CriarBodyTemplateAcomapanhamentoEntrega(templateMensagemWhatsApp);

                componentesTemplate.Add(bodyTemplateAcompanhamentoEntrega);
            }

            ObjetoCriarTemplate bodyRequest = new ObjetoCriarTemplate()
            {
                Nome = templateMensagemWhatsApp.Nome,
                Categoria = "UTILITY",
                PermiteMudancaCategoria = true,
                Idioma = templateMensagemWhatsApp.Idioma,
                Componentes = componentesTemplate
            };

            return bodyRequest;
        }

        public ComponentesTemplate CriarBodyTemplateAcomapanhamentoEntrega(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp templateMensagemWhatsApp)
        {
            //Aqui é criado o template do tipo AcompanhamentoEntrega

            //Strings de exemplo que precisam ser enviadas ao META para cada tag que será enviada na mensagem, para entender melhor tem que ler a documentação
            string urlAcompanhamentoEntrega = "https://glassmexico.multiembarcador.com.br/sua-entrega/366bdc8e8b75401ea74b7c95ae6c05b5";
            string numeroCarga = "123456";
            string nomeCliente = "NomeCliente";

            //Aqui são as tags que serão substituídas pelas variáveis da mensagem
            string tagLinkAcompanhamentoEntrega = "#LinkAcompanhamentoEntrega";
            string tagNumeroCarga = "#NumeroPedido";
            string tagNomeCliente = "#NomeCliente";

            string mensagemFormatadaParaEnvio = templateMensagemWhatsApp.Mensagem;

            List<dynamic> listaVariaveisBody = new List<dynamic>();

            //Aqui é onde verificamos a ordem que cada tag foi enviada na mensagem, e substituímos pela variável {{}} com o número da posição em que o # da tag foi encontrado.
            Regex regex = new Regex("#\\w+");
            List<dynamic> listaOrdemTagsParaEnvio = new List<dynamic>();

            foreach (Match match in regex.Matches(templateMensagemWhatsApp.Mensagem))
            {
                listaOrdemTagsParaEnvio.Add(new { tag = match.Value, posicao = match.Index });
            }

            //Aqui é feita a substituição.
            foreach (dynamic tag in listaOrdemTagsParaEnvio)
            {
                if (tag.tag == "#LinkAcompanhamentoEntrega")
                {
                    listaVariaveisBody.Add(urlAcompanhamentoEntrega);
                    mensagemFormatadaParaEnvio = Regex.Replace(mensagemFormatadaParaEnvio, tagLinkAcompanhamentoEntrega, string.Concat("{{", listaVariaveisBody.FindIndex(x => x == urlAcompanhamentoEntrega) + 1, "}}"));
                }

                if (tag.tag == "#NumeroPedido")
                {
                    listaVariaveisBody.Add(numeroCarga);
                    mensagemFormatadaParaEnvio = Regex.Replace(mensagemFormatadaParaEnvio, tagNumeroCarga, string.Concat("{{", listaVariaveisBody.FindIndex(x => x == numeroCarga) + 1, "}}"));
                }

                if (tag.tag == "#NomeCliente")
                {
                    listaVariaveisBody.Add(nomeCliente);
                    mensagemFormatadaParaEnvio = Regex.Replace(mensagemFormatadaParaEnvio, tagNomeCliente, string.Concat("{{", listaVariaveisBody.FindIndex(x => x == nomeCliente) + 1, "}}"));
                }
            }

            List<dynamic> listaConteudoVariavelBody = new List<dynamic>();

            listaConteudoVariavelBody.Add(listaVariaveisBody);

            ExemploVariavelParaTemplate variavelBody = new ExemploVariavelParaTemplate()
            {
                ConteudoVarivaelBodyMensagem = listaConteudoVariavelBody
            };

            ComponentesTemplate bodyTemplate = new ComponentesTemplate()
            {
                Tipo = "Body",
                Texto = mensagemFormatadaParaEnvio,
                ExemploVariavel = variavelBody
            };

            return bodyTemplate;
        }
    }
}
