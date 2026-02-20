using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.PreCarga
{
    public sealed class FiltroPesquisaPreCarga
    {
        #region Propriedades

        public int CodigoFilial { get; set; }

        public List<int> CodigosConfiguracaoProgramacaoCarga { get; set; }

        public List<int> CodigosModeloVeicularCarga { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public double CpfCnpjRemetente { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataViagemFinal { get; set; }

        public DateTime? DataViagemInicial { get; set; }

        public string NumeroCarga { get; set; }

        public string NumeroPedido { get; set; }

        public string NumeroPreCarga { get; set; }

        public bool SemCarga { get; set; }

        public bool SemCarregamento { get; set; }

        public List<SituacaoPreCarga> Situacao { get; set; }

        public FiltroPreCarga Status { get; set; }

        public bool SomentePreCargaAtiva { get; set; }

        public bool SomenteProgramacaoCarga { get; set; }

        public List<int> CodigosCidadesDestino { get; set; }

        public List<int> CodigosRotaFrete { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public int CodigoModeloVeicularCarga
        {
            set
            {
                if (value <= 0)
                    return;
                
                CodigosModeloVeicularCarga = new List<int>() { value };
            }
        }

        #endregion Propriedades com Regras
    }
}
