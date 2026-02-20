using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class LancamentoContabilDetalhe
    {
        public string Descricao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }

        public string TipoContaContabilDescricao
        {
            get
            {
                return TipoContaContabil.ObterDescricao();
            }
        }

        public string ContaContabil { get; set; }

        public string CentroCusto { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }

        public string TipoContabilizacaoDescricao
        {
            get
            {
                return TipoContabilizacao.ObterDescricao();
            }
        }

        public decimal Valor { get; set; }

        public string ValorDescricao
        {
            get
            {
                return Valor.ToString("n2");
            }
        }

        public string DataLancamento { get; set; }

    }
}
