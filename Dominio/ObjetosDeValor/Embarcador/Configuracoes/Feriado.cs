namespace Dominio.ObjetosDeValor.Embarcador.Configuracoes
{
    public class Feriado
    {
        public string Descricao { get; set; }
        public int Dia { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string CodigoIntegracao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado? Tipo { get; set; }
        public Dominio.ObjetosDeValor.Localidade Localidade { get; set; }
        public string SiglaUF { get; set; }
        public bool? Ativo { get; set; }
    }
}
