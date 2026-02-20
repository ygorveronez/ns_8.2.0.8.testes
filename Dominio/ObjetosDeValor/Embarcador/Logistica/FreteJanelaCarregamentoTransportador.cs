using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FreteJanelaCarregamentoTransportador
    {
        public int Codigo { get; set; }

        public string CargaCodigoEmbarcador { get; set; }

        public decimal ValorFrete { get; set; }

        public string Valor {
            get
            {
                return ValorFrete.ToString("n2");
            }
            set
            {
                ValorFrete = value.ToDecimal();
            }
        }
    }
}
