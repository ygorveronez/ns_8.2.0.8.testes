using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_DOCUMENTO", EntityName = "ControleDocumento", Name = "Dominio.Entidades.Embarcador.Documentos.ControleDocumento", NameType = typeof(ControleDocumento))]
    public class ControleDocumento : EntidadeBase, IEntidade
    {
        public ControleDocumento() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTe CargaCTe { get; set; }  
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoIrregularidade", Column = "MTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade MotivoIrregularidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoDesacordo", Column = "MTD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo MotivoDesacordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoControleDocumento", Column = "COD_SITUACAO_CONTROLE_DOCUMENTO", TypeType = typeof(SituacaoControleDocumento), NotNull = false)]
        public virtual SituacaoControleDocumento SituacaoControleDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoParqueamento", Column = "COD_MOTIVO_PARQUEAMENTO", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string MotivoParqueamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoTransportador", Column = "COD_MOTIVO_TRANSPORTADOR", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string MotivoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Analise", Column = "COD_ANALISE", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string Analise { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ServicoResponsavel", Column = "COD_SERVICO_RESPONSAVEL", TypeType = typeof(ServicoResponsavel), NotNull = false)]
        public virtual ServicoResponsavel? ServicoResponsavel { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvioAprovacao", Column = "COD_DATA_ENVIO_APROVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvioAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoVerificacao", Column = "COD_SITUACAO_VERIFICACAO", TypeType = typeof(SituacaoVerificacaoControleDocumento), NotNull = false)]
        public virtual SituacaoVerificacaoControleDocumento SituacaoVerificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcaoTratativa", Column = "COD_ACAO_TRATATIVA", TypeType = typeof(AcaoTratativaIrregularidade), NotNull = false)]
        public virtual AcaoTratativaIrregularidade AcaoTratativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAprovacaoCartaDeCorrecao", Column = "COD_SITUACAO_APROVACAO_CARTA_CORRECAO", TypeType = typeof(SituacaoAprovacaoCartaDeCorrecao), NotNull = false)]
        public virtual SituacaoAprovacaoCartaDeCorrecao SituacaoAprovacaoCartaDeCorrecao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicaoCCe", Column = "COD_MOTIVO_REJEICAO_CCE", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string MotivoRejeicaoCCe { get; set; }

        public virtual string Descricao
        {
            get { return CTe.Numero.ToString(); }
        }


        public virtual Cargas.Carga Carga
        {
            get
            {
                if (this.CargaCTe != null)
                    return this.CargaCTe.Carga;

                return null;
            }
        }

        public virtual bool SituacaoControleDocumentoAprovada
        {
            get
            {
                return (SituacaoControleDocumento == SituacaoControleDocumento.ParqueadoManualmente);
            }
        }
    }
}
