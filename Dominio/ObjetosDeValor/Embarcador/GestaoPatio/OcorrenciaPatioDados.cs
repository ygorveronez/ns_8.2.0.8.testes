using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class OcorrenciaPatioDados
    {
        public int CodigoCentroCarregamento { get; set; }

        public int CodigoTipo { get; set; }

        public int CodigoVeiculo { get; set; }

        public string Descricao { get; set; }

        public TipoLancamento TipoLancamento { get; set; }
    }
}
