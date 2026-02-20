using System;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANEJAMENTO_FROTA_DIA", EntityName = "PlanejamentoFrotaDia", Name = "Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia", NameType = typeof(PlanejamentoFrotaDia))]
    public class PlanejamentoFrotaDia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        private string _descricao = string.Empty;
        public virtual string Descricao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_descricao))
                    _descricao = "PlanejamentoFrotaDia Cod.: " + Codigo.ToString() + " Data: " + Data.ToString("dd/MM/yyyy");

                return _descricao;
            }
            set
            {
                _descricao = value;
            }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PFD_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanejamentoFrotaMes", Column = "PFM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanejamentoFrotaMes PlanejamentoFrotaMes { get; set; }

        
    }
}
