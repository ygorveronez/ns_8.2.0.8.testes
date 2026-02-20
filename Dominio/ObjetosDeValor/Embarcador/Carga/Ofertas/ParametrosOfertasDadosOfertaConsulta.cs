namespace Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas
{
    public class ParametrosOfertasDadosOfertaConsulta : Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta
    {
        public Enumeradores.DiaSemana[] DiasSemana { get; set; }
        public Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta ComoEntidadeProtese()
        {
            return new Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta()
            {
                Codigo = Codigo,
                HoraInicio = HoraInicio,
                HoraTermino = HoraTermino,
                Raio = Raio,
            };
        }
    }
}
