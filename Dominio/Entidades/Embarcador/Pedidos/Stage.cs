using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_STAGE", EntityName = "Stage", Name = "Dominio.Entidades.Embarcador.Pedidos.Stage", NameType = typeof(Stage))]
    public class Stage : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.Stage>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "STA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroStage", Column = "STA_NUMERO_STAGE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroStage { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemEntrega", Column = "STA_ORDEM_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Distancia", Column = "STA_DISTANCIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoModal", Column = "STA_TIPO_MODAL", TypeType = typeof(TipoModal), NotNull = false)]
        public virtual TipoModal TipoModal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalVenda", Column = "CNV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalVenda CanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RelevanciaCusto", Column = "STA_RELEVANCIA_CUSTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RelevanciaCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPercurso", Column = "STA_TIPO_PERCURSO", TypeType = typeof(Vazio), NotNull = false)]
        public virtual Vazio TipoPercurso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agrupamento", Column = "STA_AGRUPAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Agrupamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroVeiculo", Column = "STA_NUMERO_VEICULO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Processado", Column = "STA_PROCESSADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Processado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPossuiValePedagio", Column = "STA_NAO_POSSUI_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPossuiValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaDT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "StageAgrupamento", Column = "STG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento StageAgrupamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFolha", Column = "STA_NUMERO_FOLHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroFolha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFolha", Column = "STA_DATA_FOLHA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFolha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Calculo", Column = "STA_CALCULO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Calculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Atribuido", Column = "STA_ATRIBUIDO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Atribuido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Transferido", Column = "STA_TRANSFERIDO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Transferido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cancelado", Column = "STA_CANCELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Inconsistente", Column = "STA_INCONSISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Inconsistente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetornoEtapa", Column = "STA_MENSAGEM_RETORNO_ETAPA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MensagemRetornoEtapa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusVPEmbarcador", Column = "STA_STATUS_VP_EMBARCADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int StatusVPEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RelevanteVP", Column = "STA_RELEVANTE_VP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RelevanteVP { get; set; }

        public virtual Stage Clonar()
        {
            return (Stage)this.MemberwiseClone();
        }

        public virtual bool Equals(Stage other)
        {
            return (other.Codigo == this.Codigo);
        }
    }


}
