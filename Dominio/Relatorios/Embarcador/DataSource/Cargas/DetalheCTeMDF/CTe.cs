namespace Dominio.Relatorios.Embarcador.DataSource.Cargas
{
    public class CTe
    {
        #region Propriedades

        public string TipoDocumento { get; set; }

        public int SerieCTe { get; set; }

        public int NumeroCTe { get; set; }

        public string ChaveCTe { get; set; }

        public string EstadoCTe { get; set; }

        public string RemessaCTe { get; set; }

        public string CodigoIntegracaoClienteDestinatarioCTe { get; set; }

        public string NomeClienteDestinatarioCTe { get; set; }

        #endregion Propriedades

        #region Propriedades Com Regras

        public string ClienteCTe
        {
            get
            {
                if (string.IsNullOrEmpty(CodigoIntegracaoClienteDestinatarioCTe))
                    return NomeClienteDestinatarioCTe;

                return CodigoIntegracaoClienteDestinatarioCTe;
            }
        }

        #endregion Propriedades Com Regras
    }
}
