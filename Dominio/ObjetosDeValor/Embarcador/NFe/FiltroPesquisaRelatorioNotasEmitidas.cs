using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class FiltroPesquisaRelatorioNotasEmitidas
    {
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int Serie { get; set; }
        public int CodigoAtividade { get; set; }
        public int CodigoNaturezaOperacao { get; set; }
        public double CnpjPessoa { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataProcessamento { get; set; }
        public DateTime DataSaida { get; set; }
        public Dominio.Enumeradores.StatusNFe Status { get; set; }
        public Dominio.Enumeradores.TipoEmissaoNFe? TipoEmissao { get; set; }
        public string Chave { get; set; }
        public int FormaEmissao { get; set; }
        public TipoNota TipoDocumento { get; set; }
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
        public int CodigoEmpresa { get; set; }
        public bool ExibirItens { get; set; }
        public List<int> CodigosUsuario { get; set; }
        public List<int> CodigosCFOP { get; set; }
    }
}
