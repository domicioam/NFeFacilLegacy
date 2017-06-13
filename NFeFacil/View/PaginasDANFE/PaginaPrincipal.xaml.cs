﻿using NFeFacil.DANFE;
using NFeFacil.DANFE.Pacotes;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NFeFacil.View.PaginasDANFE
{
    public sealed partial class PaginaPrincipal : UserControl
    {
        double LarguraPagina => PartesDANFE.DimensoesPadrao.CentimeterToPixel(21);
        double AlturaPagina => PartesDANFE.DimensoesPadrao.CentimeterToPixel(29.7);
        Thickness MargemPadrao => new Thickness(PartesDANFE.DimensoesPadrao.CentimeterToPixel(1));

        DadosCabecalho ContextoCanhoto { get; }
        DadosAdicionais ContextoDadosAdicionais { get; }
        DadosCliente ContextoCliente { get; }
        DadosImposto ContextoImposto { get; }
        DadosMotorista ContextoTransporte { get; }
        DadosNFe ContextoNFe { get; }
        DadosISSQN ContextoISSQN { get; }
        Geral ContextoGeral { get; }

        UIElementCollection PaiPaginas { get; }

        public PaginaPrincipal(BibliotecaCentral.ModeloXML.Processo processo, UIElementCollection paiPaginas)
        {
            this.InitializeComponent();
            var geral = new ViewDados(processo).ObterDadosConvertidos();
            ContextoCanhoto = geral._DadosCabecalho;
            ContextoDadosAdicionais = geral._DadosAdicionais;
            ContextoCliente = geral._DadosCliente;
            ContextoImposto = geral._DadosImposto;
            ContextoTransporte = geral._DadosMotorista;
            ContextoNFe = geral._DadosNFe;
            ContextoISSQN = geral._DadosISSQN;
            ContextoGeral = geral;

            PaiPaginas = paiPaginas;
        }

        private void CampoProdutos_Loaded(object sender, RoutedEventArgs e)
        {
            double total = 0, maximo = espacoParaProdutos.ActualHeight;
            var produtosNestaPagina = ContextoGeral._DadosProdutos.TakeWhile(x =>
            {
                var item = new PartesDANFE.ItemProduto() { DataContext = x };
                item.Measure(new Windows.Foundation.Size(PartesDANFE.DimensoesPadrao.LarguraTotalStatic, espacoParaProdutos.ActualHeight));
                total += item.DesiredSize.Height;
                return total <= maximo;
            });
            ((FrameworkElement)sender).DataContext = produtosNestaPagina.ToArray();

            bool excessoProdutos = ContextoGeral._DadosProdutos.Length - produtosNestaPagina.Count() > 0;
            bool excessoObservacao = infoAdicional.CampoObservacoes.HasOverflowContent;

            if (excessoProdutos)
            {
                var produtosRestantes = ContextoGeral._DadosProdutos.Except(produtosNestaPagina);
                if (excessoObservacao)
                {
                    PaiPaginas.Add(new PaginaExtra(produtosRestantes, infoAdicional.CampoObservacoes, PaiPaginas, MotivoCriacaoPaginaExtra.Ambos));
                }
                else
                {
                    PaiPaginas.Add(new PaginaExtra(produtosRestantes, infoAdicional.CampoObservacoes, PaiPaginas, MotivoCriacaoPaginaExtra.Produtos));
                }
            }
            else if (excessoObservacao)
            {
                PaiPaginas.Add(new PaginaExtra(null, infoAdicional.CampoObservacoes, PaiPaginas, MotivoCriacaoPaginaExtra.Observacao));
            }
        }
    }
}
