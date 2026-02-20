namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class SessaoRoteirizadorParametrosTempoCarregamento
    {
        /// <summary>
        /// Contem o código da disponibilidade de frota
        /// </summary>
        public int Codigo { get; set; }

        public int CodigoTipoCarga { get; set; }

        public string DescricaoTipoCarga { get; set; }

        public int CodigoModeloVeicular { get; set; }

        public string DescricaoModeloVeicular { get; set; }

        /// <summary>
        /// Representa a QuantidadeMaximaEntregasRoteirizar
        /// </summary>
        public int Quantidade { get; set; }
        /// <summary>
        /// Representa a QuantidadeMinimaEntregasRoteirizar
        /// </summary>
        public int QuantidadeMinima { get; set; }

        /// <summary>
        /// Representa a QuantidadeMaximaEntregasRoteirizar a ser utilizada na sessão
        /// </summary>
        public int QuantidadeUtilizar { get; set; }
        /// <summary>
        /// Representa a QuantidadeMinimaEntregasRoteirizar a ser utilizada na sessão
        /// </summary>
        public int QuantidadeMinimaUtilizar { get; set; }
    }
}
