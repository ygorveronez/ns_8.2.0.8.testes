using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CCE", EntityName = "CartaDeCorrecaoEletronica", Name = "Dominio.Entidades.CartaDeCorrecaoEletronica", NameType = typeof(CartaDeCorrecaoEletronica))]
    public class CartaDeCorrecaoEletronica : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencialEvento", Column = "CCE_NUMERO_SEQUENCIAL_EVENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSequencialEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CCE_PROTOCOLO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CCE_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetornoSefaz", Column = "CCE_DATA_RETORNO_SEFAZ", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetornoSefaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegrador", Column = "CCE_CODIGO_INTEGRADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegrador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetornoSefaz", Column = "CCE_MENSAGEM_RETORNO_SEFAZ", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string MensagemRetornoSefaz { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ErroSefaz", Column = "ERR_CODIGO", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotNull = false)]
        public virtual ErroSefaz MensagemStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "CCE_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CCE_STATUS", TypeType = typeof(Enumeradores.StatusCCe), NotNull = true)]
        public virtual Enumeradores.StatusCCe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_IMPORTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Importado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SistemaEmissor", Column = "CON_SISTEMA_EMISSOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento? SistemaEmissor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return NumeroSequencialEvento.ToString();
            }
        }

        public virtual int NumeroCTe
        {
            get
            {
                return this.CTe.Numero;
            }
        }

        public virtual int SerieCTe
        {
            get
            {
                return this.CTe.Serie.Numero;
            }
        }

        public virtual string ChaveCTe
        {
            get
            {
                return this.CTe.Chave;
            }
        }

        public virtual string ProtocoloCTe
        {
            get
            {
                return this.CTe.Protocolo;
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Enumeradores.StatusCCe.Autorizado:
                        return "Autorizado";
                    case Enumeradores.StatusCCe.EmDigitacao:
                        return "Em Digitação";
                    case Enumeradores.StatusCCe.Enviado:
                        return "Enviado";
                    case Enumeradores.StatusCCe.Pendente:
                        return "Pendente";
                    case Enumeradores.StatusCCe.Rejeicao:
                        return "Rejeição";
                    default:
                        return string.Empty;
                }
            }
        }

    }
}
