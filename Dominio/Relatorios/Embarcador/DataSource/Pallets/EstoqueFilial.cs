using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class EstoqueFilial
    {
        public int Codigo { get; set; }
        public string Filial { get; set; }
        public string FilialCnpj { get; set; }
        public string FilialCodigoIntegracao { get; set; }
        public DateTime Data { get; set; }
        public string TipoLancamento { get; set; }
        public int Entrada { get; set; }
        public int Saida { get; set; }
        public int Descarte { get; set; }
        public int SaldoTotal { get; set; }
        public string Observacao { get; set; }
    }
}
