using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe.ComponenteFreteCTe
{
    public class ComponenteFreteCTe
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string ComponenteFrete { get; set; }
        public decimal ValorComponenteFrete { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public string ModeloDocumento { get; set; }
        public string Empresa { get; set; }
        public string GrupoPessoas { get; set; }
        public Dominio.Enumeradores.TipoPessoa TipoTomador { get; set; }
        public string ModeloVeicularCarga { get; set; }
        public string NumeroFrotasVeiculos { get; set; }
        public decimal Peso { get; set; }
        private DateTime DataEmissaoCTe { get; set; }
        public string DestinatarioCTe { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataEmissaoCTeFormatada
        {
            get { return DataEmissaoCTe != DateTime.MinValue ? DataEmissaoCTe.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion
    }
}
