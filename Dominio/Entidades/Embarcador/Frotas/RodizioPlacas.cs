using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frotas
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RODIZIO_PLACAS", EntityName = "RodizioPlacas", Name = "Dominio.Entidades.Embarcador.Frotas.RodizioPlacas", NameType = typeof(RodizioPlacas))]
    public class RodizioPlacas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RDZ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        public virtual string Descricao { get; set; } = string.Empty;

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinaisPlaca", Column = "RDZ_FINAIS_PLACA", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string FinaisPlaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RDZ_DIA_SEMANA", TypeType = typeof(DiaSemana), NotNull = true)]
        public virtual DiaSemana DiaSemana { get; set; }


        public virtual List<int> ObterFinaisDePlacas()
        {
            var finaisPlacas = FinaisPlaca;
            if (string.IsNullOrWhiteSpace(finaisPlacas))
                return new List<int>();
            
            if (!finaisPlacas.Contains(";"))
                return new List<int>() { finaisPlacas.Trim().ToInt() };

            var lista = finaisPlacas.Split(new char[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(x => x != null && x.Trim().Length > 0).Select(x => x.ToInt()).Distinct().ToList();
            lista.RemoveAll(x => x > 9 || x < 0);
            return lista.OrderBy(x => x).ToList();
        }

        public static List<int> ObterFinaisDePlacas(string placas)
        {
            var finaisPlacas = placas;
            
            if (string.IsNullOrWhiteSpace(finaisPlacas))
                return new List<int>();

            if (!finaisPlacas.Contains(";") && !finaisPlacas.Contains(","))
                return new List<int>() { finaisPlacas.Trim().ToInt() };

            var lista = finaisPlacas.Split(new char[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.Trim() != string.Empty).Select(x => x.Trim().ToInt()).Distinct().ToList();
            lista.RemoveAll(x => x > 9 || x < 0);
            return lista.OrderBy(x => x).ToList();
        }

        public virtual void SetarFinaisDePlacas(List<int> finais)
        {
            if (finais.IsNullOrEmpty())
            {
                FinaisPlaca = string.Empty;
                return;
            }
            finais.RemoveAll(x => x > 9 || x < 0);
            FinaisPlaca = string.Join(";", finais.Distinct().OrderBy(x => x).ToList());
        }

        public virtual string ObterFinaisParaGrid()
        {
            if (string.IsNullOrWhiteSpace(FinaisPlaca))
                return string.Empty;

            if (!FinaisPlaca.Contains(";"))
                return FinaisPlaca.Trim();

            return FinaisPlaca.Replace(";", ", ");
        }

    }
    
}
