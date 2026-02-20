
namespace Dominio.Entidades.Embarcador.EMP
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTAINER_EMP", EntityName = "ContainerEMP", Name = "Dominio.Entidades.Embarcador.EMP.ContainerEMP", NameType = typeof(ContainerEMP))]
    public class ContainerEMP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoContainer", TypeType = typeof(int), Column = "COE_CODIGO_CONTAINER", NotNull = true)]
        public virtual int CodigoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoPaisOrigem", TypeType = typeof(int), Column = "COE_CODIGO_PAIS_ORIGEM", NotNull = false)]
        public virtual int CodigoPaisOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoProgramacaoViagemContainer", TypeType = typeof(int), Column = "COE_CODIGO_PROGRAMACAO_VIAGEM_CONTAINER", NotNull = false)]
        public virtual int CodigoProgramacaoViagemContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoPropContainer", TypeType = typeof(int), Column = "COE_CODIGO_PROP_CONTAINER", NotNull = false)]
        public virtual int CodigoPropContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoTipoContainer", TypeType = typeof(int), Column = "COE_CODIGO_TIPO_CONTAINER", NotNull = false)]
        public virtual int CodigoTipoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoViagem", TypeType = typeof(int), Column = "COE_CODIGO_VIAGEM", NotNull = false)]
        public virtual int CodigoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "IndicadorReutilizado", TypeType = typeof(int), Column = "COE_INDICADOR_REUTILIZADO", NotNull = false)]
        public virtual int IndicadorReutilizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Lacres", TypeType = typeof(string), Column = "COE_LACRES", Length = 200, NotNull = false)]
        public virtual string Lacres { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Nome", TypeType = typeof(string), Column = "COE_NOME", Length = 200, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "NumeroCNPJ", TypeType = typeof(string), Column = "COE_NUMERO_CNPJ", Length = 200, NotNull = false)]
        public virtual string NumeroCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "NumeroContainer", TypeType = typeof(string), Column = "COE_NUMERO_CONTAINER", Length = 200, NotNull = false)]
        public virtual string NumeroContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "NumeroProgramacao", TypeType = typeof(string), Column = "COE_NUMERO_PROGRAMACAO", Length = 200, NotNull = false)]
        public virtual string NumeroProgramacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(Name = "Observacao", TypeType = typeof(string), Column = "COE_OBSERVACAO", Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "PesoTotal", TypeType = typeof(double), Column = "COE_PESO_TOTAL", NotNull = false)]
        public virtual double PesoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Sigla", TypeType = typeof(string), Column = "COE_SIGLA", Length = 200, NotNull = false)]
        public virtual string Sigla { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "ValorTaraEspecifica", TypeType = typeof(double), Column = "COE_VALOR_TARA_ESPECIFICA", NotNull = false)]
        public virtual double ValorTaraEspecifica { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "Status", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusContainerEMP), Column = "COE_STATUS", NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusContainerEMP Status { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Nome;
            }
        }
    }
}
