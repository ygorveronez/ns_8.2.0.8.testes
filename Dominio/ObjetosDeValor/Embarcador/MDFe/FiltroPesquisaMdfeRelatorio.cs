using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.MDFe
{
    public sealed class FiltroPesquisaMdfeRelatorio
    {
        public int CodigoEmpresa { get; set; }

        public int CodigoSerie { get; set; }

        public string CpfMotorista { get; set; }

        public DateTime? DataAutorizacaoInicial { get; set; }

        public DateTime? DataAutorizacaoLimite { get; set; }

        public DateTime? DataCancelamentoInicial { get; set; }

        public DateTime? DataCancelamentoLimite { get; set; }

        public DateTime? DataEmissaoInicial { get; set; }

        public DateTime? DataEmissaoLimite { get; set; }

        public DateTime? DataEncerramentoInicial { get; set; }

        public DateTime? DataEncerramentoLimite { get; set; }

        private string _estadoCarregamento;
        public string EstadoCarregamento
        {
            get { return _estadoCarregamento == "0" ? string.Empty : _estadoCarregamento; }
            set { _estadoCarregamento = value; }
        }

        private string _estadoDescarregamento;
        public string EstadoDescarregamento
        {
            get { return _estadoDescarregamento == "0" ? string.Empty : _estadoDescarregamento; }
            set { _estadoDescarregamento = value; }
        }

        public List<Dominio.Enumeradores.StatusMDFe> ListaStatusMdfe { get; set; }

        public int NumeroInicial { get; set; }

        public int NumeroLimite { get; set; }

        public string PlacaVeiculo { get; set; }

        public int NumeroCTe { get; set; }

        public string NumeroCarga { get; set; }

        public int TipoOperacao { get; set; }

        public bool ExibirCTes { get; set; }

        public bool? MDFeVinculadoACarga { get; set; }
        public int MunicipioDescarregamento { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<double> CodigosRecebedores { get; set; }
    }
}
