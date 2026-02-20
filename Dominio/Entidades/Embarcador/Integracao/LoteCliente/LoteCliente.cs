using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao.LoteCliente
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_CLIENTE", EntityName = "LoteCliente", Name = "Dominio.Entidades.Embarcador.Financeiro.LoteCliente.LoteCliente", NameType = typeof(LoteCliente))]
    public class LoteCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LCL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "LCL_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCL_DATA_GERACAO_LOTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataGeracaoLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCL_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCL_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCL_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LCL_GEROU_INTEGRACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOTE_CLIENTE_INTEGRACAO_EDI")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LCL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LoteClienteIntegracaoEDI", Column = "LCI_CODIGO")]
        public virtual IList<LoteClienteIntegracaoEDI> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOTE_CLIENTE_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LCL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> Clientes { get; set; }

        public virtual string Descricao
        {
            get { return this.Numero.ToString(); }
        }
    }
}
