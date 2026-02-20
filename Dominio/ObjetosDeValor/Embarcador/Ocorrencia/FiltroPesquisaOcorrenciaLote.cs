using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class FiltroPesquisaOcorrenciaLote
    {
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public SituacaoOcorrenciaLote Situacao { get; set; }
        public int CodigoTipoOcorrencia { get; set; }
    }
}
