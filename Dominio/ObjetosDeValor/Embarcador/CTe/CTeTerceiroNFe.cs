using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class CTeTerceiroNFe
    {
        public CTeTerceiro CTeTerceiro { get; set; }
        public string Chave { get; set; }
        public string Numero { get; set; }
        public string Serie { get; set; }
        public DateTime? DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal Volumes { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoCubado { get; set; }
        public string NumeroRomaneio { get; set; }
        public string NumeroPedido { get; set; }
        public string Protocolo { get; set; }
        public string ProtocoloCliente { get; set; }
        public string NumeroReferenciaEDI { get; set; }
        public string PINSuframa { get; set; }
        public string NCM { get; set; }
        public string NumeroControleCliente { get; set; }
    }
}
