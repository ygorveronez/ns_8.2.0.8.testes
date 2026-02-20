using System;
using System.Collections.Generic;

namespace AdminMultisoftware.Dominio.Entidades.MensagemAviso
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MENSAGEM_AVISO", EntityName = "MensagemAviso", Name = "AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAviso", NameType = typeof(MensagemAviso))]
    public class MensagemAviso : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulo", Column = "MAV_TITULO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MAV_DESCRICAO", TypeType = typeof(string), Length = 10000, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "MAV_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "MAV_DATA_FIM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MAV_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MAV_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoMultisoftware", Column = "MAV_TIPO_SERVICO_MULTISOFTWARE", TypeType = typeof(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware), NotNull = true)]
        public virtual AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MENSAGEM_AVISO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MAV_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "MensagemAvisoAnexo")]
        public virtual IList<Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MAV_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual string DescricaoTipoServicoMultisoftware
        {
            get
            {
                switch (TipoServicoMultisoftware)
                {
                    case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.CallCenter:
                        return "CallCenter";
                    case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe:
                        return "Portal do Transportador";
                    case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador:
                        return "MultiEmbarcador";
                    case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe:
                        return "MultiNFe";
                    case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin:
                        return "MultiNFeAdmin";
                    case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS:
                        return "MultiTMS";
                    case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros:
                        return "Terceiros";
                    case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor:
                        return "Portal do Fornecedor";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual bool Equals(MensagemAviso other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
