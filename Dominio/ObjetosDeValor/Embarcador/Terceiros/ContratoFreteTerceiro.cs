using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Terceiros
{
    public class ContratoFreteTerceiro
    {
        public int Protocolo { get; set; }
        public decimal IRRF { get; set; }
        public decimal INSS { get; set; }
        public decimal SEST_SENAT { get; set; }
        public decimal Descontos { get; set; }
        public decimal ValePedagio { get; set; }
        public decimal SubcontratacaoTerceiro { get; set; }
        public decimal TarifaSaque { get; set; }
        public decimal TarifaTransferencia { get; set; }
        public List<Adicional> Adicionais { get; set; }
        public Terceiro Terceiro { get; set; }
        public Contratante Contratante { get; set; }
        public EmpresaContrato Remetente { get; set; }
        public EmpresaContrato Destinatario { get; set; }
        public List<VeiculoContrato> Veiculos { get; set; }
        public List<MotoristaContrato> Motoristas { get; set; }
        public decimal PesoCarga { get; set; }
        public decimal ValorCarga { get; set; }
        public decimal VolumesCarga { get; set; }
        public string RotaCarga { get; set; }
        public string DataContrato { get; set; }
        public string HoraEmissaoContrato { get; set; }
        public EmpresaContrato DestinatariosCTe { get; set; }
    }

    public class Adicional
    {
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public TipoJustificativa Tipo { get; set; }
    }

    public class Contratante
    {
        public string Endereco { get; set; }
        public string CNPJ { get; set; }
        public string RazaoSocial { get; set; }
        public string CIOT { get; set; }
        public string ProtocoloCIOT { get; set; }
        public string NumeroCarga { get; set; }
        public int Numero { get; set; }
    }

    public class Terceiro
    {
        public string Nome { get; set; }
        public string CPF_CNPJ { get; set; }
        public string Endereco { get; set; }
    }

    public class VeiculoContrato
    {
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Renavam { get; set; }
    }

    public class MotoristaContrato
    {
        public string NomeMotorista { get; set; }
        public string CPF { get; set; }
        public string CNH { get; set; }
    }

    public class EmpresaContrato
    {
        public string Municipio { get; set; }
        public string Nome { get; set; }
    }

    public class DestinatariosCTe
    {
        public string Nome { get; set; }
        public string CPF_CNPJ { get; set; }
        public string Endereco { get; set; }
    }


}
