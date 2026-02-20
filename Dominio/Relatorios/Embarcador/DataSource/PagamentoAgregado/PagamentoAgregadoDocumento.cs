using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado
{
    public class PagamentoAgregadoDocumento
    {
        public string Cargas { get; set; }
        public string Cidade { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Destinatario { get; set; }
        public string Estado { get; set; }
        public string Motoristas { get; set; }
        public int Numero { get; set; }
        public string Ocorrencias { get; set; }
        public decimal Valor { get; set; }
        public string Veiculos { get; set; }
        public string DatasCargas { get; set; }
        public string DatasPedidos { get; set; }
        public string NumerosPedidos { get; set; }
    }
}
