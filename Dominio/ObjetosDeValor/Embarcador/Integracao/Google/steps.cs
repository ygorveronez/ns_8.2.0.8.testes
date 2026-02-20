namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Google
{
    public class steps
    {
        public descritivo distance { get; set; }
        public descritivo duration { get; set; }

        public location end_location { get; set; }

        string html_instructions { get; set; }

        public location start_location { get; set; }

        string travel_mode { get; set; }

        public polyline polyline { get; set;}
    }
}
