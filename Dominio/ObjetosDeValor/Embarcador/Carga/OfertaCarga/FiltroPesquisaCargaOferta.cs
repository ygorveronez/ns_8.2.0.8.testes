using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.CargaOferta
{
    public class FiltroPesquisaCargaOferta
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFim { get; set; }
        public List<int> CodigosCarga { get; set; }
        public int TipoOperacao { get; set; }
        public List<int> CodigosTiposCarga { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<int> CodigosTransportadores { get; set; }
        public Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }
        public Enumeradores.SituacaoCargaOferta? SituacaoOferta { get; set; }
    }
}
