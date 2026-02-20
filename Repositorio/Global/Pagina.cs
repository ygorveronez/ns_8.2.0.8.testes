using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class Pagina : RepositorioBase<Dominio.Entidades.Pagina>, Dominio.Interfaces.Repositorios.Pagina
    {
        public Pagina(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Pagina> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pagina>();
            var result = from obj in query where obj.Status == "A" orderby obj.Menu ascending, obj.Descricao ascending select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Pagina BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pagina>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Pagina BuscarPorFormulario(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pagina>();
            var result = from obj in query where obj.Formulario.ToLower().Equals(descricao) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Pagina> BuscarPorTipoAcesso(Dominio.Enumeradores.TipoAcesso tipoAcesso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pagina>();
            var result = from obj in query where obj.TipoAcesso == tipoAcesso && obj.Status == "A" select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Pagina> BuscarPorTipoAcessoETipoSistema(Dominio.Enumeradores.TipoAcesso tipoAcesso, Dominio.Enumeradores.TipoSistema tipoSistema)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pagina>();
            var result = from obj in query where obj.TipoAcesso == tipoAcesso && obj.Status == "A" select obj;

            if (tipoSistema != Dominio.Enumeradores.TipoSistema.MultiEmbarcador)
            {                
                result = result.Where(o => o.TipoSistema == tipoSistema);
            }

            return result.ToList();
        }

        public List<Dominio.Entidades.Pagina> Consultar(Dominio.Enumeradores.TipoSistema tipoSistema, bool ambienteAdmin, bool ambienteEmissao, string descricao, string menu, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pagina>();

            var result = from obj in query where obj.TipoSistema == tipoSistema select obj;

            if (ambienteAdmin && ambienteEmissao)
                result = result.Where(o => (o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin || o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao));
            else if (ambienteAdmin)
                    result = result.Where(o => o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin);
            else if (ambienteEmissao)
                result = result.Where(o => o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao);

            if(!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(menu))
                result = result.Where(o => o.Menu.Contains(menu));

            return result
                        .Skip(inicioRegistros)
                        .Take(maximoRegistros)
                        .ToList();
        }

        public int ContarConsulta(Dominio.Enumeradores.TipoSistema tipoSistema, bool ambienteAdmin, bool ambienteEmissao, string descricao, string menu)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pagina>();

            var result = from obj in query where obj.TipoSistema == tipoSistema select obj;

            if (ambienteAdmin && ambienteEmissao)
                result = result.Where(o => (o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin || o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao));
            else if (ambienteAdmin)
                result = result.Where(o => o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin);
            else if (ambienteEmissao)
                result = result.Where(o => o.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(menu))
                result = result.Where(o => o.Menu.Contains(menu));

            return result.Count();
        }
    }
}
