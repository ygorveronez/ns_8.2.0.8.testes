using System;
using System.Collections;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.RegraAutorizacao
{
    public abstract class AlcadaSemPadronizacao : EntidadeBase
    {
        #region Métodos Privados

        private bool IsCondicaoVerdadeiraEntidadeCodigo(double codigoEntidade, object valorComparar, ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao)
        {
            double codigoComparar = Convert.ToDouble(valorComparar);

            switch (condicao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe:
                    return codigoComparar != codigoEntidade;
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA:
                    return codigoComparar == codigoEntidade;
            }

            return false;
        }

        private bool IsCondicaoVerdadeiraEntidadeCodigo(long codigoEntidade, object valorComparar, ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao)
        {
            long codigoComparar = Convert.ToInt64(valorComparar);

            switch (condicao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe:
                    return codigoComparar != codigoEntidade;
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA:
                    return codigoComparar == codigoEntidade;
            }

            return false;
        }

        private bool IsCondicaoVerdadeiraEntidadeCodigo(string codigoEntidade, object valorComparar, ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao)
        {
            string codigoComparar = Convert.ToString(valorComparar);

            switch (condicao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe:
                    return codigoComparar != codigoEntidade;
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA:
                    return codigoComparar == codigoEntidade;
            }

            return false;
        }

        private bool IsCondicaoVerdadeiraEntidadeListaCodigos(long codigoEntidade, object valorComparar, ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao)
        {
            List<long> codigosComparar = new List<long>();
            IEnumerator enumerador = ((IEnumerable)valorComparar).GetEnumerator();

            while (enumerador.MoveNext())
                codigosComparar.Add(Convert.ToInt64(enumerador.Current));

            if (codigosComparar.Count == 0)
                return false;

            switch (condicao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe:
                    return !codigosComparar.Contains(codigoEntidade);
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA:
                    return codigosComparar.Contains(codigoEntidade);
            }

            return false;
        }

        private bool IsCondicaoVerdadeiraEntidadeListaCodigos(double codigoEntidade, object valorComparar, ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao)
        {
            List<double> codigosComparar = new List<double>();
            IEnumerator enumerador = ((IEnumerable)valorComparar).GetEnumerator();

            while (enumerador.MoveNext())
                codigosComparar.Add(Convert.ToInt64(enumerador.Current));

            if (codigosComparar.Count == 0)
                return false;

            switch (condicao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe:
                    return !codigosComparar.Contains(codigoEntidade);
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA:
                    return codigosComparar.Contains(codigoEntidade);
            }

            return false;
        }

        private bool IsCondicaoVerdadeiraEntidadeListaCodigos(string codigoEntidade, object valorComparar, ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao)
        {
            List<string> codigosComparar = new List<string>();
            IEnumerator enumerador = ((IEnumerable)valorComparar).GetEnumerator();

            while (enumerador.MoveNext())
                codigosComparar.Add(Convert.ToString(enumerador.Current));

            if (codigosComparar.Count == 0)
                return false;

            switch (condicao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe:
                    return !codigosComparar.Contains(codigoEntidade);
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA:
                    return codigosComparar.Contains(codigoEntidade);
            }

            return false;
        }

        #endregion

        #region Métodos Protegidos

        protected bool IsCondicaoVerdadeiraEntidade(double codigoEntidade, object valorComparar, ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao)
        {
            if (valorComparar == null)
                return false;

            if (valorComparar.GetType().IsGenericType && valorComparar is IEnumerable)
                return IsCondicaoVerdadeiraEntidadeListaCodigos(codigoEntidade, valorComparar, condicao);

            return IsCondicaoVerdadeiraEntidadeCodigo(codigoEntidade, valorComparar, condicao);
        }

        protected bool IsCondicaoVerdadeiraEntidade(long codigoEntidade, object valorComparar, ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao)
        {
            if (valorComparar == null)
                return false;

            if (valorComparar.GetType().IsGenericType && valorComparar is IEnumerable)
                return IsCondicaoVerdadeiraEntidadeListaCodigos(codigoEntidade, valorComparar, condicao);

            return IsCondicaoVerdadeiraEntidadeCodigo(codigoEntidade, valorComparar, condicao);
        }

        protected bool IsCondicaoVerdadeiraEntidade(string codigoEntidade, object valorComparar, ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao)
        {
            if (valorComparar == null)
                return false;

            if (valorComparar.GetType().IsGenericType && valorComparar is IEnumerable)
                return IsCondicaoVerdadeiraEntidadeListaCodigos(codigoEntidade, valorComparar, condicao);

            return IsCondicaoVerdadeiraEntidadeCodigo(codigoEntidade, valorComparar, condicao);
        }

        protected bool IsCondicaoVerdadeiraValor(decimal valorProriedade, object valorComparar, ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao)
        {
            decimal valor = Convert.ToDecimal(valorComparar);

            switch (condicao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe:
                    return valor != valorProriedade;

                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA:
                    return valor == valorProriedade;

                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue:
                    return valor >= valorProriedade;

                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue:
                    return valor > valorProriedade;

                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue:
                    return valor <= valorProriedade;

                case ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue:
                    return valor < valorProriedade;
            }

            return false;
        }
        
        #endregion

        #region Métodos Públicos Abstratos

        public abstract bool IsCondicaoVerdadeira(object valor);

        public abstract bool IsJuncaoTodasVerdadeiras();

        #endregion
    }
}
