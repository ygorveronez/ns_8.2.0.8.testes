using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ContratoFinanciamento
    {
        public int Numero { get; set; }
        public DateTime DataEmissao { get; set; }
        public string NumeroDocumento { get; set; }
        public decimal ValorTotal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento Situacao { get; set; }
        public string Observacao { get; set; }
    }
}
