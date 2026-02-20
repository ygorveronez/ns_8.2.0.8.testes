using Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb
{
    public class envPontoControle
    {
        public string Nome { get; set; }
        public envPessoa Pessoa { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class envPessoa
{
        public string Nome { get; set; }
        public string CPF_CNPJ { get; set; }
        public string CodigoExterno { get; set; }
        public bool? Condutor { get; set; }
        public string Cidade { get; set; }
        public int? UF { get; set; }
        public envComplemento Complemento { get; set; }
        public envEndereco Endereco { get; set; }
    }
}
