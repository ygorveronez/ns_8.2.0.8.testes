using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class FreteTerceirizadoAcrescimoDesconto
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int ContratoFrete { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroCIOT { get; set; }
        private double CPFCNPJTerceiro { get; set; }
        private string TipoTerceiro { get; set; }
        public string Terceiro { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public decimal ValorDesconto { get; set; }
        private DateTime DataEmissao { get; set; }
        public string Veiculo { get; set; }
        public string Motorista { get; set; }
        public string Justificativa { get; set; }
        public string TaxaTerceiro { get; set; }
        public string NumeroDocAnterior { get; set; }
        public string TipoCarga { get; set; }
        public DateTime DataCarga { get; set; }



        #endregion

        #region Propriedades com Regras

        public string CPFCNPJTerceiroFormatado
        {
            get
            {
                if (this.TipoTerceiro == "E")
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return this.TipoTerceiro == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJTerceiro) : string.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJTerceiro);
                }
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        #endregion
    }
}
