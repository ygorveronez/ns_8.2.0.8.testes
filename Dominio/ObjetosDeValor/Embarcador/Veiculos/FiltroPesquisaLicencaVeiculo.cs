using System;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public sealed class FiltroPesquisaLicencaVeiculo
    {
        public int CodigoMotorista { get; set; }

        public DateTime? DataEmissaoInicial { get; set; }

        public DateTime? DataEmissaoLimite { get; set; }

        public DateTime? DataVencimentoInicial { get; set; }

        public DateTime? DataVencimentoLimite { get; set; }

        public Entidades.Empresa Empresa { get; set; }

        public string Placa { get; set; }

        public Entidades.Cliente Proprietario { get; set; }

        public Enumeradores.StatusLicenca? StatusLicenca { get; set; }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }

        public int CodigoFilial { get; set; }

        public string NumeroContainer { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca? StatusVigencia { get; set; }
    }
}
