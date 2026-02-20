using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Avon
{
    public class InformacoesIntegracaoAvon
    {
        public InformacoesIntegracaoAvon()
        {
            this.DataConsulta = DateTime.Now;
        }

        public InformacoesIntegracaoAvon(long numeroMinuta)
        {
            this.NumeroMinuta = numeroMinuta;
            this.DataConsulta = DateTime.Now;
        }

        public string Mensagem { get; set; }

        public string CodigoMensagem { get; set; }

        public string GUID { get; set; }

        public long NumeroMinuta { get; set; }

        public DateTime DataConsulta { get; set; }

        public string Requisicao { get; set; }

        public string Resposta { get; set; }

        public Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message Response { get; set; }

        public List<Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica> Documentos { get; set; }
    }
}
