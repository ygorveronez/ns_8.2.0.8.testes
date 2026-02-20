using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public sealed class FiltroPesquisaLoteChamadoOcorrencia
    {
        #region Propriedades

        public int NumeroLote { get; set; }

        public DateTime DataCriacaoInicial { get; set; }

        public DateTime DataCriacaoFinal { get; set; }

        public int CodigoTransportador { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteChamadoOcorrencia> Situacao { get; set; }


        #endregion

    }
}
