using System;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class FiltroGestaoNotasFiscais
    {
        public int Numero { get; set; }
        public int Serie { get; set; }
        public double CPFCNPJEmitente { get; set; }
        public int CodigoCTe { get; set; }
        public int CodigoCarga { get; set; }
        public DateTime? DataEmissaoNotaFiscalInicial { get; set; }
        public DateTime? DataEmissaoNotaFiscalFinal { get; set; }
        public DateTime? DataEmissaoCTeInicial { get; set; }
        public DateTime? DataEmissaoCTeFinal { get; set; }
        public DateTime? DataEmissaoCargaInicial { get; set; }
        public DateTime? DataEmissaoCargaFinal { get; set; }
        public int CodigoEmpresa { get; set; }
        public string Produto { get; set; }
        public bool? PossuiCTe { get; set; }
        public string Veiculo { get; set; }
    }
}
