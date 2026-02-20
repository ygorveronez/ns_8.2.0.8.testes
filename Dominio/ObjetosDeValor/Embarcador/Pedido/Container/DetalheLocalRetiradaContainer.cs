namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class DetalheLocalRetiradaContainer
    {
        #region Propriedades
        public int Codigo { get; set; }
        public int CodigoContainerTipo { get; set; }
        public double CpfCnpjLocal { get; set; }
        public string CargaEmbarcador { get; set; }
        public string ContainerTipo { get; set; }
        public int QuantidadeReservada { get; set; }
        public string NumeroEXP { get; set; }

        #endregion Propriedades

    }
}
