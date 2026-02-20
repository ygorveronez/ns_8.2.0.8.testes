using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    public class ItemDocumentoEntrada : RepositorioBase<Dominio.Entidades.ItemDocumentoEntrada>, Dominio.Interfaces.Repositorios.ItemDocumentoEntrada
    {
        public ItemDocumentoEntrada(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ItemDocumentoEntrada BuscarPorCodigo(int codigo, int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemDocumentoEntrada>();

            var result = from obj in query where obj.Codigo == codigo && obj.DocumentoEntrada.Codigo == codigoDocumento select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ItemDocumentoEntrada> BuscarPorDocumentoEntrada(int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemDocumentoEntrada>();

            var result = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumento orderby obj.Sequencial select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ItemDocumentoEntrada> BuscarPorDocumentoEntrada(int[] codigosDocumentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemDocumentoEntrada>();

            var result = from obj in query where codigosDocumentos.Contains(obj.DocumentoEntrada.Codigo) select obj;

            return result.ToList();
        }
    }
}
