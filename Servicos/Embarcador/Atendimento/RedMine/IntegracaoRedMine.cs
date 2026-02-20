using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Atendimento.RedMine
{
    public sealed class IntegracaoRedMine
    {
        #region Atributos Protegidos Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private readonly Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRedMine _configuracaoRedMine;

        private Dominio.Enumeradores.TipoAmbiente _tipoAmbiente;

        private string _url = "http://redmine.multiembarcador.com.br/redmine/";


        #endregion

        #region Construtores

        public IntegracaoRedMine(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRedMine configuracaoRedMine, Dominio.Enumeradores.TipoAmbiente tipoambiente, Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _configuracaoRedMine = configuracaoRedMine;
            _tipoAmbiente = tipoambiente;
        }

        #endregion

        public int CriarTarefaRedMine(Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa atendimento)
        {
            string jsonRequest = string.Empty; string jsonResponse = string.Empty;
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Atendimento.AtendimentoChamadoRedmine novaTarefa = preencherEntidade(atendimento);

                _url += "issues.json";
                HttpClient client = ObterClienteRequisicao(_url, _configuracaoRedMine.TokenUsuarioCadastro);
                jsonRequest = JsonConvert.SerializeObject(novaTarefa, Formatting.Indented);

                // Request
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var result = client.PostAsync(_url, content).Result;

                if (result.IsSuccessStatusCode)
                {
                    string retorno = result.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic retornoJSON = JsonConvert.DeserializeObject<dynamic>(retorno);
                        return retornoJSON.issue.id;
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoRedMine");

            }

            return 0;
        }

        public void AtualizarTarefaRedMine(Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa atendimento)
        {
            if (atendimento.NumeroChamadoRedMine > 0)
            {
                string jsonRequest = string.Empty; string jsonResponse = string.Empty;
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Atendimento.AtendimentoChamadoRedmine novaTarefa = preencherEntidade(atendimento);
                    _url += "issues/" + atendimento.NumeroChamadoRedMine.ToString() + ".json";

                    HttpClient client = ObterClienteRequisicao(_url, _configuracaoRedMine.TokenUsuarioCadastro);
                    jsonRequest = JsonConvert.SerializeObject(novaTarefa, Formatting.Indented);

                    // Request
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    var result = client.PutAsync(_url, content).Result;

                    if (!result.IsSuccessStatusCode)
                    {
                        string retorno = result.Content.ReadAsStringAsync().Result;
                        // Servicos.Log.TratarErro(retorno, "IntegracaoRedMine");
                    }
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao, "IntegracaoRedMine");
                }
            }
        }

        public void EnviarAnexoTarefaRedMine(Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaAnexo anexo)
        {

            //string jsonRequest = string.Empty; string jsonResponse = string.Empty;
            //try
            //{
            //    // Dominio.ObjetosDeValor.Embarcador.Atendimento.AtendimentoChamadoRedmine novaTarefa = preencherEntidade(atendimento);
            //    HttpClient client = ObterClienteRequisicao(_url, _configuracaoRedMine.TokenUsuarioCadastro);
            //   // jsonRequest = JsonConvert.SerializeObject(novaTarefa, Formatting.Indented);

            //    FileInfo fileInfo = new FileInfo(anexo.CaminhoArquivo );
            //    long fileLength = fileInfo.Length;

            //    client.DefaultRequestHeaders.Add("Content-Type", "application/octet-stream");
            //    client.DefaultRequestHeaders.Add("Content-Length", fileLength.ToString());

            //    byte[] data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(anexo.CaminhoArquivo);
             
            //    _url += "issues/" + anexo.AtendimentoTarefa.NumeroChamadoRedMine.ToString() + ".json";
               
            //    // Request
            //    var content = new StringContent(, Encoding.UTF8, "application/json");
            //    var result = client.PostAsync(_url, content).Result;

            //    if (!result.IsSuccessStatusCode)
            //    {
            //        string retorno = result.Content.ReadAsStringAsync().Result;
            //        // Servicos.Log.TratarErro(retorno, "IntegracaoRedMine");
            //    }
            //}
            //catch (Exception excecao)
            //{
            //    Servicos.Log.TratarErro(excecao, "IntegracaoRedMine");
            //}



            //require 'rest_client'
            //require 'json'

            //key = '5daf2e447336bad7ed3993a6ebde8310ffa263bf'
            //upload_url = "http://localhost:3000/uploads.json?key=#{key}"
            //wiki_url = "http://localhost:3000/projects/some_project/wiki/some_wiki.json?key=#{key}"
            //img = File.new('/some/image.png')

            //# First we upload the image to get attachment token
            //response = RestClient.post(upload_url, img, {
            //  :multipart => true,
            //  :content_type => 'application/octet-stream'
            //})
            //token = JSON.parse(response)['upload']['token']

            //# Redmine will throw validation errors if you do not
            //# send a wiki content when attaching the image. So
            //# we just get the current content and send that
            //wiki_text = JSON.parse(RestClient.get(wiki_url))['wiki_page']['text']

            //response = RestClient.put(wiki_url, {
            //  :attachments =>
            //   {
            //    :attachment1 =>
            //     { # the hash key gets thrown away - name doesn't matter
            //      :token => token,
            //      :filename => 'image.png',
            //      :description => 'Awesome!' # optional
            //    }
            //  },
            //  :wiki_page =>
            //   {
            //    :text => wiki_text # original wiki text
            //  }
            //})

        }



        private Dominio.ObjetosDeValor.Embarcador.Atendimento.AtendimentoChamadoRedmine preencherEntidade(Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa atendimento)
        {
            string tipoAmbiente = _tipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? " (Produção)" : " (Homologação)";

            String DescriptionChamado = atendimento.Solicitante != null ? "*Funcionario Solicitante:* " + atendimento.Solicitante.Nome : "";
            DescriptionChamado += atendimento.AtendimentoTela != null ? " \r\n*Tela:* " + atendimento.AtendimentoTela.Descricao : "";
            DescriptionChamado += atendimento.AtendimentoModulo != null ? " \r\n*Módulo:* " + atendimento.AtendimentoModulo.Descricao : "";
            DescriptionChamado += " \r\n*Prioridade:* " + atendimento.Prioridade.ObterDescricao();
            DescriptionChamado += " \r\n\r\n*Motivo/Problema Relatado:* " + atendimento.MotivoProblema;
            DescriptionChamado += " \r\n\r\n\r\n*Observação:* " + atendimento.JustificativaObservacao;

            Dominio.ObjetosDeValor.Embarcador.Atendimento.AtendimentoChamadoRedmine novaTarefa = new Dominio.ObjetosDeValor.Embarcador.Atendimento.AtendimentoChamadoRedmine();
            List<Dominio.ObjetosDeValor.Embarcador.Atendimento.CustomField> customField = new List<Dominio.ObjetosDeValor.Embarcador.Atendimento.CustomField>();
            Dominio.ObjetosDeValor.Embarcador.Atendimento.Issue issue = new Dominio.ObjetosDeValor.Embarcador.Atendimento.Issue
            {
                assigned_to_id = _configuracaoRedMine.CodigoUsuarioDestino, //atribuido para
                description = DescriptionChamado, // descricao chamado
                priority_id = 2, // prioridade normal
                estimated_hours = 0, //horas estimadas
                project_id = 53, //projeto chamados
                status_id = 1, //estado - nova
                subject = atendimento.Titulo + tipoAmbiente, //titulo
                tracker_id = 7 //Nova funcionalidade
            };

            Dominio.ObjetosDeValor.Embarcador.Atendimento.CustomField Cliente = new Dominio.ObjetosDeValor.Embarcador.Atendimento.CustomField
            {
                id = 5,
                name = "Cliente",
                value = _configuracaoRedMine.ClienteRedMine,
            };

            Dominio.ObjetosDeValor.Embarcador.Atendimento.CustomField StatusChamado = new Dominio.ObjetosDeValor.Embarcador.Atendimento.CustomField
            {
                id = 10,
                name = "Status Chamado",
                value = "1 – Aberto"
            };

            customField.Add(Cliente);
            customField.Add(StatusChamado);
            issue.custom_fields = customField;
            novaTarefa.issue = issue;

            return novaTarefa;
        }

        private static HttpClient ObterClienteRequisicao(string url, string token)
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRedMine)); 

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue("robot", "jagunço@123");
            client.DefaultRequestHeaders.Add("X-Redmine-API-Key", token);

            return client;
        }
    }
}
