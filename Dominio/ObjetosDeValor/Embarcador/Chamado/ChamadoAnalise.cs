using System;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public class ChamadoAnalise
    {
        public int Codigo { get; set; }
        public int CodigoChamado { get; set; }
        public DateTime DataCriacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao Estadia { get; set; }
    }
}
