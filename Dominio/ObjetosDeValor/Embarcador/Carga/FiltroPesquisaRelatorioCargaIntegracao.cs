using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaRelatorioCargaIntegracao
    {
        public DateTime DataInicialCarga { get; set; }
        public DateTime DataFinalCarga { get; set; }
        public DateTime DataInicioIntegracao { get; set; }
        public DateTime DataFinalIntegracao { get; set; }
        public SituacaoIntegracao? Situacao { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoTipoIntegracao { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public int CodigoTransportador { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
        public SituacaoCarga SituacaoCarga { get; set; }
    }
}
