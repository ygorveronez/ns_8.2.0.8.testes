using System;

namespace Dominio.ObjetosDeValor.Embarcador.CanhotoIntegracao
{
    public class FiltroPesquisaCanhotoIntegracao
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? Situacao { get; set; }
        public int NumeroDocumento { get; set; }
        public int CodigoEmpresa { get; set; }
        public double Emitente { get; set; }
        public int CodigoCanhoto { get; set; }
        public int Carga { get; set; }
        public int Filial { get; set; }
        public int Transportador { get; set; }
        public int NumeroCTe { get; set; }
        public int CodigoTipoIntegracao { get; set; }
    }
}
