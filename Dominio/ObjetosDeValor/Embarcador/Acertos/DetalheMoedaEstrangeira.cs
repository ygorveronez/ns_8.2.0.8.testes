using System;

namespace Dominio.ObjetosDeValor.Embarcador.Acertos
{
    public class DetalheMoedaEstrangeira
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string Pessoa { get; set; }
        public string Descricao { get; set; }
        public decimal ValorReais { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral Moeda { get; set; }
        public decimal ValorMoeda { get; set; }
        public decimal ValorTotalMoeda { get; set; }
    }
}
