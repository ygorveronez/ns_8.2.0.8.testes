using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_TRIZY_INFORMACAO_ADICIONAL_RELATORIO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio", NameType = typeof(ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio))]
    public class ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio : EntidadeBase
    {
        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IAR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoTrizy", Column = "CTT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoTrizy ConfiguracaoTipoOperacaoTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rotulo", Column = "IAR_ROTULO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Rotulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "IAR_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        #endregion
    }
}