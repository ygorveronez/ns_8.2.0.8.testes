using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRAVAMENTO_CHAVE_ASSINATURA_MOTORISTA", EntityName = "TravamentoChaveAssinaturaMotorista", Name = "Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChaveAssinaturaMotorista", NameType = typeof(TravamentoChaveAssinaturaMotorista))]
    public class TravamentoChaveAssinaturaMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TAM_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TAM_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TAM_DATA_ENVIO_IMAGEM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEnvioAssinatura { get; set; }

        public virtual string Descricao => "Assinatura Motorista - " + Codigo.ToString();

    }
}
