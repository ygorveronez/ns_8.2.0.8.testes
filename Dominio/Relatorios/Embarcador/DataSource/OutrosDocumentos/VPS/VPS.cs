using System;

namespace Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.VPS
{
    public class VPS
    {
        public DateTime Data { get; set; }

        public string Destino { get; set; }

        public int Numero { get; set; }

        public decimal Valor { get; set; }

        public string ValorPorExtenso { get; set; }

        public string Caminhao { get; set; }

        public string Motorista { get; set; }

        public string RGMotorista { get; set; }

        public string Placa { get; set; }

        public int NotaFiscal { get; set; }
    }
}
