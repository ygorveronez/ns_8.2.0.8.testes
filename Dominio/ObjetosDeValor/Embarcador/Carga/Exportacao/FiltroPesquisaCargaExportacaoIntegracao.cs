using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Exportacao
{
    public sealed class FiltroPesquisaCargaExportacaoIntegracao
    {
        public List<int> CodigoFilial { get; set; }

        public int CodigoVeiculo { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public List<int> Transportadores { get; set; }

        public DateTime? DataInicialAgendamento { get; set; }

        public DateTime? DataLimiteAgendamento { get; set; }

        public string NumeroEXP { get; set; }

        public Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }
    }
}
