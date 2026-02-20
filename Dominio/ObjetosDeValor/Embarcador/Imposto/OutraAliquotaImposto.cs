using System;

namespace Dominio.ObjetosDeValor.Embarcador.Imposto
{
    public class OutraAliquotaImposto
    {
        public int Codigo { get; set; }

        public int CodigoOutraAliquota { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto TipoImposto { get; set; }

        public decimal Aliquota { get; set; }

        public decimal AliquotaUF { get; set; }

        public decimal AliquotaMunicipio { get; set; }

        public DateTime DataVigenciaInicial { get; set; }

        public DateTime DataVigenciaFinal { get; set; }

        public bool InclusaoDocumento { get; set; }

        public int CodigoLocalidade { get; set; }

        public string SiglaUF { get; set; }

        public decimal PercentualReducao { get; set; }

        public decimal PercentualReducaoUF { get; set; }

        public decimal PercentualReducaoMunicipio { get; set; }

    }
}
