using System;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOCAL_ARMAZENAMENTO_CANHOTO", EntityName = "LocalArmazenamentoCanhoto", Name = "Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto", NameType = typeof(LocalArmazenamentoCanhoto))]
    public class LocalArmazenamentoCanhoto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "LAC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeArmazenagem", Column = "LAC_CAPACIDADE_ARMAZENAGEM", TypeType = typeof(int), NotNull = true)]
        public virtual int CapacidadeArmazenagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeArmazenada", Column = "LAC_QUANTIDADE_ARMAZENADA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeArmazenada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DividirEmPacotesDe", Column = "LAC_DIVIDIR_EM_PACOTES_DE", TypeType = typeof(int), NotNull = false)]
        public virtual int DividirEmPacotesDe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PacoteAtual", Column = "LAC_PACOTE_ATUAL", TypeType = typeof(int), NotNull = false)]
        public virtual int PacoteAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalArmazenagemAtual", Column = "LAC_LOCAL_ARMAZENAGEM_ATUAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LocalArmazenagemAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "LAC_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "LAC_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCanhoto", Column = "LAC_TIPO_CANHOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto? TipoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        public virtual string DescricaoTipoCanhoto
        {
            get
            {
                switch (this.TipoCanhoto)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe:
                        return "NF-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso:
                        return "Avulso";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(LocalArmazenamentoCanhoto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
