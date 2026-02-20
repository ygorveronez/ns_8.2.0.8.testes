using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal>
    {
        public CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal> BuscarPorCargaOcorrencia(int codigoCargaOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal>();
            query = query.Where(obj => obj.CargaOcorrencia.Codigo == codigoCargaOcorrencia);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal> BuscarPorCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal>();
            query = query.Where(obj => obj.CargaCTe.Codigo == codigoCargaCTe);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal> BuscarPorCargaOcorrenciaECargaCTe(int codigoCargaOcorrencia, int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal>();

            query = query.Where(obj => obj.CargaOcorrencia.Codigo == codigoCargaOcorrencia);
            query = query.Where(obj => obj.CargaCTe.Codigo == codigoCargaCTe);

            return query.ToList();
        }

        public List<int> BuscarXMLNotaFiscalPorCargaOcorrenciaECargaCTe(int codigoCargaOcorrencia, int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal>();

            query = query.Where(obj => obj.CargaOcorrencia.Codigo == codigoCargaOcorrencia);
            query = query.Where(obj => obj.CargaCTe.Codigo == codigoCargaCTe);

            return query.Select(obj => obj.XMLNotaFiscal.Codigo).ToList();
        }

        public bool ExistePorCargaOcorrenciaCargaCTeXMLNotaFiscal(int codigoCargaOcorrencia, int codigoCargaCTe, int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal>();

            query = query.Where(obj => obj.CargaOcorrencia.Codigo == codigoCargaOcorrencia 
                                    && obj.CargaCTe.Codigo == codigoCargaCTe
                                    && obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal);

            return query.Any();
        }

    }
}
