namespace Dominio.ObjetosDeValor.Embarcador.RH
{
    public class ExportacaoComissaoMotorista
    {
        public string CPFMotorista { get; set; }
        public string NomeMotorista { get; set; }
        public decimal ValorFreteLiquido { get; set; }
        public string CodigoIntegracao { get; set; }
        public string ModeloVeicularCarga { get; set; }
        public string CodigoContabil { get; set; }
        public decimal ValorComissao { get; set; }
        public decimal ValorBonificacao { get; set; }
    }
}
