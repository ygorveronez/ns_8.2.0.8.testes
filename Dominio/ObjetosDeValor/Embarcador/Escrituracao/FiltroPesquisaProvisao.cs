using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public sealed class FiltroPesquisaProvisao
    {
        public int CodigoCarga { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoLocalidadePrestacao { get; set; }

        public int CodigoOcorrencia { get; set; }

        public int CodigoTransportador { get; set; }

        public double CpfCnpjTomador { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public int Numero { get; set; }

        public int NumeroDocumento { get; set; }

        public Enumeradores.SituacaoProvisao SituacaoProvisao { get; set; }

        public Enumeradores.TipoProvisao TipoProvisao { get; set; }
    }
}
