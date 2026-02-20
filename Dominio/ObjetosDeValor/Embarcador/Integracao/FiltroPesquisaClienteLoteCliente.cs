using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class FiltroPesquisaClienteLoteCliente
    {
        public int CodigoLote { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataLimite { get; set; }
        public bool? SelecionarTodos { get; set; }
        public List<double> CodigosSelecionados { get; set; }
        public string RazaoSocial { get; set; }
        public string CNPJ { get; set; }
        public string InscricaoEstadual { get; set; }
        public string Endereco { get; set; }
        public int Localidade { get; set; }
    }
}
