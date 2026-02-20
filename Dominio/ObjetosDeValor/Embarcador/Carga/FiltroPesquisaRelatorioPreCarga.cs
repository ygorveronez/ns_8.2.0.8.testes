using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaRelatorioPreCarga
    {
        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public DateTime DataCriacaoPreCargaInicial { get; set; }

        public DateTime DataCriacaoPreCargaFinal { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoConfiguracaoProgramacaoCarga { get; set; }

        public int CodigoOperador { get; set; }

        public string PreCarga { get; set; }

        public string Pedido { get; set; }

        public string Carga { get; set; }

        public double CpfCnpjRemetente { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public FiltroPreCarga Situacao { get; set; }

        public bool SomenteProgramacaoCarga { get; set; }
    }
}
