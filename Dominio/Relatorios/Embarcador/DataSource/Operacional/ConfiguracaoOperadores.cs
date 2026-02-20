namespace Dominio.Relatorios.Embarcador.DataSource.Operacional
{
    public sealed class ConfiguracaoOperadores
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Usuario { get; set; }
        public string Filial { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoCarga { get; set; }
        public string CentroCarregamento { get; set; }
        public string CentroDescarregamento { get; set; }
        public string GrupoPessoas { get; set; }
        public string Cliente { get; set; }
        public string FilialVenda { get; set; }
        public string TomadorGestaoDocumento { get; set; }
        public string Transportador { get; set; }
        public string Recebedor { get; set; }

        #endregion

    }
}
