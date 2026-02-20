using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaContratoPrestacaoServicoSaldo
    {
        public int CodigoContratoPrestacaoServico { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public Enumeradores.TipoLancamento? TipoLancamento { get; set; }
    }
}
