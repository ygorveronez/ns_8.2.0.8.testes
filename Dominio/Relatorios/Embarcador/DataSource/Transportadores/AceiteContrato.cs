using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Transportadores
{
    public class AceiteContrato
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string CPFCNPJ { get; set; }
        private string Tipo { get; set; }
        public string Razao { get; set; }
        public string Cidade { get; set; }
        public bool Aceite { get; set; }
        public string LogAceite { get; set; }
        public string NomeDoContrato { get; set; }
        private DateTime DataAceite { get; set; }



        #endregion

        #region Propriedades com Regras

        public string CPFCNPJFormatado
        {
            get { return string.IsNullOrWhiteSpace(CPFCNPJ) ? string.Empty : Tipo == "F" ? string.Format(@"{0:000\.000\.000\-00}", long.Parse(CPFCNPJ)) : string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(CPFCNPJ)); }
        }

        public string DescricaoAceite
        {
            get { return Aceite ? "Aceito" : "Pendente"; }
        }

        public string DataAceiteFormatada
        {
            get { return DataAceite != DateTime.MinValue ? DataAceite.ToDateTimeString() : string.Empty; }
        }

        #endregion
    }
}
