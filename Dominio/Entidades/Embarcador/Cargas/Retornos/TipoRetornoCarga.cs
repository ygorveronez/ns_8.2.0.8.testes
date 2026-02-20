using System;

namespace Dominio.Entidades.Embarcador.Cargas.Retornos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_RETORNO_CARGA", EntityName = "TipoRetornoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga", NameType = typeof(TipoRetornoCarga))]
    public class TipoRetornoCarga : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TPR_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TPR_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeClienteColeta", Column = "TOP_EXIGE_CLIENTE_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeClienteColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_CARGA_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoCargaColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCargaDeColeta", Column = "TOP_GERAR_CARGA_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaDeColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FIL_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPR_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo Tipo { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool IsGerarCargaColetaBackhaul()
        {
            return (ExigeClienteColeta && GerarCargaDeColeta);
        }

        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculo ObterTipoFilaCarregamentoVeiculo()
        {
            return Tipo == ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio ? ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculo.Vazio : ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculo.Reversa;
        }

        public virtual bool Equals(TipoRetornoCarga other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
