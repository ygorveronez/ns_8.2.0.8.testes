using System;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT
{
    public class FaturamentoEFreteOutro
    {
        public DateTime Data { get; set; }

        public decimal Valor { get; set; }

        public string Tipo { get; set; }

        public string TipoLancamento { get; set; }

        public string Documento { get; set; }

        public string Detalhes { get; set; }
    }
}
