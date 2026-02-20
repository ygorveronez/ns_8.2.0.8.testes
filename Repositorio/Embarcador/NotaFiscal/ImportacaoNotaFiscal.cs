using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ImportacaoNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal>
    {
        #region Construtores

        public ImportacaoNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal BuscarProximaImportacaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal>();
            query = from obj in query where obj.Situacao == SituacaoImportacaoNotaFiscal.Pendente || obj.Situacao == SituacaoImportacaoNotaFiscal.Processando select obj;
            return query.OrderBy(obj => obj.DataImportacao).FirstOrDefault();
        }
        
        public bool VerificarExistenciaPorNomeArquivo(string nomeArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal>()
                .Where(obj => obj.Planilha == nomeArquivo);
            
            return query.Count() > 0;
        }

        #endregion
    }
}
