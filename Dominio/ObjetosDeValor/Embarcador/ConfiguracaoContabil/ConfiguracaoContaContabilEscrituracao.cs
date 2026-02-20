namespace Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoContaContabilEscrituracao
    {
        public int Codigo { get; set; }
        public int CodigoConfiguracaoContaContabil { get; set; }
        public string ConfiguracaoContaContabilDescricao { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }
        public bool SempreGerarRegistro { get; set; }        
    }
}
