using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_CHECKLIST_PERGUNTA", EntityName = "CargaEntregaCheckListPergunta", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta", NameType = typeof(CargaEntregaCheckListPergunta))]
    public class CargaEntregaCheckListPergunta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaCheckList", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList CargaEntregaCheckList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_ASSUNTO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Assunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Obrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_TIPO_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_TIPO_HORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_PERMITE_NA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteNaoAplica { get; set; }


        /// <summary>
        /// Só é preenchido caso o Tipo seja SimNao
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_RESPOSTA_SIM_NAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RespostaSimNao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_RESPOSTA_NAO_SE_APLICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RespostaNaoSeAplica { get; set; }
        
        /// <summary>
        /// Só é preenchido caso o Tipo seja Informativo.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEP_RESPOSTA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Resposta { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Alternativas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ENTREGA_CHECKLIST_PERGUNTA_ALTERNATIVA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntregaCheckListAlternativa", Column = "CEA_CODIGO")]
        public virtual ICollection<CargaEntregaCheckListAlternativa> Alternativas { get; set; }

        public virtual bool TipoComAlternativas()
        {
            List<ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList> tiposComAlternativas = new List<ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList>() {
                ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList.Escala,
                ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList.Opcoes,
                ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList.Selecoes,
            };

            return tiposComAlternativas.Contains(Tipo);
        }
    }
}
