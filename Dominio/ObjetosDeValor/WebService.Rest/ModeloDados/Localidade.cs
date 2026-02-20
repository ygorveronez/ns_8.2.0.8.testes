namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Localidade
    {
        public string Descricao { get; set; }

        public int CodigoIbge { get; set; }

        public Regiao Regiao { get; set; }

        public Estado Estado { get; set; }
    }
}
