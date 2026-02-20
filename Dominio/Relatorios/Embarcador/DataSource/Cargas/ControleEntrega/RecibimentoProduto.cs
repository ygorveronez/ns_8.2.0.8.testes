namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega
{
    public class RecibimentoProduto
    {
        public int NumeroNf { get; set; }
        public string Produto { get; set; }
        public string Divergencia { get; set; }
        public string Condicao { get; set; }
        public decimal QtePecas { get; set; }
        public decimal QteTotal { get; set; }

        #region Propiedades Privadas
       
        #endregion

    }
}
