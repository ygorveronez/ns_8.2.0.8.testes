using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Guias
{
    public class GuiaRecolhimentoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo>
    {
        public GuiaRecolhimentoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo> Consultar(Dominio.ObjetosDeValor.Embarcador.VincularGuia.FiltroPesquisaVincularGuia filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo>();
         
            query = from obj in query select obj;

            if (filtrosPesquisa.Status != null && filtrosPesquisa.Status.Count > 0)
                query = query.Where(c => filtrosPesquisa.Status.Contains(c.EntidadeAnexo.Situacao));

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(c => c.DataAnexo.Value.Date >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(c => c.DataAnexo.Value.Date <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.CodigoCarga > 0)
                query = query.Where(c => c.EntidadeAnexo.Carga.Codigo == filtrosPesquisa.CodigoCarga);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
                query = query.Where(c => c.EntidadeAnexo.NroGuia.Contains(filtrosPesquisa.NumeroDocumento));

            return query;
        }

        public List<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo> Consultar(Dominio.ObjetosDeValor.Embarcador.VincularGuia.FiltroPesquisaVincularGuia filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }
        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.VincularGuia.FiltroPesquisaVincularGuia filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        public Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo BuscarAnexoPorGuia(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoGuiaRecolhimento tipoAnexo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo>();

            query = from obj in query select obj;

            if (codigo > 0)
                query = query.Where(o => (o.EntidadeAnexo != null ? o.EntidadeAnexo.Codigo : 0) == codigo);

            if (tipoAnexo > 0)
                query = query.Where(o => o.TipoAnexo == tipoAnexo);

            List<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo> guias = query.ToList();

            return query.FirstOrDefault();
        }
    }
}
