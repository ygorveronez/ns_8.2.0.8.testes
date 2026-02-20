using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Documentos de transporte anterior em papel
    /// </summary>
    public class Registro15321 : Registro
    {
        #region Construtores

        public Registro15321(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo de documento de transporte anterior
        /// 00-CTRC
        /// 01-CTAC
        /// 02-ACT
        /// 03 - NF Modelo 7
        /// 04 - NF Modelo 27
        /// 05-Conhecimento Aéreo Nacional
        /// 06-CTMC
        /// 07-ATRE
        /// 08-DTA (Despacho de Transito Aduaneiro)
        /// 09-Conhecimento Aéreo Internacional
        /// 10 – Conhecimento - Carta de Porte Internacional
        /// 11 – Conhecimento Avulso
        /// 12-TIF (Transporte Internacional Ferroviário)
        /// 99 - outros
        /// </summary>
        public int tpDoc { get; set; }

        public string serie { get; set; }

        public string subser { get; set; }

        /// <summary>
        /// Número
        /// </summary>
        public string nDoc { get; set; }

        /// <summary>
        /// Data da emissão
        /// </summary>
        public DateTime dEmi { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.tpDoc = this.ObterNumero(dados[1]);
            this.serie = this.ObterString(dados[2]);
            this.subser = this.ObterString(dados[3]);
            this.nDoc = this.ObterString(dados[4]);
            this.dEmi = this.ObterData(dados[5]);
        }

        #endregion
    }
}
