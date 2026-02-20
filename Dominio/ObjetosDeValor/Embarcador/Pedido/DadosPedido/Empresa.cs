using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class Empresa
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string CodigoEmpresa { get; set; }

        public string Cnpj { get; set; }

        public string RazaoSocial { get; set; }

        public bool RestringirLocaisCarregamentoAutorizadosMotoristas { get; set; }

        public Localidade Localidade { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public virtual string Descricao
        {
            get
            {
                StringBuilder descricao = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(CodigoEmpresa))
                    descricao.Append($"{CodigoEmpresa} ");

                if (!string.IsNullOrWhiteSpace(RazaoSocial))
                    descricao.Append(RazaoSocial);

                if (Localidade != null)
                    descricao.Append($" ({Localidade.DescricaoCidadeEstado})");

                return descricao.ToString();
            }
        }

        #endregion Propriedades com Regras
    }
}
