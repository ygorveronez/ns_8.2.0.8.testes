namespace Dominio.ObjetosDeValor
{
    public class ConfiguracaoFTP
    {
        public int Id { get; set; }

        public double Cliente { get; set; }

        public string DescricaoCliente { get; set; }

        public string LayoutEDI { get; set; }

        public string DescricaoLayoutEDI { get; set; }

        public Enumeradores.TipoArquivoFTP Tipo { get; set; }

        public string DescricaoTipo { get; set; }

        public Enumeradores.TipoProcessamentoArquivoFTP TipoArquivo { get; set; }

        public string DescricaoTipoArquivo { get; set; }

        public Enumeradores.TipoRateioFTP Rateio { get; set; }

        public string DescricaoRateio { get; set; }

        public string Host { get; set; }

        public string Porta { get; set; }

        public string Usuario { get; set; }

        public string Senha { get; set; }

        public string Diretorio { get; set; }

        public bool Passivo { get; set; }

        public bool Seguro { get; set; }

        public bool SSL { get; set; }

        public bool GerarNFSe { get; set; }

        public bool EmitirDocumento { get; set; }

        public bool UtilizarContratanteComoTomador { get; set; }

        public bool Excluir { get; set; }
    }
}
