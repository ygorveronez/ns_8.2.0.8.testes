using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public class Equipamento
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string Numero { get; set; }
        public string Chassi { get; set; }
        public int Hodometro { get; set; }
        public int Horimetro { get; set; }
        private DateTime DataAquisicao { get; set; }
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public string Situacao { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public string Segmento { get; set; }
        public string Observacao { get; set; }
        public string Placa { get; set; }
        public string CentroResultado { get; set; }
        private int Neokohm { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataAquisicaoFormatada
        {
            get { return DataAquisicao != DateTime.MinValue ? DataAquisicao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string NeokohmDescricao 
        { 
            get { return Neokohm == 1 ? "Sim" : "Não"; } 
        }

        #endregion
    }
}
