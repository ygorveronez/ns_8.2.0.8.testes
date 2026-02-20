using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_EBS", EntityName = "EBS", Name = "Dominio.Entidades.Embarcador.Configuracoes.EBS", NameType = typeof(EBS))]
    public class EBS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EBS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoSalvarArquivo", Column = "EBS_CAMINHO_SALVAR_ARQUIVO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string CaminhoSalvarArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Emails", Column = "EBS_EMAILS", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Emails { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraExecucao", Column = "EBS_HORA_EXECUCAO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraExecucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasRetroativos", Column = "EBS_DIAS_RETROATIVOS", TypeType = typeof(int), NotNull = true)]
        public virtual int DiasRetroativos { get; set; }
        public virtual string Descricao
        {
            get
            {
                return "Configuração EBS";
            }
        }
    }
}
