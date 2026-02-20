using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet
{
    public sealed class MovimentosPalletPendentes
    {
        public int CodigoMovimentoPallet { get; set; }
        public int TipoResponsavel { get; set; }
        public string Carga { get; set; }
        public DateTime DataEmissaoNota { get; set; }
        public int NumeroNota { get; set; }
        public string Responsavel { get; set; }
        public string EmailResponsavel { get; set; }
    }
}
