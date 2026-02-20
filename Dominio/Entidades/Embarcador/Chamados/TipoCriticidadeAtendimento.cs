using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_CRITICIDADE_ATENDIMENTO", EntityName = "TipoCriticidadeAtendimento", Name = "Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento", NameType = typeof(TipoCriticidadeAtendimento))]
    public class TipoCriticidadeAtendimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoChamado MotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCA_TIPO", TypeType = typeof(TipoParametroCriticidade), NotNull = true)]
        public virtual TipoParametroCriticidade Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCA_CONTEUDO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Conteudo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCA_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        #region Propriedades Virtuais

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
        public virtual string DescricaoTipo => Tipo.ObterDescricao();
        public virtual string DescricaoAtivo => Ativo ? "Sim" : "Não";

        #endregion
    }
}