using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga;

public class FiltroPesquisaMontarConsultaCtes
{
    public int Carga { get; set; }
    public int NumeroDocumento { get; set; }
    public int NumeroNF { get; set; }
    public string[] StatusCTe { get; set; }
    public bool ApenasCTesNormais { get; set; }
    public bool CtesSubContratacaoFilialEmissora { get; set; }
    public bool CtesSemSubContratacaoFilialEmissora { get; set; }
    public List<int> EmpresasFilialEmissora { get; set; }
    public string ProprietarioVeiculo { get; set; }
    public double Destinatario { get; set; }
    public bool BuscarPorCargaOrigem { get; set; }
    public bool RetornarPreCtes { get; set; }
    public List<Dominio.Enumeradores.TipoDocumento> TiposDocumentosDoCte { get; set; }
    public bool CTesFactura { get; set; }
    public bool CargaMercosul { get; set; }
    public bool EmitirDocumentoParaFilialEmissoraComPreCTe { get; set; }
    public int CodigoChamado { get; set; }
    public bool PermitirSelecionarCteApenasComNfeVinculadaOcorrencia { get; set; }
    public bool RetornarDocumentoOperacaoContainer { get; set; }
    public string NumeroContainer { get; set; }
}
