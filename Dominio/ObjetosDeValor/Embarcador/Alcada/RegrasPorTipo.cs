namespace Dominio.ObjetosDeValor.Embarcador.Alcada
{
    public class RegrasPorTipo
    {
        public dynamic Codigo { get; set; }

        public int Ordem { get; set; }

        public Enumeradores.CondicaoAutorizao Condicao { get; set; }

        public Enumeradores.JuncaoAutorizao Juncao { get; set; }

        public Entidade Entidade { get; set; }

        public dynamic Valor { get; set; }
    }
}
