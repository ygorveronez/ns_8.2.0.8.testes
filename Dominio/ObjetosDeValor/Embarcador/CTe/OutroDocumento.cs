using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class OutroDocumento
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento Tipo { get; set; }
        public string Descricao { get; set; }
        public string Numero { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataEmissao { get; set; }
        public string NumeroReferenciaEDI { get; set; }
        public string NumeroControleCliente { get; set; }        
        public string PINSuframa { get; set; }
        public string NCMPredominante { get; set; }
        public string CFOP { get; set; }
        public decimal Peso { get; set; }
    }
}
