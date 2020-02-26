using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Example.Controls
{
    /// <summary>
    /// Wrapper around CollectionView items to control their select/deselect appearance
    /// </summary>
    public abstract class CollectionSelectableItem<TViewModel> : ReactiveContentView<TViewModel>
        where TViewModel : class
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(CollectionSelectableItem<TViewModel>));

        // This property is set from visual state setters and it should be public
        // ReSharper disable once MemberCanBeProtected.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        protected CollectionSelectableItem()
        {
            /* the code below is equivalent to:
        <Style x:Key="TabSelector" TargetType="profile:BaseTab">
            <Setter Property="VisualStateManager.VisualStateGroups">
                <VisualStateGroupList>
                    <VisualStateGroup x:Name="CommonStates">
                        <VisualState x:Name="Normal" />
                        <VisualState x:Name="Selected">
                            <VisualState.Setters>
                                <Setter Property="IsSelected" Value="true" />
                            </VisualState.Setters>
                        </VisualState>
                    </VisualStateGroup>
                </VisualStateGroupList>
            </Setter>
        </Style>
             *
             * If we use style that targets BaseTab without key, then this wouldn't work (style doesn't work on base classes, they applies to exact classes)
             * If we use style with key, then we'll have to use this style on every subclass and not forget about it */
            
            var visualStateGroupList = new VisualStateGroupList();

            var visualStateGroup = new VisualStateGroup
            {
                Name = "Common",
                States =
                {
                    new VisualState { Name = "Normal" },
                    new VisualState
                    {
                        Name = "Selected",
                        Setters =
                        {
                            new Setter
                            {
                                Property = IsSelectedProperty,
                                Value = true
                            }
                        }
                    }
                }
            };

            visualStateGroupList.Add(visualStateGroup);
            
            VisualStateManager.SetVisualStateGroups(this, visualStateGroupList);
        }
    }
}