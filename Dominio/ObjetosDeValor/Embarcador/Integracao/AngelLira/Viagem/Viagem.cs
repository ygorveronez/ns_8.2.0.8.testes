namespace Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Viagem
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, IncludeInSchema = false, Namespace = "")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class Viagem
    {

        private int? tipoField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public int? V_Tipo
        {
            get
            {
                return this.tipoField;
            }
            set
            {
                this.tipoField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_TipoSpecified
        {
            get
            {
                return V_Tipo.HasValue;
            }
        }

        private string cargaField;
        public string V_Carga
        {
            get
            {
                return this.cargaField;
            }
            set
            {
                this.cargaField = value;
            }
        }


        private string origemField;
        public string V_Origem
        {
            get
            {
                return this.origemField;
            }
            set
            {
                this.origemField = value;
            }
        }

        private string destinoField;
        public string V_Destino
        {
            get
            {
                return this.destinoField;
            }
            set
            {
                this.destinoField = value;
            }
        }

        private int? rotaField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public int? V_Rota
        {
            get
            {
                return this.rotaField;
            }
            set
            {
                this.rotaField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_RotaSpecified
        {
            get
            {
                return V_Rota.HasValue;
            }
        }


        private string polilinhaField;
        public string V_Polilinha
        {
            get
            {
                return this.polilinhaField;
            }
            set
            {
                this.polilinhaField = value;
            }
        }


        private string prevInicioField;
        public string V_PrevInicio
        {
            get
            {
                return this.prevInicioField;
            }
            set
            {
                this.prevInicioField = value;
            }
        }

        private string prevFimField;
        public string V_PrevFim
        {
            get
            {
                return this.prevFimField;
            }
            set
            {
                this.prevFimField = value;
            }
        }

        private string dataFatField;
        public string V_DataFat
        {
            get
            {
                return this.dataFatField;
            }
            set
            {
                this.dataFatField = value;
            }
        }

        private int? tempCargaField;

        [System.Xml.Serialization.XmlElementAttribute()]
        public int? V_TempoCarga
        {
            get
            {
                return this.tempCargaField;
            }
            set
            {
                this.tempCargaField = value;
            }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_TempoCargaSpecified
        {
            get
            {
                return V_TempoCarga.HasValue;
            }
        }

        private int? tempoDescargaField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public int? V_TempoDescarga
        {
            get
            {
                return this.tempoDescargaField;
            }
            set
            {
                this.tempoDescargaField = value;
            }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_TempoDescargaSpecified
        {
            get
            {
                return V_TempoDescarga.HasValue;
            }
        }

        private string enviarEmailField;
        public string V_EnviarEmail
        {
            get
            {
                return this.enviarEmailField;
            }
            set
            {
                this.enviarEmailField = value;
            }
        }

        private string emailsAdicionaisField;
        public string V_EmailsAdicionais
        {
            get
            {
                return this.emailsAdicionaisField;
            }
            set
            {
                this.emailsAdicionaisField = value;
            }
        }

        private string enviarSMSField;
        public string V_EnviarSMS
        {
            get
            {
                return this.enviarSMSField;
            }
            set
            {
                this.enviarSMSField = value;
            }
        }

        private string smsAdicionaisField;
        public string V_SMSAdicionais
        {
            get
            {
                return this.smsAdicionaisField;
            }
            set
            {
                this.smsAdicionaisField = value;
            }
        }

        private decimal? pesoField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? V_Peso
        {
            get
            {
                return this.pesoField;
            }
            set
            {
                this.pesoField = value;
            }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_PesoSpecified
        {
            get
            {
                return V_Peso.HasValue;
            }
        }


        private string veiculoField;
        public string V_Veiculo
        {
            get
            {
                return this.veiculoField;
            }
            set
            {
                this.veiculoField = value;
            }
        }

        private string eqptoField;
        public string V_Eqpto
        {
            get
            {
                return this.eqptoField;
            }
            set
            {
                this.eqptoField = value;
            }
        }

        private int? frotaField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public int? V_Frota
        {
            get
            {
                return this.frotaField;
            }
            set
            {
                this.frotaField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_FrotaSpecified
        {
            get
            {
                return V_Frota.HasValue;
            }
        }

        private string carreta1Field;
        public string V_Carreta1
        {
            get
            {
                return this.carreta1Field;
            }
            set
            {
                this.carreta1Field = value;
            }
        }

        private string carreta2Field;
        public string V_Carreta2
        {
            get
            {
                return this.carreta2Field;
            }
            set
            {
                this.carreta2Field = value;
            }
        }

        private string carreta3Field;
        public string V_Carreta3
        {
            get
            {
                return this.carreta3Field;
            }
            set
            {
                this.carreta3Field = value;
            }
        }

        private string escolta1Field;
        public string V_Escolta1
        {
            get
            {
                return this.escolta1Field;
            }
            set
            {
                this.escolta1Field = value;
            }
        }


        private string escolta2Field;
        public string V_Escolta2
        {
            get
            {
                return this.escolta2Field;
            }
            set
            {
                this.escolta2Field = value;
            }
        }


        private string isca1Field;
        public string V_Isca1
        {
            get
            {
                return this.isca1Field;
            }
            set
            {
                this.isca1Field = value;
            }
        }

        private string isca2Field;
        public string V_Isca2
        {
            get
            {
                return this.isca2Field;
            }
            set
            {
                this.isca2Field = value;
            }
        }

        private string motoristaField;
        public string V_Motorista
        {
            get
            {
                return this.motoristaField;
            }
            set
            {
                this.motoristaField = value;
            }
        }

        private string senhaMotField;
        public string V_SenhaMot
        {
            get
            {
                return this.senhaMotField;
            }
            set
            {
                this.senhaMotField = value;
            }
        }


        private decimal? volumeField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? V_Volume
        {
            get
            {
                return this.volumeField;
            }
            set
            {
                this.volumeField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_VolumeSpecified
        {
            get
            {
                return V_Volume.HasValue;
            }
        }

        private int? unidadeMedidaField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public int? V_UnidadeMedida
        {
            get
            {
                return this.unidadeMedidaField;
            }
            set
            {
                this.unidadeMedidaField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_UnidadeMedidaSpecified
        {
            get
            {
                return V_UnidadeMedida.HasValue;
            }
        }

        private decimal? valorField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? V_Valor
        {
            get
            {
                return this.valorField;
            }
            set
            {
                this.valorField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_ValorSpecified
        {
            get
            {
                return V_Valor.HasValue;
            }
        }
        private decimal? freteField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public decimal? V_Frete
        {
            get
            {
                return this.freteField;
            }
            set
            {
                this.freteField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_FreteSpecified
        {
            get
            {
                return V_Frete.HasValue;
            }
        }

        private int procedimentoEmbarqueField;
        public int V_ProcedimentoEmbarque
        {
            get
            {
                return this.procedimentoEmbarqueField;
            }
            set
            {
                this.procedimentoEmbarqueField = value;
            }
        }

        private int? faixaTemperaturaField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public int? V_FaixaTemperatura
        {
            get
            {
                return this.faixaTemperaturaField;
            }
            set
            {
                this.faixaTemperaturaField = value;
            }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_FaixaTemperaturaSpecified
        {
            get
            {
                return V_FaixaTemperatura.HasValue;
            }
        }


        private string observacaoField;
        public string V_Observacao
        {
            get
            {
                return this.observacaoField;
            }
            set
            {
                this.observacaoField = value;
            }
        }

        private string idConsultaField;
        public string V_IdConsulta
        {
            get
            {
                return this.idConsultaField;
            }
            set
            {
                this.idConsultaField = value;
            }
        }

        private string fonteConsultaField;
        public string V_FonteConsulta
        {
            get
            {
                return this.fonteConsultaField;
            }
            set
            {
                this.fonteConsultaField = value;
            }
        }

        private string dataConsultaField;
        public string V_DataConsulta
        {
            get
            {
                return this.dataConsultaField;
            }
            set
            {
                this.dataConsultaField = value;
            }
        }

        private string unidadePagadoraField;
        public string V_UnidadePagadora
        {
            get
            {
                return this.unidadePagadoraField;
            }
            set
            {
                this.unidadePagadoraField = value;
            }
        }

        private int? eixosField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public int? V_Eixos
        {
            get
            {
                return this.eixosField;
            }
            set
            {
                this.eixosField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_EixosSpecified
        {
            get
            {
                return V_Eixos.HasValue;
            }
        }

        private bool? edicaoFinalizadaField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public bool? V_EdicaoFinalizada
        {
            get
            {
                return this.edicaoFinalizadaField;
            }
            set
            {
                this.edicaoFinalizadaField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_EdicaoFinalizadaSpecified
        {
            get
            {
                return V_EdicaoFinalizada.HasValue;
            }
        }

        private string transportadorField;
        public string V_Transportador
        {
            get
            {
                return this.transportadorField;
            }
            set
            {
                this.transportadorField = value;
            }
        }

        private string codigo_TransportadorField;
        public string V_Codigo_Transportador
        {
            get
            {
                return this.codigo_TransportadorField;
            }
            set
            {
                this.codigo_TransportadorField = value;
            }
        }

        private int modeloContratacaoField;
        public int V_Modelo_Contratacao
        {
            get
            {
                return this.modeloContratacaoField;
            }
            set
            {
                this.modeloContratacaoField = value;
            }
        }

        private string vinculoField;
        public string V_Vinculo
        {
            get
            {
                return this.vinculoField;
            }
            set
            {
                this.vinculoField = value;
            }
        }

        private string tipo_ConjuntoField;
        public string V_Tipo_Conjunto
        {
            get
            {
                return this.tipo_ConjuntoField;
            }
            set
            {
                this.tipo_ConjuntoField = value;
            }
        }

        private string tipo_ContratacaoField;
        public string V_Tipo_Contratacao
        {
            get
            {
                return this.tipo_ContratacaoField;
            }
            set
            {
                this.tipo_ContratacaoField = value;
            }
        }

        private string tipo_VendaField;
        public string V_Tipo_Venda
        {
            get
            {
                return this.tipo_VendaField;
            }
            set
            {
                this.tipo_VendaField = value;
            }
        }

        private string local_CarregamentoField;
        public string V_Local_Carregamento
        {
            get
            {
                return this.local_CarregamentoField;
            }
            set
            {
                this.local_CarregamentoField = value;
            }
        }

        private int? itinerarioField;
        [System.Xml.Serialization.XmlElementAttribute()]
        public int? V_Itinerario
        {
            get
            {
                return this.itinerarioField;
            }
            set
            {
                this.itinerarioField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_ItinerarioSpecified
        {
            get
            {
                return V_Itinerario.HasValue;
            }
        }

        private string zona_Transporte_DestinoField;
        public string V_Zona_Transporte_Destino
        {
            get
            {
                return this.zona_Transporte_DestinoField;
            }
            set
            {
                this.zona_Transporte_DestinoField = value;
            }
        }

        private string grade_MercadoriaField;
        public string V_Grade_Mercadoria
        {
            get
            {
                return this.grade_MercadoriaField;
            }
            set
            {
                this.grade_MercadoriaField = value;
            }
        }

        private int? quantidade_Produtos;
        [System.Xml.Serialization.XmlElementAttribute()]
        public int? V_Quantidade_Produtos
        {
            get
            {
                return this.quantidade_Produtos;
            }
            set
            {
                this.quantidade_Produtos = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool V_Quantidade_ProdutosSpecified
        {
            get
            {
                return V_Quantidade_Produtos.HasValue;
            }
        }

        private string data_Hora_CarregamentoField;
        public string V_Data_Hora_Carregamento
        {
            get
            {
                return this.data_Hora_CarregamentoField;
            }
            set
            {
                this.data_Hora_CarregamentoField = value;
            }
        }

        private string tipo_VeiculoField;
        public string V_Tipo_Veiculo
        {
            get
            {
                return this.tipo_VeiculoField;
            }
            set
            {
                this.tipo_VeiculoField = value;
            }
        }

        private string pedidoField;
        public string V_Pedido
        {
            get
            {
                return this.pedidoField;
            }
            set
            {
                this.pedidoField = value;
            }
        }

        private Entrega[] entregasField;

        [System.Xml.Serialization.XmlElement("Entrega")]
        public Entrega[] Entregas
        {
            get
            {
                return this.entregasField;
            }
            set
            {
                this.entregasField = value;
            }
        }

    }
}
