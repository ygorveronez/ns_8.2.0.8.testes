using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public class FiltroPesquisaRelatorioAceiteContrato
    {
        public int CodigoTransportador { get; set; }
        public SituacaoAceiteContrato Situacao { get; set; }
        public int CodigoContratoNotaFiscal { get; set; }
    }
}
