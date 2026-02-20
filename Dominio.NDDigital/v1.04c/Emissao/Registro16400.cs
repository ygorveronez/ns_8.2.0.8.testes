namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações sobre o veículo (só preenchido em caso de CT-e rodoviário de lotação)
    /// </summary>
    public class Registro16400 : Registro
    {
        #region Construtores

        public Registro16400(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código interno do veículo
        /// </summary>
        public string cInt { get; set; }

        /// <summary>
        /// RENAVAM do veículo
        /// </summary>
        public string RENAVAM { get; set; }

        public string placa { get; set; }

        /// <summary>
        /// Tara em KG
        /// </summary>
        public int tara { get; set; }

        /// <summary>
        /// Capacidade KG
        /// </summary>
        public int capKG { get; set; }

        /// <summary>
        /// Capacidade M3
        /// </summary>
        public int capM3 { get; set; }

        /// <summary>
        /// Tipo de propriedade do veículo: P-Próprio ou T-Terceiro
        /// </summary>
        public string tpProp { get; set; }

        /// <summary>
        /// Tipo de veículo
        /// </summary>
        public string tpVeic { get; set; }

        /// <summary>
        /// Tipo de Rodado: 00 - Não Aplicável , 01 - Truck, 02 - Toco, 03 - Cavalo Mecânico, 04 - VAN, 05 - Utilitário, 06 - Outros
        /// </summary>
        public string tpRod { get; set; }

        /// <summary>
        /// Tipo de Carroceria: 00 - não aplicável , 01 - Aberta, 02 - Fechada/Baú, 03 - Granelera, 04 - Porta Container, 05 - Sider
        /// </summary>
        public string tpCar { get; set; }

        /// <summary>
        /// UF em que veículo está licenciado
        /// </summary>
        public string UF { get; set; }

        public Registro16410 prop { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.cInt = this.ObterString(dados[1]);
            this.RENAVAM = this.ObterString(dados[2]);
            this.placa = this.ObterString(dados[3]);
            this.tara = this.ObterNumero(dados[4]);
            this.capKG = this.ObterNumero(dados[5]);
            this.capM3 = this.ObterNumero(dados[6]);
            this.tpProp = this.ObterString(dados[7]);
            this.tpVeic = this.ObterString(dados[8]);
            this.tpRod = this.ObterString(dados[9]);
            this.tpCar = this.ObterString(dados[10]);
            this.UF = this.ObterString(dados[11]);
        }

        #endregion
    }
}
