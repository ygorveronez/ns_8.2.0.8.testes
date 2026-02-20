using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaValePallet
    {
        public int CodigoChamado { get; set; }
        public int CodigoFilial { get; set; }
        public double CpfCnpjCliente { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataLimite { get; set; }
        public int Numero { get; set; }
        public int NumeroNfe { get; set; }
        public Enumeradores.SituacaoValePallet? Situacao { get; set; }
    }
}
