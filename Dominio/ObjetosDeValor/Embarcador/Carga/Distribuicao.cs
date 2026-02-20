namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class Distribuicao
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa TransportadorDistribuidor { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao TipoOperacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }
    }
}
