using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public sealed class FiltroPesquisaVeiculo
    {
        #region Atributos

        private bool _codigoEmpresaInformado;

        #endregion Atributos

        #region Propriedades

        public bool PendenteIntegracaoEmbarcador { get; set; }

        public bool ApenasTracao { get; set; }

        public int CodigoAcertoViagem { get; set; }

        public int CodigoCentroCarregamento { get; set; }

        public int CodigoMarcaVeiculo { get; set; }

        public int CodigoMotorista { get; set; }

        public int CodigoReboque { get; set; }

        public List<int> CodigosEmpresa { get; set; }

        public double LocalAtualFisicoDoVeiculo { get; set; }

        /// <value>O filtro de modelo veicular de carga filtra mais que somente o modelo. Para a consulta do CRUD de veículo deve filtrar apenas por modelo.</value>
        public bool ForcarFiltroModeloVeicularCarga { get; set; }

        public bool FiltrarCadastrosAprovados { get; set; }

        public Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        public int CodigoModeloVeiculo { get; set; }

        public int CodigoTecnologiaRastreador { get; set; }

        public string NumeroFrota { get; set; }

        public string Placa { get; set; }

        public List<Entidades.Embarcador.Cargas.ModeloVeicularCarga> PossiveisModelos { get; set; }

        public Entidades.Cliente Proprietario { get; set; }

        public string Renavam { get; set; }

        public SituacaoAtivoPesquisa SituacaoAtivo { get; set; }

        public bool SomenteEmpresasAtivas { get; set; }

        public string TipoPropriedade { get; set; }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }

        public string TipoVeiculo { get; set; }

        public List<int> CodigosSegmento { get; set; }

        public double CodigoProprietario { get; set; }

        public StatusPosicao StatusPosicao { get; set; }

        public StatusViagem StatusViagem { get; set; }

        public bool SomenteRastreados { get; set; }

        public string Chassi { get; set; }

        public SituacaoVeiculo? SituacaoVeiculo { get; set; }

        public List<int> CodigosEmpresas { get; set; }

        public string Terminal { get; set; }

        public DateTime? DataPosicao { get; set; }

        public bool RastreadorPosicionado { get; set; }

        public List<int> CodigosTecnologiaRastreador { get; set; }

        public bool? VeiculoOnlineOffline { get; set; }

        public int TempoSemPosicaoParaVeiculoPerderSinal { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public int CodigoEmpresa
        {
            get
            {
                if (_codigoEmpresaInformado)
                    return CodigosEmpresa.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoEmpresaInformado = true;
                    CodigosEmpresa = new List<int>() { value };
                }
            }
        }

        #endregion Propriedades com Regras
    }
}
