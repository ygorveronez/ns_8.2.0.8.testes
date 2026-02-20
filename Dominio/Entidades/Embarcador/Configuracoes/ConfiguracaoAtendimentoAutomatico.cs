using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_ATENDIMENTO_AUTOMATICO", EntityName = "ConfiguracaoAtendimentoAutomatico", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico", NameType = typeof(ConfiguracaoAtendimentoAutomatico))]
    public class ConfiguracaoAtendimentoAutomatico: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAtendimentoDivergenciaValorTabelaCTeEmitidoEmbarcador", Column = "CAA_GERAR_ATENDIMENTO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAtendimentoDivergenciaValorTabelaCTeEmitidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.MotivoChamado MotivoChamado { get; set; }

        #endregion Propriedades

    }
}
