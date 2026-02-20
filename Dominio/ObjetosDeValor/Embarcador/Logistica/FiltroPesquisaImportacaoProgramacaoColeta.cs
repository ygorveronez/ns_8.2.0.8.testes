using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaImportacaoProgramacaoColeta
    {
        public int NumeroImportacao { get; set; }
        public double CnpjCpfDestino { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public int CodigoProduto { get; set; }
        public SituacaoImportacaoProgramacaoColeta Situacao { get; set; }

    }
}
