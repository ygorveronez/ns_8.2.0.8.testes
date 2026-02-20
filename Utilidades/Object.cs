using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Utilidades
{
    public static class Object
    {
        #region Métodos Privados

        private static bool PropriedadeTipoListaGenerica(PropertyInfo propriedade)
        {
            Type tipoPropriedade = propriedade.PropertyType;

            if (!tipoPropriedade.IsGenericType)
                return false;

            Type tipoPropriedadeGenerica = tipoPropriedade.GetGenericTypeDefinition();

            return (tipoPropriedadeGenerica == typeof(ICollection<>)) || (tipoPropriedadeGenerica == typeof(IList<>));
        }

        #endregion

        #region Métodos Públicos

        public static void CopiarPropriedadesObjeto<T>(T objetoOrigem, T objetoDestino)
        {
            PropertyInfo[] propriedadesDestino = objetoDestino.GetType().GetProperties().Where(o => o.CanRead && o.CanWrite).ToArray();

            foreach (PropertyInfo propriedadeDestino in propriedadesDestino)
            {
                if (propriedadeDestino.Name == "Codigo")
                    continue;

                if (PropriedadeTipoListaGenerica(propriedadeDestino))
                    continue;

                PropertyInfo propriedadeOrigem = objetoOrigem.GetType().GetProperty(propriedadeDestino.Name);

                propriedadeDestino.SetValue(objetoDestino, propriedadeOrigem.GetValue(objetoOrigem, null), null);
            }
        }

        public static dynamic CriarExpandoObject(object objeto)
        {
            ExpandoObject expando = new ExpandoObject();
            IDictionary<string, object> dicionarioExpando = (IDictionary<string, object>)expando;
            PropertyInfo[] propriedades = objeto.GetType().GetProperties();

            foreach (PropertyInfo propriedade in propriedades)
                dicionarioExpando.Add(propriedade.Name, propriedade.GetValue(objeto));

            return expando;
        }

        public static void DefinirListasGenericasComoNulas(object objeto)
        {
            PropertyInfo[] propriedades = objeto.GetType().GetProperties().Where(o => o.CanWrite).ToArray();

            foreach (PropertyInfo propriedade in propriedades)
            {
                if (!PropriedadeTipoListaGenerica(propriedade))
                    continue;

                propriedade.SetValue(objeto, null);
            }
        }

        public static void SetNestedPropertyValue(object target, string propertyName, object value, string mask = "")
        {
            object obj = target;

            string[] properties = propertyName.Split('.');
            List<KeyValuePair<PropertyInfo, object>> valuesOfProperties = new List<KeyValuePair<PropertyInfo, object>>();

            for (int i = 0; i < properties.Length - 1; i++)
            {
                PropertyInfo propertyToGet = target.GetType().GetProperty(properties[i]);

                target = propertyToGet.GetValue(target, null);

                if (target == null)
                    target = Activator.CreateInstance(propertyToGet.PropertyType);

                valuesOfProperties.Add(new KeyValuePair<PropertyInfo, object>(propertyToGet, target));
            }

            PropertyInfo propertyToSet = target.GetType().GetProperty(properties.Last());

            if (propertyToSet.PropertyType.FullName.StartsWith("System"))
            {
                if (propertyToSet.PropertyType.FullName.Contains("DateTime") && !string.IsNullOrWhiteSpace(mask))
                {
                    System.DateTime data = System.DateTime.MinValue;

                    if (System.DateTime.TryParseExact((string)value, mask, null, System.Globalization.DateTimeStyles.None, out data))
                        value = data;
                    else if (propertyToSet.PropertyType.FullName.Contains("Nullable"))
                        value = null;
                    else
                        value = System.DateTime.Now;
                }
                else
                {
                    value = Convert.ChangeType(value, propertyToSet.PropertyType);
                }
            }

            propertyToSet.SetValue(target, value, null);

            valuesOfProperties.Add(new KeyValuePair<PropertyInfo, object>(propertyToSet, value));

            for (var i = valuesOfProperties.Count() - 1; i > 0; i--)
                valuesOfProperties[i].Key.SetValue(valuesOfProperties[i - 1].Value, valuesOfProperties[i].Value);

            valuesOfProperties[0].Key.SetValue(obj, valuesOfProperties[0].Value);
        }
        //alterar o nome do método OrdenarEnumeravelPorCompatibilidade para outro nome mais apropriado após a unificação do projeto: Everest da Unilever.
        /// <summary>
        /// Retorna um enumerável ordenado em forma decrescente pelo objeto mais compatível com o informado por paramêtro (objComparar) a partir do enumerável de objetos do mesmo tipo também informado por parâmetro (listaComparacao).
        /// </summary>
        /// <param name="objComparar">Objeto que servirá de base para a comparação de compatibilidade.</param>
        /// <param name="listaComparacao">Enumerável de objetos do mesmo tipo que o objeto informado por parâmetro (objComparar) que serão comparados com o objComparar</param>
        /// <returns>
        /// Um enumerável ordenado do mais compatível ao menos compatível com o objeto informado por parâmetro (objComparar). Objetos totalmete incompatíveis não são inclusos no enumerável retornado.
        /// </returns>
        public static IEnumerable<T> OrdenarEnumeravelPorCompatibilidade<T>(T objComparar, IEnumerable<T> listaComparacao)
        {
            System.Reflection.PropertyInfo[] propsInfo = objComparar.GetType().GetProperties();
            System.Reflection.PropertyInfo propInfoCodigo = objComparar.GetType().GetProperty("Codigo");

            List<(object codigo, int matchs)> objsAgrupados = new List<(object, int)>();

            foreach (var obj in listaComparacao)
            {
                if ((int)propInfoCodigo.GetValue(obj) == 46)
                    Console.WriteLine("");
                int matchs = 0;
                bool objValido = true;
                foreach (var propInfo in propsInfo)
                {
                    if (propInfoCodigo == propInfo)
                        continue;

                    object valorConfigValidacao = propInfo.GetValue(objComparar);
                    object valorConfig = propInfo.GetValue(obj);

                    if (typeof(IEnumerable<int>).IsAssignableFrom(propInfo.PropertyType))
                    {
                        IEnumerable<int> valoresPropConfig = (IEnumerable<int>)propInfo.GetValue(obj);
                        IEnumerable<int> valoresPropConfigValidacao = (IEnumerable<int>)propInfo.GetValue(objComparar);

                        if (valoresPropConfig != null && valoresPropConfigValidacao != null && valoresPropConfigValidacao.Count() > 0)
                        {
                            if (valoresPropConfig.Any(x => valoresPropConfigValidacao.Contains(x)))
                            {
                                matchs++;
                            }
                        }
                    }
                    else if (typeof(IEnumerable<object>).IsAssignableFrom(propInfo.PropertyType))
                    {
                        IEnumerable<object> valoresPropConfig = (IEnumerable<object>)propInfo.GetValue(obj);
                        IEnumerable<object> valoresPropConfigValidacao = (IEnumerable<object>)propInfo.GetValue(objComparar);

                        if (valoresPropConfig != null && valoresPropConfigValidacao != null && valoresPropConfigValidacao.Count() > 0)
                        {
                            System.Reflection.PropertyInfo propInfoCodigoFilha = valoresPropConfigValidacao.FirstOrDefault().GetType().GetProperty("Codigo");

                            if (propInfoCodigoFilha != null)
                            {
                                List<object> codigosEntidadePropConfig = new List<object>();
                                List<object> codigosEntidadePropConfigValidacao = new List<object>();
                                foreach (object valorPropConfig in valoresPropConfig)
                                {
                                    codigosEntidadePropConfig.Add(propInfoCodigoFilha.GetValue(valorPropConfig));
                                }
                                foreach (object valorPropConfigValidacao in valoresPropConfigValidacao)
                                {
                                    codigosEntidadePropConfigValidacao.Add(propInfoCodigoFilha.GetValue(valorPropConfigValidacao));
                                }

                                if (valoresPropConfig != null && valoresPropConfigValidacao != null && codigosEntidadePropConfigValidacao.Count() > 0)
                                {
                                    if (codigosEntidadePropConfigValidacao.Any(x => codigosEntidadePropConfig.Contains(x)))
                                    {
                                        matchs++;
                                    }
                                }
                            }
                            else
                            {
                                if (valoresPropConfig != null && valoresPropConfigValidacao != null && valoresPropConfigValidacao.Count() > 0)
                                {
                                    if (valoresPropConfigValidacao.Any(x => valoresPropConfig.Contains(x)))
                                    {
                                        matchs++;
                                    }
                                }
                            }
                        }
                    }
                    else if (valorConfigValidacao != null && valorConfig != null)
                    {
                        if (valorConfigValidacao is int || valorConfigValidacao.GetType().IsEnum)
                        {
                            if ((int)valorConfig > 0 && (int)valorConfigValidacao > 0)
                            {
                                if (valorConfig.Equals(valorConfigValidacao))
                                    matchs++;
                                else
                                {
                                    objValido = false;
                                    break;
                                }
                            }
                        }
                        else if (valorConfigValidacao is decimal)
                        {
                            if ((decimal)valorConfig > 0 && (decimal)valorConfigValidacao > 0)
                            {
                                if (valorConfig.Equals(valorConfigValidacao))
                                    matchs++;
                                else
                                {
                                    objValido = false;
                                    break;
                                }
                            }
                        }
                        else if (valorConfigValidacao is double)
                        {
                            if ((double)valorConfig > 0 && (double)valorConfigValidacao > 0)
                            {
                                if (valorConfig.Equals(valorConfigValidacao))
                                    matchs++;
                                else
                                {
                                    objValido = false;
                                    break;
                                }
                            }
                        }
                        else if (valorConfigValidacao is bool)
                        {
                            if ((bool)valorConfigValidacao)
                            {
                                if (valorConfig.Equals(valorConfigValidacao))
                                    matchs++;
                                else
                                {
                                    objValido = false;
                                    break;
                                }
                            }
                        }
                        else if (valorConfigValidacao is string)
                        {
                            if (!string.IsNullOrEmpty((string)valorConfigValidacao))
                            {
                                if (valorConfig.Equals(valorConfigValidacao))
                                    matchs++;
                                else
                                {
                                    objValido = false;
                                    break;
                                }
                            }
                        }
                        else if (propInfo.PropertyType.IsClass)
                        {
                            System.Reflection.PropertyInfo propInfoCodigoFilha = propInfo.PropertyType.GetProperty("Codigo");

                            object codigoEntidadePropConfig = propInfoCodigoFilha.GetValue(valorConfig);
                            object codigoEntidadePropConfigValidacao = propInfoCodigoFilha.GetValue(valorConfigValidacao);

                            if (codigoEntidadePropConfig != null && codigoEntidadePropConfigValidacao != null)
                            {
                                if (valorConfig.Equals(valorConfigValidacao))
                                    matchs++;
                                else
                                {
                                    objValido = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (objValido)
                    objsAgrupados.Add((codigo: propInfoCodigo.GetValue(obj), matchs: matchs));
            }

            objsAgrupados = objsAgrupados.OrderByDescending(x => x.matchs).ToList();

            listaComparacao = listaComparacao.Where(itemComp => objsAgrupados.Exists(objAgr => propInfoCodigo.GetValue(itemComp).Equals(objAgr.codigo)))
                .OrderBy(itemComp => objsAgrupados.IndexOf(objsAgrupados.Where(objAgr => propInfoCodigo.GetValue(itemComp).Equals(objAgr.codigo)).FirstOrDefault()));

            return listaComparacao;
        }

        #endregion
    }
}
