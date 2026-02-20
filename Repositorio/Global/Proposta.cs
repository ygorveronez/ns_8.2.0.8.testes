using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class Proposta : RepositorioBase<Dominio.Entidades.Proposta>
    {
        public Proposta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Proposta BuscaPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Proposta>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public int ObterUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Proposta>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            int? retorno = result.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value : 0;
        }

        public List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoProposta> RelatorioPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Proposta>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.VisualizacaoProposta()
            {
                Data = o.Data,
                ClienteCPFCNPJ = o.Cliente != null ? o.Cliente.CPF_CNPJ.ToString() : string.Empty,
                ClienteNome = o.Cliente != null ? o.Cliente.NomeFantasia : string.Empty,
                Email = o.Email != null ? o.Email : string.Empty,
                Telefone = o.Telefone != null ? o.Telefone : string.Empty,
                Nome = o.Nome,
                TipoColeta = o.TipoColeta != null ? o.TipoColeta.Descricao : string.Empty,
                ModalProposta = o.ModalProposta,
                Peso = o.Peso,
                TipoVeiculo = o.TipoVeiculo,
                TipoCarga = o.TipoCarga != null ? o.TipoCarga.Descricao : string.Empty,
                Volumes = o.Volumes,
                Dimensoes = o.Dimensoes,
                TipoCarroceria = o.TipoCarroceria,
                Rastreador = o.Rastreador == Dominio.Enumeradores.OpcaoSimNao.Sim ? "Sim" : "Não",
                Origem = o.Origem != null ? o.Origem.Descricao + " - " + o.Origem.Estado.Sigla : string.Empty,
                Destino = o.Origem != null ? o.Destino.Descricao + " - " + o.Destino.Estado.Sigla : string.Empty,
                ClienteOrigem = o.ClienteOrigem != null ? o.ClienteOrigem.NomeFantasia : string.Empty,
                ClienteDestino = o.ClienteDestino != null ? o.ClienteDestino.NomeFantasia : string.Empty,
                Observacoes = o.Observacoes,
                ValorMercadoria = o.ValorMercadoria,
                UnidadeMonetaria = o.UnidadeMonetaria != null ? o.UnidadeMonetaria : string.Empty
            }).ToList();
        }
        public List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoTextosPropostas> TextoRelatorioPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Proposta>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.VisualizacaoTextosPropostas()
            {
                DiasValidade = o.DiasValidade != null ? (int)o.DiasValidade : 30,
                TextoCustosAdicionais = o.TextoCustosAdicionais,
                TextoFormaCobranca = o.TextoFormaCobranca,
                TextoCTRN = o.TextoCTRN,
            }).ToList();
        }

        private IQueryable<Dominio.Entidades.Proposta> _Consultar(int codigoEmpresa, DateTime dataLancamento, double cpfcnpjCliente, string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Proposta>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataLancamento != DateTime.MinValue)
                result = result.Where(o => o.DataLancamento >= dataLancamento && o.DataLancamento < dataLancamento.AddDays(1));

            if(cpfcnpjCliente > 0)
                result = result.Where(o => o.Cliente.CPF_CNPJ == cpfcnpjCliente);

            if(!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Equals(nome));

            return result;
        }

        public List<Dominio.Entidades.Proposta> Consultar(int codigoEmpresa, DateTime dataLancamento, double cpfcnpjCliente, string nome, int inicioRegistro, int maximoRegistro)
        {
            var result = this._Consultar(codigoEmpresa, dataLancamento, cpfcnpjCliente, nome);

            if (maximoRegistro > 0 && inicioRegistro > 0)
                result = result.Skip(inicioRegistro).Take(maximoRegistro);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataLancamento, double cpfcnpjCliente, string nome)
        {
            var result = this._Consultar(codigoEmpresa, dataLancamento, cpfcnpjCliente, nome);

            return result.Count();
        }
    }
}
