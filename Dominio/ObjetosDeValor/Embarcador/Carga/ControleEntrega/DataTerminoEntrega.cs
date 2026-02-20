using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public sealed class DataTerminoEntrega
    {
        public int CodigoEntrega { get; set; }
        public string DataFimEntrega { get; set; }
        public DateTime DataFimEntregaFormatada
        {
            get
            {
                return System.StringExtension.ToDateTime(this.DataFimEntrega);
            }
        }
    }
}
