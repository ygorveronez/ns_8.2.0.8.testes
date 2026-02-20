using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class RetornoRegraPlanejamentoFrota
    {
        public int Numero { get; set; }
        public string Descricao { get; set; }

        //validações bool
        public bool ApenasVeiculosComRastreadorAtivo { get; set; }
        public bool ApenasVeiculoQuePossuiTravaQuintaRoda { get; set; }
        public bool ApenasVeiculoQuePossuiImobilizador { get; set; }

        //validações bool com int
        public bool ApenasTracaoComIdadeMaxima { get; set; }
        public int IdadeMaximaTracao { get; set; }
        public bool ApenasReboqueComIdadeMaxima { get; set; }
        public int IdadeMaximaReboque { get; set; }
        public bool LimitarPelaAlturaCarreta { get; set; }
        public decimal MetrosAlturaCarreta { get; set; }
        public bool ApenasComInformacoesDeIscaInformadaNoPedido { get; set; }
        public int QuantidadeIsca { get; set; }
        public bool ApenasComInformacoesDeEscoltaInformadaNoPedido { get; set; }
        public int QuantidadeEscolta { get; set; }
        public bool LimitarPelaAlturaCavalo { get; set; }
        public decimal MetrosAlturaCavalo { get; set; }
        public int QuantidadeCarga { get; set; }
        public int PeriodoQuantidadeCarga { get; set; }
        public Enumeradores.DiaSemanaMesAno TipoPeriodoQuantidadeCarga { get; set; }
        public bool ValidarQuantidadeVeiculoEReboque { get; set; }
        public bool ValidarPorQuantidadeMotorista { get; set; }

        //validações com enum
        public List<Enumeradores.TipoPropriedade> TiposPropriedade { get; set; }
        public List<Enumeradores.TipoCarroceria> TiposCarroceria { get; set; }
        public List<Enumeradores.TipoProprietarioVeiculo> TiposProprietarioVeiculo { get; set; }
        public List<Enumeradores.CategoriaHabilitacao> CategoriasHabilitacao { get; set; }
        public List<Enumeradores.TipoRodado> TiposRodado { get; set; }
        public List<Enumeradores.CondicaoLicenca> CondicaoLicencas { get; set; }
        public List<Enumeradores.CondicaoLiberacaoGR> CondicaoLiberacoesGR { get; set; }
        public List<Enumeradores.CondicaoLiberacaoGR> CondicaoLiberacoesGRVeiculo { get; set; }

        //validações com entidades
        public List<ObjetosDeValor.Embarcador.Carga.ModeloVeicular> ModelosVeicularCargaTracao { get; set; }
        public List<ObjetosDeValor.Embarcador.Carga.ModeloVeicular> ModelosVeicularCargaReboque { get; set; }        
        public List<Dominio.ObjetosDeValor.Embarcador.Frota.TecnologiaRastreador> TecnologiaRastreadores { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.Configuracoes.Licenca> Licencas { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.Configuracoes.LiberacaoGR> LiberacoesGR { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.Configuracoes.LiberacaoGR> LiberacoesGRVeiculo { get; set; }
        
    }
}
