using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class Pedagio
    {
        public int Codigo { get; set; }
        public string Veiculo { get; set; }
        public DateTime DataPassagem { get; set; }
        public string Praca { get; set; }
        public string Rodovia { get; set; }
        public decimal Valor { get; set; }
        public SituacaoPedagio Situacao { get; set; }
        public TipoPedagio TipoPedagio { get; set; }
        public string Observacao { get; set; }

        #region Propriedades com Regras

        public string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public string DescricaoTipoPedagio
        {
            get { return TipoPedagio.ObterDescricao(); }
        }

        #endregion
    }
}
