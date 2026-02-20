using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class ModeloDocumentoFiscal : RepositorioBase<Dominio.Entidades.ModeloDocumentoFiscal>, Dominio.Interfaces.Repositorios.ModeloDocumentoFiscal
    {
        public ModeloDocumentoFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ModeloDocumentoFiscal(UnitOfWork unitOfWork, CancellationToken cancellation) : base(unitOfWork, cancellation) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.ModeloDocumentoFiscal> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();
            var result = from obj in query orderby obj.Numero ascending select obj;
            return result.ToList();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();

            int? retorno = query.Max(o => (int?)o.Codigo);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        public Dominio.Entidades.ModeloDocumentoFiscal BuscarPorId(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.ModeloDocumentoFiscal> BuscarPorIdAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.FirstOrDefaultAsync();
        }
        public Dominio.Entidades.ModeloDocumentoFiscal BuscarPorModelo(string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();
            var result = from obj in query where obj.Numero.Equals(numero) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ModeloDocumentoFiscal BuscarPorModeloCRT()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();
            var result = from obj in query where obj.DocumentoTipoCRT select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ModeloDocumentoFiscal BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();
            var result = from obj in query where obj.TipoDocumentoEmissao == tipoDocumento select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.ModeloDocumentoFiscal> BuscarPorTipoDocumentoAsync(Dominio.Enumeradores.TipoDocumento tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();
            var result = from obj in query where obj.TipoDocumentoEmissao == tipoDocumento select obj;
            return result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.ModeloDocumentoFiscal BuscarPorTipoDocumentoDebitoCredito(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();
            var result = from obj in query where obj.TipoDocumentoCreditoDebito == tipoDocumento select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ModeloDocumentoFiscal> Consultar(string descricao, string numero, bool? apenasModelosEditaveis, bool? incluirNFSe, bool? incluirNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            List<Dominio.Enumeradores.TipoDocumento> tiposDocumentos = new List<Dominio.Enumeradores.TipoDocumento>();

            if (incluirNFSe == true)
                tiposDocumentos.Add(Dominio.Enumeradores.TipoDocumento.NFSe);

            if (incluirNFSManual == true)
                tiposDocumentos.Add(Dominio.Enumeradores.TipoDocumento.NFS);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(numero))
                query = query.Where(o => o.Numero.Contains(numero));

            //if (apenasModelosEditaveis.HasValue)
            //    query = query.Where(o => o.Editavel == apenasModelosEditaveis.Value);

            if (apenasModelosEditaveis == true && tiposDocumentos.Count > 0)
                query = query.Where(o => o.Editavel == true || tiposDocumentos.Contains(o.TipoDocumentoEmissao));
            else if (apenasModelosEditaveis == true)
                query = query.Where(o => o.Editavel == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Status.Equals("A"));
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => o.Status.Equals("I"));

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, string numero, bool? apenasModelosEditaveis, bool? incluirNFSe, bool? incluirNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            List<Dominio.Enumeradores.TipoDocumento> tiposDocumentos = new List<Dominio.Enumeradores.TipoDocumento>();

            if (incluirNFSe == true)
                tiposDocumentos.Add(Dominio.Enumeradores.TipoDocumento.NFSe);

            if (incluirNFSManual == true)
                tiposDocumentos.Add(Dominio.Enumeradores.TipoDocumento.NFS);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(numero))
                query = query.Where(o => o.Numero.Contains(numero));

            //if (apenasModelosEditaveis.HasValue)
            //    query = query.Where(o => o.Editavel == apenasModelosEditaveis.Value);

            if (apenasModelosEditaveis == true && tiposDocumentos.Count > 0)
                query = query.Where(o => o.Editavel == true || tiposDocumentos.Contains(o.TipoDocumentoEmissao));
            else if (apenasModelosEditaveis == true)
                query = query.Where(o => o.Editavel == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Status.Equals("A"));
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => o.Status.Equals("I"));

            return query.Count();
        }

        public List<Dominio.Entidades.ModeloDocumentoFiscal> Consultar(string descricao, string numero, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(obj => obj.Numero.Contains(numero));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(obj => obj.Numero.Contains(numero));

            return result.Count();
        }

        public List<Dominio.Entidades.ModeloDocumentoFiscal> BuscarPorCodigo(int[] codigosModelos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();

            query = query.Where(o => codigosModelos.Contains(o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.ModeloDocumentoFiscal> BuscarPorCodigos(List<int> codigosModelos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();

            query = query.Where(o => codigosModelos.Contains(o.Codigo));

            return query.ToList();
        }

        public List<string> BuscarDescricaoPorCodigo(List<int> modeloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();

            query = query.Where(o => modeloDocumento.Contains(o.Codigo));

            return query.Select(o => o.Descricao).ToList();
        }
        public async Task<IList<string>> BuscarDescricaoPorCodigoAsync(List<int> modeloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();

            query = query.Where(o => modeloDocumento.Contains(o.Codigo));

            return await query.Select(o => o.Descricao).ToListAsync();
        }

        public Dominio.Entidades.ModeloDocumentoFiscal BuscarPorAbreviacao(string abreviacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloDocumentoFiscal>();
            var result = from obj in query where obj.Abreviacao.Equals(abreviacao) select obj;
            return result.FirstOrDefault();
        }

        #endregion
    }
}
