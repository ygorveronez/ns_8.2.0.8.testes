using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga
{
    public sealed class SugestaoProgramacaoCargaHistoricoCarga
    {
        #region Propriedades

        public string CidadesDestino { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public DateTime DataCriacaoCarga { get; set; }

        public DateTime DataFinalizacaoEmissao { get; set; }

        public string EstadosDestino { get; set; }

        public string Filial { get; set; }

        public string ModeloVeicularCarga { get; set; }

        public string TipoCarga { get; set; }

        public string TipoOperacao { get; set; }

        public string Transportador { get; set; }

        public string PlacaTracao { get; set; }

        public string PlacaReboques { get; set; }

        public decimal Peso { get; set; }

        public string RegioesDestino { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string DataCriacaoCargaFormatada
        {
            get
            {
                return DataCriacaoCarga.ToDateTimeString();
            }
        }

        public string DataFinalizacaoEmissaoFormatada
        {
            get
            {
                return (DataFinalizacaoEmissao != DateTime.MinValue) ? DataFinalizacaoEmissao.ToDateTimeString() : "";
            }
        }

        public string PesoFormatado
        {
            get
            {
                return Peso.ToString("n2");
            }
        }

        public string Veiculo
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PlacaTracao) && !string.IsNullOrWhiteSpace(PlacaReboques))
                    return $"{PlacaTracao}, {PlacaReboques}";

                if (!string.IsNullOrWhiteSpace(PlacaTracao))
                    return PlacaTracao;

                return PlacaReboques;
            }
        }

        #endregion Propriedades com Regras
    }
}
