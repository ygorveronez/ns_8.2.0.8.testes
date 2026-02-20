using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad
{
    public class envCIOTPadrao
    {
        /// <summary>
        /// Data de início
        /// </summary>
        public string start_date { get; set; }

        /// <summary>
        /// Data de término
        /// </summary>
        public string end_date { get; set; }

        /// <summary>
        /// Identificação do ciot no cliente
        /// </summary>
        public string client_id { get; set; }

        /// <summary>
        /// Identificação da empresa
        /// </summary>
        public string office_id { get; set; }

        /// <summary>
        /// Dados dos contratantes
        /// </summary>
        public envCIOTPadraoHireds hireds { get; set; }

        /// <summary>
        /// Veículos utilizados no ciot
        /// </summary>
        public List<envCIOTPadraoVehicles> vehicles { get; set; }

        /// <summary>
        /// Impostos do ciot
        /// </summary>
        public List<envCIOTPadraoTaxes> taxes { get; set; }

        /// <summary>
        /// Eventos utilizados no ciot
        /// </summary>
        public List<envCIOTPadraoEvents> events { get; set; }

        /// <summary>
        /// Todos os pagamentos relacionados ao ciot
        /// </summary>
        public envCIOTPadraoPayment payment { get; set; }

        /// <summary>
        /// Valor do combustível (base x100)
        /// </summary>
        public decimal fuel_value_money { get; set; }

        /// <summary>
        /// Valor total do ciot (base x100)
        /// </summary>
        public decimal gross_value_money { get; set; }

        /// <summary>
        /// Observações
        /// </summary>
        public string note { get; set; }

        /// <summary>
        /// Chave única por integrador
        /// </summary>
        public string integrated_by { get; set; }

        /// <summary>
        /// <para>Responsável pelo recebimento do pagamento das parcelas do CIOT:</para>
        /// <para>"owner"</para>
        /// <para>"driver"</para>
        /// </summary>
        public string payment_receiver { get; set; }

        /// <summary>
        /// Tipo de emissão
        /// </summary>
        public string type { get; } = "default";

        /// <summary>
        /// Endereço do primeiro ponto de coleta
        /// </summary>
        public envCIOTPadrao_Address origin_address { get; set; }

        /// <summary>
        /// Endereço do último ponto de entrega
        /// </summary>
        public envCIOTPadrao_Address destination_address { get; set; }

        /// <summary>
        /// Dados dos interessados
        /// </summary>
        public envCIOTPadraoInterested_persons interested_persons { get; set; }

        /// <summary>
        /// Cargas do ciot
        /// </summary>
        public List<envCIOTPadraoCargoes> cargoes { get; set; }
    }

    public class envCIOTPadraoHireds
    {
        /// <summary>
        /// Dados do proprietário
        /// </summary>
        public envCIOTPadraoHiredsOwner owner { get; set; }

        /// <summary>
        /// Dados do subcontratante
        /// </summary>
        public envCIOTPadraoHiredsSubcontractor subcontractor { get; set; }

        /// <summary>
        /// Dados dos motoristas
        /// </summary>
        public List<envCIOTPadraoHiredsDrivers> drivers { get; set; }
    }

    public class envCIOTPadraoHiredsOwner
    {
        /// <summary>
        /// Nome do contratado
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Número do documento (CPF, CNPJ, RG, IE, CNH e Outros)
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// RNTRC
        /// </summary>
        public string rntrc { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        public string telephone_number { get; set; }
    }

    public class envCIOTPadraoHiredsSubcontractor
    {
        /// <summary>
        /// Nome do contratado
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Número do documento (CPF, CNPJ, RG, IE, CNH e Outros)
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// RNTRC
        /// </summary>
        public string rntrc { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        public string telephone_number { get; set; }
    }

    public class envCIOTPadraoHiredsDrivers
    {
        /// <summary>
        /// Nome do contratado
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Número do documento (CPF, CNPJ, RG, IE, CNH e Outros)
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// RNTRC
        /// </summary>
        public string rntrc { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        public string telephone_number { get; set; }
    }

    public class envCIOTPadraoVehicles
    {
        /// <summary>
        /// Placa do veículo
        /// </summary>
        public string plate { get; set; }

        /// <summary>
        /// Categoria do veículo
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// RNTRC do veículo
        /// </summary>
        public string rntrc { get; set; }
    }

    public class envCIOTPadraoTaxes
    {
        /// <summary>
        /// <para>Impostos:</para>
        /// <para>"ir"</para>
        /// <para>"inss"</para> 
        /// <para>"sest_senat"</para> 
        /// <para>"issqn"</para> 
        /// <para>"others"</para> 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Valor do imposto (base x100)
        /// </summary>
        public int value_money { get; set; }

        /// <summary>
        /// <para>"active"</para> 
        /// <para>"deleted"</para> 
        /// </summary>
        public string status { get; set; }
    }

    public class envCIOTPadraoEvents
    {
        /// <summary>
        /// <para>Tipo Evento:</para>
        /// <para>"addition"</para>
        /// <para>"discount"</para>
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// <para>Descrição Evento:</para>
        /// <para>"shipping_payment_mistake"</para>
        /// <para>"toll"</para>
        /// <para>"tracker"</para>
        /// <para>"insurance_claim"</para>
        /// <para>"ppe_uniform"</para>
        /// <para>"financial_voucher"</para>
        /// <para>"paper_voucher"</para>
        /// <para>"load_and_discharge"</para>
        /// <para>"overnight_stay"</para>
        /// <para>"difference_payment"</para>
        /// <para>"others"</para>
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Valor do imposto (base x100)
        /// </summary>
        public decimal value_money { get; set; }

        /// <summary>
        /// <para>"active"</para> 
        /// <para>"deleted"</para> 
        /// </summary>
        public string status { get; set; }
    }

    public class envCIOTPadraoPayment
    {
        /// <summary>
        /// <para>bbc = "Pagamento em Conta BBC"</para>
        /// <para>deposit = "Depósito em conta"</para>
        /// <para>pix = "Pagamento via PIX"</para>
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Parcelas
        /// </summary>
        public List<envCIOTPadraoPaymentInstallments> installments { get; set; }

        /// <summary>
        /// Dados bancários
        /// </summary>
        public envCIOTPadraoPaymentBank_detail bank_detail { get; set; }
    }

    public class envCIOTPadraoPaymentInstallments
    {
        /// <summary>
        /// <para>Tipo Parcela:</para>
        /// <para>"addition" = Acrescimo</para>
        /// <para>"advance" = Adiantamento</para>
        /// <para>"final_balance" = Saldo final</para>
        /// <para>"stay" = Estadia</para>
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Valor da parcela (base x100)
        /// </summary>
        public int value_money { get; set; }

        /// <summary>
        /// <para>Tipo de efetividade da parcela:</para>
        /// <para>"manual"</para>
        /// <para>"automatic"</para>
        /// </summary>
        public string effectiveness { get; set; }

        /// <summary>
        /// <para>pending = "Pendente"</para>
        /// <para>released = "Pago"</para>
        /// <para>bank_processing = "Processando no banco"</para>
        /// <para>error = "Erro"</para>
        /// <para>deleted = "Pago"</para>
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Identificação opcional da parcela
        /// </summary>
        public string identification { get; set; }

        /// <summary>
        /// Endereço opcional de origem
        /// </summary>
        public string origin_address { get; set; }

        /// <summary>
        /// Endereço opcional de destino
        /// </summary>
        public string destination_address { get; set; }

        /// <summary>
        /// ID externo da parcela que o cliente utiliza
        /// </summary>
        public string external_client_id { get; set; }

        /// <summary>
        /// Pagamento flexível
        /// </summary>
        public envCIOTPadraoPaymentInstallmentsFlexible_payment flexible_payment { get; set; }
    }

    public class envCIOTPadraoPaymentInstallmentsFlexible_payment
    {
        /// <summary>
        /// Chave PIX
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// <para>Recebedor do pagamento:</para>
        /// <para>"owner"</para>
        /// <para>"driver"</para>
        /// </summary>
        public string receiver { get; set; }

        /// <summary>
        /// <para>Tipo de pagamento flexível:</para>
        /// <para>"bbc"</para>
        /// <para>"pix"</para>
        /// </summary>
        public string type { get; set; }
    }

    public class envCIOTPadraoPaymentBank_detail
    {
        /// <summary>
        /// Número do banco
        /// </summary>
        public string bank { get; set; }

        /// <summary>
        /// Número da agência
        /// </summary>
        public string agency { get; set; }

        /// <summary>
        /// Número da conta
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// <para>Tipo de conta</para>
        /// <para>"current"</para>
        /// <para>"saving"</para>
        /// </summary>
        public string account_type { get; set; }
    }

    public class envCIOTPadraoInterested_persons
    {
        /// <summary>
        /// Dados do destinatário
        /// </summary>
        public envCIOTPadraoInterested_personsRecipient recipient { get; set; }

        /// <summary>
        /// Dados do tomador
        /// </summary>
        public envCIOTPadraoInterested_personsService_taker service_taker { get; set; }

        /// <summary>
        /// Dados do consignatário
        /// </summary>
        public envCIOTPadraoInterested_personsConsignee consignee { get; set; }

        /// <summary>
        /// Dados do remetente
        /// </summary>
        public envCIOTPadraoInterested_personsSender sender { get; set; }
    }

    public class envCIOTPadraoInterested_personsRecipient
    {
        /// <summary>
        /// Nome do interessado
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Número do documento (CPF, CNPJ, RG, IE, CNH e Outros)
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Endereço
        /// </summary>
        public envCIOTPadrao_Address address { get; set; }
    }

    public class envCIOTPadraoInterested_personsService_taker
    {
        /// <summary>
        /// Nome do interessado
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Número do documento (CPF, CNPJ, RG, IE, CNH e Outros)
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Endereço
        /// </summary>
        public envCIOTPadrao_Address address { get; set; }
    }

    public class envCIOTPadraoInterested_personsConsignee
    {
        /// <summary>
        /// Nome do interessado
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Número do documento (CPF, CNPJ, RG, IE, CNH e Outros)
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Endereço
        /// </summary>
        public envCIOTPadrao_Address address { get; set; }
    }

    public class envCIOTPadraoInterested_personsSender
    {
        /// <summary>
        /// Nome do interessado
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Número do documento (CPF, CNPJ, RG, IE, CNH e Outros)
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Endereço
        /// </summary>
        public envCIOTPadrao_Address address { get; set; }
    }

    public class envCIOTPadrao_Address
    {
        /// <summary>
        /// Rua
        /// </summary>
        public string street { get; set; }

        /// <summary>
        /// Número
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Complemento
        /// </summary>
        public string complement { get; set; }

        /// <summary>
        /// Bairro
        /// </summary>
        public string neighborhood { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// Estado
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// País
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// CEP
        /// </summary>
        public string zip_code { get; set; }

        /// <summary>
        /// IBGE
        /// </summary>
        public string city_code { get; set; }
    }

    public class envCIOTPadraoCargoes
    {
        /// <summary>
        /// Peso da carga
        /// </summary>
        public decimal load_weight { get; set; }

        /// <summary>
        /// Natureza da carga
        /// </summary>
        public string nature_code { get; set; }
    }
}