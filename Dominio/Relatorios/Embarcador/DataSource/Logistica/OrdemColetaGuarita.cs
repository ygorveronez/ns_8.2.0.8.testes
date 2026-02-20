using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class OrdemColetaGuarita
    {
        public string Numero { get; set; }
        public string TipoCarga { get; set; }
        public string ModeloVeiculo { get; set; }
        public DateTime DataCarregamento { get; set; }
        public decimal Peso { get; set; }
        public string TransportadorNome { get; set; }
        public string TransportadorCNPJ { get; set; }
        public string Veiculos { get; set; }
        public string Motoristas { get; set; }
        public string EmpresaNome { get; set; }
        public string EmpresaEndereco { get; set; }
        public string EmpresaCidade { get; set; }
        public string EmpresaEstado { get; set; }
        public string EmpresaCEP { get; set; }
        public string EmpresaTelefone { get; set; }
        public string EmpresaCNPJ { get; set; }
        public string EmpresaIE { get; set; }
        public string EmpresaEmail { get; set; }
        public string Observacao { get; set; }
        public string TipoOperacao { get; set; }
    }
}
