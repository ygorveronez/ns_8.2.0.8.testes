using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaMDFesDisponiveisGeracaoCargaEmbarcador
    {
        public int NumeroMDFe { get; set; }
        public string PlacaVeiculo { get; set; }
        public string NomeMotorista { get; set; }
        public int CodigoLocalidadeOrigem { get; set; }
        public int CodigoLocalidadeDestino { get; set; }
        public string UFOrigem { get; set; }
        public string UFDestino { get; set; }
        public int CodigoEmpresa { get; set; }
        public DateTime? DataEmissaoInicial { get; set; }
        public DateTime? DataEmissaoFinal { get; set; }
        public int Serie { get; set; }
        public bool? ProblemaGeracaoCargaAutomaticamente { get; set; }
        public bool? NaoGerarCargaAutomaticamente { get; set; }
    }
}
