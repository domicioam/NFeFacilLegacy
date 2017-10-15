﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewDadosBase
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class AdicionarClienteBrasileiro : Page
    {
        BaseAdicaoDestinatario Base { get; set; }

        public AdicionarClienteBrasileiro()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Base = new BaseAdicaoDestinatario(e);
        }

        void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            Base.Confirmar();
        }

        void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Base.Cancelar();
        }
    }
}
