using System;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class Usuario
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string Nome { get; set; }

        public string Cpf { get; set; }

        public string Telefone { get; set; }

        public string Email { get; set; }

        public Dominio.Enumeradores.TipoAcesso TipoAcesso { get; set; }

        public bool MotoristaEstrangeiro { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string CpfCnpjFormatado
        {
            get
            {
                return Cpf.ObterSomenteNumeros().ObterCpfOuCnpjFormatado();
            }
        }

        public virtual string Descricao
        {
            get
            {
                if (this.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador)
                    return $"{Nome}";

                return $"{Nome} ({CpfCnpjFormatado})";
            }
        }

        public virtual string DescricaoTelefoneEmail
        {
            get
            {
                StringBuilder descricao = new StringBuilder(Nome);

                if (!string.IsNullOrWhiteSpace(Telefone))
                    descricao.Append($" ({Telefone})");

                if (!string.IsNullOrWhiteSpace(Email))
                    descricao.Append($" - {Email}");

                return descricao.ToString();
            }
        }

        #endregion Propriedades com Regras
    }
}
