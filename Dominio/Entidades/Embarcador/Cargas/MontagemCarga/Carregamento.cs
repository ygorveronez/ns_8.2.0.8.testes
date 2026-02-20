using Dominio.Interfaces.Embarcador.Entidade;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO", EntityName = "Carregamento", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento", NameType = typeof(Carregamento))]
    public class Carregamento : EntidadeBase, IEquatable<Carregamento>, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AutoSequenciaNumero", Column = "CRG_AUTO_SEQUENCIA_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int AutoSequenciaNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCarregamento", Column = "CRG_NUMERO_CARREGAMENTO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NumeroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoTransportador", Column = "GRT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Transportadores.GrupoTransportador GrupoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "CRG_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCarregamento", Column = "CRG_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento SituacaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRG_MENSAGEM_PROBLEMA_CARREGAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MensagemProblemaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMontagemCarga", Column = "CRG_TIPO_CARREGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga TipoMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamentoCarga", Column = "CRG_DATA_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoSaida", Column = "CRG_DATA_PREVISAO_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoRetorno", Column = "CRG_DATA_PREVISAO_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRG_ENCAIXAR_HORARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncaixarHorario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDescarregamentoCarga", Column = "CRG_DATA_DESCARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDescarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRG_DATA_INICIO_VIAGEM_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagemPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoCarregamento", Column = "PED_PESO_CARREGADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PalletCarregamento", Column = "PED_PALLET_CARREGADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PalletCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CRG_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteManual", Column = "CRG_VALOR_FRETE_MANUAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCondicaoPagamento", Column = "CRG_TIPO_CONDICAO_PAGAMENTO", TypeType = typeof(Enumeradores.TipoCondicaoPagamento), NotNull = false)]
        public virtual Enumeradores.TipoCondicaoPagamento? TipoCondicaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Reboques", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARREGAMENTO_REBOQUES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Reboques { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Motoristas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARREGAMENTO_MOTORISTAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Ajudantes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARREGAMENTO_AJUDANTES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Ajudantes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARREGAMENTO_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CarregamentoPedido", Column = "CRP_CODIGO")]
        public virtual ICollection<CarregamentoPedido> Pedidos { get; set; }

        //todo: remover essa propriedade não tem mais utilidade.
        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARREGAMENTO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CarregamentoCarga", Column = "CRC_CODIGO")]
        public virtual ICollection<CarregamentoCarga> Cargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargasFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carga", Column = "CAR_CODIGO")]
        public virtual ICollection<Carga> CargasFrete { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Fronteiras", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARREGAMENTO_FRONTEIRA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> Fronteiras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Filiais", Column = "CRG_FILIAIS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Filiais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Destinatarios", Column = "CRG_DESTINATARIOS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Destinatarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Destinos", Column = "CRG_DESTINOS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Destinos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CarregamentoRedespacho", Column = "CRG_CARREGAMENTO_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CarregamentoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VeiculoBloqueado", Column = "CRG_VEICULO_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CarregamentoIntegradoERP", Column = "CRG_CARREGAMENTO_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CarregamentoIntegradoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CarregamentoColeta", Column = "CRG_CARREGAMENTO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CarregamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCargas.PreCarga PreCarga { get; set; }

        [Obsolete("Migrado para uma lista. (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice)")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete Rota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SessaoRoteirizador", Column = "SRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador SessaoRoteirizador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoSeparacao", Column = "TSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoSeparacao TipoSeparacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CRG_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoTransportador", Column = "CRG_OBSERVACAO_TRANSPORTADOR", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string ObservacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeTransportadora", Column = "CRG_NOME_TRANSPORTADORA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaVeiculo", Column = "CRG_PLACA_VEICULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PlacaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRG_ID_PROPOSTA_TRIZY", TypeType = typeof(int), NotNull = false)]
        public virtual int IDPropostaTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailNotificacao", Column = "CRG_EMAIL_NOTIFICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EmailNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCargaAlteradoViaIntegracao", Column = "CRG_NUMERO_CARGA_ALTERADO_VIA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NumeroCargaAlteradoViaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeIsca", Column = "CRG_EXIGE_ISCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeIsca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MontagemCarregamentoBloco", Column = "MCB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco MontagemCarregamentoBloco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteIntegracaoCarregamento", Column = "LIC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento LoteIntegracaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRG_TEMPO_LIMITE_CONFIRMACAO_MOTORISTA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan TempoLimiteConfirmacaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_AGENDAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportadaComDocumentacaoDuplicadaMontagemFeeder", Column = "CRG_IMPORTADA_COM_DOCUMENTACAO_DUPLICADA_MONTAGEM_FEEDER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportadaComDocumentacaoDuplicadaMontagemFeeder { get; set; }

        public virtual string Descricao => NumeroCarregamento;

        public virtual bool Equals(Carregamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
