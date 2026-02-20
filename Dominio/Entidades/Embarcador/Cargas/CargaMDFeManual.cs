using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE_MANUAL", EntityName = "CargaMDFeManual", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual", NameType = typeof(CargaMDFeManual))]
    public class CargaMDFeManual : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [Obsolete("Será removido. Utilizar lista de motoristas.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_USAR_DADOS_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarDadosCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_USAR_SEGURO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarSeguroCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_ADICIONAR_MOTORISTA_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarMotoristaNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINOA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_SITUACAO_CANCELAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual SituacaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_TIPO_MODAL_MDFE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe TipoModalMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "CMM_TIPO_PAGAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe? TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoChavePIX", Column = "CMM_TIPO_CHAVE_PIX", TypeType = typeof(Dominio.ObjetosDeValor.Enumerador.TipoChavePix), Length = 200, NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Enumerador.TipoChavePix? TipoChavePIX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChavePIX", Column = "CMM_CHAVE_PIX", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ChavePIX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "CMM_AGENCIA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Conta", Column = "CMM_CONTA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Conta { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJInstituicaoPagamento", Column = "CMM_CNPJ_INSTITUICAO_PAGAMENTO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJInstituicaoPagamento { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_RETORNO_PROCESSAMENTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string RetornoProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_MDFE_IMPORTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MDFeImportado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MDFeRecebidoDeIntegracao", Column = "CMM_MDFE_RECEBIDO_DE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MDFeRecebidoDeIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MDFeEnviadoComSucessoPelaIntegracao", Column = "CMM_MDFE_ENVIADO_COM_SUCESSO_PELA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MDFeEnviadoComSucessoPelaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoFisco", Column = "CMM_OBSERVACAO_FISCO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoFisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoContribuinte", Column = "CMM_OBSERVACAO_CONTRIBUINTE", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_GERANDO_INTEGRACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerandoIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Percursos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_PERCURSO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFeManualPercurso", Column = "CMP_CODIGO")]
        public virtual ICollection<CargaMDFeManualPercurso> Percursos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Lacres", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_LACRE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFeManualLacre", Column = "CML_CODIGO")]
        public virtual ICollection<CargaMDFeManualLacre> Lacres { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ValePedagios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_VALE_PEGADIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFeManualValePedagio", Column = "CMV_CODIGO")]
        public virtual ICollection<CargaMDFeManualValePedagio> ValePedagios { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CIOTs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_CIOT")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFeManualCIOT", Column = "CMC_CODIGO")]
        public virtual ICollection<CargaMDFeManualCIOT> CIOTs { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NFes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_NFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFeManualNFe", Column = "CMN_CODIGO")]
        public virtual ICollection<CargaMDFeManualNFe> NFes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Seguros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_SEGURO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFeManualSeguro", Column = "CMS_CODIGO")]
        public virtual ICollection<CargaMDFeManualSeguro> Seguros { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Destinos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFeManualDestino", Column = "CMD_CODIGO")]
        public virtual ICollection<CargaMDFeManualDestino> Destinos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTe", Column = "CCT_CODIGO")]
        public virtual ICollection<CargaCTe> CTes { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carga", Column = "CAR_CODIGO")]
        public virtual ICollection<Carga> Cargas { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "MDFeManualMDFes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_MDFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFeManualMDFe", Column = "MDM_CODIGO")]
        public virtual ICollection<CargaMDFeManualMDFe> MDFeManualMDFes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_REBOQUES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Reboques { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_TERMINAL_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> TerminalCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_TERMINAL_DESCARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> TerminalDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_CARGA_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFeManualIntegracao", Column = "CMC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao> Integracoes { get; set; }
        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmDigitacao:
                        return "Em Digitação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmEmissao:
                        return "Em Emissão";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Rejeicao:
                        return "Rejeição";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.ProcessandoIntegracao:
                        return "Processando Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.AgIntegracao:
                        return "Aguardando Integrações";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.FalhaIntegracao:
                        return "Falha na Integração";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                if (this.MDFeManualMDFes != null && this.MDFeManualMDFes.Count > 0)
                    return String.Join(", ", (from m in this.MDFeManualMDFes select m.Descricao).ToArray());

                return string.Empty;
            }
        }

        public virtual bool UsarListaDestinos()
        {
            return UsarDadosCTe || (Empresa?.EmpresaPropria ?? false);
        }
    }
}
