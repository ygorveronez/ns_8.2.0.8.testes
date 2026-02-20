using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCTesEmitidos
    {
        public int CODIGO { get; set; }

        public int CODIGO_CTE { get; set; }

        public int SERIE { get; set; }

        public DateTime DATA_EMISSAO_CTE { get; set; }

        public Dominio.Enumeradores.TipoCTE TIPO_CTE { get; set; }

        public string UF_EMPRESA { get; set; }

        public string NOME_CIDADE_INICIO { get; set; }

        public string UF_INICIO { get; set; }

        public string NOME_CIDADE_INICIO_EXTERIOR { get; set; }

        public string PAIS_INICIO { get; set; }

        public string NOME_CIDADE_FIM { get; set; }

        public string UF_FIM { get; set; }

        public string NOME_CIDADE_FIM_EXTERIOR { get; set; }

        public string PAIS_FIM { get; set; }

        public string NOME_RAZAO { get; set; }

        public string NOME_RAZAO_1 { get; set; }

        public decimal VALOR_RECEBER { get; set; }

        public decimal VALOR_SERVICO { get; set; }

        public decimal BASE_CALCULO_ICMS { get; set; }

        public decimal ALIQUOTA_ICMS { get; set; }

        public decimal VALOR_ICMS { get; set; }

        public decimal FRETE_ICMS { get; set; }

        public decimal VOLUME { get; set; }

        public decimal QUANTIDADE { get; set; }

        public decimal QUANTIDADE_TOTAL { get; set; }

        public string CST { get; set; }

        public string PLACA { get; set; }

        public string PLACAS_ADICIONAIS { get; set; }

        public bool REMETENTE_EXTERIOR { get; set; }

        public bool DESTINATARIO_EXTERIOR { get; set; }

        public string NOTAS_FISCAIS { get; set; }

        public string CHAVE { get; set; }

        public decimal VALOR_MERCADORIA { get; set; }

        public int CFOP { get; set; }

        public string LOCALIDADE_INICIO_PRESTACAO { get; set; }

        public string ESTADO_INICIO_PRESTACAO { get; set; }

        public string LOCALIDADE_TERMINO_PRESTACAO { get; set; }

        public string ESTADO_TERMINO_PRESTACAO { get; set; }

        public decimal VALOR_COMPONENTES { get; set; }

        public string TRANSPORTADOR { get; set; }

        public string CNPJ_TRANSPORTADOR { get; set; }

        public string CONDICAO_PAGAMENTO { get; set; }

        public string MODELO { get; set; }

        public string MOTORISTA { get; set; }

        public string NOME_RAZAO_TOMADOR { get; set; }

        public string NUMERO_DOCUMENTO { get; set; }

        public string Observacao { get; set; }

        public string Usuario { get; set; }

        public decimal ALIQUOTA_PIS { get; set; }

        public decimal VALOR_PIS { get; set; }

        public decimal ALIQUOTA_COFINS { get; set; }

        public decimal VALOR_COFINS { get; set; }

        public string MDFe { get; set; }

        public string PRODUTO_PREDOMINANTE { get; set; }

        public string NumeroCTeSubcontratado { get; set; }

        public string SerieCTeSubcontratado { get; set; }

        public int NumeroDuplicata { get; set; }

        public string CNPJ_REMETENTE { get; set; }
        public string CNPJ_DESTINATARIO { get; set; }
        public string ProtocoloCTe { get; set; }

        public string STATUS
        {
            get
            {
                switch (Status_Abrev)
                {
                    case "A":
                        return "Autorizados";
                    case "C":
                        return "Cancelados";
                    case "D":
                        return "Denegados";
                    case "S":
                        return "Em Digitação";
                    case "I":
                        return "Inutilizados";
                    case "R":
                        return "Rejeição";
                    case "P":
                        return "Pendentes";
                    case "E":
                        return "Enviados";
                    case "K":
                        return "Em Cancelamento";
                    case "L":
                        return "Em Inutilização";
                    default:
                        return "";
                }
            }
        }

        public string Status_Abrev;

        public Dominio.Enumeradores.TipoServico TIPO_SERVICO { get; set; }

        public string DescricaoTipoServico
        {
            get
            {
                switch (TIPO_SERVICO)
                {
                    case Enumeradores.TipoServico.Normal:
                        return "Normal";
                    case Enumeradores.TipoServico.Redespacho:
                        return "Redespacho";
                    case Enumeradores.TipoServico.RedIntermediario:
                        return "Red. Intermediário";
                    case Enumeradores.TipoServico.SubContratacao:
                        return "Subcontratação";
                    case Enumeradores.TipoServico.ServVinculadoMultimodal:
                        return "Serv. Vinculado Multimodal";
                    case Enumeradores.TipoServico.TransporteDePessoas:
                        return "Transporte de Pessoas";
                    case Enumeradores.TipoServico.TransporteDeValores:
                        return "Transporte de Valores";
                    case Enumeradores.TipoServico.ExcessoDeBagagem:
                        return "Excesso de Bagagem";
                    default:
                        return "";
                }
            }
        }
    }
}
