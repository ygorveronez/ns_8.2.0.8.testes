using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Avarias
{
    public sealed class FiltroPesquisaRelatorioAvarias
    {
        public string CodigoCargaEmbarcador { get; set; }
        public int NumeroAvaria { get; set; }
        public int CodigoSolicitante { get; set; }
        public int CodigoTransportador { get; set; }
        public DateTime DataSolicitacaoInicial { get; set; }
        public DateTime DataSolicitacaoFinal { get; set; }
        public DateTime DataGeracaoLoteInicial { get; set; }
        public DateTime DataGeracaoLoteFinal { get; set; }
        public DateTime DataIntegracaoLoteInicial { get; set; }
        public DateTime DataIntegracaoLoteFinal { get; set; }
        public List<SituacaoAvaria> SituacaoAvaria { get; set; }
        public EtapaLote Etapa { get; set; }
    }
}
