using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SELECAO", EntityName = "Selecao", Name = "Dominio.Entidades.Embarcador.WMS.Selecao", NameType = typeof(Selecao))]
    public class Selecao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SEL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "SEL_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoSelecaoSeparacao", Column = "SEL_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao SituacaoSelecaoSeparacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SELECAO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SEL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SelecaoCarga", Column = "SEC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.SelecaoCarga> Cargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Funcionarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SELECAO_FUNCIONARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SEL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SelecaoFuncionario", Column = "SEF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.SelecaoFuncionario> Funcionarios { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Data.ToString("dd/MM/yyyy");
            }
        }

        public virtual string DescricaoSituacaoSelecaoSeparacao
        {
            get
            {
                if (this.SituacaoSelecaoSeparacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Cancelada)
                    return "Cancelado";
                if (this.SituacaoSelecaoSeparacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Pendente)
                    return "Pendente";
                if (this.SituacaoSelecaoSeparacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Enviada)
                    return "Enviada a Separação";
                if (this.SituacaoSelecaoSeparacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Finalizada)
                    return "Finalizada";
                else
                    return "";
            }
        }
    }
}
