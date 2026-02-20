using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.RH
{
    public class ComissaoFuncionarioMotoristaAbastecimento
    {
        public int CodigoComissaoFuncionarioMotorista { get; set; }
        public string Veiculo { get; set; }
        public string Posto { get; set; }
        public DateTime DataAbatecimento { get; set; }
        public decimal KM { get; set; }
        public int Acerto { get; set; }
        public decimal Litros { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento TipoAbastecimento { get; set; }
        public string Situacao { get; set; }
        public decimal Media { get; set; }
        public decimal ValorTotal { get; set; }

        public string DataAbatecimentoFormatada
        {
            get { return DataAbatecimento != DateTime.MinValue ? DataAbatecimento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public virtual string DescricaoTipoAbastecimento
        {
            get { return TipoAbastecimento.ObterDescricao(); }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case "A":
                        return "Aberto";
                    case "I":
                        return "Inconsistente";
                    case "F":
                        return "Fechado";
                    case "G":
                        return "Agrupado";
                    default:
                        return "";
                }
            }
        }
    }
}
