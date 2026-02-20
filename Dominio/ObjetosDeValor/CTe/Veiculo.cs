namespace Dominio.ObjetosDeValor.CTe
{
    public class Veiculo
    {
        public int CodigoVeiculo { get; set; }

        public string UF { get; set; }
        
        public string Placa { get; set; }

        public string Chassi { get; set; }

        public int CapacidadeM3 { get; set; }

        public string Renavam { get; set; }

        public int Tara { get; set; }

        /// <summary>
        /// P - PROPRIO
        /// T - TERCEIRO
        /// </summary>
        public string TipoPropriedade { get; set; }

        /// <summary>
        /// 0 - TRACAO
        /// 1 - REBOQUE
        /// </summary>
        public string TipoVeiculo { get; set; }

        /// <summary>
        /// 00 - NAO APLICADO
        /// 01 - TRUCK
        /// 02 - TOCO
        /// 03 - CAVALO
        /// 04 - VAN
        /// 05 - UTILITARIO
        /// 06 - OUTROS
        /// </summary>
        public string TipoRodado { get; set; }

        /// <summary>
        /// 00 - NAO APLICADO
        /// 01 - ABERTA
        /// 02 - FECHADA / BAU
        /// 03 - GRANEL
        /// 04 - PORTA CONTAINER
        /// 05 - SIDER
        /// </summary>
        public string TipoCarroceria { get; set; }

        public int CapacidadeKG { get; set; }

        public int RNTRCProprietario { get; set; }

        public Enumeradores.TipoProprietarioVeiculo TipoProprietario { get; set; }

        public Cliente Proprietario { get; set; }

        public Cliente Locador { get; set; }
    }
}
