using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class FiltroPesquisaGestaoDocumento
    {
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public string Chave { get; set; }
        public List<MotivoInconsistenciaGestaoDocumento> MotivoInconsistenciaGestaoDocumento { get; set; }
        public List<SituacaoGestaoDocumento> SituacaoGestaoDocumento { get; set; }
        public List<int> CodigoEmpresa { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoOcorrencia { get; set; }
        public List<int> CodigoTipoOperacao { get; set; }
        public int CodigoUsuarioAprovador { get; set; }
        public List <int> CodigoCTe { get; set; }
        public int Serie { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public List<double> Tomador { get; set; }
        public double Remetente { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public bool RegistroComCarga { get; set; }
        public List<string> Chaves { get; set; }
        public List<string> NumeroNotasFiscais { get; set; }
        public List<string> NumeroPedidosClientes { get; set; }
        public List<string> ChavesNFe { get; set; }
    }
}
