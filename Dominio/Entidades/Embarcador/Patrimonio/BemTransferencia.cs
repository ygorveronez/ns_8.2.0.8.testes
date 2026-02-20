using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Patrimonio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BEM_TRANSFERENCIA", EntityName = "BemTransferencia", Name = "Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia", NameType = typeof(BemTransferencia))]
    public class BemTransferencia : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "BTR_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "BTR_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoSaida", Column = "BTR_OBSERVACAO_SAIDA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoEnvio", Column = "BTR_OBSERVACAO_ENVIO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Almoxarifado", Column = "AMX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.Almoxarifado Almoxarifado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ENVIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario FuncionarioEnvio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BEM_TRANSFERENCIA_ITEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "BTR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BemTransferenciaItem", Column = "BTI_CODIGO")]
        public virtual IList<BemTransferenciaItem> Bens { get; set; }

        public virtual string Descricao
        {
            get { return Funcionario.Descricao + " - " + DataEnvio.ToString("dd/MM/yyyy"); }
        }

        public virtual bool Equals(BemTransferencia other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
