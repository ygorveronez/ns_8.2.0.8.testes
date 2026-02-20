using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoColeta
{
    public class AgendamentoColetaPalletPesquisa
    {
        public int Codigo { get; set; }

        public string NumeroCarga { get; set; }

        public int QuantidadePallets { get; set; }

        public DateTime DataOrdem { get; set; }

        public SituacaoAgendamentoColetaPallet Situacao { get; set; }

        public ResponsavelPallet Responsavel { get; set; }

        public int NumeroOrdem { get; set; }

        public string Filial { get; set; }

        public string Transportador { get; set; }

        public string Cliente { get; set; }

        public string Veiculo { get; set; }

        public string Solicitante { get; set; }

        public string Motorista { get; set; }

        #region Propriedades com Regras

        public string SituacaoFormatada
        {
            get { return Situacao.ObterDescricao(); }
        }

        public string ResponsavelPalletFormatado
        {
            get { return Responsavel.ObterDescricao(); }
        }

        #endregion Propriedades com Regras
    }
}
