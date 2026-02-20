using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANAL_ENTREGA", EntityName = "CanalEntrega", Name = "Dominio.Entidades.Embarcador.Pedidos.CanalEntrega", NameType = typeof(CanalEntrega))]
    public class CanalEntrega : EntidadeBase, IComparable<CanalEntrega>, IEquatable<CanalEntrega>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CNE_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CNE_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CNE_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCargaAutomaticamente", Column = "CNE_GERAR_CARGA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarCargaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNE_LIBERAR_PEDIDO_SEM_NFE_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarPedidoSemNFeAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CNE_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        /// <summary>
        /// Atributo para definir a prioridade no processo de fechamento de cargas (MontagemCarga - ASSAI)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelPrioridade", Column = "CNE_NIVEL_PRIORIDADE", TypeType = typeof(Int32), NotNull = false)]
        public virtual int NivelPrioridade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        /// <summary>
        /// Atributos a ser utilizado para Criar um canal de entrega principal, onde novos canais poderam estar relacionados a ela.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Principal", Column = "CNE_PRINCIPAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Principal { get; set; }

        /// <summary>
        /// Identificador do Canal de Entrega principal.
        /// Ex: CARGA SECA PRINC
        ///           SECA 01
        ///           SECA 02
        ///     REFRIGERADA
        ///             PIZZA
        ///             RERIGERADA 02.
        ///             REFRIGERANTE 
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO_PRINCIPAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CanalEntrega CanalEntregaPrincipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Circuito", Column = "CNE_CIRCUITO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Circuito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePedidosPermitidosNoCanal", Column = "CNE_QUANTIDADE_PEDIDOS_PERMITIDOS_NO_CANAL", TypeType = typeof(Int32), NotNull = false)]
        public virtual int? QuantidadePedidosPermitidosNoCanal { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoUtilizarCapacidadeVeiculoMontagemCarga", Column = "CNE_NAO_UTILIZAR_CAPACIDADE_VEICULO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarCapacidadeVeiculoMontagemCarga { get; set; }


        public virtual int CompareTo(CanalEntrega other)
        {
            if (other == null)
                return -1;

            return other.Codigo.CompareTo(Codigo);
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return Localization.Resources.Gerais.Geral.Ativo;
                else
                    return Localization.Resources.Gerais.Geral.Inativo;
            }
        }

        public virtual bool Equals(CanalEntrega other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
