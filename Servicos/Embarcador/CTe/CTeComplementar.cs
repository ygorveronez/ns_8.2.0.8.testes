namespace Servicos.Embarcador.CTe
{
    public class CTeComplementar
    {
        public Dominio.ObjetosDeValor.Embarcador.CTe.CTeComplementar ConverterDynamicCTeComplementar(dynamic dynCTeComplementar)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.CTeComplementar cteComplementar = new Dominio.ObjetosDeValor.Embarcador.CTe.CTeComplementar();

            cteComplementar.ChaveCTeComplementado = Utilidades.String.OnlyNumbers((string)dynCTeComplementar.ChaveCTeComplementado);

            return cteComplementar;
        }

        public void SalvarCTeComplementar(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.CTeComplementar cteComplementar)
        {
            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                cte.ChaveCTESubComp = cteComplementar.ChaveCTeComplementado;
            else if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                cte.ChaveCTESubComp = null;
        }
    }
}
