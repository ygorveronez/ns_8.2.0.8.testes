namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class IntegracaoProprietario
    {
        public string Ambiente { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string TipoRetorno { get; set; }
        public IntegracaoProprietarioDados Proprietario { get; set; }
    }
}
