using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NATUREZAOPERACAO", EntityName = "NaturezaDaOperacao", Name = "Dominio.Entidades.NaturezaDaOperacao", NameType = typeof(NaturezaDaOperacao))]
    public class NaturezaDaOperacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "NAT_DESCRICAO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NAT_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "NAT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DentroEstado", Column = "NAT_DENTRO_ESTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DentroEstado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeraTitulo", Column = "NAT_GERA_TITULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeraTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Garantia", Column = "NAT_GARANTIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Garantia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Demonstracao", Column = "NAT_DEMONSTRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Demonstracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bonificacao", Column = "NAT_BONIFICACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bonificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Outras", Column = "NAT_OUTRAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Outras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlaEstoque", Column = "NAT_CONTROLA_ESTOQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlaEstoque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NAT_GERAR_MOVIMENTO_AUTOMATICO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoAutomaticoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO_ENTRADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoUsoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_ENTRADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NAT_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaNFSe", Column = "NAN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaNFSe NaturezaNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "NAT_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesconsideraICMSEfetivo", Column = "NAT_DESCONSIDERA_ICMS_EFETIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesconsideraICMSEfetivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Devolucao", Column = "NAT_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Devolucao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NATUREZAOPERACAO_CFOP")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CFOP", Column = "CFO_CODIGO")]
        public virtual ICollection<CFOP> CFOPs { get; set; }

        public virtual string BuscarDescricao
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CodigoIntegracao))
                    return this.CodigoIntegracao + " - " + this.Descricao;
                else
                    return this.Descricao;
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Ativo";
                    case "I":
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual Dominio.Entidades.NaturezaDaOperacao Clonar()
        {
            return (Dominio.Entidades.NaturezaDaOperacao)this.MemberwiseClone();
        }

    }
}
