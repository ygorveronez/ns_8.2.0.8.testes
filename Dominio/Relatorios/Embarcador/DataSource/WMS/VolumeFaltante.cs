using System;

namespace Dominio.Relatorios.Embarcador.DataSource.WMS
{
    public class VolumeFaltante
    {
        public string FilialEmissora { get; set; }
        public DateTime DataEmissaoNF { get; set; }
        public string NumeroNota { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string Endereco { get; set; }
        public string CidadeEstado { get; set; }
        public string CEP { get; set; }
        public string Serie { get; set; }
        public string NumeroPedido { get; set; }
        public decimal VolumeTotal { get; set; }
        public decimal Volume { get; set; }
        public string FilialDistribuidora { get; set; }
        public string Rota { get; set; }
        public string CodigoBarra { get; set; }
    }
}
