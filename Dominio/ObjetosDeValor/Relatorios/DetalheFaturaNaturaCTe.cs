using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class DetalheFaturaNaturaCTe
    {
        public DateTime DataEmissao { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal Desconto { get; set; }
    }
}
