using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class DocumentoEntradaCentroResultado
    {
        public decimal Valor { get; set; }
        public decimal Percentual { get; set; }
        public DateTime DataEmissao { get; set; }
        public CentroResultado CentroResultado { get; set; }
    }
}
