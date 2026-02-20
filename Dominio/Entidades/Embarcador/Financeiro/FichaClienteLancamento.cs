using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_FICHA_LANCAMENTO", EntityName = "FichaClienteLancamento", Name = "Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento", NameType = typeof(FichaClienteLancamento))]
    public class FichaClienteLancamento : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FichaCliente", Column = "CFI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FichaCliente FichaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CFL_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CFL_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida), NotNull = true)]
        public virtual TipoEntradaSaida Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CFL_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        public virtual string Descricao
        {
            get
            {
                return TipoEntradaSaidaHelper.ObterDescricao(this.Tipo) + ": " + this.Valor.ToString();
            }
        }

    }
}
