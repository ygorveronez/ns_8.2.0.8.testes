using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.VincularGuia
{
    public sealed class FiltroPesquisaVincularGuia
    {
        public string NumeroDocumento { get; set; }

        public List<SituacaoGuia> Status { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public int CodigoCarga { get; set; }

        public SituacaoDigitalizacaoGuiaRecolhimento? SituacaoDigitalizacaoGuia { get; set; }

        public SituacaoDigitalizacaoGuiaComprovante? SituacaoDigitalizacaoComprovante { get; set; }

    }
}
