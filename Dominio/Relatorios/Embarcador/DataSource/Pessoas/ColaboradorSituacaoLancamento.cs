using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pessoas
{
    public class ColaboradorSituacaoLancamento
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public DateTime DataLancamento { get; set; }
        public string Operador { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string Colaborador { get; set; }
        public string Situacao { get; set; }
        public string ColaboradorSituacao { get; set; }
        public string Cargo { get; set; }
        public int CodigoContabil { get; set; }
        private Cores Cor { get; set; }
        public string FrotaVinculada { get; set; }

        #region Propriedades com Regras
        public string DT_RowColor
        {
            get { return Cor.Descricao(); }
        }

        public string DT_FontColor
        {
            get
            {
                if (Cor == Cores.Cinza)
                    return Cores.Branco.Descricao();
                else if (Cor == Cores.Laranja)
                    return Cores.Branco.Descricao();
                else if (Cor == Cores.Verde)
                    return Cores.Cinza.Descricao();
                else if (Cor == Cores.VerdeEscuro)
                    return Cores.Branco.Descricao();
                else if (Cor == Cores.Vermelho)
                    return Cores.Branco.Descricao();
                else
                    return string.Empty;
            }
        }
        #endregion

    }
}
