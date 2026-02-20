using System;
using System.Text;

namespace Dominio.NDDigital.v104.Emissao
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

        public string CST { get; set; }

        public decimal ValorAPagar { get; set; }

        public decimal ValorBaseCalculoICMS { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal PercentualReducaoBaseCalculo { get; set; }

        public decimal ValorReducaoBaseCalculo { get; set; }

        public decimal AliquotaICMS { get; set; }

        public string CFOP { get; set; }

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

            this.Registro.AppendLine();

            this.Registro.Append("IMPOSTOS");
            this.Registro.Append(";");
            this.Registro.Append(string.Format("{0:000}", string.IsNullOrWhiteSpace(this.CST) ? 0 : int.Parse(this.CST)));
            this.Registro.Append(";");
            this.Registro.Append(string.Format(cultura, "{0:0000000000.00}", this.ValorAPagar));
            this.Registro.Append(";");
            this.Registro.Append(string.Format(cultura, "{0:0000000000.00}", this.ValorBaseCalculoICMS));
            this.Registro.Append(";");
            this.Registro.Append(string.Format(cultura, "{0:0000000000.00}", this.ValorICMS));
            this.Registro.Append(";");
            this.Registro.Append(string.Format(cultura, "{0:000.00}", this.PercentualReducaoBaseCalculo));
            this.Registro.Append(";");
            this.Registro.Append(string.Format(cultura, "{0:0000000000.00}", this.ValorReducaoBaseCalculo));
            this.Registro.Append(";");
            this.Registro.Append(string.Format(cultura, "{0:0000000000.00}", this.AliquotaICMS));
            this.Registro.Append(";");
            this.Registro.Append(this.CFOP);
            this.Registro.Append(";");

            return this.Registro.ToString();
        }

        #endregion
    }
}
