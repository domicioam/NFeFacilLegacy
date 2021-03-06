﻿using Windows.UI.Xaml.Controls;
using Venda.Impostos.DetalhamentoIPI;

// O modelo de item de Caixa de Diálogo de Conteúdo está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace Venda.GerenciamentoProdutos
{
    public sealed partial class CadastroIPI : ContentDialog
    {
        public ImpSimplesArmazenado.XMLIPIArmazenado Dados
        {
            get
            {
                var page = (DetalharSimples)ctnPrincipal.Content;
                var ipi = page.Conjunto;
                return new ImpSimplesArmazenado.XMLIPIArmazenado
                {
                    CEnq = ipi.CodigoEnquadramento,
                    CNPJProd = ipi.CNPJProd,
                    CSelo = ipi.CodigoSelo,
                    QSelo = ipi.QuantidadeSelos
                };
            }
        }

        public CadastroIPI(Detalhamento detalhamento)
        {
            InitializeComponent();
            ctnPrincipal.Content = new DetalharSimples(detalhamento);
        }
    }
}
