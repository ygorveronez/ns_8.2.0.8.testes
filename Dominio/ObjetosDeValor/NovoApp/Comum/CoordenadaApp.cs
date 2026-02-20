namespace Dominio.ObjetosDeValor.NovoApp.Comum
{
    /// <summary>
    /// Essa Coordenada é um modelo que o módulo de GPS do app cria automaticamente. Vai ser usada exclusicamente em /AtualizarDadosPosicionamento
    /// </summary>
    public class CoordenadaApp
    {
        public string timestamp { get; set; }
        public Coords coords { get; set; }
        public Battery battery { get; set; }
    }

    public class Coords
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double speed { get; set; }
    }

    public class Battery
    {
        public bool is_charging{ get; set; }
        public decimal level { get; set; }
    }
}
