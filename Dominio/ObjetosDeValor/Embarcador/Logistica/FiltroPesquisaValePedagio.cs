using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaValePedagio
    {
        public string CodigoCargaEmbarcador { get; set; }

        public DateTime? DataCargaInicial { get; set; }

        public DateTime? DataCargaFinal { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio? SituacaoValePedagio { get; set; }

        public string NumeroValePedagio { get; set; }

        public int CodigoTipoIntegracao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }

        public DateTime? DataIntegracaoInicial { get; set; }

        public DateTime? DataIntegracaoFinal { get; set; }

        public List<int> Filiais { get; set; }

        public List<double> Recebedores { get; set; }

        public string NumeroParcialCarga { get; set; }

    }
}
