using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaMonitoramentoNivelServico
    {
        public string CodigoCargaEmbarcador { get; set; }

        public string PlacaVeiculo { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public DateTime DataConfirmacaoDocumentosInicial { get; set; }

        public DateTime DataConfirmacaoDocumentosFinal { get; set; }

        public List<int> Filiais { get; set; }

        public List<double> Recebedores { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

    }
}
