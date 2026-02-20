using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class MDFeCIOT : RepositorioBase<Dominio.Entidades.MDFeCIOT>, Dominio.Interfaces.Repositorios.MDFeCIOT
    {
        public MDFeCIOT(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MDFeCIOT(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.MDFeCIOT BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeCIOT>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.MDFeCIOT> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeCIOT>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public bool ExistePorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeCIOT>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.Any();
        }

        public Task<List<Dominio.Entidades.MDFeCIOT>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeCIOT>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.MDFeCIOT> BuscarPorMDFes(int[] codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeCIOT>();
            var result = from obj in query where codigoMDFe.Contains(obj.MDFe.Codigo) select obj;
            return result.ToList();
        }

        public bool ExisteEncerradoPorVeiculoENumeroCiot(string placaVeiculo, string numeroCiot)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeCIOT>();

            var result = from obj in query
                         where
                            obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado &&
                            obj.NumeroCIOT == numeroCiot &&
                            obj.MDFe.Veiculos.Any(o => o.Placa == placaVeiculo)
                         select obj;

            return result.Any();
        }

        public Task<List<Dominio.Entidades.MDFeCIOT>> BuscarPorCargaCIOTAsync(int codigoCarga, string CIOT)
        {
            var codigosMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(obj => obj.Carga.Codigo == codigoCarga && obj.MDFe != null &&
                                !new[]
                                {
                                    Dominio.Enumeradores.StatusMDFe.Encerrado,
                                    Dominio.Enumeradores.StatusMDFe.Autorizado,
                                    Dominio.Enumeradores.StatusMDFe.Enviado,
                                    Dominio.Enumeradores.StatusMDFe.Cancelado
                                }.Contains(obj.MDFe.Status))
                .Select(obj => obj.MDFe.Codigo);

            var listaMDFeCIOT = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeCIOT>()
                .Where(x => codigosMDFe.Contains(x.MDFe.Codigo) && x.NumeroCIOT == CIOT)
                .ToListAsync();

            return listaMDFeCIOT;
        }

    }
}
