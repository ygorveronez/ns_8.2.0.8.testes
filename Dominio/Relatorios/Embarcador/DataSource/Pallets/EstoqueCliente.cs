using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public sealed class EstoqueCliente
    {
        public int Codigo { get; set; }

        public string Cliente { get; set; }

        public string ClienteCpfCnpj { get; set; }

        public string ClienteCodigoIntegracao { get; set; }

        public DateTime Data { get; set; }

        public string TipoLancamento { get; set; }

        public int Entrada { get; set; }

        public int Saida { get; set; }

        public int Descarte { get; set; }

        public int SaldoTotal { get; set; }

        public string Observacao { get; set; }

        public string GrupoPessoas { get; set; }
    }
}
