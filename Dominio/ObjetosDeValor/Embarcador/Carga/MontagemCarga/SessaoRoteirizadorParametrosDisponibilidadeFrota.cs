namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class SessaoRoteirizadorParametrosDisponibilidadeFrota
    {        
        /// <summary>
        /// Contem o código da disponibilidade de frota
        /// </summary>
        public int Codigo { get; set; }

        public int CodigoModeloVeicular { get; set; }

        public string DescricaoModeloVeicular { get; set; }

        public int CodigoTransportador { get; set; }

        public string DescricaoTransportador { get; set; }

        public int Quantidade { get; set; }
        
        public int QuantidadeUtilizar { get; set; }
    }
}
