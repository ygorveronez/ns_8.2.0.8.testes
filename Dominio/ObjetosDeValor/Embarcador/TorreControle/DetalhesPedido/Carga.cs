namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido
{
    public class Carga
    {
        public int Codigo { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public bool? CargaCritica { get; set; }

        public string CargaCriticaDescricao
        {
            get
            {
                return CargaCritica.HasValue && CargaCritica.Value ? "Sim" : "-";
            }
        }
    }
}
