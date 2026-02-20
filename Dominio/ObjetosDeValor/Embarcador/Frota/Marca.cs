namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class Marca
    {
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo TipoVeiculo { get; set; }
        public bool Ativo { get; set; }
    }
}
