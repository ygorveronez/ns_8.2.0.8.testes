using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pallets.Avaria
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_AVARIA", EntityName = "AvariaPallet", Name = "Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet", NameType = typeof(AvariaPallet))]
    public class AvariaPallet : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PAV_DATA", TypeType = typeof(System.DateTime), NotNull = true)]
        public virtual System.DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PAV_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Solicitante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaPallet), NotNull = true)]
        public virtual SituacaoAvariaPallet Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAvariaPallet", Column = "PMA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoAvariaPallet MotivoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoPallets", Column = "FEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoPallets Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAV_ADICIONAR_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarAoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "QuantidadesAvariadas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PALLET_AVARIA_QUANTIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AvariaPalletQuantidade", Column = "PAQ_CODIGO")]
        public virtual IList<AvariaPalletQuantidade> QuantidadesAvariadas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PALLET_AVARIA_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AvariaPalletAnexo", Column = "ANX_CODIGO")]
        public virtual IList<AvariaPalletAnexo> Anexos { get; set; }

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }

        public virtual string SituacaoDescricao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public virtual bool Equals(AvariaPallet other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
