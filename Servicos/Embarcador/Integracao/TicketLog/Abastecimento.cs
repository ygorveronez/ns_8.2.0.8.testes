using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Servicos
{
    public class AbastecimentoTicketLog : ServicoBase
    {        
        public AbastecimentoTicketLog(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public Dominio.ObjetosDeValor.Embarcador.WebService.TicketLog.Abastecimento ObterAbastecimentosTicktLog(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTicketLog repConfiguracaoIntegracaoTicketLog = new Repositorio.Embarcador.Configuracoes.IntegracaoTicketLog(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTicketLog configuracaoIntegracaoTicketLog = repConfiguracaoIntegracaoTicketLog.Buscar();
            var dados = new Dominio.ObjetosDeValor.Embarcador.WebService.TicketLog.Abastecimento();
            dados.Sucesso = false;
            dados.DataChamada = DateTime.Now;
            var objetoChamada = new
            {
                codigoCliente = configuracaoIntegracaoTicketLog.CodigoClienteTicketLog,
                codigoTipoCartao = 4,
                dataTransacaoInicial = DateTime.Now.ToString("yyyy-MM-ddT00:00:00"),
                dataTransacaoFinal = DateTime.Now.ToString("yyyy-MM-ddT23:59:59"),
                considerarTransacao = "T",
                ordem = "S"
            };
            
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(configuracaoIntegracaoTicketLog.URLTicketLog);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", configuracaoIntegracaoTicketLog.ChaveAutorizacaoTicketLog);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(objetoChamada));
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    dados = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.WebService.TicketLog.Abastecimento>(result);                    
                }
            }
            catch(Exception ex) {
                Servicos.Log.TratarErro(ex);                
            }
            
            return dados;
        }
    }
}
