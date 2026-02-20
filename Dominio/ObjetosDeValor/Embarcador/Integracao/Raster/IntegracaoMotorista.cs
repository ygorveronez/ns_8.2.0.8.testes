namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class IntegracaoMotorista
    {
        public string Ambiente { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string TipoRetorno { get; set; }
        public IntegracaoMotoristaDados Motorista { get; set; }
    }
}
