using System;
using System.Text;

namespace Dominio.NDDigital.v104.Cancelamento
{
    public class Retorno
    {
        #region Propriedades

        private StringBuilder Registro { get; set; }

        /// <summary>
        /// Id do protocolo de status
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Identificação do ambiente: 1 - Produção, 2 - Homologação
        /// </summary>
        public int tpAmb { get; set; }

        /// <summary>
        /// Versão do aplicativo que processou o documento
        /// </summary>
        public string verAplic { get; set; }

        /// <summary>
        /// Chave de acesso do CT-e
        /// </summary>
        public string chCTe { get; set; }

        /// <summary>
        /// Data e hora de processamento
        /// </summary>
        public DateTime dhRecbto { get; set; }

        /// <summary>
        /// Número de protocolo de status do documento
        /// </summary>
        public string nProt { get; set; }

        /// <summary>
        /// Digest value do documento processado
        /// </summary>
        public string digVal { get; set; }

        /// <summary>
        /// Código do status da mensagem do documento
        /// </summary>
        public string cStat { get; set; }

        /// <summary>
        /// Descrição do status da mensagem do documento
        /// </summary>
        public string xMotivo { get; set; }

        /// <summary>
        /// Tipo de emissão do documento
        /// </summary>
        public int tpEmis { get; set; }

        /// <summary>
        /// Chave de acesso do CT-e. Só utilizado quando o cStat for 022
        /// </summary>
        public string chCTe2 { get; set; }

        #endregion

        #region Métodos

        public string GerarRegistro()
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            this.Registro = new StringBuilder();

            this.Registro.Append(this.id);
            this.Registro.Append(";");
            this.Registro.Append(this.tpAmb.ToString());
            this.Registro.Append(";");
            this.Registro.Append(this.verAplic);
            this.Registro.Append(";");
            this.Registro.Append(this.chCTe);
            this.Registro.Append(";");
            this.Registro.Append(this.dhRecbto.ToString("s"));
            this.Registro.Append(";");
            this.Registro.Append(this.nProt);
            this.Registro.Append(";");
            this.Registro.Append(this.digVal);
            this.Registro.Append(";");
            this.Registro.Append(this.cStat);
            this.Registro.Append(";");
            this.Registro.Append(this.xMotivo);
            this.Registro.Append(";");
            this.Registro.Append(this.tpEmis.ToString());
            this.Registro.Append(";");
            this.Registro.Append(this.chCTe2);
            this.Registro.Append(";");

            return this.Registro.ToString();
        }

        #endregion
    }
}
