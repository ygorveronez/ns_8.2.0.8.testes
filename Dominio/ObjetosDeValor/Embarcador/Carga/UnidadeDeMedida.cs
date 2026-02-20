namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class UnidadeDeMedida
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public Dominio.Enumeradores.UnidadeMedida UnidadeMedida { get; set; }
    }
}
