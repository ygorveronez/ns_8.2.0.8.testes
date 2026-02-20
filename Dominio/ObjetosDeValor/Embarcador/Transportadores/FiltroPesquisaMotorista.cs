using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public class FiltroPesquisaMotorista
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador SituacaoColaborador { get; set; }
        public Dominio.Entidades.Empresa Empresa { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Tipo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Status { get; set; }
        public string PlacaVeiculo { get; set; }
        public bool SomentePendenteDeVinculo { get; set; }
        public bool PendenteIntegracaoEmbarcador { get; set; }
        public int CodigoCargo { get; set; }
        public double ProprietarioTerceiro { get; set; }
        public string NumeroMatricula { get; set; }
        public double CnpjRemetenteLocalCarregamentoAutorizado { get; set; }
        public bool NaoBloqueado { get; set; }
        public bool? NaoAjudante { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista TipoMotorista { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotoristaAjudante TipoMotoristaAjudante { get; set; }
        public List<int> CodigosEmpresa { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public string NumeroFrota { get; set; }
    }
}
