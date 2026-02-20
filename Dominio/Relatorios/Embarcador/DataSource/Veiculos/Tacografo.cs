using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public class Tacografo
    {
        public int Codigo { get; set; }
        public string Placa { get; set; }
        public string Motorista { get; set; }
        public int Situacao { get; set; }
        public DateTime DataRepasse { get; set; }
        public DateTime DataRetorno { get; set; }
        public bool HouveExcessoVelocidade { get; set; }
        public string Observacao { get; set; }

        public string HouveExcessoVelocidadeFormatada
        {
            get { return this.HouveExcessoVelocidade == true ? "Sim" : "Não"; }
        }

        public string DataRepasseFormatada
        {
            get { return DataRepasse != DateTime.MinValue ? DataRepasse.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataRetornoFormatada
        {
            get { return DataRetorno != DateTime.MinValue ? DataRetorno.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public virtual string SituacaoFormatada
        {
            get
            {
                switch (this.Situacao)
                {
                    case 1:
                        return "Entregue";
                    case 2:
                        return "Recebido";
                    case 3:
                        return "Perdido";
                    case 4:
                        return "Extraviado";
                    default:
                        return "";
                }
            }
        }
    }
}
