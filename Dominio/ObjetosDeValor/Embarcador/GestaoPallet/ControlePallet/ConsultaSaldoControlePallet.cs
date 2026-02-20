using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet
{
    public sealed class ConsultaSaldoControlePallet
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string Responsavel { get; set; }

        public string ResponsavelCNPJ { get; set; }

        public int PalettsPendente { get; set; }

        public int QuantidadeTotalPallets { get; set; }

        #endregion

        public string DescricaoResponsavel
        {
            get
            {
                string descricao = string.Empty;

                if (!string.IsNullOrWhiteSpace(Responsavel))
                    descricao += Responsavel;

                if (!string.IsNullOrWhiteSpace(ResponsavelCNPJ))
                    descricao += " (" + ResponsavelCNPJ.ObterCpfOuCnpjFormatado() + ")";

                return descricao;
            }
        }
    }
}
