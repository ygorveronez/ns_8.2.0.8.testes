using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FiltroPesquisaImportacaoTabelaFrete
    {
        public int CodigoUsuario { get; set; }
        public string NomeArquivo { get; set; }
        public DateTime? DataImportacaoInicial { get; set; }
        public DateTime? DataImportacaoFinal { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete? Situacao { get; set; }
        public int CodigoTabelaFrete { get; set; }
        public string Mensagem { get; set; }
    }
}
