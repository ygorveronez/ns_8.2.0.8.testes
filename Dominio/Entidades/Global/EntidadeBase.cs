using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dominio.Entidades
{
    [Serializable]
    public class EntidadeBase
    {
        #region Variáveis Privadas

        private List<Auditoria.HistoricoPropriedade> _valoresAlterados = null;
        private List<ObjetosDeValor.Auditoria.HistoricoPropriedade> _valoresExternosAlterados = null;
        private Dictionary<string, object> _valoresOriginais = null;
        private bool _initialized = false;

        #endregion

        #region Métodos Globais

        public virtual T Clonar<T>()
        {
            return (T)this.MemberwiseClone();
        }

        public virtual ObjetosDeValor.Auditoria.HistoricoPropriedade GetChangeByPropertyName(string nomePropriedade)
        {
            return GetChangesByPropertyName(nomePropriedade).FirstOrDefault();
        }

        public virtual List<ObjetosDeValor.Auditoria.HistoricoPropriedade> GetChangesByPropertyName(string nomePropriedade)
        {
            List<ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = ObterValoresAlterados();

            return valoresAlterados.Where(o => o.Propriedade == nomePropriedade).ToList();
        }

        /// <summary>
        /// Obtém as informações de alterações realizadas nas propriedades do objeto
        /// </summary>
        public virtual List<Auditoria.HistoricoPropriedade> GetChanges()
        {
            if (_valoresAlterados == null)
                CarregarValoresAlterados();

            return _valoresAlterados;
        }

        public virtual List<ObjetosDeValor.Auditoria.HistoricoPropriedade> GetCurrentChanges()
        {
            return ObterValoresAlterados();
        }

        /// <summary>
        /// Salva as informações do objeto no estado atual para auditar as alterações posteriormente pelo método GetChanges
        /// </summary>
        public virtual void Initialize()
        {
            if (_initialized)
                return;

            CarregarValoresOriginais();

            _valoresExternosAlterados = new List<ObjetosDeValor.Auditoria.HistoricoPropriedade>();
            _initialized = true;
        }

        public virtual bool IsChanged()
        {
            List<ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = ObterValoresAlterados();

            return valoresAlterados.Count > 0;
        }

        public virtual bool IsChangedByPropertyName(string nomePropriedade)
        {
            return GetChangesByPropertyName(nomePropriedade).Count > 0;
        }

        public virtual bool IsInitialized()
        {
            return _initialized;
        }

        public virtual void SetChanges()
        {
            CarregarValoresAlterados();
        }

        public virtual void SetExternalChange(ObjetosDeValor.Auditoria.HistoricoPropriedade valorAlterado)
        {
            if (valorAlterado == null)
                return;

            SetExternalChanges(new List<ObjetosDeValor.Auditoria.HistoricoPropriedade>() { valorAlterado });
        }

        public virtual void SetExternalChanges(List<ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados)
        {
            if (!_initialized)
                return;

            if ((valoresAlterados == null) || (valoresAlterados.Count == 0))
                return;

            _valoresExternosAlterados.AddRange(valoresAlterados);
        }

        public virtual List<ObjetosDeValor.Auditoria.HistoricoPropriedade> GetExternalChanges()
        {
            if (_valoresExternosAlterados == null)
                return new List<ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            return _valoresExternosAlterados;
        }

        public virtual int GetPropertyLength(string nomePropriedade)
        {
            PropertyInfo propriedade = this.GetType().GetProperty(nomePropriedade);
            if (propriedade == null)
                return 0;

            foreach (var customAttribute in propriedade.CustomAttributes)
            {
                foreach (var namedArgument in customAttribute.NamedArguments)
                {
                    if (namedArgument.MemberName == "Length")
                        return (int)namedArgument.TypedValue.Value;
                }
            }
            return 0;
        }
        #endregion

        #region Métodos Privados

        private void CarregarValoresAlterados()
        {
            if (!_initialized)
                return;

            List<ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = ObterValoresAlterados();
            _valoresAlterados = new List<Auditoria.HistoricoPropriedade>();

            foreach (ObjetosDeValor.Auditoria.HistoricoPropriedade valorAlterado in valoresAlterados)
                _valoresAlterados.Add(new Auditoria.HistoricoPropriedade(valorAlterado.Propriedade, valorAlterado.De, valorAlterado.Para));
        }

        private void CarregarValoresOriginais()
        {
            _valoresOriginais = new Dictionary<string, object>();
            PropertyInfo[] propriedades = this.GetType().GetProperties();

            foreach (PropertyInfo property in propriedades)
            {
                if (!property.CanWrite)
                    continue;

                /* bool continuar = true;
                 try
                 {
                     foreach (var customAttribute in property.CustomAttributes)
                     {
                         foreach (var namedArgument in customAttribute.NamedArguments)
                         {
                             if (namedArgument.MemberName == "Formula")
                             {
                                 continuar = false;
                                 break;
                             }
                         }
                         if (!continuar)
                             break;
                     }
                 }
                 catch (Exception)
                 {

                 }
                 if (!continuar)
                     continue;
                */
                dynamic valor = property.GetValue(this, null);

                if (valor != null && IsNHibernateSet(valor, valor.GetType()))
                {
                    List<dynamic> valoresOriginais = new List<dynamic>();

                    foreach (var item in valor)
                        valoresOriginais.Add(item);

                    _valoresOriginais.Add(property.Name, valoresOriginais);

                    continue;
                }

                this._valoresOriginais.Add(property.Name, valor);
            }
        }

        private bool IsEntidade(Type tipoPropriedade)
        {
            return tipoPropriedade.IsClass && (tipoPropriedade.Name != "String") && !tipoPropriedade.IsEnum;
        }

        private bool IsList(object valor, Type tipoPropriedade)
        {
            if (valor == null)
                return false;

            return valor is IList && tipoPropriedade.IsGenericType && (tipoPropriedade.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || tipoPropriedade.GetGenericTypeDefinition().IsAssignableFrom(typeof(NHibernate.Collection.Generic.PersistentGenericBag<>)));
        }

        private bool IsNHibernateSet(object valor, Type tipoPropriedade)
        {
            if (valor == null)
                return false;

            return tipoPropriedade.AssemblyQualifiedName.Contains("NHibernate.Collection.Generic.PersistentGenericSet");
        }

        private List<ObjetosDeValor.Auditoria.HistoricoPropriedade> ObterItensAlterados(PropertyInfo propriedade, dynamic valorOriginal, dynamic valorAtual)
        {
            List<ObjetosDeValor.Auditoria.HistoricoPropriedade> itensAlterados = new List<ObjetosDeValor.Auditoria.HistoricoPropriedade>();
            Type tipo = null;
            bool tipoEntidade = false;

            if (valorOriginal != null)
            {
                foreach (dynamic itemValorOriginal in valorOriginal)
                {
                    bool existeItem = false;

                    if (tipo == null)
                    {
                        tipo = itemValorOriginal.GetType();
                        tipoEntidade = IsEntidade(tipo);
                    }

                    if (valorAtual != null)
                    {
                        foreach (dynamic itemValorAtual in valorAtual)
                        {
                            if ((tipoEntidade && itemValorOriginal.Codigo == itemValorAtual.Codigo) || (!tipoEntidade && itemValorOriginal == itemValorAtual))
                            {
                                existeItem = true;
                                break;
                            }
                        }
                    }

                    if (!existeItem)
                        itensAlterados.Add(new ObjetosDeValor.Auditoria.HistoricoPropriedade()
                        {
                            Propriedade = propriedade.Name,
                            De = ObterValorPropriedadeFormatado(itemValorOriginal, tipo),
                            Para = ""
                        });
                }
            }

            if (valorAtual != null)
            {
                foreach (dynamic itemValorAtual in valorAtual)
                {
                    bool existeItem = false;

                    if (tipo == null)
                    {
                        tipo = itemValorAtual.GetType();
                        tipoEntidade = IsEntidade(tipo);
                    }

                    if (valorOriginal != null)
                    {
                        foreach (dynamic itemFrom in valorOriginal)
                        {
                            if ((tipoEntidade && itemFrom.Codigo == itemValorAtual.Codigo) || (!tipoEntidade && itemFrom == itemValorAtual))
                            {
                                existeItem = true;
                                break;
                            }
                        }
                    }

                    if (!existeItem)
                        itensAlterados.Add(new ObjetosDeValor.Auditoria.HistoricoPropriedade()
                        {
                            Propriedade = propriedade.Name,
                            De = "",
                            Para = ObterValorPropriedadeFormatado(itemValorAtual, tipo)
                        });
                }
            }

            return itensAlterados;
        }

        private string ObterValorPropriedadeFormatado(dynamic valor, Type tipoPropriedade)
        {
            if (valor == null)
                return string.Empty;

            if (tipoPropriedade.IsClass && (tipoPropriedade.Name != "String") && !tipoPropriedade.IsEnum)
                return $"{valor.Codigo.ToString()} - {(valor.Descricao ?? string.Empty)}";

            if (tipoPropriedade.IsEnum)
            {
                var enumerador = ((Enum)valor);
                return $"{enumerador.ToString("D")} - {enumerador.ToString("G")}";
            }

            if (tipoPropriedade == typeof(string))
                return (string)valor;

            if (tipoPropriedade == typeof(int) || tipoPropriedade == typeof(long))
                return ((long)valor).ToString("n0");

            if (tipoPropriedade == typeof(decimal) || tipoPropriedade == typeof(double) || tipoPropriedade == typeof(float))
                return ((decimal)valor).ToString("n6");

            if (tipoPropriedade == typeof(bool))
                return ((bool)valor) ? "Sim" : "Não";

            if (tipoPropriedade == typeof(DateTime))
                return ((DateTime)valor).ToString("dd/MM/yyyy HH:mm:ss");

            if (tipoPropriedade == typeof(TimeSpan))
                return ((TimeSpan)valor).ToString(@"hh\:mm");

            return string.Empty;
        }

        private List<ObjetosDeValor.Auditoria.HistoricoPropriedade> ObterValoresAlterados()
        {
            List<ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = new List<ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (!_initialized)
                return valoresAlterados;

            PropertyInfo[] propriedades = this.GetType().GetProperties().ToArray();

            for (int i = 0; i < propriedades.Length; i++)
            {
                PropertyInfo propriedade = propriedades[i];

                if (!propriedade.CanWrite)
                    continue;


                /*bool continuar = true;
                try
                {
                    foreach (var customAttribute in propriedade.CustomAttributes)
                    {
                        foreach (var namedArgument in customAttribute.NamedArguments)
                        {
                            if (namedArgument.MemberName == "Formula")
                            {
                                continuar = false;
                                break;
                            }
                        }
                        if (!continuar)
                            break;
                    }
                }
                catch (Exception)
                {

                }
                if (!continuar)
                    continue;
                */
                dynamic valorOriginal = _valoresOriginais[propriedade.Name];
                dynamic valorAtual = propriedade.GetValue(this, null);
                bool valorAlterado;

                try
                {
                    valorAlterado = !Equals(valorAtual, valorOriginal);
                }
                catch
                {
                    valorAlterado = true;
                }

                if (!valorAlterado)
                    continue;

                Type tipo = valorAtual?.GetType() ?? valorOriginal.GetType();

                //Quando é lista, utiliza só para obter os dados, então não acessa a propriedade
                if (IsList(valorAtual, tipo))
                    continue;

                if (IsNHibernateSet(valorAtual, tipo))
                {
                    valoresAlterados.AddRange(ObterItensAlterados(propriedade, valorOriginal, valorAtual));
                    continue;
                }

                if (IsEntidade(tipo) && ((valorOriginal == null && valorAtual == null) || (valorOriginal != null && valorAtual != null && valorOriginal.Codigo == valorAtual.Codigo)))
                    continue;

                valoresAlterados.Add(new ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = propriedade.Name,
                    De = ObterValorPropriedadeFormatado(valorOriginal, valorOriginal?.GetType() ?? tipo),
                    Para = ObterValorPropriedadeFormatado(valorAtual, tipo)
                });
            }

            valoresAlterados.AddRange(_valoresExternosAlterados);

            return valoresAlterados;
        }

        #endregion
    }
}
