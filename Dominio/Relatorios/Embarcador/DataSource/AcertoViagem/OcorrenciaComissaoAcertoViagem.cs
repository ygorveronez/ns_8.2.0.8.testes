using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class OcorrenciaComissaoAcertoViagem
    {        
        public int Numero { get; set; }
        public string NumeroAtuacao { get; set; }
        public string Motorista { get; set; }
        public string Veiculo { get; set; }
        public string TipoInfracao { get; set; }
        public decimal ValorInfracao { get; set; }
        public int Pontos { get; set; }
        public NivelInfracaoTransito Nivel { get; set; }
        public decimal ReducaoComissao { get; set; }
        public int CodigoMotorista { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataEmissao { get; set; }

        public string DescricaoNivel
        {
            get { return Nivel.ObterDescricao(); }
        }

        public string DataVencimentoFormatada
        {
            get { return DataVencimento != DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }
    }
}
