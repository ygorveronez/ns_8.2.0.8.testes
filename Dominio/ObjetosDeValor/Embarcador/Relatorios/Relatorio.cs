namespace Dominio.ObjetosDeValor.Embarcador.Relatorios
{
    public class Relatorio
    {
        public int Codigo { get; set; }
        public Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorios { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public bool Padrao { get; set; }
        public bool ExibirSumarios { get; set; }
        public bool CortarLinhas { get; set; }
        public bool FundoListrado { get; set; }
        public int TamanhoPadraoFonte { get; set; }
        public string FontePadrao { get; set; }
        public bool AgruparRelatorio { get; set; }
        public string PropriedadeAgrupa { get; set; }
        public string OrdemAgrupamento { get; set; }
        public string PropriedadeOrdena { get; set; }
        public string OrdemOrdenacao { get; set; }
        public Dominio.Enumeradores.TipoArquivoRelatorio TipoArquivoRelatorio { get; set; }
        public Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio OrientacaoRelatorio { get; set; }
        public dynamic Grid { get; set; }
        public dynamic Report { get; set; }
        public bool NovoRelatorio { get; set; }
        public bool OcultarDetalhe { get; set; }
        public bool RelatorioParaTodosUsuarios { get; set; }
        public bool NovaPaginaAposAgrupamento { get; set; }
    }
}
