using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.BoletimViagem
{
    public class BoletimViagem
    {
        public int CodigoCarga { get; set;}
        public string NumeroCarga { get; set; }
        public DateTime DataDaCarga { get; set; }
        public string Transportador { get; set; }
        public string Placa { get; set; }
        public string Motorista { get; set; }
        public string Lacre { get; set; }
        public string Remetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string Destinatarios { get; set; }
        public string EnderecoDestinatarios { get; set; }
        public string Observacao { get; set; }
        public int Carimbo { get; set; }
        public string CarimboDescricao { get; set; }
        public string FaixaDeTemperaturaDescricao { get; set; }
        public string TipoDeCarga { get; set; }
        public string NotasFiscais { get; set; }

    }
}
