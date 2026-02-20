namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class DadosRecebedor
    {
        public string Nome { get; set; }
        public string CPF { get; set; }
        public long DataEntrega { get; set; }
        public decimal? PercentualCompatibilidadeFoto { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor ConverterParaDadosRecebedorAntigo()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor
            {
                Nome = this.Nome,
                CPF = this.CPF,
                DataEntrega = Utilidades.DateTime.FromUnixSeconds(this.DataEntrega)?.ToString("ddMMyyyyHHmmss"),
                PercentualCompatibilidadeFoto = this.PercentualCompatibilidadeFoto,
            };
        }
    }

    
}
