using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class MDFeContratante : RepositorioBase<Dominio.Entidades.MDFeContratante>, Dominio.Interfaces.Repositorios.MDFeContratante
    {
        public MDFeContratante(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MDFeContratante(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.MDFeContratante BuscarPorCodigo(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeContratante>();
            var result = from obj in query where obj.Codigo == codigo && obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.ContratantesMDFe> BuscarPorMDFeRelacionado(int codigoMDFe)
        {
            string query = @"SELECT mc.MCO_CODIGO Id, c.CLI_NOME Nome, mc.MCO_CONTRATANTE CPF_CNPJ FROM T_MDFE_CONTRATANTE mc
                            LEFT JOIN T_CLIENTE c
                            ON mc.MCO_CONTRATANTE = c.CLI_CGCCPF
                            WHERE mc.MDF_CODIGO = " + codigoMDFe.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.ContratantesMDFe)));

            return nhQuery.List<Dominio.ObjetosDeValor.ContratantesMDFe>();
        }

        public List<Dominio.Entidades.MDFeContratante> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeContratante>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.MDFeContratante> BuscarPorMDFeCnpj(int codigoMDFe, string cnpjContratante)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeContratante>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Contratante == cnpjContratante.ObterSomenteNumeros() select obj;
            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.ContratantesMDFeGroupByCnpjNome> BuscarPorMDFeGroupByCnpjNome(int codigoMDFe)
        {
            string query = $@"SELECT MCO_CONTRATANTE as CnpjContratante,MCO_NOME_CONTRATANTE as NomeContratante FROM T_MDFE_CONTRATANTE
                            WHERE MDF_CODIGO = {codigoMDFe} GROUP BY MCO_CONTRATANTE,MCO_NOME_CONTRATANTE";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.ContratantesMDFeGroupByCnpjNome)));
            return nhQuery.List<Dominio.ObjetosDeValor.ContratantesMDFeGroupByCnpjNome>().ToList();
        }

        public Task<List<Dominio.Entidades.MDFeContratante>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeContratante>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            return result.ToListAsync(cancellationToken);
        }        
    }
}
