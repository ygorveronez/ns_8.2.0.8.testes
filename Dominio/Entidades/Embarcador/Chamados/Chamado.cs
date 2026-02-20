using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADOS", EntityName = "Chamado", Name = "Dominio.Entidades.Embarcador.Chamados.Chamado", NameType = typeof(Chamado))]
    public class Chamado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_NUMERO", NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEmbarcador", Column = "CHA_NUMERO_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [Obsolete("Migrado para uma lista, N:N Chamados.ChamadoOcorrencia")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_CARGA_DEVOLUCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga CargaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoChamado MotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_NOTIFICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Notificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_GERAR_CARGA_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_VEICULO_CARREGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoCarregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_NOTIFICACAO_MOTORISTA_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificacaoMotoristaMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_AOS_CUIDADOS_DO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ChamadoAosCuidadosDo), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ChamadoAosCuidadosDo AosCuidadosDo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_RESPONSAVEL_CHAMADO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ResponsavelChamado), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelChamado ResponsavelChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPallet", Column = "CHA_NUMERO_PALLET", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal NumeroPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeItens", Column = "CHA_QUANTIDADE_ITENS", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal QuantidadeItens { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_RESPONSAVEL_OCORRENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ChamadoResponsavelOcorrencia), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ChamadoResponsavelOcorrencia ResponsavelOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_OBSERVACAO", Type = "StringClob", NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Autor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Representante", Column = "REP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.Representante Representante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Responsavel { get; set; }

        /// <summary>
        /// Setor para o qual foi delegado
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor SetorResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_CRICAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_REGISTRO_MOTORISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRegistroMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_PRIMEIRA_VEZ_ASSUMIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrimeiraVezAssumido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CHA_VALOR", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_TRATATIVA_DEVOLUCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega TratativaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoDevolucaoEntrega", Column = "MDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega MotivoDaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_NOVA_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteNovaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_OBSERVACAO_RETORNO_MOTORISTA", Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoRetornoMotorista { get; set; }

        [Obsolete("Migrado para o CargaPedido")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DEVOLUCAO_PARCIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DevolucaoParcial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCliente", Column = "CHA_TIPO_CLIENTE", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador? TipoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_RETENCAO_BAU", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetencaoBau { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_REENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_RETENCAO_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetencaoInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_RETENCAO_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetencaoFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoRetencao", Column = "CHA_TEMPO_RETENCAO", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal TempoRetencao { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "QrCode", Column = "CHA_QRCODE", TypeType = typeof(string), Length = 100, NotNull = false)]
        //public virtual string QrCode { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CHA_USUARIO_LIBERACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorReferencia", Column = "CHA_VALOR_REFERENCIA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal ValorReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaReboque", Column = "CHA_PLACA_REBOQUE", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string PlacaReboque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoasResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoaResponsavel", Column = "CHA_TIPO_PESSOA_RESPONSAVEL", TypeType = typeof(TipoPessoa), NotNull = false)]
        public virtual TipoPessoa? TipoPessoaResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "CHA_VALOR_DESCONTO", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_PAGO_PELO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PagoPeloMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRecusaCancelamento", Column = "MRC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoRecusaCancelamento MotivoRecusaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_FRETE_RETORNO_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FreteRetornoDevolucao { get; set; }

        /// <summary>
        /// Campo utilizado para evitar duplicidade em banco com chave única
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_CONTROLE_DUPLICIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int? ControleDuplicidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_QUANTIDADE_IMAGENS_ESPERADA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeImagensEsperada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoDescontadoMotorista", Column = "CHA_SALDO_DESCONTADO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoDescontadoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_ESTORNADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Estornado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_NIVEL", TypeType = typeof(EscalationList), NotNull = false)]
        public virtual EscalationList Nivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_ESTORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEstorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ESTORNO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioEstorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe RealMotivo { get; set; }

        [Obsolete("Movido para uma lista")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "XMLNotasFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_XML_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "XMLNotaFiscal", Column = "NFX_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> XMLNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdOcorrenciaTrizy", Column = "CHA_ID_OCORRENCIA_TRIZY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IdOcorrenciaTrizy { get; set; }

        /// <summary>
        /// Campo utilizado para realizar integração de forma diferente do habitual para Marilan #52621
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_AGUARDANDO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardandoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Analistas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_ANALISTAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Analistas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChamadoAnexo", Column = "ACH_CODIGO")]
        public virtual IList<ChamadoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Datas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_DATA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChamadoData", Column = "CDA_CODIGO")]
        public virtual IList<ChamadoData> Datas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PerfisAcesso", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_PERFIL_ACESSO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PerfilAcesso", Column = "PAC_CODIGO")]
        public virtual ICollection<Usuarios.PerfilAcesso> PerfisAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasAnalise", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_REGRAS_ANALISE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasAnaliseChamados", Column = "RAC_CODIGO")]
        public virtual ICollection<RegrasAnaliseChamados> RegrasAnalise { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_INFORMAR_DADOS_CHAMADO_FINALIZADO_COM_CUSTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDadosChamadoFinalizadoComCusto { get; set; }

        [Obsolete("Migrado para a lista ChamadoInformacaoFechamento")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_MOTIVO_PROCESSO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe MotivoProcesso { get; set; }

        [Obsolete("Migrado para a lista ChamadoInformacaoFechamento")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_QUANTIDADE_DIVERGENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDivergencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_NAO_ASSUMIR_DATA_ENTREGA_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAssumirDataEntregaNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_AGUARDANDO_TRATATIVA_DO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardandoTratativaDoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_NOVA_MOVIMENTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NovaMovimentacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAtendimentoChamados", Column = "RAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAtendimentoChamados RegrasAtendimentoChamados { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoMotivoChamado", Column = "GMC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoMotivoChamado GrupoMotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteChamadoOcorrencia", Column = "LCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LoteChamadoOcorrencia LoteChamadoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoInspecaoFederal", Column = "SIF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal ServicoInspecaoFederal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TiposCausadoresOcorrencia", Column = "TPO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia TiposCausadoresOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamadoCausas", Column = "MCC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas CausasMotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_SENHA_DEVOLUCAO", NotNull = false)]
        public virtual string SenhaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_CRITICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Critico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoCriticidadeAtendimento", Column = "TCA_CODIGO_GERENCIAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoCriticidadeAtendimento Gerencial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoCriticidadeAtendimento", Column = "TCA_CODIGO_CAUSA_PROBLEMA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoCriticidadeAtendimento CausaProblema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_FUP", Type = "StringClob", NotNull = false)]
        public virtual string FUP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_ESTADIA", TypeType = typeof(int), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao Estadia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_PREVISAO_ENTREGA_PEDIDOS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntregaPedidos { get; set; }

        #region Propriedades Virtuais

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }

        public virtual string DescricaoAosCuidadosDo
        {
            get { return AosCuidadosDo.ObterDescricao(); }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public virtual decimal TotalPagamentoMotorista(bool naoDescontarValorSaldoMotorista)
        {
            return naoDescontarValorSaldoMotorista ? Valor : (Valor - SaldoDescontadoMotorista);
        }

        #endregion
    }
}
