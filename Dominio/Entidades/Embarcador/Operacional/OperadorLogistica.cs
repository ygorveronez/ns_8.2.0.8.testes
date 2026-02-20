using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Operacional
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OPERADOR_LOGISTICA", EntityName = "OperadorLogistica", Name = "Dominio.Entidades.Embarcador.Operacional.OperadorLogistica", NameType = typeof(OperadorLogistica))]
    public class OperadorLogistica : EntidadeBase, IEquatable<OperadorLogistica>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SupervisorLogistica", Column = "OPL_SUPERVISOR_LOGISTICA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SupervisorLogistica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteAdicionarComplementosDeFrete", Column = "OPL_PERMITE_ADICIONAR_COMPLEMENTOS_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteAdicionarComplementosDeFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirVisualizarValorFreteTransportadoresInteressadosCarga", Column = "OPL_PERMITIR_VISUALIZAR_VALOR_FRETE_TRANSPORTADORES_INTERESSADOS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirVisualizarValorFreteTransportadoresInteressadosCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAssumirCargasControleEntrega", Column = "OPL_PERMITIR_ASSUMIR_CARGAS_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAssumirCargasControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "OPL_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VisualizaCargasSemTipoOperacao", Column = "OPL_VISUALIZA_CARGA_SEM_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizaCargasSemTipoOperacao { get; set; } 
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraRecebedorSeraSobrepostaNasDemais", Column = "OPL_REGRA_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraRecebedorSeraSobrepostaNasDemais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiFiltroTipoOperacao", Column = "OPL_POSSUI_FILTRO_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiFiltroTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPL_PERMITE_SELECIONAR_HORARIO_ENCAIXE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSelecionarHorarioEncaixe { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoOperacoes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TipoOperacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VisualizaCargasSemGrupoPessoas", Column = "OPL_VISUALIZA_CARGA_SEM_GRUPO_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizaCargasSemGrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiFiltroGrupoPessoas", Column = "OPL_POSSUI_FILTRO_GRUPO_PESSOAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiFiltroGrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPL_TELA_PEDIDO_RESUMIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TelaPedidosResumido { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "GrupoPessoas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_GRUPO_PESSOAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoas", Column = "GRP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CentrosCarregamento", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_CENTRO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamento", Column = "CEC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> CentrosCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CentrosDescarregamento", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_CENTRO_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroDescarregamento", Column = "CED_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> CentrosDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Tomadores", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_TOMADORES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> Tomadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Filiais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OperadorFilial", Column = "OPF_CODIGO")]
        public virtual IList<OperadorFilial> Filiais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OperadorTipoCarga", Column = "OTC_CODIGO")]
        public virtual IList<OperadorTipoCarga> TiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OperadorCliente", Column = "OPC_CODIGO")]
        public virtual IList<OperadorCliente> Clientes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FiliaisVenda", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_FILIAL_VENDA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Filiais.Filial> FiliaisVenda { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Recebedores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_RECEBEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> Recebedores { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Expedidores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_EXPEDIDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> Expedidores { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Vendedores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_VENDEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Vendedores { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposTransportadorCentroCarregamento", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_LOGISTICA_TIPO_TRANSPORTADOR_CENTRO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "OPL_TIPO_TRANSPORTADOR", TypeType = typeof(TipoTransportadorCentroCarregamento), NotNull = true)]
        public virtual ICollection<TipoTransportadorCentroCarregamento> TiposTransportadorCentroCarregamento { get; set; }

        public virtual string Descricao
        {
            get { return this.Usuario?.Descricao ?? string.Empty; }
        }

        public virtual bool Equals(OperadorLogistica other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

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
