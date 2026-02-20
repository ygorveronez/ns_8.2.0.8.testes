using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_ORQUESTRADOR_FILA", EntityName = "ConfiguracaoOrquestradorFila", Name = "Dominio.Entidades.ConfiguracaoOrquestradorFila", NameType = typeof(ConfiguracaoOrquestradorFila))]
    public class ConfiguracaoOrquestradorFila : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identificador", Column = "COF_IDENTIFICADOR", TypeType = typeof(SituacaoValePedagio), NotNull = true)]
        public virtual IdentificadorControlePosicaoThread Identificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeRegistrosConsulta", Column = "COF_QUANTIDADE_REGISTROS_CONSULTA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeRegistrosConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeRegistrosRetorno", Column = "COF_QUANTIDADE_REGISTROS_RETORNO", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeRegistrosRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteTentativas", Column = "COF_LIMITE_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int LimiteTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_TRATAR_REGISTROS_COM_FALHA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool TratarRegistrosComFalha { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Identificador.ObterDescricao();
            }
        }
    }
}
