using System;

namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public sealed class FiltroPesquisaMotoristaMobile
    {
        public int CodigoCentroCarregamento { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        private string _cpf;
        public string Cpf
        {
            get { return _cpf; }
            set { _cpf = value.ObterSomenteNumeros(); }
        }

        public string Nome { get; set; }

        public string PlacaVeiculo { get; set; }

        public Entidades.Empresa Transportador { get; set; }

        public bool UtilizarProgramacaoCarga { get; set; }
    }
}
