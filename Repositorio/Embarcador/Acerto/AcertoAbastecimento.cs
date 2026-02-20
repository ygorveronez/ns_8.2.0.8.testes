using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>
    {
        public AcertoAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemAbastecimentoLancadoVeiculo(int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.Abastecimento.Veiculo != null && obj.Abastecimento.Veiculo.Codigo == codigoVeiculo && obj.Abastecimento.TipoAbastecimento == tipoAbastecimento select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> BuscarPorCodigoAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo select obj;
            return result.ToList();
        }

        public decimal BuscarValorMoedaestrangeira(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigo);

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && (obj.Abastecimento.MoedaCotacaoBancoCentral == null || moedas.Contains(obj.Abastecimento.MoedaCotacaoBancoCentral.Value)));
            else
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && obj.Abastecimento.MoedaCotacaoBancoCentral != null && moedas.Contains(obj.Abastecimento.MoedaCotacaoBancoCentral.Value));

            var queryModalidadeFornecedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var resultModalidade = from obj in queryModalidadeFornecedor where obj.PagoPorFatura == false select obj;
            query = query.Where(a => resultModalidade.Any(c => c.ModalidadePessoas.Cliente == a.Abastecimento.Posto));

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                return query.Sum(o => (decimal?)o.Abastecimento.ValorUnitario  * (decimal?)o.Abastecimento.Litros) ?? 0m;
            else
                return query.Sum(o => (decimal?)o.Abastecimento.ValorOriginalMoedaEstrangeira) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> BuscarPorVeiculoCodigoAcerto(int codigo, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo && obj.Abastecimento.Veiculo != null && obj.Abastecimento.Veiculo.Codigo == codigoVeiculo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> BuscarPorCodigoAcertoVeiculoTipo(int codigo, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo && obj.Abastecimento.Veiculo != null && obj.Abastecimento.Veiculo.Codigo == codigoVeiculo && obj.Abastecimento.TipoAbastecimento == tipoAbastecimento select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> BuscarPorCodigoAcertoVeiculoTipoParaVerificarPendencias(int codigo, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && obj.Abastecimento.Veiculo != null && obj.Abastecimento.Veiculo.Codigo == codigoVeiculo && obj.Abastecimento.TipoAbastecimento == tipoAbastecimento && obj.Abastecimento.Posto != null && obj.Abastecimento.Posto.CPF_CNPJ != double.Parse(obj.AcertoViagem.Motorista.CPF));

            return query.Fetch(o => o.Abastecimento).ThenFetch(o => o.Veiculo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> BuscarPorCodigoAcertoTipo(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo && obj.Abastecimento.TipoAbastecimento == tipoAbastecimento select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> BuscarPorVeiculoAcerto(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Abastecimento.Veiculo != null && obj.Abastecimento.Veiculo.Codigo == codigoVeiculo && obj.Abastecimento.TipoAbastecimento == tipoAbastecimento select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarBuscarPorVeiculoAcerto(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Abastecimento.Veiculo != null && obj.Abastecimento.Veiculo.Codigo == codigoVeiculo && obj.Abastecimento.TipoAbastecimento == tipoAbastecimento select obj;
            return result.Count();
        }

        public int HorimetroInicialAbastecimentos(int codigoAcerto, Dominio.Entidades.Veiculo veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Horimetro > 0 && o.Abastecimento.Veiculo != null && o.Abastecimento.Veiculo.Codigo == veiculo.Codigo && o.Abastecimento.TipoAbastecimento == tipoAbastecimento);

            int ultimoHorimetroAbastecimento = query.Min(o => (int?)o.Abastecimento.Horimetro) ?? 0;

            if (ultimoHorimetroAbastecimento > 0m)
            {
                var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

                queryAbastecimento = queryAbastecimento.Where(o => o.Horimetro > 0 && o.Veiculo != null && o.Veiculo.Codigo == veiculo.Codigo && o.Horimetro < ultimoHorimetroAbastecimento);

                int ultimoKM = queryAbastecimento.Max(o => (int?)o.Horimetro) ?? 0;
                if (ultimoKM > 0)
                    ultimoHorimetroAbastecimento = ultimoKM;
                else if (veiculo.Equipamentos != null && veiculo.Equipamentos.Count > 0)
                {
                    int equipamentoHorimetro = veiculo.Equipamentos.Max(c => c.Horimetro);

                    if (equipamentoHorimetro > 0 && equipamentoHorimetro < ultimoHorimetroAbastecimento)
                        ultimoHorimetroAbastecimento = equipamentoHorimetro;
                }
            }

            if (ultimoHorimetroAbastecimento <= 0m && veiculo.Equipamentos != null && veiculo.Equipamentos.Count > 0)
                ultimoHorimetroAbastecimento = veiculo.Equipamentos.Max(c => c.Horimetro);

            return ultimoHorimetroAbastecimento;
        }

        public int HorimetroFinalAbastecimentos(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.Veiculo.Codigo == codigoVeiculo && o.Abastecimento.TipoAbastecimento == tipoAbastecimento);

            return query.Max(o => (int?)o.Abastecimento.Horimetro) ?? 0;
        }

        public decimal KMInicialAbastecimentos(int codigoAcerto, Dominio.Entidades.Veiculo veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.Abastecimento.Kilometragem > 0 && o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.Veiculo.Codigo == veiculo.Codigo && o.Abastecimento.TipoAbastecimento == tipoAbastecimento);

            decimal ultimoKMAbastecimento = query.Min(o => (decimal?)o.Abastecimento.Kilometragem) ?? 0m;

            if (ultimoKMAbastecimento > 0m)
            {
                var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

                queryAbastecimento = queryAbastecimento.Where(o => o.Kilometragem > 0 && o.Veiculo != null && o.Veiculo.Codigo == veiculo.Codigo && o.Kilometragem < ultimoKMAbastecimento && o.TipoAbastecimento == tipoAbastecimento);

                decimal ultimoKM = queryAbastecimento.Max(o => (decimal?)o.Kilometragem) ?? 0m;
                if (ultimoKM > 0)
                    ultimoKMAbastecimento = ultimoKM;
            }

            if (ultimoKMAbastecimento <= 0m)            
                ultimoKMAbastecimento = veiculo.KilometragemAtual;            

            return ultimoKMAbastecimento;
        }

        public decimal KMFinalAbastecimentos(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.Veiculo.Codigo == codigoVeiculo && o.Abastecimento.TipoAbastecimento == tipoAbastecimento);

            return query.Max(o => (decimal?)o.Abastecimento.Kilometragem) ?? 0m;
        }

        public decimal QuantidadeLitrosAbastecimentos(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.Abastecimento.Kilometragem > 0 && o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.Veiculo.Codigo == codigoVeiculo && o.Abastecimento.TipoAbastecimento == tipoAbastecimento);

            return query.Sum(o => (decimal?)o.Abastecimento.Litros) ?? 0m;
        }

        public decimal QuantidadeLitrosAbastecimentosEquipamento(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.Abastecimento.Horimetro > 0 && o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.Veiculo.Codigo == codigoVeiculo && o.Abastecimento.TipoAbastecimento == tipoAbastecimento);

            return query.Sum(o => (decimal?)o.Abastecimento.Litros) ?? 0m;
        }

        public decimal QuantidadeLitrosAbastecimentos(int codigoAcerto, string tipoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.Veiculo.TipoVeiculo == tipoVeiculo);

            return query.Sum(o => (decimal?)o.Abastecimento.Litros) ?? 0m;
        }

        public decimal QuantidadeLitrosAbastecimentos(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, string tipoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.TipoAbastecimento == tipoAbastecimento && o.Abastecimento.Veiculo.TipoVeiculo == tipoVeiculo);

            return query.Sum(o => (decimal?)o.Abastecimento.Litros) ?? 0m;
        }

        public decimal ValorUnitarioAbastecimentos(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.Veiculo.Codigo == codigoVeiculo && o.Abastecimento.TipoAbastecimento == tipoAbastecimento);

            return query.Average(o => (decimal?)o.Abastecimento.ValorUnitario) ?? 0m;
        }

        public decimal MediaValorUnitarioAbastecimentos(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, string tipoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.TipoAbastecimento == tipoAbastecimento && o.Abastecimento.Veiculo.TipoVeiculo == tipoVeiculo);

            return query.Average(o => (decimal?)o.Abastecimento.ValorUnitario) ?? 0m;
        }

        public decimal ValorTotalAbastecimentos(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.Abastecimento.Kilometragem > 0 && o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.Veiculo.Codigo == codigoVeiculo && o.Abastecimento.TipoAbastecimento == tipoAbastecimento);

            return query.Sum(o => (decimal?)(o.Abastecimento.Litros * o.Abastecimento.ValorUnitario)) ?? 0m;
        }

        public decimal ValorTotalAbastecimentosEquipamento(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.Abastecimento.Horimetro > 0 && o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.Veiculo != null && o.Abastecimento.Veiculo.Codigo == codigoVeiculo && o.Abastecimento.TipoAbastecimento == tipoAbastecimento);

            return query.Sum(o => (decimal?)(o.Abastecimento.Litros * o.Abastecimento.ValorUnitario)) ?? 0m;
        }


        public decimal ValorTotalAbastecimentos(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.Abastecimento.TipoAbastecimento == tipoAbastecimento);

            return query.Sum(o => (decimal?)(o.Abastecimento.Litros * o.Abastecimento.ValorUnitario)) ?? 0m;

            //decimal valorTotal = 0;

            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            //var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Abastecimento.TipoAbastecimento == tipoAbastecimento select obj;
            //if (result.Count() > 0)
            //{
            //    List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> listaAbastecimento = result.ToList();
            //    for (int i = 0; i < listaAbastecimento.Count(); i++)
            //        valorTotal += listaAbastecimento[i].Abastecimento.Litros * listaAbastecimento[i].Abastecimento.ValorUnitario;
            //    return valorTotal;
            //}
            //else
            //    return 0;
        }

        public decimal ReceitaDespesaAbastecimento(int codigoAcerto, bool pagoPorFatura)
        {
            //decimal valorTotal = 0;

            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var queryModalidadeFornecedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var resultModalidade = from obj in queryModalidadeFornecedor where obj.PagoPorFatura == pagoPorFatura select obj;

            var resultAbastecimento = resultModalidade.Join(queryAbastecimento, vei => vei.ModalidadePessoas.Cliente.CPF_CNPJ, emp => emp.Abastecimento.Posto.CPF_CNPJ, (vei, emp) => emp);

            resultAbastecimento = from obj in resultAbastecimento where obj.AcertoViagem.Codigo == codigoAcerto select obj;

            return resultAbastecimento.Sum(o => (decimal?)(o.Abastecimento.Litros * o.Abastecimento.ValorUnitario)) ?? 0m;

            //if (resultAbastecimento.Count() > 0)
            //{
            //    List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> listaAbastecimento = resultAbastecimento.ToList();
            //    for (int i = 0; i < listaAbastecimento.Count(); i++)
            //        valorTotal += listaAbastecimento[i].Abastecimento.Litros * listaAbastecimento[i].Abastecimento.ValorUnitario;
            //    return valorTotal;
            //}
            //else
            //    return 0;
        }

        public List<Dominio.Entidades.Abastecimento> BuscarAbastecimentosPorCodigoAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo select obj.Abastecimento;
            return result.ToList();
        }

        public bool ContemAbastecimentoEmAcerto(int codigoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.Abastecimento.Codigo == codigoAbastecimento && obj.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Abastecimento> ConsultarAbastecimentosDoAcertoViagem(decimal kilometragem, int horimetro, DateTime data, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Abastecimento> resultAbastecimento = ConsultarAbastecimentosDoAcertoViagem(kilometragem, horimetro, data, acertoViagem, codigoVeiculo, tipoAbastecimento);

            if (inicioRegistros > 0 || maximoRegistros > 0)
                return resultAbastecimento.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Timeout(5000).ToList();
            else
                return resultAbastecimento.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Timeout(5000).ToList();
        }

        public int ContarConsultarAbastecimentosDoAcertoViagem(decimal kilometragem, int horimetro, DateTime data, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            IQueryable<Dominio.Entidades.Abastecimento> resultAbastecimento = ConsultarAbastecimentosDoAcertoViagem(kilometragem, horimetro, data, acertoViagem, codigoVeiculo, tipoAbastecimento);

            return resultAbastecimento.Timeout(5000).Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Abastecimento> ConsultarAbastecimentosDoAcertoViagem(decimal kilometragem, int horimetro, DateTime data, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var resultAbastecimento = from obj in queryAbastecimento select obj;

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var resultAcerto = from obj in queryAcerto where obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento || obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado select obj;

            if (data > DateTime.MinValue)
                resultAbastecimento = resultAbastecimento.Where(obj => obj.Data.Value >= data && obj.Data.Value <= data);

            if (kilometragem > 0)
                resultAbastecimento = resultAbastecimento.Where(obj => obj.Kilometragem == kilometragem);

            if (horimetro > 0)
                resultAbastecimento = resultAbastecimento.Where(obj => obj.Horimetro == horimetro);

            //if (acertoViagem.DataFinal.HasValue && acertoViagem.DataFinal != null && acertoViagem.DataFinal.Value > DateTime.MinValue)
            //    resultAbastecimento = resultAbastecimento.Where(obj => obj.Data.Value <= acertoViagem.DataFinal.Value);

            resultAbastecimento = resultAbastecimento.Where(obj => obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo);
            resultAbastecimento = resultAbastecimento.Where(obj => obj.TipoAbastecimento == tipoAbastecimento && obj.Situacao != "G");

            resultAbastecimento = resultAbastecimento.Where(obj => !(from p in resultAcerto select p.Abastecimento).Contains(obj));

            return resultAbastecimento;
        }

        #endregion
    }
}
