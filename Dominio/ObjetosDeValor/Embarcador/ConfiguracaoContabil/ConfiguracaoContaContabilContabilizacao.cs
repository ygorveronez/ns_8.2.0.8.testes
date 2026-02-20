namespace Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoContaContabilContabilizacao
    {
        public int Codigo { get; set; }
        public int CodigoConfiguracaoContaContabil { get; set; }
        public string ConfiguracaoContaContabilDescricao { get; set; }
        public int CodigoPlanoConta { get; set; }
        public string PlanoContaDescricao { get; set; }
        public string PlanoContabilidade { get; set; }
        public int CodigoPlanoContaContraPartidaProvisao { get; set; }
        public Enumeradores.TipoContaContabil TipoContaContabil { get; set; }
        public Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }
        public bool ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao { get; set; }
    }
}
