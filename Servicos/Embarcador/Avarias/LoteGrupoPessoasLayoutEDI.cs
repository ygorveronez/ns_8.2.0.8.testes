using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Avarias
{
    public class LoteGrupoPessoasLayoutEDI
    {
        /// <summary>
        /// Retornar uma lista de layouts EDIs dos grupo de pessoas, levando em consideração o grupo pessoa da carga das avarias que compõe o lote
        /// </summary>
        /// <param name="lote">Lote para buscar os Layouts EDIs</param>
        /// <returns></returns>
        public static List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> LayoutEDILote(Dominio.Entidades.Embarcador.Avarias.Lote lote)
        {
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = (from avaria in lote.Avarias where avaria.Carga.GrupoPessoaPrincipal != null select avaria.Carga.GrupoPessoaPrincipal).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layouts = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();
            for (var i = 0; i < grupoPessoas.Count(); i++)
                if (grupoPessoas[i].LayoutsEDI != null)
                    layouts.AddRange(grupoPessoas[i].LayoutsEDI.ToList());

            return layouts;
        }
    }
}
