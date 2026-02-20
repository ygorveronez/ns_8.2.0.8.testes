using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Terceiros
{
    public class ContratoFrete
    {
        public DateTime DataEmissao { get; set; }
        public bool ExibirVariacao { get; set; }
        public bool ConsiderarPedagioDescargaVariacao { get; set; }
        public string LeiSubcontratacao { get; set; }
        public int NumeroContrato { get; set; }
        public string NumeroCarga { get; set; }

        public string Empresa { get; set; }
        public string Operador { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairoEmpresa { get; set; }
        public string NumeroEmpresa { get; set; }
        public string ComplementoEmpresa { get; set; }
        public string LocalidadeEmpresa { get; set; }
        public string UFEmpresa { get; set; }
        public string TelefoneEmpresa { get; set; }


        public string Terceiro { get; set; }
        public string CPF_CNPJTerceiro { get; set; }
        public string NomeTerceiro { get; set; }
        public string EnderecoTerceiro { get; set; }
        public string TelefoneTerceiro { get; set; }
        public string CidadeTerceiro { get; set; }
        public string BancoTerceiro { get; set; }
        public string TitulaContaTerceiro { get; set; }
        public string TipoContaTerceiro { get; set; }
        public string AgenciaTerceiro { get; set; }
        public string ContaTerceiro { get; set; }
        public string VencimentoTerceiro { get; set; }
        public string NumeroCartaoTerceiro { get; set; }

        public string Placas { get; set; }
        public string ModeloVeicula { get; set; }
        public string Renavam { get; set; }
        public string RNTRC { get; set; }
        public string AnoModelo { get; set; }

        public string Motorista { get; set; }
        public string CPFMotorista { get; set; }
        public string CNHMotorista { get; set; }
        public string NumeroCartaoMotorista { get; set; }
        public string TipoChavePixMotorista { get; set; }
        public string ChavePixMotorista { get; set; }

        public string ObservacaoAdiantamento { get; set; }
        public string TextoAdicional { get; set; }

        public decimal ValorSubcontratacao { get; set; }
        public decimal ValorPedagio { get; set; }
        public decimal ValorDescarga { get; set; }
        public decimal OutrosDescontos { get; set; }
        public decimal IRRF { get; set; }
        public decimal INSS { get; set; }
        public decimal SEST_SENAT { get; set; }

        public decimal LiquidoSemAdiantamento { get; set; }
        public decimal Adiantamento { get; set; }
        public decimal Abastecimento { get; set; }
        public decimal SaldoReceber { get; set; }


        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string MunicipioOrigem { get; set; }
        public string MunicipioDestino { get; set; }
        public string TipoPagamento { get; set; }


        public decimal ValorFreteIngresso { get; set; }

        public decimal ValorFretePago { get; set; }
        public decimal SaldoVariacao { get; set; }
        public string PercentualVariacao { get; set; }

        public string ProtocoloAutorizacaoCIOT { get; set; }
        public string NumeroCIOT { get; set; }
        public bool PossuiCIOT { get; set; }

        public string CentroResultado { get; set; }
        
    }
}
