using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Email
{
    public class FiltroPesquisaEmailGlobalizadoFornecedor
    {
        public string Descricao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public SituacaoEnvioEmail? Situacao { get; set; }
    }
}
