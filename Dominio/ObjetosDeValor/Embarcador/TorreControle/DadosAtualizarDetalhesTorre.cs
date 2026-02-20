using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class DadosAtualizarDetalhesTorre
    {
        public int Codigo { get; set; }
        public bool Critico { get; set; }
        public string Observacao { get; set; }
        public int CodigoStatusViagem { get; set; }
        public DateTime DataInicioStatus { get; set; }
    }
}
