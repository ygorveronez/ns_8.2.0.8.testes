using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class OrdemDeCompra
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public int Numero { get; set; }
        public string Servico { get; set; }
        public string Solicitante { get; set; }
        public string Setor { get; set; }
        public string Veiculo { get; set; }
        public int KilometragemVeiculo { get; set; }
        public string ModeloVeiculo { get; set; }
        public string Descricao { get; set; }
        public string Fornecedor { get; set; }
    }
}
