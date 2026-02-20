using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta
{
    public class FiltroPesquisaAgendamentoColetaPallet
    {
        public int NumeroOrdem { get; set; }

        public List<int> CodigoCarga { get; set; }

        public List<int> CodigosFilial { get; set; }

        public DateTime DataOrdem { get; set; }

        public SituacaoAgendamentoColetaPallet? StatusAgendamento { get; set; }

        public ResponsavelPallet? ResponsavelAgendamento { get; set; }

        public long CodigoCliente { get; set; }

        public int CodigoTransportador { get; set; }
    }
}
