using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class INSSImpostoContratoFrete : RepositorioBase<Dominio.Entidades.INSSImpostoContratoFrete>, Dominio.Interfaces.Repositorios.INSSImpostoContratoFrete
    {
        public INSSImpostoContratoFrete(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.INSSImpostoContratoFrete> BuscarPorImposto(int codigoImposto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.INSSImpostoContratoFrete>();

            var result = from obj in query where obj.Imposto.Codigo == codigoImposto select obj;

            return result.ToList();
        }

        public Dominio.Entidades.INSSImpostoContratoFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.INSSImpostoContratoFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.INSSImpostoContratoFrete BuscarPorImpostoEFaixa(int codigoImposto, decimal valor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.INSSImpostoContratoFrete>();

            var result = from obj in query where obj.Imposto.Codigo == codigoImposto && obj.ValorInicial <= valor && obj.ValorFinal >= valor select obj;

            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.INSSImpostoContratoFrete> BuscarCodigosDiferentes(int imposto, List<int> codigosCadastrados)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.INSSImpostoContratoFrete>();

            var result = from obj in query where obj.Imposto.Codigo == imposto && !codigosCadastrados.Contains(obj.Codigo) select obj;

            return result.ToList();
        }
    }
}
