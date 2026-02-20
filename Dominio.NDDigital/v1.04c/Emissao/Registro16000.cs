using System;
using System.Collections.Generic;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Dados específicos do transporte rodoviário
    /// </summary>
    public class Registro16000 : Registro
    {
        #region Construtores

        public Registro16000(string registro)
            : base(registro)
        {
            this.GerarRegistro();

            this.veic = new List<Registro16400>();
            this.moto = new List<Registro16600>();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Registro Nacional de Transportadores Rodoviários de Carga
        /// </summary>
        public string RNTRC { get; set; }

        /// <summary>
        /// Data prevista da entrega
        /// </summary>
        public DateTime dPrev { get; set; }

        /// <summary>
        /// 0 - Não;
        /// 1 - Sim;
        /// Obs. Será lotação quando houver apenas 1 tomador do serviço por veículo, ou combinação veicular, e por viagem
        /// </summary>
        public int lota { get; set; }

        /// <summary>
        /// Código Identificador da Operação de Transporte
        /// </summary>
        public string CIOT { get; set; }

        public Registro16200 occ { get; set; }

        public Registro16300 valePed { get; set; }

        public List<Registro16400> veic { get; set; }

        public Registro16500 lacRodo { get; set; }

        public List<Registro16600> moto { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.RNTRC = this.ObterString(dados[1]);
            this.dPrev = this.ObterData(dados[2]);
            this.lota = this.ObterNumero(dados[3]);
            this.CIOT = this.ObterString(dados[4]);
        }

        #endregion
    }
}
