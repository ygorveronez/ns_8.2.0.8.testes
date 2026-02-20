using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class LocalRetiradaContainer
    {
        #region Propriedades

        public string LocalCodigoIntegracao { get; set; }
        public double LocalCpfCnpj { get; set; }
        public string LocalNome { get; set; }
        public string LocalNomeFantasia { get; set; }
        public bool LocalPontoTransbordo { get; set; }
        public string LocalTipo { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public int QuantidadeReservada { get; set; }
        public string TipoContainer { get; set; }
        public int CodigoTipoContainer { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public double Codigo
        {
            get
            {
                return LocalCpfCnpj;
            }
        }

        public string Descricao
        {
            get
            {
                string descricao = "";
                string nome = LocalNome;

                if (LocalPontoTransbordo)
                    nome = LocalNomeFantasia;

                if (!string.IsNullOrWhiteSpace(LocalCodigoIntegracao))
                    descricao += LocalCodigoIntegracao + " - ";

                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;

                if (!string.IsNullOrWhiteSpace(LocalTipo))
                    descricao += $" ({LocalCpfCnpj.ToString().ObterCpfOuCnpjFormatado(LocalTipo)})";

                return descricao;
            }
        }

        public string QuantidadeFormatada
        {
            get
            {
                return Quantidade.ToString("n0");
            }
        }

        public string QuantidadeDisponivelFormatada
        {
            get
            {
                return QuantidadeDisponivel.ToString("n0");
            }
        }

        public string QuantidadeReservadaFormatada
        {
            get
            {
                return QuantidadeReservada.ToString("n0");
            }
        }

        #endregion Propriedades com Regras
    }
}
