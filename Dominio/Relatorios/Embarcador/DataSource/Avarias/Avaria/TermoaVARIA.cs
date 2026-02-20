using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Avarias
{
    public class TermoAvaria
    {
        public string NomeEmpresa { get; set; }
        public int NumeroAvaria { get; set; }
        public string Transportador { get; set; }
        public string Filial { get; set; }
        public string TipoTransporte { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string MotivoAvaria { get; set; }

        public string Motorista { get; set; }
        public string RGMotorista { get; set; }
        public string CPFMotorista { get; set; }
        public string Placa { get; set; }
        public string CTe { get; set; }
        public DateTime DataEntrega { get; set; }
        public string NotasFiscais { get; set; }
        public string Viagens { get; set; }

        public decimal ValorAvaria { get; set; }
        public string ValorAvariaExtenso { get; set; }
    }
}
