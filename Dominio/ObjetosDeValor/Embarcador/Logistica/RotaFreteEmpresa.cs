namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class RotaFreteEmpresa
    {
        public Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete ConfiguracaoRotaFrete { get; set; }

        public string Descricao { get; set; }

        public Entidades.Empresa Empresa { get; set; }

        public decimal PercentualCargasDaRota { get; set; }

        public int Prioridade { get; set; }

        public Entidades.RotaFrete RotaFrete { get; set; }
    }
}
