namespace Dominio.ObjetosDeValor
{
    public class ValeAcertoDeViagem
    {
        public int Codigo { get; set; }

        public int Numero { get; set; }

        public string Data { get; set; }

        public string Descricao { get; set; }

        public string Observacao { get; set; }

        public string Valor { get; set; }

        public bool Excluir { get; set; }

        public Enumeradores.TipoValeAcertoViagem Tipo { get; set; }

        public virtual string DescricaoTipo
        {
            set {
            }
            get
            {
                switch (this.Tipo)
                {
                    case Enumeradores.TipoValeAcertoViagem.Vale:
                        return "Vale";
                    case Enumeradores.TipoValeAcertoViagem.Devolucao:
                        return "Devolução";
                    default:
                        return "";
                }
            }
        }

    }
}
