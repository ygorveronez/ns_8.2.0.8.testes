namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class RevisaoSMDadosEngate
    {
        public int? CodFilial { get; set; }
        public string PlacaVeiculo { get; set; }
        /// <summary>
        /// Vinculo do veículo (A=agregado, F=frota, T=terceiro) 
        /// </summary>
        public string VincVeiculo { get; set; }
        public int? CodPerfilSeguranca { get; set; }
        public string CPFMotorista1 { get; set; }
        /// <summary>
        /// Vinculo do 1º motorista (A=agregado, F=funcionário, T=autônomo) 
        /// </summary>
        public string VincMotorista1 { get; set; }
        public string CPFMotorista2 { get; set; }
        /// <summary>
        /// Vinculo do 2º motorista (A, F, T)
        /// </summary>
        public string VincMotorista2 { get; set; }
        public string CPFAjudante { get; set; }
        public string VincAjudante { get; set; }
        public string PlacaCarreta1 { get; set; }
        public string VincCarreta1 { get; set; }
        public string PlacaCarreta2 { get; set; }
        public string VincCarreta2 { get; set; }
        public string PlacaCarreta3 { get; set; }
        public string VincCarreta3 { get; set; }
        public int? CodFaixaTemperatura { get; set; }
        public string CNPJTransportador { get; set; }
    }
}
