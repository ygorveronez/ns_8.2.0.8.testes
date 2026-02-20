namespace Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoContaContabilProvisao
    {
        public int Codigo { get; set; }
        public int CodigoConfiguracaoContaContabil { get; set; }
        public string ConfiguracaoContaContabilDescricao { get; set; }
        public int CodigoPlanoConta { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }
        public bool? ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao { get; set; }
    }
}
