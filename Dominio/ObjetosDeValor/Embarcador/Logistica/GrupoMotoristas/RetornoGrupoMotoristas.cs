using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas
{
    public class RetornoGrupoMotoristas
    {
        #region Atributos

        public Entidades.Embarcador.Logistica.GrupoMotoristas GrupoMotoristas { get; set; }

        public List<Enumeradores.TipoIntegracao> TiposIntegracao { get; set; }

        public List<RelacionamentoGrupoMotoristas> Funcionarios { get; set; }

        #endregion Atributos

        #region Construtores

        public RetornoGrupoMotoristas(
            Entidades.Embarcador.Logistica.GrupoMotoristas grupoMotoristas,
            List<Entidades.Embarcador.Logistica.GrupoMotoristasFuncionario> grupoMotoristasFuncionarios,
            List<Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao> grupoMotoristasTipoIntegracoes)
        {
            GrupoMotoristas = grupoMotoristas;
            TiposIntegracao = grupoMotoristasTipoIntegracoes.Select(gmti => gmti.TipoIntegracao).ToList();
            Funcionarios = grupoMotoristasFuncionarios.Select(gmf => new RelacionamentoGrupoMotoristas
            {
                CodigoRelacionamento = gmf.Codigo,
                Descricao = gmf.Descricao,
                Codigo = gmf.Funcionario.Codigo,
                CPF = gmf.Funcionario.CPF_CNPJ_Formatado
            }).ToList();
        }

        public RetornoGrupoMotoristas(
            Entidades.Embarcador.Logistica.GrupoMotoristas grupoMotoristas,
            List<RelacionamentoGrupoMotoristas> funcionarios,
            List<Enumeradores.TipoIntegracao> grupoMotoristasTipoIntegracoes)
        {
            GrupoMotoristas = grupoMotoristas;
            Funcionarios = funcionarios;
            TiposIntegracao = grupoMotoristasTipoIntegracoes;
        }
        #endregion Construtores
    }
}
