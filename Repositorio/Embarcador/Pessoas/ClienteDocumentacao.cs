using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteDocumentacao : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao>
    {
        public ClienteDocumentacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao BuscarPorCodigoEPessoa(int codigo, double cpfcnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao>();
            var result = from obj in query where obj.Codigo == codigo && obj.Cliente.CPF_CNPJ == cpfcnpj select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao> Consultar(double cliente, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(double cliente, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao> BuscarPorDocumentoNaoMantidosPorPessoa(List<int> documentosMantidos, double cnpjcpfCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDocumentacao>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cnpjcpfCliente && !documentosMantidos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public static implicit operator ClienteDocumentacao(ClienteOutroEndereco v)
        {
            throw new NotImplementedException();
        }
    }
}
