using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class FiltroPesquisaCTeSemCarga
    {
        public int CodigoEmpresa { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public DateTime? DataEmissaoInicial { get; set; }
        public DateTime? DataEmissaoFinal { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public string StatusCTe { get; set; }
        public double CodigoRemetente { get; set; }
        public int NumeroNF { get; set; }
        public string ChaveNF { get; set; }
    }
}
