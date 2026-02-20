using System;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class Cliente
    {
        #region Propriedades

        public double Codigo { get; set; }

        public string CodigoIntegracao { get; set; }

        public string CpfCnpj { get; set; }

        public string Nome { get; set; }

        public string NomeFantasia { get; set; }

        public bool PontoTransbordo { get; set; }

        public string Observacao { get; set; }

        public string Tipo { get; set; }

        public bool ValidarValorMinimoMercadoriaEntregaMontagemCarregamento { get; set; }

        public decimal? ValorMinimoCarga { get; set; }

        public int? DiasDePrazoFatura { get; set; }

        public Endereco Endereco { get; set; }

        public GrupoPessoas GrupoPessoas { get; set; }

        public CategoriaPessoa Categoria { get; set; }

        public Regiao Regiao { get; set; }

        public MesoRegiao MesoRegiao { get; set; }

        public TipoPagamentoRecebimento FormaPagamento { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string CpfCnpjFormatado
        {
            get
            {
                return CpfCnpj.ObterCpfOuCnpjFormatado();
            }
        }

        public string Descricao
        {
            get
            {
                StringBuilder descricao = new StringBuilder();
                string nome = PontoTransbordo ? NomeFantasia : Nome;

                if (!string.IsNullOrWhiteSpace(CodigoIntegracao))
                    descricao.Append($"{CodigoIntegracao} - ");

                if (!string.IsNullOrWhiteSpace(nome))
                    descricao.Append(nome);

                if (!string.IsNullOrWhiteSpace(Tipo))
                    descricao.Append($" ({CpfCnpjFormatado})");

                return descricao.ToString();
            }
        }

        #endregion Propriedades com Regras
    }
}
