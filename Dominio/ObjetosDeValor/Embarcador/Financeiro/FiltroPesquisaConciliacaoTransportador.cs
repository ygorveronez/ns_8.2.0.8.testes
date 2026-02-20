using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaConciliacaoTransportador
    {
        public List<int> CodigosTransportador { get; set; }
        public string RaizCnpj { get; set; }
        public SituacaoConciliacaoTransportador SituacaoConciliacaoTransportador { get; set; }

        public int NumeroCarta { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime AnuenciaDisponivelInicio { get; set; }
        public DateTime AnuenciaDisponivelFinal { get; set; }
        public DateTime DataInicialAssinatura { get; set; }
        public DateTime DataFinalAssinatura { get; set; }
    }
}
