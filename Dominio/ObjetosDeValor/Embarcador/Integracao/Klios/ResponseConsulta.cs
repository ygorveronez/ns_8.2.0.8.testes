using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Klios
{
    public class ResponseConsulta
    {
        public string id_KliosAnalise { get; set; }
        public string data_cadastro { get; set; }
        public string data_conclusao { get; set; }
        public string data_expiracao { get; set; }
        public string StatusAnalise { get; set; }
        public string resultado { get; set; }
        public List<Restricoes> restricoes { get; set; }
        public List<string> observacoes { get; set; }
        public string observacoes_gerais { get; set; }
        public List<Documento> documnetos { get; set; }
        public string urlPdf { get; set; }
        public string sms_send { get; set; }
        public string sms_return { get; set; }
    }
}
