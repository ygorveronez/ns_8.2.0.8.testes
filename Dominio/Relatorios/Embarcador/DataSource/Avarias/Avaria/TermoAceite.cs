using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Avarias
{
    public class TermoAceite
    {
        public string NomeEmpresa { get; set; }
        public int NumeroLote { get; set; }
        public decimal ValorLote { get; set; }
        public string ValorLoteExtenso { get; set; }
        public string Transportador { get; set; }
        public string NumeroAvarias { get; set; }
        public DateTime DataGeracao { get; set; }
    }
}
