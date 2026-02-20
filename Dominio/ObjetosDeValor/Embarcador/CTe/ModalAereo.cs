using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ModalAereo
    {
        public long? NumeroOperacionalConhecimentoAereo { get; set; }
        public long? NumeroMinuta { get; set; }
        public DateTime DataPrevistaEntrega { get; set; }
        public string Dimensao { get; set; }

        public string ClasseTarifa { get; set; }
        public string CodigoTarifa { get; set; }
        public decimal? ValorTarifa { get; set; }
        public List<ModalAereoManuseio> Manuseios { get; set; }
    }
}
