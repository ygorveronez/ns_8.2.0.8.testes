using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_AJUSTE", EntityName = "AjusteTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete", NameType = typeof(AjusteTabelaFrete))]
    public class AjusteTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Número sequencial
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "TFA_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "TFA_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAjuste", Column = "TFA_DATA_AJUSTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAjuste { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "TFA_SITUACAO", TypeType = typeof(SituacaoAjusteTabelaFrete), NotNull = false)]
        public virtual SituacaoAjusteTabelaFrete? Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFA_SITUACAO_APOS_PROCESSAMENTO", TypeType = typeof(SituacaoAjusteTabelaFrete), NotNull = false)]
        public virtual SituacaoAjusteTabelaFrete? SituacaoAposProcessamento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Etapa", Column = "TFA_ETAPA", TypeType = typeof(EtapaAjusteTabelaFrete), NotNull = false)]
        public virtual EtapaAjusteTabelaFrete Etapa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_APROVADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioAprovador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "VigenciaTabelaFrete", Column = "TFV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual VigenciaTabelaFrete Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "VigenciaTabelaFrete", Column = "TFV_NOVA_VIGENCIA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual VigenciaTabelaFrete NovaVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFA_NOVA_VIGENCIA_INDEFINIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? NovaVigenciaIndefinida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoReajuste", Column = "MRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.MotivoReajuste MotivoReajuste { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoTransporteFrete", Column = "CTF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoTransporteFrete ContratoTransporteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFA_UTILIZAR_BUSCA_LOCALIDADES_ESTADO_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarBuscaNasLocalidadesPorEstadoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFA_UTILIZAR_BUSCA_LOCALIDADES_ESTADO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarBuscaNasLocalidadesPorEstadoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Remetentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_REMETENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Remetentes { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "Destinatarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_DESTINATARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Destinatarios { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "Tomadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_TOMADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Tomadores { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "Empresas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_EMPRESA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Empresa> Empresas { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "RotasFreteDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_ROTA_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFrete", Column = "ROF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFrete> RotasFreteDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RotasFreteOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_ROTA_FRETE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFrete", Column = "ROF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFrete> RotasFreteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "UFsOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_UF_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> UFsOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "UFsDestinos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_UF_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> UFsDestinos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Origens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> Origens { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "Destinos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> Destinos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegioesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_REGIAO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Regiao", Column = "REG_CODIGO")]
        public virtual ICollection<Localidades.Regiao> RegioesDestino { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Cargas.TipoDeCarga> TiposCarga { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosTracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_MODELO_TRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> ModelosTracao { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosReboque", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_MODELO_REBOQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> ModelosReboque { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Pedidos.TipoOperacao> TiposOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CanaisVenda", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_CANAL_VENDA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CanalVenda", Column = "CNV_CODIGO")]
        public virtual ICollection<Pedidos.CanalVenda> CanaisVenda { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CanaisEntrega", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_CANAL_ENTREGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CanalEntrega", Column = "CNE_CODIGO")]
        public virtual ICollection<Pedidos.CanalEntrega> CanaisEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "TFA_TIPO_PAGAMENTO", TypeType = typeof(TipoPagamentoEmissao), NotNull = false)]
        public virtual TipoPagamentoEmissao? TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApenasTabelasComCargasRealizadas", Column = "TFA_APENAS_TABELAS_COM_CARGAS_REALIZADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApenasTabelasComCargasRealizadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomenteRegistrosComValores", Column = "TFA_SOMENTE_REGISTROS_COM_VALORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomenteRegistrosComValores { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Criador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AjusteTabelaFreteAutorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUSTE_AUTORIZACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AjusteTabelaFreteAutorizacao", Column = "ATA_CODIGO")]
        public virtual ICollection<AjusteTabelaFreteAutorizacao> AjusteTabelaFreteAutorizacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AjustarPedagiosComSemParar", Column = "TFA_AJUSTAR_PEDAGIOS_COM_SEM_PARAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AjustarPedagiosComSemParar { get; set; }

        public virtual string DescricaoSituacao
        {
            get { return Situacao?.ObterDescricao(); }
        }

        public virtual string DescricaoEtapa
        {
            get { return Etapa.ObterDescricao(); }
        }

        public virtual string Descricao
        {
            get { return this.TabelaFrete?.Descricao ?? string.Empty; }
        }
    }
}
