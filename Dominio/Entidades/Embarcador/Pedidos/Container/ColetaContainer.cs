using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_CONTAINER", EntityName = "ColetaContainer", Name = "Dominio.Entidades.Embarcador.Pedidos.ColetaContainer", NameType = typeof(ColetaContainer))]
    public class ColetaContainer : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Container Container { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaDeColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_ATUAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaAtual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCR_DATA_COLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CCR_STATUS", TypeType = typeof(StatusColetaContainer), NotNull = true)]
        public virtual StatusColetaContainer Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCR_DATA_ULTIMA_MOVIMENTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaMovimentacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente LocalColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_ATUAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente LocalAtual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EMBARQUE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente LocalEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCR_DATA_EMBARQUE_NAVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmbarqueNavio { get; set; }

        /// <summary>
        /// O nome dessa propriedade no Front foi alterada para DataPorto. (regras continuam iguais, apenas descricao)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCR_DATA_EMBARQUE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCR_FREETIME", TypeType = typeof(int), NotNull = false)]
        public virtual int FreeTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCR_VALOR_DIARIA", TypeType = typeof(decimal), NotNull = false)]
        public virtual Decimal ValorDiaria { get; set; }

        [Obsolete("Campo único Migrado para uma Entidade Relacional(JustificativaColetaContainer).")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCR_JUSTIFICATIVA", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_AREA_ESPERA_VAZIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente AreaEsperaVazio { get; set; }


        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

    }
}
