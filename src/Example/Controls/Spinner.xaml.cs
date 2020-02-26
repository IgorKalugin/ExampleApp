using System;
using ReactiveUI;
using Xamarin.Forms;

namespace Example.Controls
{
    public partial class Spinner
    {
        // ReSharper disable MemberCanBePrivate.Global
        public static readonly BindableProperty IsSpinningProperty = 
            BindableProperty.Create(nameof(IsSpinning), typeof(bool), typeof(Spinner));

        public bool IsSpinning
        {
            get => (bool)GetValue(IsSpinningProperty);
            set => SetValue(IsSpinningProperty, value);
        }
        // ReSharper restore MemberCanBePrivate.Global

        public Spinner()
        {
            InitializeComponent();
        
            var spinnerAnimation = new Animation(v => spinner.Rotation = v, 0, 360);

            this.WhenAnyValue(view => view.IsSpinning)
                .Subscribe(_ =>
                {
                    spinner.IsVisible = IsSpinning;
                    if (IsSpinning)
                    {
                        spinnerAnimation.Commit(this, "SpinnerAnimation", length: 500, repeat: () => IsSpinning);
                    }
                });
        }
    }
}