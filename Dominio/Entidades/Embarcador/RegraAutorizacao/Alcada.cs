using System;
using System.Collections;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.RegraAutorizacao
{
    public abstract class Alcada<TRegra, TPropriedade> : EntidadeBase where TRegra : RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ALC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALC_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALC_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALC_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao Juncao { get; set; }

        #endregion

        #region Propriedades Abstratas

        public abstract string Descricao { get; }

        public abstract TPropriedade PropriedadeAlcada { get; set; }

        public abstract TRegra RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Privados

        private bool IsCondicaoVerdadeiraEntidade(object valor)
        {
            if (valor == null)
                return false;

            if (valor.GetType().IsGenericType && valor is IEnumerable)
                return IsCondicaoVerdadeiraEntidadeListaCodigos(valor);

            return IsCondicaoVerdadeiraEntidadeCodigo(valor);
        }

        private bool IsCondicaoVerdadeiraEntidadeCodigo(object valor)
        {
            long codigoComparar = Convert.ToInt64(valor);
            long codigoEntidade = Convert.ToInt64(ObterValorPropriedadeAlcada());

            switch (Condicao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe:
                    return codigoComparar != codigoEntidade;
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA:
                    return codigoComparar == codigoEntidade;
            }

            return false;
        }

        private bool IsCondicaoVerdadeiraEntidadeListaCodigos(object valor)
        {
            long codigoEntidade = Convert.ToInt64(ObterValorPropriedadeAlcada());
            List<long> codigosComparar = new List<long>();
            IEnumerator enumerador = ((IEnumerable)valor).GetEnumerator();

            while (enumerador.MoveNext())
                codigosComparar.Add(Convert.ToInt64(enumerador.Current));

            if (codigosComparar.Count == 0)
                return false;

            switch (Condicao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe:
                    return !codigosComparar.Contains(codigoEntidade);
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA:
                    return codigosComparar.Contains(codigoEntidade);
            }

            return false;
        }

        private bool IsCondicaoVerdadeiraValor(object valor)
        {
            var valorComparar = Convert.ToDecimal(valor);
            var valorProriedade = Convert.ToDecimal(ObterValorPropriedadeAlcada());

            switch (Condicao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe:
                    return valorComparar != valorProriedade;

                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA:
                    return valorComparar == valorProriedade;

                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue:
                    return valorComparar >= valorProriedade;

                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue:
                    return valorComparar > valorProriedade;

                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue:
                    return valorComparar <= valorProriedade;

                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue:
                    return valorComparar < valorProriedade;
            }

            return false;
        }

        #endregion

        #region Métodos Públicos

        public virtual bool Equals(Alcada<TRegra, TPropriedade> other)
        {
            return other.Codigo == this.Codigo;
        }

        public virtual bool IsCondicaoVerdadeira(object valor)
        {
            if (IsEntidadePropriedadeAlcada())
                return IsCondicaoVerdadeiraEntidade(valor);

            return IsCondicaoVerdadeiraValor(valor);
        }

        public virtual bool IsEntidadePropriedadeAlcada()
        {
            return typeof(TPropriedade).IsSubclassOf(typeof(EntidadeBase));
        }

        public virtual bool IsJuncaoTodasVerdadeiras()
        {
            return Juncao == ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E;
        }

        #endregion

        #region Métodos Públicos Abstratos

        public abstract object ObterValorPropriedadeAlcada();

        #endregion
    }
}
