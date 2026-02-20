using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAcaoIntegracao
    {
        Autorizacao = 0,
        Cancelamento = 1,
        Criacao = 2,
        Alteracao = 3,
    }

    public static class TipoAcaoIntegracaoHelper
    {

        public static string ObterDescricao(this TipoAcaoIntegracao situacao)
        {
            switch (situacao)
            {
                case TipoAcaoIntegracao.Autorizacao: return "Autorização";
                case TipoAcaoIntegracao.Cancelamento: return "Cancelamento";
                case TipoAcaoIntegracao.Criacao: return "Criação";
                case TipoAcaoIntegracao.Alteracao: return "Alteração";
                default: return string.Empty;
            }
        }
    }
}
