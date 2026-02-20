namespace Dominio.ObjetosDeValor.Embarcador.Configuracoes
{
    public class ConfiguracaoModeloEmail
    {
        public string Descricao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloEmail? Tipo { get; set; }
        public bool? Ativo { get; set; }
    }
}
