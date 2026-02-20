using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTORISTA_LICENCA", EntityName = "MotoristaLicenca", Name = "Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca", NameType = typeof(MotoristaLicenca))]
    public class MotoristaLicenca : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MLI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MLI_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "MLI_NUMERO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "MLI_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "MLI_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [Obsolete("Migrado, utilizar a lista FormasAlerta")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaAlerta", Column = "MLI_FORMA_ALERTA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FormaAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MLI_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Licenca", Column = "LIC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.Licenca Licenca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearCriacaoPedidoLicencaVencida", Column = "MLI_BLOQUEAR_CRIACAO_PEDIDO_LICENCA_VENCIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCriacaoPedidoLicencaVencida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearCriacaoPlanejamentoPedidoLicencaVencida", Column = "MLI_BLOQUEAR_CRIACAO_PLANEJAMENTO_PEDIDO_LICENCA_VENCIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCriacaoPlanejamentoPedidoLicencaVencida { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FormasAlerta", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MOTORISTA_LICENCA_FORMA_ALERTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MLI_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MLI_FORMA_ALERTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaForma), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaForma> FormasAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConfirmadaLeituraPendencia", Column = "MLI_CONFIRMADA_LEITURA_PENDENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConfirmadaLeituraPendencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConfirmadaLeituraPendencia", Column = "MLI_DATA_CONFIRMADA_LEITURA_PENDENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmadaLeituraPendencia { get; set; }
    }
}