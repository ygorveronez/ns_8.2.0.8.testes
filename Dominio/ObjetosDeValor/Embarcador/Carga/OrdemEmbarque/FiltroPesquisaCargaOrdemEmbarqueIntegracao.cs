using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.OrdemEmbarque
{
    public sealed class FiltroPesquisaCargaOrdemEmbarqueIntegracao
    {
        public List<int> CodigoFilial { get; set; }

        public bool Cancelada { get; set; }

        public int CodigoVeiculo { get; set; }

        public string NumeroOrdemEmbarque { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public List<int> Transportadores { get; set; }

        public DateTime? DataInicialAgendamento { get; set; }

        public DateTime? DataLimiteAgendamento { get; set; }

        public Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }

        public bool SomenteComLicencaInvalida { get; set; }
    }
}
