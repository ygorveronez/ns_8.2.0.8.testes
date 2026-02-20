using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class AbastecimentoFechamentoAbastecimento
    {
        public int Codigo { get; set; }
        public string Placa { get; set; }
        public string Posto { get; set; }
        public string Equipamento { get; set; }
        public DateTime Data { get; set; }
        public decimal KM { get; set; }
        public int Horimetro { get; set; }
        public decimal Litros { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public int ContagemDuplicado { get; set; }
        public decimal KMTotal { get; set; }
        public decimal Media { get; set; }
        public int HorimetroTotal { get; set; }
        public decimal MediaHorimetro { get; set; }
        public TipoAbastecimento TipoAbastecimento { get; set; }

        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string TipoAbastecimentoFormatado
        {
            get { return TipoAbastecimento.ObterDescricao(); }
        }
    }
}
