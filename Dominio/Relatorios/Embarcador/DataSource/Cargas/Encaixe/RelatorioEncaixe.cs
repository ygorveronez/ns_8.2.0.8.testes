using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Encaixe
{
    public class RelatorioEncaixe
    {
        public int Codigo { get; set; }
        public string CargaDeEncaixe { get; set; }
        public string Transportador { get; set; }
        public string Veiculo { get; set; }
        public string Motoristas { get; set; }
        public string CargaEncaixada { get; set; }
        public string PedidoEncaixado { get; set; }
        public string CTesDoEncaixe { get; set; }
        public string CTesEncaixados { get; set; }
        public string NotasEncaixadas { get; set; }
        public string CodigoClienteEncaixado { get; set; }
        public double _CNPJClienteEncaixado { get; set; }
        public string TipoCliente { get; set; }
        public string CNPJClienteEncaixado {
            get
            {
                if (this._CNPJClienteEncaixado == 0 || string.IsNullOrWhiteSpace(this.TipoCliente)) return "";
                if (this.TipoCliente.Equals("E")) return "00.000.000/0000-00";

                return this.TipoCliente.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this._CNPJClienteEncaixado) : String.Format(@"{0:000\.000\.000\-00}", this._CNPJClienteEncaixado);
            }
            set
            {
                double.TryParse(value, out double cnpj);
                this._CNPJClienteEncaixado = cnpj;
            }
        }
        public string ClienteEncaixado { get; set; }
        public string LocalidadeClienteEncaixado { get; set; }
        public decimal ValorPrestacaoEncaixe { get; set; }
    }
}
