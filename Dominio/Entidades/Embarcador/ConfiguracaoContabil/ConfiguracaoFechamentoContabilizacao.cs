using System;

namespace Dominio.Entidades.Embarcador.ConfiguracaoContabil
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FECHAMENTO_CONTABILIZACAO", EntityName = "ConfiguracaoFechamentoContabilizacao", Name = "Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao", NameType = typeof(ConfiguracaoFechamentoContabilizacao))]
    public class ConfiguracaoFechamentoContabilizacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_ULTIMO_DIA_ENVIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime UltimoDiaEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_MES_REFERENCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int MesReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_ANO_REFERENCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int AnoReferencia { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"{Codigo} - {MesReferencia}/{AnoReferencia}";
            }
        }
    }
}
