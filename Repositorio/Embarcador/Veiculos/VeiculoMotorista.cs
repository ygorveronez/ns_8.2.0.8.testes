using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>
    {
        public VeiculoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public VeiculoMotorista(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> BuscarTodos(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.ToList();
        }

        public bool ContemMotoristaPrincipal(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo && obj.Principal select obj;
            return result.Any();
        }

        public bool EMotoristaPrincipal(int codigoVeiculo, int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo && obj.Motorista.Codigo == codigoMotorista && obj.Principal select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista BuscarVeiculoMotoristaPrincipal(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo && obj.Principal select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> BuscarVeiculoMotoristasSecundarios(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo && !obj.Principal select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> BuscarMotoristaPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Usuario BuscarMotoristaPrincipal(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo && obj.Principal select obj;
            return result.Select(o => o.Motorista).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Usuario> BuscarMotoristaPrincipalAsync(int codigoVeiculo, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo && obj.Principal select obj;
            return result.Select(o => o.Motorista).FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> BuscarMotoristaPrincipal(List<int> codigosVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = query
                .Where(obj => codigosVeiculo.Contains(obj.Veiculo.Codigo) && obj.Principal)
                .Fetch(obj => obj.Veiculo)
                .Fetch(obj => obj.Motorista).ToList();
            return result;
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristasSecundarios(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo && obj.Motorista != null && !obj.Principal select obj;
            return result.Select(o => o.Motorista).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> BuscarVeiculosMotoristaPorMotorista(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Motorista.Codigo == codigoMotorista select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Veiculo BuscarPrimeiroVeiculoComMotorista(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where codigos.Contains(obj.Veiculo.Codigo) select obj.Veiculo;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarVeiculoPorCPFMotorista(string cpfMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Motorista.CPF == cpfMotorista select obj.Veiculo;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarVeiculoPorMotorista(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Motorista.Codigo == codigoMotorista select obj.Veiculo;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarVeiculoTracaoPorMotorista(int codigoMotorista)
        {
            //Prioriza um veiculo do tipo 0 = tracao 
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var result = from obj in query where obj.Motorista.Codigo == codigoMotorista orderby obj.Veiculo.TipoVeiculo select obj.Veiculo;
            return result.FirstOrDefault();
        }

        public List<RelacaoMotoristaVeiculo> ObterRelacaoMotoristaVeiculo(List<int> codigosVeiculos)
        {
            if (codigosVeiculos.IsNullOrEmpty())
                return new List<RelacaoMotoristaVeiculo>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            return query.Where(x => x.Principal == true)
                .Where(x => x.Veiculo != null)
                .Where(x => x.Motorista != null)
                .Where(x => codigosVeiculos.Contains(x.Veiculo.Codigo))
                .Select(x => new RelacaoMotoristaVeiculo
                {
                    CodigoMotorista = x.Codigo,
                    CodigoVeiculo = x.Veiculo.Codigo,
                    CPF = x.CPF,
                    Nome = x.Nome,
                    Celular = x.Motorista.Celular,
                    Telefone = x.Motorista.Telefone,
                    DataValidadeGrMotorista = x.Motorista.DataValidadeGR,
                    DataValidadeGrVeiculo = x.Veiculo.DataValidadeGerenciadoraRisco
                }).ToList();
        }

        public void DeletarMotoristaVeiculo(int codigoVeiculo, int codigoMotorista)
        {

            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoMotorista m WHERE m.Motorista.Codigo = :codigoMotorista and m.Veiculo.Codigo = :codigoVeiculo").SetInt32("codigoMotorista", codigoMotorista).SetInt32("codigoVeiculo", codigoVeiculo).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoMotorista m WHERE m.Motorista.Codigo = :codigoMotorista and m.Veiculo.Codigo = :codigoVeiculo").SetInt32("codigoMotorista", codigoMotorista).SetInt32("codigoVeiculo", codigoVeiculo).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public void DeletarMotoristaPrincipal(int codigoVeiculo)
        {

            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoMotorista m WHERE m.Principal = 1 and m.Veiculo.Codigo = :codigoVeiculo").SetInt32("codigoVeiculo", codigoVeiculo).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoMotorista m WHERE m.Principal = 1 and m.Veiculo.Codigo = :codigoVeiculo").SetInt32("codigoVeiculo", codigoVeiculo).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> BuscarVeiculosMotoristaPorMotoristaETipoTracao(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>()
                .Where(o => o.Motorista.Codigo == codigoMotorista && o.Veiculo.TipoVeiculo == "0");

            return query.ToList();
        }

        #endregion
    }
    public class RelacaoMotoristaVeiculo
    {
        public int CodigoMotorista { get; set; }
        public int CodigoVeiculo { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Celular { get; set; }
        public string Telefone { get; set; }

        public DateTime? DataValidadeGrMotorista { get; set; }
        public DateTime? DataValidadeGrVeiculo { get; set; }

        public string GR_MotoristaValida()
        {
            if (DataValidadeGrMotorista == null || !DataValidadeGrMotorista.HasValue || DataValidadeGrMotorista.Value == DateTime.MinValue)
                return "Sem Cadastro";

            if (DataValidadeGrMotorista.Value.Date >= DateTime.Now.Date)
                return "Sim";

            return "Não";
        }

        public string GR_VeiculoValida()
        {
            if (DataValidadeGrVeiculo == null || !DataValidadeGrVeiculo.HasValue || DataValidadeGrVeiculo.Value == DateTime.MinValue)
                return "Sem Cadastro";

            if (DataValidadeGrVeiculo.Value.Date >= DateTime.Now.Date)
                return "Sim";

            return "Não";
        }

        public string ObterTelefoneFormatado()
        {
            if (!string.IsNullOrEmpty(Celular))
                return Celular.ObterTelefoneFormatado();

            if (!string.IsNullOrEmpty(Telefone))
                return Telefone.ObterTelefoneFormatado();

            return string.Empty;
        }
    }
}
