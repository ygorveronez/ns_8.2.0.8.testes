namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class FiltroPesquisaProcessamentoEDIFTP
    {
        public int CodigoGrupoPessoa { get; set; }

        public System.DateTime? DataFinal { get; set; }

        public System.DateTime? DataInicial { get; set; }

        public string NomeArquivo { get; set; }

        public Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP Situacao { get; set; }
    }
}
