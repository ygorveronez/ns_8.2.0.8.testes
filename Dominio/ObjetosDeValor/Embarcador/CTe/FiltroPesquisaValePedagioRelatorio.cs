using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaValePedagioRelatorio
    {
        public int CodigoCarga { get; set; }
        public DateTime DataCargaInicial { get; set; }
        public DateTime DataCargaFinal { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
        public List<string> NumeroValePedagio { get; set; }
        public List<SituacaoValePedagio> SituacaoValePedagio { get; set; }
        public List<SituacaoIntegracao> SituacaoIntegracaoValePedagio { get; set; }
        public DateTime DataCompraVPRInicial { get; set; }
        public DateTime DataCompraVPRFinal { get; set; }
        public bool ExibirCargasAgrupadas { get; set; }
        public bool ExibirTodasCargasPorPadrao { get; set; }
        public int Transportador { get; set; }
        public double Expedidor { get; set; }
        public double Recebedor { get; set; }
        public int Motorista { get; set; }
        public int Veiculo { get; set; }

    }
}
