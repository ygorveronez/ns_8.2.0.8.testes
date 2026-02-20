using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_SUBAREA_CLIENTE", EntityName = "TipoSubareaCliente", Name = "Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente", NameType = typeof(TipoSubareaCliente))]
    public class TipoSubareaCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TSA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TSA_DESCRICAO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TSA_TIPO", TypeType = typeof(int), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoSubareaCliente Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TSA_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "TSA_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea", Column = "TSA_PERMITE_MOVIMENTACAO_DO_PATIO_POR_ENTRADA_OU_SAIDA_DA_AREA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
