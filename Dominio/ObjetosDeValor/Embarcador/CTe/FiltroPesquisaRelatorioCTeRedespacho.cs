using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaRelatorioCTeRedespacho
    {
        public int CodigoEmpresaPai { get; set; }

        public int CodigoEmpresa { get; set; }

        public string CpfCnpjEmbarcador { get; set; }

        public string CpfCnpjEmbarcadorUsuario { get; set; }

        public string SerieEmissao { get; set; }

        public DateTime DataEmissaoInicial { get; set; }

        public DateTime DataEmissaoFinal { get; set; }

        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
    }
}