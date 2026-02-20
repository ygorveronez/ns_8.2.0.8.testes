using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ArquivoImportacaoNotaFiscalCampo : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo>
    {
        public ArquivoImportacaoNotaFiscalCampo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo> BuscarPorArquivo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo>();

            query = query.Where(o => o.Arquivo.Codigo == codigo);

            return query.ToList();
        }
    }
}
