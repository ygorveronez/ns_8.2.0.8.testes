namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_OBSERVACAO_CTE", EntityName = "ClienteObservacaoCTe", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteObservacaoCTe", NameType = typeof(ClienteObservacaoCTe))]
    public class ClienteObservacaoCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_IDENTIFICADOR", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Identificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_TEXTO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Texto { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Identificador;
            }
        }

        public virtual ObjetosDeValor.CTe.Observacao ObterObservacaoCTe()
        {
            return new ObjetosDeValor.CTe.Observacao()
            {
                Descricao = this.Texto,
                Identificador = this.Identificador
            };
        }
    }
}
