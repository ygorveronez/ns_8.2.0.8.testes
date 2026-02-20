using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb
{
    public class envVeiculo
    {
        public string Placa { get; set; }
        public envProprietario Proprietario { get; set; }
        public envTipo Tipo { get; set; }
        public bool? Manutencao { get; set; }
        public envCaracteristicas Caracteristicas { get; set; }
        public envRastreador Rastreador { get; set; }
    }

    public class envProprietario
    {
        public string Nome { get; set; }
        public string CPF_CNPJ { get; set; }
        public string CodigoExterno { get; set; }
        public bool? Condutor { get; set; }
        public string Cidade { get; set; }
        public int? UF { get; set; }
        public envComplemento Complemento { get; set; }
        public envEndereco Endereco { get; set; }
        public envJuridicaComplemento JuridicaComplemento { get; set; }
    }


    public class envTipo
    {
        public string Nome { get; set; }
        public string Sigla { get; set; }
        public bool? Tracao { get; set; }
    }

    public class envCaracteristicas
    {
        public int? AnoFabricacao { get; set; }
        public int? AnoModelo { get; set; }
        public string Chassi { get; set; }
        public string Cor { get; set; }
        public string Frota { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
    }

    public class envRastreador
    {
        public string Numero { get; set; }
        public int? TecnologiaRastreamento { get; set; }
        public int? TipoComunicacao { get; set; }
    }

}
