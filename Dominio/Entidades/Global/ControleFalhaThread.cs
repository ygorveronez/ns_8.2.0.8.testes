using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONTROLE_FALHA_THREAD", EntityName = "ControleFalhaThread", Name = "Dominio.Entidades.ControleFalhaThread", NameType = typeof(ControleFalhaThread))]
    public class ControleFalhaThread : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEntidade", Column = "CFT_CODIGO_ENTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoEntidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identificador", Column = "CFT_IDENTIFICADOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread Identificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaTentativa", Column = "CFT_DATA_ULTIMA_TENTATIVA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataUltimaTentativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "CFT_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativasTotais", Column = "CFT_NUMERO_TENTATIVAS_TOTAIS", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroTentativasTotais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "CFT_LOG", TypeType = typeof(string), Length = 1000, NotNull = true)]
        public virtual string Log { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegistroComFalha", Column = "CFT_REGISTRO_COM_FALHA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegistroComFalha { get; set; }
    }
}