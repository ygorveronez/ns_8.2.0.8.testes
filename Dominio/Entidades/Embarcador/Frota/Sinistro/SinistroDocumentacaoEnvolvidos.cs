using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SINISTRO_DOCUMENTACAO_ENVOLVIDOS", EntityName = "SinistroDocumentacaoEnvolvidos", Name = "Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoEnvolvidos", NameType = typeof(SinistroDocumentacaoEnvolvidos))]
    public class SinistroDocumentacaoEnvolvidos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SinistroDados", Column = "SDS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SinistroDados SinistroDados { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "SDC_TIPO_ENVOLVIDO", TypeType = typeof(TipoEnvolvidoSinistro), NotNull = true)]
        public virtual TipoEnvolvidoSinistro TipoEnvolvido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDC_NOME", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Nome { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "SDC_RG", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string RG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDC_CPF", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDC_CNH", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CNH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDC_FONE_PRINCIPAL", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string TelefonePrincipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SDC_FONE_SECUNDARIO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string TelefoneSecundario { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "SDC_VEICULO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Veiculo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "SDC_OBSERVACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Observacao { get; set; }
        
        public virtual string Descricao
        {
            get
            {
                return $"Fluxo de Sinistro - Etapa Documentação";
            }
        }
    }
}
