namespace Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas
{
    public class ParametrosOfertasDadosOferta
    {
        public int Codigo { get; set; }
        public Enumeradores.DiaSemana[] DiasSemana { get; set; }
        public string HoraInicio { get; set; }
        public string HoraTermino { get; set; }
        public int Raio { get; set; }
    }
}
