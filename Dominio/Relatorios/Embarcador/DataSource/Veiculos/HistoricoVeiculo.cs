using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public sealed class HistoricoVeiculo
    {
        public int Codigo { get; set; }
        public string Placa { get; set; }
        public string Situacao { get; set; }
        public DateTime Data { get; set; }
        public string Usuario { get; set; }
    }
}
