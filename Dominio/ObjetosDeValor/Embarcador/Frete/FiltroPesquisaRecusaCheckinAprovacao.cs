using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FiltroPesquisaRecusaCheckinAprovacao
    {
        public int CodigoUsuario { get; set; }

        public int NumeroCTe { get; set; }
        public string CodigoCarga { get; set; }
        public DateTime? DataCriacaoCarga { get; set; }
        public int Filial { get; set; }
        public int Transportador { get; set; }
        public int SerieCte { get; set; }
        public int TipoOperacao { get; set; }

        public Enumeradores.SituacaoCheckin? SituacaoCheckin { get; set; }
    }
}
