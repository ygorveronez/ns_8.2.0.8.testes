using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.WMS
{
    public class FiltroPesquisaMontagemContainer
    {
        public string NumeroBooking { get; set; }
        public int TipoContainer { get; set; }
        public int Container { get; set; }
        public int IdMontagemContainer { get; set; }
        public StatusMontagemContainer Status {get;set;}
    }
}