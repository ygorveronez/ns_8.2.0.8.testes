
using System;

namespace Dominio.ObjetosDeValor.WebService.Filial
{
    public class FilialTanque
    {
        public string Filial { get; set; }
        public Dominio.ObjetosDeValor.WebService.Tanque.Tanque Tanque { get; set; }
        public Decimal Volume { get; set; }
        public Decimal Capacidade { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public decimal Vazao { get; set; }
        public decimal Ocupacao { get; set; }
        public string Status { get; set; }

    }    
}
