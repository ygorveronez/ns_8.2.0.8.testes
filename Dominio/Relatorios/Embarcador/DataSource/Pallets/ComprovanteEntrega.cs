using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class ComprovanteEntrega
    {
        public string NomeEmpresa { get; set; }
        public int NumeroDevolucao { get; set; }
        public string CNPJEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string TelefoneEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string Filial { get; set; }
        public string Transportador { get; set; }
        public string Placa { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int QuantidadePallets { get; set; }
        public DateTime DataDevolucao { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
