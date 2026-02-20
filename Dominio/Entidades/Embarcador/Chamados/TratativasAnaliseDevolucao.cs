using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRATIVAS_ANALISE_DEVOLUCAO", EntityName = "TratativasAnaliseDevolucao", Name = "Dominio.Entidades.Embarcador.Chamados.TratativasAnaliseDevolucao", NameType = typeof(TratativasAnaliseDevolucao))]
    public class TratativasAnaliseDevolucao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TAD_TRATATIVA_DEVOLUCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega TratativaDevolucao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.TratativaDevolucao.ObterDescricao();
            }
        }
    }
}