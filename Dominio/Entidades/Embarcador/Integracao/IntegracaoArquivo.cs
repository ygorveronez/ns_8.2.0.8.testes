using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    public abstract class IntegracaoArquivo : EntidadeBase
    {
        public abstract int Codigo { get; set; }

        public abstract DateTime Data { get; set; }

        public abstract string Mensagem { get; set; }

        public abstract TipoArquivoIntegracaoCTeCarga Tipo { get; set; }

        public abstract ArquivoIntegracao ArquivoRequisicao { get; set; }

        public abstract ArquivoIntegracao ArquivoResposta { get; set; }

        public virtual string Descricao
        {
            get { return $"Arquivo integração {Codigo} {Mensagem}"; }
        }

        public virtual string DescricaoTipo
        {
            get { return Tipo.ObterDescricao(); }
        }
    }
}
