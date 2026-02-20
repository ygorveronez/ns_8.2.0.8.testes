using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Redespacho
{
    public class Redespacho
    {
        #region Propriedades

        public int Codigo { get; set; }

        public int NumeroRedespacho { get; set; }

        public string CargaRedespacho { get; set; }

        public string CargasUtilizadas { get; set; }

        public string Carga { get; set; }

        public DateTime DataRedespacho { get; set; }

        public string Expedidor { get; set; }
        private string ExpedidorNome { get; set; }
        private string ExpedidorNomeFantasia { get; set; }
        private string ExpedidorCodigoIntegracao { get; set; }
        private double ExpedidorCnpjCpf { get; set; }
        private bool ExpedidorPontoTransbordo { get; set; }
        private string ExpedidorTipoFisJur { get; set; }            // F - Física,  J - Jurídica, E - Exterior

        #endregion

        #region Propriedades Com Regras

        public string CargasUtilizadasFormatada
        {
            get { return !(string.IsNullOrEmpty(this.CargasUtilizadas)) ? this.CargasUtilizadas : Carga; }
        }

        public string DataRedespachoFormatada
        {
            get { return this.DataRedespacho != DateTime.MinValue ? this.DataRedespacho.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public virtual string ExpedidorFormatada
        {
            get
            {
                string expedidorFormatado = "";
                string nome = this.Expedidor;
                if (this.ExpedidorPontoTransbordo)
                    nome = this.ExpedidorNomeFantasia;

                if (!string.IsNullOrWhiteSpace(this.ExpedidorCodigoIntegracao))
                    expedidorFormatado += this.ExpedidorCodigoIntegracao + " - ";
                if (!string.IsNullOrWhiteSpace(this.Expedidor))
                    expedidorFormatado += nome;
                if (!string.IsNullOrWhiteSpace(this.ExpedidorTipoFisJur))
                    expedidorFormatado += " (" + this.ExpedidorCgcCpf_Formatado + ")";

                return expedidorFormatado;
            }
        }

        public virtual string ExpedidorCgcCpf_Formatado
        {
            get
            {
                if (this.ExpedidorTipoFisJur?.Equals("E") ?? false)
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return (this.ExpedidorTipoFisJur?.Equals("J") ?? false) ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.ExpedidorCnpjCpf) : String.Format(@"{0:000\.000\.000\-00}", this.ExpedidorCnpjCpf);
                }
            }
        }

        #endregion
    }
}
