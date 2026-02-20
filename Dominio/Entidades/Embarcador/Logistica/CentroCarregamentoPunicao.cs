namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CENTRO_CARREGAMENTO_PUNICAO", EntityName = "CentroCarregamentoPunicao", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoPunicao", NameType = typeof(CentroCarregamentoPunicao))]
    public class CentroCarregamentoPunicao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Armazena a quantidade de horas e minutos em decial - Exemplo (1,5 = 1:30)
        /// Necessário fazer a conversão para horas e minutos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CPU_TEMPO_PUNICAO", TypeType = typeof(decimal), Scale = 2, Precision = 10, NotNull = false)]
        public virtual decimal TempoPunicao { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_TIPO_FROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFrota), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFrota? TipoFrota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        #region Propriedades Virtuais

        public virtual string TipoFrotaDescricao 
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFrotaHelper.ObterDescricao(TipoFrota.Value); }
        }

        public virtual double TempoPunicaoDouble
        {
            get { return (double)TempoPunicao; }
        }

        #endregion

    }
}