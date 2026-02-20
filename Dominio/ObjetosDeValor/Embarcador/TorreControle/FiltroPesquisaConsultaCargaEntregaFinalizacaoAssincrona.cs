using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaConsultaCargaEntregaFinalizacaoAssincrona
    {
        public int CodigoCarga { get; set; }
        public long CodigoCliente { get; set; }
        public DateTime DataInicialInclusaoProcessamento { get; set; }
        public DateTime DataFinalInclusaoProcessamento { get; set; }
        public SituacaoProcessamentoIntegracao? SituacaoProcessamento { get; set; }
    }
}
