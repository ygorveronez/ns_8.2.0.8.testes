using System;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public sealed class FiltroPesquisaBiddingAprovacao
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataLimite { get; set; }
        public int Numero { get; set; }
        public double CpfCnpjFornecedor { get; set; }
        public int CodigoOperador { get; set; }
        public int CodigoUsuario { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoSolicitante { get; set; }
        public int CodigoTipoBidding {  get; set; }
    }
}
