using System;

namespace Servicos.Embarcador.CTe
{
    public class CTeAnulacao : ServicoBase
    {
        public CTeAnulacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.CTe.CTeAnulacao ConverterDynamicCTeAnulacao(dynamic dynCTeAnulacao)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.CTeAnulacao cteAnulacao = new Dominio.ObjetosDeValor.Embarcador.CTe.CTeAnulacao();

            cteAnulacao.ChaveCTeAnulado = Utilidades.String.OnlyNumbers((string)dynCTeAnulacao.ChaveCTeAnulado);
            cteAnulacao.DataAnulacao = ((string)dynCTeAnulacao.DataAnulacao).ToDateTime();

            return cteAnulacao;
        }

        public void SalvarCTeAnulacao(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.CTeAnulacao cteAnulacao)
        {
            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao)
            {
                cte.ChaveCTESubComp = cteAnulacao.ChaveCTeAnulado;
                cte.DataAnulacao = cteAnulacao.DataAnulacao;
            }
            else
                cte.DataAnulacao = null;

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                cte.ChaveCTESubComp = null;
        }
    }
}
