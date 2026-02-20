using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class parameters
    {
        public string origem_request { get; set; }
        public string documento { get; set; }
        public string estrangeiro { get; set; }
        public decimal frete_adiantamento { get; set; }
        public string operation { get; set; }
        public decimal frete_valor { get; set; }
        public string data_inicio { get; set; }
        public string data_termino { get; set; }
        public decimal peso { get; set; }
        public string identificador_lote_api { get; set; }
        public string identificador_proposta_api { get; set; }
        public string observacao { get; set; }
        public List<placas> placas { get; set; }
    }
}
