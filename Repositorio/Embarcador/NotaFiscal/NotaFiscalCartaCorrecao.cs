using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalCartaCorrecao : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao>
    {
        private CancellationToken _cancellationToken;
        public NotaFiscalCartaCorrecao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public NotaFiscalCartaCorrecao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { this._cancellationToken = cancellationToken; }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao BuscarUltimaCCe(int codigoNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigoNFe select obj;
            result = result.OrderByDescending(obj => obj.Codigo);
            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao> BuscarUltimaCCeAsync(int codigoNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigoNFe select obj;
            result = result.OrderByDescending(obj => obj.Codigo);
            return await result.FirstOrDefaultAsync(_cancellationToken);
        }

        public int BuscarQuantidadeCartaCorrecao(int codigoNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigoNFe && obj.Status == Dominio.Enumeradores.StatusNFe.Autorizado select obj;
            if (result.Count() > 0)
                return result.Count() + 1;
            else
                return 1;
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao BuscarPorNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.CCeNFe> BuscarCCeNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao carta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectCCeNFE(false, propriedades, carta, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite, isRelatorio));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.CCeNFe)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.NFe.CCeNFe>();
        }

        private string ObterSelectCCeNFE(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao carta, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio)
        {
            string select = string.Empty;

            select = "SELECT N.NFI_CODIGO CodigoNota, " +
                    " E.EMP_RAZAO NomeEmitente, " +
                    " E.EMP_CNPJ CNPJEmitente, " +
                    " E.EMP_FONE FoneEmitente, " +
                    " ISNULL(E.EMP_TIPO, 'J') TipoEmitente, " +
                    " N.NFI_AMBIENTE TipoAmbiente, " +
                    " L.CLI_NOME NomeDestinatario, " +
                    " L.CLI_CGCCPF CNPJDestinatario, " +
                    " L.CLI_FISJUR TipoDestinatario, " +
                    " N.NFI_NUMERO NumeroNota, " +
                    " S.ESE_NUMERO SerieNota, " +
                    " C.NCC_NUMERO_LOTE NumeroLote, " +
                    " N.NFI_CHAVE ChaveNota, " +
                    " C.NCC_PROTOCOLO NumeroProtocolo, " +
                    " C.NCC_DATA_PROCESSAMENTO DataProcessamento, " +
                    " C.NCC_MENSAGEM Motivo " +
                    " FROM T_NOTA_FISCAL_CARTA_CORRECAO C " +
                    " JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = C.NFI_CODIGO " +
                    " JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO " +
                    " JOIN T_CLIENTE L ON L.CLI_CGCCPF = N.CLI_CGCCPF " +
                    " JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO " +
                    " WHERE C.NCC_CODIGO= " + carta.Codigo;
            return select;
        }

    }
}
