using System;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COMISSAO_FUNCIONARIO_MOTORISTA_ABASTECIMENTO", EntityName = "ComissaoFuncionarioMotoristaAbastecimento", Name = "Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento", NameType = typeof(ComissaoFuncionarioMotoristaAbastecimento))]
    public class ComissaoFuncionarioMotoristaAbastecimento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComissaoFuncionarioMotorista", Column = "CFM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista ComissaoFuncionarioMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Abastecimento", Column = "ABA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Abastecimento Abastecimento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Abastecimento?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(ComissaoFuncionarioMotoristaAbastecimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
