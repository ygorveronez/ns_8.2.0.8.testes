using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Imposto.OutrasAliquotas
{
    public class DadosOutrasAliquotasImposto
    {

        #region Propriedades Publicas
        public int Codigo { get; set; }

        public TipoImposto TipoImposto { get; set; }

        public decimal Aliquota { get; set; }

        public decimal PercentualReducao { get; set; }

        public string UF { get; set; }

        public decimal AliquotaUF { get; set; }

        public decimal PercentualReducaoUF { get; set; }

        public string Municipio { get; set; }

        public decimal AliquotaMunicipio { get; set; }

        public decimal PercentualReducaoMunicipio { get; set; }

        public DateTime? DataInicialVigencia { get; set; }

        public DateTime? DataFinalVigencia { get; set; }

        public bool InclusaoDocumento { get; set; }
        #endregion

        #region Propriedades Com Regra
        public string TipoImpostoFormatado { get { return TipoImposto.ObterDescricao(); } }

        public string Vigencia
        {
            get
            {
                return $"{(DataInicialVigencia.HasValue ? DataInicialVigencia?.ToString("dd/MM/yyyy") : "")} - {(DataFinalVigencia.HasValue ? DataFinalVigencia?.ToString("dd/MM/yyyy") : "")}";
            }
        }

        public string InclusaoDocumentoFormatado
        {
            get
            {
                return InclusaoDocumento ? "Sim" : "Não";
            }
        }
        #endregion
    }
}
