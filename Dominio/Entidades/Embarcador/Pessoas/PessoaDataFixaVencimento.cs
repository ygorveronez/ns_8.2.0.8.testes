using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_DATA_FIXA_VENCIMENTO", EntityName = "PessoaDataFixaVencimento", Name = "Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento", NameType = typeof(PessoaDataFixaVencimento))]
    public class PessoaDataFixaVencimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaInicialEmissao", Column = "DFV_DIA_INICIAL_EMISSAO", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaInicialEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaFinalEmissao", Column = "DFV_DIA_FINAL_EMISSAO", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaFinalEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaVencimento", Column = "DFV_DIA_VENCIMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaVencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        public virtual bool Equals(PessoaDataFixaVencimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
