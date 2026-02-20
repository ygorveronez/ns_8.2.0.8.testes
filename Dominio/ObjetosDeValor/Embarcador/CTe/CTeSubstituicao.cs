using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class CTeSubstituicao
    {
        public string ChaveCTeSubstituido { get; set; }
        public Dominio.Enumeradores.TipoDocumentoAnulacao Tipo { get; set; }
        public Dominio.Enumeradores.OpcaoSimNao ContribuinteICMS { get; set; }
        public string Chave { get; set; }
        public int ModeloDocumentoFiscal { get; set; }
        public double Emitente { get; set; }
        public string Numero { get; set; }
        public string Serie { get; set; }
        public string Subserie { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataEmissao { get; set; }
    }
}
