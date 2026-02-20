using System;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_LEITURA_IMAGEM_CANHOTO", EntityName = "ControleLeituraImagemCanhoto", Name = "Dominio.Entidades.Embarcador.Integracao.ControleLeituraImagemCanhoto", NameType = typeof(ControleLeituraImagemCanhoto))]
    public class ControleLeituraImagemCanhoto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "CIE_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Canhoto", Column = "CNF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Canhotos.Canhoto Canhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "CIE_GUID_ARQUIVO", TypeType = typeof(string), Length = 80, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "CEI_NUMERO_DOCUMENTO", Length = 150, NotNull = true)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CIE_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoleituraImagemCanhoto", Column = "CEI_SITUACAO_LEITURA_IMAGEM_CANHOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto SituacaoleituraImagemCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEI_EXTENSAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ExtensaoArquivo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ExtensaoArquivo Extensao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "CIE_MENSAGEM_RETORNO", Type = "StringClob", NotNull = true)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (SituacaoleituraImagemCanhoto)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.Todas:
                        return "";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.AgProcessamento:
                        return "Ag. Processamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.CanhotoVinculado:
                        return "Canhoto Vinculado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.SemCanhoto:
                        return "Sem Canhoto";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.ImagemNaoReconhecida:
                        return "Imagem não reconhecida";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.Descartada:
                        return "Descartada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.FalhaProcessamento:
                        return "Falha ao processar";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.AgImagem:
                        return "Ag. Imagem";
                    default:
                        return "";
                }
            }
        }
    }
}
