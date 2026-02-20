using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink
{

    [XmlRoot(ElementName = "TeleEvento")]
    public class TeleEvento
    {
        // Número do último televento processado no IASWS
        [XmlElement(ElementName = "NumeroSequenciaCtrl")]
        public long NumeroSequenciaCtrl { get; set; }

        // Número do último televento processado no IASWS
        [XmlElement(ElementName = "NumeroSequencia")]
        public long NumeroSequencia { get; set; }

       // // Id Sequencia gerado pelo sistema
       // [XmlElement(ElementName = "IdSeqMsg")]
       // public int IdSeqMsg { get; set; }

       // // Base de comunicacao que recebeu o teleevento
       // [XmlElement(ElementName = "Origem")]
       // public string Origem { get; set; }

       // // Modulo de Rastreamento destino do teleevento
       // [XmlElement(ElementName = "Destino")]
       // public string Destino { get; set; }

       // //// Tipo da Mensagem (0 = Controle, 1 = Normal) 
       // //[XmlElement(ElementName = "TipoMsg")]
       // //public int TipoMsg { get; set; }

       // //// Identificacao da Mensagem
       // //[XmlElement(ElementName = "CodMsg")]
       // //public string CodMsg { get; set; }

       // Data e hora da emissao do teleevento (UTC/GMT00:00)
       [XmlElement(ElementName = "DataHoraEmissao")]
       public string DataHoraEmissao { get; set; }
       public DateTime? DataHoraEmissaoDT { get 
           {
               if (string.IsNullOrWhiteSpace(DataHoraEmissao))
                   return null;
               else
                   return DateTime.Parse(DataHoraEmissao).ToLocalTime(); 
           } 
       }

       // // Prioridade do teleevento (nao utilizado)
       // //[XmlElement(ElementName = "Prioridade")]
       // //public int Prioridade { get; set; }
       // //
       // //// Tamanho da mensagem
       // //[XmlElement(ElementName = "TamanhoMensagem")]
       // //public int TamanhoMensagem { get; set; }

        // Identificacao do rastreador como cadastrado na central(com ate 6 digitos) em hexadecimal
        [XmlElement(ElementName = "IdTerminal")]
        public string IdTerminal { get; set; }
        public int IdTerminalI { get 
            {
                if (string.IsNullOrWhiteSpace(IdTerminal))
                    return 0;
                else
                    return Int32.Parse(IdTerminal, System.Globalization.NumberStyles.HexNumber); 
            }
        }
        public int NumeroSerie { get { return IdTerminalI; } }

       // // Versão do protocolo
       // [XmlElement(ElementName = "Versao_Protocolo")]
       // public int Versao_Protocolo { get; set; }

       // /*
       // Estado do Rastreador:
       // 0 = Desativado,
       // 1 = Em Local Autorizado,
       // 2 = Em Transito,
       // 3 = Interrompido,
       // 4 = Bloqueado,
       // 5 = Em Manobra,
       // 6 = Rastreado
       // */
       // [XmlElement(ElementName = "StatusVeic")]
       // public int StatusVeic { get; set; }

        // Data e hora da geração do teleevento pelo rastreador (UTC/GMT00:00)
        [XmlElement(ElementName = "DataHoraEvento")]
        public string DataHoraEvento { get; set; }
        public DateTime? DataHoraEventoDT { get 
            {
                if (string.IsNullOrWhiteSpace(DataHoraEvento))
                    return null;
                else
                    return DateTime.Parse(DataHoraEvento).ToLocalTime();
            } 
        }

       // /*
       // Estado da Ignição do Veiculo:
       // 0 = Desligada,
       // 1 = Ligada,
       // 2 = Indefinida
       // */
        [XmlElement(ElementName = "Ignicao")]
        public int Ignicao { get; set; }

       // /*
       // Estado do GPS no momento da geracao do teleevento:
       // 0 = OK,
       // 1 = Sem Visada,
       // 2 = Sendo Iniciado,
       // 3 = Nao Iniciado,
       // 4 = Nao ha posicao disponivel -
       // */
       // [XmlElement(ElementName = "Validade")]
       // public int Validade { get; set; }

       // /*
       // Sentido do veiculo no momento da geração do teleevento:
       // 0 = Norte,
       // 1 = Nordeste,
       // 2 = Leste,
       // 3 = Sudeste,
       // 4 = Sul,
       // 5 = Sudoeste,
       // 6 = Oeste,
       // 7 = Noroeste
       // */
       // [XmlElement(ElementName = "Rumo")]
       // public int Rumo { get; set; }

        // Velocidade do veiculo em Km/h (0..255)
        [XmlElement(ElementName = "Velocidade")]
        public int Velocidade { get; set; }

        // Latitude em graus(Notacao texto. Ex: 023_32_13_0_S) Onde: 023 = Graus, 32 = min, 13 = seg, 0 = décimos de segundo e S = orientação
        [XmlElement(ElementName = "Latitude")]
        public string Latitude { get; set; }
        //public double Latitude { get { return Convert.ToDouble(LatStr); } }

        // Longitude em graus (Notação texto. Ex. 046_36_40_0_W) Onde: 046 = Graus, 36 = min, 40 = seg, 0 = décimos de seg e W = orientacao
        [XmlElement(ElementName = "Longitude")]
        public string Longitude { get; set; }
        //public double Longitude { get { return Convert.ToDouble(LngStr); } }

       // // Valor do Hodômetro em metros
       // [XmlElement(ElementName = "Hodometro")]
       // public int Hodometro { get; set; }

       // /*
       // Intervalo de envio de posição automática pelo rastreador:
       // 0 = nao enviar,
       // 1 = 1 minuto,
       // 2 = 2 minutos,
       // 3 = 3 minutos,
       // 4 = 5 minutos,
       // 5 = 10 minutos,
       // 6 = 15 minutos,
       // 7 = 20 minutos,
       // 8 = 30 minutos,
       // 9 = 1 hora,
       // 10 = 2 horas,
       // 11 = 3 horas,
       // 12 = 5 horas,
       // 13 = 6 horas,
       // 14 = 12 horas,
       // 15 = 1 dia 
       // */
       // [XmlElement(ElementName = "Intervalo")]
       // public int Intervalo { get; set; }

       // // Intervalo de envio de posição automática pelo rastreador dentro de cercas(veja<Intervalo>)
       // [XmlElement(ElementName = "IntervaloDif")]
       // public int IntervaloDif { get; set; }

       // /*
       // Assume os valores
       // 0 = Não Lacrado
       // 1 = Lacrado pelo Motorista
       // 2 = Lacrado pela Central
       // */
       // [XmlElement(ElementName = "LacreCarreta")]
       // public int LacreCarreta { get; set; }

       // /*
       // Assume os valores
       // 0 = Não Lacrado
       // 1 = Lacrado pelo Motorista
       // 2 = Lacrado pela Central
       // */
       // [XmlElement(ElementName = "LacreCabine")]
       // public int LacreCabine { get; set; }

       // /*
       // Assume os valores
       // 0 = Não Lacrado
       // 1 = Lacrado pelo Motorista
       // 2 = Lacrado pela Central
       // */
       // [XmlElement(ElementName = "LacreBau")]
       // public int LacreBau { get; set; }

       // /*
       // Assume os valores
       // 0 = Não
       // 1 = Sim
       // */
       // [XmlElement(ElementName = "FalhaAbend")]
       // public int FalhaAbend { get; set; }

       // /*
       // Assume os valores
       // 0 = Não
       // 1 = Sim
       // */
       // [XmlElement(ElementName = "FalhaFlash")]
       // public int FalhaFlash { get; set; }

       // /*
       // Assume os valores
       // 0 = Não
       // 1 = Sim
       // */
       // [XmlElement(ElementName = "HodoInop")]
       // public int HodoInop { get; set; }

       // /*
       // Assume os valores
       // 0 = Não
       // 1 = Sim
       // */
       // [XmlElement(ElementName = "PerdaGPS")]
       // public int PerdaGPS { get; set; }

       // /*
       // Assume os valores
       // 0 = Não pressionado
       // 1 = pressionado
       // */
       // [XmlElement(ElementName = "BotaoPanico")]
       // public int BotaoPanico { get; set; }

       // /*
       // Assume os valores
       // 0 = Aberta
       // 1 = Fechada
       // */
       // [XmlElement(ElementName = "PortaBau")]
       // public int PortaBau { get; set; }

       // /*
       // Assume os valores
       // 0 = Aberta
       // 1 = Fechada
       // */
       // [XmlElement(ElementName = "PortaDireita")]
       // public int PortaDireita { get; set; }

       // /*
       // Assume os valores
       // 0 = Aberta
       // 1 = Fechada
       // */
       // [XmlElement(ElementName = "PortaEsquerda")]
       // public int PortaEsquerda { get; set; }

       // /*
       // Assume os valores
       // 0 = Não engatada
       // 1 = Engatada
       // */
       // [XmlElement(ElementName = "EngateCarreta")]
       // public int EngateCarreta { get; set; }

       // /*
       // Assume os valores
       // 0 = Não acionada
       // 1 = Acionada
       // */
       // [XmlElement(ElementName = "ChaveDesbloqueio")]
       // public int ChaveDesbloqueio { get; set; }

       // /*
       // Assume os valores
       // 0 = Não pressionado
       // 1 = Pressionado
       // 2 = Não instalado 
       // */
       // [XmlElement(ElementName = "BotaoBau")]
       // public int BotaoBau { get; set; }

       // /*
       // Assume os valores
       // 0 = Desconectado 
       // 1 = Conectado
       // */
       // [XmlElement(ElementName = "EstadoTerminal")]
       // public int EstadoTerminal { get; set; }

       // /*
       // Assume os valores 
       // 0 = Não
       // 1 = Sim
       // 1 = Não instalada
       // */
       // [XmlElement(ElementName = "FlagFalhaTravaMot")]
       // public int FlagFalhaTravaMot { get; set; }

       // /*
       //Assume os valores 
       //0 = Não
       //1 = Sim
       //1 = Não instalada
       //*/
       // [XmlElement(ElementName = "FalhaTravaMot")]
       // public int FalhaTravaMot { get; set; }

       // /*
       // Assume os valores
       // 0 = Não
       // 1 = Sim
       // */
       // [XmlElement(ElementName = "BatExtOut")]
       // public int BatExtOut { get; set; }

       // /*
       // Assume os valores
       // 0 = Não
       // 1 = Sim
       // */
       // [XmlElement(ElementName = "BatIntOut")]
       // public int BatIntOut { get; set; }

       // /*
       // Assume os valores
       // 0 = Não acionada
       // 1 = Acionada
       // */
       // [XmlElement(ElementName = "ChaveArmadilha")]
       // public int ChaveArmadilha { get; set; }

       // /*
       // Assume os valores
       // 0 = Não
       // 1 = Sim
       // */
       // [XmlElement(ElementName = "Historico")]
       // public int Historico { get; set; }

       // /*
       // Informa a tecnologia utilizada na comunicação do rastreador
       // 0 = Comunicação discada
       // 1 = Comunicação via mensagem de texto
       // 2 = Comunicação celular
       // 3 = Comunicação satélite
       // */
       // [XmlElement(ElementName = "Tecnologia")]
       // public int Tecnologia { get; set; }

       // // Data e hora da conexão (valido apenas para comunicação discada)
       // [XmlElement(ElementName = "DataHoraCnx")]
       // public string DataHoraCnx { get; set; }
       // public DateTime? DataHoraCnxDT { get 
       //     {
       //         if (string.IsNullOrWhiteSpace(DataHoraCnx))
       //             return null;
       //         else
       //             return DateTime.Parse(DataHoraCnx); 
       //     } 
       // }

       // // Serial
       // [XmlElement(ElementName = "Serial")]
       // public string Serial { get; set; }

       // // Id seqüencial gerado pelo rastreador quando a comunicação foi gerada
       // [XmlElement(ElementName = "IdSeqVeiculo")]
       // public int IdSeqVeiculo { get; set; }

       // // IP
       // [XmlElement(ElementName = "IP")]
       // public string IP { get; set; }

       // // Porta
       // [XmlElement(ElementName = "Port")]
       // public int Port { get; set; }

       // /*
       // Assume os valores
       // 0 = Padrão
       // 1 = Diferenciado
       // */
       // [XmlElement(ElementName = "Intervalo_OP")]
       // public int Intervalo_OP { get; set; }

       // /*
       // Caso TecnologiaIntervalo = 2 (Celular) devolve um tempo em segungos
       // Caso TecnologiaIntervalo = 3 (Satelite) devolve os seguintes valores
       // 0 = Nunca
       // 1 = 5 min
       // 2 = 10 min
       // 3 = 15 min
       // 4 = 20 min
       // 5 = 30 min
       // 6 = 45 min
       // 7 = 1 h
       // 8 = 1:30 h
       // 9 = 2 h
       // 10 = 3 h
       // 11 = 5 h
       // 12 = 6 h
       // 13 = 12 h
       // 14 = 1 dia
       // 15 = ñ def
       // */
       // [XmlElement(ElementName = "Intervalo_IP_SMS")]
       // public int Intervalo_IP_SMS { get; set; }

       // /*
       // Assume os valores
       // 2 = Celular
       // 3 = Satelite
       // */
       // [XmlElement(ElementName = "TecnologiaIntervalo")]
       // public int TecnologiaIntervalo { get; set; }

       // /*
       // Validade da data hora da geração do teleevento via satélite
       // 0 = válida
       // 1 = inválida
       // */
       // [XmlElement(ElementName = "UsandoDataHoraLES")]
       // public int UsandoDataHoraLES { get; set; }

       // // DataHora LES (Skywave e SecTrack apenas para teleeventos) 
       // [XmlElement(ElementName = "DataHoraLES")]
       // public string DataHoraLES { get; set; }
       // public DateTime? DataHoraLESDT { get
       //     {
       //         if (string.IsNullOrWhiteSpace(DataHoraLES))
       //             return null;
       //         else
       //             return DateTime.Parse(DataHoraLES);
       //     } 
       // }

       // /*
       // Operadora:
       // 0 = Não Informado,
       // 1 = Tim,
       // 2 = Claro,
       // 3 = Oi,
       // 100 = Skywave,
       // 101 = Sectrack,
       // 102 = Iridium
       // */
       // [XmlElement(ElementName = "Operadora")]
       // public int Operadora { get; set; }

       // /*
       // Modelo do Rastreador:
       // 0 = Não Informado,
       // 1 = RI 1450,
       // 2 = RI 1460 MAX (Skywave),
       // 3 = RI 1460 MAX (Sectrack),
       // 4 = RI 1480 MAX,
       // 5 = RI 1454 MAX
       // 6 = RI 4464
       // 7 = RI 4484
       // 8 = RI 4454
       // */
       // [XmlElement(ElementName = "ModeloRastreador")]
       // public int ModeloRastreador { get; set; }

        // Distância até o Ponto de Referência cadastrado mais próximo ou o Centro da Cidade, caso não haja Ponto de Referência ou este esteja mais próximo da localização atual – Por padrão essa token vem desabilitada
        [XmlElement(ElementName = "Localizacao")]
        public string Localizacao { get; set; }

        // Cidade mais próximo da localização atual – Por padrão essa token vem desabilitada
        [XmlElement(ElementName = "Cidade")]
        public string Cidade { get; set; }

        // UF mais próximo da localização atual – Por padrão essa token vem desabilitada
        [XmlElement(ElementName = "UF")]
        public string UF { get; set; }

       // // Tipo do Evento Recebido (decimal)
       // [XmlElement(ElementName = "Evento")]
       // public string Evento { get; set; }

       // /*
       // Validade da Temperatura:
       // 0: Leitura Inválida
       // 1: Leitura Válida
       // */
       // [XmlElement(ElementName = "Temperatura_Validade")]
       // public int Temperatura_Validade { get; set; }

       // /*
       // Controle de Temperatura:
       // 0: Desabilitado
       // 1: Habilitado 
       // */
       // [XmlElement(ElementName = "Temperatura_Controle")]
       // public int Temperatura_Controle { get; set; }

       // /*
       // Meio de Comunicação no qual o rastreador gerou o teleevento de temperatura:
       // 0: Celular
       // 1: SMS
       // 2: Discado
       // */
       // [XmlElement(ElementName = "Temperatura_Tecnologia")]
       // public int Temperatura_Tecnologia { get; set; }

       // // Freqüência configurada no rastreador para envio de teleeventos de temperatura, em segundos
       // [XmlElement(ElementName = "Temperatura_Intervalo")]
       // public int Temperatura_Intervalo { get; set; }

        // Valor da Temperatura em ponto flutuante, com precisão de 1 (uma) casa decimal
        [XmlElement(ElementName = "Temperatura_Valor")]
        public string Temperatura_Valor { get; set; }
        public decimal? Temperatura_ValorD { 
            get {
                if (string.IsNullOrWhiteSpace(Temperatura_Valor)) return null;
                try
                {
                    return Utilidades.Decimal.Converter(Temperatura_Valor);
                }
                catch (FormatException ex)
                {
                    NumberFormatInfo provider = new NumberFormatInfo();
                    provider.NumberDecimalSeparator = ",";
                    return Convert.ToDecimal(Temperatura_Valor, provider);
                }
            } 
        }

       // /*
       // Nível de importância do teleevento reportando pelo rastreador, de acordo com a configuração recebida da Central SaverTurbo:
       // 1: Normal (nenhuma luz é acesa na Torre de Alertas da Central)
       // 2: Alerta (luz amarela piscante)
       // 3: Alarme (luz vermelha piscante)
       // */
       // [XmlElement(ElementName = "Temperatura_AcaoAlarme")]
       // public int Temperatura_AcaoAlarme { get; set; }

       // /*
       // Código de Ocorrência na Leitura da Temperatura:
       // 0: Normal
       // 1: Defeito no Sensor
       // 2: Temperatura Máxima Excedida
       // 3: Temperatura Mínima Excedida
       // 4: Temperatura Automática
       // 5: Temperatura Avulsa
       // 6: Temperatura do Baú Desceu para Dentro dos Limites de operação
       // 7: Temperatura do Baú Subiu para Dentro dos Limites de operação
       // 8: Funcionamento Normal do Sensor 
       // */
        [XmlElement(ElementName = "Temperatura_Codigo")]
        public int Temperatura_Codigo { get; set; }

    }

}
