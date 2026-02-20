using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao;

public sealed class AdicionarIntegracaoEnvioProgramado
{
    public AdicionarIntegracaoEnvioProgramado(TipoEntidadeIntegracao tipoEntidadeIntegracao)
    {
        TipoEntidadeIntegracao = tipoEntidadeIntegracao;
    }

    public Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }
    public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }
    public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }
    public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeIntegracao TipoEntidadeIntegracao { get; private set; }
}
