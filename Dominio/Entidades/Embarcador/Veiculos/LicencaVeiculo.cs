using Dominio.Entidades.Embarcador.Cargas;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_LICENCA", EntityName = "LicencaVeiculo", Name = "Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo", NameType = typeof(LicencaVeiculo))]
    public class LicencaVeiculo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VLI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VLI_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "VLI_NUMERO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "VLI_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "VLI_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [Obsolete("Migrado, utilizar a lista FormasAlerta")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaAlerta", Column = "VLI_FORMA_ALERTA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FormaAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "VLI_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vencido", Column = "VLI_VENCIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Vencido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClassificacaoRiscoONU", Column = "CRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.ClassificacaoRiscoONU ClassificacaoRiscoONU { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Licenca", Column = "LIC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.Licenca Licenca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearCriacaoPedidoLicencaVencida", Column = "VLI_BLOQUEAR_CRIACAO_PEDIDO_LICENCA_VENCIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCriacaoPedidoLicencaVencida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearCriacaoPlanejamentoPedidoLicencaVencida", Column = "VLI_BLOQUEAR_CRIACAO_PLANEJAMENTO_PEDIDO_LICENCA_VENCIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCriacaoPlanejamentoPedidoLicencaVencida { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FaixasTemperatura", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LICENCA_VEICULO_FAIXA_TEMPERATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VLI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaixaTemperatura", Column = "FTE_CODIGO")]
        public virtual ICollection<FaixaTemperatura> FaixasTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ALTERACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "VLI_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FormasAlerta", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_LICENCA_FORMA_ALERTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VLI_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "VLI_FORMA_ALERTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaForma), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaForma> FormasAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContainer", Column = "VLI_NUMERO_CONTAINER", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroContainer { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Container Container { get; set; }

        public virtual string DescricaoVencido
        {
            get { return Vencido ? Localization.Resources.Enumeradores.StatusLicenca.Vencido : Localization.Resources.Enumeradores.StatusLicenca.Vigente; }
        }

        public virtual bool Equals(LicencaVeiculo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}