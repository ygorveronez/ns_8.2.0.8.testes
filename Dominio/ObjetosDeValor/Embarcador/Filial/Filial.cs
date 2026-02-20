namespace Dominio.ObjetosDeValor.Embarcador.Filial
{
    public class Filial
    {
        public int Protocolo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public virtual string CNPJ { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoFilial TipoFilial { get; set; }
        public int CodigoAtividade { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco Endereco { get; set; }
        public bool Ativo { get; set; }
    }
}
