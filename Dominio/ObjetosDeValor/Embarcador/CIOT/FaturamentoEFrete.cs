using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT
{
    public class FaturamentoEFrete
    {
        public DateTime Fechamento { get; set; }

        public DateTime Vencimento { get; set; }

        public string Transportadora { get; set; }

        public long Numero { get; set; }

        public decimal Taxa { get; set; }

        public decimal Tarifa { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo Tipo { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteStatus Status { get; set; }

        public List<FaturamentoEFreteItem> Itens { get; set; }

        public List<FaturamentoEFreteOutro> Outros { get; set; }

        public List<FaturamentoEFretePagamento> Pagamentos { get; set; }
    }
}
