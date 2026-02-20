using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaControleExpedicao
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoMotorista { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoVeiculo { get; set; }

        public List<int> CodigosFilial { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public Enumeradores.SituacaoCargaControleExpedicao Situacao { get; set; }
    }
}
