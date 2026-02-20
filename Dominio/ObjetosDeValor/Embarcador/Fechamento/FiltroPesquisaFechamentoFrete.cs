using System;

namespace Dominio.ObjetosDeValor.Embarcador.Fechamento
{
    public sealed class FiltroPesquisaFechamentoFrete
    {
        public int CodigoContratoFrete { get; set; }

        public int CodigoTransportador { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public int Numero { get; set; }

        public Enumeradores.SituacaoFechamentoFrete? Situacao { get; set; }
    }
}
