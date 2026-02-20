using System;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANHOTO_NOTA_FISCAL_HISTORICO", EntityName = "CanhotoHistorico", Name = "Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico", NameType = typeof(CanhotoHistorico))]
    public class CanhotoHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Canhoto", Column = "CNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Canhotos.Canhoto Canhoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoCanhoto", Column = "LAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto LocalArmazenamentoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNH_PACOTE_ARMAZENADO", TypeType = typeof(int), NotNull = false)]
        public virtual int PacoteArmazenado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNH_POSICAO_NO_PACOTE", TypeType = typeof(int), NotNull = false)]
        public virtual int PosicaoNoPacote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHistorico", Column = "CNH_DATA_ENVIO_CANHOTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataHistorico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CNH_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCanhoto", Column = "CNH_SITUACAO_CANHOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto SituacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoDigitalizacaoCanhoto", Column = "CNH_SITUACAO_DIGITALIZACAO_CANHOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto SituacaoDigitalizacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoOperador", Column = "CNH_OBSERVACAO_OPERADOR", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ObservacaoOperador { get; set; }

        public virtual string DescricaoDigitalizacao
        {
            get
            {
                switch (SituacaoDigitalizacaoCanhoto)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao:
                        return "Ag. Aprovação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada:
                        return "Rejeitada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado:
                        return "Digitalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao:
                        return "Pendente";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (SituacaoCanhoto)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente:
                        return "Pendente de Envio";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado:
                        return "Justificativa Enviada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente:
                        return "Recebido Fisicamente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Extraviado:
                        return "Extraviado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EntregueMotorista:
                        return "Entregue pelo Motorista";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EnviadoCliente:
                        return "Enviado ao Cliente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoCliente:
                        return "Recebido pelo Cliente";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
